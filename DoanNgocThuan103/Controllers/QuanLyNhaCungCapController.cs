using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DoanNgocThuan103.Models;
namespace DoanNgocThuan103.Controllers
{
    public class QuanLyNhaCungCapController : Controller
    {
        // GET: QuanLyNhaCungCap
        QuanLyBanHang1 db = new QuanLyBanHang1();

        /// <summary>
        /// Lấy ra danh sách nhà cung cấp chưa xóa
        /// </summary>
        public ActionResult Index()
        {
            ViewBag.ThongKeDDHMoi = ThongKeDDHMoi();
            ViewBag.ThongKeDoanhThu = ThongKeDoanhThu();
            var lstNCC = db.NhaCungCaps.Where(n=>n.DaXoa == "false").OrderBy(n => n.MaNCC);
            return View(lstNCC);
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
        /// Trả về trang tạo mới
        /// </summary>
        [HttpGet]
        public ActionResult TaoMoi()
        {
            return View();
        }
        
        /// <summary>
        /// Xử lý thêm nhà cung cấp
        /// </summary>
        [HttpPost]
        public ActionResult TaoMoi(NhaCungCap ncc)
        {
            var ncc1 = db.NhaCungCaps.FirstOrDefault(n => n.TenNCC == ncc.TenNCC);
            if (ncc1 == null)
            {
                ncc.DaXoa = "false";
                db.NhaCungCaps.Add(ncc);
                db.SaveChanges();
                ViewBag.ThongBao = "Thêm thành công";
            }
            else
            {
                ViewBag.ThongBao = "Tên nhà cung cấp đã tồn tại";
            }
            return View();
        }

        /// <summary>
        /// Trả về trang cập nhật sản phẩm
        /// </summary>
        [HttpGet]
        public ActionResult ChinhSua(int? id)
        { 
            //Lấy nhà cung cấp cần chỉnh sửa dựa vào id
            if (id == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            NhaCungCap ncc = db.NhaCungCaps.SingleOrDefault(n => n.MaNCC == id);
            
            if (ncc == null)
            {
                return HttpNotFound();
            }
            return View(ncc);
        }
        
        /// <summary>
        /// Xử lý cập nhật nhà cung cấp
        /// </summary>
        [HttpPost]
        public ActionResult ChinhSua(NhaCungCap ncc)
        {
            ncc.DaXoa = "false";
            db.Entry(ncc).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            ViewBag.ThongBao = "Cập nhật thành công";
   
            return View(ncc);
        }


        [HttpGet]
        public ActionResult Xoa(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NhaCungCap ncc = db.NhaCungCaps.SingleOrDefault(n => n.MaNCC == id);
            if (ncc == null)
            {
                return HttpNotFound();
            }
            ncc.DaXoa = "true";
            db.Entry(ncc).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            ViewBag.ThongBao = "Xóa thành công";
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Trả về danh sách nhà cung cấp đã xóa
        /// </summary>
        /// <returns></returns>
        public ActionResult DanhSachDaXoa()
        {
            var lstNCC = db.NhaCungCaps.Where(n => n.DaXoa == "false").OrderBy(n => n.MaNCC);
            return View(lstNCC);
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
            NhaCungCap ncc = db.NhaCungCaps.SingleOrDefault(n => n.MaNCC == id);
            if (ncc == null)
            {
                return HttpNotFound();
            }
            ncc.DaXoa = "false";
            db.Entry(ncc).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            ViewBag.ThongBao = "Đã hoàn tác";
            return RedirectToAction("Index");
        }
    }
}