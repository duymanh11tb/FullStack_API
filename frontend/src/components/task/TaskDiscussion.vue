<template>
  <div class="task-discussion">
    <!-- Comment List -->
    <div class="comment-list" ref="commentListRef">
      <LoadingSpinner v-if="loading" text="Đang tải bình luận..." />
      
      <div v-else-if="comments.length > 0" class="comments-wrapper">
        <div v-for="comment in comments" :key="comment.id" class="comment-item">
          <div class="comment-avatar">
            {{ comment.userFullName?.charAt(0)?.toUpperCase() || 'U' }}
          </div>
          <div class="comment-content">
            <div class="comment-header">
              <span class="comment-author">{{ comment.userFullName }}</span>
              <span class="comment-time">{{ formatTime(comment.createdAt) }}</span>
            </div>
            <div class="comment-body" :class="{ 'deleted': comment.isDeleted }">
              {{ comment.content }}
            </div>
            <!-- Delete Button (only if author, but for now just show if we have user id logic) -->
            <button
              v-if="!comment.isDeleted && authStore.userId === comment.userId"
              class="btn-delete"
              @click="handleDelete(comment.id)"
            >
              Xóa
            </button>

            <!-- Nested Replies (if backend supports it, we render recursively or just 1 level) -->
            <div v-if="comment.replies && comment.replies.length > 0" class="replies">
              <div v-for="reply in comment.replies" :key="reply.id" class="comment-item reply">
                <div class="comment-avatar avatar-sm">
                  {{ reply.userFullName?.charAt(0)?.toUpperCase() || 'U' }}
                </div>
                <div class="comment-content">
                  <div class="comment-header">
                    <span class="comment-author">{{ reply.userFullName }}</span>
                    <span class="comment-time">{{ formatTime(reply.createdAt) }}</span>
                  </div>
                  <div class="comment-body" :class="{ 'deleted': reply.isDeleted }">
                    {{ reply.content }}
                  </div>
                  <button
                    v-if="!reply.isDeleted && authStore.userId === reply.userId"
                    class="btn-delete"
                    @click="handleDelete(reply.id)"
                  >
                    Xóa
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <div v-else class="empty-comments">
        <svg width="48" height="48" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5">
          <path d="M21 15a2 2 0 0 1-2 2H7l-4 4V5a2 2 0 0 1 2-2h14a2 2 0 0 1 2 2z" />
        </svg>
        <p>Chưa có thảo luận nào. Hãy là người đầu tiên bình luận!</p>
      </div>
    </div>

    <!-- Input Box -->
    <div class="comment-input-area">
      <!-- Mentions Dropdown -->
      <div v-if="showMentionsDropdown && filteredMembers.length > 0" class="mentions-dropdown">
        <div
          v-for="(member, index) in filteredMembers"
          :key="member.userId || member.email"
          :class="['mention-item', { active: index === mentionSelectedIndex }]"
          @click="selectMention(member)"
        >
          <div class="mention-avatar" :style="{ background: getAvatarColor(member.displayName) }">
            {{ member.displayName.charAt(0).toUpperCase() }}
          </div>
          <div class="mention-info">
            <span class="mention-name">{{ member.displayName }}</span>
            <span class="mention-username">@{{ getMemberUsername(member) }}</span>
          </div>
        </div>
      </div>

      <textarea
        v-model="newComment"
        placeholder="Viết bình luận..."
        rows="3"
        class="comment-input"
        @keydown="handleKeyDown"
        @input="handleInput"
        @click="showMentionsDropdown = false"
        ref="textareaRef"
      ></textarea>
      <div class="input-actions">
        <span class="help-text">Nhấn Enter để gửi, Shift + Enter để xuống dòng</span>
        <BaseButton
          variant="primary"
          :disabled="!newComment.trim() || submitting"
          @click="submitComment"
        >
          Gửi bình luận
        </BaseButton>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, nextTick, computed } from 'vue'
import { getCommentsByTask, createComment, deleteComment } from '../../api/notifyApi'
import { taskApi } from '../../api/taskApi'
import { getMembers } from '../../api/projectApi'
import { useAuthStore } from '../../stores/auth'
import LoadingSpinner from '../common/LoadingSpinner.vue'
import BaseButton from '../common/BaseButton.vue'

const props = defineProps({
  taskId: {
    type: String,
    required: true
  }
})

const authStore = useAuthStore()
const loading = ref(true)
const submitting = ref(false)
const comments = ref([])
const newComment = ref('')
const commentListRef = ref(null)

// Mentions Autocomplete State
const textareaRef = ref(null)
const showMentionsDropdown = ref(false)
const mentionSearchQuery = ref('')
const mentionSelectedIndex = ref(0)
const mentionTriggerIndex = ref(-1)
const members = ref([])

const defaultSeedUsers = [
  { displayName: 'admin', email: 'admin@example.com', userId: 'seed-admin' },
  { displayName: 'duymanh', email: 'duymanh@example.com', userId: 'seed-duymanh' },
  { displayName: 'tranailinh', email: 'tranailinh@example.com', userId: 'seed-tranailinh' }
]

const avatarColors = ['#2563EB', '#7C3AED', '#DC2626', '#D97706', '#16A34A', '#0891B2', '#DB2777', '#4F46E5']

function getAvatarColor(name) {
  if (!name) return avatarColors[0]
  let hash = 0
  for (let i = 0; i < name.length; i++) hash = name.charCodeAt(i) + ((hash << 5) - hash)
  return avatarColors[Math.abs(hash) % avatarColors.length]
}

function getMemberUsername(member) {
  if (member.username) return member.username
  if (member.email) return member.email.split('@')[0]
  return member.displayName.toLowerCase().replace(/\s+/g, '')
}

const filteredMembers = computed(() => {
  const query = mentionSearchQuery.value.toLowerCase()
  if (!query) return members.value
  
  return members.value.filter(member => {
    const username = getMemberUsername(member).toLowerCase()
    const name = member.displayName.toLowerCase()
    return username.includes(query) || name.includes(query)
  })
})

async function fetchProjectMembers() {
  try {
    const taskRes = await taskApi.getTask(props.taskId)
    const taskData = taskRes.data?.data || taskRes.data
    const boardId = taskData?.boardId || taskData?.boardID || taskData?.projectId
    
    if (boardId) {
      const membersRes = await getMembers(boardId)
      const projectMembers = membersRes.data?.data || membersRes.data || []
      
      const combined = [...projectMembers]
      defaultSeedUsers.forEach(seed => {
        if (!combined.some(m => m.email?.toLowerCase() === seed.email.toLowerCase())) {
          combined.push(seed)
        }
      })
      members.value = combined
    } else {
      members.value = defaultSeedUsers
    }
  } catch (err) {
    console.error('Lỗi khi tải thành viên dự án:', err)
    members.value = defaultSeedUsers
  }
}

function handleInput(event) {
  const textarea = event.target
  const value = textarea.value
  const selectionStart = textarea.selectionStart

  const textBeforeCursor = value.slice(0, selectionStart)
  const lastAtSymbol = textBeforeCursor.lastIndexOf('@')

  if (lastAtSymbol !== -1) {
    const isFirstCharOrPrecededByWhitespace = lastAtSymbol === 0 || /\s/.test(textBeforeCursor[lastAtSymbol - 1])
    const searchString = textBeforeCursor.slice(lastAtSymbol + 1)
    const hasSpaceAfterAt = /\s/.test(searchString)
    
    if (isFirstCharOrPrecededByWhitespace && !hasSpaceAfterAt) {
      showMentionsDropdown.value = true
      mentionSearchQuery.value = searchString
      mentionTriggerIndex.value = lastAtSymbol
      mentionSelectedIndex.value = 0
      return
    }
  }
  
  showMentionsDropdown.value = false
}

function handleKeyDown(event) {
  if (showMentionsDropdown.value) {
    if (event.key === 'ArrowDown') {
      event.preventDefault()
      mentionSelectedIndex.value = (mentionSelectedIndex.value + 1) % filteredMembers.value.length
      return
    } else if (event.key === 'ArrowUp') {
      event.preventDefault()
      mentionSelectedIndex.value = (mentionSelectedIndex.value - 1 + filteredMembers.value.length) % filteredMembers.value.length
      return
    } else if (event.key === 'Enter' || event.key === 'Tab') {
      event.preventDefault()
      if (filteredMembers.value[mentionSelectedIndex.value]) {
        selectMention(filteredMembers.value[mentionSelectedIndex.value])
      }
      return
    } else if (event.key === 'Escape') {
      event.preventDefault()
      showMentionsDropdown.value = false
      return
    }
  }

  // Handle enter key to submit comment when not using dropdown
  if (event.key === 'Enter' && !event.shiftKey) {
    event.preventDefault()
    submitComment()
  }
}

function selectMention(member) {
  if (!member) return
  
  const username = getMemberUsername(member)
  const textarea = textareaRef.value
  if (!textarea) return
  
  const beforeText = newComment.value.slice(0, mentionTriggerIndex.value)
  const afterText = newComment.value.slice(textarea.selectionStart)
  
  newComment.value = beforeText + '@' + username + ' ' + afterText
  showMentionsDropdown.value = false
  
  nextTick(() => {
    textarea.focus()
    const newCursorPos = mentionTriggerIndex.value + username.length + 2
    textarea.setSelectionRange(newCursorPos, newCursorPos)
  })
}

function formatTime(dateStr) {
  if (!dateStr) return ''
  const date = new Date(dateStr)
  return new Intl.DateTimeFormat('vi-VN', {
    day: '2-digit', month: '2-digit', year: 'numeric',
    hour: '2-digit', minute: '2-digit'
  }).format(date)
}

function scrollToBottom() {
  nextTick(() => {
    if (commentListRef.value) {
      commentListRef.value.scrollTop = commentListRef.value.scrollHeight
    }
  })
}

async function loadComments() {
  loading.value = true
  try {
    const res = await getCommentsByTask(props.taskId)
    comments.value = res.data || []
    scrollToBottom()
  } catch (err) {
    console.error('Lỗi khi tải bình luận', err)
  } finally {
    loading.value = false
  }
}

async function submitComment() {
  if (!newComment.value.trim() || submitting.value) return

  submitting.value = true
  try {
    await createComment({
      taskId: props.taskId,
      content: newComment.value.trim()
    })
    newComment.value = ''
    await loadComments()
  } catch (err) {
    alert(err.response?.data?.message || 'Không thể gửi bình luận')
  } finally {
    submitting.value = false
  }
}

async function handleDelete(id) {
  if (!confirm('Xóa bình luận này?')) return
  try {
    await deleteComment(id)
    await loadComments()
  } catch (err) {
    alert(err.response?.data?.message || 'Không thể xóa bình luận')
  }
}

onMounted(() => {
  loadComments()
  fetchProjectMembers()
})
</script>

<style scoped>
.task-discussion {
  display: flex;
  flex-direction: column;
  height: 100%;
  min-height: 400px;
  max-height: 600px;
  border: 1px solid var(--color-border);
  border-radius: var(--radius-lg);
  background: var(--color-bg);
  overflow: hidden;
}

.comment-list {
  flex: 1;
  overflow-y: auto;
  padding: var(--space-4);
  background: var(--color-bg-secondary);
}

.comments-wrapper {
  display: flex;
  flex-direction: column;
  gap: var(--space-4);
}

.comment-item {
  display: flex;
  gap: var(--space-3);
}

.comment-item.reply {
  margin-top: var(--space-3);
  padding-left: var(--space-4);
  border-left: 2px solid var(--color-border);
}

.comment-avatar {
  width: 36px;
  height: 36px;
  border-radius: var(--radius-full);
  background: var(--color-primary);
  color: white;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: var(--font-weight-bold);
  font-size: var(--font-size-sm);
  flex-shrink: 0;
}

.avatar-sm {
  width: 28px;
  height: 28px;
  font-size: var(--font-size-xs);
}

.comment-content {
  flex: 1;
  background: var(--color-white);
  padding: var(--space-3) var(--space-4);
  border-radius: var(--radius-lg);
  border: 1px solid var(--color-border);
  box-shadow: var(--shadow-sm);
}

.comment-header {
  display: flex;
  justify-content: space-between;
  align-items: baseline;
  margin-bottom: var(--space-1);
}

.comment-author {
  font-weight: var(--font-weight-semibold);
  font-size: var(--font-size-sm);
  color: var(--color-text-primary);
}

.comment-time {
  font-size: var(--font-size-xs);
  color: var(--color-text-tertiary);
}

.comment-body {
  font-size: var(--font-size-sm);
  color: var(--color-text-secondary);
  line-height: var(--line-height-relaxed);
  white-space: pre-wrap;
}

.comment-body.deleted {
  font-style: italic;
  color: var(--color-text-tertiary);
}

.btn-delete {
  background: none;
  border: none;
  color: var(--color-danger);
  font-size: var(--font-size-xs);
  cursor: pointer;
  padding: 0;
  margin-top: var(--space-2);
}

.btn-delete:hover {
  text-decoration: underline;
}

.empty-comments {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  height: 100%;
  color: var(--color-text-tertiary);
}

.empty-comments svg {
  margin-bottom: var(--space-2);
  opacity: 0.5;
}

.comment-input-area {
  padding: var(--space-4);
  background: var(--color-white);
  border-top: 1px solid var(--color-border);
  position: relative;
}

.comment-input {
  width: 100%;
  border: 1px solid var(--color-border);
  border-radius: var(--radius-md);
  padding: var(--space-3);
  font-size: var(--font-size-sm);
  font-family: inherit;
  resize: none;
  transition: border-color var(--transition-fast);
}

.comment-input:focus {
  outline: none;
  border-color: var(--color-primary);
}

.input-actions {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-top: var(--space-2);
}

.help-text {
  font-size: var(--font-size-xs);
  color: var(--color-text-tertiary);
}

/* Mentions Dropdown Styling */
.mentions-dropdown {
  position: absolute;
  bottom: calc(100% - 4px);
  left: var(--space-4);
  width: calc(100% - (var(--space-4) * 2));
  max-width: 320px;
  background: var(--color-white);
  border: 1px solid var(--color-border);
  border-radius: var(--radius-lg);
  box-shadow: var(--shadow-lg);
  max-height: 200px;
  overflow-y: auto;
  z-index: 100;
  display: flex;
  flex-direction: column;
  padding: 4px;
}

.mention-item {
  display: flex;
  align-items: center;
  gap: var(--space-3);
  padding: var(--space-2) var(--space-3);
  border-radius: var(--radius-md);
  cursor: pointer;
  transition: all var(--transition-fast);
}

.mention-item:hover, .mention-item.active {
  background: var(--color-primary-light);
}

.mention-avatar {
  width: 28px;
  height: 28px;
  border-radius: 50%;
  color: white;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: bold;
  font-size: 12px;
  flex-shrink: 0;
}

.mention-info {
  display: flex;
  flex-direction: column;
  min-width: 0;
}

.mention-name {
  font-size: 13px;
  font-weight: 600;
  color: var(--color-text-primary);
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.mention-username {
  font-size: 11px;
  color: var(--color-text-tertiary);
}
</style>
