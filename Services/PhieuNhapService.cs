using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SWP.Models;

namespace SWP.Services;

// Dữ liệu 1 dòng chi tiết nhận từ giao diện — chưa phải Entity của EF.
public class ChiTietNhapInput
{
    public int SanPhamId { get; set; }
    public decimal SoLuong { get; set; }
    public decimal DonGia { get; set; }
}

public static class PhieuNhapService
{
    public static List<PhieuNhap> LayDanhSach()
    {
        using var db = new QuanLyKhoVatTuContext();
        return db.PhieuNhaps
            .Include(p => p.NhaCungCap)
            .Include(p => p.NguoiLap)
            .Include(p => p.NguoiDuyet)
            .OrderByDescending(p => p.NgayLap)
            .ToList();
    }

    public static PhieuNhap? LayChiTiet(int id)
    {
        using var db = new QuanLyKhoVatTuContext();
        return db.PhieuNhaps
            .Include(p => p.ChiTietPhieuNhaps).ThenInclude(ct => ct.SanPham)
            .Include(p => p.NhaCungCap)
            .Include(p => p.NguoiLap)
            .Include(p => p.NguoiDuyet)
            .FirstOrDefault(p => p.Id == id);
    }

    // TK01 — Thủ kho lập phiếu nhập. Trạng thái ban đầu luôn là CHODUYET (RB09: chưa duyệt thì chưa động vào tồn kho).
    public static (bool Success, string? LoiMessage) TaoPhieu(
        DateTime ngayNhap, int? nhaCungCapId, string? soHoaDon, string? nguoiGiaoHang, string? ghiChu,
        List<ChiTietNhapInput> chiTiet)
    {
        soHoaDon = VanBanHelper.LamSachChuVaSo(soHoaDon);
        nguoiGiaoHang = VanBanHelper.LamSachChuVaSo(nguoiGiaoHang);
        ghiChu = VanBanHelper.LamSachVanBan(ghiChu); // Ghi chú: được miễn, cho phép dấu câu

        if (soHoaDon == "")
        {
            return (false, "Số hóa đơn không được để trống.");
        }
        if (nguoiGiaoHang == "")
        {
            return (false, "Người giao hàng không được để trống.");
        }

        // RB07: phải có tối thiểu 1 dòng chi tiết
        if (chiTiet is null || chiTiet.Count == 0)
            return (false, "Phiếu phải có ít nhất một dòng chi tiết.");

        // RB08: một sản phẩm không được xuất hiện quá 1 lần trong cùng phiếu
        if (chiTiet.Select(c => c.SanPhamId).Distinct().Count() != chiTiet.Count)
            return (false, "Một sản phẩm không được xuất hiện hai lần trong cùng phiếu.");

        foreach (var ct in chiTiet)
        {
            // RB03: số lượng, đơn giá phải > 0
            if (ct.SoLuong <= 0 || ct.DonGia <= 0)
                return (false, "Số lượng và đơn giá phải lớn hơn 0.");
            if (ct.SoLuong > 10000)
                return (false, "Số lượng nhập không được vượt quá 10.000.");
        }

        var nguoiDangNhap = Session.CurrentUser;
        if (nguoiDangNhap == null)
            return (false, "Chưa đăng nhập.");

        using var db = new QuanLyKhoVatTuContext();

        // Số phiếu mẫu PN20260819001 — đếm lại từ 001 mỗi ngày (mục 5.1 đặc tả).
        var homNay = DateTime.Today;
        var soPhieuHomNay = db.PhieuNhaps.Count(p => p.NgayLap.Date == homNay);
        var soPhieu = "PN" + homNay.ToString("yyyyMMdd") + (soPhieuHomNay + 1).ToString("000");

        var phieu = new PhieuNhap();
        phieu.SoPhieu = soPhieu;
        phieu.NgayNhap = ngayNhap;
        phieu.NhaCungCapId = nhaCungCapId;
        phieu.SoHoaDon = soHoaDon;
        phieu.NguoiGiaoHang = nguoiGiaoHang;
        phieu.GhiChu = ghiChu;
        phieu.TrangThai = "CHODUYET";
        phieu.NguoiLapId = nguoiDangNhap.Id;
        phieu.NgayLap = DateTime.Now;
        phieu.TongTien = chiTiet.Sum(c => c.SoLuong * c.DonGia);

        foreach (var ct in chiTiet)
        {
            var dongChiTiet = new ChiTietPhieuNhap();
            dongChiTiet.SanPhamId = ct.SanPhamId;
            dongChiTiet.SoLuong = ct.SoLuong;
            dongChiTiet.DonGia = ct.DonGia;
            dongChiTiet.ThanhTien = ct.SoLuong * ct.DonGia;
            phieu.ChiTietPhieuNhaps.Add(dongChiTiet);
        }

        db.PhieuNhaps.Add(phieu);
        AuditLogService.Log(db, Session.CurrentUser, "THEM", "PhieuNhap", null, $"Lập phiếu nhập, chờ duyệt");
        db.SaveChanges();

        return (true, null);
    }

    // KT05 — Kế toán duyệt phiếu. Đây là lúc DUY NHẤT tồn kho được cộng (RB09).
    public static (bool Success, string? LoiMessage) DuyetPhieu(int phieuId)
    {
        var nguoiDangNhap = Session.CurrentUser;
        if (nguoiDangNhap == null)
            return (false, "Chưa đăng nhập.");

        using var db = new QuanLyKhoVatTuContext();
        var phieu = db.PhieuNhaps
            .Include(p => p.ChiTietPhieuNhaps).ThenInclude(ct => ct.SanPham)
            .FirstOrDefault(p => p.Id == phieuId);

        if (phieu is null)
        {
            return (false, "Không tìm thấy phiếu.");
        }
        if (phieu.TrangThai != "CHODUYET")
        {
            return (false, "Chỉ có thể duyệt phiếu đang ở trạng thái Chờ duyệt.");
        }

        // RB10: người lập không được đồng thời là người duyệt
        if (phieu.NguoiLapId == nguoiDangNhap.Id)
            return (false, "Người lập phiếu không được tự duyệt phiếu của mình.");

        // Chỉ cộng số lượng, KHÔNG đụng tới giá: giá là 1 con số cố định đặt sẵn ở màn Danh mục,
        // không tính bình quân gia quyền theo từng lô nhập.
        foreach (var ct in phieu.ChiTietPhieuNhaps)
        {
            ct.SanPham.TonKho += ct.SoLuong;
        }

        phieu.TrangThai = "DADUYET";
        phieu.NguoiDuyetId = nguoiDangNhap.Id;
        phieu.NgayDuyet = DateTime.Now;

        AuditLogService.Log(db, Session.CurrentUser, "DUYET", "PhieuNhap", phieu.Id, $"Duyệt phiếu nhập {phieu.SoPhieu}, cộng tồn kho");
        db.SaveChanges();

        return (true, null);
    }

    // KT05 (từ chối) / KT07 (hủy phiếu đã duyệt) — chung 1 hàm vì cả hai đều đưa phiếu về DAHUY.
    public static (bool Success, string? LoiMessage) HuyPhieu(int phieuId, string lyDo)
    {
        lyDo = VanBanHelper.LamSachVanBan(lyDo);
        if (lyDo == "")
            return (false, "Phải nhập lý do hủy/từ chối.");

        using var db = new QuanLyKhoVatTuContext();
        var phieu = db.PhieuNhaps
            .Include(p => p.ChiTietPhieuNhaps).ThenInclude(ct => ct.SanPham)
            .FirstOrDefault(p => p.Id == phieuId);

        if (phieu is null)
        {
            return (false, "Không tìm thấy phiếu.");
        }
        if (phieu.TrangThai == "DAHUY")
        {
            return (false, "Phiếu đã bị hủy trước đó.");
        }

        // RB13: nếu phiếu đã cộng tồn kho (đã duyệt) thì phải trừ lại đúng số đã cộng
        if (phieu.TrangThai == "DADUYET")
        {
            foreach (var ct in phieu.ChiTietPhieuNhaps)
                ct.SanPham.TonKho -= ct.SoLuong;
        }

        phieu.TrangThai = "DAHUY";
        phieu.LyDoHuy = lyDo;

        AuditLogService.Log(db, Session.CurrentUser, "HUY", "PhieuNhap", phieu.Id, $"Hủy/từ chối phiếu nhập {phieu.SoPhieu}: {lyDo}");
        db.SaveChanges();

        return (true, null);
    }
}
