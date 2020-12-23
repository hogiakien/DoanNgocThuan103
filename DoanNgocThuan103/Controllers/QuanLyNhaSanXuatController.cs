using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DoanNgocThuan103.Models;
using System.Data.Entity.Migrations;

namespace DoanNgocThuan103.Controllers
{
    public class QuanLyNhaSanXuatController : Controller
    {
        QuanLyBanHang1 db = new QuanLyBanHang1();
        // GET: QuanLyNhaSanXuat
      
        /// <summary>
        /// Hiển thị danh sách nhà sản xuất
        /// </summary>
        public ActionResult Index()
        {
            ViewBag.ThongKeDDHMoi = ThongKeDDHMoi();
            ViewBag.ThongKeDoanhThu = ThongKeDoanhThu();
            var lstNhaSanXuat = db.NhaSanXuats.Where(n => n.DaXoa == "false").OrderByDescending(n => n.MaNSX);
            return View(lstNhaSanXuat);
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
        /// Trang tạo mới nhà sản xuất
        /// </summary>
        [HttpGet]
        public ActionResult TaoMoi()
        {
            return View();
        }
        // Xử lý tạo mới nhà sản xuất khi người dùng submit lên server
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult TaoMoi(NhaSanXuat nsx, HttpPostedFileBase HinhAnh)
        {
            var temp = db.NhaSanXuats.FirstOrDefault(n => n.TenNSX == nsx.TenNSX);
            if (temp == null)
            {
                if (HinhAnh != null)
                {
                    //Kiểm tra nội dung hình ảnh
                    if (HinhAnh.ContentLength > 0)
                    {
                        //Lấy tên hình ảnh
                        var fileName = Path.GetFileName(HinhAnh.FileName);
                        //Lấy hình ảnh chuyển vào thư mục hình ảnh 
                        var path = Path.Combine(Server.MapPath("~/Content/images/sanpham"), fileName);
                        //Nếu thư mục chứa hình ảnh đó rồi thì xuất ra thông báo
                        if (System.IO.File.Exists(path))
                        {
                            System.IO.File.Delete(path);
                        }
                        HinhAnh.SaveAs(path);
                    }
                    nsx.Logo = HinhAnh.FileName;
                }
                nsx.DaXoa = "false";
                db.NhaSanXuats.Add(nsx);
                db.SaveChanges();
                TempData["ThongBao"] = "Thêm thành công";
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.ThongBao = "Tên nhà cung cấp đã tồn tại";
                return View();
            }
            
        }
        [HttpGet]
        public ActionResult Xoa(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NhaSanXuat nsx = db.NhaSanXuats.SingleOrDefault(n => n.MaNSX == id);
            if (nsx == null)
            {
                return HttpNotFound();
            }
            nsx.DaXoa = "true";
            db.Entry(nsx).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            ViewBag.ThongBao = "Xóa thành công";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult ChinhSua(int? id)
        {
            //Lấy nhà sản xuất cần chỉnh sửa dựa vào id
            if (id == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            NhaSanXuat nsx = db.NhaSanXuats.SingleOrDefault(n => n.MaNSX == id);
            if (nsx == null)
            {
                return HttpNotFound();
            }
            return View(nsx);
        }
        
        [HttpPost]
        public ActionResult ChinhSua(NhaSanXuat nsx, HttpPostedFileBase HinhAnh)
        {
            NhaSanXuat tp = db.NhaSanXuats.SingleOrDefault(n => n.MaNSX == nsx.MaNSX);
            if (tp == null)
            {
                return HttpNotFound();
            }
            if (HinhAnh != null)
            {
                //Kiểm tra nội dung hình ảnh
                if (HinhAnh.ContentLength > 0)
                {
                    //Lấy tên hình ảnh
                    var fileName = Path.GetFileName(HinhAnh.FileName);
                    //Lấy hình ảnh chuyển vào thư mục hình ảnh 
                    var path = Path.Combine(Server.MapPath("~/Content/images/sanpham"), fileName);
                    //Nếu thư mục chứa hình ảnh đó rồi thì xuất ra thông báo
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                    HinhAnh.SaveAs(path);
                }
            }
            if (HinhAnh != null)
            {
                nsx.Logo = HinhAnh.FileName;
            }
            else
            {
                nsx.Logo = tp.Logo;
            }
            nsx.DaXoa = "false";
            db.Set<NhaSanXuat>().AddOrUpdate(nsx);
            db.SaveChanges();
            TempData["ThongBao"] = "Chỉnh sửa thành công";
            return RedirectToAction("Index");
        }
        /// <summary>
        /// Trả về danh sách nhà sản xuất đã xóa
        /// </summary>
        /// <returns></returns>
        public ActionResult DanhSachDaXoa()
        {
            var lstNSX = db.NhaSanXuats.Where(n => n.DaXoa == "true").OrderBy(n => n.MaNSX);
            return View(lstNSX);
        }
        /// <summary>
        /// Xử lý hoàn tác
        /// </summary>
        [HttpGet]
        public ActionResult HoanTac(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NhaSanXuat nsx = db.NhaSanXuats.SingleOrDefault(n => n.MaNSX == id);
            if (nsx == null)
            {
                return HttpNotFound();
            }
            nsx.DaXoa = "false";
            db.Entry(nsx).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            TempData["ThongBao"] = "Hoàn tác thành công";
            return RedirectToAction("DanhSachDaXoa");
        }
    }
}