namespace SWP.Models;

// Thông tin cần nhớ về người ĐANG ĐĂNG NHẬP trong phiên làm việc hiện tại.
// KHÔNG PHẢI bản ghi NguoiDung lấy thẳng từ DB (class NguoiDung ở Models/ dùng cho màn Quản lý người dùng
// và cho cột NguoiLap/NguoiDuyet trong phiếu nhập/xuất) — tách riêng ra 1 class nhỏ như thế này để đọc code
// không bị lẫn lộn giữa "bản ghi trong DB" và "ai đang ngồi trước máy dùng app".
public class NguoiDangNhap
{
    public int Id { get; set; }
    public string TenDangNhap { get; set; } = "";
    public string HoTen { get; set; } = "";
    public string VaiTro { get; set; } = "";
}
