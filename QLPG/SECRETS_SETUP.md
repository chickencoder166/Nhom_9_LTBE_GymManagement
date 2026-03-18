# Hướng Dẫn Thiết Lập User Secrets

Dự án này dùng Azure Key Vault User Secrets để lưu cấu hình nhạy cảm trong môi trường phát triển. Làm theo các bước sau:

## Các Bước Cài Đặt

### 1. Khởi tạo User Secrets (một lần)
```bash
cd QLPG
dotnet user-secrets init
```

### 2. Thiết lập chuỗi kết nối
Thay `YOUR_SERVER` bằng tên SQL Server của bạn (ví dụ: `(localdb)\mssqllocaldb`, `DESKTOP-ABC123`, ...):

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=YOUR_SERVER;Database=QLPG;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
```

### 3. (Tùy chọn) Thiết lập cấu hình email
Nếu bạn muốn bật gửi email thông báo:

```bash
dotnet user-secrets set "Email:SmtpUser" "your-email@gmail.com"
dotnet user-secrets set "Email:SmtpPass" "your-app-password"
```

## Kiểm tra secrets

Liệt kê tất cả secrets:
```bash
dotnet user-secrets list
```

Xóa một secret cụ thể:
```bash
dotnet user-secrets remove "ConnectionStrings:DefaultConnection"
```

## Ghi chú

- User secrets được lưu tại `%APPDATA%\Microsoft\UserSecrets\qlpg-gym-management-v1\` trên Windows
- User secrets **KHÔNG BAO GIỜ** được commit vào git (chúng sẽ ghi đè `appsettings.json` trong môi trường dev)
- Với production, hãy dùng biến môi trường thực tế hoặc Azure Key Vault
- `appsettings.json` hiện có sẵn chuỗi kết nối mặc định cho phát triển local (localdb)

## Xử lý sự cố

Nếu `dotnet user-secrets init` bị lỗi, hãy đảm bảo:
- Đã cài .NET 8 SDK
- File `QLPG.csproj` có `<UserSecretsId>qlpg-gym-management-v1</UserSecretsId>` trong `PropertyGroup`
