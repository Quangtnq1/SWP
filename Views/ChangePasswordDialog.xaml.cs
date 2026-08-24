using System.Windows;
using SWP.Services;

namespace SWP.Views;

public partial class ChangePasswordDialog : Window
{
    public ChangePasswordDialog()
    {
        InitializeComponent();
    }

    private void BtnXacNhan_Click(object sender, RoutedEventArgs e)
    {
        if (TxtMatKhauMoi.Password != TxtMatKhauMoiNhacLai.Password)
        {
            MessageBox.Show("Mật khẩu nhắc lại không khớp.", "Không thể đổi mật khẩu", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var nguoiDangNhap = Session.CurrentUser;
        if (nguoiDangNhap == null) return; // không nên xảy ra, màn này chỉ mở được sau khi đăng nhập thành công

        var (ok, loi) = AuthService.DoiMatKhau(nguoiDangNhap.Id, TxtMatKhauCu.Password, TxtMatKhauMoi.Password);
        if (!ok)
        {
            string thongDiepLoi;
            if (loi == null)
                thongDiepLoi = "Đổi mật khẩu thất bại.";
            else
                thongDiepLoi = loi;
            MessageBox.Show(thongDiepLoi, "Không thể đổi mật khẩu", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        MessageBox.Show("Đổi mật khẩu thành công.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
        DialogResult = true;
    }

    private void BtnHuy_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}
