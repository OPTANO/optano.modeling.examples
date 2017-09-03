using CsvHelper.Configuration;

namespace Knapsack
{
    public sealed class KnapsackItemMap : CsvClassMap<KnapsackItem>
    {
        public KnapsackItemMap()
        {
            AutoMap();
            Map(m => m.IsPacked).Ignore();
        }
    }
}