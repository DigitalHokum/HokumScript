using System.Threading.Tasks;

namespace HokumScript.Script.Nodes
{
    public class FloatLiteralNode : LiteralNode<float> {
        public FloatLiteralNode(string value) : base(float.Parse(value)) { }
        public override async Task<DynamicReturnValue> Evaluate(Scope scope)
        {
            float value = await Task.Run(() => Value);
            return new DynamicReturnValue(value);
        }
    }
}
