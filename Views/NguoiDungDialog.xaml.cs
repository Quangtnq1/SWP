using System.Windows;
using SWP.Models;
using SWP.Services;

namespace SWP.Views;

// id == null -> thêm mới (Id gửi lên Service = 0). id != null -> sửa người dùng có Id đó.
public partial class NguoiDungDialog : Window
{
    private readonly int _id;

    public NguoiDungDialog(int? id)
    {
        InitializeComponent();
        CboVaiTro.ItemsSource = new[] { "THUKHO", "KETOAN", "QUANLY" };

        if (id is null)
        {
            _id = 0;
            TxtTieuDe.Text = "Thêm tài khoản mới";
            CboVaiTro.SelectedIndex = 0;
            ChkDangHoatDong.IsChecked = true;
        }
        else
        {
            _id = id.Value;
            TxtTieuDe.Text = "Sửa tài khoản";

            var u = NguoiDungService.LayDanhSach().Find(x => x.Id == _id);
            if (u is null)
            {
                MessageBox.Show("Không tìm thấy người dùng.");
                Close();
                return;
            }

            TxtTenDangNhap.Text = u.TenDangNhap;
            TxtHoTen.Text = u.HoTen;
            CboVaiTro.SelectedItem = u.VaiTro;
            TxtDienThoai.Text = u.DienThoai;
            ChkDangHoatDong.IsChecked = u.DangHoatDong;
            PanelMatKhauMoi.Visibility = Visibility.Collapsed; // sửa thông tin thì không đổi mật khẩu ở đây (dùng nút riêng)

            // Không cho tự đổi vai trò HOẶC tự khóa hoạt động của chính mình — tránh tự phế quyền,
            // khóa cứng không ai vào lại được Quản lý.
            var nguoiDangNhap = Session.CurrentUser;
            if (nguoiDangNhap != null && u.Id == nguoiDangNhap.Id)
            {
                LblVaiTro.Visibility = Visibility.Collapsed;
                CboVaiTro.Visibility = Visibility.Collapsed;
                ChkDangHoatDong.IsEnabled = false;
            }
        }
    }

    private void BtnLuu_Click(object sender, RoutedEventArgs e)
    {
        // CboVaiTro.SelectedItem có thể không phải kiểu string (hoặc null) trong vài trường hợp hiếm ->
        // mặc định về "THUKHO" nếu không lấy được giá trị hợp lệ.
        string vaiTro;
        if (CboVaiTro.SelectedItem is string vaiTroDaChon)
            vaiTro = vaiTroDaChon;
        else
            vaiTro = "THUKHO";

        var user = new NguoiDung();
        user.Id = _id;
        user.TenDangNhap = TxtTenDangNhap.Text.Trim();
        user.HoTen = TxtHoTen.Text.Trim();
        user.VaiTro = vaiTro;
        user.DienThoai = TxtDienThoai.Text;
        user.DangHoatDong = ChkDangHoatDong.IsChecked == true;

        string? matKhauNeuTaoMoi;
        if (_id == 0)
            matKhauNeuTaoMoi = TxtMatKhauMoi.Text;
        else
            matKhauNeuTaoMoi = null;

        var (ok, loi) = NguoiDungService.LuuNguoiDung(user, matKhauNeuTaoMoi);
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
