using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using SWP.Models;
using SWP.Services;

namespace SWP.Views;

// CN04 — Tra cứu sản phẩm và tồn kho, kèm TK07 (lọc hàng dưới định mức) qua 1 checkbox.
public partial class ProductLookupView : UserControl
{
    private List<SanPham> _tatCa = new();

    public ProductLookupView()
    {
        InitializeComponent();
        TaiLai();
    }

    private void TaiLai()
    {
        _tatCa = DanhMucService.LaySanPham();

        var ketQua = _tatCa.Where(s => s.DangSuDung).AsEnumerable();

        var tuKhoa = TxtTuKhoa.Text.Trim().ToLower();
        if (!string.IsNullOrEmpty(tuKhoa))
            ketQua = ketQua.Where(s => s.Ma.ToLower().Contains(tuKhoa) || s.Ten.ToLower().Contains(tuKhoa));

        if (ChkDuoiDinhMuc.IsChecked == true)
            ketQua = ketQua.Where(s => s.TonKho < s.TonToiThieu);

        GridSanPham.ItemsSource = ketQua.OrderBy(s => s.Ma).ToList();
    }

    private void TxtTuKhoa_TextChanged(object sender, TextChangedEventArgs e)
    {
        var ketQua = _tatCa.Where(s => s.DangSuDung).AsEnumerable();

        var tuKhoa = TxtTuKhoa.Text.Trim().ToLower();
        if (!string.IsNullOrEmpty(tuKhoa))
            ketQua = ketQua.Where(s => s.Ma.ToLower().Contains(tuKhoa) || s.Ten.ToLower().Contains(tuKhoa));

        if (ChkDuoiDinhMuc.IsChecked == true)
            ketQua = ketQua.Where(s => s.TonKho < s.TonToiThieu);

        GridSanPham.ItemsSource = ketQua.OrderBy(s => s.Ma).ToList();
    }

    // TK07 — tích/bỏ tích ô "chỉ hiện hàng dưới định mức".
    private void Filter_Changed(object sender, System.Windows.RoutedEventArgs e)
    {
        var ketQua = _tatCa.Where(s => s.DangSuDung).AsEnumerable();

        var tuKhoa = TxtTuKhoa.Text.Trim().ToLower();
        if (!string.IsNullOrEmpty(tuKhoa))
            ketQua = ketQua.Where(s => s.Ma.ToLower().Contains(tuKhoa) || s.Ten.ToLower().Contains(tuKhoa));

        if (ChkDuoiDinhMuc.IsChecked == true)
            ketQua = ketQua.Where(s => s.TonKho < s.TonToiThieu);

        GridSanPham.ItemsSource = ketQua.OrderBy(s => s.Ma).ToList();
    }

    private void BtnLamMoi_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        TaiLai();
    }
}
