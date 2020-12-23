using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoanNgocThuan103.Models
{
    public class LogDAO
    {
        public int Log_ID { get; set; }
        public string Log1 { get; set; }
        public Nullable<System.DateTime> NgayTao { get; set; }
        public Nullable<int> MaTV { get; set; }
        public string TaiKhoan { get; set; }
        private QuanLyBanHang1 db = new QuanLyBanHang1();

        public void addLog(Log entity)
        {
            db.Logs.Add(entity);
            db.SaveChanges();

        }
    }
}