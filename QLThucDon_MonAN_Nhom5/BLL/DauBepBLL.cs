using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLThucDon_MonAN_Nhom5.DAL;
using QLThucDon_MonAN_Nhom5.DTO;

namespace QLThucDon_MonAN_Nhom5.BLL
{
    public class DauBepBLL
    {
        private readonly DauBepDAL _dal = new DauBepDAL();

        public List<DauBepDTO> LayTatCa() => DbGuard.Run(() => _dal.GetAll());
        public DauBepDTO LayTheoMa(string ma) => DbGuard.Run(() => _dal.GetById(ma));
        public List<DauBepMonAnDTO> LayMonLamDuoc(string maDauBep) => DbGuard.Run(() => _dal.GetMonAn(maDauBep));

        // ---------- Yêu cầu 4 ----------
        public List<DauBepDTO> TimKiem(string ten, string maNoiHoc, string maMonAn) =>
            DbGuard.Run(() => _dal.TimKiem(ten, maNoiHoc, maMonAn));

        public void Them(DauBepDTO d, List<DauBepMonAnDTO> monAn)
        {
            Validate(d, monAn);
            if (string.IsNullOrWhiteSpace(d.MaDauBep)) d.MaDauBep = DbGuard.Run(() => _dal.NextCode());
            DbGuard.Run(() => _dal.Insert(d, monAn));
        }

        public void Sua(DauBepDTO d, List<DauBepMonAnDTO> monAn)
        {
            Check.Required(d.MaDauBep, "Mã đầu bếp");
            Validate(d, monAn);
            DbGuard.Run(() => _dal.Update(d, monAn));
        }

        public void Xoa(string ma) => DbGuard.Run(() => _dal.Delete(ma));

        private static void Validate(DauBepDTO d, List<DauBepMonAnDTO> monAn)
        {
            Check.Required(d.TenDauBep, "Tên đầu bếp");
            Check.Required(d.MaTrinhDo, "Trình độ");
            if (d.GioiTinh != "Nam" && d.GioiTinh != "Nữ")
                throw new BusinessException("Giới tính phải là Nam hoặc Nữ.");
            Check.Phone(d.DienThoai);
            d.TenDauBep = d.TenDauBep.Trim();

            monAn ??= new List<DauBepMonAnDTO>();
            foreach (var x in monAn) Check.Required(x.MaXepLoai, $"Xếp loại của món {x.TenMonAn}");
            if (monAn.GroupBy(x => x.MaMonAn).Any(g => g.Count() > 1))
                throw new BusinessException("Danh sách món làm được bị trùng.");
        }
    }
}
