//using GasaiYuno.Discord.Core.Extensions;
//using GasaiYuno.Discord.Infrastructure;
//using MediatR;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Logging;
//using Serilog.Context;
//using System;
//using System.Threading;
//using System.Threading.Tasks;

//namespace GasaiYuno.Discord.Mediator.Behaviors;

//public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
//{
//    private readonly DataContext _dbContext;
//    private readonly ILogger<TransactionBehavior<TRequest, TResponse>> _logger;

//    public TransactionBehavior(DataContext dbContext, ILogger<TransactionBehavior<TRequest, TResponse>> logger)
//    {
//        _dbContext = dbContext;
//        _logger = logger;
//    }

//    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
//    {
//        var response = default(TResponse);
//        var typeName = request.GetGenericTypeName();

//        try
//        {
//            if (_dbContext.HasActiveTransaction)
//                return await next().ConfigureAwait(false);

//            var strategy = _dbContext.Database.CreateExecutionStrategy();
//            await strategy.ExecuteAsync(async () =>
//            {
//                await using var transaction = await _dbContext.BeginTransactionAsync().ConfigureAwait(false);
//                using (LogContext.PushProperty("TransactionContext", transaction.TransactionId))
//                {
//                    _logger.LogInformation("Beginning transaction {TransactionId} for {CommandName} ({@Command})", transaction.TransactionId, typeName, request);
//                    response = await next().ConfigureAwait(false);

//                    _logger.LogInformation("Committing transaction {TransactionId} for {CommandName}", transaction.TransactionId, typeName);
//                    await _dbContext.CommitTransactionAsync(transaction).ConfigureAwait(false);
//                }
//            });

//            return response;
//        }
//        catch (Exception e)
//        {
//            _logger.LogError(e, "Unable to handle transaction for {CommandName} ({@Command})", typeName, request);
//            throw;
//        }
//    }
//}