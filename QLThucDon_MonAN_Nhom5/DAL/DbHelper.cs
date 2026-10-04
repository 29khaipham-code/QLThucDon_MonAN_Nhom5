using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Microsoft.Data.SqlClient;

namespace QLThucDon_MonAN_Nhom5.DAL
{
    // lop tien ich truy cap SQL Server (ADO.NET). Moi DAL deu di qua lop nay.
    public static class DbHelper
    {
        public static string ConnectionString { get; set; } =
            @"Server=.\SQLEXPRESS;Database=QuanLyDauBep;Trusted_Connection=True;TrustServerCertificate=True;";

        public static SqlConnection CreateConnection() => new SqlConnection(ConnectionString);

        public static SqlParameter P(string name, object value) =>
            new SqlParameter(name, value ?? DBNull.Value);

        public static string NullIfEmpty(string s) => string.IsNullOrWhiteSpace(s) ? null : s.Trim();

        public static SqlCommand Build(SqlConnection c, SqlTransaction t, string sql, SqlParameter[] ps)
        {
            var cmd = new SqlCommand(sql, c, t);
            if (ps != null && ps.Length > 0) cmd.Parameters.AddRange(ps);
            return cmd;
        }


        public static DataTable ExecuteDataTable(string sql, params SqlParameter[] ps)
        {
            using var conn = CreateConnection();
            using var cmd = Build(conn, null, sql, ps);
            using var da = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public static List<T> ExecuteList<T>(string sql, Func<SqlDataReader, T> map, params SqlParameter[] ps)
        {
            var list = new List<T>();
            using var conn = CreateConnection();
            conn.Open();
            using var cmd = Build(conn, null, sql, ps);
            using var r = cmd.ExecuteReader();
            while (r.Read()) list.Add(map(r));
            return list;
        }

        public static object ExecuteScalar(string sql, params SqlParameter[] ps)
        {
            using var conn = CreateConnection();
            conn.Open();
            using var cmd = Build(conn, null, sql, ps);
            return cmd.ExecuteScalar();
        }

        // ---------- Thao tác ghi ----------
        public static int ExecuteNonQuery(string sql, params SqlParameter[] ps)
        {
            using var conn = CreateConnection();
            conn.Open();
            using var cmd = Build(conn, null, sql, ps);
            return cmd.ExecuteNonQuery();
        }

        /// <summary>Chạy trong transaction có sẵn.</summary>
        public static int ExecuteNonQuery(SqlConnection c, SqlTransaction t, string sql, params SqlParameter[] ps)
        {
            using var cmd = Build(c, t, sql, ps);
            return cmd.ExecuteNonQuery();
        }

        /// <summary>Chạy nhiều lệnh trong 1 transaction; lỗi -> rollback toàn bộ rồi ném lại exception.</summary>
        public static void RunInTransaction(Action<SqlConnection, SqlTransaction> work)
        {
            using var conn = CreateConnection();
            conn.Open();
            using var tran = conn.BeginTransaction();
            try
            {
                work(conn, tran);
                tran.Commit();
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }

        /// <summary>Sinh mã tiếp theo, vd NextCode("MonAn","MaMonAn","MA",3) -> "MA013". Tham số table/col chỉ truyền hằng trong code.</summary>
        public static string NextCode(string table, string column, string prefix, int width)
        {
            string sql = $@"SELECT ISNULL(MAX(CAST(SUBSTRING({column}, LEN(@p) + 1, 20) AS INT)), 0) + 1
                            FROM {table} WHERE {column} LIKE @p + '[0-9]%'";
            int next = Convert.ToInt32(ExecuteScalar(sql, P("@p", prefix)));
            return prefix + next.ToString("D" + width);
        }
    }

    // Các hàm tiện ích đọc dữ liệu từ SqlDataReader, tránh lỗi DBNull.Value.
    internal static class ReaderExtensions
    {
        public static string Str(this SqlDataReader r, string col) { var o = r[col]; return o == DBNull.Value ? null : Convert.ToString(o); }
        public static decimal Dec(this SqlDataReader r, string col) { var o = r[col]; return o == DBNull.Value ? 0m : Convert.ToDecimal(o); }
        public static int Int(this SqlDataReader r, string col) { var o = r[col]; return o == DBNull.Value ? 0 : Convert.ToInt32(o); }
        public static DateTime Date(this SqlDataReader r, string col) { var o = r[col]; return o == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(o); }
    }
}
