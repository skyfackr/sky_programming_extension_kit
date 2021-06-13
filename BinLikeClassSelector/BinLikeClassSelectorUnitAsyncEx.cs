using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
//修复全部await没有configure的问题

namespace SPEkit.BinLikeClassSelector
{
    public partial class BinLikeClassSelectorUnit
    {
        /// <inheritdoc cref="GetValidBinList"/>
        public async Task<List<long>> GetValidBinListAsync() => await Task.Run(this.GetValidBinList);

        /// <inheritdoc cref="GetValidBinArray"/>
        public async Task<long[]> GetValidBinArrayAsync() => await Task.Run(this.GetValidBinArray);

        /// <inheritdoc cref="Match"/>
        public virtual async Task<bool> MatchAsync(long matchRuleCode) => await Task.Run(() => this.Match(matchRuleCode));
        
    }

    
}
