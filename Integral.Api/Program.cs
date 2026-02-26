using Integral.Api;
using Integral.App.Abstraction.Authorization;
using MediatR;
using SharedKernel.Abstraction.Ef;
using SharedKernel.Abstraction.Validation;
using SharedKernel.Abstraction.Web;
using Integral.Api.Features.Identity.Services;
using Integral.Api.Features.Manufacturing.WorkOrderInputs.Queries;
using Integral.Api.Features.Master;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddMasterModule();
builder.AddAppDbContexts();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<CurrentUserProvider>();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(Program).Assembly));
builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(EfTxBehavior<,>));


builder.Services.AddSingleton<PermissionDiscoveryService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddMemoryCache();

builder.Services.AddUnitOfWork();
builder.Services.AddEventDispatcher();

builder.Services.AddOpenApi();

builder.Services.AddScoped<WorkOrderInputQueryService>();

builder.AddMinimalEndpoints();
builder.AddAppSwagger();
builder.AddJwtAuth();

builder.Services.AddCors(options =>
{
    options.AddPolicy("All",
        policy =>
        {
            policy.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

var app = builder.Build();

// if (app.Environment.IsDevelopment())
// {
//     app.UseAppSwagger();
// }


app.UseAppSwagger();
app.MapScalarApiReference();

app.UseAppExceptionHandler();

app.UseCors("All");

app.UseAuthentication();
app.UseAuthorization();

// app.UseHttpsRedirection();
app.MapMinimalEndpoints();

app.WarmUp();

await app.RunAsync();