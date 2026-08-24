using System;
using System.Collections.Generic;

namespace SWP.Models;

public partial class ChiTietPhieuXuat
{
    public int Id { get; set; }

    public int PhieuXuatId { get; set; }

    public int SanPhamId { get; set; }

    public decimal SoLuongYeuCau { get; set; }

    public decimal? SoLuongThucXuat { get; set; }

    public decimal? DonGia { get; set; }

    public decimal? ThanhTien { get; set; }

    public virtual PhieuXuat PhieuXuat { get; set; } = null!;

    public virtual SanPham SanPham { get; set; } = null!;
}
