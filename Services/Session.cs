using SWP.Models;

namespace SWP.Services;

// Lưu người đang đăng nhập cho toàn app, thay cho việc truyền UserId qua từng màn hình.
public static class Session
{
    public static NguoiDangNhap? CurrentUser { get; private set; }

    public static void SignIn(NguoiDangNhap user)
    {
        CurrentUser = user;
    }

    public static void SignOut()
    {
        CurrentUser = null;
    }

    // Hỏi "có được PHÉP thao tác như vai trò này không", không phải "có đúng vai trò này không":
    // Quản lý làm được mọi chức năng nên luôn trả về true ở cả 2 property.
    public static bool CoTheThaoTacThuKho
    {
        get
        {
            if (CurrentUser == null)
                return false;
            if (CurrentUser.VaiTro == "THUKHO")
                return true;
            if (CurrentUser.VaiTro == "QUANLY")
                return true;
            return false;
        }
    }

    public static bool CoTheThaoTacKeToan
    {
        get
        {
            if (CurrentUser == null)
                return false;
            if (CurrentUser.VaiTro == "KETOAN")
                return true;
            if (CurrentUser.VaiTro == "QUANLY")
                return true;
            return false;
        }
    }
}
