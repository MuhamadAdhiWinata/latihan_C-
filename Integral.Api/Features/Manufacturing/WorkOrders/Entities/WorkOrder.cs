using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Integral.Api.Features.Manufacturing.WorkOrders.Entities;

[Table("f605_b")]
public class WorkOrder
{
    [StringLength(50)] [Unicode(false)] public string BranchCode { get; set; } = null!;

    [Key]
    [Column("DODNo")]
    [StringLength(50)]
    [Unicode(false)]
    public string Dodno { get; set; } = null!;

    [Precision(0)] public DateTime TransactionDate { get; set; }

    [StringLength(50)] [Unicode(false)] public string? CurrencyCode { get; set; }

    [Column(TypeName = "decimal(18, 4)")] public decimal? ExchangeRate { get; set; }

    [StringLength(50)] [Unicode(false)] public string CustomerCode { get; set; } = null!;

    [StringLength(50)] [Unicode(false)] public string? SalesmanCode { get; set; }

    [StringLength(50)] [Unicode(false)] public string? DriverCode { get; set; }

    [StringLength(50)] [Unicode(false)] public string? ExpeditionCode { get; set; }
    
    public string Status { get; set; }

    [Column("SODNo")]
    [StringLength(50)]
    [Unicode(false)]
    public string Sodno { get; set; } = null!;

    [StringLength(50)] [Unicode(false)] public string? WarehouseCode { get; set; }

    public short? TaxStatus { get; set; }

    [Unicode(false)] public string Description { get; set; } = null!;

    [Column("prepressinstruction")]
    [Unicode(false)]
    public string? Prepressinstruction { get; set; }

    [Column("pressinstruction")]
    [Unicode(false)]
    public string? Pressinstruction { get; set; }

    [Column("finishinginstruction")]
    [Unicode(false)]
    public string? Finishinginstruction { get; set; }

    [Column("packaginginstruction")]
    [Unicode(false)]
    public string? Packaginginstruction { get; set; }

    [Column(TypeName = "decimal(18, 4)")] public decimal? TotalTransaction { get; set; }

    [Column(TypeName = "decimal(4, 2)")] public decimal? DiscountPercent { get; set; }

    [Column(TypeName = "decimal(18, 4)")] public decimal? DiscountAmount { get; set; }

    [Column("PPNPercent", TypeName = "decimal(4, 2)")]
    public decimal? Ppnpercent { get; set; }

    [Column("PPNAmount", TypeName = "decimal(18, 4)")]
    public decimal? Ppnamount { get; set; }

    [Column(TypeName = "decimal(18, 4)")] public decimal? GrandTotal { get; set; }

    public short DeleteStatus { get; set; }

    [StringLength(50)] [Unicode(false)] public string CreatedBy { get; set; } = null!;

    [Precision(0)] public DateTime CreatedDate { get; set; }

    [StringLength(50)] [Unicode(false)] public string UpdatedBy { get; set; } = null!;

    [Precision(0)] public DateTime UpdatedDate { get; set; }

    [Column("SPDescription")]
    [Unicode(false)]
    public string Spdescription { get; set; } = null!;

    [Column("ALTERdBy")]
    [StringLength(50)]
    [Unicode(false)]
    public string? AlterdBy { get; set; }

    [Column("ALTERdDate")] public DateTime? AlterdDate { get; set; }

    public bool? Approved { get; set; }

    public bool? Approved2 { get; set; }

    [Column("isprepress")] public short Isprepress { get; set; }

    [Column("ispress")] public short Ispress { get; set; }

    [Column("isfinishing")] public short Isfinishing { get; set; }

    [Column("ispackaging")] public short Ispackaging { get; set; }

    
    
    [InverseProperty(nameof(WorkOrderItem.WorkOrder))]
    public virtual ICollection<WorkOrderItem> Items { get; set; } = null!;
    
    [InverseProperty(nameof(WorkOrderMaterial.WorkOrder))]
    public virtual ICollection<WorkOrderMaterial> Materials { get; set; } = null!;
    
    [InverseProperty(nameof(WorkOrderMachine.WorkOrder))]
    public virtual ICollection<WorkOrderMachine> Machines { get; set; } = null!;
    
    [InverseProperty(nameof(WorkOrderLabor.WorkOrder))]
    public virtual ICollection<WorkOrderLabor> Labors { get; set; } = null!;
    
    [InverseProperty(nameof(WorkOrderFinishing.WorkOrder))]
    public virtual ICollection<WorkOrderFinishing> Finishings { get; set; } = null!;
    
    [InverseProperty(nameof(WorkOrderPackaging.WorkOrder))]
    public virtual ICollection<WorkOrderPackaging> Packagings { get; set; } = null!;

    public void Close()
    {
        Status = WorkOrderStatus.Finished;
    }

    public void Partial()
    {
        Status = WorkOrderStatus.Partial;
    }
}