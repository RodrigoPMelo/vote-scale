using VoteScale.Domain.Entities;

namespace VoteScale.Domain.Interfaces;

public interface IVoteRepository
{
    Task AddAsync(Vote vote);
    Task<Dictionary<int, int>> GetResultsAsync(Guid surveyId);

}