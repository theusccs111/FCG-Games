using FCG_Games.Domain.Shared;

namespace FCG_Games.Domain.Games.SearchDocuments
{
    public class GameDocument : IDocument
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = "";
        public string Developer { get; set; } = "";
        public string Genre { get; set; } = "";
        public int LaunchYear { get; set; }
        public decimal Price { get; set; }
        public int SalesCount { get; set; }

        public GameDocument(Guid id, string title, string developer, string genre, int launchYear, decimal price)
        {
            Id = id;
            Title = title;
            Developer = developer;
            Genre = genre;
            LaunchYear = launchYear;
            Price = price;
        }

        public GameDocument() {}

        public void Update(string title, string developer, string genre, int launchYear, decimal price)
        {
            Title = title;
            Developer = developer;
            Genre = genre;
            LaunchYear = launchYear;
            Price = price;
        }

        public void UpdateSalesCount()
        {
            SalesCount++;
        }
    }
}
