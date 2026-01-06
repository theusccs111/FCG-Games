using FCG.Shared.Transactional;
using FCG_Games.Domain.Games.Enums;
using FCG_Games.Domain.Games.Exceptions;
using FCG_Games.Domain.Games.Exceptions.Game;

namespace FCG_Games.Domain.Games.Entities
{
    public class Game : Entity
    {
        #region Constructors
        private Game() : base(Guid.NewGuid())
        {

        }

        private Game(Guid id, string title, decimal price, int launchYear, string developer, EGenre genre) : base(id)
        {
            Title = title;
            Price = price;
            LaunchYear = launchYear;
            Genre = genre;
            Developer = developer;
        }
        #endregion

        #region Properties
        public string Title { get; private set; } = string.Empty;
        public decimal Price { get; private set; }
        public int LaunchYear { get; private set; }
        public string Developer { get; private set; } = string.Empty;
        public EGenre Genre { get; private set; }

        #endregion

        #region Factories

        public static Game Create(Guid id, string title, decimal price, int launchYear, string developer, EGenre genre)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new TitleNullOrEmptyException(ErrorMessage.Game.TitleNullOrEmpty);
            if (price < 0)
                throw new NegativePriceException(ErrorMessage.Game.NegativePrice);
            if (launchYear > DateTime.UtcNow.Year)
                throw new LaunchYearInFutureException(ErrorMessage.Game.LaunchYearInFuture);
            if (string.IsNullOrWhiteSpace(developer))
                throw new DeveloperRequiredException(ErrorMessage.Game.DeveloperRequired);
            if(!Enum.IsDefined(typeof(EGenre), genre))
                throw new ArgumentException(ErrorMessage.Game.GenderRequired);

            return new Game(id, title, price, launchYear, developer, genre);
        }

        #endregion

        #region Overrides

        public override bool Equals(object? obj)
        {
            if (obj is not Game game)
                return false;
            return Id == game.Id && Title == game.Title && Price == game.Price &&
                   Developer == game.Developer &&
                   Genre == game.Genre;
        }

        public override int GetHashCode() => HashCode.Combine(Id, Title, Price, Developer, Genre);

        #endregion

        #region Methods
       
        public void Update(string titulo, decimal preco, int anoLancamento, string desenvolvedora, EGenre genre)
        {
            Title = titulo;
            Price = preco;
            LaunchYear = anoLancamento;
            Genre = genre;
            Developer = desenvolvedora;

            UpdateLastDateChanged();
        }      
        #endregion
    }
}
