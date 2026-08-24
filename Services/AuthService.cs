using System.Linq;
using Microsoft.EntityFrameworkCore;
using SWP.Models;

namespace SWP.Services;

// CN02 (đổi mật khẩu) / QL02 (đặt lại mật khẩu).
// LƯU Ý: đồ án học tập nên mật khẩu lưu dạng CHỮ RÕ, không băm. Dự án thật phải băm kèm muối (BCrypt).
public static class AuthService
{
    public static (bool Success, string? LoiMessage) DoiMatKhau(int userId, string matKhauCu, string matKhauMoi)
    {
        using var db = new QuanLyKhoVatTuContext();
        var user = db.NguoiDungs.Find(userId);
        if (user is null)
        {
            return (false, "Không tìm thấy người dùng.");
        }
        if (user.MatKhauHash != matKhauCu)
        {
            return (false, "Mật khẩu hiện tại không đúng.");
        }
        if (string.IsNullOrWhiteSpace(matKhauMoi))
        {
            return (false, "Mật khẩu mới không được để trống.");
        }

        user.MatKhauHash = matKhauMoi;
        AuditLogService.Log(db, Session.CurrentUser, "SUA", "NguoiDung", user.Id, "Tự đổi mật khẩu");
        db.SaveChanges();
        return (true, null);
    }

    // Quản lý đặt lại mật khẩu cho người khác — không cần biết mật khẩu cũ.
    public static void DatLaiMatKhau(int userId, string matKhauMoi)
    {
        using var db = new QuanLyKhoVatTuContext();
        var user = db.NguoiDungs.Find(userId);
        if (user is null)
        {
            return;
        }

        user.MatKhauHash = matKhauMoi;
        AuditLogService.Log(db, Session.CurrentUser, "SUA", "NguoiDung", user.Id, $"Quản lý đặt lại mật khẩu cho '{user.TenDangNhap}'");
        db.SaveChanges();
    }
}
