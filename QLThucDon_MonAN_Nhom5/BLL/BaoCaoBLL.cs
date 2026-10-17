using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLThucDon_MonAN_Nhom5.DAL;
using QLThucDon_MonAN_Nhom5.DTO;

namespace QLThucDon_MonAN_Nhom5.BLL
{
    public enum KieuThongKe { Thang , Quy , Nam}
    
    public class BaoCaoBLL
    {
        private readonly BaoCaoDAL _dal = new BaoCaoDAL();

        public DataTable DoanhThuDauBep(KieuThongKe kieu ,int nam , int ky = 0)// ky co the la thang co the la quy
        {
            if( nam < 2000 ||nam > DateTime.Now.Year )
            {
                throw new BusinessException("Năm không hợp lệ!!!!");
            }
            switch (kieu)
            {
                case KieuThongKe.Thang:
                    if (ky < 1 || ky > 12) throw new BusinessException("Tháng phải từ 1 đến 12.");
                    return DbGuard.Run(() => _dal.DoanhThuTheoThang(nam, ky));
                case KieuThongKe.Quy:
                    if (ky < 1 || ky > 4) throw new BusinessException("Quý phải từ 1 đến 4.");
                    return DbGuard.Run(() => _dal.DoanhThuTheoQuy(nam, ky));
                default:
                    return DbGuard.Run(() => _dal.DoanhThuTheoNam(nam));
            }
        }

        // Tổng cộng cột DoanhThu để in dòng cuối báo cáo.

        public decimal TongDoanhThu(DataTable dt)
        {
            //DataTable chứa các dòng dữ liệu (DataRow). Phương thức AsEnumerable() cho phép LINQ duyệt các dòng đó.
            // r laf tung dong trong bang 
            return dt.AsEnumerable().Sum(r => Convert.ToDecimal(r["DoanhThu"]));
        }

        public string TieuDe(KieuThongKe kieu, int nam, int ky = 0) => kieu switch
        {
            KieuThongKe.Thang => $"BÁO CÁO DOANH THU ĐẦU BẾP - THÁNG {ky}/{nam}",
            KieuThongKe.Quy => $"BÁO CÁO DOANH THU ĐẦU BẾP - QUÝ {ky}/{nam}",
            _ => $"BÁO CÁO DOANH THU ĐẦU BẾP - NĂM {nam}"
        };
            
        public DataTable MonAnDayDu() => DbGuard.Run(() => _dal.MonAnDayDu());
        public DataTable MonAnTheoDauBep(string maDauBep = null) => DbGuard.Run(() => _dal.MonAnTheoDauBep(maDauBep));
        public DataTable ThucDonDayDu(string soThucDon = null) => DbGuard.Run(() => _dal.ThucDonDayDu(soThucDon));
    }
}
