using System.Linq;
using System.Windows;
using SWP.Models;
using SWP.Services;

namespace SWP.Views;

public partial class NhomHangDialog : Window
{
    private readonly int _id;

    public NhomHangDialog(int? id)
    {
        InitializeComponent();

        if (id is null)
        {
            _id = 0;
            TxtTieuDe.Text = "Thêm nhóm hàng mới";
        }
        else
        {
            _id = id.Value;
            TxtTieuDe.Text = "Sửa nhóm hàng";

            var nh = DanhMucService.LayNhomHang().FirstOrDefault(n => n.Id == _id);
            if (nh is null)
            {
                MessageBox.Show("Không tìm thấy nhóm hàng.");
                Close();
                return;
            }

            TxtMa.Text = nh.Ma;
            TxtTen.Text = nh.Ten;
            TxtMoTa.Text = nh.MoTa;
        }
    }

    private void BtnLuu_Click(object sender, RoutedEventArgs e)
    {
        var nh = new NhomHang();
        nh.Id = _id;
        nh.Ma = TxtMa.Text.Trim();
        nh.Ten = TxtTen.Text.Trim();
        nh.MoTa = TxtMoTa.Text;

        var (ok, loi) = DanhMucService.LuuNhomHang(nh);
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
