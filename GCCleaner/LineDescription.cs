using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCCleaner
{
    /// <summary>
    /// Class LineDescription.
    /// Describes a line to be commented out / removed
    /// string comparisions are case insensitve
    /// if given more than one comparison all must  match (AND)
    /// at least one comparison must be provided
    /// </summary>
    public class LineDescription
    {
        /// <summary>
        /// If provided every line is searched if it starts with this string
        /// </summary>
        /// <value>Value to search for at the beginning of a line</value>
        public string StartsWith { get; set; } = "";
        /// <summary>
        /// Determines whether this instance contains the object.
        /// </summary>
        /// <value>Value to search for at the beginning of a line</value>
        public string Contains { get; set; } = "";
        /// <summary>
        /// Gets or sets the ends with.
        /// </summary>
        /// <value>Value to search for at the end of a line</value>
        public string EndsWith { get; set; } = "";
        /// <summary>
        /// Gets or sets the matches.
        /// </summary>
        /// <value>Exact value of the line</value>
        public string Matches { get; set; } = "";
        /// <summary>
        /// Keeps the line (no matter what else matches) if this pattern is found
        /// </summary>
        /// <value>I use this with 'M175 P0' to keep the first line, which changes my marlin display to HH:mm:ss (done) of HH:mm:ss (open) instead of xx% HH:mm:ss (done).</value>
        public string KeepIfContains { get; set; } = "";

        /// <summary>
        /// Gets or sets the comment.
        /// </summary>
        /// <value>A comment placed at the end of a handled line - usefull for searches in the generated document - use patterns like $$$$$ or so to easily find them.</value>
        public string Comment { get; set; } = "";
        /// <summary>
        /// Gets or sets a value indicating whether [remove line].
        /// </summary>
        /// <value><c>true</c> if [true] the line will be removed; otherwise, it will be <c>commented out</c> an decorated with <see cref="Comment"/>.</value>
        public bool RemoveLine { get; set; }

        private bool _IsChecked;
        private bool _HasComment;
        private bool _HasContains;
        private bool _HasEndsWith;
        private bool _HasKeepIfContains;
        private bool _HasMatches;
        private bool _HasStartsWith;

        public string CheckDescription()
        {
            _HasComment=!string.IsNullOrEmpty(Comment);
            _HasContains=!string.IsNullOrEmpty(Contains);
            _HasEndsWith = !string.IsNullOrEmpty(EndsWith);
            _HasMatches = !string.IsNullOrEmpty(Matches);
            _HasStartsWith = !string.IsNullOrEmpty(StartsWith);
            _HasKeepIfContains= !string.IsNullOrEmpty(KeepIfContains);

            if (_HasComment) {
                if (!Comment.StartsWith(";"))
                {
                    Comment = $";{Comment}";
                }
                if (!Comment.StartsWith(" ")) {
                    Comment = $"    {Comment}";
                }
            }

            _IsChecked = true;

            if (!_HasContains && _HasEndsWith && !_HasMatches && !_HasStartsWith)
            {
                return ("At least one comparison (StartsWith, Contains, EndsWith, Matches) must be set.");
            }
            return ("OK");
        }

        public (bool hasResult,string? strErg) CheckLine(string strLine)
        {
            if (!_IsChecked)
            {
                string strTest = CheckDescription();
                if (strTest != "OK")
                {
                    throw (new LDException($"This LineDescription has a problem:{Environment.NewLine}{strTest}"));
                }
            }
            if (_HasKeepIfContains)
            {
                if (strLine.Contains(KeepIfContains, StringComparison.OrdinalIgnoreCase)) //check for existance - if found simply keep it
                {
                    return (false, null);
                }
            }

            if (_HasContains)
            {
                if (!strLine.Contains(Contains, StringComparison.OrdinalIgnoreCase))
                {
                    return (false,null);
                }
            }
            if (_HasEndsWith)
            {
                if (!strLine.EndsWith(EndsWith, StringComparison.OrdinalIgnoreCase))
                {
                    return (false, null);
                }
            }
            if (_HasStartsWith)
            {
                if (!strLine.StartsWith(StartsWith, StringComparison.OrdinalIgnoreCase))
                {
                    return (false, null);
                }
            }
            if (_HasMatches)
            {
                if (!strLine.Equals(Matches, StringComparison.OrdinalIgnoreCase))
                {
                    return (false, null);
                }
            }
            //when we reach this point we had a hit
            if (RemoveLine)
            {
                return (true,null);
            }
            return (true, $";{strLine}{(_HasComment ? Comment : "")}");
        }
    }
}
