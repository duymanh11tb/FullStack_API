<template>
  <div class="activity-log-list">
    <div v-if="loading" class="loading-container">
      <LoadingSpinner text="Đang tải nhật ký hoạt động..." />
    </div>
    
    <div v-else-if="error" class="error-container">
      <div class="error-icon">
        <svg width="40" height="40" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5">
          <circle cx="12" cy="12" r="10" />
          <line x1="12" y1="8" x2="12" y2="12" />
          <line x1="12" y1="16" x2="12.01" y2="16" />
        </svg>
      </div>
      <p class="error-message">{{ error }}</p>
      <button @click="fetchLogs" class="btn-retry">Thử lại</button>
    </div>

    <div v-else-if="logs.length === 0" class="empty-container">
      <EmptyState 
        title="Chưa có hoạt động" 
        description="Lịch sử nhật ký hoạt động sẽ hiển thị tại đây khi hệ thống ghi nhận các tương tác liên quan." 
      />
    </div>

    <div v-else class="timeline">
      <div v-for="log in logs" :key="log.id" class="timeline-item">
        <div class="timeline-badge" :class="getActionColorClass(log.actionName)" :title="log.actionName">
          <span class="timeline-icon" v-html="getActionIcon(log.actionName)"></span>
        </div>
        <div class="timeline-panel glass-panel">
          <div class="timeline-header">
            <span class="actor-name">{{ log.userFullName }}</span>
            <span class="action-label">{{ getActionLabel(log.actionName) }}</span>
            <span class="timeline-time" :title="formatFullDate(log.createdAt)">{{ formatTime(log.createdAt) }}</span>
          </div>
          <div v-if="log.detail" class="timeline-body">
            <div class="detail-content" v-html="formatDetail(log.detail)"></div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, watch, defineProps } from 'vue'
import { getActivityLogsByProject, getActivityLogsByTask, getActivityLogsByUser } from '../../api/notifyApi'
import LoadingSpinner from './LoadingSpinner.vue'
import EmptyState from './EmptyState.vue'

const props = defineProps({
  projectId: {
    type: String,
    default: null
  },
  taskId: {
    type: String,
    default: null
  },
  userId: {
    type: String,
    default: null
  }
})

const logs = ref([])
const loading = ref(false)
const error = ref(null)

const fetchLogs = async () => {
  loading.value = true
  error.value = null
  try {
    let res
    if (props.taskId) {
      res = await getActivityLogsByTask(props.taskId)
    } else if (props.projectId) {
      res = await getActivityLogsByProject(props.projectId)
    } else if (props.userId) {
      res = await getActivityLogsByUser(props.userId)
    } else {
      logs.value = []
      loading.value = false
      return
    }

    logs.value = res.data || []
  } catch (err) {
    console.error('Lỗi khi tải nhật ký hoạt động:', err)
    error.value = 'Không thể tải nhật ký hoạt động. Vui lòng kiểm tra lại kết nối.'
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  fetchLogs()
})

watch(() => [props.projectId, props.taskId, props.userId], () => {
  fetchLogs()
}, { deep: true })

const getActionLabel = (actionName) => {
  switch (actionName) {
    case 'Commented':
      return 'đã thêm bình luận'
    case 'StatusChanged':
      return 'đã cập nhật trạng thái'
    case 'Assigned':
      return 'đã phân công công việc'
    case 'MemberAdded':
      return 'đã thêm thành viên mới'
    case 'TaskCreated':
      return 'đã tạo công việc mới'
    case 'TaskUpdated':
      return 'đã chỉnh sửa công việc'
    default:
      return 'đã thực hiện hoạt động'
  }
}

const getActionColorClass = (actionName) => {
  switch (actionName) {
    case 'Commented':
      return 'action-comment'
    case 'StatusChanged':
      return 'action-status'
    case 'Assigned':
      return 'action-assign'
    case 'MemberAdded':
      return 'action-member'
    case 'TaskCreated':
      return 'action-create'
    case 'TaskUpdated':
      return 'action-update'
    default:
      return 'action-default'
  }
}

const getActionIcon = (actionName) => {
  switch (actionName) {
    case 'Commented':
      // Message bubble icon
      return `<svg viewBox="0 0 24 24" width="16" height="16" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
        <path d="M21 15a2 2 0 0 1-2 2H7l-4 4V5a2 2 0 0 1 2-2h14a2 2 0 0 1 2 2z"></path>
      </svg>`
    case 'StatusChanged':
      // Sync/arrows exchange icon
      return `<svg viewBox="0 0 24 24" width="16" height="16" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
        <polyline points="23 4 23 10 17 10"></polyline>
        <polyline points="1 20 1 14 7 14"></polyline>
        <path d="M3.51 9a9 9 0 0 1 14.85-3.36L23 10M1 14l4.64 4.36A9 9 0 0 0 20.49 15"></path>
      </svg>`
    case 'Assigned':
      // User plus icon
      return `<svg viewBox="0 0 24 24" width="16" height="16" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
        <path d="M16 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2"></path>
        <circle cx="9" cy="7" r="4"></circle>
        <line x1="19" y1="8" x2="19" y2="14"></line>
        <line x1="22" y1="11" x2="16" y2="11"></line>
      </svg>`
    case 'MemberAdded':
      // Users icon
      return `<svg viewBox="0 0 24 24" width="16" height="16" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
        <path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2"></path>
        <circle cx="9" cy="7" r="4"></circle>
        <path d="M23 21v-2a4 4 0 0 0-3-3.87"></path>
        <path d="M16 3.13a4 4 0 0 1 0 7.75"></path>
      </svg>`
    case 'TaskCreated':
      // Plus square / check square icon
      return `<svg viewBox="0 0 24 24" width="16" height="16" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
        <polyline points="9 11 12 14 22 4"></polyline>
        <path d="M21 12v7a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h11"></path>
      </svg>`
    case 'TaskUpdated':
      // Edit icon
      return `<svg viewBox="0 0 24 24" width="16" height="16" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
        <path d="M11 4H4a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2v-7"></path>
        <path d="M18.5 2.5a2.121 2.121 0 0 1 3 3L12 15l-4 1 1-4 9.5-9.5z"></path>
      </svg>`
    default:
      // Activity icon
      return `<svg viewBox="0 0 24 24" width="16" height="16" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
        <polyline points="22 12 18 12 15 21 9 3 6 12 2 12"></polyline>
      </svg>`
  }
}

const formatTime = (dateStr) => {
  if (!dateStr) return ''
  const date = new Date(dateStr)
  const now = new Date()
  const diffMs = now - date

  if (diffMs < 0) return 'Vừa xong' // Do sai lệch đồng hồ server

  const diffMins = Math.floor(diffMs / 60000)
  if (diffMins < 1) return 'Vừa xong'
  if (diffMins < 60) return `${diffMins} phút trước`

  const diffHours = Math.floor(diffMins / 60)
  if (diffHours < 24) return `${diffHours} giờ trước`

  const diffDays = Math.floor(diffHours / 24)
  if (diffDays < 7) return `${diffDays} ngày trước`

  return date.toLocaleDateString('vi-VN', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  })
}

const formatFullDate = (dateStr) => {
  if (!dateStr) return ''
  return new Date(dateStr).toLocaleString('vi-VN')
}

const formatDetail = (detailText) => {
  if (!detailText) return ''
  
  // Highlight some statuses or items
  let text = detailText
    .replace(/</g, '&lt;')
    .replace(/>/g, '&gt;')

  // Format Status change details: "To Do" to "In Progress" etc.
  text = text.replace(/(To Do|In Progress|Review|Done|Low|Medium|High|Urgent)/gi, (match) => {
    return `<span class="highlight-badge status-${match.toLowerCase().replace(' ', '')}">${match}</span>`
  })

  return text
}
</script>

<style scoped>
.activity-log-list {
  display: flex;
  flex-direction: column;
  height: 100%;
  width: 100%;
}

.loading-container, .error-container, .empty-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: var(--space-8) var(--space-4);
  text-align: center;
  background: var(--bg-card);
  border: 1px solid var(--border-color);
  border-radius: var(--radius-lg);
  box-shadow: var(--shadow-sm);
  backdrop-filter: var(--glass-blur);
}

.error-icon {
  color: var(--color-danger);
  margin-bottom: var(--space-4);
}

.error-message {
  color: var(--color-text-secondary);
  font-weight: 500;
  margin-bottom: var(--space-4);
}

.btn-retry {
  background: var(--color-primary);
  color: white;
  border: none;
  padding: 10px 24px;
  border-radius: var(--radius-md);
  font-weight: 600;
  cursor: pointer;
  transition: all var(--transition-fast);
  box-shadow: var(--shadow-sm);
}

.btn-retry:hover {
  background: var(--color-primary-hover);
  transform: translateY(-1px);
}

.timeline {
  position: relative;
  padding: var(--space-2) 0;
  list-style: none;
  max-height: 600px;
  overflow-y: auto;
  padding-right: var(--space-2);
}

/* Custom scrollbar for timeline list */
.timeline::-webkit-scrollbar {
  width: 6px;
}
.timeline::-webkit-scrollbar-track {
  background: transparent;
}
.timeline::-webkit-scrollbar-thumb {
  background: var(--border-color);
  border-radius: var(--radius-full);
}
.timeline::-webkit-scrollbar-thumb:hover {
  background: var(--color-text-tertiary);
}

.timeline::before {
  content: "";
  position: absolute;
  top: 0;
  bottom: 0;
  left: 20px;
  width: 2px;
  background-color: var(--border-color);
}

.timeline-item {
  position: relative;
  margin-bottom: var(--space-5);
  padding-left: 50px;
  animation: slideIn 0.3s ease forwards;
  opacity: 0;
  transform: translateY(10px);
}

@keyframes slideIn {
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.timeline-badge {
  position: absolute;
  top: 6px;
  left: 4px;
  width: 34px;
  height: 34px;
  border-radius: var(--radius-full);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 10;
  color: white;
  box-shadow: var(--shadow-md);
  transition: transform var(--transition-fast);
}

.timeline-item:hover .timeline-badge {
  transform: scale(1.1) rotate(5deg);
}

.timeline-icon {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 16px;
  height: 16px;
}

.timeline-panel {
  padding: var(--space-4);
  border-radius: var(--radius-lg);
  border: 1px solid var(--border-color);
  background: var(--bg-card);
  box-shadow: var(--shadow-sm);
  backdrop-filter: var(--glass-blur);
  position: relative;
  transition: all var(--transition-fast);
}

.timeline-panel:hover {
  box-shadow: var(--shadow-md);
  border-color: rgba(99, 102, 241, 0.25);
  transform: translateX(4px);
}

.timeline-header {
  display: flex;
  align-items: center;
  flex-wrap: wrap;
  gap: var(--space-2);
}

.actor-name {
  font-weight: var(--font-weight-semibold);
  color: var(--text-primary);
  font-size: var(--font-size-base);
}

.action-label {
  color: var(--text-secondary);
  font-size: var(--font-size-sm);
}

.timeline-time {
  margin-left: auto;
  font-size: var(--font-size-xs);
  color: var(--text-muted);
  font-weight: 500;
}

.timeline-body {
  font-size: var(--font-size-sm);
  color: var(--text-secondary);
  margin-top: var(--space-3);
  line-height: var(--line-height-relaxed);
  background: var(--color-bg-secondary);
  padding: var(--space-3);
  border-radius: var(--radius-md);
  border-left: 3px solid var(--color-primary-light);
}

[data-theme="dark"] .timeline-body {
  background: rgba(0, 0, 0, 0.15);
}

/* Badge colors matching activities */
.action-comment {
  background: linear-gradient(135deg, #7c3aed 0%, #a855f7 100%); /* Purple */
}

.action-status {
  background: linear-gradient(135deg, #2563eb 0%, #3b82f6 100%); /* Blue */
}

.action-assign {
  background: linear-gradient(135deg, #d97706 0%, #f59e0b 100%); /* Orange */
}

.action-member {
  background: linear-gradient(135deg, #db2777 0%, #ec4899 100%); /* Pink */
}

.action-create {
  background: linear-gradient(135deg, #059669 0%, #10b981 100%); /* Green */
}

.action-update {
  background: linear-gradient(135deg, #475569 0%, #64748b 100%); /* Gray */
}

.action-default {
  background: var(--gradient-primary);
}

/* Highlight badge styling */
:deep(.highlight-badge) {
  display: inline-flex;
  align-items: center;
  padding: 2px 8px;
  font-size: 11px;
  font-weight: 600;
  border-radius: var(--radius-full);
  margin: 0 4px;
}

:deep(.status-todo) {
  background-color: rgba(100, 116, 139, 0.15);
  color: #64748b;
}

:deep(.status-inprogress) {
  background-color: rgba(59, 130, 246, 0.15);
  color: #3b82f6;
}

:deep(.status-review) {
  background-color: rgba(245, 158, 11, 0.15);
  color: #f59e0b;
}

:deep(.status-done) {
  background-color: rgba(16, 185, 129, 0.15);
  color: #10b981;
}

:deep(.status-low) {
  background-color: rgba(100, 116, 139, 0.15);
  color: #64748b;
}

:deep(.status-medium) {
  background-color: rgba(59, 130, 246, 0.15);
  color: #3b82f6;
}

:deep(.status-high) {
  background-color: rgba(245, 158, 11, 0.15);
  color: #f59e0b;
}

:deep(.status-urgent) {
  background-color: rgba(239, 68, 68, 0.15);
  color: #ef4444;
}

@media (max-width: 640px) {
  .timeline::before {
    left: 15px;
  }
  .timeline-item {
    padding-left: 36px;
  }
  .timeline-badge {
    width: 28px;
    height: 28px;
    left: 1px;
  }
  .timeline-icon svg {
    width: 12px;
    height: 12px;
  }
  .timeline-header {
    flex-direction: column;
    align-items: flex-start;
    gap: 2px;
  }
  .timeline-time {
    margin-left: 0;
    margin-top: 2px;
  }
}
</style>
