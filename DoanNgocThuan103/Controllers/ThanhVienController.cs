using DoanNgocThuan103.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace DoanNgocThuan103.Controllers
{
    public class ThanhVienController : Controller
    {
        private QuanLyBanHang1 db = new QuanLyBanHang1();
        // GET: Admin/ThanhVien
        public ActionResult Index(int page = 1, int pagesize = 5)
        {
            var query = from tv in db.ThanhViens
                        join loai in db.LoaiThanhViens on tv.MaLoaiTV equals loai.MaLoaiTV
                        where tv.DaXoa == "false"
                        select new ThanhVienDTO()
                        {
                            MaThanhVien = tv.MaThanhVien,
                            TaiKhoan = tv.TaiKhoan,
                            HoTen = tv.HoTen,
                            DiaChi = tv.DiaChi,
                            Email = tv.Email,
                            SoDienThoai = tv.SoDienThoai,
                            TenLoai = loai.TenLoai
                        };
            var user = query.OrderByDescending(x => x.MaThanhVien).ToPagedList(page, pagesize);
            return View(user);
        }

        public ActionResult add()
        {
            ViewBag.LoaiTV = db.LoaiThanhViens.ToList();
            return View();
        }

        [HttpPost]
        public ActionResult frmAdd(ThanhVien entity)
        {
            try
            {
                db.ThanhViens.Add(entity);
                db.SaveChanges();

                var tv = Session["TaiKhoan"] as ThanhVien;
                //Lưu log
                var log = new Log();
                log.Log1 = "Quản trị viên đã THÊM thành viên " + entity.TaiKhoan;
                log.MaTV = tv.MaThanhVien;
                log.NgayTao = DateTime.Now;
                new LogDAO().addLog(log);


                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("add");
            }
        }

        public ActionResult edit(int id)
        {
            ViewBag.LoaiTV = db.LoaiThanhViens.ToList();
            var model = db.ThanhViens.Find(id);
            return View(model);
        }

        [HttpPost]
        public ActionResult frmEdit(ThanhVien entity)
        {
            var user = db.ThanhViens.Find(entity.MaThanhVien);
            user.MaLoaiTV = entity.MaLoaiTV;
            db.SaveChanges();

            var tv = Session["TaiKhoan"] as ThanhVien;
            //Lưu log
            var log = new Log();
            log.Log1 = "Quản trị viên đã SỬA thành viên " + user.TaiKhoan;
            log.MaTV = tv.MaThanhVien;
            log.NgayTao = DateTime.Now;
            new LogDAO().addLog(log);

            return RedirectToAction("Index");
        }

        public JsonResult delete(int matv)
        {
           
            ThanhVien thanhVien = db.ThanhViens.SingleOrDefault(n => n.MaThanhVien == matv);
         
           
            thanhVien.DaXoa = "true";
            db.Entry(thanhVien).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
     

            var tv = Session["TaiKhoan"] as ThanhVien;
            //Lưu log
            var log = new Log();
            log.Log1 = "Quản trị viên đã XÓA thành viên " + thanhVien.TaiKhoan;
            log.MaTV = tv.MaThanhVien;
            log.NgayTao = DateTime.Now;
            new LogDAO().addLog(log);

            return Json(new { status = true });
        }
    }
}