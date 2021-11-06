using FluentAssertions;
using Repository.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Repository.Tests
{
    public class TestDomainObject : RepositoryDomainObject { }
    public class TestDocumentObject : RepositoryDocument { }

    public class RepositoryTestsBase
    {
        internal String _partitionFixedKey = "PartitionKeyFixedValue";

        internal List<TestDomainObject> _testData = new List<TestDomainObject>() { };

        internal IRepository<TestDomainObject, TestDocumentObject> _repository;

        internal TestDomainObject ToDomainObject(TestDocumentObject from)
            => new TestDomainObject()
            {
                Id = from.Id
            };

        internal TestDocumentObject ToDocumentObject(TestDomainObject from)
        {
            string id = from.Id = from.Id ?? Guid.NewGuid().ToString();
            return new TestDocumentObject()
            {
                Id = id,
                PartitionKey = _partitionFixedKey
            };
        }

        public RepositoryTestsBase()
        {
            for (int i = 0; i < 3; i++)
            {
                String id = Guid.NewGuid().ToString();
                _testData.Add(
                    new TestDomainObject() { Id = id }
                    );
            };
        }

        public virtual void Add()
        {
            // ARRANGE
            TestDomainObject domain = new TestDomainObject() { };
            Boolean result = false;

            // ACT
            result = _repository.Upsert(domain).Result;

            // ASSERT
            result.Should().BeTrue();
            domain.Id.Should().NotBeNullOrEmpty();
        }

        public virtual void Delete()
        {
            // ARRANGE
            TestDomainObject domain = new TestDomainObject() { };
            TestDomainObject resultObject = null;
            Boolean upsertResult = false;
            String upsertId = String.Empty;
            Boolean deleteResult = false;

            // ACT
            upsertResult = _repository.Upsert(domain).Result;
            if (upsertResult)
            {
                upsertId = domain.Id;
                deleteResult = _repository.Delete(upsertId, _partitionFixedKey).Result;
                if (deleteResult)
                {
                    resultObject = _repository.Get(upsertId, _partitionFixedKey).Result;
                }
            }

            // ASSERT
            upsertResult.Should().BeTrue();
            upsertId.Should().NotBeEmpty();
            resultObject.Should().BeNull();
        }

        public virtual void Get()
        {
            // ARRANGE
            TestDomainObject domain = new TestDomainObject() { };
            TestDomainObject resultObject = null;

            // ACT
            Boolean upsertResult = _repository.Upsert(domain).Result;
            if (upsertResult)
            {
                resultObject = _repository.Get(domain.Id, _partitionFixedKey).Result;
            }

            // ASSERT
            upsertResult.Should().BeTrue();
            resultObject.Should().NotBeNull();
            domain.Id.Should().NotBeNull();
            resultObject.Id.Should().Be(domain.Id);
        }

        Expression<Func<TestDocumentObject, Boolean>> QueryById(String id)
            => q => q.Id == id && q.PartitionKey == _partitionFixedKey;

        public virtual void Query()
        {
            // ARRANGE
            Expression<Func<TestDocumentObject, Boolean>> query;
            TestDomainObject domain = new TestDomainObject() { };
            IEnumerable<TestDomainObject> results = null;

            // ACT
            Boolean upsertResult = _repository.Upsert(domain).Result;
            if (upsertResult)
            {
                query = QueryById(domain.Id);
                results = _repository.Query(query, _partitionFixedKey).Result;
            }

            // ASSERT
            upsertResult.Should().BeTrue();
            results.Count().Should().NotBe(0);
            results.ToList()[0].Id.Should().Be(domain.Id);
        }

        Expression<Func<TestDocumentObject, Boolean>> QueryAll(List<string> ids)
            => q => (q.PartitionKey == _partitionFixedKey && ids.Contains(q.Id) );

        public virtual void DataLoad()
        {
            // ARRANGE
            Boolean success = false;
            Expression<Func<TestDocumentObject, Boolean>> query;
            IEnumerable<TestDomainObject> results = null;

            // ACT
            success = _repository.WithData(_testData).Result;
            query = QueryAll(_testData.Select(x => x.Id).ToList());
            results = _repository.Query(query, _partitionFixedKey).Result;

            // ASSERT
            success.Should().BeTrue();
            results.Count().Should().Be(_testData.Count);
        }
    }
}
