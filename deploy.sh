#!/bin/bash

# Dừng script nếu có lỗi xảy ra
set -e

echo "================================================="
echo "   🚀 BẮT ĐẦU CẬP NHẬT VÀ DEPLOY LÊN VPS"
echo "================================================="

# 1. Kéo code mới nhất từ Git (Bỏ comment nếu dùng Git trên VPS)
# echo "📦 Đang kéo code mới từ Git..."
# git pull origin main

# 2. Khởi động các container bằng Docker Compose
echo "🚀 Đang khởi chạy các container..."
docker compose down
docker compose up -d --build

# 3. Dọn dẹp các images thừa để tránh đầy ổ cứng
echo "🧹 Đang dọn dẹp các image thừa..."
docker image prune -f

echo "================================================="
echo "✅ HOÀN TẤT DEPLOY!"
echo "Ứng dụng của bạn đang chạy ở chế độ nền (background)."
echo "- Truy cập Frontend: http://<ip-vps>:80"
echo "- Truy cập Backend:  http://<ip-vps>:5003"
echo "Để xem log hệ thống, hãy dùng lệnh: docker-compose logs -f"
echo "================================================="
