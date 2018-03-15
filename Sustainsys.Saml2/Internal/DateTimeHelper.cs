using System;

namespace Sustainsys.Saml2.Internal
{
    static class DateTimeHelper
    {
        internal static DateTime? EarliestTime(DateTime? value1, DateTime? value2)
        {
            if (value1 == null || 
                value1.HasValue && value2.HasValue && value1.Value > value2.Value)
            {
                return value2;
            }

            return value1;
        }
    }
}
