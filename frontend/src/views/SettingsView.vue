<template>
  <div class="settings-page">
    <div class="page-header">
      <h1>Cài đặt & Hồ sơ</h1>
      <p class="text-secondary">Quản lý tài khoản và tùy chỉnh thông báo</p>
    </div>

    <LoadingSpinner v-if="loading" text="Đang tải cài đặt..." />

    <div v-else class="settings-card">
      <div v-if="saveSuccess" class="alert alert-success">
        <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
          <polyline points="20 6 9 17 4 12" />
        </svg>
        Đã lưu cài đặt thành công!
      </div>

      <div class="setting-group profile-group">
        <h3 class="setting-group-title">Hồ sơ cá nhân</h3>
        <div class="profile-info">
          <div class="profile-avatar">
            {{ authStore.displayName?.charAt(0)?.toUpperCase() || 'U' }}
          </div>
          <div class="profile-details">
            <h4 class="profile-name">{{ authStore.displayName }}</h4>
            <p class="profile-role">Vai trò: {{ authStore.user?.role || 'Người dùng' }}</p>
            <p class="profile-email">Username/Email: {{ authStore.user?.username || '—' }}</p>
          </div>
        </div>
      </div>

      <div class="setting-group">
        <h3 class="setting-group-title">Hoạt động dự án</h3>

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

      <div class="setting-group">
        <h3 class="setting-group-title">Sprint & Nhóm</h3>

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

      <div class="settings-actions">
        <BaseButton variant="primary" @click="saveSettings" :loading="saving">
          Lưu cài đặt
        </BaseButton>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { getNotificationSettings, updateNotificationSettings } from '../api/notifyApi'
import { useAuthStore } from '../stores/auth'
import BaseButton from '../components/common/BaseButton.vue'
import LoadingSpinner from '../components/common/LoadingSpinner.vue'

const authStore = useAuthStore()

const loading = ref(true)
const saving = ref(false)
const saveSuccess = ref(false)

const settings = reactive({
  onComment: true,
  onAssigned: true,
  onStatusChanged: true,
  onMention: true,
  onSprintStarted: true,
  onMemberAdded: true
})

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
    // Use defaults
  } finally {
    loading.value = false
  }
}

async function saveSettings() {
  saving.value = true
  saveSuccess.value = false
  try {
    await updateNotificationSettings({ ...settings })
    saveSuccess.value = true
    setTimeout(() => { saveSuccess.value = false }, 3000)
  } catch (err) {
    alert(err.response?.data?.message || 'Không thể lưu cài đặt')
  } finally {
    saving.value = false
  }
}

onMounted(() => {
  loadSettings()
})
</script>

<style scoped>
.settings-page {
  animation: fadeIn 0.3s ease;
  max-width: 700px;
}

@keyframes fadeIn {
  from { opacity: 0; transform: translateY(8px); }
  to { opacity: 1; transform: translateY(0); }
}

.page-header {
  margin-bottom: var(--space-6);
}

.page-header h1 {
  font-size: var(--font-size-2xl);
  margin-bottom: var(--space-1);
}

.settings-card {
  background: var(--bg-white-to-card);
  border: 1px solid var(--border-color);
  border-radius: var(--radius-lg);
  padding: var(--space-6);
}

.alert {
  display: flex;
  align-items: center;
  gap: var(--space-2);
  padding: var(--space-3) var(--space-4);
  border-radius: var(--radius-md);
  font-size: var(--font-size-sm);
  margin-bottom: var(--space-5);
}

.alert-success {
  background: var(--color-success-light);
  color: var(--color-success);
  border: 1px solid rgba(22, 163, 74, 0.2);
}

.setting-group {
  margin-bottom: var(--space-6);
}

.setting-group:last-of-type {
  margin-bottom: var(--space-5);
}

.setting-group-title {
  font-size: var(--font-size-sm);
  font-weight: var(--font-weight-semibold);
  color: var(--color-text-tertiary);
  text-transform: uppercase;
  letter-spacing: 0.5px;
  margin-bottom: var(--space-4);
  padding-bottom: var(--space-2);
  border-bottom: 1px solid var(--color-border);
}

/* Profile specific styles */
.profile-info {
  display: flex;
  align-items: center;
  gap: var(--space-5);
  padding: var(--space-3) 0;
}

.profile-avatar {
  width: 64px;
  height: 64px;
  border-radius: var(--radius-full);
  background: var(--color-primary);
  color: white;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: var(--font-size-2xl);
  font-weight: var(--font-weight-bold);
  box-shadow: var(--shadow-sm);
}

.profile-name {
  font-size: var(--font-size-lg);
  font-weight: var(--font-weight-semibold);
  margin-bottom: var(--space-1);
}

.profile-role, .profile-email {
  font-size: var(--font-size-sm);
  color: var(--color-text-secondary);
  margin-bottom: 2px;
}

.setting-item {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: var(--space-3) 0;
  border-bottom: 1px solid var(--color-bg-secondary);
}

.setting-item:last-child {
  border-bottom: none;
}

.setting-info {
  flex: 1;
}

.setting-label {
  display: block;
  font-size: var(--font-size-base);
  font-weight: var(--font-weight-medium);
  color: var(--color-text-primary);
  margin-bottom: 2px;
}

.setting-desc {
  display: block;
  font-size: var(--font-size-sm);
  color: var(--color-text-tertiary);
}

/* Toggle switch */
.toggle {
  position: relative;
  display: inline-block;
  width: 44px;
  height: 24px;
  flex-shrink: 0;
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
  background: var(--color-border-hover);
  border-radius: var(--radius-full);
  transition: all var(--transition-base);
}

.toggle-slider::before {
  content: '';
  position: absolute;
  width: 18px;
  height: 18px;
  left: 3px;
  bottom: 3px;
  background: white;
  border-radius: 50%;
  transition: transform var(--transition-base);
  box-shadow: var(--shadow-xs);
}

.toggle input:checked + .toggle-slider {
  background: var(--color-primary);
}

.toggle input:checked + .toggle-slider::before {
  transform: translateX(20px);
}

.toggle input:focus-visible + .toggle-slider {
  box-shadow: 0 0 0 3px rgba(37, 99, 235, 0.2);
}

.settings-actions {
  padding-top: var(--space-4);
  border-top: 1px solid var(--color-border);
}
</style>
