<template>
  <span :class="['status-badge', `status-${variant}`]">
    <span class="status-dot"></span>
    {{ label }}
  </span>
</template>

<script setup>
import { computed } from 'vue'

const props = defineProps({
  status: { type: String, default: '' }
})

const statusMap = {
  'Active': { label: 'Hoạt động', variant: 'success' },
  'Planning': { label: 'Lên kế hoạch', variant: 'info' },
  'Completed': { label: 'Hoàn thành', variant: 'primary' },
  'Cancelled': { label: 'Đã hủy', variant: 'danger' },
  'OnHold': { label: 'Tạm dừng', variant: 'warning' },
  'On Hold': { label: 'Tạm dừng', variant: 'warning' }
}

const mapped = computed(() => statusMap[props.status] || { label: props.status, variant: 'default' })
const label = computed(() => mapped.value.label)
const variant = computed(() => mapped.value.variant)
</script>

<style scoped>
.status-badge {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  padding: 3px 10px;
  border-radius: var(--radius-full);
  font-size: var(--font-size-xs);
  font-weight: var(--font-weight-medium);
  white-space: nowrap;
}

.status-dot {
  width: 6px;
  height: 6px;
  border-radius: 50%;
}

.status-success { background: var(--color-success-light); color: var(--color-success); }
.status-success .status-dot { background: var(--color-success); }

.status-info { background: var(--color-info-light); color: var(--color-info); }
.status-info .status-dot { background: var(--color-info); }

.status-warning { background: var(--color-warning-light); color: var(--color-warning); }
.status-warning .status-dot { background: var(--color-warning); }

.status-danger { background: var(--color-danger-light); color: var(--color-danger); }
.status-danger .status-dot { background: var(--color-danger); }

.status-primary { background: var(--color-primary-light); color: var(--color-primary); }
.status-primary .status-dot { background: var(--color-primary); }

.status-default { background: var(--color-bg-secondary); color: var(--color-text-secondary); }
.status-default .status-dot { background: var(--color-text-tertiary); }
</style>
