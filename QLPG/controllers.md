# Controllers

Dự án QLSV-DB sử dụng các controller sau để xử lý logic nghiệp vụ và tương tác với views:

## SinhVienController
- **Mô tả**: Quản lý các thao tác CRUD cho SinhVien.
- **Actions**:
  - `Index`: Hiển thị danh sách tất cả sinh viên.
  - `Details(int? id)`: Hiển thị chi tiết một sinh viên.
  - `Create()` (GET): Hiển thị form tạo sinh viên mới.
  - `Create(SinhVien)` (POST): Tạo sinh viên mới và lưu vào DB.
  - `Edit(int? id)` (GET): Hiển thị form chỉnh sửa sinh viên.
  - `Edit(int, SinhVien)` (POST): Cập nhật thông tin sinh viên.
  - `Delete(int? id)` (GET): Hiển thị xác nhận xóa sinh viên.
  - `DeleteConfirmed(int)` (POST): Xóa sinh viên khỏi DB.

## HocPhanController
- **Mô tả**: Quản lý các thao tác CRUD cho HocPhan.
- **Actions** tương tự SinhVienController, thay thế SinhVien bằng HocPhan.

## DiemHocPhanController
- **Mô tả**: Quản lý các thao tác CRUD cho DiemHocPhan, bao gồm quan hệ với SinhVien và HocPhan.
- **Actions**:
  - `Index`: Hiển thị danh sách điểm học phần với thông tin sinh viên và học phần.
  - `Create()` (GET): Hiển thị form tạo điểm học phần với dropdown cho SinhVien và HocPhan.
  - `Create(DiemHocPhan)` (POST): Tạo điểm học phần mới.
  - `Edit(int? id)` (GET): Hiển thị form chỉnh sửa với dropdown.
  - `Edit(int, DiemHocPhan)` (POST): Cập nhật điểm học phần.
  - `Delete(int? id)` (GET): Hiển thị xác nhận xóa.
  - `DeleteConfirmed(int)` (POST): Xóa điểm học phần.

## HomeController
- **Mô tả**: Controller mặc định cho trang chủ.
- **Actions**: Index, Privacy (từ template ASP.NET Core).

Tất cả controllers đều sử dụng ApplicationDbContext để tương tác với DB và trả về Views tương ứng.

## Điểm Đặc Biệt và Cú Pháp trong Controllers

### 1. Dependency Injection
- **Cú pháp**: `private readonly ApplicationDbContext _context; public SinhVienController(ApplicationDbContext context) { _context = context; }`
- **Giải thích**: ASP.NET Core sử dụng Dependency Injection để inject ApplicationDbContext vào controller. Điều này cho phép truy cập cơ sở dữ liệu mà không cần tạo instance thủ công, giúp dễ dàng testing và quản lý lifecycle.

### 2. Asynchronous Programming
- **Cú pháp**: `public async Task<IActionResult> Index() { return View(await _context.SinhViens.ToListAsync()); }`
- **Giải thích**: Sử dụng `async` và `await` với Entity Framework Core để thực hiện các thao tác DB bất đồng bộ, tránh block thread và cải thiện performance. `ToListAsync()` thay vì `ToList()`.

### 3. Model Binding và Validation
- **Cú pháp**: `[HttpPost] public async Task<IActionResult> Create(SinhVien sinhVien) { if (ModelState.IsValid) { ... } return View(sinhVien); }`
- **Giải thích**: ASP.NET Core tự động bind dữ liệu từ form vào model object. `ModelState.IsValid` kiểm tra validation dựa trên Data Annotations trong model (như `[Required]`).

### 4. Anti-Forgery Token
- **Cú pháp**: `[ValidateAntiForgeryToken]`
- **Giải thích**: Bảo vệ chống lại Cross-Site Request Forgery (CSRF) attacks. Token được generate trong view và validate trong POST actions.

### 5. Exception Handling
- **Cú pháp**: `catch (DbUpdateConcurrencyException) { if (!_context.SinhViens.Any(e => e.Id == id)) return NotFound(); else throw; }`
- **Giải thích**: Xử lý ngoại lệ khi có conflict trong cập nhật dữ liệu (concurrency). Kiểm tra xem entity có tồn tại không trước khi throw exception.

### 6. Eager Loading với Include
- **Cú pháp**: `await _context.DiemHocPhans.Include(d => d.SinhVien).Include(d => d.HocPhan).ToListAsync()`
- **Giải thích**: Sử dụng `Include` để load các navigation properties (quan hệ) cùng lúc, tránh N+1 query problem. Cần thiết khi hiển thị dữ liệu liên quan.

### 7. SelectList cho Dropdowns
- **Cú pháp**: `ViewData["SinhVienId"] = new SelectList(sinhVienList, "Id", "HienThi");`
- **Giải thích**: Tạo dropdown list trong view. `SelectList` nhận danh sách items, property cho value, và property cho text hiển thị.

### 8. ActionName Attribute
- **Cú pháp**: `[HttpPost, ActionName("Delete")] public async Task<IActionResult> DeleteConfirmed(int id)`
- **Giải thích**: Cho phép POST action có tên khác với GET action tương ứng, thường dùng cho delete confirmation để tránh expose action name trực tiếp.

### 9. Bind Attribute
- **Cú pháp**: `public async Task<IActionResult> Edit(int id, [Bind("Id,MaSinhVien,HoVaTen")] SinhVien student)`
- **Giải thích**: Chỉ bind các thuộc tính được chỉ định từ request data, tăng bảo mật bằng cách tránh over-posting attacks.

### 10. RedirectToAction
- **Cú pháp**: `return RedirectToAction(nameof(Index));`
- **Giải thích**: Sau khi thực hiện thành công CRUD operation, redirect về trang danh sách. `nameof(Index)` đảm bảo tên action được cập nhật nếu đổi tên method.