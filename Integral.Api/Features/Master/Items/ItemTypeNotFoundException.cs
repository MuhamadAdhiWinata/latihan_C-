using SharedKernel.Abstraction;

namespace Integral.Api.Features.Master.Items;

public class ItemTypeNotFoundException(string code) : AppException($"Item Type with code {code} not found");