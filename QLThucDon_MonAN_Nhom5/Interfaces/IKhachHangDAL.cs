
using System.Collections.Generic;
using QLThucDon_MonAN_Nhom5.DTO;

namespace QLThucDon_MonAN_Nhom5.Interfaces
{
    public interface IKhachHangDAL : IRepository<KhachHangDTO>
    {
        List<KhachHangDTO> TimKiem(string ten);
    }
}
