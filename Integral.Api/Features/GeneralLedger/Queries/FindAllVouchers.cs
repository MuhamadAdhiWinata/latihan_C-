using Integral.Api.Data.Contexts;
using Integral.Api.Features.GeneralLedger.Dto;
using Integral.App.Abstraction.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstraction.CQRS;
using SharedKernel.Abstraction.Web;

namespace Integral.Api.Features.GeneralLedger.Queries;

public record FindAllVouchers(string Search = "", DateTime? StartDate = null, DateTime? EndDate = null, bool? Approved = null) : IQuery<FindAllVouchersResult>
{
}

public record FindAllVouchersResult(VoucherDto[] Data)
{
}

public class FindAllVouchersHandler(PrintingDbContext dbContext) : IQueryHandler<FindAllVouchers, FindAllVouchersResult>
{
    public async Task<FindAllVouchersResult> Handle(FindAllVouchers request, CancellationToken cancellationToken)
    {
        var startDate = request.StartDate;
        var endDate = request.EndDate;

        if (startDate is null && endDate is null && string.IsNullOrWhiteSpace(request.Search))
        {
            endDate = DateTime.UtcNow.Date.AddDays(1);
            startDate = endDate.Value.AddMonths(-1);
        }
        
        endDate = endDate.Value.Date.AddDays(1).AddSeconds(-1);

        var query = dbContext.Journals
            .AsNoTracking()
            .Include(x => x.Lines)
            .OrderByDescending(x => x.TransactionDate)
            .AsQueryable();
        
        if (startDate is not null)
            query = query.Where(x => x.TransactionDate >= startDate);

        if (endDate is not null)
            query = query.Where(x => x.TransactionDate <= endDate);

        if (request.Approved is not null)
        {
            query = request.Approved.Value ? 
                query.Where(x => x.PostingStatus > 0) : 
                query.Where(x => x.PostingStatus == 0);
        }
        
        if (!string.IsNullOrWhiteSpace(request.Search))
            query = query.Where(x => x.VoucherNo.Contains(request.Search));
        
        var res = await query.Select(x => x.ToDto()).ToArrayAsync(cancellationToken);

        return new FindAllVouchersResult(res);
    }
}

public class FindAllVouchersEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{GeneralLedgerRoot.ApiPath}", Handle)
            .RequireAuthorization()
            .WithTags($"{GeneralLedgerRoot.Title}")
            .WithName($"Find All {GeneralLedgerRoot.Title}");
        
        builder.MapGet($"{GeneralLedgerRoot.ApiPath}/{{code}}",
                async ([FromServices] PrintingDbContext dbContext, string code) =>
                {
                    var res = await dbContext.Journals
                        .Include(x => x.Lines)
                        .Where(x => x.VoucherNo == code)
                        .Select(x => x.ToDto())
                        .FirstOrDefaultAsync();
                    
                    return Results.Ok(new { Data = res });
                })
            .RequireAuthorization()
            .WithTags($"{GeneralLedgerRoot.Title}")
            .WithName($"Find By Code {GeneralLedgerRoot.Title}");

        return builder;
    }

    private static async Task<IResult> Handle(
        [FromServices] IMediator mediator,
        [AsParameters] FindAllVouchers request)
    {
        var res = await mediator.Send(request);

        return Results.Ok(res);
    }
}