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
            Bank = 6,
            Plot = 7,
            Flat = 8,
            Bonds =9,
            Gold = 12
        }
    }
}
