﻿// Everything in this file is based on the CuttingEdge.EntitySorting library (http://servicelayerhelpers.codeplex.com/).

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace EntyTea.EntityQueries
{
    /// <summary>Defines a <see cref="Sort"/> method that enables sorting of a specified collection.</summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public interface IEntitySorter<TEntity> : IEntitySorter
    {
        /// <summary>Sorts the specified collection.</summary>
        /// <param name="collection">The collection.</param>
        /// <returns>An <see cref="IOrderedEnumerable{TEntity}"/> whose elements are sorted according to the
        /// <see cref="EntitySorter{TEntity}"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="collection"/> is a null 
        /// reference.</exception>
        IOrderedQueryable<TEntity> Sort(IQueryable<TEntity> collection);

        /// <summary>Sorts the specified collection in descending order.</summary>
        /// <param name="collection">The collection.</param>
        /// <returns>An <see cref="IOrderedEnumerable{TEntity}"/> whose elements are sorted according to the
        /// <see cref="EntitySorter{TEntity}"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="collection"/> is a null 
        /// reference.</exception>
        IOrderedQueryable<TEntity> SortDescending(IQueryable<TEntity> collection);
    }

    /// <summary>Defines a <see cref="Sort"/> method that enables sorting of a specified collection, where the type of the collection is not known.</summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public interface IEntitySorter
    {
        /// <summary>Sorts the specified collection.</summary>
        /// <param name="collection">The collection.</param>
        /// <returns>An <see cref="IOrderedEnumerable{TEntity}"/> whose elements are sorted according to the <see cref="IEntitySorter"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="collection"/> is a null 
        /// reference.</exception>
        IOrderedQueryable Sort(IQueryable collection);

        /// <summary>Sorts the specified collection in descending order.</summary>
        /// <param name="collection">The collection.</param>
        /// <returns>An <see cref="IOrderedEnumerable{TEntity}"/> whose elements are sorted according to the <see cref="EntitySorter"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="collection"/> is a null 
        /// reference.</exception>
        IOrderedQueryable SortDescending(IQueryable collection);
    }

    /// <summary>
    /// A base implementation of <c>IEntitySorter{TEntity}</c>
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class EntitySorterBase<TEntity> : IEntitySorter<TEntity>
    {
        public abstract IOrderedQueryable<TEntity> Sort(IQueryable<TEntity> collection);
        public abstract IOrderedQueryable<TEntity> SortDescending(IQueryable<TEntity> collection);

        #region IOrderedQueryable

        IOrderedQueryable IEntitySorter.Sort(IQueryable collection)
        {
            return Sort(collection.OfType<TEntity>());
        }

        IOrderedQueryable IEntitySorter.SortDescending(IQueryable collection)
        {
            return SortDescending(collection.OfType<TEntity>());
        }

        #endregion
    }

    /// <summary>Enables sorting of entities.</summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public static class EntitySorter<TEntity>
    {
        /// <summary>
        /// Returns a <see cref="IEntitySorter{TEntity}"/> instance that allows construction of
        /// <see cref="IEntitySorter{TEntity}"/> objects though the use of LINQ syntax.
        /// </summary>
        /// <returns>A <see cref="IEntitySorter{TEntity}"/> instance.</returns>
        public static IEntitySorter<TEntity> AsQueryable()
        {
            return new EmptyEntitySorter();
        }

        /// <summary>
        /// Creates a new <see cref="IEntitySorter{TEntity}"/> that sorts a collection of 
        /// <typeparamref name="TEntity"/> objects in ascending order according to a key.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="keySelector">A function to extract a key from an element.</param>
        /// <returns>A new <see cref="IEntitySorter{TEntity}"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="keySelector"/> null.</exception>
        public static IEntitySorter<TEntity> OrderBy<TKey>(Expression<Func<TEntity, TKey>> keySelector)
        {
            if (keySelector == null)
            {
                throw new ArgumentNullException("keySelector");
            }
            return new OrderByEntitySorter<TEntity, TKey>(keySelector, SortDirection.Ascending);
        }

        /// <summary>
        /// Creates a new <see cref="IEntitySorter{TEntity}"/> that sorts a collection of 
        /// <typeparamref name="TEntity"/> objects in descending order according to a key.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="keySelector">A function to extract a key from an element.</param>
        /// <returns>A new <see cref="IEntitySorter{TEntity}"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the specified <paramref name="keySelector"/>
        /// is a null reference.</exception>
        public static IEntitySorter<TEntity> OrderByDescending<TKey>(Expression<Func<TEntity, TKey>> keySelector)
        {
            if (keySelector == null)
            {
                throw new ArgumentNullException("keySelector");
            }

            return new OrderByEntitySorter<TEntity, TKey>(keySelector, SortDirection.Descending);
        }

        /// <summary>
        /// Creates a new <see cref="IEntitySorter{TEntity}"/> that sorts a collection of 
        /// <typeparamref name="TEntity"/> objects in ascending order by using the property, specified by its
        /// <paramref name="propertyName"/>.</summary>
        /// <param name="propertyName">
        /// Name of the property or a list of chained properties, separated by a dot.</param>
        /// <returns>A new <see cref="IEntitySorter{TEntity}"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when the specified <paramref name="propertyName"/> is
        /// empty or when the specified property could not be found on the <typeparamref name="TEntity"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="propertyName"/> is null.</exception>
        public static IEntitySorter<TEntity> OrderBy(string propertyName)
        {
            var builder = new EntitySorterBuilder<TEntity>(propertyName);

            builder.SortDirection = SortDirection.Ascending;

            return builder.BuildOrderByEntitySorter();
        }

        /// <summary>
        /// Creates a new <see cref="IEntitySorter{TEntity}"/> that sorts a collection of 
        /// <typeparamref name="TEntity"/> objects in ascending order by using the property, specified by its
        /// <paramref name="propertyName"/>.</summary>
        /// <param name="propertyName">
        /// Name of the property or a list of chained properties, separated by a dot.</param>
        /// <returns>A <see cref="Boolean"/> which indicates whether the sort is valid.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="propertyName"/> is null.</exception>
        public static bool TryOrderBy(string propertyName, out IEntitySorter<TEntity> entitySorter)
        {
            if (propertyName == null)
            {
                throw new ArgumentNullException("propertyName");
            }
            try
            {
                entitySorter = (IEntitySorter<TEntity>)OrderBy(propertyName);
                return true;
            }
            catch (ArgumentException) { }
            entitySorter = default(IEntitySorter<TEntity>);
            return false;
        }

        /// <summary>
        /// Creates a new <see cref="IEntitySorter{TEntity}"/> that sorts a collection of 
        /// <typeparamref name="TEntity"/> objects in descending order by using the property, specified by it's
        /// <paramref name="propertyName"/>.</summary>
        /// <param name="propertyName">
        /// Name of the property or a list of chained properties, separated by a dot.</param>
        /// <returns>A new <see cref="IEntitySorter{TEntity}"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when the specified <paramref name="propertyName"/> is
        /// empty or when the specified property could not be found on the <typeparamref name="TEntity"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="propertyName"/> is null.</exception>
        public static IEntitySorter<TEntity> OrderByDescending(string propertyName)
        {
            var builder = new EntitySorterBuilder<TEntity>(propertyName);

            builder.SortDirection = SortDirection.Descending;

            return builder.BuildOrderByEntitySorter();
        }

        /// <summary>
        /// Creates a new <see cref="IEntitySorter{TEntity}"/> that sorts a collection of 
        /// <typeparamref name="TEntity"/> objects in descending order by using the property, specified by its
        /// <paramref name="propertyName"/>.</summary>
        /// <param name="propertyName">
        /// Name of the property or a list of chained properties, separated by a dot.</param>
        /// <returns>A <see cref="Boolean"/> which indicates whether the sort is valid.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="propertyName"/> is null.</exception>
        public static bool TryOrderByDescending(string propertyName, out IEntitySorter<TEntity> entitySorter)
        {
            if (propertyName == null)
            {
                throw new ArgumentNullException("propertyName");
            }
            try
            {
                entitySorter = (IEntitySorter<TEntity>)OrderByDescending(propertyName);
                return true;
            }
            catch (ArgumentException) { }
            entitySorter = default(IEntitySorter<TEntity>);
            return false;
        }

        /// <summary>The default entity sorter implementation, that throws an exception.</summary>
        [DebuggerDisplay("EmptyEntitySorter ( Unordered )")]
        private sealed class EmptyEntitySorter : EntitySorterBase<TEntity>
        {
            /// <summary>Throws an <see cref="InvalidOperationException"/> when called.</summary>
            /// <param name="collection">The collection.</param>
            /// <returns>An <see cref="InvalidOperationException"/> is thrown.</returns>
            /// <exception cref="InvalidOperationException">Thrown an <see cref="InvalidOperationException"/>
            /// when called.</exception>
            public override IOrderedQueryable<TEntity> Sort(IQueryable<TEntity> collection)
            {
                string exceptionMessage = "The EmptyEntitySorter can not be used for sorting, please call the " +
                    "OrderBy or OrderByDescending instance methods.";

                throw new InvalidOperationException(exceptionMessage);
            }

            public override System.Linq.IOrderedQueryable<TEntity> SortDescending(System.Linq.IQueryable<TEntity> collection)
            {
                string exceptionMessage = "The EmptyEntitySorter can not be used for sorting, please call the " +
                    "OrderBy or OrderByDescending instance methods.";
                throw new InvalidOperationException(exceptionMessage);
            }

            /// <summary>Returns string.Empty.</summary>
            /// <returns>An empty string.</returns>
            public override string ToString()
            {
                // The string representation of IEntitySorter objects is used in the Debugger.
                return string.Empty;
            }
        }
    }

    /// <summary>Extension methods on <see cref="IEntitySorter{TEntity}"/>.</summary>
    public static class EntitySorterExtensions
    {
        /// <summary>
        /// Creates a new <see cref="IEntitySorter{TEntity}"/> that sorts  the elements of a sequence
        /// in ascending order according to a key.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by the function that is represented by keySelector.</typeparam>
        /// <param name="sorter">The sorter.</param>
        /// <param name="keySelector">A function to extract a key from an element.</param>
        /// <returns>An <see cref="IEntitySorter{TEntity}"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="sorter"/> or 
        /// <paramref name="keySelector"/> are null.</exception>
        public static IEntitySorter<TEntity> OrderBy<TEntity, TKey>(this IEntitySorter<TEntity> sorter,
            Expression<Func<TEntity, TKey>> keySelector)
        {
            if (sorter == null)
            {
                throw new ArgumentNullException("sorter");
            }

            if (keySelector == null)
            {
                throw new ArgumentNullException("keySelector");
            }

            // Note: The sorter parameter is not used, because an OrderBy will invalidate all previous
            // OrderBy and ThenBy statements (as Enumerable.OrderBy and Queryable.OrderBy do).
            return EntitySorter<TEntity>.OrderBy(keySelector);
        }

        /// <summary>
        /// Creates a new <see cref="IEntitySorter{TEntity}"/> that sorts the elements of a sequence
        /// in descending order according to a key.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by the function that is represented by keySelector.</typeparam>
        /// <param name="sorter">The sorter.</param>
        /// <param name="keySelector">A function to extract a key from an element.</param>
        /// <returns>An <see cref="IEntitySorter{TEntity}"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="sorter"/> or 
        /// <paramref name="keySelector"/> are null.</exception>
        public static IEntitySorter<TEntity> OrderByDescending<TEntity, TKey>(this IEntitySorter<TEntity> sorter,
            Expression<Func<TEntity, TKey>> keySelector)
        {
            if (sorter == null)
            {
                throw new ArgumentNullException("sorter");
            }

            if (keySelector == null)
            {
                throw new ArgumentNullException("keySelector");
            }

            // Note: The sorter parameter is not used, because an OrderBy will invalidate all previous
            // OrderBy and ThenBy statements (as Enumerable.OrderBy and Queryable.OrderBy do).
            return EntitySorter<TEntity>.OrderByDescending(keySelector);
        }

        /// <summary>
        /// Creates a new <see cref="IEntitySorter{TEntity}"/> that performs a subsequent ordering of the
        /// elements in in a collection of <typeparamref name="TEntity"/> objects in ascending order
        /// according to a key.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="sorter">The sorter.</param>
        /// <param name="keySelector">The key selector.</param>
        /// <returns>An <see cref="IEntitySorter{TEntity}"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="sorter"/> or 
        /// <paramref name="keySelector"/> are null.</exception>
        public static IEntitySorter<TEntity> ThenBy<TEntity, TKey>(this IEntitySorter<TEntity> sorter,
            Expression<Func<TEntity, TKey>> keySelector)
        {
            if (sorter == null)
            {
                throw new ArgumentNullException("sorter");
            }

            if (keySelector == null)
            {
                throw new ArgumentNullException("keySelector");
            }

            // Wrap the original sorter in a new entity sorter to extend the sorting.
            return new ThenByEntitySorter<TEntity, TKey>(sorter, keySelector, SortDirection.Ascending);
        }

        /// <summary>
        /// Creates a new <see cref="IEntitySorter{TEntity}"/> that performs a subsequent ordering of the
        /// elements in in aa collection of <typeparamref name="TEntity"/> objects in descending order
        /// according to a key.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="sorter">The sorter.</param>
        /// <param name="keySelector">The key selector.</param>
        /// <returns>A new <see cref="EntitySorter{TEntity}"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="sorter"/> or 
        /// <paramref name="keySelector"/> are null.</exception>
        public static IEntitySorter<TEntity> ThenByDescending<TEntity, TKey>(this IEntitySorter<TEntity> sorter,
            Expression<Func<TEntity, TKey>> keySelector)
        {
            if (sorter == null)
            {
                throw new ArgumentNullException("sorter");
            }

            if (keySelector == null)
            {
                throw new ArgumentNullException("keySelector");
            }

            // Wrap the original sorter in a new entity sorter to extend the sorting.
            return new ThenByEntitySorter<TEntity, TKey>(sorter, keySelector, SortDirection.Descending);
        }

        /// <summary>
        /// Creates a new <see cref="IEntitySorter{TEntity}"/> that performs a subsequent ordering of the
        /// elements in in a collection of <typeparamref name="TEntity"/> objects in ascending order
        /// by using the property, specified by it's <paramref name="propertyName"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="sorter">The sorter.</param>
        /// <param name="propertyName">Name of the property or a list of chained properties, separated by a dot.</param>
        /// <returns>A new <see cref="IEntitySorter{TEntity}"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the specified <paramref name="keySelector"/>
        /// is a null reference.</exception>
        /// <exception cref="ArgumentException">Thrown when the specified <paramref name="propertyName"/> is
        /// empty or when the specified property could not be found on the <typeparamref name="TEntity"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="sorter"/> or 
        /// <paramref name="propertyName"/> are null.</exception>
        public static IEntitySorter<TEntity> ThenBy<TEntity>(this IEntitySorter<TEntity> sorter,
            string propertyName)
        {
            if (sorter == null)
            {
                throw new ArgumentNullException("sorter");
            }

            var builder = new EntitySorterBuilder<TEntity>(propertyName);

            builder.SortDirection = SortDirection.Ascending;

            return builder.BuildThenByEntitySorter(sorter);
        }

        /// <summary>
        /// Creates a new <see cref="IEntitySorter{TEntity}"/> that performs a subsequent ordering of the
        /// elements in in a collection of <typeparamref name="TEntity"/> objects in descending order
        /// by using the property, specified by it's <paramref name="propertyName"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="sorter">The sorter.</param>
        /// <param name="propertyName">Name of the property or a list of chained properties, separated by a dot.</param>
        /// <returns>A new <see cref="IEntitySorter{TEntity}"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the specified <paramref name="keySelector"/>
        /// is a null reference or when the specified <paramref name="sorter"/> is a null reference.</exception>
        /// <exception cref="ArgumentException">Thrown when the specified <paramref name="propertyName"/> is
        /// empty or when the specified property could not be found on the <typeparamref name="TEntity"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="sorter"/> or 
        /// <paramref name="propertyName"/> are null.</exception>
        public static IEntitySorter<TEntity> ThenByDescending<TEntity>(this IEntitySorter<TEntity> sorter,
            string propertyName)
        {
            if (sorter == null)
            {
                throw new ArgumentNullException("sorter");
            }

            var builder = new EntitySorterBuilder<TEntity>(propertyName);

            builder.SortDirection = SortDirection.Descending;

            return builder.BuildThenByEntitySorter(sorter);
        }
    }

    /// <summary>Allows creation of <see cref="IEntitySorter{TEntity}"/> types, based on a supplied property name.</summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    internal class EntitySorterBuilder<TEntity>
    {
        private readonly Type keyType;
        private readonly LambdaExpression keySelector;

        /// <summary>Initializes a new instance of the <see cref="EntitySorterBuilder{TEntity}"/> class.</summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="propertyName"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="propertyName"/> is an empty string
        /// or <paramref name="propertyName"/> could not be parsed or is not a property or chain of properties
        /// referenced by the given <typeparamref name="TEntity"/>.</exception>
        public EntitySorterBuilder(string propertyName)
        {
            if (propertyName == null)
            {
                throw new ArgumentNullException("propertyName");
            }

            if (propertyName.Length == 0)
            {
                throw new ArgumentException("The value should not be an empty string.", "propertyName");
            }

            List<MethodInfo> propertyAccessors = GetPropertyAccessors(propertyName);

            this.keyType = propertyAccessors.Last().ReturnType;

            this.keySelector = BuildLambda(propertyAccessors, this.keyType);
        }

        /// <summary>Defines a method to build LambdaExpression from a list of property accessors.</summary>
        private interface ILambdaBuilder
        {
            /// <summary>Builds the lambda from the supplied list of property accessors.</summary>
            /// <param name="propertyAccessors">The property accessors.</param>
            /// <returns>A new <see cref="LambdaExpression"/>.</returns>
            LambdaExpression BuildLambda(IEnumerable<MethodInfo> propertyAccessors);
        }

        /// <summary>Gets or sets the sort direction.</summary>
        public SortDirection SortDirection { get; set; }

        internal IEntitySorter<TEntity> BuildOrderByEntitySorter()
        {
            Type[] typeArguments = new[] { typeof(TEntity), this.keyType };

            Type sorterType = typeof(OrderByEntitySorter<,>).MakeGenericType(typeArguments);

            object[] constructorArguments = { this.keySelector, this.SortDirection };

            object instance = Activator.CreateInstance(sorterType, constructorArguments);

            return (IEntitySorter<TEntity>)instance;
        }

        internal IEntitySorter<TEntity> BuildThenByEntitySorter(IEntitySorter<TEntity> baseSorter)
        {
            Type[] typeArguments = new[] { typeof(TEntity), this.keyType };

            Type sorterType = typeof(ThenByEntitySorter<,>).MakeGenericType(typeArguments);

            object[] constructorArguments = { baseSorter, this.keySelector, this.SortDirection };

            object instance = Activator.CreateInstance(sorterType, constructorArguments);

            return (IEntitySorter<TEntity>)instance;
        }

        // Builds a Expression<Func<TEntity, TKey>>
        private static LambdaExpression BuildLambda(IEnumerable<MethodInfo> propertyAccessors,
            Type keyType)
        {
            ILambdaBuilder lambdaBuilder = CreateGenericLambdaBuilder(keyType);

            return lambdaBuilder.BuildLambda(propertyAccessors);
        }

        private static ILambdaBuilder CreateGenericLambdaBuilder(Type keyType)
        {
            Type[] typeArguments = new[] { typeof(TEntity), keyType };

            Type lambdaBuilderType = typeof(GenericLambdaBuilder<>).MakeGenericType(typeArguments);

            return (ILambdaBuilder)Activator.CreateInstance(lambdaBuilderType);
        }

        // Throws an ArgumentException when the propertyNameChain is invalid.
        private static List<MethodInfo> GetPropertyAccessors(string propertyName)
        {
            try
            {
                string propertyNameChain = propertyName;

                return GetPropertyAccessorsFromPropertyNameChain(propertyNameChain);
            }
            catch (InvalidOperationException ex)
            {
                string exceptionMessage = string.Format(CultureInfo.InvariantCulture,
                    "'{0}' could not be parsed. ", propertyName);

                // We throw a more expressive exception at this level.
                throw new ArgumentException(exceptionMessage + ex.Message, "propertyName");
            }
        }

        private static List<MethodInfo> GetPropertyAccessorsFromPropertyNameChain(
            string propertyNameChain)
        {
            var propertyAccessors = new List<MethodInfo>();

            var declaringTypeForProperty = typeof(TEntity);

            var propertyNames = propertyNameChain.Split('.');

            foreach (string propertyName in propertyNames)
            {
                MethodInfo propertyAccessor = GetPropertyAccessor(declaringTypeForProperty, propertyName);

                propertyAccessors.Add(propertyAccessor);

                declaringTypeForProperty = propertyAccessor.ReturnType;
            }

            return propertyAccessors;
        }

        // Throws an InvalidOperationException when property with name does not exist or doens't have a getter.
        private static MethodInfo GetPropertyAccessor(Type declaringType, string propertyName)
        {
            PropertyInfo property = GetPropertyByName(declaringType, propertyName);

            MethodInfo propertyAccessor = GetPropertyGetter(property, declaringType);

            return propertyAccessor;
        }

        private static PropertyInfo GetPropertyByName(Type declaringType, string propertyName)
        {
            const BindingFlags Flags = BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public;

            PropertyInfo property = declaringType.GetProperty(propertyName, Flags);

            if (property == null)
            {
                string exceptionMessage = string.Format(CultureInfo.InvariantCulture,
                    "{0} does not contain a property named '{1}'.", declaringType, propertyName);

                throw new InvalidOperationException(exceptionMessage);
            }

            return property;
        }

        private static MethodInfo GetPropertyGetter(PropertyInfo propertyInfo, Type declaringType)
        {
            var propertyAccessor = propertyInfo.GetGetMethod();

            if (propertyAccessor == null)
            {
                string exceptionMessage = string.Format(
                    CultureInfo.InvariantCulture,
                    "The property '{1}' of type {0} does not contain a public getter.",
                    declaringType, propertyInfo.Name);

                throw new InvalidOperationException(exceptionMessage);
            }

            return propertyAccessor;
        }

        /// <summary>
        /// Concrete (generic) implementation of the LambdaBuilder class to allow easy creation of
        /// Expression{Func{TEntity, TKey}} objects.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        private sealed class GenericLambdaBuilder<TKey> : ILambdaBuilder
        {
            /// <summary>Builds the lambda from the supplied list of property accessors.</summary>
            /// <param name="propertyAccessors">The property accessors.</param>
            /// <returns>A new <see cref="LambdaExpression"/>.</returns>
            public LambdaExpression BuildLambda(IEnumerable<MethodInfo> propertyAccessors)
            {
                ParameterExpression parameterExpression = Expression.Parameter(typeof(TEntity), "entity");

                Expression propertyExpression = BuildPropertyExpression(propertyAccessors, parameterExpression);

                return Expression.Lambda<Func<TEntity, TKey>>(propertyExpression, new[] { parameterExpression });
            }

            private static Expression BuildPropertyExpression(IEnumerable<MethodInfo> propertyAccessors,
                ParameterExpression parameterExpression)
            {
                Expression propertyExpression = null;

                foreach (var propertyAccessor in propertyAccessors)
                {
                    var innerExpression = propertyExpression ?? parameterExpression;

                    propertyExpression = Expression.Property(innerExpression, propertyAccessor);
                }

                return propertyExpression;
            }
        }
    }

    /// <summary>Defines an EntitySorter for the OrderBy clause.</summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    [DebuggerDisplay("EntitySorter ( orderby {ToString()})")]
    internal class OrderByEntitySorter<TEntity, TKey> : EntitySorterBase<TEntity>
    {
        private readonly Expression<Func<TEntity, TKey>> keySelector;
        private readonly SortDirection direction;

        /// <summary>Initializes a new instance of the <see cref="OrderByEntitySorter{TEntity, TKey}"/> class.</summary>
        /// <param name="keySelector">The key selector.</param>
        /// <param name="direction">The direction.</param>
        public OrderByEntitySorter(Expression<Func<TEntity, TKey>> keySelector, SortDirection direction)
        {
            this.keySelector = keySelector;
            this.direction = direction;
        }

        public override IOrderedQueryable<TEntity> Sort(IQueryable<TEntity> collection)
        {
            if (collection == null) throw new ArgumentNullException("collection");
            if (direction == SortDirection.Ascending)
            {
                return Queryable.OrderBy(collection, keySelector);
            }
            else
            {
                return Queryable.OrderByDescending(collection, keySelector);
            }
        }

        public override System.Linq.IOrderedQueryable<TEntity> SortDescending(System.Linq.IQueryable<TEntity> collection)
        {
            if (collection == null) throw new ArgumentNullException("collection");
            if (direction == SortDirection.Ascending)
            {
                return Queryable.OrderByDescending(collection, keySelector);
            }
            else
            {
                return Queryable.OrderBy(collection, keySelector);
            }
        }

        /// <summary>Returns a String that represents the current object.</summary>
        /// <returns>A string representing the object.</returns>
        public override string ToString()
        {
            string sortType = this.direction == SortDirection.Ascending ? string.Empty : " descending";

            return this.keySelector.ToString() + sortType;
        }
    }

    /// <summary>Defines an EntitySorter for the ThenBy clause.</summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    [DebuggerDisplay("EntitySorter ( OrderBy: {ToString()})")]
    internal sealed class ThenByEntitySorter<TEntity, TKey> : EntitySorterBase<TEntity>
    {
        private readonly IEntitySorter<TEntity> baseSorter;
        private readonly Expression<Func<TEntity, TKey>> keySelector;
        private readonly SortDirection direction;

        /// <summary>
        /// Initializes a new instance of the <see cref="ThenByEntitySorter{TEntity, TKey}"/> class.
        /// </summary>
        /// <param name="baseSorter">The base sorter.</param>
        /// <param name="keySelector">The key selector.</param>
        /// <param name="direction">The direction.</param>
        public ThenByEntitySorter(IEntitySorter<TEntity> baseSorter,
            Expression<Func<TEntity, TKey>> keySelector, SortDirection direction)
        {
            this.baseSorter = baseSorter;
            this.keySelector = keySelector;
            this.direction = direction;
        }

        public Expression<Func<TEntity, TKey>> KeySelector { get { return keySelector; } }
        public SortDirection Direction { get { return direction; } }

        public override IOrderedQueryable<TEntity> Sort(IQueryable<TEntity> collection)
        {
            var sortedCollection = baseSorter.Sort(collection);
            if (direction == SortDirection.Ascending)
            {
                return Queryable.ThenBy(sortedCollection, keySelector);
            }
            else
            {
                return Queryable.ThenByDescending(sortedCollection, keySelector);
            }
        }

        public override System.Linq.IOrderedQueryable<TEntity> SortDescending(System.Linq.IQueryable<TEntity> collection)
        {
            var sortedCollection = baseSorter.SortDescending(collection);
            if (direction == SortDirection.Ascending)
            {
                return Queryable.ThenByDescending(sortedCollection, keySelector);
            }
            else
            {
                return Queryable.ThenBy(sortedCollection, keySelector);
            }
        }

        /// <summary>Returns a String that represents the current object.</summary>
        /// <returns>A string representing the object.</returns>
        public override string ToString()
        {
            string sortType = this.direction == SortDirection.Ascending ? string.Empty : " descending";

            return this.baseSorter.ToString() + ", " + this.keySelector.ToString() + sortType;
        }
    }

    internal enum SortDirection
    {
        Ascending,
        Descending
    }
}