using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace SPEkit.BinLikeClassSelectors
{
    [SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
    public partial class BinLikeClassSelectorUnit
    {
        /// <inheritdoc cref="GetValidBinList" />
        public async Task<List<long>> GetValidBinListAsync()
        {
            return await Task.Run(GetValidBinList).ConfigureAwait(false);
        }

        /// <inheritdoc cref="GetValidBinArray" />
        public async Task<long[]> GetValidBinArrayAsync()
        {
            return await Task.Run(GetValidBinArray).ConfigureAwait(false);
        }

        /// <inheritdoc cref="Match" />
        public virtual async Task<bool> MatchAsync(long matchRuleCode)
        {
            return await Task.Run(() => Match(matchRuleCode)).ConfigureAwait(false);
        }
    }
}