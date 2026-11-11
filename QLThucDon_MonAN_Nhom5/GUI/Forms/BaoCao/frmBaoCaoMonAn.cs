using QLThucDon_MonAN_Nhom5.BLL;
using System.Data;

namespace QLThucDon_MonAN_Nhom5.GUI.Forms.BaoCao
{
    /// <summary>
    /// Báo cáo thông tin đầy đủ các món ăn. Không có bộ lọc.
    /// Kế thừa BaseReportForm: chỉ khai báo cột, nguồn dữ liệu và tiêu đề.
    /// Control cần có trong Designer: btnXem, btnXuat, btnIn, lblTieuDeBC, dgv, lblSoDong.
    /// </summary>
    public partial class frmBaoCaoMonAn : BaseReportForm
    {
        private readonly BaoCaoBLL _bll = new BaoCaoBLL();

        // Tên cột trùng tên cột SQL trong BaoCaoDAL.MonAnDayDu
        private static readonly CotBaoCao[] Cot =
        {
            new CotBaoCao("MaMonAn",     "Mã món",      0.7f, rong: 80),
            new CotBaoCao("TenMonAn",    "Tên món",     1.6f, rong: 180),
            new CotBaoCao("TenLoai",     "Loại",        1.0f, rong: 110),
            new CotBaoCao("TenCongDung", "Công dụng",   1.1f, rong: 120),
            new CotBaoCao("DonGia",      "Đơn giá",     1.0f, phai: true, dinhDang: "N0", rong: 100),
            new CotBaoCao("NguyenLieu",  "Nguyên liệu", 3.0f),
            new CotBaoCao("CachLam",     "Cách làm",    3.0f),
            new CotBaoCao("YeuCau",      "Yêu cầu",     1.2f, rong: 120),
        };

        protected override CotBaoCao[] CacCot => Cot;
        protected override string TenFileXuat => "ThongTinMonAn";
        protected override bool LuoiXuongDong => true;           // cột Nguyên liệu, Cách làm dài
        protected override DataTable LayDuLieu() => _bll.MonAnDayDu();
        protected override string TinhTieuDe() => "DANH SÁCH THÔNG TIN CÁC MÓN ĂN";
        protected override string TinhThongKe(DataTable dt) => "Số món ăn: " + dt.Rows.Count;

        public frmBaoCaoMonAn()
        {
            InitializeComponent();
            KhoiTaoBaoCao(dgv, btnXem, btnXuat, btnIn, lblTieuDeBC, lblSoDong);
            Text = "Báo cáo thông tin các món ăn";
        }
    }
}
