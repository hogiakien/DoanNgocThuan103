﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DoanNgocThuan103.Models;
using DoanNgocThuan103.Models.ViewModel;

namespace DoanNgocThuan103.Controllers
{
    public class ThongKeController : Controller
    {
        
        QuanLyBanHang1 db = new QuanLyBanHang1();
        public ActionResult Index()
        {
            
            ViewBag.ThongKeDoanhThu = ThongKeDoanhThu();// thống kê doanh thu
            ViewBag.ThongKeDonHang = ThongKeDonHang(); // thống kê đơn hàng
            ViewBag.ThongKeThanhVien = ThongKeThanhVien(); // thống kê thành viên
            ViewBag.ThongKeSanPham = ThongKeSanPham(); // thống kê số lượng sản phẩm tồn
            ViewBag.ThongKeMaLoaiSanPham = ThongKeMaLoaiSanPham();// thống kê nhà sản xuất
            ViewBag.ThongKeDDHMoi = ThongKeDDHMoi();
            ViewBag.ThongKeLoaiSanPham = ThongKeLoaiSanPham();
            return View();
           
        }
        public decimal ThongKeDoanhThu()
        {
            decimal TongDoanhThu = db.ChiTietDonDatHangs.Sum(n => n.SoLuong * n.DonGia).Value;
            return TongDoanhThu; // thống kê tổng doanh thu
        }
        public double ThongKeDonHang()
        {
            // đếm đơn đặt hàng
            double slddh = db.DonDatHangs.Count();
            return slddh;

            //int ddh = 0;
            //var lstDDH = db.DonDatHangs;
            //if (lstDDH.Count() > 0)
            //{
            //    ddh = lstDDH.Count();
            //}
            //return ddh;
        }
        public double ThongKeThanhVien()
        {
            // đếm đơn đặt hàng
            double sltv = db.ThanhViens.Count();
            return sltv;
        }

        public double ThongKeMaLoaiSanPham()
        {
            // đếm đơn đặt hàng
            double slncc = db.MaLoaiSanPhams.Count();
            return slncc;
        }
        public double ThongKeLoaiSanPham()
        {
            // đếm đơn đặt hàng
            double slncc = db.LoaiSanPhams.Count();
            return slncc;
        }
        public int ThongKeSanPham()
        {
            // đếm số lượng sản phẩm
            int sanpham = db.SanPhams.Sum(n => n.SoLuongTon).Value;
            return sanpham;
        }
        public int ThongKeDDHMoi()
        {
            // đếm số lượng sản phẩm
            int lstDDH = db.DonDatHangs.Where(n => n.DaThanhToan == false  && n.DaHuy == "false").Count();
            return lstDDH;
        }
       
        public decimal ThongKeDoanhThuTheoThang(int Thang, int Nam)
        {
            //thống kê tất cả doanh thu
            // list ra don dat hang có ngày tháng năm tương ứng
            var lstDDH = db.DonDatHangs.Where(n => n.NgayDat.Value.Month == Thang && n.NgayDat.Value.Year == Nam);
            decimal TongTien = 0;
            foreach (var item in lstDDH)
            {
                TongTien += decimal.Parse(item.ChiTietDonDatHangs.Sum(n => n.SoLuong * n.DonGia).Value.ToString());
            }
            return TongTien;
        }
       

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (db != null)
                    db.Dispose();
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}