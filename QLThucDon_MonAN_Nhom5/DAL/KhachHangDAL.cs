using QLThucDon_MonAN_Nhom5.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLThucDon_MonAN_Nhom5.DAL
{
    public class KhachHangDAL
    {
        private static KhachHangDTO Map(Microsoft.Data.SqlClient.SqlDataReader r) => new KhachHangDTO
        {
            MaKhachHang = r.Str("MaKhachHang"),
            TenKhachHang = r.Str("TenKhachHang"),
            DiaChi = r.Str("DiaChi"),
            DienThoai = r.Str("DienThoai"),
            MaSoThue = r.Str("MaSoThue")
        };

        public List<KhachHangDTO> GetAll() =>
            DbHelper.ExecuteList<KhachHangDTO>("SELECT * FROM KhachHang ORDER BY MaKhachHang", Map);

        public KhachHangDTO GetById(string ma) =>
            DbHelper.ExecuteList<KhachHangDTO>("SELECT * FROM KhachHang WHERE MaKhachHang=@Ma", Map, DbHelper.P("@Ma", ma)).FirstOrDefault();

        public List<KhachHangDTO> TimKiem(string ten) =>
            DbHelper.ExecuteList<KhachHangDTO>(
                "SELECT * FROM KhachHang WHERE @Ten IS NULL OR TenKhachHang LIKE N'%' + @Ten + N'%' ORDER BY MaKhachHang",
                Map, DbHelper.P("@Ten", DbHelper.NullIfEmpty(ten)));

        public string NextCode() => DbHelper.NextCode("KhachHang", "MaKhachHang", "KH", 3);

        public int Insert(KhachHangDTO k) => DbHelper.ExecuteNonQuery(
            "INSERT INTO KhachHang(MaKhachHang,TenKhachHang,DiaChi,DienThoai,MaSoThue) VALUES(@Ma,@Ten,@DC,@DT,@MST)",
            DbHelper.P("@Ma", k.MaKhachHang), DbHelper.P("@Ten", k.TenKhachHang), DbHelper.P("@DC", k.DiaChi),
            DbHelper.P("@DT", k.DienThoai), DbHelper.P("@MST", k.MaSoThue));

        public int Update(KhachHangDTO k) => DbHelper.ExecuteNonQuery(
            "UPDATE KhachHang SET TenKhachHang=@Ten, DiaChi=@DC, DienThoai=@DT, MaSoThue=@MST WHERE MaKhachHang=@Ma",
            DbHelper.P("@Ma", k.MaKhachHang), DbHelper.P("@Ten", k.TenKhachHang), DbHelper.P("@DC", k.DiaChi),
            DbHelper.P("@DT", k.DienThoai), DbHelper.P("@MST", k.MaSoThue));

        public int Delete(string ma) =>
            DbHelper.ExecuteNonQuery("DELETE FROM KhachHang WHERE MaKhachHang=@Ma", DbHelper.P("@Ma", ma));
    }
}
