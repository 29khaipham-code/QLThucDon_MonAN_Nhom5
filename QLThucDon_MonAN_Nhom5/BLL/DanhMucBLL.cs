using QLThucDon_MonAN_Nhom5.DAL;
using QLThucDon_MonAN_Nhom5.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLThucDon_MonAN_Nhom5.DAL;
using QLThucDon_MonAN_Nhom5.DTO;

namespace QLThucDon_MonAN_Nhom5.BLL
{
    public class DanhMucBLL
    {
        private readonly DanhMucDAL _dal = new DanhMucDAL();
        public List<DanhMucDTO> LayTatCa(LoaiDanhMuc loai) => DbGuard.Run(() => _dal.GetAll(loai));
        public void Them(LoaiDanhMuc loai, DanhMucDTO d)
        {
            Check.Required(d.Ten, "Tên");
            d.Ten = d.Ten.Trim();
            if (string.IsNullOrWhiteSpace(d.Ma)) d.Ma = DbGuard.Run(() => _dal.NextCode(loai));
            DbGuard.Run(() => _dal.Insert(loai, d));
        }

        public void Sua(LoaiDanhMuc loai, DanhMucDTO d)
        {
            Check.Required(d.Ma, "Mã");
            Check.Required(d.Ten, "Tên");
            d.Ten = d.Ten.Trim();
            DbGuard.Run(() => _dal.Update(loai, d));
        }

        public void Xoa(LoaiDanhMuc loai, string ma) => DbGuard.Run(() => _dal.Delete(loai, ma));
    }
}
