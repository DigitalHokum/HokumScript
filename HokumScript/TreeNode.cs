using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HokumScript
{
    public interface AstTreeNode {
        Task<DynamicReturnValue> Evaluate(Scope scope);
    }
    
    public class BlockNode : AstTreeNode
    {
        public readonly List<AstTreeNode> Statements;
            
        public BlockNode(List<AstTreeNode> statements)
        {
            Statements = statements;
        }

        public async Task<DynamicReturnValue> Evaluate(Scope scope) {
            DynamicReturnValue returnValue = new DynamicReturnValue(null);
            
            for (int i = 0; i < Statements.Count; i++) {
                returnValue = await Statements[i].Evaluate(scope);
            }
            
            return returnValue;
        }
    }
    
    class ComparisonNode : AstTreeNode
    {
        public readonly AstTreeNode Left;
        public readonly AstTreeNode Right;
        public readonly ETokenType Type;

        public ComparisonNode(
            AstTreeNode left,
            AstTreeNode right,
            ETokenType type
        )
        {
            Left = left;
            Right = right;
            Type = type;
        }

        public async Task<DynamicReturnValue> Evaluate(Scope scope) {
            DynamicReturnValue left = await Left.Evaluate(scope);
            DynamicReturnValue right = await Right.Evaluate(scope);

            switch (Type) {
                case ETokenType.EQUALS:
                    return new DynamicReturnValue(left == right);
                case ETokenType.NOT_EQUALS:
                    return new DynamicReturnValue(left != right);
                case ETokenType.GREATER_THAN:
                    return new DynamicReturnValue(left > right);
                case ETokenType.LESS_THAN:
                    return new DynamicReturnValue(left < right);
                case ETokenType.GREATER_THAN_EQUAL:
                    return new DynamicReturnValue(left >= right);
                case ETokenType.LESS_THAN_EQUAL:
                    return new DynamicReturnValue(left <= right);
            }

            return new DynamicReturnValue(false);
        }

        public static bool Matches(List<Token> tokens) {
            List<ETokenType> types = new List<ETokenType>();
            types.Add(ETokenType.EQUALS);
            types.Add(ETokenType.NOT_EQUALS);
            types.Add(ETokenType.GREATER_THAN);
            types.Add(ETokenType.LESS_THAN);
            types.Add(ETokenType.GREATER_THAN_EQUAL);
            types.Add(ETokenType.LESS_THAN_EQUAL);

            return types.Contains(tokens[0].Type);
        }

        public static ComparisonNode Parse(AstTreeNode lastNode, Token token, List<Token> tokens) {
            tokens.RemoveAt(0); // Remove comparison operator
            return new ComparisonNode(lastNode, Tree.ProcessTokens(Tree.GetNextStatementTokens(tokens)), token.Type);
        }
    }

    public class ConditionalNode : AstTreeNode
    {
        public readonly AstTreeNode Condition;
        public readonly BlockNode Block;
        
        public ConditionalNode(AstTreeNode condition, BlockNode block)
        {
            Condition = condition;
            Block = block;
        }

        public async Task<DynamicReturnValue> Evaluate(Scope scope) {
            DynamicReturnValue returnValue = await this.Condition.Evaluate(scope);
            if (!returnValue.IsNull()) {
                DynamicReturnValue two = await this.Block.Evaluate(scope);
                return two;
            }
            return returnValue;
        }
    }
    
    public class LiteralNode<T> : AstTreeNode
    {
        public readonly T Value;

        public LiteralNode(
            T value
        )
        {
            Value = value;
        }

        public virtual async Task<DynamicReturnValue> Evaluate(Scope scope)
        {
            T value = await Task.Run(() => Value);
            return new DynamicReturnValue(value);
        }
    }

    class BooleanLiteralNode : LiteralNode<bool>
    {
        public BooleanLiteralNode(string value) : base(value == "true") {}
        
        public override async Task<DynamicReturnValue> Evaluate(Scope scope)
        {
            bool value = await Task.Run(() => Value);
            return new DynamicReturnValue(value);
        }
    }

    public class IntegerLiteralNode : LiteralNode<int>
    {
        public IntegerLiteralNode(string value) : base(Int32.Parse(value)) { }
        public override async Task<DynamicReturnValue> Evaluate(Scope scope)
        {
            int value = await Task.Run(() => Value);
            return new DynamicReturnValue(value);
        }
    }
    
    public class FloatLiteralNode : LiteralNode<float> {
        public FloatLiteralNode(string value) : base(float.Parse(value)) { }
        public override async Task<DynamicReturnValue> Evaluate(Scope scope)
        {
            float value = await Task.Run(() => Value);
            return new DynamicReturnValue(value);
        }
    }
    
    public class StringNode : AstTreeNode
    {
        public readonly AstTreeNode Node;

        public StringNode(AstTreeNode node)
        {
            Node = node;
        }

        public async Task<DynamicReturnValue> Evaluate(Scope scope)
        {
            dynamic value = await Node.Evaluate(scope);
            string str = $"{value}";
            return new DynamicReturnValue(str);
        }
    }

    public interface IScopeMemberNode
    {
        Task<string> GetName(Scope scope);
        Task<Scope> GetScope(Scope scope);
        Task<DynamicReturnValue> Evaluate(Scope scope);
    }

    public class ScopeMemberNode : AstTreeNode, IScopeMemberNode {
        public readonly AstTreeNode Scope;
        public readonly AstTreeNode Name;

        public ScopeMemberNode(
            AstTreeNode scope,
            AstTreeNode name
        )
        {
            Scope = scope;
            Name = name;
        }

        public async Task<DynamicReturnValue> Evaluate(Scope scope) {
            
            DynamicReturnValue parent = await Scope.Evaluate(scope);

            if (parent.IsNull())
            {
                Console.WriteLine("ERROR ERROR BEEP BOOP");
                return new DynamicReturnValue(null);
            }
            string name = await GetName(scope);
            return new DynamicReturnValue(parent.GetValue<Scope>().Get(name));
        }

        public async Task<string> GetName(Scope scope)
        {
            return (await Name.Evaluate(scope)).GetValue<string>();
        }
        
        public async Task<Scope> GetScope(Scope scope)
        {
            return (await Scope.Evaluate(scope)).GetValue<Scope>();
        }
    }
    
    public class RootScopeMemberNode : AstTreeNode, IScopeMemberNode {
        public readonly AstTreeNode Name;

        public RootScopeMemberNode(
            AstTreeNode name
        )
        {
            Name = name;
        }

        public async Task<DynamicReturnValue> Evaluate(Scope scope)
        {
            string key = await GetName(scope);
            return new DynamicReturnValue(scope.Get(key));
        }

        public async Task<string> GetName(Scope scope)
        {
            return (await Name.Evaluate(scope)).GetValue<string>();
        }
        
        public async Task<Scope> GetScope(Scope scope)
        {
            return await Task.Run(() => scope);
        }
    }
    
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

        public static AssignmentNode Parse(AstTreeNode lastNode, Token token, List<Token> tokens) {
            if (!(lastNode is IScopeMemberNode)) {
                Console.WriteLine("Invalid assignment syntax.");
                return null;
            }
            tokens.RemoveAt(0); // consume =
            List<Token> assignmentTokens = Tree.GetNextStatementTokens(tokens, false);
            return new AssignmentNode(
                (IScopeMemberNode) lastNode,
                Tree.ProcessTokens(assignmentTokens)
            );
        }
    }
    
    public class IfStatementNode : AstTreeNode
    {
        protected List<ConditionalNode> Nodes;
        
        public IfStatementNode(
            List<ConditionalNode> nodes
        )
        {
            Nodes = nodes;
        }

        public async Task<DynamicReturnValue> Evaluate(Scope scope) {
            foreach (ConditionalNode condition in Nodes)
            {
                DynamicReturnValue uno = await condition.Condition.Evaluate(scope);
                if (uno.Value != null && (bool) uno.Value) {
                    DynamicReturnValue dose = await condition.Block.Evaluate(scope);
                    return dose;
                }
            }
            
            return new DynamicReturnValue(null);
        }

        public static ConditionalNode ParseConditional(List<Token> tokens) {
            List<ETokenType> ifTokens = new List<ETokenType>();
            ifTokens.Add(ETokenType.IF);
            ifTokens.Add(ETokenType.ELSE_IF);
            
            if (!ifTokens.Contains(tokens[0].Type))  {
                Console.WriteLine("Invalid Syntax");
                return null;
            }

            tokens.RemoveAt(0); // consume if and else if
            return new ConditionalNode(
                Tree.ProcessTokens(Tree.GetBlockTokens(tokens, EBlockType.PAREN, false)[0]),
                Tree.ProcessTokens(Tree.GetBlockTokens(tokens, EBlockType.BRACE, false)[0])
            );
        }

        public static IfStatementNode Parse(AstTreeNode lastNode, Token token, List<Token> tokens) {
            if (tokens[1].Type != ETokenType.L_PAREN) {
                Console.WriteLine("If statement needs to be followed by a condition encased in parenthesis.");
                return null;
            }
            List<ConditionalNode> nodes = new List<ConditionalNode>(); 
            nodes.Add(ParseConditional(tokens));

            while(tokens.Count > 0 && ETokenType.ELSE_IF == tokens[0].Type) {
                nodes.Add(ParseConditional(tokens));
            }

            if (tokens.Count > 0 && ETokenType.ELSE == tokens[0].Type) {
                tokens.RemoveAt(0); // Consume else
                nodes.Add(new ConditionalNode(
                    new LiteralNode<bool>(true),
                    Tree.ProcessTokens(Tree.GetBlockTokens(tokens, EBlockType.BRACE, false)[0])
                ));
            }

            return new IfStatementNode(nodes);
        }
    }
    
    public class FunctionCallNode<T> : AstTreeNode
    {
        public readonly AstTreeNode Function;
        public readonly FunctionArgumentNode Arguments;

        public FunctionCallNode(
            AstTreeNode fnc,
            FunctionArgumentNode args
        )
        {
            Function = fnc;
            Arguments = args;
        }

        public async Task<DynamicReturnValue> Evaluate(Scope scope)
        {
            object[] values = (await Arguments.Evaluate(scope)).GetValue<object[]>();
            
            return (await Function.Evaluate(scope));
        }
    }

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

        public static ForStatementNode Parse(AstTreeNode lastNode, Token token, List<Token> tokens)
        {
            if (tokens[1].Type != ETokenType.L_PAREN)
            {
                Console.WriteLine("Syntax error: Missing ('");
                return null;
            }

            if (tokens[3].Type != ETokenType.OF)
            {
                Console.WriteLine("Syntax error: Missing of");
                return null;
            }

            tokens.RemoveAt(0); // consume for
            List<Token> loopDef = Tree.GetNextStatementTokens(tokens);
            Token variableName = loopDef[0];
            loopDef.RemoveAt(0);
            loopDef.RemoveAt(0); // consume of
            AstTreeNode list = Tree.ProcessTokens(loopDef);
            AstTreeNode block = Tree.ProcessTokens(Tree.GetBlockTokens(tokens, EBlockType.BRACE, false)[0]);

            return new ForStatementNode(
                new LiteralNode<string>(variableName.Value),
                list,
                block
            );
        }
    }
    
    public class ArithmeticNode : AstTreeNode
    {
        public readonly AstTreeNode Left;
        public readonly AstTreeNode Right;
        public readonly ETokenType Type;

        ArithmeticNode(
            AstTreeNode left,
            AstTreeNode right,
            ETokenType type
        )
        {
            Left = left;
            Right = right;
            Type = type;
        }

        public async Task<DynamicReturnValue> Evaluate(Scope scope) {
            DynamicReturnValue left = await Left.Evaluate(scope);
            DynamicReturnValue right = await Right.Evaluate(scope);
            
            switch (Type)
            {
                case ETokenType.ADD:
                    return new DynamicReturnValue(left.GetValue<float>() + right.GetValue<float>());
                case ETokenType.SUBTRACT:
                    return new DynamicReturnValue(left.GetValue<float>() - right.GetValue<float>());
                case ETokenType.MULTIPLY:
                    return new DynamicReturnValue(left.GetValue<float>() * right.GetValue<float>());
                case ETokenType.DIVIDE:
                    return new DynamicReturnValue(left.GetValue<float>() / right.GetValue<float>());
            }
            
            return new DynamicReturnValue(null);
        }

        public static bool Matches(List<Token> tokens) {
            List<ETokenType> types = new List<ETokenType>();
            types.Add(ETokenType.ADD);
            types.Add(ETokenType.SUBTRACT);
            types.Add(ETokenType.MULTIPLY);
            types.Add(ETokenType.DIVIDE);
            return types.Contains(tokens[0].Type);
        }

        public static ArithmeticNode Parse(AstTreeNode lastNode, Token token, List<Token> tokens) {
            tokens.RemoveAt(0); // Remove arithmetic operator
            return new ArithmeticNode(lastNode, Tree.ProcessTokens(Tree.GetNextStatementTokens(tokens)), token.Type);
        }
    }

    public class ArithmeticAssignmentNode : AstTreeNode
    {
        public readonly IScopeMemberNode Left;
        public readonly AstTreeNode Right;
        public readonly ETokenType Type;

        public ArithmeticAssignmentNode(
            IScopeMemberNode left,
            AstTreeNode right,
            ETokenType type
        )
        {
            Left = left;
            Right = right;
            Type = type;
        }

        public async Task<DynamicReturnValue> Evaluate(Scope scope) {
            string name = await Left.GetName(scope);
            DynamicReturnValue left = await Left.Evaluate(scope);
            DynamicReturnValue right = await Right.Evaluate(scope);
            float value = 0;
            
            switch (Type) {
                case ETokenType.ADD_ASSIGN:
                    value = left.GetValue<float>() + right.GetValue<float>();
                    break;
                case ETokenType.SUBTRACT_ASSIGN:
                    value = left.GetValue<float>() - right.GetValue<float>();
                    break;
                case ETokenType.MULTIPLY_ASSIGN:
                    value = left.GetValue<float>() * right.GetValue<float>();
                    break;
                case ETokenType.DIVIDE_ASSIGN:
                    value = left.GetValue<float>()/ right.GetValue<float>();
                    break;
            }

            Scope innerScope = await Left.GetScope(scope);
            innerScope.Set(name, value);

            if ((float)innerScope.Get(name) != value)
            {
                Console.WriteLine("System Error: Failed to assign ${name} to ${right}");
                return new DynamicReturnValue(null);
            }

            return new DynamicReturnValue(value);
        }

        public static bool Matches(List<Token> tokens) {
            List<ETokenType> types = new List<ETokenType>();
            types.Add(ETokenType.ADD_ASSIGN);
            types.Add(ETokenType.SUBTRACT_ASSIGN);
            types.Add(ETokenType.MULTIPLY_ASSIGN);
            types.Add(ETokenType.DIVIDE_ASSIGN);
            return types.Contains(tokens[0].Type);
        }

        public static ArithmeticAssignmentNode Parse(AstTreeNode lastNode, Token token, List<Token> tokens) {
            if (lastNode == null || !(lastNode is IScopeMemberNode)) {
                Console.WriteLine("Invalid assignment syntax.");
                return null;
            }
            tokens.RemoveAt(0); // consume +=
            List<Token> assignmentTokens = Tree.GetNextStatementTokens(tokens);
            return new ArithmeticAssignmentNode(
                (IScopeMemberNode) Convert.ChangeType(lastNode, lastNode is RootScopeMemberNode ? typeof(RootScopeMemberNode) : typeof(ScopeMemberNode)),
                Tree.ProcessTokens(assignmentTokens),
                token.Type
            );
        }
    }
    
    public class FunctionArgumentNode : AstTreeNode
    {
        protected List<AstTreeNode> Args;

        public FunctionArgumentNode(
            List<AstTreeNode> args
        )
        {
            Args = args;
        }

        public async Task<DynamicReturnValue> Evaluate(Scope scope) {
            List<object> arguments = new List<object>();
            foreach (AstTreeNode arg in Args) {
                arguments.Add((await arg.Evaluate(scope)).Value);
            }
            return new DynamicReturnValue(arguments.ToArray());
        }
    }
    
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
