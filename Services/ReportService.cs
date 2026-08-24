using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SWP.Models;

namespace SWP.Services;

// KT09 — 4 báo cáo theo mục 3.2 (danh mục báo cáo thuộc KT09).
public class NhapXuatTonRow
{
    public string Ma { get; set; } = "";
    public string Ten { get; set; } = "";
    public string DonViTinh { get; set; } = "";
    public decimal TonDauKy { get; set; }
    public decimal TongNhap { get; set; }
    public decimal TongXuat { get; set; }
    public decimal TonCuoiKy { get; set; }
}

public class TheKhoRow
{
    public string MaSanPham { get; set; } = "";
    public string TenSanPham { get; set; } = "";
    public DateTime Ngay { get; set; }
    public string LoaiGiaoDich { get; set; } = ""; // Nhập / Xuất / Điều chỉnh
    public string SoChungTu { get; set; } = "";
    public decimal SoLuongNhap { get; set; }
    public decimal SoLuongXuat { get; set; }
    public decimal TonLuyKe { get; set; }
    public string GhiChu { get; set; } = "";

    public string SanPhamHienThi
    {
        get { return MaSanPham + " - " + TenSanPham; }
    }
}

public class ChiPhiBoPhanRow
{
    public string BoPhan { get; set; } = "";
    public string NhomHang { get; set; } = "";
    public decimal ThanhTien { get; set; }
}

public static class ReportService
{
    // Tồn đầu kỳ được dựng lại bằng cách lùi ngược từ tồn hiện tại qua các phiếu ĐÃ DUYỆT
    // (database không lưu sẵn tồn theo từng mốc thời gian).
    public static List<NhapXuatTonRow> NhapXuatTon(DateTime tuNgay, DateTime denNgay)
    {
        using var db = new QuanLyKhoVatTuContext();
        var denNgayCuoiNgay = denNgay.Date.AddDays(1).AddTicks(-1);

        var sanPhams = db.SanPhams.Where(s => s.DangSuDung).ToList();
        var result = new List<NhapXuatTonRow>();

        foreach (var sp in sanPhams)
        {
            // .Sum() trên cột nullable trả về decimal? (null nếu không có dòng nào khớp Where) -> quy về 0.
            var nhapTrongKyNullable = db.ChiTietPhieuNhaps
                .Where(ct => ct.SanPhamId == sp.Id && ct.PhieuNhap.TrangThai == "DADUYET"
                             && ct.PhieuNhap.NgayNhap >= tuNgay.Date && ct.PhieuNhap.NgayNhap <= denNgayCuoiNgay)
                .Sum(ct => (decimal?)ct.SoLuong);
            decimal nhapTrongKy;
            if (nhapTrongKyNullable == null)
                nhapTrongKy = 0;
            else
                nhapTrongKy = nhapTrongKyNullable.Value;

            var nhapSauKyNullable = db.ChiTietPhieuNhaps
                .Where(ct => ct.SanPhamId == sp.Id && ct.PhieuNhap.TrangThai == "DADUYET"
                             && ct.PhieuNhap.NgayNhap > denNgayCuoiNgay)
                .Sum(ct => (decimal?)ct.SoLuong);
            decimal nhapSauKy;
            if (nhapSauKyNullable == null)
                nhapSauKy = 0;
            else
                nhapSauKy = nhapSauKyNullable.Value;

            var xuatTrongKyNullable = db.ChiTietPhieuXuats
                .Where(ct => ct.SanPhamId == sp.Id && ct.PhieuXuat.TrangThai == "DADUYET" && ct.SoLuongThucXuat != null
                             && ct.PhieuXuat.NgayXuat >= tuNgay.Date && ct.PhieuXuat.NgayXuat <= denNgayCuoiNgay)
                .Sum(ct => (decimal?)ct.SoLuongThucXuat);
            decimal xuatTrongKy;
            if (xuatTrongKyNullable == null)
                xuatTrongKy = 0;
            else
                xuatTrongKy = xuatTrongKyNullable.Value;

            var xuatSauKyNullable = db.ChiTietPhieuXuats
                .Where(ct => ct.SanPhamId == sp.Id && ct.PhieuXuat.TrangThai == "DADUYET" && ct.SoLuongThucXuat != null
                             && ct.PhieuXuat.NgayXuat > denNgayCuoiNgay)
                .Sum(ct => (decimal?)ct.SoLuongThucXuat);
            decimal xuatSauKy;
            if (xuatSauKyNullable == null)
                xuatSauKy = 0;
            else
                xuatSauKy = xuatSauKyNullable.Value;

            var tonDauKy = sp.TonKho - nhapTrongKy - nhapSauKy + xuatTrongKy + xuatSauKy;
            var tonCuoiKy = tonDauKy + nhapTrongKy - xuatTrongKy;

            // Chỉ liệt kê sản phẩm có phát sinh hoặc có tồn, tránh báo cáo dài không cần thiết
            if (nhapTrongKy == 0 && xuatTrongKy == 0 && tonDauKy == 0 && tonCuoiKy == 0)
            {
                continue;
            }

            var dongKetQua = new NhapXuatTonRow();
            dongKetQua.Ma = sp.Ma;
            dongKetQua.Ten = sp.Ten;
            dongKetQua.DonViTinh = sp.DonViTinh;
            dongKetQua.TonDauKy = tonDauKy;
            dongKetQua.TongNhap = nhapTrongKy;
            dongKetQua.TongXuat = xuatTrongKy;
            dongKetQua.TonCuoiKy = tonCuoiKy;
            result.Add(dongKetQua);
        }

        return result.OrderBy(r => r.Ma).ToList();
    }

    // Thẻ kho: lịch sử biến động kèm tồn lũy kế.
    // sanPhamId = null nghĩa là xem GỘP tất cả sản phẩm đang sử dụng, xếp theo mã tăng dần.
    public static List<TheKhoRow> TheKho(int? sanPhamId, DateTime tuNgay, DateTime denNgay)
    {
        using var db = new QuanLyKhoVatTuContext();
        var denNgayCuoiNgay = denNgay.Date.AddDays(1).AddTicks(-1);

        List<SanPham> sanPhams;
        if (sanPhamId == null)
        {
            sanPhams = db.SanPhams.Where(s => s.DangSuDung).OrderBy(s => s.Ma).ToList();
        }
        else
        {
            var motSanPham = db.SanPhams.Find(sanPhamId.Value);
            if (motSanPham is null)
            {
                return new List<TheKhoRow>();
            }
            sanPhams = new List<SanPham>();
            sanPhams.Add(motSanPham);
        }

        var ketQua = new List<TheKhoRow>();
        foreach (var sp in sanPhams)
        {
            // Lấy TOÀN BỘ lịch sử, không giới hạn ngày — có đủ từ đầu mới cộng dồn ra tồn lũy kế đúng.
            // Việc lọc theo khoảng ngày để tận cuối hàm mới làm.
            var chiTietNhap = db.ChiTietPhieuNhaps
                .Include(ct => ct.PhieuNhap)
                .Where(ct => ct.SanPhamId == sp.Id && ct.PhieuNhap.TrangThai == "DADUYET")
                .ToList();

            var nhap = new List<TheKhoRow>();
            foreach (var ct in chiTietNhap)
            {
                var dongKetQua = new TheKhoRow();
                dongKetQua.MaSanPham = sp.Ma;
                dongKetQua.TenSanPham = sp.Ten;
                dongKetQua.Ngay = ct.PhieuNhap.NgayNhap;
                dongKetQua.LoaiGiaoDich = "Nhập";
                dongKetQua.SoChungTu = ct.PhieuNhap.SoPhieu;
                dongKetQua.SoLuongNhap = ct.SoLuong;
                dongKetQua.SoLuongXuat = 0;
                dongKetQua.GhiChu = "";
                nhap.Add(dongKetQua);
            }

            // .ToList() trước rồi mới dựng TheKhoRow bằng vòng lặp: viết ".Value" thẳng trong .Select()
            // sẽ dính cảnh báo CS8629, mà project đã bỏ hết "!" và "??" để chữa.
            var chiTietXuat = db.ChiTietPhieuXuats
                .Include(ct => ct.PhieuXuat)
                .Where(ct => ct.SanPhamId == sp.Id && ct.PhieuXuat.TrangThai == "DADUYET" && ct.SoLuongThucXuat != null)
                .ToList();

            var xuat = new List<TheKhoRow>();
            foreach (var ct in chiTietXuat)
            {
                decimal soLuongDaXuat;
                if (ct.SoLuongThucXuat == null)
                    soLuongDaXuat = 0;
                else
                    soLuongDaXuat = ct.SoLuongThucXuat.Value;

                var dongKetQua = new TheKhoRow();
                dongKetQua.MaSanPham = sp.Ma;
                dongKetQua.TenSanPham = sp.Ten;
                dongKetQua.Ngay = ct.PhieuXuat.NgayXuat;
                dongKetQua.LoaiGiaoDich = "Xuất";
                dongKetQua.SoChungTu = ct.PhieuXuat.SoPhieu;
                dongKetQua.SoLuongNhap = 0;
                dongKetQua.SoLuongXuat = soLuongDaXuat;
                dongKetQua.GhiChu = ct.PhieuXuat.LyDoXuat;
                xuat.Add(dongKetQua);
            }

            var toanBo = nhap.Concat(xuat).OrderBy(r => r.Ngay).ToList();

            decimal tonLuyKe = 0;
            foreach (var row in toanBo)
            {
                tonLuyKe += row.SoLuongNhap - row.SoLuongXuat;
                row.TonLuyKe = tonLuyKe;
            }

            var trongKhoangNgay = toanBo.Where(r => r.Ngay >= tuNgay.Date && r.Ngay <= denNgayCuoiNgay)
                .OrderByDescending(r => r.Ngay)
                .ToList();

            ketQua.AddRange(trongKhoangNgay);
        }

        return ketQua;
    }

    // Chi phí vật tư theo bộ phận: tổng giá trị xuất kho, nhóm theo bộ phận + nhóm hàng.
    public static List<ChiPhiBoPhanRow> ChiPhiTheoBoPhan(DateTime tuNgay, DateTime denNgay)
    {
        using var db = new QuanLyKhoVatTuContext();
        var denNgayCuoiNgay = denNgay.Date.AddDays(1).AddTicks(-1);

        var query = db.ChiTietPhieuXuats
            .Include(ct => ct.PhieuXuat).ThenInclude(px => px.BoPhan)
            .Include(ct => ct.SanPham).ThenInclude(sp => sp.NhomHang)
            .Where(ct => ct.PhieuXuat.TrangThai == "DADUYET" && ct.SoLuongThucXuat != null
                         && ct.PhieuXuat.NgayXuat >= tuNgay.Date && ct.PhieuXuat.NgayXuat <= denNgayCuoiNgay)
            .ToList();

        // query đã .ToList() nên đoạn dưới chạy trong bộ nhớ, EF không phải dịch sang SQL —
        // nhờ vậy mới viết được if/else bên trong lambda.
        var nhomTheoBoPhanVaNhomHang = query.GroupBy(ct =>
        {
            string tenNhomHang;
            if (ct.SanPham.NhomHang == null)
                tenNhomHang = "(Chưa phân nhóm)";
            else
                tenNhomHang = ct.SanPham.NhomHang.Ten;
            return new { BoPhan = ct.PhieuXuat.BoPhan.Ten, NhomHang = tenNhomHang };
        });

        var ketQua = new List<ChiPhiBoPhanRow>();
        foreach (var nhom in nhomTheoBoPhanVaNhomHang)
        {
            decimal tongThanhTien = 0;
            foreach (var ct in nhom)
            {
                if (ct.ThanhTien != null)
                    tongThanhTien += ct.ThanhTien.Value;
            }

            var dongKetQua = new ChiPhiBoPhanRow();
            dongKetQua.BoPhan = nhom.Key.BoPhan;
            dongKetQua.NhomHang = nhom.Key.NhomHang;
            dongKetQua.ThanhTien = tongThanhTien;
            ketQua.Add(dongKetQua);
        }

        return ketQua.OrderBy(r => r.BoPhan).ThenBy(r => r.NhomHang).ToList();
    }

    // Hàng dưới định mức tồn tối thiểu — dùng chung cho TK07 và báo cáo KT09.
    public static List<SanPham> HangDuoiDinhMuc()
    {
        using var db = new QuanLyKhoVatTuContext();
        return db.SanPhams
            .Include(s => s.NhomHang)
            .Where(s => s.DangSuDung && s.TonKho < s.TonToiThieu)
            .OrderBy(s => s.Ma)
            .ToList();
    }
}
