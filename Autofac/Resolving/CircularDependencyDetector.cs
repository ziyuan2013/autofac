using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using Autofac.Services;

namespace Autofac.Resolving
{
    class CircularDependencyDetector
    {
        string CreateDependencyGraphTo(Service service, Stack<ComponentActivation> activationStack)
        {
            Enforce.ArgumentNotNull(service, "service");
            Enforce.ArgumentNotNull(activationStack, "activationStack");

            string dependencyGraph = service.Description;

            foreach (Service requestor in activationStack.Select(a => a.RequestedService))
                dependencyGraph = requestor.Description + " -> " + dependencyGraph;

            return dependencyGraph;
        }

        public void CheckForCircularDependency(Service service, Stack<ComponentActivation> activationStack)
        {
            Enforce.ArgumentNotNull(service, "service");
            Enforce.ArgumentNotNull(activationStack, "activationStack");

            if (IsCircularDependency(service, activationStack))
                throw new DependencyResolutionException(string.Format(CultureInfo.CurrentCulture,
                    CircularDependencyDetectorResources.CircularDependency, CreateDependencyGraphTo(service, activationStack)));
        }

        bool IsCircularDependency(Service service, Stack<ComponentActivation> activationStack)
        {
            Enforce.ArgumentNotNull(service, "service");
            Enforce.ArgumentNotNull(activationStack, "activationStack");
            return activationStack.Count(a => a.RequestedService == service) >= 1;
        }
    }
}
