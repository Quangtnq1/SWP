using System.Windows;
using System.Windows.Controls;
using SWP.Models;
using SWP.Services;

namespace SWP.Views;

// KT03 — Danh mục nhà cung cấp.
public partial class NhaCungCapView : UserControl
{
    public NhaCungCapView()
    {
        InitializeComponent();
        TaiLai();
    }

    private void TaiLai()
    {
        GridNhaCungCap.ItemsSource = DanhMucService.LayNhaCungCap();
    }

    private void BtnThem_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new NhaCungCapDialog(null);
        dialog.Owner = Window.GetWindow(this);
        if (dialog.ShowDialog() == true)
        {
            TaiLai();
        }
    }

    private void BtnSua_Click(object sender, RoutedEventArgs e)
    {
        if (GridNhaCungCap.SelectedItem is not NhaCungCap ncc)
        {
            MessageBox.Show("Chọn 1 nhà cung cấp trước đã.");
            return;
        }

        var dialog = new NhaCungCapDialog(ncc.Id);
        dialog.Owner = Window.GetWindow(this);
        if (dialog.ShowDialog() == true)
        {
            TaiLai();
        }
    }

    private void BtnNgungHoatDong_Click(object sender, RoutedEventArgs e)
    {
        if (GridNhaCungCap.SelectedItem is not NhaCungCap ncc)
        {
            MessageBox.Show("Chọn 1 nhà cung cấp trước đã.");
            return;
        }
        if (MessageBox.Show($"Ngừng hoạt động nhà cung cấp '{ncc.Ten}'?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
        {
            return;
        }

        DanhMucService.NgungHoatDongNhaCungCap(ncc.Id);
        TaiLai();
    }

    private void BtnLamMoi_Click(object sender, RoutedEventArgs e)
    {
        TaiLai();
    }
}
