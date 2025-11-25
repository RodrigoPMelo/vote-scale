using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MassTransit;
using VoteScale.Domain.Interfaces;
using VoteScale.Infrastructure.Persistence;
using VoteScale.Infrastructure.Repositories;
using VoteScale.Infrastructure.Messaging;
using System.Reflection;

namespace VoteScale.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, Assembly? consumerAssembly = null)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IVoteRepository, VoteRepository>();

        services.AddMassTransit(busConfig =>
        {
            if (consumerAssembly != null)
            {
                busConfig.AddConsumers(consumerAssembly);
            }

            busConfig.UsingRabbitMq((context, cfg) =>
            {
                var rmqHost = configuration["RabbitMQ:HostName"] ?? "localhost";
                var rmqUser = configuration["RabbitMQ:UserName"] ?? "guest";
                var rmqPass = configuration["RabbitMQ:Password"] ?? "guest";

                cfg.Host(rmqHost, "/", h =>
                {
                    h.Username(rmqUser);
                    h.Password(rmqPass);
                });

                cfg.ConfigureEndpoints(context);
            });
        });

        services.AddScoped<IMessageBus, RabbitMQBus>();

        return services;
    }
}