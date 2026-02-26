using Integral.Api.Features.Master.Items;
using Integral.Api.Features.Master.Items.Features.Services;

namespace Integral.Api.Features.Master;

public static class Extensions
{
    public static void AddMasterModule(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<ItemQueryService>();
        builder.Services.AddScoped<IItemQueryService>(x => x.GetRequiredService<ItemQueryService>());
    }
}