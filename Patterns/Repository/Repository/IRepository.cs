using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TNDStudios.Repository
{
    [JsonObject]
    public class RepositoryDocument
    {
        [JsonProperty("id")]
        public String Id { get; set; }

        [JsonProperty("partitionKey")]
        public String PartitionKey { get; set; }
    }

    public class RepositoryDomainObject
    {
        public String Id { get; set; }
    }

    public interface IRepository<TDomain, TDocument> 
        where TDocument: RepositoryDocument 
        where TDomain: RepositoryDomainObject
    {
        TDomain ToDomain(TDocument document);
        TDocument ToDocument(TDomain domain);

        Task<TDomain> Get(String id);
        Task<IEnumerable<TDomain>> Query(Expression<Func<TDocument, Boolean>> query);
        Task<Boolean> Delete(String id);
        Task<Boolean> Upsert(TDomain item);

        Task<Boolean> WithData(List<TDomain> data);
    }
}
