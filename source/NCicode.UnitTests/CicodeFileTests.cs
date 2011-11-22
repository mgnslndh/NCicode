using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Irony.Parsing;
using System.IO;
using System.Text.RegularExpressions;
using Irony;
using System.Reflection;

namespace NCicode.UnitTests
{
    public abstract class CicodeFileTests
    {
        protected void ParseAndAssertNoErrors(string sourceCode)
        {
            var grammar = new CicodeGrammar();
            var parser = new Parser(grammar);
            var parseTree = parser.Parse(sourceCode);

            Assert.IsNotNull(parseTree);
            Assert.IsFalse(parseTree.HasErrors(), GetParserMessages(parseTree));
        }

        /// <summary>
        /// Load sample program from resources, run it and check its output
        /// </summary>
        protected void ParseResourceAndAssertNoErrors(string programResourceName)
        {
            var grammar = new CicodeGrammar();
            var parser = new Parser(grammar);
            var parseTree = parser.Parse(LoadResourceText(programResourceName));

            Assert.IsNotNull(parseTree);
            Assert.IsFalse(parseTree.HasErrors(), GetParserMessages(parseTree));
        }

        /// <summary>
        /// Load sample program from resources, run it and check its output
        /// </summary>
        void RunSampleAndCompareResults(string programResourceName, string outputResourceName)
        {
            var grammar = new CicodeGrammar();
            var parser = new Parser(grammar);
            var parseTree = parser.Parse(LoadResourceText(programResourceName));

            Assert.IsNotNull(parseTree);
            Assert.IsFalse(parseTree.HasErrors());

            string result = ""; //grammar.RunSample(new RunSampleArgs(parser.Language, null, parseTree));
            Assert.IsNotNull(result);
            Assert.AreEqual(LoadResourceText(outputResourceName), result);
        }

        private string GetParserMessages(ParseTree parseTree)
        {
            var sb = new StringBuilder();
            foreach (var message in parseTree.ParserMessages)
            {
                sb.AppendFormat("{0} at line {1}, column {2}", message.Message, message.Location.Line, message.Location.Column);
            }
            sb.AppendLine();
            sb.AppendLine("Source Code:");
            sb.AppendLine();
            sb.Append(parseTree.SourceText);
            return sb.ToString();
        }

        /// <summary>
        /// Load sample Cicode program or output from assembly resources
        /// </summary>
        string LoadResourceText(string resourceName)
        {
            var asm = this.GetType().Assembly;

            using (var stream = asm.GetManifestResourceStream(resourceName))
            {
                Assert.IsNotNull(stream);

                using (var sr = new StreamReader(stream))
                {
                    var s = sr.ReadToEnd();
                    Assert.IsFalse(string.IsNullOrEmpty(s));

                    s = Regex.Replace(s, @"\r\n?", Environment.NewLine);
                    return s;
                }
            }
        }
    }
}
