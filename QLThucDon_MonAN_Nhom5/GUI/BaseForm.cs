using System;
using System.Drawing;
using System.Windows.Forms;

namespace QLThucDon_MonAN_Nhom5.GUI
{
  
    /// Lớp cha của mọi form chức năng. Gom các đoạn định dạng giao diện trước đây bị
    /// copy-paste ở hơn 10 form (nút, lưới, ô nhập, nhóm). Muốn đổi theme chỉ sửa ở đây.
    /// Lưu ý: KHÔNG đặt abstract, vì Visual Studio Designer cần tạo được đối tượng của lớp cha.
    
    public class BaseForm : Form
    {
        // Font dùng chung (static, tạo một lần, không phải dispose)
        protected static readonly Font FontThuong = new Font("Segoe UI", 9F);
        protected static readonly Font FontDam = new Font("Segoe UI", 9F, FontStyle.Bold);
        protected static readonly Font FontTieuDe = new Font("Segoe UI", 12F, FontStyle.Bold);
        protected static readonly Font FontSo = new Font("Segoe UI", 10F, FontStyle.Bold);

        /// Gọi sau InitializeComponent(): đặt màu nền, chữ, font và tiêu đề form.
        protected void DinhDangForm(string tieuDe = null)
        {
            BackColor = Ui.Back;
            ForeColor = Ui.Text;
            Font = FontThuong;
            if (!string.IsNullOrEmpty(tieuDe)) Text = tieuDe;
        }

        /// <summary>Nút chính: nền Teal, chữ trắng. Nút phụ: nền trắng, viền xám.</summary>
        protected static void DinhDangNutBam(Button b, bool chinh)
        {
            b.UseVisualStyleBackColor = false;
            b.FlatStyle = FlatStyle.Flat;
            b.Cursor = Cursors.Hand;
            b.Font = FontDam;
            b.BackColor = chinh ? Ui.Accent : Ui.Surface;
            b.ForeColor = chinh ? Color.White : Ui.Dark;
            b.FlatAppearance.BorderSize = 1;
            b.FlatAppearance.BorderColor = chinh ? Ui.Accent : Ui.Border;
            b.FlatAppearance.MouseOverBackColor = chinh ? Ui.Dark : Ui.Hover;
            b.FlatAppearance.MouseDownBackColor = chinh ? Ui.Dark : Ui.Header;
        }

        /// <summary>Lưới chỉ đọc, tiêu đề xám, dòng xen kẽ, chọn cả dòng.</summary>
        protected static void DinhDangLuoi(DataGridView g, int chieuCaoDong = 32, bool xuongDong = false)
        {
            g.EnableHeadersVisualStyles = false;
            g.BackgroundColor = Ui.Surface;
            g.BorderStyle = BorderStyle.None;
            g.GridColor = Ui.Border;

            g.ReadOnly = true;
            g.AllowUserToAddRows = false;
            g.AllowUserToDeleteRows = false;
            g.AllowUserToResizeRows = false;
            g.MultiSelect = false;
            g.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            g.RowHeadersVisible = false;

            // Đặt chế độ chiều cao tiêu đề TRƯỚC rồi mới đặt chiều cao
            g.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            g.ColumnHeadersHeight = 38;

            var h = g.ColumnHeadersDefaultCellStyle;
            h.BackColor = Ui.Header;
            h.ForeColor = Ui.Dark;
            h.SelectionBackColor = Ui.Header;
            h.SelectionForeColor = Ui.Dark;
            h.Font = FontDam;
            h.Alignment = DataGridViewContentAlignment.MiddleLeft;
            h.Padding = new Padding(6, 0, 6, 0);

            var c = g.DefaultCellStyle;
            c.BackColor = Ui.Surface;
            c.ForeColor = Ui.Text;
            c.SelectionBackColor = Ui.Accent;
            c.SelectionForeColor = Color.White;
            c.Font = FontThuong;
            c.Padding = new Padding(6, 3, 6, 3);

            var a = g.AlternatingRowsDefaultCellStyle;
            a.BackColor = Ui.Back;
            a.ForeColor = Ui.Text;
            a.SelectionBackColor = Ui.Accent;
            a.SelectionForeColor = Color.White;

            g.RowTemplate.Height = chieuCaoDong;
            if (xuongDong)
            {
                c.WrapMode = DataGridViewTriState.True;
                g.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            }
        }

        /// <summary>Định dạng đệ quy Label, TextBox, ComboBox, GroupBox trong một container.</summary>
        protected static void DinhDangODuLieu(Control goc)
        {
            if (goc == null) return;
            foreach (Control c in goc.Controls)
            {
                if (c is GroupBox gb) { gb.ForeColor = Ui.Dark; gb.Font = FontDam; }
                else if (c is Label l) { l.ForeColor = Ui.Dark; l.Font = FontDam; }
                else if (c is TextBox t)
                {
                    t.BackColor = Ui.Surface; t.ForeColor = Ui.Text;
                    t.Font = FontThuong; t.BorderStyle = BorderStyle.FixedSingle;
                }
                else if (c is ComboBox cb)
                {
                    cb.BackColor = Ui.Surface; cb.ForeColor = Ui.Text;
                    cb.Font = FontThuong; cb.FlatStyle = FlatStyle.Flat;
                }

                if (c.HasChildren) DinhDangODuLieu(c);
            }
        }
    }
}
