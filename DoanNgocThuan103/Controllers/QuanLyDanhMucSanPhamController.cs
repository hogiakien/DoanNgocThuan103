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
    public class QuanLyDanhMucSanPhamController : Controller
    {
        //
        // GET: /QuanLyDanhMucSanPham/
        QuanLyBanHang1 db = new QuanLyBanHang1();
        public ActionResult Index()
        {
            var lstLSP = db.LoaiSanPhams.Where(n => n.DaXoa == false).OrderByDescending(n => n.MaLoaiSP);
            return View(lstLSP);
        }
         [HttpGet]
        public ActionResult TaoMoi()
        {
            return View();
        }
        [HttpPost]
        public ActionResult TaoMoi(LoaiSanPham loaisanpham)
        {
            db.LoaiSanPhams.Add(loaisanpham);
            db.SaveChanges();
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
            LoaiSanPham lsp = db.LoaiSanPhams.SingleOrDefault(n => n.MaLoaiSP == id);
            if (lsp == null)
            {
                return HttpNotFound();
            }
            return View(lsp);
        }

        [HttpPost]
        public ActionResult ChinhSua(LoaiSanPham lsp, HttpPostedFileBase HinhAnh)
        {
            LoaiSanPham tp = db.LoaiSanPhams.SingleOrDefault(n => n.MaLoaiSP == lsp.MaLoaiSP);
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
                lsp.Icon = HinhAnh.FileName;
            }
            else
            {
                lsp.Icon = tp.Icon;
            }
            lsp.DaXoa = false;
            db.Set<LoaiSanPham>().AddOrUpdate(lsp);
            db.SaveChanges();
            TempData["ThongBao"] = "Chỉnh sửa thành công";
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult Xoa(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LoaiSanPham lsp = db.LoaiSanPhams.SingleOrDefault(n => n.MaLoaiSP == id);
            if (lsp == null)
            {
                return HttpNotFound();
            }
            lsp.DaXoa = true;
            db.Entry(lsp).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            ViewBag.ThongBao = "Xóa thành công";
            return RedirectToAction("Index");
        }

        public ActionResult DanhMucDaXoa()
        {
            var lstLSP = db.LoaiSanPhams.Where(n => n.DaXoa == true).OrderBy(n => n.MaLoaiSP);
            return View(lstLSP);
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
            LoaiSanPham lsp = db.LoaiSanPhams.SingleOrDefault(n => n.MaLoaiSP == id);
            if (lsp == null)
            {
                return HttpNotFound();
            }
            lsp.DaXoa = false;
            db.Entry(lsp).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            TempData["ThongBao"] = "Hoàn tác thành công";
            return RedirectToAction("DanhMucDaXoa");
        }
    }
}