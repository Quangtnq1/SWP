using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SWP.Models;

namespace SWP.Services;

// Dữ liệu 1 dòng chi tiết nhận từ giao diện — lúc lập chỉ có số lượng, đơn giá điền khi kế toán duyệt.
public class ChiTietXuatInput
{
    public int SanPhamId { get; set; }
    public decimal SoLuongYeuCau { get; set; }
}

public static class PhieuXuatService
{
    public static List<PhieuXuat> LayDanhSach()
    {
        using var db = new QuanLyKhoVatTuContext();
        return db.PhieuXuats
            .Include(p => p.BoPhan)
            .Include(p => p.NguoiLap)
            .Include(p => p.NguoiDuyet)
            .OrderByDescending(p => p.NgayLap)
            .ToList();
    }

    public static PhieuXuat? LayChiTiet(int id)
    {
        using var db = new QuanLyKhoVatTuContext();
        return db.PhieuXuats
            .Include(p => p.ChiTietPhieuXuats).ThenInclude(ct => ct.SanPham)
            .Include(p => p.BoPhan)
            .Include(p => p.NguoiLap)
            .Include(p => p.NguoiDuyet)
            .FirstOrDefault(p => p.Id == id);
    }

    // TK02 — Thủ kho lập phiếu xuất, chỉ ghi số lượng. Đơn giá/thành tiền để trống, điền lúc kế toán duyệt.
    public static (bool Success, string? LoiMessage) TaoPhieu(
        DateTime ngayXuat, string loaiXuat, int boPhanId, string nguoiNhan, string lyDoXuat, string? ghiChu,
        List<ChiTietXuatInput> chiTiet)
    {
        nguoiNhan = VanBanHelper.LamSachChuVaSo(nguoiNhan);
        lyDoXuat = VanBanHelper.LamSachVanBan(lyDoXuat); // Lý do: được miễn, cho phép dấu câu
        ghiChu = VanBanHelper.LamSachVanBan(ghiChu); // Ghi chú: được miễn, cho phép dấu câu

        if (nguoiNhan == "")
        {
            return (false, "Người nhận không được để trống.");
        }
        if (lyDoXuat == "")
        {
            return (false, "Lý do xuất không được để trống.");
        }

        if (chiTiet is null || chiTiet.Count == 0)
            return (false, "Phiếu phải có ít nhất một dòng chi tiết.");

        if (chiTiet.Select(c => c.SanPhamId).Distinct().Count() != chiTiet.Count)
            return (false, "Một sản phẩm không được xuất hiện hai lần trong cùng phiếu.");

        foreach (var ct in chiTiet)
        {
            if (ct.SoLuongYeuCau <= 0)
                return (false, "Số lượng yêu cầu phải lớn hơn 0.");
            if (ct.SoLuongYeuCau > 10000)
                return (false, "Số lượng yêu cầu không được vượt quá 10.000.");
        }

        var nguoiDangNhap = Session.CurrentUser;
        if (nguoiDangNhap == null)
            return (false, "Chưa đăng nhập.");

        using var db = new QuanLyKhoVatTuContext();

        // Số phiếu mẫu PX20260819007 — đếm lại từ 001 mỗi ngày (mục 5.1 đặc tả).
        var homNay = DateTime.Today;
        var soPhieuHomNay = db.PhieuXuats.Count(p => p.NgayLap.Date == homNay);
        var soPhieu = "PX" + homNay.ToString("yyyyMMdd") + (soPhieuHomNay + 1).ToString("000");

        var phieu = new PhieuXuat();
        phieu.SoPhieu = soPhieu;
        phieu.NgayXuat = ngayXuat;
        phieu.LoaiXuat = loaiXuat;
        phieu.BoPhanId = boPhanId;
        phieu.NguoiNhan = nguoiNhan;
        phieu.LyDoXuat = lyDoXuat;
        phieu.GhiChu = ghiChu;
        phieu.TrangThai = "CHODUYET";
        phieu.NguoiLapId = nguoiDangNhap.Id;
        phieu.NgayLap = DateTime.Now;
        phieu.TongTien = null; // điền lúc kế toán duyệt

        foreach (var ct in chiTiet)
        {
            var dongChiTiet = new ChiTietPhieuXuat();
            dongChiTiet.SanPhamId = ct.SanPhamId;
            dongChiTiet.SoLuongYeuCau = ct.SoLuongYeuCau;
            phieu.ChiTietPhieuXuats.Add(dongChiTiet);
        }

        db.PhieuXuats.Add(phieu);
        AuditLogService.Log(db, Session.CurrentUser, "THEM", "PhieuXuat", null, "Lập phiếu xuất, chờ duyệt");
        db.SaveChanges();

        return (true, null);
    }

    // KT06 — Kế toán duyệt phiếu xuất. Đây là lúc DUY NHẤT tồn kho bị TRỪ.
    public static (bool Success, string? LoiMessage) DuyetPhieu(int phieuId)
    {
        var nguoiDangNhap = Session.CurrentUser;
        if (nguoiDangNhap == null)
            return (false, "Chưa đăng nhập.");

        using var db = new QuanLyKhoVatTuContext();
        var phieu = db.PhieuXuats
            .Include(p => p.ChiTietPhieuXuats).ThenInclude(ct => ct.SanPham)
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

        // RB06: kiểm ĐỦ HÀNG cho toàn bộ các dòng TRƯỚC khi trừ bất kỳ dòng nào — nếu vừa kiểm vừa trừ,
        // gặp dòng thiếu hàng ở giữa sẽ để lại dữ liệu nửa vời.
        foreach (var ct in phieu.ChiTietPhieuXuats)
        {
            if (ct.SoLuongYeuCau > ct.SanPham.TonKho)
                return (false, $"Không đủ tồn kho cho '{ct.SanPham.Ten}'. Cần {ct.SoLuongYeuCau} nhưng chỉ còn {ct.SanPham.TonKho} {ct.SanPham.DonViTinh}.");
        }

        foreach (var ct in phieu.ChiTietPhieuXuats)
        {
            ct.SoLuongThucXuat = ct.SoLuongYeuCau;
            ct.DonGia = ct.SanPham.GiaVonBinhQuan;
            ct.ThanhTien = ct.SoLuongYeuCau * ct.SanPham.GiaVonBinhQuan;

            ct.SanPham.TonKho -= ct.SoLuongYeuCau;
        }

        phieu.TongTien = phieu.ChiTietPhieuXuats.Sum(c => c.SoLuongYeuCau * c.SanPham.GiaVonBinhQuan);
        phieu.TrangThai = "DADUYET";
        phieu.NguoiDuyetId = nguoiDangNhap.Id;
        phieu.NgayDuyet = DateTime.Now;

        AuditLogService.Log(db, Session.CurrentUser, "DUYET", "PhieuXuat", phieu.Id, $"Duyệt phiếu xuất {phieu.SoPhieu}, trừ tồn kho");
        db.SaveChanges();

        return (true, null);
    }

    // KT06 (từ chối) / KT07 (hủy phiếu đã duyệt) — chung 1 hàm vì cả hai đều đưa phiếu về DAHUY.
    public static (bool Success, string? LoiMessage) HuyPhieu(int phieuId, string lyDo)
    {
        lyDo = VanBanHelper.LamSachVanBan(lyDo);
        if (lyDo == "")
            return (false, "Phải nhập lý do hủy/từ chối.");

        using var db = new QuanLyKhoVatTuContext();
        var phieu = db.PhieuXuats
            .Include(p => p.ChiTietPhieuXuats).ThenInclude(ct => ct.SanPham)
            .FirstOrDefault(p => p.Id == phieuId);

        if (phieu is null)
        {
            return (false, "Không tìm thấy phiếu.");
        }
        if (phieu.TrangThai == "DAHUY")
        {
            return (false, "Phiếu đã bị hủy trước đó.");
        }

        // RB13: chỉ phiếu ĐÃ DUYỆT mới từng bị trừ kho nên mới cần hoàn lại. Phiếu CHODUYET chưa đụng tới kho.
        if (phieu.TrangThai == "DADUYET")
        {
            foreach (var ct in phieu.ChiTietPhieuXuats)
                ct.SanPham.TonKho += ct.SoLuongYeuCau;
        }

        phieu.TrangThai = "DAHUY";
        phieu.LyDoHuy = lyDo;

        AuditLogService.Log(db, Session.CurrentUser, "HUY", "PhieuXuat", phieu.Id, $"Hủy/từ chối phiếu xuất {phieu.SoPhieu}: {lyDo}");
        db.SaveChanges();

        return (true, null);
    }
}
