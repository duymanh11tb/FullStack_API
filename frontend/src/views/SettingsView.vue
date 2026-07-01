<template>
  <div class="settings-page">
    <div class="page-header">
      <h1>Cài đặt tài khoản</h1>
      <p class="subtitle">Quản lý thông tin cá nhân và thiết lập tài khoản của bạn</p>
    </div>

    <LoadingSpinner v-if="loading" text="Đang tải thông tin cài đặt..." />

    <div v-else class="settings-container glass-panel">
      <!-- Left Tab Navigation -->
      <aside class="settings-sidebar">
        <button 
          v-for="tab in tabs" 
          :key="tab.id"
          :class="['tab-item', { active: activeTab === tab.id }]"
          @click="activeTab = tab.id"
        >
          <svg v-if="tab.id === 'general'" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2"/><circle cx="12" cy="7" r="4"/>
          </svg>
          <svg v-else-if="tab.id === 'security'" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <rect x="3" y="11" width="18" height="11" rx="2" ry="2"/><path d="M7 11V7a5 5 0 0 1 10 0v4"/>
          </svg>
          <svg v-else-if="tab.id === 'notifications'" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M18 8A6 6 0 0 0 6 8c0 7-3 9-3 9h18s-3-2-3-9"/><path d="M13.73 21a2 2 0 0 1-3.46 0"/>
          </svg>
          <svg v-else-if="tab.id === 'integrations'" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M21 16V8a2 2 0 0 0-1-1.73l-7-4a2 2 0 0 0-2 0l-7 4A2 2 0 0 0 3 8v8a2 2 0 0 0 1 1.73l7 4a2 2 0 0 0 2 0l7-4A2 2 0 0 0 21 16z" />
          </svg>
          <span>{{ tab.name }}</span>
        </button>
      </aside>

      <!-- Main Content Panel -->
      <main class="settings-content">
        <!-- 1. GENERAL TAB (THÔNG TIN CÁ NHÂN) -->
        <section v-if="activeTab === 'general'" class="tab-pane">
          <div class="profile-section">
            <div class="avatar-card">
              <div class="avatar-wrapper">
                <img 
                  v-if="profileForm.avatarUrl" 
                  :src="profileForm.avatarUrl" 
                  alt="Avatar" 
                  class="avatar-img"
                />
                <div v-else class="avatar-placeholder">
                  {{ profileForm.fullName?.charAt(0)?.toUpperCase() || 'U' }}
                </div>
                <button class="avatar-edit-overlay" @click="toggleAvatarInput" title="Sửa ảnh">
                  <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
                    <path d="M11 4H4a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2v-7" />
                    <path d="M18.5 2.5a2.121 2.121 0 1 1 3 3L12 15l-4 1 1-4z" />
                  </svg>
                </button>
              </div>

              <div class="avatar-info">
                <h3>Ảnh đại diện</h3>
                <p class="text-muted">Nhấp vào biểu tượng sửa hoặc dán liên kết để cập nhật. Hỗ trợ định dạng JPG, PNG hoặc GIF.</p>
                <div class="avatar-actions">
                  <BaseButton variant="primary" size="sm" @click="toggleAvatarInput">
                    {{ showAvatarInput ? 'Đóng nhập URL' : 'Thay đổi ảnh đại diện' }}
                  </BaseButton>
                  <BaseButton variant="secondary" size="sm" @click="removeAvatar" class="btn-remove">
                    Gỡ bỏ
                  </BaseButton>
                </div>
                
                <transition name="slide-fade">
                  <div v-if="showAvatarInput" class="avatar-url-input-box">
                    <input 
                      type="url" 
                      v-model="profileForm.avatarUrl" 
                      placeholder="Nhập đường dẫn URL ảnh đại diện..." 
                      class="custom-input"
                    />
                  </div>
                </transition>
              </div>
            </div>

            <!-- Form -->
            <form @submit.prevent="saveProfile" class="settings-form">
              <div class="form-grid">
                <div class="custom-form-group">
                  <label class="custom-label">HỌ VÀ TÊN</label>
                  <input type="text" v-model="profileForm.fullName" required class="custom-input" />
                </div>

                <div class="custom-form-group">
                  <label class="custom-label">CHỨC DANH</label>
                  <input type="text" v-model="profileForm.jobTitle" placeholder="e.g. Project Manager, Lead Dev" class="custom-input" />
                </div>

                <div class="custom-form-group">
                  <label class="custom-label">EMAIL CÔNG VIỆC</label>
                  <div class="input-lock-wrapper">
                    <input type="email" :value="authStore.user?.email" disabled class="custom-input disabled" />
                    <svg class="lock-icon" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                      <rect x="3" y="11" width="18" height="11" rx="2" ry="2" />
                      <path d="M7 11V7a5 5 0 0 1 10 0v4" />
                    </svg>
                  </div>
                  <span class="input-hint">Liên hệ quản trị viên để thay đổi email.</span>
                </div>

                <div class="custom-form-group">
                  <label class="custom-label">PHÒNG BAN</label>
                  <input type="text" v-model="profileForm.department" placeholder="e.g. Phát triển phần mềm" class="custom-input" />
                </div>
              </div>

              <div class="custom-form-group full-width">
                <label class="custom-label">GIỚI THIỆU BẢN THÂN</label>
                <textarea v-model="profileForm.bio" placeholder="Chia sẻ đôi chút về kinh nghiệm và vai trò của bạn..." class="custom-textarea" rows="4"></textarea>
              </div>

              <div class="settings-footer">
                <button type="button" class="btn-cancel" @click="resetProfileForm">Hủy</button>
                <BaseButton type="submit" variant="primary" :loading="savingProfile">Lưu thay đổi</BaseButton>
              </div>
            </form>
          </div>

          <!-- Summary Cards at Bottom -->
          <div class="summary-cards">
            <div class="summary-card glass-card">
              <div class="card-icon-box success">
                <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                  <path d="M12 22s8-4 8-10V5l-8-3-8 3v7c0 6 8 10 8 10z" />
                </svg>
              </div>
              <div class="card-text">
                <h4>Trạng thái xác minh</h4>
                <p>Tài khoản của bạn đã được xác minh qua hệ thống bảo mật của công ty.</p>
                <span class="status-badge success">Đã xác minh</span>
              </div>
            </div>

            <div class="summary-card glass-card">
              <div class="card-icon-box info">
                <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                  <circle cx="12" cy="12" r="10"/><polyline points="12 6 12 12 16 14"/>
                </svg>
              </div>
              <div class="card-text">
                <h4>Hoạt động gần đây</h4>
                <p>Thiết bị đăng nhập gần nhất:</p>
                <div class="activity-detail">
                  <span><strong>Lần cuối:</strong> Vừa xong</span>
                  <span><strong>Thiết bị:</strong> Web Browser (Chrome)</span>
                </div>
              </div>
            </div>
          </div>
        </section>

        <!-- 2. SECURITY TAB (BẢO MẬT) -->
        <section v-if="activeTab === 'security'" class="tab-pane">
          <div class="pane-header">
            <h3>Đổi mật khẩu tài khoản</h3>
            <p class="text-muted">Cập nhật mật khẩu mới của bạn để bảo vệ tài khoản</p>
          </div>

          <form @submit.prevent="savePassword" class="settings-form security-form">
            <div class="custom-form-group">
              <label class="custom-label">MẬT KHẨU HIỆN TẠI</label>
              <input type="password" v-model="passwordForm.oldPassword" required class="custom-input" placeholder="••••••••" />
            </div>

            <div class="custom-form-group">
              <label class="custom-label">MẬT KHẨU MỚI</label>
              <input type="password" v-model="passwordForm.newPassword" required class="custom-input" placeholder="••••••••" />
            </div>

            <div class="custom-form-group">
              <label class="custom-label">XÁC NHẬN MẬT KHẨU MỚI</label>
              <input type="password" v-model="passwordForm.confirmPassword" required class="custom-input" placeholder="••••••••" />
            </div>

            <div class="settings-footer">
              <BaseButton type="submit" variant="primary" :loading="savingPassword">Cập nhật mật khẩu</BaseButton>
            </div>
          </form>
        </section>

        <!-- 3. NOTIFICATIONS TAB (THÔNG BÁO) -->
        <section v-if="activeTab === 'notifications'" class="tab-pane">
          <div class="pane-header">
            <h3>Thiết lập thông báo</h3>
            <p class="text-muted">Tùy chỉnh các hoạt động bạn muốn nhận thông báo thời gian thực</p>
          </div>

          <div class="notification-settings-list">
            <div class="setting-group-box">
              <h4 class="setting-group-title">Hoạt động dự án</h4>
              <div class="setting-item">
                <div class="setting-info">
                  <span class="setting-label">Bình luận</span>
                  <span class="setting-desc">Nhận thông báo khi có bình luận mới trên task của bạn</span>
                </div>
                <label class="toggle">
                  <input type="checkbox" v-model="settings.onComment" />
                  <span class="toggle-slider"></span>
                </label>
              </div>

              <div class="setting-item">
                <div class="setting-info">
                  <span class="setting-label">Được giao việc</span>
                  <span class="setting-desc">Nhận thông báo khi bạn được giao task mới</span>
                </div>
                <label class="toggle">
                  <input type="checkbox" v-model="settings.onAssigned" />
                  <span class="toggle-slider"></span>
                </label>
              </div>

              <div class="setting-item">
                <div class="setting-info">
                  <span class="setting-label">Thay đổi trạng thái</span>
                  <span class="setting-desc">Nhận thông báo khi trạng thái task thay đổi</span>
                </div>
                <label class="toggle">
                  <input type="checkbox" v-model="settings.onStatusChanged" />
                  <span class="toggle-slider"></span>
                </label>
              </div>

              <div class="setting-item">
                <div class="setting-info">
                  <span class="setting-label">Nhắc đến (@mention)</span>
                  <span class="setting-desc">Nhận thông báo khi ai đó nhắc đến bạn</span>
                </div>
                <label class="toggle">
                  <input type="checkbox" v-model="settings.onMention" />
                  <span class="toggle-slider"></span>
                </label>
              </div>
            </div>

            <div class="setting-group-box">
              <h4 class="setting-group-title">Sprint & Nhóm</h4>
              <div class="setting-item">
                <div class="setting-info">
                  <span class="setting-label">Sprint bắt đầu</span>
                  <span class="setting-desc">Nhận thông báo khi sprint được bắt đầu</span>
                </div>
                <label class="toggle">
                  <input type="checkbox" v-model="settings.onSprintStarted" />
                  <span class="toggle-slider"></span>
                </label>
              </div>

              <div class="setting-item">
                <div class="setting-info">
                  <span class="setting-label">Thêm thành viên</span>
                  <span class="setting-desc">Nhận thông báo khi có thành viên mới được thêm vào dự án</span>
                </div>
                <label class="toggle">
                  <input type="checkbox" v-model="settings.onMemberAdded" />
                  <span class="toggle-slider"></span>
                </label>
              </div>
            </div>
          </div>

          <div class="settings-footer">
            <BaseButton variant="primary" @click="saveSettings" :loading="savingSettings">Lưu cài đặt thông báo</BaseButton>
          </div>
        </section>

        <!-- 4. INTEGRATIONS TAB (TÍCH HỢP) -->
        <section v-if="activeTab === 'integrations'" class="tab-pane">
          <div class="pane-header">
            <h3>Tích hợp ứng dụng</h3>
            <p class="text-muted">Liên kết tài khoản của bạn với các dịch vụ bên ngoài</p>
          </div>

          <div class="integrations-list">
            <div class="integration-card glass-card">
              <div class="integration-header">
                <img src="https://img.icons8.com/color/32/000000/google-logo.png" alt="Google Logo" />
                <div class="integration-info">
                  <h4>Google Account</h4>
                  <p class="text-muted">Đăng nhập nhanh và đồng bộ hóa với lịch Google</p>
                </div>
                <span class="status-badge success">Đã liên kết</span>
              </div>
            </div>

            <div class="integration-card glass-card">
              <div class="integration-header">
                <svg width="32" height="32" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" style="color: #24292e;">
                  <path d="M9 19c-5 1.5-5-2.5-7-3m14 6v-3.87a3.37 3.37 0 0 0-.94-2.61c3.14-.35 6.44-1.54 6.44-7A5.44 5.44 0 0 0 20 4.77 5.07 5.07 0 0 0 19.91 1S18.73.65 16 2.48a13.38 13.38 0 0 0-7 0C6.27.65 5.09 1 5.09 1A5.07 5.07 0 0 0 5 4.77a5.44 5.44 0 0 0-1.5 3.78c0 5.42 3.3 6.61 6.44 7A3.37 3.37 0 0 0 9 18.13V22" />
                </svg>
                <div class="integration-info">
                  <h4>GitHub Account</h4>
                  <p class="text-muted" v-if="githubUsername">Đã liên kết tài khoản GitHub: <strong>{{ githubUsername }}</strong></p>
                  <p class="text-muted" v-else>Tích hợp repository và các commit liên quan tới task</p>
                </div>
                <div class="integration-actions" style="display: flex; gap: 8px; align-items: center;">
                  <span v-if="githubUsername" class="status-badge success" style="margin: 0;">Đã liên kết</span>
                  <button v-if="githubUsername" class="btn-connect" @click="handleUnlinkGithub" :disabled="linking" style="background-color: var(--color-error, #ef4444);">Hủy liên kết</button>
                  <button v-else class="btn-connect" @click="handleLinkGithub" :disabled="linking">Kết nối tài khoản</button>
                </div>
              </div>
            </div>
          </div>
        </section>
      </main>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { getNotificationSettings, updateNotificationSettings, updateProfile, changePassword, linkGithub, unlinkGithub } from '../api/notifyApi'
import { useAuthStore } from '../stores/auth'
import BaseButton from '../components/common/BaseButton.vue'
import LoadingSpinner from '../components/common/LoadingSpinner.vue'

const route = useRoute()
const router = useRouter()
const authStore = useAuthStore()

const loading = ref(true)
const activeTab = ref(route.query.tab || 'general')
const linking = ref(false)
const githubUsername = ref('')

const tabs = [
  { id: 'general', name: 'Thông tin cá nhân' },
  { id: 'security', name: 'Bảo mật' },
  { id: 'notifications', name: 'Thông báo' },
  { id: 'integrations', name: 'Tích hợp' }
]

// Profile Form
const profileForm = reactive({
  fullName: '',
  avatarUrl: '',
  jobTitle: '',
  department: '',
  bio: ''
})

const showAvatarInput = ref(false)
const savingProfile = ref(false)

function toggleAvatarInput() {
  showAvatarInput.value = !showAvatarInput.value
}

function removeAvatar() {
  profileForm.avatarUrl = ''
}

function resetProfileForm() {
  if (authStore.user) {
    profileForm.fullName = authStore.displayName || ''
    profileForm.avatarUrl = authStore.user.avatarUrl || authStore.user.AvatarUrl || ''
    profileForm.jobTitle = authStore.user.jobTitle || authStore.user.JobTitle || ''
    profileForm.department = authStore.user.department || authStore.user.Department || ''
    profileForm.bio = authStore.user.bio || authStore.user.Bio || ''
    githubUsername.value = authStore.user.gitHubUsername || authStore.user.GitHubUsername || ''
  }
}

async function saveProfile() {
  savingProfile.value = true
  try {
    await updateProfile({
      fullName: profileForm.fullName,
      avatarUrl: profileForm.avatarUrl,
      jobTitle: profileForm.jobTitle,
      department: profileForm.department,
      bio: profileForm.bio
    })
    alert('Cập nhật hồ sơ thành công!')
    await authStore.fetchUser()
  } catch (err) {
    alert(err.response?.data?.message || 'Không thể cập nhật hồ sơ')
  } finally {
    savingProfile.value = false
  }
}

// Password Form
const passwordForm = reactive({
  oldPassword: '',
  newPassword: '',
  confirmPassword: ''
})
const savingPassword = ref(false)

async function savePassword() {
  if (passwordForm.newPassword !== passwordForm.confirmPassword) {
    alert('Mật khẩu mới và xác nhận mật khẩu không khớp.')
    return
  }

  savingPassword.value = true
  try {
    await changePassword({
      oldPassword: passwordForm.oldPassword,
      newPassword: passwordForm.newPassword
    })
    alert('Đổi mật khẩu thành công!')
    passwordForm.oldPassword = ''
    passwordForm.newPassword = ''
    passwordForm.confirmPassword = ''
  } catch (err) {
    alert(err.response?.data?.message || 'Không thể đổi mật khẩu')
  } finally {
    savingPassword.value = false
  }
}

// Notification Settings
const settings = reactive({
  onComment: true,
  onAssigned: true,
  onStatusChanged: true,
  onMention: true,
  onSprintStarted: true,
  onMemberAdded: true
})
const savingSettings = ref(false)

async function loadSettings() {
  loading.value = true
  try {
    const res = await getNotificationSettings()
    const data = res.data.data || res.data
    if (data) {
      settings.onComment = data.onComment ?? true
      settings.onAssigned = data.onAssigned ?? true
      settings.onStatusChanged = data.onStatusChanged ?? true
      settings.onMention = data.onMention ?? true
      settings.onSprintStarted = data.onSprintStarted ?? true
      settings.onMemberAdded = data.onMemberAdded ?? true
    }
  } catch {
    // Keep defaults
  } finally {
    loading.value = false
  }
}

async function saveSettings() {
  savingSettings.value = true
  try {
    await updateNotificationSettings({ ...settings })
    alert('Lưu cài đặt thông báo thành công!')
  } catch (err) {
    alert(err.response?.data?.message || 'Không thể lưu cài đặt')
  } finally {
    savingSettings.value = false
  }
}
const GITHUB_CLIENT_ID = import.meta.env.VITE_GITHUB_CLIENT_ID || 'Ov23lirIaP5cgYJ7mvKt'

function handleLinkGithub() {
  const callbackUrl = window.location.origin + '/login'
  const authUrl = `https://github.com/login/oauth/authorize?client_id=${GITHUB_CLIENT_ID}&redirect_uri=${encodeURIComponent(callbackUrl)}&scope=user:email&state=github-link`
  window.location.href = authUrl
}

async function handleUnlinkGithub() {
  if (!confirm('Bạn có chắc chắn muốn hủy liên kết tài khoản GitHub không?')) {
    return
  }
  linking.value = true
  try {
    await unlinkGithub()
    alert('Hủy liên kết tài khoản GitHub thành công!')
    await authStore.fetchUser()
    githubUsername.value = ''
  } catch (err) {
    alert(err.response?.data?.message || 'Hủy liên kết GitHub thất bại.')
  } finally {
    linking.value = false
  }
}

onMounted(async () => {
  await authStore.fetchUser()
  resetProfileForm()
  await loadSettings()
  githubUsername.value = authStore.user?.gitHubUsername || authStore.user?.GitHubUsername || ''
})
</script>

<style scoped>
.settings-page {
  animation: fadeIn 0.35s cubic-bezier(0.16, 1, 0.3, 1);
  width: 100%;
}

@keyframes fadeIn {
  from { opacity: 0; transform: translateY(12px); }
  to { opacity: 1; transform: translateY(0); }
}

.page-header {
  margin-bottom: var(--space-6);
}

.page-header h1 {
  font-size: var(--font-size-2xl);
  font-weight: 700;
  color: var(--color-text-primary);
  margin-bottom: var(--space-1);
}

.subtitle {
  color: var(--color-text-secondary);
  font-size: var(--font-size-sm);
}

.settings-container {
  display: flex;
  gap: var(--space-6);
  min-height: 600px;
  background: var(--bg-white-to-card);
  border: 1px solid var(--border-color);
  border-radius: var(--radius-xl);
  overflow: hidden;
  padding: 0;
}

@media (max-width: 992px) {
  .settings-container {
    flex-direction: column;
  }
}

/* Sidebar Tab navigation */
.settings-sidebar {
  width: 250px;
  background: var(--color-bg-secondary);
  border-right: 1px solid var(--border-color);
  padding: var(--space-6) var(--space-3);
  display: flex;
  flex-direction: column;
  gap: var(--space-2);
}

@media (max-width: 992px) {
  .settings-sidebar {
    width: 100%;
    border-right: none;
    border-bottom: 1px solid var(--border-color);
    flex-direction: row;
    overflow-x: auto;
    padding: var(--space-3);
  }
}

.tab-item {
  display: flex;
  align-items: center;
  gap: var(--space-3);
  padding: var(--space-3) var(--space-4);
  border-radius: var(--radius-lg);
  color: var(--color-text-secondary);
  background: transparent;
  border: none;
  font-size: var(--font-size-sm);
  font-weight: var(--font-weight-semibold);
  text-align: left;
  cursor: pointer;
  transition: all var(--transition-fast);
}

.tab-item:hover {
  background: var(--color-bg-primary);
  color: var(--color-text-primary);
  transform: translateX(2px);
}

.tab-item.active {
  background: var(--gradient-primary);
  color: white;
  box-shadow: 0 4px 12px rgba(37, 99, 235, 0.25);
}

.tab-icon {
  flex-shrink: 0;
}

/* Content Pane */
.settings-content {
  flex: 1;
  padding: var(--space-8);
  overflow-y: auto;
}

.tab-pane {
  animation: fadeIn 0.3s ease;
}

.pane-header {
  margin-bottom: var(--space-6);
}

.pane-header h3 {
  font-size: var(--font-size-lg);
  font-weight: 700;
  color: var(--color-text-primary);
  margin-bottom: 4px;
}

/* Avatar details */
.avatar-card {
  display: flex;
  gap: var(--space-6);
  align-items: center;
  margin-bottom: var(--space-8);
  padding-bottom: var(--space-6);
  border-bottom: 1px solid var(--border-color);
}

@media (max-width: 576px) {
  .avatar-card {
    flex-direction: column;
    text-align: center;
  }
}

.avatar-wrapper {
  position: relative;
  width: 96px;
  height: 96px;
}

.avatar-img {
  width: 100%;
  height: 100%;
  border-radius: var(--radius-full);
  object-fit: cover;
  border: 3px solid white;
  box-shadow: var(--shadow-md);
}

.avatar-placeholder {
  width: 100%;
  height: 100%;
  border-radius: var(--radius-full);
  background: var(--color-primary);
  color: white;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: var(--font-size-3xl);
  font-weight: 700;
  box-shadow: var(--shadow-md);
}

.avatar-edit-overlay {
  position: absolute;
  bottom: 0;
  right: 0;
  width: 28px;
  height: 28px;
  border-radius: var(--radius-full);
  background: var(--color-primary);
  color: white;
  display: flex;
  align-items: center;
  justify-content: center;
  border: 2px solid white;
  cursor: pointer;
  box-shadow: var(--shadow-sm);
  transition: transform 0.2s ease;
}

.avatar-edit-overlay:hover {
  transform: scale(1.1);
}

.avatar-info h3 {
  font-size: var(--font-size-base);
  font-weight: var(--font-weight-semibold);
  margin-bottom: 4px;
}

.avatar-info p {
  font-size: var(--font-size-xs);
  margin-bottom: var(--space-4);
  max-width: 400px;
}

.avatar-actions {
  display: flex;
  gap: var(--space-3);
}

.avatar-url-input-box {
  margin-top: var(--space-3);
  width: 100%;
  max-width: 450px;
}

/* Forms */
.settings-form {
  display: flex;
  flex-direction: column;
  gap: var(--space-5);
}

.form-grid {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: var(--space-5);
}

@media (max-width: 768px) {
  .form-grid {
    grid-template-columns: 1fr;
  }
}

.custom-form-group {
  display: flex;
  flex-direction: column;
  gap: var(--space-2);
}

.custom-form-group.full-width {
  grid-column: span 2;
}

@media (max-width: 768px) {
  .custom-form-group.full-width {
    grid-column: span 1;
  }
}

.custom-label {
  font-size: 11px;
  font-weight: 700;
  letter-spacing: 0.5px;
  color: var(--color-text-secondary);
}

.custom-input {
  width: 100%;
  padding: 10px var(--space-4);
  border: 1px solid var(--border-color);
  border-radius: var(--radius-lg);
  background: var(--color-bg-primary);
  color: var(--color-text-primary);
  font-size: var(--font-size-sm);
  transition: all var(--transition-fast);
}

.custom-input:focus {
  border-color: var(--color-primary);
  box-shadow: 0 0 0 3px rgba(37, 99, 235, 0.15);
}

.custom-input.disabled {
  background: var(--color-bg-secondary);
  color: var(--color-text-tertiary);
  cursor: not-allowed;
}

.custom-textarea {
  width: 100%;
  padding: var(--space-3) var(--space-4);
  border: 1px solid var(--border-color);
  border-radius: var(--radius-lg);
  background: var(--color-bg-primary);
  color: var(--color-text-primary);
  font-size: var(--font-size-sm);
  resize: vertical;
  transition: all var(--transition-fast);
}

.custom-textarea:focus {
  border-color: var(--color-primary);
  box-shadow: 0 0 0 3px rgba(37, 99, 235, 0.15);
}

.input-lock-wrapper {
  position: relative;
  display: flex;
  align-items: center;
}

.lock-icon {
  position: absolute;
  right: var(--space-4);
  color: var(--color-text-tertiary);
}

.input-hint {
  font-size: var(--font-size-xs);
  color: var(--color-text-tertiary);
}

.settings-footer {
  display: flex;
  justify-content: flex-end;
  gap: var(--space-4);
  margin-top: var(--space-6);
  padding-top: var(--space-5);
  border-top: 1px solid var(--border-color);
}

.btn-cancel {
  background: transparent;
  border: 1px solid var(--border-color);
  color: var(--color-text-secondary);
  padding: 10px var(--space-6);
  font-weight: var(--font-weight-semibold);
  border-radius: var(--radius-lg);
  cursor: pointer;
  transition: all var(--transition-fast);
}

.btn-cancel:hover {
  background: var(--color-bg-secondary);
  color: var(--color-text-primary);
}

/* Summary cards */
.summary-cards {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: var(--space-5);
  margin-top: var(--space-8);
}

@media (max-width: 768px) {
  .summary-cards {
    grid-template-columns: 1fr;
  }
}

.summary-card {
  display: flex;
  gap: var(--space-4);
  padding: var(--space-5);
  border-radius: var(--radius-xl);
  border: 1px solid var(--border-color);
}

.card-icon-box {
  width: 42px;
  height: 42px;
  border-radius: var(--radius-lg);
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

.card-icon-box.success {
  background: rgba(22, 163, 74, 0.1);
  color: var(--color-success);
}

.card-icon-box.info {
  background: rgba(59, 130, 246, 0.1);
  color: var(--color-primary);
}

.card-text h4 {
  font-size: var(--font-size-sm);
  font-weight: 700;
  margin-bottom: var(--space-1);
}

.card-text p {
  font-size: var(--font-size-xs);
  color: var(--color-text-secondary);
  margin-bottom: var(--space-2);
}

.status-badge {
  display: inline-block;
  font-size: 10px;
  font-weight: 700;
  padding: 2px 8px;
  border-radius: var(--radius-full);
}

.status-badge.success {
  background: rgba(22, 163, 74, 0.15);
  color: var(--color-success);
}

.activity-detail {
  display: flex;
  flex-direction: column;
  gap: 2px;
  font-size: var(--font-size-xs);
  color: var(--color-text-tertiary);
}

/* Security pane */
.security-form {
  max-width: 450px;
}

/* Notifications list */
.setting-group-box {
  background: var(--color-bg-secondary);
  border: 1px solid var(--border-color);
  border-radius: var(--radius-xl);
  padding: var(--space-5);
  margin-bottom: var(--space-6);
}

.setting-group-title {
  font-size: 11px;
  font-weight: 700;
  color: var(--color-text-secondary);
  text-transform: uppercase;
  letter-spacing: 0.5px;
  margin-bottom: var(--space-4);
}

.setting-item {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: var(--space-3) 0;
  border-bottom: 1px solid var(--border-color);
}

.setting-item:last-child {
  border-bottom: none;
}

.setting-info {
  flex: 1;
}

.setting-label {
  display: block;
  font-size: var(--font-size-sm);
  font-weight: var(--font-weight-semibold);
  margin-bottom: 2px;
}

.setting-desc {
  display: block;
  font-size: var(--font-size-xs);
  color: var(--color-text-tertiary);
}

/* Toggle */
.toggle {
  position: relative;
  display: inline-block;
  width: 44px;
  height: 24px;
  cursor: pointer;
}

.toggle input {
  opacity: 0;
  width: 0;
  height: 0;
}

.toggle-slider {
  position: absolute;
  inset: 0;
  background: var(--color-bg-primary);
  border: 1px solid var(--border-color);
  border-radius: var(--radius-full);
  transition: all 0.2s ease;
}

.toggle-slider::before {
  content: '';
  position: absolute;
  width: 16px;
  height: 16px;
  left: 3px;
  bottom: 3px;
  background: var(--color-text-secondary);
  border-radius: 50%;
  transition: all 0.2s ease;
}

.toggle input:checked + .toggle-slider {
  background: var(--color-primary);
  border-color: var(--color-primary);
}

.toggle input:checked + .toggle-slider::before {
  transform: translateX(20px);
  background: white;
}

/* Integrations list */
.integrations-list {
  display: flex;
  flex-direction: column;
  gap: var(--space-4);
}

.integration-card {
  padding: var(--space-5);
  border-radius: var(--radius-xl);
  border: 1px solid var(--border-color);
}

.integration-header {
  display: flex;
  align-items: center;
  gap: var(--space-5);
}

.integration-info {
  flex: 1;
}

.integration-info h4 {
  font-size: var(--font-size-base);
  font-weight: var(--font-weight-semibold);
  margin-bottom: 2px;
}

.integration-info p {
  font-size: var(--font-size-xs);
}

.btn-connect {
  background: var(--color-primary);
  color: white;
  border: none;
  padding: 8px var(--space-4);
  font-size: var(--font-size-xs);
  font-weight: var(--font-weight-semibold);
  border-radius: var(--radius-lg);
  cursor: pointer;
  transition: all var(--transition-fast);
}

.btn-connect:hover {
  background: var(--color-primary-hover);
}

/* Slide fade animation */
.slide-fade-enter-active, .slide-fade-leave-active {
  transition: all 0.25s ease;
}

.slide-fade-enter-from, .slide-fade-leave-to {
  opacity: 0;
  transform: translateY(-8px);
}
</style>
