using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoanNgocThuan103.Models
{
    // ViewModel item Giỏ Hàng 
    // Đại diện cho đối tượng giỏ hàng để lưu các thông tin đặt hàng vào cơ sở dữ liệu
    public class ItemGioHang
    {
        public int MaSP { get; set; }
        public string TenSP { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien { get; set; }
        public string HinhAnh { get; set; }
        public ItemGioHang()
        {

        }
        public ItemGioHang(int iMaSP)
        {
            using (QuanLyBanHang1 db = new QuanLyBanHang1())
            {
                this.MaSP = iMaSP;
                SanPham sp = db.SanPhams.Single(n => n.MaSP == iMaSP); // Lấy ra sản phẩm theo tên sản phẩm
                this.TenSP = sp.TenSP;
                this.HinhAnh = sp.HinhAnh;
                this.DonGia = sp.DonGia.Value;
                this.SoLuong = 1;
                this.ThanhTien = DonGia * SoLuong;
            }
        }
        public ItemGioHang(int iMaSP, int sl)
        {
            using (QuanLyBanHang1 db = new QuanLyBanHang1())
            {
                this.MaSP = iMaSP;
                SanPham sp = db.SanPhams.Single(n => n.MaSP == iMaSP); // Lấy ra sản phẩm theo tên sản phẩm
                this.TenSP = sp.TenSP;
                this.HinhAnh = sp.HinhAnh;
                this.DonGia = sp.DonGia.Value;
                this.SoLuong = sl;
                this.ThanhTien = DonGia * SoLuong;
            }
        }
    }
}