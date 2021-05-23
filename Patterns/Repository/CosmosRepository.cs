using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace TNDStudios.Patterns.Repository.Module
{
    public class CosmosRepository<TDomain, TDocument> : IRepository<TDomain, TDocument>
        where TDocument : RepositoryDocument
        where TDomain : RepositoryDomainObject
    {
        private readonly Dictionary<String, TDocument> _values;

        private readonly Func<TDomain, TDocument> _toDocument;
        private readonly Func<TDocument, TDomain> _toDomain;

        public CosmosRepository(
            Func<TDomain, TDocument> toDocument,
            Func<TDocument, TDomain> toDomain)
        {
            _toDocument = toDocument;
            _toDomain = toDomain;
            _values = new Dictionary<String, TDocument>();
        }

        public bool Delete(String id)
        {
            throw new NotImplementedException();
        }

        public TDomain Get(String id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TDomain> Query(Expression<Func<TDocument, Boolean>> query)
        {
            throw new NotImplementedException();
        }

        public TDomain ToDomain(TDocument document)
        {
            throw new NotImplementedException();
        }

        public TDocument ToDocument(TDomain domain)
        {
            throw new NotImplementedException();
        }

        public bool Upsert(TDomain item)
        {
            throw new NotImplementedException();
        }

        public bool WithData(List<TDomain> data)
        {
            throw new NotImplementedException();
        }
    }
}
