using System;
using System.Collections.Generic;

namespace SWP.Models;

public partial class PhieuNhap
{
    public int Id { get; set; }

    public string SoPhieu { get; set; } = null!;

    public DateTime NgayNhap { get; set; }

    public int? NhaCungCapId { get; set; }

    public string? SoHoaDon { get; set; }

    public string? NguoiGiaoHang { get; set; }

    public decimal TongTien { get; set; }

    public string TrangThai { get; set; } = null!;

    public int NguoiLapId { get; set; }

    public DateTime NgayLap { get; set; }

    public int? NguoiDuyetId { get; set; }

    public DateTime? NgayDuyet { get; set; }

    public string? LyDoHuy { get; set; }

    public string? GhiChu { get; set; }

    public virtual ICollection<ChiTietPhieuNhap> ChiTietPhieuNhaps { get; set; } = new List<ChiTietPhieuNhap>();

    public virtual NguoiDung? NguoiDuyet { get; set; }

    public virtual NguoiDung NguoiLap { get; set; } = null!;

    public virtual NhaCungCap? NhaCungCap { get; set; }
}
