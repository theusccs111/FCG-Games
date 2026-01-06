using FCG_Games.Domain.Games.Enums;

namespace FCG_Games.Application.Games.Responses
{
    public sealed record GameResponse(Guid Id, string Title, decimal Price, int LaunchYear, string Developer, EGenre Genre);   
}
