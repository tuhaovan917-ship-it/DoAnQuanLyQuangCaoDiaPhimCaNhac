using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DoAnQuanLyQuangCaoDiaPhimCaNhac.Models;
using System.Data.Entity;

namespace DoAnQuanLyQuangCaoDiaPhimCaNhac.Controllers
{
    public class CartController : Controller
    {
        QuanLyDiaPhimCaNhac_EditedEntities db = new QuanLyDiaPhimCaNhac_EditedEntities();

        // Kiểm tra đăng nhập khách hàng
        private bool CheckCustomerLogin()
        {
            if (Session["UserId"] == null || Session["UserType"].ToString() != "Customer")
            {
                return false;
            }
            return true;
        }

        // GET: Cart/Index - Hiển thị giỏ hàng
        public ActionResult Index()
        {
            if (!CheckCustomerLogin())
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để xem giỏ hàng!";
                return RedirectToAction("Login", "Account");
            }

            int customerId = (int)Session["UserId"];
            
            // Lấy danh sách sản phẩm trong giỏ hàng
            var cartItems = db.GioHangs
                .Include("DiaPhimCaNhac")
                .Where(g => g.MaKH == customerId)
                .ToList();

            // Tính tổng tiền
            decimal totalAmount = cartItems.Sum(item => item.SoLuong * item.DiaPhimCaNhac.GiaBan);
            ViewBag.TotalAmount = totalAmount;

            return View(cartItems);
        }

        // POST: Cart/AddToCart - Thêm vào giỏ hàng
        [HttpPost]
        public ActionResult AddToCart(int maDia, int soLuong = 1)
        {
            try
            {
                if (!CheckCustomerLogin())
                {
                    return Json(new { success = false, message = "Vui lòng đăng nhập để thêm sản phẩm vào giỏ hàng!" });
                }

                int customerId = (int)Session["UserId"];

                // Kiểm tra sản phẩm tồn tại và còn hàng
                var product = db.DiaPhimCaNhacs.FirstOrDefault(d => d.MaDia == maDia && d.TrangThai == true);
                if (product == null)
                {
                    return Json(new { success = false, message = "Sản phẩm không tồn tại!" });
                }

                if (product.SoLuongTon < soLuong)
                {
                    return Json(new { success = false, message = "Không đủ số lượng trong kho!" });
                }

                // Kiểm tra sản phẩm đã có trong giỏ hàng chưa
                var existingCartItem = db.GioHangs.FirstOrDefault(g => g.MaKH == customerId && g.MaDia == maDia);

                if (existingCartItem != null)
                {
                    // Tăng số lượng
                    existingCartItem.SoLuong += soLuong;
                }
                else
                {
                    // Thêm mới
                    var cartItem = new GioHang
                    {
                        MaKH = customerId,
                        MaDia = maDia,
                        SoLuong = soLuong,
                        NgayThem = DateTime.Now
                    };
                    db.GioHangs.Add(cartItem);
                }

                db.SaveChanges();

                // Lấy số lượng trong giỏ hàng
                //int cartCount = db.GioHangs.Where(g => g.MaKH == customerId).Sum(g => (int?)g.SoLuong) ?? 0;
                int cartCount = db.GioHangs
                .Where(g => g.MaKH == customerId)
                .Select(g => g.MaDia)
                .Distinct()
                .Count();

                return Json(new { 
                    success = true, 
                    message = "Thêm vào giỏ hàng thành công!",
                    cartCount = cartCount
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi thêm vào giỏ hàng: " + ex.Message });
            }
        }

        // POST: Cart/UpdateQuantity - Cập nhật số lượng
        [HttpPost]
        public ActionResult UpdateQuantity(int maDia, int soLuong)
        {
            try
            {
                if (!CheckCustomerLogin())
                {
                    return Json(new { success = false, message = "Vui lòng đăng nhập!" });
                }

                int customerId = (int)Session["UserId"];
                var cartItem = db.GioHangs.FirstOrDefault(g => g.MaKH == customerId && g.MaDia == maDia);

                if (cartItem == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy sản phẩm trong giỏ hàng!" });
                }

                if (soLuong <= 0)
                {
                    // Xóa sản phẩm khỏi giỏ hàng
                    db.GioHangs.Remove(cartItem);
                }
                else
                {
                    // Kiểm tra số lượng trong kho
                    var product = db.DiaPhimCaNhacs.FirstOrDefault(d => d.MaDia == maDia);
                    if (product.SoLuongTon < soLuong)
                    {
                        return Json(new { success = false, message = "Không đủ số lượng trong kho!" });
                    }

                    cartItem.SoLuong = soLuong;
                }

                db.SaveChanges();

                // Tính lại tổng tiền
                //int cartCount = db.GioHangs.Where(g => g.MaKH == customerId).Sum(g => (int?)g.SoLuong) ?? 0;
                int cartCount = db.GioHangs
                .Where(g => g.MaKH == customerId)
                .Select(g => g.MaDia)
                .Distinct()
                .Count();

                decimal totalAmount = db.GioHangs
                    .Include("DiaPhimCaNhac")
                    .Where(g => g.MaKH == customerId)
                    .Sum(g => g.SoLuong * g.DiaPhimCaNhac.GiaBan);

                return Json(new { 
                    success = true, 
                    cartCount = cartCount,
                    totalAmount = totalAmount
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi cập nhật: " + ex.Message });
            }
        }

        // POST: Cart/RemoveFromCart - Xóa khỏi giỏ hàng
        [HttpPost]
        public ActionResult RemoveFromCart(int maDia)
        {
            try
            {
                if (!CheckCustomerLogin())
                {
                    return Json(new { success = false, message = "Vui lòng đăng nhập!" });
                }

                int customerId = (int)Session["UserId"];
                var cartItem = db.GioHangs.FirstOrDefault(g => g.MaKH == customerId && g.MaDia == maDia);

                if (cartItem == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy sản phẩm trong giỏ hàng!" });
                }

                db.GioHangs.Remove(cartItem);
                db.SaveChanges();

                // Tính lại số lượng và tổng tiền
                //int cartCount = db.GioHangs.Where(g => g.MaKH == customerId).Sum(g => (int?)g.SoLuong) ?? 0;
                int cartCount = db.GioHangs
                .Where(g => g.MaKH == customerId)
                .Select(g => g.MaDia)
                .Distinct()
                .Count();

                decimal totalAmount = db.GioHangs
                    .Include("DiaPhimCaNhac")
                    .Where(g => g.MaKH == customerId)
                    .Sum(g => g.SoLuong * g.DiaPhimCaNhac.GiaBan);

                return Json(new { 
                    success = true, 
                    cartCount = cartCount,
                    totalAmount = totalAmount,
                    message = "Xóa sản phẩm thành công!"
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi xóa sản phẩm: " + ex.Message });
            }
        }

        // GET: Cart/GetCartCount - Lấy số lượng trong giỏ hàng (AJAX)
        //public ActionResult GetCartCount()
        //{
        //    if (!CheckCustomerLogin())
        //    {
        //        return Json(new { count = 0 }, JsonRequestBehavior.AllowGet);
        //    }

        //    int customerId = (int)Session["UserId"];
        //    int cartCount = db.GioHangs.Where(g => g.MaKH == customerId).Sum(g => (int?)g.SoLuong) ?? 0;

        //    return Json(new { count = cartCount }, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult GetCartCount()
        {
            if (!CheckCustomerLogin())
            {
                return Json(new { count = 0 }, JsonRequestBehavior.AllowGet);
            }

            int customerId = (int)Session["UserId"];

            // ✅ Đếm số loại sản phẩm khác nhau trong giỏ (không cộng số lượng)
            int cartCount = db.GioHangs
                              .Where(g => g.MaKH == customerId)
                              .Select(g => g.MaDia)
                              .Distinct()
                              .Count();

            return Json(new { count = cartCount }, JsonRequestBehavior.AllowGet);
        }


        // POST: Cart/Checkout - Thanh toán
        [HttpPost]
        public ActionResult Checkout(string diaChiGiao, string ghiChu, string phuongThucThanhToan)
        {
            try
            {
                if (!CheckCustomerLogin())
                {
                    TempData["ErrorMessage"] = "Vui lòng đăng nhập để thanh toán!";
                    return RedirectToAction("Login", "Account");
                }

                int customerId = (int)Session["UserId"];

                // Kiểm tra giỏ hàng có sản phẩm không
                var cartItems = db.GioHangs.Include("DiaPhimCaNhac").Where(g => g.MaKH == customerId).ToList();
                if (cartItems.Count == 0)
                {
                    TempData["ErrorMessage"] = "Giỏ hàng trống!";
                    return RedirectToAction("Index");
                }

                // Lấy danh sách người bán từ giỏ hàng
                var sellerIds = cartItems.Select(c => c.DiaPhimCaNhac.MaNguoiBan).Distinct().Where(s => s.HasValue).Select(s => s.Value).ToList();

                if (sellerIds.Count == 0)
                {
                    TempData["ErrorMessage"] = "Không thể xác định người bán!";
                    return RedirectToAction("Index");
                }

                // Tạo đơn hàng cho từng người bán
                foreach (var sellerId in sellerIds)
                {
                    var sellerCartItems = cartItems.Where(c => c.DiaPhimCaNhac.MaNguoiBan == sellerId).ToList();
                    
                    // Tạo đơn hàng mới
                    var donHang = new DonHang
                    {
                        MaKH = customerId,
                        MaNguoiBan = sellerId,
                        NgayDat = DateTime.Now,
                        PhuongThucThanhToan = string.IsNullOrEmpty(phuongThucThanhToan) ? "Tiền mặt" : phuongThucThanhToan,
                        TinhTrangGiaoHang = 0, // Chờ xử lý
                        DaThanhToan = false,
                        DiaChiGiao = diaChiGiao,
                        GhiChu = ghiChu
                    };

                    db.DonHangs.Add(donHang);
                    db.SaveChanges();

                    // Thêm chi tiết đơn hàng
                    foreach (var item in sellerCartItems)
                    {
                        var chiTietDonHang = new ChiTietDonHang
                        {
                            MaDonHang = donHang.MaDonHang,
                            MaDia = item.MaDia,
                            SoLuong = item.SoLuong,
                            DonGia = item.DiaPhimCaNhac.GiaBan
                        };
                        db.ChiTietDonHangs.Add(chiTietDonHang);

                        // Cập nhật tồn kho
                        var product = db.DiaPhimCaNhacs.FirstOrDefault(d => d.MaDia == item.MaDia);
                        if (product != null)
                        {
                            product.SoLuongTon -= item.SoLuong;
                        }
                    }

                    // Cập nhật tổng tiền
                    donHang.TongTien = sellerCartItems.Sum(item => item.SoLuong * item.DiaPhimCaNhac.GiaBan);
                }

                // Xóa giỏ hàng
                foreach(var item in cartItems)
                {
                    db.GioHangs.Remove(item);
                }
                db.SaveChanges();

                TempData["SuccessMessage"] = "Đặt hàng thành công! Chúng tôi sẽ liên hệ với bạn sớm.";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi thanh toán: " + ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}