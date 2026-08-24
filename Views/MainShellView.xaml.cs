using System.Windows;
using SWP.Services;

namespace SWP.Views;

public partial class MainShellView : Window
{
    public MainShellView()
    {
        InitializeComponent();

        var user = Session.CurrentUser;
        if (user != null) // null không nên xảy ra, màn này chỉ mở được sau khi đăng nhập thành công
        {
            TxtTenNguoiDung.Text = user.HoTen;

            if (user.VaiTro == "THUKHO")
                TxtVaiTro.Text = "Thủ kho";
            else if (user.VaiTro == "KETOAN")
                TxtVaiTro.Text = "Kế toán";
            else if (user.VaiTro == "QUANLY")
                TxtVaiTro.Text = "Quản lý";
            else
                TxtVaiTro.Text = "";
        }

        // Ẩn/hiện menu theo vai trò. Tài khoản Quản lý thấy được cả 3 nhóm menu, không riêng nhóm QUẢN LÝ.
        Visibility visThuKho;
        if (Session.CoTheThaoTacThuKho)
            visThuKho = Visibility.Visible;
        else
            visThuKho = Visibility.Collapsed;
        LblThuKho.Visibility = visThuKho;
        BtnPhieuNhapThuKho.Visibility = visThuKho;
        BtnPhieuXuatThuKho.Visibility = visThuKho;
        BtnTheKho.Visibility = visThuKho;

        Visibility visKeToan;
        if (Session.CoTheThaoTacKeToan)
            visKeToan = Visibility.Visible;
        else
            visKeToan = Visibility.Collapsed;
        LblKeToan.Visibility = visKeToan;
        BtnPhieuNhapKeToan.Visibility = visKeToan;
        BtnPhieuXuatKeToan.Visibility = visKeToan;
        BtnSanPham.Visibility = visKeToan;
        BtnNhomHang.Visibility = visKeToan;
        BtnNhaCungCap.Visibility = visKeToan;
        BtnBoPhan.Visibility = visKeToan;
        BtnBaoCao.Visibility = visKeToan;

        Visibility visQuanLy;
        if (user != null && user.VaiTro == "QUANLY")
            visQuanLy = Visibility.Visible;
        else
            visQuanLy = Visibility.Collapsed;
        LblQuanLy.Visibility = visQuanLy;
        BtnNguoiDung.Visibility = visQuanLy;
        BtnNhatKy.Visibility = visQuanLy;

        ContentArea.Content = new DashboardView();
    }

    private void BtnDashboard_Click(object sender, RoutedEventArgs e)
    {
        ContentArea.Content = new DashboardView();
    }

    private void BtnTraCuu_Click(object sender, RoutedEventArgs e)
    {
        ContentArea.Content = new ProductLookupView();
    }

    private void BtnPhieuNhap_Click(object sender, RoutedEventArgs e)
    {
        ContentArea.Content = new PhieuNhapListView();
    }

    private void BtnPhieuXuat_Click(object sender, RoutedEventArgs e)
    {
        ContentArea.Content = new PhieuXuatListView();
    }

    private void BtnTheKho_Click(object sender, RoutedEventArgs e)
    {
        ContentArea.Content = new TheKhoView();
    }

    private void BtnSanPham_Click(object sender, RoutedEventArgs e)
    {
        ContentArea.Content = new SanPhamView();
    }

    private void BtnNhomHang_Click(object sender, RoutedEventArgs e)
    {
        ContentArea.Content = new NhomHangView();
    }

    private void BtnNhaCungCap_Click(object sender, RoutedEventArgs e)
    {
        ContentArea.Content = new NhaCungCapView();
    }

    private void BtnBoPhan_Click(object sender, RoutedEventArgs e)
    {
        ContentArea.Content = new BoPhanView();
    }

    private void BtnBaoCao_Click(object sender, RoutedEventArgs e)
    {
        ContentArea.Content = new BaoCaoView();
    }

    private void BtnNguoiDung_Click(object sender, RoutedEventArgs e)
    {
        ContentArea.Content = new NguoiDungView();
    }

    private void BtnNhatKy_Click(object sender, RoutedEventArgs e)
    {
        ContentArea.Content = new NhatKyView();
    }

    private void BtnDoiMatKhau_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new ChangePasswordDialog();
        dialog.Owner = this;
        dialog.ShowDialog();
    }

    private void BtnDangXuat_Click(object sender, RoutedEventArgs e)
    {
        if (MessageBox.Show("Đăng xuất khỏi hệ thống?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
            return;

        Session.SignOut();
        var login = new LoginView();
        login.Show();
        Close();
    }
}
