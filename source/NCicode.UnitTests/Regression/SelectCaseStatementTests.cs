using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Irony.Parsing;

namespace NCicode.UnitTests.Regression
{
    [TestClass]
    public class SelectCaseStatementTests
    {
        protected void ParseAndAssertNoErrors(string sourceCode)
        {
            var grammar = new CicodeGrammar();
            var parser = new Parser(grammar);
            var parseTree = parser.Parse(sourceCode);

            Assert.IsNotNull(parseTree);
            Assert.IsFalse(parseTree.HasErrors(), GetParserMessages(parseTree));
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

        [TestMethod]
        public void ShouldNotParseCaseElseWhenNotAtTheEnd()
        {
            // Arrange
            var grammar = new CicodeGrammar();
            var parser = new Parser(grammar);
            var sourceCode = 
@"
FUNCTION A()

    SELECT CASE a
        CASE 1
            A();
        CASE ELSE
            A();
        CASE 1
            A();
    END SELECT

END
";
            // Act            
            var parseTree = parser.Parse(sourceCode);

            // Assert
            Assert.IsNotNull(parseTree);
            Assert.IsTrue(parseTree.HasErrors());
            // A parser error is expected at line 7 because of the CASE ELSE clause which is not at then end of the case list
            Assert.AreEqual<int>(1, parseTree.ParserMessages.Where(m => m.Location.Line == 7).Count());           
        }

        [TestMethod]
        public void ShouldNotParseWithoutCases()
        {
            // Arrange
            var grammar = new CicodeGrammar();
            var parser = new Parser(grammar);
            var sourceCode =
@"
FUNCTION A()

    SELECT CASE a                
    END SELECT

END
";
            // Act            
            var parseTree = parser.Parse(sourceCode);

            // Assert
            Assert.IsNotNull(parseTree);
            Assert.IsTrue(parseTree.HasErrors());
            // A parser error is expected at the end of line 4 because the expected list of cases which is missing            
            Assert.AreEqual<int>(1, parseTree.ParserMessages
                .Where(m => m.Location.Line == 4)
                .Where(m => m.ParserState.ExpectedTerminals.First().Name == "CASE")
                .Count());



        }
    }
}
