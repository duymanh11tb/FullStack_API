<template>
  <div class="landing-page">
    <!-- Top Nav Header -->
    <header class="landing-header glass-panel">
      <div class="logo">
        <div class="logo-icon">
          <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
            <line x1="6" y1="20" x2="6" y2="14" />
            <line x1="12" y1="20" x2="12" y2="4" />
            <line x1="18" y1="20" x2="18" y2="10" />
          </svg>
        </div>
        <span class="logo-text">ProTask</span>
      </div>
      <nav class="nav-links">
        <a href="#features" class="active">Tính năng</a>
        <a href="#solutions">Giải pháp</a>
        <a href="#pricing">Bảng giá</a>
        <a href="#contact">Liên hệ</a>
      </nav>
      <div class="header-actions">
        <router-link to="/login" class="btn-login-text">Đăng nhập</router-link>
        <router-link to="/register" class="btn-signup-blue">Đăng ký</router-link>
      </div>
    </header>

    <!-- Hero Section -->
    <section class="hero-section">
      <div class="hero-left">
        <span class="badge-pill">🛡️ MICROSERVICES ARCHITECTURE</span>
        <h1 class="hero-title">Quản lý dự án thông<br>minh với kiến trúc<br>Microservices</h1>
        <p class="hero-subtitle">
          Nâng cao hiệu suất làm việc nhóm với nền tảng quản lý dự án hiện đại, linh hoạt và có khả năng mở rộng không giới hạn cho mọi doanh nghiệp Enterprise.
        </p>
        <div class="hero-actions">
          <router-link to="/register" class="btn-primary-blue">
            Bắt đầu ngay
            <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" style="margin-left: 6px;">
              <line x1="5" y1="12" x2="19" y2="12" />
              <polyline points="12 5 19 12 12 19" />
            </svg>
          </router-link>
          <router-link to="/login" class="btn-demo-grey">
            <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" style="margin-right: 6px;">
              <circle cx="12" cy="12" r="10" />
              <polygon points="10 8 16 12 10 16 10 8" fill="currentColor" />
            </svg>
            Xem bản demo
          </router-link>
        </div>
        
        <!-- Trust badge with overlapping avatars -->
        <div class="trust-badge">
          <div class="overlapping-avatars">
            <div class="avatar" style="background: linear-gradient(135deg, #ff9a9e 0%, #fecfef 100%);">M</div>
            <div class="avatar" style="background: linear-gradient(135deg, #a1c4fd 0%, #c2e9fb 100%);">H</div>
            <div class="avatar" style="background: linear-gradient(135deg, #cfd9df 0%, #e2ebf0 100%);">T</div>
          </div>
          <span class="trust-text">+500 Doanh nghiệp tin dùng</span>
        </div>
      </div>

      <div class="hero-right">
        <!-- Hero Mockup Dashboard Preview -->
        <div class="hero-mockup-container">
          <!-- Floating Badge Top Left: Sprint Progress -->
          <div class="floating-badge badge-sprint glass-panel">
            <div class="badge-dot-row">
              <span class="pulse-dot"></span>
              <span class="badge-title">Tiến độ Sprint #24</span>
            </div>
            <div class="badge-stats">
              <div class="stat-item">
                <span class="stat-num">88%</span>
                <span class="stat-desc">Hoàn thành</span>
              </div>
              <div class="stat-item">
                <span class="stat-num text-primary">8</span>
                <span class="stat-desc">tasks còn lại</span>
              </div>
            </div>
          </div>

          <!-- Main Mockup Screen -->
          <div class="hero-mockup glass-panel">
            <div class="mockup-bar-top">
              <span class="dot red"></span>
              <span class="dot yellow"></span>
              <span class="dot green"></span>
              <div class="tab-indicator">ProTask Workspace - Board View</div>
            </div>
            
            <div class="mockup-board-content">
              <!-- Sidebar mock -->
              <div class="mockup-sidebar">
                <div class="sb-item active"></div>
                <div class="sb-item"></div>
                <div class="sb-item"></div>
                <div class="sb-item"></div>
              </div>
              
              <!-- Content mock -->
              <div class="mockup-main">
                <div class="mockup-main-header">
                  <div class="mockup-header-line long"></div>
                  <div class="mockup-avatars">
                    <span class="mockup-avatar"></span>
                    <span class="mockup-avatar"></span>
                  </div>
                </div>
                
                <div class="mockup-columns">
                  <div class="mockup-col">
                    <div class="mockup-col-header"></div>
                    <div class="mockup-task-card">
                      <div class="card-line long"></div>
                      <div class="card-line short"></div>
                      <div class="card-meta">
                        <span class="meta-tag blue"></span>
                        <span class="meta-avatar"></span>
                      </div>
                    </div>
                    <div class="mockup-task-card">
                      <div class="card-line long"></div>
                      <div class="card-meta">
                        <span class="meta-tag orange"></span>
                        <span class="meta-avatar"></span>
                      </div>
                    </div>
                  </div>
                  
                  <div class="mockup-col">
                    <div class="mockup-col-header"></div>
                    <div class="mockup-task-card">
                      <div class="card-line long"></div>
                      <div class="card-line short"></div>
                      <div class="card-meta">
                        <span class="meta-tag green"></span>
                        <span class="meta-avatar"></span>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- Floating Badge Bottom Right: Member Activity -->
          <div class="floating-badge badge-activity glass-panel">
            <div class="activity-header">
              <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" class="activity-icon">
                <path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2" />
                <circle cx="9" cy="7" r="4" />
                <path d="M23 21v-2a4 4 0 0 0-3-3.87" />
                <path d="M16 3.13a4 4 0 0 1 0 7.75" />
              </svg>
              <span>Member Activity</span>
            </div>
            <div class="activity-rows">
              <div class="activity-row">
                <span class="row-circle bg-blue"></span>
                <span class="row-line long"></span>
              </div>
              <div class="activity-row">
                <span class="row-circle bg-green"></span>
                <span class="row-line short"></span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- Core Services Section -->
    <section id="features" class="services-section">
      <div class="section-header-center">
        <h2 class="services-title">Dịch vụ cốt lõi của ProTask</h2>
        <p class="services-subtitle">Mọi công cụ bạn cần để quản lý dự án hiệu quả, từ lập kế hoạch đến thực thi và theo dõi tiến độ.</p>
      </div>

      <div class="services-grid">
        <!-- Service Card 1 -->
        <div class="service-card glass-panel">
          <div class="service-icon">
            <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
              <rect x="3" y="3" width="7" height="7" rx="1.5" />
              <rect x="14" y="3" width="7" height="7" rx="1.5" />
              <rect x="3" y="14" width="7" height="7" rx="1.5" />
              <rect x="14" y="14" width="7" height="7" rx="1.5" />
              <path d="M10 6.5h4M6.5 10v4M17.5 10v4M10 17.5h4" stroke-dasharray="2 2" />
            </svg>
          </div>
          <h3>Project & Member Management</h3>
          <p>Quản lý dự án song song. Phân quyền chi tiết cho từng thành viên theo vai trò và cấp bậc trong tổ chức.</p>
          <ul class="service-features">
            <li>
              <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="3" class="check-icon">
                <polyline points="20 6 9 17 4 12" />
              </svg>
              Quản lý phân quyền RBAC
            </li>
            <li>
              <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="3" class="check-icon">
                <polyline points="20 6 9 17 4 12" />
              </svg>
              Lưu trữ tài liệu tập trung
            </li>
          </ul>
        </div>

        <!-- Service Card 2 (Kanban Board) -->
        <div class="service-card glass-panel border-highlight">
          <div class="service-icon">
            <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
              <line x1="3" y1="3" x2="3" y2="21" />
              <line x1="12" y1="3" x2="12" y2="21" />
              <line x1="21" y1="3" x2="21" y2="21" />
              <rect x="5" y="5" width="5" height="4" rx="1" fill="rgba(37,99,235,0.1)" />
              <rect x="14" y="8" width="5" height="5" rx="1" fill="rgba(37,99,235,0.1)" />
            </svg>
          </div>
          <h3>Task & Kanban</h3>
          <p>Trực quan hóa quy trình làm việc với bảng Kanban mạnh mẽ. Kéo thả linh hoạt, quản lý backlog và sprint thông minh.</p>
          <div class="service-visual-kanban">
            <div class="kanban-mini-layout">
              <div class="mini-sidebar"></div>
              <div class="mini-body">
                <div class="mini-line"></div>
                <div class="mini-line"></div>
              </div>
            </div>
          </div>
        </div>

        <!-- Service Card 3 -->
        <div class="service-card glass-panel">
          <div class="service-icon">
            <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
              <path d="M21 15a2 2 0 0 1-2 2H7l-4 4V5a2 2 0 0 1 2-2h14a2 2 0 0 1 2 2z" />
            </svg>
          </div>
          <h3>Comment & Notification</h3>
          <p>Hệ thống thông báo thời gian thực giúp đội ngũ luôn cập nhật. Thảo luận trực tiếp trên từng task vụ cụ thể.</p>
          <div class="service-visual-avatars">
            <div class="avatar bg-blue">JD</div>
            <div class="avatar bg-purple">AS</div>
            <div class="avatar bg-grey">+5</div>
          </div>
        </div>
      </div>
    </section>

    <!-- Metrics Bar Banner -->
    <section class="metrics-banner">
      <div class="metrics-banner-container">
        <div class="metric-item">
          <span class="metric-number">10M+</span>
          <span class="metric-label">TASKS CREATED</span>
        </div>
        <div class="metric-item">
          <span class="metric-number">50k+</span>
          <span class="metric-label">ACTIVE TEAMS</span>
        </div>
        <div class="metric-item">
          <span class="metric-number">99.9%</span>
          <span class="metric-label">UPTIME RELIABILITY</span>
        </div>
        <div class="metric-item">
          <span class="metric-number">24/7</span>
          <span class="metric-label">ENTERPRISE SUPPORT</span>
        </div>
      </div>
    </section>

    <!-- CTA Section -->
    <section id="contact" class="cta-section">
      <div class="cta-content">
        <h2>Sẵn sàng để tối ưu hóa quy trình làm việc?</h2>
        <p>Tham gia cùng hàng ngàn doanh nghiệp đang chuyển đổi số mạnh mẽ cùng ProTask. Đăng ký dùng thử miễn phí 14 ngày ngay hôm nay.</p>
        <div class="cta-actions">
          <router-link to="/register" class="btn-cta-blue">Bắt đầu miễn phí</router-link>
          <router-link to="/login" class="btn-cta-grey">Liên hệ tư vấn</router-link>
        </div>
      </div>
    </section>

    <!-- Footer -->
    <footer class="landing-footer">
      <div class="footer-container">
        <div class="footer-left">
          <div class="logo">
            <div class="logo-icon">
              <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
                <line x1="6" y1="20" x2="6" y2="14" />
                <line x1="12" y1="20" x2="12" y2="4" />
                <line x1="18" y1="20" x2="18" y2="10" />
              </svg>
            </div>
            <span class="logo-text">ProTask</span>
          </div>
          <p class="footer-desc">
            Hệ thống quản lý dự án hàng đầu cho doanh nghiệp, xây dựng trên nền tảng công nghệ Microservices hiện đại.
          </p>
        </div>
        
        <div class="footer-cols">
          <div class="footer-col">
            <h4>Sản phẩm</h4>
            <a href="#features">Tính năng</a>
            <router-link to="/dashboard">Kanban Board</router-link>
            <router-link to="/dashboard">Báo cáo hiệu suất</router-link>
            <a href="#">Mobile App</a>
          </div>
          <div class="footer-col">
            <h4>Hỗ trợ</h4>
            <a href="#">Trung tâm trợ giúp</a>
            <a href="#">API Documentation</a>
            <a href="#">Cộng đồng</a>
            <a href="#">Liên hệ hỗ trợ</a>
          </div>
          <div class="footer-col">
            <h4>Kết nối</h4>
            <div class="social-links">
              <a href="#" class="social-btn" title="Website">
                <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                  <circle cx="12" cy="12" r="10" />
                  <line x1="2" y1="12" x2="22" y2="12" />
                  <path d="M12 2a15.3 15.3 0 0 1 4 10 15.3 15.3 0 0 1-4 10 15.3 15.3 0 0 1-4-10 15.3 15.3 0 0 1 4-10z" />
                </svg>
              </a>
              <a href="#" class="social-btn" title="Email">
                <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                  <circle cx="12" cy="12" r="4" />
                  <path d="M16 8v5a3 3 0 0 0 6 0v-1a10 10 0 1 0-3.92 7.94" />
                </svg>
              </a>
              <a href="#" class="social-btn" title="Share">
                <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                  <circle cx="18" cy="5" r="3" />
                  <circle cx="6" cy="12" r="3" />
                  <circle cx="18" cy="19" r="3" />
                  <line x1="8.59" y1="13.51" x2="15.42" y2="17.49" />
                  <line x1="15.41" y1="6.51" x2="8.59" y2="10.49" />
                </svg>
              </a>
            </div>
            <p class="copyright">© 2024 ProTask Inc. All rights reserved.</p>
          </div>
        </div>
      </div>
    </footer>
  </div>
</template>

<script setup>
// Landing page setup
</script>

<style scoped>
.landing-page {
  min-height: 100vh;
  background-color: #f8fafc;
  color: #0f172a;
  font-family: 'Inter', -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, sans-serif;
  overflow-x: hidden;
}

/* Header styling */
.landing-header {
  position: sticky;
  top: 16px;
  margin: 16px auto;
  width: 90%;
  max-width: 1200px;
  height: 68px;
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0 var(--space-6);
  z-index: 100;
  border-radius: var(--radius-full);
  background: rgba(255, 255, 255, 0.85);
  border: 1px solid rgba(226, 232, 240, 0.8);
  box-shadow: 0 4px 30px rgba(0, 0, 0, 0.03);
  backdrop-filter: blur(10px);
}

[data-theme='dark'] .landing-header {
  background: rgba(15, 23, 42, 0.85);
  border: 1px solid rgba(255, 255, 255, 0.06);
  box-shadow: 0 4px 30px rgba(0, 0, 0, 0.2);
}

.logo {
  display: flex;
  align-items: center;
  gap: var(--space-2);
}

.logo-icon {
  width: 32px;
  height: 32px;
  background: #2563eb;
  border-radius: var(--radius-md);
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  box-shadow: 0 4px 10px rgba(37, 99, 235, 0.25);
}

.logo-text {
  font-size: 1.125rem;
  font-weight: 800;
  letter-spacing: -0.5px;
  color: #0f172a;
}

[data-theme='dark'] .logo-text {
  color: #f8fafc;
}

.nav-links {
  display: flex;
  gap: var(--space-6);
}

@media (max-width: 768px) {
  .nav-links {
    display: none;
  }
}

.nav-links a {
  font-size: var(--font-size-sm);
  font-weight: 600;
  color: #475569;
  text-decoration: none;
  transition: all var(--transition-fast);
  position: relative;
  padding: 4px 0;
}

[data-theme='dark'] .nav-links a {
  color: #94a3b8;
}

.nav-links a:hover,
.nav-links a.active {
  color: #2563eb;
}

.nav-links a.active::after {
  content: '';
  position: absolute;
  bottom: 0;
  left: 0;
  width: 100%;
  height: 2px;
  background-color: #2563eb;
  border-radius: 2px;
}

.header-actions {
  display: flex;
  align-items: center;
  gap: var(--space-4);
}

.btn-login-text {
  font-size: var(--font-size-sm);
  font-weight: 600;
  color: #0f172a;
  text-decoration: none;
  transition: color var(--transition-fast);
}

[data-theme='dark'] .btn-login-text {
  color: #f8fafc;
}

.btn-login-text:hover {
  color: #2563eb;
}

.btn-signup-blue {
  font-size: var(--font-size-sm);
  font-weight: 700;
  background: #2563eb;
  color: white;
  padding: 10px 20px;
  border-radius: var(--radius-md);
  text-decoration: none;
  transition: all var(--transition-fast);
  box-shadow: 0 4px 12px rgba(37, 99, 235, 0.2);
}

.btn-signup-blue:hover {
  background: #1d4ed8;
  transform: translateY(-1px);
}

/* Hero Section */
.hero-section {
  width: 90%;
  max-width: 1200px;
  margin: var(--space-12) auto;
  display: grid;
  grid-template-columns: 1.1fr 0.9fr;
  align-items: center;
  gap: var(--space-12);
  padding: var(--space-6) 0;
}

@media (max-width: 1024px) {
  .hero-section {
    grid-template-columns: 1fr;
    text-align: center;
    gap: var(--space-10);
  }
}

.hero-left {
  display: flex;
  flex-direction: column;
  align-items: flex-start;
}

@media (max-width: 1024px) {
  .hero-left {
    align-items: center;
  }
}

.badge-pill {
  display: inline-block;
  padding: 6px 14px;
  background: rgba(37, 99, 235, 0.08);
  color: #2563eb;
  font-size: 11px;
  font-weight: 800;
  border-radius: var(--radius-full);
  margin-bottom: var(--space-6);
  border: 1px solid rgba(37, 99, 235, 0.15);
  letter-spacing: 0.5px;
}

.hero-title {
  font-size: 3.5rem;
  font-weight: 850;
  line-height: 1.15;
  margin-bottom: var(--space-5);
  color: #1e3a8a;
  letter-spacing: -1.5px;
}

[data-theme='dark'] .hero-title {
  color: #f8fafc;
}

@media (max-width: 576px) {
  .hero-title {
    font-size: 2.5rem;
  }
}

.hero-subtitle {
  font-size: 1.125rem;
  color: #475569;
  line-height: 1.6;
  margin-bottom: var(--space-8);
  font-weight: 500;
  max-width: 520px;
}

[data-theme='dark'] .hero-subtitle {
  color: #cbd5e1;
}

.hero-actions {
  display: flex;
  gap: var(--space-4);
  align-items: center;
  margin-bottom: var(--space-8);
}

.btn-primary-blue {
  font-size: var(--font-size-sm);
  font-weight: 700;
  background: #2563eb;
  color: white;
  padding: 14px 28px;
  border-radius: var(--radius-md);
  text-decoration: none;
  display: inline-flex;
  align-items: center;
  transition: all var(--transition-fast);
  box-shadow: 0 4px 14px rgba(37, 99, 235, 0.25);
}

.btn-primary-blue:hover {
  background: #1d4ed8;
  transform: translateY(-2px);
  box-shadow: 0 6px 20px rgba(37, 99, 235, 0.35);
}

.btn-demo-grey {
  font-size: var(--font-size-sm);
  font-weight: 600;
  background: #e2e8f0;
  color: #334155;
  padding: 14px 28px;
  border-radius: var(--radius-md);
  text-decoration: none;
  display: inline-flex;
  align-items: center;
  transition: all var(--transition-fast);
  border: 1px solid rgba(226, 232, 240, 0.8);
}

[data-theme='dark'] .btn-demo-grey {
  background: rgba(255, 255, 255, 0.08);
  color: #f8fafc;
  border-color: rgba(255, 255, 255, 0.1);
}

.btn-demo-grey:hover {
  background: #cbd5e1;
  transform: translateY(-2px);
}

[data-theme='dark'] .btn-demo-grey:hover {
  background: rgba(255, 255, 255, 0.15);
}

/* Trust Badge styling */
.trust-badge {
  display: flex;
  align-items: center;
  gap: var(--space-3);
}

.overlapping-avatars {
  display: flex;
  align-items: center;
}

.overlapping-avatars .avatar {
  width: 28px;
  height: 28px;
  border-radius: var(--radius-full);
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 10px;
  font-weight: 700;
  color: #0f172a;
  border: 2px solid #f8fafc;
  margin-right: -10px;
  box-shadow: 0 2px 5px rgba(0,0,0,0.1);
}

[data-theme='dark'] .overlapping-avatars .avatar {
  border-color: #0b1220;
}

.trust-text {
  font-size: var(--font-size-xs);
  color: #64748b;
  font-weight: 600;
  margin-left: var(--space-2);
}

/* Hero Right: Mockup styling */
.hero-mockup-container {
  position: relative;
  width: 100%;
  max-width: 500px;
  margin: 0 auto;
}

.hero-mockup {
  width: 100%;
  background: #0f172a;
  border-radius: var(--radius-lg);
  overflow: hidden;
  box-shadow: 0 30px 60px rgba(15, 23, 42, 0.25);
  border: 1px solid rgba(255, 255, 255, 0.08);
}

.mockup-bar-top {
  display: flex;
  align-items: center;
  gap: 6px;
  padding: 12px 16px;
  border-bottom: 1px solid rgba(255, 255, 255, 0.08);
}

.mockup-bar-top .dot {
  width: 8px;
  height: 8px;
  border-radius: 50%;
}

.mockup-bar-top .dot.red { background: #ef4444; }
.mockup-bar-top .dot.yellow { background: #eab308; }
.mockup-bar-top .dot.green { background: #22c55e; }

.tab-indicator {
  font-size: 10px;
  color: #64748b;
  margin-left: 12px;
  font-weight: 600;
}

.mockup-board-content {
  display: flex;
  height: 250px;
}

.mockup-sidebar {
  width: 54px;
  border-right: 1px solid rgba(255, 255, 255, 0.08);
  padding: 16px 12px;
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.sb-item {
  height: 8px;
  background: rgba(255, 255, 255, 0.1);
  border-radius: 4px;
}

.sb-item.active {
  background: #2563eb;
}

.mockup-main {
  flex: 1;
  padding: 20px;
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.mockup-main-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.mockup-header-line {
  height: 8px;
  background: rgba(255, 255, 255, 0.2);
  border-radius: 4px;
}

.mockup-header-line.long { width: 120px; }

.mockup-avatars {
  display: flex;
  gap: 4px;
}

.mockup-avatar {
  width: 16px;
  height: 16px;
  border-radius: 50%;
  background: rgba(255, 255, 255, 0.3);
}

.mockup-columns {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 12px;
  flex: 1;
}

.mockup-col {
  background: rgba(255, 255, 255, 0.03);
  border: 1px solid rgba(255, 255, 255, 0.05);
  border-radius: var(--radius-sm);
  padding: 8px;
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.mockup-col-header {
  height: 6px;
  width: 40px;
  background: rgba(255, 255, 255, 0.2);
  border-radius: 3px;
  margin-bottom: 4px;
}

.mockup-task-card {
  background: rgba(255, 255, 255, 0.05);
  border-radius: 6px;
  padding: 8px;
  display: flex;
  flex-direction: column;
  gap: 6px;
}

.card-line {
  height: 5px;
  background: rgba(255, 255, 255, 0.15);
  border-radius: 2px;
}

.card-line.long { width: 85%; }
.card-line.short { width: 45%; }

.card-meta {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-top: 2px;
}

.meta-tag {
  width: 18px;
  height: 4px;
  border-radius: 2px;
}

.meta-tag.blue { background: #3b82f6; }
.meta-tag.orange { background: #f57c00; }
.meta-tag.green { background: #10b981; }

.meta-avatar {
  width: 10px;
  height: 10px;
  border-radius: 50%;
  background: rgba(255, 255, 255, 0.4);
}

/* Floating badge 1: Sprint Progress */
.floating-badge {
  position: absolute;
  border-radius: var(--radius-md);
  padding: var(--space-4);
  box-shadow: 0 10px 25px rgba(0, 0, 0, 0.1);
  z-index: 20;
}

.badge-sprint {
  top: -30px;
  left: -40px;
  background: white;
  border: 1px solid rgba(226, 232, 240, 0.8);
  width: 170px;
}

[data-theme='dark'] .badge-sprint {
  background: #1e293b;
  border-color: rgba(255, 255, 255, 0.08);
}

.badge-dot-row {
  display: flex;
  align-items: center;
  gap: 6px;
  margin-bottom: var(--space-3);
}

.pulse-dot {
  width: 6px;
  height: 6px;
  background-color: #ef4444;
  border-radius: 50%;
  display: inline-block;
  animation: pulse 2s infinite;
}

.badge-title {
  font-size: 10px;
  font-weight: 700;
  color: #64748b;
}

[data-theme='dark'] .badge-title {
  color: #94a3b8;
}

.badge-stats {
  display: flex;
  justify-content: space-between;
}

.stat-item {
  display: flex;
  flex-direction: column;
}

.stat-num {
  font-size: 1.125rem;
  font-weight: 800;
  color: #0f172a;
}

[data-theme='dark'] .stat-num {
  color: #f8fafc;
}

.stat-num.text-primary {
  color: #2563eb;
}

.stat-desc {
  font-size: 9px;
  color: #64748b;
  font-weight: 600;
}

/* Floating badge 2: Member Activity */
.badge-activity {
  bottom: -20px;
  right: -30px;
  background: white;
  border: 1px solid rgba(226, 232, 240, 0.8);
  width: 160px;
  padding: 12px;
}

[data-theme='dark'] .badge-activity {
  background: #1e293b;
  border-color: rgba(255, 255, 255, 0.08);
}

.activity-header {
  display: flex;
  align-items: center;
  gap: 6px;
  font-size: 10px;
  font-weight: 700;
  color: #0f172a;
  margin-bottom: var(--space-3);
}

[data-theme='dark'] .activity-header {
  color: #f8fafc;
}

.activity-icon {
  color: #64748b;
}

.activity-rows {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.activity-row {
  display: flex;
  align-items: center;
  gap: 6px;
}

.row-circle {
  width: 12px;
  height: 12px;
  border-radius: 50%;
  flex-shrink: 0;
}

.row-circle.bg-blue { background: #3b82f6; }
.row-circle.bg-green { background: #10b981; }

.row-line {
  height: 5px;
  background: #e2e8f0;
  border-radius: 2px;
}

[data-theme='dark'] .row-line {
  background: rgba(255, 255, 255, 0.1);
}

.row-line.long { width: 80px; }
.row-line.short { width: 50px; }

/* Pulse animation */
@keyframes pulse {
  0% { transform: scale(0.95); box-shadow: 0 0 0 0 rgba(239, 68, 68, 0.7); }
  70% { transform: scale(1); box-shadow: 0 0 0 4px rgba(239, 68, 68, 0); }
  100% { transform: scale(0.95); box-shadow: 0 0 0 0 rgba(239, 68, 68, 0); }
}

/* Services section style */
.services-section {
  width: 90%;
  max-width: 1200px;
  margin: var(--space-12) auto;
  padding: var(--space-8) 0;
}

.services-title {
  font-size: 2rem;
  font-weight: 800;
  color: #1e3a8a;
  margin-bottom: var(--space-2);
}

[data-theme='dark'] .services-title {
  color: #f8fafc;
}

.services-subtitle {
  color: #64748b;
  font-size: var(--font-size-base);
  font-weight: 500;
  max-width: 600px;
  margin: 0 auto var(--space-10) auto;
}

.services-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(320px, 1fr));
  gap: var(--space-8);
}

.service-card {
  padding: var(--space-8) var(--space-6);
  display: flex;
  flex-direction: column;
  gap: var(--space-4);
  transition: all var(--transition-slow);
  background: white;
  border: 1px solid rgba(226, 232, 240, 0.8);
  border-radius: var(--radius-lg);
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.02);
}

[data-theme='dark'] .service-card {
  background: rgba(15, 23, 42, 0.6);
  border-color: rgba(255, 255, 255, 0.06);
}

.service-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 12px 30px rgba(0,0,0,0.06);
}

.service-card.border-highlight {
  border-color: #2563eb;
  box-shadow: 0 10px 25px rgba(37, 99, 235, 0.05);
}

.service-icon {
  width: 44px;
  height: 44px;
  border-radius: var(--radius-md);
  display: flex;
  align-items: center;
  justify-content: center;
  background: rgba(37, 99, 235, 0.08);
  color: #2563eb;
  box-shadow: var(--shadow-xs);
}

.service-card h3 {
  font-size: 1.125rem;
  font-weight: 800;
  color: #0f172a;
}

[data-theme='dark'] .service-card h3 {
  color: #f8fafc;
}

.service-card p {
  font-size: var(--font-size-sm);
  color: #475569;
  line-height: 1.5;
}

[data-theme='dark'] .service-card p {
  color: #94a3b8;
}

.service-features {
  list-style: none;
  padding: 0;
  margin: var(--space-3) 0 0 0;
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.service-features li {
  font-size: var(--font-size-sm);
  color: #334155;
  font-weight: 600;
  display: flex;
  align-items: center;
  gap: 8px;
}

[data-theme='dark'] .service-features li {
  color: #cbd5e1;
}

.check-icon {
  color: #10b981;
}

/* Service visual mocks */
.service-visual-kanban {
  margin-top: var(--space-4);
  background: #f8fafc;
  border: 1px solid #e2e8f0;
  border-radius: var(--radius-md);
  padding: 10px;
  height: 80px;
  display: flex;
}

[data-theme='dark'] .service-visual-kanban {
  background: rgba(255,255,255,0.02);
  border-color: rgba(255,255,255,0.05);
}

.kanban-mini-layout {
  display: flex;
  width: 100%;
  gap: 10px;
}

.mini-sidebar {
  width: 40px;
  background: #e2e8f0;
  border-radius: 4px;
}

[data-theme='dark'] .mini-sidebar {
  background: rgba(255,255,255,0.08);
}

.mini-body {
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: 6px;
  justify-content: center;
}

.mini-line {
  height: 6px;
  background: #e2e8f0;
  border-radius: 3px;
}

[data-theme='dark'] .mini-line {
  background: rgba(255,255,255,0.08);
}

.mini-line:first-child { width: 90%; }
.mini-line:last-child { width: 60%; }

.service-visual-avatars {
  display: flex;
  align-items: center;
  margin-top: var(--space-4);
  gap: 6px;
}

.service-visual-avatars .avatar {
  width: 32px;
  height: 32px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 11px;
  font-weight: 700;
  color: white;
  box-shadow: var(--shadow-sm);
}

.avatar.bg-blue { background: #3b82f6; }
.avatar.bg-purple { background: #7c3aed; }
.avatar.bg-grey { background: #64748b; }

/* Metrics Banner Styling */
.metrics-banner {
  background-color: #1e3a8a;
  color: white;
  padding: var(--space-10) 0;
  margin: var(--space-12) 0;
}

.metrics-banner-container {
  width: 90%;
  max-width: 1200px;
  margin: 0 auto;
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: var(--space-8);
  text-align: center;
}

.metric-item {
  display: flex;
  flex-direction: column;
  gap: 6px;
}

.metric-number {
  font-size: 2.25rem;
  font-weight: 850;
  letter-spacing: -1px;
}

.metric-label {
  font-size: 10px;
  font-weight: 800;
  color: #93c5fd;
  letter-spacing: 1px;
}

/* CTA Section */
.cta-section {
  width: 90%;
  max-width: 1200px;
  margin: var(--space-12) auto;
  padding: var(--space-10) var(--space-6);
  text-align: center;
  background: radial-gradient(circle at top right, rgba(37,99,235,0.05) 0%, transparent 60%), white;
  border: 1px solid rgba(226, 232, 240, 0.8);
  border-radius: var(--radius-lg);
  box-shadow: 0 10px 30px rgba(0,0,0,0.02);
}

[data-theme='dark'] .cta-section {
  background: radial-gradient(circle at top right, rgba(37,99,235,0.1) 0%, transparent 60%), rgba(15, 23, 42, 0.6);
  border-color: rgba(255, 255, 255, 0.06);
}

.cta-content h2 {
  font-size: 2rem;
  font-weight: 800;
  color: #1e3a8a;
  margin-bottom: var(--space-3);
}

[data-theme='dark'] .cta-content h2 {
  color: #f8fafc;
}

.cta-content p {
  font-size: var(--font-size-base);
  color: #475569;
  max-width: 600px;
  margin: 0 auto var(--space-8) auto;
  line-height: 1.5;
}

[data-theme='dark'] .cta-content p {
  color: #cbd5e1;
}

.cta-actions {
  display: flex;
  gap: var(--space-4);
  justify-content: center;
  align-items: center;
}

.btn-cta-blue {
  font-size: var(--font-size-sm);
  font-weight: 700;
  background: #2563eb;
  color: white;
  padding: 12px 28px;
  border-radius: var(--radius-md);
  text-decoration: none;
  transition: all var(--transition-fast);
}

.btn-cta-blue:hover {
  background: #1d4ed8;
  transform: translateY(-1px);
}

.btn-cta-grey {
  font-size: var(--font-size-sm);
  font-weight: 600;
  background: #64748b;
  color: white;
  padding: 12px 28px;
  border-radius: var(--radius-md);
  text-decoration: none;
  transition: all var(--transition-fast);
}

.btn-cta-grey:hover {
  background: #475569;
  transform: translateY(-1px);
}

/* Footer Styling */
.landing-footer {
  border-top: 1px solid rgba(226, 232, 240, 0.8);
  background: #f8fafc;
  padding: var(--space-12) 0 var(--space-8) 0;
}

[data-theme='dark'] .landing-footer {
  background: #0b1220;
  border-color: rgba(255, 255, 255, 0.05);
}

.footer-container {
  width: 90%;
  max-width: 1200px;
  margin: 0 auto;
  display: grid;
  grid-template-columns: 0.8fr 1.2fr;
  gap: var(--space-12);
}

@media (max-width: 768px) {
  .footer-container {
    grid-template-columns: 1fr;
    gap: var(--space-8);
  }
}

.footer-left {
  display: flex;
  flex-direction: column;
  gap: var(--space-4);
}

.footer-desc {
  font-size: var(--font-size-sm);
  color: #64748b;
  line-height: 1.6;
  max-width: 280px;
}

.footer-cols {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(180px, 1fr));
  gap: var(--space-8);
}

.footer-col {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.footer-col h4 {
  font-size: var(--font-size-sm);
  color: #0f172a;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  margin-bottom: var(--space-2);
}

[data-theme='dark'] .footer-col h4 {
  color: #f8fafc;
}

.footer-col a {
  font-size: var(--font-size-sm);
  color: #64748b;
  text-decoration: none;
  transition: color var(--transition-fast);
}

.footer-col a:hover {
  color: #2563eb;
}

.social-links {
  display: flex;
  gap: 10px;
  margin-bottom: var(--space-4);
}

.social-btn {
  width: 32px;
  height: 32px;
  border-radius: 50%;
  background: #cbd5e1;
  display: flex;
  align-items: center;
  justify-content: center;
  color: #0f172a;
  transition: all var(--transition-fast);
}

[data-theme='dark'] .social-btn {
  background: rgba(255, 255, 255, 0.08);
  color: #f8fafc;
}

.social-btn:hover {
  background: #2563eb;
  color: white;
  transform: translateY(-2px);
}

.copyright {
  font-size: var(--font-size-xs);
  color: #94a3b8;
}
</style>
