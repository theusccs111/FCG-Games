using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Elastic.Transport.Products.Elasticsearch;
using FCG_Games.Application.Shared.Interfaces;
using FCG_Games.Application.Shared.Results;
using FCG_Games.Domain.Games.SearchDocuments;

namespace FCG_Games.Infrastructure.Games.DatabaseSearch
{
    public class GameDatabaseSearch(ElasticsearchClient client) : IDatabaseSearch<GameDocument>
    {
        private readonly string IndexName = "games";

        public void CreateIndexIfNotExist()
        {
            var existsResponse = client.Indices.Exists(IndexName);
            if(existsResponse.Exists) { return; }

            var createIndexResponse = client.Indices.Create(IndexName);
            ThrowElasticSearchExceptionIfNecessary(createIndexResponse);
        }

        public async Task CreateDocumentAsync(GameDocument document)
        {
            var response = await client.IndexAsync(document, x => x.Index(IndexName));
            ThrowElasticSearchExceptionIfNecessary(response);
        }

        public async Task<GameDocument> GetDocumentAsync(Guid documentId)
        {
            var response = await client.GetAsync<GameDocument>(IndexName, documentId);
            ThrowElasticSearchExceptionIfNecessary(response);

            return response.Source!;
        }

        public async Task<IEnumerable<GameDocument>> GetDocumentsAsync(Pagination pagination)
        {
            var response = await client.SearchAsync<GameDocument>(s => s
                .Indices(IndexName)
                .Query(q => q.MatchAll())
                .From(pagination.Page * pagination.PageSize)
                .Size(pagination.PageSize)
            );

            ThrowElasticSearchExceptionIfNecessary(response);

            return response.Documents;
        }

        private static void ThrowElasticSearchExceptionIfNecessary(ElasticsearchResponse response)
        {
            if (!response.IsValidResponse)
                throw new Exception(response.ElasticsearchServerError?.ToString());
        }

        public async Task UpdateDocumentAsync(GameDocument document)
        {
            var response = await client.UpdateAsync<GameDocument, GameDocument>(IndexName, document.Id, u => u.Doc(document));
            ThrowElasticSearchExceptionIfNecessary(response);
        }

        public async Task DeleteDocumentAsync(Guid documentId)
        {
            var response = await client.DeleteAsync(IndexName, documentId);
            ThrowElasticSearchExceptionIfNecessary(response);
        }

        public async Task<IEnumerable<GameDocument>> GetTopSellersDocumentsAsync(Pagination pagination)
        {
            var response = await client.SearchAsync<GameDocument>(s => s
                .Indices(IndexName)
                .From(pagination.Page * pagination.PageSize)
                .Size(pagination.PageSize)
                .Query(q => q.MatchAll())
                .Sort(sort => sort
                    .Field(f => f.Field("salesCount").Order(SortOrder.Desc))
                )
            );

            ThrowElasticSearchExceptionIfNecessary(response);

            return response.Documents;
        }

        public async Task<IEnumerable<GameDocument>> GetCustomizedDocumentsAsync(string query, string genders, Pagination pagination)
        {
            var fields = new Field[] { new("title"), new("genre"), new("developer") };
            var response = await client.SearchAsync<GameDocument>(s => s
                .Indices(IndexName)
                .From(pagination.Page * pagination.PageSize)
                .Size(pagination.PageSize)
                .Query(q => q
                    .FunctionScore(fs => fs
                        .Query(qq => qq
                            .MultiMatch(mm => mm
                                .Query(string.Concat(query, " ", genders))
                                .Fields(Fields.FromFields(fields))
                            )
                        )
                        .Functions(f => f
                            .FieldValueFactor(ff => ff
                                .Field(p => p.SalesCount)
                                .Factor(1)
                                .Modifier(FieldValueFactorModifier.Log1p)
                            ).Weight(2)
                        )
                        .BoostMode(FunctionBoostMode.Sum)
                        .ScoreMode(FunctionScoreMode.Sum)
                    )
                )
            );

            ThrowElasticSearchExceptionIfNecessary(response);

            return response.Documents;
        }
    }
}
