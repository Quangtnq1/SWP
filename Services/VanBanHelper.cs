using System.Text.RegularExpressions;

namespace SWP.Services;

// Dọn dữ liệu người dùng gõ tay trước khi lưu: bỏ ký tự rác, gộp dấu cách, cắt khoảng trắng thừa.
public static class VanBanHelper
{
    private static readonly Regex KyTuRac = new(@"[!@#$%^&*()_+=\{\}\[\]|\\<>~]");

    // CHỈ dấu cách, không đụng tới ký tự xuống dòng của ô Mô tả nhiều dòng.
    private static readonly Regex NhieuDauCach = new(" {2,}");

    private static readonly Regex ChiChuVaSo = new("[^a-zA-Z0-9]");

    // Dùng cho ô "chỉ cho chữ và số" (Tên, Họ tên...) — KHÁC ChiChuVaSo ở chỗ giữ tiếng Việt có dấu và khoảng trắng
    // giữa các từ, chỉ xóa dấu câu/ký hiệu. \p{L} = mọi chữ cái Unicode (bao gồm chữ có dấu tiếng Việt).
    private static readonly Regex KyTuKhongPhaiChuSo = new(@"[^\p{L}0-9\s]");

    public static string LamSachVanBan(string? input)
    {
        if (input == null)
            return "";

        var daXoaKyTuRac = KyTuRac.Replace(input, "");
        var daGomDauCach = NhieuDauCach.Replace(daXoaKyTuRac, " ");
        return daGomDauCach.Trim();
    }

    // Ô bắt buộc "chỉ chữ và số": xóa hết dấu câu/ký hiệu, giữ nguyên chữ tiếng Việt có dấu và khoảng trắng.
    public static string LamSachChuVaSo(string? input)
    {
        if (input == null)
            return "";

        var daXoaDauCau = KyTuKhongPhaiChuSo.Replace(input, "");
        var daGomDauCach = NhieuDauCach.Replace(daXoaDauCau, " ");
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
