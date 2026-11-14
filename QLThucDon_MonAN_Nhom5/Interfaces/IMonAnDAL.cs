
using System.Collections.Generic;
using System.Data;
using QLThucDon_MonAN_Nhom5.DTO;

namespace QLThucDon_MonAN_Nhom5.Interfaces
{
    public interface IMonAnDAL
    {
        List<MonAnDTO> GetAll();

        MonAnDTO GetById(string ma);

        List<NguyenLieuMonAnDTO> GetNguyenLieu(string maMonAn);

        string NextCode();

        List<MonAnDTO> TimKiem(
            string maLoai,
            string maCongDung,
            string tenMon);

        List<MonAnDTO> GetByLoaiVaDauBep(
            string maLoai,
            string maDauBep);

        void Insert(
            MonAnDTO m,
            List<NguyenLieuMonAnDTO> nguyenLieu);

        void Update(
            MonAnDTO m,
            List<NguyenLieuMonAnDTO> nguyenLieu);

        int Delete(string ma);

        DataTable GetLoaiMon();

        DataTable GetCongDung();
    }
}
