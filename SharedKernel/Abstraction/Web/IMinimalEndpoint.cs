namespace SharedKernel.Abstraction.Web;

public interface IMinimalEndpoint
{
    IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder);
}