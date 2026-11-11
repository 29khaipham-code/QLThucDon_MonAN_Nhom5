using Microsoft.Data.SqlClient;
using QLThucDon_MonAN_Nhom5.BLL;
using QLThucDon_MonAN_Nhom5.DTO;
using QLThucDon_MonAN_Nhom5.GUI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace QLThucDon_MonAN_Nhom5.GUI.Forms.DanhMuc
{
    public partial class frmDauBep : BaseForm
    {
        // BLL đã làm việc thông qua IDauBepDAL; form không truy cập DAL trực tiếp.
        private readonly DauBepBLL _bll;
        private bool _dangThem;

        // Constructor không tham số được giữ lại để WinForms Designer và frmMain sử dụng.
        public frmDauBep() : this(new DauBepBLL())
        {
        }

        // Constructor này cho phép truyền BLL khi cần kiểm thử hoặc thay thế tầng dữ liệu.
        public frmDauBep(DauBepBLL bll)
        {
            _bll = bll ?? throw new ArgumentNullException(nameof(bll));

            InitializeComponent();

            // Tái sử dụng giao diện chung từ BaseForm.
            DinhDangForm("Quản lý đầu bếp");
            DinhDangODuLieu(this);

            DinhDangNutBam(button1, true);
            DinhDangNutBam(button2, false);
            DinhDangNutBam(button3, true);
            DinhDangNutBam(button4, false);
            DinhDangNutBam(button5, false);
            DinhDangNutBam(button6, true);

            // Bảng danh sách đầu bếp chỉ dùng để xem/chọn.
            DinhDangLuoi(dataGridView2);
            // Bảng món làm được cho phép chỉnh sửa xếp loại ngay trên lưới.
            DinhDangLuoi(dataGridView1);
            dataGridView1.ReadOnly = false;
            dataGridView1.AllowUserToAddRows = false;

            Load += QuanLyDauBep_Load;
            dataGridView2.SelectionChanged += dataGridView2_SelectionChanged;
            button3.Click += button3_Click;
            button4.Click += button4_Click;
            button5.Click += button5_Click;
            button6.Click += button6_Click;
            button1.Click += button1_Click;
            button2.Click += button2_Click;

            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView2.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView2.MultiSelect = false;
            dataGridView2.ReadOnly = true;
            dataGridView2.AllowUserToAddRows = false;
        }

        private void QuanLyDauBep_Load(object sender, EventArgs e)
        {
            try
            {
                NapDanhMuc();
                NapDanhSach();
                CheDoXem();
            }
            catch (Exception ex) { BaoLoi("Không tải được dữ liệu. Hãy kiểm tra SQL Server, database QuanLyDauBep và connection string.", ex); }
        }

        private void NapDanhMuc()
        {
            comboBox1.DataSource = _bll.LayTrinhDo();
            comboBox1.DisplayMember = nameof(DanhMucDTO.Ten);
            comboBox1.ValueMember = nameof(DanhMucDTO.Ma);

            comboBox2.DataSource = _bll.LayNoiHoc();
            comboBox2.DisplayMember = nameof(DanhMucDTO.Ten);
            comboBox2.ValueMember = nameof(DanhMucDTO.Ma);

            comboBox3.DataSource = _bll.LayDanhSachMonAn();
            comboBox3.DisplayMember = nameof(MonAnDTO.TenMonAn);
            comboBox3.ValueMember = nameof(MonAnDTO.MaMonAn);

            comboBox4.DataSource = _bll.LayXepLoai();
            comboBox4.DisplayMember = nameof(DanhMucDTO.Ten);
            comboBox4.ValueMember = nameof(DanhMucDTO.Ma);
            comboBox1.DropDownStyle = comboBox2.DropDownStyle = comboBox3.DropDownStyle = comboBox4.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void NapDanhSach()
        {
            dataGridView2.AutoGenerateColumns = false;
            dataGridView2.DataSource = _bll.LayTatCa();
            GanCot(colMa, "MaDauBep"); GanCot(colTendaubep, "TenDauBep");
            GanCot(colGioiTinh, "GioiTinh"); GanCot(colTrinhDo, "TenTrinhDo"); GanCot(colNoiHoc, "TenNoiHoc");
            if (dataGridView2.Rows.Count > 0) dataGridView2.Rows[0].Selected = true;
        }

        private static void GanCot(DataGridViewColumn c, string property)
        {
            if (c != null) c.DataPropertyName = property;
        }

        private void dataGridView2_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView2.CurrentRow == null || dataGridView2.CurrentRow.DataBoundItem == null) return;
            var d = dataGridView2.CurrentRow.DataBoundItem as DauBepDTO;
            if (d == null) return;
            textBox1.Text = d.MaDauBep; textBox2.Text = d.TenDauBep ?? "";
            textBox4.Text = d.DiaChi ?? ""; textBox3.Text = d.DienThoai ?? "";
            radioButton1.Checked = string.Equals(d.GioiTinh, "Nam", StringComparison.OrdinalIgnoreCase);
            radioButton2.Checked = !radioButton1.Checked;
            ChonGiaTri(comboBox1, d.MaTrinhDo); ChonGiaTri(comboBox2, d.MaNoiHoc);
            NapMonAnDaChon(d.MaDauBep);
        }

        private static void ChonGiaTri(ComboBox cb, string value)
        {
            if (cb.DataSource == null) return;
            cb.SelectedValue = value ?? "";
            if (cb.SelectedIndex < 0) cb.SelectedIndex = -1;
        }

        private void NapMonAnDaChon(string ma)
        {
            var list = _bll.LayMonLamDuoc(ma);
            var dt = new DataTable();
            dt.Columns.Add("MaMonAn"); dt.Columns.Add("TenMonAn");
            dt.Columns.Add("MaXepLoai"); dt.Columns.Add("TenXepLoai");
            foreach (var x in list) dt.Rows.Add(x.MaMonAn, x.TenMonAn, x.MaXepLoai, x.TenXepLoai);
            GanBangMonAn(dt);
        }

        private List<DauBepMonAnDTO> LayMonAnTrenGrid()
        {
            var list = new List<DauBepMonAnDTO>();
            foreach (DataGridViewRow r in dataGridView1.Rows)
            {
                if (r.IsNewRow) continue;
                string maMon = Convert.ToString(r.Cells["colMaMon"].Value)?.Trim();
                string maXepLoai = Convert.ToString(r.Cells["colXeploai"].Value)?.Trim();
                if (string.IsNullOrWhiteSpace(maMon)) continue;
                if (string.IsNullOrWhiteSpace(maXepLoai))
                    throw new InvalidOperationException("Món " + maMon + " chưa có xếp loại.");
                if (list.Any(x => x.MaMonAn == maMon))
                    throw new InvalidOperationException("Món " + maMon + " bị thêm trùng.");
                list.Add(new DauBepMonAnDTO { MaMonAn = maMon, MaXepLoai = maXepLoai });
            }
            return list;
        }

        private DataTable TaoBangMonAnHienTai()
        {
            var dt = new DataTable();
            dt.Columns.Add("MaMonAn");
            dt.Columns.Add("TenMonAn");
            dt.Columns.Add("MaXepLoai");
            dt.Columns.Add("TenXepLoai");
            foreach (DataGridViewRow r in dataGridView1.Rows)
            {
                if (r.IsNewRow) continue;
                string ma = Convert.ToString(r.Cells["colMaMon"].Value);
                if (string.IsNullOrWhiteSpace(ma)) continue;
                string ten = Convert.ToString(r.Cells["colTenmon"].Value);
                string xl = Convert.ToString(r.Cells["colXeploai"].Value);
                string tenXl = "";
                var selected = comboBox4.Items.Cast<DanhMucDTO>()
                    .FirstOrDefault(x => x.Ma == xl);
                if (selected != null) tenXl = selected.Ten;
                dt.Rows.Add(ma, ten ?? "", xl ?? "", tenXl);
            }
            return dt;
        }

        private void GanBangMonAn(DataTable dt)
        {
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.DataSource = dt;
            GanCot(colMaMon, "MaMonAn");
            GanCot(colTenmon, "TenMonAn");
            GanCot(colXeploai, "MaXepLoai");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBox3.SelectedValue == null || comboBox4.SelectedValue == null)
                { MessageBox.Show("Hãy chọn món ăn và xếp loại."); return; }
                string maMon = Convert.ToString(comboBox3.SelectedValue);
                if (LayMonAnTrenGrid().Any(x => x.MaMonAn == maMon))
                { MessageBox.Show("Món này đã có trong danh sách."); return; }

                var bang = TaoBangMonAnHienTai();
                var mon = comboBox3.SelectedItem as MonAnDTO;
                var xl = comboBox4.SelectedItem as DanhMucDTO;
                bang.Rows.Add(maMon, mon?.TenMonAn ?? "",
                    Convert.ToString(comboBox4.SelectedValue), xl?.Ten ?? "");
                GanBangMonAn(bang);
            }
            catch (Exception ex) { BaoLoi("Không thêm được món vào danh sách.", ex); }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null || dataGridView1.CurrentRow.IsNewRow)
            { MessageBox.Show("Chọn món cần xóa trong bảng Món làm được."); return; }
            try
            {
                int index = dataGridView1.CurrentRow.Index;
                var bang = TaoBangMonAnHienTai();
                if (index >= 0 && index < bang.Rows.Count) bang.Rows.RemoveAt(index);
                GanBangMonAn(bang);
            }
            catch (Exception ex) { BaoLoi("Không xóa được món khỏi danh sách.", ex); }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                _dangThem = true;
                XoaForm();
                textBox1.Text = _bll.TaoMaMoi();
                textBox1.ReadOnly = true;
                button6.Enabled = true;
                button1.Enabled = true;
                button2.Enabled = true;
                textBox2.Focus();
            }
            catch (Exception ex) { _dangThem = false; BaoLoi("Không tạo được mã đầu bếp mới. Kiểm tra kết nối database.", ex); }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text)) { MessageBox.Show("Chọn đầu bếp cần sửa."); return; }
            _dangThem = false; textBox1.ReadOnly = true; button6.Enabled = true; button1.Enabled = true; button2.Enabled = true;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text)) { MessageBox.Show("Chọn đầu bếp cần xóa."); return; }
            if (MessageBox.Show("Bạn chắc chắn muốn xóa đầu bếp này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            try { _bll.Xoa(textBox1.Text.Trim()); NapDanhSach(); XoaForm(); }
            catch (Exception ex) { BaoLoi("Không thể xóa. Đầu bếp có thể đang được dùng trong thực đơn.", ex); }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidateDiaChi(textBox4.Text) == false) { return; }
                if (ValidateHoTen(textBox2.Text) == false) { return; }
                if (ValidateSoDienThoai(textBox3.Text) == false) { return; }
                if (string.IsNullOrWhiteSpace(textBox2.Text)) { MessageBox.Show("Tên đầu bếp không được để trống."); textBox2.Focus(); return; }
                if (comboBox1.SelectedValue == null) { MessageBox.Show("Database chưa có danh mục trình độ."); return; }
                var d = new DauBepDTO
                {
                    MaDauBep = textBox1.Text.Trim(),
                    TenDauBep = textBox2.Text.Trim(),
                    GioiTinh = radioButton2.Checked ? "Nữ" : "Nam",
                    DiaChi = string.IsNullOrWhiteSpace(textBox4.Text) ? null : textBox4.Text.Trim(),
                    DienThoai = string.IsNullOrWhiteSpace(textBox3.Text) ? null : textBox3.Text.Trim(),
                    MaTrinhDo = Convert.ToString(comboBox1.SelectedValue),
                    MaNoiHoc = comboBox2.SelectedValue == null ? null : Convert.ToString(comboBox2.SelectedValue)
                };
                if (_dangThem) _bll.Them(d, LayMonAnTrenGrid()); else _bll.Sua(d, LayMonAnTrenGrid());
                NapDanhSach(); ChonDong(d.MaDauBep); MessageBox.Show("Lưu thông tin đầu bếp thành công.");
                _dangThem = false; textBox1.ReadOnly = true;
                button6.Enabled = false;
                button1.Enabled = false;
                button2.Enabled = false;
            }
            catch (Exception ex) { BaoLoi("Lưu không thành công. Kiểm tra dữ liệu và các danh mục liên quan.", ex); }
        }

        private void ChonDong(string ma)
        {
            foreach (DataGridViewRow r in dataGridView2.Rows)
                if (Convert.ToString(r.Cells["colMa"].Value) == ma) { r.Selected = true; dataGridView2.CurrentCell = r.Cells[0]; break; }
        }

        private void XoaForm()
        {
            textBox1.Clear(); textBox2.Clear(); textBox3.Clear(); textBox4.Clear();
            radioButton1.Checked = true; radioButton2.Checked = false;
            if (comboBox1.Items.Count > 0) comboBox1.SelectedIndex = 0;
            if (comboBox2.Items.Count > 0) comboBox2.SelectedIndex = -1;
            dataGridView1.DataSource = null;
        }

        private void CheDoXem()
        {
            textBox1.ReadOnly = true; button6.Enabled = false; button1.Enabled = false;
            button2.Enabled = false;
        }

        private static void BaoLoi(string noiDung, Exception ex)
        {
            string chiTiet = ex is SqlException ? "\nChi tiết SQL: " + ex.Message : "\nChi tiết: " + ex.Message;
            MessageBox.Show(noiDung + chiTiet, "Quản lý đầu bếp", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private bool ValidateHoTen(string hoTen)
        {
            hoTen = hoTen.Trim();

            if (string.IsNullOrWhiteSpace(hoTen))
            {
                MessageBox.Show(
                    "Họ tên không được để trống!",
                    "Lỗi nhập liệu",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return false;
            }

            // Chỉ cho phép chữ cái tiếng Việt và khoảng trắng
            if (!Regex.IsMatch(hoTen,
                @"^[\p{L}]+(?:\s+[\p{L}]+)*$"))
            {
                MessageBox.Show(
                    "Họ tên chỉ được chứa chữ cái và khoảng trắng!",
                    "Lỗi nhập liệu",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return false;
            }

            return true;
        }

        private bool ValidateSoDienThoai(string soDienThoai)
        {
            soDienThoai = soDienThoai.Trim();

            // Số điện thoại Việt Nam: 10 chữ số, bắt đầu bằng 0
            if (!Regex.IsMatch(soDienThoai, @"^0\d{9}$"))
            {
                MessageBox.Show(
                    "Số điện thoại phải có 10 chữ số và bắt đầu bằng 0!",
                    "Lỗi nhập liệu",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return false;
            }

            return true;
        }

        private bool ValidateDiaChi(string diaChi)
        {
            diaChi = diaChi.Trim();

            if (string.IsNullOrWhiteSpace(diaChi))
            {
                MessageBox.Show(
                    "Địa chỉ không được để trống!",
                    "Lỗi nhập liệu",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return false;
            }

            if (diaChi.Length < 5 || diaChi.Length > 200)
            {
                MessageBox.Show(
                    "Địa chỉ phải có từ 5 đến 200 ký tự!",
                    "Lỗi nhập liệu",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return false;
            }

            return true;
        }

    }
}