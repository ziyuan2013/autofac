using System;
using Autofac.Builder;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace Autofac.Tests.Builder
{
    [TestFixture]
    public class RegisterClosedTypesOfFixture
    {
        #region Test Types

        /// <summary>
        /// An open generic interface type.
        /// </summary>
        public interface ICommand<T>
        {
            void Execute(T data);
        }

        /// <summary>
        /// An abstract base class that implements the open generic 
        /// interface type.
        /// </summary>
        public abstract class CommandBase<T> : ICommand<T>
        {
            public abstract void Execute(T data);
        }

        /// <summary>
        /// A type to use as a generic parameter.
        /// </summary>
        public class SaveCommandData
        {
        }

        /// <summary>
        /// A type to use as a generic parameter.
        /// </summary>
        public class DeleteCommandData
        {
        }

        /// <summary>
        /// A command class that implements the open generic interface 
        /// type by inheriting from the abstract base class.
        /// </summary>
        public class DeleteCommand : CommandBase<DeleteCommandData>
        {
            public override void Execute(DeleteCommandData data)
            {
            }
        }

        /// <summary>
        /// A command class that directly implements the open 
        /// generic interface type.
        /// </summary>
        public class SaveCommand : ICommand<SaveCommandData>
        {
            public void Execute(SaveCommandData data)
            {
            }
        }

        /// <summary>
        /// An abstract open generic base class.
        /// </summary>
        public abstract class Message<T>
        {
        }

        /// <summary>
        /// A class that closed the open generic type.
        /// </summary>
        public class StringMessage : Message<string>
        {
        }

        #endregion
        
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RegisterClosedTypesOf_NullTypeProvided_ThrowsException()
        {
            var builder = new ContainerBuilder();
            var assembly = typeof(ICommand<>).Assembly;

            builder.RegisterClosedTypesOf(null, assembly);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RegisterClosedTypesOf_NullAssemblyProvided_ThrowsException()
        {
            var builder = new ContainerBuilder();

            builder.RegisterClosedTypesOf(typeof(ICommand<>), null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void RegisterClosedTypesOf_NonGenericTypeProvided_ThrowsException()
        {
            var builder = new ContainerBuilder();
            var assembly = typeof(ICommand<>).Assembly;

            builder.RegisterClosedTypesOf(typeof(SaveCommandData), assembly);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void RegisterClosedTypesOf_ClosedGenericTypeProvided_ThrowsException()
        {
            var builder = new ContainerBuilder();
            var assembly = typeof(ICommand<>).Assembly;

            builder.RegisterClosedTypesOf(typeof(ICommand<SaveCommandData>), assembly);
        }

        [Test]
        public void RegisterClosedTypesOf_OpenGenericInterfaceTypeProvided_ClosingGenericTypesRegistered()
        {
            var builder = new ContainerBuilder();
            var assembly = typeof(ICommand<>).Assembly;

            builder.RegisterClosedTypesOf(typeof(ICommand<>), assembly);
            var container = builder.Build();

            Assert.That(container.Resolve<ICommand<SaveCommandData>>(), Is.InstanceOfType(typeof(SaveCommand)));
            Assert.That(container.Resolve<ICommand<DeleteCommandData>>(), Is.InstanceOfType(typeof(DeleteCommand)));
        }

        [Test]
        public void RegisterClosedTypesOf_OpenGenericAbstractClassTypeProvided_ClosingGenericTypesRegistered()
        {
            var builder = new ContainerBuilder();
            var assembly = typeof(Message<>).Assembly;

            builder.RegisterClosedTypesOf(typeof(Message<>), assembly);
            var container = builder.Build();

            Assert.That(container.Resolve<Message<string>>(), Is.InstanceOfType(typeof(StringMessage)));
        }

        [Test]
        public void RegisterClosedTypesOf_DefaultInstanceScope_RegisteredAsSingleton()
        {
            var builder = new ContainerBuilder();
            var assembly = typeof(Message<>).Assembly;

            builder.RegisterClosedTypesOf(typeof(Message<>), assembly);
            var container = builder.Build();

            var message1 = container.Resolve<Message<string>>();
            var message2 = container.Resolve<Message<string>>();

            Assert.That(message1, Is.EqualTo(message2));
        }

        [Test]
        public void RegisterClosedTypesOf_SetDefaultScope_RegisteredAsFactory()
        {
            var builder = new ContainerBuilder();
            var assembly = typeof(Message<>).Assembly;

            using (builder.SetDefaultScope(InstanceScope.Factory))
            {
                builder.RegisterClosedTypesOf(typeof(Message<>), assembly);
            }
            var container = builder.Build();

            var message1 = container.Resolve<Message<string>>();
            var message2 = container.Resolve<Message<string>>();

            Assert.That(message1, Is.Not.EqualTo(message2));
        }
    }
}