using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GCCleaner
{
    /// <summary>
    /// Class GCCSettings.
    /// </summary>
    [JsonSerializable(typeof(LineDescription))]
    public class GCCSettings
    {
        /// <summary>
        /// Gets or sets the file name post fix.
        /// The cleaned file will add this just before the extension.
        /// </summary>
        /// <value>The file name post fix - if none is giveen _GC will be used.</value>
        public string? FileNamePostFix { get; set; }
        /// <summary>
        /// Gets or sets the line ending.
        /// </summary>
        /// <value>Used to avoid OS specific line enedings.</value>
        public string? LineEnding { get; set; }
        /// <summary>
        /// Keeps the program open to view the output when started via Drag and Drop
        /// </summary>
        /// <value><c>true</c> if [wait for key]; otherwise, <c>false</c>.</value>
        public bool WaitForKey { get; set; }
        /// <summary>
        /// Gets or sets the line descriptions.
        /// must contain at least one entry
        /// When reading the file, the first matching LineDescrition stops further processing
        /// </summary>
        /// <value>The line descriptions.</value>
        public List<LineDescription> LineDescriptions { get; set; }= new List<LineDescription>();

    }
}
