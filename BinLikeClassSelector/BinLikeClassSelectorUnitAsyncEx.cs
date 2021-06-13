using System.Collections.Generic;
using System.Threading.Tasks;

//修复全部await没有configure的问题

namespace SPEkit.BinLikeClassSelector
{
    public partial class BinLikeClassSelectorUnit
    {
        /// <inheritdoc cref="GetValidBinList" />
        public async Task<List<long>> GetValidBinListAsync()
        {
            return await Task.Run(GetValidBinList);
        }

        /// <inheritdoc cref="GetValidBinArray" />
        public async Task<long[]> GetValidBinArrayAsync()
        {
            return await Task.Run(GetValidBinArray);
        }

        /// <inheritdoc cref="Match" />
        public virtual async Task<bool> MatchAsync(long matchRuleCode)
        {
            return await Task.Run(() => Match(matchRuleCode));
        }
    }
}