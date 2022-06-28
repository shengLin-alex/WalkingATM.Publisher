namespace WalkingATM.Publisher.GrpcClient.Services;

public class StockPriceSource
{
    /// <summary>
    /// format should be: "{strategy} | {date} | {time} | {symbol} | {symbolName} | {"價格"} | {price}"
    /// the "價格" is useless, ignore.
    /// </summary>
    /// <param name="stockPriceString"></param>
    /// <param name="stockPriceRecordSeparator"></param>
    public StockPriceSource(string stockPriceString, string stockPriceRecordSeparator)
    {
        stockPriceString = stockPriceString.ToUpper().Replace(" ", string.Empty);
        var strings = stockPriceString.Split(stockPriceRecordSeparator);

        if (strings.Length != 7)
            throw new ArgumentException($"The parameter is invalid: {stockPriceString}", nameof(stockPriceString));

        Strategy = strings[0];
        Date = strings[1];
        Time = strings[2];
        Symbol = strings[3];
        SymbolName = strings[4];
        // ignore strings[5]
        Price = strings[6];
    }

    public string Strategy { get; }
    public string Date { get; }
    public string Time { get; }
    public string Symbol { get; }
    public string SymbolName { get; }
    public string Price { get; }

    public static IEqualityComparer<StockPriceSource> StockPriceSourceComparer { get; } =
        new StockPriceSourceEqualityComparer();

    private sealed class StockPriceSourceEqualityComparer : IEqualityComparer<StockPriceSource>
    {
        public bool Equals(StockPriceSource? x, StockPriceSource? y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.Strategy == y.Strategy &&
                   x.Date == y.Date &&
                   x.Time == y.Time &&
                   x.Symbol == y.Symbol &&
                   x.SymbolName == y.SymbolName;
        }

        public int GetHashCode(StockPriceSource obj)
        {
            return HashCode.Combine(obj.Strategy, obj.Date, obj.Time, obj.Symbol, obj.SymbolName);
        }
    }
}