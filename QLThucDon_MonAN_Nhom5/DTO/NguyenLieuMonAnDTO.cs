using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLThucDon_MonAN_Nhom5.DTO
{
    public class NguyenLieuMonAnDTO
    {
        public string MaMonAn { get; set; }
        public string MaNguyenLieu { get; set; }
        public string TenNguyenLieu { get; set; }
        public string TenDonViTinh { get; set; }
        public decimal SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien => SoLuong * DonGia;
    }
}
