<template>
  <div 
    class="task-card glass-panel" 
    :class="'priority-' + task.priority" 
    @click="$emit('edit', task)"
    @mousemove="handleMouseMove"
    @mouseleave="handleMouseLeave"
    ref="cardRef"
  >
    <div class="card-glow" :style="glowStyle"></div>
    <div class="task-header">
      <h3 class="task-title">{{ task.title }}</h3>
      <span v-if="task.colorLabel" class="color-label" :style="{ backgroundColor: task.colorLabel }"></span>
    </div>
    
    <p class="task-desc" v-if="task.description">{{ truncate(task.description, 65) }}</p>
    
    <div class="task-footer">
      <div class="priority-badge" :class="getPriorityClass(task.priority)">
        {{ getPriorityName(task.priority) }}
      </div>
      <div class="deadline" v-if="task.deadline">
        <svg xmlns="http://www.w3.org/2000/svg" width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round">
          <circle cx="12" cy="12" r="10"></circle>
          <polyline points="12 6 12 12 16 14"></polyline>
        </svg>
        <span>{{ formatDate(task.deadline) }}</span>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, defineProps, defineEmits } from 'vue';

const props = defineProps({
  task: {
    type: Object,
    required: true
  }
});

defineEmits(['edit']);

const cardRef = ref(null);
const mouseX = ref(0);
const mouseY = ref(0);
const isHovered = ref(false);

function handleMouseMove(e) {
  if (!cardRef.value) return;
  const rect = cardRef.value.getBoundingClientRect();
  mouseX.value = e.clientX - rect.left;
  mouseY.value = e.clientY - rect.top;
  isHovered.value = true;
}

function handleMouseLeave() {
  isHovered.value = false;
}

const glowStyle = computed(() => {
  if (!isHovered.value) return { opacity: 0 };
  return {
    opacity: 1,
    left: `${mouseX.value}px`,
    top: `${mouseY.value}px`
  };
});

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
  const priorities = { 0: 'Thấp', 1: 'Trung bình', 2: 'Cao', 3: 'Khẩn cấp' };
  return priorities[priority] || 'Trung bình';
};

const getPriorityClass = (priority) => {
  const classes = { 0: 'p-low', 1: 'p-medium', 2: 'p-high', 3: 'p-urgent' };
  return classes[priority] || 'p-medium';
};
</script>

<style scoped>
.task-card {
  padding: 16px;
  margin-bottom: 14px;
  cursor: pointer;
  transition: transform 0.25s cubic-bezier(0.25, 1, 0.5, 1), 
              box-shadow 0.25s cubic-bezier(0.25, 1, 0.5, 1),
              border-color 0.25s ease;
  border-left: 4px solid transparent;
  position: relative;
  overflow: hidden;
  background: var(--bg-white-to-card);
}

.task-card:hover {
  transform: translateY(-3px) scale(1.01);
  border-color: rgba(255, 255, 255, 0.15);
}

/* Dynamic radial glow on card hover */
.card-glow {
  position: absolute;
  width: 140px;
  height: 140px;
  background: radial-gradient(circle, rgba(99, 102, 241, 0.08) 0%, rgba(255, 255, 255, 0) 70%);
  transform: translate(-50%, -50%);
  pointer-events: none;
  z-index: 1;
  transition: opacity 0.3s ease;
}

[data-theme='dark'] .card-glow {
  background: radial-gradient(circle, rgba(99, 102, 241, 0.12) 0%, rgba(255, 255, 255, 0) 70%);
}

.task-card.priority-0 { 
  border-left-color: #94a3b8; 
  box-shadow: 0 4px 12px rgba(148, 163, 184, 0.05);
}
.task-card.priority-1 { 
  border-left-color: #3b82f6; 
  box-shadow: 0 4px 12px rgba(59, 130, 246, 0.06);
}
.task-card.priority-2 { 
  border-left-color: #f59e0b; 
  box-shadow: 0 4px 12px rgba(245, 158, 171, 0.06);
}
.task-card.priority-3 { 
  border-left-color: #ef4444; 
  box-shadow: 0 4px 12px rgba(239, 68, 68, 0.08);
}

.task-card.priority-0:hover { box-shadow: 0 8px 24px rgba(148, 163, 184, 0.1); }
.task-card.priority-1:hover { box-shadow: 0 8px 24px rgba(59, 130, 246, 0.12); }
.task-card.priority-2:hover { box-shadow: 0 8px 24px rgba(245, 158, 11, 0.12); }
.task-card.priority-3:hover { box-shadow: 0 8px 24px rgba(239, 68, 68, 0.16); }

.task-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 8px;
  position: relative;
  z-index: 2;
}

.task-title {
  font-size: 0.95rem;
  font-weight: 600;
  color: var(--color-text-primary);
  margin: 0;
  flex: 1;
  line-height: 1.4;
}

.color-label {
  width: 10px;
  height: 10px;
  border-radius: 50%;
  margin-left: 8px;
  flex-shrink: 0;
  box-shadow: 0 0 6px currentColor;
}

.task-desc {
  font-size: 0.85rem;
  color: var(--color-text-secondary);
  margin-bottom: 12px;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
  line-height: 1.5;
  position: relative;
  z-index: 2;
}

.task-footer {
  display: flex;
  justify-content: space-between;
  align-items: center;
  font-size: 0.72rem;
  position: relative;
  z-index: 2;
}

.priority-badge {
  padding: 3px 8px;
  border-radius: var(--radius-full);
  font-weight: 600;
  font-size: 0.7rem;
}

.p-low { background: rgba(148, 163, 184, 0.12); color: #64748b; }
.p-medium { background: rgba(59, 130, 246, 0.12); color: var(--accent-primary); }
.p-high { background: rgba(245, 158, 11, 0.12); color: #d97706; }
.p-urgent { background: rgba(239, 68, 68, 0.12); color: #dc2626; }

[data-theme='dark'] .p-low { background: rgba(148, 163, 184, 0.2); color: #cbd5e1; }
[data-theme='dark'] .p-medium { background: rgba(59, 130, 246, 0.2); color: #60a5fa; }
[data-theme='dark'] .p-high { background: rgba(245, 158, 11, 0.2); color: #fbbf24; }
[data-theme='dark'] .p-urgent { background: rgba(239, 68, 68, 0.2); color: #f87171; }

.deadline {
  display: flex;
  align-items: center;
  gap: 4px;
  color: var(--color-text-tertiary);
  font-weight: 500;
}
</style>

