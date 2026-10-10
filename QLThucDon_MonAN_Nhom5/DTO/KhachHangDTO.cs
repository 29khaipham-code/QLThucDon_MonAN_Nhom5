using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLThucDon_MonAN_Nhom5.DTO
{
    public class KhachHangDTO
    {
        public string MaKhachHang { get; set; }
        public string TenKhachHang { get; set; }
        public string DiaChi { get; set; }
        public string DienThoai { get; set; }
        public string MaSoThue { get; set; }
        public override string ToString() => TenKhachHang;
    }
}
