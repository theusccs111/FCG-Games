using FCG_Games.Domain.Games.Enums;

namespace FCG_Games.Application.Games.Requests
{
    public sealed record GameRequest(string Title, decimal Price, int LaunchYear, string Developer, EGenre Genre);
}
