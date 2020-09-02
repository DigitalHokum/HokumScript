using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HokumScript.Script.Nodes
{
    public class ArrayNode : AstTreeNode
    {
        private readonly List<AstTreeNode> _array;

        public ArrayNode(
            List<AstTreeNode> array
        )
        {
            _array = array;
        }

        private async Task<List<T>> BuildList<T>(Scope scope)
        {
            List<T> values = new List<T>();
            foreach (AstTreeNode val in _array)
            {
                DynamicReturnValue value = await val.Evaluate(scope);
                values.Add((T) value.Value);
            }

            return values;
        }

        public async Task<DynamicReturnValue> Evaluate(Scope scope)
        {
            DynamicReturnValue first = await _array[0].Evaluate(scope);
            Type t = first.Type;

            if (t == typeof(int))
            {
                return new DynamicReturnValue(await BuildList<int>(scope));
            }

            if (t == typeof(float))
            {
                return new DynamicReturnValue(await BuildList<float>(scope));
            }
            
            if (t == typeof(string))
            {
                return new DynamicReturnValue(await BuildList<string>(scope));
            }

            return new DynamicReturnValue(BuildList<object>(scope));
        }

        public static ArrayNode Parse(AstTreeNode lastNode, ScriptToken scriptToken, List<ScriptToken> tokens)
        {
            List<ScriptToken> values = ScriptTree.GetEnclosedTokens(tokens);
            List<AstTreeNode> nodes = new List<AstTreeNode>();

            for (int i = 0; i < values.Count; i++)
            {
                ScriptToken arg = values[i];
                if (i % 2 == 0 && arg.Type != EScriptTokenType.COMMA)
                {
                    nodes.Add(new IntegerLiteralNode(arg.Value));
                }
                else
                {
                    Console.WriteLine("Syntax Error.");
                }
            }

            return new ArrayNode(nodes);
        }
    }
}
