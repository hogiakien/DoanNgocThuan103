using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DoanNgocThuan103.Models
{
     [MetadataTypeAttribute(typeof(LoaiSanPhamMetadata))]

    public partial class LoaiSanPham
    {
         internal sealed class LoaiSanPhamMetadata
         {
             public int MaLoaiSP { get; set; }
             [DisplayName("Tên danh mục:")]
             public string TenLoai { get; set; }
            [DisplayName("Mô tả:")]
             public string Icon { get; set; }
             public string BiDanh { get; set; }
    
         }
    }
}