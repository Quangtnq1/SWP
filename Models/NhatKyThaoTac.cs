using System;
using System.Collections.Generic;

namespace SWP.Models;

public partial class NhatKyThaoTac
{
    public int Id { get; set; }

    public DateTime ThoiGian { get; set; }

    public int NguoiDungId { get; set; }

    public string TenDangNhap { get; set; } = null!;

    public string HanhDong { get; set; } = null!;

    public string DoiTuong { get; set; } = null!;

    public int? DoiTuongId { get; set; }

    public string? MoTa { get; set; }
}
