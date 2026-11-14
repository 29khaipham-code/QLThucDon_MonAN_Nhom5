
using System.Data;

namespace QLThucDon_MonAN_Nhom5.Interfaces
{
    public interface IBaoCaoDAL
    {
        DataTable DoanhThuTheoThang(int nam, int thang);

        DataTable DoanhThuTheoQuy(int nam, int quy);

        DataTable DoanhThuTheoNam(int nam);

        DataTable MonAnDayDu();

        DataTable MonAnTheoDauBep(string maDauBep = null);

        DataTable ThucDonDayDu(string soThucDon = null);
    }
}
