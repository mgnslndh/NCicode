using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace NCicode.UnitTests.Regression
{
    [TestClass]
    public class CicodeTests : RegressionTests
    {
		[TestMethod]
		public void ShouldParsePublicFunction()
		{
			ParseAndAssertNoErrors("NCicode.UnitTests.Regression.Cicode.PublicFunction.ci");
		}

		[TestMethod]
		public void ShouldParsePrivateFunction()
		{
			ParseAndAssertNoErrors("NCicode.UnitTests.Regression.Cicode.PrivateFunction.ci");
		}

		[TestMethod]
		public void ShouldParseFunctionWithIntDeclaration()
		{
			ParseAndAssertNoErrors("NCicode.UnitTests.Regression.Cicode.FunctionWithIntDeclaration.ci");
		}

		[TestMethod]
		public void ShouldParseMultipleFunctions()
		{
			ParseAndAssertNoErrors("NCicode.UnitTests.Regression.Cicode.MultipleFunctions.ci");
		}

		[TestMethod]
		public void ShouldParseFunctionWithIntReturnType()
		{
			ParseAndAssertNoErrors("NCicode.UnitTests.Regression.Cicode.FunctionWithIntReturnType.ci");
		}

		[TestMethod]
		public void ShouldParseFunctionWithMultipleParameters()
		{
			ParseAndAssertNoErrors("NCicode.UnitTests.Regression.Cicode.FunctionWithMultipleParameters.ci");
		}

		[TestMethod]
		public void ShouldParseFunctionWithMultipleDeclarations()
		{
			ParseAndAssertNoErrors("NCicode.UnitTests.Regression.Cicode.FunctionWithMultipleDeclarations.ci");
		}

		[TestMethod]
		public void ShouldParseFunctionWithStringReturnType()
		{
			ParseAndAssertNoErrors("NCicode.UnitTests.Regression.Cicode.FunctionWithStringReturnType.ci");
		}

		[TestMethod]
		public void ShouldParseFunctionWithoutModifier()
		{
			ParseAndAssertNoErrors("NCicode.UnitTests.Regression.Cicode.FunctionWithoutModifier.ci");
		}

		[TestMethod]
		public void ShouldParseFunctionWithSingleParameter()
		{
			ParseAndAssertNoErrors("NCicode.UnitTests.Regression.Cicode.FunctionWithSingleParameter.ci");
		}

		[TestMethod]
		public void ShouldParseFunctionWithStringDeclaration()
		{
			ParseAndAssertNoErrors("NCicode.UnitTests.Regression.Cicode.FunctionWithStringDeclaration.ci");
		}

		[TestMethod]
		public void ShouldParseIf()
		{
			ParseAndAssertNoErrors("NCicode.UnitTests.Regression.Cicode.If.ci");
		}

		[TestMethod]
		public void ShouldParseFunctionWithIntParameterInitializer()
		{
			ParseAndAssertNoErrors("NCicode.UnitTests.Regression.Cicode.FunctionWithIntParameterInitializer.ci");
		}

		[TestMethod]
		public void ShouldParseIfElse()
		{
			ParseAndAssertNoErrors("NCicode.UnitTests.Regression.Cicode.IfElse.ci");
		}

		[TestMethod]
		public void ShouldParseFunctionWithStringParameterInitializer()
		{
			ParseAndAssertNoErrors("NCicode.UnitTests.Regression.Cicode.FunctionWithStringParameterInitializer.ci");
		}

		[TestMethod]
		public void ShouldParseReturnWithExpression()
		{
			ParseAndAssertNoErrors("NCicode.UnitTests.Regression.Cicode.ReturnWithExpression.ci");
		}

		[TestMethod]
		public void ShouldParseReturnWithoutExpression()
		{
			ParseAndAssertNoErrors("NCicode.UnitTests.Regression.Cicode.ReturnWithoutExpression.ci");
		}

		
	}
}