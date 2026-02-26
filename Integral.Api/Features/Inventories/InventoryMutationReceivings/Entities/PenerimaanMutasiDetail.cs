using System.ComponentModel.DataAnnotations.Schema;

namespace Integral.Api.Features.Inventories.InventoryMutationReceivings.Entities;

public class PenerimaanMutasiDetail
{
    public long Id { get; set; }

    public long PenerimaanMutasiId { get; set; }

    public long MutationItemId { get; set; }

    public string KodeProduksi { get; set; } = null!;

    public string BarangId { get; set; } = null!;

    public string? BarangDetailId { get; set; }

    public decimal Quantity { get; set; }
    
    public decimal QuantityReceived { get; set; }

    public decimal Harga { get; set; }
    
    public decimal HargaJual { get; set; }
    
    public string User { get; set; } = "";
    
    [ForeignKey(nameof(PenerimaanMutasiId))]
    [InverseProperty(nameof(PenerimaanMutasi.Details))]
    public virtual PenerimaanMutasi Header { get; set; } = null!;
}