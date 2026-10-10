using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLThucDon_MonAN_Nhom5.DTO
{
    public class DauBepDTO
    {
        public string MaDauBep { get; set; }
        public string TenDauBep { get; set; }
        public string GioiTinh { get; set; }      // "Nam" | "Nữ"
        public string DiaChi { get; set; }
        public string DienThoai { get; set; }
        public string MaTrinhDo { get; set; }
        public string TenTrinhDo { get; set; }
        public string MaNoiHoc { get; set; }
        public string TenNoiHoc { get; set; }
        public List<DauBepMonAnDTO> MonAn { get; set; } = new List<DauBepMonAnDTO>();
        public override string ToString() => TenDauBep;
    }
}
