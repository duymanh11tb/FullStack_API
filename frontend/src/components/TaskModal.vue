<template>
  <div v-if="isOpen" class="modal-overlay" @click.self="closeModal">
    <div class="modal-content glass-panel" :class="{ 'modal-large': isEdit }">
      <div class="modal-header">
        <h2>{{ isEdit ? 'Chi tiết tác vụ' : 'Tạo tác vụ mới' }}</h2>
        <button class="close-btn" @click="closeModal">&times;</button>
      </div>
      
      <div class="modal-body" :class="{ 'with-discussion': isEdit }">
        <div class="task-form-container">
          <form @submit.prevent="submitForm" class="task-form">
            <!-- Project and Assignee Selection -->
            <div class="form-row">
              <div class="form-group">
                <label for="project">Dự án (Project)</label>
                <select 
                  id="project" 
                  v-model="formData.boardId" 
                  :disabled="isEdit" 
                  required
                >
                  <option value="" disabled>Chọn dự án...</option>
                  <option 
                    v-for="p in projectStore.projects" 
                    :key="p.id" 
                    :value="p.id"
                  >
                    {{ p.name }}
                  </option>
                </select>
              </div>
              
              <div class="form-group">
                <label for="assignee">Người phụ trách (Assignee)</label>
                <div class="assignee-select-wrapper">
                  <select id="assignee" v-model="formData.assigneeId">
                    <option value="">Chưa giao việc (None)</option>
                    <option 
                      v-for="m in projectMembers" 
                      :key="m.userId || m.id" 
                      :value="m.userId || m.id"
                    >
                      {{ m.displayName }}
                    </option>
                  </select>
                  <button 
                    v-if="formData.boardId"
                    type="button" 
                    class="btn-assign-me" 
                    @click="assignToMe"
                    title="Giao cho tôi"
                  >
                    Tôi
                  </button>
                </div>
              </div>
            </div>

            <div class="form-group">
              <label for="title">Tiêu đề</label>
              <input id="title" v-model="formData.title" type="text" required placeholder="Tên tác vụ..." />
            </div>
            
            <div class="form-group">
              <label for="description">Mô tả</label>
              <textarea id="description" v-model="formData.description" rows="3" placeholder="Thêm mô tả chi tiết..."></textarea>
            </div>
            
            <div class="form-row">
              <div class="form-group">
                <label for="priority">Độ ưu tiên</label>
                <select id="priority" v-model="formData.priority">
                  <option :value="0">Thấp (Low)</option>
                  <option :value="1">Trung bình (Medium)</option>
                  <option :value="2">Cao (High)</option>
                  <option :value="3">Khẩn cấp (Urgent)</option>
                </select>
              </div>
              
              <div class="form-group">
                <label for="status">Trạng thái</label>
                <select id="status" v-model="formData.currentStatus">
                  <option :value="0">Cần làm (To Do)</option>
                  <option :value="1">Đang làm (In Progress)</option>
                  <option :value="2">Đánh giá (Review)</option>
                  <option :value="3">Hoàn thành (Done)</option>
                </select>
              </div>
            </div>

            <div class="form-row">
              <div class="form-group">
                <label for="deadline">Hạn chót</label>
                <input id="deadline" v-model="formData.deadline" type="datetime-local" />
                <div class="quick-dates">
                  <button type="button" class="btn-quick-date" @click="setDeadlineDays(0)">Hôm nay</button>
                  <button type="button" class="btn-quick-date" @click="setDeadlineDays(3)">3 ngày</button>
                  <button type="button" class="btn-quick-date" @click="setDeadlineDays(7)">1 tuần</button>
                </div>
              </div>
              
              <div class="form-group">
                <label for="colorLabel">Nhãn màu</label>
                <input id="colorLabel" v-model="formData.colorLabel" type="color" />
              </div>
            </div>

            <!-- Subtasks checklist -->
            <div class="form-group subtasks-group">
              <label>Công việc phụ (Subtasks)</label>
              
              <!-- Add subtask input -->
              <div class="subtask-input-wrapper">
                <input 
                  type="text" 
                  v-model="newSubtaskTitle" 
                  placeholder="Nhập việc phụ và nhấn Enter..." 
                  @keydown.enter.prevent="addSubtask" 
                />
                <button type="button" class="btn-add-subtask" @click="addSubtask">Thêm</button>
              </div>

              <!-- Progress bar -->
              <div v-if="totalSubTasksCount > 0" class="subtasks-progress">
                <div class="progress-info">
                  <span>Tiến độ việc phụ: {{ completedSubTasksCount }}/{{ totalSubTasksCount }}</span>
                  <span>{{ subTasksProgressPercent }}%</span>
                </div>
                <div class="progress-track">
                  <div class="progress-fill" :style="{ width: subTasksProgressPercent + '%' }"></div>
                </div>
              </div>

              <!-- Subtask list -->
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

              <!-- Auto status sync toggle -->
              <div v-if="totalSubTasksCount > 0" class="auto-sync-toggle">
                <label class="toggle-label">
                  <input type="checkbox" v-model="autoTransition" />
                  <span class="toggle-text">Tự động hoàn thành task khi xong việc phụ</span>
                </label>
              </div>
            </div>
            
            <div class="modal-actions">
              <button type="button" class="btn-cancel" @click="closeModal">Hủy</button>
              <button type="submit" class="btn-submit" :disabled="loading">
                {{ loading ? 'Đang lưu...' : 'Lưu tác vụ' }}
              </button>
              <button v-if="isEdit" type="button" class="btn-delete" @click="deleteTask" :disabled="loading">
                Xóa
              </button>
            </div>
          </form>
        </div>

        <div v-if="isEdit" class="task-discussion-container">
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
              Hoạt động
            </button>
          </div>
          
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
</template>

<script setup>
import { ref, watch, computed, onMounted, defineProps, defineEmits } from 'vue';
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
  background: rgba(15, 23, 42, 0.65);
  backdrop-filter: blur(10px);
  -webkit-backdrop-filter: blur(10px);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 1000;
  animation: fadeIn 0.25s ease;
}

@keyframes fadeIn {
  from { opacity: 0; }
  to { opacity: 1; }
}

.modal-content {
  width: 95%;
  max-width: 520px;
  background: var(--bg-white-to-card);
  border: 1px solid var(--border-color);
  box-shadow: var(--shadow-xl), var(--shadow-3d-card);
  border-radius: var(--radius-xl);
  padding: 24px;
  max-height: 90vh;
  overflow-y: auto;
  animation: slideUp 0.4s cubic-bezier(0.16, 1, 0.3, 1);
  transition: max-width 0.3s ease;
}

.modal-large {
  max-width: 950px;
}

.modal-body.with-discussion {
  display: flex;
  gap: 28px;
}

@media (max-width: 768px) {
  .modal-body.with-discussion {
    flex-direction: column;
    gap: 20px;
  }
}

.task-form-container {
  flex: 1.1;
  min-width: 0;
}

.task-discussion-container {
  flex: 0.9;
  border-left: 1px solid var(--border-color);
  padding-left: 28px;
  display: flex;
  flex-direction: column;
  min-width: 0;
}

@media (max-width: 768px) {
  .task-discussion-container {
    border-left: none;
    padding-left: 0;
    border-top: 1px solid var(--border-color);
    padding-top: 20px;
  }
}

@keyframes slideUp {
  from { transform: translateY(24px); opacity: 0; }
  to { transform: translateY(0); opacity: 1; }
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 24px;
  border-bottom: 1px solid var(--border-color);
  padding-bottom: 12px;
}

.modal-header h2 {
  font-size: var(--font-size-lg);
  font-weight: 800;
  color: var(--color-text-primary);
}

.close-btn {
  font-size: 1.6rem;
  color: var(--color-text-tertiary);
  transition: color var(--transition-fast);
  background: none;
  border: none;
  cursor: pointer;
}

.close-btn:hover {
  color: var(--color-text-primary);
}

.form-group {
  margin-bottom: 18px;
  display: flex;
  flex-direction: column;
}

.form-row {
  display: flex;
  gap: 16px;
}

@media (max-width: 480px) {
  .form-row {
    flex-direction: column;
    gap: 0;
  }
}

.form-row .form-group {
  flex: 1;
}

label {
  font-size: var(--font-size-xs);
  font-weight: 700;
  margin-bottom: 8px;
  color: var(--color-text-secondary);
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

input[type="text"],
select,
textarea {
  background: var(--color-bg-secondary);
  border: 1px solid var(--border-color);
  color: var(--color-text-primary);
  padding: 10px 14px;
  border-radius: var(--radius-md);
  font-family: inherit;
  font-size: var(--font-size-sm);
  transition: all var(--transition-fast);
}

[data-theme='dark'] input[type="text"],
[data-theme='dark'] select,
[data-theme='dark'] textarea {
  background: rgba(15, 23, 42, 0.4);
}

input:focus, select:focus, textarea:focus {
  outline: none;
  border-color: var(--accent-primary);
  box-shadow: 0 0 0 3px rgba(37, 99, 235, 0.12);
}

input[type="datetime-local"] {
  background: var(--color-bg-secondary);
  border: 1px solid var(--border-color);
  color: var(--color-text-primary);
  padding: 8px 12px;
  border-radius: var(--radius-md);
  font-family: inherit;
  font-size: var(--font-size-sm);
}

[data-theme='dark'] input[type="datetime-local"] {
  background: rgba(15, 23, 42, 0.4);
}

input[type="color"] {
  height: 40px;
  padding: 4px;
  width: 100%;
  border-radius: var(--radius-md);
  border: 1px solid var(--border-color);
  background: transparent;
  cursor: pointer;
}

/* Assignee input specific integration */
.assignee-select-wrapper {
  display: flex;
  gap: 8px;
}

.assignee-select-wrapper select {
  flex: 1;
}

.btn-assign-me {
  padding: 0 14px;
  border-radius: var(--radius-md);
  border: 1px solid var(--border-color);
  background: var(--color-bg-secondary);
  color: var(--color-text-primary);
  font-weight: 700;
  font-size: var(--font-size-xs);
  cursor: pointer;
  transition: all var(--transition-fast);
}

.btn-assign-me:hover {
  background: var(--color-primary-light);
  color: var(--color-primary);
  border-color: rgba(37, 99, 235, 0.2);
}

/* Quick dates helpers */
.quick-dates {
  display: flex;
  gap: 6px;
  margin-top: 6px;
}

.btn-quick-date {
  background: transparent;
  border: none;
  color: var(--color-primary);
  font-size: 11px;
  font-weight: 700;
  cursor: pointer;
  padding: 2px 4px;
  transition: opacity 0.2s;
}

.btn-quick-date:hover {
  text-decoration: underline;
  opacity: 0.85;
}

/* Subtasks Checklist Styles */
.subtasks-group {
  border-top: 1px dashed var(--border-color);
  padding-top: 18px;
  margin-top: 24px;
}

.subtask-input-wrapper {
  display: flex;
  gap: 8px;
  margin-bottom: 14px;
}

.subtask-input-wrapper input {
  flex: 1;
}

.btn-add-subtask {
  padding: 0 16px;
  border-radius: var(--radius-md);
  background: var(--color-primary);
  color: white;
  font-weight: 700;
  font-size: var(--font-size-sm);
  border: none;
  cursor: pointer;
  transition: background var(--transition-fast);
}

.btn-add-subtask:hover {
  background: var(--color-primary-hover);
}

.subtasks-progress {
  margin-bottom: 14px;
}

.progress-info {
  display: flex;
  justify-content: space-between;
  font-size: 11px;
  color: var(--color-text-secondary);
  font-weight: 700;
  margin-bottom: 4px;
}

.progress-track {
  height: 6px;
  background: var(--color-bg-secondary);
  border-radius: var(--radius-full);
  overflow: hidden;
}

[data-theme='dark'] .progress-track {
  background: rgba(255, 255, 255, 0.05);
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
  gap: 8px;
  max-height: 180px;
  overflow-y: auto;
  margin-bottom: 12px;
  padding-right: 4px;
}

.subtask-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 8px 12px;
  background: var(--color-bg-secondary);
  border-radius: var(--radius-md);
  border: 1px solid transparent;
  transition: border-color 0.2s;
}

[data-theme='dark'] .subtask-item {
  background: rgba(15, 23, 42, 0.2);
}

.subtask-item:hover {
  border-color: var(--border-color);
}

.subtask-checkbox-label {
  display: flex;
  align-items: center;
  gap: 10px;
  cursor: pointer;
  margin: 0;
  flex: 1;
  text-transform: none;
  letter-spacing: normal;
  font-weight: normal;
  font-size: var(--font-size-sm);
  color: var(--color-text-primary);
}

.subtask-checkbox-label input {
  display: none;
}

.custom-checkbox {
  width: 18px;
  height: 18px;
  border-radius: 4px;
  border: 2px solid var(--border-color);
  display: inline-block;
  position: relative;
  transition: all 0.2s;
  flex-shrink: 0;
}

.subtask-checkbox-label input:checked + .custom-checkbox {
  background: var(--status-done);
  border-color: var(--status-done);
}

.subtask-checkbox-label input:checked + .custom-checkbox::after {
  content: '';
  position: absolute;
  left: 5px;
  top: 2px;
  width: 5px;
  height: 9px;
  border: solid white;
  border-width: 0 2px 2px 0;
  transform: rotate(45deg);
}

.subtask-title-text {
  transition: color 0.2s;
}

.subtask-title-text.line-through {
  text-decoration: line-through;
  color: var(--color-text-tertiary);
}

.btn-remove-subtask {
  background: none;
  border: none;
  color: var(--color-text-tertiary);
  font-size: 1.2rem;
  cursor: pointer;
  padding: 0 4px;
  transition: color 0.2s;
}

.btn-remove-subtask:hover {
  color: var(--color-danger);
}

.auto-sync-toggle {
  display: flex;
  align-items: center;
  margin-top: 8px;
}

.toggle-label {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 11px;
  font-weight: 600;
  color: var(--color-text-secondary);
  cursor: pointer;
  text-transform: none;
  letter-spacing: normal;
}

.toggle-label input {
  cursor: pointer;
}

/* Modal actions spacing */
.modal-actions {
  display: flex;
  justify-content: flex-end;
  gap: 12px;
  margin-top: 32px;
  border-top: 1px solid var(--border-color);
  padding-top: 16px;
}

.btn-cancel, .btn-submit, .btn-delete {
  padding: 10px 20px;
  border-radius: var(--radius-md);
  font-weight: 700;
  font-size: var(--font-size-sm);
  cursor: pointer;
  transition: all var(--transition-fast);
}

.btn-cancel {
  background: transparent;
  color: var(--color-text-secondary);
  border: 1px solid var(--border-color);
}

.btn-cancel:hover {
  background: var(--color-bg-secondary);
  color: var(--color-text-primary);
}

.btn-submit {
  background: var(--color-primary);
  color: white;
  border: none;
  box-shadow: 0 4px 12px rgba(37, 99, 235, 0.2);
}

.btn-submit:hover:not(:disabled) {
  background: var(--color-primary-hover);
  box-shadow: 0 6px 16px rgba(37, 99, 235, 0.3);
}

.btn-delete {
  background: transparent;
  color: var(--color-danger);
  border: 1px solid var(--color-danger);
  margin-right: auto;
}

.btn-delete:hover:not(:disabled) {
  background: var(--color-danger);
  color: white;
}

button:disabled {
  opacity: 0.5;
  cursor: not-allowed;
  box-shadow: none;
}

/* Discussion tabs design */
.task-modal-tabs {
  display: flex;
  gap: 16px;
  border-bottom: 1px solid var(--border-color);
  margin-bottom: 16px;
}

.tab-btn {
  background: transparent;
  border: none;
  padding: 8px 4px;
  font-size: 1rem;
  font-weight: 700;
  color: var(--color-text-tertiary);
  cursor: pointer;
  border-bottom: 2px solid transparent;
  transition: all var(--transition-fast);
}

.tab-btn:hover {
  color: var(--color-text-primary);
}

.tab-btn.active {
  color: var(--color-primary);
  border-bottom-color: var(--color-primary);
}

.tab-pane {
  flex: 1;
  display: flex;
  flex-direction: column;
  min-height: 0;
}
</style>
