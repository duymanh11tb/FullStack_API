import { defineStore } from 'pinia'
import {
  getNotifications as fetchNotifications,
  markAsRead as markReadApi,
  markAllAsRead as markAllReadApi
} from '../api/notifyApi'
import { taskApi } from '../api/taskApi'
import { getProject } from '../api/projectApi'

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
        const notifs = res.data.data || res.data || []
        
        const promises = notifs.map(async (n) => {
          let enhancedMessage = n.message || 'Bạn có thông báo mới'
          try {
            if (n.type === 'task.assigned' || n.type === 'task.status.changed' || n.type === 'comment.mention') {
              const taskRes = await taskApi.getTask(n.referenceId)
              const title = taskRes.data?.data?.title || taskRes.data?.title || n.referenceId
              if (n.type === 'task.assigned') enhancedMessage = `Bạn đã được phân công vào công việc '${title}'`
              if (n.type === 'task.status.changed') enhancedMessage = `Trạng thái của công việc '${title}' đã thay đổi`
              if (n.type === 'comment.mention') enhancedMessage = `Bạn đã được nhắc đến trong bình luận ở công việc '${title}'`
            } else if (n.type === 'member.added') {
              const projRes = await getProject(n.referenceId)
              const name = projRes.data?.data?.name || projRes.data?.name || n.referenceId
              enhancedMessage = `Bạn đã được thêm vào dự án '${name}'`
            }
          } catch {
            // keep generic message on error
          }
          return { ...n, displayMessage: enhancedMessage }
        })
        
        this.notifications = await Promise.all(promises)
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
