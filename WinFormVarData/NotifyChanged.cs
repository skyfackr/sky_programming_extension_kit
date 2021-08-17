using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPEkit.WinFormVarData
{
    public sealed partial class DataBinder<T>
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public void Refresh()
        {

        }

        public async Task RefreshAsync()
        {

        }
    }
}
