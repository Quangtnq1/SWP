using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using SWP.Models;
using SWP.Services;

namespace SWP.Views;

// id == null -> thêm mới (Id gửi lên Service = 0). id != null -> sửa sản phẩm có Id đó.
public partial class SanPhamDialog : Window
{
    private readonly int _id;

    public SanPhamDialog(int? id)
    {
        InitializeComponent();
        CboNhomHang.ItemsSource = DanhMucService.LayNhomHang();

        if (id is null)
        {
            _id = 0;
            TxtTieuDe.Text = "Thêm sản phẩm mới";
            TxtGia.Text = "0";
            TxtTonToiThieu.Text = "0";
            ChkDangSuDung.IsChecked = true;
        }
        else
        {
            _id = id.Value;
            TxtTieuDe.Text = "Sửa sản phẩm";

            var sp = DanhMucService.LaySanPham().FirstOrDefault(s => s.Id == _id);
            if (sp is null)
            {
                MessageBox.Show("Không tìm thấy sản phẩm.");
                Close();
                return;
            }

            TxtMa.Text = sp.Ma;
            TxtTen.Text = sp.Ten;
            TxtGhiChu.Text = sp.GhiChu;
            CboNhomHang.SelectedItem = ((List<NhomHang>)CboNhomHang.ItemsSource).Find(n => n.Id == sp.NhomHangId);
            TxtDonViTinh.Text = sp.DonViTinh;
            TxtGia.Text = ((int)sp.GiaVonBinhQuan).ToString(CultureInfo.InvariantCulture);
            TxtTonToiThieu.Text = ((int)sp.TonToiThieu).ToString(CultureInfo.InvariantCulture);
            ChkDangSuDung.IsChecked = sp.DangSuDung;
        }
    }

    private void BtnLuu_Click(object sender, RoutedEventArgs e)
    {
        if (!int.TryParse(TxtGia.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out var gia) || gia < 0)
        {
            MessageBox.Show("Giá phải là số nguyên không âm.", "Không thể lưu", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        if (gia > 100000000)
        {
            MessageBox.Show("Giá không được vượt quá 100.000.000.", "Không thể lưu", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (!int.TryParse(TxtTonToiThieu.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out var tonToiThieu) || tonToiThieu < 0)
        {
            MessageBox.Show("Tồn tối thiểu phải là số nguyên không âm.", "Không thể lưu", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        if (tonToiThieu > 10000)
        {
            MessageBox.Show("Tồn tối thiểu không được vượt quá 10.000.", "Không thể lưu", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        // CboNhomHang.SelectedItem có thể đang là null (chưa chọn nhóm hàng) -> NhomHangId để null theo.
        var nhomHangDangChon = CboNhomHang.SelectedItem as NhomHang;
        int? nhomHangId;
        if (nhomHangDangChon == null)
            nhomHangId = null;
        else
            nhomHangId = nhomHangDangChon.Id;

        var sp = new SanPham();
        sp.Id = _id;
        sp.Ma = TxtMa.Text.Trim();
        sp.Ten = TxtTen.Text.Trim();
        sp.GhiChu = TxtGhiChu.Text;
        sp.NhomHangId = nhomHangId;
        sp.DonViTinh = TxtDonViTinh.Text.Trim();
        sp.GiaVonBinhQuan = gia;
        sp.TonToiThieu = tonToiThieu;
        sp.DangSuDung = ChkDangSuDung.IsChecked == true;

        var (ok, loi) = DanhMucService.LuuSanPham(sp);
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
