using SharedKernel.Abstraction;

namespace Integral.Api.Features.Manufacturing.WorkOrderOuts.Exceptions;

public class WorkOrderOutNotFoundException(string requestCode) : AppException($"Work Order Out with code {requestCode} not found");