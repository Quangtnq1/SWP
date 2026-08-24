using System.Linq;
using System.Windows.Controls;
using SWP.Models;
using SWP.Services;

namespace SWP.Views;

// CN03 — Màn hình tổng quan.
public partial class DashboardView : UserControl
{
    public DashboardView()
    {
        InitializeComponent();

        string hoTen;
        if (Session.CurrentUser == null)
            hoTen = "";
        else
            hoTen = Session.CurrentUser.HoTen;
        TxtLoiChao.Text = $"Xin chào, {hoTen}";

        using var db = new QuanLyKhoVatTuContext();
        var sanPhams = db.SanPhams.Where(s => s.DangSuDung).ToList();

        TxtTongSanPham.Text = sanPhams.Count.ToString();
        TxtTongGiaTriTonKho.Text = sanPhams.Sum(s => s.TonKho * s.GiaVonBinhQuan).ToString("N0") + " đ";
        TxtPhieuNhapChoDuyet.Text = db.PhieuNhaps.Count(p => p.TrangThai == "CHODUYET").ToString();
        TxtPhieuXuatChoDuyet.Text = db.PhieuXuats.Count(p => p.TrangThai == "CHODUYET").ToString();

        var hangDuoiDinhMuc = ReportService.HangDuoiDinhMuc();
        TxtSoHangDuoiDinhMuc.Text = hangDuoiDinhMuc.Count.ToString();

        GridDuoiDinhMuc.ItemsSource = hangDuoiDinhMuc;
    }
}
