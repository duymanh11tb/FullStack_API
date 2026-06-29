<template>
  <div 
    class="task-card glass-panel" 
    :class="['priority-' + task.priority, { 'has-subtasks': hasSubTasks }]" 
    @click="$emit('edit', task)"
    @mousemove="handleMouseMove"
    @mouseleave="handleMouseLeave"
    ref="cardRef"
  >
    <div class="card-glow" :style="glowStyle"></div>
    
    <!-- Top Meta Row: ID & Priority -->
    <div class="task-top-meta">
      <span class="task-id">{{ taskCode }}</span>
      <div class="priority-icon" :class="priorityClass" :title="priorityName">
        <!-- Urgent -->
        <svg v-if="task.priority === 3" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="3" stroke-linecap="round" stroke-linejoin="round">
          <line x1="12" y1="4" x2="12" y2="16"></line>
          <line x1="12" y1="20" x2="12.01" y2="20"></line>
        </svg>
        <!-- High -->
        <svg v-else-if="task.priority === 2" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round">
          <polyline points="18 15 12 9 6 15"></polyline>
        </svg>
        <!-- Medium -->
        <svg v-else-if="task.priority === 1" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round">
          <line x1="5" y1="12" x2="19" y2="12"></line>
        </svg>
        <!-- Low -->
        <svg v-else width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round">
          <polyline points="6 9 12 15 18 9"></polyline>
        </svg>
      </div>
    </div>

    <!-- Task Title -->
    <h3 class="task-title">{{ task.title }}</h3>
    
    <!-- Task Description (truncated) -->
    <p class="task-desc" v-if="task.description">{{ truncate(task.description, 75) }}</p>
    
    <!-- Subtasks Progress Bar (Mockup feature) -->
    <div class="task-subtasks-progress" v-if="hasSubTasks">
      <div class="progress-bar-container">
        <div class="progress-bar-fill" :style="{ width: `${subtasksPercent}%` }"></div>
      </div>
      <div class="progress-bar-text">{{ subtasksPercent }}% hoàn thành</div>
    </div>

    <!-- Task Footer: Icons & Assignee -->
    <div class="task-footer">
      <div class="footer-left">
        <!-- Checklist Count Indicator -->
        <div class="meta-indicator" v-if="hasSubTasks" title="Công việc phụ">
          <svg width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round">
            <polyline points="9 11 12 14 22 4"></polyline>
            <path d="M21 12v7a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h11"></path>
          </svg>
          <span>{{ completedSubTasksCount }}/{{ totalSubTasksCount }}</span>
        </div>

        <!-- Deadline Clock -->
        <div class="meta-indicator deadline" :class="deadlineAlertClass" v-if="task.deadline">
          <svg width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round">
            <circle cx="12" cy="12" r="10"></circle>
            <polyline points="12 6 12 12 16 14"></polyline>
          </svg>
          <span>{{ formatDate(task.deadline) }}</span>
        </div>

        <!-- Sprint status Active indicator (if task has active tag or similar) -->
        <div class="meta-indicator sprint-active" v-if="task.currentStatus === 1" title="Đang thực hiện tích cực">
          <svg width="10" height="10" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
            <path d="M21.5 2v6h-6M21.34 15.57a10 10 0 1 1-.57-8.38l5.67-5.67"/>
          </svg>
          <span>Active</span>
        </div>

        <!-- Comment Indicators -->
        <div class="meta-indicator comments-mention animate-fade" v-if="hasMentionForMe" title="Cần phản hồi (Có người tag bạn)">
          <svg width="10" height="10" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round">
            <path d="M21 15a2 2 0 0 1-2 2H7l-4 4V5a2 2 0 0 1 2-2h14a2 2 0 0 1 2 2z"></path>
          </svg>
          <span>Cần phản hồi</span>
        </div>
        <div class="meta-indicator comments-count" v-else-if="commentCount > 0" title="Bình luận thảo luận">
          <svg width="10" height="10" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round">
            <path d="M21 15a2 2 0 0 1-2 2H7l-4 4V5a2 2 0 0 1 2-2h14a2 2 0 0 1 2 2z"></path>
          </svg>
          <span>{{ commentCount }}</span>
        </div>
      </div>

      <!-- Assignee Avatar -->
      <div class="footer-right">
        <div 
          class="assignee-avatar" 
          v-if="assigneeUser" 
          :style="{ backgroundColor: getAvatarColor(assigneeUser.displayName || assigneeUser.username) }"
          :title="'Được giao cho: ' + (assigneeUser.displayName || assigneeUser.username)"
        >
          {{ (assigneeUser.displayName || assigneeUser.username).charAt(0).toUpperCase() }}
        </div>
        <div class="assignee-unassigned" v-else title="Chưa phân công">
          <svg width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
            <path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2"></path>
            <circle cx="12" cy="7" r="4"></circle>
          </svg>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue';
import { useAuthStore } from '../stores/auth';

const props = defineProps({
  task: {
    type: Object,
    required: true
  },
  members: {
    type: Array,
    default: () => []
  },
  comments: {
    type: Array,
    default: () => []
  }
});

const authStore = useAuthStore();

defineEmits(['edit']);

const cardRef = ref(null);
const mouseX = ref(0);
const mouseY = ref(0);
const isHovered = ref(false);

// Generate short readable Task Code e.g. T-1024 or T-18AF
const taskCode = computed(() => {
  if (props.task.taskId) {
    const idStr = String(props.task.taskId)
    // Find numeric or short hex identifier
    const short = idStr.substring(idStr.length - 4).toUpperCase()
    return `T-${short}`
  }
  return 'T-TASK'
});

// Match assignee details
const assigneeUser = computed(() => {
  const assId = props.task.assigneeId || props.task.assignedToId;
  if (!assId) return null;
  return props.members.find(m => m.id === assId || m.userId === assId);
});

// Subtasks calculations
const hasSubTasks = computed(() => {
  return props.task.subTasks && props.task.subTasks.length > 0;
});

const totalSubTasksCount = computed(() => {
  return props.task.subTasks ? props.task.subTasks.length : 0;
});

const completedSubTasksCount = computed(() => {
  if (!props.task.subTasks) return 0;
  return props.task.subTasks.filter(st => st.isCompleted).length;
});

const subtasksPercent = computed(() => {
  if (totalSubTasksCount.value === 0) return 0;
  return Math.round((completedSubTasksCount.value / totalSubTasksCount.value) * 100);
});

// Priority naming and classes
const priorityName = computed(() => {
  const priorities = { 0: 'Thấp (Low)', 1: 'Trung bình (Medium)', 2: 'Cao (High)', 3: 'Khẩn cấp (Urgent)' };
  return priorities[props.task.priority] || 'Trung bình (Medium)';
});

const priorityClass = computed(() => {
  const classes = { 0: 'prio-low', 1: 'prio-medium', 2: 'prio-high', 3: 'prio-urgent' };
  return classes[props.task.priority] || 'prio-medium';
});

// Deadline formatting
const deadlineAlertClass = computed(() => {
  if (!props.task.deadline) return '';
  const diff = new Date(props.task.deadline) - new Date();
  if (diff < 0) return 'overdue';
  if (diff < 24 * 60 * 60 * 1000) return 'urgent';
  return '';
});

const formatDate = (dateString) => {
  if (!dateString) return '';
  const date = new Date(dateString);
  return new Intl.DateTimeFormat('vi-VN', { month: 'short', day: 'numeric' }).format(date);
};

const commentCount = computed(() => props.comments ? props.comments.length : 0);

const hasMentionForMe = computed(() => {
  if (!props.comments || props.comments.length === 0) return false;
  
  const usernames = [];
  if (authStore.user?.username) usernames.push(authStore.user.username.toLowerCase());
  if (authStore.user?.displayName) {
    usernames.push(authStore.user.displayName.toLowerCase().replace(/\s+/g, ''));
  }
  
  if (usernames.length === 0) return false;
  
  const checkText = (text) => {
    if (!text) return false;
    const lowerText = text.toLowerCase();
    return usernames.some(name => lowerText.includes(`@${name}`));
  };
  
  for (const comment of props.comments) {
    if (checkText(comment.content)) return true;
    if (comment.replies) {
      for (const reply of comment.replies) {
        if (checkText(reply.content)) return true;
      }
    }
  }
  return false;
});

// 3D glow hover coordinates
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

// Avatar colors mapping
const avatarColors = ['#2563EB', '#7C3AED', '#DC2626', '#D97706', '#16A34A', '#0891B2', '#DB2777', '#4F46E5'];
function getAvatarColor(name) {
  if (!name) return avatarColors[0];
  let hash = 0;
  for (let i = 0; i < name.length; i++) hash = name.charCodeAt(i) + ((hash << 5) - hash);
  return avatarColors[Math.abs(hash) % avatarColors.length];
}
</script>

<style scoped>
.task-card {
  padding: 16px;
  margin-bottom: 14px;
  cursor: pointer;
  transition: transform 0.25s cubic-bezier(0.25, 1, 0.5, 1), 
              box-shadow 0.25s cubic-bezier(0.25, 1, 0.5, 1),
              border-color 0.25s ease;
  border-top: 3px solid transparent;
  position: relative;
  overflow: hidden;
  background: var(--bg-white-to-card);
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.task-card:hover {
  transform: translateY(-3px);
  border-color: rgba(255, 255, 255, 0.15);
}

/* Priority colors linked to top border */
.task-card.priority-0 { border-top-color: #94a3b8; }
.task-card.priority-1 { border-top-color: #3b82f6; }
.task-card.priority-2 { border-top-color: #f59e0b; }
.task-card.priority-3 { border-top-color: #ef4444; }

.task-card.priority-0:hover { box-shadow: 0 8px 24px rgba(148, 163, 184, 0.1); }
.task-card.priority-1:hover { box-shadow: 0 8px 24px rgba(59, 130, 246, 0.12); }
.task-card.priority-2:hover { box-shadow: 0 8px 24px rgba(245, 158, 11, 0.12); }
.task-card.priority-3:hover { box-shadow: 0 8px 24px rgba(239, 68, 68, 0.16); }

/* Radial glow on card hover */
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

.task-top-meta {
  display: flex;
  justify-content: space-between;
  align-items: center;
  z-index: 2;
}

.task-id {
  font-size: 10px;
  font-weight: 700;
  color: var(--text-muted);
  letter-spacing: 0.5px;
}

/* Priority Icons Styling */
.priority-icon {
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 2px;
  border-radius: 4px;
}

.priority-icon.prio-low { color: #8a99ad; }
.priority-icon.prio-medium { color: #3b82f6; }
.priority-icon.prio-high { color: #f59e0b; }
.priority-icon.prio-urgent { color: #ef4444; }

.task-title {
  font-size: 0.92rem;
  font-weight: 600;
  color: var(--color-text-primary);
  margin: 0;
  line-height: 1.4;
  z-index: 2;
}

.task-desc {
  font-size: 0.8rem;
  color: var(--color-text-secondary);
  line-height: 1.5;
  margin: 0;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
  z-index: 2;
}

/* Progress bar inside card */
.task-subtasks-progress {
  display: flex;
  flex-direction: column;
  gap: 4px;
  z-index: 2;
}

.progress-bar-container {
  height: 4px;
  background: var(--color-bg-secondary);
  border-radius: var(--radius-full);
  overflow: hidden;
}

.progress-bar-fill {
  height: 100%;
  background: var(--accent-primary);
  border-radius: var(--radius-full);
  transition: width 0.3s ease;
}

.progress-bar-text {
  font-size: 9px;
  font-weight: 600;
  color: var(--text-muted);
}

.task-footer {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding-top: 6px;
  border-top: 1px solid var(--border-color);
  z-index: 2;
}

.footer-left {
  display: flex;
  align-items: center;
  gap: 8px;
}

.meta-indicator {
  display: flex;
  align-items: center;
  gap: 4px;
  font-size: 10px;
  color: var(--color-text-tertiary);
  font-weight: 600;
}

.meta-indicator.deadline.urgent {
  color: var(--color-warning);
}

.meta-indicator.deadline.overdue {
  color: var(--color-danger);
}

.meta-indicator.sprint-active {
  background: var(--color-primary-light);
  color: var(--color-primary);
  padding: 1px 6px;
  border-radius: var(--radius-sm);
  font-size: 9px;
  font-weight: 700;
  text-transform: uppercase;
}

[data-theme='dark'] .meta-indicator.sprint-active {
  background: rgba(59, 130, 246, 0.2);
  color: #60a5fa;
}

.meta-indicator.comments-mention {
  background: #e11d48;
  color: white;
  padding: 1px 6px;
  border-radius: var(--radius-sm);
  font-size: 8px;
  font-weight: 700;
  animation: pulse-glow 2s infinite;
}

@keyframes pulse-glow {
  0% { box-shadow: 0 0 0 0 rgba(225, 29, 72, 0.4); }
  70% { box-shadow: 0 0 0 5px rgba(225, 29, 72, 0); }
  100% { box-shadow: 0 0 0 0 rgba(225, 29, 72, 0); }
}

.meta-indicator.comments-count {
  color: var(--color-primary);
}

[data-theme='dark'] .meta-indicator.comments-count {
  color: #60a5fa;
}

.footer-right {
  display: flex;
  align-items: center;
}

.assignee-avatar {
  width: 24px;
  height: 24px;
  border-radius: 50%;
  color: white;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 700;
  font-size: 10px;
  box-shadow: var(--shadow-xs);
}

.assignee-unassigned {
  width: 24px;
  height: 24px;
  border-radius: 50%;
  border: 1px dashed var(--border-color);
  display: flex;
  align-items: center;
  justify-content: center;
  color: var(--text-muted);
}
</style>
