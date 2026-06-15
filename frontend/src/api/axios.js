import axios from 'axios'

// ── API 1: Project & Member Service ──
export const projectAPI = axios.create({
  baseURL: import.meta.env.VITE_PROJECT_API_URL,
  headers: { 'Content-Type': 'application/json' },
  timeout: 10000 // 10 seconds timeout
})

// ── API 2: Comment & Notify Service ──
export const notifyAPI = axios.create({
  baseURL: import.meta.env.VITE_NOTIFY_API_URL,
  headers: { 'Content-Type': 'application/json' },
  timeout: 10000
})

// ── API 3: Task Service ──
export const taskAPI = axios.create({
  baseURL: import.meta.env.VITE_TASK_API_URL,
  headers: { 'Content-Type': 'application/json' },
  timeout: 10000
})

// ── Request interceptor: attach JWT token ──
function attachToken(config) {
  const token = localStorage.getItem('token')
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
}

projectAPI.interceptors.request.use(attachToken, Promise.reject)
notifyAPI.interceptors.request.use(attachToken, Promise.reject)
taskAPI.interceptors.request.use(attachToken, Promise.reject)

// ── Response interceptor: handle errors ──
function handleError(error) {
  // Xử lý lỗi mạng / từ chối kết nối
  if (error.code === 'ERR_NETWORK' || error.message === 'Network Error') {
    console.error('⚠️ Không thể kết nối tới API:', error.config?.baseURL || error.config?.url);
    // Loại bỏ alert() để không chặn giao diện (blocking UI) khi một API bị lỗi
  }

  // Xử lý lỗi 401 Không có quyền truy cập
  if (error.response && error.response.status === 401) {
    localStorage.removeItem('token')
    localStorage.removeItem('user')
    if (window.location.pathname !== '/login' && window.location.pathname !== '/register') {
      window.location.href = '/login'
    }
  }
  return Promise.reject(error)
}

projectAPI.interceptors.response.use(res => res, handleError)
notifyAPI.interceptors.response.use(res => res, handleError)
taskAPI.interceptors.response.use(res => res, handleError)
