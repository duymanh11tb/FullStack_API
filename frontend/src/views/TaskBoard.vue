<template>
  <div class="board-container">
    <header class="board-header">
      <div class="header-left">
        <h1>Project Board</h1>
        <p class="subtitle">Manage your tasks visually</p>
      </div>
      <button class="btn-primary" @click="openCreateModal">
        <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><line x1="12" y1="5" x2="12" y2="19"></line><line x1="5" y1="12" x2="19" y2="12"></line></svg>
        New Task
      </button>
    </header>

    <div class="board-layout" v-if="!taskStore.loading || taskStore.tasks.length > 0">
      <div class="column" v-for="col in columns" :key="col.status">
        <div class="column-header">
          <h2 class="column-title">
            <span class="status-indicator" :style="{ backgroundColor: col.color }"></span>
            {{ col.title }}
          </h2>
          <span class="task-count">{{ getTasksByStatus(col.status).length }}</span>
        </div>
        
        <div class="column-content" 
             @dragover.prevent 
             @drop="onDrop($event, col.status)">
          
          <div v-for="task in getTasksByStatus(col.status)" 
               :key="task.taskId"
               draggable="true"
               @dragstart="onDragStart($event, task)"
               class="draggable-item">
            <TaskCard :task="task" @edit="openEditModal" />
          </div>
          
          <div v-if="getTasksByStatus(col.status).length === 0" class="empty-state">
            No tasks here
          </div>
        </div>
      </div>
    </div>
    
    <div v-else class="loading-state">
      <div class="spinner"></div>
      <p>Loading tasks...</p>
    </div>

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
import { ref, onMounted, watch } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { useTaskStore } from '../stores/taskStore';
import TaskCard from '../components/TaskCard.vue';
import TaskModal from '../components/TaskModal.vue';

const route = useRoute();
const router = useRouter();
const taskStore = useTaskStore();

// Status Mapping based on Swagger enum
// 0: Todo, 1: InProgress, 2: Review, 3: Done, 4: Cancelled
const columns = [
  { status: 0, title: 'To Do', color: 'var(--status-todo)' },
  { status: 1, title: 'In Progress', color: 'var(--status-inprogress)' },
  { status: 2, title: 'Review', color: 'var(--status-review)' },
  { status: 3, title: 'Done', color: 'var(--status-done)' }
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
  return taskStore.tasks.filter(t => t.currentStatus === status) || [];
};

const onDragStart = (evt, task) => {
  evt.dataTransfer.dropEffect = 'move';
  evt.dataTransfer.effectAllowed = 'move';
  evt.dataTransfer.setData('taskId', task.taskId);
};

const onDrop = async (evt, newStatus) => {
  const taskId = evt.dataTransfer.getData('taskId');
  const task = taskStore.tasks.find(t => t.taskId === taskId);
  
  if (task && task.currentStatus !== newStatus) {
    // Optimistic update
    const oldStatus = task.currentStatus;
    task.currentStatus = newStatus;
    
    try {
      await taskStore.updateTaskStatus(taskId, newStatus);
    } catch (err) {
      // Revert if API fails
      task.currentStatus = oldStatus;
      alert('Failed to update task status');
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
    alert('Failed to save task: ' + err.message);
  }
};

const onDeleteTask = async (taskId) => {
  try {
    await taskStore.deleteTask(taskId);
    closeModal();
  } catch (err) {
    alert('Failed to delete task: ' + err.message);
  }
};
</script>

<style scoped>
.board-container {
  padding: 24px 32px;
  height: 100vh;
  display: flex;
  flex-direction: column;
  background-color: var(--bg-board);
}

.board-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-end;
  margin-bottom: 32px;
}

.header-left h1 {
  font-size: 2rem;
  margin-bottom: 4px;
}

.subtitle {
  color: var(--text-secondary);
}

.btn-primary {
  display: flex;
  align-items: center;
  gap: 8px;
  background: var(--gradient-primary);
  color: white;
  padding: 10px 20px;
  border-radius: var(--radius-lg);
  font-weight: 600;
  transition: all var(--transition-fast);
  box-shadow: var(--shadow-sm);
}

.btn-primary:hover {
  background: var(--gradient-primary-hover);
  box-shadow: var(--shadow-md), var(--gradient-glow);
  transform: translateY(-2px);
}

.board-layout {
  display: flex;
  gap: 24px;
  flex: 1;
  overflow-x: auto;
  padding-bottom: 16px;
}

.column {
  min-width: 300px;
  width: 320px;
  display: flex;
  flex-direction: column;
  background: var(--bg-card);
  backdrop-filter: var(--glass-blur);
  -webkit-backdrop-filter: var(--glass-blur);
  border-radius: var(--radius-xl);
  border: 1px solid var(--border-color);
  box-shadow: var(--shadow-sm);
}

.column-header {
  padding: 16px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  border-bottom: 1px solid var(--border-color);
  background: rgba(255, 255, 255, 0.02);
}

.column-title {
  font-size: 1.1rem;
  display: flex;
  align-items: center;
  gap: 8px;
  font-weight: var(--font-weight-bold);
}

.status-indicator {
  width: 12px;
  height: 12px;
  border-radius: 50%;
  box-shadow: 0 0 8px currentColor;
}

.task-count {
  background: var(--color-primary-light);
  color: var(--color-primary);
  padding: 2px 10px;
  border-radius: var(--radius-full);
  font-size: 0.8rem;
  font-weight: 700;
}

.column-content {
  padding: 16px;
  flex: 1;
  overflow-y: auto;
}

.draggable-item {
  cursor: grab;
}

.draggable-item:active {
  cursor: grabbing;
}

.empty-state {
  text-align: center;
  padding: 32px 0;
  color: var(--text-muted);
  font-style: italic;
  font-size: 0.9rem;
}

.loading-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  flex: 1;
  color: var(--text-secondary);
}

.spinner {
  width: 40px;
  height: 40px;
  border: 4px solid var(--border-color);
  border-top-color: var(--accent-primary);
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin-bottom: 16px;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}
</style>
