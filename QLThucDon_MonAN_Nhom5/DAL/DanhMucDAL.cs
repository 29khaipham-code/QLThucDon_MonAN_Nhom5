using QLThucDon_MonAN_Nhom5.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLThucDon_MonAN_Nhom5.DAL
{
    public enum LoaiDanhMuc { LoaiMon, CongDung, DonViTinh, NoiHoc, TrinhDo, XepLoai }
    public class DanhMucDAL
    {
        private static readonly Dictionary<LoaiDanhMuc, (string Table, string Ma, string Ten, string Prefix)> Map = new Dictionary<LoaiDanhMuc, (string Table, string Ma, string Ten, string Prefix)>
        {
            [LoaiDanhMuc.LoaiMon] = ("LoaiMon", "MaLoai", "TenLoai", "LM"),
            [LoaiDanhMuc.CongDung] = ("CongDung", "MaCongDung", "TenCongDung", "CD"),
            [LoaiDanhMuc.DonViTinh] = ("DonViTinh", "MaDonViTinh", "TenDonViTinh", "DV"),
            [LoaiDanhMuc.NoiHoc] = ("NoiHoc", "MaNoiHoc", "TenNoiHoc", "NH"),
            [LoaiDanhMuc.TrinhDo] = ("TrinhDo", "MaTrinhDo", "TenTrinhDo", "TD"),
            [LoaiDanhMuc.XepLoai] = ("XepLoai", "MaXepLoai", "TenXepLoai", "XL"),
        };

        public List<DanhMucDTO> GetAll(LoaiDanhMuc loai)
        {
            var m = Map[loai];
            return DbHelper.ExecuteList<DanhMucDTO>(
                $"SELECT {m.Ma} AS Ma, {m.Ten} AS Ten FROM {m.Table} ORDER BY {m.Ten}",
                r => new DanhMucDTO { Ma = r.Str("Ma"), Ten = r.Str("Ten") });
        }

        public string NextCode(LoaiDanhMuc loai)
        {
            var m = Map[loai];
            return DbHelper.NextCode(m.Table, m.Ma, m.Prefix, 2);
        }

        public int Insert(LoaiDanhMuc loai, DanhMucDTO d)
        {
            var m = Map[loai];
            return DbHelper.ExecuteNonQuery($"INSERT INTO {m.Table}({m.Ma},{m.Ten}) VALUES(@Ma,@Ten)",
                DbHelper.P("@Ma", d.Ma), DbHelper.P("@Ten", d.Ten));
        }

        public int Update(LoaiDanhMuc loai, DanhMucDTO d)
        {
            var m = Map[loai];
            return DbHelper.ExecuteNonQuery($"UPDATE {m.Table} SET {m.Ten}=@Ten WHERE {m.Ma}=@Ma",
                DbHelper.P("@Ma", d.Ma), DbHelper.P("@Ten", d.Ten));
        }

        //Ném SqlException (Number = 547) nếu danh mục đang được dùng (vi phạm FK) - BLL/GUI nên bắt và báo người dùng
        public int Delete(LoaiDanhMuc loai, string ma)
        {
            var m = Map[loai];
            return DbHelper.ExecuteNonQuery($"DELETE FROM {m.Table} WHERE {m.Ma}=@Ma", DbHelper.P("@Ma", ma));
        }
    }
}
