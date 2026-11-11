using QLThucDon_MonAN_Nhom5.DTO;
using System.Collections.Generic;
using System.Data;

namespace QLThucDon_MonAN_Nhom5.DAL
{
    // Interface 4
    public interface IKhachHangDAL : IRepository<KhachHangDTO>
    {
        List<KhachHangDTO> TimKiem(string ten);
    }

    // Interface 5
    public interface INguyenLieuDAL : IRepository<NguyenLieuDTO>
    {
    }

    // Interface 6
    public interface IMonAnDAL : IMasterDetailRepository<MonAnDTO, NguyenLieuMonAnDTO>
    {
        List<NguyenLieuMonAnDTO> GetNguyenLieu(string maMonAn);
        List<MonAnDTO> TimKiem(string maLoai, string maCongDung, string tenMon);
        List<MonAnDTO> GetByLoaiVaDauBep(string maLoai, string maDauBep);
        DataTable GetLoaiMon();
        DataTable GetCongDung();
    }

    // Interface 7
    public interface IDauBepDAL : IMasterDetailRepository<DauBepDTO, DauBepMonAnDTO>
    {
        List<DauBepMonAnDTO> GetMonAn(string maDauBep);
        List<DauBepDTO> TimKiem(string ten, string maNoiHoc, string maMonAn);
    }

    // Interface 8
    public interface IThucDonDAL : IMasterDetailRepository<ThucDonDTO, ChiTietThucDonDTO>
    {
        List<ChiTietThucDonDTO> GetChiTiet(string soThucDon);
    }

    // Interface 9
    public interface IDanhMucDAL
    {
        List<DanhMucDTO> GetAll(LoaiDanhMuc loai);
        string NextCode(LoaiDanhMuc loai);
        int Insert(LoaiDanhMuc loai, DanhMucDTO d);
        int Update(LoaiDanhMuc loai, DanhMucDTO d);
        int Delete(LoaiDanhMuc loai, string ma);
    }

    // Interface 10
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
