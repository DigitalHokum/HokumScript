using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HokumScript.Script.Nodes;

namespace HokumScript.Script
{
    public class ScriptTree : Tree
    {
        public readonly string Code;
        public readonly List<ScriptToken> Tokens;
        private AstTreeNode RootNode;

        public ScriptTree(string code)
        {
            Code = code;
            ScriptParser parser = new ScriptParser();
            Tokens = parser.Tokenize(code);
            RootNode = ProcessTokens(Tokens);
        }

        public async Task<DynamicReturnValue> Evaluate(Scope scope)
        {
            return await RootNode.Evaluate(scope);
        }

        public static BlockNode ProcessTokens(List<ScriptToken> tokens)
        {
            List<AstTreeNode> blockNodes = new List<AstTreeNode>();
            AstTreeNode node = new BlockNode(null);
            int count = 0;

            StripWhiteSpace(tokens);

            while (tokens.Count > 0)
            {
                count++;
                if (count > 1000) break; // Limit to 1000 iterations while in development

                if (tokens[0].Type == EScriptTokenType.RETURN)
                    tokens.RemoveAt(0); // Last value in block is returned by default

                ScriptToken scriptToken = tokens[0];

                if (scriptToken.Type == EScriptTokenType.NAME)
                {
                    node = new RootScopeMemberNode(
                        new LiteralNode<string>(scriptToken.Value)
                    );
                    tokens.RemoveAt(0);
                }
                else if (scriptToken.Type == EScriptTokenType.ASSIGN)
                {
                    node = AssignmentNode.Parse(node, scriptToken, tokens);
                }
                else if (scriptToken.Type == EScriptTokenType.IF)
                {
                    node = IfStatementNode.Parse(node, scriptToken, tokens);
                    blockNodes.Add(node);
                    node = null;
                }
                else if (scriptToken.Type == EScriptTokenType.FOR)
                {
                    node = ForStatementNode.Parse(node, scriptToken, tokens);
                    blockNodes.Add(node);
                    node = null;
                }
                else if (scriptToken.Type == EScriptTokenType.STRING_LITERAL)
                {
                    node = new LiteralNode<string>(scriptToken.Value);
                    tokens.RemoveAt(0);
                }
                else if (scriptToken.Type == EScriptTokenType.NUMBER_LITERAL)
                {
                    AstTreeNode _node;
                    if (scriptToken.Value.Contains("."))
                    {
                        _node = new FloatLiteralNode(scriptToken.Value);
                    }
                    else
                    {
                        _node = new IntegerLiteralNode(scriptToken.Value);    
                    }

                    node = _node;
                    tokens.RemoveAt(0);
                }
                else if (scriptToken.Type == EScriptTokenType.PERIOD)
                {
                    if (tokens[1].Type == EScriptTokenType.NAME)
                    {
                        node = new ScopeMemberNode(
                            node,
                            new LiteralNode<string>(tokens[1].Value)
                        );
                        tokens.RemoveAt(0);
                        tokens.RemoveAt(0);
                    }
                }
                else if (scriptToken.Type == EScriptTokenType.L_PAREN)
                {
                    List<List<ScriptToken>> funcArgs = GetBlockTokens(tokens);
                    List<AstTreeNode> nodes = new List<AstTreeNode>();
                    ;
                    foreach (List<ScriptToken> arg in funcArgs) {
                        nodes.Add(ProcessTokens(arg));
                    }
                    node = new FunctionCallNode(
                        node, // Previous node should be a NAME
                        new FunctionArgumentNode(nodes)
                    );

                }
                else if (scriptToken.Type == EScriptTokenType.SEMI_COLON)
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
                    node = ComparisonNode.Parse(node, scriptToken, tokens);
                }
                else if (ArithmeticNode.Matches(tokens))
                {
                    AstTreeNode _node = ArithmeticNode.Parse(node, scriptToken, tokens);
                    node = _node;
                }
                else if (ArithmeticAssignmentNode.Matches(tokens))
                {
                    node = ArithmeticAssignmentNode.Parse(node, scriptToken, tokens);
                }
                else if (scriptToken.Type == EScriptTokenType.WHITESPACE)
                {
                    tokens.RemoveAt(0);
                }
                else if (scriptToken.Type == EScriptTokenType.BOOLEAN_LITERAL)
                {
                    node = new BooleanLiteralNode(tokens[0].Value);
                    tokens.RemoveAt(0);
                }
                else if (scriptToken.Type == EScriptTokenType.NULL_LITERAL)
                {
                    node = new LiteralNode<object>(null);
                    tokens.RemoveAt(0);
                }
                else
                {
                    string code = ScriptTree.ToCode(tokens, 10);
                    Console.WriteLine($"Syntax Error.Near {code}");
                }
            }

            if (node != null)
                blockNodes.Add(node);

            return new BlockNode(blockNodes);
        }

        public static List<ScriptToken> StripWhiteSpace(List<ScriptToken> tokens) {
            for (int i = 0; i < tokens.Count; i++) {
                if (tokens[i].Type == EScriptTokenType.WHITESPACE) {
                    tokens.RemoveAt(i);
                    i--;
                }
            }
            return tokens;
        }

        public static List<ScriptToken> GetNextStatementTokens(List<ScriptToken> tokens, bool consumeSemicolon = true, bool consumeOpener = false)
        {
            List<ScriptToken> statementTokens = new List<ScriptToken>();
            List<EScriptTokenType> openingBlocks = new List<EScriptTokenType>();
            openingBlocks.Add(EScriptTokenType.L_BRACKET);
            openingBlocks.Add(EScriptTokenType.L_BRACE);
            openingBlocks.Add(EScriptTokenType.L_PAREN);
            
            List<EScriptTokenType> closingBlocks = new List<EScriptTokenType>();
            closingBlocks.Add(EScriptTokenType.SEMI_COLON);
            closingBlocks.Add(EScriptTokenType.R_BRACKET);
            closingBlocks.Add(EScriptTokenType.R_BRACE);
            closingBlocks.Add(EScriptTokenType.R_PAREN);
            
            // Consume opening block
            if (consumeOpener && openingBlocks.Contains(tokens[0].Type)) {
                tokens.RemoveAt(0);
            }

            int openParens = 0;
            for (int i = 0; i < tokens.Count; i++) {
                ScriptToken scriptToken = tokens[i];
                if (scriptToken.Type == EScriptTokenType.L_PAREN)
                    openParens++;

                if (closingBlocks.Contains(scriptToken.Type)) {
                    if (consumeSemicolon && scriptToken.Type != EScriptTokenType.SEMI_COLON)
                        tokens.RemoveAt(0); // Consume end of block

                    if (openParens > 0 && scriptToken.Type == EScriptTokenType.R_PAREN) {
                        openParens--;
                    } else {
                        break;
                    }
                }

                statementTokens.Add(scriptToken);
                tokens.RemoveAt(0); // Consume part of statement
                i--;
            }
            return statementTokens;
        }

        public static List<List<ScriptToken>> GetBlockTokens(List<ScriptToken> tokens, EBlockType blockType = EBlockType.PAREN, bool groupByComma = true) {
            EScriptTokenType open = EScriptTokenType.L_PAREN;
            EScriptTokenType close = EScriptTokenType.R_PAREN;
            string closeSymbol = ")";
            
            switch(blockType) {
                case EBlockType.BRACE:
                    open = EScriptTokenType.L_BRACE;
                    close = EScriptTokenType.R_BRACE;
                    closeSymbol = "}";
                    break;
                case EBlockType.BRACKET:
                    open = EScriptTokenType.L_BRACKET;
                    close = EScriptTokenType.R_BRACKET;
                    closeSymbol = "]";
                    break;
            }
            
            int openBlocks = 0;
            List<List<ScriptToken>> args = new List<List<ScriptToken>>();
            List<ScriptToken> arg = new List<ScriptToken>();
            
            for (int i = 0; i < tokens.Count; i++) {
                ScriptToken scriptToken = tokens[i];
                if (scriptToken.Type == open) {
                    openBlocks += 1;
                    if (openBlocks > 1)
                        arg.Add(scriptToken);
                } else if (scriptToken.Type == close) {
                    openBlocks -= 1;
                    if (openBlocks > 0)
                        arg.Add(scriptToken);
                } else if (groupByComma && scriptToken.Type == EScriptTokenType.COMMA && openBlocks == 1) {
                    args.Add(arg);
                    arg = new List<ScriptToken>();
                } else if (scriptToken.Type != EScriptTokenType.WHITESPACE) {
                    arg.Add(scriptToken);
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

        public static string ToCode(List<ScriptToken> tokens, int count)
        {
            return "";
        }
    }
}
