<template>
  <div v-if="isOpen" class="modal-overlay" @click.self="closeModal">
    <div class="modal-content glass-panel" :class="{ 'modal-large': isEdit }">
      <!-- Modal Header -->
      <div class="modal-header">
        <div class="header-left-side" v-if="isEdit">
          <!-- Breadcrumbs: Project Name / Task Code -->
          <div class="modal-breadcrumbs">
            <span class="project-crumb">{{ projectName }}</span>
            <span class="divider">/</span>
            <span class="task-crumb">{{ taskCode }}</span>
          </div>
        </div>
        <h2 v-else class="create-title">Tạo tác vụ mới</h2>
        <button class="close-btn" @click="closeModal">&times;</button>
      </div>
      
      <!-- Modal Body (Two columns in Edit Mode) -->
      <div class="modal-body" :class="{ 'edit-mode-grid': isEdit }">
        <!-- 1. LEFT COLUMN: Task Main Info (Title, Description, Subtasks) -->
        <div class="task-left-section">
          <form @submit.prevent="submitForm" class="task-form">
            <!-- Project Selection (For Create Mode only, disabled in Edit) -->
            <div class="form-group" v-if="!isEdit">
              <label for="project">Dự án (Project)</label>
              <select id="project" v-model="formData.boardId" required>
                <option value="" disabled>Chọn dự án...</option>
                <option v-for="p in projectStore.projects" :key="p.id" :value="p.id">
                  {{ p.name }}
                </option>
              </select>
            </div>

            <!-- Task Title (Large style in Edit mode) -->
            <div class="form-group title-group">
              <label for="title" v-if="!isEdit">Tiêu đề</label>
              <input 
                id="title" 
                v-model="formData.title" 
                type="text" 
                required 
                placeholder="Tên tác vụ..." 
                :class="{ 'title-large': isEdit }"
              />
            </div>
            
            <!-- Description -->
            <div class="form-group desc-group">
              <label for="description">Mô tả tác vụ</label>
              <textarea 
                id="description" 
                v-model="formData.description" 
                rows="4" 
                placeholder="Thêm mô tả chi tiết cho công việc này..."
              ></textarea>
            </div>
            
            <!-- Subtasks checklist -->
            <div class="form-group subtasks-group">
              <label>Công việc phụ (Subtasks)</label>
              
              <!-- Input box to add subtasks -->
              <div class="subtask-input-wrapper">
                <input 
                  type="text" 
                  v-model="newSubtaskTitle" 
                  placeholder="Thêm việc phụ và nhấn Enter..." 
                  @keydown.enter.prevent="addSubtask" 
                />
                <button type="button" class="btn-add-subtask" @click="addSubtask">Thêm</button>
              </div>

              <!-- Subtasks Progress line -->
              <div v-if="totalSubTasksCount > 0" class="subtasks-progress">
                <div class="progress-info">
                  <span>Tiến độ: {{ completedSubTasksCount }}/{{ totalSubTasksCount }} việc phụ</span>
                  <span>{{ subTasksProgressPercent }}%</span>
                </div>
                <div class="progress-track">
                  <div class="progress-fill" :style="{ width: subTasksProgressPercent + '%' }"></div>
                </div>
              </div>

              <!-- List of subtasks -->
              <div class="subtask-list" v-if="totalSubTasksCount > 0">
                <div 
                  v-for="(st, idx) in formData.subTasks" 
                  :key="st.subTaskId" 
                  class="subtask-item"
                >
                  <label class="subtask-checkbox-label">
                    <input type="checkbox" v-model="st.isCompleted" />
                    <span class="custom-checkbox"></span>
                    <span :class="['subtask-title-text', { 'line-through': st.isCompleted }]">
                      {{ st.title }}
                    </span>
                  </label>
                  <button type="button" class="btn-remove-subtask" @click="removeSubtask(idx)" title="Xóa">
                    &times;
                  </button>
                </div>
              </div>

              <!-- Auto update parent task status toggle -->
              <div v-if="totalSubTasksCount > 0" class="auto-sync-toggle">
                <label class="toggle-label">
                  <input type="checkbox" v-model="autoTransition" />
                  <span class="toggle-text">Tự động hoàn thành task khi xong hết việc phụ</span>
                </label>
              </div>
            </div>

            <!-- Standard attributes form layout for Create Mode only -->
            <div class="create-mode-attributes" v-if="!isEdit">
              <div class="form-row">
                <div class="form-group">
                  <label for="assignee">Người phụ trách</label>
                  <div class="assignee-select-wrapper">
                    <select id="assignee" v-model="formData.assigneeId">
                      <option value="">Chưa giao việc (None)</option>
                      <option v-for="m in projectMembers" :key="m.userId || m.id" :value="m.userId || m.id">
                        {{ m.displayName }}
                      </option>
                    </select>
                    <button type="button" class="btn-assign-me" @click="assignToMe">Tôi</button>
                  </div>
                </div>
                
                <div class="form-group">
                  <label for="priority">Độ ưu tiên</label>
                  <select id="priority" v-model="formData.priority">
                    <option :value="0">Thấp (Low)</option>
                    <option :value="1">Trung bình (Medium)</option>
                    <option :value="2">Cao (High)</option>
                    <option :value="3">Khẩn cấp (Urgent)</option>
                  </select>
                </div>
              </div>

              <div class="form-row">
                <div class="form-group">
                  <label for="status">Trạng thái</label>
                  <select id="status" v-model="formData.currentStatus">
                    <option :value="0">Cần làm (To Do)</option>
                    <option :value="1">Đang làm (In Progress)</option>
                    <option :value="2">Đánh giá (Review)</option>
                    <option :value="3">Hoàn thành (Done)</option>
                  </select>
                </div>

                <div class="form-group">
                  <label for="deadline">Hạn chót</label>
                  <input id="deadline" v-model="formData.deadline" type="datetime-local" />
                </div>
              </div>
            </div>
            
            <!-- Main actions buttons -->
            <div class="modal-actions">
              <button v-if="isEdit" type="button" class="btn-delete" @click="deleteTask" :disabled="loading">
                Xóa tác vụ
              </button>
              <button type="button" class="btn-cancel" @click="closeModal">Hủy</button>
              <button type="submit" class="btn-submit" :disabled="loading">
                {{ loading ? 'Đang lưu...' : (isEdit ? 'Lưu thay đổi' : 'Tạo tác vụ') }}
              </button>
            </div>
          </form>
        </div>

        <!-- 2. RIGHT COLUMN: Properties Sidebar & Activity Feed (Edit Mode only) -->
        <div class="task-right-section" v-if="isEdit">
          <!-- Properties Panel Card -->
          <div class="properties-panel-card glass-panel">
            <h3 class="panel-section-title">Thuộc tính tác vụ</h3>
            <div class="properties-grid">
              <!-- Assignee property -->
              <div class="prop-item">
                <span class="prop-label">
                  <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
                    <path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2" />
                    <circle cx="12" cy="7" r="4" />
                  </svg>
                  Phân công
                </span>
                <div class="prop-value">
                  <div class="assignee-select-wrapper">
                    <select v-model="formData.assigneeId">
                      <option value="">Chưa phân công</option>
                      <option v-for="m in projectMembers" :key="m.userId || m.id" :value="m.userId || m.id">
                        {{ m.displayName }}
                      </option>
                    </select>
                    <button type="button" class="btn-assign-me-tiny" @click="assignToMe" title="Giao cho tôi">Tôi</button>
                  </div>
                </div>
              </div>

              <!-- Status property -->
              <div class="prop-item">
                <span class="prop-label">
                  <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
                    <circle cx="12" cy="12" r="10"/><polyline points="12 6 12 12 16 14"/>
                  </svg>
                  Trạng thái
                </span>
                <div class="prop-value">
                  <select v-model="formData.currentStatus">
                    <option :value="0">Cần làm (To Do)</option>
                    <option :value="1">Đang làm (In Progress)</option>
                    <option :value="2">Đánh giá (Review)</option>
                    <option :value="3">Hoàn thành (Done)</option>
                  </select>
                </div>
              </div>

              <!-- Priority property -->
              <div class="prop-item">
                <span class="prop-label">
                  <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
                    <polygon points="12 2 15.09 8.26 22 9.27 17 14.14 18.18 21.02 12 17.77 5.82 21.02 7 14.14 2 9.27 8.91 8.26 12 2"/>
                  </svg>
                  Độ ưu tiên
                </span>
                <div class="prop-value">
                  <select v-model="formData.priority" :class="`prio-select-${formData.priority}`">
                    <option :value="0">Thấp (Low)</option>
                    <option :value="1">Trung bình (Medium)</option>
                    <option :value="2">Cao (High)</option>
                    <option :value="3">Khẩn cấp (Urgent)</option>
                  </select>
                </div>
              </div>

              <!-- Deadline property -->
              <div class="prop-item">
                <span class="prop-label">
                  <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
                    <rect x="3" y="4" width="18" height="18" rx="2" ry="2" />
                    <line x1="16" y1="2" x2="16" y2="6" />
                    <line x1="8" y1="2" x2="8" y2="6" />
                    <line x1="3" y1="10" x2="21" y2="10" />
                  </svg>
                  Hạn chót
                </span>
                <div class="prop-value">
                  <input v-model="formData.deadline" type="datetime-local" class="prop-deadline-input" />
                  <div class="quick-dates-tiny">
                    <button type="button" @click="setDeadlineDays(0)">Hôm nay</button>
                    <button type="button" @click="setDeadlineDays(3)">3 ngày</button>
                    <button type="button" @click="setDeadlineDays(7)">1 tuần</button>
                  </div>
                </div>
              </div>

              <!-- Color label property -->
              <div class="prop-item">
                <span class="prop-label">
                  <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
                    <path d="M12 2L2 7l10 5 10-5-10-5zM2 17l10 5 10-5M2 12l10 5 10-5" />
                  </svg>
                  Nhãn màu
                </span>
                <div class="prop-value">
                  <input v-model="formData.colorLabel" type="color" class="prop-color-input" />
                </div>
              </div>
            </div>
          </div>

          <!-- Activity log tabs & Feed -->
          <div class="activity-feed-container">
            <div class="task-modal-tabs">
              <button 
                type="button" 
                class="tab-btn" 
                :class="{ active: activeTab === 'discussion' }"
                @click="activeTab = 'discussion'"
              >
                Thảo luận
              </button>
              <button 
                type="button" 
                class="tab-btn" 
                :class="{ active: activeTab === 'activities' }"
                @click="activeTab = 'activities'"
              >
                Nhật ký hoạt động
              </button>
            </div>
            
            <div class="feed-body-pane">
              <div v-if="activeTab === 'discussion'" class="tab-pane">
                <TaskDiscussion :task-id="formData.taskId" />
              </div>
              <div v-else-if="activeTab === 'activities'" class="tab-pane">
                <ActivityLogList :task-id="formData.taskId" />
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, watch, computed, onMounted } from 'vue';
import TaskDiscussion from './task/TaskDiscussion.vue';
import ActivityLogList from './common/ActivityLogList.vue';
import { useProjectStore } from '../stores/projects';
import { useAuthStore } from '../stores/auth';
import { getMembers } from '../api/projectApi';

const props = defineProps({
  isOpen: Boolean,
  task: Object,
  loading: Boolean
});

const emit = defineEmits(['close', 'save', 'delete']);

const isEdit = ref(false);
const activeTab = ref('discussion');
const projectStore = useProjectStore();
const authStore = useAuthStore();

const projectMembers = ref([]);
const newSubtaskTitle = ref('');
const autoTransition = ref(true);

const formData = ref({
  taskId: '',
  boardId: '',
  title: '',
  description: '',
  priority: 1,
  currentStatus: 0,
  deadline: '',
  colorLabel: '#6366f1',
  assigneeId: '',
  subTasks: []
});

onMounted(async () => {
  await projectStore.loadProjects();
});

// Dynamic Breadcrumbs
const projectName = computed(() => {
  const p = projectStore.projects.find(proj => proj.id === formData.value.boardId);
  return p ? p.name : 'Dự án';
});

const taskCode = computed(() => {
  if (formData.value.taskId) {
    const idStr = String(formData.value.taskId);
    const short = idStr.substring(idStr.length - 4).toUpperCase();
    return `T-${short}`;
  }
  return 'T-TASK';
});

watch(() => formData.value.boardId, async (newVal) => {
  if (newVal) {
    try {
      const res = await getMembers(newVal);
      projectMembers.value = res.data?.data || res.data || [];
    } catch (err) {
      console.error('Failed to load project members:', err);
      projectMembers.value = [];
    }
  } else {
    projectMembers.value = [];
  }
}, { immediate: true });

function assignToMe() {
  const userId = authStore.user?.id || authStore.user?.userId;
  if (userId) {
    formData.value.assigneeId = userId;
  }
}

function setDeadlineDays(days) {
  const d = new Date();
  d.setDate(d.getDate() + days);
  d.setMinutes(d.getMinutes() - d.getTimezoneOffset());
  formData.value.deadline = d.toISOString().slice(0, 16);
}

function generateUUID() {
  if (typeof crypto !== 'undefined' && typeof crypto.randomUUID === 'function') {
    return crypto.randomUUID();
  }
  return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
    const r = Math.random() * 16 | 0;
    const v = c === 'x' ? r : (r & 0x3 | 0x8);
    return v.toString(16);
  });
}

function addSubtask() {
  if (!newSubtaskTitle.value.trim()) return;
  if (!formData.value.subTasks) {
    formData.value.subTasks = [];
  }
  
  formData.value.subTasks.push({
    subTaskId: generateUUID(),
    title: newSubtaskTitle.value.trim(),
    isCompleted: false,
    createdAt: new Date().toISOString()
  });
  newSubtaskTitle.value = '';
}

function removeSubtask(index) {
  formData.value.subTasks.splice(index, 1);
}

const completedSubTasksCount = computed(() => {
  if (!formData.value.subTasks) return 0;
  return formData.value.subTasks.filter(st => st.isCompleted).length;
});

const totalSubTasksCount = computed(() => {
  if (!formData.value.subTasks) return 0;
  return formData.value.subTasks.length;
});

const subTasksProgressPercent = computed(() => {
  if (totalSubTasksCount.value === 0) return 0;
  return Math.round((completedSubTasksCount.value / totalSubTasksCount.value) * 100);
});

watch(() => formData.value.subTasks, (newSubTasks) => {
  if (!autoTransition.value || !newSubTasks || newSubTasks.length === 0) return;
  const allCompleted = newSubTasks.every(st => st.isCompleted);
  if (allCompleted && formData.value.currentStatus !== 3) {
    formData.value.currentStatus = 3; // Status 3 maps to Done
  } else if (!allCompleted && formData.value.currentStatus === 3) {
    formData.value.currentStatus = 1; // Status 1 maps to In Progress
  }
}, { deep: true });

watch(() => props.isOpen, (newVal) => {
  if (newVal) {
    projectStore.loadProjects();
    activeTab.value = 'discussion';
    if (props.task) {
      isEdit.value = true;
      let deadlineStr = '';
      if (props.task.deadline) {
        const d = new Date(props.task.deadline);
        d.setMinutes(d.getMinutes() - d.getTimezoneOffset());
        deadlineStr = d.toISOString().slice(0, 16);
      }
      
      formData.value = {
        taskId: props.task.taskId,
        boardId: props.task.boardId || '',
        title: props.task.title || '',
        description: props.task.description || '',
        priority: props.task.priority ?? 1,
        currentStatus: props.task.currentStatus ?? 0,
        deadline: deadlineStr,
        colorLabel: props.task.colorLabel || '#6366f1',
        assigneeId: props.task.assigneeId || '',
        subTasks: props.task.subTasks ? [...props.task.subTasks] : []
      };
    } else {
      isEdit.value = false;
      formData.value = {
        taskId: '',
        title: '',
        description: '',
        priority: 1,
        currentStatus: 0,
        deadline: '',
        colorLabel: '#6366f1',
        boardId: '',
        assigneeId: '',
        subTasks: []
      };
    }
  }
});

const closeModal = () => {
  emit('close');
};

const submitForm = () => {
  const payload = { ...formData.value };
  
  if (!payload.boardId) {
    alert('Vui lòng chọn dự án!');
    return;
  }
  
  // Clean assigneeId: send null instead of "" if empty
  if (!payload.assigneeId) {
    payload.assigneeId = null;
  }
  
  // If not edit mode (creating new task), omit taskId so the backend can generate it
  if (!isEdit.value) {
    delete payload.taskId;
  }
  
  if (!payload.deadline) {
    payload.deadline = null;
  } else {
    payload.deadline = new Date(payload.deadline).toISOString();
  }
  
  emit('save', payload);
};

const deleteTask = () => {
  if (confirm('Bạn có chắc chắn muốn xóa tác vụ này?')) {
    emit('delete', formData.value.taskId);
  }
};
</script>

<style scoped>
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100vw;
  height: 100vh;
  background: rgba(15, 23, 42, 0.6);
  backdrop-filter: blur(12px);
  -webkit-backdrop-filter: blur(12px);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 1000;
  animation: fadeIn 0.2s ease;
}

@keyframes fadeIn {
  from { opacity: 0; }
  to { opacity: 1; }
}

.modal-content {
  width: 95%;
  max-width: 540px;
  background: var(--bg-card);
  border: 1px solid var(--border-color);
  box-shadow: var(--shadow-xl), var(--shadow-3d-card);
  border-radius: var(--radius-xl);
  padding: 24px;
  max-height: 92vh;
  overflow-y: auto;
  animation: slideUp 0.35s cubic-bezier(0.16, 1, 0.3, 1);
  transition: max-width 0.3s ease;
  display: flex;
  flex-direction: column;
}

.modal-large {
  max-width: 1020px;
}

@keyframes slideUp {
  from { transform: translateY(16px); opacity: 0; }
  to { transform: translateY(0); opacity: 1; }
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: var(--space-4);
  border-bottom: 1px solid var(--border-color);
  padding-bottom: var(--space-3);
}

.modal-breadcrumbs {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: var(--font-size-xs);
  font-weight: 700;
  color: var(--text-muted);
}

.modal-breadcrumbs .project-crumb {
  color: var(--color-primary);
  text-decoration: underline;
  cursor: pointer;
}

.modal-breadcrumbs .divider {
  color: var(--border-color);
}

.modal-breadcrumbs .task-crumb {
  color: var(--text-primary);
}

.create-title {
  font-size: var(--font-size-md);
  font-weight: 800;
  color: var(--text-primary);
}

.close-btn {
  font-size: 1.8rem;
  color: var(--text-muted);
  transition: color var(--transition-fast);
  background: none;
  border: none;
  cursor: pointer;
}

.close-btn:hover {
  color: var(--text-primary);
}

/* Redesigned grid in Edit Mode */
.modal-body {
  display: flex;
  flex-direction: column;
  gap: var(--space-4);
}

.modal-body.edit-mode-grid {
  display: grid;
  grid-template-columns: 1.2fr 1fr;
  gap: var(--space-6);
}

@media (max-width: 768px) {
  .modal-body.edit-mode-grid {
    grid-template-columns: 1fr;
    gap: var(--space-5);
  }
}

.task-left-section {
  display: flex;
  flex-direction: column;
  min-width: 0;
}

.task-right-section {
  display: flex;
  flex-direction: column;
  gap: var(--space-5);
  border-left: 1px solid var(--border-color);
  padding-left: var(--space-5);
  min-width: 0;
}

@media (max-width: 768px) {
  .task-right-section {
    border-left: none;
    padding-left: 0;
    border-top: 1px solid var(--border-color);
    padding-top: var(--space-4);
  }
}

/* Edit Mode Large Title */
.title-large {
  font-size: var(--font-size-lg) !important;
  font-weight: 800 !important;
  border-bottom: 1px dashed var(--border-color) !important;
  background: transparent !important;
  padding: var(--space-2) 0 !important;
  border-radius: 0 !important;
  color: var(--text-primary) !important;
  border: none !important;
}

.title-large:focus {
  border-bottom-color: var(--color-primary) !important;
  box-shadow: none !important;
}

.form-group {
  margin-bottom: var(--space-4);
  display: flex;
  flex-direction: column;
}

label {
  font-size: var(--font-size-xs);
  font-weight: 700;
  margin-bottom: var(--space-2);
  color: var(--text-secondary);
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

input[type="text"],
select,
textarea {
  background: var(--color-bg-secondary);
  border: 1px solid var(--border-color);
  color: var(--text-primary);
  padding: 10px 14px;
  border-radius: var(--radius-md);
  font-family: inherit;
  font-size: var(--font-size-sm);
  transition: all var(--transition-fast);
}

input:focus, select:focus, textarea:focus {
  outline: none;
  border-color: var(--accent-primary);
  box-shadow: 0 0 0 3px rgba(37, 99, 235, 0.12);
}

/* Subtasks styling */
.subtasks-group {
  border-top: 1px dashed var(--border-color);
  padding-top: var(--space-4);
  margin-top: var(--space-3);
}

.subtask-input-wrapper {
  display: flex;
  gap: 8px;
  margin-bottom: var(--space-3);
}

.subtask-input-wrapper input {
  flex: 1;
}

.btn-add-subtask {
  padding: 0 var(--space-4);
  border-radius: var(--radius-md);
  background: var(--color-primary);
  color: white;
  font-weight: 700;
  font-size: var(--font-size-xs);
  cursor: pointer;
}
.btn-add-subtask:hover {
  background: var(--color-primary-hover);
}

.subtasks-progress {
  margin-bottom: var(--space-3);
}

.progress-info {
  display: flex;
  justify-content: space-between;
  font-size: 10px;
  color: var(--text-secondary);
  font-weight: 700;
  margin-bottom: 4px;
}

.progress-track {
  height: 5px;
  background: var(--color-bg-secondary);
  border-radius: var(--radius-full);
  overflow: hidden;
}

.progress-fill {
  height: 100%;
  background: var(--status-done);
  border-radius: var(--radius-full);
  transition: width 0.3s ease;
}

.subtask-list {
  display: flex;
  flex-direction: column;
  gap: var(--space-2);
  max-height: 140px;
  overflow-y: auto;
  margin-bottom: var(--space-3);
}

.subtask-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 8px 10px;
  background: var(--color-bg-secondary);
  border-radius: var(--radius-md);
}

.subtask-checkbox-label {
  display: flex;
  align-items: center;
  gap: 8px;
  cursor: pointer;
  text-transform: none;
  font-weight: normal;
  letter-spacing: normal;
  margin-bottom: 0;
  font-size: var(--font-size-sm);
  color: var(--text-primary);
}

.subtask-checkbox-label input {
  display: none;
}

.custom-checkbox {
  width: 16px;
  height: 16px;
  border-radius: 4px;
  border: 2px solid var(--border-color);
  display: inline-block;
  position: relative;
  flex-shrink: 0;
}

.subtask-checkbox-label input:checked + .custom-checkbox {
  background: var(--status-done);
  border-color: var(--status-done);
}

.subtask-checkbox-label input:checked + .custom-checkbox::after {
  content: '';
  position: absolute;
  left: 4px;
  top: 1px;
  width: 4px;
  height: 8px;
  border: solid white;
  border-width: 0 2px 2px 0;
  transform: rotate(45deg);
}

.subtask-title-text.line-through {
  text-decoration: line-through;
  color: var(--text-muted);
}

.btn-remove-subtask {
  font-size: 1.1rem;
  color: var(--text-muted);
  cursor: pointer;
}
.btn-remove-subtask:hover {
  color: var(--color-danger);
}

.auto-sync-toggle {
  margin-top: 6px;
}

.toggle-label {
  display: flex;
  align-items: center;
  gap: 6px;
  font-size: 10px;
  color: var(--text-muted);
  font-weight: 600;
  text-transform: none;
  letter-spacing: normal;
  cursor: pointer;
}

/* Create mode fields layout */
.create-mode-attributes {
  display: flex;
  flex-direction: column;
}

.form-row {
  display: flex;
  gap: var(--space-4);
}

.form-row .form-group {
  flex: 1;
}

.assignee-select-wrapper {
  display: flex;
  gap: 6px;
}
.assignee-select-wrapper select {
  flex: 1;
}

.btn-assign-me {
  padding: 0 12px;
  border-radius: var(--radius-md);
  border: 1px solid var(--border-color);
  font-size: var(--font-size-xs);
  font-weight: 700;
  cursor: pointer;
  background: var(--color-bg-secondary);
  color: var(--text-primary);
}

.btn-assign-me:hover {
  background: var(--color-primary-light);
  color: var(--color-primary);
  border-color: rgba(37, 99, 235, 0.2);
}

/* Sidebar properties styling */
.properties-panel-card {
  padding: var(--space-4);
  border-radius: var(--radius-lg);
  background: var(--color-bg-secondary);
}

.panel-section-title {
  font-size: 11px;
  font-weight: 800;
  text-transform: uppercase;
  color: var(--text-muted);
  letter-spacing: 0.8px;
  margin-bottom: var(--space-3);
  padding-bottom: 6px;
  border-bottom: 1px solid var(--border-color);
}

.properties-grid {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.prop-item {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: var(--space-4);
}

.prop-label {
  display: flex;
  align-items: center;
  gap: 6px;
  font-size: var(--font-size-xs);
  color: var(--text-secondary);
  font-weight: var(--font-weight-semibold);
  width: 100px;
  flex-shrink: 0;
}

.prop-value {
  flex: 1;
  display: flex;
  justify-content: flex-end;
}

.prop-value select,
.prop-value input {
  height: 32px;
  padding: 0 10px;
  border-radius: var(--radius-sm);
  font-size: var(--font-size-xs);
  border: 1px solid var(--border-color);
  background: var(--color-bg-secondary);
  color: var(--text-primary);
  cursor: pointer;
  max-width: 180px;
}

.prio-select-3 { border-color: rgba(239, 68, 68, 0.3) !important; color: #ef4444 !important; }
.prio-select-2 { border-color: rgba(245, 158, 11, 0.3) !important; color: #d97706 !important; }
.prio-select-1 { border-color: rgba(59, 130, 246, 0.3) !important; color: #3b82f6 !important; }
.prio-select-0 { border-color: var(--border-color) !important; color: var(--text-muted) !important; }

.prop-deadline-input {
  max-width: 180px;
}

.quick-dates-tiny {
  display: flex;
  gap: 4px;
  margin-top: 4px;
  justify-content: flex-end;
}
.quick-dates-tiny button {
  font-size: 8px;
  font-weight: 700;
  color: var(--color-primary);
  background: transparent;
  cursor: pointer;
}
.quick-dates-tiny button:hover {
  text-decoration: underline;
}

.prop-color-input {
  width: 44px;
  height: 24px;
  padding: 0;
  border: 1px solid var(--border-color);
  cursor: pointer;
}

.btn-assign-me-tiny {
  height: 32px;
  padding: 0 8px;
  border-radius: var(--radius-sm);
  font-size: 9px;
  font-weight: 700;
  border: 1px solid var(--border-color);
  background: var(--color-bg-secondary);
  color: var(--text-primary);
  cursor: pointer;
}

.btn-assign-me-tiny:hover {
  background: var(--color-primary-light);
  color: var(--color-primary);
}

/* Activity logs and feed layout */
.activity-feed-container {
  display: flex;
  flex-direction: column;
  flex: 1;
  min-height: 280px;
}

.task-modal-tabs {
  display: flex;
  gap: 14px;
  border-bottom: 1px solid var(--border-color);
  margin-bottom: var(--space-3);
}

.tab-btn {
  background: transparent;
  border: none;
  padding: 6px 2px;
  font-size: var(--font-size-sm);
  font-weight: 700;
  color: var(--text-muted);
  cursor: pointer;
  border-bottom: 2px solid transparent;
  transition: all var(--transition-fast);
}

.tab-btn:hover {
  color: var(--text-primary);
}

.tab-btn.active {
  color: var(--color-primary);
  border-bottom-color: var(--color-primary);
}

.feed-body-pane {
  flex: 1;
  display: flex;
  flex-direction: column;
  min-height: 220px;
  max-height: 300px;
  overflow-y: auto;
}

.tab-pane {
  flex: 1;
  display: flex;
  flex-direction: column;
}

/* Modal Actions buttons styles */
.modal-actions {
  display: flex;
  justify-content: flex-end;
  gap: 10px;
  margin-top: var(--space-6);
  border-top: 1px solid var(--border-color);
  padding-top: var(--space-4);
}

.btn-cancel, .btn-submit, .btn-delete {
  padding: 8px 16px;
  border-radius: var(--radius-md);
  font-weight: 700;
  font-size: var(--font-size-xs);
  cursor: pointer;
  transition: all var(--transition-fast);
}

.btn-cancel {
  background: transparent;
  color: var(--text-secondary);
  border: 1px solid var(--border-color);
}
.btn-cancel:hover {
  background: var(--color-bg-secondary);
}

.btn-submit {
  background: var(--color-primary);
  color: white;
  border: none;
}
.btn-submit:hover {
  background: var(--color-primary-hover);
}

.btn-delete {
  background: transparent;
  color: var(--color-danger);
  border: 1px solid var(--color-danger);
  margin-right: auto;
}
.btn-delete:hover {
  background: var(--color-danger);
  color: white;
}
</style>
