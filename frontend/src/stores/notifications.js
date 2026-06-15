import { defineStore } from 'pinia'
import {
  getNotifications as fetchNotifications,
  markAsRead as markReadApi,
  markAllAsRead as markAllReadApi
} from '../api/notifyApi'

export const useNotificationStore = defineStore('notifications', {
  state: () => ({
    notifications: [],
    loading: false,
    error: null
  }),

  getters: {
    unreadCount: (state) =>
      state.notifications.filter(n => !n.isRead).length,
    unreadNotifications: (state) =>
      state.notifications.filter(n => !n.isRead),
    readNotifications: (state) =>
      state.notifications.filter(n => n.isRead)
  },

  actions: {
    async loadNotifications(isRead = undefined) {
      this.loading = true
      try {
        const res = await fetchNotifications(isRead)
        this.notifications = res.data.data || res.data || []
      } catch (err) {
        this.error = err.response?.data?.message || 'Không thể tải thông báo'
        this.notifications = []
      } finally {
        this.loading = false
      }
    },

    async markAsRead(id) {
      try {
        await markReadApi(id)
        const notif = this.notifications.find(n => n.id === id)
        if (notif) notif.isRead = true
      } catch {
        // silent
      }
    },

    async markAllAsRead() {
      try {
        await markAllReadApi()
        this.notifications.forEach(n => { n.isRead = true })
      } catch {
        // silent
      }
    }
  }
})
