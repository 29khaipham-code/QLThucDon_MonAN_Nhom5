using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLThucDon_MonAN_Nhom5.DTO
{
    public class NguyenLieuDTO
    {
        public string MaNguyenLieu { get; set; }
        public string TenNguyenLieu { get; set; }
        public string MaDonViTinh { get; set; }
        public string TenDonViTinh { get; set; }   // hiển thị (JOIN)
        public string MaCongDung { get; set; }
        public string TenCongDung { get; set; }    // hiển thị (JOIN)
        public decimal DonGia { get; set; }
        public string DinhDuong { get; set; }
    }
}
