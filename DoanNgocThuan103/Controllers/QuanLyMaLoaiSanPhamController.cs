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
    public class QuanLyMaLoaiSanPhamController : Controller
    {
        //
        // GET: /QuanLyDanhMucCap2/
        QuanLyBanHang1 db = new QuanLyBanHang1();
        public ActionResult Index()
        {
            var lstLSP = db.MaLoaiSanPhams.Where(n => n.DaXoa == false).OrderBy(n => n.IdMLSP);
            return View(lstLSP);
        }
        [HttpGet]
        public ActionResult TaoMoi()
        {
            return View();
        }
        [HttpPost]
        public ActionResult TaoMoi(MaLoaiSanPham mlsp)
        {
            mlsp.DaXoa = false;
            db.MaLoaiSanPhams.Add(mlsp);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult ChinhSua(int? id)
        {
            //Lấy nhà sản xuất cần chỉnh sửa dựa vào id
            if (id == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            MaLoaiSanPham lsp = db.MaLoaiSanPhams.SingleOrDefault(n => n.IdMLSP == id);
            if (lsp == null)
            {
                return HttpNotFound();
            }
            return View(lsp);
        }

        [HttpPost]
        public ActionResult ChinhSua(MaLoaiSanPham lsp, HttpPostedFileBase HinhAnh)
        {
            MaLoaiSanPham tp = db.MaLoaiSanPhams.SingleOrDefault(n => n.IdMLSP == lsp.IdMLSP);
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
                lsp.Logo = HinhAnh.FileName;
            }
            else
            {
                lsp.Logo = tp.Logo;
            }
            lsp.DaXoa = false;
            db.Set<MaLoaiSanPham>().AddOrUpdate(lsp);
            db.SaveChanges();
            TempData["ThongBao"] = "Chỉnh sửa thành công";
            return RedirectToAction("Index");
        }

        public ActionResult Xoa(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MaLoaiSanPham lsp = db.MaLoaiSanPhams.SingleOrDefault(n => n.IdMLSP == id);
            if (lsp == null)
            {
                return HttpNotFound();
            }
            lsp.DaXoa = true;
            db.Entry(lsp).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            TempData["ThongBao"] = "Xóa thành công";
            return RedirectToAction("Index");
        }

        public ActionResult DanhMucDaXoa()
        {
            var lstLSP = db.MaLoaiSanPhams.Where(n => n.DaXoa == true).OrderBy(n => n.IdMLSP);
            return View(lstLSP);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// 
        [HttpGet]
        public ActionResult HoanTac(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MaLoaiSanPham lsp = db.MaLoaiSanPhams.SingleOrDefault(n => n.IdMLSP == id);
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