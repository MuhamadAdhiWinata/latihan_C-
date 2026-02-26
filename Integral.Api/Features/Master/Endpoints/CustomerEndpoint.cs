using Integral.Api.Data.Contexts;
using Integral.Api.Features.Master.Entities;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using SharedKernel.Abstraction.Web;

namespace Integral.Api.Features.Master.Endpoints;

public record CustomerPostRequest(
    string Name,
    string BrandName,
    string CompanyName,
    string Address,
    string MailAddress,
    string ZipCode,
    string Npwp,
    string NpwpAddress,
    string CityCode,
    string CountryCode,
    string Phone1,
    string Phone2,
    string CustomerType,
    string Ktp,
    DateOnly BirthDate);

public record CustomerPutRequest(
    string? Name,
    string? BrandName,
    string? CompanyName,
    string? Address,
    string? MailAddress,
    string? ZipCode,
    string? Npwp,
    string? NpwpAddress,
    string? CityCode,
    string? CountryCode,
    string? Phone1,
    string? Phone2,
    string? CustomerType,
    string? Ktp,
    DateOnly? BirthDate);

public record CustomerDto(
    string Code,
    string Name,
    string BrandName,
    string CompanyName,
    string Address,
    string MailAddress,
    string ZipCode,
    string Npwp,
    string NpwpAddress,
    string CityCode,
    string CountryCode,
    string Phone1,
    string Phone2,
    string CustomerType,
    string Ktp,
    DateOnly BirthDate);

public static class CustomerMapper
{
    public static CustomerDto ToDto(this Customer entity)
    {
        return new CustomerDto(
            entity.Code,
            entity.Name,
            entity.BrandName,
            entity.CompanyName,
            entity.Address,
            entity.MailingAddress,
            entity.ZipCode,
            entity.Npwp,
            entity.Npwpaddress,
            entity.CityCode,
            entity.CountryCode,
            entity.Phone1,
            entity.Phone2,
            entity.CustomerType,
            entity.Ktp,
            entity.Birthdate
        );
    }
}

public class CustomerEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup($"{MasterRoot.BaseApiPath}/customers").WithTags("Master Customers").RequireAuthorization();

        group.MapGet("", async (PrintingDbContext dbContext, int limit = 100, int offset = 0, string? search = null) =>
        {
            var query = dbContext.Customers.AsNoTracking().OrderBy(x => x.Code).AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(x =>
                    x.Name.Contains(search) ||
                    x.Code.Contains(search) ||
                    x.BrandName.Contains(search) ||
                    x.CompanyName.Contains(search) ||
                    x.ContactPerson.Contains(search));

            var res = await query.Skip(offset).Take(limit).Select(x => x.ToDto()).ToListAsync();

            return Results.Ok(new
            {
                count = query.Count(),
                data = res
            });
        });

        group.MapGet("/{code}", async (PrintingDbContext dbContext, string code) =>
        {
            var customer = await dbContext.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Code == code);

            return customer is null
                ? Results.NotFound()
                : Results.Ok(customer.ToDto());
        });

        group.MapPost("", async (PrintingDbContext dbContext, CustomerPostRequest request) =>
        {
            var codegen = new CodeGenerator(dbContext);

            var customer = new Customer
            {
                Code = await codegen.GenerateAsync<Customer>("C-", nameof(Customer.Code), 6),
                BranchCode = AppDefaults.BranchCode,
                Name = request.Name,
                CompanyName = request.CompanyName,
                BrandName = request.BrandName,
                Address = request.Address,
                MailingAddress = request.MailAddress,
                ZipCode = request.ZipCode,
                Npwp = request.Npwp,
                Npwpaddress = request.NpwpAddress,
                CityCode = request.CityCode,
                CountryCode = request.CountryCode,
                Phone1 = request.Phone1,
                Phone2 = request.Phone2,
                CustomerType = request.CustomerType,
                Ktp = request.Ktp,
                Birthdate = request.BirthDate,
                Status = "BARU"
            };

            await dbContext.Customers.AddAsync(customer);
            await dbContext.SaveChangesAsync();
            return Results.Ok(customer.ToDto());
        });

        group.MapPut("{code}", async (PrintingDbContext dbContext, CustomerPutRequest request, string code) =>
        {
            var customer = await dbContext.Customers.FirstOrDefaultAsync(x => x.Code == code);
            if (customer == null) return Results.NotFound();

            customer.Name = request.Name ?? customer.Name;
            customer.BrandName = request.BrandName ?? customer.BrandName;
            customer.CompanyName = request.CompanyName ?? customer.CompanyName;
            customer.Address = request.Address ?? customer.Address;
            customer.MailingAddress = request.MailAddress ?? customer.MailingAddress;
            customer.ZipCode = request.ZipCode ?? customer.ZipCode;
            customer.Npwp = request.Npwp ?? customer.Npwp;
            customer.Npwpaddress = request.NpwpAddress ?? customer.Npwpaddress;
            customer.CityCode = request.CityCode ?? customer.CityCode;
            customer.CountryCode = request.CountryCode ?? customer.CountryCode;
            customer.Phone1 = request.Phone1 ?? customer.Phone1;
            customer.Phone2 = request.Phone2 ?? customer.Phone2;
            customer.CustomerType = request.CustomerType ?? customer.CustomerType;
            customer.Ktp = request.Ktp ?? customer.Ktp;
            customer.Birthdate = request.BirthDate ?? customer.Birthdate;

            await dbContext.SaveChangesAsync();
            return Results.Ok(customer.ToDto());
        });

        return builder;
    }
}