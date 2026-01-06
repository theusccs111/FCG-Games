using FCG.Shared.EventService.Enums;

namespace FCG_Games.Application.Shared.ExternalServices.Library
{
    public sealed record LibraryResponse(Guid ItemId, Guid UserId, Guid GameId, EOrderStatus Status, decimal? PricePaid, EPaymentType PaymentType);

}
