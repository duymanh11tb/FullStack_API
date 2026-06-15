<template>
  <div class="app-layout">
    <AppSidebar :collapsed="sidebarCollapsed" />
    <div class="main-wrapper">
      <AppHeader @toggle-sidebar="sidebarCollapsed = !sidebarCollapsed" />
      <main class="main-content">
        <slot />
      </main>
    </div>

    <!-- Mobile overlay -->
    <transition name="fade">
      <div
        v-if="!sidebarCollapsed && isMobile"
        class="sidebar-overlay"
        @click="sidebarCollapsed = true"
      ></div>
    </transition>
  </div>
</template>

<script setup>
import { ref, onMounted, onBeforeUnmount } from 'vue'
import AppHeader from './AppHeader.vue'
import AppSidebar from './AppSidebar.vue'

const sidebarCollapsed = ref(false)
const isMobile = ref(false)

function checkMobile() {
  isMobile.value = window.innerWidth <= 768
  if (isMobile.value) sidebarCollapsed.value = true
}

onMounted(() => {
  checkMobile()
  window.addEventListener('resize', checkMobile)
})

onBeforeUnmount(() => {
  window.removeEventListener('resize', checkMobile)
})
</script>

<style scoped>
.app-layout {
  display: flex;
  min-height: 100vh;
}

.main-wrapper {
  flex: 1;
  margin-left: var(--sidebar-width);
  display: flex;
  flex-direction: column;
  min-height: 100vh;
}

@media (max-width: 768px) {
  .main-wrapper {
    margin-left: 0;
  }
}

.main-content {
  flex: 1;
  padding: var(--space-6);
  max-width: 1400px;
  width: 100%;
}

@media (max-width: 768px) {
  .main-content {
    padding: var(--space-4);
  }
}

.sidebar-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.4);
  z-index: 55;
}
</style>
