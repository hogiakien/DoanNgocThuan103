using DoanNgocThuan103.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoanNgocThuan103.Controllers
{
    public class LichSuController : Controller
    {
        private QuanLyBanHang1 db = new QuanLyBanHang1();
        // GET: Admin/LichSu
        public ActionResult Index()
        {
            var query = from don in db.DonDatHangs
                        join kh in db.KhachHangs on don.MaKH equals kh.MaKH
                        select new LichSuDTO()
                        {
                            MaDDH = don.MaDDH,
                            NgayDat = don.NgayDat,
                            TinhTrangGiaoHang = don.TinhTrangGiaoHang,
                            DaThanhToan = don.DaThanhToan,
                            TenKH = kh.TenKH
                        };
            var his = query.OrderByDescending(x => x.NgayDat).ToPagedList(1, 10);
            return View(his);
        }

        public ActionResult Detail(int id)
        {
            //id là MaDDH
            var donhang = db.DonDatHangs.Find(id);
            ViewBag.KhachHang = db.KhachHangs.Find(donhang.MaKH);
            var query = db.ChiTietDonDatHangs.Where(x => x.MaDDH == id).ToList();
            return View(query);
        }

        public ActionResult Info(int id)
        {
            //id là MaDDH
            var donhang = db.DonDatHangs.Find(id);
            var query = db.KhachHangs.Find(donhang.MaKH);
            return View(query);
        }

        public ActionResult ChangeStatus(int id)
        {
            //id là MaDDH
            var donhang = db.DonDatHangs.Find(id);
            var query = db.KhachHangs.Find(donhang.MaKH);
            donhang.TinhTrangGiaoHang = true;
            donhang.DaThanhToan = true;
            db.SaveChanges();

            var tv = Session["Admin"] as ThanhVien;
            //Lưu log
            var log = new Log();
            log.Log1 = "Quản trị viên đã Xử lý đơn hàng cho khách hàng:  " + query.TenKH;
            log.MaTV = tv.MaThanhVien;
            log.NgayTao = DateTime.Now;
            new LogDAO().addLog(log);

            return RedirectToAction("Index");
        }
    }
}