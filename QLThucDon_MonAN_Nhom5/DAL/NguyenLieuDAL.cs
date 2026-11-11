using Microsoft.Data.SqlClient;
using QLThucDon_MonAN_Nhom5.DTO;

namespace QLThucDon_MonAN_Nhom5.DAL
{
    public class NguyenLieuDAL : BaseDAL<NguyenLieuDTO>, INguyenLieuDAL
    {
        private const string SelectBase = @"
            SELECT nl.MaNguyenLieu, nl.TenNguyenLieu, nl.MaDonViTinh, dvt.TenDonViTinh,
                   nl.MaCongDung, cd.TenCongDung, nl.DonGia, nl.DinhDuong
            FROM NguyenLieu nl
            JOIN DonViTinh dvt ON dvt.MaDonViTinh = nl.MaDonViTinh
            LEFT JOIN CongDung cd ON cd.MaCongDung = nl.MaCongDung";

        protected override string TableName => "NguyenLieu";
        protected override string KeyColumn => "MaNguyenLieu";
        protected override string KeyColumnInSelect => "nl.MaNguyenLieu";
        protected override string CodePrefix => "NL";
        protected override string SelectSql => SelectBase;
        protected override string OrderBySql => "nl.MaNguyenLieu";

        protected override NguyenLieuDTO Map(SqlDataReader r) => new NguyenLieuDTO
        {
            MaNguyenLieu = r.Str("MaNguyenLieu"),
            TenNguyenLieu = r.Str("TenNguyenLieu"),
            MaDonViTinh = r.Str("MaDonViTinh"),
            TenDonViTinh = r.Str("TenDonViTinh"),
            MaCongDung = r.Str("MaCongDung"),
            TenCongDung = r.Str("TenCongDung"),
            DonGia = r.Dec("DonGia"),
            DinhDuong = r.Str("DinhDuong")
        };

        public int Insert(NguyenLieuDTO n) => DbHelper.ExecuteNonQuery(
            @"INSERT INTO NguyenLieu(MaNguyenLieu,TenNguyenLieu,MaDonViTinh,MaCongDung,DonGia,DinhDuong)
              VALUES(@Ma,@Ten,@DVT,@CD,@Gia,@DD)",
            DbHelper.P("@Ma", n.MaNguyenLieu), DbHelper.P("@Ten", n.TenNguyenLieu), DbHelper.P("@DVT", n.MaDonViTinh),
            DbHelper.P("@CD", DbHelper.NullIfEmpty(n.MaCongDung)), DbHelper.P("@Gia", n.DonGia), DbHelper.P("@DD", n.DinhDuong));

        /// <summary>Sửa nguyên liệu và TÍNH LẠI đơn giá các món ăn đang dùng nguyên liệu này.
        /// ThanhTien của các thực đơn đã lập được giữ nguyên (lưu lịch sử).</summary>
        public int Update(NguyenLieuDTO n) => DbHelper.ExecuteNonQuery(
            @"UPDATE NguyenLieu SET TenNguyenLieu=@Ten, MaDonViTinh=@DVT, MaCongDung=@CD, DonGia=@Gia, DinhDuong=@DD
              WHERE MaNguyenLieu=@Ma;
              UPDATE m SET m.DonGia = ISNULL((SELECT SUM(nl.DonGia * x.SoLuong)
                                              FROM NguyenLieu_MonAn x JOIN NguyenLieu nl ON nl.MaNguyenLieu = x.MaNguyenLieu
                                              WHERE x.MaMonAn = m.MaMonAn), 0)
              FROM MonAn m
              WHERE m.MaMonAn IN (SELECT MaMonAn FROM NguyenLieu_MonAn WHERE MaNguyenLieu=@Ma);",
            DbHelper.P("@Ma", n.MaNguyenLieu), DbHelper.P("@Ten", n.TenNguyenLieu), DbHelper.P("@DVT", n.MaDonViTinh),
            DbHelper.P("@CD", DbHelper.NullIfEmpty(n.MaCongDung)), DbHelper.P("@Gia", n.DonGia), DbHelper.P("@DD", n.DinhDuong));

        public int Delete(string ma) =>
            DbHelper.ExecuteNonQuery(
                @"IF EXISTS (SELECT 1 FROM NguyenLieu_MonAn WHERE MaNguyenLieu = @Ma)
                  BEGIN
                      THROW 50001, N'Nguyên liệu đang được sử dụng trong món ăn, không thể xóa!', 1;
                  END
                  DELETE FROM NguyenLieu WHERE MaNguyenLieu = @Ma;",
                DbHelper.P("@Ma", ma));
    }
}
