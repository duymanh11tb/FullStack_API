<template>
  <div v-if="isOpen" class="modal-overlay" @click.self="closeModal">
    <div class="modal-content glass-panel" :class="{ 'modal-large': isEdit }">
      <div class="modal-header">
        <h2>{{ isEdit ? 'Task Details' : 'Create New Task' }}</h2>
        <button class="close-btn" @click="closeModal">&times;</button>
      </div>
      
      <div class="modal-body" :class="{ 'with-discussion': isEdit }">
        <div class="task-form-container">
          <form @submit.prevent="submitForm" class="task-form">
            <div class="form-group">
              <label for="title">Title</label>
              <input id="title" v-model="formData.title" type="text" required placeholder="Task title..." />
            </div>
            
            <div class="form-group">
              <label for="description">Description</label>
              <textarea id="description" v-model="formData.description" rows="3" placeholder="Add some details..."></textarea>
            </div>
            
            <div class="form-row">
              <div class="form-group">
                <label for="priority">Priority</label>
                <select id="priority" v-model="formData.priority">
                  <option :value="0">Low</option>
                  <option :value="1">Medium</option>
                  <option :value="2">High</option>
                  <option :value="3">Urgent</option>
                </select>
              </div>
              
              <div class="form-group">
                <label for="status">Status</label>
                <select id="status" v-model="formData.currentStatus">
                  <option :value="0">To Do</option>
                  <option :value="1">In Progress</option>
                  <option :value="2">Review</option>
                  <option :value="3">Done</option>
                </select>
              </div>
            </div>

            <div class="form-row">
              <div class="form-group">
                <label for="deadline">Deadline</label>
                <input id="deadline" v-model="formData.deadline" type="datetime-local" />
              </div>
              
              <div class="form-group">
                <label for="colorLabel">Color Label</label>
                <input id="colorLabel" v-model="formData.colorLabel" type="color" />
              </div>
            </div>
            
            <div class="modal-actions">
              <button type="button" class="btn-cancel" @click="closeModal">Cancel</button>
              <button type="submit" class="btn-submit" :disabled="loading">
                {{ loading ? 'Saving...' : 'Save Task' }}
              </button>
              <button v-if="isEdit" type="button" class="btn-delete" @click="deleteTask" :disabled="loading">
                Delete
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
import { ref, watch, defineProps, defineEmits } from 'vue';
import TaskDiscussion from './task/TaskDiscussion.vue';
import ActivityLogList from './common/ActivityLogList.vue';

const props = defineProps({
  isOpen: Boolean,
  task: Object,
  loading: Boolean
});

const emit = defineEmits(['close', 'save', 'delete']);

const isEdit = ref(false);
const activeTab = ref('discussion');
const formData = ref({
  title: '',
  description: '',
  priority: 1,
  currentStatus: 0,
  deadline: '',
  colorLabel: '#6366f1'
});

watch(() => props.isOpen, (newVal) => {
  if (newVal) {
    activeTab.value = 'discussion';
    if (props.task) {
      isEdit.value = true;
      // Format datetime-local string
      let deadlineStr = '';
      if (props.task.deadline) {
        const d = new Date(props.task.deadline);
        d.setMinutes(d.getMinutes() - d.getTimezoneOffset());
        deadlineStr = d.toISOString().slice(0, 16);
      }
      
      formData.value = {
        taskId: props.task.taskId,
        title: props.task.title || '',
        description: props.task.description || '',
        priority: props.task.priority ?? 1,
        currentStatus: props.task.currentStatus ?? 0,
        deadline: deadlineStr,
        colorLabel: props.task.colorLabel || '#6366f1'
      };
    } else {
      isEdit.value = false;
      formData.value = {
        title: '',
        description: '',
        priority: 1,
        currentStatus: 0,
        deadline: '',
        colorLabel: '#6366f1'
      };
    }
  }
});

const closeModal = () => {
  emit('close');
};

const submitForm = () => {
  const payload = { ...formData.value };
  if (!payload.deadline) payload.deadline = null;
  else payload.deadline = new Date(payload.deadline).toISOString();
  
  emit('save', payload);
};

const deleteTask = () => {
  if (confirm('Are you sure you want to delete this task?')) {
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
  background: rgba(15, 23, 42, 0.55);
  backdrop-filter: blur(8px);
  -webkit-backdrop-filter: blur(8px);
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
  width: 100%;
  max-width: 500px;
  background: var(--bg-secondary);
  border: 1px solid var(--border-color);
  box-shadow: var(--shadow-xl);
  border-radius: var(--radius-xl);
  padding: 24px;
  animation: slideUp 0.35s cubic-bezier(0.16, 1, 0.3, 1);
  transition: max-width 0.3s ease;
}

.modal-large {
  max-width: 900px;
}

.modal-body.with-discussion {
  display: flex;
  gap: 24px;
}

.task-form-container {
  flex: 1;
}

.task-discussion-container {
  flex: 1;
  border-left: 1px solid var(--border-color);
  padding-left: 24px;
  display: flex;
  flex-direction: column;
}

.discussion-title {
  font-size: 1.1rem;
  margin-bottom: 16px;
  color: var(--text-primary);
}

@keyframes slideUp {
  from { transform: translateY(20px); opacity: 0; }
  to { transform: translateY(0); opacity: 1; }
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 24px;
}

.close-btn {
  font-size: 1.5rem;
  color: var(--text-muted);
  transition: color var(--transition-fast);
}

.close-btn:hover {
  color: var(--text-primary);
}

.form-group {
  margin-bottom: 16px;
  display: flex;
  flex-direction: column;
}

.form-row {
  display: flex;
  gap: 16px;
}

.form-row .form-group {
  flex: 1;
}

label {
  font-size: 0.875rem;
  font-weight: 500;
  margin-bottom: 8px;
  color: var(--text-secondary);
}

input[type="text"],
input[type="datetime-local"],
select,
textarea {
  background: var(--bg-primary);
  border: 1px solid var(--border-color);
  color: var(--text-primary);
  padding: 10px 12px;
  border-radius: 8px;
  font-family: inherit;
  font-size: 1rem;
  transition: border-color var(--transition-fast);
}

input:focus, select:focus, textarea:focus {
  outline: none;
  border-color: var(--accent-primary);
}

input[type="color"] {
  height: 42px;
  padding: 4px;
  width: 100%;
}

.modal-actions {
  display: flex;
  justify-content: flex-end;
  gap: 12px;
  margin-top: 32px;
}

.btn-cancel, .btn-submit, .btn-delete {
  padding: 10px 20px;
  border-radius: 8px;
  font-weight: 600;
  transition: all var(--transition-fast);
}

.btn-cancel {
  background: transparent;
  color: var(--text-secondary);
  border: 1px solid var(--border-color);
}

.btn-cancel:hover {
  background: rgba(255, 255, 255, 0.05);
  color: var(--text-primary);
}

.btn-submit {
  background: var(--accent-primary);
  color: white;
}

.btn-submit:hover:not(:disabled) {
  background: var(--accent-primary-hover);
}

.btn-delete {
  background: transparent;
  color: var(--status-cancelled);
  border: 1px solid var(--status-cancelled);
  margin-right: auto;
}

.btn-delete:hover {
  background: var(--status-cancelled);
  color: white;
}

button:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

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
  font-weight: 600;
  color: var(--text-secondary);
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

.tab-pane {
  flex: 1;
  display: flex;
  flex-direction: column;
  min-height: 0;
}
</style>
