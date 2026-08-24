using System;
using System.Collections.Generic;

namespace SWP.Models;

public partial class ChiTietPhieuNhap
{
    public int Id { get; set; }

    public int PhieuNhapId { get; set; }

    public int SanPhamId { get; set; }

    public decimal SoLuong { get; set; }

    public decimal DonGia { get; set; }

    public decimal ThanhTien { get; set; }

    public virtual PhieuNhap PhieuNhap { get; set; } = null!;

    public virtual SanPham SanPham { get; set; } = null!;
}
