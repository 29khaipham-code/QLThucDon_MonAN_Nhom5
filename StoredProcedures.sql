CREATE OR ALTER PROCEDURE sp_DoanhThuDauBep
    @Nam INT,
    @Thang INT = NULL,
    @Quy INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        d.MaDauBep,
        d.TenDauBep,
        ISNULL(SUM(ct.SoLuong), 0) AS SoPhan,
        ISNULL(SUM(ct.ThanhTien), 0) AS DoanhThu
    FROM DauBep d
    LEFT JOIN (
        ChiTietThucDon ct
        JOIN ThucDon td
            ON td.SoThucDon = ct.SoThucDon
            AND YEAR(td.NgayDung) = @Nam
            AND (@Thang IS NULL OR MONTH(td.NgayDung) = @Thang)
            AND (@Quy IS NULL OR DATEPART(QUARTER, td.NgayDung) = @Quy)
    )
        ON ct.MaDauBep = d.MaDauBep
    GROUP BY d.MaDauBep, d.TenDauBep
    ORDER BY DoanhThu DESC, d.TenDauBep;
END;
GO








CREATE OR ALTER PROCEDURE sp_MonAnDayDu
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        m.MaMonAn,
        m.TenMonAn,
        l.TenLoai,
        cd.TenCongDung,
        m.DonGia,
        m.CachLam,
        m.YeuCau,
        (
            SELECT STRING_AGG(
                nl.TenNguyenLieu + N' (' +
                CAST(CAST(x.SoLuong AS FLOAT) AS NVARCHAR(20)) +
                N' ' + dvt.TenDonViTinh + N')',
                N', '
            )
            FROM NguyenLieu_MonAn x
            JOIN NguyenLieu nl
                ON nl.MaNguyenLieu = x.MaNguyenLieu
            JOIN DonViTinh dvt
                ON dvt.MaDonViTinh = nl.MaDonViTinh
            WHERE x.MaMonAn = m.MaMonAn
        ) AS NguyenLieu
    FROM MonAn m
    JOIN LoaiMon l
        ON l.MaLoai = m.MaLoai
    LEFT JOIN CongDung cd
        ON cd.MaCongDung = m.MaCongDung
    ORDER BY m.MaMonAn;
END;
GO

EXEC sp_MonAnDayDu;





CREATE OR ALTER PROCEDURE sp_MonAnTheoDauBep
    @Bep NVARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        d.MaDauBep,
        d.TenDauBep,
        m.MaMonAn,
        m.TenMonAn,
        l.TenLoai,
        m.DonGia,
        xl.TenXepLoai
    FROM DauBep d
    JOIN DauBep_MonAn x
        ON x.MaDauBep = d.MaDauBep
    JOIN MonAn m
        ON m.MaMonAn = x.MaMonAn
    JOIN LoaiMon l
        ON l.MaLoai = m.MaLoai
    JOIN XepLoai xl
        ON xl.MaXepLoai = x.MaXepLoai
    WHERE (@Bep IS NULL OR d.MaDauBep = @Bep)
    ORDER BY d.MaDauBep, m.MaMonAn;
END;
GO



-- Lấy tất cả đầu bếp
EXEC sp_MonAnTheoDauBep;

-- Lấy món ăn của một đầu bếp cụ thể
EXEC sp_MonAnTheoDauBep @Bep = N'DB001';








CREATE OR ALTER PROCEDURE sp_ThucDonDayDu
    @So NVARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        td.SoThucDon,
        td.NgayDung,
        kh.MaKhachHang,
        kh.TenKhachHang,
        kh.DiaChi,
        kh.DienThoai,
        kh.MaSoThue,
        m.MaMonAn,
        m.TenMonAn,
        d.TenDauBep,
        ct.SoLuong,
        m.DonGia,
        ct.ThanhTien,
        td.ThueVAT,
        td.GiamGia,
        td.TongTien
    FROM ThucDon td
    JOIN KhachHang kh
        ON kh.MaKhachHang = td.MaKhachHang
    JOIN ChiTietThucDon ct
        ON ct.SoThucDon = td.SoThucDon
    JOIN MonAn m
        ON m.MaMonAn = ct.MaMonAn
    JOIN DauBep d
        ON d.MaDauBep = ct.MaDauBep
    WHERE (@So IS NULL OR td.SoThucDon = @So)
    ORDER BY td.SoThucDon, m.MaMonAn;
END;
GO


-- Lấy tất cả thực đơn
EXEC sp_ThucDonDayDu;

-- Lấy một thực đơn cụ thể
EXEC sp_ThucDonDayDu @So = N'TD001';











CREATE OR ALTER PROCEDURE sp_TimKiemMonAn
    @Loai NVARCHAR(20) = NULL,
    @CD NVARCHAR(20) = NULL,
    @Ten NVARCHAR(200) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        m.MaMonAn,
        m.TenMonAn,
        m.MaLoai,
        l.TenLoai,
        m.MaCongDung,
        cd.TenCongDung,
        m.DonGia,
        m.CachLam,
        m.YeuCau
    FROM MonAn m
    JOIN LoaiMon l
        ON l.MaLoai = m.MaLoai
    LEFT JOIN CongDung cd
        ON cd.MaCongDung = m.MaCongDung
    WHERE (@Loai IS NULL OR m.MaLoai = @Loai)
      AND (@CD IS NULL OR m.MaCongDung = @CD)
      AND (@Ten IS NULL OR m.TenMonAn LIKE N'%' + @Ten + N'%')
    ORDER BY m.MaMonAn;
END;
GO









-- Lấy tất cả món ăn
EXEC sp_TimKiemMonAn;

-- Tìm theo tên
EXEC sp_TimKiemMonAn @Ten = N'Phở';

-- Tìm theo loại và công dụng
EXEC sp_TimKiemMonAn @Loai = N'LM001', @CD = N'CD001';





CREATE OR ALTER PROCEDURE sp_TimKiemDauBep
    @Ten NVARCHAR(200) = NULL,
    @NH NVARCHAR(20) = NULL,
    @Mon NVARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT DISTINCT
        d.MaDauBep,
        d.TenDauBep,
        d.GioiTinh,
        d.DiaChi,
        d.DienThoai,
        d.MaTrinhDo,
        td.TenTrinhDo,
        d.MaNoiHoc,
        nh.TenNoiHoc
    FROM DauBep d
    JOIN TrinhDo td
        ON td.MaTrinhDo = d.MaTrinhDo
    LEFT JOIN NoiHoc nh
        ON nh.MaNoiHoc = d.MaNoiHoc
    LEFT JOIN DauBep_MonAn dm
        ON dm.MaDauBep = d.MaDauBep
    WHERE (@Ten IS NULL OR d.TenDauBep LIKE N'%' + @Ten + N'%')
      AND (@NH IS NULL OR d.MaNoiHoc = @NH)
      AND (@Mon IS NULL OR dm.MaMonAn = @Mon)
    ORDER BY d.MaDauBep;
END;
GO



-- Lấy tất cả đầu bếp
EXEC sp_TimKiemDauBep;

-- Tìm theo tên
EXEC sp_TimKiemDauBep @Ten = N'Nguyễn';

-- Tìm theo nơi học và món ăn
EXEC sp_TimKiemDauBep @NH = N'NH001', @Mon = N'MA001';