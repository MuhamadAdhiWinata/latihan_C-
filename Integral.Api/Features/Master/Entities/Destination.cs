using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Integral.Api.Features.Master.Entities;

public class Destination
{
    public long Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string CustomerCode { get; set; }
    
    [StringLength(50)] [Unicode(false)] public string CreatedBy { get; set; } = null!;

    [Precision(0)] public DateTime CreatedDate { get; set; }

    [StringLength(50)] [Unicode(false)] public string? UpdatedBy { get; set; } = null!;

    [Precision(0)] public DateTime? UpdatedDate { get; set; }
}

