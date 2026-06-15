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
  font-weight: var(--font-weight-medium);
  border-radius: var(--radius-md);
  transition: all var(--transition-fast);
  white-space: nowrap;
  cursor: pointer;
  border: 1px solid transparent;
  line-height: 1;
  box-shadow: var(--shadow-xs);
}

.btn:hover:not(:disabled) {
  transform: translateY(-1px);
  box-shadow: var(--shadow-sm);
}

.btn:active:not(:disabled) {
  transform: translateY(0);
  box-shadow: none;
}

.btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
  box-shadow: none;
}

/* Sizes */
.btn-sm { padding: 6px 12px; font-size: var(--font-size-sm); }
.btn-md { padding: 8px 16px; font-size: var(--font-size-base); }
.btn-lg { padding: 10px 20px; font-size: var(--font-size-md); }

/* Variants */
.btn-primary {
  background: var(--color-primary);
  color: white;
  box-shadow: 0 1px 2px rgba(37, 99, 235, 0.2);
}
.btn-primary:hover:not(:disabled) {
  background: var(--color-primary-hover);
  box-shadow: 0 4px 6px -1px rgba(37, 99, 235, 0.25), 0 2px 4px -2px rgba(37, 99, 235, 0.1);
}

.btn-secondary {
  background: var(--color-white);
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
  box-shadow: 0 1px 2px rgba(220, 38, 38, 0.2);
}
.btn-danger:hover:not(:disabled) {
  background: #B91C1C;
  box-shadow: 0 4px 6px -1px rgba(220, 38, 38, 0.25), 0 2px 4px -2px rgba(220, 38, 38, 0.1);
}

.btn-ghost {
  background: transparent;
  color: var(--color-text-secondary);
}
.btn-ghost:hover:not(:disabled) {
  background: var(--color-bg-secondary);
  color: var(--color-text-primary);
}

.btn-outline {
  background: transparent;
  color: var(--color-primary);
  border-color: var(--color-primary);
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
  width: 16px;
  height: 16px;
  border: 2px solid rgba(255,255,255,0.3);
  border-top-color: white;
  border-radius: 50%;
  animation: spin 0.6s linear infinite;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}
</style>
