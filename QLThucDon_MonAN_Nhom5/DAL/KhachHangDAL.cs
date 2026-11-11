using Microsoft.Data.SqlClient;
using QLThucDon_MonAN_Nhom5.DTO;
using System.Collections.Generic;

namespace QLThucDon_MonAN_Nhom5.DAL
{
    public class KhachHangDAL : BaseDAL<KhachHangDTO>, IKhachHangDAL
    {
        protected override string TableName => "KhachHang";
        protected override string KeyColumn => "MaKhachHang";
        protected override string CodePrefix => "KH";
        protected override string SelectSql => "SELECT * FROM KhachHang";
        protected override string OrderBySql => "MaKhachHang";

        protected override KhachHangDTO Map(SqlDataReader r) => new KhachHangDTO
        {
            MaKhachHang = r.Str("MaKhachHang"),
            TenKhachHang = r.Str("TenKhachHang"),
            DiaChi = r.Str("DiaChi"),
            DienThoai = r.Str("DienThoai"),
            MaSoThue = r.Str("MaSoThue")
        };

        public List<KhachHangDTO> TimKiem(string ten) =>
            DbHelper.ExecuteList<KhachHangDTO>(
                "SELECT * FROM KhachHang WHERE @Ten IS NULL OR TenKhachHang LIKE N'%' + @Ten + N'%' ORDER BY MaKhachHang",
                Map, DbHelper.P("@Ten", DbHelper.NullIfEmpty(ten)));

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
