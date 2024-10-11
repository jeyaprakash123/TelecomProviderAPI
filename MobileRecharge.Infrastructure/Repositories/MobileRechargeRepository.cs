﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MobileRecharge.Domain.Configuration;
using System.Text;
using TelecomProviderAPI.Core.IRepository;
using TopUpAPI.DataAccess;
using TopUpAPI.Models;

namespace TelecomProviderAPI.Infrastructure.Repositories
{
    public class MobileRechargeRepository:IMobileRechargeRepository
    {
        private readonly TopUpDbContext _context;
        private readonly Appsettings _appSettings;
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;
        private readonly decimal charge;
        private readonly IMapper _mapper;

        public MobileRechargeRepository(TopUpDbContext context, IHttpClientFactory httpClientFactory, IOptions<Appsettings> appSettings, IConfiguration config, IMapper mapper)
        {
            _context = context;
            _httpClient = httpClientFactory.CreateClient("PaymentApi");
            _config = config;
            _appSettings = appSettings.Value;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TopUpOption>> GetTopUpOptions()
        {
            return await _context.TopUpOptions.ToListAsync();
        }

        public async Task TopUpBeneficiary(int userId, int beneficiaryId, decimal amount)
        {
            var user = await _context.Users.AsNoTracking().Include(u => u.Beneficiaries).ThenInclude(b => b.BeneficiaryTopUp)
                .FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) throw new Exception("User not found.");

            var beneficiary = user.Beneficiaries.FirstOrDefault(b => b.Id == beneficiaryId);
            if (beneficiary == null) throw new Exception("Beneficiary not found.");

            var totalTopUpsThisMonth = beneficiary.BeneficiaryTopUp
                                 .Where(x => x.BeneficiaryId == beneficiaryId
                                    && x.MonthWise == DateTime.Now.Month
                                    && x.YearWise == DateTime.Now.Year)
                                 .Sum(x => x.Amount);

            var userTotalTopUpsThisMonth = CheckUserMonthlyLimit(userId);

            await ValidatePlan(amount);

            if (!UserTopUpLimitPerMonth(beneficiaryId, amount, totalTopUpsThisMonth))
                throw new Exception("User top-up Limit exceed for this month...Please wait until next month");

            if (totalTopUpsThisMonth + amount > beneficiary.MonthlyTopUpLimit)
                throw new Exception("Monthly top-up limit exceeded for this beneficiary.");

            // Update user's total top-up amount
            userTotalTopUpsThisMonth += amount;

            if (userTotalTopUpsThisMonth > user.TotalTopUpLimit)
                throw new Exception("Monthly top-up limit exceeded for all beneficiaries.");
        }

        public async Task ValidateUserBalance(decimal balance,decimal amount)
        {
            if (balance < amount)
                throw new Exception("Insufficient balance.");
        }
        public async Task ValidatePlan(decimal amount)
        {
            bool isAvail = _context.TopUpOptions.AsNoTracking().Any(t => t.Amount == amount);
            if (!isAvail) throw new Exception("TopUp Plan is Invalid...");
        }
        public async Task UpdateTransaction(int userId,int beneficiaryId,decimal amount)
        {
            var _user = await _context.Users.FindAsync(userId);
            if (_user == null)
            {
                throw new Exception("Failed to update user balance...");
            }
            _user.AvailableBalance = await GetUserBalance(userId);
            var beneficiaryTopUp = new BeneficiaryTopUpDetails
            {
                UserId = userId,
                BeneficiaryId = beneficiaryId,
                Amount = amount,
                Charge = _appSettings.ChargeFee,
                MonthWise = DateTime.Now.Month,
                YearWise = DateTime.Now.Year
            };

            _context.BeneficiaryTopUpDetails.Add(beneficiaryTopUp);
        }
        public async Task DoPayment(int userId,decimal amount)
        {
            decimal TotalAmount = amount + _appSettings.ChargeFee;

            // POST request to deduct the balance
            var deductContent = new StringContent(amount.ToString(), Encoding.UTF8, "application/json");
            var deductResponse = await _httpClient.PutAsync($"update-user-balance?userid={userId}&amount={TotalAmount}", deductContent);

            if (!deductResponse.IsSuccessStatusCode)
            {
                throw new Exception("Failed to deduct balance from balance service.");
            }
        }
        public async Task<decimal> GetUserBalance(int userId)
        {
            var response = await _httpClient.GetAsync($"get-user-balance?userId={userId}");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to get balance from balance service.");
            }
            var balanceContent = await response.Content.ReadAsStringAsync();
            var balance=decimal.Parse(balanceContent);
            return balance;
        }
        public bool UserTopUpLimitPerMonth(int beneficiaryId, decimal amount, decimal totalTopUpsThisMonth)
        {
            var beneficiary = _context.Beneficiaries.AsNoTracking().FirstOrDefaultAsync(u => u.Id == beneficiaryId);
            if (totalTopUpsThisMonth + amount > beneficiary.Result.MonthlyTopUpLimit)
            {
                return false;
            }
            return true; 
        }
        public decimal CheckUserMonthlyLimit(int userId)
        {
            return _context.BeneficiaryTopUpDetails
                                .Where(x => x.UserId == userId
                                   && x.MonthWise == DateTime.Now.Month
                                   && x.YearWise == DateTime.Now.Year)
                                .Sum(x => x.Amount);
        }
    }
}
