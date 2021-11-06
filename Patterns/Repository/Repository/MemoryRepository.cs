using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Repository.Common
{
    public class MemoryRepository<TDomain, TDocument> : RepositoryBase<TDomain, TDocument>
        where TDocument : RepositoryDocument
        where TDomain : RepositoryDomainObject
    {
        private readonly Dictionary<String, TDocument> _values;

        public MemoryRepository(
            Func<TDomain, TDocument> toDocument,
            Func<TDocument, TDomain> toDomain) : base(toDocument, toDomain)
        {
            _values = new Dictionary<String, TDocument>();
        }

        public override async Task<bool> Delete(String id, String partitionKey) 
            => _values.Remove(id);

        public override async Task<TDomain> Get(String id, String partitionKey)
        {
            if (_values.ContainsKey(id))
            {
                return ToDomain(_values[id]);
            }

            return null;
        }

        public override async Task<IEnumerable<TDomain>> Query(Expression<Func<TDocument, Boolean>> query, string partitionKey)
        {
            var filtered = _values.Select(x => x.Value).AsQueryable<TDocument>().Where(query);
            return filtered.Select(x => ToDomain(x)).ToList();
        }

        public override async Task<bool> Upsert(TDomain item)
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

        public override async Task<bool> WithData(List<TDomain> data)
        {
            Int32 insertCount = 0;

            data.ForEach(item =>
            {
                var value = Upsert(item).Result;
                insertCount += (value ? 1 : 0);
            });

            return insertCount == data.Count;
        }
    }
}
