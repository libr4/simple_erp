namespace Backend.Constants
{
    /// <summary>
    /// Central place for commission-related constants.
    /// Keeps magic numbers out of the service implementation.
    /// </summary>
    public static class CommissionConstants
    {
        /// <summary>
        /// Rate applied for sales >= 100 and &lt; 500.
        /// </summary>
        public const decimal RateLow = 0.01m;

        /// <summary>
        /// Rate applied for sales &gt;= 500.
        /// </summary>
        public const decimal RateHigh = 0.05m;
    }
}
