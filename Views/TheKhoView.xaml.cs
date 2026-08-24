using System;
using System.Collections.Generic;
using System.Windows.Controls;
using SWP.Models;
using SWP.Services;

namespace SWP.Views;

// TK06 — Thẻ kho.
public partial class TheKhoView : UserControl
{
    // Sản phẩm giả Id = 0 chèn lên đầu ComboBox, đại diện lựa chọn "xem tất cả sản phẩm cùng lúc".
    private const int MaSanPhamTatCa = 0;

    public TheKhoView()
    {
        InitializeComponent();

        var danhSachChon = new List<SanPham>();
        var phanTuMoi = new SanPham();
        phanTuMoi.Id = MaSanPhamTatCa;
        phanTuMoi.Ma = "";
        phanTuMoi.Ten = "(Tất cả sản phẩm)";
        phanTuMoi.DonViTinh = "";
        danhSachChon.Add(phanTuMoi);
        danhSachChon.AddRange(DanhMucService.LaySanPham());
        CboSanPham.ItemsSource = danhSachChon;

        DpTuNgay.SelectedDate = DateTime.Today.AddMonths(-3);
        DpDenNgay.SelectedDate = DateTime.Today;

        CboSanPham.SelectedIndex = 0; // mặc định mở lên là xem thẻ kho của TẤT CẢ sản phẩm

        if (CboSanPham.SelectedItem is not SanPham sp)
        {
            return;
        }

        DateTime tuNgay;
        if (DpTuNgay.SelectedDate == null)
            tuNgay = DateTime.Today.AddMonths(-3);
        else
            tuNgay = DpTuNgay.SelectedDate.Value;

        DateTime denNgay;
        if (DpDenNgay.SelectedDate == null)
            denNgay = DateTime.Today;
        else
            denNgay = DpDenNgay.SelectedDate.Value;

        if (sp.Id == MaSanPhamTatCa)
        {
            TxtMaSanPham.Text = "";
            TxtTenSanPham.Text = "Tất cả sản phẩm";
            GridKetQua.ItemsSource = ReportService.TheKho(null, tuNgay, denNgay);
        }
        else
        {
            TxtMaSanPham.Text = sp.Ma;
            TxtTenSanPham.Text = sp.Ten;
            GridKetQua.ItemsSource = ReportService.TheKho(sp.Id, tuNgay, denNgay);
        }
    }

    private void TimKiem_Changed(object sender, SelectionChangedEventArgs e)
    {
        if (CboSanPham.SelectedItem is not SanPham sp)
        {
            return;
        }

        DateTime tuNgay;
        if (DpTuNgay.SelectedDate == null)
            tuNgay = DateTime.Today.AddMonths(-3);
        else
            tuNgay = DpTuNgay.SelectedDate.Value;

        DateTime denNgay;
        if (DpDenNgay.SelectedDate == null)
            denNgay = DateTime.Today;
        else
            denNgay = DpDenNgay.SelectedDate.Value;

        if (sp.Id == MaSanPhamTatCa)
        {
            TxtMaSanPham.Text = "";
            TxtTenSanPham.Text = "Tất cả sản phẩm";
            GridKetQua.ItemsSource = ReportService.TheKho(null, tuNgay, denNgay);
        }
        else
        {
            TxtMaSanPham.Text = sp.Ma;
            TxtTenSanPham.Text = sp.Ten;
            GridKetQua.ItemsSource = ReportService.TheKho(sp.Id, tuNgay, denNgay);
        }
    }
}
