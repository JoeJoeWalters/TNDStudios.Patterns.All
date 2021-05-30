using Xunit;
using TNDStudios.Repository;

namespace TNDStudios.Repository.Tests
{
    public class CosmosRepositoryTests : RepositoryTestsBase
    {
        private const string _connectionString = "AccountEndpoint=https://localhost:8081/;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
        private const string _databaseName = "testDatabase";
        private const string _collectionName = "testCollection";

        public CosmosRepositoryTests()
        {
            _repository = new CosmosRepository<TestDomainObject, TestDocumentObject>
                (ToDocumentObject, ToDomainObject, 
                _connectionString, _databaseName, _collectionName);
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