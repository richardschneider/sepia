using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sepia
{
    class TimeExample
    {
        #region Credit Card Check
        class CreditCard
        {
            DateTimeOffset IssuedOn { get; set; }
            DateTimeOffset ExpiresOn { get; set; }

            void CheckValidityAt(DateTimeOffset usedOn)
            {
                if (!usedOn.IsIn(IssuedOn, ExpiresOn))
                    throw new Exception("Invalid credit card.");
            }
        }
        #endregion
        
        void Snippets()
        {
        }
    }
}
