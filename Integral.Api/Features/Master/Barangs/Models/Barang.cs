using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Integral.Api.Features.Master.Barangs.Models;

[Table("barangs")]
[Index("BarangAsalId", Name = "barangs_barang_asal_id_foreign")]
[Index("KategoriId", Name = "barangs_kategori_id_foreign")]
[Index("KategoriPerkiraanId", Name = "barangs_kategori_perkiraan_id_foreign")]
[Index("PosisiBarangId", Name = "barangs_posisi_barang_id_foreign")]
[Index("TipeBarangId", Name = "barangs_tipe_barang_id_foreign")]
[MySqlCollation("utf8mb4_unicode_ci")]
public class Barang
{
    [Key]
    [Column("id", TypeName = "bigint(20) unsigned")]
    public ulong Id { get; set; }

    [Column("kode_barang")]
    [StringLength(255)]
    public string KodeBarang { get; set; } = null!;

    [Column("kategori_id", TypeName = "bigint(20) unsigned")]
    public ulong? KategoriId { get; set; }

    [Column("nama_barang")]
    [StringLength(255)]
    public string NamaBarang { get; set; } = null!;

    // [Column("stok")] [StringLength(255)] public string? Stok { get; set; }

    [Column("markup")] [StringLength(255)] public string? Markup { get; set; }

    [Column("satuan")] [StringLength(255)] public string? Satuan { get; set; }

    [Column("harga_jual")]
    [StringLength(255)]
    public string? HargaJual { get; set; }

    [Column("diskon_persen")]
    [StringLength(255)]
    public string? DiskonPersen { get; set; }

    [Column("diskon_nominal")]
    [StringLength(255)]
    public string? DiskonNominal { get; set; }

    [Column("posisi_barang_id", TypeName = "bigint(20) unsigned")]
    public ulong? PosisiBarangId { get; set; }

    [Column("tipe_barang_id", TypeName = "bigint(20) unsigned")]
    public ulong? TipeBarangId { get; set; }

    [Column("minimum")]
    [StringLength(255)]
    public string? Minimum { get; set; }

    [Column("buffering")]
    [StringLength(255)]
    public string? Buffering { get; set; }

    [Column("kategori_perkiraan_id", TypeName = "bigint(20) unsigned")]
    public ulong? KategoriPerkiraanId { get; set; }

    [Column("created_at", TypeName = "timestamp")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at", TypeName = "timestamp")]
    public DateTime? UpdatedAt { get; set; }

    [Column("barang_asal_id", TypeName = "bigint(20) unsigned")]
    public ulong? BarangAsalId { get; set; }

    [Column("kepemilikan_barang")]
    [StringLength(255)]
    public string? KepemilikanBarang { get; set; }
}