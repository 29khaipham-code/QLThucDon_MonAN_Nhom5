
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using QLThucDon_MonAN_Nhom5.BLL;
using QLThucDon_MonAN_Nhom5.GUI;
using QLThucDon_MonAN_Nhom5.GUI.Forms.BaoCao;
namespace QLThucDon_MonAN_Nhom5.GUI.Forms.BaoCao
{
    /// <summary>
    /// Báo cáo thông tin đầy đủ các thực đơn.
    /// Lọc theo một thực đơn hoặc xem tất cả.
    /// Mỗi dòng của lưới là một món trong thực đơn.
    ///
    /// Các control cần có trong Designer:
    /// lblLoc, cmbLoc, btnXem, btnXuat, btnIn,
    /// lblTieuDeBC, dgv, lblSoDong.
    /// </summary>
    public partial class frmBaoCaoThucDon : Form
    {
        private readonly BaoCaoBLL _bll = new BaoCaoBLL();
        private readonly ThucDonBLL _thucDonBLL = new ThucDonBLL();

        private DataTable _dt;

        private string _tieuDe = "THÔNG TIN CÁC THỰC ĐƠN";

        // Tên cột tương ứng với dữ liệu từ BaoCaoDAL.ThucDonDayDu.
        private static readonly CotBaoCao[] Cot =
        {
            new CotBaoCao("SoThucDon", "Số TĐ", 0.8f, rong: 80),
            new CotBaoCao("NgayDung", "Ngày dùng", 1.0f,
                dinhDang: "dd/MM/yyyy", rong: 90),
            new CotBaoCao("TenKhachHang", "Khách hàng", 1.5f, rong: 130),
            new CotBaoCao("MaSoThue", "MST", 1.0f, rong: 90),
            new CotBaoCao("TenMonAn", "Tên món", 1.8f),
            new CotBaoCao("TenDauBep", "Đầu bếp", 1.3f, rong: 110),
            new CotBaoCao("SoLuong", "SL", 0.5f,
                phai: true, dinhDang: "N0", rong: 50),
            new CotBaoCao("DonGia", "Đơn giá", 1.0f,
                phai: true, dinhDang: "N0", rong: 80),
            new CotBaoCao("ThanhTien", "Thành tiền", 1.1f,
                phai: true, dinhDang: "N0", rong: 90),
            new CotBaoCao("ThueVAT", "VAT (%)", 0.7f,
                phai: true, dinhDang: "0.##", rong: 60),
            new CotBaoCao("GiamGia", "Giảm giá", 1.0f,
                phai: true, dinhDang: "N0", rong: 80),
            new CotBaoCao("TongTien", "Tổng tiền", 1.1f,
                phai: true, dinhDang: "N0", rong: 90)
        };

        public frmBaoCaoThucDon()
        {
            InitializeComponent();

            ApDungGiaoDien();

            Text = "Báo cáo thông tin các thực đơn";
            lblLoc.Text = "Số thực đơn";

            Load += frmBaoCaoThucDon_Load;

            btnXem.Click += btnXem_Click;

            btnXuat.Click += (s, e) =>
                BaoCaoHelper.XuatExcel(
                    this,
                    _dt,
                    _tieuDe,
                    Cot,
                    "ThongTinThucDon");

            btnIn.Click += (s, e) =>
                BaoCaoHelper.In(
                    this,
                    _dt,
                    _tieuDe,
                    Cot);
        }

        // ===================== KHỞI TẠO FORM =====================

        private void frmBaoCaoThucDon_Load(object sender, EventArgs e)
        {
            try
            {
                BaoCaoHelper.CauHinhLuoi(dgv, Cot);

                // Nạp danh sách thực đơn vào bộ lọc.
                // Dòng đầu tiên là "Tất cả thực đơn".
                bool napThanhCong = Ui.Run(() =>
                {
                    var ds = _thucDonBLL.LayTatCa()
                        .Select(t => new MucLoc
                        {
                            Ma = t.SoThucDon,
                            Ten = string.Format(
                                "{0} - {1} ({2:dd/MM/yyyy})",
                                t.SoThucDon,
                                t.TenKhachHang,
                                t.NgayDung)
                        })
                        .ToList();

                    BaoCaoHelper.NapLoc(
                        cmbLoc,
                        "Tất cả thực đơn",
                        ds);
                });

                if (!napThanhCong)
                    return;

                // Gắn sự kiện sau khi nạp dữ liệu xong.
                cmbLoc.SelectedIndexChanged += cmbLoc_SelectedIndexChanged;

                Xem();
            }
            catch (BusinessException ex)
            {
                Ui.Info(ex.Message);
            }
            catch (Exception ex)
            {
                Ui.Error(
                    "Không thể khởi tạo báo cáo thực đơn:\n"
                    + ex.Message);
            }
        }

        // ===================== SỰ KIỆN LỌC =====================

        private void cmbLoc_SelectedIndexChanged(object sender, EventArgs e)
        {
            Xem();
        }

        private void btnXem_Click(object sender, EventArgs e)
        {
            Xem();

            // Không mở báo cáo nếu không có dữ liệu
            if (_dt == null || _dt.Rows.Count == 0)
                return;

            // Mở cửa sổ Crystal Report
            using (frmXemBaoCaoThucDon frm =
                new frmXemBaoCaoThucDon(_dt, _tieuDe))
            {
                frm.ShowDialog(this);
            }
        }

        // ===================== XEM BÁO CÁO =====================

        private void Xem()
        {
            _dt = null;
            string ma = BaoCaoHelper.LayMaLoc(cmbLoc);

            DataTable dt = null;

            bool thanhCong = Ui.Run(() =>
            {
                dt = _bll.ThucDonDayDu(ma);
            });

            if (!thanhCong)
                return;

            if (dt == null)
            {
                Ui.Error("Không nhận được dữ liệu báo cáo thực đơn.");
                return;
            }

            _dt = dt;

            _tieuDe = string.IsNullOrEmpty(ma)
                ? "THÔNG TIN CÁC THỰC ĐƠN"
                : "THÔNG TIN THỰC ĐƠN " + ma;

            dgv.DataSource = _dt;

            lblTieuDeBC.Text = _tieuDe;

            string soDong = "Số dòng: " + _dt.Rows.Count;

            // Nếu xem một thực đơn thì hiển thị tổng tiền.
            if (!string.IsNullOrEmpty(ma) && _dt.Rows.Count > 0)
            {
                if (_dt.Columns.Contains("TongTien")
                    && _dt.Rows[0]["TongTien"] != DBNull.Value)
                {
                    decimal tongTien =
                        Convert.ToDecimal(_dt.Rows[0]["TongTien"]);

                    soDong += "   |   Tổng tiền: " + tongTien.ToString("N0");
                }
            }

            lblSoDong.Text = soDong;
        }

        // ===================== GIAO DIỆN =====================

        private void ApDungGiaoDien()
        {
            // ==================== 1. FORM ====================

            BackColor = Ui.Back;
            ForeColor = Ui.Text;
            Font = new Font("Segoe UI", 9F);

            // ==================== 2. BỘ LỌC THỰC ĐƠN ====================

            lblLoc.ForeColor = Ui.Dark;

            lblLoc.Font =
                new Font("Segoe UI", 9F, FontStyle.Bold);

            cmbLoc.BackColor = Ui.Surface;
            cmbLoc.ForeColor = Ui.Text;
            cmbLoc.FlatStyle = FlatStyle.Flat;

            cmbLoc.Font =
                new Font("Segoe UI", 9F);

            // ==================== 3. CÁC NÚT CHỨC NĂNG ====================

            Button[] dsNut =
            {
                btnXem,
                btnXuat,
                btnIn
            };

            foreach (Button b in dsNut)
            {
                b.UseVisualStyleBackColor = false;
                b.FlatStyle = FlatStyle.Flat;
                b.FlatAppearance.BorderSize = 1;
                b.FlatAppearance.BorderColor = Ui.Border;

                b.BackColor = Ui.Surface;
                b.ForeColor = Ui.Dark;

                b.Font =
                    new Font("Segoe UI", 9F, FontStyle.Bold);

                b.FlatAppearance.MouseOverBackColor = Ui.Hover;
                b.FlatAppearance.MouseDownBackColor = Ui.Header;

                b.Cursor = Cursors.Hand;
            }

            // Nút Xem báo cáo: màu nhấn chính.

            btnXem.BackColor = Ui.Accent;
            btnXem.ForeColor = Color.White;
            btnXem.FlatAppearance.BorderColor = Ui.Accent;

            btnXem.FlatAppearance.MouseOverBackColor = Ui.Dark;
            btnXem.FlatAppearance.MouseDownBackColor = Ui.Dark;

            // ==================== 4. BẢNG THÔNG TIN THỰC ĐƠN ====================

            dgv.EnableHeadersVisualStyles = false;
            dgv.BackgroundColor = Ui.Surface;
            dgv.BorderStyle = BorderStyle.None;
            dgv.GridColor = Ui.Border;

            dgv.ReadOnly = true;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.AllowUserToResizeRows = false;

            dgv.MultiSelect = false;

            dgv.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            dgv.RowHeadersVisible = false;

            // Tiêu đề cột.

            dgv.ColumnHeadersDefaultCellStyle.BackColor = Ui.Header;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Ui.Dark;

            dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor =
                Ui.Header;

            dgv.ColumnHeadersDefaultCellStyle.SelectionForeColor =
                Ui.Dark;

            dgv.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 9F, FontStyle.Bold);

            dgv.ColumnHeadersDefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleLeft;

            dgv.ColumnHeadersDefaultCellStyle.Padding =
                new Padding(5, 0, 5, 0);

            dgv.ColumnHeadersHeight = 38;

            dgv.ColumnHeadersHeightSizeMode =
                DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            // Nội dung ô.

            dgv.DefaultCellStyle.BackColor = Ui.Surface;
            dgv.DefaultCellStyle.ForeColor = Ui.Text;
            dgv.DefaultCellStyle.SelectionBackColor = Ui.Accent;
            dgv.DefaultCellStyle.SelectionForeColor = Color.White;

            dgv.DefaultCellStyle.Font =
                new Font("Segoe UI", 9F);

            dgv.DefaultCellStyle.Padding =
                new Padding(5, 3, 5, 3);

            dgv.DefaultCellStyle.WrapMode =
                DataGridViewTriState.True;

            // Hàng xen kẽ.

            dgv.AlternatingRowsDefaultCellStyle.BackColor = Ui.Back;
            dgv.AlternatingRowsDefaultCellStyle.ForeColor = Ui.Text;

            dgv.AlternatingRowsDefaultCellStyle.SelectionBackColor =
                Ui.Accent;

            dgv.AlternatingRowsDefaultCellStyle.SelectionForeColor =
                Color.White;

            // Nội dung thực đơn có nhiều cột.
            dgv.AutoSizeRowsMode =
                DataGridViewAutoSizeRowsMode.AllCells;

            // ==================== 5. TIÊU ĐỀ BÁO CÁO ====================

            lblTieuDeBC.ForeColor = Ui.Dark;

            lblTieuDeBC.Font =
                new Font("Segoe UI", 12F, FontStyle.Bold);

            // ==================== 6. THỐNG KÊ SỐ DÒNG / TỔNG TIỀN ====================

            lblSoDong.ForeColor = Ui.Accent;

            lblSoDong.Font =
                new Font("Segoe UI", 10F, FontStyle.Bold);
        }
    }
}
