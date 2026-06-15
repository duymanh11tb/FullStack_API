import { taskAPI } from './axios';

export const taskApi = {
  getTasks() {
    return taskAPI.get('/Task');
  },
  getTask(id) {
    return taskAPI.get(`/Task/${id}`);
  },
  createTask(taskData) {
    return taskAPI.post('/Task', taskData);
  },
  updateTask(id, taskData) {
    return taskAPI.put(`/Task/${id}`, taskData);
  },
  deleteTask(id) {
    return taskAPI.delete(`/Task/${id}`);
  },
  updateTaskStatus(id, status) {
    return taskAPI.patch(`/Task/${id}/status`, { status });
  },
};
