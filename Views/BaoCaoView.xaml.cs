using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using SWP.Models;
using SWP.Services;

namespace SWP.Views;

// KT09 — 4 báo cáo (4 tab) + KT10 — Xuất CSV (đơn giản hóa thay cho .xlsx thật, đủ dùng, mở được bằng Excel).
public partial class BaoCaoView : UserControl
{
    // Sản phẩm giả Id = 0 chèn lên đầu ComboBox, đại diện lựa chọn "xem tất cả sản phẩm cùng lúc".
    private const int MaSanPhamTatCa = 0;

    public BaoCaoView()
    {
        InitializeComponent();

        DpTuNgay.SelectedDate = DateTime.Today.AddMonths(-3);
        DpDenNgay.SelectedDate = DateTime.Today;

        var danhSachChon = new List<SanPham>();
        var phanTuMoi = new SanPham();
        phanTuMoi.Id = MaSanPhamTatCa;
        phanTuMoi.Ma = "";
        phanTuMoi.Ten = "(Tất cả sản phẩm)";
        phanTuMoi.DonViTinh = "";
        danhSachChon.Add(phanTuMoi);
        danhSachChon.AddRange(DanhMucService.LaySanPham());
        CboSanPhamTheKho.ItemsSource = danhSachChon;
        CboSanPhamTheKho.SelectedIndex = 0; // mặc định "(Tất cả sản phẩm)"

        TaiLai();
    }

    private void Loc_Changed(object sender, SelectionChangedEventArgs e)
    {
        TaiLai();
    }

    private void TaiLai()
    {
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

        GridNhapXuatTon.ItemsSource = ReportService.NhapXuatTon(tuNgay, denNgay);
        GridChiPhi.ItemsSource = ReportService.ChiPhiTheoBoPhan(tuNgay, denNgay);
        GridHangDuoiDinhMuc.ItemsSource = ReportService.HangDuoiDinhMuc();

        if (CboSanPhamTheKho.SelectedItem is SanPham sp)
        {
            if (sp.Id == MaSanPhamTatCa)
                GridTheKho.ItemsSource = ReportService.TheKho(null, tuNgay, denNgay);
            else
                GridTheKho.ItemsSource = ReportService.TheKho(sp.Id, tuNgay, denNgay);
        }
    }

    // KT10 — Xuất báo cáo Nhập-Xuất-Tồn ra file CSV.
    private void BtnXuatCsvNhapXuatTon_Click(object sender, RoutedEventArgs e)
    {
        var data = (IEnumerable<NhapXuatTonRow>)GridNhapXuatTon.ItemsSource;

        var dialog = new SaveFileDialog();
        dialog.Filter = "CSV (*.csv)|*.csv";
        dialog.FileName = "NhapXuatTon_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".csv";
        if (dialog.ShowDialog() != true)
            return;

        // Mỗi ô bọc trong dấu " để dấu phẩy trong tên không bị hiểu thành dấu ngăn cột.
        var sb = new StringBuilder();
        sb.AppendLine("Ma,Ten,DVT,TonDauKy,TongNhap,TongXuat,TonCuoiKy");
        foreach (var r in data)
            sb.AppendLine($"\"{r.Ma}\",\"{r.Ten}\",\"{r.DonViTinh}\",\"{r.TonDauKy}\",\"{r.TongNhap}\",\"{r.TongXuat}\",\"{r.TonCuoiKy}\"");

        // UTF8Encoding(true) = ghi kèm BOM, thiếu nó Excel mở file lên sẽ hỏng tiếng Việt.
        File.WriteAllText(dialog.FileName, sb.ToString(), new UTF8Encoding(true));
        MessageBox.Show("Đã xuất file thành công.", "Hoàn tất", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    // KT10 — Xuất báo cáo Chi phí theo bộ phận ra file CSV.
    private void BtnXuatCsvChiPhi_Click(object sender, RoutedEventArgs e)
    {
        var data = (IEnumerable<ChiPhiBoPhanRow>)GridChiPhi.ItemsSource;

        var dialog = new SaveFileDialog();
        dialog.Filter = "CSV (*.csv)|*.csv";
        dialog.FileName = "ChiPhiTheoBoPhan_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".csv";
        if (dialog.ShowDialog() != true)
            return;

        var sb = new StringBuilder();
        sb.AppendLine("BoPhan,NhomHang,ThanhTien");
        foreach (var r in data)
            sb.AppendLine($"\"{r.BoPhan}\",\"{r.NhomHang}\",\"{r.ThanhTien}\"");

        File.WriteAllText(dialog.FileName, sb.ToString(), new UTF8Encoding(true));
        MessageBox.Show("Đã xuất file thành công.", "Hoàn tất", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    // KT10 — Xuất danh sách Hàng dưới định mức ra file CSV.
    private void BtnXuatCsvHangDuoiDinhMuc_Click(object sender, RoutedEventArgs e)
    {
        var data = (IEnumerable<SanPham>)GridHangDuoiDinhMuc.ItemsSource;

        var dialog = new SaveFileDialog();
        dialog.Filter = "CSV (*.csv)|*.csv";
        dialog.FileName = "HangDuoiDinhMuc_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".csv";
        if (dialog.ShowDialog() != true)
            return;

        var sb = new StringBuilder();
        sb.AppendLine("Ma,Ten,DVT,TonHienTai,TonToiThieu");
        foreach (var r in data)
            sb.AppendLine($"\"{r.Ma}\",\"{r.Ten}\",\"{r.DonViTinh}\",\"{r.TonKho}\",\"{r.TonToiThieu}\"");

        File.WriteAllText(dialog.FileName, sb.ToString(), new UTF8Encoding(true));
        MessageBox.Show("Đã xuất file thành công.", "Hoàn tất", MessageBoxButton.OK, MessageBoxImage.Information);
    }
}
