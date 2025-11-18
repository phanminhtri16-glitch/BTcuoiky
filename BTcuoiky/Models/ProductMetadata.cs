using System;
using System.ComponentModel.DataAnnotations;

namespace BTcuoiky.Models
{
    public class ProductMetadata
    {
        // --- ĐOẠN MỚI THÊM VÀO --- //
        [Required(ErrorMessage = "Vui lòng nhập mã sản phẩm.")] // Đổi thông báo lỗi tiếng Anh -> Tiếng Việt
        public int ProductID { get; set; }


        // --- ĐOẠN CŨ CỦA BẠN (GIỮ NGUYÊN) --- //
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public Nullable<decimal> Price { get; set; }
    }

    [MetadataType(typeof(ProductMetadata))]
    public partial class Product
    {
    }
}