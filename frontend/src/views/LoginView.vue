<template>
  <div class="auth-page">
    <!-- Left Panel: Modern SaaS Showcase (Desktop only) -->
    <div class="showcase-panel">
      <div class="showcase-content">
        <div class="showcase-tag">Phiên bản v2.0</div>
        <h2 class="showcase-title">Quản lý dự án tối ưu & trực quan</h2>
        <p class="showcase-subtitle">Hợp tác đội ngũ, quản lý tác vụ Kanban và theo dõi tiến độ thời gian thực một cách liền mạch.</p>
        
        <!-- CSS Mockup Illustration -->
        <div class="mockup-container">
          <div class="mockup-header">
            <span class="dot red"></span>
            <span class="dot yellow"></span>
            <span class="dot green"></span>
            <div class="mockup-tab">Project Analytics</div>
          </div>
          <div class="mockup-body">
            <div class="mockup-row">
              <div class="mockup-card p-4">
                <span class="mockup-label">Tổng công việc</span>
                <span class="mockup-val">42</span>
                <div class="mockup-bar"><div class="fill" style="width: 78%"></div></div>
              </div>
              <div class="mockup-card p-4">
                <span class="mockup-label">Hoàn thành</span>
                <span class="mockup-val text-emerald">82%</span>
                <div class="mockup-chart-circle">
                  <svg width="40" height="40" viewBox="0 0 36 36">
                    <path class="circle-bg" d="M18 2.0845 a 15.9155 15.9155 0 0 1 0 31.831 a 15.9155 15.9155 0 0 1 0 -31.831" />
                    <path class="circle" stroke-dasharray="82, 100" d="M18 2.0845 a 15.9155 15.9155 0 0 1 0 31.831 a 15.9155 15.9155 0 0 1 0 -31.831" />
                  </svg>
                </div>
              </div>
            </div>
            <div class="mockup-list mt-4">
              <div class="mockup-list-item">
                <span class="indicator todo"></span>
                <span class="item-txt">Thiết kế landing page UI/UX</span>
                <span class="item-tag low">Low</span>
              </div>
              <div class="mockup-list-item">
                <span class="indicator progress"></span>
                <span class="item-txt">Tích hợp API Auth C#</span>
                <span class="item-tag high">High</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Right Panel: Login Form -->
    <div class="form-panel">
      <div class="auth-card glass-panel animate-pulse-glow">
        <div class="auth-header">
          <div class="auth-logo">
            <div class="logo-icon">
              <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
                <rect x="3" y="3" width="7" height="7" rx="1.5" />
                <rect x="14" y="3" width="7" height="7" rx="1.5" />
                <rect x="3" y="14" width="7" height="7" rx="1.5" />
                <rect x="14" y="14" width="7" height="7" rx="1.5" />
              </svg>
            </div>
          </div>
          <h1>Chào mừng trở lại</h1>
          <p class="auth-subtitle">Đăng nhập tài khoản của bạn để tiếp tục làm việc</p>
        </div>

        <form @submit.prevent="handleLogin" class="auth-form">
          <transition name="fade">
            <div v-if="authStore.error" class="alert alert-error">
              <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <circle cx="12" cy="12" r="10"/>
                <line x1="12" y1="8" x2="12" y2="12"/>
                <line x1="12" y1="16" x2="12.01" y2="16"/>
              </svg>
              <span>{{ authStore.error }}</span>
            </div>
          </transition>

          <BaseInput
            v-model="form.usernameOrEmail"
            label="Tên đăng nhập hoặc Email"
            placeholder="Nhập tên hoặc email..."
            required
            id="input-login-username"
          >
            <template #start>
              <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2" />
                <circle cx="12" cy="7" r="4" />
              </svg>
            </template>
          </BaseInput>

          <BaseInput
            v-model="form.password"
            label="Mật khẩu"
            type="password"
            placeholder="Nhập mật khẩu..."
            required
            id="input-login-password"
          >
            <template #start>
              <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <rect x="3" y="11" width="18" height="11" rx="2" ry="2" />
                <path d="M7 11V7a5 5 0 0 1 10 0v4" />
              </svg>
            </template>
          </BaseInput>

          <BaseButton
            type="submit"
            variant="primary"
            block
            :loading="authStore.loading"
            size="lg"
            class="mt-6"
          >
            Đăng nhập
          </BaseButton>
        </form>

        <div class="auth-footer">
          <p>Chưa có tài khoản? <router-link to="/register">Đăng ký ngay</router-link></p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { reactive } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth'
import BaseInput from '../components/common/BaseInput.vue'
import BaseButton from '../components/common/BaseButton.vue'

const router = useRouter()
const authStore = useAuthStore()

const form = reactive({
  usernameOrEmail: '',
  password: ''
})

async function handleLogin() {
  const success = await authStore.login(form)
  if (success) {
    router.push('/dashboard')
  }
}
</script>

<style scoped>
.auth-page {
  min-height: 100vh;
  display: flex;
  background-color: var(--bg-primary);
}

/* Showcase Panel Styling */
.showcase-panel {
  flex: 1.2;
  background: linear-gradient(135deg, #0f172a 0%, #1e1b4b 50%, #030712 100%);
  display: flex;
  align-items: center;
  justify-content: center;
  padding: var(--space-12);
  color: white;
  position: relative;
  overflow: hidden;
}

@media (max-width: 1024px) {
  .showcase-panel {
    display: none;
  }
}

.showcase-panel::before {
  content: '';
  position: absolute;
  width: 300px;
  height: 300px;
  background: radial-gradient(circle, rgba(99, 102, 241, 0.15) 0%, transparent 70%);
  top: -50px;
  left: -50px;
}

.showcase-content {
  max-width: 520px;
  z-index: 10;
}

.showcase-tag {
  display: inline-block;
  padding: 4px 12px;
  background: rgba(99, 102, 241, 0.18);
  border: 1px solid rgba(99, 102, 241, 0.3);
  color: #a5b4fc;
  border-radius: var(--radius-full);
  font-size: var(--font-size-xs);
  font-weight: var(--font-weight-semibold);
  margin-bottom: var(--space-4);
  letter-spacing: 0.5px;
}

.showcase-title {
  font-size: var(--font-size-3xl);
  font-weight: var(--font-weight-extrabold);
  line-height: 1.25;
  margin-bottom: var(--space-3);
  background: linear-gradient(to right, #ffffff, #c7d2fe);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
}

.showcase-subtitle {
  color: #94a3b8;
  font-size: var(--font-size-base);
  line-height: 1.6;
  margin-bottom: var(--space-10);
}

/* CSS Mockup Dashboard Grid */
.mockup-container {
  background: rgba(30, 41, 59, 0.6);
  border: 1px solid rgba(255, 255, 255, 0.08);
  border-radius: var(--radius-lg);
  box-shadow: 0 20px 40px rgba(0, 0, 0, 0.4);
  backdrop-filter: blur(12px);
  width: 100%;
  animation: float 6s ease-in-out infinite;
}

.mockup-header {
  display: flex;
  align-items: center;
  gap: 6px;
  padding: 12px 16px;
  border-bottom: 1px solid rgba(255, 255, 255, 0.06);
}

.dot {
  width: 10px;
  height: 10px;
  border-radius: 50%;
}
.dot.red { background: #ef4444; }
.dot.yellow { background: #eab308; }
.dot.green { background: #22c55e; }

.mockup-tab {
  font-size: 11px;
  color: #64748b;
  margin-left: 12px;
  font-weight: 600;
}

.mockup-body {
  padding: 20px;
}

.mockup-row {
  display: flex;
  gap: 16px;
}

.mockup-card {
  flex: 1;
  background: rgba(15, 23, 42, 0.5);
  border: 1px solid rgba(255, 255, 255, 0.04);
  border-radius: var(--radius-md);
  padding: 14px;
  display: flex;
  flex-direction: column;
  position: relative;
}

.mockup-label {
  font-size: 11px;
  color: #94a3b8;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.mockup-val {
  font-size: 20px;
  font-weight: 700;
  margin: 6px 0 10px;
}

.mockup-bar {
  height: 4px;
  background: rgba(255, 255, 255, 0.1);
  border-radius: 2px;
  overflow: hidden;
}
.mockup-bar .fill {
  height: 100%;
  background: #6366f1;
}

.mockup-chart-circle {
  position: absolute;
  right: 14px;
  top: 14px;
}

.circle-bg {
  fill: none;
  stroke: rgba(255, 255, 255, 0.06);
  stroke-width: 3.8;
}

.circle {
  fill: none;
  stroke: #10b981;
  stroke-width: 3.8;
  stroke-linecap: round;
}

.mockup-list {
  background: rgba(15, 23, 42, 0.3);
  border-radius: var(--radius-md);
  padding: 6px;
}

.mockup-list-item {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 10px;
  border-bottom: 1px solid rgba(255, 255, 255, 0.03);
}
.mockup-list-item:last-child {
  border-bottom: none;
}

.indicator {
  width: 8px;
  height: 8px;
  border-radius: 50%;
}
.indicator.todo { background: #64748b; }
.indicator.progress { background: #3b82f6; }

.item-txt {
  font-size: 12px;
  color: #cbd5e1;
  flex: 1;
}

.item-tag {
  font-size: 10px;
  padding: 2px 6px;
  border-radius: 4px;
  font-weight: 600;
}
.item-tag.low { background: rgba(100, 116, 139, 0.15); color: #94a3b8; }
.item-tag.high { background: rgba(245, 158, 11, 0.15); color: #fbbf24; }

/* Form Panel Styling */
.form-panel {
  flex: 1;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: var(--space-8);
}

.auth-card {
  width: 100%;
  max-width: 440px;
  border-radius: var(--radius-xl);
  padding: var(--space-10);
  background: var(--bg-card);
  animation: slideUp 0.45s cubic-bezier(0.16, 1, 0.3, 1) forwards;
  position: relative;
}

@keyframes slideUp {
  from { opacity: 0; transform: translateY(30px); }
  to { opacity: 1; transform: translateY(0); }
}

.auth-header {
  text-align: center;
  margin-bottom: var(--space-8);
}

.auth-logo {
  display: flex;
  justify-content: center;
  margin-bottom: var(--space-4);
}

.logo-icon {
  width: 52px;
  height: 52px;
  background: var(--gradient-primary);
  border-radius: var(--radius-xl);
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  box-shadow: 0 8px 24px rgba(37, 99, 235, 0.25);
}

.auth-header h1 {
  font-size: var(--font-size-2xl);
  font-weight: var(--font-weight-bold);
  margin-bottom: var(--space-2);
  color: var(--text-primary);
}

.auth-subtitle {
  color: var(--text-secondary);
  font-size: var(--font-size-sm);
  font-weight: 500;
}

.auth-form {
  margin-bottom: var(--space-6);
}

.alert {
  display: flex;
  align-items: center;
  gap: var(--space-2);
  padding: var(--space-3) var(--space-4);
  border-radius: var(--radius-md);
  font-size: var(--font-size-sm);
  margin-bottom: var(--space-4);
  font-weight: var(--font-weight-medium);
  border: 1px solid transparent;
}

.alert-error {
  background: var(--color-danger-light);
  color: var(--color-danger);
  border-color: rgba(225, 29, 72, 0.2);
}

.auth-footer {
  text-align: center;
  padding-top: var(--space-5);
  border-top: 1px solid var(--border-color);
}

.auth-footer p {
  font-size: var(--font-size-sm);
  color: var(--text-secondary);
}

.auth-footer a {
  font-weight: var(--font-weight-bold);
  color: var(--accent-primary);
  transition: all var(--transition-fast);
}

.auth-footer a:hover {
  color: var(--accent-primary-hover);
  text-shadow: 0 0 10px rgba(37, 99, 235, 0.2);
}

.mt-6 {
  margin-top: 24px;
}
.mt-4 {
  margin-top: 16px;
}
</style>
