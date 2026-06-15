import { defineStore } from 'pinia'
import {
  getProjects as fetchProjects,
  getProject as fetchProject,
  createProject as createProjectApi,
  updateProject as updateProjectApi,
  deleteProject as deleteProjectApi
} from '../api/projectApi'

export const useProjectStore = defineStore('projects', {
  state: () => ({
    projects: [],
    currentProject: null,
    loading: false,
    error: null
  }),

  getters: {
    activeProjects: (state) =>
      state.projects.filter(p => p.status === 'Active' || p.status === 'Planning'),
    projectCount: (state) => state.projects.length
  },

  actions: {
    async loadProjects(myProjects = false) {
      this.loading = true
      this.error = null
      try {
        const res = await fetchProjects(myProjects)
        this.projects = res.data.data || res.data || []
      } catch (err) {
        this.error = err.response?.data?.message || 'Không thể tải danh sách dự án'
        this.projects = []
      } finally {
        this.loading = false
      }
    },

    async loadProject(id) {
      this.loading = true
      this.error = null
      try {
        const res = await fetchProject(id)
        this.currentProject = res.data.data || res.data
        return this.currentProject
      } catch (err) {
        this.error = err.response?.data?.message || 'Không tìm thấy dự án'
        return null
      } finally {
        this.loading = false
      }
    },

    async createProject(data) {
      try {
        const res = await createProjectApi(data)
        const project = res.data.data || res.data
        this.projects.unshift(project)
        return project
      } catch (err) {
        throw err.response?.data?.message || 'Không thể tạo dự án'
      }
    },

    async updateProject(id, data) {
      try {
        const res = await updateProjectApi(id, data)
        const updated = res.data.data || res.data
        const idx = this.projects.findIndex(p => p.id === id)
        if (idx >= 0) this.projects[idx] = updated
        if (this.currentProject?.id === id) {
          this.currentProject = { ...this.currentProject, ...updated }
        }
        return updated
      } catch (err) {
        throw err.response?.data?.message || 'Không thể cập nhật dự án'
      }
    },

    async deleteProject(id) {
      try {
        await deleteProjectApi(id)
        this.projects = this.projects.filter(p => p.id !== id)
        if (this.currentProject?.id === id) this.currentProject = null
      } catch (err) {
        throw err.response?.data?.message || 'Không thể xóa dự án'
      }
    }
  }
})
