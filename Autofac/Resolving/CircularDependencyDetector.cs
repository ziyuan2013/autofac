using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using Autofac.Services;
using Autofac.Registry;

namespace Autofac.Resolving
{
    class CircularDependencyDetector
    {
        string CreateDependencyGraphTo(IComponentRegistration registration, Stack<ComponentActivation> activationStack)
        {
            Enforce.ArgumentNotNull(registration, "registration");
            Enforce.ArgumentNotNull(activationStack, "activationStack");

            string dependencyGraph = registration.ToString();

            foreach (IComponentRegistration requestor in activationStack.Select(a => a.Registration))
                dependencyGraph = requestor.ToString() + " -> " + dependencyGraph;

            return dependencyGraph;
        }

        public void CheckForCircularDependency(IComponentRegistration registration, Stack<ComponentActivation> activationStack)
        {
            Enforce.ArgumentNotNull(registration, "registration");
            Enforce.ArgumentNotNull(activationStack, "activationStack");

            if (IsCircularDependency(registration, activationStack))
                throw new DependencyResolutionException(string.Format(CultureInfo.CurrentCulture,
                    CircularDependencyDetectorResources.CircularDependency, CreateDependencyGraphTo(registration, activationStack)));
        }

        bool IsCircularDependency(IComponentRegistration registration, Stack<ComponentActivation> activationStack)
        {
            Enforce.ArgumentNotNull(registration, "registration");
            Enforce.ArgumentNotNull(activationStack, "activationStack");
            return activationStack.Count(a => a.Registration == registration) >= 1;
        }
    }
}
