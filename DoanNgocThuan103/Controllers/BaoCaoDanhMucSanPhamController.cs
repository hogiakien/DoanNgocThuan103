using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using PagedList;
using DoanNgocThuan103.Models;

namespace DoanNgocThuan103.Controllers
{
    public class BaoCaoDanhMucSanPhamController : Controller
    {
        QuanLyBanHang1 db = new QuanLyBanHang1();
        // GET: BaoCaoDanhMucSanPham
        public ActionResult Index(int page = 1, int pageSize = 10)
        {
            ViewBag.ThongKeDDHMoi = ThongKeDDHMoi();
            ViewBag.ThongKeDoanhThu = ThongKeDoanhThu();
            var model = db.SanPhams.OrderBy(x => x.MaSP).ToPagedList(page, pageSize);
            return View(model);
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
        public ActionResult ExportExel()
        {
            GridView gv = new GridView();
            var list = db.SanPhams.Select(p => new
            {
                MaSP = p.MaSP,
                TenSP = p.TenSP,
                LoaiSanPham = p.LoaiSanPham.TenLoai,
                SoLuongTon = p.SoLuongTon,
                DonGia = p.DonGia,
                NhaCungCap = p.NhaCungCap.TenNCC,
                NhaSanXuat = p.NhaSanXuat.TenNSX,
            }).ToList();
            gv.DataSource = list;

            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=ProductReport.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            gv.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

            return RedirectToAction("Index");
        }
    }
}