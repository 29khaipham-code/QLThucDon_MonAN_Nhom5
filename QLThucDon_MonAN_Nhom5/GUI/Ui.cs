using QLThucDon_MonAN_Nhom5.BLL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QLThucDon_MonAN_Nhom5.GUI
{
    public class Ui
    {

        public static readonly Color Accent = ColorTranslator.FromHtml("#0F766E");
        public static readonly Color Dark = ColorTranslator.FromHtml("#115E59");
        public static readonly Color Back = ColorTranslator.FromHtml("#F3F4F6");
        public static readonly Color Header = ColorTranslator.FromHtml("#E5E7EB");
        public static readonly Color Surface = ColorTranslator.FromHtml("#FFFFFF");
        public static readonly Color Hover = ColorTranslator.FromHtml("#CCFBF1");
        public static readonly Color Border = ColorTranslator.FromHtml("#D1D5DB");
        public static readonly Color Text = ColorTranslator.FromHtml("#1F2937"); // Chữ chính


        public static void Info(string msg) =>
            MessageBox.Show(msg, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

        public static void Error(string msg) =>
            MessageBox.Show(msg, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        public static bool Confirm(string msg) =>
            MessageBox.Show(msg, "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;

        //Chạy một thao tác; lỗi nghiệp vụ hiện đúng thông điệp của BLL. Trả về true nếu thành công
        public static bool Run(Action action)
        {
            try { action(); return true; }
            catch (BusinessException ex) { Error(ex.Message); }
            catch (Exception ex) { Error("Đã xảy ra lỗi: " + ex.Message); }
            return false;
        }


        public static Button Btn(string text, bool primary = true)
        {
            var b = new Button
            {
                Text = text,
                Height = 34,
                AutoSize = true,
                MinimumSize = new Size(88, 34),
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(0, 4, 8, 4),
                Cursor = Cursors.Hand,
                UseVisualStyleBackColor = false,
                BackColor = primary ? Accent : Surface,
                ForeColor = primary ? Color.White : Dark
            };

            b.FlatAppearance.BorderSize = 1;
            b.FlatAppearance.BorderColor = primary ? Accent : Border;
            b.FlatAppearance.MouseOverBackColor = Hover;
            b.FlatAppearance.MouseDownBackColor = Header;

            return b;
        }


        public static Label Lbl(string text) =>
            new Label { Text = text, AutoSize = true, Margin = new Padding(0, 9, 6, 0), Font = new Font("Segoe UI", 9F, FontStyle.Bold) };

        public static void StyleGrid(DataGridView g)
        {
            g.BackgroundColor = Surface;
            g.EnableHeadersVisualStyles = false;
            g.ColumnHeadersDefaultCellStyle.BackColor = Header;
            g.ColumnHeadersDefaultCellStyle.ForeColor = Dark;
            g.DefaultCellStyle.BackColor = Surface;
            g.DefaultCellStyle.ForeColor = Text;
            g.DefaultCellStyle.SelectionBackColor = Accent;
            g.DefaultCellStyle.SelectionForeColor = Color.White;
            g.AlternatingRowsDefaultCellStyle.BackColor = Back;
            g.GridColor = Border;
        }

        public static DataGridViewTextBoxColumn Col(string prop, string header, int width = 0, string format = null)
        {
            var c = new DataGridViewTextBoxColumn { DataPropertyName = prop, HeaderText = header, Name = prop };
            if (width > 0) c.Width = width; else c.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            if (format != null) c.DefaultCellStyle.Format = format;
            return c;
        }
    }
}
