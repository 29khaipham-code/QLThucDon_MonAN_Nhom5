using QLThucDon_MonAN_Nhom5.BLL;
using QLThucDon_MonAN_Nhom5.GUI;
using QLThucDon_MonAN_Nhom5.BLL;
using System;
using System.Data;
using System.Windows.Forms;
using QLThucDon_MonAN_Nhom5.BLL;
using QLThucDon_MonAN_Nhom5.GUI;
    /// Báo cáo thông tin đầy đủ các món ăn. Không có bộ lọc.
    /// Kế thừa BaseReportForm: chỉ khai báo cột, nguồn dữ liệu và tiêu đề.
    /// Control cần có trong Designer: btnXem, btnXuat, btnIn, lblTieuDeBC, dgv, lblSoDong.
    /// Control cần có trong Designer (đặt đúng tên):
    ///   btnXem, btnXuat, btnIn (Button)   lblTieuDeBC (Label)   dgv (DataGridView)   lblSoDong (Label)
    /// Form này không dùng bộ lọc: xoá lblLoc và cmbLoc khỏi thiết kế (hoặc để trống, code không dùng đến).
    /// Mọi sự kiện được gắn bằng code trong constructor, KHÔNG bấm đúp control trong Designer.
    {
        private readonly BaoCaoBLL _bll = new BaoCaoBLL();
        private DataTable _dt;
        private const string TieuDe = "DANH SÁCH THÔNG TIN CÁC MÓN ĂN";

        private const string TieuDe =
            "DANH SÁCH THÔNG TIN CÁC MÓN ĂN";
            new CotBaoCao("MaMonAn",     "Mã món",      0.7f, rong: 80),
            new CotBaoCao("TenMonAn",    "Tên món",     1.6f, rong: 180),
            new CotBaoCao("TenLoai",     "Loại",        1.0f, rong: 110),
            new CotBaoCao("TenCongDung", "Công dụng",   1.1f, rong: 120),
            new CotBaoCao("DonGia",      "Đơn giá",     1.0f, phai: true, dinhDang: "N0", rong: 100),
            new CotBaoCao("NguyenLieu",  "Nguyên liệu", 3.0f),
            new CotBaoCao("CachLam",     "Cách làm",    3.0f),
            new CotBaoCao("YeuCau",      "Yêu cầu",     1.2f, rong: 120),
            new CotBaoCao("DonGia",      "Đơn giá",     1.0f, phai: true, dinhDang: "N0", rong: 100),
            new CotBaoCao("NguyenLieu",  "Nguyên liệu", 3.0f),                  // cột tự giãn
            new CotBaoCao("CachLam",     "Cách làm",    3.0f),                  // cột tự giãn
            new CotBaoCao("YeuCau",      "Yêu cầu",     1.2f, rong: 120),
        };

        protected override CotBaoCao[] CacCot => Cot;
        protected override string TenFileXuat => "ThongTinMonAn";
        protected override bool LuoiXuongDong => true;           // cột Nguyên liệu, Cách làm dài
        protected override DataTable LayDuLieu() => _bll.MonAnDayDu();
        protected override string TinhTieuDe() => "DANH SÁCH THÔNG TIN CÁC MÓN ĂN";
        protected override string TinhThongKe(DataTable dt) => "Số món ăn: " + dt.Rows.Count;
            KhoiTaoBaoCao(dgv, btnXem, btnXuat, btnIn, lblTieuDeBC, lblSoDong);
        public frmBaoCaoMonAn()
        {

            // ==================== 5. THỐNG KÊ SỐ MÓN ====================
            lblSoDong.ForeColor = Ui.Accent;
            lblSoDong.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        }
    }
}
