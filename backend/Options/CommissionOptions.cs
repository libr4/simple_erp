namespace Backend.Options
{
    /// <summary>
    /// Configuration options for commission calculation.
    /// Default values are provided so behavior doesn't change if configuration section is absent.
    /// </summary>
    public class CommissionOptions
    {
        /// <summary>
        /// Rate applied for sales >= 100 and < 500.
        /// </summary>
        public decimal RateLow { get; set; } = 0.01m;

        /// <summary>
        /// Rate applied for sales >= 500.
        /// </summary>
        public decimal RateHigh { get; set; } = 0.05m;
    }
}
