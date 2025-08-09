using background_jobs.Services;
using Microsoft.AspNetCore.Mvc;
using background_jobs.models;
using Microsoft.AspNetCore.Cors;

namespace background_jobs.Controllers
{
    [EnableCors("AllowSpecificOrigin")]
    [Route("api/[controller]")]
    [ApiController]
    public class CoinDataController(ICoinDataService coinDataService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<CoinDataDto>>> GetCoinsAsync()
        {
            return Ok(await coinDataService.GetCoinsAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<List<CoinDataDto>>> FetchCoinById(Guid id)
        {
            return Ok(await coinDataService.FetchCoinById(id));
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

        [HttpPut("{id}")]
        public async Task<ActionResult<CoinDataDto>> UpdateCoinAsync(Guid id, [FromBody] CreateCoinDto updateCoinDto)
        {
            try
            {
                return Ok(await coinDataService.UpdateCoinAsync(id, updateCoinDto));
            }
            catch (Exception e)
            {

                return BadRequest(new { message = e.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> DeleteCoinAsync(Guid id)
        {
            try
            {
                return Ok(await coinDataService.DeleteCoinAsync(id));
            }
            catch (Exception e)
            {
                
                return BadRequest(new { message = e.Message });
            }
        }
    }


}