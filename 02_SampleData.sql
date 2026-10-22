/* Dữ liệu mẫu - chạy SAU 01_CreateTables.sql. Chạy lại nhiều lần được (xoá dữ liệu cũ trước). */
USE QuanLyDauBep;
GO
DELETE FROM ChiTietThucDon;  DELETE FROM ThucDon;
DELETE FROM DauBep_MonAn;    DELETE FROM NguyenLieu_MonAn;
DELETE FROM DauBep;          DELETE FROM MonAn;
DELETE FROM NguyenLieu;      DELETE FROM KhachHang;
DELETE FROM XepLoai; DELETE FROM TrinhDo; DELETE FROM NoiHoc;
DELETE FROM DonViTinh; DELETE FROM CongDung; DELETE FROM LoaiMon;
GO
INSERT INTO LoaiMon VALUES ('LM01',N'Khai vị'),('LM02',N'Món chính'),('LM03',N'Tráng miệng');
INSERT INTO CongDung VALUES ('CD01',N'Bổ máu'),('CD02',N'Tăng lực'),('CD03',N'Giàu omega-3'),('CD04',N'Thanh nhiệt');
INSERT INTO DonViTinh VALUES ('DV01',N'kg'),('DV02',N'g'),('DV03',N'lít'),('DV04',N'quả');
INSERT INTO NoiHoc VALUES ('NH01',N'ĐH Công nghiệp Thực phẩm'),('NH02',N'CĐ Du lịch Hà Nội');
INSERT INTO TrinhDo VALUES ('TD01',N'Đại học'),('TD02',N'Cao đẳng'),('TD03',N'Trung cấp');
INSERT INTO XepLoai VALUES ('XL01',N'Xuất sắc'),('XL02',N'Giỏi'),('XL03',N'Khá');

INSERT INTO NguyenLieu(MaNguyenLieu,TenNguyenLieu,MaDonViTinh,MaCongDung,DonGia,DinhDuong) VALUES
('NL001',N'Thịt bò nạm','DV01','CD01',250000,N'Giàu đạm, sắt'),
('NL002',N'Tiêu xanh','DV02',NULL,400,NULL),
('NL003',N'Cà rốt','DV01',NULL,30000,N'Giàu vitamin A'),
('NL004',N'Cá hồi phi lê','DV01','CD03',520000,N'Giàu omega-3'),
('NL005',N'Tôm sú','DV01',NULL,300000,N'Giàu đạm'),
('NL006',N'Ngó sen','DV01','CD04',80000,NULL),
('NL007',N'Hạt sen','DV02','CD04',250,NULL),
('NL008',N'Long nhãn','DV02',NULL,200,NULL),
('NL009',N'Đường phèn','DV02',NULL,40,NULL);

INSERT INTO MonAn(MaMonAn,TenMonAn,MaLoai,MaCongDung,CachLam,YeuCau) VALUES
('MA001',N'Gỏi ngó sen tôm','LM01','CD04',N'Luộc tôm, trộn ngó sen với nước mắm chua ngọt',N'Dùng lạnh'),
('MA002',N'Bò hầm tiêu xanh','LM02','CD01',N'Hầm bò 90 phút với tiêu xanh và cà rốt',N'Dùng nóng'),
('MA003',N'Cá hồi áp chảo','LM02','CD03',N'Áp chảo 3 phút mỗi mặt',N'Dùng nóng'),
('MA004',N'Chè hạt sen long nhãn','LM03','CD04',N'Nấu hạt sen mềm rồi thêm long nhãn, đường phèn',N'Dùng lạnh');

-- Trigger trg_TinhDonGiaMonAn tự tính MonAn.DonGia sau các lệnh này
INSERT INTO NguyenLieu_MonAn(MaMonAn,MaNguyenLieu,SoLuong) VALUES
('MA001','NL005',0.2),('MA001','NL006',0.3),
('MA002','NL001',0.3),('MA002','NL002',50),('MA002','NL003',0.2),
('MA003','NL004',0.25),('MA003','NL002',10),
('MA004','NL007',100),('MA004','NL008',50),('MA004','NL009',30);

INSERT INTO DauBep(MaDauBep,TenDauBep,GioiTinh,DiaChi,DienThoai,MaTrinhDo,MaNoiHoc) VALUES
('DB001',N'Lê Thu Hà',N'Nữ',N'45 Hàng Bông, Hà Nội','0912345678','TD02','NH02'),
('DB002',N'Trần Minh Khôi',N'Nam',N'12 Lê Lợi, Hà Nội','0901234567','TD01','NH01');

INSERT INTO DauBep_MonAn(MaDauBep,MaMonAn,MaXepLoai) VALUES
('DB001','MA001','XL02'),('DB001','MA004','XL01'),
('DB002','MA002','XL01'),('DB002','MA003','XL02'),('DB002','MA001','XL03');

INSERT INTO KhachHang VALUES
('KH001',N'Nguyễn Văn An',N'8 Trần Hưng Đạo, Hà Nội','0987654321','0312345678'),
('KH002',N'Công ty TNHH ABC',N'100 Cầu Giấy, Hà Nội','0243111222','0109876543');

INSERT INTO ThucDon(SoThucDon,MaKhachHang,NgayDung,ThueVAT,GiamGia) VALUES
('TD0001','KH001','20260820',8,0),
('TD0002','KH002','20260920',10,50000),
('TD0003','KH001','20261003',8,0);

INSERT INTO ChiTietThucDon(SoThucDon,MaMonAn,MaDauBep,SoLuong,ThanhTien)
SELECT v.So, v.Mon, v.Bep, v.SL, v.SL * m.DonGia
FROM (VALUES ('TD0001','MA001','DB001',3),('TD0001','MA004','DB001',4),
             ('TD0002','MA002','DB002',2),('TD0002','MA003','DB002',1),('TD0002','MA004','DB001',5),
             ('TD0003','MA001','DB002',2),('TD0003','MA002','DB002',1)) v(So,Mon,Bep,SL)
JOIN MonAn m ON m.MaMonAn = v.Mon;

UPDATE td SET TongTien = CASE WHEN s.Tong*(1+td.ThueVAT/100.0)-td.GiamGia < 0 THEN 0
                              ELSE s.Tong*(1+td.ThueVAT/100.0)-td.GiamGia END
FROM ThucDon td
CROSS APPLY (SELECT ISNULL(SUM(ThanhTien),0) AS Tong FROM ChiTietThucDon WHERE SoThucDon=td.SoThucDon) s;
GO
-- Kiểm tra nhanh
SELECT MaMonAn, TenMonAn, DonGia FROM MonAn;
SELECT SoThucDon, NgayDung, TongTien FROM ThucDon;
