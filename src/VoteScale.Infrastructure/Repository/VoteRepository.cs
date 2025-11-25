using Microsoft.EntityFrameworkCore; // Necessário para ToDictionaryAsync
using VoteScale.Domain.Entities;
using VoteScale.Domain.Interfaces;
using VoteScale.Infrastructure.Persistence;

namespace VoteScale.Infrastructure.Repositories;

public class VoteRepository : IVoteRepository
{
    private readonly AppDbContext _context;

    public VoteRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Vote vote)
    {
        await _context.Votes.AddAsync(vote);
        await _context.SaveChangesAsync();
    }

    public async Task<Dictionary<int, int>> GetResultsAsync(Guid surveyId)
    {
        var query = _context.Votes.AsQueryable();

        if (surveyId != Guid.Empty)
        {
            query = query.Where(v => v.SurveyId == surveyId);
        }

        return await query
            .GroupBy(v => v.CandidateId)
            .Select(g => new { Candidato = g.Key, Votos = g.Count() })
            .ToDictionaryAsync(k => k.Candidato, v => v.Votos);
    }
}