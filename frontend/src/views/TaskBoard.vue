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

    <!-- Top Navigation Header (Mockup styled) -->
    <header class="board-header glass-panel animate-fade-in">
      <div class="header-left">
        <!-- Project dropdown selector -->
        <div class="project-selector-wrapper" ref="projectDropdownRef">
          <button class="project-dropdown-btn" @click="showProjectDropdown = !showProjectDropdown" id="btn-project-select">
            <span class="project-indicator-dot" :style="{ background: currentProjectColor }"></span>
            <span class="project-name-selected">{{ currentProjectName }}</span>
            <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
              <polyline points="6 9 12 15 18 9" />
            </svg>
          </button>
          
          <transition name="fade">
            <div v-if="showProjectDropdown" class="project-dropdown-list">
              <div class="dropdown-header">Chọn dự án làm việc</div>
              <div 
                v-for="proj in projectStore.projects" 
                :key="proj.id" 
                class="dropdown-item"
                :class="{ active: proj.id === activeProjectId }"
                @click="selectProject(proj.id)"
              >
                <span class="project-color-dot" :style="{ background: proj.color || '#2563EB' }"></span>
                <span>{{ proj.name }}</span>
              </div>
              <div v-if="projectStore.projects.length === 0" class="dropdown-empty">
                Chưa có dự án nào
              </div>
            </div>
          </transition>
        </div>

        <!-- Search Bar inside header -->
        <div class="header-search-box">
          <svg class="search-icon" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
            <circle cx="11" cy="11" r="8"></circle>
            <line x1="21" y1="21" x2="16.65" y2="16.65"></line>
          </svg>
          <input 
            type="text" 
            v-model="searchQuery" 
            placeholder="Tìm kiếm tác vụ..." 
            class="header-search-input"
          />
          <button v-if="searchQuery" @click="searchQuery = ''" class="clear-search-btn">✕</button>
        </div>
      </div>

      <div class="header-right">
        <!-- View Toggle Tabs -->
        <div class="view-toggles">
          <button 
            :class="['toggle-btn', { active: currentView === 'board' }]" 
            @click="currentView = 'board'"
          >
            <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
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
            <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
              <line x1="18" y1="20" x2="18" y2="10" />
              <line x1="12" y1="20" x2="12" y2="4" />
              <line x1="6" y1="20" x2="6" y2="14" />
            </svg>
            Thống kê
          </button>
        </div>

        <!-- Add Task Button -->
        <BaseButton variant="primary" @click="openCreateModal" id="btn-new-task">
          Tạo tác vụ
        </BaseButton>
      </div>
    </header>

    <!-- Kanban Sub-Header: Active Sprint Info & Filter/Sort buttons -->
    <div class="board-subheader animate-fade-in" v-if="currentView === 'board'">
      <div class="subheader-left">
        <!-- Active Sprint Badge -->
        <div class="sprint-active-badge">
          <svg class="sprint-icon" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
            <polygon points="13 2 3 14 12 14 11 22 21 10 12 10 13 2"></polygon>
          </svg>
          <span class="sprint-name">{{ activeSprintText }}</span>
        </div>

        <!-- Sprint Members avatars -->
        <div class="sprint-members-avatars" v-if="projectMembers.length > 0">
          <div 
            v-for="(member, index) in projectMembers.slice(0, 4)" 
            :key="member.id || index" 
            class="member-avatar"
            :style="{ background: getAvatarColor(member.displayName || member.username) }"
            :title="member.displayName || member.username"
          >
            {{ (member.displayName || member.username || 'U').charAt(0).toUpperCase() }}
          </div>
          <div v-if="projectMembers.length > 4" class="member-avatar plus-more">
            +{{ projectMembers.length - 4 }}
          </div>
        </div>
      </div>

      <div class="subheader-right">
        <!-- Filter toggle button -->
        <button 
          class="filter-toggle-btn" 
          :class="{ active: showFiltersPanel }" 
          @click="showFiltersPanel = !showFiltersPanel"
        >
          <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
            <polygon points="22 3 2 3 10 12.46 10 19 14 21 14 12.46 22 3"></polygon>
          </svg>
          Lọc
        </button>

        <!-- Sort Select Box -->
        <div class="sort-selector-wrapper">
          <span class="sort-lbl">Sắp xếp:</span>
          <select v-model="sortBy" class="sort-select">
            <option value="default">Mặc định</option>
            <option value="priority">Độ ưu tiên</option>
            <option value="deadline">Hạn chót</option>
          </select>
        </div>
      </div>
    </div>

    <!-- Sliding Filters Panel -->
    <transition name="slide-down">
      <div class="filters-panel glass-panel" v-if="showFiltersPanel && currentView === 'board'">
        <div class="filter-group">
          <label>Lọc độ ưu tiên:</label>
          <select v-model="filterPriority" class="filter-select">
            <option value="">Tất cả mức độ</option>
            <option value="0">Thấp</option>
            <option value="1">Trung bình</option>
            <option value="2">Cao</option>
            <option value="3">Khẩn cấp</option>
          </select>
        </div>
        <div class="filter-group">
          <label class="checkbox-label">
            <input type="checkbox" v-model="showMyTasksOnly" />
            <span>Chỉ xem tác vụ được phân công cho tôi</span>
          </label>
        </div>
        <button v-if="filterPriority !== '' || showMyTasksOnly" @click="resetFilters" class="btn-reset-filters">
          Xóa bộ lọc
        </button>
      </div>
    </transition>

    <!-- KANBAN BOARD VIEW (5 Columns Layout) -->
    <div class="view-panel" v-if="currentView === 'board'">
      <div class="board-layout" v-if="!taskStore.loading || taskStore.tasks.length > 0">
        <!-- 5 Columns loop -->
        <div 
          class="column glass-panel" 
          v-for="col in kanbanColumns" 
          :key="col.id"
          :style="{ borderTop: `3px solid ${col.color}` }"
        >
          <div class="column-header">
            <h2 class="column-title">
              <span class="status-indicator" :style="{ backgroundColor: col.color, color: col.color }"></span>
              {{ col.title }}
            </h2>
            <span class="task-count">{{ getTasksByColumn(col.id).length }}</span>
          </div>
          
          <div 
            class="column-content" 
            :class="{ 'drag-over': activeDragCol === col.id }"
            @dragover.prevent 
            @dragenter="onDragEnter(col.id)"
            @dragleave="onDragLeave($event)"
            @drop="onDrop($event, col.id)"
          >
            <div 
              v-for="task in getTasksByColumn(col.id)" 
              :key="task.taskId"
              draggable="true"
              @dragstart="onDragStart($event, task)"
              class="draggable-item"
            >
              <TaskCard :task="task" :members="projectMembers" :comments="taskComments[task.taskId] || []" @edit="openEditModal" />
            </div>
            
            <div v-if="getTasksByColumn(col.id).length === 0" class="empty-column-state">
              Kéo tác vụ vào đây
            </div>
          </div>
        </div>
      </div>
      
      <div v-else class="board-layout">
        <SkeletonLoader type="board" :count="5" />
      </div>
    </div>

    <!-- ANALYTICS DASHBOARD VIEW -->
    <div class="view-panel analytics-panel animate-slide-up" v-else-if="currentView === 'analytics'">
      <div class="analytics-layout" v-if="projectTasks.length > 0">
        <!-- Top Metrics Row -->
        <div class="metrics-grid">
          <!-- Overall rate ring -->
          <div class="metric-card glass-panel text-center">
            <h4>Tiến độ dự án chung</h4>
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
              <span class="lbl">Tác vụ dự án</span>
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
                  <circle class="donut-hole" cx="20" cy="20" r="15.9154" fill="transparent"></circle>
                  <circle class="donut-ring" cx="20" cy="20" r="15.9154" fill="transparent" stroke="rgba(255,255,255,0.06)" stroke-width="4.2"></circle>
                  
                  <circle v-for="(seg, i) in donutSegments" :key="i"
                    class="donut-segment" cx="20" cy="20" r="15.9154" 
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
        Dự án chưa có tác vụ nào để lập biểu đồ thống kê.
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
import { useProjectStore } from '../stores/projects';
import { useAuthStore } from '../stores/auth';
import { getMembers, getSprints } from '../api/projectApi';
import { getCommentsByTask } from '../api/notifyApi';
import TaskCard from '../components/TaskCard.vue';
import TaskModal from '../components/TaskModal.vue';
import LoadingSpinner from '../components/common/LoadingSpinner.vue';
import BaseButton from '../components/common/BaseButton.vue';
import SkeletonLoader from '../components/common/SkeletonLoader.vue';

const route = useRoute();
const router = useRouter();
const taskStore = useTaskStore();
const projectStore = useProjectStore();
const authStore = useAuthStore();

const currentView = ref('board'); // 'board' or 'analytics'
const activeProjectId = ref('');
const showProjectDropdown = ref(false);
const projectMembers = ref([]);
const sprints = ref([]);
const taskComments = ref({});

const showFiltersPanel = ref(false);
const sortBy = ref('default');
const projectDropdownRef = ref(null);

// 5 Columns mapping for Agile Kanban layout
// 0: Todo (split into Backlog and Todo based on assigneeId), 1: InProgress, 2: Review, 3: Done
const kanbanColumns = [
  { id: 'backlog', title: 'Backlog', color: '#94a3b8' },
  { id: 'todo', title: 'Cần làm', color: 'var(--status-todo)' },
  { id: 'inprogress', title: 'Đang thực hiện', color: 'var(--status-inprogress)' },
  { id: 'review', title: 'Đang kiểm tra', color: 'var(--status-review)' },
  { id: 'done', title: 'Hoàn thành', color: 'var(--status-done)' }
];

// Project selector text helpers
const currentProjectName = computed(() => {
  const proj = projectStore.projects.find(p => p.id === activeProjectId.value);
  return proj ? proj.name : 'Chọn dự án';
});

const currentProjectColor = computed(() => {
  const proj = projectStore.projects.find(p => p.id === activeProjectId.value);
  return proj ? (proj.color || '#2563EB') : '#2563EB';
});

// Sprint Active badges
const activeSprintText = computed(() => {
  const active = sprints.value.find(s => s.status === 'Active');
  if (active) {
    return `${active.name}: ${active.goal || 'Giai đoạn phát triển'}`;
  }
  return sprints.value.length > 0 ? 'Dự án chưa có Sprint hoạt động' : 'Chưa lập Sprint';
});

// Load Sprints & members of the selected project
async function fetchProjectMeta(projectId) {
  if (!projectId) return;
  
  // Load Sprints
  try {
    const sRes = await getSprints(projectId);
    sprints.value = sRes.data?.data || sRes.data || [];
  } catch (err) {
    console.error('Không thể tải Sprints:', err);
    sprints.value = [];
  }

  // Load Members
  try {
    const mRes = await getMembers(projectId);
    projectMembers.value = mRes.data?.data || mRes.data || [];
  } catch (err) {
    console.error('Không thể tải thành viên:', err);
    projectMembers.value = [];
  }
  
  // Load comments
  await loadAllTasksComments();
}

// Handle project switching
async function selectProject(projectId) {
  activeProjectId.value = projectId;
  showProjectDropdown.value = false;
  
  // Update route query parameters
  router.replace({ query: { ...route.query, projectId } });
  
  await fetchProjectMeta(projectId);
}

// Click outside to close dropdown
function handleClickOutside(e) {
  if (projectDropdownRef.value && !projectDropdownRef.value.contains(e.target)) {
    showProjectDropdown.value = false;
  }
}

// Filter tasks belonging only to active board (project)
const projectTasks = computed(() => {
  if (!activeProjectId.value) return [];
  return taskStore.tasks.filter(t => t.boardId === activeProjectId.value);
});

async function loadAllTasksComments() {
  if (!projectTasks.value || projectTasks.value.length === 0) return;
  projectTasks.value.forEach(async (task) => {
    try {
      const res = await getCommentsByTask(task.taskId);
      taskComments.value[task.taskId] = res.data || [];
    } catch (err) {
      console.error('Không thể tải bình luận cho task:', task.taskId, err);
    }
  });
}

watch(() => projectTasks.value, () => {
  loadAllTasksComments();
}, { deep: false });

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
  let result = projectTasks.value;
  
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
      result = result.filter(t => t.assigneeId === currentUserId || t.assignedToId === currentUserId);
    }
  }

  return result;
});

// Sort filtered tasks
const sortedTasks = computed(() => {
  let result = [...filteredTasks.value];
  if (sortBy.value === 'priority') {
    result.sort((a, b) => b.priority - a.priority);
  } else if (sortBy.value === 'deadline') {
    result.sort((a, b) => {
      if (!a.deadline) return 1;
      if (!b.deadline) return -1;
      return new Date(a.deadline) - new Date(b.deadline);
    });
  }
  return result;
});

// Distribute tasks into columns based on Agile statuses
const getTasksByColumn = (colId) => {
  const tasksList = sortedTasks.value;
  switch (colId) {
    case 'backlog':
      return tasksList.filter(t => t.currentStatus === 0 && !t.assigneeId && !t.assignedToId);
    case 'todo':
      return tasksList.filter(t => t.currentStatus === 0 && (t.assigneeId || t.assignedToId));
    case 'inprogress':
      return tasksList.filter(t => t.currentStatus === 1);
    case 'review':
      return tasksList.filter(t => t.currentStatus === 2);
    case 'done':
      return tasksList.filter(t => t.currentStatus === 3);
    default:
      return [];
  }
};

// ── Drag & Drop Logic ──
const activeDragCol = ref(null);

const onDragEnter = (colId) => {
  activeDragCol.value = colId;
};

const onDragLeave = (evt) => {
  if (evt.currentTarget && !evt.currentTarget.contains(evt.relatedTarget)) {
    activeDragCol.value = null;
  }
};

const onDragStart = (evt, task) => {
  evt.dataTransfer.dropEffect = 'move';
  evt.dataTransfer.effectAllowed = 'move';
  evt.dataTransfer.setData('taskId', task.taskId);
};

const onDrop = async (evt, colId) => {
  activeDragCol.value = null;
  const taskId = evt.dataTransfer.getData('taskId');
  const task = taskStore.tasks.find(t => t.taskId === taskId);
  if (!task) return;

  // Map colId to database status fields
  let targetStatus = 0;
  let shouldClearAssignee = false;

  if (colId === 'backlog') {
    targetStatus = 0;
    shouldClearAssignee = true;
  } else if (colId === 'todo') {
    targetStatus = 0;
  } else if (colId === 'inprogress') {
    targetStatus = 1;
  } else if (colId === 'review') {
    targetStatus = 2;
  } else if (colId === 'done') {
    targetStatus = 3;
  }

  // Optimize state updates locally for smooth dragging response
  const oldStatus = task.currentStatus;
  const oldAssigneeId = task.assigneeId;
  
  task.currentStatus = targetStatus;
  if (shouldClearAssignee) {
    task.assigneeId = null;
    task.assignedToId = null;
  } else if (colId === 'todo' && !task.assigneeId && !task.assignedToId) {
    // If dragged from backlog to todo, auto assign to current user
    const currentUserId = authStore.user?.id || authStore.user?.Id;
    task.assigneeId = currentUserId;
  }

  try {
    if (shouldClearAssignee || (colId === 'todo' && task.assigneeId !== oldAssigneeId)) {
      // Use updateTask API when assignee changes
      const payload = { ...task };
      await taskStore.updateTask(taskId, payload);
    } else {
      // Use simple status update API otherwise
      await taskStore.updateTaskStatus(taskId, targetStatus);
    }
    
    if (colId === 'done' && oldStatus !== 3) {
      triggerConfetti();
    }
  } catch (err) {
    // Rollback state on api errors
    task.currentStatus = oldStatus;
    task.assigneeId = oldAssigneeId;
    alert('Không thể kéo thả cập nhật tác vụ: ' + err.message);
  }
};

// Routing queries for direct task card focuses
const checkRouteParams = () => {
  const taskId = route.query.taskId;
  if (taskId) {
    const task = taskStore.tasks.find(t => t.taskId === taskId || t.id === taskId);
    if (task) {
      openEditModal(task);
    }
  }
};

onMounted(async () => {
  document.addEventListener('click', handleClickOutside);
  
  await projectStore.loadProjects();
  
  // Set default project ID on startup
  if (route.query.projectId) {
    activeProjectId.value = route.query.projectId;
  } else if (projectStore.projects.length > 0) {
    activeProjectId.value = projectStore.projects[0].id;
    router.replace({ query: { ...route.query, projectId: activeProjectId.value } });
  }

  await taskStore.fetchTasks();
  await fetchProjectMeta(activeProjectId.value);
  
  checkRouteParams();
});

watch(() => route.query.taskId, () => {
  checkRouteParams();
});

watch(() => taskStore.tasks, () => {
  checkRouteParams();
}, { deep: true });

// ── Confetti Canvas ──
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
      p.vy += 0.35;
      p.vx *= 0.98;
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

// ── Modals setup ──
const isModalOpen = ref(false);
const editingTask = ref(null);

const openCreateModal = () => {
  editingTask.value = null;
  // Pre-fill boardId
  editingTask.value = { boardId: activeProjectId.value };
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
    // Reload comments
    if (taskData.taskId) {
      const res = await getCommentsByTask(taskData.taskId);
      taskComments.value[taskData.taskId] = res.data || [];
    }
  } catch (err) {
    const errorMsg = err.response?.data?.message || err.message || 'Lỗi lưu tác vụ';
    alert(errorMsg);
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

// ── Analytics calculators ──
const totalTasksCount = computed(() => projectTasks.value.length);
const doneTasksCount = computed(() => projectTasks.value.filter(t => t.currentStatus === 3).length);

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
    { name: 'Hoàn thành (Done)', value: 3, color: 'var(--status-done)' }
  ];

  let offsetAccumulator = 25;
  
  return statusGroups.map(group => {
    const count = projectTasks.value.filter(t => t.currentStatus === group.value).length;
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
    const count = projectTasks.value.filter(t => t.priority === group.value).length;
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

// Colorful avatars setup
const avatarColors = ['#2563EB', '#7C3AED', '#DC2626', '#D97706', '#16A34A', '#0891B2', '#DB2777', '#4F46E5'];
function getAvatarColor(name) {
  if (!name) return avatarColors[0];
  let hash = 0;
  for (let i = 0; i < name.length; i++) hash = name.charCodeAt(i) + ((hash << 5) - hash);
  return avatarColors[Math.abs(hash) % avatarColors.length];
}
</script>

<style scoped>
.board-container {
  padding: var(--space-5) var(--space-6) var(--space-8);
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
  opacity: 0.12;
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

/* Top Navigation Header bar */
.board-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 14px var(--space-5);
  border-radius: var(--radius-xl);
  margin-bottom: var(--space-4);
  background: var(--bg-secondary);
  position: relative;
  z-index: 50;
  gap: var(--space-4);
  flex-wrap: wrap;
}

.header-left {
  display: flex;
  align-items: center;
  gap: var(--space-4);
  flex: 1;
  min-width: 280px;
}

/* Project dropdown styles */
.project-selector-wrapper {
  position: relative;
}

.project-dropdown-btn {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 8px var(--space-4);
  border-radius: var(--radius-md);
  background: var(--color-bg-secondary);
  color: var(--text-primary);
  font-weight: 700;
  font-size: var(--font-size-base);
  border: 1px solid var(--border-color);
  transition: all var(--transition-fast);
}

.project-dropdown-btn:hover {
  background: var(--color-border-hover);
  border-color: var(--color-text-tertiary);
}

.project-indicator-dot {
  width: 10px;
  height: 10px;
  border-radius: 50%;
  box-shadow: 0 0 6px rgba(0, 0, 0, 0.15);
}

.project-dropdown-list {
  position: absolute;
  top: calc(100% + 8px);
  left: 0;
  width: 260px;
  background: var(--glass-bg);
  backdrop-filter: var(--glass-blur);
  -webkit-backdrop-filter: var(--glass-blur);
  border: 1px solid var(--border-color);
  border-radius: var(--radius-lg);
  box-shadow: var(--shadow-glass);
  padding: 6px;
  z-index: 100;
  animation: slideDown 0.2s cubic-bezier(0.16, 1, 0.3, 1);
}

@keyframes slideDown {
  from { opacity: 0; transform: translateY(-8px); }
  to { opacity: 1; transform: translateY(0); }
}

.dropdown-header {
  font-size: 10px;
  font-weight: 700;
  color: var(--text-muted);
  text-transform: uppercase;
  letter-spacing: 1px;
  padding: var(--space-2) var(--space-3);
  border-bottom: 1px solid var(--border-color);
}

.dropdown-item {
  display: flex;
  align-items: center;
  gap: var(--space-3);
  padding: 10px var(--space-3);
  border-radius: var(--radius-md);
  cursor: pointer;
  font-size: var(--font-size-sm);
  font-weight: var(--font-weight-medium);
  transition: all var(--transition-fast);
  color: var(--text-primary);
}

.dropdown-item:hover {
  background: var(--sidebar-hover);
  transform: translateX(4px);
}

.dropdown-item.active {
  background: var(--color-primary-light);
  color: var(--color-primary);
  font-weight: 700;
}

.project-color-dot {
  width: 8px;
  height: 8px;
  border-radius: 50%;
  flex-shrink: 0;
}

.dropdown-empty {
  padding: var(--space-4);
  text-align: center;
  font-size: var(--font-size-xs);
  color: var(--text-muted);
}

/* Header search box */
.header-search-box {
  position: relative;
  display: flex;
  align-items: center;
  flex: 1;
  max-width: 320px;
}

.search-icon {
  position: absolute;
  left: 12px;
  color: var(--text-muted);
  pointer-events: none;
}

.header-search-input {
  width: 100%;
  height: 38px;
  padding: 0 var(--space-4) 0 36px;
  border-radius: var(--radius-md);
  border: 1px solid var(--border-color);
  background: var(--color-bg-secondary);
  color: var(--text-primary);
  font-size: var(--font-size-sm);
  font-weight: 500;
  transition: all var(--transition-fast);
}

.header-search-input:focus {
  border-color: var(--color-primary);
  background: var(--bg-primary);
  box-shadow: 0 0 12px rgba(37, 99, 235, 0.15);
  outline: none;
}

.clear-search-btn {
  position: absolute;
  right: 12px;
  color: var(--text-muted);
  font-size: 11px;
}
.clear-search-btn:hover {
  color: var(--text-primary);
}

.header-right {
  display: flex;
  align-items: center;
  gap: var(--space-4);
  flex-wrap: wrap;
}

/* View switcher styling */
.view-toggles {
  display: flex;
  background: var(--color-bg-secondary);
  border: 1px solid var(--border-color);
  padding: 3px;
  border-radius: var(--radius-md);
  gap: 2px;
}

.toggle-btn {
  display: flex;
  align-items: center;
  gap: 6px;
  padding: 6px 14px;
  border-radius: var(--radius-sm);
  border: none;
  font-size: var(--font-size-xs);
  font-weight: var(--font-weight-bold);
  color: var(--text-secondary);
  background: transparent;
  cursor: pointer;
  transition: all var(--transition-fast);
}

.toggle-btn:hover {
  color: var(--text-primary);
}

.toggle-btn.active {
  background: var(--gradient-primary);
  color: white;
  box-shadow: var(--shadow-sm);
}

/* Kanban Sub-Header styling */
.board-subheader {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: var(--space-5);
  padding: 0 4px;
  gap: var(--space-4);
  flex-wrap: wrap;
}

.subheader-left {
  display: flex;
  align-items: center;
  gap: var(--space-4);
}

.sprint-active-badge {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  padding: 6px 12px;
  background: rgba(37, 99, 235, 0.08);
  border: 1px solid rgba(37, 99, 235, 0.15);
  color: var(--color-primary);
  border-radius: var(--radius-full);
  font-size: var(--font-size-xs);
  font-weight: 700;
}

[data-theme='dark'] .sprint-active-badge {
  background: rgba(59, 130, 246, 0.12);
  border-color: rgba(59, 130, 246, 0.2);
  color: #60a5fa;
}

.sprint-icon {
  color: var(--color-primary);
}

[data-theme='dark'] .sprint-icon {
  color: #60a5fa;
}

.sprint-members-avatars {
  display: flex;
  align-items: center;
}

.member-avatar {
  width: 26px;
  height: 26px;
  border-radius: 50%;
  border: 2px solid var(--bg-primary);
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-size: 10px;
  font-weight: 700;
  margin-left: -6px;
  box-shadow: var(--shadow-xs);
}

.member-avatar:first-child {
  margin-left: 0;
}

.member-avatar.plus-more {
  background: var(--color-bg-secondary);
  color: var(--text-secondary);
  font-weight: 700;
}

.subheader-right {
  display: flex;
  align-items: center;
  gap: var(--space-4);
}

.filter-toggle-btn {
  display: flex;
  align-items: center;
  gap: 6px;
  padding: 7px 14px;
  border-radius: var(--radius-md);
  border: 1px solid var(--border-color);
  background: var(--bg-secondary);
  color: var(--text-secondary);
  font-size: var(--font-size-xs);
  font-weight: var(--font-weight-bold);
  cursor: pointer;
  transition: all var(--transition-fast);
}

.filter-toggle-btn:hover {
  background: var(--color-bg-secondary);
  color: var(--text-primary);
}

.filter-toggle-btn.active {
  background: var(--color-primary-light);
  color: var(--color-primary);
  border-color: var(--color-primary);
}

.sort-selector-wrapper {
  display: flex;
  align-items: center;
  gap: 6px;
}

.sort-lbl {
  font-size: var(--font-size-xs);
  font-weight: var(--font-weight-semibold);
  color: var(--text-muted);
}

.sort-select {
  height: 30px;
  padding: 0 10px;
  border-radius: var(--radius-sm);
  border: 1px solid var(--border-color);
  background: var(--bg-secondary);
  color: var(--text-primary);
  font-size: var(--font-size-xs);
  font-weight: var(--font-weight-semibold);
  outline: none;
  cursor: pointer;
}

/* Sliding Filters Panel styles */
.filters-panel {
  display: flex;
  gap: var(--space-5);
  padding: var(--space-4) var(--space-5);
  border-radius: var(--radius-lg);
  margin-bottom: var(--space-4);
  align-items: center;
  flex-wrap: wrap;
}

.filter-group {
  display: flex;
  align-items: center;
  gap: 8px;
}

.filter-group label {
  font-size: var(--font-size-xs);
  font-weight: var(--font-weight-bold);
  color: var(--text-secondary);
}

.filter-select {
  height: 32px;
  padding: 0 10px;
  border-radius: var(--radius-sm);
  border: 1px solid var(--border-color);
  background: var(--color-bg-secondary);
  color: var(--text-primary);
  font-size: var(--font-size-xs);
  outline: none;
  cursor: pointer;
}

.checkbox-label {
  display: flex;
  align-items: center;
  gap: 6px;
  cursor: pointer;
  font-weight: 500 !important;
}

.btn-reset-filters {
  font-size: 10px;
  font-weight: 700;
  color: var(--color-danger);
  margin-left: auto;
}

.btn-reset-filters:hover {
  text-decoration: underline;
}

/* Slide Down animation */
.slide-down-enter-active, .slide-down-leave-active {
  transition: all 0.25s ease-out;
}
.slide-down-enter-from, .slide-down-leave-to {
  transform: translateY(-8px);
  opacity: 0;
}

/* View panels */
.view-panel {
  flex: 1;
  display: flex;
  flex-direction: column;
  position: relative;
  z-index: 10;
}

.animate-fade-in {
  animation: fadeIn 0.4s ease-out;
}

/* Kanban board 5-columns layout */
.board-layout {
  display: flex;
  gap: var(--space-4);
  flex: 1;
  overflow-x: auto;
  align-items: flex-start;
  padding-bottom: var(--space-5);
}

/* Scrollbar styles for Horizontal Board list */
.board-layout::-webkit-scrollbar {
  height: 8px;
}
.board-layout::-webkit-scrollbar-track {
  background: transparent;
}
.board-layout::-webkit-scrollbar-thumb {
  background: var(--border-color);
  border-radius: var(--radius-full);
}

.column {
  min-width: 270px;
  width: 290px;
  max-height: calc(100vh - 220px);
  display: flex;
  flex-direction: column;
  background: var(--bg-card);
  border-radius: var(--radius-lg);
  padding: 4px;
  flex-shrink: 0;
}

.column-header {
  padding: var(--space-3) var(--space-4);
  display: flex;
  justify-content: space-between;
  align-items: center;
  border-bottom: 1px solid var(--border-color);
}

.column-title {
  font-size: var(--font-size-sm);
  display: flex;
  align-items: center;
  gap: 8px;
  font-weight: 700;
  color: var(--text-primary);
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
  font-size: 10px;
  font-weight: 700;
}

[data-theme='dark'] .task-count {
  background: rgba(59, 130, 246, 0.2);
  color: #60a5fa;
}

.column-content {
  padding: var(--space-4) 10px;
  flex: 1;
  overflow-y: auto;
  min-height: 250px;
  transition: background-color var(--transition-fast), border-color var(--transition-fast);
}

/* Drag overlay highlight */
.column-content.drag-over {
  background: rgba(37, 99, 235, 0.04) !important;
  border: 1px dashed rgba(37, 99, 235, 0.25) !important;
  border-radius: var(--radius-md);
}

/* Scrollbar styles for vertical column content */
.column-content::-webkit-scrollbar {
  width: 4px;
}
.column-content::-webkit-scrollbar-track {
  background: transparent;
}
.column-content::-webkit-scrollbar-thumb {
  background: var(--border-color);
  border-radius: 2px;
}

.draggable-item {
  outline: none;
}

.empty-column-state {
  text-align: center;
  padding: var(--space-8) 0;
  font-size: var(--font-size-xs);
  color: var(--text-muted);
  font-weight: 500;
  border: 1px dashed var(--border-color);
  border-radius: var(--radius-md);
  margin-top: 10px;
}

/* Confetti canvas overlay styling */
.confetti-canvas {
  position: fixed;
  top: 0;
  left: 0;
  width: 100vw;
  height: 100vh;
  z-index: 999;
  pointer-events: none;
}

/* Analytics view styling overrides */
.analytics-panel {
  gap: var(--space-6);
  padding-bottom: var(--space-8);
}

.metrics-grid {
  display: grid;
  grid-template-columns: 1fr 1fr 1fr;
  gap: var(--space-5);
}

@media (max-width: 768px) {
  .metrics-grid {
    grid-template-columns: 1fr;
  }
}

.metric-card {
  padding: var(--space-5);
  border-radius: var(--radius-xl);
  display: flex;
  align-items: center;
  gap: var(--space-5);
  box-shadow: var(--shadow-sm);
  background: var(--bg-card);
}

.metric-card.text-center {
  flex-direction: column;
  justify-content: center;
  text-align: center;
}

.metric-card h4 {
  font-size: var(--font-size-xs);
  font-weight: 700;
  text-transform: uppercase;
  color: var(--text-muted);
  letter-spacing: 0.5px;
}

.progress-ring-wrapper {
  position: relative;
  width: 120px;
  height: 120px;
  display: flex;
  align-items: center;
  justify-content: center;
  margin-top: var(--space-3);
}

.progress-ring {
  transform: rotate(-90deg);
}

.ring-bg {
  stroke: var(--color-bg-secondary);
}

.ring-fill {
  transition: stroke-dashoffset 0.6s ease;
}

.ring-label {
  position: absolute;
  display: flex;
  flex-direction: column;
  align-items: center;
}

.ring-label .val {
  font-size: var(--font-size-lg);
  font-weight: var(--font-weight-extrabold);
  color: var(--text-primary);
}

.ring-label .lbl {
  font-size: 8px;
  font-weight: 700;
  color: var(--text-muted);
  text-transform: uppercase;
  margin-top: 2px;
}

.metric-icon {
  width: 48px;
  height: 48px;
  border-radius: var(--radius-md);
  display: flex;
  align-items: center;
  justify-content: center;
}

.metric-info {
  display: flex;
  flex-direction: column;
}

.metric-info .num {
  font-size: var(--font-size-xl);
  font-weight: var(--font-weight-extrabold);
  color: var(--text-primary);
  line-height: 1.1;
}

.metric-info .lbl {
  font-size: var(--font-size-xs);
  color: var(--text-secondary);
  font-weight: var(--font-weight-semibold);
  margin-top: 4px;
}

.charts-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: var(--space-6);
  margin-top: var(--space-2);
}

@media (max-width: 768px) {
  .charts-grid {
    grid-template-columns: 1fr;
  }
}

.chart-card {
  padding: var(--space-5);
  border-radius: var(--radius-xl);
  background: var(--bg-card);
  box-shadow: var(--shadow-sm);
  display: flex;
  flex-direction: column;
  gap: var(--space-4);
}

.chart-title {
  font-size: var(--font-size-sm);
  font-weight: 700;
  text-transform: uppercase;
  color: var(--text-primary);
  letter-spacing: 0.5px;
}

/* Donut chart layout overrides */
.donut-layout {
  display: flex;
  align-items: center;
  justify-content: space-around;
  padding: var(--space-3) var(--space-4);
  gap: var(--space-4);
}

.legend {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.legend-item {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: var(--font-size-xs);
  font-weight: 500;
}

.legend-item .dot {
  width: 8px;
  height: 8px;
  border-radius: 50%;
}

.legend-item .name {
  color: var(--text-secondary);
}

.legend-item .count {
  color: var(--text-primary);
  font-weight: 700;
  margin-left: auto;
}

/* Bar chart priority layout overrides */
.bar-chart-container {
  display: flex;
  gap: var(--space-4);
  height: 160px;
  padding: var(--space-3) 0 var(--space-2);
}

.y-axis {
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  font-size: 9px;
  font-weight: 700;
  color: var(--text-muted);
  padding-bottom: 24px;
}

.bars-wrapper {
  display: flex;
  justify-content: space-around;
  flex: 1;
  align-items: flex-end;
  height: 100%;
}

.bar-col {
  display: flex;
  flex-direction: column;
  align-items: center;
  width: 45px;
  height: 100%;
  justify-content: flex-end;
  position: relative;
}

.bar-pill {
  width: 14px;
  border-radius: var(--radius-sm) var(--radius-sm) 0 0;
  min-height: 6px;
  transition: height 0.6s cubic-bezier(0.16, 1, 0.3, 1);
  cursor: pointer;
}

.bar-pill:hover + .bar-val-hover {
  opacity: 1;
}

.bar-val-hover {
  position: absolute;
  bottom: calc(100% + 4px);
  background: var(--text-primary);
  color: var(--bg-primary);
  font-size: 8px;
  font-weight: 700;
  padding: 2px 6px;
  border-radius: 3px;
  pointer-events: none;
  opacity: 0;
  transition: opacity 0.2s ease;
}

.bar-col:hover .bar-val-hover {
  opacity: 1;
}

.bar-lbl {
  font-size: 9px;
  font-weight: 700;
  color: var(--text-muted);
  margin-top: 10px;
  white-space: nowrap;
}
</style>
