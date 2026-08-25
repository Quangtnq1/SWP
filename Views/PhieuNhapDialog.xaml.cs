using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using SWP.Models;
using SWP.Services;

namespace SWP.Views;

// Giữ tạm các dòng chi tiết trên lưới, tới khi bấm "Lưu phiếu" mới ghi xuống DB.
public class DongNhap
{
    // Constructor bắt buộc truyền SanPham ngay lúc tạo, nhờ vậy property này không bao giờ null.
    public DongNhap(SanPham sanPham)
    {
        SanPham = sanPham;
    }

    public SanPham SanPham { get; set; }
    public decimal SoLuong { get; set; }
    public decimal DonGia { get; set; }
    public decimal ThanhTien
    {
        get { return SoLuong * DonGia; }
    }
}

public partial class PhieuNhapDialog : Window
{
    private readonly List<DongNhap> _dsChiTiet = new();

    // id == null -> lập phiếu mới. id != null -> xem chi tiết phiếu đã có (chỉ đọc).
    public PhieuNhapDialog(int? id)
    {
        InitializeComponent();

        CboNhaCungCap.ItemsSource = DanhMucService.LayNhaCungCap();
        CboSanPham.ItemsSource = DanhMucService.LaySanPham();

        if (id is null)
        {
            TxtTieuDe.Text = "Lập phiếu nhập kho";
            DpNgayNhap.SelectedDate = DateTime.Today;
            GridChiTiet.ItemsSource = _dsChiTiet;
        }
        else
        {
            var phieu = PhieuNhapService.LayChiTiet(id.Value);
            if (phieu is null)
            {
                MessageBox.Show("Không tìm thấy phiếu.");
                Close();
                return;
            }

            TxtTieuDe.Text = $"Phiếu nhập {phieu.SoPhieu} ({phieu.TrangThai})";
            DpNgayNhap.SelectedDate = phieu.NgayNhap;
            CboNhaCungCap.SelectedItem = ((List<NhaCungCap>)CboNhaCungCap.ItemsSource).FirstOrDefault(n => n.Id == phieu.NhaCungCapId);
            TxtSoHoaDon.Text = phieu.SoHoaDon;
            TxtNguoiGiaoHang.Text = phieu.NguoiGiaoHang;

            foreach (var ct in phieu.ChiTietPhieuNhaps)
            {
                var dongDaLuu = new DongNhap(ct.SanPham);
                dongDaLuu.SoLuong = ct.SoLuong;
                dongDaLuu.DonGia = ct.DonGia;
                _dsChiTiet.Add(dongDaLuu);
            }
            GridChiTiet.ItemsSource = _dsChiTiet;

            // Phiếu đã lập rồi thì chỉ cho xem: khóa ô nhập, ẩn nút Lưu.
            DpNgayNhap.IsEnabled = false;
            CboNhaCungCap.IsEnabled = false;
            TxtSoHoaDon.IsEnabled = false;
            TxtNguoiGiaoHang.IsEnabled = false;
            PanelThemDong.Visibility = Visibility.Collapsed;
            PanelXoaDong.Visibility = Visibility.Collapsed;
            BtnLuu.Visibility = Visibility.Collapsed;
        }

        TxtTongTien.Text = _dsChiTiet.Sum(d => d.ThanhTien).ToString("N0") + " đ";
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

        if (!int.TryParse(TxtSoLuong.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out var slNhap) || slNhap <= 0)
        {
            MessageBox.Show("Số lượng nhập phải là số nguyên lớn hơn 0.", "Không thể thêm dòng", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        if (slNhap > 10000)
        {
            MessageBox.Show("Số lượng nhập không được vượt quá 10.000.", "Không thể thêm dòng", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        // Đơn giá không gõ tay — lấy giá cố định đặt sẵn trên sản phẩm ở màn Danh mục.
        if (sp.GiaVonBinhQuan <= 0)
        {
            MessageBox.Show($"Sản phẩm '{sp.Ten}' chưa được đặt giá. Vào Danh mục sản phẩm để đặt giá trước khi nhập.", "Không thể thêm dòng", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var dongMoi = new DongNhap(sp);
        dongMoi.SoLuong = slNhap;
        dongMoi.DonGia = sp.GiaVonBinhQuan;
        _dsChiTiet.Add(dongMoi);

        // List<T> không tự báo cho DataGrid biết danh sách vừa đổi (khác ObservableCollection),
        // nên phải gán lại ItemsSource để ép vẽ lại.
        GridChiTiet.ItemsSource = null;
        GridChiTiet.ItemsSource = _dsChiTiet;
        TxtTongTien.Text = _dsChiTiet.Sum(d => d.ThanhTien).ToString("N0") + " đ";

        TxtSoLuong.Clear();
        CboSanPham.SelectedItem = null;
    }

    private void BtnXoaDong_Click(object sender, RoutedEventArgs e)
    {
        if (GridChiTiet.SelectedItem is DongNhap dong)
        {
            _dsChiTiet.Remove(dong);

            GridChiTiet.ItemsSource = null;
            GridChiTiet.ItemsSource = _dsChiTiet;
            TxtTongTien.Text = _dsChiTiet.Sum(d => d.ThanhTien).ToString("N0") + " đ";
        }
    }

    private void BtnLuu_Click(object sender, RoutedEventArgs e)
    {
        var input = new List<ChiTietNhapInput>();
        foreach (var dong in _dsChiTiet)
        {
            var dongGuiXuong = new ChiTietNhapInput();
            dongGuiXuong.SanPhamId = dong.SanPham.Id;
            dongGuiXuong.SoLuong = dong.SoLuong;
            dongGuiXuong.DonGia = dong.DonGia;
            input.Add(dongGuiXuong);
        }

        var nhaCungCap = CboNhaCungCap.SelectedItem as NhaCungCap;
        int? nhaCungCapId;
        if (nhaCungCap == null)
            nhaCungCapId = null;
        else
            nhaCungCapId = nhaCungCap.Id;

        DateTime ngayNhap;
        if (DpNgayNhap.SelectedDate == null)
            ngayNhap = DateTime.Today;
        else
            ngayNhap = DpNgayNhap.SelectedDate.Value;

        var (ok, loi) = PhieuNhapService.TaoPhieu(
            ngayNhap, nhaCungCapId, TxtSoHoaDon.Text, TxtNguoiGiaoHang.Text, null, input);

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

        MessageBox.Show("Đã lập phiếu nhập, chờ kế toán duyệt.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
        DialogResult = true;
    }

    private void BtnDong_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}
