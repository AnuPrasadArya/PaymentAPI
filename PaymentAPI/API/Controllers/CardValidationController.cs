using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using PaymentAPI.Application.DTOs;
using PaymentAPI.Application.Interfaces;
using PaymentAPI.Helpers;
using PaymentAPI.Infrastructure.Data;

namespace PaymentAPI.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    // [ApiVersion("1.0")]
    // [ApiVersion("2.0")]
    public class CardValidationController : ControllerBase
    {
        private readonly ICardValidation _cardValidationService;
        private readonly IDistributedCache _cache;
        public CardValidationController(ICardValidation cardValidationService, IDistributedCache distributedCache)
        {
            _cardValidationService = cardValidationService;
            _cache = distributedCache;

        }
        [Authorize]
        [HttpPost("ValidateCard")]
      //  [MapToApiVersion("1.0")]
        public async Task<IActionResult> ValidateCard([FromBody] CardValidationRequest request)
        {
            var isValidCard = await _cardValidationService.ValidateCard(request.CardNumber!, request.CVV, request.ExpiryMonth, request.ExpiryYear);
            return Ok(new { valid = isValidCard });
        }
        [Authorize]
        [HttpPost("ValidateCardRedis")]
       // [MapToApiVersion("2.0")]
        public async Task<IActionResult> ValidateCardRedis([FromBody] CardValidationRequest request)
        {
            // Generate a cache key by hashing the card info (never store raw data!)
            string cacheKey = Helper.GenerateCacheKey(request);

            // Try to get from cache
            var cachedResult = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedResult))
            {
                bool cachedValid = bool.Parse(cachedResult);
                return Ok(new { valid = cachedValid, source = "cache" });
            }

            // If not cached, validate and cache the result
            var isValidCard = await _cardValidationService.ValidateCard(
                request.CardNumber!, request.CVV, request.ExpiryMonth, request.ExpiryYear);

            // Cache result for 10 minutes
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            };

            await _cache.SetStringAsync(cacheKey, isValidCard.ToString(), options);
            return Ok(new { valid = isValidCard, source = "validated" });
        }
    }

}
