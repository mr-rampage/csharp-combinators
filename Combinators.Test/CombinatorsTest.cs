using System;
using FsCheck;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Combinators.Test
{
    [TestClass]
    public class CombinatorsTest
    {
        [TestMethod]
        public void TestIdentity()
        {
            Prop
                .ForAll<object>(input => input == input.Thrush(Combinator.Identity))
                .QuickCheckThrowOnFailure();
        }

        [TestMethod]
        public void TestConstant()
        {
            static object SideEffect(dynamic input) => input;
            
            Prop
                .ForAll<object>(input => Combinator.Constant(true)(SideEffect(input)))
                .QuickCheckThrowOnFailure();
        }

        [TestMethod]
        public void TestApply()
        {
            Prop
                .ForAll<NonNull<object>>(input => 
                    input.GetType() == input.Thrush(Combinator.Apply((object x) => x.GetType()))
                ).QuickCheckThrowOnFailure();
        }

        [TestMethod]
        public void TestThrush()
        {
            Prop
                .ForAll<object>(input => 
                    input == Combinator.Thrush<object, object>(input)(Combinator.Identity)
                ).QuickCheckThrowOnFailure();
        }

        [TestMethod]
        public void TestDuplication()
        {
            Prop
                .ForAll<NonNull<object>>(input =>
                    input + input.ToString() ==
                    input.Thrush(Combinator.Duplication<object, string>(a => b => a.ToString() + b))
                ).QuickCheckThrowOnFailure();
        }

        [TestMethod]
        public void TestFlip()
        {
            var tee = Combinator.Flip<object, string, object>(Combinator.Constant);
            Prop
                .ForAll<NonNull<object>>(input =>
                {
                    var sideEffect = Combinator.Thrush<object, string>(input);
                    return input.Equals(input.Thrush(tee(sideEffect(x => x.ToString()))));
                }).QuickCheckThrowOnFailure();
        }

        [TestMethod]
        public void TestCompose()
        {
            var toTuple = new Func<object, Tuple<object, object>>(thing => Tuple.Create(thing, thing));
            var first = new Func<Tuple<object, object>, object>(things => things.Item1);
            Prop
                .ForAll<object>(input =>
                    input == Combinator.Compose<object, Tuple<object, object>, object>(first)(toTuple)(input)
                ).QuickCheckThrowOnFailure();
                
        }
        
        [TestMethod]
        public void TestPipe()
        {
            var toTuple = new Func<object, Tuple<object, object>>(thing => Tuple.Create(thing, thing));
            var first = new Func<Tuple<object, object>, object>(things => things.Item1);
            Prop
                .ForAll<object>(input =>
                    input == Combinator.Pipe<object, Tuple<object, object>, object>(toTuple)(first)(input)
                ).QuickCheckThrowOnFailure();
        }

        [TestMethod]
        public void TestSubstitution()
        {
            var calculateTax = new Func<double, double>(price => price * 0.15);
            var calculateTotal = new Func<double, Func<double, double>>(tax => price => price + tax);
            var calculatePayment = Combinator.Substitution(calculateTotal)(calculateTax);
            Prop
                .ForAll<double>(price =>
                    (price * 1.15 - calculatePayment(price) < 10e-4)
                    .When(price is not double.NaN)
                    .When(price is not double.PositiveInfinity)
                    .When(price is not double.NegativeInfinity)
                    .When(price is not < 10e-4)
                    .When(price is not > 10e10)
                ).QuickCheckThrowOnFailure();

        }
    }
}