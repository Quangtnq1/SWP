using System.Linq;
using System.Windows;
using SWP.Models;
using SWP.Services;

namespace SWP.Views;

public partial class LoginView : Window
{
    public LoginView()
    {
        InitializeComponent();
        TxtTenDangNhap.Focus();
    }

    // CN01 — Nút đặt IsDefault="True" trong XAML nên bấm Enter ở ô nào cũng chạy vào đây.
    private void BtnDangNhap_Click(object sender, RoutedEventArgs e)
    {
        string tenDangNhap = TxtTenDangNhap.Text.Trim();
        string matKhau = TxtMatKhau.Password;

        using var db = new QuanLyKhoVatTuContext();

        var user = db.NguoiDungs.FirstOrDefault(u => u.TenDangNhap == tenDangNhap);

        if (user is null)
        {
            MessageBox.Show("Tài khoản không tồn tại.", "Không thể đăng nhập", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (!user.DangHoatDong)
        {
            MessageBox.Show("Tài khoản đã bị khóa, liên hệ Quản lý.", "Không thể đăng nhập", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        
        if (user.MatKhauHash != matKhau)
        {
            MessageBox.Show("Sai mật khẩu.", "Không thể đăng nhập", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        AuditLogService.Log(db, Session.CurrentUser, "DANGNHAP", "NguoiDung", user.Id, $"Đăng nhập thành công: {user.TenDangNhap}");
        db.SaveChanges();

        
        // không nên rải đi khắp app.
        var nguoiDangNhap = new NguoiDangNhap();
        nguoiDangNhap.Id = user.Id;
        nguoiDangNhap.TenDangNhap = user.TenDangNhap;
        nguoiDangNhap.HoTen = user.HoTen;
        nguoiDangNhap.VaiTro = user.VaiTro;

        // Từ đây mọi màn hình khác đọc Session.CurrentUser là biết ai đang dùng máy.
        Session.SignIn(nguoiDangNhap);

        var mainShell = new MainShellView();
        mainShell.Show();
        Close();
    }
}
