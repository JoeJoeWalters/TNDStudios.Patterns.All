using Xunit;
using TNDStudios.Patterns.Repository.Module;

namespace TNDStudios.Patterns.Repository.Tests
{
    public class CosmosRepositoryTests : RepositoryTestsBase
    {
        public CosmosRepositoryTests()
        {
            _repository = new CosmosRepository<TestDomainObject, TestDocumentObject>(ToDocumentObject, ToDomainObject);
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
