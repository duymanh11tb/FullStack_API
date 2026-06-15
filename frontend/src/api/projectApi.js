import { projectAPI } from './axios'

// ── Projects ──
export const getProjects = (myProjects = false) =>
  projectAPI.get('/api/Projects', { params: { myProjects } })

export const getProject = (id) =>
  projectAPI.get(`/api/Projects/${id}`)

export const createProject = (data) =>
  projectAPI.post('/api/Projects', data)

export const updateProject = (id, data) =>
  projectAPI.put(`/api/Projects/${id}`, data)

export const deleteProject = (id) =>
  projectAPI.delete(`/api/Projects/${id}`)

// ── Members ──
export const getMembers = (projectId) =>
  projectAPI.get(`/api/projects/${projectId}/Members`)

export const addMember = (projectId, data) =>
  projectAPI.post(`/api/projects/${projectId}/Members`, data)

export const updateMemberRole = (projectId, memberId, data) =>
  projectAPI.put(`/api/projects/${projectId}/Members/${memberId}`, data)

export const removeMember = (projectId, memberId) =>
  projectAPI.delete(`/api/projects/${projectId}/Members/${memberId}`)

// ── Sprints ──
export const getSprints = (projectId) =>
  projectAPI.get(`/api/projects/${projectId}/Sprints`)

export const getSprint = (projectId, sprintId) =>
  projectAPI.get(`/api/projects/${projectId}/Sprints/${sprintId}`)

export const createSprint = (projectId, data) =>
  projectAPI.post(`/api/projects/${projectId}/Sprints`, data)

export const updateSprint = (projectId, sprintId, data) =>
  projectAPI.put(`/api/projects/${projectId}/Sprints/${sprintId}`, data)

export const startSprint = (projectId, sprintId) =>
  projectAPI.put(`/api/projects/${projectId}/Sprints/${sprintId}/start`)

export const completeSprint = (projectId, sprintId) =>
  projectAPI.put(`/api/projects/${projectId}/Sprints/${sprintId}/complete`)

// ── Milestones ──
export const getMilestones = (projectId) =>
  projectAPI.get(`/api/projects/${projectId}/Milestones`)

export const getMilestone = (projectId, milestoneId) =>
  projectAPI.get(`/api/projects/${projectId}/Milestones/${milestoneId}`)

export const createMilestone = (projectId, data) =>
  projectAPI.post(`/api/projects/${projectId}/Milestones`, data)

export const updateMilestone = (projectId, milestoneId, data) =>
  projectAPI.put(`/api/projects/${projectId}/Milestones/${milestoneId}`, data)

export const deleteMilestone = (projectId, milestoneId) =>
  projectAPI.delete(`/api/projects/${projectId}/Milestones/${milestoneId}`)
