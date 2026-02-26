using Integral.Api.Data.Contexts;
using Integral.Api.Features.Master.Entities;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstraction.Web;

namespace Integral.Api.Features.Master.Endpoints;

public record DestinationDto(long Id, string Code, string Name, string Description, string CustomerCode);

public record DestinationPostRequest(string Code, string Name, string Description, string CustomerCode);

public record DestinationPutRequest(string? Code, string? Name, string? Description, string? CustomerCode);

public static class DestinationMapper
{
    public static DestinationDto ToDto(this Destination dest)
    {
        return new DestinationDto(dest.Id, dest.Code, dest.Name,  dest.Description, dest.CustomerCode);
    }
}

public class DestinationEndpoint(PrintingDbContext dbContext, CurrentUserProvider user) : IMinimalEndpoint
{
    private const string BaseApiPath = $"{MasterRoot.BaseApiPath}/destinations";

    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup(BaseApiPath).WithTags("Master Destinations").RequireAuthorization();

        group.MapGet("", async (int limit = 100, int offset = 0, string? search = null, string? customerCode = null ) =>
        {
            var query = dbContext.Destinations
                .AsNoTracking()
                .OrderBy(x => x.Code).AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(x =>
                    x.Code.Contains(search) ||
                    x.Name.Contains(search));
            
            if (!string.IsNullOrWhiteSpace(customerCode))
                query = query.Where(x => x.CustomerCode == customerCode);

            var res = await query.Skip(offset).Take(limit).Select(x => x.ToDto()).ToListAsync();

            return Results.Ok(new
            {
                count = await query.CountAsync(),
                data = res
            });
        });

        group.MapGet("/{code}", async (string code) =>
        {
            var destination = await dbContext.Destinations
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Code == code);

            return destination is null
                ? Results.NotFound()
                : Results.Ok(destination.ToDto());
        });

        group.MapPost("", async (DestinationPostRequest request) =>
        {
            var destination = new Destination
            {
                Code = request.Code,
                Name = request.Name,
                Description = request.Description,
                CustomerCode = request.CustomerCode,
                CreatedBy = user.GetUsername(),
                CreatedDate = DateTime.Now
            };

            await dbContext.Destinations.AddAsync(destination);
            await dbContext.SaveChangesAsync();
            return Results.Ok(destination.ToDto());
        });

        group.MapPut("{code}", async (DestinationPutRequest request, string code) =>
        {
            var destination = await dbContext.Destinations.FirstOrDefaultAsync(x => x.Code == code);
            if (destination == null) return Results.NotFound();

            destination.Code = request.Code ?? destination.Code;
            destination.Name = request.Name ?? destination.Name;
            destination.Description = request.Description ?? destination.Description;
            destination.CustomerCode = request.CustomerCode ?? destination.CustomerCode;

            await dbContext.SaveChangesAsync();
            return Results.Ok(destination.ToDto());
        });

        return builder;
    }
}