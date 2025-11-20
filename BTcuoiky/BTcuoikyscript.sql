USE [BTcuoiky]
GO

-- =============================================
-- 1. TẠO CÁC BẢNG "CHA" (KHÔNG CÓ KHÓA NGOẠI)
-- =========================================

-- Bảng Category (Danh mục sản phẩm)
-- Ví dụ: Kính mát, Gọng kính, Tròng kính...
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Category]') AND type in (N'U'))
BEGIN
    CREATE TABLE Category (
        CategoryID INT PRIMARY KEY,             -- Mã danh mục (Tự nhập, không tự tăng)
        CategoryName NVARCHAR(100) NOT NULL,    -- Tên danh mục
        Description NVARCHAR(255)               -- Mô tả thêm
    );
END
GO

-- Bảng Customer (Khách hàng)
-- Lưu thông tin người dùng đăng ký và mua hàng
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Customer]') AND type in (N'U'))
BEGIN
    CREATE TABLE Customer (
        CustomerID NVARCHAR(10) PRIMARY KEY,    -- Mã khách hàng (Tự tạo hoặc Random string)
        FullName NVARCHAR(100) NOT NULL,        -- Tên hiển thị
        Address NVARCHAR(255),                  -- Địa chỉ
        Phone VARCHAR(15),                      -- Số điện thoại
        
        -- Các cột mới thêm cho chức năng Đăng nhập/Phân quyền
        Email NVARCHAR(100) NOT NULL UNIQUE,    -- Email đăng nhập (Không trùng)
        Password NVARCHAR(100) NOT NULL,        -- Mật khẩu (Lưu ý: Nên mã hóa trong thực tế)
        Role NVARCHAR(50) NOT NULL DEFAULT 'User' -- Quyền: 'User' hoặc 'Admin'
    );
END
GO

-- Bảng Tag (Thẻ)
-- Dùng để gắn nhãn cho sản phẩm (VD: 'Mới', 'Hot', 'Sale')
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Tag]') AND type in (N'U'))
BEGIN
    CREATE TABLE Tag (
        TagID INT PRIMARY KEY,              -- Mã thẻ
        TagName NVARCHAR(50) NOT NULL       -- Tên thẻ
    );
END
GO


-- =============================================
-- 2. TẠO CÁC BẢNG "CON" (CÓ KHÓA NGOẠI CẤP 1)
-- =============================================

-- Bảng Product (Sản phẩm)
-- Phụ thuộc vào bảng Category
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Product]') AND type in (N'U'))
BEGIN
    CREATE TABLE Product (
        ProductID INT PRIMARY KEY,              -- Mã sản phẩm
        ProductName NVARCHAR(100) NOT NULL,     -- Tên sản phẩm
        Unit NVARCHAR(50),                      -- Đơn vị tính
        Price DECIMAL(18, 2),                   -- Giá bán
        CategoryID INT,                         -- Mã danh mục (FK)
        image_uri VARCHAR(255) NULL,            -- Đường dẫn ảnh sản phẩm
        
        -- Khóa ngoại liên kết đến Category
        CONSTRAINT FK_Product_Category FOREIGN KEY (CategoryID) REFERENCES Category(CategoryID)
    );
END
GO

-- Bảng Order (Đơn hàng)
-- Phụ thuộc vào bảng Customer
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Order]') AND type in (N'U'))
BEGIN
    CREATE TABLE [Order] (
        OrderID INT PRIMARY KEY,                -- Mã đơn hàng
        OrderDate DATE,                         -- Ngày đặt hàng
        ShippedDate DATE,                       -- Ngày giao hàng
        CustomerID NVARCHAR(10),                -- Mã khách hàng (FK)
        ShipAddress NVARCHAR(255),              -- Địa chỉ giao hàng
        
        -- Khóa ngoại liên kết đến Customer
        CONSTRAINT FK_Order_Customer FOREIGN KEY (CustomerID) REFERENCES Customer(CustomerID)
    );
END
GO


-- =============================================
-- 3. TẠO CÁC BẢNG CHI TIẾT & TRUNG GIAN (CẤP 2)
-- =============================================

-- Bảng OrderDetail (Chi tiết đơn hàng)
-- Quan hệ nhiều-nhiều giữa Order và Product
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[OrderDetail]') AND type in (N'U'))
BEGIN
    CREATE TABLE OrderDetail (
        OrderID INT NOT NULL,                   -- FK tới Order
        ProductID INT NOT NULL,                 -- FK tới Product
        Quantity INT,                           -- Số lượng
        Discount FLOAT,                         -- Giảm giá
        
        -- Khóa chính phức hợp (Composite Key)
        CONSTRAINT PK_OrderDetail PRIMARY KEY (OrderID, ProductID),
        
        -- Các khóa ngoại
        CONSTRAINT FK_OrderDetail_Order FOREIGN KEY (OrderID) REFERENCES [Order](OrderID),
        CONSTRAINT FK_OrderDetail_Product FOREIGN KEY (ProductID) REFERENCES Product(ProductID)
    );
END
GO

-- Bảng ProductTag (Bảng trung gian cho Tag)
-- Quan hệ nhiều-nhiều giữa Product và Tag
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ProductTag]') AND type in (N'U'))
BEGIN
    CREATE TABLE ProductTag (
        ProductID INT NOT NULL,             -- FK tới Product
        TagID INT NOT NULL,                 -- FK tới Tag
        
        -- Khóa chính phức hợp
        CONSTRAINT PK_ProductTag PRIMARY KEY (ProductID, TagID),
        
        -- Các khóa ngoại (ON DELETE CASCADE: Xóa Product/Tag thì xóa luôn liên kết)
        CONSTRAINT FK_ProductTag_Product FOREIGN KEY (ProductID) REFERENCES Product(ProductID) ON DELETE CASCADE,
        CONSTRAINT FK_ProductTag_Tag FOREIGN KEY (TagID) REFERENCES Tag(TagID) ON DELETE CASCADE
    );
END
GO


-- =============================================
-- 4. THÊM DỮ LIỆU MẪU (OPTIONAL)
-- =============================================

-- Thêm dữ liệu mẫu cho Category (nếu chưa có)
IF NOT EXISTS (SELECT 1 FROM Category)
BEGIN
    INSERT INTO Category (CategoryID, CategoryName, Description)
    VALUES 
    (1, N'Kính mát', N'Các loại kính râm thời trang, chống UV'),
    (2, N'Gọng kính cận', N'Gọng kính nhựa, kim loại, titan...'),
    (3, N'Tròng kính', N'Tròng cận, viễn, loạn, chống ánh sáng xanh'),
    (4, N'Kính áp tròng', N'Lens cận, lens màu, nước ngâm');
END

-- Thêm dữ liệu mẫu cho Tag (nếu chưa có)
IF NOT EXISTS (SELECT 1 FROM Tag)
BEGIN
    INSERT INTO Tag (TagID, TagName)
    VALUES 
    (1, N'Mới'),
    (2, N'Bán chạy'),
    (3, N'Khuyến mãi');
END