using System.Text.RegularExpressions;

namespace SWP.Services;

// Dọn dữ liệu người dùng gõ tay trước khi lưu: bỏ ký tự rác, gộp dấu cách, cắt khoảng trắng thừa.
public static class VanBanHelper
{
    private static readonly Regex KyTuRac = new(@"[!@#$%^&*()_+=\{\}\[\]|\\<>~]");

    // CHỈ dấu cách, không đụng tới ký tự xuống dòng của ô Mô tả nhiều dòng.
    private static readonly Regex NhieuDauCach = new(" {2,}");

    private static readonly Regex ChiChuVaSo = new("[^a-zA-Z0-9]");

    public static string LamSachVanBan(string? input)
    {
        if (input == null)
            return "";

        var daXoaKyTuRac = KyTuRac.Replace(input, "");
        var daGomDauCach = NhieuDauCach.Replace(daXoaKyTuRac, " ");
        return daGomDauCach.Trim();
    }

    // Ô "Mã": chỉ giữ chữ+số rồi viết hoa toàn bộ, theo quy ước dữ liệu mẫu (VPP, NCC01, VPP001).
    public static string LamSachMa(string? input)
    {
        if (input == null)
            return "";

        return ChiChuVaSo.Replace(input, "").ToUpper();
    }

    // Khác LamSachMa ở chỗ KHÔNG ép viết hoa, vì tài khoản mẫu toàn chữ thường: thukho1, ketoan1.
    public static string LamSachTenDangNhap(string? input)
    {
        if (input == null)
            return "";

        return ChiChuVaSo.Replace(input, "");
    }
}
