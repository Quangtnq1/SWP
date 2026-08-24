using System;
using System.Collections.Generic;

namespace SWP.Models;

public partial class NguoiDung
{
    public int Id { get; set; }

    public string TenDangNhap { get; set; } = null!;

    public string MatKhauHash { get; set; } = null!;

    public string HoTen { get; set; } = null!;

    public string VaiTro { get; set; } = null!;

    public string? DienThoai { get; set; }

    public bool DangHoatDong { get; set; }

    public DateTime NgayTao { get; set; }

    public virtual ICollection<PhieuNhap> PhieuNhapNguoiDuyets { get; set; } = new List<PhieuNhap>();

    public virtual ICollection<PhieuNhap> PhieuNhapNguoiLaps { get; set; } = new List<PhieuNhap>();

    public virtual ICollection<PhieuXuat> PhieuXuatNguoiDuyets { get; set; } = new List<PhieuXuat>();

    public virtual ICollection<PhieuXuat> PhieuXuatNguoiLaps { get; set; } = new List<PhieuXuat>();
}
