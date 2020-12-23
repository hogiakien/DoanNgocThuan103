using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DoanNgocThuan103.Models;

namespace DoanNgocThuan103.Controllers
{
    public class QuanLyNhanVienController : Controller
    {
        QuanLyBanHang1 db = new QuanLyBanHang1();
        // GET: QuanLyNhanVien

        /// <summary>
        /// Trả về view hiển thị danh sách nhân viên còn hoạt động
        /// </summary>
        public ActionResult Index()
        {
            ViewBag.ThongKeDDHMoi = ThongKeDDHMoi();
            ViewBag.ThongKeDoanhThu = ThongKeDoanhThu();
            var lstNhanVien = db.ThanhViens.Where(n => n.MaLoaiTV == 3 && n.DaXoa == "false"); // Nhân viên bán hàng trong cửa hàng còn hoạt động
            return View(lstNhanVien);
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
        /// <summary>
        /// Trang thêm mới nhân viên
        /// </summary>
        [HttpGet]
        public ActionResult TaoMoi()
        {
            return View();
        }

        [Obsolete]
        public string mahoa(string pass)
        {
            return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(pass.Trim(), "SHA1");
        }

        /// <summary>
        /// Xử lý thêm mới nhân viên
        /// </summary>
        [HttpPost]
        [Obsolete]
        public ActionResult TaoMoi(ThanhVien tv)
        {
            var tv1 = db.ThanhViens.FirstOrDefault(n => n.TaiKhoan == tv.TaiKhoan);
            if (tv1 == null)
            {
                tv.MaLoaiTV = 3;
                tv.DaXoa = "false";
                // Mã hóa password
                tv.MatKhau = mahoa(tv.MatKhau);
                // Thêm khách hàng vào csdl
                db.ThanhViens.Add(tv);
                db.SaveChanges();
                ViewBag.ThongBao = "Thêm thành công";
            }
            else
            {
                ViewBag.ThongBao = "Tên tài khoản đã tồn tại";
            }

            return View();
        }

        /// <summary>
        /// lấy ra các byte từ chuỗi str
        /// </summary>

        private static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        /// <summary>
        /// Chỉnh sửa thông tin nhân viên
        /// </summary>
        [HttpGet]
        public ActionResult ChinhSua(int? id)
        {
            //Lấy sản phẩm cần chỉnh sửa dựa vào id
            if (id == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            ThanhVien tv = db.ThanhViens.SingleOrDefault(n => n.MaThanhVien == id);
            if (tv == null)
            {
                return HttpNotFound();
            }
            return View(tv);
        }

        /// <summary>
        /// Xử lý chỉnh sửa nhân viên
        /// </summary>
        [HttpPost]
        public ActionResult ChinhSua(ThanhVien tv)
        {
            tv.MaLoaiTV = 3;
            tv.DaXoa = "false";
            db.Entry(tv).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            ViewBag.ThongBao = "Cập nhật thành công";

            return View(tv);
        }

        /// <summary>
        /// Xóa nhân viên
        /// </summary>
        [HttpGet]
        public ActionResult Xoa(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ThanhVien tv = db.ThanhViens.SingleOrDefault(n => n.MaThanhVien == id);
            if (tv == null)
            {
                return HttpNotFound();
            }
            tv.DaXoa = "True";
            db.Entry(tv).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            ViewBag.ThongBao = "Xóa thành công";
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Trả về view hiển thị danh sách nhân viên đã xóa
        /// </summary>
        public ActionResult DanhSachDaXoa()
        {
            var lstNhanVien = db.ThanhViens.Where(n => n.MaLoaiTV == 3 && n.DaXoa == "True"); // Nhân viên bán hàng trong cửa hàng còn hoạt động
            return View(lstNhanVien);
        }

        /// <summary>
        /// Xử lý hoàn tác
        /// </summary>
        public ActionResult HoanTac(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ThanhVien tv = db.ThanhViens.SingleOrDefault(n => n.MaThanhVien == id);
            if (tv == null)
            {
                return HttpNotFound();
            }
            tv.DaXoa = "false";
            db.Entry(tv).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            ViewBag.ThongBao = "Đã hoàn tác";
            return RedirectToAction("Index");
        }
    }
}