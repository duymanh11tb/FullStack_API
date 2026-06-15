import { notifyAPI } from './axios'

// ── Auth ──
export const register = (data) =>
  notifyAPI.post('/api/Auth/register', data)

export const login = (data) =>
  notifyAPI.post('/api/Auth/login', data)

export const getMe = () =>
  notifyAPI.get('/api/Auth/me')

// ── Comments ──
export const createComment = (data) =>
  notifyAPI.post('/api/Comment', data)

export const getCommentsByTask = (taskId) =>
  notifyAPI.get(`/api/Comment/task/${taskId}`)

export const deleteComment = (id) =>
  notifyAPI.delete(`/api/Comment/${id}`)

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
