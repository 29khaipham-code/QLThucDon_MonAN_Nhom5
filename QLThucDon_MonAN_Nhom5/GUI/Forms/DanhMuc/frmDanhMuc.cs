using QLThucDon_MonAN_Nhom5.BLL;
using QLThucDon_MonAN_Nhom5.DAL;
using QLThucDon_MonAN_Nhom5.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace QLThucDon_MonAN_Nhom5.GUI.Forms.DanhMuc
{
    public partial class frmDanhMuc : Form
    {

        /// Một form dùng chung cho 6 danh mục: loại món, công dụng, đơn vị tính, nơi học, trình độ, xếp loại
        /// Cách mở: new frmDanhMuc(LoaiDanhMuc.LoaiMon)private readonly LoaiDanhMuc _loai;
        
        private readonly LoaiDanhMuc _loai;
        private readonly DanhMucBLL _bll = new DanhMucBLL();
        private bool _dangNap;   // true khi đang nạp lưới: bỏ qua sự kiện SelectionChanged

        private static readonly Dictionary<LoaiDanhMuc, string> TenHienThi = new Dictionary<LoaiDanhMuc, string>()
        {
            [LoaiDanhMuc.LoaiMon] = "loại món",
            [LoaiDanhMuc.CongDung] = "công dụng",
            [LoaiDanhMuc.DonViTinh] = "đơn vị tính",
            [LoaiDanhMuc.NoiHoc] = "nơi học",
            [LoaiDanhMuc.TrinhDo] = "trình độ",
            [LoaiDanhMuc.XepLoai] = "xếp loại",
        };


        //Constructor không tham số để Visual Studio Designer mở được form; chạy thật luôn dùng constructor bên dưới
        public frmDanhMuc() : this(LoaiDanhMuc.LoaiMon) { }

        public frmDanhMuc(LoaiDanhMuc loai)
        {
            InitializeComponent();
            ApDungGiaoDien();
            _loai = loai;//lay kieu danh muc dc truyen (dc goi)

            // Tiêu đề và nhãn đổi theo loại danh mục
            string ten = TenHienThi[loai];
            Text = "Danh mục " + ten;
            lblTen.Text = "Tên " + ten;
            txtMa.ReadOnly = true;
            txtTen.MaxLength = 100;

            Load += frmDanhMuc_Load;
            dgv.SelectionChanged += dgv_SelectionChanged;
            dgv.KeyDown += dgv_KeyDown;
            txtTen.KeyDown += txtTen_KeyDown;
            btnThem.Click += btnThem_Click;
            btnSua.Click += btnSua_Click;
            btnXoa.Click += btnXoa_Click;
            btnMoi.Click += btnMoi_Click;
        }

        private void frmDanhMuc_Load(object sender, EventArgs e)
        {
            CauHinhLuoi();
            TaiDuLieu();
        }

        /// Gắn cột lưới với DanhMucDTO (cột 1 = Ma, cột 2 = Ten)
        private void CauHinhLuoi()
        {
            dgv.AutoGenerateColumns = false;
            dgv.ReadOnly = true;

            if (dgv.Columns.Count == 0)
            {
                dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Ma", Width = 120 });
                dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Ten", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            }
            dgv.Columns[0].DataPropertyName = nameof(DanhMucDTO.Ma);
            dgv.Columns[0].HeaderText = "Mã";
            dgv.Columns[1].DataPropertyName = nameof(DanhMucDTO.Ten);
            dgv.Columns[1].HeaderText = "Tên " + TenHienThi[_loai];
        }

        
        ///Nạp lại lưới; nếu truyền chonMa thì chọn sẵn dòng đó, ngược lại về trạng thái nhập mới
        private void TaiDuLieu(string chonMa = null)
        {
            List<DanhMucDTO> ds = null;
            if (!Ui.Run(() => ds = _bll.LayTatCa(_loai))) return;

            _dangNap = true;
            dgv.DataSource = ds;
            _dangNap = false;

            if (chonMa != null && ChonDong(chonMa)) return;
            LamMoiO();
        }

        private bool ChonDong(string ma)
        {
            foreach (DataGridViewRow r in dgv.Rows)
            {
                if (r.DataBoundItem is DanhMucDTO d && d.Ma == ma)
                {
                    dgv.CurrentCell = r.Cells[0];
                    r.Selected = true;
                    if (r.Index >= 0) dgv.FirstDisplayedScrollingRowIndex = Math.Max(0, Math.Min(r.Index, dgv.Rows.Count - 1));
                    HienDongChon();
                    return true;
                }
            }
            return false;
        }

        // ===================== HIỂN THỊ & TRẠNG THÁI NÚT =====================
        private void dgv_SelectionChanged(object sender, EventArgs e)
        {
            if (_dangNap) return;
            HienDongChon();
        }

        /// >Đổ dòng đang chọn lên ô nhập; không có dòng nào thì xoá trắng.
        private void HienDongChon()
        {
            if (dgv.CurrentRow?.DataBoundItem is DanhMucDTO d && dgv.CurrentRow.Selected)
            {
                txtMa.Text = d.Ma;
                txtTen.Text = d.Ten;
            }
            else
            {
                txtMa.Clear();
                txtTen.Clear();
            }
            CapNhatNut();
        }

        /// <summary>Chưa chọn dòng (nhập mới): chỉ bật Thêm. Đã chọn dòng: bật Sửa và Xoá.</summary>
        private void CapNhatNut()
        {
            bool daChon = !string.IsNullOrEmpty(txtMa.Text);
            btnThem.Enabled = !daChon;
            btnSua.Enabled = daChon;
            btnXoa.Enabled = daChon;
        }

        /// <summary>Về trạng thái nhập mới: bỏ chọn lưới, xoá trắng ô, đưa con trỏ vào ô tên.</summary>
        private void LamMoiO()
        {
            _dangNap = true;
            dgv.CurrentCell = null;
            dgv.ClearSelection();
            _dangNap = false;

            txtMa.Clear();
            txtTen.Clear();
            CapNhatNut();
            txtTen.Focus();
        }

        // ===================== NÚT CHỨC NĂNG =====================
        private void btnThem_Click(object sender, EventArgs e)
        {
            var dto = new DanhMucDTO { Ma = "", Ten = txtTen.Text };   // mã để trống: BLL tự sinh
            if (!Ui.Run(() => _bll.Them(_loai, dto)))
            {
                txtTen.Focus();
                return;
            }
            TaiDuLieu(dto.Ma);   // nạp lại và cuộn tới dòng vừa thêm
            Ui.Info("Đã thêm thành công.");
            LamMoiO();           // sẵn sàng nhập tiếp
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMa.Text))
            {
                Ui.Error("Hãy chọn một dòng trong danh sách để sửa.");
                return;
            }
            var dto = new DanhMucDTO { Ma = txtMa.Text, Ten = txtTen.Text };
            if (!Ui.Run(() => _bll.Sua(_loai, dto))) return;

            TaiDuLieu(dto.Ma);
            Ui.Info("Đã cập nhật.");
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMa.Text))
            {
                Ui.Error("Hãy chọn một dòng trong danh sách để xoá.");
                return;
            }
            if (!Ui.Confirm($"Xoá \"{txtTen.Text}\"?")) return;

            // Nếu danh mục đang được dùng (vd loại món đã có món ăn) thì BLL báo lỗi thân thiện
            if (!Ui.Run(() => _bll.Xoa(_loai, txtMa.Text))) return;

            TaiDuLieu();
            Ui.Info("Đã xoá.");
        }

        private void btnMoi_Click(object sender, EventArgs e) => TaiDuLieu();

        // ===================== PHÍM TẮT =====================
        /// <summary>Enter trong ô tên: Thêm (nhập mới) hoặc Sửa (đang chọn dòng). Esc: làm mới.</summary>
        private void txtTen_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                (string.IsNullOrEmpty(txtMa.Text) ? btnThem : btnSua).PerformClick();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                e.SuppressKeyPress = true;
                LamMoiO();
            }
        }

        /// <summary>Phím Delete trên lưới: xoá dòng đang chọn (có hỏi xác nhận).</summary>
        private void dgv_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && btnXoa.Enabled)
            {
                e.Handled = true;
                btnXoa.PerformClick();
            }
        }

        public void ApDungGiaoDien()
        {
            BackColor = Ui.Back;
            ForeColor = Ui.Text;

            

            foreach (Control c in flowLayout.Controls)
            {
                if (!(c is Button b))
                    continue;

                b.UseVisualStyleBackColor = false;
                b.FlatStyle = FlatStyle.Flat;
                b.FlatAppearance.BorderSize = 1;

                // Nút chính: nền Teal, chữ trắng
                // Nút phụ: nền trắng, chữ Teal đậm
                bool nutChinh = b == btnThem;

                b.BackColor = nutChinh ? Ui.Accent : Ui.Surface;
                b.ForeColor = nutChinh ? Color.White : Ui.Dark;

                b.FlatAppearance.BorderColor = b.Enabled ? Ui.Accent: Ui.Border;

                // Màu khi rê chuột
                b.FlatAppearance.MouseOverBackColor = Ui.Hover;

                // Màu khi nhấn
                b.FlatAppearance.MouseDownBackColor = Ui.Header;

                b.Cursor = Cursors.Hand;


            }
            dgv.EnableHeadersVisualStyles = false;
            dgv.BackgroundColor = Ui.Surface;
            dgv.BorderStyle = BorderStyle.None;
            dgv.GridColor = Ui.Border;

            // Tiêu đề cột
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Ui.Header;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Ui.Dark;
            dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = Ui.Header;
            dgv.ColumnHeadersDefaultCellStyle.SelectionForeColor = Ui.Dark;
            dgv.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 9F, FontStyle.Bold);

            // Các ô dữ liệu bình thường
            dgv.DefaultCellStyle.BackColor = Ui.Surface;
            dgv.DefaultCellStyle.ForeColor = Ui.Text;

            // Khi chọn dòng: nền Teal, chữ trắng
            dgv.DefaultCellStyle.SelectionBackColor = Ui.Accent;
            dgv.DefaultCellStyle.SelectionForeColor = Color.White;

            // Dòng xen kẽ
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Ui.Back;

            // Không để màu chọn của dòng xen kẽ ghi đè màu chọn chung
            dgv.AlternatingRowsDefaultCellStyle.SelectionBackColor = Ui.Accent;
            dgv.AlternatingRowsDefaultCellStyle.SelectionForeColor = Color.White;

            // Chiều cao và kiểu tiêu đề
            dgv.ColumnHeadersHeight = 36;
            dgv.ColumnHeadersHeightSizeMode =
                DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            dgv.RowTemplate.Height = 32;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false;


        }

    }
}
