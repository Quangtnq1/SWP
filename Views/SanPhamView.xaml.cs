using System.Windows;
using System.Windows.Controls;
using SWP.Models;
using SWP.Services;

namespace SWP.Views;

// KT01 — Danh mục sản phẩm.
public partial class SanPhamView : UserControl
{
    public SanPhamView()
    {
        InitializeComponent();
        TaiLai();
    }

    private void TaiLai()
    {
        GridSanPham.ItemsSource = DanhMucService.LaySanPham();
    }

    private void BtnThem_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new SanPhamDialog(null);
        dialog.Owner = Window.GetWindow(this);
        if (dialog.ShowDialog() == true)
        {
            TaiLai();
        }
    }

    private void BtnSua_Click(object sender, RoutedEventArgs e)
    {
        if (GridSanPham.SelectedItem is not SanPham sp)
        {
            MessageBox.Show("Chọn 1 sản phẩm trước đã.");
            return;
        }

        var dialog = new SanPhamDialog(sp.Id);
        dialog.Owner = Window.GetWindow(this);
        if (dialog.ShowDialog() == true)
        {
            TaiLai();
        }
    }

    private void BtnNgungSuDung_Click(object sender, RoutedEventArgs e)
    {
        if (GridSanPham.SelectedItem is not SanPham sp)
        {
            MessageBox.Show("Chọn 1 sản phẩm trước đã.");
            return;
        }
        if (MessageBox.Show($"Ngừng sử dụng sản phẩm '{sp.Ten}'?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
        {
            return;
        }

        DanhMucService.NgungSuDungSanPham(sp.Id);
        TaiLai();
    }

    private void BtnLamMoi_Click(object sender, RoutedEventArgs e)
    {
        TaiLai();
    }
}
