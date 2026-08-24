using System.Linq;
using System.Windows;
using SWP.Models;
using SWP.Services;

namespace SWP.Views;

public partial class NhaCungCapDialog : Window
{
    private readonly int _id;

    public NhaCungCapDialog(int? id)
    {
        InitializeComponent();

        if (id is null)
        {
            _id = 0;
            TxtTieuDe.Text = "Thêm nhà cung cấp mới";
            ChkDangHoatDong.IsChecked = true;
        }
        else
        {
            _id = id.Value;
            TxtTieuDe.Text = "Sửa nhà cung cấp";

            var ncc = DanhMucService.LayNhaCungCap().FirstOrDefault(n => n.Id == _id);
            if (ncc is null)
            {
                MessageBox.Show("Không tìm thấy nhà cung cấp.");
                Close();
                return;
            }

            TxtMa.Text = ncc.Ma;
            TxtTen.Text = ncc.Ten;
            TxtMaSoThue.Text = ncc.MaSoThue;
            TxtDiaChi.Text = ncc.DiaChi;
            TxtDienThoai.Text = ncc.DienThoai;
            TxtNguoiLienHe.Text = ncc.NguoiLienHe;
            ChkDangHoatDong.IsChecked = ncc.DangHoatDong;
        }
    }

    private void BtnLuu_Click(object sender, RoutedEventArgs e)
    {
        var ncc = new NhaCungCap();
        ncc.Id = _id;
        ncc.Ma = TxtMa.Text.Trim();
        ncc.Ten = TxtTen.Text.Trim();
        ncc.MaSoThue = TxtMaSoThue.Text;
        ncc.DiaChi = TxtDiaChi.Text;
        ncc.DienThoai = TxtDienThoai.Text;
        ncc.NguoiLienHe = TxtNguoiLienHe.Text;
        ncc.DangHoatDong = ChkDangHoatDong.IsChecked == true;

        var (ok, loi) = DanhMucService.LuuNhaCungCap(ncc);
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
