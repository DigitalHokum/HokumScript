using System.Threading.Tasks;

namespace HokumScript.Script.Nodes
{
    public class FunctionCallNode : AstTreeNode
    {
        public readonly AstTreeNode Fnc;
        public readonly FunctionArgumentNode Args;

        public FunctionCallNode(
            AstTreeNode fnc,
            FunctionArgumentNode args
        )
        {
            Fnc = fnc;
            Args = args;
        }

        public async Task<DynamicReturnValue> Evaluate(Scope scope)
        {
            object[] arguments = (await Args.Evaluate(scope)).GetValue<object[]>();
            Function function = (await Fnc.Evaluate(scope)).GetValue<Function>();
            if (function.IsAsyncMethod())
                return new DynamicReturnValue(await function.AsyncInvoke(arguments));
            return new DynamicReturnValue(function.Invoke(arguments));
        }
    }
}
