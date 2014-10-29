// Everything in this file is based on the CuttingEdge.EntitySorting library (http://servicelayerhelpers.codeplex.com/).

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace EntyTea.EntityQueries.UnitTests
{
    public class EntityFilterTests
    {
        [Test]
        public void AsQueryable_Always_ReturnsAValue()
        {
            // Act
            var filter = EntityFilter<Person>.AsQueryable();

            // Assert
            Assert.IsNotNull(filter, "AsQueryable should return a value.");
        }

        [Test]
        public void Where_WithValidPredicate_ReturnsAValue()
        {
            // Act
            var filter = EntityFilter<Person>.Where(p => p.Id < 5);

            // Assert
            Assert.IsNotNull(filter, "Where should never return null.");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Where_WithNullPredicate_ThrowsException()
        {
            // Arrange
            Expression<Func<Person, bool>> predicate = null;

            // Act
            EntityFilter<Person>.Where(predicate);
        }

        [Test]
        public void AsQueryable_Always_DoesNotFilter()
        {
            // Arrange
            var collection = (new Person[]
            {
                new Person { Id = 2 },
                new Person { Id = 1 }
            }).AsQueryable();

            var filter = EntityFilter<Person>.AsQueryable();

            // Act
            var stillUnfilteredCollection = filter.Filter(collection);

            // Assert
            Assert.AreEqual(2, stillUnfilteredCollection.Count(), "AsQueryable should not filter.");
        }

        [Test]
        public void Where_WithValidPredicate_FiltersCorrectly()
        {
            // Arrange
            var collection = (new Person[]
            {
                new Person { Id = 2 },
                new Person { Id = 1 }
            }).AsQueryable();

            Expression<Func<Person, bool>> predicate = p => p.Id < 2;

            var filter = EntityFilter<Person>.Where(predicate);

            // Act
            var filteredCollection = filter.Filter(collection);

            // Assert
            Assert.AreEqual(1, filteredCollection.Count());
            Assert.AreEqual(1, filteredCollection.First().Id, 1);
        }

        [Test]
        public void Where_CalledMultipleTimes_FiltersCorrectly()
        {
            // Arrange
            var collection = (new Person[]
            {
                new Person { Id = 1 },
                new Person { Id = 2 },
                new Person { Id = 3 }
            }).AsQueryable();

            var filter =
                from person in EntityFilter<Person>.AsQueryable()
                where person.Id > 1
                where person.Id < 3
                select person;

            // Act
            var filteredCollection = filter.Filter(collection);

            // Assert
            Assert.AreEqual(1, filteredCollection.Count());
            Assert.AreEqual(2, filteredCollection.First().Id, "The filter filtered incorrectly.");
        }

        [Test]
        public void ToString_OnAsQueryable_ShouldReturnAValue()
        {
            // Act
            string value = EntityFilter<Person>.AsQueryable().ToString();

            // Assert
            Assert.IsNotNull(value);
        }

        #region Test Entities

        private class Person
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public Address Address { get; set; }
            public string SetOnlyProperty { private get; set; }

            public override string ToString()
            {
                return string.Format("Person (Id: {0}, Name: {1}, Address: {2})", Id, Name, Address);
            }
        }

        private class Address
        {
            public string City { get; set; }

            public override string ToString()
            {
                return string.Format("Address (City: {0})", City);
            }
        }

        #endregion
    }
}
