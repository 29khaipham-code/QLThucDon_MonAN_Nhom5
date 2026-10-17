using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLThucDon_MonAN_Nhom5.BLL
{
    
     //Lỗi nghiệp vụ có thông điệp thân thiện. GUI bắt và hiện bằng MessageBox.
    public class BusinessException : Exception
    {
        public BusinessException(string message) : base(message) { }
    }
    
}
