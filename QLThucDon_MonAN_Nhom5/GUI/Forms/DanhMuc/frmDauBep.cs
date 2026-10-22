using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using Microsoft.Data.SqlClient;
using QLThucDon_MonAN_Nhom5.DAL;
using QLThucDon_MonAN_Nhom5.BLL;
using QLThucDon_MonAN_Nhom5.DTO;

namespace QLThucDon_MonAN_Nhom5.GUI.Forms.DanhMuc
{
    public partial class frmDauBep : Form
    {
        private readonly DauBepBLL _bll = new DauBepBLL();
        private readonly DauBepDAL _dal = new DauBepDAL();
        private bool _dangThem;

        public frmDauBep()
        {
            InitializeComponent();
            ApDungGiaoDien();
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
            dataGridView1.AllowUserToAddRows = false;
        }


        // Áp dụng bảng màu và style thống nhất từ GUI/Ui.cs cho giao diện danh mục.
        private void ApDungGiaoDien()
        {
            BackColor = Ui.Back;
            ForeColor = Ui.Text;
            Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            panel1.BackColor = Ui.Back;

            StyleGroupBox(groupBox1);
            StyleGroupBox(groupBox2);

            // Dùng chung style DataGridView đã định nghĩa trong Ui.cs.
            Ui.StyleGrid(dataGridView1);
            Ui.StyleGrid(dataGridView2);
            StyleGridRows(dataGridView1);
            StyleGridRows(dataGridView2);

            // Giữ nguyên các button có sẵn trong Designer, chỉ thay đổi giao diện.
            StyleButton(button1, true);
            StyleButton(button2, false);
            StyleButton(button3, true);
            StyleButton(button4, false);
            StyleButton(button5, false);
            StyleButton(button6, true);

            foreach (var label in new[] { label1, label2, label3, label4, label5, label6, label7, label8, label9 })
            {
                label.ForeColor = Ui.Text;
                label.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            }

            foreach (var input in new[] { textBox1, textBox2, textBox3, textBox4 })
            {
                input.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
                input.BackColor = Ui.Surface;
                input.ForeColor = Ui.Text;
                input.BorderStyle = BorderStyle.FixedSingle;
            }

            foreach (var combo in new[] { comboBox1, comboBox2, comboBox3, comboBox4 })
            {
                combo.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
                combo.BackColor = Ui.Surface;
                combo.ForeColor = Ui.Text;
                combo.FlatStyle = FlatStyle.Flat;
            }

            radioButton1.ForeColor = Ui.Text;
            radioButton2.ForeColor = Ui.Text;
            radioButton1.Font = radioButton2.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
        }

        private void StyleGroupBox(GroupBox group)
        {
            group.BackColor = Ui.Surface;
            group.ForeColor = Ui.Dark;
            group.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        }

        private void StyleButton(Button button, bool primary)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.UseVisualStyleBackColor = false;
            button.BackColor = primary ? Ui.Accent : Ui.Surface;
            button.ForeColor = primary ? Color.White : Ui.Dark;
            button.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            button.Cursor = Cursors.Hand;
            button.FlatAppearance.BorderSize = 1;
            button.FlatAppearance.BorderColor = primary ? Ui.Accent : Ui.Border;
            button.FlatAppearance.MouseOverBackColor = Ui.Hover;
            button.FlatAppearance.MouseDownBackColor = Ui.Header;
            button.Height = Math.Max(button.Height, 32);
        }

        private void StyleGridRows(DataGridView grid)
        {
            grid.BorderStyle = BorderStyle.None;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            grid.DefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            grid.RowTemplate.Height = 28;
            grid.ColumnHeadersHeight = 34;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = false;
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
            comboBox1.DataSource = DbHelper.ExecuteDataTable("SELECT MaTrinhDo, TenTrinhDo FROM TrinhDo ORDER BY TenTrinhDo");
            comboBox1.DisplayMember = "TenTrinhDo"; comboBox1.ValueMember = "MaTrinhDo";
            comboBox2.DataSource = DbHelper.ExecuteDataTable("SELECT MaNoiHoc, TenNoiHoc FROM NoiHoc ORDER BY TenNoiHoc");
            comboBox2.DisplayMember = "TenNoiHoc"; comboBox2.ValueMember = "MaNoiHoc";
            comboBox3.DataSource = DbHelper.ExecuteDataTable("SELECT MaMonAn, TenMonAn FROM MonAn ORDER BY TenMonAn");
            comboBox3.DisplayMember = "TenMonAn"; comboBox3.ValueMember = "MaMonAn";
            comboBox4.DataSource = DbHelper.ExecuteDataTable("SELECT MaXepLoai, TenXepLoai FROM XepLoai ORDER BY TenXepLoai");
            comboBox4.DisplayMember = "TenXepLoai"; comboBox4.ValueMember = "MaXepLoai";
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
            textBox3.Text = d.DiaChi ?? ""; textBox4.Text = d.DienThoai ?? "";
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
            var list = _dal.GetMonAn(ma);
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
                var selected = comboBox4.Items.Cast<object>().OfType<DataRowView>()
                    .FirstOrDefault(x => Convert.ToString(x[comboBox4.ValueMember]) == xl);
                if (selected != null) tenXl = Convert.ToString(selected[comboBox4.DisplayMember]);
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
                var mon = comboBox3.SelectedItem as DataRowView;
                var xl = comboBox4.SelectedItem as DataRowView;
                bang.Rows.Add(maMon, Convert.ToString(mon?[comboBox3.DisplayMember]),
                    Convert.ToString(comboBox4.SelectedValue), Convert.ToString(xl?[comboBox4.DisplayMember]));
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
                textBox1.Text = _dal.NextCode();
                textBox1.ReadOnly = true;
                button6.Enabled = true;
                textBox2.Focus();
            }
            catch (Exception ex) { _dangThem = false; BaoLoi("Không tạo được mã đầu bếp mới. Kiểm tra kết nối database.", ex); }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text)) { MessageBox.Show("Chọn đầu bếp cần sửa."); return; }
            _dangThem = false; textBox1.ReadOnly = true; button6.Enabled = true;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text)) { MessageBox.Show("Chọn đầu bếp cần xóa."); return; }
            if (MessageBox.Show("Bạn chắc chắn muốn xóa đầu bếp này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            try { _dal.Delete(textBox1.Text.Trim()); NapDanhSach(); XoaForm(); }
            catch (Exception ex) { BaoLoi("Không thể xóa. Đầu bếp có thể đang được dùng trong thực đơn.", ex); }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(textBox2.Text)) { MessageBox.Show("Tên đầu bếp không được để trống."); textBox2.Focus(); return; }
                if (comboBox1.SelectedValue == null) { MessageBox.Show("Database chưa có danh mục trình độ."); return; }
                var d = new DauBepDTO {
                    MaDauBep = textBox1.Text.Trim(), TenDauBep = textBox2.Text.Trim(),
                    GioiTinh = radioButton2.Checked ? "Nữ" : "Nam",
                    DiaChi = string.IsNullOrWhiteSpace(textBox3.Text) ? null : textBox3.Text.Trim(),
                    DienThoai = string.IsNullOrWhiteSpace(textBox4.Text) ? null : textBox4.Text.Trim(),
                    MaTrinhDo = Convert.ToString(comboBox1.SelectedValue),
                    MaNoiHoc = comboBox2.SelectedValue == null ? null : Convert.ToString(comboBox2.SelectedValue)
                };
                if (_dangThem) _dal.Insert(d, LayMonAnTrenGrid()); else _dal.Update(d, LayMonAnTrenGrid());
                NapDanhSach(); ChonDong(d.MaDauBep); MessageBox.Show("Lưu thông tin đầu bếp thành công.");
                _dangThem = false; textBox1.ReadOnly = true;
                button6.Enabled = false;
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
            textBox1.ReadOnly = true; button6.Enabled = false;
        }

        private static void BaoLoi(string noiDung, Exception ex)
        {
            string chiTiet = ex is SqlException ? "\nChi tiết SQL: " + ex.Message : "\nChi tiết: " + ex.Message;
            MessageBox.Show(noiDung + chiTiet, "Quản lý đầu bếp", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
