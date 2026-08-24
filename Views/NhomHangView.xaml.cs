using System.Windows;
using System.Windows.Controls;
using SWP.Models;
using SWP.Services;

namespace SWP.Views;

// KT02 — Danh mục nhóm hàng.
public partial class NhomHangView : UserControl
{
    public NhomHangView()
    {
        InitializeComponent();
        TaiLai();
    }

    private void TaiLai()
    {
        GridNhomHang.ItemsSource = DanhMucService.LayNhomHang();
    }

    private void BtnThem_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new NhomHangDialog(null);
        dialog.Owner = Window.GetWindow(this);
        if (dialog.ShowDialog() == true)
        {
            TaiLai();
        }
    }

    private void BtnSua_Click(object sender, RoutedEventArgs e)
    {
        if (GridNhomHang.SelectedItem is not NhomHang nh)
        {
            MessageBox.Show("Chọn 1 nhóm hàng trước đã.");
            return;
        }

        var dialog = new NhomHangDialog(nh.Id);
        dialog.Owner = Window.GetWindow(this);
        if (dialog.ShowDialog() == true)
        {
            TaiLai();
        }
    }

    private void BtnXoa_Click(object sender, RoutedEventArgs e)
    {
        if (GridNhomHang.SelectedItem is not NhomHang nh)
        {
            MessageBox.Show("Chọn 1 nhóm hàng trước đã.");
            return;
        }
        if (MessageBox.Show($"Xóa nhóm hàng '{nh.Ten}'?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
        {
            return;
        }

        var (ok, loi) = DanhMucService.XoaNhomHang(nh.Id);
        if (!ok)
        {
            MessageBox.Show(loi, "Không thể xóa", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        TaiLai();
    }

    private void BtnLamMoi_Click(object sender, RoutedEventArgs e)
    {
        TaiLai();
    }
}
