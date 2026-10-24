using System.Drawing;
using QLThucDon_MonAN_Nhom5.BLL;
using QLThucDon_MonAN_Nhom5.GUI;
using QLThucDon_MonAN_Nhom5.GUI.Forms.BaoCao;
using QLThucDon_MonAN_Nhom5.BLL;
using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace QLThucDon_MonAN_Nhom5.GUI.Forms.BaoCao
{
    /// <summary>
    /// Báo cáo danh sách món ăn của từng đầu bếp (yêu cầu 7). Lọc theo một đầu bếp hoặc xem tất cả.
    ///
    /// Control cần có trong Designer (đặt đúng tên):
    ///   lblLoc (Label)   cmbLoc (ComboBox)   btnXem, btnXuat, btnIn (Button)
    ///   lblTieuDeBC (Label)   dgv (DataGridView)   lblSoDong (Label)
    /// Mọi sự kiện được gắn bằng code trong constructor, KHÔNG bấm đúp control trong Designer.
    /// </summary>
    public partial class frmBaoCaoMonTheoDauBep : Form
    {
        private readonly BaoCaoBLL _bll = new BaoCaoBLL();
        private DataTable _dt;
        private string _tieuDe = "DANH SÁCH MÓN ĂN CỦA TỪNG ĐẦU BẾP";

        // Tên cột trùng tên cột SQL trong BaoCaoDAL.MonAnTheoDauBep
        private static readonly CotBaoCao[] Cot =
        {
            new CotBaoCao("MaDauBep",   "Mã đầu bếp",  1.0f, rong: 100),
            new CotBaoCao("TenDauBep",  "Tên đầu bếp", 1.6f, rong: 160),
            new CotBaoCao("MaMonAn",    "Mã món",      0.9f, rong: 90),
            new CotBaoCao("TenMonAn",   "Tên món",     2.2f),                    // cột tự giãn
            new CotBaoCao("TenLoai",    "Loại",        1.2f, rong: 120),
            new CotBaoCao("DonGia",     "Đơn giá",     1.1f, phai: true, dinhDang: "N0", rong: 110),
            new CotBaoCao("TenXepLoai", "Xếp loại",    1.0f, rong: 100),
        };

        public frmBaoCaoMonTheoDauBep()
        {
            InitializeComponent();

            ApDungGiaoDien();
            Text = "Báo cáo món ăn của từng đầu bếp";
            lblLoc.Text = "Đầu bếp";

            Load += frmBaoCaoMonTheoDauBep_Load;
            btnXem.Click += (s, e) => Xem();
            btnXuat.Click += (s, e) => BaoCaoHelper.XuatExcel(this, _dt, _tieuDe, Cot, "MonAnTheoDauBep");
            btnIn.Click += (s, e) => BaoCaoHelper.In(this, _dt, _tieuDe, Cot);
        }

        private void frmBaoCaoMonTheoDauBep_Load(object sender, EventArgs e)
        {
            BaoCaoHelper.CauHinhLuoi(dgv, Cot);

            // Nạp danh sách đầu bếp vào bộ lọc, dòng đầu là "Tất cả đầu bếp"
            Ui.Run(() =>
            {
                var ds = new DauBepBLL().LayTatCa()
                    .Select(d => new MucLoc { Ma = d.MaDauBep, Ten = d.TenDauBep });
                BaoCaoHelper.NapLoc(cmbLoc, "Tất cả đầu bếp", ds);
            });

            Xem();

            // Gắn sự kiện sau khi nạp xong để việc nạp không kích hoạt xem báo cáo thừa
            cmbLoc.SelectedIndexChanged += (s, ev) => Xem();
        }

        private void Xem()
        {
            string ma = BaoCaoHelper.LayMaLoc(cmbLoc);   // "" = tất cả

            DataTable dt = null;
            if (!Ui.Run(() => dt = _bll.MonAnTheoDauBep(ma))) return;

            _dt = dt;
            _tieuDe = string.IsNullOrEmpty(ma)
                ? "DANH SÁCH MÓN ĂN CỦA TỪNG ĐẦU BẾP"
                : "DANH SÁCH MÓN ĂN CỦA ĐẦU BẾP " + cmbLoc.Text.ToUpper();

            dgv.DataSource = dt;
            lblTieuDeBC.Text = _tieuDe;
            lblSoDong.Text = "Số dòng: " + dt.Rows.Count;
        }
        private void ApDungGiaoDien()
        {
            // ==================== 1. FORM ====================
            BackColor = Ui.Back;
            ForeColor = Ui.Text;
            Font = new Font("Segoe UI", 9F);

            // ==================== 2. BỘ LỌC ĐẦU BẾP ====================
            lblLoc.ForeColor = Ui.Dark;
            lblLoc.Font = new Font("Segoe UI", 9F, FontStyle.Bold);

            cmbLoc.BackColor = Ui.Surface;
            cmbLoc.ForeColor = Ui.Text;
            cmbLoc.FlatStyle = FlatStyle.Flat;
            cmbLoc.Font = new Font("Segoe UI", 9F);

            // ==================== 3. CÁC NÚT CHỨC NĂNG ====================
            Button[] dsNut = { btnXem, btnXuat, btnIn };

            foreach (Button b in dsNut)
            {
                b.UseVisualStyleBackColor = false;
                b.FlatStyle = FlatStyle.Flat;
                b.FlatAppearance.BorderSize = 1;
                b.FlatAppearance.BorderColor = Ui.Border;

                b.BackColor = Ui.Surface;
                b.ForeColor = Ui.Dark;
                b.Font = new Font("Segoe UI", 9F, FontStyle.Bold);

                b.FlatAppearance.MouseOverBackColor = Ui.Hover;
                b.FlatAppearance.MouseDownBackColor = Ui.Header;

                b.Cursor = Cursors.Hand;
            }

            // Nút Xem báo cáo: màu nhấn chính
            btnXem.BackColor = Ui.Accent;
            btnXem.ForeColor = Color.White;
            btnXem.FlatAppearance.BorderColor = Ui.Accent;
            btnXem.FlatAppearance.MouseOverBackColor = Ui.Dark;
            btnXem.FlatAppearance.MouseDownBackColor = Ui.Dark;

            // ==================== 4. BẢNG DỮ LIỆU ====================
            dgv.EnableHeadersVisualStyles = false;
            dgv.BackgroundColor = Ui.Surface;
            dgv.BorderStyle = BorderStyle.None;
            dgv.GridColor = Ui.Border;

            dgv.ReadOnly = true;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.AllowUserToResizeRows = false;

            dgv.MultiSelect = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.RowHeadersVisible = false;

            // Tiêu đề cột
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Ui.Header;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Ui.Dark;
            dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = Ui.Header;
            dgv.ColumnHeadersDefaultCellStyle.SelectionForeColor = Ui.Dark;
            dgv.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 9F, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleLeft;
            dgv.ColumnHeadersDefaultCellStyle.Padding =
                new Padding(6, 0, 6, 0);

            dgv.ColumnHeadersHeight = 38;
            dgv.ColumnHeadersHeightSizeMode =
                DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            // Nội dung ô
            dgv.DefaultCellStyle.BackColor = Ui.Surface;
            dgv.DefaultCellStyle.ForeColor = Ui.Text;
            dgv.DefaultCellStyle.SelectionBackColor = Ui.Accent;
            dgv.DefaultCellStyle.SelectionForeColor = Color.White;
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 9F);
            dgv.DefaultCellStyle.Padding = new Padding(6, 0, 6, 0);

            // Hàng xen kẽ
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Ui.Back;
            dgv.AlternatingRowsDefaultCellStyle.ForeColor = Ui.Text;
            dgv.AlternatingRowsDefaultCellStyle.SelectionBackColor = Ui.Accent;
            dgv.AlternatingRowsDefaultCellStyle.SelectionForeColor = Color.White;

            dgv.RowTemplate.Height = 34;

            // ==================== 5. TIÊU ĐỀ BÁO CÁO ====================
            lblTieuDeBC.ForeColor = Ui.Dark;
            lblTieuDeBC.Font = new Font("Segoe UI", 12F, FontStyle.Bold);

            // ==================== 6. THỐNG KÊ SỐ DÒNG ====================
            lblSoDong.ForeColor = Ui.Accent;
            lblSoDong.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        }
    }
}