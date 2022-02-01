using System;
using System.Collections.Generic;

namespace Models
{
    [Serializable]
    public class PriceStep : Dictionary<decimal, decimal>
    {
    }

    [Serializable]
    public sealed class PriceStepList : Dictionary<string, PriceStep>
    {
        static PriceStepList()
        {
            AllPriceSteps = new PriceStepList();

            PriceStep stockStep = new PriceStep
            {
                { 10m, 0.01m },
                { 50m, 0.05m },
                { 100m, 0.1m },
                { 500m, 0.5m },
                { 1000m, 1m },
                { 9999m, 5m },
                { 99999m, 5m }
            };
            AllPriceSteps.Add("Stock", stockStep);

            PriceStep etfPriceStep = new PriceStep
            {
                { 50m, 0.01m },
                { 1000m, 0.05m }
            };
            AllPriceSteps.Add("ETF", etfPriceStep);
        }

        public static PriceStepList AllPriceSteps
        {
            get;
            private set;
        }

    }
}