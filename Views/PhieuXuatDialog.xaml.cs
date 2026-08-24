using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using SWP.Models;
using SWP.Services;

namespace SWP.Views;

// Giữ tạm các dòng chi tiết trên lưới. Lúc lập chỉ có số lượng, đơn giá để trống tới khi kế toán duyệt.
public class DongXuat
{
    // Constructor bắt buộc truyền SanPham ngay lúc tạo, nhờ vậy property này không bao giờ null.
    public DongXuat(SanPham sanPham)
    {
        SanPham = sanPham;
    }

    public SanPham SanPham { get; set; }
    public decimal SoLuongYeuCau { get; set; }
    public decimal? DonGia { get; set; }
    public decimal? ThanhTien { get; set; }
}

public partial class PhieuXuatDialog : Window
{
    private readonly List<DongXuat> _dsChiTiet = new();

    public PhieuXuatDialog(int? id)
    {
        InitializeComponent();

        CboBoPhan.ItemsSource = DanhMucService.LayBoPhan();
        CboSanPham.ItemsSource = DanhMucService.LaySanPham();
        CboLoaiXuat.ItemsSource = new[] { "CAPPHAT", "DOTXUAT", "SUKIEN", "THAYTHE", "HUY" };

        if (id is null)
        {
            TxtTieuDe.Text = "Lập phiếu xuất kho";
            DpNgayXuat.SelectedDate = DateTime.Today;
            CboLoaiXuat.SelectedIndex = 0;
            GridChiTiet.ItemsSource = _dsChiTiet;
        }
        else
        {
            var phieu = PhieuXuatService.LayChiTiet(id.Value);
            if (phieu is null)
            {
                MessageBox.Show("Không tìm thấy phiếu.");
                Close();
                return;
            }

            TxtTieuDe.Text = $"Phiếu xuất {phieu.SoPhieu} ({phieu.TrangThai})";
            DpNgayXuat.SelectedDate = phieu.NgayXuat;
            CboLoaiXuat.SelectedItem = phieu.LoaiXuat;
            CboBoPhan.SelectedItem = ((List<BoPhan>)CboBoPhan.ItemsSource).FirstOrDefault(b => b.Id == phieu.BoPhanId);
            TxtNguoiNhan.Text = phieu.NguoiNhan;
            TxtLyDoXuat.Text = phieu.LyDoXuat;

            foreach (var ct in phieu.ChiTietPhieuXuats)
            {
                var dongDaLuu = new DongXuat(ct.SanPham);
                dongDaLuu.SoLuongYeuCau = ct.SoLuongYeuCau;
                dongDaLuu.DonGia = ct.DonGia;
                dongDaLuu.ThanhTien = ct.ThanhTien;
                _dsChiTiet.Add(dongDaLuu);
            }
            GridChiTiet.ItemsSource = _dsChiTiet;

            // Phiếu đã lập rồi thì chỉ cho xem: khóa ô nhập, ẩn nút Lưu.
            DpNgayXuat.IsEnabled = false;
            CboLoaiXuat.IsEnabled = false;
            CboBoPhan.IsEnabled = false;
            TxtNguoiNhan.IsEnabled = false;
            TxtLyDoXuat.IsEnabled = false;
            PanelThemDong.Visibility = Visibility.Collapsed;
            PanelXoaDong.Visibility = Visibility.Collapsed;
            BtnLuu.Visibility = Visibility.Collapsed;
        }
    }

    private void BtnThemDong_Click(object sender, RoutedEventArgs e)
    {
        if (CboSanPham.SelectedItem is not SanPham sp)
        {
            MessageBox.Show("Chọn sản phẩm.", "Không thể thêm dòng", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        if (_dsChiTiet.Any(d => d.SanPham.Id == sp.Id))
        {
            MessageBox.Show("Sản phẩm này đã có trong phiếu rồi.", "Không thể thêm dòng", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (!decimal.TryParse(TxtSoLuongYeuCau.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out var slYeuCau) || slYeuCau <= 0)
        {
            MessageBox.Show("Số lượng yêu cầu phải là số lớn hơn 0.", "Không thể thêm dòng", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var dongMoi = new DongXuat(sp);
        dongMoi.SoLuongYeuCau = slYeuCau;
        _dsChiTiet.Add(dongMoi);

        // List<T> không tự báo cho DataGrid biết danh sách vừa đổi (khác ObservableCollection),
        // nên phải gán lại ItemsSource để ép vẽ lại.
        GridChiTiet.ItemsSource = null;
        GridChiTiet.ItemsSource = _dsChiTiet;

        TxtSoLuongYeuCau.Clear();
        CboSanPham.SelectedItem = null;
    }

    private void BtnXoaDong_Click(object sender, RoutedEventArgs e)
    {
        if (GridChiTiet.SelectedItem is DongXuat dong)
        {
            _dsChiTiet.Remove(dong);

            GridChiTiet.ItemsSource = null;
            GridChiTiet.ItemsSource = _dsChiTiet;
        }
    }

    private void BtnLuu_Click(object sender, RoutedEventArgs e)
    {
        if (CboBoPhan.SelectedItem is not BoPhan boPhan)
        {
            MessageBox.Show("Chọn bộ phận nhận vật tư.", "Không thể lưu phiếu", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var input = new List<ChiTietXuatInput>();
        foreach (var dong in _dsChiTiet)
        {
            var dongGuiXuong = new ChiTietXuatInput();
            dongGuiXuong.SanPhamId = dong.SanPham.Id;
            dongGuiXuong.SoLuongYeuCau = dong.SoLuongYeuCau;
            input.Add(dongGuiXuong);
        }

        DateTime ngayXuat;
        if (DpNgayXuat.SelectedDate == null)
            ngayXuat = DateTime.Today;
        else
            ngayXuat = DpNgayXuat.SelectedDate.Value;

        string loaiXuat;
        if (CboLoaiXuat.SelectedItem is string loaiXuatDaChon)
            loaiXuat = loaiXuatDaChon;
        else
            loaiXuat = "CAPPHAT";

        var (ok, loi) = PhieuXuatService.TaoPhieu(
            ngayXuat, loaiXuat, boPhan.Id, TxtNguoiNhan.Text, TxtLyDoXuat.Text, null, input);

        if (!ok)
        {
            string thongDiepLoi;
            if (loi == null)
                thongDiepLoi = "Có lỗi xảy ra.";
            else
                thongDiepLoi = loi;
            MessageBox.Show(thongDiepLoi, "Không thể lưu phiếu", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        MessageBox.Show("Đã lập phiếu xuất, chờ kế toán duyệt.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
        DialogResult = true;
    }

    private void BtnDong_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}
