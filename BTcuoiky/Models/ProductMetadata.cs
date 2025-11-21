using System;
using System.ComponentModel.DataAnnotations;

namespace BTcuoiky.Models
{
    public class ProductMetadata
    {
        // --- Thông báo lỗi = tiếng việt --- //
        [Required(ErrorMessage = "Vui lòng nhập mã sản phẩm.")]
        public int ProductID { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên sản phẩm.")]
        public int ProductName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số lượng sản phẩm.")]
        public int Unit { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn danh mục sản phẩm.")]
        public Nullable<int> CategoryID { get; set; }

        // --- ĐOẠN CŨ CỦA BẠN (GIỮ NGUYÊN) --- //
        [DisplayFormat(DataFormatString = "{0:N0}")]
        [Required(ErrorMessage = "Vui lòng nhập giá sản phẩm.")]
        public Nullable<decimal> Price { get; set; }
    }

    [MetadataType(typeof(ProductMetadata))]
    public partial class Product
    {
    }
}