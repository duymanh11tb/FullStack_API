<template>
  <div class="board-container">
    <!-- Confetti Canvas -->
    <canvas ref="confettiCanvas" class="confetti-canvas"></canvas>

    <!-- Floating Background Decorative Blobs -->
    <div class="bg-blobs">
      <div class="blob blob-1"></div>
      <div class="blob blob-2"></div>
      <div class="blob blob-3"></div>
    </div>

    <header class="board-header glass-panel">
      <div class="header-left">
        <h1>Bảng công việc</h1>
        <p class="subtitle">Trực quan hóa và quản lý các tác vụ của dự án</p>
      </div>

      <!-- Tab View Switcher -->
      <div class="view-toggles">
        <button 
          :class="['toggle-btn', { active: currentView === 'board' }]" 
          @click="currentView = 'board'"
        >
          <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
            <rect x="3" y="3" width="7" height="7" rx="1" />
            <rect x="14" y="3" width="7" height="7" rx="1" />
            <rect x="3" y="14" width="7" height="7" rx="1" />
            <rect x="14" y="14" width="7" height="7" rx="1" />
          </svg>
          Bảng Kanban
        </button>
        <button 
          :class="['toggle-btn', { active: currentView === 'analytics' }]" 
          @click="currentView = 'analytics'"
        >
          <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
            <line x1="18" y1="20" x2="18" y2="10" />
            <line x1="12" y1="20" x2="12" y2="4" />
            <line x1="6" y1="20" x2="6" y2="14" />
          </svg>
          Thống kê Board
        </button>
      </div>

      <BaseButton variant="primary" @click="openCreateModal" id="btn-new-task">
        <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round">
          <line x1="12" y1="5" x2="12" y2="19"></line>
          <line x1="5" y1="12" x2="19" y2="12"></line>
        </svg>
        Tác vụ mới
      </BaseButton>
    </header>

    <!-- TASK BOARD FILTER BAR -->
    <div class="board-filters glass-panel animate-slide-up" v-if="currentView === 'board'">
      <div class="filter-left">
        <!-- Search bar -->
        <div class="search-box">
          <svg class="search-icon" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
            <circle cx="11" cy="11" r="8"></circle>
            <line x1="21" y1="21" x2="16.65" y2="16.65"></line>
          </svg>
          <input 
            type="text" 
            v-model="searchQuery" 
            placeholder="Tìm kiếm tác vụ theo tên, mô tả..." 
            class="filter-search-input"
          />
          <button v-if="searchQuery" @click="searchQuery = ''" class="clear-search-btn">✕</button>
        </div>

        <!-- Priority Filter -->
        <div class="filter-select-wrapper">
          <label>Độ ưu tiên:</label>
          <select v-model="filterPriority" class="filter-select">
            <option value="">Tất cả</option>
            <option value="0">Thấp</option>
            <option value="1">Trung bình</option>
            <option value="2">Cao</option>
            <option value="3">Khẩn cấp</option>
          </select>
        </div>
      </div>

      <div class="filter-right">
        <!-- My Tasks toggle button -->
        <button 
          :class="['my-tasks-btn', { active: showMyTasksOnly }]" 
          @click="showMyTasksOnly = !showMyTasksOnly"
        >
          <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
            <path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2" />
            <circle cx="12" cy="7" r="4" />
          </svg>
          Chỉ xem việc của tôi
        </button>

        <!-- Reset filters -->
        <button 
          v-if="searchQuery || filterPriority !== '' || showMyTasksOnly" 
          @click="resetFilters" 
          class="reset-filters-btn"
        >
          Đặt lại bộ lọc
        </button>
      </div>
    </div>

    <!-- KANBAN BOARD VIEW -->
    <div class="view-panel" v-if="currentView === 'board'">
      <div class="board-layout" v-if="!taskStore.loading || taskStore.tasks.length > 0">
        <div class="column glass-panel" v-for="col in columns" :key="col.status">
          <div class="column-header">
            <h2 class="column-title">
              <span class="status-indicator" :style="{ backgroundColor: col.color, color: col.color }"></span>
              {{ col.title }}
            </h2>
            <span class="task-count">{{ getTasksByStatus(col.status).length }}</span>
          </div>
          
          <div 
            class="column-content" 
            :class="{ 'drag-over': activeDragCol === col.status }"
            @dragover.prevent 
            @dragenter="onDragEnter(col.status)"
            @dragleave="onDragLeave($event)"
            @drop="onDrop($event, col.status)"
          >
            <div 
              v-for="task in getTasksByStatus(col.status)" 
              :key="task.taskId"
              draggable="true"
              @dragstart="onDragStart($event, task)"
              class="draggable-item"
            >
              <TaskCard :task="task" @edit="openEditModal" />
            </div>
            
            <div v-if="getTasksByStatus(col.status).length === 0" class="empty-state">
              Không có tác vụ nào
            </div>
          </div>
        </div>
      </div>
      
      <div v-else class="board-layout">
        <SkeletonLoader type="board" :count="4" />
      </div>
    </div>

    <!-- ANALYTICS DASHBOARD VIEW -->
    <div class="view-panel analytics-panel animate-slide-up" v-else-if="currentView === 'analytics'">
      <div class="analytics-layout" v-if="taskStore.tasks.length > 0">
        <!-- Top Metrics Row -->
        <div class="metrics-grid">
          <!-- Overall rate ring -->
          <div class="metric-card glass-panel text-center">
            <h4>Tiến độ chung</h4>
            <div class="progress-ring-wrapper">
              <svg class="progress-ring" width="120" height="120">
                <circle class="ring-bg" stroke="rgba(255,255,255,0.06)" stroke-width="8" fill="transparent" r="50" cx="60" cy="60"/>
                <circle class="ring-fill" stroke="var(--status-done)" stroke-width="8" :stroke-dasharray="strokeDasharray" :stroke-dashoffset="strokeDashoffset" stroke-linecap="round" fill="transparent" r="50" cx="60" cy="60"/>
              </svg>
              <div class="ring-label">
                <span class="val">{{ percentTasksDone }}%</span>
                <span class="lbl">{{ doneTasksCount }}/{{ totalTasksCount }} tác vụ</span>
              </div>
            </div>
          </div>

          <!-- Total Counter -->
          <div class="metric-card glass-panel metric-counter">
            <div class="metric-icon" style="background: rgba(99, 102, 241, 0.15); color: var(--accent-primary);">
              <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
                <path d="M12 2v20M17 5H9.5a3.5 3.5 0 0 0 0 7h5a3.5 3.5 0 0 1 0 7H6" />
              </svg>
            </div>
            <div class="metric-info">
              <span class="num">{{ totalTasksCount }}</span>
              <span class="lbl">Tổng số tác vụ</span>
            </div>
          </div>

          <!-- Done tasks metrics info -->
          <div class="metric-card glass-panel metric-counter">
            <div class="metric-icon" style="background: rgba(16, 185, 129, 0.15); color: var(--status-done);">
              <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
                <polyline points="20 6 9 17 4 12" />
              </svg>
            </div>
            <div class="metric-info">
              <span class="num">{{ doneTasksCount }}</span>
              <span class="lbl">Tác vụ hoàn thành</span>
            </div>
          </div>
        </div>

        <!-- Middle Row: Custom SVG Charts -->
        <div class="charts-grid">
          <!-- Donut chart: Status Distribution -->
          <div class="chart-card glass-panel">
            <h3 class="chart-title">Trạng thái công việc</h3>
            <div class="chart-body donut-layout">
              <div class="svg-container">
                <svg width="150" height="150" viewBox="0 0 40 40" class="donut">
                  <circle class="donut-hole" cx="20" cy="20" r="15.91549430918954" fill="transparent"></circle>
                  <circle class="donut-ring" cx="20" cy="20" r="15.91549430918954" fill="transparent" stroke="rgba(255,255,255,0.06)" stroke-width="4.2"></circle>
                  
                  <circle v-for="(seg, i) in donutSegments" :key="i"
                    class="donut-segment" cx="20" cy="20" r="15.91549430918954" 
                    fill="transparent" :stroke="seg.color" stroke-width="4.2"
                    :stroke-dasharray="`${seg.percent} ${100 - seg.percent}`"
                    :stroke-dashoffset="seg.offset"
                    stroke-linecap="round">
                  </circle>
                </svg>
                <div class="donut-inner">
                  <span class="num">{{ totalTasksCount }}</span>
                  <span class="txt">Tác vụ</span>
                </div>
              </div>
              <div class="legend">
                <div class="legend-item" v-for="seg in donutSegments" :key="seg.name">
                  <span class="dot" :style="{ background: seg.color }"></span>
                  <span class="name">{{ seg.name }}</span>
                  <span class="count">{{ seg.count }} ({{ Math.round(seg.percent) }}%)</span>
                </div>
              </div>
            </div>
          </div>

          <!-- Bar chart: Priority distribution -->
          <div class="chart-card glass-panel">
            <h3 class="chart-title">Mức độ ưu tiên</h3>
            <div class="chart-body bar-layout">
              <div class="bar-chart-container">
                <div class="y-axis">
                  <span>{{ maxPriorityCount }}</span>
                  <span>{{ Math.round(maxPriorityCount / 2) }}</span>
                  <span>0</span>
                </div>
                <div class="bars-wrapper">
                  <div class="bar-col" v-for="bar in barData" :key="bar.name">
                    <div class="bar-val-hover">{{ bar.count }}</div>
                    <div class="bar-pill" :style="{ height: getBarHeight(bar.count), background: bar.color }"></div>
                    <span class="bar-lbl">{{ bar.name }}</span>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <div class="empty-state glass-panel py-12" v-else>
        Không có tác vụ nào để tạo báo cáo thống kê. Hãy tạo tác vụ mới đầu tiên!
      </div>
    </div>

    <!-- Details Modal -->
    <TaskModal 
      :is-open="isModalOpen" 
      :task="editingTask" 
      :loading="taskStore.loading"
      @close="closeModal" 
      @save="onSaveTask"
      @delete="onDeleteTask"
    />
  </div>
</template>

<script setup>
import { ref, computed, onMounted, watch } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { useTaskStore } from '../stores/taskStore';
import { useAuthStore } from '../stores/auth';
import TaskCard from '../components/TaskCard.vue';
import TaskModal from '../components/TaskModal.vue';
import LoadingSpinner from '../components/common/LoadingSpinner.vue';
import BaseButton from '../components/common/BaseButton.vue';
import SkeletonLoader from '../components/common/SkeletonLoader.vue';

const route = useRoute();
const router = useRouter();
const taskStore = useTaskStore();
const authStore = useAuthStore();

const currentView = ref('board'); // 'board' or 'analytics'

// Status Mapping based on Swagger enum
// 0: Todo, 1: InProgress, 2: Review, 3: Done, 4: Cancelled
const columns = [
  { status: 0, title: 'Cần làm (Todo)', color: 'var(--status-todo)' },
  { status: 1, title: 'Đang làm (In Progress)', color: 'var(--status-inprogress)' },
  { status: 2, title: 'Đánh giá (Review)', color: 'var(--status-review)' },
  { status: 3, title: 'Hoàn thành (Done)', color: 'var(--status-done)' }
];

const checkRouteTaskId = () => {
  const taskId = route.query.taskId;
  if (taskId) {
    const task = taskStore.tasks.find(t => t.taskId === taskId || t.id === taskId);
    if (task) {
      openEditModal(task);
    }
  }
};

onMounted(async () => {
  await taskStore.fetchTasks();
  checkRouteTaskId();
});

watch(() => route.query.taskId, () => {
  checkRouteTaskId();
});

watch(() => taskStore.tasks, () => {
  checkRouteTaskId();
}, { deep: true });

const getTasksByStatus = (status) => {
  return getTasksByStatusFiltered(status);
};

// ── Search & Filter Logic ──
const searchQuery = ref('');
const filterPriority = ref('');
const showMyTasksOnly = ref(false);

const resetFilters = () => {
  searchQuery.value = '';
  filterPriority.value = '';
  showMyTasksOnly.value = false;
};

const filteredTasks = computed(() => {
  let result = taskStore.tasks || [];
  
  if (searchQuery.value.trim()) {
    const q = searchQuery.value.toLowerCase().trim();
    result = result.filter(t => 
      (t.title && t.title.toLowerCase().includes(q)) || 
      (t.description && t.description.toLowerCase().includes(q))
    );
  }

  if (filterPriority.value !== '') {
    const p = parseInt(filterPriority.value);
    result = result.filter(t => t.priority === p);
  }

  if (showMyTasksOnly.value) {
    const currentUserId = authStore.user?.id || authStore.user?.Id;
    if (currentUserId) {
      result = result.filter(t => t.assignedToId === currentUserId);
    }
  }

  return result;
});

const getTasksByStatusFiltered = (status) => {
  return filteredTasks.value.filter(t => t.currentStatus === status) || [];
};

// ── Drag & Drop Styling Feedback ──
const activeDragCol = ref(null);

const onDragEnter = (status) => {
  activeDragCol.value = status;
};

const onDragLeave = (evt) => {
  if (evt.currentTarget && !evt.currentTarget.contains(evt.relatedTarget)) {
    activeDragCol.value = null;
  }
};

// ── Custom Confetti Canvas Animation ──
const confettiCanvas = ref(null);

function triggerConfetti() {
  const canvas = confettiCanvas.value;
  if (!canvas) return;
  const ctx = canvas.getContext('2d');
  canvas.width = window.innerWidth;
  canvas.height = window.innerHeight;

  const particles = [];
  const colors = ['#f43f5e', '#3b82f6', '#10b981', '#fbbf24', '#a855f7', '#06b6d4'];

  for (let i = 0; i < 120; i++) {
    particles.push({
      x: canvas.width / 2 + (Math.random() - 0.5) * 50,
      y: canvas.height / 2 + (Math.random() - 0.5) * 50,
      vx: (Math.random() - 0.5) * 16,
      vy: (Math.random() - 2) * 8 - 4,
      r: Math.random() * 4 + 4,
      color: colors[Math.floor(Math.random() * colors.length)],
      opacity: 1,
      rotation: Math.random() * 360,
      rotationSpeed: (Math.random() - 0.5) * 10
    });
  }

  function animate() {
    ctx.clearRect(0, 0, canvas.width, canvas.height);
    let active = false;

    particles.forEach(p => {
      if (p.opacity <= 0) return;
      active = true;

      p.x += p.vx;
      p.y += p.vy;
      p.vy += 0.35; // gravity
      p.vx *= 0.98; // friction
      p.opacity -= 0.012;
      p.rotation += p.rotationSpeed;

      ctx.save();
      ctx.translate(p.x, p.y);
      ctx.rotate(p.rotation * Math.PI / 180);
      ctx.fillStyle = p.color;
      ctx.globalAlpha = p.opacity;
      ctx.fillRect(-p.r, -p.r / 2, p.r * 2, p.r);
      ctx.restore();
    });

    if (active) {
      requestAnimationFrame(animate);
    } else {
      ctx.clearRect(0, 0, canvas.width, canvas.height);
    }
  }

  animate();
}

const onDragStart = (evt, task) => {
  evt.dataTransfer.dropEffect = 'move';
  evt.dataTransfer.effectAllowed = 'move';
  evt.dataTransfer.setData('taskId', task.taskId);
};

const onDrop = async (evt, newStatus) => {
  activeDragCol.value = null;
  const taskId = evt.dataTransfer.getData('taskId');
  const task = taskStore.tasks.find(t => t.taskId === taskId);
  
  if (task && task.currentStatus !== newStatus) {
    const oldStatus = task.currentStatus;
    task.currentStatus = newStatus;
    
    try {
      await taskStore.updateTaskStatus(taskId, newStatus);
      if (newStatus === 3) {
        triggerConfetti();
      }
    } catch (err) {
      task.currentStatus = oldStatus;
      alert('Không thể cập nhật trạng thái tác vụ');
    }
  }
};

const isModalOpen = ref(false);
const editingTask = ref(null);

const openCreateModal = () => {
  editingTask.value = null;
  isModalOpen.value = true;
};

const openEditModal = (task) => {
  editingTask.value = task;
  isModalOpen.value = true;
};

const closeModal = () => {
  isModalOpen.value = false;
  editingTask.value = null;
  if (route.query.taskId) {
    router.replace({ query: { ...route.query, taskId: undefined } });
  }
};

const onSaveTask = async (taskData) => {
  try {
    if (taskData.taskId) {
      await taskStore.updateTask(taskData.taskId, taskData);
    } else {
      await taskStore.createTask(taskData);
    }
    closeModal();
  } catch (err) {
    const errorMsg = err.response?.data?.message || 
                     (err.response?.data ? JSON.stringify(err.response.data) : '') || 
                     err.message;
    alert('Lỗi lưu tác vụ: ' + errorMsg);
  }
};

const onDeleteTask = async (taskId) => {
  try {
    await taskStore.deleteTask(taskId);
    closeModal();
  } catch (err) {
    alert('Lỗi xóa tác vụ: ' + err.message);
  }
};

// ── Analytics calculations ──
const totalTasksCount = computed(() => taskStore.tasks.length);
const doneTasksCount = computed(() => taskStore.tasks.filter(t => t.currentStatus === 3).length);

const percentTasksDone = computed(() => {
  if (totalTasksCount.value === 0) return 0;
  return Math.round((doneTasksCount.value / totalTasksCount.value) * 100);
});

const ringRadius = 50;
const ringCircumference = 2 * Math.PI * ringRadius;
const strokeDasharray = computed(() => `${ringCircumference} ${ringCircumference}`);
const strokeDashoffset = computed(() => {
  const offset = ringCircumference - (percentTasksDone.value / 100) * ringCircumference;
  return isNaN(offset) ? ringCircumference : offset;
});

// Donut segments calculations
const donutSegments = computed(() => {
  if (totalTasksCount.value === 0) return [];
  
  const statusGroups = [
    { name: 'Cần làm (Todo)', value: 0, color: 'var(--status-todo)' },
    { name: 'Đang làm (In Progress)', value: 1, color: 'var(--status-inprogress)' },
    { name: 'Đánh giá (Review)', value: 2, color: 'var(--status-review)' },
    { name: 'Hoàn thành (Done)', value: 3, color: 'var(--status-done)' },
    { name: 'Đã hủy (Cancelled)', value: 4, color: 'var(--status-cancelled, #64748b)' }
  ];

  let offsetAccumulator = 25;
  
  return statusGroups.map(group => {
    const count = taskStore.tasks.filter(t => t.currentStatus === group.value).length;
    const percent = (count / totalTasksCount.value) * 100;
    const offset = offsetAccumulator;
    offsetAccumulator = (offsetAccumulator - percent + 100) % 100;
    
    return {
      name: group.name,
      count,
      percent,
      offset,
      color: group.color
    };
  }).filter(seg => seg.count > 0);
});

// Priority distribution calculations
const barData = computed(() => {
  const priorityGroups = [
    { name: 'Thấp', value: 0, color: '#94a3b8' },
    { name: 'Trung bình', value: 1, color: '#3b82f6' },
    { name: 'Cao', value: 2, color: '#f59e0b' },
    { name: 'Khẩn cấp', value: 3, color: '#ef4444' }
  ];

  return priorityGroups.map(group => {
    const count = taskStore.tasks.filter(t => t.priority === group.value).length;
    return {
      name: group.name,
      count,
      color: group.color
    };
  });
});

const maxPriorityCount = computed(() => {
  const counts = barData.value.map(b => b.count);
  const max = Math.max(...counts);
  return max === 0 ? 4 : max;
});

function getBarHeight(count) {
  if (count === 0) return '6px';
  return `${(count / maxPriorityCount.value) * 100}%`;
}
</script>

<style scoped>
.board-container {
  padding: 24px 32px;
  min-height: 100vh;
  display: flex;
  flex-direction: column;
  position: relative;
  overflow: hidden;
}

/* Background blob decorations */
.bg-blobs {
  position: absolute;
  inset: 0;
  pointer-events: none;
  z-index: 0;
}

.blob {
  position: absolute;
  border-radius: 50%;
  filter: blur(120px);
  opacity: 0.15;
  animation: float 20s infinite alternate ease-in-out;
}

.blob-1 {
  width: 350px;
  height: 350px;
  background: var(--color-primary);
  top: -10%;
  left: 20%;
}

.blob-2 {
  width: 400px;
  height: 400px;
  background: var(--color-success);
  bottom: 10%;
  right: 10%;
  animation-delay: -5s;
}

.blob-3 {
  width: 300px;
  height: 300px;
  background: #7c3aed;
  top: 40%;
  left: -5%;
  animation-delay: -10s;
}

@keyframes float {
  0% { transform: translate(0, 0) scale(1); }
  100% { transform: translate(40px, 60px) scale(1.15); }
}

.board-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 20px 24px;
  border-radius: var(--radius-xl);
  margin-bottom: 24px;
  background: var(--bg-white-to-card);
  position: relative;
  z-index: 10;
}

.header-left h1 {
  font-size: var(--font-size-2xl);
  font-weight: 800;
  margin-bottom: 4px;
  color: var(--color-text-primary);
}

.subtitle {
  color: var(--color-text-secondary);
  font-size: var(--font-size-sm);
}

/* View switcher styling */
.view-toggles {
  display: flex;
  background: rgba(255, 255, 255, 0.05);
  border: 1px solid var(--border-color);
  padding: 4px;
  border-radius: var(--radius-lg);
  gap: 4px;
}

[data-theme='dark'] .view-toggles {
  background: rgba(15, 23, 42, 0.4);
}

.toggle-btn {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 8px 16px;
  border-radius: var(--radius-md);
  border: none;
  font-size: var(--font-size-sm);
  font-weight: var(--font-weight-semibold);
  color: var(--color-text-secondary);
  background: transparent;
  cursor: pointer;
  transition: all var(--transition-fast);
}

.toggle-btn:hover {
  color: var(--color-text-primary);
}

.toggle-btn.active {
  background: var(--gradient-primary);
  color: white;
  box-shadow: 0 4px 12px rgba(37, 99, 235, 0.2);
}

/* Base button header adjust */
#btn-new-task {
  display: flex;
  align-items: center;
  gap: 6px;
}

/* View panels */
.view-panel {
  flex: 1;
  display: flex;
  flex-direction: column;
  position: relative;
  z-index: 10;
}

.animate-slide-up {
  animation: slideUp 0.4s cubic-bezier(0.16, 1, 0.3, 1);
}

@keyframes slideUp {
  from { opacity: 0; transform: translateY(12px); }
  to { opacity: 1; transform: translateY(0); }
}

/* Kanban board layout */
.board-layout {
  display: flex;
  gap: 20px;
  flex: 1;
  overflow-x: auto;
  align-items: flex-start;
  padding-bottom: 20px;
}

.column {
  min-width: 290px;
  width: 320px;
  max-height: calc(100vh - 200px);
  display: flex;
  flex-direction: column;
  background: var(--bg-white-to-card);
  border-radius: var(--radius-xl);
  padding: 4px;
}

.column-header {
  padding: 16px 20px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  border-bottom: 1px solid var(--border-color);
}

.column-title {
  font-size: 0.98rem;
  display: flex;
  align-items: center;
  gap: 8px;
  font-weight: 700;
  color: var(--color-text-primary);
}

.status-indicator {
  width: 8px;
  height: 8px;
  border-radius: 50%;
  box-shadow: 0 0 6px currentColor;
}

.task-count {
  background: var(--color-primary-light);
  color: var(--color-primary);
  padding: 2px 8px;
  border-radius: var(--radius-full);
  font-size: 0.75rem;
  font-weight: 700;
}

[data-theme='dark'] .task-count {
  background: rgba(59, 130, 246, 0.2);
  color: #60a5fa;
}

.column-content {
  padding: 16px 14px;
  flex: 1;
  overflow-y: auto;
  min-height: 200px;
  display: flex;
  flex-direction: column;
}

.draggable-item {
  cursor: grab;
}

.draggable-item:active {
  cursor: grabbing;
}

.empty-state {
  text-align: center;
  padding: 40px 0;
  color: var(--color-text-tertiary);
  font-style: italic;
  font-size: 0.88rem;
}

.loading-state {
  display: flex;
  justify-content: center;
  align-items: center;
  padding: 80px 0;
  border-radius: var(--radius-xl);
  background: var(--bg-white-to-card);
}

/* Analytics dashboard view */
.analytics-layout {
  display: flex;
  flex-direction: column;
  gap: 24px;
}

.metrics-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
  gap: 20px;
}

.metric-card {
  padding: 24px;
  display: flex;
  align-items: center;
  background: var(--bg-white-to-card);
  border-radius: var(--radius-xl);
}

.metric-card.text-center {
  flex-direction: column;
  text-align: center;
}

.metric-card h4 {
  font-size: var(--font-size-sm);
  color: var(--color-text-secondary);
  text-transform: uppercase;
  letter-spacing: 0.5px;
  margin-bottom: var(--space-4);
  font-weight: 700;
}

.progress-ring-wrapper {
  display: flex;
  align-items: center;
  justify-content: center;
  position: relative;
  height: 120px;
}

.progress-ring {
  transform: rotate(-90deg);
}

.ring-bg {
  stroke: var(--color-bg-secondary);
}

[data-theme='dark'] .ring-bg {
  stroke: rgba(255, 255, 255, 0.05);
}

.ring-fill {
  transition: stroke-dashoffset 0.45s ease-out;
}

.ring-label {
  position: absolute;
  display: flex;
  flex-direction: column;
  align-items: center;
}

.ring-label .val {
  font-size: var(--font-size-2xl);
  font-weight: 850;
  color: var(--color-text-primary);
}

.ring-label .lbl {
  font-size: 11px;
  color: var(--color-text-tertiary);
  font-weight: 500;
  margin-top: 2px;
}

.metric-counter {
  gap: 20px;
}

.metric-icon {
  width: 52px;
  height: 52px;
  border-radius: var(--radius-lg);
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
  box-shadow: var(--shadow-sm);
}

.metric-info {
  display: flex;
  flex-direction: column;
}

.metric-info .num {
  font-size: var(--font-size-3xl);
  font-weight: 800;
  color: var(--color-text-primary);
  line-height: 1.1;
  margin-bottom: 4px;
}

.metric-info .lbl {
  font-size: var(--font-size-sm);
  color: var(--color-text-secondary);
  font-weight: 500;
}

/* Charts Grid */
.charts-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 20px;
}

@media (max-width: 820px) {
  .charts-grid {
    grid-template-columns: 1fr;
  }
}

.chart-card {
  padding: 24px;
  background: var(--bg-white-to-card);
  border-radius: var(--radius-xl);
  min-height: 250px;
  display: flex;
  flex-direction: column;
}

.chart-title {
  font-size: var(--font-size-base);
  font-weight: 800;
  color: var(--color-text-primary);
  margin-bottom: 20px;
  border-left: 3px solid var(--accent-primary);
  padding-left: 8px;
}

.chart-body {
  flex: 1;
  display: flex;
  align-items: center;
}

/* Donut chart layout */
.donut-layout {
  justify-content: space-around;
  gap: 20px;
  flex-wrap: wrap;
}

.svg-container {
  position: relative;
  width: 150px;
  height: 150px;
}

.donut {
  width: 100%;
  height: 100%;
}

.donut-segment {
  transform: rotate(0deg);
  transform-origin: center;
  transition: stroke-dasharray 0.35s ease;
}

.donut-inner {
  position: absolute;
  inset: 0;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
}

.donut-inner .num {
  font-size: var(--font-size-2xl);
  font-weight: 800;
  color: var(--color-text-primary);
}

.donut-inner .txt {
  font-size: 10px;
  color: var(--color-text-tertiary);
  text-transform: uppercase;
  font-weight: 600;
}

.legend {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.legend-item {
  display: flex;
  align-items: center;
  font-size: var(--font-size-xs);
  color: var(--color-text-secondary);
}

.legend-item .dot {
  width: 8px;
  height: 8px;
  border-radius: 50%;
  margin-right: 8px;
  flex-shrink: 0;
  box-shadow: 0 0 4px currentColor;
}

.legend-item .name {
  flex: 1;
  font-weight: 500;
  margin-right: 16px;
}

.legend-item .count {
  font-weight: 700;
  color: var(--color-text-primary);
}

/* Bar chart layout */
.bar-layout {
  width: 100%;
}

.bar-chart-container {
  display: flex;
  width: 100%;
  height: 150px;
  gap: 12px;
  position: relative;
}

.y-axis {
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  font-size: 10px;
  color: var(--color-text-tertiary);
  width: 20px;
  border-right: 1px solid var(--border-color);
  padding-right: 6px;
  height: 110px;
}

.bars-wrapper {
  flex: 1;
  display: flex;
  justify-content: space-around;
  align-items: flex-end;
  height: 110px;
}

.bar-col {
  display: flex;
  flex-direction: column;
  align-items: center;
  width: 44px;
  position: relative;
}

.bar-pill {
  width: 24px;
  border-radius: var(--radius-md) var(--radius-md) 0 0;
  transition: height 0.4s cubic-bezier(0.16, 1, 0.3, 1);
  cursor: pointer;
  min-height: 4px;
}

.bar-pill:hover {
  filter: brightness(1.1);
}

.bar-val-hover {
  position: absolute;
  bottom: calc(100% + 4px);
  background: var(--color-text-primary);
  color: var(--bg-white-to-card);
  font-size: 10px;
  font-weight: 700;
  padding: 2px 6px;
  border-radius: 4px;
  opacity: 0;
  transform: translateY(4px);
  transition: all var(--transition-fast);
  pointer-events: none;
}

.bar-col:hover .bar-val-hover {
  opacity: 1;
  transform: translateY(0);
}

.bar-lbl {
  position: absolute;
  top: 120px;
  font-size: 9px;
  font-weight: 700;
  color: var(--color-text-tertiary);
  text-transform: uppercase;
  white-space: nowrap;
}

/* Filter Bar Styles */
.board-filters {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 14px 24px;
  border-radius: var(--radius-xl);
  margin-bottom: 24px;
  background: var(--bg-white-to-card);
  gap: var(--space-4);
  flex-wrap: wrap;
  z-index: 15;
  position: relative;
}

.filter-left {
  display: flex;
  align-items: center;
  gap: 16px;
  flex: 1;
  min-width: 280px;
  flex-wrap: wrap;
}

.search-box {
  display: flex;
  align-items: center;
  background: var(--color-bg-secondary);
  border: 1px solid var(--border-color);
  padding: 6px 12px;
  border-radius: var(--radius-md);
  flex: 1;
  max-width: 380px;
  min-width: 200px;
  position: relative;
}

[data-theme='dark'] .search-box {
  background: rgba(15, 23, 42, 0.4);
}

.search-icon {
  color: var(--text-muted);
  margin-right: 8px;
  flex-shrink: 0;
}

.filter-search-input {
  border: none;
  background: transparent;
  outline: none;
  color: var(--text-primary);
  font-size: var(--font-size-sm);
  width: 100%;
}

.clear-search-btn {
  background: transparent;
  border: none;
  color: var(--text-muted);
  font-size: 11px;
  cursor: pointer;
}

.filter-select-wrapper {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: var(--font-size-sm);
  color: var(--text-secondary);
  font-weight: 500;
}

.filter-select {
  background: var(--color-bg-secondary);
  border: 1px solid var(--border-color);
  padding: 6px 12px;
  border-radius: var(--radius-md);
  color: var(--text-primary);
  outline: none;
  font-weight: 600;
  cursor: pointer;
}

[data-theme='dark'] .filter-select {
  background: rgba(15, 23, 42, 0.4);
}

.filter-right {
  display: flex;
  align-items: center;
  gap: 12px;
}

.my-tasks-btn {
  display: flex;
  align-items: center;
  gap: 6px;
  padding: 8px 16px;
  border-radius: var(--radius-md);
  border: 1px solid var(--border-color);
  background: var(--color-bg-secondary);
  color: var(--text-secondary);
  font-size: var(--font-size-sm);
  font-weight: 600;
  transition: all var(--transition-fast);
}

[data-theme='dark'] .my-tasks-btn {
  background: rgba(15, 23, 42, 0.4);
}

.my-tasks-btn:hover {
  border-color: var(--color-border-hover);
  color: var(--text-primary);
}

.my-tasks-btn.active {
  background: var(--gradient-primary);
  color: white;
  border-color: transparent;
  box-shadow: 0 4px 12px rgba(37, 99, 235, 0.2);
}

.reset-filters-btn {
  background: transparent;
  color: var(--color-danger);
  font-size: var(--font-size-sm);
  font-weight: 600;
  border: none;
  cursor: pointer;
  transition: all var(--transition-fast);
}

.reset-filters-btn:hover {
  text-decoration: underline;
}

.confetti-canvas {
  position: fixed;
  inset: 0;
  width: 100%;
  height: 100%;
  pointer-events: none;
  z-index: 1000;
}
</style>
