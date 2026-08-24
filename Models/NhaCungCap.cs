using System;
using System.Collections.Generic;

namespace SWP.Models;

public partial class NhaCungCap
{
    public int Id { get; set; }

    public string Ma { get; set; } = null!;

    public string Ten { get; set; } = null!;

    public string? MaSoThue { get; set; }

    public string? DiaChi { get; set; }

    public string? DienThoai { get; set; }

    public string? NguoiLienHe { get; set; }

    public bool DangHoatDong { get; set; }

    public virtual ICollection<PhieuNhap> PhieuNhaps { get; set; } = new List<PhieuNhap>();
}
