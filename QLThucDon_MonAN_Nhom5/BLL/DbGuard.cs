using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace QLThucDon_MonAN_Nhom5.BLL
{
    internal class DbGuard
    {
        public static void Run(Action action)
        {
            try { action(); }
            catch (SqlException ex) { throw Translate(ex); }
        }

        public static T Run<T>(Func<T> func)
        {
            try { return func(); }
            catch (SqlException ex) { throw Translate(ex); }
        }

        private static BusinessException Translate(SqlException ex) => ex.Number switch
        {
            547 => new BusinessException("Dữ liệu đang được sử dụng ở nơi khác (hoặc tham chiếu không hợp lệ) nên không thực hiện được."),
            var n when n == 2627 || n== 2601 => new BusinessException("Mã này đã tồn tại, vui lòng nhập mã khác."),
            _ => new BusinessException("Lỗi cơ sở dữ liệu: " + ex.Message)
        };
    }
}
