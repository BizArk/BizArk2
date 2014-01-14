using System.Text;
using BizArk.Core.Extensions.StringExt;

namespace BizArk.Core.CmdLine
{
    /// <summary>
    /// Builds HtlpText for the command-line object
    /// </summary>
    public class HelpTextBuilder
    {
        private readonly int _maxWidth;
        readonly StringBuilder _stringBuilder = new StringBuilder();

        /// <summary>
        /// Creates an instance of HelpTextBuilder
        /// </summary>
        /// <param name="maxWidth"></param>
        public HelpTextBuilder(int maxWidth)
        {
            _maxWidth = maxWidth;
        }

        /// <summary>
        /// Appends a line
        /// </summary>
        /// <param name="value"></param>
        public void AppendLine(string value)
        {
            _stringBuilder.AppendLine(value.Wrap(_maxWidth));
        }

        /// <summary>
        /// Appends an empty line
        /// </summary>
        public void AppendLine()
        {
            _stringBuilder.AppendLine();
        }

        /// <summary>
        /// Appends a value
        /// </summary>
        /// <param name="value"></param>
        public void Append(string value)
        {
            _stringBuilder.Append(value);
        }

        public override string ToString()
        {
            return _stringBuilder.ToString();
        }
    }
}