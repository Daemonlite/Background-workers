

using background_jobs.Data;
using background_jobs.models;
using background_jobs.Entities;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;

namespace background_jobs.Services
{
    public class CoinDataService(AppDbContext context, RedisCacheService cache) : ICoinDataService
    {
        public async Task<List<CoinDataDto>> GetCoinsAsync()
        {
            var lumenPrice = await cache.CacheGetAsync("eth");
            Console.WriteLine($"Exchange rate for Lumens is {lumenPrice}");

            return await context.Coins
                .Select(x => new CoinDataDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Symbol = x.Symbol,
                    Price = x.Price,
                    LastUpdated = x.LastUpdated
                }).ToListAsync();

        }
        public async Task<ConversionCoinResponseDto> ConvertCoinToCoinAsync(ConvertCoinDto2 convertCoinDto2)
        {
            // 1. Get rates from cache (individual keys)
            var fromRateStr = await cache.CacheGetAsync(convertCoinDto2.FromCryptoSymbol.ToLower());
            var toRateStr = await cache.CacheGetAsync(convertCoinDto2.ToCryptoSymbol.ToLower());

            if (fromRateStr == null || toRateStr == null)
            {
                throw new InvalidOperationException("Required exchange rates not available in cache");
            }

            // 2. Parse decimal values
            if (!decimal.TryParse(fromRateStr, out var fromRate) ||
                !decimal.TryParse(toRateStr, out var toRate))
            {
                throw new InvalidOperationException("Invalid exchange rate format in cache");
            }

            // 3. Convert directly using USD rates
            decimal exchangeRate = fromRate / toRate;
            decimal convertedAmount = convertCoinDto2.Amount * exchangeRate;

            // 4. Calculate fiat (USD) value of original amount
            decimal fiatValue = convertCoinDto2.Amount * fromRate;

            // 5. Get coin names (optional - could be cached separately)
            var fromCoin = await context.Coins.FirstOrDefaultAsync(x => x.Symbol == convertCoinDto2.FromCryptoSymbol);
            var toCoin = await context.Coins.FirstOrDefaultAsync(x => x.Symbol == convertCoinDto2.ToCryptoSymbol);

            return new ConversionCoinResponseDto
            {
                FromCoin = new CoinDto
                {
                    Name = fromCoin?.Name ?? convertCoinDto2.FromCryptoSymbol.ToUpper(),
                    Symbol = convertCoinDto2.FromCryptoSymbol
                },
                ToCoin = new CoinDto
                {
                    Name = toCoin?.Name ?? convertCoinDto2.ToCryptoSymbol.ToUpper(),
                    Symbol = convertCoinDto2.ToCryptoSymbol
                },
                Amount = convertCoinDto2.Amount,
                ConvertedAmount = convertedAmount,
                ExchangeRate = exchangeRate, // 1 FROM = X TO
                FiatValue = Math.Round(fiatValue, 2)
            };
        }



        public async Task<ConversionFiatResponseDto> ConvertCoinToFiatCurrencyAsync(ConvertCoinDto convertCoinDto)
        {
            var coin = await context.Coins.FirstOrDefaultAsync(x => x.Symbol == convertCoinDto.CryptoSymbol) ?? throw new Exception($"Coin {convertCoinDto.CryptoSymbol} not found or not supported.");

            // Get the crypto rate in USD from the cache. This is the value of 1 crypto unit in USD.
            var cryptoRate = await cache.CacheGetAsync(convertCoinDto.CryptoSymbol);
            Console.WriteLine($"Exchange rate for {convertCoinDto.CryptoSymbol} is {cryptoRate}");

            // Get the fiat rate in USD from the cache. This is the value of 1 fiat unit in USD.
            var fiatRate = await cache.CacheGetAsync(convertCoinDto.ToCurrency);
            Console.WriteLine($"Exchange rate for {convertCoinDto.ToCurrency} is {fiatRate}");

            if (cryptoRate == null || fiatRate == null)
            {
                // One of the rates is missing. This means the cache is incomplete or outdated.
                throw new Exception("One or both exchange rates are missing.");
            }

            var cryptoRateDecimal = Convert.ToDecimal(cryptoRate);
            var fiatRateDecimal = Convert.ToDecimal(fiatRate);

            // Convert the amount from crypto to fiat using USD as the common base currency.
            // This formula calculates: (Crypto Amount * Crypto USD Value) / Fiat USD Value
            var convertedAmount = convertCoinDto.Amount * cryptoRateDecimal / fiatRateDecimal;

            return new ConversionFiatResponseDto
            {
                Amount = convertCoinDto.Amount,
                ConvertedAmount = convertedAmount,
                FromCurrency = convertCoinDto.CryptoSymbol,
                ToCurrency = convertCoinDto.ToCurrency,
                Coin = new CoinDto
                {
                    Name = coin.Name,
                    Symbol = convertCoinDto.CryptoSymbol
                }
            };
        }


        public async Task<CoinDataDto> CreateCoinAsync(CreateCoinDto createCoinDto)
        {
            var existingCoin = await context.Coins.FirstOrDefaultAsync(x => x.Name == createCoinDto.Name && x.Symbol == createCoinDto.Symbol);
            if (existingCoin != null)
            {
                throw new Exception($"Coin with name {createCoinDto.Name} and symbol {createCoinDto.Symbol} already exists.");
            }
            else
            {
                var coin = new Coin
                {
                    Name = createCoinDto.Name,
                    Symbol = createCoinDto.Symbol,
                    Price = createCoinDto.Price,
                    LastUpdated = DateTime.Now
                };
                await context.Coins.AddAsync(coin);
                await context.SaveChangesAsync();
                return new CoinDataDto { Id = coin.Id, Name = coin.Name, Symbol = coin.Symbol, Price = coin.Price, LastUpdated = coin.LastUpdated };
            }
        }

        public async Task<bool> DeleteCoinAsync(Guid id)
        {
            var coin = await context.Coins.FirstOrDefaultAsync(x => x.Id == id);
            if (coin == null)
            {
                throw new Exception($"Coin with id {id} not found.");
            }
            else
            {
                context.Coins.Remove(coin);
                await context.SaveChangesAsync();
                return true;
            }
        }
    }
}