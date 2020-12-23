using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoanNgocThuan103.Models
{
    [MetadataType(typeof(SanPham.SanPhamMetadata))]
    public partial class SanPham
    {
        internal sealed class SanPhamMetadata
        {
            [DisplayName("Tên sản phẩm")]
            public string TenSP { get; set; }

            [DisplayName("Đơn giá")]
            public Nullable<decimal> DonGia { get; set; }

            [DisplayName("Cấu hình")]
            
            public string CauHinh { get; set; }

            [DisplayName("Mô tả")]
            [AllowHtml] 
            public string MoTa { get; set; }

            [DisplayName("Hình ảnh")]
            public string HinhAnh { get; set; }

            [DisplayName("Số lượng tồn")]
            public Nullable<int> SoLuongTon { get; set; }

            [DisplayName("Mới")]
            public Nullable<bool> Moi { get; set; }

            [DisplayName("Nhà cung cấp")]
            public Nullable<int> MaNCC { get; set; }

            [DisplayName("Nhà sản xuất")]
            public Nullable<int> MaNSX { get; set; }

            [DisplayName("Loại sản phẩm")]
            public Nullable<int> MaLoaiSP { get; set; }

            [DisplayName("Đã xóa")]
            public Nullable<bool> DaXoa { get; set; }

            [DisplayName("Hình ảnh 1")]
            public string HinhAnh1 { get; set; }

            [DisplayName("Hình ảnh 2")]
            public string HinhAnh2 { get; set; }

            [DisplayName("Hình ảnh 3")]
            public string HinhAnh3 { get; set; }
        }
    }
}