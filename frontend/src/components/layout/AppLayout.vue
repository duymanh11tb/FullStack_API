<template>
  <div class="app-layout">
    <!-- Floating 3D Backdrop Blobs -->
    <div class="blob-container">
      <div class="blob bg-blob-1"></div>
      <div class="blob bg-blob-2"></div>
      <div class="blob bg-blob-3"></div>
    </div>

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
  margin-left: calc(var(--sidebar-width) + 32px);
  width: calc(100% - var(--sidebar-width) - 32px);
  display: flex;
  flex-direction: column;
  min-height: 100vh;
}

@media (max-width: 768px) {
  .main-wrapper {
    margin-left: 0;
    width: 100%;
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
  z-index: 90;
}

/* Background Blobs styling */
.blob-container {
  position: fixed;
  inset: 0;
  overflow: hidden;
  z-index: -1;
  pointer-events: none;
}

.blob {
  position: absolute;
  border-radius: 50%;
  filter: blur(120px);
  opacity: 0.08;
  mix-blend-mode: multiply;
}

[data-theme='dark'] .blob {
  opacity: 0.12;
  mix-blend-mode: screen;
}

.blob.bg-blob-1 {
  top: 10%;
  left: 20%;
  width: 450px;
  height: 450px;
  background: radial-gradient(circle, #6366f1, #3b82f6);
}

.blob.bg-blob-2 {
  bottom: 10%;
  right: 15%;
  width: 500px;
  height: 500px;
  background: radial-gradient(circle, #a855f7, #ec4899);
}

.blob.bg-blob-3 {
  top: 45%;
  left: 55%;
  width: 350px;
  height: 350px;
  background: radial-gradient(circle, #0ea5e9, #10b981);
}
</style>
