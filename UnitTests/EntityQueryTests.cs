using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntyTea.EntityQueries.UnitTests
{
    public class EntityQueryTests
    {
        [Test]
        public void Apply_FilterSorterSkipTake()
        {
            var query = new EntityQuery<Person>(
                filter: EntityFilter<Person>.Where(p => p.Name.StartsWith("T")), 
                sorter: EntitySorter<Person>.OrderBy(p => p.Id), 
                skip: 1,
                take: 2
            );

            var people = new[]
            {
                new Person { Id = 3, Name = "T" },
                new Person { Id = 1, Name = "Bob" },
                new Person { Id = 4, Name = "Test" },
                new Person { Id = 2, Name = "Ted" },
                new Person { Id = 7, Name = "TName" },
                new Person { Id = 5, Name = "Target" },
                new Person { Id = 6, Name = "Name" },
                
            }.AsQueryable();
            var filteredPeople = query.Apply(people);

            Assert.AreEqual(2, filteredPeople.Count());
            Assert.AreEqual(3, filteredPeople.ElementAt(0).Id);
            Assert.AreEqual(4, filteredPeople.ElementAt(1).Id);
        }

        [Test]
        public void Aggregate_Apply()
        {
            var query1 = new EntityQuery<Person>(p => p.Name.EndsWith("t", StringComparison.OrdinalIgnoreCase));
            var query2 = new EntityQuery<Person>(p => p.Name.StartsWith("T"));
            var aggregateQuery = new AggregateEntityQuery<Person>(query1, query2);

            var people = new[]
            {
                new Person { Id = 1, Name = "Bob" },
                new Person { Id = 2, Name = "T" },
                new Person { Id = 3, Name = "Ted" },
                new Person { Id = 4, Name = "Test" },
            }.AsQueryable();
            var filteredPeople = aggregateQuery.Apply(people);

            Assert.AreEqual(2, filteredPeople.Count());
            Assert.AreEqual(2, filteredPeople.ElementAt(0).Id);
            Assert.AreEqual(4, filteredPeople.ElementAt(1).Id);
        }

        [Test]
        public void Union_Apply_NoOverlap()
        {
            var query1 = new EntityQuery<Person>(p => p.Name == "Test");
            var query2 = new EntityQuery<Person>(p => p.Name == "T");
            var unionQuery = new UnionEntityQuery<Person>(query1, query2);

            var people = new[]
            {
                new Person { Id = 1, Name = "Bob" },
                new Person { Id = 2, Name = "T" },
                new Person { Id = 3, Name = "Ted" },
                new Person { Id = 4, Name = "Test" },
            }.AsQueryable();
            var filteredPeople = unionQuery.Apply(people);

            Assert.AreEqual(2, filteredPeople.Count());
            Assert.AreEqual(4, filteredPeople.ElementAt(0).Id);
            Assert.AreEqual(2, filteredPeople.ElementAt(1).Id);
        }

        [Test]
        public void Union_Apply_Overlap()
        {
            var query1 = new EntityQuery<Person>(p => p.Name.EndsWith("t", StringComparison.OrdinalIgnoreCase));
            var query2 = new EntityQuery<Person>(p => p.Name.StartsWith("T"));
            var unionQuery = new UnionEntityQuery<Person>(query1, query2);

            var people = new[]
            {
                new Person { Id = 1, Name = "Bob" },
                new Person { Id = 2, Name = "T" },
                new Person { Id = 3, Name = "Ted" },
                new Person { Id = 4, Name = "Test" },
            }.AsQueryable();
            var filteredPeople = unionQuery.Apply(people);

            Assert.AreEqual(3, filteredPeople.Count());
            Assert.AreEqual(2, filteredPeople.ElementAt(0).Id);
            Assert.AreEqual(4, filteredPeople.ElementAt(1).Id);
            Assert.AreEqual(3, filteredPeople.ElementAt(2).Id);
        }

        #region Test classes

        private class Person
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        #endregion
    }
}
