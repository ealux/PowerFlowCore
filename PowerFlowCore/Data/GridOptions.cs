using System;
using System.Collections.Generic;
using System.Text;

namespace PowerFlowCore.Data
{
    /// <summary>
    /// Options to initialize and build <see cref="Grid"/> object
    /// </summary>
    public class GridOptions
    {
        /// <summary>
        /// Use of template impedance value on <see cref="IBranch"/> with 0.0 or near impedance value (set as Breaker)
        /// </summary>
        public bool BreakersByTemplate { get; set; } = true;

        /// <summary>
        /// 
        /// </summary>
        public bool CheckGridOnCreation { get; set; } = true;
    }
}
