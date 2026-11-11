using QLThucDon_MonAN_Nhom5.BLL;
using System.Data;
using System.Linq;

namespace QLThucDon_MonAN_Nhom5.GUI.Forms.BaoCao
{
    /// <summary>Báo cáo món ăn của từng đầu bếp (lọc theo đầu bếp hoặc xem tất cả).
    /// Control cần có: lblLoc, cmbLoc, btnXem, btnXuat, btnIn, lblTieuDeBC, dgv, lblSoDong.</summary>
    public partial class frmBaoCaoMonTheoDauBep : BaseReportForm
    {
        private readonly BaoCaoBLL _bll = new BaoCaoBLL();
        private readonly DauBepBLL _dauBepBll = new DauBepBLL();

        // Tên cột trùng tên cột SQL trong BaoCaoDAL.MonAnTheoDauBep
        private static readonly CotBaoCao[] Cot =
        {
            new CotBaoCao("MaDauBep",   "Mã đầu bếp",  1.0f, rong: 100),
            new CotBaoCao("TenDauBep",  "Tên đầu bếp", 1.6f, rong: 160),
            new CotBaoCao("MaMonAn",    "Mã món",      0.9f, rong: 90),
            new CotBaoCao("TenMonAn",   "Tên món",     2.2f),
            new CotBaoCao("TenLoai",    "Loại",        1.2f, rong: 120),
            new CotBaoCao("DonGia",     "Đơn giá",     1.1f, phai: true, dinhDang: "N0", rong: 110),
            new CotBaoCao("TenXepLoai", "Xếp loại",    1.0f, rong: 100),
        };

        protected override CotBaoCao[] CacCot => Cot;
        protected override string TenFileXuat => "MonAnTheoDauBep";

        protected override void NapBoLoc()
        {
            Ui.Run(() =>
            {
                var ds = _dauBepBll.LayTatCa().Select(d => new MucLoc { Ma = d.MaDauBep, Ten = d.TenDauBep });
                BaoCaoHelper.NapLoc(cmbLoc, "Tất cả đầu bếp", ds);
            });
        }

        // Gắn sự kiện sau khi nạp xong để việc nạp không kích hoạt xem báo cáo thừa
        protected override void SauKhiNapXong() => cmbLoc.SelectedIndexChanged += (s, e) => Xem();

        protected override DataTable LayDuLieu() => _bll.MonAnTheoDauBep(BaoCaoHelper.LayMaLoc(cmbLoc));

        protected override string TinhTieuDe() =>
            string.IsNullOrEmpty(BaoCaoHelper.LayMaLoc(cmbLoc))
                ? "DANH SÁCH MÓN ĂN CỦA TỪNG ĐẦU BẾP"
                : "DANH SÁCH MÓN ĂN CỦA ĐẦU BẾP " + cmbLoc.Text.ToUpper();

        public frmBaoCaoMonTheoDauBep()
        {
            InitializeComponent();
            KhoiTaoBaoCao(dgv, btnXem, btnXuat, btnIn, lblTieuDeBC, lblSoDong);
            Text = "Báo cáo món ăn của từng đầu bếp";
            lblLoc.Text = "Đầu bếp";
        }
    }
}
