// Everything in this file is based on the CuttingEdge.EntitySorting library (http://servicelayerhelpers.codeplex.com/).

using NUnit.Framework;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace EntyTea.EntityQueries.UnitTests
{
    public class EntitySorterExtensionsTests
    {
        private readonly IEntitySorter<Person> validEntitySorter = new ValidPersonEntitySorter();
        private readonly Expression<Func<Person, int>> validKeySelector = p => p.Id;
        private readonly string validPropertyName = "Id";

        [Test]
        public void OrderBy_WithValidArguments_ReturnsAValue()
        {
            // Act
            var sorter = EntitySorterExtensions.OrderBy(this.validEntitySorter, this.validKeySelector);

            // Assert
            Assert.IsNotNull(sorter);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void OrderBy_WithNullBaseSorter_ThrowsException()
        {
            EntitySorterExtensions.OrderBy(null, this.validKeySelector);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void OrderBy_WithNullLambda_ThrowsException()
        {
            // Arrange
            Expression<Func<Person, int>> invalidKeySelector = null;

            // Act
            EntitySorterExtensions.OrderBy(this.validEntitySorter, invalidKeySelector);
        }

        [Test]
        public void OrderByDescending_WithValidArguments_ReturnsAValue()
        {
            // Act
            var sorter =
                EntitySorterExtensions.OrderByDescending(this.validEntitySorter, this.validKeySelector);

            // Assert
            Assert.IsNotNull(sorter);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void OrderByDescending_WithNullBaseSorter_ThrowsException()
        {
            EntitySorterExtensions.OrderByDescending(null, this.validKeySelector);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void OrderByDescending_WithNullLambda_ThrowsException()
        {
            // Arrange
            Expression<Func<Person, int>> invalidKeySelector = null;

            // Act
            EntitySorterExtensions.OrderByDescending(this.validEntitySorter, invalidKeySelector);
        }

        [Test]
        public void ThenByKeySelector_WithValidArguments_ReturnsAValue()
        {
            // Act
            var sorter = EntitySorterExtensions.ThenBy(this.validEntitySorter, this.validKeySelector);

            // Assert
            Assert.IsNotNull(sorter);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ThenByKeySelector_WithNullBaseSorter_ThrowsException()
        {
            // Arrange
            IEntitySorter<Person> invalidSorter = null;

            // Act
            EntitySorterExtensions.ThenBy(invalidSorter, this.validKeySelector);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ThenByKeySelector_WithNullLambda_ThrowsException()
        {
            // Arrange
            Expression<Func<Person, int>> invalidExpression = null;

            // Act
            EntitySorterExtensions.ThenBy(this.validEntitySorter, invalidExpression);
        }

        [Test]
        public void ThenByDescendingKeySelector_WithValidArguments_ReturnsAValue()
        {
            // Act
            var sorter =
                EntitySorterExtensions.ThenByDescending(this.validEntitySorter, this.validKeySelector);

            // Assert
            Assert.IsNotNull(sorter);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ThenByDescendingKeySelector_WithNullBaseSorter_ThrowsException()
        {
            EntitySorterExtensions.ThenByDescending(null, this.validKeySelector);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ThenByDescendingKeySelector_WithNullLambda_ThrowsException()
        {
            // Arrange
            Expression<Func<Person, int>> invalidExpression = null;

            // Act
            EntitySorterExtensions.ThenByDescending(this.validEntitySorter, invalidExpression);
        }

        [Test]
        public void ThenByPropertyName_WithValidArguments_ReturnsAValue()
        {
            // Act
            var sorter = EntitySorterExtensions.ThenBy(this.validEntitySorter, this.validPropertyName);

            // Assert
            Assert.IsNotNull(sorter);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ThenByPropertyName_WithNullBaseSorter_ThrowsException()
        {
            EntitySorterExtensions.ThenBy<Person>(null, this.validPropertyName);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ThenByPropertyName_WithNullPropertyName_ThrowsException()
        {
            // Arrange
            string invalidPropertyName = null;

            // Act
            EntitySorterExtensions.ThenBy(this.validEntitySorter, invalidPropertyName);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ThenByPropertyName_WithEmptyPropertyName_ThrowsException()
        {
            // Arrange
            string invalidPropertyName = string.Empty;

            // Act
            EntitySorterExtensions.ThenBy(this.validEntitySorter, invalidPropertyName);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ThenByPropertyName_WithInvalidPropertyName_ThrowsException()
        {
            // Arrange
            string invalidPropertyName = "Address.NonExistingProperty";

            // Act
            EntitySorterExtensions.ThenBy(this.validEntitySorter, invalidPropertyName);
        }

        [Test]
        public void ThenByDescendingPropertyName_WithValidArguments_ReturnsAValue()
        {
            // Act
            var sorter =
                EntitySorterExtensions.ThenByDescending(this.validEntitySorter, this.validPropertyName);

            // Assert
            Assert.IsNotNull(sorter);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ThenByDescendingPropertyName_WithNullBaseSorter_ThrowsException()
        {
            EntitySorterExtensions.ThenByDescending<Person>(null, this.validPropertyName);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ThenByDescendingPropertyName_WithNullPropertyName_ThrowsException()
        {
            // Arrange
            string invalidPropertyName = null;

            // Act
            EntitySorterExtensions.ThenByDescending(this.validEntitySorter, invalidPropertyName);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ThenByDescendingPropertyName_WithEmptyPropertyName_ThrowsException()
        {
            // Arrange
            string invalidPropertyName = string.Empty;

            // Act
            EntitySorterExtensions.ThenByDescending(this.validEntitySorter, invalidPropertyName);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ThenByDescendingPropertyName_WithInvalidPropertyName_ThrowsException()
        {
            // Arrange
            string invalidPropertyName = "Address.NonExistingProperty";

            // Act
            EntitySorterExtensions.ThenByDescending(this.validEntitySorter, invalidPropertyName);
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