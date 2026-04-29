using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using DoAnQuanLyQuangCaoDiaPhimCaNhac.Models;

namespace DoAnQuanLyQuangCaoDiaPhimCaNhac.Controllers
{
    public class AccountController : Controller
    {

        QuanLyDiaPhimCaNhac_EditedEntities db = new QuanLyDiaPhimCaNhac_EditedEntities();

        // GET: Account/Login
        [HttpGet]
        public ActionResult Login()
        {
            // Nếu đã đăng nhập thì redirect về trang chủ
            if (Session["UserId"] != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        public ActionResult Login(string taiKhoan, string matKhau)
        {
            try
            {
                // Mã hóa mật khẩu
                //string hashedPassword = HashPassword(matKhau);

                // Kiểm tra trong bảng KhachHang
                var khachHang = db.KhachHangs.FirstOrDefault(k => k.TaiKhoan == taiKhoan /*&& k.MatKhau == hashedPassword*/ && k.TrangThai == true);

                if (khachHang != null)
                {
                    // Lưu thông tin vào session
                    Session["UserId"] = khachHang.MaKH;
                    Session["UserName"] = khachHang.HoTen;
                    Session["UserType"] = "Customer";
                    Session["UserEmail"] = khachHang.Email;

                    TempData["SuccessMessage"] = "Đăng nhập thành công! Xin chào " + khachHang.HoTen;
                    return RedirectToAction("Index", "Home");
                }

                // Kiểm tra trong bảng NguoiBan
                var nguoiBan = db.NguoiBans.FirstOrDefault(nb => nb.TaiKhoan == taiKhoan && nb.TrangThai == true && nb.MatKhau == matKhau);

                if (nguoiBan != null)
                {
                    // Lưu thông tin vào session
                    Session["UserId"] = nguoiBan.MaNguoiBan;
                    Session["UserName"] = nguoiBan.HoTen;
                    Session["UserType"] = "Seller";
                    Session["UserEmail"] = nguoiBan.Email;
                    Session["SellerId"] = nguoiBan.MaNguoiBan;

                    TempData["SuccessMessage"] = "Đăng nhập thành công! Xin chào " + nguoiBan.HoTen;
                    return RedirectToAction("Index", "Seller");
                }

                ViewBag.ErrorMessage = "Tài khoản hoặc mật khẩu không đúng!";
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Lỗi đăng nhập: " + ex.Message;
                return View();
            }
        }

        // GET: Account/Register
        [HttpGet]
        public ActionResult Register()
        {
            // Nếu đã đăng nhập thì redirect về trang chủ
            if (Session["UserId"] != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        public ActionResult Register(string hoTen, string taiKhoan, string matKhau, string email, string DienThoai, string diaChi, string gioiTinh)
        {
            try
            {
                // Kiểm tra tài khoản đã tồn tại chưa
                var existingCustomer = db.KhachHangs.FirstOrDefault(k => k.TaiKhoan == taiKhoan || k.Email == email);
                var existingSeller = db.NguoiBans.FirstOrDefault(nb => nb.TaiKhoan == taiKhoan || nb.Email == email);

                if (existingCustomer != null || existingSeller != null)
                {
                    ViewBag.ErrorMessage = "Tài khoản hoặc email đã tồn tại!";
                    return View();
                }

                // Tạo khách hàng mới
                var khachHang = new KhachHang
                {
                    HoTen = hoTen,
                    TaiKhoan = taiKhoan,
                    MatKhau = /*HashPassword(matKhau)*/ matKhau,
                    Email = email,
                    DienThoai = DienThoai,
                    DiaChi = diaChi,
                    GioiTinh = gioiTinh,
                    NgayDangKy = DateTime.Now,
                    TrangThai = true
                };

                db.KhachHangs.Add(khachHang);
                db.SaveChanges();

                ViewBag.SuccessMessage = "Đăng ký thành công! Vui lòng đăng nhập.";
                return View("Login");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Lỗi đăng ký: " + ex.Message;
                return View();
            }
        }

        // POST: Account/Logout
        [HttpGet]
        public ActionResult Logout()
        {
            Session.Clear();
            TempData["SuccessMessage"] = "Đăng xuất thành công!";
            return RedirectToAction("Index", "Home");
        }

        // Helper method: Mã hóa mật khẩu
        //private string HashPassword(string password)
        //{
        //    using (SHA256 sha256Hash = SHA256.Create())
        //    {
        //        byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
        //        StringBuilder builder = new StringBuilder();
        //        for (int i = 0; i < bytes.Length; i++)
        //        {
        //            builder.Append(bytes[i].ToString("x2"));
        //        }
        //        return builder.ToString();
        //    }
        //}
    }
}