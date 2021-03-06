﻿using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DoanNgocThuan103.Models;

namespace DoanNgocThuan103.Controllers
{
    public class TimKiemSPController : Controller
    {
        // GET: TimKiem
        QuanLyBanHang1 db = new QuanLyBanHang1();
        // GET: TimKiem

        public ActionResult TimKiem(string sTuKhoa, int? page)
        {
            if (Request.HttpMethod != "GET")
            {
                page = 1;
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            //tìm kiếm theo ten sản phẩm
            var lstSP = db.SanPhams.Where(n => n.TenSP.Contains(sTuKhoa) && n.DaXoa == "false");
            ViewBag.TuKhoa = sTuKhoa;
            return View(lstSP.OrderBy(n => n.TenSP).ToPagedList(pageNumber, pageSize));
        }

        [HttpPost]
        public ActionResult LayTuKhoa(string sTuKhoa)
        {

            return RedirectToAction("TimKiemSP", new { @sTuKhoa = sTuKhoa });
        }
    }
}