<template>
  <div class="project-analytics">
    <div v-if="loading" class="loading-state">
      <LoadingSpinner text="Đang phân tích dữ liệu dự án..." />
    </div>

    <div v-else class="analytics-layout">
      <!-- Top Row: Core Metrics Grid -->
      <div class="metrics-grid">
        <div class="metric-card glass-panel animate-pulse-glow">
          <div class="metric-header">
            <h4>Tiến độ công việc</h4>
            <span class="badge">{{ percentTasksDone }}%</span>
          </div>
          <div class="progress-ring-wrapper">
            <svg class="progress-ring" width="120" height="120">
              <circle class="ring-bg" stroke="rgba(255,255,255,0.06)" stroke-width="8" fill="transparent" r="50" cx="60" cy="60"/>
              <circle class="ring-fill" stroke="var(--status-done)" stroke-width="8" :stroke-dasharray="strokeDasharray" :stroke-dashoffset="strokeDashoffset" stroke-linecap="round" fill="transparent" r="50" cx="60" cy="60"/>
            </svg>
            <div class="ring-label">
              <span class="val">{{ doneTasksCount }}/{{ totalTasksCount }}</span>
              <span class="lbl">Tác vụ xong</span>
            </div>
          </div>
        </div>

        <div class="metric-card glass-panel">
          <div class="metric-header">
            <h4>Sprints</h4>
            <span class="badge font-bold">{{ sprints.length }}</span>
          </div>
          <div class="list-stats">
            <div class="list-stat-item">
              <span class="bullet active"></span>
              <span class="label">Đang chạy (Active)</span>
              <span class="value">{{ activeSprintsCount }}</span>
            </div>
            <div class="list-stat-item">
              <span class="bullet completed"></span>
              <span class="label">Đã xong (Completed)</span>
              <span class="value">{{ completedSprintsCount }}</span>
            </div>
            <div class="list-stat-item">
              <span class="bullet planning"></span>
              <span class="label">Lên kế hoạch (Planning)</span>
              <span class="value">{{ planningSprintsCount }}</span>
            </div>
          </div>
        </div>

        <div class="metric-card glass-panel">
          <div class="metric-header">
            <h4>Milestones</h4>
            <span class="badge font-bold">{{ milestones.length }}</span>
          </div>
          <div class="progress-bar-metrics">
            <div class="bar-lbl-row">
              <span class="lbl">Hoàn thành</span>
              <span class="val">{{ completedMilestonesCount }}/{{ milestones.length }}</span>
            </div>
            <div class="bar-track">
              <div class="bar-fill" :style="{ width: percentMilestonesDone + '%', background: 'var(--color-success)' }"></div>
            </div>
            <div class="overdue-row mt-4" v-if="overdueMilestonesCount > 0">
              <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="var(--color-danger)" stroke-width="2">
                <circle cx="12" cy="12" r="10"/><line x1="12" y1="8" x2="12" y2="12"/><line x1="12" y1="16" x2="12.01" y2="16"/>
              </svg>
              <span class="text-danger">{{ overdueMilestonesCount }} milestone quá hạn!</span>
            </div>
            <div class="overdue-row mt-4" v-else>
              <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="var(--color-success)" stroke-width="2">
                <polyline points="20 6 9 17 4 12"/>
              </svg>
              <span class="text-success">Tất cả milestone đúng hạn</span>
            </div>
          </div>
        </div>
      </div>

      <!-- Second Row: Custom SVG Charts -->
      <div class="charts-row">
        <!-- Donut Chart: Task Status -->
        <div class="chart-card glass-panel">
          <h3 class="chart-title">Trạng thái công việc</h3>
          <div class="chart-body donut-layout" v-if="totalTasksCount > 0">
            <div class="svg-container">
              <svg width="160" height="160" viewBox="0 0 40 40" class="donut">
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
          <div class="chart-empty" v-else>
            <p>Không có tác vụ nào để thống kê trạng thái.</p>
          </div>
        </div>

        <!-- Bar Chart: Task Priority -->
        <div class="chart-card glass-panel">
          <h3 class="chart-title">Phân phối độ ưu tiên</h3>
          <div class="chart-body bar-layout" v-if="totalTasksCount > 0">
            <div class="bar-chart-container">
              <!-- Y-Axis indicators -->
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
          <div class="chart-empty" v-else>
            <p>Không có tác vụ nào để thống kê độ ưu tiên.</p>
          </div>
        </div>
      </div>

      <!-- Third Row: Member Activity Logs Allocation -->
      <div class="member-section glass-panel">
        <h3 class="chart-title">Đóng góp của thành viên (Lượt hoạt động)</h3>
        <div class="member-list" v-if="activityData.length > 0">
          <div class="member-progress-item" v-for="item in activityData" :key="item.name">
            <div class="member-info">
              <div class="avatar" :style="{ background: getAvatarColor(item.name) }">
                {{ item.name.charAt(0).toUpperCase() }}
              </div>
              <span class="name">{{ item.name }}</span>
              <span class="count font-bold">{{ item.count }} hoạt động</span>
            </div>
            <div class="progress-row">
              <div class="progress-bar-track">
                <div class="progress-bar-fill" :style="{ width: item.percent + '%', background: getAvatarColor(item.name) }"></div>
              </div>
              <span class="percent-txt">{{ Math.round(item.percent) }}%</span>
            </div>
          </div>
        </div>
        <div class="chart-empty py-8" v-else>
          <p>Không có nhật ký hoạt động nào ghi nhận đóng góp thành viên gần đây.</p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { getSprints, getMilestones } from '../../api/projectApi'
import { getActivityLogsByProject } from '../../api/notifyApi'
import { useTaskStore } from '../../stores/taskStore'
import LoadingSpinner from '../common/LoadingSpinner.vue'

const props = defineProps({
  projectId: {
    type: String,
    required: true
  }
})

const loading = ref(true)
const sprints = ref([])
const milestones = ref([])
const activities = ref([])
const taskStore = useTaskStore()

// Ring Progress variables
const ringRadius = 50
const ringCircumference = 2 * Math.PI * ringRadius

async function loadAnalytics() {
  loading.value = true
  try {
    const fetchPromises = [
      getSprints(props.projectId).then(res => sprints.value = res.data?.data || res.data || []),
      getMilestones(props.projectId).then(res => milestones.value = res.data?.data || res.data || []),
      getActivityLogsByProject(props.projectId).then(res => activities.value = res.data || []),
      taskStore.fetchTasks()
    ]
    await Promise.allSettled(fetchPromises)
  } catch (err) {
    console.error('Failed to load project statistics:', err)
  } finally {
    loading.value = false
  }
}

// ── Tasks computations ──
const projectTasks = computed(() => {
  return taskStore.tasks.filter(t => 
    String(t.projectId) === String(props.projectId) || 
    String(t.boardId) === String(props.projectId) ||
    String(t.boardID) === String(props.projectId)
  )
})

const totalTasksCount = computed(() => projectTasks.value.length)
const doneTasksCount = computed(() => projectTasks.value.filter(t => t.currentStatus === 3).length)

const percentTasksDone = computed(() => {
  if (totalTasksCount.value === 0) return 0
  return Math.round((doneTasksCount.value / totalTasksCount.value) * 100)
})

const strokeDasharray = computed(() => `${ringCircumference} ${ringCircumference}`)
const strokeDashoffset = computed(() => {
  const offset = ringCircumference - (percentTasksDone.value / 100) * ringCircumference
  return isNaN(offset) ? ringCircumference : offset
})

// ── Sprints computations ──
// Statuses: Planning, Active, Completed
const activeSprintsCount = computed(() => sprints.value.filter(s => s.status === 'Active').length)
const completedSprintsCount = computed(() => sprints.value.filter(s => s.status === 'Completed').length)
const planningSprintsCount = computed(() => sprints.value.filter(s => s.status === 'Planning').length)

// ── Milestones computations ──
const completedMilestonesCount = computed(() => milestones.value.filter(m => m.isCompleted).length)
const percentMilestonesDone = computed(() => {
  if (milestones.value.length === 0) return 0
  return Math.round((completedMilestonesCount.value / milestones.value.length) * 100)
})

const overdueMilestonesCount = computed(() => {
  const now = new Date()
  return milestones.value.filter(m => !m.isCompleted && m.dueDate && new Date(m.dueDate) < now).length
})

// ── SVG Donut Chart computations (Status) ──
const donutSegments = computed(() => {
  if (totalTasksCount.value === 0) return []
  
  const statusGroups = [
    { name: 'To Do', value: 0, color: 'var(--status-todo)' },
    { name: 'In Progress', value: 1, color: 'var(--status-inprogress)' },
    { name: 'Review', value: 2, color: 'var(--status-review)' },
    { name: 'Done', value: 3, color: 'var(--status-done)' }
  ]

  let offsetAccumulator = 25 // Start from top 12 o'clock position (25% offset in SVG circle geometry)
  
  return statusGroups.map(group => {
    const count = projectTasks.value.filter(t => t.currentStatus === group.value).length
    const percent = (count / totalTasksCount.value) * 100
    
    // SVG stroke-dashoffset counts backwards, so we subtract
    const offset = offsetAccumulator
    offsetAccumulator = (offsetAccumulator - percent + 100) % 100
    
    return {
      name: group.name,
      count,
      percent,
      offset,
      color: group.color
    }
  }).filter(seg => seg.count > 0) // Only render segmented blocks for active states
})

// ── SVG Bar Chart computations (Priority) ──
// Priority: 0: Low, 1: Medium, 2: High, 3: Urgent
const barData = computed(() => {
  const priorityGroups = [
    { name: 'Low', value: 0, color: '#94a3b8' },
    { name: 'Medium', value: 1, color: '#3b82f6' },
    { name: 'High', value: 2, color: '#f59e0b' },
    { name: 'Urgent', value: 3, color: '#ef4444' }
  ]

  return priorityGroups.map(group => {
    const count = projectTasks.value.filter(t => t.priority === group.value).length
    return {
      name: group.name,
      count,
      color: group.color
    }
  })
})

const maxPriorityCount = computed(() => {
  const counts = barData.value.map(b => b.count)
  const max = Math.max(...counts)
  return max === 0 ? 4 : max
})

function getBarHeight(count) {
  if (count === 0) return '6px' // Show thin dot line indicator
  return `${(count / maxPriorityCount.value) * 100}%`
}

// ── Member contributions (Activity Logs) computations ──
const activityData = computed(() => {
  if (activities.value.length === 0) return []
  
  const userCounts = {}
  activities.value.forEach(log => {
    const name = log.userFullName || 'Unknown User'
    userCounts[name] = (userCounts[name] || 0) + 1
  })

  const totalLogs = activities.value.length
  return Object.keys(userCounts).map(name => {
    const count = userCounts[name]
    return {
      name,
      count,
      percent: (count / totalLogs) * 100
    }
  }).sort((a, b) => b.count - a.count)
})

const avatarColors = ['#2563EB', '#7C3AED', '#DC2626', '#D97706', '#10B981', '#0891B2', '#DB2777', '#4F46E5']
function getAvatarColor(name) {
  if (!name) return avatarColors[0]
  let hash = 0
  for (let i = 0; i < name.length; i++) hash = name.charCodeAt(i) + ((hash << 5) - hash)
  return avatarColors[Math.abs(hash) % avatarColors.length]
}

onMounted(() => {
  loadAnalytics()
})
</script>

<style scoped>
.project-analytics {
  width: 100%;
}

.loading-state {
  display: flex;
  align-items: center;
  justify-content: center;
  padding: var(--space-12) 0;
}

.analytics-layout {
  display: flex;
  flex-direction: column;
  gap: var(--space-6);
}

/* Metrics row */
.metrics-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(220px, 1fr));
  gap: var(--space-5);
}

.metric-card {
  padding: var(--space-5) var(--space-6);
  display: flex;
  flex-direction: column;
}

.metric-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: var(--space-4);
  border-bottom: 1px solid var(--border-color);
  padding-bottom: var(--space-2);
}

.metric-header h4 {
  font-size: var(--font-size-sm);
  color: var(--color-text-secondary);
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.metric-header .badge {
  font-size: var(--font-size-xs);
  padding: 3px 8px;
  background: var(--color-primary-light);
  color: var(--color-primary);
  border-radius: var(--radius-full);
}

[data-theme='dark'] .metric-header .badge {
  background: rgba(59, 130, 246, 0.2);
  color: #60a5fa;
}

/* Circle progress ring */
.progress-ring-wrapper {
  display: flex;
  align-items: center;
  justify-content: center;
  position: relative;
  height: 120px;
  margin: var(--space-2) 0;
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
  justify-content: center;
  text-align: center;
}

.ring-label .val {
  font-size: var(--font-size-xl);
  font-weight: var(--font-weight-extrabold);
  color: var(--text-primary);
}

.ring-label .lbl {
  font-size: 10px;
  color: var(--text-muted);
  text-transform: uppercase;
  font-weight: 600;
  margin-top: 2px;
}

/* Sprints list metrics */
.list-stats {
  display: flex;
  flex-direction: column;
  gap: 12px;
  justify-content: center;
  flex: 1;
  padding: var(--space-2) 0;
}

.list-stat-item {
  display: flex;
  align-items: center;
  font-size: var(--font-size-sm);
}

.list-stat-item .bullet {
  width: 8px;
  height: 8px;
  border-radius: 50%;
  margin-right: var(--space-3);
  flex-shrink: 0;
}

.list-stat-item .bullet.active { background: var(--color-primary); }
.list-stat-item .bullet.completed { background: var(--color-success); }
.list-stat-item .bullet.planning { background: var(--color-warning); }

.list-stat-item .label {
  color: var(--text-secondary);
  flex: 1;
}

.list-stat-item .value {
  font-weight: var(--font-weight-bold);
  color: var(--text-primary);
}

/* Milestones metrics */
.progress-bar-metrics {
  display: flex;
  flex-direction: column;
  justify-content: center;
  flex: 1;
  padding: var(--space-2) 0;
}

.bar-lbl-row {
  display: flex;
  justify-content: space-between;
  font-size: var(--font-size-sm);
  color: var(--text-secondary);
  margin-bottom: var(--space-2);
  font-weight: 500;
}

.bar-track {
  height: 8px;
  background: var(--color-bg-secondary);
  border-radius: var(--radius-full);
  overflow: hidden;
}

[data-theme='dark'] .bar-track {
  background: rgba(255,255,255,0.06);
}

.bar-fill {
  height: 100%;
  border-radius: var(--radius-full);
  transition: width 0.35s ease-out;
}

.overdue-row {
  display: flex;
  align-items: center;
  gap: 6px;
  font-size: var(--font-size-xs);
  font-weight: 600;
}

.text-danger { color: var(--color-danger); }
.text-success { color: var(--color-success); }

/* Charts section */
.charts-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: var(--space-5);
}

@media (max-width: 768px) {
  .charts-row {
    grid-template-columns: 1fr;
  }
}

.chart-card {
  padding: var(--space-5) var(--space-6);
  min-height: 260px;
  display: flex;
  flex-direction: column;
}

.chart-title {
  font-size: var(--font-size-base);
  font-weight: var(--font-weight-bold);
  color: var(--text-primary);
  margin-bottom: var(--space-5);
  border-left: 3px solid var(--accent-primary);
  padding-left: var(--space-2);
}

.chart-body {
  flex: 1;
  display: flex;
  align-items: center;
}

/* Donut style */
.donut-layout {
  justify-content: space-around;
  gap: var(--space-4);
  flex-wrap: wrap;
}

.svg-container {
  position: relative;
  width: 160px;
  height: 160px;
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
  text-align: center;
}

.donut-inner .num {
  font-size: var(--font-size-2xl);
  font-weight: var(--font-weight-extrabold);
  color: var(--text-primary);
}

.donut-inner .txt {
  font-size: 10px;
  color: var(--text-muted);
  text-transform: uppercase;
  font-weight: 600;
}

.legend {
  display: flex;
  flex-direction: column;
  gap: var(--space-2);
}

.legend-item {
  display: flex;
  align-items: center;
  font-size: var(--font-size-xs);
  color: var(--text-secondary);
}

.legend-item .dot {
  width: 10px;
  height: 10px;
  border-radius: 4px;
  margin-right: var(--space-2);
  flex-shrink: 0;
}

.legend-item .name {
  flex: 1;
  font-weight: 500;
  margin-right: var(--space-3);
}

.legend-item .count {
  font-weight: var(--font-weight-bold);
  color: var(--text-primary);
}

/* Bar chart style */
.bar-layout {
  width: 100%;
  padding-top: var(--space-2);
}

.bar-chart-container {
  display: flex;
  width: 100%;
  height: 160px;
  gap: var(--space-3);
  position: relative;
}

.y-axis {
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  font-size: 10px;
  color: var(--text-muted);
  width: 20px;
  border-right: 1px solid var(--border-color);
  padding-right: 6px;
  height: 120px;
}

.bars-wrapper {
  flex: 1;
  display: flex;
  justify-content: space-around;
  align-items: flex-end;
  height: 120px;
  padding-bottom: 2px;
}

.bar-col {
  display: flex;
  flex-direction: column;
  align-items: center;
  width: 44px;
  position: relative;
}

.bar-pill {
  width: 28px;
  border-radius: 6px 6px 0 0;
  transition: height 0.4s cubic-bezier(0.16, 1, 0.3, 1), filter var(--transition-fast);
  cursor: pointer;
  min-height: 4px;
}

.bar-pill:hover {
  filter: brightness(1.1);
}

.bar-val-hover {
  position: absolute;
  bottom: calc(100% + 4px);
  background: var(--text-primary);
  color: var(--bg-white-to-card);
  font-size: 10px;
  font-weight: var(--font-weight-bold);
  padding: 2px 6px;
  border-radius: 4px;
  opacity: 0;
  transform: translateY(4px);
  transition: all var(--transition-fast);
  pointer-events: none;
  box-shadow: var(--shadow-sm);
}

.bar-col:hover .bar-val-hover {
  opacity: 1;
  transform: translateY(0);
}

.bar-lbl {
  position: absolute;
  top: 130px;
  font-size: 10px;
  font-weight: var(--font-weight-bold);
  color: var(--text-muted);
  text-transform: uppercase;
}

.chart-empty {
  flex: 1;
  display: flex;
  align-items: center;
  justify-content: center;
  color: var(--text-muted);
  font-size: var(--font-size-sm);
  font-style: italic;
  padding: var(--space-8) 0;
}

/* Members section styling */
.member-section {
  padding: var(--space-5) var(--space-6);
}

.member-list {
  display: flex;
  flex-direction: column;
  gap: var(--space-3);
  padding: var(--space-2) 0;
}

.member-progress-item {
  display: flex;
  flex-direction: column;
  gap: 8px;
  padding: 10px;
  border-radius: var(--radius-md);
  transition: background var(--transition-fast);
}

.member-progress-item:hover {
  background: var(--color-bg-secondary);
}

[data-theme='dark'] .member-progress-item:hover {
  background: rgba(255,255,255,0.02);
}

.member-info {
  display: flex;
  align-items: center;
  gap: var(--space-3);
}

.member-info .avatar {
  width: 28px;
  height: 28px;
  border-radius: var(--radius-full);
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-weight: var(--font-weight-bold);
  font-size: var(--font-size-xs);
}

.member-info .name {
  font-size: var(--font-size-sm);
  font-weight: var(--font-weight-semibold);
  color: var(--text-primary);
  flex: 1;
}

.member-info .count {
  font-size: var(--font-size-xs);
  color: var(--text-secondary);
}

.progress-row {
  display: flex;
  align-items: center;
  gap: var(--space-3);
  padding-left: 36px;
}

.progress-bar-track {
  flex: 1;
  height: 6px;
  background: var(--color-bg-secondary);
  border-radius: var(--radius-full);
  overflow: hidden;
}

[data-theme='dark'] .progress-bar-track {
  background: rgba(255,255,255,0.05);
}

.progress-bar-fill {
  height: 100%;
  border-radius: var(--radius-full);
  transition: width 0.35s ease-out;
}

.percent-txt {
  font-size: var(--font-size-xs);
  font-weight: var(--font-weight-bold);
  color: var(--text-primary);
  width: 32px;
  text-align: right;
}

.mt-4 { margin-top: 16px; }
.py-8 { padding-top: 32px; padding-bottom: 32px; }
</style>
