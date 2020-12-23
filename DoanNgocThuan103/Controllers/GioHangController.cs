using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Migrations;
using System.Linq;

using System.Web.Mvc;
using DoanNgocThuan103.Models;
using DoanNgocThuan103.Models.ClassLibrary;
using Newtonsoft.Json.Linq;

namespace DoanNgocThuan103.Controllers
{
    public class GioHangController : Controller
    {
        private QuanLyBanHang1 db = new QuanLyBanHang1();

        /// <summary>
        /// Xem giỏ hàng
        /// </summary>
        /// <returns></returns>
        public ActionResult XemGioHang()
        {
            // Lấy giỏ hàng
            List<ItemGioHang> lstGioHang = LayGioHang();
            return View(lstGioHang);
        }

        /// <summary>
        /// Lấy giỏ hàng
        /// </summary>
        public List<ItemGioHang> LayGioHang()
        {
            List<ItemGioHang> lstGioHang = Session["GioHang"] as List<ItemGioHang>;
            if (lstGioHang == null)
            {
                // Nếu session giỏ hàng chưa tồn tại thì khởi tạo giỏ hàng
                lstGioHang = new List<ItemGioHang>();
                Session["GioHang"] = lstGioHang;
            }
            return lstGioHang;
        }

        /// <summary>
        /// Thêm giỏ hàng bằng cách thêm vào session
        /// </summary>
        public ActionResult ThemGioHang(int MaSP, string strURL)
        {
            // Kiểm tra sản phẩm có tồn tại trong CSDL hay không
            SanPham sp = db.SanPhams.SingleOrDefault(n => n.MaSP == MaSP);
            ThanhVien tv = Session["TaiKhoan"] as ThanhVien;
            if (tv == null)
            {
                var khachhang = db.KhachHangs.Where(n => n.MaThanhVien == tv.MaThanhVien);

                if (sp == null)
                {
                    // Không tìm thấy sản phẩm
                    Response.StatusCode = 404;
                    return null;
                }
                // Lấy giỏ hàng từ session
                List<ItemGioHang> lstGioHang = LayGioHang();
                // Trường hợp nếu sản phẩm đã tồn tại trong giỏ hàng
                ItemGioHang spCheck = lstGioHang.SingleOrDefault(n => n.MaSP == MaSP);
                if (spCheck != null)
                {
                    // Kiểm tra số lượng tồn trước khi cho khách hàng mua hàng
                    if (sp.SoLuongTon - spCheck.SoLuong < spCheck.SoLuong)
                    {
                        // Hết sản phẩm có thể mua
                        return View("ThongBao");
                    }
                    spCheck.SoLuong++;
                    spCheck.ThanhTien = spCheck.SoLuong * spCheck.DonGia;

                    return Redirect(strURL);
                }

                ItemGioHang itemGH = new ItemGioHang(MaSP);
                if (sp.SoLuongTon < itemGH.SoLuong)
                {
                    return View("ThongBao");
                }

                lstGioHang.Add(itemGH);
            }
            else
            {
                var khachhang = db.KhachHangs.Where(n => n.MaThanhVien == tv.MaThanhVien);

                if (sp == null)
                {
                    // Không tìm thấy sản phẩm
                    Response.StatusCode = 404;
                    return null;
                }
                // Lấy giỏ hàng từ session
                List<ItemGioHang> lstGioHang = LayGioHang();
                // Trường hợp nếu sản phẩm đã tồn tại trong giỏ hàng
                ItemGioHang spCheck = lstGioHang.SingleOrDefault(n => n.MaSP == MaSP);
                if (spCheck != null)
                {
                    // Kiểm tra số lượng tồn trước khi cho khách hàng mua hàng
                    if (sp.SoLuongTon - spCheck.SoLuong < spCheck.SoLuong)
                    {
                        // Hết sản phẩm có thể mua
                        return View("ThongBao");
                    }
                    spCheck.SoLuong++;
                    spCheck.ThanhTien = spCheck.SoLuong * spCheck.DonGia;

                    return Redirect(strURL);
                }

                ItemGioHang itemGH = new ItemGioHang(MaSP);
                if (sp.SoLuongTon < itemGH.SoLuong)
                {
                    return View("ThongBao");
                }

                lstGioHang.Add(itemGH);
                Log lg = new Log();

                lg.Log1 = "Đã THÊM VÀO GIỎ HÀNG " + " " + sp.TenSP;
                lg.MaTV = tv.MaThanhVien;
                lg.NgayTao = DateTime.Now;
                new LogDAO().addLog(lg);
                db.SaveChanges();
            }
            return Redirect(strURL);
        }


        /// <summary>
        /// Tính tổng số lượng sản phẩm mua
        /// </summary>
        public double TinhTongSoLuong()
        {
            // Lấy giỏ hàng
            List<ItemGioHang> lstGioHang = Session["GioHang"] as List<ItemGioHang>;
            if (lstGioHang == null)
            {
                return 0;
            }
            return lstGioHang.Sum(n => n.SoLuong);
        }

        /// <summary>
        /// Tính tổng tiền trong giỏ hàng
        /// </summary>
        public decimal TinhTongTien()
        {
            // Lấy giỏ hàng
            List<ItemGioHang> lstGioHang = Session["GioHang"] as List<ItemGioHang>;
            if (lstGioHang == null)
            {
                return 0;
            }
            return lstGioHang.Sum(n => n.ThanhTien);
        }

        /// <summary>
        /// Làm giao diện cho trang giỏ hàng
        /// </summary>
        public ActionResult GioHangPartial()
        {
            if (TinhTongSoLuong() == 0)
            {
                ViewBag.TongSoLuong = 0;
                ViewBag.TongTien = 0;
                return PartialView();
            }
            ViewBag.TongSoLuong = TinhTongSoLuong();
            ViewBag.TongTien = TinhTongTien();
            return PartialView();
        }

        /// <summary>
        /// Sửa giỏ hàng
        /// </summary>
        public ActionResult SuaGioHang(int MaSP)
        {
            // Kiểm tra session giỏ hàng tồn tại chưa
            if (Session["GioHang"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            // Kiểm tra sản phẩm có tồn tại trong CSDL hay không
            SanPham sp = db.SanPhams.SingleOrDefault(n => n.MaSP == MaSP);
            if (sp == null)
            {
                // Báo không tìm thấy sản phẩm
                Response.StatusCode = 404;
                return null;
            }
            // Lấy list giỏ hàng từ session
            List<ItemGioHang> lstGioHang = LayGioHang();
            // Kiểm tra xem sản phẩm đó có tồn tại trong giỏ hàng hay không
            ItemGioHang spCheck = lstGioHang.SingleOrDefault(n => n.MaSP == MaSP);
            if (spCheck == null)
            {
                return RedirectToAction("Index", "Home");
            }
            // Lấy list giỏ hàng gởi đến giao diện
            ViewBag.GioHang = lstGioHang;

            // Nếu tồn tại rồi
            return View(spCheck);
        }

        /// <summary>
        /// Xử lý khi nhấp nút cập nhật bên view sửa giỏ hàng
        /// </summary>
        [HttpPost]
        public ActionResult CapNhatGioHang(ItemGioHang itemGH)
        {
            // Kiểm tra số lượng tồn
            SanPham spCheck = db.SanPhams.Single(n => n.MaSP == itemGH.MaSP);
            if (spCheck.SoLuongTon < itemGH.SoLuong)
            {
                return View("ThongBao");
            }
            // Cập nhật số lượng trong session giỏ hàng
            // Bước 1: Lấy list giỏ hàng từ session giỏ hàng
            List<ItemGioHang> lstGH = LayGioHang();
            // Bước 2: Lấy sản phẩm cần cập nhật từ trong list giỏ hàng ra
            ItemGioHang itemGHUpdate = lstGH.Find(n => n.MaSP == itemGH.MaSP);
            // Bước 3: Tiến hành cập nhật lại số lượng cũng như thành tiền
            itemGHUpdate.SoLuong = itemGH.SoLuong;
            itemGHUpdate.ThanhTien = itemGHUpdate.SoLuong * itemGHUpdate.DonGia;
            return RedirectToAction("XemGioHang");
        }

        /// <summary>
        /// Xóa giỏ hàng
        /// </summary>
        public ActionResult XoaGioHang(int MaSP)
        {
            // Kiểm tra session giỏ hàng tồn tại chưa
            if (Session["Giohang"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            // Kiểm tra sản phẩm có tồn tại trong CSDL hay không
            SanPham sp = db.SanPhams.SingleOrDefault(n => n.MaSP == MaSP);
            if (sp == null)
            {
                // Không tìm thấy sản phẩm trong csdl
                Response.StatusCode = 404;
                return null;
            }
            // Lấy list giỏ hàng từ session
            List<ItemGioHang> lstGioHang = LayGioHang();
            // Kiểm tra xem sản phẩm đó có tồn tại trong giỏ hàng hay không
            ItemGioHang spCheck = lstGioHang.SingleOrDefault(n => n.MaSP == MaSP);
            if (spCheck == null)
            {
                return RedirectToAction("Index", "Home");
            }
            // Xóa item trong giỏ hàng
            lstGioHang.Remove(spCheck);
            return RedirectToAction("XemGioHang");
        }

        public ActionResult XoaToanBoGioHang()
        {
            // Kiểm tra session giỏ hàng tồn tại chưa
            
            Session["GioHang"] = null;
            return RedirectToAction("XemGioHang");
        }

        /// <summary>
        /// Đặt hàng từ giỏ hàng
        /// </summary>
        public ActionResult DatHang(KhachHang kh, DonDatHang model, IEnumerable<ChiTietDonDatHang> lstModel)
        {
          
               
            // Kiểm tra session giỏ hàng tồn tại chưa
            if (Session["GioHang"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            KhachHang kHang = new KhachHang();
            
            if (Session["TaiKhoan"] == null)
            {
                // Thêm khách hàng vào bảng khách hàng đối với khác hàng vãng lai (kh chưa có tài khoản)
                kHang = kh;
                db.KhachHangs.Add(kHang);
                db.SaveChanges();
            }
            else
            {
                ThanhVien tv = Session["TaiKhoan"] as ThanhVien;
               
                var khachhang = db.KhachHangs.Where(n => n.MaThanhVien == tv.MaThanhVien);
                Log lg = new Log();
               
                
                    lg.Log1 = "Đã ĐẶT HÀNG ";
                    lg.MaTV = tv.MaThanhVien;
                    lg.NgayTao = DateTime.Now;
                    new LogDAO().addLog(lg);
                    db.SaveChanges();
                
                if (!khachhang.Any())
                {
                    kHang.TenKH = tv.HoTen;
                    kHang.DiaChi = tv.DiaChi;
                    kHang.Email = tv.Email;
                    kHang.SoDienThoai = tv.SoDienThoai;
                    kHang.MaThanhVien = tv.MaThanhVien;
                    db.KhachHangs.Add(kHang);
                    db.SaveChanges();

                    //Lưu log
                   
                }
                else
                {
                    kHang = khachhang.FirstOrDefault();
                }
            }

            // Thêm đơn hàng
            DonDatHang ddh = new DonDatHang();
            ddh.MaKH = kHang.MaKH;
            ddh.NgayDat = DateTime.Now;
            ddh.TinhTrangGiaoHang = false;
            ddh.DaThanhToan = false;
            ddh.UuDai = 0;
            ddh.DaXoa = "false";
            ddh.DaHuy = "false";
            db.DonDatHangs.Add(ddh);
            

            db.SaveChanges();
            

            SanPham sp = new SanPham();
              List<ItemGioHang> gh = LayGioHang();

              foreach (var item in gh)
              {


                  
                  sp = db.SanPhams.Single(n => n.MaSP == item.MaSP);
                 sp.SoLuongTon = sp.SoLuongTon - item.SoLuong;
                   
                    db.SaveChanges();
                    ViewBag.ThongBao = "Đã đặt hàng thành công";

              }






            
            // Thêm chi tiết đơn đặt hàng
            List<ItemGioHang> lstGH = LayGioHang();
            foreach (var item in lstGH)
            {
               
                    ChiTietDonDatHang ctdh = new ChiTietDonDatHang();
                    ctdh.MaDDH = ddh.MaDDH;
                    ctdh.MaSP = item.MaSP;
                    ctdh.TenSP = item.TenSP;
                    ctdh.SoLuong = item.SoLuong;
                    ctdh.DonGia = item.DonGia * ctdh.SoLuong;
                    db.ChiTietDonDatHangs.Add(ctdh);
                   
                
                
               

            }
            db.SaveChanges();
            string content = System.IO.File.ReadAllText(Server.MapPath("/assets/client/template/neworder.html"));

            content = content.Replace("{{CustomerName}}", ddh.KhachHang.TenKH);
            content = content.Replace("{{Phone}}", ddh.KhachHang.SoDienThoai);
            content = content.Replace("{{Email}}", ddh.KhachHang.Email);
            content = content.Replace("{{Address}}", ddh.KhachHang.DiaChi);
            content = content.Replace("{{Total}}", TinhTongTien().ToString("N0"));
            var toEmail = ddh.KhachHang.Email;
            new Mail().SendMail(toEmail, "Đơn hàng mới từ DNT Bookstore", content);
            var toEmails = ConfigurationManager.AppSettings["ToEmailAddress"].ToString();
            new Mail().SendMail(toEmails, "Đơn hàng mới ", content);

            db.SaveChanges();
                Session["GioHang"] = null;
                return RedirectToAction("XemGioHang");
            
        }
        public ActionResult ThanhToan(KhachHang kh, DonDatHang model, IEnumerable<ChiTietDonDatHang> lstModel)
        {
            List<ItemGioHang> gioHang = Session["GioHang"] as List<ItemGioHang>;
            if (Session["GioHang"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            KhachHang kHang = new KhachHang();

            if (Session["TaiKhoan"] == null)
            {
                // Thêm khách hàng vào bảng khách hàng đối với khác hàng vãng lai (kh chưa có tài khoản)
                kHang = kh;
                db.KhachHangs.Add(kHang);
                db.SaveChanges();
            }
            else
            {
                ThanhVien tv = Session["TaiKhoan"] as ThanhVien;

                var khachhang = db.KhachHangs.Where(n => n.MaThanhVien == tv.MaThanhVien);
                Log lg = new Log();


                lg.Log1 = "Đã ĐẶT HÀNG ";
                lg.MaTV = tv.MaThanhVien;
                lg.NgayTao = DateTime.Now;
                new LogDAO().addLog(lg);
                db.SaveChanges();

                if (!khachhang.Any())
                {
                    kHang.TenKH = tv.HoTen;
                    kHang.DiaChi = tv.DiaChi;
                    kHang.Email = tv.Email;
                    kHang.SoDienThoai = tv.SoDienThoai;
                    kHang.MaThanhVien = tv.MaThanhVien;
                    db.KhachHangs.Add(kHang);
                    db.SaveChanges();

                    //Lưu log

                }
                else
                {
                    kHang = khachhang.FirstOrDefault();
                }
            }

            // Thêm đơn hàng
            DonDatHang ddh = new DonDatHang();
            ddh.MaKH = kHang.MaKH;
            ddh.NgayDat = DateTime.Now;
            ddh.TinhTrangGiaoHang = false;
            ddh.DaThanhToan = false;
            ddh.UuDai = 0;
            ddh.DaXoa = "false";
            ddh.DaHuy = "false";
            db.DonDatHangs.Add(ddh);


            db.SaveChanges();


            SanPham sp = new SanPham();
            List<ItemGioHang> gh = LayGioHang();

            foreach (var item in gh)
            {



                sp = db.SanPhams.Single(n => n.MaSP == item.MaSP);
                sp.SoLuongTon = sp.SoLuongTon - item.SoLuong;

                db.SaveChanges();
                

            }







            // Thêm chi tiết đơn đặt hàng
            List<ItemGioHang> lstGH = LayGioHang();
            foreach (var item in lstGH)
            {

                ChiTietDonDatHang ctdh = new ChiTietDonDatHang();
                ctdh.MaDDH = ddh.MaDDH;
                ctdh.MaSP = item.MaSP;
                ctdh.TenSP = item.TenSP;
                ctdh.SoLuong = item.SoLuong;
                ctdh.DonGia = item.DonGia * ctdh.SoLuong;
                db.ChiTietDonDatHangs.Add(ctdh);





            }
            db.SaveChanges();
            string endpoint = ConfigurationManager.AppSettings["endpoint"].ToString();
            string partnerCode = ConfigurationManager.AppSettings["partnerCode"].ToString();
            string accessKey = ConfigurationManager.AppSettings["accessKey"].ToString();
            string serectkey = ConfigurationManager.AppSettings["serectkey"].ToString();
            string orderInfo = "DH" + DateTime.Now.ToString("yyyyMMddHHmmss");
            string returnUrl = ConfigurationManager.AppSettings["returnUrl"].ToString();
            string notifyurl = ConfigurationManager.AppSettings["notifyurl"].ToString();

            string amount = TinhTongTien().ToString();
            string orderid = Guid.NewGuid().ToString();
            string requestId = Guid.NewGuid().ToString();
            string extraData = "";

            //before sign HMAC SHA256 signature
            string rawHash = "partnerCode=" +
                partnerCode + "&accessKey=" +
                accessKey + "&requestId=" +
                requestId + "&amount=" +
                amount + "&orderId=" +
                orderid + "&orderInfo=" +
                orderInfo + "&returnUrl=" +
                returnUrl + "&notifyUrl=" +
                notifyurl + "&extraData=" +
                extraData;
            MoMoSecurity crypto = new MoMoSecurity();
            string signature = crypto.signSHA256(rawHash, serectkey);
            JObject message = new JObject
            {
                { "partnerCode", partnerCode },
                { "accessKey", accessKey },
                { "requestId", requestId },
                { "amount", amount },
                { "orderId", orderid },
                { "orderInfo", orderInfo },
                { "returnUrl", returnUrl },
                { "notifyUrl", notifyurl },
                { "requestType", "captureMoMoWallet" },
                { "signature", signature }

            };
            string responseFromMomo = PaymentRequest.sendPaymentRequest(endpoint, message.ToString());
            JObject jmessage = JObject.Parse(responseFromMomo);
            return Redirect(jmessage.GetValue("payUrl").ToString());
        }
        public ActionResult ReturnUrl()
        {
            string param = Request.QueryString.ToString().Substring(0, Request.QueryString.ToString().IndexOf("signature") - 1);
            param = Server.UrlDecode(param);
            MoMoSecurity crypto = new MoMoSecurity();
            string serectkey = ConfigurationManager.AppSettings["serectkey"].ToString();
            string signature = crypto.signSHA256(param, serectkey);
            if (signature != Request["signature"].ToString())
            {
                ViewBag.message = "Thông tin Request không hợp lệ";
                return View();
            }
            if (!Request.QueryString["errorCode"].Equals("0"))
            {
                ViewBag.message = "Thanh toán thất bại";
            }
            else
            {
                ViewBag.message = "Thanh toán thành công";
                Session["GioHang"] = new List<ItemGioHang>();

            }
            return View();

        }
        public JsonResult NotifyUrl(int id)
        {
            var donhang = db.DonDatHangs.Find(id);
            var query = db.KhachHangs.Find(donhang.MaKH);
            string param = "";//Request.Form.ToString().Substring(0, Request.Form.ToString().IndexOf("signature") - 1);
            param = "partner_code=" + Request["partner_code"] +
                    "&access_key=" + Request["access_key"] +
                    "&amount=" + Request["amount"] +
                    "&order_id=" + Request["order_id"] +
                    "&order_info=" + Request["order_info"] +
                    "&order_type=" + Request["order_type"] +
                    "&transaction_id=" + Request["transaction_id"] +
                    "&message=" + Request["message"] +
                    "&response_time=" + Request["response_time"] +
                    "&status_code=" + Request["status_code"];

            param = Server.UrlDecode(param);
            MoMoSecurity crypto = new MoMoSecurity();
            string serectkey = ConfigurationManager.AppSettings["serectkey"].ToString();
            string signature = crypto.signSHA256(param, serectkey);
            //Không được phép cập nhật trang thái đơn khi trạng đơn trong Database khác trạng đang chờ thanh toán
            //Trạng thái đơn kích nút thanh toán - Đang chờ thanh toán (-1)
            //Trạng thái giao dịch thành công (1)
            //Trạng thái giao dịch thất bại (0)
            
           
            if (signature != Request["signature"].ToString())
            {

                donhang.DaThanhToan = false;
            }
            string status_code = Request["status_code"].ToString();
            if ((status_code != "0"))
            {
                //Thất bại - Cập nhật trang thái đơn hàng
                string content = System.IO.File.ReadAllText(Server.MapPath("/assets/client/template/neworder.html"));

                content = content.Replace("{{CustomerName}}", donhang.KhachHang.TenKH);
                content = content.Replace("{{Phone}}", donhang.KhachHang.SoDienThoai);
                content = content.Replace("{{Email}}", donhang.KhachHang.Email);
                content = content.Replace("{{Address}}", donhang.KhachHang.DiaChi);
                content = content.Replace("{{Total}}", TinhTongTien().ToString("N0"));
                var toEmail = donhang.KhachHang.Email;
                new Mail().SendMail(toEmail, "Đơn hàng mới từ DNT Bookstore", content);
                var toEmails = ConfigurationManager.AppSettings["ToEmailAddress"].ToString();
                new Mail().SendMail(toEmails, "Đơn hàng mới ", content);
                donhang.DaThanhToan = false;
                db.SaveChanges();
            }
            else
            {
                //Thành công - Cập nhật trang thái đơn hàng

                donhang.DaThanhToan = true;
                db.SaveChanges();
            }

            return Json("", JsonRequestBehavior.AllowGet);
        }

        

    }
}