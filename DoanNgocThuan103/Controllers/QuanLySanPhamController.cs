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
    
    public class QuanLySanPhamController : Controller
    {
        QuanLyBanHang1 db = new QuanLyBanHang1();

        /// <summary>
        /// Hiển thị sản phẩm ra trang quản trị
        /// </summary>
        public ActionResult Index()
        {
            ViewBag.ThongKeDDHMoi = ThongKeDDHMoi();
            ViewBag.ThongKeDoanhThu = ThongKeDoanhThu();
            var lstSanPham = db.SanPhams.Where(n => n.DaXoa == "false").OrderByDescending(n => n.MaSP);
            return View(lstSanPham);
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
        public ActionResult TaoMoi()
        {
            // Load dropdownlist nhà cung cấp và nhà sản xuất
            ViewBag.MaNCC = new SelectList(db.NhaCungCaps.OrderBy(n => n.TenNCC), "MaNCC", "TenNCC");
            ViewBag.MaLoaiSP = new SelectList(db.LoaiSanPhams.OrderBy(n => n.MaLoaiSP), "MaLoaiSP", "TenLoai");
            ViewBag.MaNSX = new SelectList(db.NhaSanXuats.OrderBy(n => n.MaNSX), "MaNSX", "TenNSX");
            ViewBag.IdMLSP = new SelectList(db.MaLoaiSanPhams.OrderBy(n => n.IdMLSP), "IdMLSP", "TenLSP");
            return View();
        }

        /// <summary>
        /// 
        /// Xử lý tạo mới sản phẩm khi người dùng submit lên server
        /// </summary>
        /// <param name="sp"></param>
        /// <param name="HinhAnh"></param>
        /// <returns></returns>
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult TaoMoi(SanPham sp, HttpPostedFileBase[] HinhAnh)
        {
            
            // Load dropdownlist nhà cung cấp và nhà sản xuất
            ViewBag.MaNCC = new SelectList(db.NhaCungCaps.OrderBy(n => n.TenNCC), "MaNCC", "TenNCC");
            ViewBag.MaLoaiSP = new SelectList(db.LoaiSanPhams.OrderBy(n => n.MaLoaiSP), "MaLoaiSP", "TenLoai");
            ViewBag.MaNSX = new SelectList(db.NhaSanXuats.OrderBy(n => n.MaNSX), "MaNSX", "TenNSX");
            ViewBag.IdMLSP = new SelectList(db.MaLoaiSanPhams.OrderBy(n => n.IdMLSP), "IdMLSP", "TenLSP");
            
            var temp = db.SanPhams.FirstOrDefault(n => n.TenSP == sp.TenSP);
            if (temp == null)
            {
                for (int i = 0; i < HinhAnh.Count(); i++)
                {
                    if (HinhAnh[i] != null)
                    {
                        //Kiểm tra nội dung hình ảnh
                        if (HinhAnh[i].ContentLength > 0)
                        {
                            //Kiểm tra định dạng hình ảnh
                            if (HinhAnh[i].ContentType != "images/jpeg" && HinhAnh[i].ContentType != "images/png" && HinhAnh[i].ContentType != "images/gif" && HinhAnh[i].ContentType != "images/jpg")
                            {
                                ViewBag.upload += "Hình ảnh" + i + " không hợp lệ <br />";
                                
                            }
                            else
                            {
                                //Kiểm tra hình ảnh tồn tại

                                //Lấy tên hình ảnh
                                var fileName = Path.GetFileName(HinhAnh[0].FileName);
                                //Lấy hình ảnh chuyển vào thư mục hình ảnh 
                                var path = Path.Combine(Server.MapPath("~/Content/images/sanpham"), fileName);
                                //Nếu thư mục chứa hình ảnh đó rồi thì xuất ra thông báo
                                if (System.IO.File.Exists(path))
                                {
                                    ViewBag.upload1 = "Hình " + i + "đã tồn tại <br />";
                                    
                                }
                            }
                        }
                    }

                }

                sp.HinhAnh = HinhAnh[0].FileName;
                sp.HinhAnh1 = HinhAnh[1].FileName;
                sp.HinhAnh2 = HinhAnh[2].FileName;
                sp.HinhAnh3 = HinhAnh[3].FileName;

                
                sp.SoLuongTon = 10;
                db.SanPhams.Add(sp);
                db.SaveChanges();
                TempData["ThongBao"] = "Thêm thành công";
                return RedirectToAction("Index");
            }
            else
            {
                
                return View("ThongBaoTrung");
            }
            
        }

        public ActionResult ChinhSua(int? id)
        {
            //Lấy sản phẩm cần chỉnh sửa dựa vào id
            if (id == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            SanPham sp = db.SanPhams.SingleOrDefault(n => n.MaSP == id);
            if (sp == null)
            {
                return HttpNotFound();
            }

            //Load dropdownlist nhà cung cấp và dropdownlist loại sp, mã nhà sản xuất
            ViewBag.MaNCC = new SelectList(db.NhaCungCaps.OrderBy(n => n.TenNCC), "MaNCC", "TenNCC", sp.MaNCC);
            ViewBag.MaLoaiSP = new SelectList(db.LoaiSanPhams.OrderBy(n => n.MaLoaiSP), "MaLoaiSP", "TenLoai", sp.MaLoaiSP);
            ViewBag.MaNSX = new SelectList(db.NhaSanXuats.OrderBy(n => n.MaNSX), "MaNSX", "TenNSX", sp.MaNSX);
            ViewBag.IdMLSP = new SelectList(db.MaLoaiSanPhams.OrderBy(n => n.TenLSP), "IdMLSP", "TenLSP", sp.MaSP);
            return View(sp);
        }

        /// <summary>
        /// Xử lý update sản phẩm
        /// </summary>
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult ChinhSua(SanPham sp, HttpPostedFileBase[] HinhAnh)
        {
            ViewBag.MaNCC = new SelectList(db.NhaCungCaps.OrderBy(n => n.TenNCC), "MaNCC", "TenNCC", sp.MaNCC);
            ViewBag.MaLoaiSP = new SelectList(db.LoaiSanPhams.OrderBy(n => n.MaLoaiSP), "MaLoaiSP", "TenLoai", sp.MaLoaiSP);
            ViewBag.MaNSX = new SelectList(db.NhaSanXuats.OrderBy(n => n.MaNSX), "MaNSX", "TenNSX", sp.MaNSX);
            ViewBag.IdMLSP = new SelectList(db.MaLoaiSanPhams.OrderBy(n => n.TenLSP), "IdMLSP", "TenLSP", sp.MaSP);
            SanPham tp = db.SanPhams.SingleOrDefault(n => n.MaSP == sp.MaSP);
            // Lưu ảnh
            for (int i = 0; i < HinhAnh.Count(); i++)
            {
                if (HinhAnh[i] != null)
                {
                    //Kiểm tra nội dung hình ảnh
                    if (HinhAnh[i].ContentLength > 0)
                    {
                        //Lấy tên hình ảnh
                        var fileName = Path.GetFileName(HinhAnh[i].FileName);
                        //Lấy hình ảnh chuyển vào thư mục hình ảnh 
                        var path = Path.Combine(Server.MapPath("~/Content/images/sanpham"), fileName);
                        //Nếu thư mục chứa hình ảnh đó rồi thì xuất ra thông báo
                       
                        HinhAnh[i].SaveAs(path);
                    }
                }
            }

            if (HinhAnh[0] != null)
            {
                sp.HinhAnh = HinhAnh[0].FileName;
            }
            else
            {
                sp.HinhAnh = tp.HinhAnh;
            }
            if (HinhAnh[1] != null)
            {
                sp.HinhAnh1 = HinhAnh[1].FileName;
            }
            else
            {
                sp.HinhAnh1 = tp.HinhAnh1;
            }
            if (HinhAnh[2] != null)
            {
                sp.HinhAnh2 = HinhAnh[2].FileName;
            }
            else
            {
                sp.HinhAnh2 = tp.HinhAnh2;
            }
            if (HinhAnh[3] != null)
            {
                sp.HinhAnh3 = HinhAnh[3].FileName;
            }
            else
            {
                sp.HinhAnh3 = tp.HinhAnh3;
            }
            db.Set<SanPham>().AddOrUpdate(sp);

            db.SaveChanges();
            TempData["ThongBao"] = "Chỉnh sửa thành công";
            return RedirectToAction("Index");
        }
        /// <summary>
        /// Xóa thông tin sản phẩm
        /// </summary>

        [HttpGet]
        public ActionResult Xoa(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SanPham sp = db.SanPhams.SingleOrDefault(n => n.MaSP == id);
            if (sp == null)
            {
                return HttpNotFound();
            }
            sp.DaXoa = "true";
            db.Entry(sp).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            TempData["ThongBao"] = "Xóa thành công";

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Trả về danh sách nhà cung cấp đã xóa
        /// </summary>
        public ActionResult DanhSachDaXoa()
        {
            var lstSanPham = db.SanPhams.Where(n => n.DaXoa == "true").OrderByDescending(n => n.MaSP);
            return View(lstSanPham);
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
            SanPham sp = db.SanPhams.SingleOrDefault(n => n.MaSP == id);
            if (sp == null)
            {
                return HttpNotFound();
            }
            sp.DaXoa = "false";
            db.Entry(sp).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            TempData["ThongBao"] = "Hoàn tác thành công";
            return RedirectToAction("Index");
        }

        //[HttpGet]
        //public ActionResult Xoa(int id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    SanPham sp = db.SanPhams.SingleOrDefault(n => n.MaSP == id);
        //    if (sp == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    db.SanPhams.Remove(sp);
        //    db.SaveChanges();

        //    var path = Path.Combine(Server.MapPath("~/Content/images/sanpham"), sp.HinhAnh);
        //    if (System.IO.File.Exists(path))
        //    {
        //        System.IO.File.Delete(path);
        //    }

        //    path = Path.Combine(Server.MapPath("~/Content/images/sanpham"), sp.HinhAnh1);
        //    if (System.IO.File.Exists(path))
        //    {
        //        System.IO.File.Delete(path);
        //    }

        //    path = Path.Combine(Server.MapPath("~/Content/images/sanpham"), sp.HinhAnh2);
        //    if (System.IO.File.Exists(path))
        //    {
        //        System.IO.File.Delete(path);
        //    }

        //    path = Path.Combine(Server.MapPath("~/Content/images/sanpham"), sp.HinhAnh3);
        //    if (System.IO.File.Exists(path))
        //    {
        //        System.IO.File.Delete(path);
        //    }

        //    return RedirectToAction("Index");
        //}
    }
}