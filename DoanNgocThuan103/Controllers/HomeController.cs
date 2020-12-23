using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DoanNgocThuan103.Models;
using DoanNgocThuan103.Models.ViewModel;


namespace DoanNgocThuan103.Controllers
{
    public class HomeController : Controller
    {
        private QuanLyBanHang1 db = new QuanLyBanHang1();

        /// <summary>
        /// Trả về view trang Index
        /// </summary>
        public ActionResult Index()
        {
           
            // Lần lượt tạo các viewbag để lấy list sản phẩm từ cơ sở dữ liệu
            
            var listSp1 = db.SanPhams.Where(n => n.MaLoaiSP == 1 && n.Moi == "true" && n.DaXoa == "false");
            ViewBag.listSP1 = listSp1;
            var listSp2 = db.SanPhams.Where(n => n.MaLoaiSP == 2 && n.Moi == "true" && n.DaXoa == "false");
            ViewBag.listSP2 = listSp2;
            var listSp3 = db.SanPhams.Where(n => n.MaLoaiSP == 3 && n.Moi == "true" && n.DaXoa == "false");
            ViewBag.listSP3 = listSp3;
            var listPhuKienMoi = db.SanPhams.Where(n => (n.MaLoaiSP == 4 || n.MaLoaiSP == 5 || n.MaLoaiSP == 6) && n.Moi == "true" && n.DaXoa == "false");
            ViewBag.listPhuKienMoi = listPhuKienMoi;
            return View();
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

        public ActionResult MenuPartial()
        {
            var lstSanPham1 = db.SanPhams.Where(n => n.MaLoaiSP == 1 || n.MaLoaiSP == 2 || n.MaLoaiSP == 3);
            ViewBag.lstSanPham1 = lstSanPham1;
            // Truy vấn lấy về 1 list các sản phẩm khác 3 loại trên
            var lstSanPham2 = db.SanPhams.Where(n => n.MaLoaiSP == 4 || n.MaLoaiSP == 5 || n.MaLoaiSP == 6);
            ViewBag.lstSanPham2 = lstSanPham2;
            return PartialView();
        }

        [Obsolete]
        public string mahoa(string pass)
        {
            return FormsAuthentication.HashPasswordForStoringInConfigFile(pass.Trim(), "SHA1");
        }
        /// <summary>
        /// Trả về trang đăng ký
        /// </summary>
        [HttpGet]
        public ActionResult DangKy()
        {
            return View();
        }

        /// <summary>
        /// Xử lý dữ liệu gởi lên từ trang đăng ký
        /// </summary>
        [HttpPost]
        [Obsolete]
        public ActionResult DangKy(ThanhVien tv)
        {
            var tv1 = db.ThanhViens.FirstOrDefault(n => n.TaiKhoan == tv.TaiKhoan);
            if (tv1 == null)
            {
                tv.MaLoaiTV = 4;
                tv.DaXoa = "false";
                // Mã hóa password
                tv.MatKhau = mahoa(tv.MatKhau);
                // Thêm khách hàng vào csdl
                db.ThanhViens.Add(tv);
                db.SaveChanges();

                //Lưu log
                var log = new Log();
                log.Log1 = "Đã ĐĂNG KÝ thành công ";
                log.MaTV = tv.MaThanhVien;
                log.NgayTao = DateTime.Now;
                new LogDAO().addLog(log);

                ViewBag.ThongBao = "Thêm thành công";
            }
            else
            {
                ViewBag.ThongBao = "Tên tài khoản đã tồn tại";
            }

            return View();
        }

        private static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        /// <summary>
        /// Phân quyền cho người dùng
        /// </summary>
        public void PhanQuyen(string TaiKhoan, string Quyen)
        {
            FormsAuthentication.Initialize();
            var ticket = new FormsAuthenticationTicket(1,
                                          TaiKhoan, //user
                                          DateTime.Now, //Thời gian bắt đầu
                                          DateTime.Now.AddHours(1800000), //Thời gian kết thúc
                                          false, //Ghi nhớ?
                                          Quyen, // "DangKy,QuanLyDonHang,QuanLySanPham"
                                          FormsAuthentication.FormsCookiePath);

            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(ticket));
            if (ticket.IsPersistent) cookie.Expires = ticket.Expiration;
            Response.Cookies.Add(cookie);
        }
        [HttpGet]
        public ActionResult LichSu()
        {
            return View();
        }

        /// <summary>
        /// Hàm xử lý đăng nhập
        /// </summary>
        [HttpPost]
        [Obsolete]
        public ActionResult DangNhap(FormCollection f)
        {
            string taikhoan = f["txtTenDangNhap"].ToString();
            string matkhau = mahoa(f["txtMatKhau"].ToString());
            //Truy vấn kiểm tra đăng nhập lấy thông tin thành viên
            ThanhVien tv = db.ThanhViens.SingleOrDefault(n => n.TaiKhoan == taikhoan);
            if (tv != null)
            {
                //if (PasswordHelper.VerifyHash(matkhau, "MD5", tv.MatKhau))
                //{
               
                
                 if (tv.MatKhau == matkhau)
                {
                    //Lấy ra list quyền của thành viên tương ứng với loại thành viên
                    var lstQuyen = db.LoaiThanhVien_Quyen.Where(n => n.MaLoaiTV == tv.MaLoaiTV);
                    //Duyệt list quyền
                    string Quyen = "";
                    if (lstQuyen.Count() != 0)
                    {
                        foreach (var item in lstQuyen)
                        {
                            Quyen += item.Quyen.MaQuyen + ",";
                        }
                        Quyen = Quyen.Substring(0, Quyen.Length - 1); //Cắt dấu ","
                        PhanQuyen(tv.TaiKhoan.ToString(), Quyen);
                    }

                    //Lưu log
                    var log = new Log();
                    log.Log1 = "Đã ĐĂNG NHẬP";
                    log.MaTV = tv.MaThanhVien;
                    log.NgayTao = DateTime.Now;
                    new LogDAO().addLog(log);

                    Session["TaiKhoan"] = tv;
                    return RedirectToAction("Index");
                }
            }
            return Content("Tài khoản hoặc mật khẩu không đúng!");
        }

        /// <summary>
        /// Hàm xử lý đăng xuất
        /// </summary>
        public ActionResult DangXuat()
        {
            var tv = (DoanNgocThuan103.Models.ThanhVien)Session["TaiKhoan"];

            //Lưu log
            var log = new Log();
            log.Log1 = "Đã ĐĂNG XUẤT";
            log.MaTV = tv.MaThanhVien;
            log.NgayTao = DateTime.Now;
            new LogDAO().addLog(log);
            Session["GioHang"] = null;
            Session["TaiKhoan"] = null;

            FormsAuthentication.SignOut();
            return RedirectToAction("Index");
        }

        public ActionResult DoiMatKhau()
        {
          
            return View();
        }

        [HttpPost]
        [Obsolete]
        public ActionResult frmDoiMatKhau(int MaThanhVien, string exMatKhau, string MatKhau)
        {
            var tv = db.ThanhViens.Find(MaThanhVien);
            if(tv.MatKhau != mahoa(exMatKhau))
            {
                Session["error"] = "Mật khẩu cũ không đúng, vui lòng nhập lại.";
                return RedirectToAction("DoiMatKhau");
            }
            else
            {
                tv.MatKhau = mahoa(MatKhau);
                db.SaveChanges();
                ViewBag.ThongBao = "Sửa mật khẩu thành công";
                return RedirectToAction("Index");
            }
            
        }

        public ActionResult QLDonHang(int id)
        {
            //id là MaThanhVien
            var query = from dh in db.DonDatHangs
                        join ct in db.ChiTietDonDatHangs on dh.MaDDH equals ct.MaDDH
                        join kh in db.KhachHangs on dh.MaKH equals kh.MaKH
                        where kh.MaThanhVien == id
                        select new DonHangDTO()
                        {
                            TenSP = ct.TenSP,
                            SoLuong = (int)ct.SoLuong,
                            DonGia = ct.DonGia,
                            NgayDat = dh.NgayDat,
                            DaThanhToan = dh.DaThanhToan,
                            TinhTrangGiaoHang = dh.TinhTrangGiaoHang
                        };
            var model = query.OrderByDescending(x => x.NgayDat).ToList();
            return View(model);
        }

        /// <summary>
        /// Tạo 1 trang kế thừa từ AdminLayout
        /// </summary>
        public ActionResult QuanTri()
        {
            ViewBag.ThongKeDDHMoi = ThongKeDDHMoi();
            ViewBag.ThongKeDoanhThu = ThongKeDoanhThu();
            return View();
        }

        public ActionResult MenuQuanTri()
        {
            
            // Lấy ra danh sách các quyền ứng với tài khoản
            if (Session["TaiKhoan"] != null)
            {
                ThanhVien tv = (ThanhVien)Session["TaiKhoan"];
                List<String> lstQuyen = db.LoaiThanhVien_Quyen.Where(n => n.MaLoaiTV == tv.MaLoaiTV).OrderByDescending(n=>n.MaQuyen).Select(n => n.MaQuyen).ToList();
                ViewBag.lstQuyen = lstQuyen;
            }
            else
            {
                return RedirectToAction("Index","Home");
            }
            return PartialView();
        }
        public JsonResult Statitical_sell()
        {
            var model = from ct in db.ChiTietDonDatHangs
                        join dh in db.DonDatHangs on ct.MaDDH equals dh.MaDDH
                        select new DonHangDTO()
                        {
                            SoLuong = (int)ct.SoLuong,
                            NgayDat = dh.NgayDat
                        };
            var lstmonth = new List<int>();
            var lstSell = new List<Sell_DTO>();
            float tong = 0;
            foreach (var item in model)
            {
                lstmonth.Add(item.NgayDat.Value.Month);
            }
            var query = lstmonth.Distinct().ToList();

            foreach (var item in query)
            {
                var sell = new Sell_DTO();
                var mod = from ct in db.ChiTietDonDatHangs
                          join dh in db.DonDatHangs on ct.MaDDH equals dh.MaDDH
                          where dh.NgayDat.Value.Month == item
                          select new
                          {
                              SoLuong = (int)ct.SoLuong,
                          };
                foreach (var jtem in mod)
                {
                    tong += (float)jtem.SoLuong;
                }

                sell.month = item;
                sell.total = tong;
                lstSell.Add(sell);

                tong = 0;
            }

            var sortLst = lstSell.OrderBy(x => x.month);
            return Json(sortLst, JsonRequestBehavior.AllowGet);

        }
        

        public ActionResult PermissionError()
        {
            return View();
        }
    }
}