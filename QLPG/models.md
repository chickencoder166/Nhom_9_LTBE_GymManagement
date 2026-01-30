# Models

Dự án QLSV-DB sử dụng các model sau để đại diện cho dữ liệu trong cơ sở dữ liệu:

## SinhVien
- **Mô tả**: Đại diện cho thông tin của một sinh viên.
- **Thuộc tính**:
  - `Id` (int): Khóa chính, tự động tăng.
  - `MaSinhVien` (string): Mã sinh viên, bắt buộc, có validation.
  - `HoVaTen` (string): Họ và tên, bắt buộc, có validation.

## HocPhan
- **Mô tả**: Đại diện cho thông tin của một học phần.
- **Thuộc tính**:
  - `Id` (int): Khóa chính, tự động tăng.
  - `MaHocPhan` (string): Mã học phần, bắt buộc.
  - `TenHocPhan` (string): Tên học phần, bắt buộc.

## DiemHocPhan
- **Mô tả**: Đại diện cho điểm của sinh viên trong một học phần.
- **Thuộc tính**:
  - `Id` (int): Khóa chính, tự động tăng.
  - `SinhVienId` (int): Khóa ngoại tham chiếu đến SinhVien, bắt buộc.
  - `SinhVien` (SinhVien?): Navigation property.
  - `HocPhanId` (int): Khóa ngoại tham chiếu đến HocPhan, bắt buộc.
  - `HocPhan` (HocPhan?): Navigation property.
  - `DiemSo` (float): Điểm số, bắt buộc, trong khoảng 0-10.

Các model này sử dụng Entity Framework Core để ánh xạ với cơ sở dữ liệu, với các validation attributes để đảm bảo tính toàn vẹn dữ liệu.

## Điểm Đặc Biệt và Cú Pháp trong Models

### 1. Data Annotations cho Validation
- **Cú pháp**: `[Required(ErrorMessage = "Mã sinh viên là bắt buộc")]`
- **Giải thích**: Thuộc tính này đảm bảo field không được null hoặc empty. `ErrorMessage` tùy chỉnh thông báo lỗi hiển thị khi validation fail.

### 2. Display Attribute
- **Cú pháp**: `[Display(Name = "Mã sinh viên")]`
- **Giải thích**: Tùy chỉnh tên hiển thị của thuộc tính trong UI (như labels trong forms), thay vì dùng tên property.

### 3. Range Validation
- **Cú pháp**: `[Range(0, 10, ErrorMessage = "Điểm phải nằm trong khoảng từ 0 đến 10")]`
- **Giải thích**: Giới hạn giá trị của thuộc tính trong một khoảng, với thông báo lỗi tùy chỉnh.

### 4. Foreign Key Attribute
- **Cú pháp**: `[ForeignKey("SinhVienId")] public SinhVien? SinhVien { get; set; }`
- **Giải thích**: Xác định thuộc tính là khóa ngoại tham chiếu đến bảng khác. Entity Framework sử dụng để tạo quan hệ trong DB.

### 5. Navigation Properties
- **Cú pháp**: `public SinhVien? SinhVien { get; set; }`
- **Giải thích**: Thuộc tính virtual cho phép truy cập dữ liệu liên quan mà không cần query riêng. Sử dụng `Include` trong queries để load.

### 6. Nullable Types
- **Cú pháp**: `public SinhVien? SinhVien { get; set; }`
- **Giải thích**: Dấu `?` cho phép thuộc tính có giá trị null, phù hợp với navigation properties có thể không được load.

### 7. Primary Key Convention
- **Cú pháp**: `public int Id { get; set; }`
- **Giải thích**: Theo convention của EF Core, thuộc tính tên `Id` hoặc `ClassNameId` được tự động coi là primary key và auto-increment.

### 8. String Length Validation (Implicit)
- **Mặc dù không có trong code, có thể thêm**: `[StringLength(100)]`
- **Giải thích**: Giới hạn độ dài chuỗi, nhưng trong project này chưa sử dụng, có thể thêm cho các trường string nếu cần.

### 9. Data Type Attributes
- **Ví dụ**: `[DataType(DataType.Date)]` (không có trong code hiện tại)
- **Giải thích**: Chỉ định kiểu dữ liệu cho UI rendering, như date picker cho dates.

### 10. ScaffoldColumn Attribute
- **Ví dụ**: `[ScaffoldColumn(false)]` (không có trong code)
- **Giải thích**: Ẩn thuộc tính khỏi scaffolding (tự động generate views), hữu ích cho các field internal.

Các cú pháp này giúp model không chỉ đại diện dữ liệu mà còn cung cấp metadata cho validation, UI generation, và database schema creation.