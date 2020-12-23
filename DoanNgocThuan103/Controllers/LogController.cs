using DoanNgocThuan103.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoanNgocThuan103.Controllers
{
    public class LogController : Controller
    {
        private QuanLyBanHang1 db = new QuanLyBanHang1();
        // GET: Admin/Log
        public ActionResult Index(int page = 1, int pagesize = 10)
        {
            var query = from log in db.Logs
                        join tv in db.ThanhViens on log.MaTV equals tv.MaThanhVien
                        select new LogDAO()
                        {
                            Log_ID = log.Log_ID,
                            Log1 = log.Log1,
                            NgayTao = log.NgayTao,
                            TaiKhoan = tv.TaiKhoan
                        };
            var lg = query.OrderByDescending(x => x.NgayTao).ToPagedList(page, pagesize);
            return View(lg);
        }

        public JsonResult autocomplete_log(string keyword)
        {
            var log = db.Logs.Where(x => x.Log1.Contains(keyword)).Select(x => x.Log1).ToList();
            return Json(new
            {
                data = log,
                status = true
            }, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public ActionResult SearchLog(string log)
        {
            var query = from lg in db.Logs
                        join tv in db.ThanhViens on lg.MaTV equals tv.MaThanhVien
                        select new LogDAO()
                        {
                            Log_ID = lg.Log_ID,
                            Log1 = lg.Log1,
                            NgayTao = lg.NgayTao,
                            TaiKhoan = tv.TaiKhoan
                        };
            var Log = query.Where(x => x.Log1.Contains(log)).ToList().OrderByDescending(x => x.NgayTao).ToPagedList(1, 10);
            ViewBag.log = log;
            return View(Log);

        }
    }
}