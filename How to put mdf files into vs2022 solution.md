Dựa vào hình ảnh `image_6a9fb4.png` mà bạn gửi, tôi xác nhận: **Dự án của bạn đã hoàn toàn ỔN ĐỊNH.**

Các bảng dữ liệu (`Category`, `Customer`, `Product`...) đã xuất hiện đầy đủ trong thư mục `Tables` của database `BTcuoiky` trên `(LocalDB)\MSSQLLocalDB`. Điều này nghĩa là bạn đã tạo thành công cơ sở dữ liệu và nó đã sẵn sàng để website kết nối.

Dưới đây là **Quy trình chuẩn** mà chúng ta đã thực hiện. Bạn hãy lưu lại hướng dẫn này để sau này làm dự án khác hoặc gửi cho bạn bè nhé.

-----

### 📖 HƯỚNG DẪN TẠO DATABASE CHẠY TRÊN MỌI MÁY (Visual Studio 2022)

**Vấn đề đã gặp:** File `.mdf` copy từ máy này sang máy khác thường bị lỗi "Version không tương thích" (969 vs 904) hoặc lỗi đường dẫn.
**Giải pháp:** Sử dụng **SQL Script** để tạo lại database mới sạch sẽ trên từng máy.

#### Bước 1: Chuẩn bị Script "Sạch"

Mở file `.sql` của bạn và đảm bảo đoạn đầu tiên luôn trông như thế này (để tránh lỗi đường dẫn cứng và xóa cái cũ nếu có):

```sql
USE [master]
GO

-- 1. Xóa database cũ nếu tồn tại (để Reset)
IF EXISTS (SELECT name FROM sys.databases WHERE name = N'BTcuoiky')
BEGIN
    ALTER DATABASE [BTcuoiky] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [BTcuoiky];
END
GO

-- 2. Tạo Database mới (Để tự động, không ép đường dẫn C:\...)
CREATE DATABASE [BTcuoiky]
GO

-- 3. Sử dụng Database vừa tạo
USE [BTcuoiky]
GO

-- ... (Phần còn lại là code tạo bảng và insert dữ liệu của bạn) ...
```

*Lưu ý: Xóa bỏ các dòng lệnh kén phiên bản như `LEDGER = ON/OFF` hoặc `COMPATIBILITY_LEVEL`.*

#### Bước 2: Chạy Script trong Visual Studio

1.  Mở file Script `.sql` trong Visual Studio.
2.  Nhấn nút **Connect** (hoặc icon phích cắm) trên thanh công cụ của cửa sổ code.
3.  Điền thông tin kết nối:
      * **Server Name:** `(LocalDB)\MSSQLLocalDB`
      * **Authentication:** Windows Authentication.
      * **Database:** Để trống hoặc chọn `<default>`.
4.  Nhấn **Connect**.
5.  Nhấn nút **Execute** (hoặc phím `F5`).
6.  Đợi thông báo: `Command(s) completed successfully`.

#### Bước 3: Cấu hình Web.config

Mở file `Web.config` và sửa đoạn `connectionStrings` để trỏ đúng vào database tên là `BTcuoiky` trên LocalDB:

```xml
<connectionStrings>
    <add name="DefaultConnection" 
         connectionString="Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=BTcuoiky;Integrated Security=True" 
         providerName="System.Data.SqlClient" />
    
    <add name="BTcuoikyEntities" 
         connectionString="metadata=res://*/Models.ShopModel.csdl|res://*/Models.ShopModel.ssdl|res://*/Models.ShopModel.msl;provider=System.Data.SqlClient;provider connection string=&quot;data source=(LocalDB)\MSSQLLocalDB;initial catalog=BTcuoiky;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework&quot;" 
         providerName="System.Data.EntityClient" />
</connectionStrings>
```

#### Bước 4: Kiểm tra kết quả

1.  Mở **View** \> **SQL Server Object Explorer**.
2.  Tìm đến: `SQL Server` \> `(LocalDB)\MSSQLLocalDB` \> `Databases`.
3.  Nếu chưa thấy `BTcuoiky`, chuột phải vào `(LocalDB)...` chọn **Refresh**.
4.  Mở rộng `BTcuoiky` \> `Tables` để xem các bảng.
5.  Chuột phải vào bảng bất kỳ (ví dụ `Product`) \> **View Data** để kiểm tra dữ liệu.

-----

### 🤝 Quy trình làm việc nhóm (GitHub)

Khi bạn bè của bạn Pull code về, họ chỉ cần làm đúng **Bước 2** (Mở script lên và ấn nút Chạy).

  * Không cần sửa `Web.config` (vì bạn đã chỉnh chuẩn rồi).
  * Không cần cài đặt SQL Server phức tạp (vì ai dùng Visual Studio cũng có sẵn LocalDB).

Chúc mừng bạn đã hoàn thành việc setup này\! Giờ bạn có thể yên tâm code tiếp các tính năng khác.
