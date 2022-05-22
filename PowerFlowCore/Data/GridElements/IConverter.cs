using System.Collections.Generic;

namespace PowerFlowCore.Data
{
    /// <summary>
    /// Interface to be inherits by consumer for data migration
    /// </summary>
    public interface IConverter
    {

        /// <summary>
        /// Collection of Nodes in engine-applicable type <see cref="INode"/>
        /// </summary>
        IEnumerable<INode> Nodes { get; set; }

        /// <summary>
        /// Collection of Branches in engine-applicable type <see cref="IBranch"/>
        /// </summary>
        IEnumerable<IBranch> Branches { get; set; }
    }
}
