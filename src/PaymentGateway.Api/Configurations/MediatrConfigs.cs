using Ardalis.SharedKernel;
using MediatR;
using System.Reflection;

using PaymentGateway.Core.Domains;
using PaymentGateway.UseCases.Payments.Create;

namespace PaymentGateway.Api.Configurations;

public static class MediatrConfigs
{
  public static IServiceCollection AddMediatrConfigs(this IServiceCollection services)
  {
    var mediatRAssemblies = new[]
      {
        Assembly.GetAssembly(typeof(Payment)), // Core
        Assembly.GetAssembly(typeof(CreatePaymentCommand)) // UseCases
      };

    services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(mediatRAssemblies!))
            .AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>))
            .AddScoped<IDomainEventDispatcher, MediatRDomainEventDispatcher>();

    return services;
  }
}
