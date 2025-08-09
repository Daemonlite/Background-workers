using System.ComponentModel.DataAnnotations;
using background_jobs.Entities;
using Microsoft.EntityFrameworkCore;

namespace background_jobs.models
{
    public class CreateCoinDto
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Symbol is required")]
        public string Symbol { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price is required")]
        public decimal Price { get; set; }
    }

    public class CoinDataDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Symbol { get; set; } = string.Empty;
        public decimal Price { get; set; }

        public DateTime LastUpdated { get; set; }
    }

    public class FetchCoinDto
    {
        public string Symbol { get; set; } = string.Empty;
        public decimal Price { get; set; }

        public DateTime LastUpdated { get; set; }

    }

    public class CoinDto
    {
        public string Name { get; set; } = string.Empty;
        public string Symbol { get; set; } = string.Empty;
    }

    // convert example Bitcoin to Ghanaian Cedis
    public class ConvertCoinDto
    {
        public decimal Amount { get; set; }
        public string CryptoSymbol { get; set; } = string.Empty;   // e.g., "BTC"
        public string ToCurrency { get; set; } = string.Empty;     // e.g., "GHS"
    }

    // convert example bitcoin to ethereum
    public class ConvertCoinDto2
    {
        public decimal Amount { get; set; }
        public string FromCryptoSymbol { get; set; } = string.Empty;   // e.g., "BTC"
        public string ToCryptoSymbol { get; set; } = string.Empty;     // e.g., "ETH"
    }

    public class ConversionCoinResponseDto
    {
        public CoinDto FromCoin { get; set; } = new CoinDto();
        public CoinDto ToCoin { get; set; } = new CoinDto();

        public decimal Amount { get; set; }
        public decimal ConvertedAmount { get; set; }
        public decimal FiatValue { get; set; }
    }

    public class ConversionFiatResponseDto
    {
        public decimal Amount { get; set; }
        public string FromCurrency { get; set; } = string.Empty;
        public string ToCurrency { get; set; } = string.Empty;

        public decimal ConvertedAmount { get; set; }

        public CoinDto Coin { get; set; } = new CoinDto();
    }
}