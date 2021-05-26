using Xunit;
using TNDStudios.Repository;

namespace TNDStudios.Repository.Tests
{
    public class CosmosRepositoryTests : RepositoryTestsBase
    {
        private const string _connectionString = "";
        private const string _collectionName = "testCollection";

        public CosmosRepositoryTests()
        {
            _repository = new CosmosRepository<TestDomainObject, TestDocumentObject>
                (ToDocumentObject, ToDomainObject, 
                _connectionString, _collectionName);
        }

        [Fact]
        public override void Add() => base.Add();

        [Fact]
        public override void Delete() => base.Delete();
        
        [Fact]
        public override void Get() => base.Get();
        
        [Fact]
        public override void Query() => base.Query();

        [Fact]
        public override void DataLoad() => base.DataLoad();
    }
}
