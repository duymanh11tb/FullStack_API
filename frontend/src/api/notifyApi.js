import { notifyAPI } from './axios'

// ── Auth ──
export const register = (data) =>
  notifyAPI.post('/api/Auth/register', data)

export const login = (data) =>
  notifyAPI.post('/api/Auth/login', data)

export const getMe = () =>
  notifyAPI.get('/api/Auth/me')

export const searchUsers = (query) =>
  notifyAPI.get('/api/Auth/users/search', { params: { query } })

export const getAllUsers = () =>
  notifyAPI.get('/api/Auth/users')

export const updateUser = (id, data) =>
  notifyAPI.put(`/api/Auth/users/${id}`, data)

export const deleteUser = (id) =>
  notifyAPI.delete(`/api/Auth/users/${id}`)

// ── Comments ──
export const createComment = (data) =>
  notifyAPI.post('/api/Comment', data)

export const getCommentsByTask = (taskId) =>
  notifyAPI.get(`/api/Comment/task/${taskId}`)

export const deleteComment = (id) =>
  notifyAPI.delete(`/api/Comment/${id}`)

export const updateComment = (id, data) =>
  notifyAPI.put(`/api/Comment/${id}`, data)


// ── Notifications ──
export const getNotifications = (isRead = undefined) =>
  notifyAPI.get('/api/Notification', { params: isRead !== undefined ? { isRead } : {} })

export const markAsRead = (id) =>
  notifyAPI.put(`/api/Notification/${id}/read`)

export const markAllAsRead = () =>
  notifyAPI.put('/api/Notification/read-all')

// ── Notification Settings ──
export const getNotificationSettings = () =>
  notifyAPI.get('/api/Notification/settings')

export const updateNotificationSettings = (data) =>
  notifyAPI.put('/api/Notification/settings', data)

// ── Activity Log ──
export const getActivityLogsByProject = (projectId) =>
  notifyAPI.get(`/api/ActivityLog/project/${projectId}`)

export const getActivityLogsByTask = (taskId) =>
  notifyAPI.get(`/api/ActivityLog/task/${taskId}`)

export const getActivityLogsByUser = (userId) =>
  notifyAPI.get(`/api/ActivityLog/user/${userId}`)

// ── Profile and Security ──
export const updateProfile = (data) =>
  notifyAPI.put('/api/Auth/profile', data)

export const changePassword = (data) =>
  notifyAPI.put('/api/Auth/change-password', data)

export const forgotPassword = (data) =>
  notifyAPI.post('/api/Auth/forgot-password', data)

