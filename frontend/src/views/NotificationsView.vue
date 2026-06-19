<template>
  <div class="notifications-page">
    <div class="page-header">
      <div>
        <h1>Thông báo</h1>
        <p class="text-secondary">Tất cả thông báo của bạn</p>
      </div>
      <BaseButton
        v-if="notifStore.unreadCount > 0"
        variant="secondary"
        size="sm"
        @click="notifStore.markAllAsRead()"
        id="btn-mark-all-read"
      >
        <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
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

    <div v-else-if="filteredNotifs.length > 0" class="notif-list-container">
      <div class="notif-list">
        <div
          v-for="notif in paginatedNotifs"
          :key="notif.id"
          class="notif-item"
          :class="{ unread: !notif.isRead }"
          @click="handleClick(notif)"
        >
          <div class="notif-icon" :class="getIconClass(notif.eventType)">
            <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <path d="M18 8A6 6 0 0 0 6 8c0 7-3 9-3 9h18s-3-2-3-9" />
              <path d="M13.73 21a2 2 0 0 1-3.46 0" />
            </svg>
          </div>
          <div class="notif-body">
            <p class="notif-message">{{ notif.displayMessage || notif.message || notif.detail || 'Thông báo mới' }}</p>
            <p v-if="notif.detail && notif.displayMessage" class="notif-detail">{{ notif.detail }}</p>
            <span class="notif-time">{{ formatTime(notif.createdAt) }}</span>
          </div>
          <div class="notif-status">
            <span v-if="!notif.isRead" class="unread-dot"></span>
          </div>
        </div>
      </div>

      <!-- Reusable Pagination Component -->
      <BasePagination
        v-model:currentPage="currentPage"
        :totalItems="filteredNotifs.length"
        :itemsPerPage="itemsPerPage"
      />
    </div>

    <EmptyState
      v-else
      :title="filter === 'unread' ? 'Không có thông báo chưa đọc' : 'Không có thông báo'"
      :description="filter === 'unread' ? 'Bạn đã đọc tất cả thông báo.' : 'Chưa có thông báo nào.'"
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

function getIconClass(eventType) {
  const map = {
    'comment': 'icon-blue',
    'assigned': 'icon-green',
    'status_changed': 'icon-orange',
    'mention': 'icon-purple',
    'sprint_started': 'icon-cyan',
    'member_added': 'icon-pink'
  }
  return map[eventType] || 'icon-blue'
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
  animation: fadeIn 0.3s ease;
  max-width: 800px;
}

@keyframes fadeIn {
  from { opacity: 0; transform: translateY(8px); }
  to { opacity: 1; transform: translateY(0); }
}

.page-header {
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
  margin-bottom: var(--space-6);
}

.page-header h1 {
  font-size: var(--font-size-2xl);
  margin-bottom: var(--space-1);
}

.filter-tabs-wrapper {
  margin-bottom: var(--space-5);
}

.filter-tabs {
  display: flex;
  gap: var(--space-1);
  background: var(--color-bg-secondary);
  border-radius: var(--radius-lg);
  padding: 3px;
  width: fit-content;
}

.filter-tab {
  display: flex;
  align-items: center;
  gap: var(--space-2);
  padding: var(--space-2) var(--space-4);
  font-size: var(--font-size-sm);
  font-weight: var(--font-weight-medium);
  color: var(--color-text-secondary);
  border-radius: var(--radius-md);
  transition: all var(--transition-fast);
}

.filter-tab:hover {
  color: var(--color-text-primary);
}

.filter-tab.active {
  background: var(--bg-white-to-card);
  color: var(--color-text-primary);
  box-shadow: var(--shadow-xs);
}

.filter-badge {
  min-width: 18px;
  height: 18px;
  background: var(--color-danger);
  color: white;
  font-size: 11px;
  border-radius: var(--radius-full);
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 0 5px;
}

/* Notification list */
.notif-list {
  background: var(--bg-white-to-card);
  border: 1px solid var(--color-border);
  border-radius: var(--radius-lg);
  overflow: hidden;
}

.notif-item {
  display: flex;
  align-items: flex-start;
  gap: var(--space-4);
  padding: var(--space-4) var(--space-5);
  border-bottom: 1px solid var(--color-bg-secondary);
  cursor: pointer;
  transition: background var(--transition-fast);
}

.notif-item:last-child {
  border-bottom: none;
}

.notif-item:hover {
  background: var(--color-bg);
}

.notif-item.unread {
  background: var(--color-primary-subtle);
}

.notif-item.unread:hover {
  background: var(--color-primary-light);
}

.notif-icon {
  width: 40px;
  height: 40px;
  border-radius: var(--radius-full);
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

.icon-blue { background: var(--color-primary-light); color: var(--color-primary); }
.icon-green { background: var(--color-success-light); color: var(--color-success); }
.icon-orange { background: var(--color-warning-light); color: var(--color-warning); }
.icon-purple { background: #EDE9FE; color: #7C3AED; }
.icon-cyan { background: var(--color-info-light); color: var(--color-info); }
.icon-pink { background: #FCE7F3; color: #DB2777; }

.notif-body {
  flex: 1;
  min-width: 0;
}

.notif-message {
  font-size: var(--font-size-sm);
  color: var(--color-text-primary);
  line-height: 1.5;
  font-weight: var(--font-weight-medium);
}

.notif-detail {
  font-size: var(--font-size-xs);
  color: var(--color-text-secondary);
  margin-top: var(--space-1);
}

.notif-time {
  font-size: var(--font-size-xs);
  color: var(--color-text-tertiary);
  margin-top: var(--space-1);
  display: block;
}

.notif-status {
  display: flex;
  align-items: center;
  padding-top: var(--space-1);
}

.unread-dot {
  width: 8px;
  height: 8px;
  background: var(--color-primary);
  border-radius: 50%;
}
</style>
