using BTcuoiky.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity.Validation;

namespace BTcuoiky.Controllers
{
    [AdminAuthorize]
    public class ProductsController : Controller
    {
        private BTcuoikyEntities db = new BTcuoikyEntities();

        // GET: Products
        public ActionResult Index()
        {
            var products = db.Products.Include(p => p.Category);
            return View(products.ToList());
        }

        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName");

            // 1. Gửi danh sách tất cả Tag sang View để hiển thị checkbox
            ViewBag.AllTags = db.Tags.ToList();

            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductID,ProductName,Unit,Price,CategoryID")] Product product, HttpPostedFileBase ImageFile, int[] SelectedTags)
        {

            // --- 2. KIỂM TRA CÁC LỖI KHÁC (NHƯ BÌNH THƯỜNG) ---
            if (ModelState.IsValid)
            {
                // KIỂM TRA TRÙNG ID
                bool isDuplicate = db.Products.Any(p => p.ProductID == product.ProductID);
                if (isDuplicate)
                {
                    ModelState.AddModelError("ProductID", "Mã sản phẩm (ID) này đã tồn tại. Vui lòng nhập số khác.");
                    ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
                    return View(product);
                }

                // XỬ LÝ UPLOAD ẢNH
                if (ImageFile != null && ImageFile.ContentLength > 0)
                {
                    string extension = Path.GetExtension(ImageFile.FileName).ToLower();
                    if (extension != ".jpg" && extension != ".jpeg" && extension != ".png" && extension != ".gif")
                    {
                        ModelState.AddModelError("ImageUpload", "Chỉ chấp nhận file ảnh (jpg, png, gif).");
                        ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
                        return View(product);
                    }

                    string fileName = Guid.NewGuid().ToString() + extension;
                    string savePath = Path.Combine(Server.MapPath("~/Content/Uploads/Products"), fileName);
                    Directory.CreateDirectory(Path.GetDirectoryName(savePath));
                    ImageFile.SaveAs(savePath);
                    product.image_uri = "/Content/Uploads/Products/" + fileName;
                }

                // XỬ LÝ LƯU TAG
                product.Tags = new List<Tag>(); // Khởi tạo danh sách
                if (SelectedTags != null)
                {
                    foreach (var tagId in SelectedTags)
                    {
                        var tag = db.Tags.Find(tagId);
                        if (tag != null)
                        {
                            product.Tags.Add(tag);
                        }
                    }
                }

                // LƯU VÀO DATABASE (Dùng try-catch để bắt các lỗi khác nếu có)
                try
                {
                    db.Products.Add(product);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                {
                    foreach (var entityValidationErrors in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in entityValidationErrors.ValidationErrors)
                        {
                            ModelState.AddModelError(validationError.PropertyName, validationError.ErrorMessage);
                        }
                    }
                }
            }

            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);

            // Nếu lỗi, gửi lại danh sách Tag để hiện lại form
            ViewBag.AllTags = db.Tags.ToList();
            return View(product);
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Load sản phẩm KÈM THEO TAGS (quan trọng)
            Product product = db.Products.Include("Tags").FirstOrDefault(p => p.ProductID == id);

            if (product == null) return HttpNotFound();

            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);

            // Gửi danh sách Tag sang View
            ViewBag.AllTags = db.Tags.ToList();

            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        // Thêm tham số HttpPostedFileBase ImageFile vào hàm
        public ActionResult Edit([Bind(Include = "ProductID,ProductName,Unit,Price,CategoryID")] Product product, int[] SelectedTags, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {
                var productInDb = db.Products.Include("Tags").FirstOrDefault(p => p.ProductID == product.ProductID);

                if (productInDb != null)
                {
                    // 1. Cập nhật thông tin cơ bản
                    productInDb.ProductName = product.ProductName;
                    productInDb.Unit = product.Unit;
                    productInDb.Price = product.Price;
                    productInDb.CategoryID = product.CategoryID;

                    // 2. Cập nhật Tags
                    productInDb.Tags.Clear();
                    if (SelectedTags != null)
                    {
                        foreach (var tagId in SelectedTags)
                        {
                            var tag = db.Tags.Find(tagId);
                            if (tag != null) productInDb.Tags.Add(tag);
                        }
                    }

                    // 3. XỬ LÝ ẢNH (MỚI THÊM)
                    if (ImageFile != null && ImageFile.ContentLength > 0)
                    {
                        // a. Kiểm tra định dạng file
                        string extension = Path.GetExtension(ImageFile.FileName).ToLower();
                        if (extension != ".jpg" && extension != ".jpeg" && extension != ".png" && extension != ".gif")
                        {
                            ModelState.AddModelError("ImageUpload", "Chỉ chấp nhận file ảnh (jpg, png, gif).");
                            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
                            ViewBag.AllTags = db.Tags.ToList();
                            return View(product);
                        }

                        // b. Tạo tên file mới và lưu
                        string fileName = Guid.NewGuid().ToString() + extension;
                        string savePath = Path.Combine(Server.MapPath("~/Content/Uploads/Products"), fileName);

                        // Tạo thư mục nếu chưa có (dù Create đã làm rồi nhưng cứ thêm cho chắc)
                        Directory.CreateDirectory(Path.GetDirectoryName(savePath));

                        ImageFile.SaveAs(savePath);

                        // c. Cập nhật đường dẫn vào DB
                        productInDb.image_uri = "/Content/Uploads/Products/" + fileName;
                    }
                    // Nếu ImageFile == null, ta KHÔNG làm gì cả, nghĩa là image_uri cũ vẫn được giữ nguyên.

                    // 4. Lưu thay đổi
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            ViewBag.AllTags = db.Tags.ToList();
            return View(product);
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

