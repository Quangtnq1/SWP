using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SWP.Models;
using SWP.Services;

namespace SWP.Views;

public partial class PhieuXuatListView : UserControl
{
    public PhieuXuatListView()
    {
        InitializeComponent();

        // Thủ kho: được lập phiếu, không được duyệt/hủy. Kế toán: ngược lại. Quản lý: được cả hai.
        if (Session.CoTheThaoTacThuKho)
            BtnLapPhieuMoi.Visibility = Visibility.Visible;
        else
            BtnLapPhieuMoi.Visibility = Visibility.Collapsed;

        if (Session.CoTheThaoTacKeToan)
        {
            BtnDuyet.Visibility = Visibility.Visible;
            BtnTuChoiHuy.Visibility = Visibility.Visible;
        }
        else
        {
            BtnDuyet.Visibility = Visibility.Collapsed;
            BtnTuChoiHuy.Visibility = Visibility.Collapsed;
        }

        TaiLai();
    }

    private void TaiLai()
    {
        GridPhieu.ItemsSource = PhieuXuatService.LayDanhSach();
    }

    private PhieuXuat? PhieuDangChon
    {
        get { return GridPhieu.SelectedItem as PhieuXuat; }
    }

    private void BtnLapPhieuMoi_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new PhieuXuatDialog(null);
        dialog.Owner = Window.GetWindow(this);
        if (dialog.ShowDialog() == true)
        {
            TaiLai();
        }
    }

    private void BtnXemChiTiet_Click(object sender, RoutedEventArgs e)
    {
        if (PhieuDangChon is null)
        {
            MessageBox.Show("Chọn 1 phiếu trước đã.");
            return;
        }

        var dialog = new PhieuXuatDialog(PhieuDangChon.Id);
        dialog.Owner = Window.GetWindow(this);
        if (dialog.ShowDialog() == true)
        {
            TaiLai();
        }
    }

    private void GridPhieu_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (PhieuDangChon is null)
        {
            return;
        }

        var dialog = new PhieuXuatDialog(PhieuDangChon.Id);
        dialog.Owner = Window.GetWindow(this);
        if (dialog.ShowDialog() == true)
        {
            TaiLai();
        }
    }

    private void BtnDuyet_Click(object sender, RoutedEventArgs e)
    {
        if (PhieuDangChon is null)
        {
            MessageBox.Show("Chọn 1 phiếu trước đã.");
            return;
        }
        if (PhieuDangChon.TrangThai != "CHODUYET")
        {
            MessageBox.Show("Chỉ duyệt được phiếu đang Chờ duyệt.");
            return;
        }

        var (ok, loi) = PhieuXuatService.DuyetPhieu(PhieuDangChon.Id);
        if (!ok)
        {
            MessageBox.Show(loi, "Không thể duyệt", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        TaiLai();
    }

    private void BtnTuChoiHuy_Click(object sender, RoutedEventArgs e)
    {
        if (PhieuDangChon is null)
        {
            MessageBox.Show("Chọn 1 phiếu trước đã.");
            return;
        }
        if (PhieuDangChon.TrangThai == "DAHUY")
        {
            MessageBox.Show("Phiếu đã bị hủy trước đó.");
            return;
        }

        string tieuDe;
        if (PhieuDangChon.TrangThai == "CHODUYET")
            tieuDe = "Từ chối phiếu xuất";
        else
            tieuDe = "Hủy phiếu xuất";
        var lyDo = InputDialog.Show(tieuDe, "Vui lòng nhập lý do (bắt buộc):");
        if (string.IsNullOrWhiteSpace(lyDo))
        {
            return;
        }

        var (ok, loi) = PhieuXuatService.HuyPhieu(PhieuDangChon.Id, lyDo);
        if (!ok)
        {
            MessageBox.Show(loi, "Không thể hủy", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        TaiLai();
    }

    private void BtnLamMoi_Click(object sender, RoutedEventArgs e)
    {
        TaiLai();
    }
}
