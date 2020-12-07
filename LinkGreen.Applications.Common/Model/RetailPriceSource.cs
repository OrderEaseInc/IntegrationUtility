namespace LinkGreen.Applications.Common.Model
{
    public enum RetailPriceSource
    {
        /*
         * Price method is not set, we should configure defaults
         */
        NotSet = 0,

        /*
         * Price is set manually
         */
        Custom = 1,
        /*
         * Price is marked up from supplier price
         */
        Markup = 2,
        /*
         * Price is MSRP
         */
        MSRP = 3
    }
}