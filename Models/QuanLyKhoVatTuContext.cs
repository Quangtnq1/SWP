using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SWP.Models;

public partial class QuanLyKhoVatTuContext : DbContext
{
    public QuanLyKhoVatTuContext()
    {
    }

    public QuanLyKhoVatTuContext(DbContextOptions<QuanLyKhoVatTuContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BoPhan> BoPhans { get; set; }

    public virtual DbSet<ChiTietPhieuNhap> ChiTietPhieuNhaps { get; set; }

    public virtual DbSet<ChiTietPhieuXuat> ChiTietPhieuXuats { get; set; }

    public virtual DbSet<NguoiDung> NguoiDungs { get; set; }

    public virtual DbSet<NhaCungCap> NhaCungCaps { get; set; }

    public virtual DbSet<NhatKyThaoTac> NhatKyThaoTacs { get; set; }

    public virtual DbSet<NhomHang> NhomHangs { get; set; }

    public virtual DbSet<PhieuNhap> PhieuNhaps { get; set; }

    public virtual DbSet<PhieuXuat> PhieuXuats { get; set; }

    public virtual DbSet<SanPham> SanPhams { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=DESKTOP-G3JJ958\\QUANG;Database=QuanLyKhoVatTu;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BoPhan>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__BoPhan__3214EC07C56ECF47");

            entity.ToTable("BoPhan");

            entity.HasIndex(e => e.Ma, "UQ_BoPhan_Ma").IsUnique();

            entity.Property(e => e.DangHoatDong).HasDefaultValue(true);
            entity.Property(e => e.Ma)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Ten).HasMaxLength(100);
            entity.Property(e => e.TruongBoPhan).HasMaxLength(100);
        });

        modelBuilder.Entity<ChiTietPhieuNhap>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ChiTietP__3214EC07A5838ACA");

            entity.ToTable("ChiTietPhieuNhap");

            entity.HasIndex(e => e.SanPhamId, "IX_CTPN_SanPham");

            entity.HasIndex(e => new { e.PhieuNhapId, e.SanPhamId }, "UQ_CTPN_Phieu_SanPham").IsUnique();

            entity.Property(e => e.DonGia).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.SoLuong).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ThanhTien).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.PhieuNhap).WithMany(p => p.ChiTietPhieuNhaps)
                .HasForeignKey(d => d.PhieuNhapId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CTPN_PhieuNhap");

            entity.HasOne(d => d.SanPham).WithMany(p => p.ChiTietPhieuNhaps)
                .HasForeignKey(d => d.SanPhamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CTPN_SanPham");
        });

        modelBuilder.Entity<ChiTietPhieuXuat>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ChiTietP__3214EC07FB85A4FE");

            entity.ToTable("ChiTietPhieuXuat");

            entity.HasIndex(e => e.SanPhamId, "IX_CTPX_SanPham");

            entity.HasIndex(e => new { e.PhieuXuatId, e.SanPhamId }, "UQ_CTPX_Phieu_SanPham").IsUnique();

            entity.Property(e => e.DonGia).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.SoLuongThucXuat).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.SoLuongYeuCau).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ThanhTien).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.PhieuXuat).WithMany(p => p.ChiTietPhieuXuats)
                .HasForeignKey(d => d.PhieuXuatId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CTPX_PhieuXuat");

            entity.HasOne(d => d.SanPham).WithMany(p => p.ChiTietPhieuXuats)
                .HasForeignKey(d => d.SanPhamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CTPX_SanPham");
        });

        modelBuilder.Entity<NguoiDung>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__NguoiDun__3214EC07AC045489");

            entity.ToTable("NguoiDung");

            entity.HasIndex(e => e.TenDangNhap, "UQ_NguoiDung_TenDangNhap").IsUnique();

            entity.Property(e => e.DangHoatDong).HasDefaultValue(true);
            entity.Property(e => e.DienThoai)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.HoTen).HasMaxLength(100);
            entity.Property(e => e.MatKhauHash)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TenDangNhap)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.VaiTro)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<NhaCungCap>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__NhaCungC__3214EC0758BB9BB6");

            entity.ToTable("NhaCungCap");

            entity.HasIndex(e => e.Ma, "UQ_NhaCungCap_Ma").IsUnique();

            entity.Property(e => e.DangHoatDong).HasDefaultValue(true);
            entity.Property(e => e.DiaChi).HasMaxLength(255);
            entity.Property(e => e.DienThoai)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Ma)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MaSoThue)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.NguoiLienHe).HasMaxLength(100);
            entity.Property(e => e.Ten).HasMaxLength(200);
        });

        modelBuilder.Entity<NhatKyThaoTac>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__NhatKyTh__3214EC074E812C91");

            entity.ToTable("NhatKyThaoTac");

            entity.HasIndex(e => new { e.DoiTuong, e.DoiTuongId }, "IX_NhatKy_DoiTuong");

            entity.HasIndex(e => e.ThoiGian, "IX_NhatKy_ThoiGian");

            entity.Property(e => e.DoiTuong).HasMaxLength(50);
            entity.Property(e => e.HanhDong)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MoTa).HasMaxLength(500);
            entity.Property(e => e.TenDangNhap).HasMaxLength(50);
            entity.Property(e => e.ThoiGian)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<NhomHang>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__NhomHang__3214EC0777967939");

            entity.ToTable("NhomHang");

            entity.HasIndex(e => e.Ma, "UQ_NhomHang_Ma").IsUnique();

            entity.Property(e => e.Ma)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MoTa).HasMaxLength(255);
            entity.Property(e => e.Ten).HasMaxLength(100);
        });

        modelBuilder.Entity<PhieuNhap>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PhieuNha__3214EC072A1E187A");

            entity.ToTable("PhieuNhap");

            entity.HasIndex(e => e.NgayNhap, "IX_PhieuNhap_NgayNhap");

            entity.HasIndex(e => e.TrangThai, "IX_PhieuNhap_TrangThai");

            entity.HasIndex(e => e.SoPhieu, "UQ_PhieuNhap_SoPhieu").IsUnique();

            entity.Property(e => e.GhiChu).HasMaxLength(500);
            entity.Property(e => e.LyDoHuy).HasMaxLength(255);
            entity.Property(e => e.NgayDuyet).HasColumnType("datetime");
            entity.Property(e => e.NgayLap)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NgayNhap).HasColumnType("datetime");
            entity.Property(e => e.NguoiGiaoHang).HasMaxLength(100);
            entity.Property(e => e.SoHoaDon)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.SoPhieu)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.TongTien).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("CHODUYET");

            entity.HasOne(d => d.NguoiDuyet).WithMany(p => p.PhieuNhapNguoiDuyets)
                .HasForeignKey(d => d.NguoiDuyetId)
                .HasConstraintName("FK_PhieuNhap_NguoiDuyet");

            entity.HasOne(d => d.NguoiLap).WithMany(p => p.PhieuNhapNguoiLaps)
                .HasForeignKey(d => d.NguoiLapId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PhieuNhap_NguoiLap");

            entity.HasOne(d => d.NhaCungCap).WithMany(p => p.PhieuNhaps)
                .HasForeignKey(d => d.NhaCungCapId)
                .HasConstraintName("FK_PhieuNhap_NhaCungCap");
        });

        modelBuilder.Entity<PhieuXuat>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PhieuXua__3214EC07AC5F9F7A");

            entity.ToTable("PhieuXuat");

            entity.HasIndex(e => e.BoPhanId, "IX_PhieuXuat_BoPhan");

            entity.HasIndex(e => e.NgayXuat, "IX_PhieuXuat_NgayXuat");

            entity.HasIndex(e => e.TrangThai, "IX_PhieuXuat_TrangThai");

            entity.HasIndex(e => e.SoPhieu, "UQ_PhieuXuat_SoPhieu").IsUnique();

            entity.Property(e => e.GhiChu).HasMaxLength(500);
            entity.Property(e => e.LoaiXuat)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.LyDoHuy).HasMaxLength(255);
            entity.Property(e => e.LyDoXuat).HasMaxLength(255);
            entity.Property(e => e.NgayDuyet).HasColumnType("datetime");
            entity.Property(e => e.NgayLap)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NgayXuat).HasColumnType("datetime");
            entity.Property(e => e.NguoiNhan).HasMaxLength(100);
            entity.Property(e => e.SoPhieu)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.TongTien).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("CHODUYET");

            entity.HasOne(d => d.BoPhan).WithMany(p => p.PhieuXuats)
                .HasForeignKey(d => d.BoPhanId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PhieuXuat_BoPhan");

            entity.HasOne(d => d.NguoiDuyet).WithMany(p => p.PhieuXuatNguoiDuyets)
                .HasForeignKey(d => d.NguoiDuyetId)
                .HasConstraintName("FK_PhieuXuat_NguoiDuyet");

            entity.HasOne(d => d.NguoiLap).WithMany(p => p.PhieuXuatNguoiLaps)
                .HasForeignKey(d => d.NguoiLapId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PhieuXuat_NguoiLap");
        });

        modelBuilder.Entity<SanPham>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SanPham__3214EC07985ACD96");

            entity.ToTable("SanPham");

            entity.HasIndex(e => e.NhomHangId, "IX_SanPham_NhomHang");

            entity.HasIndex(e => e.Ma, "UQ_SanPham_Ma").IsUnique();

            entity.Property(e => e.DangSuDung).HasDefaultValue(true);
            entity.Property(e => e.DonViTinh).HasMaxLength(20);
            entity.Property(e => e.GiaVonBinhQuan).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Ma)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.GhiChu).HasMaxLength(200);
            entity.Property(e => e.Ten).HasMaxLength(200);
            entity.Property(e => e.TonKho).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TonToiThieu).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.NhomHang).WithMany(p => p.SanPhams)
                .HasForeignKey(d => d.NhomHangId)
                .HasConstraintName("FK_SanPham_NhomHang");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
