using System.Linq;
using SWP.Models;

namespace SWP.Services;

// QL01 — Quản lý tài khoản người dùng.
public static class NguoiDungService
{
    public static List<NguoiDung> LayDanhSach()
    {
        using var db = new QuanLyKhoVatTuContext();
        return db.NguoiDungs.OrderBy(u => u.TenDangNhap).ToList();
    }

    public static (bool Success, string? LoiMessage) LuuNguoiDung(NguoiDung user, string? matKhauNeuTaoMoi)
    {
        // Cố ý không dọn MatKhauHash (phải giữ nguyên ký tự đặc biệt) và DienThoai (cần luật riêng cho số).
        user.TenDangNhap = VanBanHelper.LamSachTenDangNhap(user.TenDangNhap);
        user.HoTen = VanBanHelper.LamSachVanBan(user.HoTen);

        if (user.TenDangNhap == "")
        {
            return (false, "Tên đăng nhập không được để trống.");
        }
        if (user.HoTen == "")
        {
            return (false, "Họ tên không được để trống.");
        }

        using var db = new QuanLyKhoVatTuContext();

        // RB01-tương tự: tên đăng nhập duy nhất
        if (db.NguoiDungs.Any(u => u.TenDangNhap == user.TenDangNhap && u.Id != user.Id))
            return (false, $"Tên đăng nhập '{user.TenDangNhap}' đã tồn tại.");

        if (user.Id == 0)
        {
            if (string.IsNullOrWhiteSpace(matKhauNeuTaoMoi))
                return (false, "Phải đặt mật khẩu ban đầu cho tài khoản mới.");

            user.MatKhauHash = matKhauNeuTaoMoi;
            user.NgayTao = DateTime.Now;
            db.NguoiDungs.Add(user);
            AuditLogService.Log(db, Session.CurrentUser, "THEM", "NguoiDung", null, $"Tạo tài khoản {user.TenDangNhap} ({user.VaiTro})");
        }
        else
        {
            var existing = db.NguoiDungs.Find(user.Id);
            if (existing is null)
            {
                return (false, "Không tìm thấy người dùng.");
            }

            existing.TenDangNhap = user.TenDangNhap;
            existing.HoTen = user.HoTen;
            existing.VaiTro = user.VaiTro;
            existing.DienThoai = user.DienThoai;
            existing.DangHoatDong = user.DangHoatDong;
            AuditLogService.Log(db, Session.CurrentUser, "SUA", "NguoiDung", user.Id, $"Sửa thông tin tài khoản {user.TenDangNhap}");
        }

        db.SaveChanges();
        return (true, null);
    }

    // Không xóa vật lý được vì phiếu nhập/xuất tham chiếu tài khoản làm người lập/người duyệt.
    public static void KhoaTaiKhoan(int id)
    {
        using var db = new QuanLyKhoVatTuContext();
        var user = db.NguoiDungs.Find(id);
        if (user is null)
        {
            return;
        }

        user.DangHoatDong = false;
        AuditLogService.Log(db, Session.CurrentUser, "SUA", "NguoiDung", id, $"Khóa tài khoản {user.TenDangNhap}");
        db.SaveChanges();
    }
}
