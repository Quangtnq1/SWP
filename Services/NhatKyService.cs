using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SWP.Models;

namespace SWP.Services;

// QL03 — Xem và lọc nhật ký thao tác. Chỉ đọc (RB15: nhật ký không được sửa/xóa nên không có hàm Update/Delete).
public static class NhatKyService
{
    public static List<NhatKyThaoTac> LayDanhSach(DateTime? tuNgay, DateTime? denNgay, string? hanhDong, string? doiTuong)
    {
        using var db = new QuanLyKhoVatTuContext();
        var query = db.NhatKyThaoTacs.AsQueryable();

        if (tuNgay.HasValue)
        {
            query = query.Where(n => n.ThoiGian >= tuNgay.Value.Date);
        }
        if (denNgay.HasValue)
        {
            query = query.Where(n => n.ThoiGian <= denNgay.Value.Date.AddDays(1).AddTicks(-1));
        }
        if (!string.IsNullOrWhiteSpace(hanhDong))
        {
            query = query.Where(n => n.HanhDong == hanhDong);
        }
        if (!string.IsNullOrWhiteSpace(doiTuong))
        {
            query = query.Where(n => n.DoiTuong == doiTuong);
        }

        return query.OrderByDescending(n => n.ThoiGian).Take(500).ToList();
    }
}
