<template>
  <div class="task-discussion">
    <!-- Comment List -->
    <div class="comment-list" ref="commentListRef">
      <LoadingSpinner v-if="loading" text="Đang tải bình luận..." />
      
      <div v-else-if="comments.length > 0" class="comments-wrapper">
        <div v-for="comment in comments" :key="comment.id" class="comment-item">
          <!-- Timeline point dot -->
          <div class="timeline-dot"></div>
          
          <div class="comment-avatar" :style="{ background: getAvatarColor(comment.userFullName) }">
            {{ comment.userFullName?.charAt(0)?.toUpperCase() || 'U' }}
          </div>
          <div class="comment-content">
            <div class="comment-header">
              <span class="comment-author">{{ comment.userFullName }}</span>
              <span class="comment-time">
                <svg width="11" height="11" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" style="margin-right: 4px; vertical-align: middle;">
                  <circle cx="12" cy="12" r="10"/><polyline points="12 6 12 12 16 14"/>
                </svg>
                {{ formatTime(comment.createdAt) }}
              </span>
            </div>
            
            <!-- Edit Root Comment -->
            <div v-if="editingCommentId === comment.id" class="edit-comment-area animate-fade">
              <textarea v-model="editingContent" class="comment-input edit-textarea" rows="2"></textarea>
              <div class="edit-actions">
                <BaseButton size="sm" variant="secondary" @click="cancelEdit">Hủy</BaseButton>
                <BaseButton size="sm" variant="primary" :disabled="!editingContent.trim()" @click="saveEdit(comment.id)">Lưu</BaseButton>
              </div>
            </div>
            <div v-else class="comment-body" :class="{ 'deleted': comment.isDeleted }">
              {{ comment.content }}
              <span v-if="comment.updatedAt && !comment.isDeleted" class="edited-flag">(Đã chỉnh sửa)</span>
            </div>

            <!-- Root Comment Actions -->
            <div v-if="!comment.isDeleted" class="comment-actions">
              <button 
                v-if="canManageComment(comment)" 
                class="btn-action-icon" 
                @click="startEdit(comment)"
                title="Chỉnh sửa bình luận"
              >
                <svg width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
                  <path d="M12 20h9M16.5 3.5a2.121 2.121 0 0 1 3 3L7 19l-4 1 1-4L16.5 3.5z" />
                </svg>
                Sửa
              </button>
              <button 
                v-if="canManageComment(comment)" 
                class="btn-action-icon danger" 
                @click="handleDelete(comment.id)"
                title="Xóa bình luận"
              >
                <svg width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
                  <polyline points="3 6 5 6 21 6"/><path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"/>
                </svg>
                Xóa
              </button>
              <button 
                class="btn-action-icon reply" 
                @click="startReply(comment.id)"
                title="Trả lời bình luận"
              >
                <svg width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
                  <polyline points="9 17 4 12 9 7"/><path d="M20 18v-2a4 4 0 0 0-4-4H4"/>
                </svg>
                Trả lời
              </button>
            </div>

            <!-- Reply Input Box -->
            <div v-if="replyingToId === comment.id" class="reply-input-area animate-fade">
              <textarea 
                v-model="replyContent" 
                placeholder="Trả lời bình luận..." 
                class="comment-input reply-textarea" 
                rows="2"
              ></textarea>
              <div class="reply-actions">
                <BaseButton size="sm" variant="secondary" @click="cancelReply">Hủy</BaseButton>
                <BaseButton size="sm" variant="primary" :disabled="!replyContent.trim() || submitting" @click="submitReply(comment.id)">Gửi</BaseButton>
              </div>
            </div>

            <!-- Nested Replies -->
            <div v-if="comment.replies && comment.replies.length > 0" class="replies">
              <div v-for="reply in comment.replies" :key="reply.id" class="comment-item reply">
                <div class="comment-avatar avatar-sm" :style="{ background: getAvatarColor(reply.userFullName) }">
                  {{ reply.userFullName?.charAt(0)?.toUpperCase() || 'U' }}
                </div>
                <div class="comment-content reply-content">
                  <div class="comment-header">
                    <span class="comment-author reply-author">{{ reply.userFullName }}</span>
                    <span class="comment-time">
                      <svg width="10" height="10" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" style="margin-right: 3px; vertical-align: middle;">
                        <circle cx="12" cy="12" r="10"/><polyline points="12 6 12 12 16 14"/>
                      </svg>
                      {{ formatTime(reply.createdAt) }}
                    </span>
                  </div>
                  
                  <!-- Edit Reply Comment -->
                  <div v-if="editingCommentId === reply.id" class="edit-comment-area animate-fade">
                    <textarea v-model="editingContent" class="comment-input edit-textarea" rows="2"></textarea>
                    <div class="edit-actions">
                      <BaseButton size="sm" variant="secondary" @click="cancelEdit">Hủy</BaseButton>
                      <BaseButton size="sm" variant="primary" :disabled="!editingContent.trim()" @click="saveEdit(reply.id)">Lưu</BaseButton>
                    </div>
                  </div>
                  <div v-else class="comment-body text-sm" :class="{ 'deleted': reply.isDeleted }">
                    {{ reply.content }}
                    <span v-if="reply.updatedAt && !reply.isDeleted" class="edited-flag">(Đã chỉnh sửa)</span>
                  </div>

                  <!-- Reply Actions -->
                  <div v-if="!reply.isDeleted" class="comment-actions">
                    <button 
                      v-if="canManageComment(reply)" 
                      class="btn-action-icon" 
                      @click="startEdit(reply)"
                    >
                      <svg width="11" height="11" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
                        <path d="M12 20h9M16.5 3.5a2.121 2.121 0 0 1 3 3L7 19l-4 1 1-4L16.5 3.5z" />
                      </svg>
                      Sửa
                    </button>
                    <button 
                      v-if="canManageComment(reply)" 
                      class="btn-action-icon danger" 
                      @click="handleDelete(reply.id)"
                    >
                      <svg width="11" height="11" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
                        <polyline points="3 6 5 6 21 6"/><path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"/>
                      </svg>
                      Xóa
                    </button>
                    <button 
                      class="btn-action-icon reply" 
                      @click="startReply(comment.id)"
                    >
                      <svg width="11" height="11" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
                        <polyline points="9 17 4 12 9 7"/><path d="M20 18v-2a4 4 0 0 0-4-4H4"/>
                      </svg>
                      Trả lời
                    </button>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <div v-else class="empty-comments animate-fade">
        <div class="empty-comment-icon">
          <svg width="36" height="36" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
            <path d="M21 15a2 2 0 0 1-2 2H7l-4 4V5a2 2 0 0 1 2-2h14a2 2 0 0 1 2 2z" />
          </svg>
        </div>
        <p class="empty-text-main">Chưa có cuộc thảo luận nào</p>
        <p class="empty-text-sub">Hãy là người đầu tiên bình luận và tag đồng nghiệp bằng @</p>
      </div>
    </div>

    <!-- Input Box -->
    <div class="comment-input-area">
      <!-- Mentions Dropdown -->
      <transition name="fade">
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
      </transition>

      <div class="input-wrapper">
        <textarea
          v-model="newComment"
          placeholder="Viết bình luận hoặc tag thành viên bằng @..."
          rows="3"
          class="comment-input main-input"
          @keydown="handleKeyDown"
          @input="handleInput"
          @click="showMentionsDropdown = false"
          ref="textareaRef"
        ></textarea>
        <div class="input-actions">
          <span class="help-text">
            💡 <strong>Shift + Enter</strong> để xuống dòng. Nhấn <strong>Enter</strong> để gửi nhanh.
          </span>
          <BaseButton
            variant="primary"
            :disabled="!newComment.trim() || submitting"
            @click="submitComment"
            class="send-btn"
          >
            <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" style="margin-right: 6px;">
              <line x1="22" y1="2" x2="11" y2="13"/><polyline points="22 2 15 22 11 13 2 9 22 2"/>
            </svg>
            Gửi bình luận
          </BaseButton>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, nextTick, computed } from 'vue'
import { getCommentsByTask, createComment, deleteComment, updateComment } from '../../api/notifyApi'
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

// Edit Comment state
const editingCommentId = ref(null)
const editingContent = ref('')

// Reply state
const replyingToId = ref(null)
const replyContent = ref('')

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
    if (editingCommentId.value === id) cancelEdit()
    if (replyingToId.value === id) cancelReply()
    await loadComments()
  } catch (err) {
    alert(err.response?.data?.message || 'Không thể xóa bình luận')
  }
}

// Edit actions
function startEdit(comment) {
  editingCommentId.value = comment.id
  editingContent.value = comment.content
}

function cancelEdit() {
  editingCommentId.value = null
  editingContent.value = ''
}

async function saveEdit(id) {
  if (!editingContent.value.trim()) return
  try {
    await updateComment(id, { content: editingContent.value.trim() })
    cancelEdit()
    await loadComments()
  } catch (err) {
    alert(err.response?.data?.message || 'Không thể cập nhật bình luận')
  }
}

// Reply actions
function startReply(parentId) {
  replyingToId.value = parentId
  replyContent.value = ''
}

function cancelReply() {
  replyingToId.value = null
  replyContent.value = ''
}

async function submitReply(parentId) {
  if (!replyContent.value.trim() || submitting.value) return
  submitting.value = true
  try {
    await createComment({
      taskId: props.taskId,
      content: replyContent.value.trim(),
      parentCommentId: parentId
    })
    cancelReply()
    await loadComments()
  } catch (err) {
    alert(err.response?.data?.message || 'Không thể gửi phản hồi')
  } finally {
    submitting.value = false
  }
}

// Permission check helpers
const currentUserRole = computed(() => {
  if (!authStore.user?.id) return null
  const member = members.value.find(m => m.userId === authStore.user.id || m.id === authStore.user.id)
  return member ? member.role : null
})

function canManageComment(comment) {
  if (comment.isDeleted) return false
  
  // 1. Author check
  const isAuthor = authStore.user?.id === comment.userId
  if (isAuthor) return true
  
  // 2. Admin check
  if (authStore.isAdmin) return true
  
  // 3. Project Manager/Owner check
  const role = currentUserRole.value
  const isProjectManager = role === 0 || role === 1 || role === 'Owner' || role === 'Manager' || role === 'Admin'
  
  return isProjectManager
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
  min-height: 460px;
  max-height: 620px;
  border: 1px solid var(--border-color);
  border-radius: var(--radius-xl);
  background: var(--bg-card);
  backdrop-filter: var(--glass-blur);
  -webkit-backdrop-filter: var(--glass-blur);
  box-shadow: var(--shadow-glass);
  overflow: hidden;
  position: relative;
}

.comment-list {
  flex: 1;
  overflow-y: auto;
  padding: var(--space-6);
  background: transparent;
  position: relative;
}

/* Glass timeline line */
.comments-wrapper {
  display: flex;
  flex-direction: column;
  gap: 24px;
  position: relative;
}

.comments-wrapper::before {
  content: '';
  position: absolute;
  top: 16px;
  bottom: 16px;
  left: 19px;
  width: 2px;
  background: var(--border-color);
  z-index: 0;
  opacity: 0.7;
}

.comment-item {
  display: flex;
  gap: var(--space-4);
  position: relative;
  z-index: 2;
}

.timeline-dot {
  position: absolute;
  left: 17px;
  top: 14px;
  width: 6px;
  height: 6px;
  border-radius: var(--radius-full);
  background: var(--color-primary);
  box-shadow: 0 0 6px var(--color-primary);
  z-index: 3;
}

.comment-avatar {
  width: 38px;
  height: 38px;
  border-radius: var(--radius-full);
  color: white;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: var(--font-weight-extrabold);
  font-size: var(--font-size-sm);
  flex-shrink: 0;
  border: 2.5px solid var(--bg-white-to-card);
  box-shadow: var(--shadow-sm);
  z-index: 2;
  transition: transform var(--transition-fast);
}

.comment-item:hover .comment-avatar {
  transform: scale(1.06);
}

.avatar-sm {
  width: 30px;
  height: 30px;
  font-size: var(--font-size-xs);
  border-width: 2px;
}

.comment-content {
  flex: 1;
  background: var(--glass-bg);
  padding: var(--space-4) var(--space-5);
  border-radius: var(--radius-lg);
  border: 1px solid var(--glass-border);
  box-shadow: var(--shadow-xs);
  transition: all var(--transition-base);
}

.comment-item:hover .comment-content {
  background: var(--bg-white-to-card);
  border-color: var(--color-border-hover);
  box-shadow: var(--shadow-md);
}

.reply-content {
  background: rgba(255, 255, 255, 0.4);
}

[data-theme='dark'] .reply-content {
  background: rgba(15, 23, 42, 0.3);
}

.comment-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 6px;
}

.comment-author {
  font-weight: 700;
  font-size: var(--font-size-sm);
  color: var(--color-text-primary);
}

.reply-author {
  color: var(--color-primary);
}

.comment-time {
  font-size: 11px;
  color: var(--color-text-tertiary);
  display: flex;
  align-items: center;
}

.comment-body {
  font-size: var(--font-size-sm);
  color: var(--color-text-secondary);
  line-height: 1.6;
  white-space: pre-wrap;
}

.text-sm {
  font-size: var(--font-size-xs);
}

.comment-body.deleted {
  font-style: italic;
  color: var(--color-text-tertiary);
  opacity: 0.8;
}

.comment-actions {
  display: flex;
  gap: 16px;
  margin-top: 10px;
  opacity: 0;
  transition: opacity var(--transition-fast);
}

.comment-content:hover .comment-actions {
  opacity: 1;
}

.btn-action-icon {
  display: inline-flex;
  align-items: center;
  gap: 4px;
  background: none;
  border: none;
  color: var(--color-text-tertiary);
  font-size: 11px;
  font-weight: 700;
  cursor: pointer;
  padding: 4px 6px;
  border-radius: var(--radius-sm);
  transition: all var(--transition-fast);
}

.btn-action-icon:hover {
  background: var(--color-primary-light);
  color: var(--color-primary);
}

.btn-action-icon.danger:hover {
  background: var(--color-danger-light);
  color: var(--color-danger);
}

.btn-action-icon.reply:hover {
  background: var(--color-success-light);
  color: var(--color-success);
}

.edited-flag {
  font-size: 10px;
  color: var(--color-text-tertiary);
  margin-left: var(--space-2);
  font-style: italic;
}

.comment-item.reply {
  margin-top: var(--space-3);
  position: relative;
}

.comment-item.reply::before {
  content: '';
  position: absolute;
  left: -20px;
  top: 14px;
  width: 14px;
  height: 2px;
  background: var(--border-color);
}

.replies {
  margin-top: var(--space-3);
  padding-left: var(--space-3);
  border-left: 2px solid var(--border-color);
  display: flex;
  flex-direction: column;
  gap: var(--space-3);
}

.edit-comment-area, .reply-input-area {
  margin-top: var(--space-3);
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.edit-textarea, .reply-textarea {
  min-height: 60px;
  background: var(--color-bg);
}

.edit-actions, .reply-actions {
  display: flex;
  justify-content: flex-end;
  gap: 8px;
}

.empty-comments {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  height: 240px;
  color: var(--color-text-tertiary);
  text-align: center;
}

.empty-comment-icon {
  width: 64px;
  height: 64px;
  border-radius: 50%;
  background: var(--color-bg-secondary);
  display: flex;
  align-items: center;
  justify-content: center;
  margin-bottom: var(--space-4);
  color: var(--text-muted);
}

.empty-text-main {
  font-weight: 700;
  font-size: var(--font-size-base);
  color: var(--color-text-primary);
  margin-bottom: 4px;
}

.empty-text-sub {
  font-size: var(--font-size-xs);
  color: var(--color-text-tertiary);
  max-width: 250px;
}

.comment-input-area {
  padding: var(--space-5) var(--space-6);
  background: var(--bg-white-to-card);
  border-top: 1px solid var(--color-border);
  position: relative;
}

.input-wrapper {
  display: flex;
  flex-direction: column;
  gap: var(--space-3);
}

.comment-input {
  width: 100%;
  border: 1px solid var(--border-color);
  border-radius: var(--radius-md);
  padding: 12px 16px;
  font-size: var(--font-size-sm);
  font-family: inherit;
  resize: none;
  background: var(--color-bg);
  color: var(--text-primary);
  transition: all var(--transition-fast);
}

.comment-input:focus {
  outline: none;
  border-color: var(--color-primary);
  background: var(--bg-white-to-card);
  box-shadow: 0 0 0 3px var(--color-primary-light);
}

.input-actions {
  display: flex;
  justify-content: space-between;
  align-items: center;
  flex-wrap: wrap;
  gap: var(--space-2);
}

.help-text {
  font-size: 11px;
  color: var(--color-text-tertiary);
}

.send-btn {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  font-weight: 700;
}

/* Mentions Dropdown Styling */
.mentions-dropdown {
  position: absolute;
  bottom: calc(100% - 8px);
  left: var(--space-6);
  width: calc(100% - (var(--space-6) * 2));
  max-width: 320px;
  background: var(--glass-bg);
  backdrop-filter: var(--glass-blur);
  -webkit-backdrop-filter: var(--glass-blur);
  border: 1px solid var(--border-color);
  border-radius: var(--radius-lg);
  box-shadow: var(--shadow-glass);
  max-height: 200px;
  overflow-y: auto;
  z-index: 100;
  display: flex;
  flex-direction: column;
  padding: 6px;
}

.mention-item {
  display: flex;
  align-items: center;
  gap: var(--space-3);
  padding: 8px 12px;
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
  box-shadow: var(--shadow-xs);
}

.mention-info {
  display: flex;
  flex-direction: column;
  min-width: 0;
}

.mention-name {
  font-size: 13px;
  font-weight: 700;
  color: var(--color-text-primary);
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.mention-username {
  font-size: 11px;
  color: var(--color-text-tertiary);
}

.animate-fade {
  animation: fadeIn 0.25s ease;
}

@keyframes fadeIn {
  from { opacity: 0; transform: translateY(4px); }
  to { opacity: 1; transform: translateY(0); }
}
</style>
