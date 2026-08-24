using System.Windows;
using System.Windows.Controls;
using SWP.Models;
using SWP.Services;

namespace SWP.Views;

// KT04 — Danh mục bộ phận.
public partial class BoPhanView : UserControl
{
    public BoPhanView()
    {
        InitializeComponent();
        TaiLai();
    }

    private void TaiLai()
    {
        GridBoPhan.ItemsSource = DanhMucService.LayBoPhan();
    }

    private void BtnThem_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new BoPhanDialog(null);
        dialog.Owner = Window.GetWindow(this);
        if (dialog.ShowDialog() == true)
        {
            TaiLai();
        }
    }

    private void BtnSua_Click(object sender, RoutedEventArgs e)
    {
        if (GridBoPhan.SelectedItem is not BoPhan bp)
        {
            MessageBox.Show("Chọn 1 bộ phận trước đã.");
            return;
        }

        var dialog = new BoPhanDialog(bp.Id);
        dialog.Owner = Window.GetWindow(this);
        if (dialog.ShowDialog() == true)
        {
            TaiLai();
        }
    }

    private void BtnNgungHoatDong_Click(object sender, RoutedEventArgs e)
    {
        if (GridBoPhan.SelectedItem is not BoPhan bp)
        {
            MessageBox.Show("Chọn 1 bộ phận trước đã.");
            return;
        }
        if (MessageBox.Show($"Ngừng hoạt động bộ phận '{bp.Ten}'?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
        {
            return;
        }

        DanhMucService.NgungHoatDongBoPhan(bp.Id);
        TaiLai();
    }

    private void BtnLamMoi_Click(object sender, RoutedEventArgs e)
    {
        TaiLai();
    }
}
