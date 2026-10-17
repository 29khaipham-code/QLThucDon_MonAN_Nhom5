using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLThucDon_MonAN_Nhom5.DTO;
using QLThucDon_MonAN_Nhom5.DAL;

namespace QLThucDon_MonAN_Nhom5.BLL
{
    public class MonAnBLL
    {
        private  readonly MonAnDAL _dal = new MonAnDAL();

        public List<MonAnDTO> LayTatCa() => DbGuard.Run(() => _dal.GetAll());// dua nghiep vu vao Dbguard 
        public MonAnDTO LayTheoMa(string ma) => DbGuard.Run(() => _dal.GetById(ma));
        public List<NguyenLieuMonAnDTO> LayNguyenLieu(string maMonAn) => DbGuard.Run(() => _dal.GetNguyenLieu(maMonAn));
        public List<MonAnDTO> TimKiem(string maLoai, string maCongDung, string tenMon) =>
            DbGuard.Run(() => _dal.TimKiem(maLoai, maCongDung, tenMon));

        // ---------- Yêu cầu 2: chỉ cho lấy món khi đã chọn đủ Loại và Đầu bếp ----------
        public List<MonAnDTO> LayMonTheoLoaiVaDauBep(string maLoai, string maDauBep)
        {
            if (string.IsNullOrWhiteSpace(maLoai) || string.IsNullOrWhiteSpace(maDauBep))
                return new List<MonAnDTO>();
            return DbGuard.Run(() => _dal.GetByLoaiVaDauBep(maLoai, maDauBep));
        }

        // ---------- Yêu cầu 1: đơn giá = tổng tiền nguyên liệu (xem trước trên form) ----------
        public decimal TinhDonGia(IEnumerable<NguyenLieuMonAnDTO> nguyenLieu) =>
            Math.Round(nguyenLieu.Sum(x => x.SoLuong * x.DonGia), 2, MidpointRounding.AwayFromZero);

        /// <summary>Thêm 1 nguyên liệu vào danh sách đang soạn trên form (trùng thì cộng dồn số lượng).</summary>
        public void ThemDongNguyenLieu(List<NguyenLieuMonAnDTO> ds, NguyenLieuDTO nl, decimal soLuong)
        {
            Check.Positive(soLuong, "Số lượng nguyên liệu");
            var cu = ds.FirstOrDefault(x => x.MaNguyenLieu == nl.MaNguyenLieu);
            if (cu != null) { cu.SoLuong += soLuong; cu.ThanhTien = cu.SoLuong * cu.DonGia; return; }
            ds.Add(new NguyenLieuMonAnDTO
            {
                MaNguyenLieu = nl.MaNguyenLieu,
                TenNguyenLieu = nl.TenNguyenLieu,
                TenDonViTinh = nl.TenDonViTinh,
                SoLuong = soLuong,
                DonGia = nl.DonGia,
                ThanhTien = soLuong * nl.DonGia
            });
        }

        public void Them(MonAnDTO m, List<NguyenLieuMonAnDTO> nguyenLieu)
        {
            Validate(m, nguyenLieu);
            if (string.IsNullOrWhiteSpace(m.MaMonAn)) m.MaMonAn = DbGuard.Run(() => _dal.NextCode());
            DbGuard.Run(() => _dal.Insert(m, nguyenLieu));
        }

        public void Sua(MonAnDTO m, List<NguyenLieuMonAnDTO> nguyenLieu)
        {
            Check.Required(m.MaMonAn, "Mã món ăn");
            Validate(m, nguyenLieu);
            DbGuard.Run(() => _dal.Update(m, nguyenLieu));
        }

        public void Xoa(string ma) => DbGuard.Run(() => _dal.Delete(ma));

        private static void Validate(MonAnDTO m, List<NguyenLieuMonAnDTO> nl)
        {
            Check.Required(m.TenMonAn, "Tên món ăn");
            Check.Required(m.MaLoai, "Loại món");
            if (nl == null || nl.Count == 0)
                throw new BusinessException("Món ăn phải có ít nhất một nguyên liệu.");
            foreach (var x in nl) Check.Positive(x.SoLuong, $"Số lượng của nguyên liệu {x.TenNguyenLieu}");
            if (nl.GroupBy(x => x.MaNguyenLieu).Any(g => g.Count() > 1))
                throw new BusinessException("Danh sách nguyên liệu bị trùng.");
            m.TenMonAn = m.TenMonAn.Trim();
        }
    }
}
