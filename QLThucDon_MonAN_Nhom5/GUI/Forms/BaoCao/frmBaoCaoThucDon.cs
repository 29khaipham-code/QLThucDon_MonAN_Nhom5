using QLThucDon_MonAN_Nhom5.BLL;
using System;
using System.Data;
using System.Linq;

namespace QLThucDon_MonAN_Nhom5.GUI.Forms.BaoCao
{
    /// <summary>Báo cáo thông tin đầy đủ các thực đơn (lọc theo một thực đơn hoặc xem tất cả).
    /// Control cần có: lblLoc, cmbLoc, btnXem, btnXuat, btnIn, lblTieuDeBC, dgv, lblSoDong.</summary>
    public partial class frmBaoCaoThucDon : BaseReportForm
    {
        private readonly BaoCaoBLL _bll = new BaoCaoBLL();
        private readonly ThucDonBLL _thucDonBll = new ThucDonBLL();

        // Tên cột trùng tên cột SQL trong BaoCaoDAL.ThucDonDayDu
        private static readonly CotBaoCao[] Cot =
        {
            new CotBaoCao("SoThucDon",    "Số TĐ",      0.8f, rong: 80),
            new CotBaoCao("NgayDung",     "Ngày dùng",  1.0f, dinhDang: "dd/MM/yyyy", rong: 90),
            new CotBaoCao("TenKhachHang", "Khách hàng", 1.5f, rong: 130),
            new CotBaoCao("MaSoThue",     "MST",        1.0f, rong: 90),
            new CotBaoCao("TenMonAn",     "Tên món",    1.8f),
            new CotBaoCao("TenDauBep",    "Đầu bếp",    1.3f, rong: 110),
            new CotBaoCao("SoLuong",      "SL",         0.5f, phai: true, dinhDang: "N0", rong: 50),
            new CotBaoCao("DonGia",       "Đơn giá",    1.0f, phai: true, dinhDang: "N0", rong: 80),
            new CotBaoCao("ThanhTien",    "Thành tiền", 1.1f, phai: true, dinhDang: "N0", rong: 90),
            new CotBaoCao("ThueVAT",      "VAT (%)",    0.7f, phai: true, dinhDang: "0.##", rong: 60),
            new CotBaoCao("GiamGia",      "Giảm giá",   1.0f, phai: true, dinhDang: "N0", rong: 80),
            new CotBaoCao("TongTien",     "Tổng tiền",  1.1f, phai: true, dinhDang: "N0", rong: 90),
        };

        protected override CotBaoCao[] CacCot => Cot;
        protected override string TenFileXuat => "ThongTinThucDon";
        protected override bool LuoiXuongDong => true;

        protected override void NapBoLoc()
        {
            Ui.Run(() =>
            {
                var ds = _thucDonBll.LayTatCa().Select(t => new MucLoc
                {
                    Ma = t.SoThucDon,
                    Ten = $"{t.SoThucDon} - {t.TenKhachHang} ({t.NgayDung:dd/MM/yyyy})"
                });
                BaoCaoHelper.NapLoc(cmbLoc, "Tất cả thực đơn", ds);
            });
        }

        protected override void SauKhiNapXong() => cmbLoc.SelectedIndexChanged += (s, e) => Xem();

        protected override DataTable LayDuLieu() => _bll.ThucDonDayDu(BaoCaoHelper.LayMaLoc(cmbLoc));

        protected override string TinhTieuDe()
        {
            string ma = BaoCaoHelper.LayMaLoc(cmbLoc);
            return string.IsNullOrEmpty(ma) ? "THÔNG TIN CÁC THỰC ĐƠN" : "THÔNG TIN THỰC ĐƠN " + ma;
        }

        protected override string TinhThongKe(DataTable dt)
        {
            string s = "Số dòng: " + dt.Rows.Count;
            // Xem một thực đơn: hiện luôn tổng tiền của thực đơn đó
            if (!string.IsNullOrEmpty(BaoCaoHelper.LayMaLoc(cmbLoc)) && dt.Rows.Count > 0)
                s += "   |   Tổng tiền: " + Convert.ToDecimal(dt.Rows[0]["TongTien"]).ToString("N0");
            return s;
        }

        public frmBaoCaoThucDon()
        {
            InitializeComponent();
            KhoiTaoBaoCao(dgv, btnXem, btnXuat, btnIn, lblTieuDeBC, lblSoDong);
            Text = "Báo cáo thông tin các thực đơn";
            lblLoc.Text = "Số thực đơn";
        }
    }
}
