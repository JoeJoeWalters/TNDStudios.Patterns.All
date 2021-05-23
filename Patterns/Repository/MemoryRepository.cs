using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace TNDStudios.Patterns.Repository.Module
{
    public class MemoryRepository<TDomain, TDocument> : IRepository<TDomain, TDocument>
        where TDocument : RepositoryDocument
        where TDomain : RepositoryDomainObject
    {
        private readonly Dictionary<String, TDocument> _values;

        private readonly Func<TDomain, TDocument> _toDocument;
        private readonly Func<TDocument, TDomain> _toDomain;

        public MemoryRepository(
            Func<TDomain, TDocument> toDocument,
            Func<TDocument, TDomain> toDomain)
        {
            _toDocument = toDocument;
            _toDomain = toDomain;
            _values = new Dictionary<String, TDocument>();
        }

        public bool Delete(String id) 
            => _values.Remove(id);

        public TDomain Get(String id)
        {
            if (_values.ContainsKey(id))
            {
                return ToDomain(_values[id]);
            }

            return null;
        }

        public IEnumerable<TDomain> Query(Expression<Func<TDocument, Boolean>> query)
        {
            var filtered = _values.Select(x => x.Value).AsQueryable<TDocument>().Where(query);
            return filtered.Select(x => ToDomain(x)).ToList();
        }

        public TDomain ToDomain(TDocument document) 
            => _toDomain(document);

        public TDocument ToDocument(TDomain domain) 
            => _toDocument(domain);

        public bool Upsert(TDomain item)
        {
            TDocument document = ToDocument(item);
            if (document != null)
            {
                item.Id = document.Id = document.Id ?? Guid.NewGuid().ToString();
                _values[document.Id] = document;
                return true;
            }

            return false;
        }

        public bool WithData(List<TDomain> data)
        {
            Int32 insertCount = 0;

            data.ForEach(item =>
                insertCount += (Upsert(item) ? 1 : 0));

            return insertCount == data.Count;
        }
    }
}
