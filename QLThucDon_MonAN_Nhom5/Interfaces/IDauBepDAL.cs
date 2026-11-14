
using System.Collections.Generic;
using System.Data;
using QLThucDon_MonAN_Nhom5.DTO;

namespace QLThucDon_MonAN_Nhom5.Interfaces
{
    public interface IDauBepDAL
    {
        List<DauBepDTO> GetAll();

        DauBepDTO GetById(string ma);

        List<DauBepMonAnDTO> GetMonAn(string maDauBep);

        string NextCode();

        List<DauBepDTO> TimKiem(
            string ten,
            string maNoiHoc,
            string maMonAn);

        DataTable GetTrinhDo();

        DataTable GetNoiHoc();

        DataTable GetDanhSachMonAn();

        DataTable GetXepLoai();

        void Insert(
            DauBepDTO d,
            List<DauBepMonAnDTO> monAn);

        void Update(
            DauBepDTO d,
            List<DauBepMonAnDTO> monAn);

        int Delete(string ma);
    }
}
