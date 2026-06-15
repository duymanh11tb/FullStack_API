import { defineStore } from 'pinia';
import { taskApi } from '../api/taskApi';

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
        await taskApi.updateTask(id, taskData);
        const index = this.tasks.findIndex((t) => t.taskId === id);
        if (index !== -1) {
          this.tasks[index] = { ...this.tasks[index], ...taskData };
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
