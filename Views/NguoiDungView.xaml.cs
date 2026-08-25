using System.Windows;
using System.Windows.Controls;
using SWP.Models;
using SWP.Services;

namespace SWP.Views;

// QL01 — Quản lý tài khoản người dùng, QL02 — Đặt lại mật khẩu.
public partial class NguoiDungView : UserControl
{
    public NguoiDungView()
    {
        InitializeComponent();
        TaiLai();
    }

    private void TaiLai()
    {
        GridNguoiDung.ItemsSource = NguoiDungService.LayDanhSach();
    }

    private NguoiDung? NguoiDungDangChon
    {
        get { return GridNguoiDung.SelectedItem as NguoiDung; }
    }

    private void BtnThem_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new NguoiDungDialog(null);
        dialog.Owner = Window.GetWindow(this);
        if (dialog.ShowDialog() == true)
        {
            TaiLai();
        }
    }

    private void BtnSua_Click(object sender, RoutedEventArgs e)
    {
        if (NguoiDungDangChon is null)
        {
            MessageBox.Show("Chọn 1 người dùng trước đã.");
            return;
        }

        var dialog = new NguoiDungDialog(NguoiDungDangChon.Id);
        dialog.Owner = Window.GetWindow(this);
        if (dialog.ShowDialog() == true)
        {
            TaiLai();
        }
    }

    private void BtnDatLaiMatKhau_Click(object sender, RoutedEventArgs e)
    {
        if (NguoiDungDangChon is null)
        {
            MessageBox.Show("Chọn 1 người dùng trước đã.");
            return;
        }

        var matKhauMoi = InputDialog.Show($"Đặt lại mật khẩu cho '{NguoiDungDangChon.TenDangNhap}'", "Nhập mật khẩu mới:");
        if (string.IsNullOrWhiteSpace(matKhauMoi))
        {
            return;
        }

        AuthService.DatLaiMatKhau(NguoiDungDangChon.Id, matKhauMoi);
        MessageBox.Show("Đã đặt lại mật khẩu.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void BtnLamMoi_Click(object sender, RoutedEventArgs e)
    {
        TaiLai();
    }
}
