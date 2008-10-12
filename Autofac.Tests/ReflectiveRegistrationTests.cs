using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Autofac.Registry;
using Autofac.Services;

namespace Autofac.Tests
{
    [TestFixture]
    public class ReflectiveRegistrationTests
    {
        class A1 { }
        class A2 { }

        class TwoCtors
        {
            public Type[] CalledCtor { get; private set; }

            public TwoCtors(A1 a1)
            {
                CalledCtor = new[] { typeof(A1) };
            }

            public TwoCtors(A1 a1, A2 a2)
            {
                CalledCtor = new[] { typeof(A1), typeof(A2) };
            }
        }

        [Test]
        public void ExplicitCtorCalled()
        {
            var selected = new[] { typeof(A1), typeof(A2) };
            ResolveTwoCtorsWith(selected);
        }

        [Test]
        public void OtherExplicitCtorCalled()
        {
            var selected = new[] { typeof(A1) };
            ResolveTwoCtorsWith(selected);
        }

        void ResolveTwoCtorsWith(Type[] selected)
        {
            var c = new Container();
            c.RegisterType<A1>();
            c.RegisterType<A2>();

            c.RegisterType<TwoCtors>()
                .UsingConstructor(selected);

            var result = c.Resolve<TwoCtors>();
            Assert.IsNotNull(result);
            Assert.AreEqual(typeof(TwoCtors).GetConstructor(selected),
                typeof(TwoCtors).GetConstructor(result.CalledCtor));
        }

        [Test]
        [ExpectedException(typeof(DependencyResolutionException))]
        public void ExplicitCtorNotPresent()
        {
            var c = new Container();
            c.RegisterType<TwoCtors>()
                .UsingConstructor(typeof(A2));
            c.Resolve<TwoCtors>();
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ExplicitCtorNull()
        {
            var c = new Container();
            c.RegisterType<TwoCtors>()
                .UsingConstructor(null);
        }

        class WithParam
        {
            public int I { get; private set; }
            public WithParam(int i) { I = i; }
        }

        //[Test]
        //public void ParametersProvided()
        //{
        //    var ival = 10;

        //    var c = new Container();
        //    c.RegisterType<WithParam>().
        //        WithParameters(new NamedParameter("i", ival));

        //    var result = c.Resolve<WithParam>();
        //    Assert.IsNotNull(result);
        //    Assert.AreEqual(ival, result.I);
        //}

        class WithProp
        {
            public string Prop { get; set; }
        }

        //[Test]
        //public void PropertyProvided()
        //{
        //    var pval = "Hello";

        //    var c = new Container();
        //    c.RegisterType<WithProp>()
        //        .WithProperties(new NamedPropertyParameter("Prop", pval));

        //    var result = c.Resolve<WithProp>();
        //    Assert.IsNotNull(result);
        //    Assert.AreEqual(pval, result.Prop);
        //}

        [Test]
        public void ExposesImplementationType()
        {
            var c = new Container();
            c.RegisterType(typeof(A1)).As<object>();
            IComponentRegistration cr;
            Assert.IsTrue(c.ComponentRegistry.TryGetRegistration(
                new TypedService(typeof(object)), out cr));
            Assert.AreEqual(typeof(A1), cr.Activator.BestGuessImplementationType);
        }
    }
}
