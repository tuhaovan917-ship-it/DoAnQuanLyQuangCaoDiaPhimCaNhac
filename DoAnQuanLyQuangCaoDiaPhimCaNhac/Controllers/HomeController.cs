using DoAnQuanLyQuangCaoDiaPhimCaNhac.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAnQuanLyQuangCaoDiaPhimCaNhac.Controllers
{
    public class HomeController : Controller
    {
        QuanLyDiaPhimCaNhac_EditedEntities db = new QuanLyDiaPhimCaNhac_EditedEntities();
        public ActionResult Index()
        {
            // ✅ Lấy danh sách sản phẩm nổi bật
            var spNoiBat = db.sp_GetSanPhamNoiBat(1, 8).ToList();

            // ✅ Lấy danh sách sản phẩm mới (ví dụ: theo ngày cập nhật)
            var spMoi = db.DiaPhimCaNhacs
                          .OrderByDescending(d => d.NgayCapNhat)
                          .Take(8)
                          .ToList();

            // ✅ Gửi dữ liệu sang View qua ViewBag
            ViewBag.SanPhamNoiBat = spNoiBat;
            ViewBag.SanPhamMoi = spMoi;
            return View();
        }

        public ActionResult ChiTiet(int? maDia)
        {
            if (maDia == null)
                return RedirectToAction("Index"); // hoặc thông báo lỗi phù hợp

            var dia = db.sp_GetChiTietDiaPhimCaNhac(maDia.Value).FirstOrDefault();
            
            // Kiểm tra xem sản phẩm có trong giỏ hàng không (nếu đã đăng nhập)
            ViewBag.InCart = false;
            if (Session["UserId"] != null && Session["UserType"].ToString() == "Customer")
            {
                int customerId = (int)Session["UserId"];
                var cartItem = db.GioHangs.FirstOrDefault(g => g.MaKH == customerId && g.MaDia == maDia.Value);
                ViewBag.InCart = cartItem != null;
            }

            return View(dia);
        }


        [HttpGet]
        public ActionResult TimKiem(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                ViewBag.ThongBao = "Vui lòng nhập từ khóa tìm kiếm.";
                return View(new List<DiaPhimCaNhac>());
            }

            var kq = db.DiaPhimCaNhacs
                       .Where(d => d.TenDia.Contains(keyword))
                       .ToList();

            ViewBag.Keyword = keyword;
            return View(kq);
        }

        [HttpGet]
        public ActionResult LocSanPham(string loai = null, List<string> price = null, bool? laPhim = null)
        {
            var query = db.DiaPhimCaNhacs.Where(d => d.TrangThai == true).AsQueryable();

            // Lọc theo loại thể loại con
            if (!string.IsNullOrEmpty(loai))
            {
                query = query.Where(d => d.LoaiDia.TenLoaiDia.Contains(loai));
                ViewBag.Loai = loai;
            }

            // Lọc theo loại đĩa chính (phim hoặc nhạc)
            if (laPhim.HasValue)
            {
                query = query.Where(d => d.LaDiaPhim == laPhim.Value);
                ViewBag.LaPhim = laPhim.Value;
            }

            // Lọc theo giá
            if (price != null && price.Any())
            {
                var priceFiltered = new List<DiaPhimCaNhac>();

                foreach (var range in price)
                {
                    switch (range)
                    {
                        case "under100":
                            priceFiltered.AddRange(query.Where(d => d.GiaBan < 100000));
                            break;
                        case "100-300":
                            priceFiltered.AddRange(query.Where(d => d.GiaBan >= 100000 && d.GiaBan <= 300000));
                            break;
                        case "300-500":
                            priceFiltered.AddRange(query.Where(d => d.GiaBan > 300000 && d.GiaBan <= 500000));
                            break;
                        case "over500":
                            priceFiltered.AddRange(query.Where(d => d.GiaBan > 500000));
                            break;
                    }
                }

                query = priceFiltered.Distinct().AsQueryable();
                ViewBag.SelectedPrices = price;
            }

            var kq = query.OrderByDescending(d => d.NgayCapNhat).ToList();
            ViewBag.Count = kq.Count;

            // Xác định icon hiển thị
            if (laPhim == true)
                ViewBag.Icon = "🎬";
            else if (laPhim == false)
                ViewBag.Icon = "🎵";
            else
                ViewBag.Icon = "🔍";

            return View(kq);
        }

        // Action riêng cho AJAX (trả về JSON nếu cần)
        [HttpPost]
        public JsonResult LocSanPhamAjax(string loai = null, List<string> price = null, bool? laPhim = null)
        {
            var query = db.DiaPhimCaNhacs.Where(d => d.TrangThai == true).AsQueryable();

            if (!string.IsNullOrEmpty(loai))
            {
                query = query.Where(d => d.LoaiDia.TenLoaiDia.Contains(loai));
            }

            if (laPhim.HasValue)
            {
                query = query.Where(d => d.LaDiaPhim == laPhim.Value);
            }

            if (price != null && price.Any())
            {
                var priceFiltered = new List<DiaPhimCaNhac>();

                foreach (var range in price)
                {
                    switch (range)
                    {
                        case "under100":
                            priceFiltered.AddRange(query.Where(d => d.GiaBan < 100000));
                            break;
                        case "100-300":
                            priceFiltered.AddRange(query.Where(d => d.GiaBan >= 100000 && d.GiaBan <= 300000));
                            break;
                        case "300-500":
                            priceFiltered.AddRange(query.Where(d => d.GiaBan > 300000 && d.GiaBan <= 500000));
                            break;
                        case "over500":
                            priceFiltered.AddRange(query.Where(d => d.GiaBan > 500000));
                            break;
                    }
                }

                query = priceFiltered.Distinct().AsQueryable();
            }

            var kq = query.OrderByDescending(d => d.NgayCapNhat)
                          .Select(d => new {
                              d.MaDia,
                              d.TenDia,
                              d.GiaBan,
                              d.AnhBia,
                              TenLoaiDia = d.LoaiDia.TenLoaiDia,
                              d.DanhGia
                          })
                          .ToList();

            return Json(new { success = true, data = kq, count = kq.Count });
        }


    }
}