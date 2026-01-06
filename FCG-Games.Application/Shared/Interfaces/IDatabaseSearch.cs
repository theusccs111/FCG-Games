using FCG_Games.Application.Shared.Results;
using FCG_Games.Domain.Shared;

namespace FCG_Games.Application.Shared.Interfaces
{
    public interface IDatabaseSearch<T> where T : IDocument
    {
        void CreateIndexIfNotExist();
        Task CreateDocumentAsync(T document);
        Task UpdateDocumentAsync(T document);
        Task<T> GetDocumentAsync(Guid documentId);
        Task<IEnumerable<T>> GetDocumentsAsync(Pagination pagination);
        Task<IEnumerable<T>> GetTopSellersDocumentsAsync(Pagination pagination);
        Task<IEnumerable<T>> GetCustomizedDocumentsAsync(string query, string genders, Pagination pagination);
        Task DeleteDocumentAsync(Guid documentId);
    }
}
