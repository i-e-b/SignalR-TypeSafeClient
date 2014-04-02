namespace TypeSafeClientTests
{
    using System;
    using System.Linq.Expressions;
    using Management.WorkerRoleContainer.Implementations.Reflection;
    using TypeSafeClient.Reflection;

    [TestFixture]
    public class ExpressionToolsTests
    {
        private readonly object _three = "three";

        public interface ISampleInterface
        {
            void PlainMethod();
            void ComplexMethod(int i, string two, object three);
            int Returns(int i);
        }

        public class SampleClass
        {
            public void PlainMethod() { }
            public void ComplexMethod(int i, string two, object three) { }
            public int Returns(int i) { return i; }
        }

        [Test]
        public void can_get_invocation_name_from_interface()
        {
            var result = ExpressionTools.GetInvocation<ISampleInterface>(i => i.PlainMethod());

            Assert.AreEqual("PlainMethod", result.MethodName);
            Assert.AreEqual(0, result.ParameterValues.Length);
        }

        [Test]
        public void can_get_invocation_name_and_parameters_from_interface()
        {
            var result = ExpressionTools.GetInvocation<ISampleInterface>(
                i => i.ComplexMethod(1, "two", _three)
                );

            Assert.AreEqual("ComplexMethod", result.MethodName);
            Assert.AreEqual(1, result.ParameterValues[0]);
            Assert.AreEqual("two", result.ParameterValues[1]);
            Assert.AreEqual(_three, result.ParameterValues[2]);
        }

        [Test]
        public void can_get_invocation_name_and_parameters_from_returning_functions_on_interfaces()
        {
            var result = ExpressionTools.GetInvocation<ISampleInterface>(i => i.Returns(1));

            Assert.AreEqual("Returns", result.MethodName);
            Assert.AreEqual(1, result.ParameterValues[0]);
        }

        [Test]
        public void variable_values_are_returned_as_at_call_time()
        {
            // ReSharper disable AccessToModifiedClosure
            var v = 1;
            var result = ExpressionTools.GetInvocation<ISampleInterface>(i => i.Returns(v));
            v = 2;
            // ReSharper restore AccessToModifiedClosure

            Assert.AreEqual("Returns", result.MethodName);
            Assert.AreEqual(1, result.ParameterValues[0]);
            Assert.AreNotEqual(v, result.ParameterValues[0]);
        }

        [Test]
        public void can_get_invocation_name_from_class()
        {
            var result = ExpressionTools.GetInvocation<SampleClass>(i => i.PlainMethod());

            Assert.AreEqual("PlainMethod", result.MethodName);
            Assert.AreEqual(0, result.ParameterValues.Length);
        }

        [Test]
        public void can_get_invocation_name_and_parameters_from_class()
        {
            var result = ExpressionTools.GetInvocation<SampleClass>(
                i => i.ComplexMethod(1, "two", _three)
                );

            Assert.AreEqual("ComplexMethod", result.MethodName);
            Assert.AreEqual(1, result.ParameterValues[0]);
            Assert.AreEqual("two", result.ParameterValues[1]);
            Assert.AreEqual(_three, result.ParameterValues[2]);
        }

        [Test]
        public void can_get_invocation_name_and_parameters_from_returning_functions_on_class()
        {
            var result = ExpressionTools.GetInvocation<SampleClass>(i => i.Returns(1));

            Assert.AreEqual("Returns", result.MethodName);
            Assert.AreEqual(1, result.ParameterValues[0]);
        }

        [Test]
        public void can_get_method_name_and_parameter_types_for_a_method_for_binding()
        {
            var result = ExpressionTools.GetBinding(E(i => i.ComplexMethod));

            Assert.AreEqual("ComplexMethod", result.MethodName);
            Assert.AreEqual(typeof(int), result.ParameterTypes[0]);
            Assert.AreEqual(typeof(string), result.ParameterTypes[1]);
            Assert.AreEqual(typeof(object), result.ParameterTypes[2]);
        }

        static LambdaExpression E(Expression<Func<ISampleInterface, Action<int, string, object>>> e)
        {
            return e;
        }
    }
}