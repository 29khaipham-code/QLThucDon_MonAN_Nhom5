
using System;
using System.Data;
using System.IO;
using System.Windows.Forms;
using CrystalDecisions.CrystalReports.Engine;

namespace QLThucDon_MonAN_Nhom5.GUI.Forms.BaoCao
{
    public partial class frmXemBaoCaoDoanhThu : Form
    {
        private ReportDocument _report;

        public frmXemBaoCaoDoanhThu(
            DataTable duLieu,
            string tieuDe,
            decimal tongDoanhThu)
        {
            InitializeComponent();

            Text = tieuDe;

            try
            {
                if (duLieu == null || duLieu.Rows.Count == 0)
                {
                    MessageBox.Show(
                        "Không có dữ liệu để hiển thị báo cáo!",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    return;
                }

                string duongDan = Path.Combine(
                    Application.StartupPath,
                    "Reports",
                    "crptDoanhThu.rpt"
                );

                if (!File.Exists(duongDan))
                {
                    MessageBox.Show(
                        "Không tìm thấy file báo cáo:\n" + duongDan,
                        "Lỗi",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    return;
                }

                _report = new ReportDocument();
                _report.Load(duongDan);

                // Truyền dữ liệu doanh thu
                _report.SetDataSource(duLieu);

                // Truyền tổng doanh thu vào tham số của Crystal Reports
                _report.SetParameterValue(
                    "pTongDoanhThu",
                    tongDoanhThu
                );

                // Hiển thị báo cáo
                crystalReportViewer1.ReportSource = _report;
                crystalReportViewer1.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể hiển thị báo cáo:\n\n" + ex.ToString(),
                    "Lỗi báo cáo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (_report != null)
            {
                crystalReportViewer1.ReportSource = null;
                _report.Close();
                _report.Dispose();
                _report = null;
            }

            base.OnFormClosed(e);
        }
    }
}
