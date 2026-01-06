using FCG.Shared.EventLog.Contracts;

namespace FCG_Games.Domain.Games.SourcingEvents
{
    public class GameDeletedEventLog(Guid id) : EventLogMessage
    {
        public Guid GameId { get; private set; } = id;
    }
}
