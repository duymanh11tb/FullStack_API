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
    error: null,
    hubConnection: null
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
            const isTaskType = n.type === 'task.assigned' || n.type === 'task.status.changed' || n.type === 'comment.mention' || n.type === 'CommentMention' || n.type === 'TaskAssigned' || n.type === 'TaskStatusChanged'
            if (isTaskType) {
              const taskRes = await taskApi.getTask(n.referenceId)
              const title = taskRes.data?.data?.title || taskRes.data?.title || n.referenceId
              if (n.type === 'task.assigned' || n.type === 'TaskAssigned') enhancedMessage = `Bạn đã được phân công vào công việc '${title}'`
              if (n.type === 'task.status.changed' || n.type === 'TaskStatusChanged') enhancedMessage = `Trạng thái của công việc '${title}' đã thay đổi`
              if (n.type === 'comment.mention' || n.type === 'CommentMention') enhancedMessage = `Bạn đã được nhắc đến trong bình luận ở công việc '${title}'`
            } else if (n.type === 'member.added' || n.type === 'MemberAdded') {
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
    },

    async startSignalR(userId) {
      if (this.hubConnection) return

      try {
        const signalR = await import('@microsoft/signalr')
        const token = localStorage.getItem('token')
        
        const connection = new signalR.HubConnectionBuilder()
          .withUrl(`/proxy-notify/hub/notifications?userId=${userId}`, {
            accessTokenFactory: () => token || ''
          })
          .withAutomaticReconnect()
          .build()

        connection.on('ReceiveNotification', async (notif) => {
          let enhancedMessage = notif.message || 'Bạn có thông báo mới'
          try {
            const typeStr = notif.type
            const isTaskType = typeStr === 'task.assigned' || typeStr === 'task.status.changed' || typeStr === 'comment.mention' || typeStr === 'CommentMention' || typeStr === 'TaskAssigned' || typeStr === 'TaskStatusChanged'
            
            if (isTaskType) {
              const taskRes = await taskApi.getTask(notif.referenceId)
              const title = taskRes.data?.data?.title || taskRes.data?.title || notif.referenceId
              if (typeStr === 'task.assigned' || typeStr === 'TaskAssigned') enhancedMessage = `Bạn đã được phân công vào công việc '${title}'`
              if (typeStr === 'task.status.changed' || typeStr === 'TaskStatusChanged') enhancedMessage = `Trạng thái của công việc '${title}' đã thay đổi`
              if (typeStr === 'comment.mention' || typeStr === 'CommentMention') enhancedMessage = `Bạn đã được nhắc đến trong bình luận ở công việc '${title}'`
            } else if (typeStr === 'member.added' || typeStr === 'MemberAdded') {
              const projRes = await getProject(notif.referenceId)
              const name = projRes.data?.data?.name || projRes.data?.name || notif.referenceId
              enhancedMessage = `Bạn đã được thêm vào dự án '${name}'`
            }
          } catch (err) {
            console.error('SignalR enhancement error:', err)
          }

          const newNotif = {
            ...notif,
            id: notif.id || notif.Id,
            displayMessage: enhancedMessage,
            isRead: notif.isRead || notif.IsRead || false,
            createdAt: notif.createdAt || notif.CreatedAt
          }

          // Avoid duplicates
          const exists = this.notifications.some(n => n.id === newNotif.id)
          if (!exists) {
            this.notifications.unshift(newNotif)
          }
        })

        await connection.start()
        this.hubConnection = connection
        console.log('SignalR Hub connected successfully')
      } catch (err) {
        console.error('SignalR Hub connection failed:', err)
      }
    },

    stopSignalR() {
      if (this.hubConnection) {
        this.hubConnection.stop()
        this.hubConnection = null
        console.log('SignalR Hub connection stopped')
      }
    }
  }
})
