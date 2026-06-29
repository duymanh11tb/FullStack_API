import { defineStore } from 'pinia';
import { taskApi } from '../api/taskApi';
import { publishEvent } from '../api/notifyApi';

export const useTaskStore = defineStore('task', {
  state: () => ({
    tasks: [],
    loading: false,
    error: null,
  }),
  actions: {
    async fetchTasks() {
      this.loading = true;
      this.error = null;
      try {
        const response = await taskApi.getTasks();
        this.tasks = response.data;
      } catch (err) {
        this.error = err.message || 'Failed to fetch tasks';
        console.error(err);
      } finally {
        this.loading = false;
      }
    },
    async createTask(taskData) {
      this.loading = true;
      try {
        const response = await taskApi.createTask(taskData);
        this.tasks.push(response.data);

        // Notify if there is an assignee
        const createdTask = response.data;
        if (createdTask && createdTask.assigneeId && createdTask.assigneeId !== '00000000-0000-0000-0000-000000000000') {
          try {
            const userJson = localStorage.getItem('user');
            const currentUser = userJson ? JSON.parse(userJson) : null;
            await publishEvent({
              eventType: 'task.assigned',
              taskId: createdTask.taskId,
              projectId: createdTask.boardId,
              referenceId: createdTask.taskId,
              actorUserId: currentUser?.id || currentUser?.Id,
              targetUserId: createdTask.assigneeId,
              recipientUserIds: [createdTask.assigneeId],
              message: `Bạn đã được phân công vào công việc '${createdTask.title}'.`
            });
          } catch (e) {
            console.error('Failed to notify task assignment:', e);
          }
        }
      } catch (err) {
        this.error = err.message || 'Failed to create task';
        throw err;
      } finally {
        this.loading = false;
      }
    },
    async updateTask(id, taskData) {
      this.loading = true;
      try {
        const existingTask = this.tasks.find((t) => t.taskId === id);
        const oldAssigneeId = existingTask?.assigneeId;

        await taskApi.updateTask(id, taskData);
        const index = this.tasks.findIndex((t) => t.taskId === id);
        if (index !== -1) {
          this.tasks[index] = { ...this.tasks[index], ...taskData };
        }

        const updatedTask = this.tasks[index];
        if (updatedTask && updatedTask.assigneeId && updatedTask.assigneeId !== '00000000-0000-0000-0000-000000000000' && updatedTask.assigneeId !== oldAssigneeId) {
          try {
            const userJson = localStorage.getItem('user');
            const currentUser = userJson ? JSON.parse(userJson) : null;
            await publishEvent({
              eventType: 'task.assigned',
              taskId: updatedTask.taskId,
              projectId: updatedTask.boardId,
              referenceId: updatedTask.taskId,
              actorUserId: currentUser?.id || currentUser?.Id,
              targetUserId: updatedTask.assigneeId,
              recipientUserIds: [updatedTask.assigneeId],
              message: `Bạn đã được phân công vào công việc '${updatedTask.title}'.`
            });
          } catch (e) {
            console.error('Failed to notify task assignment:', e);
          }
        }
      } catch (err) {
        this.error = err.message || 'Failed to update task';
        throw err;
      } finally {
        this.loading = false;
      }
    },
    async deleteTask(id) {
      this.loading = true;
      try {
        await taskApi.deleteTask(id);
        this.tasks = this.tasks.filter((t) => t.taskId !== id);
      } catch (err) {
        this.error = err.message || 'Failed to delete task';
        throw err;
      } finally {
        this.loading = false;
      }
    },
    async updateTaskStatus(id, status) {
      try {
        await taskApi.updateTaskStatus(id, status);
        const task = this.tasks.find((t) => t.taskId === id);
        if (task) {
          task.currentStatus = status;
        }
      } catch (err) {
        this.error = err.message || 'Failed to update task status';
        throw err;
      }
    },
  },
});
