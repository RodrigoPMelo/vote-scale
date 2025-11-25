using VoteScale.Infrastructure;
using VoteScale.Worker;

var builder = Host.CreateApplicationBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

Console.Title = "VoteScale Worker Service";

builder.Services.AddInfrastructure(builder.Configuration, typeof(VoteConsumer).Assembly);

var host = builder.Build();

var logger = host.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("VoteScale Worker Iniciado! Aguardando mensagens na fila...");

host.Run();