using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Parsing;
using Irony.Interpreter;

namespace NCicode
{
    [Language("Cicode")]
    public class CicodeGrammar : Grammar
    {
        public CicodeGrammar() : base(false)
        {

            var program = new NonTerminal("program");
            var declaration = new NonTerminal("declaration");
            var declarations = new NonTerminal("declarations");
            var variableType = new NonTerminal("variable-type");
            var parenParameters = new NonTerminal("paran-parameters");
            var parameter = new NonTerminal("parameter");
            var parameters = new NonTerminal("parameters");
            var parameterInitializer = new NonTerminal("parameterInitializer");
            var functionDeclaration = new NonTerminal("function-declaration");            
            var statements = new NonTerminal("statements");
            var statement = new NonTerminal("statement");
            var semiStatement = new NonTerminal("semi-statement");
            var variableDeclaration = new NonTerminal("variable-declaration");
            var block = new NonTerminal("block");
            var variableInitializer = new NonTerminal("variableInitializer");
            var variableInitializers = new NonTerminal("variableInitializers");
            var blockContent = new NonTerminal("block-content");
            var blockDeclarations = new NonTerminal("block-declarations");
            var blockDeclaration = new NonTerminal("block-declaration");
            var functionScope = new NonTerminal("function-scope");
            var localVariableDeclaration = new NonTerminal("localVariableDeclaration");
            var parameterType = new NonTerminal("parameterType");
            var functionReturnType = new NonTerminal("function-returnType");
            var returnStatement = new NonTerminal("return-statement");
            var ifStatement = new NonTerminal("if-statement");
            var optionalElseStatement = new NonTerminal("elseStatement");
            var variableScope = new NonTerminal("variableScope");

            // Lexical Structure

            var stringLiteral = new StringLiteral("string", "\"", StringOptions.None);            
            var numberLiteral = new NumberLiteral("number");
            var identifier = TerminalFactory.CreateCSharpIdentifier("id");

            var semi = ToTerm(";", "semi");
            var colon = ToTerm(":", "colon");
            var optionalSemi = new NonTerminal("semi?");
            optionalSemi.Rule = Empty | semi;

            var lineComment = new CommentTerminal("comment", "//", "\n", "\r");
            var blockComment = new CommentTerminal("BLOCK_COMMENT", "/*", "*/");
            //comment must to be added to NonGrammarTerminals list; it is not used directly in grammar rules,
            // so we add it to this list to let Scanner know that it is also a valid terminal. 
            base.NonGrammarTerminals.Add(lineComment);
            base.NonGrammarTerminals.Add(blockComment);
            
            
            // Expressions


            
            var expression = new NonTerminal("expression");
            var optionalExpression = new NonTerminal("optional-expression");
            var conditionalExpression = new NonTerminal("conditionalExpression");
            var literal = new NonTerminal("literal");
            var term = new NonTerminal("term");
            var arithmicExpression = new NonTerminal("arithmicExpression");
            var parenthesizedExpression = new NonTerminal("ParExpr");            
            var unaryExpression = new NonTerminal("UnExpr");
            var unaryOperator = new NonTerminal("UnOp");
            var arithmicOperator = new NonTerminal("arithmicOperator", "operator");            
            var assignmentStatement = new NonTerminal("AssignmentStmt");
            var assignmentOperator = new NonTerminal("assignmentOperator");
            
            // 3. BNF rules
            expression.Rule = term | unaryExpression | arithmicExpression;
            optionalExpression.Rule = expression | Empty;
            term.Rule = numberLiteral | parenthesizedExpression | identifier;
            parenthesizedExpression.Rule = "(" + expression + ")";
            unaryExpression.Rule = unaryOperator + term;
            unaryOperator.Rule = ToTerm("-");
            arithmicExpression.Rule = expression + arithmicOperator + expression;
            arithmicOperator.Rule = ToTerm("+") | "-" | "*" | "/";                       
            assignmentStatement.Rule = identifier + assignmentOperator + expression + semi;
            assignmentOperator.Rule = ToTerm("=");
                       
            // 4. Operators precedence
            RegisterOperators(1, "+", "-");
            RegisterOperators(2, "*", "/");            

            // 5. Punctuation and transient terms
            MarkPunctuation("(", ")");
            RegisterBracePair("(", ")");
            MarkTransient(term, expression, arithmicOperator, unaryOperator, parenthesizedExpression);

            var parameter_array = new NonTerminal("parameter_array");
            var arrayIndexer = new NonTerminal("arrayIndexer");
            var rank_specifier = new NonTerminal("rank_specifier");
            var rank_specifiers = new NonTerminal("rank_specifiers");
            var rank_specifiers_opt = new NonTerminal("rank_specifiers_opt");
            var dim_specifier = new NonTerminal("dim_specifier");
            var dim_specifier_opt = new NonTerminal("dim_specifier_opt");
            var list_initializer = new NonTerminal("array_initializer");
            var list_initializer_opt = new NonTerminal("array_initializer_opt");
            var arrayIndexDeclaration = new NonTerminal("arrayIndexDeclaration");
            var arrayIndexDeclarations = new NonTerminal("arrayIndexDeclarations");
            var arrayInitializers = new NonTerminal("arrayInitializers");
            var arrayDeclaration = new NonTerminal("arrayDeclaration");
            ////////////////

            literal.Rule = numberLiteral | stringLiteral;

            ifStatement.Rule
                = ToTerm("IF") + expression + ToTerm("THEN") + block + optionalElseStatement;

            optionalElseStatement.Rule = Empty | ToTerm("ELSE") + block;


            this.Root = program;

            program.Rule = declarations;
            declarations.Rule = MakeStarRule(declarations, declaration);

            variableInitializer.Rule = identifier + assignmentOperator + literal;
            variableInitializers.Rule
                = variableInitializers + "," + variableInitializer
                | variableInitializer                
                ;

            localVariableDeclaration.Rule
                = variableType + variableInitializers + ";"
                | variableType + identifier + ";"
                ;

            variableDeclaration.Rule
                = variableScope + variableType + variableInitializers + ";"
                | variableScope + variableType + identifier + ";" 
                | arrayDeclaration + ";"
                ;

            arrayInitializers.Rule
                = arrayInitializers + "," + literal
                | literal
                ;
            
            arrayDeclaration.Rule 
                = variableScope + variableType + identifier + arrayIndexDeclarations
                | variableScope + variableType + identifier + arrayIndexDeclarations + assignmentOperator + arrayInitializers;

            arrayIndexDeclarations.Rule
                = arrayIndexDeclarations + arrayIndexDeclaration
                | arrayIndexDeclaration                
                ;
               

            arrayIndexer.Rule = "[" + expression + "]";
            arrayIndexDeclaration.Rule = "[" + numberLiteral + "]";

            variableType.Rule
                = ToTerm("INT")
                | ToTerm("STRING")
                | ToTerm("REAL")
                | ToTerm("OBJECT")
                ;

            variableScope.Rule
                = Empty                                    
                | ToTerm("GLOBAL")
                | ToTerm("MODULE")
                ;

            parameterInitializer.Rule = Empty | assignmentOperator + literal;

            parameter.Rule = variableType + identifier + parameterInitializer;

            parameters.Rule
                = parameters + "," + parameter
                | parameter;

            parenParameters.Rule
                = ToTerm("(") + ")"
                | "(" + parameters + ")";

            statements.Rule = MakeStarRule(statements, statement);
            statement.Rule
                = semiStatement
                | ifStatement
                ;

            returnStatement.Rule = ToTerm("RETURN") + optionalExpression + semi;

            semiStatement.Rule
                = assignmentStatement
                | returnStatement
                ;

            block.Rule
                = blockContent + "END";

            blockContent.Rule
                = blockDeclarations + statements                
                ;

            blockDeclarations.Rule = MakeStarRule(blockDeclarations, blockDeclaration);

            blockDeclaration.Rule
                = localVariableDeclaration;

            functionScope.Rule
                = ToTerm("PUBLIC") | "PRIVATE" | Empty;

            // These reduce hints are needed to help determine the difference between
            // a function declaration and a variable declaration.
            functionScope.ReduceIf("FUNCTION");
            variableScope.ReduceIf(semi);

            functionReturnType.Rule = variableType | Empty;

            declaration.Rule 
                = functionDeclaration
                | variableDeclaration
                ;
            functionDeclaration.Rule = functionScope + functionReturnType + "FUNCTION" + identifier + parenParameters + block;

            MarkTransient(semiStatement, blockDeclaration);

        }
    }
}
