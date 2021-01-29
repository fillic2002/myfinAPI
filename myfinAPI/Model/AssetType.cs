using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace myfinAPI.Model
{
    public class AssetClass
    {
        public enum AssetType {
            Shares = 1,
            Equity_MF = 2,
            PF = 3,
            PPF = 4,
            Debt_MF = 5,
            Flat = 6,
            Gold = 7,
            Plot = 8,
            Bank = 9
        }
    }
}
