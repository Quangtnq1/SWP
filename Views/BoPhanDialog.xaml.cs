using System.Globalization;
using System.Linq;
using System.Windows;
using SWP.Models;
using SWP.Services;

namespace SWP.Views;

public partial class BoPhanDialog : Window
{
    private readonly int _id;

    public BoPhanDialog(int? id)
    {
        InitializeComponent();

        if (id is null)
        {
            _id = 0;
            TxtTieuDe.Text = "Thêm bộ phận mới";
            ChkDangHoatDong.IsChecked = true;
        }
        else
        {
            _id = id.Value;
            TxtTieuDe.Text = "Sửa bộ phận";

            var bp = DanhMucService.LayBoPhan().FirstOrDefault(b => b.Id == _id);
            if (bp is null)
            {
                MessageBox.Show("Không tìm thấy bộ phận.");
                Close();
                return;
            }

            TxtMa.Text = bp.Ma;
            TxtTen.Text = bp.Ten;
            TxtTruongBoPhan.Text = bp.TruongBoPhan;
            if (bp.SoNhanSu is null)
                TxtSoNhanSu.Text = "";
            else
                TxtSoNhanSu.Text = bp.SoNhanSu.ToString();
            ChkDangHoatDong.IsChecked = bp.DangHoatDong;
        }
    }

    private void BtnLuu_Click(object sender, RoutedEventArgs e)
    {
        int? soNhanSu = null;
        if (!string.IsNullOrWhiteSpace(TxtSoNhanSu.Text))
        {
            if (!int.TryParse(TxtSoNhanSu.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out var sl) || sl < 0)
            {
                MessageBox.Show("Số nhân sự phải là số nguyên không âm.", "Không thể lưu", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (sl > 10000)
            {
                MessageBox.Show("Số nhân sự không được vượt quá 10.000.", "Không thể lưu", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            soNhanSu = sl;
        }

        var bp = new BoPhan();
        bp.Id = _id;
        bp.Ma = TxtMa.Text.Trim();
        bp.Ten = TxtTen.Text.Trim();
        bp.TruongBoPhan = TxtTruongBoPhan.Text;
        bp.SoNhanSu = soNhanSu;
        bp.DangHoatDong = ChkDangHoatDong.IsChecked == true;

        var (ok, loi) = DanhMucService.LuuBoPhan(bp);
        if (!ok)
        {
            string thongDiepLoi;
            if (loi == null)
                thongDiepLoi = "Có lỗi xảy ra.";
            else
                thongDiepLoi = loi;
            MessageBox.Show(thongDiepLoi, "Không thể lưu", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        DialogResult = true;
    }

    private void BtnHuy_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}
