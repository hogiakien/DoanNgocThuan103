using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DoanNgocThuan103.Models;

namespace DoanNgocThuan103.Controllers
{
    public class QuanLyKhachHangController : Controller
    {
        QuanLyBanHang1 db = new QuanLyBanHang1();
        // GET: QuanLyKhachHang
        // Hiển thị danh sách khách hàng ra trang quản trị.
        public ActionResult Index()
        {
            ViewBag.ThongKeDDHMoi = ThongKeDDHMoi();
            ViewBag.ThongKeDoanhThu = ThongKeDoanhThu();
            var lstKhachHang = db.KhachHangs;
            return View(lstKhachHang);
        }
        public decimal ThongKeDoanhThu()
        {
            decimal TongDoanhThu = db.ChiTietDonDatHangs.Sum(n => n.SoLuong * n.DonGia).Value;
            return TongDoanhThu; // thống kê tổng doanh thu
        }
        private int ThongKeDDHMoi()
        {
            int lstDDH = db.DonDatHangs.Where(n => n.DaThanhToan == false && n.DaHuy == "false").Count();
            return lstDDH;
        }
    }
}