using System.Linq;
using Microsoft.EntityFrameworkCore;
using SWP.Models;

namespace SWP.Services;

// KT01-KT04 — CRUD 4 danh mục: SanPham, NhomHang, NhaCungCap, BoPhan.
// Không đụng cột TonKho, không cần ai duyệt.
public static class DanhMucService
{
    // ---------- SẢN PHẨM ----------

    public static List<SanPham> LaySanPham()
    {
        using var db = new QuanLyKhoVatTuContext();
        return db.SanPhams.Include(s => s.NhomHang).OrderBy(s => s.Ma).ToList();
    }

    public static (bool Success, string? LoiMessage) LuuSanPham(SanPham sp)
    {
        sp.Ma = VanBanHelper.LamSachMa(sp.Ma);
        sp.Ten = VanBanHelper.LamSachVanBan(sp.Ten);
        sp.DonViTinh = VanBanHelper.LamSachVanBan(sp.DonViTinh);
        sp.GhiChu = VanBanHelper.LamSachVanBan(sp.GhiChu);

        if (sp.Ma == "")
        {
            return (false, "Mã sản phẩm không được để trống.");
        }
        if (sp.Ten == "")
        {
            return (false, "Tên sản phẩm không được để trống.");
        }
        if (sp.DonViTinh == "")
        {
            return (false, "Đơn vị tính không được để trống.");
        }

        using var db = new QuanLyKhoVatTuContext();

        // RB01: mã sản phẩm duy nhất
        bool trungMa = db.SanPhams.Any(s => s.Ma == sp.Ma && s.Id != sp.Id);
        if (trungMa)
        {
            return (false, $"Mã sản phẩm '{sp.Ma}' đã tồn tại.");
        }

        if (sp.Id == 0)
        {
            sp.NgayTao = DateTime.Now;
            db.SanPhams.Add(sp);
            AuditLogService.Log(db, Session.CurrentUser, "THEM", "SanPham", null, $"Thêm sản phẩm {sp.Ma} - {sp.Ten}");
        }
        else
        {
            var existing = db.SanPhams.Find(sp.Id);
            if (existing is null)
            {
                return (false, "Không tìm thấy sản phẩm.");
            }

            // Cố ý KHÔNG chép TonKho: cột đó chỉ đổi qua phiếu nhập/xuất đã duyệt.
            // GiaVonBinhQuan thì cho sửa ở đây — 1 giá cố định, không tính bình quân gia quyền theo lô.
            existing.Ma = sp.Ma;
            existing.Ten = sp.Ten;
            existing.NhomHangId = sp.NhomHangId;
            existing.DonViTinh = sp.DonViTinh;
            existing.GiaVonBinhQuan = sp.GiaVonBinhQuan;
            existing.TonToiThieu = sp.TonToiThieu;
            existing.GhiChu = sp.GhiChu;
            existing.DangSuDung = sp.DangSuDung;

            AuditLogService.Log(db, Session.CurrentUser, "SUA", "SanPham", sp.Id, $"Sửa thông tin sản phẩm {sp.Ma}");
        }

        db.SaveChanges();
        return (true, null);
    }

    // RB11: không xóa sản phẩm đã phát sinh giao dịch, chỉ đánh dấu ngừng sử dụng.
    public static (bool Success, string? LoiMessage) NgungSuDungSanPham(int id)
    {
        using var db = new QuanLyKhoVatTuContext();
        var sp = db.SanPhams.Find(id);
        if (sp is null)
        {
            return (false, "Không tìm thấy sản phẩm.");
        }

        sp.DangSuDung = false;
        AuditLogService.Log(db, Session.CurrentUser, "SUA", "SanPham", id, $"Ngừng sử dụng sản phẩm {sp.Ma}");
        db.SaveChanges();
        return (true, null);
    }

    // ---------- NHÓM HÀNG ----------

    public static List<NhomHang> LayNhomHang()
    {
        using var db = new QuanLyKhoVatTuContext();
        return db.NhomHangs.OrderBy(n => n.Ma).ToList();
    }

    public static (bool Success, string? LoiMessage) LuuNhomHang(NhomHang nh)
    {
        nh.Ma = VanBanHelper.LamSachMa(nh.Ma);
        nh.Ten = VanBanHelper.LamSachVanBan(nh.Ten);
        nh.MoTa = VanBanHelper.LamSachVanBan(nh.MoTa);

        if (nh.Ma == "")
        {
            return (false, "Mã nhóm hàng không được để trống.");
        }
        if (nh.Ten == "")
        {
            return (false, "Tên nhóm hàng không được để trống.");
        }

        using var db = new QuanLyKhoVatTuContext();
        if (db.NhomHangs.Any(n => n.Ma == nh.Ma && n.Id != nh.Id))
            return (false, $"Mã nhóm hàng '{nh.Ma}' đã tồn tại.");

        if (nh.Id == 0)
        {
            db.NhomHangs.Add(nh);
            AuditLogService.Log(db, Session.CurrentUser, "THEM", "NhomHang", null, $"Thêm nhóm hàng {nh.Ma}");
        }
        else
        {
            var existing = db.NhomHangs.Find(nh.Id);
            if (existing is null)
            {
                return (false, "Không tìm thấy nhóm hàng.");
            }
            existing.Ma = nh.Ma;
            existing.Ten = nh.Ten;
            existing.MoTa = nh.MoTa;
            AuditLogService.Log(db, Session.CurrentUser, "SUA", "NhomHang", nh.Id, $"Sửa nhóm hàng {nh.Ma}");
        }
        db.SaveChanges();
        return (true, null);
    }

    // RB12: không xóa nhóm hàng đang được sản phẩm tham chiếu
    public static (bool Success, string? LoiMessage) XoaNhomHang(int id)
    {
        using var db = new QuanLyKhoVatTuContext();
        bool dangDuocDung = db.SanPhams.Any(s => s.NhomHangId == id);
        if (dangDuocDung)
        {
            return (false, "Không thể xóa: đang có sản phẩm thuộc nhóm hàng này.");
        }

        var nh = db.NhomHangs.Find(id);
        if (nh is null)
        {
            return (false, "Không tìm thấy nhóm hàng.");
        }

        db.NhomHangs.Remove(nh);
        AuditLogService.Log(db, Session.CurrentUser, "XOA", "NhomHang", id, $"Xóa nhóm hàng {nh.Ma}");
        db.SaveChanges();
        return (true, null);
    }

    // ---------- NHÀ CUNG CẤP ----------

    public static List<NhaCungCap> LayNhaCungCap()
    {
        using var db = new QuanLyKhoVatTuContext();
        return db.NhaCungCaps.OrderBy(n => n.Ma).ToList();
    }

    public static (bool Success, string? LoiMessage) LuuNhaCungCap(NhaCungCap ncc)
    {
        // DienThoai và MaSoThue cố ý không dọn — cần luật riêng cho chuỗi số.
        ncc.Ma = VanBanHelper.LamSachMa(ncc.Ma);
        ncc.Ten = VanBanHelper.LamSachVanBan(ncc.Ten);
        ncc.DiaChi = VanBanHelper.LamSachVanBan(ncc.DiaChi);
        ncc.NguoiLienHe = VanBanHelper.LamSachVanBan(ncc.NguoiLienHe);

        if (ncc.Ma == "")
        {
            return (false, "Mã nhà cung cấp không được để trống.");
        }
        if (ncc.Ten == "")
        {
            return (false, "Tên nhà cung cấp không được để trống.");
        }

        using var db = new QuanLyKhoVatTuContext();
        if (db.NhaCungCaps.Any(n => n.Ma == ncc.Ma && n.Id != ncc.Id))
            return (false, $"Mã nhà cung cấp '{ncc.Ma}' đã tồn tại.");

        if (ncc.Id == 0)
        {
            db.NhaCungCaps.Add(ncc);
            AuditLogService.Log(db, Session.CurrentUser, "THEM", "NhaCungCap", null, $"Thêm nhà cung cấp {ncc.Ma}");
        }
        else
        {
            var existing = db.NhaCungCaps.Find(ncc.Id);
            if (existing is null)
            {
                return (false, "Không tìm thấy nhà cung cấp.");
            }
            existing.Ma = ncc.Ma;
            existing.Ten = ncc.Ten;
            existing.MaSoThue = ncc.MaSoThue;
            existing.DiaChi = ncc.DiaChi;
            existing.DienThoai = ncc.DienThoai;
            existing.NguoiLienHe = ncc.NguoiLienHe;
            existing.DangHoatDong = ncc.DangHoatDong;
            AuditLogService.Log(db, Session.CurrentUser, "SUA", "NhaCungCap", ncc.Id, $"Sửa nhà cung cấp {ncc.Ma}");
        }
        db.SaveChanges();
        return (true, null);
    }

    // RB12: không xóa nhà cung cấp đang được phiếu nhập tham chiếu — chỉ được ngừng hoạt động
    public static (bool Success, string? LoiMessage) NgungHoatDongNhaCungCap(int id)
    {
        using var db = new QuanLyKhoVatTuContext();
        var ncc = db.NhaCungCaps.Find(id);
        if (ncc is null)
        {
            return (false, "Không tìm thấy nhà cung cấp.");
        }

        ncc.DangHoatDong = false;
        AuditLogService.Log(db, Session.CurrentUser, "SUA", "NhaCungCap", id, $"Ngừng hoạt động nhà cung cấp {ncc.Ma}");
        db.SaveChanges();
        return (true, null);
    }

    // ---------- BỘ PHẬN ----------

    public static List<BoPhan> LayBoPhan()
    {
        using var db = new QuanLyKhoVatTuContext();
        return db.BoPhans.OrderBy(b => b.Ma).ToList();
    }

    public static (bool Success, string? LoiMessage) LuuBoPhan(BoPhan bp)
    {
        bp.Ma = VanBanHelper.LamSachMa(bp.Ma);
        bp.Ten = VanBanHelper.LamSachVanBan(bp.Ten);
        bp.TruongBoPhan = VanBanHelper.LamSachVanBan(bp.TruongBoPhan);

        if (bp.Ma == "")
        {
            return (false, "Mã bộ phận không được để trống.");
        }
        if (bp.Ten == "")
        {
            return (false, "Tên bộ phận không được để trống.");
        }

        using var db = new QuanLyKhoVatTuContext();
        if (db.BoPhans.Any(b => b.Ma == bp.Ma && b.Id != bp.Id))
            return (false, $"Mã bộ phận '{bp.Ma}' đã tồn tại.");

        if (bp.Id == 0)
        {
            db.BoPhans.Add(bp);
            AuditLogService.Log(db, Session.CurrentUser, "THEM", "BoPhan", null, $"Thêm bộ phận {bp.Ma}");
        }
        else
        {
            var existing = db.BoPhans.Find(bp.Id);
            if (existing is null)
            {
                return (false, "Không tìm thấy bộ phận.");
            }
            existing.Ma = bp.Ma;
            existing.Ten = bp.Ten;
            existing.TruongBoPhan = bp.TruongBoPhan;
            existing.SoNhanSu = bp.SoNhanSu;
            existing.DangHoatDong = bp.DangHoatDong;
            AuditLogService.Log(db, Session.CurrentUser, "SUA", "BoPhan", bp.Id, $"Sửa bộ phận {bp.Ma}");
        }
        db.SaveChanges();
        return (true, null);
    }

    public static (bool Success, string? LoiMessage) NgungHoatDongBoPhan(int id)
    {
        using var db = new QuanLyKhoVatTuContext();
        var bp = db.BoPhans.Find(id);
        if (bp is null)
        {
            return (false, "Không tìm thấy bộ phận.");
        }

        bp.DangHoatDong = false;
        AuditLogService.Log(db, Session.CurrentUser, "SUA", "BoPhan", id, $"Ngừng hoạt động bộ phận {bp.Ma}");
        db.SaveChanges();
        return (true, null);
    }
}
