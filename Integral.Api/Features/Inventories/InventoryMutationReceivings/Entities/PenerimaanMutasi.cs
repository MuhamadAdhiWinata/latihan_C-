using System.ComponentModel.DataAnnotations.Schema;

namespace Integral.Api.Features.Inventories.InventoryMutationReceivings.Entities;

public class PenerimaanMutasi
{
    public long Id { get; set; }

    public string RefCode { get; set; } = null!;

    public DateTime TransactionDate { get; set; }

    public decimal GrandTotal { get; set; } = 0;
    public decimal TotalSellingPrice { get; set; } = 0;

    public string Status { get; set; }
    
    
    [InverseProperty(nameof(PenerimaanMutasiDetail.Header))]
    public virtual ICollection<PenerimaanMutasiDetail> Details { get; set; } = new List<PenerimaanMutasiDetail>();
}