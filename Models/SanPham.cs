using System;
using System.Collections.Generic;

namespace SWP.Models;

public partial class SanPham
{
    public int Id { get; set; }

    public string Ma { get; set; } = null!;

    public string Ten { get; set; } = null!;

    public int? NhomHangId { get; set; }

    public string DonViTinh { get; set; } = null!;

    public decimal GiaVonBinhQuan { get; set; }

    public decimal TonKho { get; set; }

    public decimal TonToiThieu { get; set; }

    public string? GhiChu { get; set; }

    public bool DangSuDung { get; set; }

    public DateTime NgayTao { get; set; }

    public virtual ICollection<ChiTietPhieuNhap> ChiTietPhieuNhaps { get; set; } = new List<ChiTietPhieuNhap>();

    public virtual ICollection<ChiTietPhieuXuat> ChiTietPhieuXuats { get; set; } = new List<ChiTietPhieuXuat>();

    public virtual NhomHang? NhomHang { get; set; }
}
