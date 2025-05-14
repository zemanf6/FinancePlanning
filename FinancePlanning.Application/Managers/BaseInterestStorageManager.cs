using AutoMapper;
using FinancePlanning.Domain.Entities;
using FinancePlanning.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace FinancePlanning.Application.Managers
{
    public abstract class BaseInterestStorageManager<TDto, TEntity, TRepository>
        where TDto : class
        where TEntity : class, IUserOwnedEntity
        where TRepository : IBaseRepository<TEntity>, IUserScopedRepository<TEntity>
    {
        protected readonly TRepository _repository;
        protected readonly UserManager<ApplicationUser> _userManager;
        protected readonly IMapper _mapper;

        protected BaseInterestStorageManager(
            TRepository repository,
            UserManager<ApplicationUser> userManager,
            IMapper mapper)
        {
            _repository = repository;
            _userManager = userManager;
            _mapper = mapper;
        }

        protected string? GetUserId(ClaimsPrincipal user) => _userManager.GetUserId(user);

        public async Task SaveCalculationAsync(TDto dto, ClaimsPrincipal user)
        {
            var userId = GetUserId(user);
            if (string.IsNullOrEmpty(userId))
                throw new InvalidOperationException("User is not authenticated.");

            var entity = _mapper.Map<TEntity>(dto);
            entity.UserId = userId;

            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
        }

        public async Task<IEnumerable<TEntity>> GetSavedCalculationsAsync(ClaimsPrincipal user)
        {
            var userId = GetUserId(user);
            if (string.IsNullOrEmpty(userId))
                return Enumerable.Empty<TEntity>();

            return await _repository.GetByUserIdAsync(userId);
        }

        public async Task<TDto?> LoadDtoByIdAsync(int id, ClaimsPrincipal user)
        {
            var userId = GetUserId(user);
            if (string.IsNullOrEmpty(userId))
                return null;

            var entity = await _repository.GetByIdAsync(id);
            if (entity == null || entity.UserId != userId)
                return null;

            return _mapper.Map<TDto>(entity);
        }

        public async Task DeleteByIdAsync(int id, ClaimsPrincipal user)
        {
            var userId = GetUserId(user);
            if (string.IsNullOrEmpty(userId))
                return;

            var entity = await _repository.GetByIdAsync(id);
            if (entity != null && entity.UserId == userId)
            {
                _repository.Remove(entity);
                await _repository.SaveChangesAsync();
            }
        }

        public async Task DeleteAllAsync(ClaimsPrincipal user)
        {
            var userId = GetUserId(user);
            if (string.IsNullOrEmpty(userId))
                return;

            await _repository.DeleteAllByUserIdAsync(userId);
            await _repository.SaveChangesAsync();
        }
    }
}
