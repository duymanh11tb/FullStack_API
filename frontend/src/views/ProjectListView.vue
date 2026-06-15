<template>
  <div class="project-list-page">
    <div class="page-header">
      <div>
        <h1>Dự án</h1>
        <p class="text-secondary">Quản lý tất cả dự án của bạn</p>
      </div>
      <BaseButton variant="primary" @click="showCreateModal = true" id="btn-create-project">
        <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
          <line x1="12" y1="5" x2="12" y2="19" />
          <line x1="5" y1="12" x2="19" y2="12" />
        </svg>
        Tạo dự án
      </BaseButton>
    </div>

    <!-- Filter bar -->
    <div class="filter-bar">
      <div class="filter-tabs">
        <button
          :class="['filter-tab', { active: !myProjects }]"
          @click="myProjects = false; loadData()"
        >Tất cả</button>
        <button
          :class="['filter-tab', { active: myProjects }]"
          @click="myProjects = true; loadData()"
        >Dự án của tôi</button>
      </div>
    </div>

    <LoadingSpinner v-if="projectStore.loading" text="Đang tải dự án..." />

    <div v-else-if="projectStore.projects.length > 0" class="projects-grid">
      <div
        v-for="project in projectStore.projects"
        :key="project.id"
        class="project-card"
        @click="$router.push(`/projects/${project.id}`)"
      >
        <div class="card-top" :style="{ borderTopColor: project.color || '#2563EB' }">
          <div class="card-header">
            <div class="project-icon" :style="{ background: project.color || '#2563EB' }">
              {{ project.name.charAt(0).toUpperCase() }}
            </div>
            <StatusBadge :status="project.status" />
          </div>

          <h3 class="project-name">{{ project.name }}</h3>
          <p class="project-desc">{{ project.description || 'Không có mô tả' }}</p>

          <div class="card-footer">
            <div class="meta-row">
              <span class="meta-item">
                <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                  <path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2" />
                  <circle cx="9" cy="7" r="4" />
                </svg>
                {{ project.memberCount || 0 }}
              </span>
              <span class="meta-item">
                <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                  <circle cx="12" cy="12" r="10" />
                  <polyline points="12 6 12 12 16 14" />
                </svg>
                {{ project.sprintCount || 0 }} sprints
              </span>
            </div>
            <div class="meta-date">
              {{ formatDate(project.startDate) }}
              <template v-if="project.endDate"> — {{ formatDate(project.endDate) }}</template>
            </div>
          </div>
        </div>
      </div>
    </div>

    <EmptyState
      v-else
      title="Chưa có dự án nào"
      description="Bắt đầu bằng cách tạo dự án đầu tiên."
    >
      <BaseButton variant="primary" class="mt-4" @click="showCreateModal = true">
        Tạo dự án mới
      </BaseButton>
    </EmptyState>

    <!-- Create Project Modal -->
    <BaseModal v-model="showCreateModal" title="Tạo dự án mới">
      <form @submit.prevent="handleCreate" id="form-create-project">
        <div v-if="formError" class="alert alert-error">{{ formError }}</div>

        <BaseInput v-model="createForm.name" label="Tên dự án" placeholder="Ví dụ: Website Bán Hàng" required id="input-project-name" />
        <BaseInput v-model="createForm.description" label="Mô tả" type="textarea" placeholder="Mô tả ngắn gọn về dự án..." id="input-project-desc" />
        <BaseInput v-model="createForm.startDate" label="Ngày bắt đầu" type="date" required id="input-project-start" />
        <BaseInput v-model="createForm.endDate" label="Ngày kết thúc" type="date" id="input-project-end" />

        <div class="form-group">
          <label class="form-label">Màu sắc</label>
          <div class="color-picker">
            <button
              v-for="color in colors"
              :key="color"
              type="button"
              class="color-option"
              :class="{ active: createForm.color === color }"
              :style="{ background: color }"
              @click="createForm.color = color"
            ></button>
          </div>
        </div>
      </form>

      <template #footer>
        <BaseButton variant="secondary" @click="showCreateModal = false">Hủy</BaseButton>
        <BaseButton variant="primary" :loading="creating" @click="handleCreate">Tạo dự án</BaseButton>
      </template>
    </BaseModal>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { useProjectStore } from '../stores/projects'
import StatusBadge from '../components/common/StatusBadge.vue'
import BaseButton from '../components/common/BaseButton.vue'
import BaseModal from '../components/common/BaseModal.vue'
import BaseInput from '../components/common/BaseInput.vue'
import LoadingSpinner from '../components/common/LoadingSpinner.vue'
import EmptyState from '../components/common/EmptyState.vue'

const projectStore = useProjectStore()
const showCreateModal = ref(false)
const creating = ref(false)
const formError = ref('')
const myProjects = ref(false)

const colors = ['#2563EB', '#7C3AED', '#DC2626', '#D97706', '#16A34A', '#0891B2', '#DB2777', '#4F46E5']

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

function loadData() {
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
  animation: fadeIn 0.3s ease;
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

.filter-bar {
  margin-bottom: var(--space-6);
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
  background: var(--color-white);
  color: var(--color-text-primary);
  box-shadow: var(--shadow-xs);
}

.projects-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(320px, 1fr));
  gap: var(--space-4);
}

.project-card {
  background: var(--color-white);
  border: 1px solid var(--color-border);
  border-radius: var(--radius-lg);
  overflow: hidden;
  cursor: pointer;
  transition: all var(--transition-fast);
}

.project-card:hover {
  border-color: var(--color-primary);
  box-shadow: var(--shadow-md);
  transform: translateY(-2px);
}

.card-top {
  padding: var(--space-5);
  border-top: 3px solid;
}

.card-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: var(--space-3);
}

.project-icon {
  width: 36px;
  height: 36px;
  border-radius: var(--radius-lg);
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-weight: var(--font-weight-bold);
  font-size: var(--font-size-md);
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

.card-footer {
  padding-top: var(--space-3);
  border-top: 1px solid var(--color-border);
}

.meta-row {
  display: flex;
  gap: var(--space-4);
  margin-bottom: var(--space-2);
}

.meta-item {
  display: flex;
  align-items: center;
  gap: 4px;
  font-size: var(--font-size-xs);
  color: var(--color-text-tertiary);
}

.meta-date {
  font-size: var(--font-size-xs);
  color: var(--color-text-tertiary);
}

/* Color picker */
.color-picker {
  display: flex;
  gap: var(--space-2);
  margin-top: var(--space-1);
}

.color-option {
  width: 28px;
  height: 28px;
  border-radius: var(--radius-full);
  border: 2px solid transparent;
  transition: all var(--transition-fast);
  cursor: pointer;
}

.color-option:hover {
  transform: scale(1.1);
}

.color-option.active {
  border-color: var(--color-text-primary);
  box-shadow: 0 0 0 2px var(--color-white), 0 0 0 4px currentColor;
}

.form-group {
  margin-bottom: var(--space-4);
}

.form-label {
  display: block;
  font-size: var(--font-size-sm);
  font-weight: var(--font-weight-medium);
  color: var(--color-text-primary);
  margin-bottom: var(--space-1);
}

.alert {
  padding: var(--space-3) var(--space-4);
  border-radius: var(--radius-md);
  font-size: var(--font-size-sm);
  margin-bottom: var(--space-4);
}

.alert-error {
  background: var(--color-danger-light);
  color: var(--color-danger);
}
</style>
