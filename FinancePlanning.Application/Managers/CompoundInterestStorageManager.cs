using AutoMapper;
using FinancePlanning.Application.DTOs;
using FinancePlanning.Application.Interfaces;
using FinancePlanning.Domain.Entities;
using FinancePlanning.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FinancePlanning.Application.Managers
{
    public class CompoundInterestStorageManager : ICompoundInterestStorageManager
    {
        private readonly ICompoundInterestCalculationRepository _repository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public CompoundInterestStorageManager(
            ICompoundInterestCalculationRepository repository,
            UserManager<ApplicationUser> userManager,
            IMapper mapper)
        {
            _repository = repository;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task SaveCalculationAsync(CompoundInterestDto dto, ClaimsPrincipal user)
        {
            var userId = _userManager.GetUserId(user);
            if (string.IsNullOrEmpty(userId))
                throw new InvalidOperationException("User is not authenticated.");

            var entity = _mapper.Map<SavedCompoundInterest>(dto);
            entity.UserId = userId;

            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
        }

        public async Task<IEnumerable<SavedCompoundInterest>> GetSavedCalculationsAsync(ClaimsPrincipal user)
        {
            var userId = _userManager.GetUserId(user);
            if (string.IsNullOrEmpty(userId))
                return Enumerable.Empty<SavedCompoundInterest>();

            return await _repository.GetByUserIdAsync(userId);
        }

        public async Task<CompoundInterestDto?> LoadDtoByIdAsync(int id, ClaimsPrincipal user)
        {
            var userId = _userManager.GetUserId(user);
            if (string.IsNullOrEmpty(userId))
                return null;

            var entity = await _repository.GetByIdAsync(id);
            if (entity == null || entity.UserId != userId)
                return null;

            return _mapper.Map<CompoundInterestDto>(entity);
        }

        public async Task DeleteByIdAsync(int id, ClaimsPrincipal user)
        {
            var userId = _userManager.GetUserId(user);
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
            var userId = _userManager.GetUserId(user);
            if (string.IsNullOrEmpty(userId))
                return;

            await _repository.DeleteAllByUserIdAsync(userId);
            await _repository.SaveChangesAsync();
        }
    }
}
