using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace SPEkit.BinLikeClassSelector
{
    public partial class BinLikeClassSelectorUnit
    {
        public async Task<List<long>> GetValidBinListAsync() => await Task.Run(this.GetValidBinList);

        public async Task<long[]> GetValidBinArrayAsync() => await Task.Run(this.GetValidBinArray);

        public virtual async Task<bool> MatchAsync(long matchRuleCode) => await Task.Run(() => this.Match(matchRuleCode));
    }
}
