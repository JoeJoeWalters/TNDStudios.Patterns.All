using System;
using System.Collections.Generic;
using System.Text;

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

        public virtual bool Delete(string id) => throw new NotImplementedException();

        public virtual TDomain Get(string id) => throw new NotImplementedException();

        public virtual IEnumerable<TDomain> Query(System.Linq.Expressions.Expression<Func<TDocument, bool>> query) => throw new NotImplementedException();

        public virtual TDocument ToDocument(TDomain domain) => throw new NotImplementedException();

        public virtual TDomain ToDomain(TDocument document) => throw new NotImplementedException();

        public virtual bool Upsert(TDomain item) => throw new NotImplementedException();

        public virtual bool WithData(List<TDomain> data) => throw new NotImplementedException();
    }
}
