using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoanNgocThuan103.Models.ViewModel
{
    public class DonDatHangViewModel
    {
        public int MaDDH { get; set; }
        public Nullable<System.DateTime> NgayDat { get; set; }
        public Nullable<bool> TinhTrangGiaoHang { get; set; }
        public Nullable<System.DateTime> NgayGiao { get; set; }
        public Nullable<bool> DaThanhToan { get; set; }
        public Nullable<int> MaKH { get; set; }
        public string TenKH { get; set; }
        public Nullable<int> UuDai { get; set; }
        public Nullable<bool> DaHuy { get; set; }
        public Nullable<bool> DaXoa { get; set; }
        public virtual KhachHang KhachHang { get; set; }
    }
}