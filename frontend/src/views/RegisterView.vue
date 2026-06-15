<template>
  <div class="auth-page">
    <div class="auth-card">
      <div class="auth-header">
        <div class="auth-logo">
          <div class="logo-icon">
            <svg width="28" height="28" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <rect x="3" y="3" width="7" height="7" rx="1" />
              <rect x="14" y="3" width="7" height="7" rx="1" />
              <rect x="3" y="14" width="7" height="7" rx="1" />
              <rect x="14" y="14" width="7" height="7" rx="1" />
            </svg>
          </div>
        </div>
        <h1>Tạo tài khoản</h1>
        <p class="auth-subtitle">Đăng ký tài khoản mới để bắt đầu quản lý dự án.</p>
      </div>

      <form @submit.prevent="handleRegister" class="auth-form">
        <div v-if="authStore.error" class="alert alert-error">
          {{ authStore.error }}
        </div>

        <div v-if="success" class="alert alert-success">
          Đăng ký thành công! <router-link to="/login">Đăng nhập ngay</router-link>
        </div>

        <BaseInput
          v-model="form.fullName"
          label="Họ và tên"
          placeholder="Nguyễn Văn A"
          required
          id="input-register-fullname"
        />

        <BaseInput
          v-model="form.username"
          label="Tên đăng nhập"
          placeholder="username"
          required
          id="input-register-username"
        />

        <BaseInput
          v-model="form.email"
          label="Email"
          type="email"
          placeholder="email@example.com"
          required
          id="input-register-email"
        />

        <BaseInput
          v-model="form.password"
          label="Mật khẩu"
          type="password"
          placeholder="Tối thiểu 6 ký tự"
          required
          id="input-register-password"
        />

        <BaseButton
          type="submit"
          variant="primary"
          block
          :loading="authStore.loading"
          size="lg"
        >
          Đăng ký
        </BaseButton>
      </form>

      <div class="auth-footer">
        <p>Đã có tài khoản? <router-link to="/login">Đăng nhập</router-link></p>
      </div>
    </div>
  </div>
</template>

<script setup>
import { reactive, ref } from 'vue'
import { useAuthStore } from '../stores/auth'
import BaseInput from '../components/common/BaseInput.vue'
import BaseButton from '../components/common/BaseButton.vue'

const authStore = useAuthStore()
const success = ref(false)

const form = reactive({
  fullName: '',
  username: '',
  email: '',
  password: ''
})

async function handleRegister() {
  success.value = false
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
  align-items: center;
  justify-content: center;
  background-color: var(--bg-primary);
  padding: var(--space-4);
}

.auth-card {
  width: 100%;
  max-width: 420px;
  background: var(--bg-card);
  backdrop-filter: var(--glass-blur);
  -webkit-backdrop-filter: var(--glass-blur);
  border-radius: var(--radius-xl);
  border: 1px solid var(--border-color);
  box-shadow: var(--shadow-xl);
  padding: var(--space-8);
  animation: slideUp 0.4s cubic-bezier(0.16, 1, 0.3, 1);
}

@keyframes slideUp {
  from { opacity: 0; transform: translateY(20px); }
  to { opacity: 1; transform: translateY(0); }
}

.auth-header {
  text-align: center;
  margin-bottom: var(--space-8);
}

.auth-logo {
  display: flex;
  justify-content: center;
  margin-bottom: var(--space-5);
}

.logo-icon {
  width: 56px;
  height: 56px;
  background: var(--gradient-primary);
  border-radius: var(--radius-xl);
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  box-shadow: var(--shadow-md), var(--gradient-glow);
}

.auth-header h1 {
  font-size: var(--font-size-2xl);
  margin-bottom: var(--space-2);
  background: var(--gradient-primary);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
}

.auth-subtitle {
  color: var(--text-secondary);
  font-size: var(--font-size-sm);
  font-weight: var(--font-weight-medium);
}

.auth-form {
  margin-bottom: var(--space-6);
}

.alert {
  padding: var(--space-3) var(--space-4);
  border-radius: var(--radius-md);
  font-size: var(--font-size-sm);
  margin-bottom: var(--space-4);
  font-weight: var(--font-weight-medium);
}

.alert-error {
  background: var(--color-danger-light);
  color: var(--color-danger);
  border: 1px solid rgba(244, 63, 94, 0.2);
}

.alert-success {
  background: var(--color-success-light);
  color: var(--color-success);
  border: 1px solid rgba(16, 185, 129, 0.2);
}

.alert-success a {
  color: var(--color-success);
  font-weight: var(--font-weight-bold);
  text-decoration: underline;
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
  color: var(--color-primary);
  transition: all var(--transition-fast);
}

.auth-footer a:hover {
  color: var(--color-primary-hover);
  text-shadow: var(--gradient-glow);
}
</style>
