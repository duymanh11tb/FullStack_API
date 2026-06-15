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
        <h1>Đăng nhập</h1>
        <p class="auth-subtitle">Chào mừng trở lại! Vui lòng đăng nhập để tiếp tục.</p>
      </div>

      <form @submit.prevent="handleLogin" class="auth-form">
        <div v-if="authStore.error" class="alert alert-error">
          {{ authStore.error }}
        </div>

        <BaseInput
          v-model="form.usernameOrEmail"
          label="Tên đăng nhập hoặc Email"
          placeholder="Nhập tên đăng nhập hoặc email"
          required
          id="input-login-username"
        />

        <BaseInput
          v-model="form.password"
          label="Mật khẩu"
          type="password"
          placeholder="Nhập mật khẩu"
          required
          id="input-login-password"
        />

        <BaseButton
          type="submit"
          variant="primary"
          block
          :loading="authStore.loading"
          size="lg"
        >
          Đăng nhập
        </BaseButton>
      </form>

      <div class="auth-footer">
        <p>Chưa có tài khoản? <router-link to="/register">Đăng ký ngay</router-link></p>
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
    router.push('/')
  }
}
</script>

<style scoped>
.auth-page {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  background: var(--color-bg);
  padding: var(--space-4);
}

.auth-card {
  width: 100%;
  max-width: 420px;
  background: var(--color-white);
  border-radius: var(--radius-xl);
  border: 1px solid var(--color-border);
  box-shadow: var(--shadow-lg);
  padding: var(--space-8);
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
  width: 52px;
  height: 52px;
  background: var(--color-primary);
  border-radius: var(--radius-xl);
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
}

.auth-header h1 {
  font-size: var(--font-size-2xl);
  margin-bottom: var(--space-2);
}

.auth-subtitle {
  color: var(--color-text-secondary);
  font-size: var(--font-size-sm);
}

.auth-form {
  margin-bottom: var(--space-6);
}

.alert {
  padding: var(--space-3) var(--space-4);
  border-radius: var(--radius-md);
  font-size: var(--font-size-sm);
  margin-bottom: var(--space-4);
}

.alert-error {
  background: var(--color-danger-light);
  color: var(--color-danger);
  border: 1px solid rgba(220, 38, 38, 0.2);
}

.auth-footer {
  text-align: center;
  padding-top: var(--space-4);
  border-top: 1px solid var(--color-border);
}

.auth-footer p {
  font-size: var(--font-size-sm);
  color: var(--color-text-secondary);
}

.auth-footer a {
  font-weight: var(--font-weight-medium);
}
</style>
