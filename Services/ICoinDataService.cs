using background_jobs.models;

namespace background_jobs.Services
{
    public interface ICoinDataService
    {
        Task<List<CoinDataDto>> GetCoinsAsync();


        Task<CoinDataDto> CreateCoinAsync(CreateCoinDto createCoinDto);

        Task<ConversionFiatResponseDto> ConvertCoinToFiatCurrencyAsync(ConvertCoinDto convertCoinDto);

        Task<ConversionCoinResponseDto> ConvertCoinToCoinAsync(ConvertCoinDto2 convertCoinDto2);

        Task<List<CoinDataDto>> FetchCoinById (Guid id);

        Task<CoinDataDto> UpdateCoinAsync(Guid id, CreateCoinDto updateCoinDto);
        Task<bool> DeleteCoinAsync(Guid id);
        
    }
}