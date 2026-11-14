
using System.Collections.Generic;
using QLThucDon_MonAN_Nhom5.DTO;

namespace QLThucDon_MonAN_Nhom5.Interfaces
{
    public interface INguyenLieuDAL
    {
        List<NguyenLieuDTO> GetAll();

        NguyenLieuDTO GetById(string ma);

        string NextCode();

        int Insert(NguyenLieuDTO n);

        int Update(NguyenLieuDTO n);

        int Delete(string ma);
    }
}
