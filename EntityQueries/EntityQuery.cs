using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace EntyTea.EntityQueries
{
    /// <summary>
    /// A query for entities.
    /// </summary>
    /// <typeparam name="T">the type of entity to query</typeparam>
    public interface IEntityQuery<T> : IEntityQuery
    {
        /// <summary>
        /// Applies the query to the specified queryable data source.
        /// </summary>
        /// <param name="queryable">the data source on which to apply the query</param>
        /// <returns>the query results</returns>
        IQueryable<T> Apply(IQueryable<T> queryable);

        /// <summary>
        /// Applies the filter part of the query to the specified queryable data source.  
        /// This should not necessarily order the results, although it could in cases such as when applying a skip/take filter.
        /// </summary>
        /// <param name="queryable">the data source on which to apply the filter</param>
        /// <returns>the filtered query results</returns>
        IQueryable<T> ApplyFilter(IQueryable<T> queryable);

        /// <summary>
        /// Applies the sort part of the query to the specified queryable data source.
        /// This should not apply any filter to the results.
        /// </summary>
        /// <param name="queryable">the data source to sort</param>
        /// <returns>the sorted query results</returns>
        IQueryable<T> ApplySort(IQueryable<T> queryable);
    }

    /// <summary>
    /// A query for entities where the entity type is not known at compile time.
    /// </summary>
    public interface IEntityQuery
    {
        /// <summary>
        /// The type of entity to query, or null if the type is not known.
        /// </summary>
        Type EntityType { get; }

        /// <summary>
        /// Applies the query to the specified queryable data source.
        /// </summary>
        /// <param name="queryable">the data source on which to apply the query</param>
        /// <returns>the query results</returns>
        IQueryable Apply(IQueryable queryable);

        /// <summary>
        /// Applies the filter part of the query to the specified queryable data source.  
        /// This should not necessarily order the results, although it could in cases such as when applying a skip/take filter.
        /// </summary>
        /// <param name="queryable">the data source on which to apply the filter</param>
        /// <returns>the filtered query results</returns>
        IQueryable ApplyFilter(IQueryable queryable);

        /// <summary>
        /// Applies the sort part of the query to the specified queryable data source.
        /// This should not apply any filter to the results.
        /// </summary>
        /// <param name="queryable">the data source to sort</param>
        /// <returns>the sorted query results</returns>
        IQueryable ApplySort(IQueryable queryable);
    }

    /// <summary>
    /// The base class for an entity query with a known entity type.
    /// </summary>
    /// <typeparam name="T">the type of entity to query</typeparam>
    public abstract class EntityQueryBase<T> : IEntityQuery<T>
    {
        public virtual IQueryable<T> Apply(IQueryable<T> queryable)
        {
            return ApplySort(ApplyFilter(queryable));
        }

        public abstract IQueryable<T> ApplyFilter(IQueryable<T> queryable);

        public abstract IQueryable<T> ApplySort(IQueryable<T> queryable);

        #region Non-generic

        Type IEntityQuery.EntityType
        {
            get { return typeof(T); }
        }

        IQueryable IEntityQuery.Apply(IQueryable queryable)
        {
            return Apply(queryable.Cast<T>());
        }

        IQueryable IEntityQuery.ApplyFilter(IQueryable queryable)
        {
            return ApplyFilter(queryable.Cast<T>());
        }

        IQueryable IEntityQuery.ApplySort(IQueryable queryable)
        {
            return ApplySort(queryable.Cast<T>());
        }

        #endregion
    }

    /// <summary>
    /// A default implementation of the <c>IEntityQuery</c> interface.
    /// </summary>
    /// <typeparam name="T">the type of entity to query</typeparam>
    public class EntityQuery<T> : EntityQueryBase<T>
    {
        /// <summary>
        /// Constructs a query with the specified optional parts.
        /// </summary>
        /// <param name="filter">a filter to apply on the queried entities</param>
        /// <param name="sorter">a sorter to apply after any filter</param>
        /// <param name="skip">the number of entities to skip from the start of the query results; this is applied after any provided sorter</param>
        /// <param name="take">the maximum number of entities to take from the query results; this is applied after any provided sorter and skipped entities</param>
        public EntityQuery(IEntityFilter<T> filter = null, IEntitySorter<T> sorter = null, int skip = 0, int? take = null)
        {
            this.Filter = filter;
            this.Sorter = sorter;
            this.Skip = skip;
            this.Take = take;
        }

        /// <summary>
        /// Constructs a query with the specified filter predicate and other optional 
        /// </summary>
        /// <param name="predicate">a filter predicate to apply on the queried entities</param>
        /// <param name="sorter">a sorter to apply after any filter</param>
        /// <param name="skip">the number of entities to skip from the start of the query results; this is applied after any provided sorter</param>
        /// <param name="take">the maximum number of entities to take from the query results; this is applied after any provided sorter and skipped entities</param>
        public EntityQuery(Expression<Func<T, bool>> predicate, IEntitySorter<T> sorter = null, int skip = 0, int? take = null)
            : this(EntityFilter<T>.Where(predicate), sorter, skip, take) { }

        /// <summary>
        /// The filter to apply, if any.
        /// </summary>
        public IEntityFilter<T> Filter { get; set; }

        /// <summary>
        /// The sorter to apply, if any.
        /// </summary>
        public IEntitySorter<T> Sorter { get; set; }

        /// <summary>
        /// The number of elements to skip, or zero if none should be skipped.
        /// </summary>
        public int Skip { get; set; }

        /// <summary>
        /// The maximum number of elements to include in the query results, if any.
        /// </summary>
        public int? Take { get; set; }

        public override IQueryable<T> Apply(IQueryable<T> queryable)
        {
            var filter = Filter;
            if (filter != null)
            {
                queryable = filter.Filter(queryable);
            }

            var sorter = Sorter;
            if (sorter != null)
            {
                queryable = sorter.Sort(queryable);
            }

            var skip = Skip;
            if (skip > 0)
            {
                queryable = queryable.Skip(Skip);
            }

            var take = Take;
            if (take.HasValue)
            {
                queryable = queryable.Take(take.Value);
            }
            return queryable;
        }

        public override IQueryable<T> ApplyFilter(IQueryable<T> queryable)
        {
            var filter = Filter;
            if (filter != null)
            {
                queryable = filter.Filter(queryable);
            }

            var take = Take;
            var skip = Skip;
            if (take.HasValue || skip != 0)
            {
                // It's important that we apply any sorter if we have a skip and/or take value,
                // since that does affect the results (and the skip might actually fail if there is no sorter).
                var sorter = Sorter;
                if (sorter != null)
                {
                    queryable = sorter.Sort(queryable);
                }
                if (skip > 0)
                {
                    queryable = queryable.Skip(skip);
                }
                if (take.HasValue)
                {
                    queryable = queryable.Take(take.Value);
                }
            }

            return queryable;
        }

        public override IQueryable<T> ApplySort(IQueryable<T> queryable)
        {
            var sorter = Sorter;
            if (Sorter != null)
            {
                queryable = sorter.Sort(queryable);
            }
            return queryable;
        }
    }

    /// <summary>
    /// An entity query that is the intersection of the results of some number of inner queries.
    /// </summary>
    /// <typeparam name="T">the type of entity to query</typeparam>
    public class AggregateEntityQuery<T> : EntityQueryBase<T>
    {
        private readonly ICollection<IEntityQuery<T>> innerQueries;

        /// <summary>
        /// Constructs a query that returns the intersection of the specified queries.
        /// </summary>
        /// <param name="queries">the queries to aggregate</param>
        public AggregateEntityQuery(params IEntityQuery<T>[] queries)
            : this(queries.AsEnumerable()) { }

        /// <summary>
        /// Constructs a query that returns the intersection of the specified queries.
        /// </summary>
        /// <param name="queries">the queries to aggregate</param>
        public AggregateEntityQuery(IEnumerable<IEntityQuery<T>> queries)
        {
            this.innerQueries = queries.Where(q => q != null).ToList().AsReadOnly();
        }

        /// <summary>
        /// The queries that are included in this aggregate query.
        /// </summary>
        public ICollection<IEntityQuery<T>> InnerQueries
        {
            get { return innerQueries; }
        }

        public override IQueryable<T> Apply(IQueryable<T> queryable)
        {
            return innerQueries.Aggregate(queryable, (current, query) => query.Apply(current));
        }

        public override IQueryable<T> ApplyFilter(IQueryable<T> queryable)
        {
            return innerQueries.Aggregate(queryable, (current, query) => query.ApplyFilter(current));
        }

        public override IQueryable<T> ApplySort(IQueryable<T> queryable)
        {
            return innerQueries.Aggregate(queryable, (current, query) => query.ApplySort(current));
        }
    }

    /// <summary>
    /// A query that unions together the results of some number of inner queries.
    /// </summary>
    /// <remarks>
    /// This query does not apply any sort to the unioned results.
    /// </remarks>
    /// <typeparam name="T">the type of entity to query</typeparam>
    public class UnionEntityQuery<T> : EntityQueryBase<T>
    {
        private readonly ICollection<IEntityQuery<T>> innerQueries;

        public UnionEntityQuery(params IEntityQuery<T>[] queries)
            : this(queries.AsEnumerable()) { }

        public UnionEntityQuery(IEnumerable<IEntityQuery<T>> queries)
        {
            if (queries == null) throw new ArgumentNullException("queries");
            this.innerQueries = queries.ToList().AsReadOnly();
        }

        public override IQueryable<T> ApplyFilter(IQueryable<T> queryable)
        {
            if (innerQueries == null || !innerQueries.Any()) return queryable;

            var seed = innerQueries.First().ApplyFilter(queryable);
            return innerQueries.Skip(1).Aggregate(seed, (current, query) => current.Union(query.ApplyFilter(queryable)));
        }

        public override IQueryable<T> ApplySort(IQueryable<T> queryable)
        {
            return queryable;
        }
    }
}
