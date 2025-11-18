using System;
using System.Collections.Generic;
using System.Data.Entity; // Cần thiết để dùng .Include()
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BTcuoiky.Models; // Cần thiết để hiểu Product, Category...
using PagedList; // Cần thiết để dùng PagedList

namespace BTcuoiky.Controllers
{
    public class HomeController : Controller
    {
        // 1. Thêm dòng này để kết nối database
        private BTcuoikyEntities db = new BTcuoikyEntities();

        // GET: Home/Index
        // page: số trang hiện tại (nullable)
        // categoryID: ID danh mục để lọc (nullable)
        public ActionResult Index(int? page, int? categoryID)
        {
            // 1. Lấy danh sách sản phẩm gốc (kèm Category)
            var products = db.Products.Include(p => p.Category);

            // 2. Xử lý LỌC THEO DANH MỤC (nếu có categoryID)
            if (categoryID != null)
            {
                products = products.Where(p => p.CategoryID == categoryID);

                // Tìm tên danh mục để hiển thị lên tiêu đề
                var category = db.Categories.Find(categoryID);
                if (category != null)
                {
                    ViewBag.CurrentCategoryName = category.CategoryName;
                }
            }
            else
            {
                ViewBag.CurrentCategoryName = "Tất Cả Sản Phẩm";
            }

            // 3. Sắp xếp (Bắt buộc phải có OrderBy trước khi phân trang)
            // Ở đây mình sắp xếp theo tên, bạn có thể đổi thành Price hoặc ID
            products = products.OrderBy(p => p.ProductName);

            // 4. Phân trang
            int pageSize = 8; // Số sản phẩm mỗi trang (bạn đang có layout 4 cột x 2 hàng = 8)
            int pageNumber = (page ?? 1); // Nếu page null thì mặc định là 1

            // Lưu categoryID hiện tại vào ViewBag để giữ trạng thái khi chuyển trang
            ViewBag.CurrentCategoryID = categoryID;

            // Trả về danh sách đã phân trang (IPagedList)
            return View(products.ToPagedList(pageNumber, pageSize));
        }

        // Bạn có thể xóa các Action "About" và "Contact" nếu muốn
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }
    }
}