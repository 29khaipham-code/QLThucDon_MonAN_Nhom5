
using System.Collections.Generic;
using QLThucDon_MonAN_Nhom5.DTO;


namespace QLThucDon_MonAN_Nhom5.Interfaces
{
    public interface IDanhMucDAL
    {
        List<DanhMucDTO> GetAll(LoaiDanhMuc loai);

        string NextCode(LoaiDanhMuc loai);

        int Insert(LoaiDanhMuc loai, DanhMucDTO d);

        int Update(LoaiDanhMuc loai, DanhMucDTO d);

        int Delete(LoaiDanhMuc loai, string ma);
    }
}
