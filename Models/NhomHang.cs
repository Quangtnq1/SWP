using System;
using System.Collections.Generic;

namespace SWP.Models;

public partial class NhomHang
{
    public int Id { get; set; }

    public string Ma { get; set; } = null!;

    public string Ten { get; set; } = null!;

    public string? MoTa { get; set; }

    public virtual ICollection<SanPham> SanPhams { get; set; } = new List<SanPham>();
}
