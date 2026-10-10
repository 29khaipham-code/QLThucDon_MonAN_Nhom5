using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLThucDon_MonAN_Nhom5.DTO
{
    //Dùng chung cho 6 bảng danh mục: LoaiMon, CongDung, DonViTinh, NoiHoc, TrinhDo, XepLoai (Ma + Ten).
    public class DanhMucDTO
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
        public override string ToString() => Ten;
    }
}
