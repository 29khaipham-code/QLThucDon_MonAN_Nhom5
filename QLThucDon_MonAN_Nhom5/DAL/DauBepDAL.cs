using QLThucDon_MonAN_Nhom5.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace QLThucDon_MonAN_Nhom5.DAL
{
    public class DauBepDAL
    {
        private const string SelectBase = @"
            SELECT d.MaDauBep, d.TenDauBep, d.GioiTinh, d.DiaChi, d.DienThoai,
                   d.MaTrinhDo, td.TenTrinhDo, d.MaNoiHoc, nh.TenNoiHoc
            FROM DauBep d
            JOIN TrinhDo td ON td.MaTrinhDo = d.MaTrinhDo
            LEFT JOIN NoiHoc nh ON nh.MaNoiHoc = d.MaNoiHoc";

        private static DauBepDTO Map(SqlDataReader r) => new DauBepDTO
        {
            MaDauBep = r.Str("MaDauBep"),
            TenDauBep = r.Str("TenDauBep"),
            GioiTinh = r.Str("GioiTinh"),
            DiaChi = r.Str("DiaChi"),
            DienThoai = r.Str("DienThoai"),
            MaTrinhDo = r.Str("MaTrinhDo"),
            TenTrinhDo = r.Str("TenTrinhDo"),
            MaNoiHoc = r.Str("MaNoiHoc"),
            TenNoiHoc = r.Str("TenNoiHoc")
        };

        public List<DauBepDTO> GetAll() =>
            DbHelper.ExecuteList<DauBepDTO>(SelectBase + " ORDER BY d.MaDauBep", Map);

        public DauBepDTO GetById(string ma) =>
            DbHelper.ExecuteList<DauBepDTO>(SelectBase + " WHERE d.MaDauBep=@Ma", Map, DbHelper.P("@Ma", ma)).FirstOrDefault();

        //Các món một đầu bếp làm được (gọi riêng, không nhúng trong DauBepDTO)
        public List<DauBepMonAnDTO> GetMonAn(string maDauBep) =>
            DbHelper.ExecuteList<DauBepMonAnDTO>(@"
                SELECT x.MaDauBep, x.MaMonAn, m.TenMonAn, x.MaXepLoai, xl.TenXepLoai
                FROM DauBep_MonAn x
                JOIN MonAn m ON m.MaMonAn = x.MaMonAn
                JOIN XepLoai xl ON xl.MaXepLoai = x.MaXepLoai
                WHERE x.MaDauBep = @Ma ORDER BY x.MaMonAn",
                r => new DauBepMonAnDTO
                {
                    MaDauBep = r.Str("MaDauBep"),
                    MaMonAn = r.Str("MaMonAn"),
                    TenMonAn = r.Str("TenMonAn"),
                    MaXepLoai = r.Str("MaXepLoai"),
                    TenXepLoai = r.Str("TenXepLoai")
                }, DbHelper.P("@Ma", maDauBep));

        public string NextCode() => DbHelper.NextCode("DauBep", "MaDauBep", "DB", 3);

        // ---------- Yêu cầu 4: tìm đầu bếp theo tên, nơi học, món làm được ----------
        public List<DauBepDTO> TimKiem(string ten, string maNoiHoc, string maMonAn) =>
            DbHelper.ExecuteList<DauBepDTO>(@"
                SELECT DISTINCT d.MaDauBep, d.TenDauBep, d.GioiTinh, d.DiaChi, d.DienThoai,
                       d.MaTrinhDo, td.TenTrinhDo, d.MaNoiHoc, nh.TenNoiHoc
                FROM DauBep d
                JOIN TrinhDo td ON td.MaTrinhDo = d.MaTrinhDo
                LEFT JOIN NoiHoc nh ON nh.MaNoiHoc = d.MaNoiHoc
                LEFT JOIN DauBep_MonAn dm ON dm.MaDauBep = d.MaDauBep
                WHERE (@Ten IS NULL OR d.TenDauBep LIKE N'%' + @Ten + N'%')
                  AND (@NH  IS NULL OR d.MaNoiHoc = @NH)
                  AND (@Mon IS NULL OR dm.MaMonAn = @Mon)
                ORDER BY d.MaDauBep", Map,
                DbHelper.P("@Ten", DbHelper.NullIfEmpty(ten)),
                DbHelper.P("@NH", DbHelper.NullIfEmpty(maNoiHoc)),
                DbHelper.P("@Mon", DbHelper.NullIfEmpty(maMonAn)));

        // ---------- CRUD ----------
        public void Insert(DauBepDTO d, List<DauBepMonAnDTO> monAn) => DbHelper.RunInTransaction((c, t) =>
        {
            DbHelper.ExecuteNonQuery(c, t,
                @"INSERT INTO DauBep(MaDauBep,TenDauBep,GioiTinh,DiaChi,DienThoai,MaTrinhDo,MaNoiHoc)
                  VALUES(@Ma,@Ten,@GT,@DC,@DT,@TD,@NH)", Params(d));
            foreach (var x in monAn ?? new List<DauBepMonAnDTO>())
                UpsertMonAn(c, t, d.MaDauBep, x);
        });

        //Đồng bộ danh sách món trong cùng transaction. Không xoá-rồi-thêm lại, vì ChiTietThucDon có FK tới DauBep_MonAn.</summary>
        public void Update(DauBepDTO d, List<DauBepMonAnDTO> monAn) => DbHelper.RunInTransaction((c, t) =>
        {
            DbHelper.ExecuteNonQuery(c, t,
                @"UPDATE DauBep SET TenDauBep=@Ten, GioiTinh=@GT, DiaChi=@DC, DienThoai=@DT, MaTrinhDo=@TD, MaNoiHoc=@NH
                  WHERE MaDauBep=@Ma", Params(d));

            var moi = (monAn ?? new List<DauBepMonAnDTO>()).Select(x => x.MaMonAn).ToHashSet();
            var cu = DbHelper.ExecuteList<string>(c, t, "SELECT MaMonAn FROM DauBep_MonAn WHERE MaDauBep=@Ma",
                r => r.Str("MaMonAn"), DbHelper.P("@Ma", d.MaDauBep));

            foreach (var ma in cu.Where(x => !moi.Contains(x)))
                DbHelper.ExecuteNonQuery(c, t, "DELETE FROM DauBep_MonAn WHERE MaDauBep=@Bep AND MaMonAn=@Mon",
                    DbHelper.P("@Bep", d.MaDauBep), DbHelper.P("@Mon", ma));
            foreach (var x in monAn ?? new List<DauBepMonAnDTO>())
                UpsertMonAn(c, t, d.MaDauBep, x);
        });

        //SqlException.Number = 547 nếu đầu bếp đã có trong chi tiết thực đơn.
        public int Delete(string ma) =>
            DbHelper.ExecuteNonQuery("DELETE FROM DauBep WHERE MaDauBep=@Ma", DbHelper.P("@Ma", ma));

        private static SqlParameter[] Params(DauBepDTO d) => new[]
        {
            DbHelper.P("@Ma", d.MaDauBep), DbHelper.P("@Ten", d.TenDauBep), DbHelper.P("@GT", d.GioiTinh),
            DbHelper.P("@DC", d.DiaChi), DbHelper.P("@DT", d.DienThoai), DbHelper.P("@TD", d.MaTrinhDo),
            DbHelper.P("@NH", DbHelper.NullIfEmpty(d.MaNoiHoc))
        };

        private static void UpsertMonAn(SqlConnection c, SqlTransaction t, string maDauBep, DauBepMonAnDTO x) =>
            DbHelper.ExecuteNonQuery(c, t,
                @"IF EXISTS (SELECT 1 FROM DauBep_MonAn WHERE MaDauBep=@Bep AND MaMonAn=@Mon)
                      UPDATE DauBep_MonAn SET MaXepLoai=@XL WHERE MaDauBep=@Bep AND MaMonAn=@Mon
                  ELSE
                      INSERT INTO DauBep_MonAn(MaDauBep,MaMonAn,MaXepLoai) VALUES(@Bep,@Mon,@XL)",
                DbHelper.P("@Bep", maDauBep), DbHelper.P("@Mon", x.MaMonAn), DbHelper.P("@XL", x.MaXepLoai));
    }
}
