using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace TNDStudios.Repository
{
    public class CosmosRepository<TDomain, TDocument> : RepositoryBase<TDomain, TDocument>
        where TDocument : RepositoryDocument
        where TDomain : RepositoryDomainObject
    {
        private readonly string _connectionString = String.Empty;
        private readonly string _collectionName = String.Empty;

        private readonly CosmosClient _client;
        private readonly Container _container;

        public CosmosRepository(
            Func<TDomain, TDocument> toDocument,
            Func<TDocument, TDomain> toDomain,
            string connectionString,
            string databaseName,
            string collectionName) : base(toDocument, toDomain)
        {
            _connectionString = connectionString;
            _collectionName = collectionName;

            CosmosClientOptions clientOptions = 
                new CosmosClientOptions() 
                { 
                    ConnectionMode = ConnectionMode.Direct,
                    AllowBulkExecution = true
                };

            _client = new CosmosClient(_connectionString, clientOptions);
            _container = _client.GetContainer(databaseName, _collectionName);
        }

        public override async Task<bool> Delete(String id)
        {
            await _container.DeleteItemAsync<TDocument>(id, new PartitionKey(id));
            return true;
        }

        public override async Task<TDomain> Get(String id)
        {
            throw new NotImplementedException();
        }

        public override async Task<IEnumerable<TDomain>> Query(Expression<Func<TDocument, Boolean>> query)
        {
            throw new NotImplementedException();
        }

        public override async Task<bool> Upsert(TDomain item)
        {
            await _container.UpsertItemAsync<TDocument>(ToDocument(item), new PartitionKey(item.Id));
            return true;
        }

        public override async Task<bool> WithData(List<TDomain> data)
        {
            throw new NotImplementedException();
        }
    }
}
