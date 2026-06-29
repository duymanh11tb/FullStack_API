<template>
  <header class="app-header">
    <div class="header-left">
      <button class="sidebar-toggle" @click="$emit('toggle-sidebar')" id="btn-toggle-sidebar">
        <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
          <line x1="3" y1="6" x2="21" y2="6" />
          <line x1="3" y1="12" x2="21" y2="12" />
          <line x1="3" y1="18" x2="21" y2="18" />
        </svg>
      </button>
      <div class="breadcrumb">
        <span class="breadcrumb-item">{{ currentPageTitle }}</span>
      </div>
    </div>

    <div class="header-right">
      <!-- Theme Toggle -->
      <button class="icon-btn theme-toggle" @click="toggleTheme" title="Chuyển đổi giao diện">
        <svg v-if="isDarkTheme" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
          <circle cx="12" cy="12" r="5"/>
          <line x1="12" y1="1" x2="12" y2="3"/>
          <line x1="12" y1="21" x2="12" y2="23"/>
          <line x1="4.22" y1="4.22" x2="5.64" y2="5.64"/>
          <line x1="18.36" y1="18.36" x2="19.78" y2="19.78"/>
          <line x1="1" y1="12" x2="3" y2="12"/>
          <line x1="21" y1="12" x2="23" y2="12"/>
          <line x1="4.22" y1="19.78" x2="5.64" y2="18.36"/>
          <line x1="18.36" y1="5.64" x2="19.78" y2="4.22"/>
        </svg>
        <svg v-else width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
          <path d="M21 12.79A9 9 0 1 1 11.21 3 7 7 0 0 0 21 12.79z"/>
        </svg>
      </button>

      <!-- Notification Bell -->
      <div class="notification-wrapper" ref="notifRef">
        <button class="icon-btn" @click="toggleNotifDropdown" id="btn-notifications">
          <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M18 8A6 6 0 0 0 6 8c0 7-3 9-3 9h18s-3-2-3-9" />
            <path d="M13.73 21a2 2 0 0 1-3.46 0" />
          </svg>
          <span v-if="unreadCount > 0" class="notif-badge">{{ unreadCount > 9 ? '9+' : unreadCount }}</span>
        </button>

        <transition name="fade">
          <div v-if="showNotifDropdown" class="notif-dropdown">
            <div class="notif-dropdown-header">
              <h4>Thông báo</h4>
              <button v-if="unreadCount > 0" class="link-btn" @click="handleMarkAllRead">
                Đọc tất cả
              </button>
            </div>
            <div class="notif-dropdown-body">
              <div v-if="notifications.length === 0" class="notif-empty">
                Không có thông báo
              </div>
              <div
                v-for="notif in notifications.slice(0, 5)"
                :key="notif.id"
                class="notif-item"
                :class="{ unread: !notif.isRead }"
                @click="handleNotifClick(notif)"
              >
                <div class="notif-dot" v-if="!notif.isRead"></div>
                <div class="notif-content">
                  <p class="notif-message">{{ notif.message || notif.detail || 'Thông báo mới' }}</p>
                  <span class="notif-time">{{ formatTime(notif.createdAt) }}</span>
                </div>
              </div>
            </div>
            <router-link to="/notifications" class="notif-dropdown-footer" @click="showNotifDropdown = false">
              Xem tất cả thông báo
            </router-link>
          </div>
        </transition>
      </div>

      <!-- User Menu -->
      <div class="user-menu" ref="userRef">
        <button class="user-btn" @click="toggleUserMenu" id="btn-user-menu">
          <div class="user-avatar">
            {{ userInitial }}
          </div>
          <span class="user-name">{{ displayName }}</span>
          <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <polyline points="6 9 12 15 18 9" />
          </svg>
        </button>

        <transition name="fade">
          <div v-if="showUserMenu" class="dropdown-menu">
            <router-link to="/settings" class="dropdown-item" @click="showUserMenu = false">
              <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <circle cx="12" cy="12" r="3" />
                <path d="M12 1v2M12 21v2M4.22 4.22l1.42 1.42M18.36 18.36l1.42 1.42M1 12h2M21 12h2M4.22 19.78l1.42-1.42M18.36 5.64l1.42-1.42" />
              </svg>
              Cài đặt
            </router-link>
            <button class="dropdown-item danger" @click="handleLogout">
              <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <path d="M9 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h4" />
                <polyline points="16 17 21 12 16 7" />
                <line x1="21" y1="12" x2="9" y2="12" />
              </svg>
              Đăng xuất
            </button>
          </div>
        </transition>
      </div>
    </div>
  </header>
</template>

<script setup>
import { ref, computed, onMounted, onBeforeUnmount, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useAuthStore } from '../../stores/auth'
import { useNotificationStore } from '../../stores/notifications'

defineEmits(['toggle-sidebar'])

const route = useRoute()
const router = useRouter()
const authStore = useAuthStore()
const notifStore = useNotificationStore()

const showNotifDropdown = ref(false)
const showUserMenu = ref(false)
const notifRef = ref(null)
const userRef = ref(null)

const isDarkTheme = ref(false)

const displayName = computed(() => authStore.displayName)
const userInitial = computed(() => displayName.value.charAt(0).toUpperCase())
const unreadCount = computed(() => notifStore.unreadCount)
const notifications = computed(() => notifStore.notifications)

const pageTitles = {
  Dashboard: 'Tổng quan',
  Projects: 'Dự án',
  ProjectDetail: 'Chi tiết dự án',
  Notifications: 'Thông báo',
  Settings: 'Cài đặt'
}

const currentPageTitle = computed(() => pageTitles[route.name] || '')

function toggleNotifDropdown() {
  showNotifDropdown.value = !showNotifDropdown.value
  showUserMenu.value = false
}

function toggleUserMenu() {
  showUserMenu.value = !showUserMenu.value
  showNotifDropdown.value = false
}

function handleNotifClick(notif) {
  if (!notif.isRead) {
    notifStore.markAsRead(notif.id)
  }
  showNotifDropdown.value = false

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

function handleMarkAllRead() {
  notifStore.markAllAsRead()
}

function handleLogout() {
  authStore.logout()
  router.push('/login')
}

function toggleTheme() {
  isDarkTheme.value = !isDarkTheme.value
  if (isDarkTheme.value) {
    document.documentElement.setAttribute('data-theme', 'dark')
    localStorage.setItem('theme', 'dark')
  } else {
    document.documentElement.removeAttribute('data-theme')
    localStorage.setItem('theme', 'light')
  }
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
  return date.toLocaleDateString('vi-VN')
}

const userId = computed(() => authStore.user?.id || authStore.user?.Id)

function handleClickOutside(e) {
  if (notifRef.value && !notifRef.value.contains(e.target)) {
    showNotifDropdown.value = false
  }
  if (userRef.value && !userRef.value.contains(e.target)) {
    showUserMenu.value = false
  }
}

watch(userId, (newUserId) => {
  if (newUserId) {
    notifStore.startSignalR(newUserId)
  } else {
    notifStore.stopSignalR()
  }
})

onMounted(() => {
  document.addEventListener('click', handleClickOutside)
  notifStore.loadNotifications()
  
  if (userId.value) {
    notifStore.startSignalR(userId.value)
  }
  
  // Initialize Theme
  const savedTheme = localStorage.getItem('theme')
  if (savedTheme === 'dark' || (!savedTheme && window.matchMedia('(prefers-color-scheme: dark)').matches)) {
    isDarkTheme.value = true
    document.documentElement.setAttribute('data-theme', 'dark')
  }
})

onBeforeUnmount(() => {
  document.removeEventListener('click', handleClickOutside)
  notifStore.stopSignalR()
})
</script>

<style scoped>
.app-header {
  height: var(--header-height);
  background: var(--glass-bg);
  backdrop-filter: var(--glass-blur);
  -webkit-backdrop-filter: var(--glass-blur);
  border-bottom: 1px solid var(--border-color);
  box-shadow: var(--shadow-sm);
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0 var(--space-6);
  position: sticky;
  top: 0;
  z-index: 80;
  transition: all var(--transition-slow);
}

.header-left {
  display: flex;
  align-items: center;
  gap: var(--space-4);
}

.sidebar-toggle {
  display: none;
  padding: var(--space-2);
  border-radius: var(--radius-md);
  color: var(--color-text-secondary);
  transition: all var(--transition-fast);
}

.sidebar-toggle:hover {
  background: var(--color-bg-secondary);
  color: var(--color-text-primary);
}

@media (max-width: 768px) {
  .sidebar-toggle {
    display: flex;
  }
}

.breadcrumb-item {
  font-size: var(--font-size-md);
  font-weight: var(--font-weight-bold);
  background: var(--gradient-primary);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
  color: transparent;
}

.header-right {
  display: flex;
  align-items: center;
  gap: var(--space-2);
}

.icon-btn {
  position: relative;
  padding: var(--space-2);
  border-radius: var(--radius-md);
  color: var(--color-text-secondary);
  transition: all var(--transition-fast);
}

.icon-btn:hover {
  background: var(--color-primary-light);
  color: var(--color-primary);
  transform: translateY(-1px);
}

.notif-badge {
  position: absolute;
  top: 2px;
  right: 2px;
  min-width: 18px;
  height: 18px;
  background: var(--color-danger);
  color: white;
  font-size: 11px;
  font-weight: var(--font-weight-bold);
  border-radius: var(--radius-full);
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 0 4px;
  box-shadow: 0 2px 4px rgba(244, 63, 94, 0.4);
}

.notification-wrapper, .user-menu {
  position: relative;
}

.notif-dropdown {
  position: absolute;
  top: calc(100% + 12px);
  right: 0;
  width: 380px;
  background: var(--glass-bg);
  backdrop-filter: var(--glass-blur);
  -webkit-backdrop-filter: var(--glass-blur);
  border: 1px solid var(--border-color);
  border-radius: var(--radius-lg);
  box-shadow: var(--shadow-glass);
  overflow: hidden;
  z-index: var(--z-dropdown);
  animation: slideDown 0.2s cubic-bezier(0.16, 1, 0.3, 1);
}

@keyframes slideDown {
  from { opacity: 0; transform: translateY(-10px); }
  to { opacity: 1; transform: translateY(0); }
}

.notif-dropdown-header {
  padding: var(--space-4);
  border-bottom: 1px solid var(--border-color);
  display: flex;
  align-items: center;
  justify-content: space-between;
  background: var(--color-bg-secondary);
}

.notif-dropdown-header h4 {
  font-size: var(--font-size-base);
  font-weight: var(--font-weight-semibold);
}

.link-btn {
  font-size: var(--font-size-sm);
  color: var(--color-primary);
  font-weight: var(--font-weight-medium);
  transition: all var(--transition-fast);
}

.link-btn:hover {
  color: var(--color-primary-hover);
  text-shadow: var(--gradient-glow);
}

.notif-dropdown-body {
  max-height: 320px;
  overflow-y: auto;
}

.notif-empty {
  padding: var(--space-8) var(--space-4);
  text-align: center;
  color: var(--color-text-tertiary);
  font-size: var(--font-size-sm);
}

.notif-item {
  display: flex;
  align-items: flex-start;
  gap: var(--space-3);
  padding: var(--space-3) var(--space-4);
  cursor: pointer;
  transition: all var(--transition-fast);
  border-bottom: 1px solid var(--color-border);
}

.notif-item:hover {
  background: var(--sidebar-hover);
}

.notif-item.unread {
  background: var(--color-primary-light);
}

.notif-item.unread:hover {
  background: rgba(99, 102, 241, 0.2);
}

.notif-dot {
  width: 8px;
  height: 8px;
  background: var(--gradient-primary);
  border-radius: 50%;
  flex-shrink: 0;
  margin-top: 6px;
  box-shadow: var(--gradient-glow);
}

.notif-content {
  flex: 1;
  min-width: 0;
}

.notif-message {
  font-size: var(--font-size-sm);
  color: var(--color-text-primary);
  line-height: 1.4;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.notif-time {
  font-size: var(--font-size-xs);
  color: var(--color-text-tertiary);
  margin-top: 2px;
  display: block;
}

.notif-dropdown-footer {
  display: block;
  padding: var(--space-3) var(--space-4);
  text-align: center;
  font-size: var(--font-size-sm);
  font-weight: var(--font-weight-medium);
  color: var(--color-primary);
  border-top: 1px solid var(--border-color);
  transition: all var(--transition-fast);
  background: var(--color-bg-secondary);
}

.notif-dropdown-footer:hover {
  background: var(--sidebar-hover);
  color: var(--color-primary-hover);
}

/* User button */
.user-btn {
  display: flex;
  align-items: center;
  gap: var(--space-3);
  padding: var(--space-1) var(--space-2) var(--space-1) var(--space-1);
  border-radius: var(--radius-full);
  color: var(--color-text-secondary);
  transition: all var(--transition-fast);
  border: 1px solid transparent;
}

.user-btn:hover {
  background: var(--sidebar-hover);
  border-color: var(--border-color);
}

.user-avatar {
  width: 36px;
  height: 36px;
  background: var(--gradient-primary);
  color: white;
  border-radius: var(--radius-full);
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: var(--font-weight-bold);
  font-size: var(--font-size-base);
  box-shadow: var(--shadow-sm);
}

.user-name {
  font-size: var(--font-size-sm);
  font-weight: var(--font-weight-semibold);
  color: var(--color-text-primary);
}

@media (max-width: 768px) {
  .user-name { display: none; }
}

.dropdown-menu {
  position: absolute;
  top: calc(100% + 12px);
  right: 0;
  width: 200px;
  background: var(--glass-bg);
  backdrop-filter: var(--glass-blur);
  -webkit-backdrop-filter: var(--glass-blur);
  border: 1px solid var(--border-color);
  border-radius: var(--radius-lg);
  box-shadow: var(--shadow-glass);
  padding: var(--space-2);
  z-index: var(--z-dropdown);
  animation: slideDown 0.2s cubic-bezier(0.16, 1, 0.3, 1);
}

.dropdown-item {
  display: flex;
  align-items: center;
  gap: var(--space-3);
  width: 100%;
  padding: var(--space-2) var(--space-3);
  font-size: var(--font-size-sm);
  color: var(--color-text-primary);
  font-weight: var(--font-weight-medium);
  border-radius: var(--radius-md);
  transition: all var(--transition-fast);
  text-decoration: none;
}

.dropdown-item:hover {
  background: var(--sidebar-hover);
  transform: translateX(4px);
  color: var(--color-primary);
}

.dropdown-item.danger:hover {
  background: var(--color-danger-light);
  color: var(--color-danger);
}
</style>
