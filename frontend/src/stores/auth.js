import { defineStore } from 'pinia'
import { login as loginApi, register as registerApi, getMe } from '../api/notifyApi'

export const useAuthStore = defineStore('auth', {
  state: () => ({
    user: JSON.parse(localStorage.getItem('user') || 'null'),
    token: localStorage.getItem('token') || null,
    loading: false,
    error: null
  }),

  getters: {
    isAuthenticated: (state) => !!state.token,
    isAdmin: (state) => state.user?.role === 1 || state.user?.role === 0 || state.user?.role === 'Admin' || state.user?.role === 'admin' || state.user?.Role === 1 || state.user?.Role === 'Admin',
    isManager: (state) => state.user?.role === 2 || state.user?.role === 'Manager' || state.user?.role === 'manager' || state.user?.Role === 2 || state.user?.Role === 'Manager',
    displayName: (state) => state.user?.fullName || state.user?.username || 'User'
  },

  actions: {
    async login(credentials) {
      this.loading = true
      this.error = null
      try {
        const res = await loginApi(credentials)
        const data = res.data
        this.token = data.token || data.data?.token || data
        if (typeof this.token === 'object') {
          // Handle different response shapes
          this.token = this.token.token || JSON.stringify(this.token)
        }
        localStorage.setItem('token', this.token)
        await this.fetchUser()
        return true
      } catch (err) {
        this.error = err.response?.data?.message || err.response?.data || 'Đăng nhập thất bại'
        return false
      } finally {
        this.loading = false
      }
    },

    async register(userData) {
      this.loading = true
      this.error = null
      try {
        await registerApi(userData)
        return true
      } catch (err) {
        this.error = err.response?.data?.message || err.response?.data || 'Đăng ký thất bại'
        return false
      } finally {
        this.loading = false
      }
    },

    async fetchUser() {
      try {
        const res = await getMe()
        this.user = res.data.data || res.data
        localStorage.setItem('user', JSON.stringify(this.user))
      } catch {
        // Token may be invalid
      }
    },

    logout() {
      this.user = null
      this.token = null
      localStorage.removeItem('token')
      localStorage.removeItem('user')
    }
  }
})
