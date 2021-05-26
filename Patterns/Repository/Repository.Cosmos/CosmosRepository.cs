using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace TNDStudios.Repository
{
    public class CosmosRepository<TDomain, TDocument> : RepositoryBase<TDomain, TDocument>
        where TDocument : RepositoryDocument
        where TDomain : RepositoryDomainObject
    {
        private readonly string _connectionString = String.Empty;
        private readonly string _collectionName = String.Empty;

        public CosmosRepository(
            Func<TDomain, TDocument> toDocument,
            Func<TDocument, TDomain> toDomain,
            string connectionString,
            string collectionName) : base(toDocument, toDomain)
        {
            _connectionString = connectionString;
            _collectionName = collectionName;
        }

        public override bool Delete(String id)
        {
            throw new NotImplementedException();
        }

        public override TDomain Get(String id)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<TDomain> Query(Expression<Func<TDocument, Boolean>> query)
        {
            throw new NotImplementedException();
        }

        public override TDomain ToDomain(TDocument document)
        {
            throw new NotImplementedException();
        }

        public override TDocument ToDocument(TDomain domain)
        {
            throw new NotImplementedException();
        }

        public override bool Upsert(TDomain item)
        {
            throw new NotImplementedException();
        }

        public override bool WithData(List<TDomain> data)
        {
            throw new NotImplementedException();
        }
    }
}
