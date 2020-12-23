using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoanNgocThuan103.Models
{
    public class DonHangDTO
    {
        public string TenSP { get; set; }
        public Nullable<decimal> DonGia { get; set; }
        public int SoLuong { get; set; }
        public Nullable<System.DateTime> NgayDat { get; set; }
        public Nullable<bool> TinhTrangGiaoHang { get; set; }
        public Nullable<bool> DaThanhToan { get; set; }
    }
}