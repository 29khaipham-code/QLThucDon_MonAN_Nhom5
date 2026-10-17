using QLThucDon_MonAN_Nhom5.DAL;
using QLThucDon_MonAN_Nhom5.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLThucDon_MonAN_Nhom5.BLL
{
    public class NguyenLieuBLL
    {
        private readonly NguyenLieuDAL _dal = new NguyenLieuDAL();

        public List<NguyenLieuDTO> LayTatCa() => DbGuard.Run(() => _dal.GetAll());
        public NguyenLieuDTO LayTheoMa(string ma) => DbGuard.Run(() => _dal.GetById(ma));

        public void Them(NguyenLieuDTO n)
        {
            Validate(n);
            if (string.IsNullOrWhiteSpace(n.MaNguyenLieu)) n.MaNguyenLieu = DbGuard.Run(() => _dal.NextCode());
            DbGuard.Run(() => _dal.Insert(n));
        }

        //Sửa giá nguyên liệu => DAL tự tính lại đơn giá các món ăn liên quan.
        public void Sua(NguyenLieuDTO n)
        {
            Check.Required(n.MaNguyenLieu, "Mã nguyên liệu");
            Validate(n);
            DbGuard.Run(() => _dal.Update(n));
        }

        public void Xoa(string ma) => DbGuard.Run(() => _dal.Delete(ma));

        private static void Validate(NguyenLieuDTO n)
        {
            Check.Required(n.TenNguyenLieu, "Tên nguyên liệu");
            Check.Required(n.MaDonViTinh, "Đơn vị tính");
            Check.NonNegative(n.DonGia, "Đơn giá");
            n.TenNguyenLieu = n.TenNguyenLieu.Trim();
        }
    }
}
