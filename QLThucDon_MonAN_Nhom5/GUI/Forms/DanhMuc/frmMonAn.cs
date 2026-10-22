
using Microsoft.Data.SqlClient;
using QLThucDon_MonAN_Nhom5.BLL;
using QLThucDon_MonAN_Nhom5.DAL;
using QLThucDon_MonAN_Nhom5.DTO;
//using QLThucDon_MonAN_Nhom5.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Windows.Forms;


namespace QLThucDon_MonAN_Nhom5.GUI.Forms.DanhMuc
{
    public partial class frmMonAn : Form
    {
        // =====================================================
        // ÁNH XẠ CONTROL TỪ DESIGNER
        // =====================================================

        private GroupBox grpThongTin => groupBox1;
        private GroupBox grpNguyenLieu => groupBox2;

        private TextBox txtMaMon => textBox1;
        private TextBox txtTenMon => textBox2;
        private ComboBox cboLoai => comboBox1;
        private ComboBox cboCongDung => comboBox2;
        private TextBox txtCachLam => textBox3;
        private TextBox txtYeuCau => textBox4;
        private TextBox txtDonGia => textBox5;

        private ComboBox cboNguyenLieu => comboBox3;
        private TextBox txtSoLuongNL => textBox6;

        private DataGridView dgvNguyenLieu => dataGridView1;
        private DataGridView dgvMonAn => dataGridView2;

        private Button btnThemNL => button1;
        private Button btnXoaNL => button2;
        private Button btnThem => button3;
        private Button btnSua => button4;
        private Button btnXoa => button5;
        private Button btnLuu => button6;

        // =====================================================
        // DỮ LIỆU VÀ TRẠNG THÁI NGHIỆP VỤ
        // =====================================================

        private enum CheDo
        {
            Them,
            Sua
        }

        private readonly MonAnDAL monAnDAL = new MonAnDAL();
        private readonly NguyenLieuBLL nguyenLieuBLL = new NguyenLieuBLL();
        private readonly DataTable dtNguyenLieu = new DataTable();

        private CheDo cheDo = CheDo.Sua;
        private bool dangTai = true;

        // =====================================================
        // CONSTRUCTOR
        // =====================================================

        public frmMonAn()
        {
            InitializeComponent();

            // Áp dụng giao diện trực tiếp theo Ui.cs
            ApDungGiaoDien();

            // Khởi tạo các cột và nguồn dữ liệu
            SetupGrids();

            // Đăng ký sự kiện
            Load += frmMonAn_Load;

            btnThemNL.Click -= btnThemNL_Click;
            btnThemNL.Click += btnThemNL_Click;

            btnXoaNL.Click -= btnXoaNL_Click;
            btnXoaNL.Click += btnXoaNL_Click;

            btnThem.Click -= btnThem_Click;
            btnThem.Click += btnThem_Click;

            btnSua.Click -= btnSua_Click;
            btnSua.Click += btnSua_Click;

            btnXoa.Click -= button5_Click;
            btnXoa.Click += button5_Click;

            btnLuu.Click -= btnLuu_Click;
            btnLuu.Click += btnLuu_Click;

            dgvMonAn.SelectionChanged -= dgvMonAn_SelectionChanged;
            dgvMonAn.SelectionChanged += dgvMonAn_SelectionChanged;

            txtSoLuongNL.KeyPress -= SoThapPhan_KeyPress;
            txtSoLuongNL.KeyPress += SoThapPhan_KeyPress;
        }

        // =====================================================
        // ÁP DỤNG GIAO DIỆN THEO Ui.cs
        // =====================================================

        private void ApDungGiaoDien()
        {
            // Thiết lập giao diện chung cho Form
            this.BackColor = Ui.Back;
            this.ForeColor = Ui.Text;
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Quản lý món ăn";

            // Đặt tiêu đề rõ ràng cho hai nhóm, không đổi Name trong Designer.
            grpThongTin.Text = "Thông tin món ăn";
            grpNguyenLieu.Text = "Nguyên liệu của món ăn";

            // Hai nhóm thông tin từ Designer (không thay đổi bố cục chính)
            foreach (GroupBox group in new[] { grpThongTin, grpNguyenLieu })
            {
                group.ForeColor = Ui.Dark;
                group.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
                group.BackColor = Ui.Back;

                foreach (Control control in group.Controls)
                {
                    if (control is Label nhan)
                    {
                        nhan.ForeColor = Ui.Text;
                        nhan.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
                        nhan.AutoSize = true; // Không bị cắt chữ như "Tên Mó", "Cách Là"
                    }
                    else if (control is TextBox)
                    {
                        control.BackColor = Ui.Surface;
                        control.ForeColor = Ui.Text;
                        ((TextBox)control).BorderStyle = BorderStyle.FixedSingle;
                    }
                    else if (control is ComboBox)
                    {
                        control.BackColor = Ui.Surface;
                        control.ForeColor = Ui.Text;
                    }
                }
            }

            // Tránh nhãn sau khi nở đủ chữ đè lên ô nhập bên cạnh.
            CanChinhOVaoTheoNhan(grpThongTin, txtTenMon);
            CanChinhOVaoTheoNhan(grpThongTin, txtCachLam);
            CanChinhOVaoTheoNhan(grpThongTin, txtYeuCau);

            txtMaMon.ReadOnly = true;
            txtDonGia.ReadOnly = true;
            cboLoai.DropDownStyle = ComboBoxStyle.DropDownList;
            cboCongDung.DropDownStyle = ComboBoxStyle.DropDownList;
            cboNguyenLieu.DropDownStyle = ComboBoxStyle.DropDownList;

            // Chỉ thay đổi kiểu dáng nút Designer, không tạo nút mới để giữ sự kiện Click
            DinhDangNut(btnThemNL, true);
            DinhDangNut(btnXoaNL, false);
            DinhDangNut(btnThem, false);
            DinhDangNut(btnSua, false);
            DinhDangNut(btnXoa, false);
            DinhDangNut(btnLuu, true);

            // Dùng đúng hàm có sẵn trong Ui.cs
            Ui.StyleGrid(dgvNguyenLieu);
            Ui.StyleGrid(dgvMonAn);

            foreach (DataGridView grid in new[] { dgvNguyenLieu, dgvMonAn })
            {
                grid.AutoGenerateColumns = false;
                grid.AllowUserToAddRows = false;
                grid.AllowUserToDeleteRows = false;
                grid.ReadOnly = true;
                grid.MultiSelect = false;
                grid.RowHeadersVisible = false;
                grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                grid.ColumnHeadersHeight = 34;
                grid.RowTemplate.Height = 28;
            }
        }

        // Nếu nhãn nằm ngay bên trái ô nhập bị cắt chữ, nới vị trí ô nhập khi cần.
        private static void CanChinhOVaoTheoNhan(GroupBox nhom, TextBox oNhap)
        {
            foreach (Control c in nhom.Controls)
            {
                Label nhan = c as Label;
                if (nhan == null) continue;
                bool cungHang = Math.Abs((nhan.Top + nhan.Height / 2) -
                                         (oNhap.Top + oNhap.Height / 2)) <= 16;
                if (cungHang && nhan.Left < oNhap.Left && nhan.Right + 8 > oNhap.Left)
                {
                    int viTriMoi = nhan.Right + 8;
                    if (viTriMoi + oNhap.Width <= nhom.ClientSize.Width - 8)
                        oNhap.Left = viTriMoi;
                }
            }
        }

        private void DinhDangNut(Button nut, bool chinh)
        {
            nut.FlatStyle = FlatStyle.Flat;
            nut.Cursor = Cursors.Hand;
            nut.UseVisualStyleBackColor = false;
            nut.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            nut.BackColor = chinh ? Ui.Accent : Ui.Surface;
            nut.ForeColor = chinh ? System.Drawing.Color.White : Ui.Dark;
            nut.FlatAppearance.BorderSize = 1;
            nut.FlatAppearance.BorderColor = chinh ? Ui.Accent : Ui.Border;
            nut.FlatAppearance.MouseOverBackColor = Ui.Hover;
            nut.FlatAppearance.MouseDownBackColor = Ui.Header;
        }

        // =====================================================
        // KHỞI TẠO DATAGRIDVIEW
        // =====================================================

        private static void ThemCot(
            DataGridView g,
            string prop,
            string header,
            float weight,
            string format)
        {
            var col = new DataGridViewTextBoxColumn
            {
                DataPropertyName = prop,
                HeaderText = header,
                Name = "col" + prop,
                FillWeight = weight,
                SortMode = DataGridViewColumnSortMode.NotSortable
            };

            if (format != null)
            {
                col.DefaultCellStyle.Format = format;
            }

            g.Columns.Add(col);
        }

        private void SetupGrids()
        {
            // Tránh tạo lại cột nếu hàm được gọi nhiều lần
            dgvNguyenLieu.AutoGenerateColumns = false;
            dgvNguyenLieu.Columns.Clear();

            dgvMonAn.AutoGenerateColumns = false;
            dgvMonAn.Columns.Clear();

            // Bảng nguyên liệu của món ăn
            if (!dtNguyenLieu.Columns.Contains("MaNL"))
                dtNguyenLieu.Columns.Add("MaNL", typeof(string));

            if (!dtNguyenLieu.Columns.Contains("TenNL"))
                dtNguyenLieu.Columns.Add("TenNL", typeof(string));

            if (!dtNguyenLieu.Columns.Contains("DVT"))
                dtNguyenLieu.Columns.Add("DVT", typeof(string));

            if (!dtNguyenLieu.Columns.Contains("SoLuong"))
                dtNguyenLieu.Columns.Add("SoLuong", typeof(decimal));

            if (!dtNguyenLieu.Columns.Contains("DonGia"))
                dtNguyenLieu.Columns.Add("DonGia", typeof(decimal));

            if (!dtNguyenLieu.Columns.Contains("ThanhTien"))
                dtNguyenLieu.Columns.Add("ThanhTien", typeof(decimal));

            ThemCot(
                dgvNguyenLieu,
                "MaNL",
                "Mã NL",
                12,
                null);

            ThemCot(
                dgvNguyenLieu,
                "TenNL",
                "Tên nguyên liệu",
                30,
                null);

            ThemCot(
                dgvNguyenLieu,
                "DVT",
                "ĐVT",
                10,
                null);

            ThemCot(
                dgvNguyenLieu,
                "SoLuong",
                "Số lượng",
                12,
                "0.###");

            ThemCot(
                dgvNguyenLieu,
                "DonGia",
                "Đơn giá",
                18,
                "N0");

            ThemCot(
                dgvNguyenLieu,
                "ThanhTien",
                "Thành tiền",
                18,
                "N0");

            dgvNguyenLieu.DataSource = dtNguyenLieu;

            // Bảng danh sách món ăn
            ThemCot(
                dgvMonAn,
                "MaMonAn",
                "Mã món",
                12,
                null);

            ThemCot(
                dgvMonAn,
                "TenMonAn",
                "Tên món",
                30,
                null);

            ThemCot(
                dgvMonAn,
                "TenLoai",
                "Loại",
                20,
                null);

            ThemCot(
                dgvMonAn,
                "TenCongDung",
                "Công dụng",
                22,
                null);

            ThemCot(
                dgvMonAn,
                "DonGia",
                "Đơn giá",
                16,
                "N0");
        }

        // =====================================================
        // LOAD DỮ LIỆU BAN ĐẦU
        // =====================================================

        private void frmMonAn_Load(object sender, EventArgs e)
        {
            try
            {
                dangTai = true;

                // 1. Load loại món
                cboLoai.DataSource = monAnDAL.GetLoaiMon();
                cboLoai.DisplayMember = "TenLoai";
                cboLoai.ValueMember = "MaLoai";

                // 2. Load công dụng
                DataTable dtCD = monAnDAL.GetCongDung();

                DataRow trong = dtCD.NewRow();
                trong["MaCongDung"] = "";
                trong["TenCongDung"] = "(Không)";
                dtCD.Rows.InsertAt(trong, 0);

                cboCongDung.DataSource = dtCD;
                cboCongDung.DisplayMember = "TenCongDung";
                cboCongDung.ValueMember = "MaCongDung";

                // 3. Load nguyên liệu
                cboNguyenLieu.DataSource = nguyenLieuBLL.LayTatCa();
                cboNguyenLieu.DisplayMember = "TenNguyenLieu";
                cboNguyenLieu.ValueMember = "MaNguyenLieu";

                dangTai = false;

                // 4. Load danh sách món ăn
                LoadDanhSach(null);
            }
            catch (Exception ex)
            {
                dangTai = false;

                MessageBox.Show(
                    "Lỗi load dữ liệu:\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =====================================================
        // LOAD DANH SÁCH MÓN ĂN
        // =====================================================

        private void LoadDanhSach(string chonMa)
        {
            try
            {
                dangTai = true;
                dgvMonAn.DataSource = monAnDAL.GetAll();
                dangTai = false;

                bool timThay = false;

                if (chonMa != null)
                {
                    foreach (DataGridViewRow r in dgvMonAn.Rows)
                    {
                        var m = r.DataBoundItem as MonAnDTO;

                        if (m != null && m.MaMonAn == chonMa)
                        {
                            if (r.Cells.Count > 0)
                            {
                                dgvMonAn.CurrentCell = r.Cells[0];
                            }

                            timThay = true;
                            break;
                        }
                    }
                }

                if (!timThay)
                {
                    var hienTai = dgvMonAn.CurrentRow != null
                        ? dgvMonAn.CurrentRow.DataBoundItem as MonAnDTO
                        : null;

                    if (hienTai != null)
                    {
                        SetCheDo(CheDo.Sua);
                        HienThiMon(hienTai);
                    }
                    else
                    {
                        XoaForm();
                        SetCheDo(CheDo.Them);

                        try
                        {
                            txtMaMon.Text = monAnDAL.NextCode();
                        }
                        catch
                        {
                            // Không làm dừng form nếu chưa tạo được mã
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                dangTai = false;

                MessageBox.Show(
                    "Lỗi tải danh sách món ăn:\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void dgvMonAn_SelectionChanged(object sender, EventArgs e)
        {
            if (dangTai)
                return;

            var m = dgvMonAn.CurrentRow != null
                ? dgvMonAn.CurrentRow.DataBoundItem as MonAnDTO
                : null;

            if (m == null)
                return;

            SetCheDo(CheDo.Sua);
            HienThiMon(m);
        }

        // =====================================================
        // HIỂN THỊ VÀ CHUYỂN CHẾ ĐỘ
        // =====================================================

        private void SetCheDo(CheDo cd)
        {
            cheDo = cd;

            this.Text = "frmMonAn - Quản lý món ăn" +
                (cd == CheDo.Them
                    ? "     |     Đang thêm món mới"
                    : "");
        }

        private void HienThiMon(MonAnDTO m)
        {
            txtMaMon.Text = m.MaMonAn;
            txtTenMon.Text = m.TenMonAn;

            if (m.MaLoai != null)
                cboLoai.SelectedValue = m.MaLoai;

            if (m.MaCongDung != null)
                cboCongDung.SelectedValue = m.MaCongDung;
            else if (cboCongDung.Items.Count > 0)
                cboCongDung.SelectedIndex = 0;

            txtCachLam.Text = m.CachLam;
            txtYeuCau.Text = m.YeuCau;
            txtDonGia.Text = m.DonGia.ToString("N0");

            dtNguyenLieu.Clear();

            try
            {
                foreach (var x in monAnDAL.GetNguyenLieu(m.MaMonAn))
                {
                    dtNguyenLieu.Rows.Add(
                        x.MaNguyenLieu,
                        x.TenNguyenLieu,
                        x.TenDonViTinh,
                        x.SoLuong,
                        x.DonGia,
                        x.ThanhTien);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi lấy nguyên liệu của món:\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void XoaForm()
        {
            txtMaMon.Clear();
            txtTenMon.Clear();
            txtCachLam.Clear();
            txtYeuCau.Clear();
            txtDonGia.Text = "0";
            txtSoLuongNL.Clear();

            dtNguyenLieu.Clear();

            if (cboLoai.Items.Count > 0)
                cboLoai.SelectedIndex = 0;

            if (cboCongDung.Items.Count > 0)
                cboCongDung.SelectedIndex = 0;
        }

        // =====================================================
        // TÍNH ĐƠN GIÁ TẠM TÍNH
        // =====================================================

        private void TinhDonGiaTamTinh()
        {
            decimal tong = 0;

            foreach (DataRow r in dtNguyenLieu.Rows)
            {
                tong += Convert.ToDecimal(r["ThanhTien"]);
            }

            txtDonGia.Text = tong.ToString("N0");
        }

        private void SoThapPhan_KeyPress(
            object sender,
            KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar)
                && !char.IsDigit(e.KeyChar)
                && e.KeyChar != '.'
                && e.KeyChar != ',')
            {
                e.Handled = true;
            }
        }

        // =====================================================
        // THÊM NGUYÊN LIỆU VÀO MÓN
        // =====================================================

        private void btnThemNL_Click(object sender, EventArgs e)
        {
            var nl = cboNguyenLieu.SelectedItem as NguyenLieuDTO;

            if (nl == null)
            {
                MessageBox.Show(
                    "Vui lòng chọn nguyên liệu!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            decimal sl;

            string s = txtSoLuongNL.Text
                .Trim()
                .Replace(',', '.');

            if (!decimal.TryParse(
                    s,
                    NumberStyles.Number,
                    CultureInfo.InvariantCulture,
                    out sl)
                || sl <= 0)
            {
                MessageBox.Show(
                    "Số lượng phải là số lớn hơn 0!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtSoLuongNL.Focus();
                return;
            }

            DataRow dong = null;

            foreach (DataRow r in dtNguyenLieu.Rows)
            {
                if ((string)r["MaNL"] == nl.MaNguyenLieu)
                {
                    dong = r;
                    break;
                }
            }

            if (dong != null)
            {
                decimal moi =
                    Convert.ToDecimal(dong["SoLuong"]) + sl;

                dong["SoLuong"] = moi;

                dong["ThanhTien"] =
                    moi * Convert.ToDecimal(dong["DonGia"]);
            }
            else
            {
                dtNguyenLieu.Rows.Add(
                    nl.MaNguyenLieu,
                    nl.TenNguyenLieu,
                    nl.TenDonViTinh,
                    sl,
                    nl.DonGia,
                    nl.DonGia * sl);
            }

            txtSoLuongNL.Clear();
            TinhDonGiaTamTinh();
        }

        // =====================================================
        // XÓA NGUYÊN LIỆU KHỎI MÓN
        // =====================================================

        private void btnXoaNL_Click(object sender, EventArgs e)
        {
            var drv = dgvNguyenLieu.CurrentRow != null
                ? dgvNguyenLieu.CurrentRow.DataBoundItem as DataRowView
                : null;

            if (drv == null)
            {
                MessageBox.Show(
                    "Vui lòng chọn dòng nguyên liệu cần xoá!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            dtNguyenLieu.Rows.Remove(drv.Row);
            TinhDonGiaTamTinh();
        }

        // =====================================================
        // THÊM MÓN ĂN MỚI
        // =====================================================

        private void btnThem_Click(object sender, EventArgs e)
        {
            try
            {
                dangTai = true;

                dgvMonAn.ClearSelection();
                dgvMonAn.CurrentCell = null;

                dangTai = false;

                XoaForm();

                txtMaMon.Text = monAnDAL.NextCode();

                SetCheDo(CheDo.Them);
                txtTenMon.Focus();
            }
            catch (Exception ex)
            {
                dangTai = false;

                MessageBox.Show(
                    "Lỗi tạo mã món:\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =====================================================
        // SỬA MÓN ĂN
        // =====================================================

        private void btnSua_Click(object sender, EventArgs e)
        {
            var m = dgvMonAn.CurrentRow != null
                ? dgvMonAn.CurrentRow.DataBoundItem as MonAnDTO
                : null;

            if (m == null)
            {
                MessageBox.Show(
                    "Vui lòng chọn món cần sửa trong danh sách!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            SetCheDo(CheDo.Sua);

            txtTenMon.Focus();
            txtTenMon.SelectAll();
        }

        // =====================================================
        // LƯU MÓN ĂN
        // =====================================================

        private void btnLuu_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaMon.Text))
            {
                MessageBox.Show(
                    "Chưa có mã món. Hãy bấm Thêm để tạo món mới!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            string ten = txtTenMon.Text.Trim();

            if (ten.Length == 0)
            {
                MessageBox.Show(
                    "Vui lòng nhập tên món!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtTenMon.Focus();
                return;
            }

            if (cboLoai.SelectedValue == null)
            {
                MessageBox.Show(
                    "Vui lòng chọn loại món!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            var mon = new MonAnDTO
            {
                MaMonAn = txtMaMon.Text.Trim(),
                TenMonAn = ten,
                MaLoai = cboLoai.SelectedValue.ToString(),

                MaCongDung = cboCongDung.SelectedValue != null
                    ? cboCongDung.SelectedValue.ToString()
                    : "",

                CachLam = txtCachLam.Text.Trim(),
                YeuCau = txtYeuCau.Text.Trim()
            };

            var dsNL = new List<NguyenLieuMonAnDTO>();

            foreach (DataRow r in dtNguyenLieu.Rows)
            {
                dsNL.Add(new NguyenLieuMonAnDTO
                {
                    MaMonAn = mon.MaMonAn,
                    MaNguyenLieu = Convert.ToString(r["MaNL"]),
                    SoLuong = Convert.ToDecimal(r["SoLuong"])
                });
            }

            try
            {
                if (cheDo == CheDo.Them)
                {
                    monAnDAL.Insert(mon, dsNL);
                }
                else
                {
                    monAnDAL.Update(mon, dsNL);
                }

                MessageBox.Show(
                    "Lưu món ăn thành công!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                // Tải lại dữ liệu để lấy đơn giá mới từ cơ sở dữ liệu
                LoadDanhSach(mon.MaMonAn);
            }
            catch (SqlException ex)
                when (ex.Number == 2627 || ex.Number == 2601)
            {
                MessageBox.Show(
                    "Mã món \"" + mon.MaMonAn + "\" đã tồn tại!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi khi lưu:\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =====================================================
        // XÓA MÓN ĂN
        // =====================================================

        private void button5_Click(object sender, EventArgs e)
        {
            var m = dgvMonAn.CurrentRow != null
                ? dgvMonAn.CurrentRow.DataBoundItem as MonAnDTO
                : null;

            if (m == null)
            {
                MessageBox.Show(
                    "Vui lòng chọn món cần xoá trong danh sách!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            DialogResult xacNhan = MessageBox.Show(
                "Xoá món \"" + m.TenMonAn + "\"?",
                "Xác nhận",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (xacNhan != DialogResult.Yes)
                return;

            try
            {
                // Xóa nguyên liệu liên quan và món ăn trong một transaction
                DbHelper.RunInTransaction((c, t) =>
                {
                    DbHelper.ExecuteNonQuery(
                        c,
                        t,
                        "DELETE FROM NguyenLieu_MonAn WHERE MaMonAn=@Ma",
                        DbHelper.P("@Ma", m.MaMonAn));

                    DbHelper.ExecuteNonQuery(
                        c,
                        t,
                        "DELETE FROM MonAn WHERE MaMonAn=@Ma",
                        DbHelper.P("@Ma", m.MaMonAn));
                });

                MessageBox.Show(
                    "Đã xoá món ăn!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                LoadDanhSach(null);
            }
            catch (SqlException ex) when (ex.Number == 547)
            {
                MessageBox.Show(
                    "Không xoá được: món này đang có đầu bếp làm hoặc nằm trong thực đơn.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi khi xoá:\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }
    }
}
