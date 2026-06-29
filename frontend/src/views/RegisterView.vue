<template>
  <div class="auth-page">
    <!-- Left Panel: Modern SaaS Showcase (Desktop only) -->
    <div class="showcase-panel">
      <!-- Geometric Background Patterns -->
      <div class="showcase-bg">
        <div class="geo-shape shape-1"></div>
        <div class="geo-shape shape-2"></div>
      </div>

      <div class="showcase-content">
        <!-- Logo Header Card -->
        <div class="showcase-logo-card">
          <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" class="showcase-logo-svg">
            <line x1="6" y1="20" x2="6" y2="14" />
            <line x1="12" y1="20" x2="12" y2="4" />
            <line x1="18" y1="20" x2="18" y2="10" />
          </svg>
        </div>

        <h2 class="showcase-title">Quản lý dự án chuẩn Enterprise với ProTask</h2>
        <p class="showcase-subtitle">
          Hệ thống quản lý tinh gọn cho các đội nhóm chuyên nghiệp. Đưa năng suất làm việc của doanh nghiệp lên một tầm cao mới.
        </p>
        
        <!-- Showcase Task Card matching mockup -->
        <div class="showcase-task-card">
          <div class="task-card-header">
            <span class="task-tag">PRO-2402</span>
            <div class="task-toggle">
              <span class="toggle-circle"></span>
            </div>
          </div>
          <h3 class="task-card-title">Triển khai hạ tầng Cloud</h3>
          <span class="task-card-meta">Ưu tiên cao • Còn 2 ngày</span>
          <div class="task-progress-bar">
            <div class="progress-fill" style="width: 80%"></div>
          </div>
        </div>

        <div class="showcase-footer">
          © 2024 ProTask Inc. Enterprise PM Solution.
        </div>
      </div>
    </div>

    <!-- Right Panel: Signup Form -->
    <div class="form-panel">
      <div class="auth-card">
        <div class="auth-header">
          <h1>Tạo tài khoản mới</h1>
          <p class="auth-subtitle">Bắt đầu quản lý dự án hiệu quả cùng ProTask ngay hôm nay.</p>
        </div>

        <form @submit.prevent="handleRegister" class="auth-form">
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

          <transition name="fade">
            <div v-if="success" class="alert alert-success">
              <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <polyline points="20 6 9 17 4 12"/>
              </svg>
              <span>Đăng ký thành công! <router-link to="/login">Đăng nhập ngay</router-link></span>
            </div>
          </transition>

          <!-- Full Name Input -->
          <div class="custom-form-group">
            <label class="custom-label">HỌ VÀ TÊN</label>
            <div class="input-wrapper">
              <span class="input-icon">
                <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                  <path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2" />
                  <circle cx="12" cy="7" r="4" />
                </svg>
              </span>
              <input
                type="text"
                v-model="form.fullName"
                placeholder="Nguyễn Văn A"
                required
                class="custom-input"
                id="input-register-fullname"
              />
            </div>
          </div>

          <!-- Email Input -->
          <div class="custom-form-group">
            <label class="custom-label">EMAIL CÔNG TY</label>
            <div class="input-wrapper">
              <span class="input-icon">
                <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                  <path d="M4 4h16c1.1 0 2 .9 2 2v12c0 1.1-.9 2-2 2H4c-1.1 0-2-.9-2-2V6c0-1.1.9-2 2-2z" />
                  <polyline points="22,6 12,13 2,6" />
                </svg>
              </span>
              <input
                type="email"
                v-model="form.email"
                placeholder="name@company.com"
                required
                class="custom-input"
                id="input-register-email"
              />
            </div>
          </div>

          <!-- Password Input -->
          <div class="custom-form-group">
            <label class="custom-label">MẬT KHẨU</label>
            <div class="input-wrapper">
              <span class="input-icon">
                <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                  <rect x="3" y="11" width="18" height="11" rx="2" ry="2" />
                  <path d="M7 11V7a5 5 0 0 1 10 0v4" />
                </svg>
              </span>
              <input
                :type="showPassword ? 'text' : 'password'"
                v-model="form.password"
                placeholder="••••••••"
                required
                class="custom-input"
                id="input-register-password"
              />
              <button type="button" @click="showPassword = !showPassword" class="password-toggle-btn" title="Hiển thị mật khẩu">
                <svg v-if="showPassword" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                  <path d="M17.94 17.94A10.07 10.07 0 0 1 12 20c-7 0-11-8-11-8a18.45 18.45 0 0 1 5.06-5.94M9.9 4.24A9.12 9.12 0 0 1 12 4c7 0 11 8 11 8a18.5 18.5 0 0 1-2.16 3.19m-6.72-1.07a3 3 0 1 1-4.24-4.24" />
                  <line x1="1" y1="1" x2="23" y2="23" />
                </svg>
                <svg v-else width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                  <path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z" />
                  <circle cx="12" cy="12" r="3" />
                </svg>
              </button>
            </div>
          </div>

          <!-- Confirm Password Input -->
          <div class="custom-form-group">
            <label class="custom-label">XÁC NHẬN MẬT KHẨU</label>
            <div class="input-wrapper">
              <span class="input-icon">
                <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                  <path d="M12 22c5.523 0 10-4.477 10-10S17.523 2 12 2 2 6.477 2 12s4.477 10 10 10z" />
                  <path d="M12 6v6l4 2" />
                </svg>
              </span>
              <input
                :type="showConfirmPassword ? 'text' : 'password'"
                v-model="confirmPassword"
                placeholder="••••••••"
                required
                class="custom-input"
                id="input-register-confirm"
              />
              <button type="button" @click="showConfirmPassword = !showConfirmPassword" class="password-toggle-btn" title="Hiển thị mật khẩu">
                <svg v-if="showConfirmPassword" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                  <path d="M17.94 17.94A10.07 10.07 0 0 1 12 20c-7 0-11-8-11-8a18.45 18.45 0 0 1 5.06-5.94M9.9 4.24A9.12 9.12 0 0 1 12 4c7 0 11 8 11 8a18.5 18.5 0 0 1-2.16 3.19m-6.72-1.07a3 3 0 1 1-4.24-4.24" />
                  <line x1="1" y1="1" x2="23" y2="23" />
                </svg>
                <svg v-else width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                  <path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z" />
                  <circle cx="12" cy="12" r="3" />
                </svg>
              </button>
            </div>
          </div>

          <!-- Terms Checkbox option -->
          <div class="form-options">
            <label class="checkbox-container">
              <input type="checkbox" v-model="agreeTerms" required />
              <span class="checkmark"></span>
              <span class="checkbox-label">Tôi đồng ý với <a href="#" class="inline-link">Điều khoản</a> & <a href="#" class="inline-link">Chính sách bảo mật</a> của ProTask.</span>
            </label>
          </div>

          <!-- Submit Button -->
          <button type="submit" class="btn-submit-blue" :disabled="authStore.loading">
            <span v-if="authStore.loading">Đang đăng ký...</span>
            <span v-else>Đăng ký tài khoản</span>
          </button>
        </form>

        <!-- Social Separator -->
        <div class="social-separator">
          <span class="separator-text">HOẶC TIẾP TỤC VỚI</span>
        </div>

        <!-- Social Buttons Grid -->
        <div class="social-grid">
          <button type="button" @click="loginWithGoogle" class="btn-social">
            <img src="https://img.icons8.com/color/16/000000/google-logo.png" alt="Google" class="social-icon-img" />
            Google
          </button>
          <button type="button" @click="loginWithGithub" class="btn-social">
            <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" style="color: #24292e;">
              <path d="M9 19c-5 1.5-5-2.5-7-3m14 6v-3.87a3.37 3.37 0 0 0-.94-2.61c3.14-.35 6.44-1.54 6.44-7A5.44 5.44 0 0 0 20 4.77 5.07 5.07 0 0 0 19.91 1S18.73.65 16 2.48a13.38 13.38 0 0 0-7 0C6.27.65 5.09 1 5.09 1A5.07 5.07 0 0 0 5 4.77a5.44 5.44 0 0 0-1.5 3.78c0 5.42 3.3 6.61 6.44 7A3.37 3.37 0 0 0 9 18.13V22" />
            </svg>
            GitHub
          </button>
        </div>

        <!-- Footer Link -->
        <div class="auth-footer">
          <span class="footer-text">Đã có tài khoản?</span>
          <router-link to="/login" class="footer-link">Đăng nhập</router-link>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive } from 'vue'
import { useAuthStore } from '../stores/auth'

const authStore = useAuthStore()
const success = ref(false)
const showPassword = ref(false)
const showConfirmPassword = ref(false)
const agreeTerms = ref(false)
const confirmPassword = ref('')

const GOOGLE_CLIENT_ID = import.meta.env.VITE_GOOGLE_CLIENT_ID || '919341718553-lsbmr6rfflptpbmckpbht0h4b1iccjj9.apps.googleusercontent.com'
const GITHUB_CLIENT_ID = import.meta.env.VITE_GITHUB_CLIENT_ID || 'Ov23lirIaP5cgYJ7mvKt'
const CALLBACK_URL = window.location.origin + '/login'

function loginWithGoogle() {
  const authUrl = `https://accounts.google.com/o/oauth2/v2/auth?client_id=${GOOGLE_CLIENT_ID}&redirect_uri=${encodeURIComponent(CALLBACK_URL)}&response_type=code&scope=openid%20email%20profile&state=google`
  window.location.href = authUrl
}

function loginWithGithub() {
  const authUrl = `https://github.com/login/oauth/authorize?client_id=${GITHUB_CLIENT_ID}&redirect_uri=${encodeURIComponent(CALLBACK_URL)}&scope=user:email&state=github`
  window.location.href = authUrl
}

const form = reactive({
  fullName: '',
  username: '',
  email: '',
  password: ''
})

async function handleRegister() {
  authStore.error = null
  success.value = false

  // Generate username from email prefix dynamically
  if (form.email) {
    form.username = form.email.split('@')[0]
  }

  // Validate passwords match
  if (form.password !== confirmPassword.value) {
    authStore.error = 'Mật khẩu xác nhận không khớp!'
    return
  }

  const result = await authStore.register(form)
  if (result) {
    success.value = true
  }
}
</script>

<style scoped>
.auth-page {
  min-height: 100vh;
  display: flex;
  background-color: #f8fafc;
}

/* Showcase Panel Styling (Left side) */
.showcase-panel {
  flex: 1.1;
  background: #004ecc; /* Primary rich blue background from mockup */
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 48px;
  color: white;
  position: relative;
  overflow: hidden;
}

@media (max-width: 1024px) {
  .showcase-panel {
    display: none;
  }
}

/* Geometric backgrounds matching mockup */
.showcase-bg {
  position: absolute;
  inset: 0;
  pointer-events: none;
  z-index: 1;
}

.geo-shape {
  position: absolute;
  opacity: 0.15;
  background: linear-gradient(135deg, rgba(255,255,255,0.4) 0%, transparent 100%);
}

.geo-shape.shape-1 {
  width: 120%;
  height: 120%;
  top: -10%;
  left: -20%;
  clip-path: polygon(0 0, 100% 0, 80% 100%, 0% 100%);
}

.geo-shape.shape-2 {
  width: 100%;
  height: 100%;
  bottom: -30%;
  right: -20%;
  clip-path: polygon(30% 0, 100% 40%, 100% 100%, 0% 100%);
}

.showcase-content {
  max-width: 480px;
  z-index: 10;
  display: flex;
  flex-direction: column;
  height: 100%;
  justify-content: center;
}

.showcase-logo-card {
  width: 48px;
  height: 48px;
  background: rgba(255, 255, 255, 0.12);
  border: 1px solid rgba(255, 255, 255, 0.2);
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  margin-bottom: 32px;
  color: white;
}

.showcase-logo-svg {
  filter: drop-shadow(0 2px 8px rgba(0,0,0,0.15));
}

.showcase-title {
  font-size: 2.5rem;
  font-weight: 800;
  line-height: 1.2;
  margin-bottom: 16px;
  color: white;
  letter-spacing: -1px;
}

.showcase-subtitle {
  color: rgba(255, 255, 255, 0.85);
  font-size: 0.95rem;
  line-height: 1.6;
  margin-bottom: 40px;
  font-weight: 500;
}

/* Custom Task Card for registration left panel */
.showcase-task-card {
  background: rgba(255, 255, 255, 0.08);
  border: 1.5px solid rgba(255, 255, 255, 0.18);
  border-radius: 16px;
  padding: 24px;
  display: flex;
  flex-direction: column;
  backdrop-filter: blur(10px);
  margin-bottom: 48px;
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.15);
}

.task-card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 14px;
}

.task-tag {
  font-size: 10px;
  font-weight: 800;
  color: #93c5fd;
  background: rgba(37, 99, 235, 0.3);
  padding: 4px 8px;
  border-radius: 4px;
}

.task-toggle {
  width: 32px;
  height: 16px;
  border-radius: 8px;
  background: white;
  padding: 2px;
  display: flex;
  align-items: center;
  justify-content: flex-end; /* Toggle switch active state */
}

.toggle-circle {
  width: 12px;
  height: 12px;
  border-radius: 50%;
  background: #004ecc;
}

.task-card-title {
  font-size: 1.25rem;
  font-weight: 700;
  color: white;
  margin: 0 0 6px 0;
}

.task-card-meta {
  font-size: 0.75rem;
  color: rgba(255, 255, 255, 0.7);
  font-weight: 500;
  margin-bottom: 16px;
}

.task-progress-bar {
  height: 6px;
  background: rgba(255, 255, 255, 0.15);
  border-radius: 3px;
  overflow: hidden;
}

.progress-fill {
  height: 100%;
  background: white;
  border-radius: 3px;
}

.showcase-footer {
  font-size: 0.75rem;
  color: rgba(255, 255, 255, 0.5);
  font-weight: 500;
  margin-top: auto;
}

/* Form Panel Styling (Right side) */
.form-panel {
  flex: 0.9;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 48px;
  background: #f8fafc;
}

.auth-card {
  width: 100%;
  max-width: 420px;
  background: transparent;
}

.auth-header {
  margin-bottom: 28px;
}

.auth-header h1 {
  font-size: 1.75rem;
  font-weight: 800;
  color: #0f172a;
  margin: 0 0 8px 0;
  letter-spacing: -0.5px;
}

.auth-subtitle {
  color: #64748b;
  font-size: 0.875rem;
  margin: 0;
  font-weight: 500;
}

/* Custom inputs styling */
.custom-form-group {
  display: flex;
  flex-direction: column;
  gap: 8px;
  margin-bottom: 18px;
}

.custom-label {
  font-size: 0.7rem;
  font-weight: 800;
  color: #475569;
  letter-spacing: 0.5px;
}

.input-wrapper {
  position: relative;
  display: flex;
  align-items: center;
}

.input-icon {
  position: absolute;
  left: 14px;
  color: #94a3b8;
  display: flex;
  align-items: center;
}

.custom-input {
  width: 100%;
  padding: 12px 14px 12px 42px;
  background: #f1f5f9;
  border: 1px solid #e2e8f0;
  border-radius: 8px;
  font-size: 0.875rem;
  color: #0f172a;
  outline: none;
  transition: all 0.2s ease;
}

.custom-input:focus {
  background: white;
  border-color: #2563eb;
  box-shadow: 0 0 0 3px rgba(37, 99, 235, 0.1);
}

.password-toggle-btn {
  position: absolute;
  right: 14px;
  background: transparent;
  border: none;
  color: #94a3b8;
  cursor: pointer;
  padding: 0;
  display: flex;
  align-items: center;
}

.password-toggle-btn:hover {
  color: #64748b;
}

/* Checkbox and options */
.form-options {
  display: flex;
  align-items: center;
  margin-bottom: 24px;
}

.checkbox-container {
  display: flex;
  align-items: flex-start;
  position: relative;
  padding-left: 24px;
  cursor: pointer;
  user-select: none;
}

.checkbox-container input {
  position: absolute;
  opacity: 0;
  cursor: pointer;
  height: 0;
  width: 0;
}

.checkmark {
  position: absolute;
  left: 0;
  top: 2px;
  height: 16px;
  width: 16px;
  background-color: white;
  border: 1.5px solid #cbd5e1;
  border-radius: 4px;
  transition: all 0.2s ease;
}

.checkbox-container:hover input ~ .checkmark {
  border-color: #94a3b8;
}

.checkbox-container input:checked ~ .checkmark {
  background-color: #2563eb;
  border-color: #2563eb;
}

.checkmark::after {
  content: "";
  position: absolute;
  display: none;
  left: 5px;
  top: 2px;
  width: 4px;
  height: 8px;
  border: solid white;
  border-width: 0 2px 2px 0;
  transform: rotate(45deg);
}

.checkbox-container input:checked ~ .checkmark::after {
  display: block;
}

.checkbox-label {
  font-size: 0.75rem;
  color: #475569;
  font-weight: 500;
  line-height: 1.4;
}

.inline-link {
  color: #004ecc;
  font-weight: 700;
  text-decoration: none;
}

.inline-link:hover {
  text-decoration: underline;
}

/* Submit Button */
.btn-submit-blue {
  width: 100%;
  padding: 14px;
  background: #004ecc;
  color: white;
  border: none;
  border-radius: 8px;
  font-size: 0.875rem;
  font-weight: 700;
  cursor: pointer;
  transition: all 0.2s ease;
  box-shadow: 0 4px 12px rgba(0, 78, 204, 0.15);
}

.btn-submit-blue:hover {
  background: #003bb3;
  transform: translateY(-1px);
}

.btn-submit-blue:active {
  transform: translateY(0);
}

.btn-text-flex {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
}

/* Social Login Separator */
.social-separator {
  position: relative;
  text-align: center;
  margin: 28px 0 20px;
}

.social-separator::before {
  content: "";
  position: absolute;
  left: 0;
  top: 50%;
  width: 100%;
  height: 1px;
  background: #e2e8f0;
  z-index: 1;
}

.separator-text {
  position: relative;
  background: #f8fafc;
  padding: 0 12px;
  font-size: 0.7rem;
  font-weight: 800;
  color: #94a3b8;
  letter-spacing: 0.8px;
  z-index: 2;
}

/* Social buttons */
.social-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 12px;
  margin-bottom: 28px;
}

.btn-social {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
  padding: 10px;
  background: #eef2f6;
  border: 1px solid #e2e8f0;
  border-radius: 8px;
  font-size: 0.8rem;
  font-weight: 700;
  color: #334155;
  cursor: pointer;
  transition: all 0.2s ease;
}

.btn-social:hover {
  background: #e2e8f0;
}

.social-icon-img {
  width: 16px;
  height: 16px;
}

/* Footer Link styling */
.auth-footer {
  display: flex;
  justify-content: center;
  gap: 6px;
  font-size: 0.8rem;
  font-weight: 600;
}

.footer-text {
  color: #64748b;
}

.footer-link {
  color: #004ecc;
  font-weight: 700;
  text-decoration: none;
}

.footer-link:hover {
  text-decoration: underline;
}

/* Alert Styling */
.alert {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 12px;
  border-radius: 8px;
  font-size: 0.8rem;
  margin-bottom: 16px;
  font-weight: 600;
  border: 1px solid transparent;
}

.alert-error {
  background: #fee2e2;
  color: #ef4444;
  border-color: rgba(239, 68, 68, 0.2);
}

.alert-success {
  background: #d1fae5;
  color: #10b981;
  border-color: rgba(16, 185, 129, 0.2);
}

.fade-enter-active, .fade-leave-active {
  transition: opacity 0.25s ease;
}

.fade-enter-from, .fade-leave-to {
  opacity: 0;
}
</style>
