namespace SharedKernel.Abstraction.Domain;

public class DomainRuleException(string message = "") : Exception(message)
{
}