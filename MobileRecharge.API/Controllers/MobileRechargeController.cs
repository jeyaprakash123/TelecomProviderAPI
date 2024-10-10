using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using TelecomProviderAPI.Application.Interfaces;

namespace TelecomProviderAPI.Controllers
{
    [Authorize]
    // Controllers/UserController.cs
    [Route("api/[controller]")]
    [ApiController]
    public class MobileRechargeController : Controller
    {
        private readonly IMobileRechargeService _rechargeService;
        public MobileRechargeController(IMobileRechargeService rechargeService)
        {
            _rechargeService = rechargeService; 
        }

        [HttpGet("topup-options")]
        public async Task<IActionResult> GetTopUpOptions()
        {
            try
            {
                var options = await _rechargeService.GetTopUpOptions();
                return Ok(options);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error fetching top-up options");
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpPost("top-up")]
        public async Task<IActionResult> TopUpBeneficiary(int userId, int beneficiaryId, decimal amount)
        {
            try
            {
                bool result = await _rechargeService.TopUpBeneficiary(userId, beneficiaryId, amount);
                return result ? Ok("Top-up successful") : BadRequest("Failed to complete top-up");
            }
            catch (ArgumentException ex)
            {
                Log.Error(ex, "Validation error during top-up");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error processing top-up");
                return StatusCode(500, ex.Message);
            }
        }
    }
}
