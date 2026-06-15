<template>
  <div class="project-detail">
    <LoadingSpinner v-if="loading" text="Đang tải dự án..." />

    <template v-else-if="project">
      <!-- Back button + Project header -->
      <div class="page-header">
        <div class="header-top">
          <button class="back-btn" @click="$router.push('/projects')">
            <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <polyline points="15 18 9 12 15 6" />
            </svg>
            Quay lại
          </button>
          <div class="header-actions">
            <BaseButton variant="secondary" size="sm" @click="showEditModal = true" id="btn-edit-project">
              <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <path d="M11 4H4a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2v-7" />
                <path d="M18.5 2.5a2.121 2.121 0 0 1 3 3L12 15l-4 1 1-4 9.5-9.5z" />
              </svg>
              Sửa
            </BaseButton>
            <BaseButton variant="danger" size="sm" @click="handleDelete" id="btn-delete-project">
              <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <polyline points="3 6 5 6 21 6" />
                <path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2" />
              </svg>
              Xóa
            </BaseButton>
          </div>
        </div>

        <div class="project-info">
          <div class="project-title-row">
            <div class="project-color-dot" :style="{ background: project.color || '#2563EB' }"></div>
            <h1>{{ project.name }}</h1>
            <StatusBadge :status="project.status" />
          </div>
          <p v-if="project.description" class="project-desc">{{ project.description }}</p>
          <div class="project-meta-row">
            <span class="meta">
              <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <rect x="3" y="4" width="18" height="18" rx="2" /><line x1="16" y1="2" x2="16" y2="6" /><line x1="8" y1="2" x2="8" y2="6" /><line x1="3" y1="10" x2="21" y2="10" />
              </svg>
              {{ formatDate(project.startDate) }} <template v-if="project.endDate">— {{ formatDate(project.endDate) }}</template>
            </span>
            <span class="meta">
              <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2" /><circle cx="9" cy="7" r="4" />
              </svg>
              {{ project.memberCount || members.length }} thành viên
            </span>
          </div>
        </div>
      </div>

      <!-- Tabs -->
      <div class="tab-bar">
        <button
          v-for="tab in tabs"
          :key="tab.key"
          :class="['tab', { active: activeTab === tab.key }]"
          @click="activeTab = tab.key"
        >
          {{ tab.label }}
          <span v-if="tab.count !== undefined" class="tab-count">{{ tab.count }}</span>
        </button>
      </div>

      <!-- Tab content -->
      <div class="tab-content">
        <!-- Members Tab -->
        <div v-if="activeTab === 'members'" class="tab-panel">
          <div class="panel-header">
            <h3>Thành viên ({{ members.length }})</h3>
            <BaseButton variant="primary" size="sm" @click="showAddMemberModal = true">
              <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <line x1="12" y1="5" x2="12" y2="19" /><line x1="5" y1="12" x2="19" y2="12" />
              </svg>
              Thêm thành viên
            </BaseButton>
          </div>

          <div v-if="members.length > 0" class="member-table-wrapper">
            <table class="data-table">
              <thead>
                <tr>
                  <th>Thành viên</th>
                  <th>Email</th>
                  <th>Vai trò</th>
                  <th>Ngày tham gia</th>
                  <th></th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="member in members" :key="member.id">
                  <td>
                    <div class="member-cell">
                      <div class="avatar-sm" :style="{ background: getAvatarColor(member.displayName) }">
                        {{ member.displayName?.charAt(0)?.toUpperCase() || '?' }}
                      </div>
                      <span class="font-medium">{{ member.displayName }}</span>
                    </div>
                  </td>
                  <td class="text-secondary">{{ member.email || '—' }}</td>
                  <td>
                    <select
                      :value="member.role"
                      @change="handleUpdateRole(member.id, $event.target.value)"
                      class="role-select"
                    >
                      <option value="Owner">Owner</option>
                      <option value="Admin">Admin</option>
                      <option value="Member">Member</option>
                      <option value="Viewer">Viewer</option>
                      <option value="0">Owner</option>
                      <option value="1">Admin</option>
                      <option value="2">Member</option>
                      <option value="3">Viewer</option>
                    </select>
                  </td>
                  <td class="text-secondary text-sm">{{ formatDate(member.joinedAt) }}</td>
                  <td>
                    <button class="icon-btn-sm danger" @click="handleRemoveMember(member.id)" title="Xóa">
                      <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                        <line x1="18" y1="6" x2="6" y2="18" /><line x1="6" y1="6" x2="18" y2="18" />
                      </svg>
                    </button>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
          <EmptyState v-else title="Chưa có thành viên" description="Thêm thành viên vào dự án để bắt đầu phân công." />
        </div>

        <!-- Sprints Tab -->
        <div v-if="activeTab === 'sprints'" class="tab-panel">
          <div class="panel-header">
            <h3>Sprints ({{ sprints.length }})</h3>
            <BaseButton variant="primary" size="sm" @click="showSprintModal = true; Object.assign(sprintForm, { name: '', goal: '', startDate: '', endDate: '' }); editingSprintId = null">
              <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <line x1="12" y1="5" x2="12" y2="19" /><line x1="5" y1="12" x2="19" y2="12" />
              </svg>
              Tạo Sprint
            </BaseButton>
          </div>

          <div v-if="sprints.length > 0" class="sprint-list">
            <div v-for="sprint in sprints" :key="sprint.id" class="sprint-card">
              <div class="sprint-header">
                <div class="sprint-info">
                  <h4>{{ sprint.name }}</h4>
                  <StatusBadge :status="sprint.status" />
                </div>
                <div class="sprint-actions">
                  <BaseButton
                    v-if="sprint.status === 'Planning'"
                    variant="primary" size="sm"
                    @click="handleStartSprint(sprint.id)"
                  >Bắt đầu</BaseButton>
                  <BaseButton
                    v-if="sprint.status === 'Active'"
                    variant="secondary" size="sm"
                    @click="handleCompleteSprint(sprint.id)"
                  >Hoàn thành</BaseButton>
                  <button class="icon-btn-sm" @click="openEditSprint(sprint)" title="Sửa">
                    <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                      <path d="M11 4H4a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2v-7" />
                      <path d="M18.5 2.5a2.121 2.121 0 0 1 3 3L12 15l-4 1 1-4 9.5-9.5z" />
                    </svg>
                  </button>
                </div>
              </div>
              <p v-if="sprint.goal" class="sprint-goal">{{ sprint.goal }}</p>
              <div class="sprint-dates">
                {{ formatDate(sprint.startDate) }} — {{ formatDate(sprint.endDate) }}
              </div>
            </div>
          </div>
          <EmptyState v-else title="Chưa có sprint nào" description="Tạo sprint đầu tiên để lên kế hoạch." />
        </div>

        <!-- Milestones Tab -->
        <div v-if="activeTab === 'milestones'" class="tab-panel">
          <div class="panel-header">
            <h3>Milestones ({{ milestones.length }})</h3>
            <BaseButton variant="primary" size="sm" @click="showMilestoneModal = true; Object.assign(milestoneForm, { title: '', description: '', dueDate: '' }); editingMilestoneId = null">
              <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <line x1="12" y1="5" x2="12" y2="19" /><line x1="5" y1="12" x2="19" y2="12" />
              </svg>
              Tạo Milestone
            </BaseButton>
          </div>

          <div v-if="milestones.length > 0" class="milestone-list">
            <div v-for="ms in milestones" :key="ms.id" class="milestone-card" :class="{ completed: ms.isCompleted }">
              <div class="milestone-check">
                <button
                  :class="['check-btn', { checked: ms.isCompleted }]"
                  @click="handleToggleMilestone(ms)"
                  :title="ms.isCompleted ? 'Đánh dấu chưa hoàn thành' : 'Đánh dấu hoàn thành'"
                >
                  <svg v-if="ms.isCompleted" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="3">
                    <polyline points="20 6 9 17 4 12" />
                  </svg>
                </button>
              </div>
              <div class="milestone-body">
                <div class="milestone-top">
                  <h4 :class="{ 'line-through': ms.isCompleted }">{{ ms.title }}</h4>
                  <div class="milestone-actions">
                    <button class="icon-btn-sm" @click="openEditMilestone(ms)" title="Sửa">
                      <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                        <path d="M11 4H4a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2v-7" />
                        <path d="M18.5 2.5a2.121 2.121 0 0 1 3 3L12 15l-4 1 1-4 9.5-9.5z" />
                      </svg>
                    </button>
                    <button class="icon-btn-sm danger" @click="handleDeleteMilestone(ms.id)" title="Xóa">
                      <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                        <polyline points="3 6 5 6 21 6" />
                        <path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2" />
                      </svg>
                    </button>
                  </div>
                </div>
                <p v-if="ms.description" class="milestone-desc">{{ ms.description }}</p>
                <span class="milestone-due" :class="{ overdue: isDueSoon(ms.dueDate) && !ms.isCompleted }">
                  Hạn: {{ formatDate(ms.dueDate) }}
                </span>
              </div>
            </div>
          </div>
          <EmptyState v-else title="Chưa có milestone nào" description="Tạo milestone để theo dõi tiến độ dự án." />
        </div>
        <!-- Discussion Tab -->
        <div v-if="activeTab === 'discussion'" class="tab-panel discussion-panel">
          <div class="panel-header">
            <h3>Thảo luận</h3>
          </div>
          <ProjectDiscussion :task-id="project.id" />
        </div>
      </div>
    </template>

    <EmptyState v-else title="Không tìm thấy dự án" description="Dự án không tồn tại hoặc bạn không có quyền truy cập." />

    <!-- Edit Project Modal -->
    <BaseModal v-model="showEditModal" title="Sửa dự án">
      <form @submit.prevent="handleUpdate">
        <BaseInput v-model="editForm.name" label="Tên dự án" required id="input-edit-name" />
        <BaseInput v-model="editForm.description" label="Mô tả" type="textarea" id="input-edit-desc" />
        <BaseInput v-model="editForm.startDate" label="Ngày bắt đầu" type="date" id="input-edit-start" />
        <BaseInput v-model="editForm.endDate" label="Ngày kết thúc" type="date" id="input-edit-end" />
      </form>
      <template #footer>
        <BaseButton variant="secondary" @click="showEditModal = false">Hủy</BaseButton>
        <BaseButton variant="primary" @click="handleUpdate">Lưu</BaseButton>
      </template>
    </BaseModal>

    <!-- Add Member Modal -->
    <BaseModal v-model="showAddMemberModal" title="Thêm thành viên" size="sm">
      <form @submit.prevent="handleAddMember">
        <BaseInput v-model="memberForm.userId" label="User ID" required id="input-member-userid" />
        <BaseInput v-model="memberForm.displayName" label="Tên hiển thị" required id="input-member-name" />
        <BaseInput v-model="memberForm.email" label="Email" type="email" id="input-member-email" />
        <div class="form-group">
          <label class="form-label">Vai trò</label>
          <select v-model="memberForm.role" class="form-select">
            <option :value="2">Member</option>
            <option :value="1">Admin</option>
            <option :value="3">Viewer</option>
          </select>
        </div>
      </form>
      <template #footer>
        <BaseButton variant="secondary" @click="showAddMemberModal = false">Hủy</BaseButton>
        <BaseButton variant="primary" @click="handleAddMember">Thêm</BaseButton>
      </template>
    </BaseModal>

    <!-- Sprint Modal -->
    <BaseModal v-model="showSprintModal" :title="editingSprintId ? 'Sửa Sprint' : 'Tạo Sprint'">
      <form @submit.prevent="handleSaveSprint">
        <BaseInput v-model="sprintForm.name" label="Tên Sprint" required id="input-sprint-name" />
        <BaseInput v-model="sprintForm.goal" label="Mục tiêu" type="textarea" id="input-sprint-goal" />
        <BaseInput v-model="sprintForm.startDate" label="Ngày bắt đầu" type="date" id="input-sprint-start" />
        <BaseInput v-model="sprintForm.endDate" label="Ngày kết thúc" type="date" id="input-sprint-end" />
      </form>
      <template #footer>
        <BaseButton variant="secondary" @click="showSprintModal = false">Hủy</BaseButton>
        <BaseButton variant="primary" @click="handleSaveSprint">{{ editingSprintId ? 'Lưu' : 'Tạo' }}</BaseButton>
      </template>
    </BaseModal>

    <!-- Milestone Modal -->
    <BaseModal v-model="showMilestoneModal" :title="editingMilestoneId ? 'Sửa Milestone' : 'Tạo Milestone'">
      <form @submit.prevent="handleSaveMilestone">
        <BaseInput v-model="milestoneForm.title" label="Tiêu đề" required id="input-milestone-title" />
        <BaseInput v-model="milestoneForm.description" label="Mô tả" type="textarea" id="input-milestone-desc" />
        <BaseInput v-model="milestoneForm.dueDate" label="Ngày hạn" type="date" required id="input-milestone-due" />
      </form>
      <template #footer>
        <BaseButton variant="secondary" @click="showMilestoneModal = false">Hủy</BaseButton>
        <BaseButton variant="primary" @click="handleSaveMilestone">{{ editingMilestoneId ? 'Lưu' : 'Tạo' }}</BaseButton>
      </template>
    </BaseModal>
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useProjectStore } from '../stores/projects'
import {
  getMembers, addMember, updateMemberRole, removeMember,
  getSprints, createSprint, updateSprint, startSprint, completeSprint,
  getMilestones, createMilestone, updateMilestone, deleteMilestone
} from '../api/projectApi'
import StatusBadge from '../components/common/StatusBadge.vue'
import BaseButton from '../components/common/BaseButton.vue'
import BaseModal from '../components/common/BaseModal.vue'
import BaseInput from '../components/common/BaseInput.vue'
import LoadingSpinner from '../components/common/LoadingSpinner.vue'
import EmptyState from '../components/common/EmptyState.vue'
import ProjectDiscussion from '../components/project/ProjectDiscussion.vue'

const route = useRoute()
const router = useRouter()
const projectStore = useProjectStore()

const loading = ref(true)
const project = ref(null)
const members = ref([])
const sprints = ref([])
const milestones = ref([])
const activeTab = ref('members')

// Modals
const showEditModal = ref(false)
const showAddMemberModal = ref(false)
const showSprintModal = ref(false)
const showMilestoneModal = ref(false)

const editingSprintId = ref(null)
const editingMilestoneId = ref(null)

const editForm = reactive({ name: '', description: '', startDate: '', endDate: '' })
const memberForm = reactive({ userId: '', displayName: '', email: '', role: 2 })
const sprintForm = reactive({ name: '', goal: '', startDate: '', endDate: '' })
const milestoneForm = reactive({ title: '', description: '', dueDate: '' })

const projectId = computed(() => route.params.id)

const tabs = computed(() => [
  { key: 'members', label: 'Thành viên', count: members.value.length },
  { key: 'sprints', label: 'Sprints', count: sprints.value.length },
  { key: 'milestones', label: 'Milestones', count: milestones.value.length },
  { key: 'discussion', label: 'Thảo luận' }
])

const avatarColors = ['#2563EB', '#7C3AED', '#DC2626', '#D97706', '#16A34A', '#0891B2', '#DB2777', '#4F46E5']

function getAvatarColor(name) {
  if (!name) return avatarColors[0]
  let hash = 0
  for (let i = 0; i < name.length; i++) hash = name.charCodeAt(i) + ((hash << 5) - hash)
  return avatarColors[Math.abs(hash) % avatarColors.length]
}

function formatDate(dateStr) {
  if (!dateStr) return '—'
  return new Date(dateStr).toLocaleDateString('vi-VN', { day: '2-digit', month: '2-digit', year: 'numeric' })
}

function isDueSoon(dateStr) {
  if (!dateStr) return false
  const due = new Date(dateStr)
  const now = new Date()
  return due <= now
}

async function loadProjectData() {
  loading.value = true
  try {
    const res = await projectStore.loadProject(projectId.value)
    project.value = res

    if (project.value) {
      // Load sub-resources
      if (project.value.members) {
        members.value = project.value.members
      } else {
        try {
          const mRes = await getMembers(projectId.value)
          members.value = mRes.data.data || mRes.data || []
        } catch { members.value = [] }
      }

      if (project.value.sprints) {
        sprints.value = project.value.sprints
      } else {
        try {
          const sRes = await getSprints(projectId.value)
          sprints.value = sRes.data.data || sRes.data || []
        } catch { sprints.value = [] }
      }

      if (project.value.milestones) {
        milestones.value = project.value.milestones
      } else {
        try {
          const msRes = await getMilestones(projectId.value)
          milestones.value = msRes.data.data || msRes.data || []
        } catch { milestones.value = [] }
      }

      // Populate edit form
      editForm.name = project.value.name
      editForm.description = project.value.description || ''
      editForm.startDate = project.value.startDate?.split('T')[0] || ''
      editForm.endDate = project.value.endDate?.split('T')[0] || ''
    }
  } finally {
    loading.value = false
  }
}

async function handleUpdate() {
  try {
    const payload = {
      name: editForm.name,
      description: editForm.description || null,
      startDate: editForm.startDate ? new Date(editForm.startDate).toISOString() : undefined,
      endDate: editForm.endDate ? new Date(editForm.endDate).toISOString() : null
    }
    await projectStore.updateProject(projectId.value, payload)
    showEditModal.value = false
    await loadProjectData()
  } catch (err) {
    alert(err)
  }
}

async function handleDelete() {
  if (!confirm('Bạn có chắc muốn xóa dự án này?')) return
  try {
    await projectStore.deleteProject(projectId.value)
    router.push('/projects')
  } catch (err) {
    alert(err)
  }
}

// Members
async function handleAddMember() {
  try {
    await addMember(projectId.value, { ...memberForm })
    showAddMemberModal.value = false
    memberForm.userId = ''
    memberForm.displayName = ''
    memberForm.email = ''
    memberForm.role = 2
    const res = await getMembers(projectId.value)
    members.value = res.data.data || res.data || []
  } catch (err) {
    alert(err.response?.data?.message || 'Không thể thêm thành viên')
  }
}

async function handleUpdateRole(memberId, role) {
  try {
    const roleNum = parseInt(role) || 0
    await updateMemberRole(projectId.value, memberId, { role: roleNum })
  } catch (err) {
    alert(err.response?.data?.message || 'Không thể cập nhật vai trò')
  }
}

async function handleRemoveMember(memberId) {
  if (!confirm('Xóa thành viên này?')) return
  try {
    await removeMember(projectId.value, memberId)
    members.value = members.value.filter(m => m.id !== memberId)
  } catch (err) {
    alert(err.response?.data?.message || 'Không thể xóa thành viên')
  }
}

// Sprints
function openEditSprint(sprint) {
  editingSprintId.value = sprint.id
  sprintForm.name = sprint.name
  sprintForm.goal = sprint.goal || ''
  sprintForm.startDate = sprint.startDate?.split('T')[0] || ''
  sprintForm.endDate = sprint.endDate?.split('T')[0] || ''
  showSprintModal.value = true
}

async function handleSaveSprint() {
  try {
    const payload = {
      name: sprintForm.name,
      goal: sprintForm.goal || null,
      startDate: sprintForm.startDate ? new Date(sprintForm.startDate).toISOString() : null,
      endDate: sprintForm.endDate ? new Date(sprintForm.endDate).toISOString() : null
    }
    if (editingSprintId.value) {
      await updateSprint(projectId.value, editingSprintId.value, payload)
    } else {
      await createSprint(projectId.value, payload)
    }
    showSprintModal.value = false
    const res = await getSprints(projectId.value)
    sprints.value = res.data.data || res.data || []
  } catch (err) {
    alert(err.response?.data?.message || 'Lỗi')
  }
}

async function handleStartSprint(sprintId) {
  try {
    await startSprint(projectId.value, sprintId)
    const res = await getSprints(projectId.value)
    sprints.value = res.data.data || res.data || []
  } catch (err) {
    alert(err.response?.data?.message || 'Không thể bắt đầu sprint')
  }
}

async function handleCompleteSprint(sprintId) {
  try {
    await completeSprint(projectId.value, sprintId)
    const res = await getSprints(projectId.value)
    sprints.value = res.data.data || res.data || []
  } catch (err) {
    alert(err.response?.data?.message || 'Không thể hoàn thành sprint')
  }
}

// Milestones
function openEditMilestone(ms) {
  editingMilestoneId.value = ms.id
  milestoneForm.title = ms.title
  milestoneForm.description = ms.description || ''
  milestoneForm.dueDate = ms.dueDate?.split('T')[0] || ''
  showMilestoneModal.value = true
}

async function handleSaveMilestone() {
  try {
    const payload = {
      title: milestoneForm.title,
      description: milestoneForm.description || null,
      dueDate: new Date(milestoneForm.dueDate).toISOString()
    }
    if (editingMilestoneId.value) {
      payload.isCompleted = milestones.value.find(m => m.id === editingMilestoneId.value)?.isCompleted || false
      await updateMilestone(projectId.value, editingMilestoneId.value, payload)
    } else {
      await createMilestone(projectId.value, payload)
    }
    showMilestoneModal.value = false
    const res = await getMilestones(projectId.value)
    milestones.value = res.data.data || res.data || []
  } catch (err) {
    alert(err.response?.data?.message || 'Lỗi')
  }
}

async function handleToggleMilestone(ms) {
  try {
    await updateMilestone(projectId.value, ms.id, {
      title: ms.title,
      description: ms.description,
      dueDate: ms.dueDate,
      isCompleted: !ms.isCompleted
    })
    ms.isCompleted = !ms.isCompleted
  } catch (err) {
    alert(err.response?.data?.message || 'Lỗi')
  }
}

async function handleDeleteMilestone(msId) {
  if (!confirm('Xóa milestone này?')) return
  try {
    await deleteMilestone(projectId.value, msId)
    milestones.value = milestones.value.filter(m => m.id !== msId)
  } catch (err) {
    alert(err.response?.data?.message || 'Không thể xóa')
  }
}

watch(() => route.params.id, () => {
  if (route.params.id) loadProjectData()
})

onMounted(() => {
  loadProjectData()
})
</script>

<style scoped>
.project-detail {
  animation: fadeIn 0.3s ease;
}

@keyframes fadeIn {
  from { opacity: 0; transform: translateY(8px); }
  to { opacity: 1; transform: translateY(0); }
}

/* Page header */
.header-top {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: var(--space-4);
}

.back-btn {
  display: flex;
  align-items: center;
  gap: var(--space-1);
  font-size: var(--font-size-sm);
  color: var(--color-text-secondary);
  font-weight: var(--font-weight-medium);
  padding: var(--space-1) var(--space-2);
  border-radius: var(--radius-md);
  transition: all var(--transition-fast);
}

.back-btn:hover {
  background: var(--color-bg-secondary);
  color: var(--color-text-primary);
}

.header-actions {
  display: flex;
  gap: var(--space-2);
}

.project-info {
  margin-bottom: var(--space-6);
}

.project-title-row {
  display: flex;
  align-items: center;
  gap: var(--space-3);
  margin-bottom: var(--space-2);
}

.project-color-dot {
  width: 14px;
  height: 14px;
  border-radius: var(--radius-full);
  flex-shrink: 0;
}

.project-title-row h1 {
  font-size: var(--font-size-2xl);
}

.project-desc {
  color: var(--color-text-secondary);
  font-size: var(--font-size-base);
  margin-bottom: var(--space-3);
  line-height: var(--line-height-relaxed);
}

.project-meta-row {
  display: flex;
  gap: var(--space-5);
}

.meta {
  display: flex;
  align-items: center;
  gap: var(--space-1);
  font-size: var(--font-size-sm);
  color: var(--color-text-secondary);
}

/* Tabs */
.tab-bar {
  display: flex;
  gap: var(--space-1);
  border-bottom: 1px solid var(--color-border);
  margin-bottom: 0;
}

.tab {
  display: flex;
  align-items: center;
  gap: var(--space-2);
  padding: var(--space-3) var(--space-4);
  font-size: var(--font-size-sm);
  font-weight: var(--font-weight-medium);
  color: var(--color-text-secondary);
  border-bottom: 2px solid transparent;
  transition: all var(--transition-fast);
  margin-bottom: -1px;
}

.tab:hover {
  color: var(--color-text-primary);
}

.tab.active {
  color: var(--color-primary);
  border-bottom-color: var(--color-primary);
}

.tab-count {
  min-width: 20px;
  height: 20px;
  background: var(--color-bg-secondary);
  border-radius: var(--radius-full);
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 11px;
  font-weight: var(--font-weight-semibold);
}

.tab.active .tab-count {
  background: var(--color-primary-light);
  color: var(--color-primary);
}

/* Tab content */
.tab-content {
  background: var(--color-white);
  border: 1px solid var(--color-border);
  border-top: none;
  border-radius: 0 0 var(--radius-lg) var(--radius-lg);
}

.tab-panel {
  padding: var(--space-6);
}

.panel-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: var(--space-5);
}

.panel-header h3 {
  font-size: var(--font-size-md);
}

/* Data table */
.member-table-wrapper {
  overflow-x: auto;
}

.data-table {
  width: 100%;
  border-collapse: collapse;
}

.data-table th {
  text-align: left;
  padding: var(--space-3) var(--space-4);
  font-size: var(--font-size-xs);
  font-weight: var(--font-weight-semibold);
  color: var(--color-text-tertiary);
  text-transform: uppercase;
  letter-spacing: 0.5px;
  border-bottom: 1px solid var(--color-border);
}

.data-table td {
  padding: var(--space-3) var(--space-4);
  font-size: var(--font-size-sm);
  border-bottom: 1px solid var(--color-bg-secondary);
  vertical-align: middle;
}

.data-table tr:hover td {
  background: var(--color-bg);
}

.member-cell {
  display: flex;
  align-items: center;
  gap: var(--space-3);
}

.avatar-sm {
  width: 32px;
  height: 32px;
  border-radius: var(--radius-full);
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-weight: var(--font-weight-semibold);
  font-size: var(--font-size-sm);
  flex-shrink: 0;
}

.role-select {
  padding: 4px 8px;
  border: 1px solid var(--color-border);
  border-radius: var(--radius-md);
  font-size: var(--font-size-sm);
  background: var(--color-white);
  cursor: pointer;
}

.icon-btn-sm {
  padding: 4px;
  border-radius: var(--radius-md);
  color: var(--color-text-tertiary);
  transition: all var(--transition-fast);
}

.icon-btn-sm:hover {
  background: var(--color-bg-secondary);
  color: var(--color-text-primary);
}

.icon-btn-sm.danger:hover {
  background: var(--color-danger-light);
  color: var(--color-danger);
}

/* Sprint cards */
.sprint-list {
  display: flex;
  flex-direction: column;
  gap: var(--space-3);
}

.sprint-card {
  border: 1px solid var(--color-border);
  border-radius: var(--radius-lg);
  padding: var(--space-4) var(--space-5);
  transition: box-shadow var(--transition-fast);
}

.sprint-card:hover {
  box-shadow: var(--shadow-sm);
}

.sprint-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: var(--space-2);
}

.sprint-info {
  display: flex;
  align-items: center;
  gap: var(--space-3);
}

.sprint-info h4 {
  font-size: var(--font-size-base);
  font-weight: var(--font-weight-semibold);
}

.sprint-actions {
  display: flex;
  gap: var(--space-2);
  align-items: center;
}

.sprint-goal {
  font-size: var(--font-size-sm);
  color: var(--color-text-secondary);
  margin-bottom: var(--space-2);
}

.sprint-dates {
  font-size: var(--font-size-xs);
  color: var(--color-text-tertiary);
}

/* Milestones */
.milestone-list {
  display: flex;
  flex-direction: column;
  gap: var(--space-3);
}

.milestone-card {
  display: flex;
  gap: var(--space-4);
  border: 1px solid var(--color-border);
  border-radius: var(--radius-lg);
  padding: var(--space-4) var(--space-5);
  transition: box-shadow var(--transition-fast);
}

.milestone-card:hover {
  box-shadow: var(--shadow-sm);
}

.milestone-card.completed {
  opacity: 0.7;
}

.check-btn {
  width: 24px;
  height: 24px;
  border: 2px solid var(--color-border-hover);
  border-radius: var(--radius-full);
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all var(--transition-fast);
  flex-shrink: 0;
  margin-top: 2px;
}

.check-btn:hover {
  border-color: var(--color-success);
}

.check-btn.checked {
  background: var(--color-success);
  border-color: var(--color-success);
  color: white;
}

.milestone-body {
  flex: 1;
  min-width: 0;
}

.milestone-top {
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
  gap: var(--space-2);
}

.milestone-top h4 {
  font-size: var(--font-size-base);
  font-weight: var(--font-weight-medium);
}

.line-through {
  text-decoration: line-through;
  color: var(--color-text-tertiary);
}

.milestone-actions {
  display: flex;
  gap: var(--space-1);
  flex-shrink: 0;
}

.milestone-desc {
  font-size: var(--font-size-sm);
  color: var(--color-text-secondary);
  margin-top: var(--space-1);
}

.milestone-due {
  font-size: var(--font-size-xs);
  color: var(--color-text-tertiary);
  margin-top: var(--space-2);
  display: block;
}

.milestone-due.overdue {
  color: var(--color-danger);
  font-weight: var(--font-weight-medium);
}

/* Form elements */
.form-group { margin-bottom: var(--space-4); }
.form-label {
  display: block;
  font-size: var(--font-size-sm);
  font-weight: var(--font-weight-medium);
  margin-bottom: var(--space-1);
}
.form-select {
  width: 100%;
  padding: 8px 12px;
  border: 1px solid var(--color-border);
  border-radius: var(--radius-md);
  font-size: var(--font-size-base);
  background: var(--color-white);
}
.form-select:focus {
  border-color: var(--color-primary);
  outline: none;
  box-shadow: 0 0 0 3px rgba(37, 99, 235, 0.1);
}

.alert { padding: var(--space-3) var(--space-4); border-radius: var(--radius-md); font-size: var(--font-size-sm); margin-bottom: var(--space-4); }
.alert-error { background: var(--color-danger-light); color: var(--color-danger); }
</style>
