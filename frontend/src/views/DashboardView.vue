<template>
  <div class="dashboard">
    <div class="page-header">
      <div class="header-main">
        <h1 class="welcome-title">Xin chào, {{ displayName }}!</h1>
        <p class="subtitle">Chào mừng bạn trở lại. Đây là tổng quan hoạt động dự án hôm nay.</p>
      </div>
    </div>

    <!-- Stats Grid -->
    <div class="stats-grid">
      <div class="stat-card glass-panel" @click="$router.push('/projects')">
        <div class="stat-icon icon-blue">
          <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M22 19a2 2 0 0 1-2 2H4a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h5l2 3h9a2 2 0 0 1 2 2z" />
          </svg>
        </div>
        <div class="stat-content">
          <span class="stat-value">{{ projectStore.projectCount }}</span>
          <span class="stat-label">Tổng số dự án</span>
        </div>
      </div>

      <div class="stat-card glass-panel" @click="$router.push('/projects')">
        <div class="stat-icon icon-emerald">
          <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <polyline points="20 6 9 17 4 12" />
          </svg>
        </div>
        <div class="stat-content">
          <span class="stat-value text-emerald">{{ activeCount }}</span>
          <span class="stat-label">Dự án hoạt động</span>
        </div>
      </div>

      <div class="stat-card glass-panel" @click="$router.push('/notifications')">
        <div class="stat-icon icon-amber">
          <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M18 8A6 6 0 0 0 6 8c0 7-3 9-3 9h18s-3-2-3-9" />
            <path d="M13.73 21a2 2 0 0 1-3.46 0" />
          </svg>
        </div>
        <div class="stat-content">
          <span class="stat-value text-amber">{{ notifStore.unreadCount }}</span>
          <span class="stat-label">Thông báo mới</span>
        </div>
      </div>

      <div class="stat-card glass-panel">
        <div class="stat-icon icon-violet">
          <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2" />
            <circle cx="9" cy="7" r="4" />
            <path d="M23 21v-2a4 4 0 0 0-3-3.87" />
            <path d="M16 3.13a4 4 0 0 1 0 7.75" />
          </svg>
        </div>
        <div class="stat-content">
          <span class="stat-value text-violet">{{ totalMembers }}</span>
          <span class="stat-label">Thành viên đội</span>
        </div>
      </div>
    </div>

    <!-- Recent Projects -->
    <div class="section-card glass-panel">
      <div class="section-header">
        <div class="section-title-wrapper">
          <h2 class="section-title">Dự án gần đây</h2>
          <span class="section-badge">Gần nhất</span>
        </div>
        <router-link to="/projects" class="view-all-link">
          Xem tất cả
          <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
            <polyline points="9 18 15 12 9 6" />
          </svg>
        </router-link>
      </div>

      <div class="section-body">
        <LoadingSpinner v-if="projectStore.loading" text="Đang tải dự án..." />

        <div v-else-if="projectStore.projects.length > 0" class="project-grid">
          <div
            v-for="project in recentProjects"
            :key="project.id"
            class="project-card"
            @click="$router.push(`/projects/${project.id}`)"
          >
            <div class="project-card-header">
              <div class="project-indicator" :style="{ background: project.color || '#2563EB' }"></div>
              <StatusBadge :status="project.status" />
            </div>
            
            <h3 class="project-name">{{ project.name }}</h3>
            <p class="project-desc">{{ project.description || 'Không có mô tả chi tiết cho dự án này.' }}</p>
            
            <div class="project-card-footer">
              <div class="meta-item" title="Thành viên">
                <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                  <path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2" />
                  <circle cx="9" cy="7" r="4" />
                </svg>
                <span>{{ project.memberCount || 0 }} thành viên</span>
              </div>
              <div class="meta-item" title="Ngày bắt đầu">
                <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                  <rect x="3" y="4" width="18" height="18" rx="2" ry="2" />
                  <line x1="16" y1="2" x2="16" y2="6" />
                  <line x1="8" y1="2" x2="8" y2="6" />
                  <line x1="3" y1="10" x2="21" y2="10" />
                </svg>
                <span>{{ formatDate(project.startDate) }}</span>
              </div>
            </div>
          </div>
        </div>

        <EmptyState
          v-else
          title="Chưa có dự án nào"
          description="Hãy tạo dự án đầu tiên của bạn để bắt đầu phân công tác vụ và làm việc cùng đồng đội."
        >
          <BaseButton variant="primary" class="mt-4" @click="$router.push('/projects')">
            <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" style="margin-right: 4px;">
              <line x1="12" y1="5" x2="12" y2="19" />
              <line x1="5" y1="12" x2="19" y2="12" />
            </svg>
            Tạo dự án mới
          </BaseButton>
        </EmptyState>
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed, onMounted } from 'vue'
import { useAuthStore } from '../stores/auth'
import { useProjectStore } from '../stores/projects'
import { useNotificationStore } from '../stores/notifications'
import StatusBadge from '../components/common/StatusBadge.vue'
import BaseButton from '../components/common/BaseButton.vue'
import LoadingSpinner from '../components/common/LoadingSpinner.vue'
import EmptyState from '../components/common/EmptyState.vue'

const authStore = useAuthStore()
const projectStore = useProjectStore()
const notifStore = useNotificationStore()

const displayName = computed(() => authStore.displayName)
const recentProjects = computed(() => projectStore.projects.slice(0, 6))
const activeCount = computed(() =>
  projectStore.projects.filter(p => p.status === 'Active').length
)
const totalMembers = computed(() =>
  projectStore.projects.reduce((sum, p) => sum + (p.memberCount || 0), 0)
)

function formatDate(dateStr) {
  if (!dateStr) return '—'
  return new Date(dateStr).toLocaleDateString('vi-VN', {
    day: '2-digit', month: '2-digit', year: 'numeric'
  })
}

onMounted(() => {
  projectStore.loadProjects()
  notifStore.loadNotifications()
})
</script>

<style scoped>
.dashboard {
  animation: fadeIn 0.4s ease-out;
}

@keyframes fadeIn {
  from { opacity: 0; transform: translateY(12px); }
  to { opacity: 1; transform: translateY(0); }
}

.page-header {
  margin-bottom: var(--space-8);
}

.welcome-title {
  font-size: var(--font-size-2xl);
  font-weight: var(--font-weight-extrabold);
  background: var(--gradient-primary);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  margin-bottom: var(--space-1);
}

.subtitle {
  color: var(--text-secondary);
  font-size: var(--font-size-base);
  font-weight: 500;
}

/* Stats Styling */
.stats-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(240px, 1fr));
  gap: var(--space-5);
  margin-bottom: var(--space-8);
}

.stat-card {
  padding: var(--space-5) var(--space-6);
  display: flex;
  align-items: center;
  gap: var(--space-5);
  cursor: pointer;
}

.stat-icon {
  width: 52px;
  height: 52px;
  border-radius: var(--radius-lg);
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  flex-shrink: 0;
}

.icon-blue {
  background: linear-gradient(135deg, #3b82f6 0%, #1d4ed8 100%);
  box-shadow: 0 8px 20px rgba(59, 130, 246, 0.28);
}

.icon-emerald {
  background: linear-gradient(135deg, #10b981 0%, #047857 100%);
  box-shadow: 0 8px 20px rgba(16, 185, 129, 0.28);
}

.icon-amber {
  background: linear-gradient(135deg, #f59e0b 0%, #b45309 100%);
  box-shadow: 0 8px 20px rgba(245, 158, 11, 0.28);
}

.icon-violet {
  background: linear-gradient(135deg, #8b5cf6 0%, #5b21b6 100%);
  box-shadow: 0 8px 20px rgba(139, 92, 246, 0.28);
}

.stat-content {
  display: flex;
  flex-direction: column;
}

.stat-value {
  font-size: var(--font-size-2xl);
  font-weight: var(--font-weight-extrabold);
  line-height: 1.1;
  color: var(--text-primary);
}

.text-emerald { color: #10b981 !important; }
.text-amber { color: #f59e0b !important; }
.text-violet { color: #8b5cf6 !important; }

.stat-label {
  font-size: var(--font-size-sm);
  color: var(--text-secondary);
  font-weight: var(--font-weight-semibold);
  margin-top: 4px;
}

/* Sections Styling */
.section-card {
  border-radius: var(--radius-xl);
  overflow: hidden;
  box-shadow: var(--shadow-sm);
}

.section-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: var(--space-5) var(--space-6);
  border-bottom: 1px solid var(--border-color);
  background: rgba(0, 0, 0, 0.01);
}

.section-title-wrapper {
  display: flex;
  align-items: center;
  gap: var(--space-3);
}

.section-title {
  font-size: var(--font-size-lg);
  font-weight: var(--font-weight-bold);
  color: var(--text-primary);
}

.section-badge {
  font-size: 10px;
  padding: 3px 8px;
  background: var(--color-primary-light);
  color: var(--color-primary);
  font-weight: 700;
  border-radius: var(--radius-full);
}

[data-theme='dark'] .section-badge {
  background: rgba(59, 130, 246, 0.2);
  color: #60a5fa;
}

.view-all-link {
  display: flex;
  align-items: center;
  gap: 4px;
  font-size: var(--font-size-sm);
  font-weight: var(--font-weight-bold);
  color: var(--accent-primary);
  transition: all var(--transition-fast);
}

.view-all-link:hover {
  color: var(--accent-primary-hover);
  text-shadow: 0 0 8px rgba(37, 99, 235, 0.2);
}

.view-all-link svg {
  transition: transform var(--transition-fast);
}
.view-all-link:hover svg {
  transform: translateX(3px);
}

.section-body {
  padding: var(--space-6);
}

/* Project Cards Grid */
.project-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: var(--space-5);
}

.project-card {
  background: var(--bg-secondary);
  border: 1px solid var(--border-color);
  border-radius: var(--radius-lg);
  padding: var(--space-5);
  cursor: pointer;
  transition: all var(--transition-slow);
  display: flex;
  flex-direction: column;
}

.project-card:hover {
  border-color: rgba(99, 102, 241, 0.35);
  box-shadow: var(--shadow-md);
  transform: translateY(-4px) scale(1.01);
}

.project-card-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: var(--space-4);
}

.project-indicator {
  width: 14px;
  height: 14px;
  border-radius: 50%;
  box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
}

.project-name {
  font-size: var(--font-size-md);
  font-weight: var(--font-weight-bold);
  color: var(--text-primary);
  margin-bottom: var(--space-2);
}

.project-desc {
  font-size: var(--font-size-sm);
  color: var(--text-secondary);
  line-height: 1.5;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
  margin-bottom: var(--space-5);
  flex: 1;
}

.project-card-footer {
  border-top: 1px solid var(--border-color);
  padding-top: var(--space-4);
  display: flex;
  justify-content: space-between;
  gap: var(--space-3);
}

.meta-item {
  display: flex;
  align-items: center;
  gap: 6px;
  font-size: var(--font-size-xs);
  color: var(--text-muted);
  font-weight: 500;
}
</style>
