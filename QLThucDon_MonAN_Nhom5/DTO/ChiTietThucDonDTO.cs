using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLThucDon_MonAN_Nhom5.DTO
{
    public class ChiTietThucDonDTO
    {
        public string SoThucDon { get; set; }
        public string MaMonAn { get; set; }
        public string TenMonAn { get; set; }
        public string MaDauBep { get; set; }
        public string TenDauBep { get; set; }
        public int SoLuong { get; set; }    
        public decimal DonGia { get; set; }        // đơn giá món hiện tại (chỉ để hiển thị)
        public decimal ThanhTien { get; set; }
    }
}
