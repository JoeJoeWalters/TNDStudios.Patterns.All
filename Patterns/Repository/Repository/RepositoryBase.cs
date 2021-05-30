using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TNDStudios.Repository
{
    public class RepositoryBase<TDomain, TDocument> : IRepository<TDomain, TDocument>
        where TDocument : RepositoryDocument
        where TDomain : RepositoryDomainObject
    {
        internal readonly Func<TDomain, TDocument> _toDocument;
        internal readonly Func<TDocument, TDomain> _toDomain;

        public RepositoryBase(
            Func<TDomain, TDocument> toDocument,
            Func<TDocument, TDomain> toDomain)
        {
            _toDocument = toDocument;
            _toDomain = toDomain;
        }

        public virtual async Task<bool> Delete(string id) => throw new NotImplementedException();

        public virtual async Task<TDomain> Get(string id) => throw new NotImplementedException();

        public virtual async Task<IEnumerable<TDomain>> Query(System.Linq.Expressions.Expression<Func<TDocument, bool>> query) => throw new NotImplementedException();

        public TDomain ToDomain(TDocument document)
                    => _toDomain(document);

        public TDocument ToDocument(TDomain domain)
            => _toDocument(domain);

        public virtual async Task<bool> Upsert(TDomain item) => throw new NotImplementedException();

        public virtual async Task<bool> WithData(List<TDomain> data) => throw new NotImplementedException();
    }
}
