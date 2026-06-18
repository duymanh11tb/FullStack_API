<template>
  <div class="admin-view">
    <header class="page-header">
      <div class="header-content">
        <h1 class="page-title">Quản trị viên</h1>
        <p class="page-subtitle">Quản lý tài khoản hệ thống và phân quyền</p>
      </div>
      <div class="search-bar">
        <BaseInput
          v-model="filterText"
          placeholder="Tìm kiếm tài khoản..."
          id="input-admin-search"
        />
      </div>
    </header>

    <div class="admin-content">
      <div v-if="loading" class="loading-container">
        <LoadingSpinner />
      </div>

      <div v-else-if="filteredUsers.length === 0" class="glass-card">
        <EmptyState title="Không tìm thấy tài khoản" description="Thử tìm kiếm với từ khóa khác." />
      </div>

      <div v-else class="glass-card">
        <div class="table-responsive">
          <table class="admin-table">
            <thead>
              <tr>
                <th>Thành viên</th>
                <th>Tên đăng nhập</th>
                <th>Email</th>
                <th>Vai trò</th>
                <th>Trạng thái</th>
                <th>Ngày tạo</th>
                <th>Thao tác</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="user in filteredUsers" :key="user.id">
                <td>
                  <div class="user-cell">
                    <div class="user-avatar" :style="{ backgroundColor: getAvatarColor(user.fullName || user.username) }">
                      {{ (user.fullName || user.username).charAt(0).toUpperCase() }}
                    </div>
                    <div class="user-names">
                      <span class="user-fullname">{{ user.fullName }}</span>
                      <span class="user-id-sub">{{ user.id }}</span>
                    </div>
                  </div>
                </td>
                <td>@{{ user.username }}</td>
                <td>{{ user.email }}</td>
                <td>
                  <span class="badge" :class="user.role === 'Admin' || user.role === 1 ? 'badge-admin' : 'badge-user'">
                    {{ user.role === 'Admin' || user.role === 1 ? 'Admin' : 'User' }}
                  </span>
                </td>
                <td>
                  <span class="badge" :class="user.isActive ? 'badge-active' : 'badge-locked'">
                    {{ user.isActive ? 'Hoạt động' : 'Bị khóa' }}
                  </span>
                </td>
                <td>{{ formatDate(user.createdAt) }}</td>
                <td>
                  <div class="actions-cell">
                    <BaseButton variant="secondary" class="btn-action" @click="openEdit(user)">
                      Sửa
                    </BaseButton>
                    <BaseButton variant="danger" class="btn-action" @click="handleDeleteUser(user)">
                      Xóa
                    </BaseButton>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>

    <!-- Edit User Modal -->
    <BaseModal v-model="showEditModal" title="Cập nhật tài khoản">
      <form @submit.prevent="handleUpdateUser">
        <BaseInput v-model="editForm.fullName" label="Tên đầy đủ" required id="input-edit-fullname" />
        <BaseInput v-model="editForm.username" label="Tên đăng nhập" required id="input-edit-username" />
        <BaseInput v-model="editForm.email" label="Email" type="email" required id="input-edit-email" />
        
        <div class="form-group">
          <label class="form-label">Vai trò</label>
          <select v-model="editForm.role" class="form-select">
            <option :value="2">User (Thành viên)</option>
            <option :value="1">Admin (Quản trị viên)</option>
          </select>
        </div>

        <div class="form-group">
          <label class="form-label">Trạng thái hoạt động</label>
          <select v-model="editForm.isActive" class="form-select">
            <option :value="true">Hoạt động (Active)</option>
            <option :value="false">Khóa tài khoản (Locked)</option>
          </select>
        </div>

        <BaseInput 
          v-model="editForm.password" 
          label="Đổi mật khẩu (Bỏ trống nếu không đổi)" 
          type="password" 
          placeholder="Nhập mật khẩu mới tại đây..."
          id="input-edit-password" 
        />
      </form>
      <template #footer>
        <BaseButton variant="secondary" @click="showEditModal = false">Hủy</BaseButton>
        <BaseButton variant="primary" @click="handleUpdateUser">Lưu</BaseButton>
      </template>
    </BaseModal>
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted } from 'vue'
import { getAllUsers, updateUser, deleteUser } from '../api/notifyApi'
import BaseButton from '../components/common/BaseButton.vue'
import BaseInput from '../components/common/BaseInput.vue'
import BaseModal from '../components/common/BaseModal.vue'
import LoadingSpinner from '../components/common/LoadingSpinner.vue'
import EmptyState from '../components/common/EmptyState.vue'

const users = ref([])
const loading = ref(true)
const filterText = ref('')
const showEditModal = ref(false)
const selectedUser = ref(null)

const editForm = reactive({
  username: '',
  email: '',
  fullName: '',
  role: 2,
  isActive: true,
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
  return new Date(dateStr).toLocaleDateString('vi-VN', { day: '2-digit', month: '2-digit', year: 'numeric' })
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

const filteredUsers = computed(() => {
  if (!filterText.value) return users.value
  const query = filterText.value.toLowerCase()
  return users.value.filter(u => 
    (u.fullName && u.fullName.toLowerCase().includes(query)) ||
    (u.username && u.username.toLowerCase().includes(query)) ||
    (u.email && u.email.toLowerCase().includes(query))
  )
})

function openEdit(user) {
  selectedUser.value = user
  editForm.username = user.username
  editForm.email = user.email
  editForm.fullName = user.fullName
  editForm.role = user.role === 'Admin' || user.role === 1 ? 1 : 2
  editForm.isActive = user.isActive
  editForm.password = ''
  showEditModal.value = true
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

onMounted(() => {
  loadUsers()
})
</script>

<style scoped>
.admin-view {
  padding: var(--space-6);
  height: 100%;
  overflow-y: auto;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: var(--space-6);
  gap: var(--space-4);
  flex-wrap: wrap;
}

.page-title {
  font-size: var(--font-size-2xl);
  font-weight: var(--font-weight-bold);
  color: var(--text-primary);
  margin-bottom: var(--space-2);
}

.page-subtitle {
  color: var(--text-secondary);
  font-size: var(--font-size-sm);
}

.search-bar {
  max-width: 320px;
  width: 100%;
}

.glass-card {
  background: var(--bg-card);
  backdrop-filter: var(--glass-blur);
  -webkit-backdrop-filter: var(--glass-blur);
  border-radius: var(--radius-xl);
  border: 1px solid var(--border-color);
  box-shadow: var(--shadow-glass);
  overflow: hidden;
}

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
  font-size: var(--font-size-xs);
  font-weight: var(--font-weight-semibold);
  color: var(--color-text-tertiary);
  text-transform: uppercase;
  border-bottom: 1px solid var(--border-color);
  background: rgba(255, 255, 255, 0.01);
}

.admin-table td {
  padding: var(--space-4) var(--space-5);
  font-size: var(--font-size-sm);
  border-bottom: 1px solid var(--border-color);
  vertical-align: middle;
  color: var(--text-primary);
}

.admin-table tr:hover td {
  background: var(--sidebar-hover);
}

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
}

.user-names {
  display: flex;
  flex-direction: column;
}

.user-fullname {
  font-weight: var(--font-weight-medium);
  color: var(--text-primary);
}

.user-id-sub {
  font-size: 10px;
  color: var(--color-text-tertiary);
  margin-top: 2px;
}

.badge {
  display: inline-flex;
  align-items: center;
  padding: 4px 10px;
  border-radius: var(--radius-full);
  font-size: var(--font-size-xs);
  font-weight: var(--font-weight-medium);
}

.badge-admin {
  background: rgba(99, 102, 241, 0.15);
  color: #818cf8;
  border: 1px solid rgba(99, 102, 241, 0.3);
}

.badge-user {
  background: rgba(148, 163, 184, 0.15);
  color: var(--color-text-secondary);
  border: 1px solid rgba(148, 163, 184, 0.3);
}

.badge-active {
  background: rgba(16, 185, 129, 0.15);
  color: #34d399;
  border: 1px solid rgba(16, 185, 129, 0.3);
}

.badge-locked {
  background: rgba(244, 63, 94, 0.15);
  color: #fb7185;
  border: 1px solid rgba(244, 63, 94, 0.3);
}

.actions-cell {
  display: flex;
  gap: var(--space-2);
}

.btn-action {
  padding: 6px 12px;
  font-size: var(--font-size-xs);
  display: inline-flex;
  align-items: center;
  gap: 4px;
}

.loading-container {
  display: flex;
  justify-content: center;
  align-items: center;
  padding: var(--space-8) 0;
}

/* Modals styles */
.form-group {
  margin-bottom: var(--space-4);
}

.form-label {
  display: block;
  font-size: var(--font-size-sm);
  font-weight: var(--font-weight-medium);
  color: var(--text-primary);
  margin-bottom: var(--space-1);
}

.form-select {
  width: 100%;
  padding: 10px 14px;
  border: 1px solid var(--border-color);
  border-radius: var(--radius-md);
  font-size: var(--font-size-base);
  background: var(--bg-card);
  color: var(--text-primary);
  outline: none;
  transition: all var(--transition-fast);
}

.form-select:focus {
  border-color: var(--color-primary);
  box-shadow: 0 0 0 3px var(--color-primary-light);
}

.form-select option {
  background: var(--bg-primary);
  color: var(--text-primary);
}
</style>
