using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
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

        private readonly PartitionKey _partitionKey;

        private readonly CosmosClient _client;
        private readonly Container _container;

        public CosmosRepository(
            Func<TDomain, TDocument> toDocument,
            Func<TDocument, TDomain> toDomain,
            string connectionString,
            string databaseName,
            string collectionName,
            string partitionKey) : base(toDocument, toDomain)
        {
            _partitionKey = new PartitionKey(partitionKey);

            _connectionString = connectionString;
            _collectionName = collectionName;

            CosmosClientOptions clientOptions =
                new CosmosClientOptions()
                {
                    ConnectionMode = ConnectionMode.Gateway,
                    AllowBulkExecution = false
                };

            _client = new CosmosClient(_connectionString, clientOptions);
            DatabaseResponse databaseResponse = _client.CreateDatabaseIfNotExistsAsync(databaseName).Result;
            ContainerResponse containerResponse = databaseResponse.Database.CreateContainerIfNotExistsAsync(new ContainerProperties(_collectionName, partitionKey)).Result;
            _container = containerResponse.Container;
        }

        public override async Task<bool> Delete(String id)
        {
            await _container.DeleteItemAsync<TDocument>(id, new PartitionKey(id));
            return true;
        }

        public override async Task<TDomain> Get(String id)
        {
            TDocument document = null;
            TDomain domain = null;

            try
            {
                document = await _container.ReadItemAsync<TDocument>(id, new PartitionKey(id));
            }
            catch(CosmosException cosEx) when (cosEx.StatusCode == HttpStatusCode.NotFound)
            {

            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (document != null)
                    domain = ToDomain(document);
            }

            return domain;
        }

        public override async Task<IEnumerable<TDomain>> Query(Expression<Func<TDocument, Boolean>> query)
        {
            List<TDocument> documents = _container
                    .GetItemLinqQueryable<TDocument>(allowSynchronousQueryExecution: true)
                    .Where(query).ToList();

            return documents.Select(document => ToDomain(document)).ToList<TDomain>();
        }

        public override async Task<bool> Upsert(TDomain item)
        {
            TDocument document = ToDocument(item);
            await _container.UpsertItemAsync<TDocument>(document, new PartitionKey(document.PartitionKey));
            return true;
        }

        public override async Task<bool> WithData(List<TDomain> data)
        {
            bool result = true;

            foreach(TDomain domain in data)
            {
                bool upsertResult = await Upsert(domain);
                result = !upsertResult ? false : result;
            }

            return result;
        }
    }
}
