<template>
  <div class="skeleton-container" :class="[type, { 'animate-shimmer': animate }]">
    <!-- BOARD SKELETON -->
    <template v-if="type === 'board'">
      <div class="skeleton-board-col" v-for="c in count" :key="c">
        <div class="skeleton-header">
          <div class="skeleton-bone title"></div>
          <div class="skeleton-bone count-badge"></div>
        </div>
        <div class="skeleton-body">
          <div class="skeleton-card-item" v-for="i in 3" :key="i">
            <div class="skeleton-bone line short"></div>
            <div class="skeleton-bone line long"></div>
            <div class="skeleton-bone line medium"></div>
            <div class="skeleton-footer">
              <div class="skeleton-bone circle"></div>
              <div class="skeleton-bone pill"></div>
            </div>
          </div>
        </div>
      </div>
    </template>

    <!-- CARD SKELETON -->
    <template v-else-if="type === 'card'">
      <div class="skeleton-card-grid">
        <div class="skeleton-card-item" v-for="i in count" :key="i">
          <div class="skeleton-header">
            <div class="skeleton-bone square"></div>
            <div class="skeleton-bone pill"></div>
          </div>
          <div class="skeleton-bone line short"></div>
          <div class="skeleton-bone line long"></div>
          <div class="skeleton-footer mt-4">
            <div class="skeleton-bone line medium"></div>
            <div class="skeleton-bone circle"></div>
          </div>
        </div>
      </div>
    </template>

    <!-- TABLE SKELETON -->
    <template v-else-if="type === 'table'">
      <div class="skeleton-table">
        <div class="skeleton-table-row header">
          <div class="skeleton-bone cell" v-for="c in 4" :key="c"></div>
        </div>
        <div class="skeleton-table-row" v-for="i in count" :key="i">
          <div class="skeleton-bone cell flex-row">
            <div class="skeleton-bone circle"></div>
            <div class="skeleton-bone line short"></div>
          </div>
          <div class="skeleton-bone cell"></div>
          <div class="skeleton-bone cell"></div>
          <div class="skeleton-bone cell"></div>
        </div>
      </div>
    </template>

    <!-- DEFAULT TEXT SKELETON -->
    <template v-else>
      <div class="skeleton-text-wrapper" v-for="i in count" :key="i">
        <div class="skeleton-bone line short"></div>
        <div class="skeleton-bone line long"></div>
        <div class="skeleton-bone line medium"></div>
      </div>
    </template>
  </div>
</template>

<script setup>
defineProps({
  type: {
    type: String,
    default: 'card' // 'card' | 'board' | 'table' | 'text'
  },
  count: {
    type: Number,
    default: 3
  },
  animate: {
    type: Boolean,
    default: true
  }
})
</script>

<style scoped>
.skeleton-container {
  width: 100%;
}

.skeleton-bone {
  background: var(--color-bg-secondary);
  border-radius: var(--radius-sm);
  position: relative;
  overflow: hidden;
}

[data-theme='dark'] .skeleton-bone {
  background: rgba(255, 255, 255, 0.05);
}

/* Shimmer overlay effect */
.animate-shimmer .skeleton-bone::after {
  content: '';
  position: absolute;
  top: 0;
  right: 0;
  bottom: 0;
  left: 0;
  transform: translateX(-100%);
  background: linear-gradient(
    90deg,
    transparent 0%,
    rgba(255, 255, 255, 0.06) 20%,
    rgba(255, 255, 255, 0.12) 60%,
    transparent 100%
  );
  animation: shimmer 1.6s infinite;
}

[data-theme='dark'] .animate-shimmer .skeleton-bone::after {
  background: linear-gradient(
    90deg,
    transparent 0%,
    rgba(255, 255, 255, 0.03) 20%,
    rgba(255, 255, 255, 0.06) 60%,
    transparent 100%
  );
}

@keyframes shimmer {
  100% {
    transform: translateX(100%);
  }
}

/* Sizing units */
.line {
  height: 12px;
  margin-bottom: var(--space-2);
}
.line.short { width: 40%; }
.line.medium { width: 65%; }
.line.long { width: 90%; }

.circle {
  width: 32px;
  height: 32px;
  border-radius: var(--radius-full);
}

.square {
  width: 42px;
  height: 42px;
  border-radius: var(--radius-md);
}

.pill {
  width: 70px;
  height: 20px;
  border-radius: var(--radius-full);
}

.title {
  width: 120px;
  height: 18px;
}

.count-badge {
  width: 24px;
  height: 20px;
  border-radius: var(--radius-full);
}

/* Board skeleton styling */
.board {
  display: flex;
  gap: 20px;
  width: 100%;
}

.skeleton-board-col {
  flex: 1;
  min-width: 290px;
  background: var(--bg-white-to-card);
  border-radius: var(--radius-xl);
  border: 1px solid var(--border-color);
  padding: 16px;
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.skeleton-body {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.skeleton-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.skeleton-footer {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-top: auto;
}

/* Card grid skeleton styling */
.skeleton-card-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(320px, 1fr));
  gap: var(--space-5);
  width: 100%;
}

.skeleton-card-item {
  background: var(--bg-white-to-card);
  border: 1px solid var(--border-color);
  border-radius: var(--radius-xl);
  padding: var(--space-5);
  display: flex;
  flex-direction: column;
  gap: 12px;
  box-shadow: var(--shadow-sm);
}

/* Table skeleton styling */
.skeleton-table {
  width: 100%;
  border: 1px solid var(--border-color);
  border-radius: var(--radius-xl);
  overflow: hidden;
  background: var(--bg-white-to-card);
}

.skeleton-table-row {
  display: grid;
  grid-template-columns: 2fr 1.5fr 1fr 1fr;
  padding: var(--space-4);
  border-bottom: 1px solid var(--border-color);
  align-items: center;
  gap: 16px;
}

.skeleton-table-row.header {
  background: var(--color-bg-secondary);
}

[data-theme='dark'] .skeleton-table-row.header {
  background: rgba(255, 255, 255, 0.02);
}

.cell {
  height: 14px;
}

.cell.flex-row {
  display: flex;
  align-items: center;
  gap: 12px;
  height: auto;
}

.mt-4 { margin-top: 16px; }
</style>
