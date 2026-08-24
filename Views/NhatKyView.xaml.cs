using System;
using System.Windows.Controls;
using SWP.Services;

namespace SWP.Views;

// QL03 — Xem và lọc nhật ký thao tác.
public partial class NhatKyView : UserControl
{
    public NhatKyView()
    {
        InitializeComponent();

        DpTuNgay.SelectedDate = DateTime.Today.AddMonths(-1);
        DpDenNgay.SelectedDate = DateTime.Today;
        CboHanhDong.ItemsSource = new[] { "(Tất cả)", "THEM", "SUA", "XOA", "DUYET", "HUY" };
        CboHanhDong.SelectedIndex = 0;

        DateTime tuNgay;
        if (DpTuNgay.SelectedDate == null)
            tuNgay = DateTime.Today.AddMonths(-1);
        else
            tuNgay = DpTuNgay.SelectedDate.Value;

        DateTime denNgay;
        if (DpDenNgay.SelectedDate == null)
            denNgay = DateTime.Today;
        else
            denNgay = DpDenNgay.SelectedDate.Value;

        var hanhDong = CboHanhDong.SelectedItem as string;
        string? hanhDongLoc;
        if (hanhDong == "(Tất cả)")
            hanhDongLoc = null;
        else
            hanhDongLoc = hanhDong;

        GridKetQua.ItemsSource = NhatKyService.LayDanhSach(tuNgay, denNgay, hanhDongLoc, null);
    }

    private void Loc_Changed(object sender, SelectionChangedEventArgs e)
    {
        DateTime tuNgay;
        if (DpTuNgay.SelectedDate == null)
            tuNgay = DateTime.Today.AddMonths(-1);
        else
            tuNgay = DpTuNgay.SelectedDate.Value;

        DateTime denNgay;
        if (DpDenNgay.SelectedDate == null)
            denNgay = DateTime.Today;
        else
            denNgay = DpDenNgay.SelectedDate.Value;

        var hanhDong = CboHanhDong.SelectedItem as string;
        string? hanhDongLoc;
        if (hanhDong == "(Tất cả)")
            hanhDongLoc = null;
        else
            hanhDongLoc = hanhDong;

        GridKetQua.ItemsSource = NhatKyService.LayDanhSach(tuNgay, denNgay, hanhDongLoc, null);
    }
}
