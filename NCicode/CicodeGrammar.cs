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
            var arrayIndexers = new NonTerminal("arrayIndexers");
            var functionCall = new NonTerminal("functionCall");
            var functionCallStatement = new NonTerminal("functionCallStatement");
            
            // Lexical Structure

            var stringLiteral = new StringLiteral("string", "\"", StringOptions.None);
            stringLiteral.EscapeChar = '^';
            var numberLiteral = new NumberLiteral("number");
            var identifier = TerminalFactory.CreateCSharpIdentifier("id");

            var semi = ToTerm(";", "semi");
            var colon = ToTerm(":", "colon");
            var optionalSemi = new NonTerminal("semi?");
            optionalSemi.Rule = Empty | semi;

            var dashLineComment = new CommentTerminal("comment", "//", "\n", "\r");
            var bangLineComment = new CommentTerminal("comment", "!", "\n", "\r");
            var blockComment = new CommentTerminal("BLOCK_COMMENT", "/*", "*/");
            //comment must to be added to NonGrammarTerminals list; it is not used directly in grammar rules,
            // so we add it to this list to let Scanner know that it is also a valid terminal. 
            base.NonGrammarTerminals.Add(dashLineComment);
            base.NonGrammarTerminals.Add(bangLineComment);
            base.NonGrammarTerminals.Add(blockComment);
            
            
            // Expressions


            
            var expression = new NonTerminal("expression");
            var expressionList = new NonTerminal("expressionList");
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
            var variable = new NonTerminal("variable");

            // 3. BNF rules
            expression.Rule = term | unaryExpression | arithmicExpression | functionCall;
            expressionList.Rule
                = expressionList + "," + expression
                | expression
                | Empty
                ;
            optionalExpression.Rule = expression | Empty;
            term.Rule = literal | parenthesizedExpression | variable;
            parenthesizedExpression.Rule = "(" + expression + ")";
            unaryExpression.Rule = unaryOperator + term;
            unaryOperator.Rule = ToTerm("-");
            arithmicExpression.Rule = expression + arithmicOperator + expression;
            arithmicOperator.Rule = ToTerm("+") | "-" | "*" | "/" | "MOD" | "BITAND" | "BITOR" | "BITXOR";                       
            assignmentStatement.Rule = variable + assignmentOperator + expression + semi;
            assignmentOperator.Rule = ToTerm("=");
                       
            // 4. Operators precedence
            RegisterOperators(1, "+", "-");
            RegisterOperators(2, "*", "/", "MOD");
            RegisterOperators(10, "BITAND");
            RegisterOperators(11, "BITXOR");
            RegisterOperators(12, "BITOR");
            

            // 5. Punctuation and transient terms
            MarkPunctuation("(", ")");
            RegisterBracePair("(", ")");
            MarkTransient(term, expression, arithmicOperator, unaryOperator, parenthesizedExpression);

            var arrayIdentifier = new NonTerminal("arrayIdentifier");
            var arrayIndexer = new NonTerminal("arrayIndexer");
            var arrayIndexDeclaration = new NonTerminal("arrayIndexDeclaration");
            var arrayIndexDeclarations = new NonTerminal("arrayIndexDeclarations");
            var arrayInitializers = new NonTerminal("arrayInitializers");
            var arrayDeclaration = new NonTerminal("arrayDeclaration");
            ////////////////

            literal.Rule = numberLiteral | stringLiteral;

            ifStatement.Rule
                = ToTerm("IF") + expression + ToTerm("THEN") + block + optionalElseStatement;

            optionalElseStatement.Rule = Empty | ToTerm("ELSE") + block;

            functionCall.Rule = identifier + "(" + expressionList + ")";
            
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

            
            arrayIndexers.Rule
                = arrayIndexers + arrayIndexer
                | arrayIndexer
                ;

            arrayIdentifier.Rule = identifier + arrayIndexers;
            arrayIndexer.Rule = "[" + expression + "]";
            arrayIndexDeclaration.Rule = "[" + numberLiteral + "]";

            variable.Rule
                = identifier
                | identifier + arrayIndexers
                ;

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

            functionCallStatement.Rule = functionCall + semi;

            returnStatement.Rule = ToTerm("RETURN") + optionalExpression + semi;

            semiStatement.Rule
                = assignmentStatement
                | returnStatement
                | functionCallStatement
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
