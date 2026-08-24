using System;
using System.Collections.Generic;

namespace SWP.Models;

public partial class BoPhan
{
    public int Id { get; set; }

    public string Ma { get; set; } = null!;

    public string Ten { get; set; } = null!;

    public string? TruongBoPhan { get; set; }

    public int? SoNhanSu { get; set; }

    public bool DangHoatDong { get; set; }

    public virtual ICollection<PhieuXuat> PhieuXuats { get; set; } = new List<PhieuXuat>();
}
