using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QLThucDon_MonAN_Nhom5.BLL
{
    // cac ham kiem tra du lieu dung chung , sai thi dua qua businessException de thong bao
    internal class Check
    {
        public static void Required(string value, string tenTruong)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new BusinessException($"{tenTruong} không được để trống.");
        }

        public static void NonNegative(decimal value, string tenTruong)
        {
            if (value < 0) throw new BusinessException($"{tenTruong} không được âm.");
        }

        public static void Positive(decimal value, string tenTruong)
        {
            if (value <= 0) throw new BusinessException($"{tenTruong} phải lớn hơn 0.");
        }

        // Số điện thoại VN: bắt đầu bằng 0, 10-11 chữ số. Cho phép để trống.
        public static void Phone(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return;
            if (!Regex.IsMatch(value.Trim(), @"^0\d{9,10}$"))
                throw new BusinessException("Số điện thoại không hợp lệ (bắt đầu bằng 0, gồm 10-11 chữ số).");
        }

        //Mã số thuế: 10 số hoặc 10 số-3 số. Cho phép để trống
        public static void MaSoThue(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return;
            if (!Regex.IsMatch(value.Trim(), @"^\d{10}(-?\d{3})?$"))
                throw new BusinessException("Mã số thuế không hợp lệ (10 số hoặc 13 số).");
        }
    }
}
