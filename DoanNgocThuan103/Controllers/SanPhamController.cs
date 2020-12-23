using PagedList;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using DoanNgocThuan103.Models;
using System;
using System.Data.Entity.Migrations;

namespace DoanNgocThuan103.Controllers
{
    public class SanPhamController : Controller
    {
        private QuanLyBanHang1 db = new QuanLyBanHang1();

        // GET: SanPham
        [ChildActionOnly]
        public ActionResult SanPhamStylePartial()
        {
            return PartialView();
        }

        /// <summary>
        /// 
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult XemChiTiet(int id)
        {

#pragma warning disable CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'
            if (id == null)
#pragma warning restore CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            SanPham sp = db.SanPhams.SingleOrDefault(n => n.MaSP == id  && n.DaXoa == "false");
            sp.LuotXem = sp.LuotXem+1;
            
            if (sp == null)
            {

                return HttpNotFound();
            }
            if (Session["TaiKhoan"] != null)
            {
                ThanhVien tv = (ThanhVien)Session["TaiKhoan"];
               
                var user = db.SanPhams.Find(sp.MaSP);
                user.IdMLSP = sp.IdMLSP;
                db.SaveChanges();
                Log lg = new Log();
                lg.Log1 = "Đã Xem Sản Phẩm" + " " + user.TenSP;
                lg.MaTV = tv.MaThanhVien;
                lg.NgayTao = DateTime.Now;
                new LogDAO().addLog(lg);
                db.SaveChanges();

            }


            return View(sp);
           
        }

        /// <summary>
        /// 
        /// 
        /// </summary>
        /// <param name="MaLoaiSP"></param>
        /// <param name="IdMLSP"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult SanPham(int? MaLoaiSP, int? IdMLSP, int? page)
        {

            if (MaLoaiSP == null || IdMLSP == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var listSanPham = db.SanPhams.Where(n => n.MaLoaiSP == MaLoaiSP && n.IdMLSP == IdMLSP).OrderBy(n => n.TenSP);
            if (!listSanPham.Any())
            {

                return HttpNotFound();
            }



            if (Request.HttpMethod != "GET")
            {
                page = 1;
            }
            int Pagesize = 9;
            // tạo biến số trang hiện tại
            int PageNumber = (page ?? 1);
            ViewBag.MaLoaiSP = MaLoaiSP;
            ViewBag.IdMLSP = IdMLSP;
            return View(listSanPham.OrderBy(n => n.MaSP).ToPagedList(PageNumber, Pagesize));
        }
        public ActionResult TrangSanPham(int? MaLoaiSP, int? page)
        {

            if (MaLoaiSP == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // load sản phẩm dựa vào loại sản phẩm và nhà sản xuất trong bảng sản phẩm
            var lstSanPham = db.SanPhams.Where(n => n.MaLoaiSP == MaLoaiSP);
            if (lstSanPham.Count() == 0)
            {
                // thông báo ko tìm thấy
                return HttpNotFound();
            }
            // thực hiện phân trang
            // tạo biến số sản phẩm trên trang
            if (Request.HttpMethod != "GET")
            {
                page = 1;
            }
            int Pagesize = 9;
            // tạo biến số trang hiện tại
            int PageNumber = (page ?? 1);
            ViewBag.MaLoaiSP = MaLoaiSP;
            return View(lstSanPham.OrderBy(n => n.MaLoaiSP).ToPagedList(PageNumber, Pagesize));
        }

        /// <summary>
        /// </summary>
        /// <param name="MaLoaiSP"></param>
        /// <param name="MaNSX"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult SanPhamGiaTangDan(int? MaLoaiSP, int? IdMLSP, int? page)
        {

            if (MaLoaiSP == null || IdMLSP == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var listSanPham = db.SanPhams.Where(n => n.MaLoaiSP == MaLoaiSP && n.IdMLSP == IdMLSP).OrderBy(n => n.DonGia);
            if (!listSanPham.Any())
            {

                return HttpNotFound();
            }


            const int pageSize = 6;

            int pageNumber = (page ?? 1);
            ViewBag.MaLoaiSP = MaLoaiSP;
            ViewBag.IDMLSP = IdMLSP;
            return View(listSanPham.OrderBy(n => n.DonGia).ToPagedList(pageNumber, pageSize));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="MaLoaiSP"></param>
        /// <param name="MaNSX"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult SanPhamGiaGiamDan(int? MaLoaiSP, int? IdMLSP, int? page)
        {

            if (MaLoaiSP == null || IdMLSP == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var listSanPham = db.SanPhams.Where(n => n.MaLoaiSP == MaLoaiSP && n.IdMLSP == IdMLSP).OrderBy(n => n.DonGia);
            if (!listSanPham.Any())
            {

                return HttpNotFound();
            }


            const int pageSize = 6;

            int pageNumber = (page ?? 1);
            ViewBag.MaLoaiSP = MaLoaiSP;
            ViewBag.IDMLSP = IdMLSP;
            return View(listSanPham.OrderByDescending(n => n.DonGia).ToPagedList(pageNumber, pageSize));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult DanhMucStylePartial()
        {

            var lstSanPham = db.SanPhams;
            ViewBag.lstSanPham = lstSanPham;
            return PartialView();
        }
    }
}