using System.Web;
using System.Web.Mvc;

namespace BTcuoiky.Controllers
{
    // Đây là một "Attribute" tùy chỉnh, kế thừa từ AuthorizeAttribute
    public class AdminAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            // 1. Kiểm tra xem user đã đăng nhập chưa (giống [Authorize] bình thường)
            if (!httpContext.User.Identity.IsAuthenticated)
            {
                return false;
            }

            // 2. Kiểm tra Session["UserRole"]
            var userRole = httpContext.Session["UserRole"];
            if (userRole == null)
            {
                return false; // Không có Session Role
            }

            // 3. Kiểm tra xem Role có phải là "Admin" không
            if (userRole.ToString() == "Admin")
            {
                return true; // Là Admin, cho phép truy cập
            }

            return false; // Không phải Admin
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            // Nếu không phải Admin, trả về trang chủ thay vì trang Login
            filterContext.Result = new RedirectToRouteResult(
                new System.Web.Routing.RouteValueDictionary
                {
                    { "controller", "Home" },
                    { "action", "Index" }
                });
        }
    }
}