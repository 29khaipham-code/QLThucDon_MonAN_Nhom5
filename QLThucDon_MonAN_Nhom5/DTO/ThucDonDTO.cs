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
        public DateTime NgayDung { get; set; } = DateTime.Now;
        public decimal ThueVAT { get; set; }       // phần trăm: 8 = 8%
        public decimal GiamGia { get; set; }       // số tiền
        public decimal TongTien { get; set; }
        public List<ChiTietThucDonDTO> ChiTiet { get; set; } = new List<ChiTietThucDonDTO>();

        //Tổng trước thuế/giảm giá, dùng để xem trước trên form
        public decimal TongTienHang => ChiTiet.Sum(c => c.ThanhTien);

    }
}
