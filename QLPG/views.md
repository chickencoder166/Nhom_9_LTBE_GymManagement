# Views

Dự án QLSV-DB sử dụng Razor Views để hiển thị giao diện người dùng. Các views được tổ chức theo folder tương ứng với controller.

## Home Views
- **Index.cshtml**: Trang chủ của ứng dụng.
- **Privacy.cshtml**: Trang chính sách bảo mật.

## SinhVien Views
- **Index.cshtml**: Danh sách sinh viên với các liên kết đến Details, Edit, Delete.
- **Details.cshtml**: Hiển thị chi tiết thông tin một sinh viên.
- **Create.cshtml**: Form tạo sinh viên mới.
- **Edit.cshtml**: Form chỉnh sửa thông tin sinh viên.
- **Delete.cshtml**: Xác nhận xóa sinh viên.

## HocPhan Views
- **Index.cshtml**: Danh sách học phần.
- **Details.cshtml**: Chi tiết học phần.
- **Create.cshtml**: Form tạo học phần.
- **Edit.cshtml**: Form chỉnh sửa học phần.
- **Delete.cshtml**: Xác nhận xóa học phần.

## DiemHocPhan Views
- **Index.cshtml**: Danh sách điểm học phần, hiển thị sinh viên, học phần và điểm.
- **Create.cshtml**: Form tạo điểm học phần với dropdown chọn sinh viên và học phần.
- **Edit.cshtml**: Form chỉnh sửa điểm học phần.
- **Delete.cshtml**: Xác nhận xóa điểm học phần.

## Shared Views
- **_Layout.cshtml**: Layout chính của ứng dụng, bao gồm header, navigation, footer.
- **_ValidationScriptsPartial.cshtml**: Scripts cho validation.
- **Error.cshtml**: Trang lỗi.

Các views sử dụng Bootstrap cho styling và Razor syntax để render dữ liệu từ models.