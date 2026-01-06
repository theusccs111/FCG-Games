using FCG.Shared.EventLog.Contracts;
using FCG_Games.Domain.Games.Enums;

namespace FCG_Games.Domain.Games.SourcingEvents
{
    public class GameUpdatedEventLog(Guid id, string title, decimal price, int launchYear, string developer, EGenre genre) : EventLogMessage
    {
        public Guid GameId { get; private set; } = id;
        public string Title { get; private set; } = title;
        public decimal Price { get; private set; } = price;
        public int LaunchYear { get; private set; } = launchYear;
        public string Developer { get; private set; } = developer;
        public EGenre Genre { get; private set; } = genre;
    }
}
