using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HokumScript.Script.Nodes
{
    public class ForStatementNode : AstTreeNode
    {
        public readonly LiteralNode<string> Variable;
        public readonly AstTreeNode Array;
        public readonly AstTreeNode Block;

        public ForStatementNode(
            LiteralNode<string> var,
            AstTreeNode array,
            AstTreeNode block
        )
        {
            Variable = var;
            Array = array;
            Block = block;
        }

        private async Task EvaluateBlock(Scope scope, string variable, object val)
        {
            scope.Set(variable, val);
            await Block.Evaluate(scope);
        }

        public async Task<DynamicReturnValue> Evaluate(Scope scope)
        {
            string variable = (await Variable.Evaluate(scope)).GetValue<string>();

            DynamicReturnValue value = (await Array.Evaluate(scope));
            Type type = value.Value.GetType();

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                if (type == typeof(List<float>))
                {
                    foreach (float val in value.GetValue<List<float>>())
                    {
                        await EvaluateBlock(scope, variable, val);
                    }
                }
                else if (type == typeof(List<string>))
                {
                    foreach (string val in value.GetValue<List<string>>())
                    {
                        await EvaluateBlock(scope, variable, val);
                    }
                }
            }

            return new DynamicReturnValue(null);
        }

        public static ForStatementNode Parse(AstTreeNode lastNode, ScriptToken scriptToken, List<ScriptToken> tokens)
        {
            if (tokens[1].Type != EScriptTokenType.L_PAREN)
            {
                Console.WriteLine("Syntax error: Missing ('");
                return null;
            }

            if (tokens[3].Type != EScriptTokenType.OF)
            {
                Console.WriteLine("Syntax error: Missing of");
                return null;
            }

            tokens.RemoveAt(0); // consume for
            tokens.RemoveAt(0); // consume opening paren
            List<ScriptToken> loopDef = ScriptTree.GetNextStatementTokens(tokens);
            ScriptToken variableName = loopDef[0];
            loopDef.RemoveAt(0); // consume variable name
            loopDef.RemoveAt(0); // consume of
            AstTreeNode list = ScriptTree.ProcessTokens(loopDef);
            AstTreeNode block = ScriptTree.ProcessTokens(ScriptTree.GetBlockTokens(tokens, EBlockType.BRACE, false)[0]);

            return new ForStatementNode(
                new LiteralNode<string>(variableName.Value),
                list,
                block
            );
        }
    }
}
