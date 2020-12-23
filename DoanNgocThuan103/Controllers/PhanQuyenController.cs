using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DoanNgocThuan103.Models;

namespace DoanNgocThuan103.Controllers
{
    public class PhanQuyenController : Controller
    {
        QuanLyBanHang1 db = new QuanLyBanHang1();
        public ActionResult Index()
        {
            ViewBag.ThongKeDDHMoi = ThongKeDDHMoi();
            ViewBag.ThongKeDoanhThu = ThongKeDoanhThu();
            return View(db.LoaiThanhViens.OrderBy(n => n.MaLoaiTV));
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
        [HttpGet]
        public ActionResult PhanQuyen(int? id)
        {
            //Lấy mã loại tv dựa vào id
            if (id == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            LoaiThanhVien ltv = db.LoaiThanhViens.SingleOrDefault(n => n.MaLoaiTV == id);
            if (ltv == null)
            {
                return HttpNotFound();
            }
            //Lấy ra danh sách quyền để load ra check box
            ViewBag.MaQuyen = db.Quyens;
            //Lấy ra danh sách quyền của loại thành viên đó
            //Bước 1: Lấy ra những quyền thuộc loại thành viên đó dựa vào bảng LoaiThanhVien_Quyen
            ViewBag.LoaiTVQuyen = db.LoaiThanhVien_Quyen.Where(n => n.MaLoaiTV == id);
            return View(ltv);
        }
        [HttpPost]
        public ActionResult PhanQuyen(int? MaLTV, IEnumerable<LoaiThanhVien_Quyen> lstPhanQuyen)
        {

            //Trường hợp : Nếu đã đã tiến hành phân quyền rồi nhưng muốn phân quyền lại
            //Bước 1: Xóa những quyền cũa thuộc loại TV đó
            var lstDaPhanQuyen = db.LoaiThanhVien_Quyen.Where(n => n.MaLoaiTV == MaLTV);
            if (lstDaPhanQuyen.Count() != 0)
            {
                db.LoaiThanhVien_Quyen.RemoveRange(lstDaPhanQuyen);
                db.SaveChanges();
            }
            if (lstPhanQuyen != null)
            {
                //Kiểm tra list danh sách quyền được check
                foreach (var item in lstPhanQuyen)
                {
                    item.MaLoaiTV = int.Parse(MaLTV.ToString());
                    //Nếu được check thì insert dữ liệu vào bảng phân quyền
                    db.LoaiThanhVien_Quyen.Add(item);


                }
                db.SaveChanges();
            }

            return RedirectToAction("Index");
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