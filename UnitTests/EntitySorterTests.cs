// Everything in this file is based on the CuttingEdge.EntitySorting library (http://servicelayerhelpers.codeplex.com/).

using NUnit.Framework;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace EntyTea.EntityQueries.UnitTests
{
    public class EntitySorterTests
    {
        private readonly IEntitySorter<Person> validEntitySorter = new ValidPersonEntitySorter();

        [Test]
        public void AsQueryable_ReturnsAValue()
        {
            // Act
            var order = EntitySorter<Person>.AsQueryable();

            // Assert
            Assert.IsNotNull(order, "AsQueryable should return a value.");
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AsQueryable_ReturnsAValueThatCanNotSort()
        {
            // Arrange
            var persons = Enumerable.Repeat(new Person(), 0).AsQueryable();

            // Act
            var defaultSorter = EntitySorter<Person>.AsQueryable();

            // The default should throw an InvalidOperationException
            defaultSorter.Sort(persons);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void OrderBy_WithNullString_ThrowsException()
        {
            EntitySorter<Person>.OrderBy((string)null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void OrderBy_SortCalledWithNullCollection_ThrowsException()
        {
            // Arrange
            var sorter = EntitySorter<Person>.OrderBy(p => p.Id);

            // Act
            sorter.Sort(null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void OrderBy_WithEmptyString_ThrowsException()
        {
            EntitySorter<Person>.OrderBy(string.Empty);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void OrderBy_WithInvalidString_ThrowsException()
        {
            EntitySorter<Person>.OrderBy("nonExistingPropertyName");
        }

        [Test]
        public void OrderBy_WithValidPropertyName_ReturnsAValue()
        {
            // Act
            var sorter = EntitySorter<Person>.OrderBy("Id");

            // Assert
            Assert.IsNotNull(sorter, "OrderBy(string) should never return null.");
        }

        [Test]
        public void OrderBy_WithValidChainedPropertyName_ReturnsAValue()
        {
            // Act
            var sorter = EntitySorter<Person>.OrderBy("Address.City");

            // Assert
            Assert.IsNotNull(sorter, "OrderBy(string) should never return null.");
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void OrderBy_WithInvalidSetOnlyProperty_ThrowsException()
        {
            const string SetOnlyPropertyName = "SetOnlyProperty";

            // Validate the existance of the SetOnlyProperty
            Assert.IsNotNull(typeof(Person).GetProperty(SetOnlyPropertyName),
                "Test could not be executed! Property {0} is missing on {1} class",
                SetOnlyPropertyName,
                typeof(Person));

            // Act
            EntitySorter<Person>.OrderBy("SetOnlyProperty");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void OrderBy_WithNullLambda_ThrowsException()
        {
            // Arrange
            Expression<Func<Person, int>> expression = null;

            // Act
            EntitySorter<Person>.OrderBy(expression);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ThenBy_WithNullString_ThrowsException()
        {
            // Arrange
            var sorter = this.validEntitySorter;

            // Act
            sorter.ThenBy((string)null);
        }

        [Test]
        public void ThenBy_WithValidPropertyName_ReturnsAValue()
        {
            // Arrange
            var sorter = this.validEntitySorter;

            // Act
            var newSorter = sorter.ThenBy("Id");

            // Assert
            Assert.IsNotNull(newSorter, "ThenBy(string) should never return null.");
        }

        [Test]
        public void ThenBy_WithValidChainedPropertyName_ReturnsAValue()
        {
            // Arrange
            var sorter = this.validEntitySorter;

            // Act
            var newSorter = sorter.ThenBy("Address.City");

            // Assert
            Assert.IsNotNull(newSorter, "ThenBy(string) should never return null.");
        }

        [Test]
        public void ThenBy_WithValidLambda_ReturnsAValue()
        {
            // Arrange
            var sorter = this.validEntitySorter;

            // Act
            var newSorter = sorter.ThenBy(p => p.Id);

            // Assert
            Assert.IsNotNull(newSorter, "ThenBy(string) should never return null.");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void OrderByDescending_WithNullString_ThrowsException()
        {
            EntitySorter<Person>.OrderByDescending((string)null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void OrderByDescending_WithEmptyString_ThrowsException()
        {
            EntitySorter<Person>.OrderByDescending(string.Empty);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void OrderByDescending_WithInvalidString_ThrowsException()
        {
            EntitySorter<Person>.OrderByDescending("nonExistingPropertyName");
        }

        [Test]
        public void OrderByDescending_WithValidPropertyName_ReturnsAValue()
        {
            // Act
            var sorter = EntitySorter<Person>.OrderByDescending("Id");

            // Assert
            Assert.IsNotNull(sorter, "OrderBy(string) should never return null.");
        }

        [Test]
        public void OrderByDescending_WithValidChainedPropertyName_ReturnsAValue()
        {
            // Act
            var sorter = EntitySorter<Person>.OrderByDescending("Address.City");

            // Assert
            Assert.IsNotNull(sorter, "OrderBy(string) should never return null.");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void OrderByDescending_WithNullLambda_ThrowsException()
        {
            Expression<Func<Person, int>> expression = null;

            EntitySorter<Person>.OrderByDescending(expression);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ThenByDescending_WithNullString_ThrowsException()
        {
            // Arrange
            var sorter = this.validEntitySorter;

            // Act
            sorter.ThenByDescending((string)null);
        }

        [Test]
        public void ThenByDescending_WithValidPropertyName_ReturnsAValue()
        {
            // Arrange
            var sorter = this.validEntitySorter;

            // Act
            var newSorter = sorter.ThenByDescending("Id");

            // Assert
            Assert.IsNotNull(newSorter, "ThenBy(string) should never return null.");
        }

        [Test]
        public void ThenByDescending_WithValidChainedPropertyName_ReturnsAValue()
        {
            // Arrange
            var sorter = this.validEntitySorter;

            // Act
            var newSorter = sorter.ThenByDescending("Address.City");

            // Assert
            Assert.IsNotNull(newSorter, "ThenBy(string) should never return null.");
        }

        [Test]
        public void ThenByDescending_WithValidLambda_ReturnsAValue()
        {
            // Arrange
            var sorter = this.validEntitySorter;

            // Act
            var newSorter = sorter.ThenByDescending(p => p.Id);

            // Assert
            Assert.IsNotNull(newSorter, "ThenBy(string) should never return null.");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Sort_WithNullCollection_ThrowsException()
        {
            // Arrange
            var sorter = this.validEntitySorter;

            // Act
            sorter.Sort(null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Sort_WithNullCollectionOnThenBySorter_ThrowsException()
        {
            // Arrange
            var sorter = this.validEntitySorter;

            sorter = sorter.ThenBy(p => p.Name);

            // Act
            sorter.Sort(null);
        }

        [Test]
        public void Sort_WithValidOrderByPropertyName_SortsCorrectly()
        {
            // Arrange
            var collection = (new Person[]
            {
                new Person { Id = 6 },
                new Person { Id = 1 }
            }).AsQueryable();

            var sorter = EntitySorter<Person>.OrderBy("Id");

            // Act
            var sortedCollection = sorter.Sort(collection);

            // Assert
            Assert.AreEqual(2, sortedCollection.Count());
            Assert.AreEqual(1, sortedCollection.First().Id, "The collection is not correctly sorted.");
            Assert.AreEqual(6, sortedCollection.Last().Id, "The collection is not correctly sorted.");
        }

        [Test]
        public void Sort_WithValidOrderByDescendingPropertyName_SortsCorrectly()
        {
            // Arrange
            var collection = (new Person[]
            {
                new Person { Id = 1 },
                new Person { Id = 6 }
            }).AsQueryable();

            var sorter = EntitySorter<Person>.OrderByDescending("Id");

            // Act
            var sortedCollection = sorter.Sort(collection);

            // Assert
            Assert.AreEqual(collection.Count(), sortedCollection.Count());
            Assert.AreEqual(6, sortedCollection.First().Id, "The collection is not correctly sorted.");
            Assert.AreEqual(1, sortedCollection.Last().Id, "The collection is not correctly sorted.");
        }

        [Test]
        public void Sort_WithValidOrderByChainedPropertyName_SortsCorrectly()
        {
            // Arrange
            var collection = (new Person[]
            {
                new Person { Address = new Address { City = "Bravo" } },
                new Person { Address = new Address { City = "Alpha" } }
            }).AsQueryable();

            var sorter = EntitySorter<Person>.OrderBy("Address.City");

            // Act
            var sortedCollection = sorter.Sort(collection);

            // Assert
            Assert.AreEqual(collection.Count(), sortedCollection.Count());
            Assert.AreEqual("Alpha", sortedCollection.First().Address.City, "The collection is not correctly sorted.");
            Assert.AreEqual("Bravo", sortedCollection.Last().Address.City, "The collection is not correctly sorted.");
        }

        [Test]
        public void Sort_WithValidOrderByDescendingChainedPropertyName_SortsCorrectly()
        {
            // Arrange
            var collection = (new Person[]
            {
                new Person { Address = new Address { City = "Alpha" } },
                new Person { Address = new Address { City = "Bravo" } }
            }).AsQueryable();

            var sorter = EntitySorter<Person>.OrderByDescending("Address.City");

            // Act
            var sortedCollection = sorter.Sort(collection);

            // Assert
            Assert.AreEqual(collection.Count(), sortedCollection.Count());
            Assert.AreEqual("Bravo", sortedCollection.First().Address.City, "The collection is not correctly sorted.");
            Assert.AreEqual("Alpha", sortedCollection.Last().Address.City, "The collection is not correctly sorted.");
        }

        [Test]
        public void Sort_WithValidOrderByLambda_SortsCorrectly()
        {
            // Arrange
            var collection = (new Person[]
            {
                new Person { Id = 6 },
                new Person { Id = 1 }
            }).AsQueryable();

            var sorter = EntitySorter<Person>.OrderBy(p => p.Id);

            // Act
            var sortedCollection = sorter.Sort(collection).ToArray();

            // Assert
            Assert.AreEqual(collection.Count(), sortedCollection.Count());
            Assert.AreEqual(1, sortedCollection.First().Id, "The collection is not correctly sorted.");
            Assert.AreEqual(6, sortedCollection.Last().Id, "The collection is not correctly sorted.");
        }

        [Test]
        public void Sort_WithValidOrderByDescendingLambda_SortsCorrectly()
        {
            // Arrange
            var collection = (new Person[]
            {
                new Person { Id = 1 },
                new Person { Id = 6 }
            }).AsQueryable();

            var sorter = EntitySorter<Person>.OrderByDescending(p => p.Id);

            // Act
            var sortedCollection = sorter.Sort(collection);

            // Assert
            Assert.AreEqual(collection.Count(), sortedCollection.Count());
            Assert.AreEqual(6, sortedCollection.First().Id, "The collection is not correctly sorted.");
            Assert.AreEqual(1, sortedCollection.Last().Id, "The collection is not correctly sorted.");
        }

        [Test]
        public void Sort_WithValidOrderByPropertyNameThenByPropertyName_SortsCorrectly()
        {
            // Arrange
            var collection = (new Person[]
            {
                new Person { Id = 6, Name = "Bravo" },
                new Person { Id = 1, Name = "Alpha" },
                new Person { Id = 6, Name = "Alpha" },
                new Person { Id = 1, Name = "Bravo" },
            }).AsQueryable();

            var sorter = EntitySorter<Person>.OrderBy("Id").ThenBy("Name");

            // Act
            var sortedCollection = sorter.Sort(collection).ToArray();

            // Assert
            Assert.AreEqual(4, sortedCollection.Count());

            Assert.AreEqual(1, sortedCollection[0].Id, "The collection is not correctly sorted.");
            Assert.AreEqual("Alpha", sortedCollection[0].Name, "The collection is not correctly sorted.");

            Assert.AreEqual(1, sortedCollection[1].Id, "The collection is not correctly sorted.");
            Assert.AreEqual("Bravo", sortedCollection[1].Name, "The collection is not correctly sorted.");

            Assert.AreEqual(6, sortedCollection[2].Id, "The collection is not correctly sorted.");
            Assert.AreEqual("Alpha", sortedCollection[2].Name, "The collection is not correctly sorted.");

            Assert.AreEqual(6, sortedCollection[3].Id, "The collection is not correctly sorted.");
            Assert.AreEqual("Bravo", sortedCollection[3].Name, "The collection is not correctly sorted.");
        }

        [Test]
        public void Sort_WithValidOrderByPropertyNameThenByDescendingPropertyName_SortsCorrectly()
        {
            // Arrange
            var collection = (new Person[]
            {
                new Person { Id = 6, Name = "Bravo" },
                new Person { Id = 1, Name = "Alpha" },
                new Person { Id = 6, Name = "Alpha" },
                new Person { Id = 1, Name = "Bravo" },
            }).AsQueryable();

            var sorter = EntitySorter<Person>.OrderBy("Id").ThenByDescending("Name");

            // Act
            var sortedCollection = sorter.Sort(collection).ToArray();

            // Assert
            Assert.AreEqual(4, sortedCollection.Count());

            Assert.AreEqual(1, sortedCollection[0].Id, "The collection is not correctly sorted.");
            Assert.AreEqual("Bravo", sortedCollection[0].Name, "The collection is not correctly sorted.");

            Assert.AreEqual(1, sortedCollection[1].Id, "The collection is not correctly sorted.");
            Assert.AreEqual("Alpha", sortedCollection[1].Name, "The collection is not correctly sorted.");

            Assert.AreEqual(6, sortedCollection[2].Id, "The collection is not correctly sorted.");
            Assert.AreEqual("Bravo", sortedCollection[2].Name, "The collection is not correctly sorted.");

            Assert.AreEqual(6, sortedCollection[3].Id, "The collection is not correctly sorted.");
            Assert.AreEqual("Alpha", sortedCollection[3].Name, "The collection is not correctly sorted.");
        }

        [Test]
        public void Sort_WithThenByLambda_SortsCorrectly()
        {
            // Arrange
            var collection = (new Person[]
            {
                new Person { Id = 6, Name = "Bravo" },
                new Person { Id = 1, Name = "Alpha" },
                new Person { Id = 6, Name = "Alpha" },
                new Person { Id = 1, Name = "Bravo" },
            }).AsQueryable();

            var sorter = EntitySorter<Person>.OrderBy(p => p.Id).ThenBy(p => p.Name);

            // Act
            var sortedCollection = sorter.Sort(collection).ToArray();

            // Assert
            Assert.AreEqual(4, sortedCollection.Count());

            Assert.AreEqual(1, sortedCollection[0].Id, "The collection is not correctly sorted.");
            Assert.AreEqual("Alpha", sortedCollection[0].Name, "The collection is not correctly sorted.");

            Assert.AreEqual(1, sortedCollection[1].Id, "The collection is not correctly sorted.");
            Assert.AreEqual("Bravo", sortedCollection[1].Name, "The collection is not correctly sorted.");

            Assert.AreEqual(6, sortedCollection[2].Id, "The collection is not correctly sorted.");
            Assert.AreEqual("Alpha", sortedCollection[2].Name, "The collection is not correctly sorted.");

            Assert.AreEqual(6, sortedCollection[3].Id, "The collection is not correctly sorted.");
            Assert.AreEqual("Bravo", sortedCollection[3].Name, "The collection is not correctly sorted.");
        }

        [Test]
        public void Sort_WithThenByDescendingLambda_SortsCorrectly()
        {
            // Arrange
            var collection = (new Person[]
            {
                new Person { Id = 6, Name = "Bravo" },
                new Person { Id = 1, Name = "Alpha" },
                new Person { Id = 6, Name = "Alpha" },
                new Person { Id = 1, Name = "Bravo" },
            }).AsQueryable();

            var sorter = EntitySorter<Person>.OrderBy(p => p.Id).ThenByDescending(p => p.Name);

            // Act
            var sortedCollection = sorter.Sort(collection).ToArray();

            // Assert
            Assert.AreEqual(4, sortedCollection.Count());

            Assert.AreEqual(1, sortedCollection[0].Id, "The collection is not correctly sorted.");
            Assert.AreEqual("Bravo", sortedCollection[0].Name, "The collection is not correctly sorted.");

            Assert.AreEqual(1, sortedCollection[1].Id, "The collection is not correctly sorted.");
            Assert.AreEqual("Alpha", sortedCollection[1].Name, "The collection is not correctly sorted.");

            Assert.AreEqual(6, sortedCollection[2].Id, "The collection is not correctly sorted.");
            Assert.AreEqual("Bravo", sortedCollection[2].Name, "The collection is not correctly sorted.");

            Assert.AreEqual(6, sortedCollection[3].Id, "The collection is not correctly sorted.");
            Assert.AreEqual("Alpha", sortedCollection[3].Name, "The collection is not correctly sorted.");
        }

        [Test]
        public void ToString_OnOrderByEntitySorter_ReturnsAValue()
        {
            // Arrange
            var sorter = EntitySorter<Person>.OrderBy("Id");

            // Act
            string value = sorter.ToString();

            // Assert
            Assert.IsNotNull(value);
            Assert.AreNotEqual(string.Empty, value);
        }

        [Test]
        public void ToString_OnThenByEntitySorter_ReturnsAValue()
        {
            // Arrange
            var sorter = EntitySorter<Person>.OrderBy("Id").ThenBy("Name");

            // Act
            string value = sorter.ToString();

            // Assert
            Assert.IsNotNull(value);
            Assert.AreNotEqual(string.Empty, value);
        }

        [Test]
        public void ToString_OnAsQueryabke_ReturnsAValue()
        {
            // Arrange
            var sorter = EntitySorter<Person>.AsQueryable();

            // Act
            string value = sorter.ToString();

            // Assert
            Assert.IsNotNull(value);
        }

        #region Test Sorters

        private sealed class ValidPersonEntitySorter : EntitySorterBase<Person>
        {
            public override IOrderedQueryable<Person> Sort(IQueryable<Person> collection)
            {
                return collection.OrderBy(p => p.Id);
            }

            public override IOrderedQueryable<Person> SortDescending(IQueryable<Person> collection)
            {
                return collection.OrderByDescending(p => p.Id);
            }
        }

        #endregion

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
