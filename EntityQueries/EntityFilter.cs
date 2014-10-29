// Everything in this file is based on the CuttingEdge.EntitySorting library (http://servicelayerhelpers.codeplex.com/).

using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

namespace EntyTea.EntityQueries
{
    /// <summary>
    /// Specifies a method that filters a collection by returning a filtered collection.
    /// </summary>
    /// <typeparam name="TEntity">The element type of the collection to filter.</typeparam>
    public interface IEntityFilter<TEntity> : IEntityFilter
    {
        /// <summary>Filters the specified collection.</summary>
        /// <param name="collection">The collection.</param>
        /// <returns>A filtered collection.</returns>
        IQueryable<TEntity> Filter(IQueryable<TEntity> collection);
    }

    /// <summary>
    /// Specifies a method that filters a collection by returning a filtered collection, where the type of the collection is not known.
    /// </summary>
    public interface IEntityFilter
    {
        /// <summary>Filters the specified collection.</summary>
        /// <param name="collection">The collection.</param>
        /// <returns>A filtered collection.</returns>
        IQueryable Filter(IQueryable collection);
    }

    /// <summary>
    /// A base implementation of <c>IEntityFilter{TEntity}</c>.
    /// </summary>
    /// <typeparam name="TEntity">The element type of the collection to filter.</typeparam>
    public abstract class EntityFilterBase<TEntity> : IEntityFilter<TEntity>
    {
        public abstract IQueryable<TEntity> Filter(IQueryable<TEntity> collection);

        #region IEntityFilter

        IQueryable IEntityFilter.Filter(IQueryable collection)
        {
            return Filter(collection.OfType<TEntity>());
        }

        #endregion
    }

    /// <summary>Enables filtering of entities.</summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public static class EntityFilter<TEntity>
    {
        /// <summary>
        /// Returns a <see cref="IEntityFilter{TEntity}"/> instance that allows construction of
        /// <see cref="IEntityFilter{TEntity}"/> objects though the use of LINQ syntax.
        /// </summary>
        /// <returns>A <see cref="IEntityFilter{TEntity}"/> instance.</returns>
        public static IEntityFilter<TEntity> AsQueryable()
        {
            return new EmptyEntityFilter();
        }

        /// <summary>
        /// Returns a <see cref="IEntityFilter{TEntity}"/> that filters a sequence based on a predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>A new <see cref="IEntityFilter{TEntity}"/>.</returns>
        public static IEntityFilter<TEntity> Where(Expression<Func<TEntity, bool>> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }

            return new WhereEntityFilter<TEntity>(predicate);
        }

        /// <summary>An empty entity filter.</summary>
        [DebuggerDisplay("EntityFilter ( Unfiltered )")]
        private sealed class EmptyEntityFilter : EntityFilterBase<TEntity>
        {
            /// <summary>Filters the specified collection.</summary>
            /// <param name="collection">The collection.</param>
            /// <returns>A filtered collection.</returns>
            public override IQueryable<TEntity> Filter(IQueryable<TEntity> collection)
            {
                // We don't filter, but simply return the collection.
                return collection;
            }

            /// <summary>Returns an empty string.</summary>
            /// <returns>An empty string.</returns>
            public override string ToString()
            {
                return string.Empty;
            }
        }
    }

    /// <summary>
    /// Extension methods for the <see cref="IEntityFilter{TEntity}"/> interface.
    /// </summary>
    public static class EntityFilterExtensions
    {
        /// <summary>
        /// Returns a <see cref="IEntityFilter{TEntity}"/> that filters a sequence based on a predicate.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="baseFilter">The base filter.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>A new <see cref="IEntityFilter{TEntity}"/>.</returns>
        public static IEntityFilter<TEntity> Where<TEntity>(this IEntityFilter<TEntity> baseFilter,
            Expression<Func<TEntity, bool>> predicate)
        {
            if (baseFilter == null)
            {
                throw new ArgumentNullException("baseFilter");
            }

            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }

            return new WhereEntityFilter<TEntity>(baseFilter, predicate);
        }
    }

    /// <summary>
    /// Filters the collection using a predicate.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    [DebuggerDisplay("EntityFilter ( where {ToString()} )")]
    public sealed class WhereEntityFilter<TEntity> : EntityFilterBase<TEntity>
    {
        private readonly IEntityFilter<TEntity> baseFilter;
        private readonly Expression<Func<TEntity, bool>> predicate;

        /// <summary>Initializes a new instance of the <see cref="WhereEntityFilter{TEntity}"/> class.</summary>
        /// <param name="predicate">The predicate.</param>
        public WhereEntityFilter(Expression<Func<TEntity, bool>> predicate)
        {
            this.predicate = predicate;
        }

        /// <summary>Initializes a new instance of the <see cref="WhereEntityFilter{TEntity}"/> class.</summary>
        /// <param name="baseFilter">The base filter.</param>
        /// <param name="predicate">The predicate.</param>
        public WhereEntityFilter(IEntityFilter<TEntity> baseFilter, Expression<Func<TEntity, bool>> predicate)
        {
            this.baseFilter = baseFilter;
            this.predicate = predicate;
        }

        /// <summary>
        /// The base filter (if any).
        /// </summary>
        public IEntityFilter<TEntity> BaseFilter { get { return baseFilter; } }

        /// <summary>
        /// The predicate of this filter.
        /// </summary>
        public Expression<Func<TEntity, bool>> Predicate { get { return predicate; } }

        /// <summary>Filters the specified collection.</summary>
        /// <param name="collection">The collection.</param>
        /// <returns>A filtered collection.</returns>
        public override IQueryable<TEntity> Filter(IQueryable<TEntity> collection)
        {
            if (this.baseFilter == null)
            {
                return collection.Where(this.predicate);
            }
            else
            {
                return this.baseFilter.Filter(collection).Where(this.predicate);
            }
        }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            string baseFilterPresentation =
                this.baseFilter != null ? this.baseFilter.ToString() : string.Empty;

            // The returned string is used in de DebuggerDisplay.
            if (!string.IsNullOrEmpty(baseFilterPresentation))
            {
                return baseFilterPresentation + ", " + this.predicate.ToString();
            }
            else
            {
                return this.predicate.ToString();
            }
        }
    }
}