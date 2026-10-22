using System.Drawing;
using QLThucDon_MonAN_Nhom5.BLL;
using QLThucDon_MonAN_Nhom5.GUI;
using QLThucDon_MonAN_Nhom5.GUI.Forms.BaoCao;
using QLThucDon_MonAN_Nhom5.BLL;
using System;
using System.Data;
using System.Windows.Forms;

namespace QLThucDon_MonAN_Nhom5.GUI.Forms.BaoCao
{
    /// <summary>
    /// Báo cáo thông tin đầy đủ các món ăn (yêu cầu 6). Không có bộ lọc.
    ///
    /// Control cần có trong Designer (đặt đúng tên):
    ///   btnXem, btnXuat, btnIn (Button)   lblTieuDeBC (Label)   dgv (DataGridView)   lblSoDong (Label)
    /// Form này không dùng bộ lọc: xoá lblLoc và cmbLoc khỏi thiết kế (hoặc để trống, code không dùng đến).
    /// Mọi sự kiện được gắn bằng code trong constructor, KHÔNG bấm đúp control trong Designer.
    /// </summary>
    public partial class frmBaoCaoMonAn : Form
    {
        private readonly BaoCaoBLL _bll = new BaoCaoBLL();
        private DataTable _dt;
        private const string TieuDe = "DANH SÁCH THÔNG TIN CÁC MÓN ĂN";

        // Tên cột trùng tên cột SQL trong BaoCaoDAL.MonAnDayDu
        private static readonly CotBaoCao[] Cot =
        {
            new CotBaoCao("MaMonAn",     "Mã món",      0.7f, rong: 80),
            new CotBaoCao("TenMonAn",    "Tên món",     1.6f, rong: 180),
            new CotBaoCao("TenLoai",     "Loại",        1.0f, rong: 110),
            new CotBaoCao("TenCongDung", "Công dụng",   1.1f, rong: 120),
            new CotBaoCao("DonGia",      "Đơn giá",     1.0f, phai: true, dinhDang: "N0", rong: 100),
            new CotBaoCao("NguyenLieu",  "Nguyên liệu", 3.0f),                  // cột tự giãn
            new CotBaoCao("CachLam",     "Cách làm",    3.0f),                  // cột tự giãn
            new CotBaoCao("YeuCau",      "Yêu cầu",     1.2f, rong: 120),
        };

        public frmBaoCaoMonAn()
        {
            InitializeComponent();
            ApDungGiaoDien();
            Text = "Báo cáo thông tin các món ăn";
            lblTieuDeBC.Text = TieuDe;

            Load += (s, e) =>
            {
                BaoCaoHelper.CauHinhLuoi(dgv, Cot);
                Xem();   // mở form là có báo cáo ngay
            };
            btnXem.Click += (s, e) => Xem();
            btnXuat.Click += (s, e) => BaoCaoHelper.XuatExcel(this, _dt, TieuDe, Cot, "ThongTinMonAn");
            btnIn.Click += (s, e) => BaoCaoHelper.In(this, _dt, TieuDe, Cot);
        }

        private void Xem()
        {
            DataTable dt = null;
            if (!Ui.Run(() => dt = _bll.MonAnDayDu())) return;

            _dt = dt;
            dgv.DataSource = dt;
            lblSoDong.Text = "Số món ăn: " + dt.Rows.Count;
        }
        private void ApDungGiaoDien()
        {
            // ==================== 1. FORM ====================
            BackColor = Ui.Back;
            ForeColor = Ui.Text;
            Font = new Font("Segoe UI", 9F);

            // ==================== 2. CÁC NÚT CHỨC NĂNG ====================
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

            // Nút Xem: màu nhấn chính
            btnXem.BackColor = Ui.Accent;
            btnXem.ForeColor = Color.White;
            btnXem.FlatAppearance.BorderColor = Ui.Accent;
            btnXem.FlatAppearance.MouseOverBackColor = Ui.Dark;
            btnXem.FlatAppearance.MouseDownBackColor = Ui.Dark;

            // ==================== 3. BẢNG MÓN ĂN ====================
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
            dgv.DefaultCellStyle.Padding = new Padding(6, 4, 6, 4);
            dgv.DefaultCellStyle.WrapMode = DataGridViewTriState.True;

            // Hàng xen kẽ
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Ui.Back;
            dgv.AlternatingRowsDefaultCellStyle.ForeColor = Ui.Text;
            dgv.AlternatingRowsDefaultCellStyle.SelectionBackColor = Ui.Accent;
            dgv.AlternatingRowsDefaultCellStyle.SelectionForeColor = Color.White;

            // Chiều cao hàng linh hoạt vì có cột Nguyên liệu, Cách làm
            dgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            // ==================== 4. TIÊU ĐỀ BÁO CÁO ====================
            lblTieuDeBC.ForeColor = Ui.Dark;
            lblTieuDeBC.Font = new Font("Segoe UI", 12F, FontStyle.Bold);

            // ==================== 5. THỐNG KÊ SỐ MÓN ====================
            lblSoDong.ForeColor = Ui.Accent;
            lblSoDong.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        }
    }
}