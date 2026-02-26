using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Integral.Api.Features.Master.Barangs.Models;

[Table("barang_details")]
[Index("KodeBarang", Name = "kode")]
[MySqlCollation("utf8mb4_unicode_ci")]
public class BarangDetail
{
    [Key]
    [Column("id", TypeName = "bigint(20) unsigned")]
    public ulong Id { get; set; }

    [Column("kelola_barang_id", TypeName = "int(11)")]
    public int? KelolaBarangId { get; set; }

    [Column("barang_id", TypeName = "bigint(20) unsigned")]
    public ulong? BarangId { get; set; }

    [Column("kode_barang")] public string? KodeBarang { get; set; }

    [Column("stok")] [StringLength(255)] public string? Stok { get; set; }

    [Column("stok_in")]
    [StringLength(255)]
    public string? StokIn { get; set; }

    [Column("stok_out")]
    [StringLength(255)]
    public string? StokOut { get; set; }

    [Column("bongkar_in", TypeName = "int(11)")]
    public int? BongkarIn { get; set; }

    [Column("ganti_cover_out", TypeName = "int(11)")]
    public int? GantiCoverOut { get; set; }

    [Column("harga_beli2")]
    [StringLength(255)]
    public string? HargaBeli2 { get; set; }

    [Column("harga_beli", TypeName = "double(11,2)")]
    public double? HargaBeli { get; set; }

    [Column("harga_jual")]
    [StringLength(255)]
    public string? HargaJual { get; set; }

    [Column("po_id", TypeName = "bigint(20) unsigned")]
    public ulong? PoId { get; set; }

    [Column("ket")] [StringLength(255)] public string? Ket { get; set; }

    [Column("po_d1_id", TypeName = "bigint(20) unsigned")]
    public ulong? PoD1Id { get; set; }

    [Column("asal_barang_detail_id", TypeName = "int(11)")]
    public int? AsalBarangDetailId { get; set; }

    [Column("barang_ganti_cover_id", TypeName = "bigint(20)")]
    public long? BarangGantiCoverId { get; set; }

    [Column("created_at", TypeName = "timestamp")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at", TypeName = "timestamp")]
    public DateTime? UpdatedAt { get; set; }

    [Column("id_stock", TypeName = "int(11)")]
    public int? IdStock { get; set; }

    [Column("perusahaan_id", TypeName = "bigint(20)")]
    public long? PerusahaanId { get; set; }

    [Column("jenis_barang")]
    [StringLength(255)]
    public string? JenisBarang { get; set; }

    // [InverseProperty("BarangDetail")]
    // public virtual ICollection<StokItemDetail> StokItemDetails { get; set; } = new List<StokItemDetail>();
}