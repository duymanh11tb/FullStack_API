import { taskAPI } from './axios';

export const taskApi = {
  getTasks() {
    return taskAPI.get('/api/Task');
  },
  getTask(id) {
    return taskAPI.get(`/api/Task/${id}`);
  },
  createTask(taskData) {
    return taskAPI.post('/api/Task', taskData);
  },
  updateTask(id, taskData) {
    return taskAPI.put(`/api/Task/${id}`, taskData);
  },
  deleteTask(id) {
    return taskAPI.delete(`/api/Task/${id}`);
  },
  updateTaskStatus(id, status) {
    return taskAPI.patch(`/api/Task/${id}/status`, { status });
  },
};
