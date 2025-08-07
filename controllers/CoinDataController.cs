using background_jobs.Services;
using Microsoft.AspNetCore.Mvc;
using background_jobs.models;

namespace background_jobs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoinDataController(ICoinDataService coinDataService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<CoinDataDto>>> GetCoinsAsync()
        {
            return Ok(await coinDataService.GetCoinsAsync());
        }

        [HttpPost("create-coin")]
        public async Task<ActionResult<CoinDataDto>> CreateCoinAsync([FromBody] CreateCoinDto createCoinDto)
        {
            try
            {
                return Ok(await coinDataService.CreateCoinAsync(createCoinDto));
            }
            catch (Exception e)
            {

                return BadRequest(new { message = e.Message });
            }
        }

        [HttpPost("convert-coin-to-fiat")]
        public async Task<ActionResult<ConversionFiatResponseDto>> ConvertCoinToFiatCurrencyAsync([FromBody] ConvertCoinDto convertCoinDto)
        {
            try
            {
                return Ok(await coinDataService.ConvertCoinToFiatCurrencyAsync(convertCoinDto));
            }
            catch (ArgumentException e)
            {

                return BadRequest(new { message = e.Message });
            }
            catch (Exception e)
            {

                return BadRequest(new { message = e.Message });
            }
        }
        
        [HttpPost("convert-coin-to-coin")]
        public async Task<ActionResult<ConversionCoinResponseDto>> ConvertCoinToCoinAsync([FromBody] ConvertCoinDto2 convertCoinDto2)
        {
            try
            {
                return Ok(await coinDataService.ConvertCoinToCoinAsync(convertCoinDto2));
            }
            catch (Exception e)
            {

                return BadRequest(new { message = e.Message });
            }
        }
    }


}