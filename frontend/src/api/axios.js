import axios from 'axios'

// ── API 1: Project & Member Service ──
export const projectAPI = axios.create({
  baseURL: import.meta.env.VITE_PROJECT_API_URL,
  headers: { 'Content-Type': 'application/json' },
  timeout: 10000 // 10 seconds timeout
})

// ── API 2: Task Service ──
export const taskAPI = axios.create({
  baseURL: import.meta.env.VITE_TASK_API_URL,
  headers: { 'Content-Type': 'application/json' },
  timeout: 10000
})

// ── API 3: Comment & Notify Service ──
export const notifyAPI = axios.create({
  baseURL: import.meta.env.VITE_NOTIFY_API_URL,
  headers: { 'Content-Type': 'application/json' },
  timeout: 10000
})

// ── Request interceptor: attach JWT token and log request ──
function handleRequest(config) {
  const token = localStorage.getItem('token')
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  // Console log khi kết nối gửi request tới API
  console.log(`📡 [API Request] ${config.method.toUpperCase()} -> ${config.baseURL || ''}${config.url}`);
  return config
}

projectAPI.interceptors.request.use(handleRequest, Promise.reject)
taskAPI.interceptors.request.use(handleRequest, Promise.reject)
notifyAPI.interceptors.request.use(handleRequest, Promise.reject)

// ── Response logger ──
function handleResponse(response) {
  console.log(`📥 [API Response] ${response.status} <- ${response.config.baseURL || ''}${response.config.url}`);
  return response;
}

projectAPI.interceptors.response.use(handleResponse, (err) => handleError(err))
taskAPI.interceptors.response.use(handleResponse, (err) => handleError(err))
notifyAPI.interceptors.response.use(handleResponse, (err) => handleError(err))

let lastAlertTime = 0

// ── Response interceptor: handle errors ──
function handleError(error) {
  const isNetworkError = error.code === 'ERR_NETWORK' || error.message === 'Network Error';
  const isTimeout = error.code === 'ECONNABORTED' || error.message?.includes('timeout');
  const isServerDown = error.response && (error.response.status === 502 || error.response.status === 503 || error.response.status === 504);

  // Log chi tiết lỗi ra console
  console.error(`❌ [API Error] ${error.response ? error.response.status : error.code || 'UNKNOWN'} <- ${error.config?.baseURL || ''}${error.config?.url || ''}`, error.message);

  if (isNetworkError || isTimeout || isServerDown) {
    let errorMsg = '⚠️ Không thể kết nối tới máy chủ API';
    if (isTimeout) {
      errorMsg = '⚠️ Kết nối tới máy chủ API bị quá hạn (Timeout)';
    } else if (isServerDown) {
      errorMsg = `⚠️ Máy chủ API phản hồi lỗi hệ thống (HTTP ${error.response.status})`;
    }

    const now = Date.now();
    if (now - lastAlertTime > 5000) {
      lastAlertTime = now;
      const apiName = error.config?.baseURL || 'API';
      alert(`${errorMsg} (${apiName}). Vui lòng kiểm tra lại kết nối mạng hoặc trạng thái hoạt động của server!`);
    }
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

// ── Kiểm tra kết nối tới 3 API khi tải trang (F12 Console Log) ──
async function checkApiConnections() {
  const apis = [
    { name: 'API 1: Project & Member Service', client: projectAPI, pingPath: '/openapi/v1.json' },
    { name: 'API 2: Task & Kanban Service', client: taskAPI, pingPath: '/api/Task' },
    { name: 'API 3: Comment & Notify Service', client: notifyAPI, pingPath: '/swagger/v1/swagger.json' }
  ]

  console.log('%c🔍 Đang kiểm tra kết nối tới các dịch vụ API...', 'color: #3b82f6; font-weight: bold;');

  apis.forEach(async (api) => {
    try {
      // Gửi request ngắn tới API/swagger JSON của mỗi API để kiểm tra kết nối (trả về 200 OK để không bị đỏ F12)
      await axios.get(api.client.defaults.baseURL + api.pingPath, { timeout: 5000 })
      console.log(`%c✅ [${api.name}] Kết nối thành công tới ${api.client.defaults.baseURL}`, 'color: #10b981; font-weight: bold;');
    } catch (error) {
      if (error.response) {
        console.log(`%c✅ [${api.name}] Kết nối thành công (Phản hồi: ${error.response.status}) tới ${api.client.defaults.baseURL}`, 'color: #10b981; font-weight: bold;');
      } else {
        console.error(`%c❌ [${api.name}] Kết nối THẤT BẠI tới ${api.client.defaults.baseURL}:`, 'color: #ef4444; font-weight: bold;', error.message || error.code);
      }
    }
  })
}

// Gọi kiểm tra ngay khi khởi động trang web
checkApiConnections();
