
using System.Collections.Generic;
using System.Data;
using QLThucDon_MonAN_Nhom5.DTO;

namespace QLThucDon_MonAN_Nhom5.Interfaces
{
    public interface IThucDonDAL
    {
        DataTable GetLoaiMon();

        List<ThucDonDTO> GetAll();

        ThucDonDTO GetById(string so);

        List<ChiTietThucDonDTO> GetChiTiet(string so);

        string NextCode();

        void Insert(ThucDonDTO td, List<ChiTietThucDonDTO> chiTiet);

        void Update(ThucDonDTO td, List<ChiTietThucDonDTO> chiTiet);

        int Delete(string so);
    }
}
