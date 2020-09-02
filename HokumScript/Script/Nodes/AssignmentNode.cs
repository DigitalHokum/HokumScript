using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HokumScript.Script.Nodes
{
    public class AssignmentNode : AstTreeNode
    {
        public readonly IScopeMemberNode RootNode;
        public readonly AstTreeNode ToAssign;

        public AssignmentNode(
            IScopeMemberNode rootNode,
            AstTreeNode toAssign
        )
        {
            RootNode = rootNode;
            ToAssign = toAssign;
        }

        public async Task<DynamicReturnValue> Evaluate(Scope scope)
        {
            string name = await RootNode.GetName(scope);
            DynamicReturnValue value = await ToAssign.Evaluate(scope);

            Scope innerScope = await RootNode.GetScope(scope);            

            innerScope.Set(name, value.Value);
            if (innerScope.Get(name) != value.Value)
                Console.WriteLine($"System Error: Failed to assign ${name} to ${value}");   

            return value;
        }

        public static AssignmentNode Parse(AstTreeNode lastNode, ScriptToken scriptToken, List<ScriptToken> tokens) {
            if (!(lastNode is IScopeMemberNode)) {
                Console.WriteLine("Invalid assignment syntax.");
                return null;
            }
            tokens.RemoveAt(0); // consume =
            List<ScriptToken> assignmentTokens = ScriptTree.GetNextStatementTokens(tokens, false);
            return new AssignmentNode(
                (IScopeMemberNode) lastNode,
                ScriptTree.ProcessTokens(assignmentTokens)
            );
        }
    }
}
