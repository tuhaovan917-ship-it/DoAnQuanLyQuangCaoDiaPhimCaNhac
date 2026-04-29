using DoAnQuanLyQuangCaoDiaPhimCaNhac.Models;
using DoAnQuanLyQuangCaoDiaPhimCaNhac.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAnQuanLyQuangCaoDiaPhimCaNhac.Controllers
{
    public class SellerController : Controller
    {
        QuanLyDiaPhimCaNhac_EditedEntities db = new QuanLyDiaPhimCaNhac_EditedEntities();

        // Kiểm tra đăng nhập người bán
        //private bool CheckSellerLogin()
        //{
        //    if (Session["UserId"] == null || Session["UserType"].ToString() != "Seller")
        //    {
        //        return false;
        //    }
        //    return true;
        //}

        // Kiểm tra đăng nhập người bán (sửa để dùng SellerId và tránh NRE)
        private bool CheckSellerLogin()
        {
            if (Session["SellerId"] == null)
            {
                return false;
            }
            // Nếu bạn vẫn muốn kiểm tra kiểu user
            var userType = Session["UserType"] as string;
            if (string.IsNullOrEmpty(userType) || userType != "Seller")
            {
                return false;
            }
            return true;
        }

        // GET: Seller/Index - Dashboard chính
        public ActionResult Index()
        {
            if (!CheckSellerLogin())
            {
                return RedirectToAction("Login", "Account");
            }

            int sellerId = (int)Session["SellerId"];
            
            // Lấy thống kê tổng quan
            ViewBag.TotalProducts = db.DiaPhimCaNhacs.Count(d => d.MaNguoiBan == sellerId);
            ViewBag.TotalOrders = db.DonHangs.Count(d => d.MaNguoiBan == sellerId);
            ViewBag.CompletedOrders = db.DonHangs.Count(d => d.MaNguoiBan == sellerId && d.TinhTrangGiaoHang == 2);
            ViewBag.PendingOrders = db.DonHangs.Count(d => d.MaNguoiBan == sellerId && d.TinhTrangGiaoHang == 0);

            return View();
        }

        // GET: Seller/Products - Quản lý sản phẩm
        public ActionResult Products()
        {
            if (!CheckSellerLogin())
            {
                return RedirectToAction("Login", "Account");
            }

            int sellerId = (int)Session["SellerId"];
            var products = db.DiaPhimCaNhacs.Where(d => d.MaNguoiBan == sellerId).ToList();
            
            // Lấy danh sách loại đĩa và nhà sản xuất cho dropdown
            ViewBag.LoaiDiaList = new SelectList(db.LoaiDias, "MaLoaiDia", "TenLoaiDia");
            ViewBag.NhaSanXuatList = new SelectList(db.NhaSanXuats, "MaNhaSanXuat", "TenNhaSanXuat");

            return View(products);
        }

        // GET: Seller/AddProduct - Thêm sản phẩm mới
        [HttpGet]
        public ActionResult AddProduct()
        {
            if (!CheckSellerLogin())
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.LoaiDiaList = new SelectList(db.LoaiDias, "MaLoaiDia", "TenLoaiDia");
            ViewBag.NhaSanXuatList = new SelectList(db.NhaSanXuats, "MaNhaSanXuat", "TenNhaSanXuat");
            return View();
        }

        // POST: Seller/AddProduct - Thêm sản phẩm mới
        [HttpPost]
        public ActionResult AddProduct(DiaPhimCaNhac product, HttpPostedFileBase AnhBiaFile)
        {
            try
            {
                if (!CheckSellerLogin())
                {
                    return RedirectToAction("Login", "Account");
                }

                int sellerId = (int)Session["SellerId"];

                // Xử lý upload hình ảnh
                if (AnhBiaFile != null && AnhBiaFile.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(AnhBiaFile.FileName);
                    string path = Path.Combine(Server.MapPath("~/Content/Images"), fileName);
                    AnhBiaFile.SaveAs(path);
                    product.AnhBia = fileName;
                }

                product.MaNguoiBan = sellerId;
                product.NgayCapNhat = DateTime.Now;
                product.TrangThai = true;

                db.DiaPhimCaNhacs.Add(product);
                db.SaveChanges();

                TempData["SuccessMessage"] = "Thêm sản phẩm thành công!";
                return RedirectToAction("Products");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Lỗi thêm sản phẩm: " + ex.Message;
                ViewBag.LoaiDiaList = new SelectList(db.LoaiDias, "MaLoaiDia", "TenLoaiDia");
                ViewBag.NhaSanXuatList = new SelectList(db.NhaSanXuats, "MaNhaSanXuat", "TenNhaSanXuat");
                return View(product);
            }
        }

        // GET: Seller/EditProduct/5 - Sửa sản phẩm
        [HttpGet]
        public ActionResult EditProduct(int id)
        {
            if (!CheckSellerLogin())
            {
                return RedirectToAction("Login", "Account");
            }

            int sellerId = (int)Session["SellerId"];
            var product = db.DiaPhimCaNhacs.FirstOrDefault(d => d.MaDia == id && d.MaNguoiBan == sellerId);
            
            if (product == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy sản phẩm!";
                return RedirectToAction("Products");
            }

            ViewBag.LoaiDiaList = new SelectList(db.LoaiDias, "MaLoaiDia", "TenLoaiDia", product.MaLoaiDia);
            ViewBag.NhaSanXuatList = new SelectList(db.NhaSanXuats, "MaNhaSanXuat", "TenNhaSanXuat", product.MaNhaSanXuat);
            return View(product);
        }

        // POST: Seller/EditProduct/5 - Sửa sản phẩm
        [HttpPost]
        public ActionResult EditProduct(DiaPhimCaNhac product, HttpPostedFileBase AnhBiaFile)
        {
            try
            {
                if (!CheckSellerLogin())
                {
                    return RedirectToAction("Login", "Account");
                }

                int sellerId = (int)Session["SellerId"];
                var existingProduct = db.DiaPhimCaNhacs.FirstOrDefault(d => d.MaDia == product.MaDia && d.MaNguoiBan == sellerId);
                
                if (existingProduct == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy sản phẩm!";
                    return RedirectToAction("Products");
                }

                // Xử lý upload hình ảnh mới
                if (AnhBiaFile != null && AnhBiaFile.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(AnhBiaFile.FileName);
                    string path = Path.Combine(Server.MapPath("~/Content/Images/Products"), fileName);
                    AnhBiaFile.SaveAs(path);
                    existingProduct.AnhBia = fileName;
                }

                // Cập nhật thông tin sản phẩm
                existingProduct.TenDia = product.TenDia;
                existingProduct.GiaBan = product.GiaBan;
                existingProduct.MoTa = product.MoTa;
                existingProduct.NgayPhatHanh = product.NgayPhatHanh;
                existingProduct.SoLuongTon = product.SoLuongTon;
                existingProduct.ThoiLuong = product.ThoiLuong;
                existingProduct.ChatLuong = product.ChatLuong;
                existingProduct.NgonNgu = product.NgonNgu;
                existingProduct.MaLoaiDia = product.MaLoaiDia;
                existingProduct.MaNhaSanXuat = product.MaNhaSanXuat;
                existingProduct.LaDiaPhim = product.LaDiaPhim;
                existingProduct.TrangThai = product.TrangThai;
                existingProduct.NgayCapNhat = DateTime.Now;

                db.SaveChanges();

                TempData["SuccessMessage"] = "Cập nhật sản phẩm thành công!";
                return RedirectToAction("Products");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Lỗi cập nhật sản phẩm: " + ex.Message;
                ViewBag.LoaiDiaList = new SelectList(db.LoaiDias, "MaLoaiDia", "TenLoaiDia");
                ViewBag.NhaSanXuatList = new SelectList(db.NhaSanXuats, "MaNhaSanXuat", "TenNhaSanXuat");
                return View(product);
            }
        }

        // POST: Seller/DeleteProduct/5 - Xóa sản phẩm
        [HttpPost]
        public ActionResult DeleteProduct(int id)
        {
            try
            {
                if (!CheckSellerLogin())
                {
                    return RedirectToAction("Login", "Account");
                }

                int sellerId = (int)Session["SellerId"];
                var product = db.DiaPhimCaNhacs.FirstOrDefault(d => d.MaDia == id && d.MaNguoiBan == sellerId);
                
                if (product == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy sản phẩm!" });
                }

                // Kiểm tra xem sản phẩm có trong đơn hàng nào chưa
                var orderExists = db.ChiTietDonHangs.Any(ct => ct.MaDia == id);
                if (orderExists)
                {
                    // Nếu có trong đơn hàng thì chỉ ẩn sản phẩm
                    product.TrangThai = false;
                    product.NgayCapNhat = DateTime.Now;
                }
                else
                {
                    // Nếu không có trong đơn hàng thì xóa hoàn toàn
                    db.DiaPhimCaNhacs.Remove(product);
                }
                
                db.SaveChanges();

                return Json(new { success = true, message = "Xóa sản phẩm thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi xóa sản phẩm: " + ex.Message });
            }
        }

        // GET: Seller/Orders - Quản lý đơn hàng
        public ActionResult Orders()
        {
            if (!CheckSellerLogin())
            {
                return RedirectToAction("Login", "Account");
            }

            int sellerId = (int)Session["SellerId"];
            var orders = db.DonHangs.Where(d => d.MaNguoiBan == sellerId)
                                   .OrderByDescending(d => d.NgayDat)
                                   .ToList();

            // Lấy danh sách trạng thái giao hàng
            ViewBag.TrangThaiList = new SelectList(db.TrangThaiGiaoHangs, "MaTrangThai", "TenTrangThai");

            return View(orders);
        }

        // POST: Seller/UpdateOrderStatus - Cập nhật trạng thái đơn hàng
        [HttpPost]
        public ActionResult UpdateOrderStatus(int orderId, int status)
        {
            try
            {
                if (!CheckSellerLogin())
                {
                    return Json(new { success = false, message = "Bạn chưa đăng nhập!" });
                }

                int sellerId = (int)Session["SellerId"];
                var order = db.DonHangs.FirstOrDefault(d => d.MaDonHang == orderId && d.MaNguoiBan == sellerId);
                
                if (order == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy đơn hàng!" });
                }

                order.TinhTrangGiaoHang = status;
                order.NgayGiao = status == 2 ? DateTime.Now : (DateTime?)null;
                db.SaveChanges();

                string statusText = db.TrangThaiGiaoHangs.FirstOrDefault(t => t.MaTrangThai == status)?.TenTrangThai ?? "Không xác định";

                return Json(new { success = true, message = $"Cập nhật trạng thái thành: {statusText}" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi cập nhật trạng thái: " + ex.Message });
            }
        }

        //// GET: Seller/OrderDetails/5 - Chi tiết đơn hàng

        //public ActionResult OrderDetails(int id)
        //{
        //    var donHang = db.DonHangs
        //        .Include(d => d.KhachHang)
        //        .Include(d => d.TrangThaiGiaoHang)
        //        .FirstOrDefault(d => d.MaDonHang == id);

        //    if (donHang == null)
        //    {
        //        return HttpNotFound();
        //    }

        //    // Lấy chi tiết đơn hàng + join sang DiaPhimCaNhac
        //    var chiTiet = db.ChiTietDonHangs
        //        .Include(ct => ct.DiaPhimCaNhac)
        //        .Where(ct => ct.MaDonHang == id)
        //        .Select(ct => new
        //        {
        //            TenDia = ct.DiaPhimCaNhac.TenDia,
        //            AnhBia = ct.DiaPhimCaNhac.AnhBia,
        //            SoLuong = ct.SoLuong,
        //            DonGia = ct.DonGia,
        //            ThanhTien = ct.SoLuong * ct.DonGia
        //        })
        //        .ToList();

        //    ViewBag.OrderDetails = chiTiet;

        //    return View(donHang);
        //}

        // GET: Seller/OrderDetails/5 - Chi tiết đơn hàng
        public ActionResult OrderDetails(int id)
        {
            if (!CheckSellerLogin())
            {
                return RedirectToAction("Login", "Account");
            }

            int sellerId = (int)Session["SellerId"];

            // Lấy đơn hàng kèm chi tiết và sản phẩm để render view an toàn
            var donHang = db.DonHangs
                .Include(d => d.KhachHang)
                .Include(d => d.TrangThaiGiaoHang)
                .Include(d => d.ChiTietDonHangs.Select(ct => ct.DiaPhimCaNhac))
                .FirstOrDefault(d => d.MaDonHang == id && d.MaNguoiBan == sellerId);

            if (donHang == null)
            {
                return HttpNotFound();
            }

            return View(donHang);
        }


        // GET: Seller/Revenue - Thống kê doanh thu
        public ActionResult Revenue()
        {
            if (!CheckSellerLogin())
            {
                return RedirectToAction("Login", "Account");
            }

            int sellerId = (int)Session["SellerId"];
            
            // Thống kê doanh thu tổng
            var totalRevenue = db.DonHangs
                .Where(d => d.MaNguoiBan == sellerId && d.TinhTrangGiaoHang == 2)
                .Sum(d => (decimal?)d.TongTien) ?? 0;

            ViewBag.TotalRevenue = totalRevenue;

            var fromDate = DateTime.Now.AddMonths(-6);
            // Thống kê doanh thu theo tháng (6 tháng gần đây)
            //var revenueByMonth = db.DonHangs
            //    .Where(d => d.MaNguoiBan == sellerId && d.TinhTrangGiaoHang == 2)
            //    .Where(d => d.NgayDat >= fromDate)
            //    .GroupBy(d => new { d.NgayDat.Year, d.NgayDat.Month })
            //    .Select(g => new
            //    {
            //        Month = g.Key.Year + "/" + g.Key.Month.ToString("00"),
            //        Revenue = g.Sum(d => d.TongTien),
            //        Orders = g.Count()
            //    })
            //    .OrderBy(x => x.Month)
            //    .ToList();

            var revenueByMonthRaw = db.DonHangs
                .Where(d => d.MaNguoiBan == sellerId && d.TinhTrangGiaoHang == 2)
                .Where(d => d.NgayDat >= fromDate)
                .GroupBy(d => new { d.NgayDat.Year, d.NgayDat.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Revenue = g.Sum(d => d.TongTien),
                    Orders = g.Count()
                })
                .OrderBy(g => g.Year).ThenBy(g => g.Month)
                .ToList();

            var revenueByMonth = revenueByMonthRaw
                .Select(x => new
                {
                    Month = $"{x.Year}/{x.Month:00}",   // format ngoài SQL → OK
                    x.Revenue,
                    x.Orders
                })
                .ToList();

            ViewBag.RevenueByMonth = revenueByMonth;

            // Top sản phẩm bán chạy nhất
            var topProducts = db.ChiTietDonHangs
                .Include("DiaPhimCaNhac")
                .Include("DonHang")
                .Where(ct => ct.DonHang.MaNguoiBan == sellerId && ct.DonHang.TinhTrangGiaoHang == 2)
                .GroupBy(ct => new { ct.MaDia, ct.DiaPhimCaNhac.TenDia })
                .Select(g => new TopProductViewModel
                {
                    ProductName = g.Key.TenDia,
                    TotalSold = g.Sum(ct => ct.SoLuong),
                    TotalRevenue = g.Sum(ct => ct.SoLuong * ct.DonGia)
                })
                .OrderByDescending(x => x.TotalSold)
                .Take(10)
                .ToList();

            ViewBag.TopProducts = topProducts;

            return View();
        }
    }
}