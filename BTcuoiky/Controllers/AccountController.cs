using BTcuoiky.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security; // Cần cho FormsAuthentication

namespace BTcuoiky.Controllers
{
    public class AccountController : Controller
    {
        private BTcuoikyEntities db = new BTcuoikyEntities();

        //
        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // CẢNH BÁO BẢO MẬT: Đây là cách kiểm tra mật khẩu KHÔNG an toàn
                var user = db.Customers.FirstOrDefault(u => u.Email == model.Email && u.Password == model.Password);

                if (user != null)
                {
                    // Đăng nhập thành công, tạo một "vé" xác thực
                    FormsAuthentication.SetAuthCookie(user.Email, model.RememberMe);
                    // Lưu Role của user vào Session
                    Session["UserRole"] = user.Role;
                    // Quay lại trang chủ
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    // Thêm lỗi vào Model
                    ModelState.AddModelError("", "Email hoặc Mật khẩu không đúng.");
                }
            }

            // Nếu thất bại, chúng ta cần 'thông báo' cho Modal biết là có lỗi
            // Chúng ta lưu lỗi vào TempData
            TempData["LoginError"] = "true";
            TempData["LoginErrors"] = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();


            // Trả về trang chủ (nơi chứa modal)
            return RedirectToAction("Index", "Home");
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra xem Email đã tồn tại chưa
                var existingUser = db.Customers.FirstOrDefault(u => u.Email == model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("", "Email này đã được sử dụng.");
                }
                else
                {
                    // Tạo Customer mới
                    Customer newUser = new Customer
                    {
                        // Tạo một CustomerID ngẫu nhiên (vì nó không tự tăng và là NVARCHAR)
                        CustomerID = Guid.NewGuid().ToString().Substring(0, 10),
                        FullName = model.Email, // Tạm thời dùng Email làm FullName, bạn nên thêm ô FullName vào form
                        Email = model.Email,
                        Password = model.Password, // CẢNH BÁO BẢO MẬT!
                        Role = "User" // Gán vai trò mặc định là "User"
                    };
                    
                    db.Customers.Add(newUser);
                    db.SaveChanges();

                    // Tự động đăng nhập người dùng sau khi đăng ký
                    FormsAuthentication.SetAuthCookie(newUser.Email, false);
                    return RedirectToAction("Index", "Home");
                }
            }

            // Nếu đăng ký thất bại, lưu lỗi và quay lại trang chủ
            TempData["RegisterError"] = "true";
            TempData["RegisterErrors"] = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();

            return RedirectToAction("Index", "Home");
        }

        //
        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
    }
}