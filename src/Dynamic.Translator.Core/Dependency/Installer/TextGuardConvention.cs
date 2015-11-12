﻿namespace Dynamic.Translator.Core.Dependency.Installer
{
    #region using

    using System.Linq;
    using Castle.Core;
    using Castle.MicroKernel;
    using Castle.MicroKernel.Facilities;
    using Interceptors;
    using Orchestrators;

    #endregion

    public class TextGuardConvention : AbstractFacility
    {
        protected override void Init()
        {
            Kernel.ComponentRegistered += KernelOnComponentRegistered;
        }

        private void KernelOnComponentRegistered(string key, IHandler handler)
        {
            var isMeanFinder = handler.ComponentModel.Implementation.GetInterfaces().Contains(typeof (IMeanFinder));
            var isObserver = handler.ComponentModel.Implementation.GetInterfaces().Any(i => i.Name.Contains("Observer"));

            if (isMeanFinder)
            {
                handler.ComponentModel.Interceptors.AddFirst(new InterceptorReference(typeof (ExceptionInterceptor)));
                handler.ComponentModel.Interceptors.Add(new InterceptorReference(typeof (TextGuardInterceptor)));
            }

            if (isObserver)
            {
                handler.ComponentModel.Interceptors.AddFirst(new InterceptorReference(typeof (ObserverExceptionInterceptor)));
            }
        }
    }
}