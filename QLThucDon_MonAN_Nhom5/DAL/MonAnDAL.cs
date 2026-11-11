using Microsoft.Data.SqlClient;
using QLThucDon_MonAN_Nhom5.DTO;
using System.Collections.Generic;
using System.Data;

namespace QLThucDon_MonAN_Nhom5.DAL
{
    public class MonAnDAL : BaseDAL<MonAnDTO>, IMonAnDAL
    {
        private const string SelectBase = @"
            SELECT m.MaMonAn, m.TenMonAn, m.MaLoai, l.TenLoai, m.MaCongDung, cd.TenCongDung,
                   m.DonGia, m.CachLam, m.YeuCau
            FROM MonAn m
            JOIN LoaiMon l ON l.MaLoai = m.MaLoai
            LEFT JOIN CongDung cd ON cd.MaCongDung = m.MaCongDung";

        protected override string TableName => "MonAn";
        protected override string KeyColumn => "MaMonAn";
        protected override string KeyColumnInSelect => "m.MaMonAn";
        protected override string CodePrefix => "MA";
        protected override string SelectSql => SelectBase;
        protected override string OrderBySql => "m.MaMonAn";

        protected override MonAnDTO Map(SqlDataReader r) => new MonAnDTO
        {
            MaMonAn = r.Str("MaMonAn"),
            TenMonAn = r.Str("TenMonAn"),
            MaLoai = r.Str("MaLoai"),
            TenLoai = r.Str("TenLoai"),
            MaCongDung = r.Str("MaCongDung"),
            TenCongDung = r.Str("TenCongDung"),
            DonGia = r.Dec("DonGia"),
            CachLam = r.Str("CachLam"),
            YeuCau = r.Str("YeuCau")
        };

        public List<NguyenLieuMonAnDTO> GetNguyenLieu(string maMonAn) =>
            DbHelper.ExecuteList<NguyenLieuMonAnDTO>(@"
                SELECT x.MaMonAn, x.MaNguyenLieu, nl.TenNguyenLieu, dvt.TenDonViTinh,
                       x.SoLuong, nl.DonGia, x.SoLuong * nl.DonGia AS ThanhTien
                FROM NguyenLieu_MonAn x
                JOIN NguyenLieu nl ON nl.MaNguyenLieu = x.MaNguyenLieu
                JOIN DonViTinh dvt ON dvt.MaDonViTinh = nl.MaDonViTinh
                WHERE x.MaMonAn = @Ma ORDER BY x.MaNguyenLieu",
                r => new NguyenLieuMonAnDTO
                {
                    MaMonAn = r.Str("MaMonAn"),
                    MaNguyenLieu = r.Str("MaNguyenLieu"),
                    TenNguyenLieu = r.Str("TenNguyenLieu"),
                    TenDonViTinh = r.Str("TenDonViTinh"),
                    SoLuong = r.Dec("SoLuong"),
                    DonGia = r.Dec("DonGia"),
                    ThanhTien = r.Dec("ThanhTien")
                }, DbHelper.P("@Ma", maMonAn));

        public List<MonAnDTO> TimKiem(string maLoai, string maCongDung, string tenMon) =>
            DbHelper.ExecuteList<MonAnDTO>(SelectBase + @"
                WHERE (@Loai IS NULL OR m.MaLoai = @Loai)
                  AND (@CD   IS NULL OR m.MaCongDung = @CD)
                  AND (@Ten  IS NULL OR m.TenMonAn LIKE N'%' + @Ten + N'%')
                ORDER BY m.MaMonAn", Map,
                DbHelper.P("@Loai", DbHelper.NullIfEmpty(maLoai)),
                DbHelper.P("@CD", DbHelper.NullIfEmpty(maCongDung)),
                DbHelper.P("@Ten", DbHelper.NullIfEmpty(tenMon)));

        public List<MonAnDTO> GetByLoaiVaDauBep(string maLoai, string maDauBep) =>
            DbHelper.ExecuteList<MonAnDTO>(SelectBase + @"
                JOIN DauBep_MonAn db ON db.MaMonAn = m.MaMonAn
                WHERE m.MaLoai = @Loai AND db.MaDauBep = @Bep
                ORDER BY m.TenMonAn", Map,
                DbHelper.P("@Loai", maLoai), DbHelper.P("@Bep", maDauBep));

        // DonGia do trigger trg_TinhDonGiaMonAn tự tính
        public void Insert(MonAnDTO m, List<NguyenLieuMonAnDTO> nguyenLieu) => DbHelper.RunInTransaction((c, t) =>
        {
            DbHelper.ExecuteNonQuery(c, t,
                @"INSERT INTO MonAn(MaMonAn,TenMonAn,MaLoai,MaCongDung,CachLam,YeuCau) VALUES(@Ma,@Ten,@Loai,@CD,@CL,@YC)",
                MonAnParams(m));
            SaveNguyenLieu(c, t, m.MaMonAn, nguyenLieu);
        });

        public void Update(MonAnDTO m, List<NguyenLieuMonAnDTO> nguyenLieu) => DbHelper.RunInTransaction((c, t) =>
        {
            DbHelper.ExecuteNonQuery(c, t,
                @"UPDATE MonAn SET TenMonAn=@Ten, MaLoai=@Loai, MaCongDung=@CD, CachLam=@CL, YeuCau=@YC WHERE MaMonAn=@Ma",
                MonAnParams(m));
            DbHelper.ExecuteNonQuery(c, t, "DELETE FROM NguyenLieu_MonAn WHERE MaMonAn=@Ma", DbHelper.P("@Ma", m.MaMonAn));
            SaveNguyenLieu(c, t, m.MaMonAn, nguyenLieu);
        });

        // SqlException.Number = 547 nếu món đang có đầu bếp làm được hoặc nằm trong thực đơn.
        public int Delete(string ma) =>
            DbHelper.ExecuteNonQuery("DELETE FROM MonAn WHERE MaMonAn=@Ma", DbHelper.P("@Ma", ma));

        private static SqlParameter[] MonAnParams(MonAnDTO m) => new[]
        {
            DbHelper.P("@Ma", m.MaMonAn), DbHelper.P("@Ten", m.TenMonAn), DbHelper.P("@Loai", m.MaLoai),
            DbHelper.P("@CD", DbHelper.NullIfEmpty(m.MaCongDung)), DbHelper.P("@CL", m.CachLam), DbHelper.P("@YC", m.YeuCau)
        };

        private static void SaveNguyenLieu(SqlConnection c, SqlTransaction t, string maMonAn, List<NguyenLieuMonAnDTO> ds)
        {
            foreach (var nl in ds ?? new List<NguyenLieuMonAnDTO>())
                DbHelper.ExecuteNonQuery(c, t,
                    "INSERT INTO NguyenLieu_MonAn(MaMonAn,MaNguyenLieu,SoLuong) VALUES(@MA,@NL,@SL)",
                    DbHelper.P("@MA", maMonAn), DbHelper.P("@NL", nl.MaNguyenLieu), DbHelper.P("@SL", nl.SoLuong));
        }

        public DataTable GetLoaiMon() =>
            DbHelper.ExecuteDataTable("SELECT MaLoai, TenLoai FROM LoaiMon ORDER BY TenLoai");

        public DataTable GetCongDung() =>
            DbHelper.ExecuteDataTable("SELECT MaCongDung, TenCongDung FROM CongDung ORDER BY TenCongDung");

        
    }
}


