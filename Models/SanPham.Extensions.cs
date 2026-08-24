namespace SWP.Models;

// Mở rộng class SanPham do EF Core scaffold sinh ra (SanPham.cs) bằng 1 property tính toán, không map cột DB.
// Tách file riêng để lần sau chạy lại "dotnet ef dbcontext scaffold --force" không bị ghi đè mất phần này.
public partial class SanPham
{
    public bool IsDuoiDinhMuc
    {
        get { return TonKho < TonToiThieu; }
    }
}
