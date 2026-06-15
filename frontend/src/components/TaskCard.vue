<template>
  <div class="task-card glass-panel" :class="'priority-' + task.priority" @click="$emit('edit', task)">
    <div class="task-header">
      <h3 class="task-title">{{ task.title }}</h3>
      <span v-if="task.colorLabel" class="color-label" :style="{ backgroundColor: task.colorLabel }"></span>
    </div>
    
    <p class="task-desc" v-if="task.description">{{ truncate(task.description, 60) }}</p>
    
    <div class="task-footer">
      <div class="priority-badge" :class="getPriorityClass(task.priority)">
        {{ getPriorityName(task.priority) }}
      </div>
      <div class="deadline" v-if="task.deadline">
        <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
          <circle cx="12" cy="12" r="10"></circle>
          <polyline points="12 6 12 12 16 14"></polyline>
        </svg>
        {{ formatDate(task.deadline) }}
      </div>
    </div>
  </div>
</template>

<script setup>
import { defineProps } from 'vue';

const props = defineProps({
  task: {
    type: Object,
    required: true
  }
});

defineEmits(['edit']);

const truncate = (text, length) => {
  if (!text) return '';
  return text.length > length ? text.substring(0, length) + '...' : text;
};

const formatDate = (dateString) => {
  if (!dateString) return '';
  const date = new Date(dateString);
  return new Intl.DateTimeFormat('vi-VN', { month: 'short', day: 'numeric' }).format(date);
};

const getPriorityName = (priority) => {
  const priorities = { 0: 'Low', 1: 'Medium', 2: 'High', 3: 'Urgent' };
  return priorities[priority] || 'Normal';
};

const getPriorityClass = (priority) => {
  const classes = { 0: 'p-low', 1: 'p-medium', 2: 'p-high', 3: 'p-urgent' };
  return classes[priority] || 'p-medium';
};
</script>

<style scoped>
.task-card {
  padding: 16px;
  margin-bottom: 12px;
  cursor: pointer;
  transition: transform var(--transition-fast), box-shadow var(--transition-fast);
  border-left: 4px solid transparent;
}

.task-card:hover {
  transform: translateY(-2px);
  box-shadow: var(--shadow-lg);
}

.task-card.priority-2 { border-left-color: var(--status-review); }
.task-card.priority-3 { border-left-color: var(--status-cancelled); }

.task-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 8px;
}

.task-title {
  font-size: 1rem;
  color: var(--text-primary);
  margin: 0;
  flex: 1;
}

.color-label {
  width: 12px;
  height: 12px;
  border-radius: 50%;
  margin-left: 8px;
  flex-shrink: 0;
}

.task-desc {
  font-size: 0.875rem;
  color: var(--text-secondary);
  margin-bottom: 12px;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.task-footer {
  display: flex;
  justify-content: space-between;
  align-items: center;
  font-size: 0.75rem;
}

.priority-badge {
  padding: 4px 8px;
  border-radius: 12px;
  font-weight: 500;
}

.p-low { background: rgba(100, 116, 139, 0.2); color: #94a3b8; }
.p-medium { background: rgba(59, 130, 246, 0.2); color: #60a5fa; }
.p-high { background: rgba(245, 158, 11, 0.2); color: #fbbf24; }
.p-urgent { background: rgba(239, 68, 68, 0.2); color: #f87171; }

.deadline {
  display: flex;
  align-items: center;
  gap: 4px;
  color: var(--text-muted);
}
</style>
