using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLThucDon_MonAN_Nhom5.DTO
{
    public class MonAnDTO
    {
        public string MaMonAn { get; set; }
        public string TenMonAn { get; set; }
        public string MaLoai { get; set; }
        public string TenLoai { get; set; }
        public string MaCongDung { get; set; }
        public string TenCongDung { get; set; }
        public decimal DonGia { get; set; }        // tự tính từ nguyên liệu (trigger trong DB)
        public string CachLam { get; set; }
        public string YeuCau { get; set; }
        public List<NguyenLieuMonAnDTO> NguyenLieu { get; set; } = new List<NguyenLieuMonAnDTO>();
        public override string ToString() => TenMonAn;
    }
}
