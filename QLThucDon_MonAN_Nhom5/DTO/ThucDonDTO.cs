using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLThucDon_MonAN_Nhom5.DTO
{
    public class ThucDonDTO
    {
        public string SoThucDon { get; set; }
        public string MaKhachHang { get; set; }
        public string TenKhachHang { get; set; }
        public string MaSoThue { get; set; }
        public DateTime NgayDung { get; set; }
        public decimal ThueVAT { get; set; }
        public decimal GiamGia { get; set; }
        public decimal TongTien { get; set; }

    }
}
