using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using VoteScale.Domain.Entities;
using VoteScale.Infrastructure.Persistence;
using VoteScale.Infrastructure.Repositories;

namespace VoteScale.Infrastructure.Tests;

public class VoteRepositoryTests : IAsyncLifetime
{
    // Container PostgreSQL descartável
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:15-alpine")
        .Build();

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await _postgres.DisposeAsync();
    }

    [Fact]
    public async Task SaveAsync_ShouldPersistVote_WhenDatabaseIsAvailable()
    {
        // Arrange (Configuração do contexto com a connection string do container)
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(_postgres.GetConnectionString())
            .Options;

        // Cria o banco e aplica migrations
        using var context = new AppDbContext(options);
        await context.Database.EnsureCreatedAsync();

        var repository = new VoteRepository(context);
        var vote = Vote.Create(10, Guid.NewGuid()); 

        // Act (Ação)
        await repository.AddAsync(vote);

        // Assert (Verificação)
        var savedVote = await context.Votes.FirstOrDefaultAsync();
        Assert.NotNull(savedVote);
        Assert.Equal(10, savedVote.CandidateId);
    }
}