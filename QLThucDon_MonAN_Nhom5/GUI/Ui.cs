
using QLThucDon_MonAN_Nhom5.BLL;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace QLThucDon_MonAN_Nhom5.GUI
{
    public static class Ui
    {
        // ===================== MÀU GIAO DIỆN =====================

        public static readonly Color Accent =
            ColorTranslator.FromHtml("#0F766E");

        public static readonly Color Dark =
            ColorTranslator.FromHtml("#115E59");

        public static readonly Color Back =
            ColorTranslator.FromHtml("#F3F4F6");

        public static readonly Color Header =
            ColorTranslator.FromHtml("#E5E7EB");

        public static readonly Color Surface =
            ColorTranslator.FromHtml("#FFFFFF");

        public static readonly Color Hover =
            ColorTranslator.FromHtml("#CCFBF1");

        public static readonly Color Border =
            ColorTranslator.FromHtml("#D1D5DB");

        public static readonly Color Text =
            ColorTranslator.FromHtml("#1F2937");

        // ===================== THÔNG BÁO =====================

        public static void Info(string msg)
        {
            MessageBox.Show(
                msg ?? "",
                "Thông báo",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        public static void Error(string msg)
        {
            MessageBox.Show(
                msg ?? "",
                "Lỗi",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }

        public static bool Confirm(string msg)
        {
            return MessageBox.Show(
                msg ?? "",
                "Xác nhận",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question)
                == DialogResult.Yes;
        }

        // ===================== XỬ LÝ NGOẠI LỆ =====================

        /// <summary>
        /// Thực hiện thao tác không có giá trị trả về.
        /// Trả về true nếu thành công, false nếu xảy ra lỗi.
        /// </summary>
        public static bool Run(Action action)
        {
            if (action == null)
            {
                Error("Thao tác thực hiện không hợp lệ.");
                return false;
            }

            try
            {
                action();
                return true;
            }
            catch (BusinessException ex)
            {
                Error(ex.Message);
            }
            catch (Exception ex)
            {
                Error("Đã xảy ra lỗi: " + ex.Message);
            }

            return false;
        }

        /// <summary>
        /// Thực hiện thao tác có giá trị trả về.
        /// Trả về kết quả và thông báo thành công/thất bại.
        /// </summary>
        public static bool Run<T>(Func<T> func, out T result)
        {
            result = default(T);

            if (func == null)
            {
                Error("Thao tác thực hiện không hợp lệ.");
                return false;
            }

            try
            {
                result = func();
                return true;
            }
            catch (BusinessException ex)
            {
                Error(ex.Message);
            }
            catch (Exception ex)
            {
                Error("Đã xảy ra lỗi: " + ex.Message);
            }

            return false;
        }

        // ===================== TẠO BUTTON =====================

        public static Button Btn(string text, bool primary = true)
        {
            var b = new Button
            {
                Text = text ?? "",
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

            b.FlatAppearance.BorderColor =
                primary ? Accent : Border;

            b.FlatAppearance.MouseOverBackColor =
                primary ? Dark : Hover;

            b.FlatAppearance.MouseDownBackColor =
                Header;

            b.Font = new Font(
                "Segoe UI",
                9F,
                FontStyle.Bold);

            return b;
        }

        // ===================== TẠO LABEL =====================

        public static Label Lbl(string text)
        {
            return new Label
            {
                Text = text ?? "",
                AutoSize = true,
                Margin = new Padding(0, 9, 6, 0),
                ForeColor = Dark,
                Font = new Font(
                    "Segoe UI",
                    9F,
                    FontStyle.Bold)
            };
        }

        // ===================== ĐỊNH DẠNG DATAGRIDVIEW =====================

        public static void StyleGrid(DataGridView g)
        {
            if (g == null)
                throw new ArgumentNullException(nameof(g));

            g.BackgroundColor = Surface;
            g.BorderStyle = BorderStyle.None;
            g.EnableHeadersVisualStyles = false;

            g.ColumnHeadersDefaultCellStyle.BackColor = Header;
            g.ColumnHeadersDefaultCellStyle.ForeColor = Dark;
            g.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 9F, FontStyle.Bold);

            g.DefaultCellStyle.BackColor = Surface;
            g.DefaultCellStyle.ForeColor = Text;
            g.DefaultCellStyle.SelectionBackColor = Accent;
            g.DefaultCellStyle.SelectionForeColor = Color.White;
            g.DefaultCellStyle.Font =
                new Font("Segoe UI", 9F);

            g.AlternatingRowsDefaultCellStyle.BackColor = Back;
            g.AlternatingRowsDefaultCellStyle.ForeColor = Text;
            g.AlternatingRowsDefaultCellStyle.SelectionBackColor = Accent;
            g.AlternatingRowsDefaultCellStyle.SelectionForeColor = Color.White;

            g.GridColor = Border;
        }

        // ===================== TẠO CỘT DATAGRIDVIEW =====================

        public static DataGridViewTextBoxColumn Col(
            string prop,
            string header,
            int width = 0,
            string format = null)
        {
            if (string.IsNullOrWhiteSpace(prop))
                throw new ArgumentException(
                    "Tên thuộc tính cột không được để trống.",
                    nameof(prop));

            var c = new DataGridViewTextBoxColumn
            {
                DataPropertyName = prop,
                HeaderText = header ?? "",
                Name = prop
            };

            if (width > 0)
            {
                c.AutoSizeMode =
                    DataGridViewAutoSizeColumnMode.None;

                c.Width = width;
            }
            else
            {
                c.AutoSizeMode =
                    DataGridViewAutoSizeColumnMode.Fill;
            }

            if (!string.IsNullOrEmpty(format))
                c.DefaultCellStyle.Format = format;

            return c;
        }
    }
}
