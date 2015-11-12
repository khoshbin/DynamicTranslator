﻿namespace Dynamic.Translator.Core.Dependency.Interceptors
{
    #region using

    using System;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using Castle.DynamicProxy;
    using Exception;
    using Orchestrators;

    #endregion

    public class ExceptionInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            try
            {
                invocation.Proceed();

                if (invocation.Method.ReturnType.GetGenericTypeDefinition() == typeof (Task<>) || invocation.Method.ReturnType == typeof (Task))
                {
                    var task = invocation.ReturnValue as Task;
                    if (task != null && task.IsFaulted)
                        invocation.ReturnValue = HandleReturnAsync(invocation, task.Exception);
                }
            }
            catch (ApiKeyNullException ex)
            {
                if (invocation.Method.ReturnType.GetGenericTypeDefinition() == typeof (Task<>))
                    invocation.ReturnValue = HandleReturnAsync(invocation, ex);
            }
            catch (MaximumCharacterLimitException ex)
            {
                if (invocation.Method.ReturnType.GetGenericTypeDefinition() == typeof (Task<>))
                    invocation.ReturnValue = HandleReturnAsync(invocation, ex);
            }
            catch (WebException ex)
            {
                if (invocation.Method.ReturnType.GetGenericTypeDefinition() == typeof (Task<>))
                    invocation.ReturnValue = HandleReturnAsync(invocation, ex);
            }
            catch (Exception ex)
            {
                invocation.ReturnValue = HandleReturnAsync(invocation, ex);
            }
        }

        private dynamic HandleReturnAsync(IInvocation invocation, Exception ex)
        {
            if (invocation.Method.ReturnType == typeof (void))
                return null;

            var retVal = new Task<TranslateResult>(() =>
                new TranslateResult(false,
                    new Maybe<string>(new StringBuilder()
                        .AppendLine("Exception Occured on:" + invocation.TargetType.Name)
                        .AppendLine(ex.Message)
                        .AppendLine(ex.InnerException?.Message ?? string.Empty).ToString())
                    ));

            retVal.Start();

            return retVal;
        }
    }
}