
using System;
using System.Data;
using System.IO;
using System.Windows.Forms;
using CrystalDecisions.CrystalReports.Engine;

namespace QLThucDon_MonAN_Nhom5.GUI.Forms.BaoCao
{
    public partial class frmXemBaoCaoMonAn : Form
    {
        private ReportDocument _report;

        public frmXemBaoCaoMonAn(DataTable duLieu, string tieuDe)
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
                    "crptMonAn.rpt");

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
                _report.SetDataSource(duLieu);

                crystalReportViewer1.ReportSource = _report;
                crystalReportViewer1.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể hiển thị báo cáo:\n\n" + ex,
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
