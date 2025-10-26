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
        QuanLyDiaPhimCaNhacEntities db = new QuanLyDiaPhimCaNhacEntities();
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

        //[HttpGet]
        //public ActionResult LocSanPham(string loai = null, bool? laPhim = null, List<string> price = null)
        //{
        //    var query = db.DiaPhimCaNhacs.AsQueryable();

        //    // ✅ 1. Nếu truyền loại (thể loại con)
        //    if (!string.IsNullOrEmpty(loai))
        //    {
        //        query = query.Where(d => d.LoaiDia.TenLoaiDia.Contains(loai));
        //        ViewBag.Loai = loai;
        //    }

        //    // ✅ 2. Nếu truyền đĩa phim / đĩa nhạc
        //    if (laPhim.HasValue)
        //    {
        //        query = query.Where(d => d.LaDiaPhim == laPhim.Value);
        //    }

        //    // ✅ 3. Lọc theo giá
        //    if (price != null && price.Any())
        //    {
        //        var filtered = new List<DiaPhimCaNhac>();

        //        foreach (var range in price)
        //        {
        //            switch (range)
        //            {
        //                case "under100":
        //                    filtered.AddRange(query.Where(d => d.GiaBan < 100000));
        //                    break;
        //                case "100-300":
        //                    filtered.AddRange(query.Where(d => d.GiaBan >= 100000 && d.GiaBan <= 300000));
        //                    break;
        //                case "300-500":
        //                    filtered.AddRange(query.Where(d => d.GiaBan > 300000 && d.GiaBan <= 500000));
        //                    break;
        //                case "over500":
        //                    filtered.AddRange(query.Where(d => d.GiaBan > 500000));
        //                    break;
        //            }
        //        }

        //        query = filtered.Distinct().AsQueryable();
        //    }

        //    // ✅ 4. Trả kết quả
        //    var kq = query.OrderByDescending(d => d.NgayCapNhat).ToList();
        //    ViewBag.Count = kq.Count;

        //    // ✅ 5. Xác định icon
        //    if (laPhim == true)
        //        ViewBag.Icon = "💿";
        //    else if (laPhim == false)
        //        ViewBag.Icon = "🎧";
        //    else
        //        ViewBag.Icon = "🔍";

        //    return View(kq);
        //}

        [HttpGet]
        public ActionResult LocSanPham(string loai = null, bool? laPhim = null)
        {
            var query = db.DiaPhimCaNhacs.AsQueryable();

            // ✅ 1. Lọc theo loại thể loại con (vd: Tình cảm, Pop, Rock)
            if (!string.IsNullOrEmpty(loai))
            {
                query = query.Where(d => d.LoaiDia.TenLoaiDia.Contains(loai));
                ViewBag.Loai = loai;
            }

            // ✅ 2. Lọc theo loại đĩa chính (phim hoặc nhạc)
            if (laPhim.HasValue)
            {
                query = query.Where(d => d.LaDiaPhim == laPhim.Value);
            }

            // ✅ 3. Sắp xếp và trả về danh sách
            var kq = query.OrderByDescending(d => d.NgayCapNhat).ToList();
            ViewBag.Count = kq.Count;

            // ✅ 4. Xác định icon hiển thị
            if (laPhim == true)
                ViewBag.Icon = "💿"; // Đĩa phim
            else if (laPhim == false)
                ViewBag.Icon = "🎧"; // Đĩa nhạc
            else
                ViewBag.Icon = "🔍"; // Không xác định (tất cả)

            return View(kq);
        }


        [HttpGet]
        public ActionResult LocTheoGia(List<string> price, bool? laPhim = null, string loai = null)
        {
            var query = db.DiaPhimCaNhacs.AsQueryable();

            // ✅ Nếu người dùng đang ở trang phim hoặc nhạc
            if (laPhim.HasValue)
                query = query.Where(d => d.LaDiaPhim == laPhim.Value);

            // ✅ Nếu đang ở 1 thể loại cụ thể (vd: “Pop”, “Tình cảm”)
            if (!string.IsNullOrEmpty(loai))
                query = query.Where(d => d.LoaiDia.TenLoaiDia.Contains(loai));

            // ✅ Lọc giá theo nhiều checkbox
            if (price != null && price.Any())
            {
                var filtered = new List<DiaPhimCaNhac>();

                foreach (var range in price)
                {
                    switch (range)
                    {
                        case "under100":
                            filtered.AddRange(query.Where(d => d.GiaBan < 100000));
                            break;
                        case "100-300":
                            filtered.AddRange(query.Where(d => d.GiaBan >= 100000 && d.GiaBan <= 300000));
                            break;
                        case "300-500":
                            filtered.AddRange(query.Where(d => d.GiaBan > 300000 && d.GiaBan <= 500000));
                            break;
                        case "over500":
                            filtered.AddRange(query.Where(d => d.GiaBan > 500000));
                            break;
                    }
                }

                query = filtered.Distinct().AsQueryable();
            }

            // ✅ Lấy danh sách kết quả
            var kq = query.OrderByDescending(d => d.NgayCapNhat).ToList();
            ViewBag.Count = kq.Count;

            // ✅ Hiển thị icon phù hợp
            if (laPhim == true)
                ViewBag.Icon = "💿";
            else if (laPhim == false)
                ViewBag.Icon = "🎧";
            else
                ViewBag.Icon = "🔍";

            ViewBag.Loai = loai ?? "Tất cả sản phẩm";
            return View("~/Views/Home/LocSanPham.cshtml", kq);
        }
    }
}