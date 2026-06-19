<template>
  <div class="landing-page">
    <!-- Top Nav Header -->
    <header class="landing-header glass-panel">
      <div class="logo">
        <div class="logo-icon">
          <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
            <rect x="3" y="3" width="7" height="7" rx="1.5" />
            <rect x="14" y="3" width="7" height="7" rx="1.5" />
            <rect x="3" y="14" width="7" height="7" rx="1.5" />
            <rect x="14" y="14" width="7" height="7" rx="1.5" />
          </svg>
        </div>
        <span class="logo-text">QLDA</span>
      </div>
      <nav class="nav-links">
        <a href="#features">Tính năng</a>
        <a href="#pricing">Bảng giá</a>
        <a href="#about">Về chúng tôi</a>
      </nav>
      <div class="header-actions">
        <router-link to="/login" class="btn-login">Đăng nhập</router-link>
        <router-link to="/register" class="btn-signup-header">Đăng ký miễn phí</router-link>
      </div>
    </header>

    <!-- Hero Section -->
    <section class="hero-section">
      <div class="hero-bg-shapes">
        <div class="shape shape-1"></div>
        <div class="shape shape-2"></div>
      </div>
      
      <div class="hero-content">
        <span class="badge animate-float">🚀 Giải pháp Quản lý Dự án số 1</span>
        <h1 class="hero-title">Đơn giản hóa công việc.<br>Tăng tốc hiệu suất đội ngũ.</h1>
        <p class="hero-subtitle">Một không gian cộng tác toàn diện giúp lập kế hoạch sprint, quản lý tác vụ Kanban trực quan và theo dõi milestone dự án chính xác.</p>
        <div class="hero-actions">
          <router-link to="/register" class="btn-get-started">Bắt đầu ngay</router-link>
          <router-link to="/login" class="btn-demo">Xem bản thử nghiệm</router-link>
        </div>
      </div>

      <!-- Hero Mockup Dashboard Preview -->
      <div class="hero-mockup-wrapper animate-float">
        <div class="hero-mockup glass-panel">
          <div class="mockup-bar-top">
            <span class="dot red"></span>
            <span class="dot yellow"></span>
            <span class="dot green"></span>
            <div class="tab-indicator">QLDA Workspace - Dashboard Overview</div>
          </div>
          <div class="mockup-content-grid">
            <div class="mockup-sidebar">
              <div class="sb-item active"></div>
              <div class="sb-item"></div>
              <div class="sb-item"></div>
              <div class="sb-item"></div>
            </div>
            <div class="mockup-main">
              <div class="row-header">
                <div class="box-title"></div>
                <div class="box-user"></div>
              </div>
              <div class="row-cards">
                <div class="m-card">
                  <div class="line short"></div>
                  <div class="num-large">12</div>
                  <div class="line long"></div>
                </div>
                <div class="m-card border-green">
                  <div class="line short"></div>
                  <div class="num-large text-emerald">82%</div>
                  <div class="line long"></div>
                </div>
                <div class="m-card">
                  <div class="line short"></div>
                  <div class="num-large text-purple">5</div>
                  <div class="line long"></div>
                </div>
              </div>
              <div class="row-chart">
                <div class="chart-header"></div>
                <div class="chart-bars">
                  <div class="bar" style="height: 40%"></div>
                  <div class="bar" style="height: 70%"></div>
                  <div class="bar" style="height: 55%"></div>
                  <div class="bar" style="height: 90%"></div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- Features Section -->
    <section id="features" class="features-section">
      <div class="section-header-center">
        <h2>Công cụ mạnh mẽ cho đội ngũ Agile</h2>
        <p>Mọi tính năng bạn cần để hoàn thành công việc đúng hạn, chất lượng cao.</p>
      </div>

      <div class="features-grid">
        <div class="feature-card glass-panel">
          <div class="f-icon bg-indigo">
            <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <rect x="3" y="3" width="7" height="9" rx="1" /><rect x="14" y="3" width="7" height="5" rx="1" />
              <rect x="14" y="12" width="7" height="9" rx="1" /><rect x="3" y="16" width="7" height="5" rx="1" />
            </svg>
          </div>
          <h3>Bảng Kanban tương tác</h3>
          <p>Kéo và thả tác vụ dễ dàng giữa các trạng thái để theo dõi trực quan công việc của từng thành viên.</p>
        </div>

        <div class="feature-card glass-panel">
          <div class="f-icon bg-emerald">
            <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <circle cx="12" cy="12" r="10" /><polyline points="12 6 12 12 16 14" />
            </svg>
          </div>
          <h3>Quản lý Sprint</h3>
          <p>Lên kế hoạch sprint rõ ràng, thiết lập mục tiêu và theo dõi thời gian thực hiện của từng giai đoạn phát triển.</p>
        </div>

        <div class="feature-card glass-panel">
          <div class="f-icon bg-amber">
            <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <polyline points="22 12 18 12 15 21 9 3 6 12 2 12" />
            </svg>
          </div>
          <h3>Thống kê trực quan</h3>
          <p>Theo dõi tiến độ tổng quan với biểu đồ trạng thái tác vụ, độ ưu tiên và báo cáo đóng góp thành viên.</p>
        </div>
      </div>
    </section>

    <!-- Pricing Section -->
    <section id="pricing" class="pricing-section">
      <div class="section-header-center">
        <h2>Bảng giá linh hoạt</h2>
        <p>Lựa chọn gói phù hợp với quy mô đội ngũ của bạn.</p>
      </div>

      <div class="pricing-grid">
        <div class="pricing-card glass-panel">
          <span class="plan-name">Cơ bản (Free)</span>
          <div class="price-row">
            <span class="amount">$0</span>
            <span class="period">/ tháng</span>
          </div>
          <p class="plan-desc">Phù hợp cho cá nhân hoặc nhóm nhỏ bắt đầu dự án.</p>
          <ul class="plan-features">
            <li>✓ Tối đa 3 dự án hoạt động</li>
            <li>✓ Bảng Kanban kéo thả</li>
            <li>✓ 5 thành viên tối đa</li>
            <li>✓ Thống kê cơ bản</li>
          </ul>
          <router-link to="/register" class="btn-plan">Bắt đầu miễn phí</router-link>
        </div>

        <div class="pricing-card glass-panel featured">
          <div class="popular-badge">Phổ biến nhất</div>
          <span class="plan-name">Chuyên nghiệp (Pro)</span>
          <div class="price-row">
            <span class="amount">$9</span>
            <span class="period">/ người / tháng</span>
          </div>
          <p class="plan-desc">Tối ưu cho các đội ngũ Agile phát triển chuyên nghiệp.</p>
          <ul class="plan-features">
            <li>✓ Không giới hạn dự án</li>
            <li>✓ Quản lý Sprint nâng cao</li>
            <li>✓ Không giới hạn thành viên</li>
            <li>✓ Thống kê phân tích sâu</li>
            <li>✓ Hỗ trợ ưu tiên 24/7</li>
          </ul>
          <router-link to="/register" class="btn-plan primary">Bắt đầu trải nghiệm</router-link>
        </div>
      </div>
    </section>

    <!-- Footer -->
    <footer class="landing-footer">
      <div class="footer-content">
        <div class="footer-brand">
          <div class="logo">
            <div class="logo-icon">
              <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
                <rect x="3" y="3" width="7" height="7" rx="1.5" />
                <rect x="14" y="3" width="7" height="7" rx="1.5" />
                <rect x="3" y="14" width="7" height="7" rx="1.5" />
                <rect x="14" y="14" width="7" height="7" rx="1.5" />
              </svg>
            </div>
            <span class="logo-text">QLDA</span>
          </div>
          <p class="mt-4 text-muted">Hệ thống quản lý dự án Agile hiện đại nhất hiện nay.</p>
        </div>
        <div class="footer-links-group">
          <div class="links-col">
            <h5>Liên kết</h5>
            <a href="#features">Tính năng</a>
            <a href="#pricing">Bảng giá</a>
          </div>
          <div class="links-col">
            <h5>Pháp lý</h5>
            <a href="#">Điều khoản sử dụng</a>
            <a href="#">Chính sách bảo mật</a>
          </div>
        </div>
      </div>
      <div class="footer-bottom">
        <p>&copy; 2026 QLDA. Thiết kế bởi Antigravity AI.</p>
      </div>
    </footer>
  </div>
</template>

<script setup>
// Purely view representation, routing guards handle active sessions
</script>

<style scoped>
.landing-page {
  min-height: 100vh;
  background-color: var(--bg-primary);
  color: var(--text-primary);
  overflow-x: hidden;
}

/* Header style */
.landing-header {
  position: sticky;
  top: 16px;
  margin: 16px auto;
  width: 90%;
  max-width: 1200px;
  height: 64px;
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0 var(--space-6);
  z-index: 100;
  border-radius: var(--radius-full);
}

.logo {
  display: flex;
  align-items: center;
  gap: var(--space-2);
}

.logo-icon {
  width: 32px;
  height: 32px;
  background: var(--gradient-primary);
  border-radius: var(--radius-md);
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  box-shadow: 0 4px 10px rgba(37, 99, 235, 0.25);
}

.logo-text {
  font-size: var(--font-size-md);
  font-weight: var(--font-weight-bold);
  letter-spacing: 0.5px;
  font-family: var(--font-heading);
}

.nav-links {
  display: flex;
  gap: var(--space-6);
}

@media (max-width: 768px) {
  .nav-links { display: none; }
}

.nav-links a {
  font-size: var(--font-size-sm);
  font-weight: var(--font-weight-bold);
  color: var(--text-secondary);
  transition: color var(--transition-fast);
}

.nav-links a:hover {
  color: var(--color-primary);
}

.header-actions {
  display: flex;
  align-items: center;
  gap: var(--space-4);
}

.btn-login {
  font-size: var(--font-size-sm);
  font-weight: var(--font-weight-bold);
  color: var(--text-primary);
}

.btn-signup-header {
  font-size: var(--font-size-xs);
  font-weight: var(--font-weight-bold);
  background: var(--gradient-primary);
  color: white;
  padding: 8px 16px;
  border-radius: var(--radius-full);
  box-shadow: 0 4px 12px rgba(37, 99, 235, 0.2);
  transition: all var(--transition-fast);
}

.btn-signup-header:hover {
  transform: translateY(-1px);
  box-shadow: var(--gradient-glow);
}

/* Hero Section */
.hero-section {
  position: relative;
  width: 90%;
  max-width: 1200px;
  margin: var(--space-12) auto;
  display: grid;
  grid-template-columns: 1fr 1fr;
  align-items: center;
  gap: var(--space-10);
  padding: var(--space-6) 0;
}

@media (max-width: 1024px) {
  .hero-section {
    grid-template-columns: 1fr;
    text-align: center;
    margin: var(--space-8) auto;
  }
}

.hero-bg-shapes {
  position: absolute;
  inset: 0;
  pointer-events: none;
  z-index: 1;
}

.shape {
  position: absolute;
  border-radius: 50%;
  filter: blur(100px);
}
.shape-1 {
  width: 300px;
  height: 300px;
  background: rgba(99, 102, 241, 0.12);
  top: -80px;
  left: 20%;
}
.shape-2 {
  width: 250px;
  height: 250px;
  background: rgba(236, 72, 153, 0.1);
  bottom: 0px;
  right: 10%;
}

.hero-content {
  z-index: 10;
}

.hero-content .badge {
  display: inline-block;
  padding: 6px 14px;
  background: var(--color-primary-light);
  color: var(--color-primary);
  font-size: var(--font-size-xs);
  font-weight: 700;
  border-radius: var(--radius-full);
  margin-bottom: var(--space-5);
  border: 1px solid rgba(37, 99, 235, 0.15);
}

[data-theme='dark'] .hero-content .badge {
  background: rgba(59, 130, 246, 0.15);
  color: #60a5fa;
  border-color: rgba(59, 130, 246, 0.25);
}

.hero-title {
  font-size: var(--font-size-3xl);
  font-weight: var(--font-weight-extrabold);
  line-height: 1.2;
  margin-bottom: var(--space-4);
  color: var(--text-primary);
}

.hero-subtitle {
  font-size: var(--font-size-md);
  color: var(--text-secondary);
  line-height: 1.6;
  margin-bottom: var(--space-8);
  font-weight: 500;
}

.hero-actions {
  display: flex;
  gap: var(--space-4);
  align-items: center;
}

@media (max-width: 1024px) {
  .hero-actions {
    justify-content: center;
  }
}

.btn-get-started {
  font-size: var(--font-size-base);
  font-weight: var(--font-weight-bold);
  background: var(--gradient-primary);
  color: white;
  padding: 12px 28px;
  border-radius: var(--radius-md);
  box-shadow: 0 4px 14px rgba(37, 99, 235, 0.25);
  transition: all var(--transition-fast);
}

.btn-get-started:hover {
  transform: translateY(-2px);
  box-shadow: var(--gradient-glow), 0 6px 20px rgba(37, 99, 235, 0.3);
}

.btn-demo {
  font-size: var(--font-size-base);
  font-weight: var(--font-weight-bold);
  background: var(--bg-white-to-card);
  color: var(--text-primary);
  padding: 12px 28px;
  border-radius: var(--radius-md);
  border: 1px solid var(--border-color);
  transition: all var(--transition-fast);
}

.btn-demo:hover {
  background: var(--color-bg-secondary);
  border-color: var(--color-border-hover);
  transform: translateY(-2px);
}

/* Mockup Layout */
.hero-mockup-wrapper {
  z-index: 10;
  display: flex;
  justify-content: center;
}

.hero-mockup {
  width: 100%;
  max-width: 520px;
  background: rgba(30, 41, 59, 0.2);
  border-radius: var(--radius-lg);
  overflow: hidden;
  box-shadow: 0 30px 60px rgba(15, 23, 42, 0.15);
}

[data-theme='dark'] .hero-mockup {
  background: rgba(15, 23, 42, 0.6);
  border-color: rgba(255,255,255,0.06);
}

.mockup-bar-top {
  display: flex;
  align-items: center;
  gap: 6px;
  padding: 12px 16px;
  border-bottom: 1px solid var(--border-color);
}

.tab-indicator {
  font-size: 10px;
  color: var(--text-muted);
  margin-left: 12px;
  font-weight: 600;
}

.mockup-content-grid {
  display: flex;
  height: 240px;
}

.mockup-sidebar {
  width: 60px;
  border-right: 1px solid var(--border-color);
  padding: 16px 12px;
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.sb-item {
  height: 8px;
  background: var(--border-color);
  border-radius: 4px;
}
.sb-item.active {
  background: var(--accent-primary);
  opacity: 0.8;
}

.mockup-main {
  flex: 1;
  padding: 20px;
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.row-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.box-title {
  width: 100px;
  height: 12px;
  background: var(--text-secondary);
  opacity: 0.3;
  border-radius: 4px;
}

.box-user {
  width: 24px;
  height: 24px;
  background: var(--border-color);
  border-radius: 50%;
}

.row-cards {
  display: flex;
  gap: 12px;
}

.m-card {
  flex: 1;
  background: var(--color-bg-secondary);
  border: 1px solid var(--border-color);
  border-radius: var(--radius-sm);
  padding: 10px;
  display: flex;
  flex-direction: column;
  gap: 6px;
}

[data-theme='dark'] .m-card {
  background: rgba(255,255,255,0.02);
}

.m-card.border-green {
  border-color: rgba(16, 185, 129, 0.4);
}

.line {
  height: 6px;
  background: var(--text-muted);
  opacity: 0.25;
  border-radius: 3px;
}
.line.short { width: 50%; }
.line.long { width: 90%; }

.num-large {
  font-size: 16px;
  font-weight: 700;
}

.text-emerald { color: #10b981; }
.text-purple { color: #8b5cf6; }

.row-chart {
  background: var(--color-bg-secondary);
  border-radius: var(--radius-sm);
  padding: 12px;
  display: flex;
  flex-direction: column;
  gap: 10px;
}

[data-theme='dark'] .row-chart {
  background: rgba(255,255,255,0.01);
}

.chart-header {
  width: 80px;
  height: 6px;
  background: var(--text-muted);
  opacity: 0.3;
  border-radius: 3px;
}

.chart-bars {
  display: flex;
  align-items: flex-end;
  justify-content: space-around;
  height: 40px;
  padding-bottom: 2px;
}

.bar {
  width: 14px;
  background: var(--accent-primary);
  opacity: 0.75;
  border-radius: 3px 3px 0 0;
}

/* Features Section */
.features-section {
  width: 90%;
  max-width: 1200px;
  margin: var(--space-12) auto;
  padding: var(--space-6) 0;
}

.section-header-center {
  text-align: center;
  margin-bottom: var(--space-10);
}

.section-header-center h2 {
  font-size: var(--font-size-2xl);
  font-weight: var(--font-weight-extrabold);
  margin-bottom: var(--space-3);
  color: var(--text-primary);
}

.section-header-center p {
  color: var(--text-secondary);
  font-size: var(--font-size-base);
  font-weight: 500;
  max-width: 580px;
  margin: 0 auto;
}

.features-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
  gap: var(--space-6);
}

.feature-card {
  padding: var(--space-8) var(--space-6);
  display: flex;
  flex-direction: column;
  gap: var(--space-4);
  transition: all var(--transition-slow);
}

.feature-card:hover {
  transform: translateY(-4px);
}

.f-icon {
  width: 48px;
  height: 48px;
  border-radius: var(--radius-md);
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  box-shadow: var(--shadow-sm);
}

.bg-indigo {
  background: linear-gradient(135deg, #6366f1 0%, #4338ca 100%);
  box-shadow: 0 8px 16px rgba(99, 102, 241, 0.25);
}
.bg-emerald {
  background: linear-gradient(135deg, #10b981 0%, #047857 100%);
  box-shadow: 0 8px 16px rgba(16, 185, 129, 0.25);
}
.bg-amber {
  background: linear-gradient(135deg, #f59e0b 0%, #b45309 100%);
  box-shadow: 0 8px 16px rgba(245, 158, 11, 0.25);
}

.feature-card h3 {
  font-size: var(--font-size-base);
  font-weight: var(--font-weight-bold);
  color: var(--text-primary);
}

.feature-card p {
  font-size: var(--font-size-sm);
  color: var(--text-secondary);
  line-height: 1.5;
}

/* Pricing Section */
.pricing-section {
  width: 90%;
  max-width: 1200px;
  margin: var(--space-12) auto;
  padding: var(--space-6) 0;
}

.pricing-grid {
  display: flex;
  justify-content: center;
  align-items: stretch;
  gap: var(--space-6);
  flex-wrap: wrap;
}

.pricing-card {
  flex: 1;
  min-width: 280px;
  max-width: 360px;
  padding: var(--space-8) var(--space-6);
  display: flex;
  flex-direction: column;
  position: relative;
  transition: all var(--transition-slow);
}

.pricing-card:hover {
  transform: translateY(-4px);
}

.pricing-card.featured {
  border-color: var(--accent-primary);
  box-shadow: 0 16px 40px rgba(37, 99, 235, 0.15);
}

[data-theme='dark'] .pricing-card.featured {
  box-shadow: 0 16px 40px rgba(59, 130, 246, 0.12);
}

.popular-badge {
  position: absolute;
  top: -12px;
  left: 50%;
  transform: translateX(-50%);
  background: var(--gradient-primary);
  color: white;
  font-size: 10px;
  font-weight: 700;
  padding: 4px 12px;
  border-radius: var(--radius-full);
  box-shadow: var(--gradient-glow);
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.plan-name {
  font-size: var(--font-size-base);
  font-weight: var(--font-weight-extrabold);
  color: var(--text-primary);
  margin-bottom: var(--space-4);
  display: block;
}

.price-row {
  display: flex;
  align-items: baseline;
  margin-bottom: var(--space-4);
}

.price-row .amount {
  font-size: var(--font-size-3xl);
  font-weight: var(--font-weight-extrabold);
  color: var(--text-primary);
}

.price-row .period {
  font-size: var(--font-size-sm);
  color: var(--text-muted);
  margin-left: 4px;
}

.plan-desc {
  font-size: var(--font-size-sm);
  color: var(--text-secondary);
  line-height: 1.5;
  margin-bottom: var(--space-6);
  min-height: 44px;
}

.plan-features {
  list-style: none;
  display: flex;
  flex-direction: column;
  gap: 12px;
  margin-bottom: var(--space-8);
  flex: 1;
}

.plan-features li {
  font-size: var(--font-size-sm);
  color: var(--text-secondary);
  font-weight: 500;
}

.btn-plan {
  display: block;
  text-align: center;
  padding: 10px;
  font-size: var(--font-size-sm);
  font-weight: var(--font-weight-bold);
  color: var(--text-primary);
  background: var(--color-bg-secondary);
  border: 1px solid var(--border-color);
  border-radius: var(--radius-md);
  transition: all var(--transition-fast);
}

[data-theme='dark'] .btn-plan {
  background: rgba(255,255,255,0.02);
}

.btn-plan:hover {
  background: var(--border-color);
  transform: translateY(-1px);
}

.btn-plan.primary {
  background: var(--gradient-primary);
  color: white;
  border: none;
  box-shadow: 0 4px 12px rgba(37, 99, 235, 0.2);
}

.btn-plan.primary:hover {
  box-shadow: var(--gradient-glow);
  background: var(--gradient-primary-hover);
}

/* Footer styling */
.landing-footer {
  border-top: 1px solid var(--border-color);
  background: rgba(0,0,0,0.01);
  padding: var(--space-12) 0 var(--space-6);
  margin-top: var(--space-12);
}

.footer-content {
  width: 90%;
  max-width: 1200px;
  margin: 0 auto;
  display: flex;
  justify-content: space-between;
  flex-wrap: wrap;
  gap: var(--space-8);
  margin-bottom: var(--space-10);
}

.footer-brand {
  max-width: 320px;
}

.footer-links-group {
  display: flex;
  gap: var(--space-12);
}

@media (max-width: 576px) {
  .footer-links-group {
    width: 100%;
    justify-content: space-between;
  }
}

.links-col {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.links-col h5 {
  font-size: var(--font-size-sm);
  color: var(--text-primary);
  text-transform: uppercase;
  letter-spacing: 0.5px;
  margin-bottom: var(--space-2);
}

.links-col a {
  font-size: var(--font-size-sm);
  color: var(--text-muted);
  transition: color var(--transition-fast);
}

.links-col a:hover {
  color: var(--color-primary);
}

.footer-bottom {
  width: 90%;
  max-width: 1200px;
  margin: 0 auto;
  padding-top: var(--space-6);
  border-top: 1px solid var(--border-color);
  text-align: center;
  font-size: var(--font-size-xs);
  color: var(--text-muted);
}
</style>
