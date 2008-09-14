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
        /// <summary>
        /// Catch circular dependencies that are triggered by post-resolve processing (e.g. 'OnActivated')
        /// </summary>
        const int MaxResolveDepth = 100;

        string CreateDependencyGraphTo(IComponentRegistration registration, Stack<ComponentActivation> activationStack)
        {
            Enforce.ArgumentNotNull(registration, "registration");
            Enforce.ArgumentNotNull(activationStack, "activationStack");

            string dependencyGraph = registration.ToString();

            foreach (IComponentRegistration requestor in activationStack.Select(a => a.Registration))
                dependencyGraph = requestor.ToString() + " -> " + dependencyGraph;

            return dependencyGraph;
        }

        public void CheckForCircularDependency(IComponentRegistration registration, Stack<ComponentActivation> activationStack, int callDepth)
        {
            Enforce.ArgumentNotNull(registration, "registration");
            Enforce.ArgumentNotNull(activationStack, "activationStack");

            if (callDepth > MaxResolveDepth)
                throw new DependencyResolutionException(string.Format(CultureInfo.CurrentCulture,
                    CircularDependencyDetectorResources.MaxDepthExceeded, registration));

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
