<template>
  <div v-if="totalPages > 1" class="pagination-container">
    <!-- Prev Button -->
    <button
      class="pagination-btn arrow-btn"
      :disabled="currentPage === 1"
      @click="changePage(currentPage - 1)"
      title="Trang trước"
    >
      <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
        <polyline points="15 18 9 12 15 6" />
      </svg>
    </button>

    <!-- Page List -->
    <div class="pages-list">
      <span v-for="(page, index) in pages" :key="index" class="page-item-wrapper">
        <span v-if="page === '...'" class="pagination-ellipsis">...</span>
        <button
          v-else
          class="pagination-btn number-btn"
          :class="{ active: currentPage === page }"
          @click="changePage(page)"
        >
          {{ page }}
        </button>
      </span>
    </div>

    <!-- Next Button -->
    <button
      class="pagination-btn arrow-btn"
      :disabled="currentPage === totalPages"
      @click="changePage(currentPage + 1)"
      title="Trang sau"
    >
      <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
        <polyline points="9 18 15 12 9 6" />
      </svg>
    </button>
  </div>
</template>

<script setup>
import { computed } from 'vue'

const props = defineProps({
  currentPage: {
    type: Number,
    required: true
  },
  totalItems: {
    type: Number,
    required: true
  },
  itemsPerPage: {
    type: Number,
    default: 10
  }
})

const emit = defineEmits(['update:currentPage', 'change'])

const totalPages = computed(() => {
  return Math.max(1, Math.ceil(props.totalItems / props.itemsPerPage))
})

const pages = computed(() => {
  const range = []
  if (totalPages.value <= 6) {
    for (let i = 1; i <= totalPages.value; i++) {
      range.push(i)
    }
  } else {
    // Under 6 pages, calculate dynamic display with ellipses
    range.push(1)
    
    const start = Math.max(2, props.currentPage - 1)
    const end = Math.min(totalPages.value - 1, props.currentPage + 1)
    
    if (start > 2) {
      range.push('...')
    }
    
    for (let i = start; i <= end; i++) {
      range.push(i)
    }
    
    if (end < totalPages.value - 1) {
      range.push('...')
    }
    
    range.push(totalPages.value)
  }
  return range
})

function changePage(page) {
  if (page >= 1 && page <= totalPages.value && page !== props.currentPage) {
    emit('update:currentPage', page)
    emit('change', page)
  }
}
</script>

<style scoped>
.pagination-container {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: var(--space-3);
  margin-top: var(--space-8);
  margin-bottom: var(--space-4);
  animation: slideUpFade 0.4s ease-out;
}

@keyframes slideUpFade {
  from {
    opacity: 0;
    transform: translateY(8px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.pages-list {
  display: flex;
  align-items: center;
  gap: var(--space-2);
}

.pagination-btn {
  width: 38px;
  height: 38px;
  border-radius: var(--radius-md);
  background: var(--bg-secondary);
  border: 1px solid var(--border-color);
  color: var(--text-secondary);
  display: inline-flex;
  align-items: center;
  justify-content: center;
  font-size: var(--font-size-sm);
  font-weight: var(--font-weight-semibold);
  transition: all var(--transition-fast);
  box-shadow: var(--shadow-xs);
  cursor: pointer;
}

[data-theme='dark'] .pagination-btn {
  background: rgba(15, 23, 42, 0.4);
}

.pagination-btn:hover:not(:disabled) {
  border-color: rgba(99, 102, 241, 0.35);
  background: var(--color-primary-subtle);
  color: var(--accent-primary);
  transform: translateY(-1px);
  box-shadow: var(--shadow-sm);
}

.pagination-btn:disabled {
  opacity: 0.4;
  cursor: not-allowed;
  box-shadow: none;
}

.pagination-btn.active {
  background: var(--gradient-primary);
  color: white;
  border-color: transparent;
  box-shadow: 0 4px 12px rgba(37, 99, 235, 0.28);
  transform: scale(1.04);
}

.arrow-btn svg {
  transition: transform var(--transition-fast);
}

.arrow-btn:hover:not(:disabled) svg {
  color: var(--accent-primary);
}

.arrow-btn:first-child:hover:not(:disabled) svg {
  transform: translateX(-2px);
}

.arrow-btn:last-child:hover:not(:disabled) svg {
  transform: translateX(2px);
}

.pagination-ellipsis {
  padding: 0 var(--space-1);
  color: var(--text-muted);
  font-weight: var(--font-weight-bold);
  font-size: var(--font-size-sm);
}
</style>
