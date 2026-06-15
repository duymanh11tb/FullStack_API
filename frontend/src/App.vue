<template>
  <component :is="layout">
    <router-view v-slot="{ Component }">
      <transition name="fade" mode="out-in">
        <component :is="Component" />
      </transition>
    </router-view>
  </component>
</template>

<script setup>
import { computed } from 'vue'
import { useRoute } from 'vue-router'
import AppLayout from './components/layout/AppLayout.vue'

const route = useRoute()

const layout = computed(() => {
  if (route.meta.guest) return 'div'
  return AppLayout
})
</script>

<style>
#app {
  min-height: 100vh;
}
</style>
