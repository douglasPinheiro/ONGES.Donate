namespace ONGES.Donate.Domain.Exceptions;

public sealed class DomainValidationException(string message) : Exception(message);
