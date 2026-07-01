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

        <h2 class="showcase-title">Chào mừng quay trở lại!</h2>
        <p class="showcase-subtitle">
          Trải nghiệm giải pháp quản lý dự án tối ưu dành cho doanh nghiệp. Tiếp tục hành trình tối ưu hóa hiệu suất làm việc cùng ProTask.
        </p>
        
        <!-- Highlight cards row -->
        <div class="showcase-cards">
          <!-- Card 1: Trực quan -->
          <div class="showcase-card">
            <div class="card-icon">
              <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
                <rect x="3" y="3" width="7" height="7" rx="1.5" />
                <rect x="14" y="3" width="7" height="7" rx="1.5" />
                <rect x="3" y="14" width="7" height="7" rx="1.5" />
                <rect x="14" y="14" width="7" height="7" rx="1.5" />
              </svg>
            </div>
            <div class="card-info">
              <h4>Trực quan</h4>
              <p>Bảng Kanban & Scrum linh hoạt.</p>
            </div>
          </div>

          <!-- Card 2: Báo cáo -->
          <div class="showcase-card">
            <div class="card-icon">
              <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
                <line x1="18" y1="20" x2="18" y2="10" />
                <line x1="12" y1="20" x2="12" y2="4" />
                <line x1="6" y1="20" x2="6" y2="14" />
              </svg>
            </div>
            <div class="card-info">
              <h4>Báo cáo</h4>
              <p>Dữ liệu phân tích thời gian thực.</p>
            </div>
          </div>
        </div>

        <div class="showcase-footer">
          © 2024 ProTask Enterprise. Build with precision.
        </div>
      </div>
    </div>

    <!-- Right Panel: Login Form -->
    <div class="form-panel">
      <div class="auth-card">
        <!-- 1. LOGIN CARD VIEW -->
        <div v-if="!forgotMode">
          <div class="auth-header">
            <h1>Đăng nhập hệ thống</h1>
            <p class="auth-subtitle">Vui lòng nhập thông tin tài khoản của bạn để tiếp tục.</p>
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

            <!-- Email / Username Input -->
            <div class="custom-form-group">
              <label class="custom-label">EMAIL</label>
              <div class="input-wrapper">
                <span class="input-icon">
                  <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                    <path d="M4 4h16c1.1 0 2 .9 2 2v12c0 1.1-.9 2-2 2H4c-1.1 0-2-.9-2-2V6c0-1.1.9-2 2-2z" />
                    <polyline points="22,6 12,13 2,6" />
                  </svg>
                </span>
                <input
                  type="text"
                  v-model="form.usernameOrEmail"
                  placeholder="example@protask.com"
                  required
                  class="custom-input"
                  id="input-login-username"
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
                  id="input-login-password"
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

            <!-- Checkbox / Forgot row -->
            <div class="form-options">
              <label class="checkbox-container">
                <input type="checkbox" v-model="rememberMe" />
                <span class="checkmark"></span>
                <span class="checkbox-label">Ghi nhớ đăng nhập</span>
              </label>
              <a href="#" @click.prevent="forgotMode = true" class="forgot-link">Quên mật khẩu?</a>
            </div>

            <!-- Submit Button -->
            <button type="submit" class="btn-submit-blue" :disabled="authStore.loading">
              <span v-if="authStore.loading">Đang đăng nhập...</span>
              <span v-else class="btn-text-flex">
                Đăng nhập
                <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" class="btn-arrow-icon">
                  <line x1="5" y1="12" x2="19" y2="12" />
                  <polyline points="12 5 19 12 12 19" />
                </svg>
              </span>
            </button>
          </form>

          <!-- Social Separator -->
          <div class="social-separator">
            <span class="separator-text">HOẶC ĐĂNG NHẬP VỚI</span>
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
            <span class="footer-text">Bạn chưa có tài khoản?</span>
            <router-link to="/register" class="footer-link">Đăng ký tài khoản mới</router-link>
          </div>
        </div>

        <!-- 2. FORGOT PASSWORD VIEW -->
        <div v-else>
          <div class="auth-header">
            <h1>Khôi phục mật khẩu</h1>
            <p class="auth-subtitle">Nhập email hoặc tên tài khoản của bạn để nhận mật khẩu tạm thời.</p>
          </div>

          <form @submit.prevent="handleForgotPassword" class="auth-form">
            <transition name="fade">
              <div v-if="forgotSuccessMsg" class="alert alert-success">
                <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                  <polyline points="20 6 9 17 4 12" />
                </svg>
                <span>{{ forgotSuccessMsg }}</span>
              </div>
              <div v-else-if="forgotErrorMsg" class="alert alert-error">
                <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                  <circle cx="12" cy="12" r="10"/><line x1="12" y1="8" x2="12" y2="12"/><line x1="12" y1="16" x2="12.01" y2="16"/>
                </svg>
                <span>{{ forgotErrorMsg }}</span>
              </div>
            </transition>

            <div v-if="!tempPasswordReceived" class="custom-form-group">
              <label class="custom-label">EMAIL HOẶC TÊN TÀI KHOẢN</label>
              <div class="input-wrapper">
                <span class="input-icon">
                  <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                    <path d="M4 4h16c1.1 0 2 .9 2 2v12c0 1.1-.9 2-2 2H4c-1.1 0-2-.9-2-2V6c0-1.1.9-2 2-2z" />
                    <polyline points="22,6 12,13 2,6" />
                  </svg>
                </span>
                <input
                  type="text"
                  v-model="forgotEmail"
                  placeholder="Nhập email hoặc username..."
                  required
                  class="custom-input"
                />
              </div>
            </div>

            <div v-else class="temp-password-box">
              <p class="temp-label">Mật khẩu tạm thời mới của bạn là:</p>
              <div class="temp-code-wrapper">
                <span class="temp-code">{{ tempPasswordReceived }}</span>
                <button type="button" class="btn-copy" @click="copyTempPassword">
                  Sao chép
                </button>
              </div>
              <p class="temp-desc">Vui lòng đăng nhập bằng mật khẩu này và đổi lại mật khẩu trong trang Cài đặt.</p>
            </div>

            <button v-if="!tempPasswordReceived" type="submit" class="btn-submit-blue" :disabled="loadingForgot">
              <span v-if="loadingForgot">Đang gửi yêu cầu...</span>
              <span v-else class="btn-text-flex">
                Gửi yêu cầu khôi phục
                <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" class="btn-arrow-icon">
                  <line x1="5" y1="12" x2="19" y2="12" />
                  <polyline points="12 5 19 12 12 19" />
                </svg>
              </span>
            </button>

            <button type="button" class="btn-back-login" @click="backToLogin">
              Quay lại đăng nhập
            </button>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useAuthStore } from '../stores/auth'
import { forgotPassword, linkGithub } from '../api/notifyApi'
import axios from 'axios'

const router = useRouter()
const route = useRoute()
const authStore = useAuthStore()

const showPassword = ref(false)
const rememberMe = ref(false)

const form = reactive({
  usernameOrEmail: '',
  password: ''
})

// Forgot Password recovery states
const forgotMode = ref(false)
const forgotEmail = ref('')
const loadingForgot = ref(false)
const forgotSuccessMsg = ref('')
const forgotErrorMsg = ref('')
const tempPasswordReceived = ref('')

async function handleForgotPassword() {
  loadingForgot.value = true
  forgotSuccessMsg.value = ''
  forgotErrorMsg.value = ''
  tempPasswordReceived.value = ''
  try {
    const res = await forgotPassword({ email: forgotEmail.value })
    forgotSuccessMsg.value = res.data.message || 'Yêu cầu khôi phục thành công!'
    tempPasswordReceived.value = res.data.tempPassword || ''
  } catch (err) {
    forgotErrorMsg.value = err.response?.data?.message || 'Khôi phục mật khẩu thất bại.'
  } finally {
    loadingForgot.value = false
  }
}

function backToLogin() {
  forgotMode.value = false
  forgotEmail.value = ''
  forgotSuccessMsg.value = ''
  forgotErrorMsg.value = ''
  tempPasswordReceived.value = ''
}

function copyTempPassword() {
  if (tempPasswordReceived.value) {
    navigator.clipboard.writeText(tempPasswordReceived.value)
    alert('Đã sao chép mật khẩu tạm thời vào clipboard!')
  }
}

const GOOGLE_CLIENT_ID = import.meta.env.VITE_GOOGLE_CLIENT_ID || '919341718553-lsbmr6rfflptpbmckpbht0h4b1iccjj9.apps.googleusercontent.com'
const GITHUB_CLIENT_ID = import.meta.env.VITE_GITHUB_CLIENT_ID || 'Ov23lirIaP5cgYJ7mvKt'
const CALLBACK_URL = window.location.origin + '/login'

onMounted(async () => {
  const code = route.query.code
  const state = route.query.state

  if (code && state) {
    authStore.loading = true
    authStore.error = null
    try {
      if (state === 'github-link') {
        await linkGithub({
          code: code,
          redirectUri: CALLBACK_URL
        })
        alert('Liên kết tài khoản GitHub thành công!')
        await authStore.fetchUser()
        router.push({ path: '/settings', query: { tab: 'integrations' } })
        return
      }

      let endpoint = ''
      if (state === 'google') {
        endpoint = import.meta.env.VITE_NOTIFY_API_URL + '/api/Auth/google-login'
      } else if (state === 'github') {
        endpoint = import.meta.env.VITE_NOTIFY_API_URL + '/api/Auth/github-login'
      }

      if (endpoint) {
        const response = await axios.post(endpoint, {
          code: code,
          redirectUri: CALLBACK_URL
        })

        if (response.data && response.data.token) {
          authStore.token = response.data.token
          localStorage.setItem('token', response.data.token)
          await authStore.fetchUser()
          router.push('/dashboard')
        }
      }
    } catch (err) {
      console.error('Social login error:', err)
      if (state === 'github-link') {
        alert(err.response?.data?.message || 'Liên kết GitHub thất bại.')
        router.push({ path: '/settings', query: { tab: 'integrations' } })
      } else {
        authStore.error = err.response?.data?.message || 'Đăng nhập bằng mạng xã hội thất bại.'
      }
    } finally {
      authStore.loading = false
    }
  }
})

function loginWithGoogle() {
  const authUrl = `https://accounts.google.com/o/oauth2/v2/auth?client_id=${GOOGLE_CLIENT_ID}&redirect_uri=${encodeURIComponent(CALLBACK_URL)}&response_type=code&scope=openid%20email%20profile&state=google`
  window.location.href = authUrl
}

function loginWithGithub() {
  const authUrl = `https://github.com/login/oauth/authorize?client_id=${GITHUB_CLIENT_ID}&redirect_uri=${encodeURIComponent(CALLBACK_URL)}&scope=user:email&state=github`
  window.location.href = authUrl
}

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

/* Info Cards Row */
.showcase-cards {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 16px;
  margin-bottom: 48px;
}

.showcase-card {
  background: rgba(255, 255, 255, 0.08);
  border: 1px solid rgba(255, 255, 255, 0.12);
  border-radius: 12px;
  padding: 16px;
  display: flex;
  flex-direction: column;
  gap: 12px;
  backdrop-filter: blur(8px);
}

.card-icon {
  width: 36px;
  height: 36px;
  border-radius: 8px;
  background: rgba(255, 255, 255, 0.12);
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
}

.card-info h4 {
  font-size: 0.9rem;
  font-weight: 700;
  margin: 0 0 4px 0;
  color: white;
}

.card-info p {
  font-size: 0.75rem;
  color: rgba(255, 255, 255, 0.75);
  margin: 0;
  font-weight: 500;
  line-height: 1.4;
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
  margin-bottom: 32px;
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
  margin-bottom: 20px;
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
  background: #f1f5f9; /* Light grey tint from mockup */
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
  justify-content: space-between;
  align-items: center;
  margin-bottom: 28px;
}

.checkbox-container {
  display: flex;
  align-items: center;
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
  font-size: 0.8rem;
  color: #475569;
  font-weight: 600;
}

.forgot-link {
  font-size: 0.8rem;
  font-weight: 700;
  color: #004ecc;
  text-decoration: none;
}

.forgot-link:hover {
  text-decoration: underline;
}

/* Submit Button */
.btn-submit-blue {
  width: 100%;
  padding: 14px;
  background: #004ecc; /* Solid blue from mockup */
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

.btn-arrow-icon {
  transition: transform 0.2s ease;
}

.btn-submit-blue:hover .btn-arrow-icon {
  transform: translateX(2px);
}

/* Social Login Separator */
.social-separator {
  position: relative;
  text-align: center;
  margin: 32px 0 24px;
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
  margin-bottom: 32px;
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
  background: #dcfce7;
  color: #16a34a;
  border-color: rgba(22, 163, 74, 0.2);
}

/* Forgot Password Card Styling */
.temp-password-box {
  text-align: center;
  padding: 20px;
  background: #f0f9ff;
  border: 1px solid #bae6fd;
  border-radius: 12px;
  margin-bottom: 16px;
}

.temp-label {
  font-size: 0.85rem;
  color: #475569;
  font-weight: 600;
  margin-bottom: 12px;
}

.temp-code-wrapper {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 12px;
  margin-bottom: 12px;
}

.temp-code {
  background: white;
  padding: 10px 20px;
  border-radius: 8px;
  font-family: 'Courier New', monospace;
  font-size: 1.1rem;
  font-weight: 700;
  color: #0f172a;
  letter-spacing: 1px;
  border: 1px solid #e2e8f0;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.05);
}

.btn-copy {
  background: #2563eb;
  color: white;
  border: none;
  padding: 8px 16px;
  border-radius: 8px;
  font-size: 0.75rem;
  font-weight: 700;
  cursor: pointer;
  transition: all 0.2s ease;
}

.btn-copy:hover {
  background: #1d4ed8;
}

.temp-desc {
  font-size: 0.75rem;
  color: #64748b;
}

.btn-back-login {
  width: 100%;
  background: transparent;
  border: 1px solid #e2e8f0;
  color: #475569;
  padding: 12px;
  border-radius: 10px;
  font-size: 0.85rem;
  font-weight: 700;
  cursor: pointer;
  margin-top: 8px;
  transition: all 0.2s ease;
}

.btn-back-login:hover {
  background: #f1f5f9;
  color: #0f172a;
}

.fade-enter-active, .fade-leave-active {
  transition: opacity 0.25s ease;
}

.fade-enter-from, .fade-leave-to {
  opacity: 0;
}
</style>
