using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLThucDon_MonAN_Nhom5.DAL
{
    //Các truy vấn báo cáo trả về DataTable để gắn thẳng vào DataGridView / RDLC
    public class BaoCaoDAL
    {

            // ---------- Yêu cầu 5: doanh thu từng đầu bếp theo tháng / quý / năm ----------
            // Doanh thu = tổng ThanhTien các món đầu bếp thực hiện (trước VAT và giảm giá). Đầu bếp không có doanh thu vẫn hiện dòng 0.
            private const string DoanhThuSql = @"
            SELECT d.MaDauBep, d.TenDauBep,
                   ISNULL(SUM(ct.SoLuong), 0)  AS SoPhan,
                   ISNULL(SUM(ct.ThanhTien), 0) AS DoanhThu
            FROM DauBep d
            LEFT JOIN (ChiTietThucDon ct
                       JOIN ThucDon td ON td.SoThucDon = ct.SoThucDon
                                      AND YEAR(td.NgayDung) = @Nam
                                      AND (@Thang IS NULL OR MONTH(td.NgayDung) = @Thang)
                                      AND (@Quy   IS NULL OR DATEPART(QUARTER, td.NgayDung) = @Quy))
                   ON ct.MaDauBep = d.MaDauBep
            GROUP BY d.MaDauBep, d.TenDauBep
            ORDER BY DoanhThu DESC, d.TenDauBep";

            public DataTable DoanhThuTheoThang(int nam, int thang) => DoanhThu(nam, thang, null);
            public DataTable DoanhThuTheoQuy(int nam, int quy) => DoanhThu(nam, null, quy);
            public DataTable DoanhThuTheoNam(int nam) => DoanhThu(nam, null, null);

            private static DataTable DoanhThu(int nam, int? thang, int? quy) =>
                DbHelper.ExecuteDataTable(DoanhThuSql,
                    DbHelper.P("@Nam", nam), DbHelper.P("@Thang", thang), DbHelper.P("@Quy", quy));

            // ---------- Yêu cầu 6: thông tin đầy đủ các món ăn (kèm nguyên liệu) ----------
            public DataTable MonAnDayDu() => DbHelper.ExecuteDataTable(@"
            SELECT m.MaMonAn, m.TenMonAn, l.TenLoai, cd.TenCongDung, m.DonGia, m.CachLam, m.YeuCau,
                   (SELECT STRING_AGG(nl.TenNguyenLieu + N' (' + CAST(CAST(x.SoLuong AS FLOAT) AS NVARCHAR(20)) + N' ' + dvt.TenDonViTinh + N')', N', ')
                    FROM NguyenLieu_MonAn x
                    JOIN NguyenLieu nl ON nl.MaNguyenLieu = x.MaNguyenLieu
                    JOIN DonViTinh dvt ON dvt.MaDonViTinh = nl.MaDonViTinh
                    WHERE x.MaMonAn = m.MaMonAn) AS NguyenLieu
            FROM MonAn m
            JOIN LoaiMon l ON l.MaLoai = m.MaLoai
            LEFT JOIN CongDung cd ON cd.MaCongDung = m.MaCongDung
            ORDER BY m.MaMonAn");

            // ---------- Yêu cầu 7: danh sách món ăn của từng đầu bếp ----------
            public DataTable MonAnTheoDauBep(string maDauBep = null) => DbHelper.ExecuteDataTable(@"
            SELECT d.MaDauBep, d.TenDauBep, m.MaMonAn, m.TenMonAn, l.TenLoai, m.DonGia, xl.TenXepLoai
            FROM DauBep d
            JOIN DauBep_MonAn x ON x.MaDauBep = d.MaDauBep
            JOIN MonAn m ON m.MaMonAn = x.MaMonAn
            JOIN LoaiMon l ON l.MaLoai = m.MaLoai
            JOIN XepLoai xl ON xl.MaXepLoai = x.MaXepLoai
            WHERE (@Bep IS NULL OR d.MaDauBep = @Bep)
            ORDER BY d.MaDauBep, m.MaMonAn",
                DbHelper.P("@Bep", DbHelper.NullIfEmpty(maDauBep)));

            // ---------- Yêu cầu 8: thông tin đầy đủ các thực đơn (null = tất cả) ----------
            public DataTable ThucDonDayDu(string soThucDon = null) => DbHelper.ExecuteDataTable(@"
            SELECT td.SoThucDon, td.NgayDung, kh.MaKhachHang, kh.TenKhachHang, kh.DiaChi, kh.DienThoai, kh.MaSoThue,
                   m.MaMonAn, m.TenMonAn, d.TenDauBep, ct.SoLuong, m.DonGia, ct.ThanhTien,
                   td.ThueVAT, td.GiamGia, td.TongTien
            FROM ThucDon td
            JOIN KhachHang kh ON kh.MaKhachHang = td.MaKhachHang
            JOIN ChiTietThucDon ct ON ct.SoThucDon = td.SoThucDon
            JOIN MonAn m ON m.MaMonAn = ct.MaMonAn
            JOIN DauBep d ON d.MaDauBep = ct.MaDauBep
            WHERE (@So IS NULL OR td.SoThucDon = @So)
            ORDER BY td.SoThucDon, m.MaMonAn",
                DbHelper.P("@So", DbHelper.NullIfEmpty(soThucDon)));
        }
    
}
