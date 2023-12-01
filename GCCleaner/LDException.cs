using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCCleaner
{
    /// <summary>
    /// Class LDException.
    /// Implements the <see cref="Exception" />
    /// </summary>
    /// <seealso cref="Exception" />
    internal class LDException : Exception {
        /// <summary>
        /// Initializes a new instance of the <see cref="LDException"/> class.
        /// </summary>
        /// <param name="strMessage">The string message.</param>
        public LDException(string strMessage): base(strMessage) { }
    }
}
