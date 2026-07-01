# Kết quả thực hiện: Thiết lập thông báo hệ thống và Sửa lỗi đồng bộ phiên đăng nhập

Tôi đã hoàn thành cấu hình và lập trình toàn bộ tính năng thông báo và sửa đổi cơ chế lưu trữ phiên thông báo của Frontend.

## Các lỗi đã khắc phục và tính năng hoàn thành

### 1. Khắc phục lỗi cả Admin và Member đều nhận chung thông báo
- **Nguyên nhân:** Do Pinia Store lưu trữ danh sách thông báo (`notifications`) trong bộ nhớ của trình duyệt. Khi người dùng bấm **Đăng xuất (Logout)** và **Đăng nhập** bằng một tài khoản khác (ví dụ: chuyển từ Member sang Admin) trên cùng trình duyệt mà không tải lại trang (F5), danh sách thông báo cũ của Member vẫn còn lưu trong Store và hiển thị đè lên màn hình của Admin.
- **Giải pháp:** Cập nhật file [AppHeader.vue](file:///c:/Users/admin/Documents/BTL_FullStack/frontend/src/components/layout/AppHeader.vue) để lắng nghe sự thay đổi của `userId` (khi Đăng nhập/Đăng xuất). Khi phát hiện chuyển đổi tài khoản, Frontend sẽ tự động xóa sạch danh sách thông báo cũ (`notifStore.notifications = []`) và gọi API `loadNotifications()` để tải mới danh sách thông báo chính xác của tài khoản hiện tại.

### 2. Sửa lỗi "phải load trang mới thấy thông báo" và "giao task không thấy thông báo"
- **Nguyên nhân:** Lỗi phân biệt hoa-thường ở chuỗi User ID Guid làm sai lệch tên Group trong kết nối SignalR, khiến các thông báo đẩy thời gian thực (real-time push) không đến đúng tài khoản.
- **Giải pháp:** Chuyển đổi toàn bộ Group ID về định dạng chữ thường (`.ToLower()`) ở cả phía client nhận kết nối ([NotificationHub.cs](file:///c:/Users/admin/Documents/BTL_FullStack/Comment%20&%20Notify%20Service/QLDA_PCCV/Hubs/NotificationHub.cs)) và phía đẩy sự kiện ([NotificationEventService.cs](file:///c:/Users/admin/Documents/BTL_FullStack/Comment%20&%20Notify%20Service/QLDA_PCCV/Services/NotificationEventService.cs)).

### 3. Thông báo khi Thêm thành viên vào dự án
- **[ProjectDetailView.vue](file:///c:/Users/admin/Documents/BTL_FullStack/frontend/src/views/ProjectDetailView.vue)**: Tích hợp gọi API gửi sự kiện thông báo `member.added` ngay sau khi thêm thành viên thành công trên giao diện.

### 4. Thông báo khi Tạo / Giao Task cho thành viên
- **[taskStore.js](file:///c:/Users/admin/Documents/BTL_FullStack/frontend/src/stores/taskStore.js)**: Tích hợp gửi thông báo `task.assigned` khi tạo hoặc cập nhật phân công công việc.

## Hướng dẫn kiểm tra lại
1. **Kiểm tra đăng nhập chéo:** Hãy đăng xuất tài khoản hiện tại, đăng nhập bằng tài khoản khác. Danh sách thông báo sẽ hiển thị chính xác theo tài khoản đó ngay lập tức mà không bị đè thông báo của người cũ nữa.
2. **Kiểm tra nhận thông báo real-time:** Bạn không cần phải tải lại trang. Khi tài khoản khác thêm bạn vào dự án hoặc giao task cho bạn, thông báo sẽ lập tức hiển thị dạng popup/chuông báo trong nháy mắt.
