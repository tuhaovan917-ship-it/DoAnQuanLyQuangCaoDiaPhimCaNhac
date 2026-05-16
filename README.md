# 💿🎬🎵 HỆ THỐNG QUẢN LÝ VÀ BÁN ĐĨA PHIM CA NHẠC

Nền tảng E-commerce đa người bán (**Multi-vendor**) được xây dựng trên nền tảng **ASP.NET MVC 5**, **Entity Framework 6** và **SQL Server**, cho phép quản lý và giao dịch các sản phẩm đĩa phim ca nhạc. Dự án tập trung vào việc cung cấp một hệ thống mạnh mẽ, linh hoạt với các tính năng nghiệp vụ phức tạp và trải nghiệm người dùng tối ưu.

---

## 📂 1. Cấu Trúc Thư Mục (Folder Structure)

Dự án tuân thủ nghiêm ngặt kiến trúc ASP.NET MVC, phân tách minh bạch giữa logic xử lý, tầng dữ liệu và giao diện hiển thị.

```text
. 
├── DoAnLapTrinhWeb/                 # Thư mục gốc của dự án ASP.NET MVC
│   ├── App_Start/                   # Cấu hình khởi tạo ứng dụng (Routes, Bundles, Filters)
│   │   ├── BundleConfig.cs          # Cấu hình gộp và nén tài nguyên CSS/JS
│   │   ├── FilterConfig.cs          # Cấu hình các bộ lọc toàn cục
│   │   ├── RouteConfig.cs           # Cấu hình định tuyến URL cho MVC
│   │   └── WebApiConfig.cs          # Cấu hình định tuyến cho Web API
│   ├── Content/                     # Tài nguyên tĩnh (CSS, hình ảnh)
│   │   ├── Images/                  # Thư mục chứa các ảnh sản phẩm và banner
│   │   ├── Site.css                 # CSS tùy chỉnh của ứng dụng
│   │   └── site-custom.css          # CSS tùy chỉnh bổ sung (ví dụ: card sản phẩm)
│   ├── Controllers/                 # Chứa các Controller xử lý logic nghiệp vụ và điều hướng
│   │   ├── API/                     # Các API Controller (ví dụ: CategoryApiController)
│   │   ├── AccountController.cs     # Xử lý đăng nhập, đăng ký, phân quyền người dùng
│   │   ├── CartController.cs        # Xử lý giỏ hàng và quy trình thanh toán
│   │   ├── HomeController.cs        # Các chức năng chính của trang chủ (tìm kiếm, lọc sản phẩm)
│   │   └── SellerController.cs      # Các chức năng dành cho người bán (quản lý sản phẩm, đơn hàng, doanh thu)
│   │   └── ...
│   ├── Models/                      # Chứa các lớp Model (Entities, DTOs, ViewModels) và cấu hình Entity Framework
│   │   ├── DTOs/                    # Data Transfer Objects (ví dụ: DiaDTO.cs, PagedResult.cs)
│   │   ├── ViewModels/              # View Models (ví dụ: TopProductViewModel.cs)
│   │   ├── QuanLyQuangCaoDiaPhimCaNhac.edmx # Sơ đồ Entity Data Model (Database First)
│   │   |   ├── QuanLyQuangCaoDiaPhimCaNhac.Context.cs # Context của Entity Framework
│   │   |   ├── QuanLyQuangCaoDiaPhimCaNhac.Designer.cs # Phần thiết kế của EDMX
│   │   |   └── ...                      # Các file entity .cs khác (DiaPhimCaNhac.cs, KhachHang.cs, DonHang.cs)
│   ├── Scripts/                     # Các thư viện JavaScript (jQuery, Bootstrap JS)
│   ├── Views/                       # Chứa các View (.cshtml) hiển thị giao diện người dùng
│   │   ├── Account/                 # Views liên quan đến tài khoản
│   │   ├── Cart/                    # Views giỏ hàng
│   │   ├── Category/                # Views quản lý danh mục
│   │   ├── Home/                    # Views trang chủ và sản phẩm
│   │   ├── Seller/                  # Views dành cho người bán
│   │   └── Shared/                  # Các View dùng chung (_Layout.cshtml, Error.cshtml)
│   ├── Global.asax                  # Cấu hình ứng dụng toàn cục (Application_Start)
│   ├── packages.config              # Danh sách các gói NuGet được sử dụng trong dự án
│   └── Web.config                   # Cấu hình ứng dụng (connection string, app settings, Entity Framework)

```

## 🚀 2. Tính Năng Chính (Key Features)
- **Quản lý Sản phẩm (CRUD):** Thêm, sửa, xóa và theo dõi chi tiết đĩa phim/nhạc.

- **Tìm kiếm & Lọc thông minh:** Tra cứu nhanh theo từ khóa, thể loại, định dạng và khoảng giá bằng AJAX (không tải lại trang).

- **Giỏ hàng & Thanh toán:** Cập nhật giỏ hàng trực quan; tự động tách đơn hàng theo nhà cung cấp (Multi-vendor Order Splitting).

- **Quản lý tồn kho:** Tự động cập nhật số lượng kho ngay khi thanh toán thành công.

- **Xác thực & Phân quyền (RBAC):** Đăng ký/đăng nhập và kiểm soát quyền truy cập riêng biệt cho Khách hàng, Người bán và Admin.

- **Thống kê & Báo cáo:** Biểu đồ doanh thu trực quan, theo dõi tăng trưởng tháng và sản phẩm bán chạy.

## 🛠 3. Công Nghệ Sử Dụng (Tech Stack)

🖥️ Backend
- **Ngôn ngữ:** `C#`
- **Framework:** `ASP.NET MVC 5`
- **Tầng dữ liệu (ORM):** `Entity Framework 6` (Tiếp cận theo hướng Database First)
- **Dịch vụ API:** `ASP.NET Web API 2`

🗄️ Database
- **Hệ quản trị:** `Microsoft SQL Server`

🎨 Frontend
- **Công nghệ cốt lõi:** `HTML5`, `CSS3`, `JavaScript (ES6+)`
- **UI Framework:** `Bootstrap 5.2.3`
- **Thư viện bổ trợ:** `jQuery 3.7.0`, `jQuery Validation`, `jQuery Unobtrusive Validation` để đồng bộ hóa và xác thực dữ liệu phía Client.

⚙️ Công cụ phát triển
- **IDE:** `Visual Studio` (Tối ưu trên các phiên bản tương thích tốt với .NET Framework 4.7.2)

## 📸 4. Giao Diện Ứng Dụng Tiêu Biểu (Screenshots)
(Bổ sung sau)

## 💡 5. Điểm Nhấn Kỹ Thuật (Technical Highlights)
1. **Kiến trúc Multi-vendor & Tách đơn hàng:** Tự động phân tích và bóc tách giỏ hàng tổng thành các đơn hàng con theo từng người bán, đảm bảo minh bạch dòng tiền và đồng bộ tồn kho chính xác.

2. **Tối ưu hóa LINQ nâng cao:** Sử dụng GroupBy, Include, Select để trích xuất dữ liệu đa tầng; tách biệt truy vấn thô và định dạng hiển thị trong bộ nhớ (In-memory Formatting) giúp giảm tải cho SQL Server.

3. **Phân quyền RBAC & Luồng an toàn:** Quản lý trạng thái qua Session để bảo vệ tài nguyên từ tầng Controller; bao bọc các nghiệp vụ nhạy cảm (như thanh toán) bằng try-catch chặt chẽ nhằm kiểm soát lỗi hệ thống.

4. **Tương tác mượt mà với AJAX:** Tích hợp các AJAX Endpoints xử lý cập nhật giỏ hàng thời gian thực và bộ lọc động, tối ưu hóa băng thông và hạn chế tải lại trang.

## 💻 6. Cài Đặt và Khởi Chạy (Installation)
Để chạy dự án này trên máy cục bộ, bạn cần thực hiện các bước sau:

### 6.1. Tiền đề (Prerequisites)

Để khởi chạy dự án, máy tính của bạn cần có:

*   **Visual Studio 2022** (hoặc 2019) có cài đặt workload *ASP.NET and web development*
*   **SQL Server Management Studio (SSMS)** hoặc SQL Server Express
*   **.NET Framework 4.7.2** hoặc phiên bản tương đương

### 6.2. Các bước thực hiện

#### Bước 1: Tải mã nguồn về máy

```bash
git clone https://github.com/tuhaovan917-ship-it/DoAnQuanLyQuangCaoDiaPhimCaNhac.git
cd DoAnQuanLyQuangCaoDiaPhimCaNhac
```

#### Bước 2: Thiết lập Cơ sở dữ liệu

1. Mở **SQL Server Management Studio (SSMS)**.
2. Tạo một Database mới với tên: `QuanLyDiaPhimCaNhac_Edited`.
3. Mở và thực thi (**Execute**) tệp script SQL `Backup.sql` để khởi tạo cấu trúc bảng và dữ liệu mẫu.

#### Bước 3: Cấu hình Chuỗi kết nối (Connection String)

1. Mở file solution `DoAnQuanLyQuangCaoDiaPhimCaNhac.sln` bằng **Visual Studio**.
2. Tìm đến file `Web.config` trong thư mục gốc của dự án (`DoAnLapTrinhWeb`).
3. Cập nhật thuộc tính `connectionString` trong thẻ `<connectionStrings>` sao cho khớp với tên SQL Server trên máy của bạn, áp dụng cho connection string có tên `QuanLyDiaPhimCaNhac_EditedEntities`.

Ví dụ cấu trúc trong `Web.config`:
```xml
<add name="QuanLyDiaPhimCaNhac_EditedEntities"
     connectionString="metadata=res://*/Models.QuanLyQuangCaoDiaPhimCaNhac.csdl|res://*/Models.QuanLyQuangCaoDiaPhimCaNhac.ssdl|res://*/Models.QuanLyQuangCaoDiaPhimCaNhac.msl;provider=System.Data.SqlClient;provider connection string=&quot;
     data source=TEN_MAY_CUA_BAN\SQLEXPRESS;
     initial catalog=QuanLyDiaPhimCaNhac_Edited;
     integrated security=True;
     MultipleActiveResultSets=True;
     App=EntityFramework&quot;"
     providerName="System.Data.EntityClient" />
```

> **Lưu ý:** Thay `TEN_MAY_CUA_BAN\SQLEXPRESS` bằng tên SQL Server thực tế trên máy của bạn.

#### Bước 4: Khởi chạy dự án

1. Trong **Visual Studio**, nhấp chuột phải vào **Solution** $\rightarrow$ chọn **Restore NuGet Packages** để tự động tải lại các thư viện cần thiết.
2. Nhấn phím **F5** hoặc bấm vào nút **Start (IIS Express)** trên thanh công cụ để khởi chạy và trải nghiệm website.

## 💡 Mẹo Khi Phát Triển

- **NuGet Restore:** Nếu project báo lỗi thư viện khi vừa mở, hãy chọn **Build Solution** để Visual Studio tự động tải lại package còn thiếu.
- **Database First (.edmx):** Nếu thay đổi cấu trúc Database trong SQL Server, hãy mở file `.edmx` → chuột phải → chọn **Update Model from Database** để đồng bộ dữ liệu vào project.
