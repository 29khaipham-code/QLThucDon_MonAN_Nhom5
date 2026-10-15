using QLThucDon_MonAN_Nhom5.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLThucDon_MonAN_Nhom5.DAL
{
    public class NguyenLieuDAL
    {
        private const string SelectBase = @"
            SELECT nl.MaNguyenLieu, nl.TenNguyenLieu, nl.MaDonViTinh, dvt.TenDonViTinh,
                   nl.MaCongDung, cd.TenCongDung, nl.DonGia, nl.DinhDuong
            FROM NguyenLieu nl
            JOIN DonViTinh dvt ON dvt.MaDonViTinh = nl.MaDonViTinh
            LEFT JOIN CongDung cd ON cd.MaCongDung = nl.MaCongDung";

        private static NguyenLieuDTO Map(Microsoft.Data.SqlClient.SqlDataReader r) => new NguyenLieuDTO
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

        public List<NguyenLieuDTO> GetAll() =>
            DbHelper.ExecuteList<NguyenLieuDTO>(SelectBase + " ORDER BY nl.MaNguyenLieu", Map);

        public NguyenLieuDTO GetById(string ma) =>
            DbHelper.ExecuteList<NguyenLieuDTO>(SelectBase + " WHERE nl.MaNguyenLieu=@Ma", Map, DbHelper.P("@Ma", ma)).FirstOrDefault();

        public string NextCode() => DbHelper.NextCode("NguyenLieu", "MaNguyenLieu", "NL", 3);

        public int Insert(NguyenLieuDTO n) => DbHelper.ExecuteNonQuery(
            @"INSERT INTO NguyenLieu(MaNguyenLieu,TenNguyenLieu,MaDonViTinh,MaCongDung,DonGia,DinhDuong)
              VALUES(@Ma,@Ten,@DVT,@CD,@Gia,@DD)",
            DbHelper.P("@Ma", n.MaNguyenLieu), DbHelper.P("@Ten", n.TenNguyenLieu), DbHelper.P("@DVT", n.MaDonViTinh),
            DbHelper.P("@CD", DbHelper.NullIfEmpty(n.MaCongDung)), DbHelper.P("@Gia", n.DonGia), DbHelper.P("@DD", n.DinhDuong));

        /// <summary>Sửa nguyên liệu và TÍNH LẠI đơn giá các món ăn đang dùng nguyên liệu này (trigger chỉ chạy khi đổi NguyenLieu_MonAn).
        /// Lưu ý: ThanhTien của các thực đơn đã lập được giữ nguyên (lưu lịch sử).</summary>
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
            DbHelper.ExecuteNonQuery("DELETE FROM NguyenLieu WHERE MaNguyenLieu=@Ma", DbHelper.P("@Ma", ma));
    }
}
