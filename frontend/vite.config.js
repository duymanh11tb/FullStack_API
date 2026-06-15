import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'

// https://vite.dev/config/
export default defineConfig({
  plugins: [vue()],
  server: {
    proxy: {
      '/proxy-project': {
        target: 'http://103.178.235.78:5001',
        changeOrigin: true,
        rewrite: (path) => path.replace(/^\/proxy-project/, '')
      },
      '/proxy-task': {
        target: 'http://103.178.235.78:5002',
        changeOrigin: true,
        rewrite: (path) => path.replace(/^\/proxy-task/, '')
      },
      '/proxy-notify': {
        target: 'http://103.178.235.78:5004',
        changeOrigin: true,
        rewrite: (path) => path.replace(/^\/proxy-notify/, '')
      }
    }
  }
})
