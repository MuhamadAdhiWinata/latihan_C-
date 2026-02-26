using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Integral.Api.Features.Master.Entities;

[Table("f106")]
public class Customer
{
    [StringLength(50)] [Unicode(false)] public string BranchCode { get; set; } = null!;

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string Code { get; set; } = null!;

    [StringLength(50)] [Unicode(false)] public string AccNo { get; set; } = null!;

    [StringLength(100)] [Unicode(false)] public string Name { get; set; } = null!;

    [StringLength(50)] [Unicode(false)] public string BrandName { get; set; } = null!;

    [StringLength(50)] [Unicode(false)] public string CompanyName { get; set; } = null!;

    [StringLength(255)] [Unicode(false)] public string Address { get; set; } = null!;

    [StringLength(200)] [Unicode(false)] public string MailingAddress { get; set; } = null!;

    [StringLength(50)] [Unicode(false)] public string ZipCode { get; set; } = null!;

    [Column("NPWP")]
    [StringLength(50)]
    [Unicode(false)]
    public string Npwp { get; set; } = null!;

    [Column("NPWPan")]
    [StringLength(200)]
    [Unicode(false)]
    public string Npwpan { get; set; } = null!;

    [Column("NPWPAddress")]
    [StringLength(255)]
    [Unicode(false)]
    public string Npwpaddress { get; set; } = null!;

    [StringLength(50)] [Unicode(false)] public string CityCode { get; set; } = null!;

    [StringLength(50)] [Unicode(false)] public string CountryCode { get; set; } = null!;

    [StringLength(50)] [Unicode(false)] public string Phone1 { get; set; } = null!;

    [StringLength(50)] [Unicode(false)] public string Phone2 { get; set; } = null!;

    [StringLength(20)] [Unicode(false)] public string Fax { get; set; } = null!;

    [StringLength(50)] [Unicode(false)] public string ContactPerson { get; set; } = null!;

    [StringLength(50)] [Unicode(false)] public string ContactPerson2 { get; set; } = null!;

    [StringLength(50)] [Unicode(false)] public string CompanyTypeCode { get; set; } = null!;

    [StringLength(50)] [Unicode(false)] public string CustomerType { get; set; } = null!;

    [Column(TypeName = "decimal(18, 4)")] public decimal CreditLimit { get; set; }

    [StringLength(50)] [Unicode(false)] public string EmailAddress { get; set; } = null!;

    public DateOnly PeriodFromDate { get; set; }

    public DateOnly PeriodTodate { get; set; }

    [StringLength(12)] [Unicode(false)] public string Status { get; set; } = null!;

    [StringLength(1)] [Unicode(false)] public string Type { get; set; } = null!;

    [Column(TypeName = "decimal(10, 4)")] public decimal DiscDelivery { get; set; }

    [Column(TypeName = "decimal(10, 4)")] public decimal DiscFee { get; set; }

    [StringLength(50)] [Unicode(false)] public string DeliveryTypeCode { get; set; } = null!;

    [StringLength(50)] [Unicode(false)] public string PaymentTypeCode { get; set; } = null!;

    [StringLength(50)] [Unicode(false)] public string SalesmanCode { get; set; } = null!;

    [Column(TypeName = "decimal(10, 4)")] public decimal Komisi { get; set; }

    [StringLength(50)] [Unicode(false)] public string AccountCode { get; set; } = null!;

    [Column("YesChkPPh4")] public short YesChkPph4 { get; set; }

    public short ActiveStatus { get; set; }

    [StringLength(50)] [Unicode(false)] public string CreatedBy { get; set; } = null!;

    [Precision(0)] public DateTime CreatedDate { get; set; }

    [StringLength(50)] [Unicode(false)] public string UpdatedBy { get; set; } = null!;

    [Precision(0)] public DateTime UpdatedDate { get; set; }

    [Column("KTP")]
    [StringLength(50)]
    [Unicode(false)]
    public string Ktp { get; set; } = null!;

    [Column("birthdate")] public DateOnly Birthdate { get; set; }
}