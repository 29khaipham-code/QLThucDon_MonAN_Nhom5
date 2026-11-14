using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLThucDon_MonAN_Nhom5.DAL;
using QLThucDon_MonAN_Nhom5.DTO;
using QLThucDon_MonAN_Nhom5.Interfaces;

namespace QLThucDon_MonAN_Nhom5.BLL
{
    public class KhachHangBLL
    {
        private readonly IKhachHangDAL _dal;

        public KhachHangBLL() : this(new KhachHangDAL())
        {
        }

        public KhachHangBLL(IKhachHangDAL dal)
        {
            _dal = dal ?? throw new ArgumentNullException(nameof(dal));
        }

        public string TaoMaMoi() => DbGuard.Run(() => _dal.NextCode());

        public List<KhachHangDTO> LayTatCa() => DbGuard.Run(() => _dal.GetAll());
        public KhachHangDTO LayTheoMa(string ma) => DbGuard.Run(() => _dal.GetById(ma));
        public List<KhachHangDTO> TimKiem(string ten) => DbGuard.Run(() => _dal.TimKiem(ten));

        public void Them(KhachHangDTO k)
        {
            Validate(k);
            if (string.IsNullOrWhiteSpace(k.MaKhachHang)) k.MaKhachHang = DbGuard.Run(() => _dal.NextCode());
            DbGuard.Run(() => _dal.Insert(k));
        }

        public void Sua(KhachHangDTO k)
        {
            Check.Required(k.MaKhachHang, "Mã khách hàng");
            Validate(k);
            DbGuard.Run(() => _dal.Update(k));
        }

        public void Xoa(string ma) => DbGuard.Run(() => _dal.Delete(ma));

        private static void Validate(KhachHangDTO k)
        {
            Check.Required(k.TenKhachHang, "Tên khách hàng");
            Check.Phone(k.DienThoai);
            Check.MaSoThue(k.MaSoThue);
            k.TenKhachHang = k.TenKhachHang.Trim();
        }

    }
}
