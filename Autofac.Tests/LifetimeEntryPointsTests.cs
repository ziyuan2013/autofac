using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Autofac.Tests
{
    [TestFixture]
    public class LifetimeEntryPointsTests
    {
        public delegate int AddOperation(int a, int b);

        public class Counter
        {
            public int Count;
        }

        public class Adder : IDisposable
        {
            Counter _counter;

            public Adder(Counter counter)
            {
                _counter = counter;
            }

            public int Add(int a, int b)
            {
                return a + b;
            }

            public void Dispose()
            {
                _counter.Count++;
            }
        }

        [Test]
        public void ExecutesOperationInNestedScope()
        {
            var container = new Container();
            container.RegisterType<Counter>().SingleInstance();
            container.RegisterType<Adder>().UnsharedInstances();

            //container.RegisterDelegate<AddOperation>(c =>
            //{
            //    var l = c.Resolve<ILifetimeScope>();
            //    return (a, b) =>
            //    {
            //        using (var inner = l.BeginLifetimeScope())
            //            return inner.Resolve<Adder>().Add(a, b);
            //    };
            //});

            container.RegisterLifetimeEntryPoint<AddOperation>((c, p) =>
                c.Resolve<Adder>().Add(p.Positional<int>(0), p.Positional<int>(1)));

            var op = container.Resolve<AddOperation>();

            var counter = container.Resolve<Counter>();
            Assert.AreEqual(0, counter.Count);

            var ret = op(1, 2);

            Assert.AreEqual(3, ret);

            Assert.AreEqual(1, counter.Count);
        }

        [Test]
        public void AppliesNameToNestedScope()
        {
            var container = new Container();
            container.RegisterType<Counter>().SingleInstance();
            container.RegisterType<Adder>().InstancePer("add-op");
            container.RegisterLifetimeEntryPoint<AddOperation>("add-op", (c, p) =>
                c.Resolve<Adder>().Add(p.Positional<int>(0), p.Positional<int>(1)));

            // Will throw if scope is not correctly named
            var ret = container.Resolve<AddOperation>().Invoke(1, 2);
            Assert.AreEqual(3, ret);
        }
    }
}
