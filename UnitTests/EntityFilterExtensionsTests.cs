// Everything in this file is based on the CuttingEdge.EntitySorting library (http://servicelayerhelpers.codeplex.com/).

using NUnit.Framework;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace EntyTea.EntityQueries.UnitTests
{
    public class EntityFilterExtensionsTests
    {
        private readonly IEntityFilter<Person> emptyFilter = new GoodPersonEntityFilter();

        [Test]
        public void Where_WithCorrectBaseFilterAndPredicate_ReturnsAValue()
        {
            // Arrange
            Expression<Func<Person, bool>> predicate = p => p.Id < 5;

            // Act
            var newFilter = EntityFilterExtensions.Where(this.emptyFilter, predicate);

            // Assert
            Assert.IsNotNull(newFilter, "EntityFilterExtensions.Where should never return null.");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Where_WithNullPredicate_ThrowsException()
        {
            // Arrange
            Expression<Func<Person, bool>> predicate = null;

            // Act
            EntityFilterExtensions.Where(this.emptyFilter, predicate);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Where_WithNullBaseFilter_ThrowsException()
        {
            // Arrange
            IEntityFilter<Person> invalidFilter = null;
            Expression<Func<Person, bool>> predicate = p => p.Id < 5;

            // Act
            var newFilter = EntityFilterExtensions.Where(invalidFilter, predicate);
        }

        #region Test Filters

        private sealed class GoodPersonEntityFilter : EntityFilterBase<Person>
        {
            public override IQueryable<Person> Filter(IQueryable<Person> collection)
            {
                return collection;
            }
        }

        #endregion

        #region Test Entities

        private class Person
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string SetOnlyProperty { private get; set; }

            public override string ToString()
            {
                return string.Format("Person (Id: {0}, Name: {1})", Id, Name);
            }
        }

        #endregion
    }
}
