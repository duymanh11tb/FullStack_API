<template>
  <div class="notifications-page">
    <div class="page-header">
      <div>
        <h1>Thông báo</h1>
        <p class="subtitle">Quản lý và cập nhật toàn bộ hoạt động của bạn</p>
      </div>
      <BaseButton
        v-if="notifStore.unreadCount > 0"
        variant="secondary"
        size="sm"
        @click="notifStore.markAllAsRead()"
        id="btn-mark-all-read"
        class="mark-all-btn"
      >
        <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" style="margin-right: 6px;">
          <polyline points="20 6 9 17 4 12" />
        </svg>
        Đọc tất cả
      </BaseButton>
    </div>

    <!-- Filter -->
    <div class="filter-tabs-wrapper">
      <div class="filter-tabs">
        <button :class="['filter-tab', { active: filter === 'all' }]" @click="filter = 'all'; loadData()">
          Tất cả
        </button>
        <button :class="['filter-tab', { active: filter === 'unread' }]" @click="filter = 'unread'; loadData()">
          Chưa đọc
          <span v-if="notifStore.unreadCount > 0" class="filter-badge">{{ notifStore.unreadCount }}</span>
        </button>
        <button :class="['filter-tab', { active: filter === 'read' }]" @click="filter = 'read'; loadData()">
          Đã đọc
        </button>
      </div>
    </div>

    <LoadingSpinner v-if="notifStore.loading" text="Đang tải thông báo..." />

    <div v-else-if="filteredNotifs.length > 0" class="notif-feed-container">
      <div class="notif-feed">
        <div
          v-for="notif in paginatedNotifs"
          :key="notif.id"
          class="notif-card glass-panel"
          :class="{ unread: !notif.isRead }"
          @click="handleClick(notif)"
        >
          <!-- Accent stripe on the left -->
          <div class="notif-accent" :class="getIconClass(notif.type)"></div>

          <div class="notif-icon-box" :class="getIconClass(notif.type)">
            <!-- Custom SVGs depending on type -->
            <svg v-if="isCommentType(notif.type)" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
              <path d="M21 15a2 2 0 0 1-2 2H7l-4 4V5a2 2 0 0 1 2-2h14a2 2 0 0 1 2 2z" />
            </svg>
            <svg v-else-if="isTaskType(notif.type)" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
              <rect x="3" y="4" width="18" height="18" rx="2" ry="2"/><line x1="16" y1="2" x2="16" y2="6"/><line x1="8" y1="2" x2="8" y2="6"/><line x1="3" y1="10" x2="21" y2="10"/>
            </svg>
            <svg v-else-if="isMemberType(notif.type)" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
              <path d="M16 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2"/><circle cx="8.5" cy="7" r="4"/>
            </svg>
            <svg v-else width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
              <path d="M18 8A6 6 0 0 0 6 8c0 7-3 9-3 9h18s-3-2-3-9" /><path d="M13.73 21a2 2 0 0 1-3.46 0" />
            </svg>
          </div>

          <div class="notif-body">
            <div class="notif-meta">
              <span class="notif-category" :class="getIconClass(notif.type)">
                {{ getCategoryName(notif.type) }}
              </span>
              <span class="notif-time">
                <svg width="11" height="11" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" style="margin-right: 4px; vertical-align: middle;">
                  <circle cx="12" cy="12" r="10"/><polyline points="12 6 12 12 16 14"/>
                </svg>
                {{ formatTime(notif.createdAt) }}
              </span>
            </div>
            <p class="notif-message">{{ notif.displayMessage || notif.message || notif.detail || 'Thông báo mới' }}</p>
            <p v-if="notif.detail && notif.displayMessage" class="notif-detail">{{ notif.detail }}</p>
          </div>

          <!-- Quick Actions Panel -->
          <div class="notif-actions" @click.stop>
            <button 
              v-if="!notif.isRead" 
              class="quick-action-btn"
              @click="notifStore.markAsRead(notif.id)"
              title="Đánh dấu đã đọc"
            >
              <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="3">
                <polyline points="20 6 9 17 4 12" />
              </svg>
            </button>
          </div>
        </div>
      </div>

      <!-- Reusable Pagination Component -->
      <BasePagination
        :currentPage="currentPage"
        @update:currentPage="currentPage = $event"
        :totalItems="filteredNotifs.length"
        :itemsPerPage="itemsPerPage"
      />
    </div>

    <EmptyState
      v-else
      :title="filter === 'unread' ? 'Không có thông báo chưa đọc' : 'Không có thông báo'"
      :description="filter === 'unread' ? 'Tuyệt vời! Bạn đã xem hết mọi thông báo.' : 'Hộp thư của bạn hiện đang trống.'"
      class="empty-state-card"
    />
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useNotificationStore } from '../stores/notifications'
import BaseButton from '../components/common/BaseButton.vue'
import LoadingSpinner from '../components/common/LoadingSpinner.vue'
import EmptyState from '../components/common/EmptyState.vue'
import BasePagination from '../components/common/BasePagination.vue'

const router = useRouter()
const notifStore = useNotificationStore()
const filter = ref('all')

const currentPage = ref(1)
const itemsPerPage = ref(8) // 8 notifications per page

const filteredNotifs = computed(() => {
  if (filter.value === 'unread') return notifStore.unreadNotifications
  if (filter.value === 'read') return notifStore.readNotifications
  return notifStore.notifications
})

const paginatedNotifs = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage.value
  const end = start + itemsPerPage.value
  return filteredNotifs.value.slice(start, end)
})

function loadData() {
  currentPage.value = 1
  if (filter.value === 'unread') {
    notifStore.loadNotifications(false)
  } else if (filter.value === 'read') {
    notifStore.loadNotifications(true)
  } else {
    notifStore.loadNotifications()
  }
}

function handleClick(notif) {
  if (!notif.isRead) {
    notifStore.markAsRead(notif.id)
  }

  const type = notif.type
  const typeName = notif.typeName
  const refId = notif.referenceId

  const isTaskRelated = [1, 2, 3, 6, 7].includes(type) || ['TaskAssigned', 'TaskStatusChanged', 'CommentMention', 'TaskDeadlineApproaching', 'CommentAdded'].includes(typeName)
  const isProjectRelated = [5].includes(type) || ['MemberAdded'].includes(typeName)

  if (isTaskRelated && refId) {
    router.push({ path: '/tasks', query: { taskId: refId } })
  } else if (isProjectRelated && refId) {
    router.push(`/projects/${refId}`)
  }
}

function isCommentType(type) {
  return ['comment.added', 'comment.mention', 'CommentAdded', 'CommentMention'].includes(type)
}

function isTaskType(type) {
  return ['task.assigned', 'task.status.changed', 'task.deadline.approaching', 'task.deadline.overdue', 'TaskAssigned', 'TaskStatusChanged', 'TaskDeadlineApproaching'].includes(type)
}

function isMemberType(type) {
  return ['member.added', 'MemberAdded'].includes(type)
}

function getCategoryName(type) {
  if (isCommentType(type)) return 'Thảo luận'
  if (isTaskType(type)) return 'Tác vụ'
  if (isMemberType(type)) return 'Dự án'
  return 'Hệ thống'
}

function getIconClass(type) {
  if (isCommentType(type)) return 'notif-blue'
  if (isTaskType(type)) return 'notif-green'
  if (isMemberType(type)) return 'notif-purple'
  return 'notif-orange'
}

function formatTime(dateStr) {
  if (!dateStr) return ''
  const date = new Date(dateStr)
  const now = new Date()
  const diff = now - date
  const minutes = Math.floor(diff / 60000)
  if (minutes < 1) return 'Vừa xong'
  if (minutes < 60) return `${minutes} phút trước`
  const hours = Math.floor(minutes / 60)
  if (hours < 24) return `${hours} giờ trước`
  const days = Math.floor(hours / 24)
  if (days < 7) return `${days} ngày trước`
  return date.toLocaleDateString('vi-VN', { day: '2-digit', month: '2-digit', year: 'numeric' })
}

onMounted(() => {
  loadData()
})
</script>

<style scoped>
.notifications-page {
  animation: fadeIn 0.4s cubic-bezier(0.16, 1, 0.3, 1);
  max-width: 900px;
  margin: 0 auto;
  padding: var(--space-4) 0;
}

@keyframes fadeIn {
  from { opacity: 0; transform: translateY(12px); }
  to { opacity: 1; transform: translateY(0); }
}

.page-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: var(--space-6);
  flex-wrap: wrap;
  gap: var(--space-4);
}

.page-header h1 {
  font-size: var(--font-size-2xl);
  font-weight: var(--font-weight-extrabold);
  background: var(--gradient-primary);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
}

.subtitle {
  color: var(--text-secondary);
  font-size: var(--font-size-sm);
  font-weight: 500;
  margin-top: 2px;
}

.mark-all-btn {
  display: inline-flex;
  align-items: center;
  font-weight: 700;
  border-radius: var(--radius-full);
}

.filter-tabs-wrapper {
  margin-bottom: var(--space-6);
}

.filter-tabs {
  display: flex;
  gap: 4px;
  background: var(--color-bg-secondary);
  border-radius: var(--radius-lg);
  padding: 4px;
  width: fit-content;
  border: 1px solid var(--border-color);
}

[data-theme='dark'] .filter-tabs {
  background: rgba(15, 23, 42, 0.4);
}

.filter-tab {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: var(--space-2) var(--space-5);
  font-size: var(--font-size-sm);
  font-weight: var(--font-weight-bold);
  color: var(--text-secondary);
  border-radius: var(--radius-md);
  transition: all var(--transition-fast);
}

.filter-tab:hover {
  color: var(--color-text-primary);
}

.filter-tab.active {
  background: var(--bg-white-to-card);
  color: var(--color-text-primary);
  box-shadow: var(--shadow-sm);
}

.filter-badge {
  min-width: 18px;
  height: 18px;
  background: var(--color-danger);
  color: white;
  font-size: 10px;
  font-weight: 700;
  border-radius: var(--radius-full);
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 0 5px;
  box-shadow: 0 2px 4px rgba(244, 63, 94, 0.3);
}

/* Feed Cards Container */
.notif-feed {
  display: flex;
  flex-direction: column;
  gap: var(--space-3);
  margin-bottom: var(--space-6);
}

.notif-card {
  display: flex;
  align-items: flex-start;
  gap: var(--space-4);
  padding: 18px 24px;
  background: var(--bg-white-to-card);
  border-radius: var(--radius-xl);
  border: 1px solid var(--border-color);
  box-shadow: var(--shadow-xs);
  cursor: pointer;
  position: relative;
  overflow: hidden;
  transition: all var(--transition-base);
}

.notif-card:hover {
  transform: translateY(-2px);
  box-shadow: var(--shadow-md);
  border-color: var(--color-border-hover);
}

/* Accent lines on card left */
.notif-accent {
  position: absolute;
  top: 0;
  bottom: 0;
  left: 0;
  width: 4px;
  transition: width var(--transition-fast);
}

.notif-card:hover .notif-accent {
  width: 6px;
}

.notif-accent.notif-blue { background: var(--color-primary); }
.notif-accent.notif-green { background: var(--color-success); }
.notif-accent.notif-purple { background: var(--color-accent); }
.notif-accent.notif-orange { background: var(--color-warning); }

/* Mute design for read items */
.notif-card:not(.unread) {
  opacity: 0.72;
}

.notif-card.unread {
  background: var(--glass-bg);
}

.notif-icon-box {
  width: 42px;
  height: 42px;
  border-radius: var(--radius-md);
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
  box-shadow: var(--shadow-xs);
  border: 1px solid transparent;
}

.notif-icon-box.notif-blue { background: var(--color-primary-light); color: var(--color-primary); }
.notif-icon-box.notif-green { background: var(--color-success-light); color: var(--color-success); }
.notif-icon-box.notif-purple { background: var(--color-accent-light); color: var(--color-accent); }
.notif-icon-box.notif-orange { background: var(--color-warning-light); color: var(--color-warning); }

.notif-body {
  flex: 1;
  min-width: 0;
}

.notif-meta {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: var(--space-2);
}

.notif-category {
  font-size: 11px;
  font-weight: 800;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  padding: 2px 8px;
  border-radius: var(--radius-sm);
}

.notif-category.notif-blue { background: var(--color-primary-light); color: var(--color-primary); }
.notif-category.notif-green { background: var(--color-success-light); color: var(--color-success); }
.notif-category.notif-purple { background: var(--color-accent-light); color: var(--color-accent); }
.notif-category.notif-orange { background: var(--color-warning-light); color: var(--color-warning); }

.notif-time {
  font-size: 11px;
  color: var(--color-text-tertiary);
  display: flex;
  align-items: center;
  font-weight: 500;
}

.notif-message {
  font-size: var(--font-size-sm);
  color: var(--color-text-primary);
  line-height: 1.5;
  font-weight: 600;
}

.notif-card:not(.unread) .notif-message {
  font-weight: 500;
}

.notif-detail {
  font-size: var(--font-size-xs);
  color: var(--color-text-secondary);
  margin-top: var(--space-1);
}

/* Quick Actions Button appearing on hover */
.notif-actions {
  display: flex;
  align-items: center;
  opacity: 0;
  transform: translateX(8px);
  transition: all var(--transition-fast);
  margin-left: var(--space-2);
}

.notif-card:hover .notif-actions {
  opacity: 1;
  transform: translateX(0);
}

.quick-action-btn {
  width: 28px;
  height: 28px;
  border-radius: var(--radius-full);
  background: var(--color-bg-secondary);
  border: 1px solid var(--border-color);
  color: var(--color-text-secondary);
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  transition: all var(--transition-fast);
}

.quick-action-btn:hover {
  background: var(--color-success-light);
  color: var(--color-success);
  border-color: transparent;
}

.empty-state-card {
  padding: 60px !important;
  background: var(--bg-white-to-card) !important;
  border-radius: var(--radius-xl) !important;
  border: 1px solid var(--border-color) !important;
  box-shadow: var(--shadow-glass) !important;
}
</style>
