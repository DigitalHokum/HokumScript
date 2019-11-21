using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HokumScript
{
    public class Tree
    {
        public readonly string Code;
        public readonly List<Token> Tokens;
        private AstTreeNode RootNode;

        public Tree(string code)
        {
            Code = code;
            Tokens = Parser.Tokenize(code);
            RootNode = ProcessTokens(Tokens);
        }

        public async Task<DynamicReturnValue> Evaluate(Scope scope)
        {
            return await RootNode.Evaluate(scope);
        }

        public static BlockNode ProcessTokens(List<Token> tokens)
        {
            List<AstTreeNode> blockNodes = new List<AstTreeNode>();
            AstTreeNode node = new BlockNode(null);
            int count = 0;

            StripWhiteSpace(tokens);

            while (tokens.Count > 0)
            {
                count++;
                if (count > 1000) break; // Limit to 1000 iterations while in development

                if (tokens[0].Type == ETokenType.RETURN)
                    tokens.RemoveAt(0); // Last value in block is returned by default

                Token token = tokens[0];

                if (token.Type == ETokenType.NAME)
                {
                    node = new RootScopeMemberNode(
                        new LiteralNode<string>(token.Value)
                    );
                    tokens.RemoveAt(0);
                }
                else if (token.Type == ETokenType.ASSIGN)
                {
                    node = AssignmentNode.Parse(node, token, tokens);
                }
                else if (token.Type == ETokenType.IF)
                {
                    node = IfStatementNode.Parse(node, token, tokens);
                    blockNodes.Add(node);
                    node = null;
                }
                else if (token.Type == ETokenType.FOR)
                {
                    node = ForStatementNode.Parse(node, token, tokens);
                    blockNodes.Add(node);
                    node = null;
                }
                else if (token.Type == ETokenType.STRING_LITERAL)
                {
                    node = new LiteralNode<string>(token.Value);
                    tokens.RemoveAt(0);
                }
                else if (token.Type == ETokenType.NUMBER_LITERAL)
                {
                    if (token.Value.Contains("."))
                    {
                        node = new FloatLiteralNode(token.Value);
                    }
                    else
                    {
                        node = new IntegerLiteralNode(token.Value);    
                    }
                    
                    tokens.RemoveAt(0);
                }
                else if (token.Type == ETokenType.PERIOD && tokens[1].Type == ETokenType.NAME)
                {
                    node = new ScopeMemberNode(
                        node,
                        new LiteralNode<string>(tokens[1].Value)
                    );
                    tokens.RemoveAt(0);
                    tokens.RemoveAt(0);
                }
                else if (tokens[0].Type == ETokenType.L_PAREN)
                {
                    List<List<Token>> funcArgs = GetBlockTokens(tokens);
                    List<AstTreeNode> nodes = new List<AstTreeNode>();
                    ;
                    foreach (List<Token> arg in funcArgs) {
                        nodes.Add(ProcessTokens(arg));
                    }
                    node = new FunctionCallNode(
                        node, // Previous node should be a NAME
                        new FunctionArgumentNode(nodes)
                    );

                }
                else if (tokens[0].Type == ETokenType.SEMI_COLON)
                {
                    if (node != null)
                    {
                        blockNodes.Add(node);
                    }

                    node = null;
                    tokens.RemoveAt(0);
                }
                else if (ComparisonNode.Matches(tokens))
                {
                    node = ComparisonNode.Parse(node, token, tokens);
                }
                else if (ArithmeticNode.Matches(tokens))
                {
                    node = ArithmeticNode.Parse(node, token, tokens);
                }
                else if (ArithmeticAssignmentNode.Matches(tokens))
                {
                    node = ArithmeticAssignmentNode.Parse(node, token, tokens);
                }
                else if (tokens[0].Type == ETokenType.WHITESPACE)
                {
                    tokens.RemoveAt(0);
                }
                else if (tokens[0].Type == ETokenType.BOOLEAN_LITERAL)
                {
                    node = new BooleanLiteralNode(tokens[0].Value);
                    tokens.RemoveAt(0);
                }
                else if (tokens[0].Type == ETokenType.NULL_LITERAL)
                {
                    node = new LiteralNode<object>(null);
                    tokens.RemoveAt(0);
                }
                else
                {
                    string code = Tree.ToCode(tokens, 10);
                    Console.WriteLine($"Syntax Error.Near {code}");
                    
                }
            }

            if (node != null)
                blockNodes.Add(node);

            return new BlockNode(blockNodes);
        }

        public static List<Token> StripWhiteSpace(List<Token> tokens) {
            for (int i = 0; i < tokens.Count; i++) {
                if (tokens[i].Type == ETokenType.WHITESPACE) {
                    tokens.RemoveAt(i);
                    i--;
                }
            }
            return tokens;
        }
        
        public static List<Token> GetNextStatementTokens(List<Token> tokens, bool consumeSemicolon = true)
        {
            List<Token> statementTokens = new List<Token>();
            List<ETokenType> openingBlocks = new List<ETokenType>();
            openingBlocks.Add(ETokenType.L_BRACKET);
            openingBlocks.Add(ETokenType.L_BRACE);
            openingBlocks.Add(ETokenType.L_PAREN);
            
            List<ETokenType> closingBlocks = new List<ETokenType>();
            closingBlocks.Add(ETokenType.SEMI_COLON);
            closingBlocks.Add(ETokenType.R_BRACKET);
            closingBlocks.Add(ETokenType.R_BRACE);
            closingBlocks.Add(ETokenType.R_PAREN);

            // Consume opening block
            if (openingBlocks.Contains(tokens[0].Type)) {
                tokens.RemoveAt(0);
            }

            int openParens = 0;
            for (int i = 0; i < tokens.Count; i++) {
                Token token = tokens[i];
                if (token.Type == ETokenType.L_PAREN)
                    openParens++;

                if (closingBlocks.Contains(token.Type)) {
                    if (consumeSemicolon && token.Type != ETokenType.SEMI_COLON)
                        tokens.RemoveAt(0); // Consume end of block

                    if (openParens > 0 && token.Type == ETokenType.R_PAREN) {
                        openParens--;
                    } else {
                        break;
                    }
                }

                statementTokens.Add(token);
                tokens.RemoveAt(0); // Consume part of statement
                i--;
            }
            return statementTokens;
        }
        
        public static List<List<Token>> GetBlockTokens(List<Token> tokens, EBlockType blockType = EBlockType.PAREN, bool groupByComma = true) {
            ETokenType open = ETokenType.L_PAREN;
            ETokenType close = ETokenType.R_PAREN;
            string closeSymbol = ")";
            
            switch(blockType) {
                case EBlockType.BRACE:
                    open = ETokenType.L_BRACE;
                    close = ETokenType.R_BRACE;
                    closeSymbol = "}";
                    break;
                case EBlockType.BRACKET:
                    open = ETokenType.L_BRACKET;
                    close = ETokenType.R_BRACKET;
                    closeSymbol = "]";
                    break;
            }
            
            int openBlocks = 0;
            List<List<Token>> args = new List<List<Token>>();
            List<Token> arg = new List<Token>();
            
            for (int i = 0; i < tokens.Count; i++) {
                Token token = tokens[i];
                if (token.Type == open) {
                    openBlocks += 1;
                    if (openBlocks > 1)
                        arg.Add(token);
                } else if (token.Type == close) {
                    openBlocks -= 1;
                    if (openBlocks > 0)
                        arg.Add(token);
                } else if (groupByComma && token.Type == ETokenType.COMMA && openBlocks == 1) {
                    args.Add(arg);
                    arg = new List<Token>();
                } else if (token.Type != ETokenType.WHITESPACE) {
                    arg.Add(token);
                }

                // Consume token
                tokens.RemoveAt(0);
                i--;
                if (openBlocks == 0) {
                    if (arg.Count > 0)
                        args.Add(arg);

                    return args;
                }
            }
            Console.WriteLine($"Invalid Syntax, missing ${closeSymbol}");
            return null;
        }

        public static string ToCode(List<Token> tokens, int count)
        {
            return "";
        }
    }
}
