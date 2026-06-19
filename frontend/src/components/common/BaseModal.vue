<template>
  <teleport to="body">
    <transition name="fade">
      <div v-if="modelValue" class="modal-backdrop" @click.self="close">
        <transition name="slide">
          <div v-if="modelValue" class="modal" :class="`modal-${size}`">
            <div class="modal-header">
              <h3 class="modal-title">{{ title }}</h3>
              <button class="modal-close" @click="close" id="btn-modal-close">
                <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                  <line x1="18" y1="6" x2="6" y2="18" />
                  <line x1="6" y1="6" x2="18" y2="18" />
                </svg>
              </button>
            </div>
            <div class="modal-body">
              <slot />
            </div>
            <div v-if="$slots.footer" class="modal-footer">
              <slot name="footer" />
            </div>
          </div>
        </transition>
      </div>
    </transition>
  </teleport>
</template>

<script setup>
const props = defineProps({
  modelValue: Boolean,
  title: { type: String, default: '' },
  size: { type: String, default: 'md' }
})

const emit = defineEmits(['update:modelValue'])

function close() {
  emit('update:modelValue', false)
}
</script>

<style scoped>
.modal-backdrop {
  position: fixed;
  inset: 0;
  background: rgba(15, 23, 42, 0.5);
  backdrop-filter: blur(6px);
  -webkit-backdrop-filter: blur(6px);
  display: flex;
  align-items: flex-start;
  justify-content: center;
  padding: 10vh var(--space-4) var(--space-4);
  z-index: var(--z-modal-backdrop);
  overflow-y: auto;
}

.modal {
  background: var(--bg-white-to-card);
  border-radius: var(--radius-xl);
  border: 1px solid var(--border-color);
  box-shadow: var(--shadow-xl);
  width: 100%;
  max-height: 80vh;
  display: flex;
  flex-direction: column;
  z-index: var(--z-modal);
}

.modal-sm { max-width: 400px; }
.modal-md { max-width: 560px; }
.modal-lg { max-width: 720px; }

.modal-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: var(--space-5) var(--space-6);
  border-bottom: 1px solid var(--border-color);
}

.modal-title {
  font-size: var(--font-size-lg);
  font-weight: var(--font-weight-semibold);
  color: var(--color-text-primary);
}

.modal-close {
  padding: var(--space-1);
  border-radius: var(--radius-md);
  color: var(--color-text-tertiary);
  transition: all var(--transition-fast);
  display: flex;
  align-items: center;
  justify-content: center;
}

.modal-close:hover {
  background: var(--color-bg-secondary);
  color: var(--color-text-primary);
}

.modal-body {
  padding: var(--space-6);
  overflow-y: auto;
  flex: 1;
}

.modal-footer {
  padding: var(--space-4) var(--space-6);
  border-top: 1px solid var(--border-color);
  display: flex;
  justify-content: flex-end;
  gap: var(--space-3);
  background: rgba(0, 0, 0, 0.01);
}

/* Transitions */
.fade-enter-active, .fade-leave-active {
  transition: opacity 0.25s ease;
}

.fade-enter-from, .fade-leave-to {
  opacity: 0;
}

.slide-enter-active, .slide-leave-active {
  transition: transform 0.35s cubic-bezier(0.34, 1.56, 0.64, 1), opacity 0.3s ease;
}

.slide-enter-from, .slide-leave-to {
  transform: translateY(-24px) scale(0.96);
  opacity: 0;
}
</style>
