using QLThucDon_MonAN_Nhom5.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace QLThucDon_MonAN_Nhom5.DAL
{
    public class ThucDonDAL
    {
        private const string SelectBase = @"
            SELECT td.SoThucDon, td.MaKhachHang, kh.TenKhachHang, kh.MaSoThue,
                   td.NgayDung, td.ThueVAT, td.GiamGia, td.TongTien
            FROM ThucDon td JOIN KhachHang kh ON kh.MaKhachHang = td.MaKhachHang";

        private static ThucDonDTO Map(SqlDataReader r) => new ThucDonDTO
        {
            SoThucDon = r.Str("SoThucDon"),
            MaKhachHang = r.Str("MaKhachHang"),
            TenKhachHang = r.Str("TenKhachHang"),
            MaSoThue = r.Str("MaSoThue"),
            NgayDung = r.Date("NgayDung"),
            ThueVAT = r.Dec("ThueVAT"),
            GiamGia = r.Dec("GiamGia"),
            TongTien = r.Dec("TongTien")
        };

        public List<ThucDonDTO> GetAll() =>
            DbHelper.ExecuteList<ThucDonDTO>(SelectBase + " ORDER BY td.NgayDung DESC, td.SoThucDon DESC", Map);

        public ThucDonDTO GetById(string so) =>
            DbHelper.ExecuteList<ThucDonDTO>(SelectBase + " WHERE td.SoThucDon=@So", Map, DbHelper.P("@So", so)).FirstOrDefault();

        /// <summary>Chi tiết một thực đơn (gọi riêng, không nhúng trong ThucDonDTO).</summary>
        public List<ChiTietThucDonDTO> GetChiTiet(string so) =>
            DbHelper.ExecuteList<ChiTietThucDonDTO>(@"
                SELECT ct.SoThucDon, ct.MaMonAn, m.TenMonAn, ct.MaDauBep, d.TenDauBep,
                       ct.SoLuong, m.DonGia, ct.ThanhTien
                FROM ChiTietThucDon ct
                JOIN MonAn m ON m.MaMonAn = ct.MaMonAn
                JOIN DauBep d ON d.MaDauBep = ct.MaDauBep
                WHERE ct.SoThucDon = @So ORDER BY ct.MaMonAn",
                r => new ChiTietThucDonDTO
                {
                    SoThucDon = r.Str("SoThucDon"),
                    MaMonAn = r.Str("MaMonAn"),
                    TenMonAn = r.Str("TenMonAn"),
                    MaDauBep = r.Str("MaDauBep"),
                    TenDauBep = r.Str("TenDauBep"),
                    SoLuong = r.Int("SoLuong"),
                    DonGia = r.Dec("DonGia"),
                    ThanhTien = r.Dec("ThanhTien")
                }, DbHelper.P("@So", so));

        public string NextCode() => DbHelper.NextCode("ThucDon", "SoThucDon", "TD", 4);

        /// <summary>Lưu thực đơn + chi tiết trong 1 transaction; ThanhTien và TongTien do SQL tính từ MonAn.DonGia.</summary>
        public void Insert(ThucDonDTO td, List<ChiTietThucDonDTO> chiTiet) => DbHelper.RunInTransaction((c, t) =>
        {
            DbHelper.ExecuteNonQuery(c, t,
                @"INSERT INTO ThucDon(SoThucDon,MaKhachHang,NgayDung,ThueVAT,GiamGia) VALUES(@So,@KH,@Ngay,@VAT,@GG)",
                HeaderParams(td));
            SaveChiTiet(c, t, td.SoThucDon, chiTiet);
            TinhLaiTongTien(c, t, td.SoThucDon);
        });

        public void Update(ThucDonDTO td, List<ChiTietThucDonDTO> chiTiet) => DbHelper.RunInTransaction((c, t) =>
        {
            DbHelper.ExecuteNonQuery(c, t,
                @"UPDATE ThucDon SET MaKhachHang=@KH, NgayDung=@Ngay, ThueVAT=@VAT, GiamGia=@GG WHERE SoThucDon=@So",
                HeaderParams(td));
            DbHelper.ExecuteNonQuery(c, t, "DELETE FROM ChiTietThucDon WHERE SoThucDon=@So", DbHelper.P("@So", td.SoThucDon));
            SaveChiTiet(c, t, td.SoThucDon, chiTiet);
            TinhLaiTongTien(c, t, td.SoThucDon);
        });

        /// <summary>Chi tiết bị xoá theo (ON DELETE CASCADE).</summary>
        public int Delete(string so) =>
            DbHelper.ExecuteNonQuery("DELETE FROM ThucDon WHERE SoThucDon=@So", DbHelper.P("@So", so));

        private static SqlParameter[] HeaderParams(ThucDonDTO td) => new[]
        {
            DbHelper.P("@So", td.SoThucDon), DbHelper.P("@KH", td.MaKhachHang), DbHelper.P("@Ngay", td.NgayDung),
            DbHelper.P("@VAT", td.ThueVAT), DbHelper.P("@GG", td.GiamGia)
        };

        private static void SaveChiTiet(SqlConnection c, SqlTransaction t, string so, List<ChiTietThucDonDTO> ds)
        {
            foreach (var ct in ds ?? new List<ChiTietThucDonDTO>())
                DbHelper.ExecuteNonQuery(c, t,
                    @"INSERT INTO ChiTietThucDon(SoThucDon,MaMonAn,MaDauBep,SoLuong,ThanhTien)
                      SELECT @So, MaMonAn, @Bep, @SL, @SL * DonGia FROM MonAn WHERE MaMonAn=@Mon",
                    DbHelper.P("@So", so), DbHelper.P("@Mon", ct.MaMonAn),
                    DbHelper.P("@Bep", ct.MaDauBep), DbHelper.P("@SL", ct.SoLuong));
        }

        //TongTien = SUM(ThanhTien) * (1 + VAT/100) - GiamGia (không âm)
        private static void TinhLaiTongTien(SqlConnection c, SqlTransaction t, string so) =>
            DbHelper.ExecuteNonQuery(c, t, @"
                UPDATE td SET TongTien = CASE WHEN s.Tong * (1 + td.ThueVAT / 100.0) - td.GiamGia < 0 THEN 0
                                              ELSE s.Tong * (1 + td.ThueVAT / 100.0) - td.GiamGia END
                FROM ThucDon td
                CROSS APPLY (SELECT ISNULL(SUM(ThanhTien), 0) AS Tong FROM ChiTietThucDon WHERE SoThucDon = td.SoThucDon) s
                WHERE td.SoThucDon = @So", DbHelper.P("@So", so));
    }
}
