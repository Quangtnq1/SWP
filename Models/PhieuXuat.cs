using System;
using System.Collections.Generic;

namespace SWP.Models;

public partial class PhieuXuat
{
    public int Id { get; set; }

    public string SoPhieu { get; set; } = null!;

    public DateTime NgayXuat { get; set; }

    public string LoaiXuat { get; set; } = null!;

    public int BoPhanId { get; set; }

    public string NguoiNhan { get; set; } = null!;

    public string LyDoXuat { get; set; } = null!;

    public decimal? TongTien { get; set; }

    public string TrangThai { get; set; } = null!;

    public int NguoiLapId { get; set; }

    public DateTime NgayLap { get; set; }

    public int? NguoiDuyetId { get; set; }

    public DateTime? NgayDuyet { get; set; }

    public string? LyDoHuy { get; set; }

    public string? GhiChu { get; set; }

    public virtual BoPhan BoPhan { get; set; } = null!;

    public virtual ICollection<ChiTietPhieuXuat> ChiTietPhieuXuats { get; set; } = new List<ChiTietPhieuXuat>();

    public virtual NguoiDung? NguoiDuyet { get; set; }

    public virtual NguoiDung NguoiLap { get; set; } = null!;
}
