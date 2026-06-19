<template>
  <button
    :class="['btn', `btn-${variant}`, `btn-${size}`, { 'btn-loading': loading, 'btn-block': block }]"
    :disabled="disabled || loading"
    :type="type"
    @click="$emit('click', $event)"
  >
    <span v-if="loading" class="btn-spinner"></span>
    <slot />
  </button>
</template>

<script setup>
defineProps({
  variant: { type: String, default: 'primary' },
  size: { type: String, default: 'md' },
  loading: Boolean,
  disabled: Boolean,
  block: Boolean,
  type: { type: String, default: 'button' }
})

defineEmits(['click'])
</script>

<style scoped>
.btn {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  gap: var(--space-2);
  font-weight: var(--font-weight-semibold);
  border-radius: var(--radius-md);
  transition: all var(--transition-fast);
  white-space: nowrap;
  cursor: pointer;
  border: 1px solid transparent;
  line-height: 1;
  box-shadow: var(--shadow-xs);
}

.btn:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: var(--shadow-md);
}

.btn:active:not(:disabled) {
  transform: translateY(0);
  box-shadow: var(--shadow-xs);
}

.btn:disabled {
  opacity: 0.55;
  cursor: not-allowed;
  box-shadow: none;
}

/* Sizes */
.btn-sm { padding: 8px 14px; font-size: var(--font-size-sm); }
.btn-md { padding: 10px 18px; font-size: var(--font-size-base); }
.btn-lg { padding: 12px 24px; font-size: var(--font-size-md); }

/* Variants */
.btn-primary {
  background: var(--gradient-primary);
  color: white;
  box-shadow: 0 4px 14px rgba(37, 99, 235, 0.25);
  border: none;
}
.btn-primary:hover:not(:disabled) {
  background: var(--gradient-primary-hover);
  box-shadow: var(--gradient-glow), 0 6px 20px rgba(37, 99, 235, 0.35);
}

.btn-secondary {
  background: var(--bg-white-to-card);
  color: var(--color-text-primary);
  border-color: var(--color-border);
  box-shadow: var(--shadow-xs);
}
.btn-secondary:hover:not(:disabled) {
  background: var(--color-bg-secondary);
  border-color: var(--color-border-hover);
}

.btn-danger {
  background: var(--color-danger);
  color: white;
  box-shadow: 0 4px 12px rgba(225, 29, 72, 0.2);
}
.btn-danger:hover:not(:disabled) {
  background: #be123c;
  box-shadow: 0 6px 18px rgba(225, 29, 72, 0.3);
}

.btn-ghost {
  background: transparent;
  color: var(--color-text-secondary);
  box-shadow: none;
}
.btn-ghost:hover:not(:disabled) {
  background: var(--color-bg-secondary);
  color: var(--color-text-primary);
}

.btn-outline {
  background: transparent;
  color: var(--accent-primary);
  border-color: var(--accent-primary);
  box-shadow: none;
}
.btn-outline:hover:not(:disabled) {
  background: var(--color-primary-subtle);
}

.btn-block {
  width: 100%;
}

/* Loading spinner */
.btn-loading {
  position: relative;
  color: transparent !important;
}

.btn-spinner {
  position: absolute;
  width: 18px;
  height: 18px;
  border: 2px solid rgba(255,255,255,0.35);
  border-top-color: white;
  border-radius: 50%;
  animation: spin 0.6s linear infinite;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}
</style>
