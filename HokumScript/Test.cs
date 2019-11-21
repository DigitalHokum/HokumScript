using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;

namespace HokumScript
{
    public class FunctionTest
    {
        public string MyTestFunction(string whatToReturn)
        {
            Console.WriteLine(whatToReturn);
            return whatToReturn;
        }
        
        public async Task<string> MyTestAsyncFunction(string whatToReturn)
        {
            Console.WriteLine(whatToReturn);
            string value = await Task.Run(() => whatToReturn);
            return value;
        }
        
        public string MyTestFunctionWithMultipleArguments(string whatToReturn, string cheese)
        {
            return $"{whatToReturn} {cheese}";
        }
    }
    
    [TestFixture]
    public class Test
    {
        [Test]
        public void TestTokenize()
        {
            List<Token> tokens = Parser.Tokenize("1 + 5;something = false;callFunc();indice[0];block {};");
            Assert.AreEqual(26, tokens.Count);
        }
        
        [Test]
        public async Task TestNumberLiteral()
        {
            Tree tree = new Tree("5.52");
            DynamicReturnValue value = await tree.Evaluate(new Scope());
            Assert.AreEqual(5.52f, value.GetValue<float>());
            
            tree = new Tree("5");
            value = await tree.Evaluate(new Scope());
            Assert.AreEqual(5, value.GetValue<int>());
            
            tree = new Tree("490231");
            value = await tree.Evaluate(new Scope());
            Assert.AreEqual(490231, value.GetValue<int>());
        }

        [Test]
        public async Task TestBooleanLiteral()
        {
            Tree tree = new Tree("true");
            DynamicReturnValue value = await tree.Evaluate(new Scope());
            Assert.AreEqual(true, value.GetValue<bool>());
            
            tree = new Tree("false");
            value = await tree.Evaluate(new Scope());
            Assert.AreEqual(false, value.GetValue<bool>());
        }
        
        [Test]
        public async Task TestStringLiteral()
        {
            Tree tree = new Tree("'testing'");
            DynamicReturnValue value = await tree.Evaluate(new Scope());
            Assert.AreEqual("testing", value.GetValue<string>());
            
            tree = new Tree("\"testing double quotes\"");
            value = await tree.Evaluate(new Scope());
            Assert.AreEqual("testing double quotes", value.GetValue<string>());
        }
        
        [Test]
        public async Task TestRootScopeNode()
        {
            Tree tree = new Tree("foo");
            Scope scope = new Scope();
            scope.Set("foo", 5);
            DynamicReturnValue value = await tree.Evaluate(scope);
            Assert.AreEqual(5, value.GetValue<int>());
            
            tree = new Tree("foo");
            scope.Set("foo", false);
            value = await tree.Evaluate(scope);
            Assert.AreEqual(false, value.GetValue<bool>());
        }
        
        [Test]
        public async Task TestScopeNode()
        {
            Tree tree = new Tree("foo.bar;");
            Scope scope = new Scope();
            Scope innerScope = new Scope();
            scope.Set("foo", innerScope);
            innerScope.Set("bar", 5);
            DynamicReturnValue value = await tree.Evaluate(scope);
            Console.WriteLine($"result {value.GetValue<int>()}");
            Assert.AreEqual(5, value.GetValue<int>());
            
            /*
            innerScope.Set("bar", false);
            value = await tree.Evaluate(scope);
            Assert.AreEqual(false, value.GetValue<bool>());
            
            tree = new Tree("foo.baz.nitch");
            Scope lastScope = new Scope();
            lastScope.Set("nitch", 101);
            innerScope.Set("baz", lastScope);
            value = await tree.Evaluate(scope);
            Assert.AreEqual(101, value.GetValue<int>());
            */
        }
        
        [Test]
        public async Task TestScopeAssignmentNode()
        {
            Tree tree = new Tree("foo = 150");
            Scope scope = new Scope();
            await tree.Evaluate(scope);
            Assert.AreEqual(150, scope.Get("foo"));
            
            tree = new Tree("bar = 1.5;bar;");
            DynamicReturnValue value = await tree.Evaluate(scope);
            Assert.AreEqual(1.5f, value.GetValue<float>());
            
            scope = new Scope();
            tree = new Tree("foo.bar = 7.5;foo.bar;");
            Scope innerScope = new Scope();
            scope.Set("foo", innerScope);
            value = await tree.Evaluate(scope);
            Assert.AreEqual(7.5f, value.GetValue<float>());
        }
        
        [Test]
        public async Task TestIfStatementNode()
        {
            Tree tree = new Tree(@"
            if (false) {
                result = false;
            } else if (1 > 2) {
                result = 'impossibru!';
            } else if (2 > 1) {
                result = true;
            } else {
                result = 'not right';
            }
            ");
            Console.WriteLine("Tree");
            Scope scope = new Scope();
            await tree.Evaluate(scope);
            Console.WriteLine($"Result it Type {scope.GetType("result")}");
            Assert.AreEqual(true, scope.Get("result"));
        }
        
        /*
        [Test]
        public async Task TestForStatementNode()
        {
            Scope scope = new Scope();
            Scope innerScope = new Scope();
            List<int> loopTest = new List<int>();
            for (int i = 1; i <= 9; i++)
            {
                loopTest.Add(i);
            }
            scope.Set("loopTest", loopTest);
            scope.Set("test", innerScope);
            
            Tree tree = new Tree(@"
                test.foo = 1;
                foo = 1;
                for (var of loopTest) {
                    test.foo += var;
                    foo = var;
                }
            ");
           
            await tree.Evaluate(scope);
            Assert.AreEqual(46.0f, innerScope.Get<float>("foo"));
            Assert.AreEqual(9.0f, scope.Get<float>("foo"));
        }
        */
        
        [Test]
        public async Task TestFunctionCallNode()
        {
            Function function = new Function(typeof(FunctionTest), "MyTestFunction");
            Tree tree = new Tree(@"test_function(words)");
            Scope scope = new Scope();
            string words = "pancakes are tasty.";
            scope.Set("words", words);
            scope.Set("test_function", function);
            DynamicReturnValue r = await tree.Evaluate(scope);
            Assert.AreEqual(words, r.GetValue<string>());
        }
        
        [Test]
        public async Task TestFunctionMultipleArgumentsCallNode()
        {
            Function function = new Function(typeof(FunctionTest), "MyTestFunctionWithMultipleArguments");
            Tree tree = new Tree(@"test_function(words, 'Cheese is also rather tasty.')");
            Scope scope = new Scope();
            string words = "pancakes are tasty.";
            scope.Set("words", words);
            scope.Set("test_function", function);
            DynamicReturnValue r = await tree.Evaluate(scope);
            Assert.AreEqual($"{words} Cheese is also rather tasty.", r.GetValue<string>());
        }
        
        [Test]
        public async Task TestAsyncFunctionCallNode()
        {
            Function function = new Function(typeof(FunctionTest), "MyTestAsyncFunction");
            Tree tree = new Tree(@"test_function(words)");
            Scope scope = new Scope();
            string words = "pancakes are super tasty.";
            scope.Set("words", words);
            scope.Set("test_function", function);
            DynamicReturnValue r = await tree.Evaluate(scope);
            Assert.AreEqual(words, r.GetValue<string>());
        }
    }
}
