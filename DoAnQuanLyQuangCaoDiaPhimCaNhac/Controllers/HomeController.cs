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
    }
}