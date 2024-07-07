using ControleAcesso.Domain.Enumerations;
using ControleAcesso.Domain.Interfaces.Entities;
using ControleAcesso.Domain.Interfaces.Repositories;
using ControleAcesso.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace ControleAcesso.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class, IEntity
    {
        protected readonly AppDbContext _context;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
        }

        public T Create(T entity)
        {
            _context.Set<T>().Add(entity);
            _context.SaveChanges();
            return entity;
        }

        public async Task<T> CreateAsync(T entity, NavigationLevel navigationLevel = NavigationLevel.None)
        {
            _context.Set<T>().Add(entity);
            await _context.SaveChangesAsync();

            await LoadNavigationPropertiesAsync(entity, navigationLevel);

            return entity;
        }

        public T Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
            _context.SaveChanges();
            return entity;
        }

        /*public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }*/

        public async Task<IEnumerable<T>> GetAllAsync(NavigationLevel navigationLevel = NavigationLevel.None)
        {
            IQueryable<T> query = _context.Set<T>();
            query = GetNavigationLevel(navigationLevel, query);
            return await query.ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate,NavigationLevel navigationLevel = NavigationLevel.None)
        {
            IQueryable<T> query = _context.Set<T>();
            query = GetNavigationLevel(navigationLevel, query);
            return await query.Where(predicate).ToListAsync();
        }

        /*public async Task<IEnumerable<T>> GetAllAsync(
            Expression<Func<T, bool>> predicate,
             Func<IQueryable<T>, IIncludableQueryable<T, object>> include)
        {
            IQueryable<T> query = _context.Set<T>();
            query = query.Where(predicate);
            query = include(query);
            return await query.ToListAsync();
        }*/
        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IQueryable<T>> include)
        {
            IQueryable<T> query = _context.Set<T>();
            query = query.Where(predicate);
            query = include(query);
            return await query.ToListAsync();
        }

        public async Task<T?> GetAsync(Expression<Func<T, bool>> predicate, NavigationLevel navigationLevel = NavigationLevel.None)
        {
            IQueryable<T> query = _context.Set<T>();
            query = GetNavigationLevel(navigationLevel, query);
            return await query.FirstOrDefaultAsync(predicate);
        }

        public T Update(T entity)
        {
            _context.Set<T>().Update(entity);
            _context.SaveChanges();
            return entity;
        }

        public async Task<T> UpdateAsync(T entity)
        {
            _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        private async Task LoadNavigationPropertiesAsync(T entity, NavigationLevel navigationLevel)
        {
            var entityType = _context.Model.FindEntityType(typeof(T));
            var firstLevelNavigations = entityType.GetNavigations();

            if (navigationLevel >= NavigationLevel.FirstLevel)
            {
                foreach (var firstLevelNavigation in firstLevelNavigations)
                {
                    var firstLevelEntry = _context.Entry(entity).Navigation(firstLevelNavigation.Name);
                    await firstLevelEntry.LoadAsync();

                    if (navigationLevel >= NavigationLevel.SecondLevel)
                    {
                        var firstLevelEntityType = firstLevelNavigation.TargetEntityType;
                        var secondLevelNavigations = firstLevelEntityType.GetNavigations();

                        foreach (var secondLevelNavigation in secondLevelNavigations)
                        {
                            var secondLevelEntries = firstLevelEntry.CurrentValue as IEnumerable<object>;
                            if (secondLevelEntries != null)
                            {
                                foreach (var secondLevelEntry in secondLevelEntries)
                                {
                                    var secondLevelNav = _context.Entry(secondLevelEntry).Navigation(secondLevelNavigation.Name);
                                    await secondLevelNav.LoadAsync();
                                }
                            }
                            else
                            {
                                var secondLevelEntry = _context.Entry(firstLevelEntry.CurrentValue).Navigation(secondLevelNavigation.Name);
                                await secondLevelEntry.LoadAsync();
                            }

                            if (navigationLevel >= NavigationLevel.ThirdLevel)
                            {
                                var secondLevelEntityType = secondLevelNavigation.TargetEntityType;
                                var thirdLevelNavigations = secondLevelEntityType.GetNavigations();

                                foreach (var thirdLevelNavigation in thirdLevelNavigations)
                                {
                                    var thirdLevelEntries = secondLevelEntries ?? new[] { firstLevelEntry.CurrentValue };
                                    foreach (var thirdLevelEntry in thirdLevelEntries)
                                    {
                                        var thirdLevelNav = _context.Entry(thirdLevelEntry).Navigation(thirdLevelNavigation.Name);
                                        await thirdLevelNav.LoadAsync();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        private IQueryable<T>? GetNavigationLevel(NavigationLevel navigationLevel, IQueryable<T> query)
        {
            if (navigationLevel >= NavigationLevel.FirstLevel)
            {
                var entityType = _context.Model.FindEntityType(typeof(T));
                var firstLevelNavigations = entityType.GetNavigations();
                foreach (var firstLevelNavigation in firstLevelNavigations)
                {
                    var firstLevelQuery = query.Include(firstLevelNavigation.Name);

                    if (navigationLevel >= NavigationLevel.SecondLevel)
                    {
                        var firstLevelEntityType = firstLevelNavigation.TargetEntityType;
                        var secondLevelNavigations = firstLevelEntityType.GetNavigations();

                        foreach (var secondLevelNavigation in secondLevelNavigations)
                        {
                            if (secondLevelNavigation.TargetEntityType == entityType) continue; // Avoid cycles

                            var secondLevelQuery = firstLevelQuery.Include($"{firstLevelNavigation.Name}.{secondLevelNavigation.Name}");

                            if (navigationLevel >= NavigationLevel.ThirdLevel)
                            {
                                var secondLevelEntityType = secondLevelNavigation.TargetEntityType;
                                var thirdLevelNavigations = secondLevelEntityType.GetNavigations();

                                foreach (var thirdLevelNavigation in thirdLevelNavigations)
                                {
                                    if (thirdLevelNavigation.TargetEntityType == firstLevelEntityType || thirdLevelNavigation.TargetEntityType == entityType) continue; // Avoid cycles

                                    secondLevelQuery = secondLevelQuery.Include($"{firstLevelNavigation.Name}.{secondLevelNavigation.Name}.{thirdLevelNavigation.Name}");
                                }
                            }

                            firstLevelQuery = secondLevelQuery;
                        }
                    }

                    query = firstLevelQuery;
                }
            }
            return query;

        }
    }
}
