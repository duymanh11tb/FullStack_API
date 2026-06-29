<template>
  <div class="project-list-page">
    <div class="page-header">
      <div class="header-main">
        <h1>Dự án</h1>
        <p class="subtitle">Quản lý và cộng tác trên tất cả dự án hệ thống</p>
      </div>
      <BaseButton v-if="isAdmin || isManager" variant="primary" @click="showCreateModal = true" id="btn-create-project">
        <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
          <line x1="12" y1="5" x2="12" y2="19" />
          <line x1="5" y1="12" x2="19" y2="12" />
        </svg>
        Tạo dự án mới
      </BaseButton>
    </div>

    <!-- Filter bar -->
    <div class="filter-bar">
      <div class="filter-tabs">
        <button
          :class="['filter-tab', { active: !myProjects }]"
          @click="myProjects = false; loadData()"
        >Tất cả dự án</button>
        <button
          :class="['filter-tab', { active: myProjects }]"
          @click="myProjects = true; loadData()"
        >Dự án của tôi</button>
      </div>
    </div>

    <SkeletonLoader v-if="projectStore.loading" type="card" :count="6" />

    <div v-else-if="projectStore.projects.length > 0" class="projects-grid-container">
      <div class="projects-grid">
        <div
          v-for="project in paginatedProjects"
          :key="project.id"
          class="project-card glass-panel tilt-card glossy-reflection"
          :style="{ borderTop: `4px solid ${project.color || '#2563EB'}` }"
          @mousemove="onMouseMove"
          @mouseleave="onMouseLeave"
          @click="$router.push(`/projects/${project.id}`)"
        >
          <div class="card-header popout-3d">
            <div class="project-icon" :style="{ background: project.color || '#2563EB' }">
              {{ project.name.charAt(0).toUpperCase() }}
            </div>
            <StatusBadge :status="project.status" />
          </div>

          <h3 class="project-name popout-3d">{{ project.name }}</h3>
          <p class="project-desc popout-3d">{{ project.description || 'Không có mô tả cho dự án này.' }}</p>

          <div class="card-footer popout-3d">
            <div class="meta-row">
              <span class="meta-item" title="Thành viên">
                <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                  <path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2" />
                  <circle cx="9" cy="7" r="4" />
                </svg>
                {{ project.memberCount || 0 }} thành viên
              </span>
              <span class="meta-item" title="Sprints">
                <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                  <circle cx="12" cy="12" r="10" />
                  <polyline points="12 6 12 12 16 14" />
                </svg>
                {{ project.sprintCount || 0 }} sprints
              </span>
            </div>
            <div class="meta-date">
              <span>{{ formatDate(project.startDate) }}</span>
              <span v-if="project.endDate"> — {{ formatDate(project.endDate) }}</span>
            </div>
          </div>
        </div>
      </div>

      <!-- Reusable Pagination Component -->
      <BasePagination
        :currentPage="currentPage"
        @update:currentPage="currentPage = $event"
        :totalItems="projectStore.projects.length"
        :itemsPerPage="itemsPerPage"
      />
    </div>

    <EmptyState
      v-else
      title="Chưa có dự án nào"
      description="Không tìm thấy dự án nào khớp với bộ lọc hoặc bạn chưa được thêm vào dự án nào."
    >
      <BaseButton v-if="isAdmin || isManager" variant="primary" class="mt-4" @click="showCreateModal = true">
        <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" style="margin-right: 4px;">
          <line x1="12" y1="5" x2="12" y2="19" />
          <line x1="5" y1="12" x2="19" y2="12" />
        </svg>
        Tạo dự án ngay
      </BaseButton>
    </EmptyState>

    <!-- Create Project Modal -->
    <BaseModal v-model="showCreateModal" title="Tạo dự án mới">
      <form @submit.prevent="handleCreate" id="form-create-project">
        <div v-if="formError" class="alert alert-error">{{ formError }}</div>

        <BaseInput v-model="createForm.name" label="Tên dự án" placeholder="Ví dụ: Hệ thống quản lý kho..." required id="input-project-name" />
        <BaseInput v-model="createForm.description" label="Mô tả dự án" type="textarea" placeholder="Nhập mô tả ngắn gọn về mục tiêu dự án..." id="input-project-desc" />
        
        <div class="form-dates-row">
          <BaseInput v-model="createForm.startDate" label="Ngày bắt đầu" type="date" required id="input-project-start" />
          <BaseInput v-model="createForm.endDate" label="Ngày kết thúc (không bắt buộc)" type="date" id="input-project-end" />
        </div>

        <div class="form-group color-picker-group">
          <label class="form-label">Tông màu chủ đề dự án</label>
          <div class="color-picker">
            <button
              v-for="color in colors"
              :key="color"
              type="button"
              class="color-option"
              :class="{ active: createForm.color === color }"
              :style="{ background: color, color: color }"
              @click="createForm.color = color"
            ></button>
          </div>
        </div>
      </form>

      <template #footer>
        <BaseButton variant="secondary" @click="showCreateModal = false">Hủy bỏ</BaseButton>
        <BaseButton variant="primary" :loading="creating" @click="handleCreate">Tạo dự án</BaseButton>
      </template>
    </BaseModal>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted, computed } from 'vue'
import { useProjectStore } from '../stores/projects'
import { useAuthStore } from '../stores/auth'
import StatusBadge from '../components/common/StatusBadge.vue'
import BaseButton from '../components/common/BaseButton.vue'
import BaseModal from '../components/common/BaseModal.vue'
import BaseInput from '../components/common/BaseInput.vue'
import LoadingSpinner from '../components/common/LoadingSpinner.vue'
import EmptyState from '../components/common/EmptyState.vue'
import BasePagination from '../components/common/BasePagination.vue'
import SkeletonLoader from '../components/common/SkeletonLoader.vue'

const projectStore = useProjectStore()
const authStore = useAuthStore()
const isAdmin = computed(() => authStore.isAdmin)
const isManager = computed(() => authStore.isManager)
const showCreateModal = ref(false)
const creating = ref(false)
const formError = ref('')
const myProjects = ref(false)

const currentPage = ref(1)
const itemsPerPage = ref(6) // 6 projects per page

const paginatedProjects = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage.value
  const end = start + itemsPerPage.value
  return projectStore.projects.slice(start, end)
})

const colors = ['#2563EB', '#7C3AED', '#DC2626', '#D97706', '#10B981', '#0891B2', '#DB2777', '#4F46E5']

const createForm = reactive({
  name: '',
  description: '',
  startDate: new Date().toISOString().split('T')[0],
  endDate: '',
  color: '#2563EB'
})

function formatDate(dateStr) {
  if (!dateStr) return '—'
  return new Date(dateStr).toLocaleDateString('vi-VN', {
    day: '2-digit', month: '2-digit', year: 'numeric'
  })
}

function onMouseMove(e) {
  const card = e.currentTarget
  const rect = card.getBoundingClientRect()
  const x = e.clientX - rect.left
  const y = e.clientY - rect.top
  const xc = rect.width / 2
  const yc = rect.height / 2
  const dx = x - xc
  const dy = y - yc
  
  // 3D rotation angle limits (e.g. max 8 degrees)
  const rx = -(dy / yc) * 8
  const ry = (dx / xc) * 8
  
  card.style.setProperty('--rx', `${rx}deg`)
  card.style.setProperty('--ry', `${ry}deg`)
}

function onMouseLeave(e) {
  const card = e.currentTarget
  card.style.setProperty('--rx', '0deg')
  card.style.setProperty('--ry', '0deg')
}

function loadData() {
  currentPage.value = 1
  projectStore.loadProjects(myProjects.value)
}

async function handleCreate() {
  formError.value = ''
  creating.value = true
  try {
    const payload = {
      name: createForm.name,
      description: createForm.description || null,
      startDate: new Date(createForm.startDate).toISOString(),
      endDate: createForm.endDate ? new Date(createForm.endDate).toISOString() : null,
      color: createForm.color
    }
    await projectStore.createProject(payload)
    showCreateModal.value = false
    // Reset form
    createForm.name = ''
    createForm.description = ''
    createForm.startDate = new Date().toISOString().split('T')[0]
    createForm.endDate = ''
    createForm.color = '#2563EB'
  } catch (err) {
    formError.value = err
  } finally {
    creating.value = false
  }
}

onMounted(() => {
  loadData()
})
</script>

<style scoped>
.project-list-page {
  animation: fadeIn 0.4s ease-out;
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

.filter-bar {
  margin-bottom: var(--space-6);
}

.filter-tabs {
  display: flex;
  gap: 4px;
  background: var(--color-bg-secondary);
  border-radius: var(--radius-lg);
  padding: 4px;
  width: fit-content;
}

[data-theme='dark'] .filter-tabs {
  background: rgba(15, 23, 42, 0.4);
}

.filter-tab {
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

/* Projects Cards Grid */
.projects-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(320px, 1fr));
  gap: var(--space-5);
}

.project-card {
  padding: var(--space-5);
  cursor: pointer;
  display: flex;
  flex-direction: column;
  transition: all var(--transition-slow);
}

.project-card:hover {
  transform: translateY(-4px);
}

.card-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: var(--space-4);
}

.project-icon {
  width: 42px;
  height: 42px;
  border-radius: var(--radius-md);
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-weight: var(--font-weight-extrabold);
  font-size: var(--font-size-lg);
  box-shadow: var(--shadow-sm);
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

.card-footer {
  padding-top: var(--space-4);
  border-top: 1px solid var(--border-color);
  display: flex;
  flex-direction: column;
  gap: var(--space-2);
}

.meta-row {
  display: flex;
  gap: var(--space-4);
}

.meta-item {
  display: flex;
  align-items: center;
  gap: 6px;
  font-size: var(--font-size-xs);
  color: var(--text-muted);
  font-weight: 500;
}

.meta-date {
  font-size: var(--font-size-xs);
  color: var(--text-muted);
  font-weight: 500;
}

/* Modals styles */
.form-dates-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: var(--space-4);
}

@media (max-width: 576px) {
  .form-dates-row {
    grid-template-columns: 1fr;
    gap: 0;
  }
}

.color-picker-group {
  margin-top: var(--space-4);
}

.form-label {
  display: block;
  font-size: var(--font-size-sm);
  font-weight: var(--font-weight-semibold);
  color: var(--color-text-primary);
  margin-bottom: var(--space-2);
}

.color-picker {
  display: flex;
  flex-wrap: wrap;
  gap: 10px;
}

.color-option {
  width: 32px;
  height: 32px;
  border-radius: var(--radius-full);
  border: 2px solid transparent;
  transition: all var(--transition-fast);
  cursor: pointer;
}

.color-option:hover {
  transform: scale(1.15);
}

.color-option.active {
  border-color: var(--color-text-primary);
  box-shadow: 0 0 0 2px var(--bg-white-to-card), 0 0 0 4px currentColor;
}

.alert {
  padding: var(--space-3) var(--space-4);
  border-radius: var(--radius-md);
  font-size: var(--font-size-sm);
  margin-bottom: var(--space-4);
  font-weight: var(--font-weight-medium);
}

.alert-error {
  background: var(--color-danger-light);
  color: var(--color-danger);
  border: 1px solid rgba(225, 29, 72, 0.2);
}
</style>
