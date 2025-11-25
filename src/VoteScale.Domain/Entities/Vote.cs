namespace VoteScale.Domain.Entities;

public record Vote(
    Guid Id,
    int CandidateId,
    Guid SurveyId,
    DateTime CreatedAt
)
{
    public static Vote Create(int candidateId, Guid surveyId)
    {
        return new Vote(Guid.NewGuid(), candidateId, surveyId, DateTime.UtcNow);
    }
}