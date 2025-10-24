using DoAnQuanLyQuangCaoDiaPhimCaNhac.Models;
using DoAnQuanLyQuangCaoDiaPhimCaNhac.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAnQuanLyQuangCaoDiaPhimCaNhac.Controllers
{
    public class DiaPhimNhacController : BaseController
    {
        QuanLyDiaPhimCaNhacEntities db = new QuanLyDiaPhimCaNhacEntities();
        // GET: DiaPhimNhac
        public ActionResult Index()
        {
            return View();
        }

        // Chi tiết đĩa
        public JsonResult ChiTietDiaPhimCaNhac(int maDia)
        {
            try
            {
                // 🟢 1. Ghi log lại mỗi lần API được gọi
                System.Diagnostics.Debug.WriteLine($"[API] Xem chi tiết đĩa {maDia} lúc {DateTime.Now}");

                // 🟢 2. Kiểm tra cache (nếu đã có sẵn thì trả về luôn)
                var cacheKey = "ChiTietDia_" + maDia;
                var cachedData = HttpContext.Cache[cacheKey];
                if (cachedData != null)
                {
                    return JsonNet(new { success = true, data = cachedData });
                }

                // 🟢 3. Nếu chưa có cache, truy vấn DB
                var result = db.sp_GetChiTietDiaPhimCaNhac(maDia).FirstOrDefault();
                if (result == null)
                {
                    return JsonNet(new { success = false, message = "Không tìm thấy sản phẩm" });
                }

                // 🟢 4. Lưu dữ liệu vào cache 10 giây
                HttpContext.Cache.Insert(
                    cacheKey,                  // key
                    result,                    // data cần lưu
                    null,                      // dependency (null = không phụ thuộc file nào)
                    DateTime.Now.AddSeconds(10), // thời gian hết hạn cache
                    System.Web.Caching.Cache.NoSlidingExpiration
                );

                // 🟢 5. Cập nhật lượt xem
                db.Database.ExecuteSqlCommand(
                    "UPDATE DiaPhimCaNhac SET SoLuotXem = ISNULL(SoLuotXem,0) + 1 WHERE MaDia = @p0",
                    maDia
                );

                // 🟢 6. Trả dữ liệu ra client
                return JsonNet(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Đã xảy ra lỗi, vui lòng thử lại sau." }, JsonRequestBehavior.AllowGet);
            }
        }

        // Phân đĩa theo loại
        public JsonResult GetByLoaiDia(int? maLoai, int pageIndex = 1, int pageSize = 12)
        {
            try
            {
                // 🟢 1. Ghi log truy cập API
                System.Diagnostics.Debug.WriteLine($"[API] Lấy danh sách đĩa loại {maLoai} - Trang {pageIndex} lúc {DateTime.Now}");

                // 🟢 2. Kiểm tra tham số đầu vào
                if (pageIndex < 1) pageIndex = 1;
                if (pageSize <= 0 || pageSize > 100) pageSize = 12;

                // 🟢 3. Tạo key cache riêng cho mỗi request
                string cacheKey = $"DanhSachDia_{maLoai}_{pageIndex}_{pageSize}";
                var cached = HttpContext.Cache[cacheKey];
                if (cached != null)
                {
                    return JsonNet(new { success = true, data = cached });
                }

                // 🟢 4. Gọi stored procedure
                var raw = db.sp_GetDiaPhimCaNhacByLoai(maLoai, pageIndex, pageSize).ToList();

                if (raw == null || raw.Count == 0)
                {
                    return JsonNet(new { success = false, message = "Không tìm thấy sản phẩm nào" });
                }

                // 🟢 5. Map dữ liệu sang DTO
                var items = raw.Select(r => new DiaDTO
                {
                    MaDia = r.MaDia,
                    TenDia = r.TenDia,
                    GiaBan = r.GiaBan,
                    AnhBia = r.AnhBia,
                    SoLuongTon = r.SoLuongTon,
                    TenLoaiDia = r.TenLoaiDia,
                    LaSanPhamNoiBat = r.LaSanPhamNoiBat
                }).ToList();

                // 🟢 6. Lấy tổng số lượng (TotalCount)
                int total = 0;
                var first = raw.FirstOrDefault();
                if (first != null)
                {
                    var prop = first.GetType().GetProperty("TotalCount");
                    if (prop != null)
                    {
                        var val = prop.GetValue(first, null);
                        if (val != null) total = Convert.ToInt32(val);
                    }
                }
                if (total == 0) total = items.Count;

                // 🟢 7. Tạo đối tượng trả về (PagedResult)
                var result = new PagedResult<DiaDTO>
                {
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    TotalItems = total,
                    Items = items
                };

                // 🟢 8. Lưu cache (ví dụ 15 giây)
                HttpContext.Cache.Insert(
                    cacheKey,
                    result,
                    null,
                    DateTime.Now.AddSeconds(15),
                    System.Web.Caching.Cache.NoSlidingExpiration
                );

                // 🟢 9. Trả JSON cho client
                return JsonNet(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return JsonNet(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        // Tìm kiếm đĩa
        public JsonResult Search(string keyword, int pageIndex = 1, int pageSize = 12)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"[API] Tìm kiếm '{keyword}' - Trang {pageIndex}");

                if (string.IsNullOrWhiteSpace(keyword))
                    return JsonNet(new { success = false, message = "Vui lòng nhập từ khóa tìm kiếm." });

                var raw = db.sp_SearchDiaPhimCaNhac(keyword, pageIndex, pageSize).ToList();

                if (raw == null || raw.Count == 0)
                    return JsonNet(new { success = false, message = "Không tìm thấy sản phẩm phù hợp." });

                var items = raw.Select(r => new DiaDTO
                {
                    MaDia = r.MaDia,
                    TenDia = r.TenDia,
                    GiaBan = r.GiaBan,
                    AnhBia = r.AnhBia,
                    TenLoaiDia = r.TenLoaiDia,
                    LaSanPhamNoiBat = r.LaSanPhamNoiBat
                }).ToList();

                int total = items.Count;
                var result = new PagedResult<DiaDTO>
                {
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    TotalItems = total,
                    Items = items
                };

                return JsonNet(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return JsonNet(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        // Lấy sản phẩm nổi bật
        public JsonResult GetSanPhamNoiBat(int pageIndex = 1, int pageSize = 12)
        {
            try
            {
                var cacheKey = $"SanPhamNoiBat_{pageIndex}_{pageSize}";
                var cached = HttpContext.Cache[cacheKey];
                if (cached != null)
                    return JsonNet(new { success = true, data = cached });

                // 🟢 Gọi stored procedure với tham số
                var raw = db.sp_GetSanPhamNoiBat(pageIndex, pageSize).ToList();

                if (raw == null || raw.Count == 0)
                    return JsonNet(new { success = false, message = "Không có sản phẩm nổi bật nào." });

                var items = raw.Select(r => new DiaDTO
                {
                    MaDia = r.MaDia,
                    TenDia = r.TenDia,
                    GiaBan = r.GiaBan,
                    AnhBia = r.AnhBia,
                    TenLoaiDia = r.TenLoaiDia,
                    LaSanPhamNoiBat = r.LaSanPhamNoiBat
                }).ToList();

                HttpContext.Cache.Insert(cacheKey, items, null, DateTime.Now.AddMinutes(1), System.Web.Caching.Cache.NoSlidingExpiration);

                return JsonNet(new { success = true, data = items });
            }
            catch (Exception ex)
            {
                return JsonNet(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }


        // Thêm sản phẩm vào giỏ hàng
        [HttpPost]
        public JsonResult ThemVaoGioHang(int maKH, int maDia, int soLuong)
        {
            try
            {
                // 🟢 1. Kiểm tra dữ liệu đầu vào
                if (soLuong <= 0)
                    return JsonNet(new { success = false, message = "Số lượng phải lớn hơn 0." });

                if (maKH <= 0)
                    return JsonNet(new { success = false, message = "Người dùng chưa đăng nhập." });

                // 🟢 2. Ghi log
                System.Diagnostics.Debug.WriteLine($"[API] {maKH} thêm {soLuong} đĩa {maDia} vào giỏ hàng");

                // 🟢 3. Gọi stored procedure
                db.sp_ThemVaoGioHang(maKH, maDia, soLuong);

                // 🟢 4. Trả phản hồi JSON
                return JsonNet(new
                {
                    success = true,
                    message = "Đã thêm sản phẩm vào giỏ hàng thành công!"
                });
            }
            catch (Exception ex)
            {
                // 🟢 5. Xử lý lỗi (vd: lỗi kho không đủ)
                return JsonNet(new { success = false, message = "Lỗi khi thêm vào giỏ hàng: " + ex.Message });
            }
        }


    }
}