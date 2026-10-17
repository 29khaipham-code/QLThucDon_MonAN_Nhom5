using QLThucDon_MonAN_Nhom5.DAL;
using QLThucDon_MonAN_Nhom5.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLThucDon_MonAN_Nhom5.BLL
{
    public class ThucDonBLL
    {
        private readonly ThucDonDAL _dal = new ThucDonDAL();
        private readonly DauBepDAL _dauBepDal = new DauBepDAL();

        public List<ThucDonDTO> LayTatCa() => DbGuard.Run(() => _dal.GetAll());
        public ThucDonDTO LayTheoSo(string so) => DbGuard.Run(() => _dal.GetById(so));
        public List<ChiTietThucDonDTO> LayChiTiet(string so) => DbGuard.Run(() => _dal.GetChiTiet(so));
        public string TaoSoMoi() => DbGuard.Run(() => _dal.NextCode());

        // ---------- Tính tiền (xem trước trên form; cùng công thức với SQL) ----------
        public decimal TinhThanhTien(int soLuong, decimal donGia) =>
            Math.Round(soLuong * donGia, 2, MidpointRounding.AwayFromZero);

        //TongTien = tổng ThanhTien * (1 + VAT/100) - GiamGia, không âm.
        public decimal TinhTongTien(IEnumerable<ChiTietThucDonDTO> chiTiet, decimal vatPhanTram, decimal giamGia)
        {
            decimal tong = chiTiet.Sum(c => c.ThanhTien) * (1 + vatPhanTram / 100m) - giamGia;
            return Math.Max(0, Math.Round(tong, 2, MidpointRounding.AwayFromZero));
        }

        //Thêm một dòng món vào danh sách đang soạn (trùng món thì cộng dồn số lượng)
        public void ThemDong(List<ChiTietThucDonDTO> ds, MonAnDTO mon, string maDauBep, string tenDauBep, int soLuong)
        {
            if (mon == null) throw new BusinessException("Hãy chọn món ăn.");
            Check.Required(maDauBep, "Đầu bếp");
            Check.Positive(soLuong, "Số lượng");

            var cu = ds.FirstOrDefault(x => x.MaMonAn == mon.MaMonAn);
            if (cu != null)
            {
                if (cu.MaDauBep != maDauBep)
                    throw new BusinessException($"Món {mon.TenMonAn} đã có trong thực đơn do đầu bếp {cu.TenDauBep} làm.");
                cu.SoLuong += soLuong;
                cu.ThanhTien = TinhThanhTien(cu.SoLuong, cu.DonGia);
                return;
            }
            ds.Add(new ChiTietThucDonDTO
            {
                MaMonAn = mon.MaMonAn,
                TenMonAn = mon.TenMonAn,
                MaDauBep = maDauBep,
                TenDauBep = tenDauBep,
                SoLuong = soLuong,
                DonGia = mon.DonGia,
                ThanhTien = TinhThanhTien(soLuong, mon.DonGia)
            });
        }

        // ---------- Lưu ----------
        public void Them(ThucDonDTO td, List<ChiTietThucDonDTO> chiTiet)
        {
            Validate(td, chiTiet);
            if (string.IsNullOrWhiteSpace(td.SoThucDon)) td.SoThucDon = TaoSoMoi();
            DbGuard.Run(() => _dal.Insert(td, chiTiet));
        }

        public void Sua(ThucDonDTO td, List<ChiTietThucDonDTO> chiTiet)
        {
            Check.Required(td.SoThucDon, "Số thực đơn");
            Validate(td, chiTiet);
            DbGuard.Run(() => _dal.Update(td, chiTiet));
        }

        public void Xoa(string so) => DbGuard.Run(() => _dal.Delete(so));

        private void Validate(ThucDonDTO td, List<ChiTietThucDonDTO> ds)
        {
            Check.Required(td.MaKhachHang, "Khách hàng");
            if (td.ThueVAT < 0 || td.ThueVAT > 100) throw new BusinessException("VAT phải từ 0 đến 100 (%).");
            Check.NonNegative(td.GiamGia, "Giảm giá");
            if (ds == null || ds.Count == 0) throw new BusinessException("Thực đơn phải có ít nhất một món.");
            foreach (var c in ds) Check.Positive(c.SoLuong, $"Số lượng của món {c.TenMonAn}");
            if (ds.GroupBy(c => c.MaMonAn).Any(g => g.Count() > 1))
                throw new BusinessException("Danh sách món bị trùng.");

            // Giảm giá không được vượt tổng tiền sau thuế
            decimal sauThue = ds.Sum(c => c.ThanhTien) * (1 + td.ThueVAT / 100m);
            if (td.GiamGia > sauThue) throw new BusinessException("Giảm giá không được lớn hơn tổng tiền thực đơn.");

            // Đầu bếp phải là người làm được món đó (báo lỗi thân thiện trước khi DB từ chối)
            foreach (var nhom in ds.GroupBy(c => c.MaDauBep))
            {
                var lamDuoc = DbGuard.Run(() => _dauBepDal.GetMonAn(nhom.Key)).Select(x => x.MaMonAn).ToHashSet();
                var sai = nhom.FirstOrDefault(c => !lamDuoc.Contains(c.MaMonAn));
                if (sai != null)
                    throw new BusinessException($"Đầu bếp {sai.TenDauBep} không làm được món {sai.TenMonAn}.");
            }
        }
    }
}
