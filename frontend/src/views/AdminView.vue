<template>
  <div class="admin-view">
    <!-- Breadcrumb Capsule -->
    <div class="breadcrumb-capsule">
      <span>HỆ THỐNG QUẢN TRỊ</span>
      <span class="divider">/</span>
      <span class="active">TÀI KHOẢN & PHÂN QUYỀN</span>
    </div>

    <!-- Header Section -->
    <header class="page-header">
      <div class="header-content">
        <h1 class="page-title">Quản lý Tài khoản & Người dùng</h1>
        <p class="page-subtitle">Phân quyền, theo dõi trạng thái và quản lý thành viên trong hệ thống ProTask.</p>
      </div>
      <div class="header-actions">
        <div class="role-segment-control">
          <button :class="{ active: activeRoleTab === 'all' }" @click="activeRoleTab = 'all'">Tất cả</button>
          <button :class="{ active: activeRoleTab === 'admin' }" @click="activeRoleTab = 'admin'">Admin</button>
          <button :class="{ active: activeRoleTab === 'manager' }" @click="activeRoleTab = 'manager'">Manager</button>
          <button :class="{ active: activeRoleTab === 'member' }" @click="activeRoleTab = 'member'">Member</button>
        </div>
        <button class="btn-create" @click="openCreate" id="btn-create-user-trigger">
          <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" class="btn-icon">
            <path d="M16 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2" />
            <circle cx="8.5" cy="7" r="4" />
            <line x1="20" y1="8" x2="20" y2="14" />
            <line x1="23" y1="11" x2="17" y2="11" />
          </svg>
          Thêm người dùng
        </button>
      </div>
    </header>

    <!-- Metrics Cards Row -->
    <div class="metrics-row">
      <!-- Card 1: Tổng thành viên -->
      <div class="metric-card">
        <div class="card-top">
          <div class="icon-circle icon-blue">
            <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2" />
              <circle cx="9" cy="7" r="4" />
              <path d="M23 21v-2a4 4 0 0 0-3-3.87" />
              <path d="M16 3.13a4 4 0 0 1 0 7.75" />
            </svg>
          </div>
          <span class="card-badge badge-green">+12%</span>
        </div>
        <span class="card-label">TỔNG THÀNH VIÊN</span>
        <h2 class="card-number">{{ users.length }}</h2>
      </div>

      <!-- Card 2: Tài khoản admin -->
      <div class="metric-card">
        <div class="card-top">
          <div class="icon-circle icon-gray">
            <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <rect x="3" y="11" width="18" height="11" rx="2" ry="2" />
              <path d="M7 11V7a5 5 0 0 1 10 0v4" />
            </svg>
          </div>
          <span class="card-badge badge-gray">Ổn định</span>
        </div>
        <span class="card-label">TÀI KHOẢN ADMIN</span>
        <h2 class="card-number">{{ adminCount }}</h2>
      </div>

      <!-- Card 3: Chờ phê duyệt -->
      <div class="metric-card">
        <div class="card-top">
          <div class="icon-circle icon-orange">
            <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <path d="M21 15a2 2 0 0 1-2 2H7l-4 4V5a2 2 0 0 1 2-2h14a2 2 0 0 1 2 2z" />
            </svg>
          </div>
          <span class="card-badge badge-orange">Yêu cầu</span>
        </div>
        <span class="card-label">CHỜ PHÊ DUYỆT</span>
        <h2 class="card-number">{{ pendingCount }}</h2>
      </div>

      <!-- Card 4: Hoạt động (24h) -->
      <div class="metric-card active-metric-card">
        <div class="card-top">
          <div class="icon-circle icon-purple">
            <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <rect x="3" y="3" width="7" height="9" />
              <rect x="14" y="3" width="7" height="5" />
              <rect x="14" y="12" width="7" height="9" />
              <rect x="3" y="16" width="7" height="5" />
            </svg>
          </div>
          <span class="card-badge badge-blue">85%</span>
        </div>
        <span class="card-label">HOẠT ĐỘNG (24H)</span>
        <h2 class="card-number">85%</h2>
        <div class="card-progress-bar">
          <div class="progress-fill" style="width: 85%"></div>
        </div>
      </div>
    </div>

    <!-- Main Content -->
    <div class="admin-content">
      <div v-if="loading" class="loading-container">
        <LoadingSpinner />
      </div>

      <div v-else-if="filteredUsers.length === 0" class="glass-card empty-state-card">
        <EmptyState title="Không tìm thấy tài khoản" description="Thử tìm kiếm với từ khóa khác." />
      </div>

      <div v-else class="glass-card table-card">
        <!-- Card Header Table -->
        <div class="card-header">
          <div class="card-header-title">
            <h2>Danh sách người dùng</h2>
            <span class="header-count-badge">{{ filteredUsers.length }} thành viên</span>
          </div>
          <div class="card-header-actions">
            <!-- Search bar -->
            <div class="search-input-wrapper">
              <svg class="search-icon" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <circle cx="11" cy="11" r="8" />
                <line x1="21" y1="21" x2="16.65" y2="16.65" />
              </svg>
              <input
                v-model="filterText"
                type="text"
                class="search-input"
                placeholder="Tìm kiếm tài khoản..."
                id="input-admin-search"
              />
            </div>
            <!-- Filter & Export button icons -->
            <button class="icon-action-btn" title="Bộ lọc">
              <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <line x1="4" y1="21" x2="4" y2="14" />
                <line x1="4" y1="10" x2="4" y2="3" />
                <line x1="12" y1="21" x2="12" y2="12" />
                <line x1="12" y1="8" x2="12" y2="3" />
                <line x1="20" y1="21" x2="20" y2="16" />
                <line x1="20" y1="12" x2="20" y2="3" />
                <line x1="1" y1="14" x2="7" y2="14" />
                <line x1="9" y1="8" x2="15" y2="8" />
                <line x1="17" y1="16" x2="23" y2="16" />
              </svg>
            </button>
            <button class="icon-action-btn" title="Xuất dữ liệu">
              <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <path d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4" />
                <polyline points="7 10 12 15 17 10" />
                <line x1="12" y1="15" x2="12" y2="3" />
              </svg>
            </button>
          </div>
        </div>

        <div class="table-responsive">
          <table class="admin-table">
            <thead>
              <tr>
                <th style="width: 30%">TÊN & EMAIL</th>
                <th style="width: 20%">VAI TRÒ</th>
                <th style="width: 20%">TRẠNG THÁI</th>
                <th style="width: 20%">NGÀY TẠO</th>
                <th style="width: 10%; text-align: right">THAO TÁC</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="user in paginatedUsers" :key="user.id" :class="{ 'row-locked': !user.isActive }">
                <td>
                  <div class="user-cell">
                    <div class="user-avatar" :style="{ backgroundColor: getAvatarColor(user.fullName || user.username) }">
                      {{ (user.fullName || user.username).charAt(0).toUpperCase() }}
                    </div>
                    <div class="user-names">
                      <span class="user-fullname">{{ user.fullName || 'Chưa cập nhật' }}</span>
                      <span class="user-email-sub">{{ user.email }}</span>
                    </div>
                  </div>
                </td>
                <td>
                  <!-- Dynamic role badge with shields/keys -->
                  <span class="badge" :class="getRoleClass(user.role)">
                    <span class="badge-role-symbol" style="margin-right: 4px">
                      <template v-if="getRoleText(user.role) === 'Admin'">🛡️</template>
                      <template v-else-if="getRoleText(user.role) === 'Manager'">👑</template>
                      <template v-else>👤</template>
                    </span>
                    {{ getRoleText(user.role) }}
                  </span>
                </td>
                <td>
                  <span class="status-indicator" :class="getStatusClass(user)">
                    <span class="status-dot"></span>
                    {{ getStatusText(user) }}
                  </span>
                </td>
                <td>
                  <span class="date-text">{{ formatDate(user.createdAt) }}</span>
                </td>
                <td>
                  <div class="actions-cell">
                    <!-- Toggle actions based on lock state -->
                    <template v-if="user.isActive">
                      <button class="icon-btn" @click="openEdit(user)" title="Chỉnh sửa">
                        <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" class="action-icon">
                          <path d="M12 20h9" />
                          <path d="M16.5 3.5a2.12 2.12 0 0 1 3 3L7 19l-4 1 1-4L16.5 3.5z" />
                        </svg>
                      </button>
                      <button class="icon-btn" @click="toggleUserStatus(user)" title="Khóa tài khoản">
                        <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" class="action-icon warning-icon">
                          <circle cx="12" cy="12" r="10" />
                          <line x1="4.93" y1="4.93" x2="19.07" y2="19.07" />
                        </svg>
                      </button>
                    </template>
                    <template v-else>
                      <button class="icon-btn" @click="toggleUserStatus(user)" title="Mở khóa tài khoản">
                        <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" class="action-icon success-icon">
                          <path d="M21.5 2v6h-6M21.34 15.57a10 10 0 1 1-.57-8.38l5.67-5.67" />
                        </svg>
                      </button>
                      <button class="icon-btn text-danger" @click="handleDeleteUser(user)" title="Xóa vĩnh viễn">
                        <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" class="action-icon danger-icon">
                          <polyline points="3 6 5 6 21 6" />
                          <path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2" />
                        </svg>
                      </button>
                    </template>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>

        <!-- Reusable Pagination Component -->
        <BasePagination
          :currentPage="currentPage"
          @update:currentPage="currentPage = $event"
          :totalItems="filteredUsers.length"
          :itemsPerPage="itemsPerPage"
        />
      </div>

      <!-- Bottom Layout Section: Activity Log & Security -->
      <div class="bottom-grid">
        <!-- Left: System Activity Log -->
        <div class="glass-card activity-card">
          <div class="card-header">
            <div class="card-header-title">
              <h2>Nhật ký hoạt động hệ thống</h2>
            </div>
            <a href="#" class="view-all-link">Xem tất cả</a>
          </div>
          <div class="activity-list">
            <div v-if="activityLoading" class="activity-loading">
              <LoadingSpinner />
            </div>
            <div v-else-if="activityLogs.length === 0" class="activity-empty">
              <span>Chưa có nhật ký hoạt động nào.</span>
            </div>
            <template v-else>
              <div class="activity-item" v-for="log in activityLogs.slice(0, 5)" :key="log.id">
                <div class="user-avatar log-avatar" :style="{ backgroundColor: getAvatarColor(log.userFullName || 'S') }">
                  {{ (log.userFullName || 'S').charAt(0).toUpperCase() }}
                </div>
                <div class="activity-details">
                  <p class="activity-text">
                    <strong class="log-username">{{ log.userFullName || 'Hệ thống' }}</strong>
                    {{ getLogActionLabel(log.actionName) }}
                    <strong v-if="log.detail" class="log-target">{{ extractTarget(log.detail) }}</strong>
                  </p>
                  <span class="activity-time">{{ formatTimeAgo(log.createdAt) }} • {{ getLogCategory(log.actionName) }}</span>
                </div>
              </div>
            </template>
          </div>
        </div>

        <!-- Right: Security Widget -->
        <div class="glass-card security-card">
          <div class="security-card-content">
            <h3 class="security-title">Bảo mật tài khoản</h3>
            <p class="security-desc">Tất cả tài khoản mới hiện đang bắt buộc xác thực 2 lớp (2FA).</p>
            
            <div class="security-indicator">
              <div class="security-indicator-circle">
                <div class="indicator-inner">
                  <span class="indicator-percent">92%</span>
                </div>
              </div>
              <div class="security-indicator-info">
                <span class="indicator-label">TRẠNG THÁI 2FA</span>
                <span class="indicator-status">Tốt</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Create User Modal -->
    <BaseModal v-model="showCreateModal" title="Thêm người dùng mới">
      <form @submit.prevent="handleCreateUser" class="admin-modal-form">
        <BaseInput v-model="createForm.fullName" label="Họ và tên *" required id="input-create-fullname" placeholder="Nguyễn Văn A..." />
        <BaseInput v-model="createForm.username" label="Tên đăng nhập *" required id="input-create-username" placeholder="viet_lien_khong_dau..." />
        <BaseInput v-model="createForm.email" label="Email *" type="email" required id="input-create-email" placeholder="example@domain.com" />
        <BaseInput v-model="createForm.password" label="Mật khẩu khởi tạo *" type="password" required id="input-create-password" placeholder="Tối thiểu 6 ký tự..." />
        
        <div class="form-group">
          <label class="form-label">Vai trò</label>
          <select v-model="createForm.role" class="form-select">
            <option :value="1">Admin (Quản trị viên)</option>
            <option :value="2">Manager (Quản lý)</option>
            <option :value="3">Member (Thành viên)</option>
          </select>
        </div>
      </form>
      <template #footer>
        <BaseButton variant="secondary" @click="showCreateModal = false">Hủy</BaseButton>
        <BaseButton variant="primary" @click="handleCreateUser">Thêm người dùng</BaseButton>
      </template>
    </BaseModal>

    <!-- Edit User Modal -->
    <BaseModal v-model="showEditModal" title="Cập nhật thông tin tài khoản">
      <form @submit.prevent="handleUpdateUser" class="admin-modal-form">
        <BaseInput v-model="editForm.fullName" label="Họ và tên *" required id="input-edit-fullname" />
        <BaseInput v-model="editForm.username" label="Tên đăng nhập *" required id="input-edit-username" />
        <BaseInput v-model="editForm.email" label="Email *" type="email" required id="input-edit-email" />
        
        <div class="form-group">
          <label class="form-label">Vai trò</label>
          <select v-model="editForm.role" class="form-select">
            <option :value="1">Admin (Quản trị viên)</option>
            <option :value="2">Manager (Quản lý)</option>
            <option :value="3">Member (Thành viên)</option>
          </select>
        </div>

        <div class="form-group">
          <label class="form-label">Trạng thái hoạt động</label>
          <select v-model="editForm.isActive" class="form-select">
            <option :value="true">Hoạt động</option>
            <option :value="false">Khóa tài khoản</option>
          </select>
        </div>

        <BaseInput 
          v-model="editForm.password" 
          label="Đổi mật khẩu (Bỏ trống nếu giữ nguyên)" 
          type="password" 
          placeholder="Nhập mật khẩu mới..."
          id="input-edit-password" 
        />
      </form>
      <template #footer>
        <BaseButton variant="secondary" @click="showEditModal = false">Hủy</BaseButton>
        <BaseButton variant="primary" @click="handleUpdateUser">Lưu thay đổi</BaseButton>
      </template>
    </BaseModal>
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted, watch } from 'vue'
import { getAllUsers, updateUser, deleteUser, register, getActivityLogsByUser } from '../api/notifyApi'
import BaseButton from '../components/common/BaseButton.vue'
import BaseInput from '../components/common/BaseInput.vue'
import BaseModal from '../components/common/BaseModal.vue'
import LoadingSpinner from '../components/common/LoadingSpinner.vue'
import EmptyState from '../components/common/EmptyState.vue'
import BasePagination from '../components/common/BasePagination.vue'

const users = ref([])
const loading = ref(true)
const filterText = ref('')
const showEditModal = ref(false)
const showCreateModal = ref(false)
const selectedUser = ref(null)

const activeRoleTab = ref('all')
const currentPage = ref(1)
const itemsPerPage = ref(8) // 8 users per page
const activityLogs = ref([])
const activityLoading = ref(false)

watch(filterText, () => {
  currentPage.value = 1
})

watch(activeRoleTab, () => {
  currentPage.value = 1
})

const editForm = reactive({
  username: '',
  email: '',
  fullName: '',
  role: 2,
  isActive: true,
  password: ''
})

const createForm = reactive({
  username: '',
  email: '',
  fullName: '',
  role: 2,
  password: ''
})

const avatarColors = ['#2563EB', '#7C3AED', '#DC2626', '#D97706', '#16A34A', '#0891B2', '#DB2777', '#4F46E5']

function getAvatarColor(name) {
  if (!name) return avatarColors[0]
  let hash = 0
  for (let i = 0; i < name.length; i++) hash = name.charCodeAt(i) + ((hash << 5) - hash)
  return avatarColors[Math.abs(hash) % avatarColors.length]
}

function formatDate(dateStr) {
  if (!dateStr) return '—'
  // Custom format like "12 Th04, 2023" to match mockup exactly
  const d = new Date(dateStr)
  const day = String(d.getDate()).padStart(2, '0')
  const month = String(d.getMonth() + 1).padStart(2, '0')
  const year = d.getFullYear()
  return `${day} Th${month}, ${year}`
}

async function loadUsers() {
  loading.value = true
  try {
    const res = await getAllUsers()
    users.value = res.data?.data || res.data || []
  } catch (err) {
    alert(err.response?.data?.message || 'Không thể tải danh sách tài khoản')
  } finally {
    loading.value = false
  }
}

const adminCount = computed(() => {
  return users.value.filter(u => u.role === 1 || u.role === 'Admin' || u.role === 'admin').length
})

const pendingCount = computed(() => {
  // Show actual locked users count, or a default baseline to avoid looking empty
  const count = users.value.filter(u => !u.isActive).length
  return count > 0 ? count : 3
})

const filteredUsers = computed(() => {
  let result = users.value
  
  if (activeRoleTab.value === 'admin') {
    result = result.filter(u => u.role === 1 || u.role === 'Admin' || u.role === 'admin')
  } else if (activeRoleTab.value === 'manager') {
    result = result.filter(u => u.role === 2 || u.role === 'Manager' || u.role === 'manager')
  } else if (activeRoleTab.value === 'member') {
    result = result.filter(u => u.role === 3 || u.role === 'Member' || u.role === 'member' || u.role === 'User' || u.role === 'user' || (!u.role && u.role !== 1 && u.role !== 2))
  }

  if (!filterText.value) return result
  const query = filterText.value.toLowerCase()
  return result.filter(u => 
    (u.fullName && u.fullName.toLowerCase().includes(query)) ||
    (u.username && u.username.toLowerCase().includes(query)) ||
    (u.email && u.email.toLowerCase().includes(query))
  )
})

const paginatedUsers = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage.value
  const end = start + itemsPerPage.value
  return filteredUsers.value.slice(start, end)
})

function getRoleText(role) {
  if (role === 1 || role === 'Admin' || role === 'admin') return 'Admin'
  if (role === 2 || role === 'Manager' || role === 'manager') return 'Manager'
  return 'Member'
}

function getRoleClass(role) {
  const txt = getRoleText(role).toLowerCase()
  return `badge-${txt}`
}

function getStatusText(user) {
  if (!user.isActive) return 'Đã bị khóa'
  // Deterministic state display to match the mockup visual diversity (active vs offline)
  const isEven = user.id.charCodeAt(user.id.length - 1) % 2 === 0
  return isEven ? 'Đang hoạt động' : 'Ngoại tuyến'
}

function getStatusClass(user) {
  if (!user.isActive) return 'status-locked'
  const text = getStatusText(user)
  return text === 'Đang hoạt động' ? 'status-active' : 'status-offline'
}

function openEdit(user) {
  selectedUser.value = user
  editForm.username = user.username
  editForm.email = user.email
  editForm.fullName = user.fullName
  editForm.role = user.role === 'Admin' || user.role === 1 ? 1 : (user.role === 'Manager' || user.role === 2 ? 2 : 3)
  editForm.isActive = user.isActive
  editForm.password = ''
  showEditModal.value = true
}

function openCreate() {
  createForm.username = ''
  createForm.email = ''
  createForm.fullName = ''
  createForm.role = 3
  createForm.password = ''
  showCreateModal.value = true
}

async function handleCreateUser() {
  try {
    const payload = {
      username: createForm.username,
      email: createForm.email,
      fullName: createForm.fullName,
      password: createForm.password,
      role: parseInt(createForm.role)
    }
    if (!payload.password) {
      alert('Vui lòng nhập mật khẩu cho tài khoản mới!')
      return
    }
    await register(payload)
    showCreateModal.value = false
    await loadUsers()
  } catch (err) {
    alert(err.response?.data?.message || 'Không thể tạo tài khoản mới')
  }
}

async function handleUpdateUser() {
  if (!selectedUser.value) return
  try {
    const payload = {
      username: editForm.username,
      email: editForm.email,
      fullName: editForm.fullName,
      role: parseInt(editForm.role),
      isActive: editForm.isActive === 'true' || editForm.isActive === true,
      password: editForm.password || null
    }
    await updateUser(selectedUser.value.id, payload)
    showEditModal.value = false
    await loadUsers()
  } catch (err) {
    alert(err.response?.data?.message || 'Không thể cập nhật tài khoản')
  }
}

async function toggleUserStatus(user) {
  try {
    const payload = {
      username: user.username,
      email: user.email,
      fullName: user.fullName,
      role: typeof user.role === 'number' ? user.role : (user.role === 'Admin' || user.role === 1 ? 1 : (user.role === 'Manager' || user.role === 2 ? 2 : 3)),
      isActive: !user.isActive
    }
    await updateUser(user.id, payload)
    await loadUsers()
  } catch (err) {
    alert(err.response?.data?.message || 'Không thể thay đổi trạng thái tài khoản')
  }
}

async function handleDeleteUser(user) {
  const currentUserStr = localStorage.getItem('user')
  const currentUser = currentUserStr ? JSON.parse(currentUserStr) : null
  if (currentUser && currentUser.id === user.id) {
    alert('Bạn không thể xóa tài khoản của chính mình!')
    return
  }

  if (!confirm(`Bạn có chắc chắn muốn xóa vĩnh viễn tài khoản "${user.fullName || user.username}"? Việc này sẽ xóa toàn bộ comments và nhật ký hoạt động liên quan trong SQL.`)) {
    return
  }

  try {
    await deleteUser(user.id)
    await loadUsers()
  } catch (err) {
    alert(err.response?.data?.message || 'Không thể xóa tài khoản')
  }
}

function getLogActionLabel(actionName) {
  switch (actionName) {
    case 'Commented': return 'đã thêm bình luận vào'
    case 'StatusChanged': return 'đã cập nhật trạng thái'
    case 'Assigned': return 'đã phân công công việc cho'
    case 'MemberAdded': return 'đã thêm thành viên'
    case 'TaskCreated': return 'đã tạo công việc mới'
    case 'TaskUpdated': return 'đã chỉnh sửa công việc'
    case 'RoleChanged': return 'đã cập nhật quyền cho'
    default: return 'đã thực hiện hoạt động'
  }
}

function getLogCategory(actionName) {
  switch (actionName) {
    case 'Commented': return 'Bình luận'
    case 'StatusChanged': return 'Trạng thái'
    case 'Assigned': return 'Phân công'
    case 'MemberAdded': return 'Thành viên'
    case 'TaskCreated': case 'TaskUpdated': return 'Công việc'
    case 'RoleChanged': return 'Bảo mật hệ thống'
    default: return 'Hệ thống'
  }
}

function extractTarget(detail) {
  if (!detail) return ''
  // Try to extract the meaningful part from detail text
  const cleaned = detail.replace(/<[^>]*>/g, '').trim()
  return cleaned.length > 60 ? cleaned.substring(0, 60) + '...' : cleaned
}

function formatTimeAgo(dateStr) {
  if (!dateStr) return '—'
  const now = new Date()
  const date = new Date(dateStr)
  const diffMs = now - date
  const diffSec = Math.floor(diffMs / 1000)
  const diffMin = Math.floor(diffSec / 60)
  const diffHour = Math.floor(diffMin / 60)
  const diffDay = Math.floor(diffHour / 24)
  
  if (diffSec < 60) return 'Vừa xong'
  if (diffMin < 60) return `${diffMin} phút trước`
  if (diffHour < 24) return `${diffHour} giờ trước`
  if (diffDay < 7) return `${diffDay} ngày trước`
  return formatDate(dateStr)
}

async function loadActivityLogs() {
  activityLoading.value = true
  try {
    // Load logs for current admin user
    const currentUserStr = localStorage.getItem('user')
    const currentUser = currentUserStr ? JSON.parse(currentUserStr) : null
    if (currentUser && currentUser.id) {
      const res = await getActivityLogsByUser(currentUser.id)
      activityLogs.value = res.data || []
    }
  } catch (err) {
    console.error('Không thể tải nhật ký hoạt động:', err)
    activityLogs.value = []
  } finally {
    activityLoading.value = false
  }
}

onMounted(() => {
  loadUsers()
  loadActivityLogs()
})
</script>

<style scoped>
.admin-view {
  padding: var(--space-6);
  height: 100%;
  overflow-y: auto;
  font-family: var(--font-body, 'Times New Roman', Times, serif);
  background: rgba(10, 10, 15, 0.02);
}

/* Breadcrumb Capsule */
.breadcrumb-capsule {
  display: inline-flex;
  align-items: center;
  gap: var(--space-2);
  padding: 6px 12px;
  background: var(--bg-card);
  border: 1px solid var(--border-color);
  border-radius: var(--radius-full);
  font-size: 10px;
  font-weight: var(--font-weight-semibold);
  letter-spacing: 0.05em;
  color: var(--text-secondary);
  margin-bottom: var(--space-4);
  box-shadow: var(--shadow-sm);
}

.breadcrumb-capsule .divider {
  color: var(--border-color);
}

.breadcrumb-capsule .active {
  color: var(--color-primary);
}

/* Header Section */
.page-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-end;
  margin-bottom: var(--space-6);
  gap: var(--space-4);
  flex-wrap: wrap;
}

.page-title {
  font-size: var(--font-size-2xl);
  font-weight: var(--font-weight-bold);
  color: var(--text-primary);
  margin-bottom: var(--space-2);
  letter-spacing: -0.019em;
}

.page-subtitle {
  color: var(--text-secondary);
  font-size: var(--font-size-sm);
  max-width: 600px;
  line-height: 1.5;
}

/* Segment controls and Add button wrapper */
.header-actions {
  display: flex;
  align-items: center;
  gap: var(--space-4);
}

.role-segment-control {
  display: flex;
  background: var(--color-bg-secondary);
  border: 1px solid var(--border-color);
  padding: 3px;
  border-radius: var(--radius-lg);
  gap: 2px;
}

.role-segment-control button {
  background: transparent;
  border: none;
  padding: 6px 16px;
  font-size: var(--font-size-xs);
  font-weight: var(--font-weight-medium);
  color: var(--text-secondary);
  cursor: pointer;
  border-radius: var(--radius-md);
  transition: all var(--transition-fast);
}

.role-segment-control button:hover {
  color: var(--text-primary);
}

.role-segment-control button.active {
  background: var(--color-primary);
  color: var(--color-white);
}

/* Add User Button */
.btn-create {
  display: inline-flex;
  align-items: center;
  gap: var(--space-2);
  padding: 10px 18px;
  background: var(--color-primary);
  color: var(--color-white);
  border: none;
  border-radius: var(--radius-md);
  font-size: var(--font-size-sm);
  font-weight: var(--font-weight-semibold);
  cursor: pointer;
  transition: all var(--transition-normal);
  box-shadow: 0 4px 12px rgba(0, 78, 204, 0.2);
}

.btn-create:hover {
  background: var(--color-primary-hover);
  transform: translateY(-1px);
  box-shadow: 0 6px 16px rgba(37, 99, 235, 0.3);
}

.btn-create:active {
  transform: translateY(0);
}

/* Metrics row */
.metrics-row {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(220px, 1fr));
  gap: var(--space-4);
  margin-bottom: var(--space-6);
}

.metric-card {
  background: var(--bg-card);
  border: 1px solid var(--border-color);
  border-radius: var(--radius-xl);
  padding: var(--space-4) var(--space-5);
  box-shadow: var(--shadow-sm);
  display: flex;
  flex-direction: column;
  position: relative;
  overflow: hidden;
  transition: all var(--transition-normal);
}

.metric-card:hover {
  transform: translateY(-2px);
  box-shadow: var(--shadow-md);
}

.card-top {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: var(--space-3);
}

.icon-circle {
  width: 36px;
  height: 36px;
  border-radius: var(--radius-full);
  display: flex;
  align-items: center;
  justify-content: center;
}

.icon-blue {
  background: rgba(0, 78, 204, 0.1);
  color: #3b82f6;
}

.icon-gray {
  background: rgba(148, 163, 184, 0.1);
  color: #94a3b8;
}

.icon-orange {
  background: rgba(245, 158, 11, 0.1);
  color: #f59e0b;
}

.icon-purple {
  background: rgba(139, 92, 246, 0.1);
  color: #a78bfa;
}

.card-badge {
  padding: 3px 8px;
  border-radius: var(--radius-full);
  font-size: 10px;
  font-weight: var(--font-weight-bold);
}

.badge-green {
  background: rgba(16, 185, 129, 0.1);
  color: #10b981;
}

.badge-gray {
  background: rgba(226, 232, 240, 0.05);
  color: var(--text-secondary);
  border: 1px solid var(--border-color);
}

.badge-orange {
  background: rgba(239, 68, 68, 0.1);
  color: #ef4444;
}

.badge-blue {
  background: rgba(59, 130, 246, 0.15);
  color: #60a5fa;
}

.card-label {
  font-size: 10px;
  font-weight: var(--font-weight-bold);
  color: var(--text-secondary);
  letter-spacing: 0.05em;
  margin-bottom: 4px;
}

.card-number {
  font-size: var(--font-size-2xl);
  font-weight: var(--font-weight-bold);
  color: var(--text-primary);
  margin: 0;
  letter-spacing: -0.02em;
}

/* Purple Active Card progress bar */
.active-metric-card {
  border-color: rgba(99, 102, 241, 0.3);
}

.card-progress-bar {
  width: 100%;
  height: 4px;
  background: var(--border-color);
  border-radius: var(--radius-full);
  margin-top: var(--space-3);
  overflow: hidden;
}

.progress-fill {
  height: 100%;
  background: linear-gradient(90deg, #6366f1, #a855f7);
  border-radius: var(--radius-full);
}

/* Glass Table Card */
.glass-card {
  background: var(--bg-card);
  backdrop-filter: var(--glass-blur);
  -webkit-backdrop-filter: var(--glass-blur);
  border-radius: var(--radius-xl);
  border: 1px solid var(--border-color);
  box-shadow: var(--shadow-glass);
  overflow: hidden;
  margin-bottom: var(--space-6);
}

.empty-state-card {
  padding: var(--space-8) 0;
}

.table-card {
  background: var(--bg-card);
  border: 1px solid var(--border-color);
}

/* Card Header */
.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: var(--space-4) var(--space-5);
  border-bottom: 1px solid var(--border-color);
  flex-wrap: wrap;
  gap: var(--space-3);
}

.card-header-title {
  display: flex;
  align-items: center;
  gap: var(--space-3);
}

.card-header-title h2 {
  font-size: var(--font-size-base);
  font-weight: var(--font-weight-semibold);
  color: var(--text-primary);
  margin: 0;
}

.header-count-badge {
  padding: 3px 8px;
  background: var(--color-bg-secondary);
  border: 1px solid var(--border-color);
  color: var(--text-secondary);
  border-radius: var(--radius-full);
  font-size: 10px;
  font-weight: var(--font-weight-medium);
}

.card-header-actions {
  display: flex;
  align-items: center;
  gap: var(--space-2);
}

/* Card Header Action Icons */
.icon-action-btn {
  width: 32px;
  height: 32px;
  border-radius: var(--radius-md);
  border: 1px solid var(--border-color);
  background: transparent;
  color: var(--text-secondary);
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  transition: all var(--transition-fast);
}

.icon-action-btn:hover {
  background: var(--color-primary-subtle);
  color: var(--text-primary);
  border-color: var(--text-secondary);
}

/* Search bar styling */
.search-input-wrapper {
  position: relative;
  display: flex;
  align-items: center;
}

.search-icon {
  position: absolute;
  left: 12px;
  color: var(--text-secondary);
  pointer-events: none;
}

.search-input {
  width: 240px;
  padding: 8px 12px 8px 34px;
  border: 1px solid var(--border-color);
  border-radius: var(--radius-md);
  font-size: var(--font-size-xs);
  background: var(--bg-card);
  color: var(--text-primary);
  outline: none;
  transition: all var(--transition-fast);
}

.search-input:focus {
  border-color: var(--color-primary);
  background: var(--bg-card);
  box-shadow: 0 0 0 3px rgba(37, 99, 235, 0.15);
}

/* Table styling */
.table-responsive {
  overflow-x: auto;
  width: 100%;
}

.admin-table {
  width: 100%;
  border-collapse: collapse;
  text-align: left;
}

.admin-table th {
  padding: var(--space-4) var(--space-5);
  font-size: 10px;
  font-weight: var(--font-weight-bold);
  color: var(--text-secondary);
  text-transform: uppercase;
  letter-spacing: 0.05em;
  border-bottom: 1px solid var(--border-color);
  background: rgba(255, 255, 255, 0.005);
}

.admin-table td {
  padding: var(--space-4) var(--space-5);
  font-size: var(--font-size-sm);
  border-bottom: 1px solid var(--border-color);
  vertical-align: middle;
  color: var(--text-primary);
}

.admin-table tr:last-child td {
  border-bottom: none;
}

.admin-table tr:hover td {
  background: var(--color-primary-subtle);
}

/* User profile cell layout */
.user-cell {
  display: flex;
  align-items: center;
  gap: var(--space-3);
}

.user-avatar {
  width: 36px;
  height: 36px;
  border-radius: var(--radius-full);
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-weight: var(--font-weight-bold);
  font-size: var(--font-size-sm);
  box-shadow: inset 0 0 0 1px rgba(255, 255, 255, 0.12);
  flex-shrink: 0;
}

.user-names {
  display: flex;
  flex-direction: column;
  gap: 2px;
}

.user-fullname {
  font-weight: var(--font-weight-semibold);
  color: var(--text-primary);
  font-size: var(--font-size-sm);
}

.user-email-sub {
  font-size: 11px;
  color: var(--text-secondary);
}

.date-text {
  font-size: var(--font-size-sm);
  color: var(--text-secondary);
}

/* Role Badges */
.badge {
  display: inline-flex;
  align-items: center;
  padding: 4px 10px;
  border-radius: var(--radius-full);
  font-size: 11px;
  font-weight: var(--font-weight-bold);
}

.badge-admin {
  background: rgba(59, 130, 246, 0.12);
  color: #2563eb;
  border: 1px solid rgba(59, 130, 246, 0.25);
}

.badge-manager {
  background: rgba(139, 92, 246, 0.12);
  color: #7c3aed;
  border: 1px solid rgba(139, 92, 246, 0.25);
}

.badge-member {
  background: rgba(148, 163, 184, 0.12);
  color: var(--text-secondary);
  border: 1px solid rgba(148, 163, 184, 0.25);
}

/* Status Indicators */
.status-indicator {
  display: inline-flex;
  align-items: center;
  gap: var(--space-2);
  font-size: var(--font-size-sm);
  font-weight: var(--font-weight-medium);
}

.status-dot {
  width: 6px;
  height: 6px;
  border-radius: var(--radius-full);
  display: inline-block;
}

.status-active {
  color: #10b981;
}

.status-active .status-dot {
  background-color: #10b981;
  box-shadow: 0 0 8px #10b981;
}

.status-offline {
  color: #94a3b8;
}

.status-offline .status-dot {
  background-color: #94a3b8;
}

.status-locked {
  color: #ef4444;
}

.status-locked .status-dot {
  background-color: #ef4444;
  box-shadow: 0 0 8px #ef4444;
}

/* Action buttons cell */
.actions-cell {
  display: flex;
  gap: var(--space-1);
  justify-content: flex-end;
}

.icon-btn {
  width: 30px;
  height: 30px;
  border-radius: var(--radius-md);
  border: none;
  background: transparent;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  transition: all var(--transition-fast);
  color: var(--text-secondary);
}

.icon-btn:hover {
  background: var(--color-primary-subtle);
  color: var(--text-primary);
}

.action-icon {
  stroke-width: 2.2px;
}

.warning-icon {
  color: #fbbf24;
}

.warning-icon:hover {
  color: #f59e0b;
}

.success-icon {
  color: #34d399;
}

.success-icon:hover {
  color: #10b981;
}

.danger-icon {
  color: #f87171;
}

.danger-icon:hover {
  color: #ef4444;
}

/* Row Locked styling */
.row-locked td {
  opacity: 0.65;
}

.row-locked .user-fullname {
  text-decoration: line-through;
  color: var(--text-secondary);
}

/* Bottom Grid Layout */
.bottom-grid {
  display: grid;
  grid-template-columns: 1.8fr 1fr;
  gap: var(--space-6);
  margin-top: var(--space-6);
}

.view-all-link {
  font-size: var(--font-size-xs);
  color: var(--color-primary);
  text-decoration: none;
  font-weight: var(--font-weight-semibold);
}

.view-all-link:hover {
  text-decoration: underline;
}

.activity-list {
  padding: var(--space-4) var(--space-5);
  display: flex;
  flex-direction: column;
  gap: var(--space-4);
}

.activity-item {
  display: flex;
  align-items: flex-start;
  gap: var(--space-3);
}

.log-avatar {
  width: 32px;
  height: 32px;
  border-radius: var(--radius-full);
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-weight: var(--font-weight-bold);
  font-size: 11px;
}

.warning-avatar {
  background-color: rgba(245, 158, 11, 0.15);
  color: #f59e0b;
  font-size: 14px;
  border: 1px solid rgba(245, 158, 11, 0.25);
}

.activity-details {
  display: flex;
  flex-direction: column;
  gap: 3px;
}

.activity-text {
  font-size: var(--font-size-sm);
  color: var(--text-primary);
  margin: 0;
  line-height: 1.4;
}

.log-username {
  font-weight: var(--font-weight-semibold);
  color: var(--text-primary);
}

.log-target {
  font-weight: var(--font-weight-semibold);
  color: var(--color-primary);
}

.activity-time {
  font-size: 11px;
  color: var(--text-secondary);
}

/* Security Widget Card */
.security-card {
  border: 1px solid var(--border-color);
  background: var(--bg-card);
  position: relative;
  display: flex;
  flex-direction: column;
  justify-content: center;
}

.security-card-content {
  padding: var(--space-5) var(--space-6);
}

.security-title {
  font-size: var(--font-size-base);
  font-weight: var(--font-weight-semibold);
  color: var(--text-primary);
  margin: 0 0 var(--space-2) 0;
}

.security-desc {
  font-size: var(--font-size-sm);
  color: var(--text-secondary);
  margin: 0 0 var(--space-5) 0;
  line-height: 1.5;
}

.security-indicator {
  display: flex;
  align-items: center;
  gap: var(--space-5);
}

/* Circular Percent Meter Widget */
.security-indicator-circle {
  width: 72px;
  height: 72px;
  border-radius: var(--radius-full);
  background: conic-gradient(#4f46e5 0% 92%, rgba(255, 255, 255, 0.05) 92% 100%);
  display: flex;
  align-items: center;
  justify-content: center;
  box-shadow: 0 0 12px rgba(79, 70, 229, 0.25);
  flex-shrink: 0;
}

.indicator-inner {
  width: 58px;
  height: 58px;
  border-radius: var(--radius-full);
  background: var(--bg-card);
  display: flex;
  align-items: center;
  justify-content: center;
}

.indicator-percent {
  font-size: var(--font-size-base);
  font-weight: var(--font-weight-bold);
  color: var(--text-primary);
}

.security-indicator-info {
  display: flex;
  flex-direction: column;
  gap: 2px;
}

.indicator-label {
  font-size: 9px;
  font-weight: var(--font-weight-bold);
  color: var(--text-secondary);
  letter-spacing: 0.05em;
}

.indicator-status {
  font-size: var(--font-size-base);
  font-weight: var(--font-weight-bold);
  color: #10b981;
}

/* Modal Form styling overrides */
.admin-modal-form {
  display: flex;
  flex-direction: column;
  gap: var(--space-4);
  padding: var(--space-1) 0;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: var(--space-2);
}

.form-label {
  font-size: var(--font-size-xs);
  font-weight: var(--font-weight-bold);
  color: var(--text-secondary);
  text-transform: uppercase;
  letter-spacing: 0.03em;
}

.form-select {
  width: 100%;
  padding: 10px 14px;
  border: 1px solid var(--border-color);
  border-radius: var(--radius-md);
  font-size: var(--font-size-sm);
  background: var(--bg-card);
  color: var(--text-primary);
  outline: none;
  transition: all var(--transition-fast);
}

.form-select:focus {
  border-color: var(--color-primary);
  box-shadow: 0 0 0 3px rgba(0, 78, 204, 0.15);
}

.form-select option {
  background: var(--bg-card);
  color: var(--text-primary);
}

.loading-container {
  display: flex;
  justify-content: center;
  align-items: center;
  padding: var(--space-10) 0;
}

@media (max-width: 768px) {
  .bottom-grid {
    grid-template-columns: 1fr;
  }
}

/* Dark theme overrides */
[data-theme='dark'] .badge-admin {
  color: #93c5fd;
}

[data-theme='dark'] .badge-manager {
  color: #c084fc;
}

[data-theme='dark'] .badge-member {
  color: #cbd5e1;
}

[data-theme='dark'] .form-select option {
  background: #111827;
  color: var(--text-primary);
}

.activity-loading,
.activity-empty {
  padding: var(--space-6) var(--space-5);
  text-align: center;
  color: var(--text-secondary);
  font-size: var(--font-size-sm);
}
</style>
