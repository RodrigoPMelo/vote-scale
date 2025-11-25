using Microsoft.EntityFrameworkCore;
using VoteScale.Domain.Entities;

namespace VoteScale.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Vote> Votes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configuração explícita da tabela Vote (Best Practice)
        modelBuilder.Entity<Vote>(builder =>
        {
            builder.HasKey(v => v.Id);
            builder.Property(v => v.CandidateId).IsRequired();
            builder.Property(v => v.SurveyId).IsRequired();
            builder.Property(v => v.CreatedAt).IsRequired();
        });

        base.OnModelCreating(modelBuilder);
    }
}