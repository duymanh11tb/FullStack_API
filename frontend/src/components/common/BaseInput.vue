<template>
  <div class="form-group" :class="{ 'has-error': error }">
    <label v-if="label" :for="id" class="form-label">
      {{ label }}
      <span v-if="required" class="required">*</span>
    </label>
    <div class="input-wrapper">
      <span v-if="$slots.start" class="icon-start">
        <slot name="start" />
      </span>
      <input
        v-if="type !== 'textarea'"
        :id="id"
        :type="inputType"
        :value="modelValue"
        :placeholder="placeholder"
        :disabled="disabled"
        :required="required"
        :class="['form-input', { 'has-icon-start': $slots.start, 'has-icon-end': type === 'password' || $slots.end }]"
        @input="$emit('update:modelValue', $event.target.value)"
        @focus="$emit('focus', $event)"
        @blur="$emit('blur', $event)"
      />
      <textarea
        v-else
        :id="id"
        :value="modelValue"
        :placeholder="placeholder"
        :disabled="disabled"
        :required="required"
        :rows="rows"
        :class="['form-input', 'form-textarea', { 'has-icon-start': $slots.start }]"
        @input="$emit('update:modelValue', $event.target.value)"
        @focus="$emit('focus', $event)"
        @blur="$emit('blur', $event)"
      ></textarea>
      
      <!-- Password toggle -->
      <button 
        v-if="type === 'password'" 
        type="button" 
        class="icon-end toggle-password" 
        @click="isPasswordVisible = !isPasswordVisible"
        tabindex="-1"
      >
        <svg v-if="isPasswordVisible" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
          <path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z" />
          <circle cx="12" cy="12" r="3" />
        </svg>
        <svg v-else width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
          <path d="M17.94 17.94A10.07 10.07 0 0 1 12 20c-7 0-11-8-11-8a18.45 18.45 0 0 1 5.06-5.94M9.9 4.24A9.12 9.12 0 0 1 12 4c7 0 11 8 11 8a18.5 18.5 0 0 1-2.16 3.19m-6.72-1.07a3 3 0 1 1-4.24-4.24" />
          <line x1="1" y1="1" x2="23" y2="23" />
        </svg>
      </button>
      <span v-else-if="$slots.end" class="icon-end">
        <slot name="end" />
      </span>
    </div>
    <p v-if="error" class="form-error">{{ error }}</p>
    <p v-if="hint && !error" class="form-hint">{{ hint }}</p>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'

const props = defineProps({
  modelValue: [String, Number],
  label: String,
  type: { type: String, default: 'text' },
  placeholder: String,
  error: String,
  hint: String,
  disabled: Boolean,
  required: Boolean,
  rows: { type: Number, default: 3 },
  id: String
})

defineEmits(['update:modelValue', 'focus', 'blur'])

const isPasswordVisible = ref(false)

const inputType = computed(() => {
  if (props.type === 'password') {
    return isPasswordVisible.value ? 'text' : 'password'
  }
  return props.type
})
</script>

<style scoped>
.form-group {
  margin-bottom: var(--space-4);
  width: 100%;
}

.form-label {
  display: block;
  font-size: var(--font-size-sm);
  font-weight: var(--font-weight-medium);
  color: var(--color-text-primary);
  margin-bottom: var(--space-1);
}

.required {
  color: var(--color-danger);
}

.input-wrapper {
  position: relative;
  display: flex;
  align-items: center;
  width: 100%;
}

.form-input {
  width: 100%;
  padding: 10px 14px;
  background: var(--bg-white-to-card);
  border: 1px solid var(--color-border);
  border-radius: var(--radius-md);
  font-size: var(--font-size-base);
  color: var(--color-text-primary);
  transition: all var(--transition-fast);
  outline: none;
}

.form-input::placeholder {
  color: var(--color-text-tertiary);
  opacity: 0.7;
}

.form-input:focus {
  border-color: var(--accent-primary);
  box-shadow: 0 0 0 3px rgba(37, 99, 235, 0.15);
}

[data-theme='dark'] .form-input:focus {
  box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.25);
}

.form-input:disabled {
  background: var(--color-bg-secondary);
  cursor: not-allowed;
  opacity: 0.6;
}

.has-icon-start {
  padding-left: 40px;
}

.has-icon-end {
  padding-right: 40px;
}

.icon-start {
  position: absolute;
  left: 14px;
  display: flex;
  align-items: center;
  justify-content: center;
  color: var(--text-muted);
}

.icon-end {
  position: absolute;
  right: 14px;
  display: flex;
  align-items: center;
  justify-content: center;
  color: var(--text-muted);
  background: none;
  border: none;
  cursor: pointer;
  padding: 0;
}

.toggle-password {
  transition: color var(--transition-fast);
}

.toggle-password:hover {
  color: var(--color-text-primary);
}

.has-error .form-input {
  border-color: var(--color-danger);
}

.has-error .form-input:focus {
  box-shadow: 0 0 0 3px rgba(225, 29, 72, 0.15);
}

.form-textarea {
  resize: vertical;
  min-height: 80px;
}

.form-error {
  font-size: var(--font-size-xs);
  color: var(--color-danger);
  margin-top: var(--space-1);
  font-weight: 500;
}

.form-hint {
  font-size: var(--font-size-xs);
  color: var(--color-text-tertiary);
  margin-top: var(--space-1);
}
</style>
