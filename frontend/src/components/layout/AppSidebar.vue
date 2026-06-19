<template>
  <aside class="app-sidebar" :class="{ collapsed: collapsed }">
    <div class="sidebar-header">
      <router-link to="/dashboard" class="sidebar-logo" id="link-home">
        <div class="logo-icon">
          <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
            <rect x="3" y="3" width="7" height="7" rx="1.5" />
            <rect x="14" y="3" width="7" height="7" rx="1.5" />
            <rect x="3" y="14" width="7" height="7" rx="1.5" />
            <rect x="14" y="14" width="7" height="7" rx="1.5" />
          </svg>
        </div>
        <span class="logo-text">QLDA</span>
      </router-link>
    </div>

    <nav class="sidebar-nav">
      <div class="nav-section">
        <span class="nav-section-title">CHÍNH</span>

        <router-link to="/dashboard" class="nav-item" :class="{ active: isActive('/dashboard') }" id="nav-dashboard">
          <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <rect x="3" y="3" width="7" height="9" rx="1" />
            <rect x="14" y="3" width="7" height="5" rx="1" />
            <rect x="14" y="12" width="7" height="9" rx="1" />
            <rect x="3" y="16" width="7" height="5" rx="1" />
          </svg>
          <span>Tổng quan</span>
        </router-link>

        <router-link to="/projects" class="nav-item" :class="{ active: isActive('/projects') }" id="nav-projects">
          <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M22 19a2 2 0 0 1-2 2H4a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h5l2 3h9a2 2 0 0 1 2 2z" />
          </svg>
          <span>Dự án</span>
        </router-link>

        <router-link to="/tasks" class="nav-item" :class="{ active: isActive('/tasks') }" id="nav-tasks">
          <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <rect x="3" y="4" width="18" height="18" rx="2" ry="2" />
            <line x1="16" y1="2" x2="16" y2="6" />
            <line x1="8" y1="2" x2="8" y2="6" />
            <line x1="3" y1="10" x2="21" y2="10" />
            <path d="M8 14h.01" />
            <path d="M12 14h.01" />
            <path d="M16 14h.01" />
            <path d="M8 18h.01" />
            <path d="M12 18h.01" />
            <path d="M16 18h.01" />
          </svg>
          <span>Tác vụ (Task Board)</span>
        </router-link>

        <router-link to="/notifications" class="nav-item" :class="{ active: isActive('/notifications') }" id="nav-notifications">
          <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M18 8A6 6 0 0 0 6 8c0 7-3 9-3 9h18s-3-2-3-9" />
            <path d="M13.73 21a2 2 0 0 1-3.46 0" />
          </svg>
          <span>Thông báo</span>
          <span v-if="unreadCount > 0" class="nav-badge">{{ unreadCount }}</span>
        </router-link>
      </div>

      <div class="nav-section" v-if="isAdmin">
        <span class="nav-section-title">HỆ THỐNG</span>

        <router-link to="/admin" class="nav-item" :class="{ active: isActive('/admin') }" id="nav-admin">
          <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2" />
            <circle cx="9" cy="7" r="4" />
            <path d="M23 21v-2a4 4 0 0 0-3-3.87" />
            <path d="M16 3.13a4 4 0 0 1 0 7.75" />
          </svg>
          <span>Quản trị viên</span>
        </router-link>

        <router-link to="/settings" class="nav-item" :class="{ active: isActive('/settings') }" id="nav-settings">
          <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <circle cx="12" cy="12" r="3" />
            <path d="M19.4 15a1.65 1.65 0 0 0 .33 1.82l.06.06a2 2 0 0 1 0 2.83 2 2 0 0 1-2.83 0l-.06-.06a1.65 1.65 0 0 0-1.82-.33 1.65 1.65 0 0 0-1 1.51V21a2 2 0 0 1-2 2 2 2 0 0 1-2-2v-.09A1.65 1.65 0 0 0 9 19.4a1.65 1.65 0 0 0-1.82.33l-.06.06a2 2 0 0 1-2.83 0 2 2 0 0 1 0-2.83l.06-.06A1.65 1.65 0 0 0 4.68 15a1.65 1.65 0 0 0-1.51-1H3a2 2 0 0 1-2-2 2 2 0 0 1 2-2h.09A1.65 1.65 0 0 0 4.6 9a1.65 1.65 0 0 0-.33-1.82l-.06-.06a2 2 0 0 1 0-2.83 2 2 0 0 1 2.83 0l.06.06A1.65 1.65 0 0 0 9 4.68a1.65 1.65 0 0 0 1-1.51V3a2 2 0 0 1 2-2 2 2 0 0 1 2 2v.09a1.65 1.65 0 0 0 1 1.51 1.65 1.65 0 0 0 1.82-.33l.06-.06a2 2 0 0 1 2.83 0 2 2 0 0 1 0 2.83l-.06.06a1.65 1.65 0 0 0-.33 1.82V9a1.65 1.65 0 0 0 1.51 1H21a2 2 0 0 1 2 2 2 2 0 0 1-2 2h-.09a1.65 1.65 0 0 0-1.51 1z" />
          </svg>
          <span>Cài đặt</span>
        </router-link>
      </div>
    </nav>
  </aside>
</template>

<script setup>
import { computed } from 'vue'
import { useRoute } from 'vue-router'
import { useNotificationStore } from '../../stores/notifications'
import { useAuthStore } from '../../stores/auth'

defineProps({
  collapsed: Boolean
})

const route = useRoute()
const notifStore = useNotificationStore()
const authStore = useAuthStore()

const unreadCount = computed(() => notifStore.unreadCount)
const isAdmin = computed(() => authStore.isAdmin)

function isActive(path) {
  if (path === '/dashboard') return route.path === '/dashboard'
  return route.path.startsWith(path)
}
</script>

<style scoped>
.app-sidebar {
  width: var(--sidebar-width);
  height: 100vh;
  background: var(--sidebar-bg);
  backdrop-filter: var(--glass-blur);
  -webkit-backdrop-filter: var(--glass-blur);
  border-right: 1px solid var(--glass-border);
  display: flex;
  flex-direction: column;
  position: fixed;
  left: 0;
  top: 0;
  z-index: 60;
  transition: transform var(--transition-slow);
}

@media (max-width: 768px) {
  .app-sidebar {
    transform: translateX(0);
  }
  .app-sidebar.collapsed {
    transform: translateX(-100%);
  }
}

.sidebar-header {
  height: var(--header-height);
  display: flex;
  align-items: center;
  padding: 0 var(--space-5);
  border-bottom: 1px solid rgba(255, 255, 255, 0.06);
}

.sidebar-logo {
  display: flex;
  align-items: center;
  gap: var(--space-3);
  text-decoration: none;
  color: var(--sidebar-text-active);
}

.logo-icon {
  width: 36px;
  height: 36px;
  background: var(--gradient-primary);
  border-radius: var(--radius-lg);
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  box-shadow: 0 4px 12px rgba(37, 99, 235, 0.3);
}

.logo-text {
  font-size: var(--font-size-lg);
  font-weight: var(--font-weight-bold);
  letter-spacing: 0.5px;
  font-family: var(--font-heading);
}

.sidebar-nav {
  flex: 1;
  padding: var(--space-5) var(--space-3);
  overflow-y: auto;
}

.nav-section {
  margin-bottom: var(--space-6);
}

.nav-section-title {
  display: block;
  font-size: 10px;
  font-weight: 700;
  color: rgba(255, 255, 255, 0.35);
  letter-spacing: 2px;
  padding: 0 var(--space-3);
  margin-bottom: var(--space-3);
}

.nav-item {
  display: flex;
  align-items: center;
  gap: var(--space-3);
  padding: 10px var(--space-3);
  border-radius: var(--radius-md);
  color: var(--sidebar-text);
  text-decoration: none;
  font-size: var(--font-size-sm);
  font-weight: var(--font-weight-medium);
  transition: all var(--transition-fast);
  margin-bottom: 4px;
  position: relative;
}

.nav-item:hover {
  background: var(--sidebar-hover);
  color: var(--sidebar-text-active);
  transform: translateX(4px);
}

.nav-item.active {
  background: var(--sidebar-active);
  color: var(--sidebar-text-active);
  font-weight: 600;
}

.nav-item.active::before {
  content: '';
  position: absolute;
  left: 0;
  top: 6px;
  bottom: 6px;
  width: 3px;
  background: var(--sidebar-active-border);
  border-radius: 0 2px 2px 0;
}

.nav-badge {
  margin-left: auto;
  min-width: 20px;
  height: 20px;
  background: var(--color-danger);
  color: white;
  font-size: 11px;
  font-weight: var(--font-weight-semibold);
  border-radius: var(--radius-full);
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 0 6px;
  box-shadow: 0 2px 5px rgba(225, 29, 72, 0.4);
}
</style>
