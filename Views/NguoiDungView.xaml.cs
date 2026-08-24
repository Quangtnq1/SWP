using System.Windows;
using System.Windows.Controls;
using SWP.Models;
using SWP.Services;

namespace SWP.Views;

// QL01 — Quản lý tài khoản người dùng, QL02 — Đặt lại mật khẩu.
public partial class NguoiDungView : UserControl
{
    private int _dangSuaId;

    public NguoiDungView()
    {
        InitializeComponent();
        CboVaiTro.ItemsSource = new[] { "THUKHO", "KETOAN", "QUANLY" };
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
        _dangSuaId = 0;
        TxtTenDangNhap.Text = ""; TxtHoTen.Text = ""; CboVaiTro.SelectedIndex = 0;
        TxtDienThoai.Text = ""; ChkDangHoatDong.IsChecked = true; TxtMatKhauMoi.Text = "";
        PanelMatKhauMoi.Visibility = Visibility.Visible;
        FormNguoiDung.Visibility = Visibility.Visible;
    }

    private void BtnSua_Click(object sender, RoutedEventArgs e)
    {
        if (NguoiDungDangChon is null)
        {
            MessageBox.Show("Chọn 1 người dùng trước đã.");
            return;
        }

        var u = NguoiDungDangChon;
        _dangSuaId = u.Id;
        TxtTenDangNhap.Text = u.TenDangNhap; TxtHoTen.Text = u.HoTen; CboVaiTro.SelectedItem = u.VaiTro;
        TxtDienThoai.Text = u.DienThoai; ChkDangHoatDong.IsChecked = u.DangHoatDong;
        PanelMatKhauMoi.Visibility = Visibility.Collapsed; // sửa thông tin thì không đổi mật khẩu ở đây (dùng nút riêng)
        FormNguoiDung.Visibility = Visibility.Visible;
    }

    private void BtnHuy_Click(object sender, RoutedEventArgs e)
    {
        FormNguoiDung.Visibility = Visibility.Collapsed;
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
        user.Id = _dangSuaId;
        user.TenDangNhap = TxtTenDangNhap.Text.Trim();
        user.HoTen = TxtHoTen.Text.Trim();
        user.VaiTro = vaiTro;
        user.DienThoai = TxtDienThoai.Text;
        user.DangHoatDong = ChkDangHoatDong.IsChecked == true;

        string? matKhauNeuTaoMoi;
        if (_dangSuaId == 0)
            matKhauNeuTaoMoi = TxtMatKhauMoi.Text;
        else
            matKhauNeuTaoMoi = null;

        var (ok, loi) = NguoiDungService.LuuNguoiDung(user, matKhauNeuTaoMoi);
        if (!ok)
        {
            MessageBox.Show(loi, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        FormNguoiDung.Visibility = Visibility.Collapsed;
        TaiLai();
    }

    private void BtnKhoaTaiKhoan_Click(object sender, RoutedEventArgs e)
    {
        if (NguoiDungDangChon is null)
        {
            MessageBox.Show("Chọn 1 người dùng trước đã.");
            return;
        }

        var nguoiDangNhap = Session.CurrentUser;
        if (nguoiDangNhap != null && NguoiDungDangChon.Id == nguoiDangNhap.Id)
        {
            MessageBox.Show("Không thể tự khóa tài khoản đang đăng nhập.");
            return;
        }

        if (MessageBox.Show($"Khóa tài khoản '{NguoiDungDangChon.TenDangNhap}'?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
        {
            return;
        }

        NguoiDungService.KhoaTaiKhoan(NguoiDungDangChon.Id);
        TaiLai();
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
}
