<template>
  <div class="task-discussion">
    <!-- Comment List Section -->
    <div class="comment-list" ref="commentListRef">
      <LoadingSpinner v-if="loading" text="Đang tải bình luận..." />
      
      <div v-else-if="comments.length > 0" class="comments-wrapper">
        <div v-for="comment in comments" :key="comment.id" class="comment-item animate-fade">
          <!-- Timeline connector line dot -->
          <div class="timeline-dot-wrapper">
            <div class="timeline-dot"></div>
          </div>
          
          <div class="comment-avatar" :style="{ background: getAvatarColor(comment.userFullName) }">
            {{ comment.userFullName?.charAt(0)?.toUpperCase() || 'U' }}
          </div>
          <div class="comment-bubble-wrapper">
            <div class="comment-bubble">
              <div class="comment-header">
                <span class="comment-author">{{ comment.userFullName }}</span>
                <span class="comment-time">
                  <svg width="10" height="10" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" class="time-icon">
                    <circle cx="12" cy="12" r="10"/><polyline points="12 6 12 12 16 14"/>
                  </svg>
                  {{ formatTime(comment.createdAt) }}
                </span>
              </div>
              
              <!-- Edit Area for Root Comment -->
              <div v-if="editingCommentId === comment.id" class="edit-comment-area animate-fade">
                <textarea v-model="editingContent" class="comment-textarea" rows="2"></textarea>
                <div class="edit-actions">
                  <button class="btn-text-cancel" @click="cancelEdit">Hủy</button>
                  <button class="btn-save" :disabled="!editingContent.trim()" @click="saveEdit(comment.id)">Lưu</button>
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
                  class="btn-action" 
                  @click="startEdit(comment)"
                  title="Chỉnh sửa"
                >
                  Sửa
                </button>
                <button 
                  v-if="canManageComment(comment)" 
                  class="btn-action danger" 
                  @click="handleDelete(comment.id)"
                  title="Xóa"
                >
                  Xóa
                </button>
                <button 
                  class="btn-action reply" 
                  @click="startReply(comment.id)"
                  title="Phản hồi"
                >
                  Trả lời
                </button>
              </div>
            </div>

            <!-- Reply Form Trigger Input Box -->
            <div v-if="replyingToId === comment.id" class="reply-input-area animate-fade">
              <textarea 
                v-model="replyContent" 
                placeholder="Phản hồi cuộc thảo luận này..." 
                class="comment-textarea" 
                rows="2"
              ></textarea>
              <div class="reply-actions">
                <button class="btn-text-cancel" @click="cancelReply">Hủy</button>
                <button class="btn-save" :disabled="!replyContent.trim() || submitting" @click="submitReply(comment.id)">Gửi</button>
              </div>
            </div>

            <!-- Nested Replies list -->
            <div v-if="comment.replies && comment.replies.length > 0" class="replies-wrapper">
              <div v-for="reply in comment.replies" :key="reply.id" class="comment-item reply animate-fade">
                <div class="comment-avatar avatar-sm" :style="{ background: getAvatarColor(reply.userFullName) }">
                  {{ reply.userFullName?.charAt(0)?.toUpperCase() || 'U' }}
                </div>
                <div class="comment-bubble reply-bubble">
                  <div class="comment-header">
                    <span class="comment-author reply-author">{{ reply.userFullName }}</span>
                    <span class="comment-time">
                      <svg width="10" height="10" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" class="time-icon">
                        <circle cx="12" cy="12" r="10"/><polyline points="12 6 12 12 16 14"/>
                      </svg>
                      {{ formatTime(reply.createdAt) }}
                    </span>
                  </div>
                  
                  <!-- Edit Area for Reply Comment -->
                  <div v-if="editingCommentId === reply.id" class="edit-comment-area animate-fade">
                    <textarea v-model="editingContent" class="comment-textarea" rows="2"></textarea>
                    <div class="edit-actions">
                      <button class="btn-text-cancel" @click="cancelEdit">Hủy</button>
                      <button class="btn-save" :disabled="!editingContent.trim()" @click="saveEdit(reply.id)">Lưu</button>
                    </div>
                  </div>
                  <div v-else class="comment-body text-sm" :class="{ 'deleted': reply.isDeleted }">
                    {{ reply.content }}
                    <span v-if="reply.updatedAt && !reply.isDeleted" class="edited-flag">(Đã chỉnh sửa)</span>
                  </div>

                  <!-- Reply Comment Actions -->
                  <div v-if="!reply.isDeleted" class="comment-actions">
                    <button 
                      v-if="canManageComment(reply)" 
                      class="btn-action" 
                      @click="startEdit(reply)"
                    >
                      Sửa
                    </button>
                    <button 
                      v-if="canManageComment(reply)" 
                      class="btn-action danger" 
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
      </div>

      <div v-else class="empty-comments animate-fade">
        <div class="empty-comment-icon">
          <svg width="28" height="28" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
            <path d="M21 15a2 2 0 0 1-2 2H7l-4 4V5a2 2 0 0 1 2-2h14a2 2 0 0 1 2 2z" />
          </svg>
        </div>
        <h4 class="empty-text-main">Chưa có thảo luận</h4>
        <p class="empty-text-sub">Hãy gõ bình luận hoặc tag đồng nghiệp bằng ký tự @</p>
      </div>
    </div>

    <!-- Input Section at the Bottom -->
    <div class="comment-input-section">
      <!-- Autocomplete Mentions Dropdown -->
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

      <div class="input-container">
        <textarea
          v-model="newComment"
          placeholder="Nhập nội dung thảo luận... (Gõ @ để tag thành viên)"
          rows="2"
          class="discussion-textarea"
          @keydown="handleKeyDown"
          @input="handleInput"
          @click="showMentionsDropdown = false"
          ref="textareaRef"
        ></textarea>
        
        <div class="input-footer">
          <span class="keyboard-hint">Nhấn <strong>Enter</strong> để gửi nhanh</span>
          <button
            class="btn-send-comment"
            :disabled="!newComment.trim() || submitting"
            @click="submitComment"
          >
            <svg width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="3">
              <line x1="22" y1="2" x2="11" y2="13"/><polyline points="22 2 15 22 11 13 2 9 22 2"/>
            </svg>
            Gửi
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, nextTick, computed, defineProps } from 'vue';
import { getCommentsByTask, createComment, deleteComment, updateComment } from '../../api/notifyApi';
import { taskApi } from '../../api/taskApi';
import { getMembers } from '../../api/projectApi';
import { useAuthStore } from '../../stores/auth';
import LoadingSpinner from '../common/LoadingSpinner.vue';

const props = defineProps({
  taskId: {
    type: String,
    required: true
  }
});

const authStore = useAuthStore();
const loading = ref(true);
const submitting = ref(false);
const comments = ref([]);
const newComment = ref('');
const commentListRef = ref(null);

const editingCommentId = ref(null);
const editingContent = ref('');

const replyingToId = ref(null);
const replyContent = ref('');

const textareaRef = ref(null);
const showMentionsDropdown = ref(false);
const mentionSearchQuery = ref('');
const mentionSelectedIndex = ref(0);
const mentionTriggerIndex = ref(-1);
const members = ref([]);

const defaultSeedUsers = [
  { displayName: 'admin', email: 'admin@example.com', userId: 'seed-admin' },
  { displayName: 'duymanh', email: 'duymanh@example.com', userId: 'seed-duymanh' },
  { displayName: 'tranailinh', email: 'tranailinh@example.com', userId: 'seed-tranailinh' }
];

const avatarColors = ['#2563EB', '#7C3AED', '#DC2626', '#D97706', '#16A34A', '#0891B2', '#DB2777', '#4F46E5'];

function getAvatarColor(name) {
  if (!name) return avatarColors[0];
  let hash = 0;
  for (let i = 0; i < name.length; i++) hash = name.charCodeAt(i) + ((hash << 5) - hash);
  return avatarColors[Math.abs(hash) % avatarColors.length];
}

function getMemberUsername(member) {
  if (member.username) return member.username;
  if (member.email) return member.email.split('@')[0];
  return member.displayName.toLowerCase().replace(/\s+/g, '');
}

const filteredMembers = computed(() => {
  const query = mentionSearchQuery.value.toLowerCase();
  if (!query) return members.value;
  
  return members.value.filter(member => {
    const username = getMemberUsername(member).toLowerCase();
    const name = member.displayName.toLowerCase();
    return username.includes(query) || name.includes(query);
  });
});

async function fetchProjectMembers() {
  try {
    const taskRes = await taskApi.getTask(props.taskId);
    const taskData = taskRes.data?.data || taskRes.data;
    const boardId = taskData?.boardId || taskData?.boardID || taskData?.projectId;
    
    if (boardId) {
      const membersRes = await getMembers(boardId);
      const projectMembers = membersRes.data?.data || membersRes.data || [];
      
      const combined = [...projectMembers];
      defaultSeedUsers.forEach(seed => {
        if (!combined.some(m => m.email?.toLowerCase() === seed.email.toLowerCase())) {
          combined.push(seed);
        }
      });
      members.value = combined;
    } else {
      members.value = defaultSeedUsers;
    }
  } catch (err) {
    console.error('Lỗi khi tải thành viên dự án:', err);
    members.value = defaultSeedUsers;
  }
}

function handleInput(event) {
  const textarea = event.target;
  const value = textarea.value;
  const selectionStart = textarea.selectionStart;

  const textBeforeCursor = value.slice(0, selectionStart);
  const lastAtSymbol = textBeforeCursor.lastIndexOf('@');

  if (lastAtSymbol !== -1) {
    const isFirstCharOrPrecededByWhitespace = lastAtSymbol === 0 || /\s/.test(textBeforeCursor[lastAtSymbol - 1]);
    const searchString = textBeforeCursor.slice(lastAtSymbol + 1);
    const hasSpaceAfterAt = /\s/.test(searchString);
    
    if (isFirstCharOrPrecededByWhitespace && !hasSpaceAfterAt) {
      showMentionsDropdown.value = true;
      mentionSearchQuery.value = searchString;
      mentionTriggerIndex.value = lastAtSymbol;
      mentionSelectedIndex.value = 0;
      return;
    }
  }
  
  showMentionsDropdown.value = false;
}

function handleKeyDown(event) {
  if (showMentionsDropdown.value) {
    if (event.key === 'ArrowDown') {
      event.preventDefault();
      mentionSelectedIndex.value = (mentionSelectedIndex.value + 1) % filteredMembers.value.length;
      return;
    } else if (event.key === 'ArrowUp') {
      event.preventDefault();
      mentionSelectedIndex.value = (mentionSelectedIndex.value - 1 + filteredMembers.value.length) % filteredMembers.value.length;
      return;
    } else if (event.key === 'Enter' || event.key === 'Tab') {
      event.preventDefault();
      if (filteredMembers.value[mentionSelectedIndex.value]) {
        selectMention(filteredMembers.value[mentionSelectedIndex.value]);
      }
      return;
    } else if (event.key === 'Escape') {
      event.preventDefault();
      showMentionsDropdown.value = false;
      return;
    }
  }

  if (event.key === 'Enter' && !event.shiftKey) {
    event.preventDefault();
    submitComment();
  }
}

function selectMention(member) {
  if (!member) return;
  
  const username = getMemberUsername(member);
  const textarea = textareaRef.value;
  if (!textarea) return;
  
  const beforeText = newComment.value.slice(0, mentionTriggerIndex.value);
  const afterText = newComment.value.slice(textarea.selectionStart);
  
  newComment.value = beforeText + '@' + username + ' ' + afterText;
  showMentionsDropdown.value = false;
  
  nextTick(() => {
    textarea.focus();
    const newCursorPos = mentionTriggerIndex.value + username.length + 2;
    textarea.setSelectionRange(newCursorPos, newCursorPos);
  });
}

function formatTime(dateStr) {
  if (!dateStr) return '';
  const date = new Date(dateStr);
  return new Intl.DateTimeFormat('vi-VN', {
    day: '2-digit', month: '2-digit', year: 'numeric',
    hour: '2-digit', minute: '2-digit'
  }).format(date);
}

function scrollToBottom() {
  nextTick(() => {
    if (commentListRef.value) {
      commentListRef.value.scrollTop = commentListRef.value.scrollHeight;
    }
  });
}

async function loadComments() {
  loading.value = true;
  try {
    const res = await getCommentsByTask(props.taskId);
    comments.value = res.data || [];
    scrollToBottom();
  } catch (err) {
    console.error('Lỗi khi tải bình luận', err);
  } finally {
    loading.value = false;
  }
}

async function submitComment() {
  if (!newComment.value.trim() || submitting.value) return;

  submitting.value = true;
  try {
    await createComment({
      taskId: props.taskId,
      content: newComment.value.trim()
    });
    newComment.value = '';
    await loadComments();
  } catch (err) {
    alert(err.response?.data?.message || 'Không thể gửi bình luận');
  } finally {
    submitting.value = false;
  }
}

async function handleDelete(id) {
  if (!confirm('Xóa bình luận này?')) return;
  try {
    await deleteComment(id);
    if (editingCommentId.value === id) cancelEdit();
    if (replyingToId.value === id) cancelReply();
    await loadComments();
  } catch (err) {
    alert(err.response?.data?.message || 'Không thể xóa bình luận');
  }
}

function startEdit(comment) {
  editingCommentId.value = comment.id;
  editingContent.value = comment.content;
}

function cancelEdit() {
  editingCommentId.value = null;
  editingContent.value = '';
}

async function saveEdit(id) {
  if (!editingContent.value.trim()) return;
  try {
    await updateComment(id, { content: editingContent.value.trim() });
    cancelEdit();
    await loadComments();
  } catch (err) {
    alert(err.response?.data?.message || 'Không thể cập nhật bình luận');
  }
}

function startReply(parentId) {
  replyingToId.value = parentId;
  replyContent.value = '';
}

function cancelReply() {
  replyingToId.value = parentId;
  replyContent.value = '';
}

async function submitReply(parentId) {
  if (!replyContent.value.trim() || submitting.value) return;
  submitting.value = true;
  try {
    await createComment({
      taskId: props.taskId,
      content: replyContent.value.trim(),
      parentCommentId: parentId
    });
    cancelReply();
    await loadComments();
  } catch (err) {
    alert(err.response?.data?.message || 'Không thể gửi phản hồi');
  } finally {
    submitting.value = false;
  }
}

const currentUserRole = computed(() => {
  if (!authStore.user?.id) return null;
  const member = members.value.find(m => m.userId === authStore.user.id || m.id === authStore.user.id);
  return member ? member.role : null;
});

function canManageComment(comment) {
  if (comment.isDeleted) return false;
  const isAuthor = authStore.user?.id === comment.userId;
  if (isAuthor) return true;
  if (authStore.isAdmin) return true;
  const role = currentUserRole.value;
  const isProjectManager = role === 0 || role === 1 || role === 'Owner' || role === 'Manager' || role === 'Admin';
  return isProjectManager;
}

onMounted(() => {
  loadComments();
  fetchProjectMembers();
});
</script>

<style scoped>
.task-discussion {
  display: flex;
  flex-direction: column;
  height: 100%;
  max-height: 480px;
  background: transparent;
  overflow: hidden;
  position: relative;
}

.comment-list {
  flex: 1;
  overflow-y: auto;
  padding: 10px 4px var(--space-4) 4px;
}

/* Scrollbar styling */
.comment-list::-webkit-scrollbar {
  width: 4px;
}
.comment-list::-webkit-scrollbar-thumb {
  background: var(--border-color);
  border-radius: 2px;
}

.comments-wrapper {
  display: flex;
  flex-direction: column;
  gap: 16px;
  position: relative;
}

/* Timeline center connector */
.comments-wrapper::before {
  content: '';
  position: absolute;
  top: 10px;
  bottom: 10px;
  left: 17px;
  width: 1px;
  background: var(--border-color);
  z-index: 0;
  opacity: 0.6;
}

.comment-item {
  display: flex;
  gap: 12px;
  position: relative;
  z-index: 2;
}

.timeline-dot-wrapper {
  position: absolute;
  left: 15px;
  top: 12px;
  z-index: 5;
  display: flex;
  align-items: center;
  justify-content: center;
}

.timeline-dot {
  width: 5px;
  height: 5px;
  border-radius: 50%;
  background: var(--color-primary);
  box-shadow: 0 0 4px var(--color-primary);
}

.comment-avatar {
  width: 32px;
  height: 32px;
  border-radius: 50%;
  color: white;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 800;
  font-size: 11px;
  flex-shrink: 0;
  border: 2px solid var(--bg-card);
  box-shadow: var(--shadow-xs);
  z-index: 2;
}

.avatar-sm {
  width: 24px;
  height: 24px;
  font-size: 9px;
  border-width: 1.5px;
}

.comment-bubble-wrapper {
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: 8px;
  min-width: 0;
}

/* Modern Chat Bubble style */
.comment-bubble {
  background: var(--color-bg-secondary);
  border: 1px solid var(--border-color);
  border-radius: 4px var(--radius-lg) var(--radius-lg) var(--radius-lg);
  padding: 10px 14px;
  box-shadow: var(--shadow-xs);
  position: relative;
  transition: all var(--transition-fast);
}

.comment-bubble:hover {
  border-color: var(--color-border-hover);
  box-shadow: var(--shadow-sm);
}

.reply-bubble {
  background: var(--color-bg-secondary);
  border-radius: var(--radius-md);
  padding: 8px 12px;
}

.comment-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 4px;
}

.comment-author {
  font-weight: 700;
  font-size: var(--font-size-xs);
  color: var(--text-primary);
}

.reply-author {
  color: var(--color-primary);
}

.comment-time {
  font-size: 9px;
  color: var(--text-muted);
  display: flex;
  align-items: center;
  gap: 3px;
}

.time-icon {
  opacity: 0.7;
}

.comment-body {
  font-size: var(--font-size-sm);
  color: var(--text-secondary);
  line-height: 1.5;
  white-space: pre-wrap;
}

.text-sm {
  font-size: var(--font-size-xs);
}

.comment-body.deleted {
  font-style: italic;
  color: var(--text-muted);
}

/* Hover-only comment action links */
.comment-actions {
  display: flex;
  gap: 12px;
  margin-top: 6px;
  opacity: 0;
  transition: opacity var(--transition-fast);
  justify-content: flex-end;
}

.comment-bubble:hover .comment-actions {
  opacity: 1;
}

.btn-action {
  background: transparent;
  border: none;
  font-size: 9px;
  font-weight: 700;
  color: var(--text-muted);
  cursor: pointer;
}
.btn-action:hover {
  color: var(--color-primary);
  text-decoration: underline;
}
.btn-action.danger:hover {
  color: var(--color-danger);
}
.btn-action.reply:hover {
  color: var(--color-success);
}

.edited-flag {
  font-size: 9px;
  color: var(--text-muted);
  margin-left: 6px;
  font-style: italic;
}

/* Reply Thread indentations */
.replies-wrapper {
  display: flex;
  flex-direction: column;
  gap: 8px;
  margin-top: 4px;
  border-left: 1px dashed var(--border-color);
  padding-left: 12px;
}

.comment-item.reply {
  position: relative;
}

.edit-comment-area, .reply-input-area {
  display: flex;
  flex-direction: column;
  gap: 6px;
  margin-top: 6px;
}

.comment-textarea {
  width: 100%;
  background: var(--color-bg-secondary);
  border: 1px solid var(--border-color);
  color: var(--text-primary);
  border-radius: var(--radius-md);
  padding: 8px 10px;
  font-size: var(--font-size-xs);
  resize: none;
}

.edit-actions, .reply-actions {
  display: flex;
  justify-content: flex-end;
  gap: 6px;
}

.btn-text-cancel {
  font-size: 10px;
  font-weight: 700;
  color: var(--text-muted);
  background: transparent;
  cursor: pointer;
}

.btn-save {
  font-size: 10px;
  font-weight: 700;
  color: white;
  background: var(--color-primary);
  padding: 4px 10px;
  border-radius: var(--radius-sm);
  cursor: pointer;
}
.btn-save:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.empty-comments {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: var(--space-8) 0;
  text-align: center;
}

.empty-comment-icon {
  width: 44px;
  height: 44px;
  border-radius: 50%;
  background: var(--color-bg-secondary);
  color: var(--text-muted);
  display: flex;
  align-items: center;
  justify-content: center;
  margin-bottom: var(--space-3);
}

.empty-text-main {
  font-size: var(--font-size-sm);
  font-weight: 700;
  color: var(--text-primary);
  margin-bottom: 2px;
}

.empty-text-sub {
  font-size: 10px;
  color: var(--text-muted);
}

/* Discussion Input form area */
.comment-input-section {
  padding-top: var(--space-3);
  border-top: 1px solid var(--border-color);
  position: relative;
}

.input-container {
  display: flex;
  flex-direction: column;
  gap: 8px;
  background: var(--color-bg-secondary);
  border: 1px solid var(--border-color);
  border-radius: var(--radius-md);
  padding: 8px var(--space-3);
}

.discussion-textarea {
  width: 100%;
  border: none;
  background: transparent;
  color: var(--text-primary);
  font-size: var(--font-size-xs);
  resize: none;
  font-family: inherit;
  outline: none;
}

.input-footer {
  display: flex;
  justify-content: space-between;
  align-items: center;
  border-top: 1px solid rgba(255, 255, 255, 0.05);
  padding-top: 6px;
}

.keyboard-hint {
  font-size: 8px;
  color: var(--text-muted);
}

.btn-send-comment {
  display: inline-flex;
  align-items: center;
  gap: 4px;
  font-size: 10px;
  font-weight: 800;
  color: white;
  background: var(--color-primary);
  padding: 4px 12px;
  border-radius: var(--radius-sm);
  cursor: pointer;
  transition: background var(--transition-fast);
}

.btn-send-comment:hover:not(:disabled) {
  background: var(--color-primary-hover);
}

.btn-send-comment:disabled {
  opacity: 0.4;
  cursor: not-allowed;
}

/* Mentions Dropdown Auto-complete list */
.mentions-dropdown {
  position: absolute;
  bottom: calc(100% + 4px);
  left: 0;
  width: 100%;
  max-width: 280px;
  background: var(--glass-bg);
  backdrop-filter: var(--glass-blur);
  -webkit-backdrop-filter: var(--glass-blur);
  border: 1px solid var(--border-color);
  border-radius: var(--radius-md);
  box-shadow: var(--shadow-lg);
  max-height: 160px;
  overflow-y: auto;
  z-index: 100;
  padding: 4px;
}

.mention-item {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 6px 10px;
  border-radius: var(--radius-sm);
  cursor: pointer;
}

.mention-item:hover, .mention-item.active {
  background: var(--color-primary-light);
  color: var(--color-primary);
}

.mention-avatar {
  width: 24px;
  height: 24px;
  border-radius: 50%;
  color: white;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: bold;
  font-size: 10px;
}

.mention-info {
  display: flex;
  flex-direction: column;
}

.mention-name {
  font-size: 11px;
  font-weight: 700;
  color: var(--text-primary);
}

.mention-username {
  font-size: 9px;
  color: var(--text-muted);
}

.animate-fade {
  animation: fadeIn 0.25s ease;
}

@keyframes fadeIn {
  from { opacity: 0; transform: translateY(4px); }
  to { opacity: 1; transform: translateY(0); }
}
</style>
