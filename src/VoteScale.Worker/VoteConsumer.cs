using MassTransit;
using VoteScale.Domain.Entities;
using VoteScale.Domain.Interfaces;

namespace VoteScale.Worker;

public class VoteConsumer : IConsumer<Vote>
{
    private readonly ILogger<VoteConsumer> _logger;
    private readonly IServiceProvider _serviceProvider;

    public VoteConsumer(ILogger<VoteConsumer> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public async Task Consume(ConsumeContext<Vote> context)
    {
        var vote = context.Message;
        _logger.LogInformation("Processando voto ID: {VoteId} para o Candidato: {CandidateId}", vote.Id, vote.CandidateId);

        using (var scope = _serviceProvider.CreateScope())
        {
            var repository = scope.ServiceProvider.GetRequiredService<IVoteRepository>();

            await repository.AddAsync(vote);

            _logger.LogInformation("Voto ID: {VoteId} persistido no banco com sucesso.", vote.Id);
        }
    }
}