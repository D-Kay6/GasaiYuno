using FluentValidation;
using GasaiYuno.Discord.Core.Extensions;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GasaiYuno.Discord.Mediator.Behaviors;

public class ValidatorBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly ILogger<ValidatorBehavior<TRequest, TResponse>> _logger;
    private readonly IValidator<TRequest>[] _validators;

    public ValidatorBehavior(IValidator<TRequest>[] validators, ILogger<ValidatorBehavior<TRequest, TResponse>> logger)
    {
        _validators = validators;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        var typeName = request.GetGenericTypeName();
        _logger.LogInformation("----- Validating command {CommandType}", typeName);

        var failures = _validators
            .Select(x => x.Validate(request))
            .SelectMany(x => x.Errors)
            .Where(x => x != null)
            .ToList();
        if (failures.Any())
        {
            _logger.LogWarning("Validation errors - {CommandType} - Command: {@Command} - Errors: {@ValidationErrors}", typeName, request, failures);

            throw new ValidationException($"Command Validation Errors for type {typeof(TRequest).Name}", new FluentValidation.ValidationException("Validation exception", failures));
        }

        return await next();
    }
}