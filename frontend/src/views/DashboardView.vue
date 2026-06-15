<template>
  <div class="dashboard">
    <div class="page-header">
      <div>
        <h1>Xin chào, {{ displayName }}!</h1>
        <p class="text-secondary">Đây là tổng quan hoạt động dự án của bạn.</p>
      </div>
    </div>

    <!-- Stats Grid -->
    <div class="stats-grid">
      <div class="stat-card">
        <div class="stat-icon stat-icon-blue">
          <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M22 19a2 2 0 0 1-2 2H4a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h5l2 3h9a2 2 0 0 1 2 2z" />
          </svg>
        </div>
        <div class="stat-content">
          <span class="stat-value">{{ projectStore.projectCount }}</span>
          <span class="stat-label">Tổng dự án</span>
        </div>
      </div>

      <div class="stat-card">
        <div class="stat-icon stat-icon-green">
          <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <polyline points="20 6 9 17 4 12" />
          </svg>
        </div>
        <div class="stat-content">
          <span class="stat-value">{{ activeCount }}</span>
          <span class="stat-label">Đang hoạt động</span>
        </div>
      </div>

      <div class="stat-card">
        <div class="stat-icon stat-icon-orange">
          <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M18 8A6 6 0 0 0 6 8c0 7-3 9-3 9h18s-3-2-3-9" />
            <path d="M13.73 21a2 2 0 0 1-3.46 0" />
          </svg>
        </div>
        <div class="stat-content">
          <span class="stat-value">{{ notifStore.unreadCount }}</span>
          <span class="stat-label">Thông báo chưa đọc</span>
        </div>
      </div>

      <div class="stat-card">
        <div class="stat-icon stat-icon-purple">
          <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2" />
            <circle cx="9" cy="7" r="4" />
            <path d="M23 21v-2a4 4 0 0 0-3-3.87" />
            <path d="M16 3.13a4 4 0 0 1 0 7.75" />
          </svg>
        </div>
        <div class="stat-content">
          <span class="stat-value">{{ totalMembers }}</span>
          <span class="stat-label">Thành viên</span>
        </div>
      </div>
    </div>

    <!-- Recent Projects -->
    <div class="section">
      <div class="section-header">
        <h2>Dự án gần đây</h2>
        <router-link to="/projects" class="view-all-link">
          Xem tất cả
          <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <polyline points="9 18 15 12 9 6" />
          </svg>
        </router-link>
      </div>

      <LoadingSpinner v-if="projectStore.loading" text="Đang tải dự án..." />

      <div v-else-if="projectStore.projects.length > 0" class="project-grid">
        <div
          v-for="project in recentProjects"
          :key="project.id"
          class="project-card"
          @click="$router.push(`/projects/${project.id}`)"
        >
          <div class="project-card-header">
            <div class="project-color" :style="{ background: project.color || '#2563EB' }"></div>
            <StatusBadge :status="project.status" />
          </div>
          <h3 class="project-name">{{ project.name }}</h3>
          <p class="project-desc">{{ project.description || 'Không có mô tả' }}</p>
          <div class="project-meta">
            <span class="meta-item">
              <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2" />
                <circle cx="9" cy="7" r="4" />
              </svg>
              {{ project.memberCount || 0 }} thành viên
            </span>
            <span class="meta-item">
              <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <rect x="3" y="4" width="18" height="18" rx="2" ry="2" />
                <line x1="16" y1="2" x2="16" y2="6" />
                <line x1="8" y1="2" x2="8" y2="6" />
                <line x1="3" y1="10" x2="21" y2="10" />
              </svg>
              {{ formatDate(project.startDate) }}
            </span>
          </div>
        </div>
      </div>

      <EmptyState
        v-else
        title="Chưa có dự án nào"
        description="Tạo dự án đầu tiên để bắt đầu quản lý công việc."
      >
        <BaseButton variant="primary" class="mt-4" @click="$router.push('/projects')">
          Tạo dự án
        </BaseButton>
      </EmptyState>
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
  animation: fadeIn 0.3s ease;
}

@keyframes fadeIn {
  from { opacity: 0; transform: translateY(8px); }
  to { opacity: 1; transform: translateY(0); }
}

.page-header {
  margin-bottom: var(--space-8);
}

.page-header h1 {
  font-size: var(--font-size-2xl);
  margin-bottom: var(--space-1);
}

.page-header p {
  font-size: var(--font-size-base);
}

/* Stats */
.stats-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(220px, 1fr));
  gap: var(--space-4);
  margin-bottom: var(--space-8);
}

.stat-card {
  background: var(--color-white);
  border: 1px solid var(--color-border);
  border-radius: var(--radius-lg);
  padding: var(--space-5);
  display: flex;
  align-items: center;
  gap: var(--space-4);
  transition: box-shadow var(--transition-fast);
}

.stat-card:hover {
  box-shadow: var(--shadow-md);
}

.stat-icon {
  width: 48px;
  height: 48px;
  border-radius: var(--radius-lg);
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

.stat-icon-blue { background: var(--color-primary-light); color: var(--color-primary); }
.stat-icon-green { background: var(--color-success-light); color: var(--color-success); }
.stat-icon-orange { background: var(--color-warning-light); color: var(--color-warning); }
.stat-icon-purple { background: #EDE9FE; color: #7C3AED; }

.stat-content {
  display: flex;
  flex-direction: column;
}

.stat-value {
  font-size: var(--font-size-2xl);
  font-weight: var(--font-weight-bold);
  line-height: 1;
}

.stat-label {
  font-size: var(--font-size-sm);
  color: var(--color-text-secondary);
  margin-top: var(--space-1);
}

/* Section */
.section {
  background: var(--color-white);
  border: 1px solid var(--color-border);
  border-radius: var(--radius-lg);
  overflow: hidden;
}

.section-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: var(--space-5) var(--space-6);
  border-bottom: 1px solid var(--color-border);
}

.section-header h2 {
  font-size: var(--font-size-lg);
}

.view-all-link {
  display: flex;
  align-items: center;
  gap: var(--space-1);
  font-size: var(--font-size-sm);
  font-weight: var(--font-weight-medium);
}

/* Project Grid */
.project-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(320px, 1fr));
  gap: var(--space-4);
  padding: var(--space-6);
}

.project-card {
  border: 1px solid var(--color-border);
  border-radius: var(--radius-lg);
  padding: var(--space-5);
  cursor: pointer;
  transition: all var(--transition-fast);
}

.project-card:hover {
  border-color: var(--color-primary);
  box-shadow: var(--shadow-md);
  transform: translateY(-1px);
}

.project-card-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: var(--space-3);
}

.project-color {
  width: 12px;
  height: 12px;
  border-radius: var(--radius-full);
}

.project-name {
  font-size: var(--font-size-md);
  font-weight: var(--font-weight-semibold);
  margin-bottom: var(--space-2);
}

.project-desc {
  font-size: var(--font-size-sm);
  color: var(--color-text-secondary);
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
  margin-bottom: var(--space-4);
  line-height: var(--line-height-relaxed);
}

.project-meta {
  display: flex;
  gap: var(--space-4);
}

.meta-item {
  display: flex;
  align-items: center;
  gap: var(--space-1);
  font-size: var(--font-size-xs);
  color: var(--color-text-tertiary);
}
</style>
