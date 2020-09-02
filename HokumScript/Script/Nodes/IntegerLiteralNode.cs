using System;
using System.Threading.Tasks;

namespace HokumScript.Script.Nodes
{
    public class IntegerLiteralNode : LiteralNode<int>
    {
        public IntegerLiteralNode(string value) : base(Int32.Parse(value)) { }
        public override async Task<DynamicReturnValue> Evaluate(Scope scope)
        {
            int value = await Task.Run(() => Value);
            return new DynamicReturnValue(value);
        }
    }
}
