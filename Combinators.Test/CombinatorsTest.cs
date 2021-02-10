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
                .ForAll<NonNull<object>>(input => 
                    Combinator.Identity<Type>()(input.GetType()) == Combinator.Identity<object>()(input).GetType())
                .Label("Should be commutative and associative")
                .QuickCheckThrowOnFailure();
            
            Prop
                .ForAll<object>(input => 
                    Combinator.Identity<object>()(Combinator.Identity<object>()(input)) == input)
                .Label("Should be idempotent")
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
                    input == Combinator.Thrush<object, object>(input)(Combinator.Identity<object>())
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
            var calculateTax = new Func<Price, Tax>(price => new Tax(price.Value * 0.15));
            var calculateTotal = new Func<Price, Func<Tax, double>>(price => tax => price.Value + tax.Value);
            var calculatePayment = Combinator.Substitution(calculateTotal)(calculateTax);
            Prop
                .ForAll<double>(price =>
                    (price * 1.15 - calculatePayment(new Price(price)) < 10e-4)
                    .When(price is not double.NaN)
                    .When(price is not double.PositiveInfinity)
                    .When(price is not double.NegativeInfinity)
                    .When(price is not < 10e-4)
                    .When(price is not > 10e10)
                ).QuickCheckThrowOnFailure();
        }
        
        [TestMethod]
        public void TestChain()
        {
            var calculateTax = new Func<Price, Tax>(price => new Tax(price.Value * 0.15));
            var calculateTotal = new Func<Tax, Func<Price, double>>(tax => price => price.Value + tax.Value);
            var calculatePayment = Combinator.Chain(calculateTotal)(calculateTax);
            Prop
                .ForAll<double>(price =>
                    (price * 1.15 - calculatePayment(new Price(price)) < 10e-4)
                    .When(price is not double.NaN)
                    .When(price is not double.PositiveInfinity)
                    .When(price is not double.NegativeInfinity)
                    .When(price is not < 10e-4)
                    .When(price is not > 10e10)
                ).QuickCheckThrowOnFailure();
        }

        [TestMethod]
        public void TestConverge()
        {
            var calculateTax = new Func<double, Tax>(price => new Tax(price * 0.15));
            var calculateDiscount= new Func<double, Discount>(price => new Discount(price * 0.1));
            var calculateTotal = new Func<Discount, Func<Tax, double>>(d => t => d.Value + t.Value);
            var calculatePayment = Combinator.Converge<double, Discount, Tax, double>(calculateTotal)(calculateDiscount)(calculateTax);
            Prop
                .ForAll<double>(price =>
                    (price * 0.15 + price * 0.1 - calculatePayment(price) < 10e-4)
                    .When(price is not double.NaN)
                    .When(price is not double.PositiveInfinity)
                    .When(price is not double.NegativeInfinity)
                    .When(price is not < 10e-4)
                    .When(price is not > 10e10)
                ).QuickCheckThrowOnFailure();
        }

        [TestMethod]
        public void TestPsi()
        {
            var money = new Func<double, Money>(value => new Money(value));
            var calculateTotal = new Func<Money, Func<Money, Net>>(current => deduction => new Net(current.Value - deduction.Value));
            var calculateNet = Combinator.Psi<double, Money, Net>(calculateTotal)(money);
            
            Prop
                .ForAll<double>(price =>
                    (price - 0.1 - calculateNet(price)(0.1).Value < 10e-4)
                    .When(price is not double.NaN)
                    .When(price is not double.PositiveInfinity)
                    .When(price is not double.NegativeInfinity)
                    .When(price is not < 10e-4)
                    .When(price is not > 10e10)
                ).QuickCheckThrowOnFailure();
        }
    }

    internal record Money(double Value);

    internal record Tax(double Value);

    internal record Price(double Value);

    internal record Discount(double Value);

    internal record Net(double Value);
}