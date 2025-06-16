# Financial Project Version 3

Ứng dụng quản lý tài chính cá nhân được phát triển bằng ASP.NET Core MVC, giúp người dùng theo dõi thu chi và quản lý tài chính cá nhân hiệu quả.

## 🚀 Tính năng chính

### 👤 Quản lý tài khoản
- **Đăng ký/Đăng nhập**: Hệ thống xác thực với Cookie Authentication
- **Quản lý hồ sơ**: Cập nhật thông tin cá nhân, email, họ tên
- **Avatar**: Upload và quản lý ảnh đại diện
- **Đổi mật khẩu**: Thay đổi mật khẩu với xác thực mật khẩu cũ
- **Bảo mật**: Mã hóa mật khẩu bằng PBKDF2 với salt

### 💰 Quản lý giao dịch
- **Thêm giao dịch**: Tạo giao dịch thu/chi với mô tả, số tiền, ngày tháng
- **Phân loại giao dịch**: Gán danh mục cho từng giao dịch
- **Xem danh sách**: Hiển thị và lọc giao dịch theo các tiêu chí
- **Chỉnh sửa/Xóa**: Quản lý giao dịch đã tạo
- **Giao dịch gần đây**: Xem các giao dịch mới nhất
- **Tìm kiếm**: Tìm kiếm giao dịch theo từ khóa
- **Lọc giao dịch**: Lọc theo danh mục, loại giao dịch, ngày tháng

### 🏷️ Quản lý danh mục
- **Tạo danh mục**: Tạo danh mục thu nhập và chi tiêu
- **Icon danh mục**: Upload icon cho từng danh mục
- **Phân loại**: Danh mục thu nhập (income) và chi tiêu (expense)

### 📊 Dashboard & Báo cáo
- **Tổng quan tài chính**: Hiển thị tổng thu nhập, chi tiêu, số dư
- **Thống kê tháng**: Thu chi trong tháng hiện tại
- **Biểu đồ đường**: Thu chi 6 tháng gần nhất (Chart.js)
- **Biểu đồ tròn**: Phân bố chi tiêu theo danh mục
- **Giao dịch gần đây**: 8 giao dịch mới nhất

### 👨‍💼 Quản trị hệ thống (Admin)
- **Quản lý người dùng**: Xem danh sách tất cả user
- **Khóa/Mở khóa**: Block/Unblock tài khoản người dùng
- **Phân quyền**: Hệ thống role-based (admin/user)

## 🛠️ Công nghệ sử dụng

- **Backend**: ASP.NET Core 8.0 MVC
- **Database**: SQL Server với Entity Framework Core
- **Frontend**: HTML, CSS, Bootstrap, JavaScript
- **Biểu đồ**: Chart.js
- **Authentication**: Cookie Authentication
- **Security**: PBKDF2 password hashing
- **Icons**: Font Awesome

## 📋 Yêu cầu hệ thống

- .NET 8.0 SDK
- SQL Server 2019+
- Visual Studio 2022 hoặc Visual Studio Code
- Windows 10+ / macOS / Linux

## ⚡ Cài đặt và chạy ứng dụng

### 1. Clone repository
```bash
git clone https://github.com/dawnmoriaty/FinacialProjectVersion3.git
cd FinacialProjectVersion3
```

### 2. Cấu hình database
```bash
# Cập nhật connection string trong appsettings.json
# Tạo database và chạy migration
dotnet ef database update
```

### 3. Cài đặt dependencies
```bash
dotnet restore
```

### 4. Chạy ứng dụng
```bash
dotnet run
```

Ứng dụng sẽ chạy tại: "http://localhost:5091"

## 📊 Cấu trúc dự án

```
FinacialProjectVersion3/
├── Controllers/          # MVC Controllers
│   ├── AccountController.cs
│   ├── TransactionController.cs
│   ├── CategoryController.cs
│   ├── DashboardController.cs
│   └── AdminController.cs
├── Models/              # Entity Models
│   └── Entity/
│       ├── User.cs
│       ├── Transaction.cs
│       └── Category.cs
├── Services/            # Business Logic Layer
│   ├── IUserService.cs
│   ├── ITransactionService.cs
│   ├── ICategoryService.cs
│   └── IDashboardService.cs
├── Repository/          # Data Access Layer
├── ViewModels/          # View Models
├── Views/               # Razor Views
├── wwwroot/            # Static files
├── Data/               # DbContext
└── Utils/              # Utilities (PasswordHasher, ServiceResult)
```

## 🎯 Cách sử dụng

### Bước 1: Đăng ký tài khoản
1. Truy cập trang đăng ký
2. Nhập username, email, password
3. Đăng nhập sau khi đăng ký thành công

### Bước 2: Tạo danh mục
1. Vào **Danh mục** → **Thêm mới**
2. Nhập tên danh mục và chọn loại (Thu nhập/Chi tiêu)
3. Upload icon cho danh mục (tùy chọn)

### Bước 3: Quản lý giao dịch
1. Vào **Giao dịch** → **Thêm mới**
2. Nhập mô tả, số tiền, chọn danh mục và ngày
3. Xem danh sách và quản lý giao dịch

### Bước 4: Xem báo cáo
1. Truy cập **Dashboard**
2. Xem tổng quan tài chính và biểu đồ
3. Theo dõi xu hướng thu chi qua các tháng

## 🔒 Bảo mật

- **Mật khẩu**: Mã hóa bằng PBKDF2 với 10,000 iterations
- **Authentication**: Cookie-based với thời gian sống 7 ngày
- **Authorization**: Role-based access control
- **File Upload**: Giới hạn kích thước 10MB cho avatar và icon

## 🤝 Đóng góp

1. Fork repository
2. Tạo feature branch (`git checkout -b feature/TenTinhNang`)
3. Commit changes (`git commit -m 'Thêm tính năng XYZ'`)
4. Push to branch (`git push origin feature/TenTinhNang`)
5. Tạo Pull Request

## 📞 Liên hệ

**Developer**: dawnmoriaty  
**GitHub**: [@dawnmoriaty](https://github.com/dawnmoriaty)  
**Project**: [FinacialProjectVersion3](https://github.com/dawnmoriaty/FinacialProjectVersion3)

---

*Dự án được phát triển như một ứng dụng quản lý tài chính cá nhân đơn giản và hiệu quả.*