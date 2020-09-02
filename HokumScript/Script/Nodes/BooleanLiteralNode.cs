using System.Threading.Tasks;

namespace HokumScript.Script.Nodes
{
    class BooleanLiteralNode : LiteralNode<bool>
    {
        public BooleanLiteralNode(string value) : base(value == "true") {}

        public override async Task<DynamicReturnValue> Evaluate(Scope scope)
        {
            bool value = await Task.Run(() => Value);
            return new DynamicReturnValue(value);
        }
    }
}
