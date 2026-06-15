<template>
  <div class="admin-view">
    <header class="page-header">
      <div class="header-content">
        <h1 class="page-title">Quản trị viên</h1>
        <p class="page-subtitle">Quản lý hệ thống và người dùng</p>
      </div>
    </header>

    <div class="admin-content">
      <div class="card">
        <div class="card-header">
          <h2>Tổng quan hệ thống</h2>
        </div>
        <div class="card-body">
          <p>Chào mừng bạn đến với trang quản trị. Tính năng quản lý người dùng và hệ thống sẽ được phát triển tại đây.</p>
          <div class="stats-grid">
            <div class="stat-card">
              <div class="stat-title">Người dùng</div>
              <div class="stat-value">--</div>
            </div>
            <div class="stat-card">
              <div class="stat-title">Dự án</div>
              <div class="stat-value">--</div>
            </div>
            <div class="stat-card">
              <div class="stat-title">Nhiệm vụ</div>
              <div class="stat-value">--</div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
// Admin view logic will be implemented here
</script>

<style scoped>
.admin-view {
  padding: var(--space-6);
  height: 100%;
  overflow-y: auto;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: var(--space-6);
}

.page-title {
  font-size: var(--font-size-2xl);
  font-weight: var(--font-weight-bold);
  color: var(--text-primary);
  margin-bottom: var(--space-2);
}

.page-subtitle {
  color: var(--text-secondary);
  font-size: var(--font-size-sm);
}

.card {
  background: var(--bg-card);
  backdrop-filter: var(--glass-blur);
  -webkit-backdrop-filter: var(--glass-blur);
  border-radius: var(--radius-xl);
  border: 1px solid var(--border-color);
  overflow: hidden;
  box-shadow: var(--shadow-sm);
}

.card-header {
  padding: var(--space-5) var(--space-6);
  border-bottom: 1px solid var(--border-color);
  background: rgba(255, 255, 255, 0.02);
}

.card-header h2 {
  font-size: var(--font-size-lg);
  font-weight: var(--font-weight-bold);
  margin: 0;
}

.card-body {
  padding: var(--space-6);
}

.stats-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(220px, 1fr));
  gap: var(--space-5);
  margin-top: var(--space-4);
}

.stat-card {
  background: var(--bg-secondary);
  padding: var(--space-5);
  border-radius: var(--radius-lg);
  border: 1px solid var(--border-color);
  transition: all var(--transition-slow);
  box-shadow: var(--shadow-sm);
}

.stat-card:hover {
  box-shadow: var(--shadow-md);
  transform: translateY(-4px);
  border-color: rgba(99, 102, 241, 0.3);
}

.stat-title {
  color: var(--text-secondary);
  font-size: var(--font-size-sm);
  margin-bottom: var(--space-2);
  font-weight: var(--font-weight-medium);
}

.stat-value {
  font-size: var(--font-size-2xl);
  font-weight: var(--font-weight-bold);
  background: var(--gradient-primary);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
}
</style>
