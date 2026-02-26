using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace SharedKernel.Abstraction.Ef;

public interface ICreatedAuditable
{
    [StringLength(50)] [Unicode(false)] 
    public string CreatedBy { get; set; }
    
    [Precision(0)] 
    public DateTime CreatedDate { get; set; }
}

public interface IUpdatedAuditable
{
    [StringLength(50)] [Unicode(false)] 
    public string? UpdatedBy { get; set; }
    
    [Precision(0)] 
    public DateTime? UpdatedDate { get; set; }
}

public interface IDeleteAuditable
{
    [StringLength(50)] [Unicode(false)] 
    public string? DeletedBy { get; set; }
    
    [Precision(0)] 
    public DateTime? DeletedDate { get; set; }
}

