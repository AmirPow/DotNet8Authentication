using DotNet8Authentication.Data;
using MediatR;
namespace DotNet8Authentication.Behaviors;

public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly ApplicationDbContext context;

    public TransactionBehavior(ApplicationDbContext context)
    {
        this.context = context;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var response = await next();

        await context.SaveChangesAsync(cancellationToken);

        return response;
    }
}
