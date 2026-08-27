// Writes append-only entries to NhatKyThaoTac inside the same transaction as the data change it records.
using System;
using SWP.Models;

namespace SWP.Services;

// RB15: nhật ký chỉ được THÊM, không sửa không xóa.
// TenDangNhap được chép lại chứ không chỉ tham chiếu Id, để đọc được cả khi tài khoản bị xóa sau này.
public static class AuditLogService
{
    public static void Log(QuanLyKhoVatTuContext db, NguoiDangNhap? nguoiDung, string hanhDong, string doiTuong, int? doiTuongId, string moTa)
    {
        int nguoiDungId;
        string tenDangNhap;
        if (nguoiDung == null)
        {
            nguoiDungId = 0;
            tenDangNhap = "(unknown)";
        }
        else
        {
            nguoiDungId = nguoiDung.Id;
            tenDangNhap = nguoiDung.TenDangNhap;
        }

        var banGhiNhatKy = new NhatKyThaoTac();
        banGhiNhatKy.ThoiGian = DateTime.Now;
        banGhiNhatKy.NguoiDungId = nguoiDungId;
        banGhiNhatKy.TenDangNhap = tenDangNhap;
        banGhiNhatKy.HanhDong = hanhDong;
        banGhiNhatKy.DoiTuong = doiTuong;
        banGhiNhatKy.DoiTuongId = doiTuongId;
        banGhiNhatKy.MoTa = moTa;
        db.NhatKyThaoTacs.Add(banGhiNhatKy);
    }
}
