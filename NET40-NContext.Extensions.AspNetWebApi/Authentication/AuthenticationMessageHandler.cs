﻿namespace NContext.Extensions.AspNetWebApi.Authentication
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Http.Filters;
    using Microsoft.FSharp.Core;
    using NContext.Common;
    using NContext.Extensions.AspNetWebApi.ErrorHandling;

    /// <summary>
    /// Defines an <see cref="ActionFilterAttribute"/> for authentication.
    /// </summary>
    public class AuthenticationMessageHandler : DelegatingHandler
    {
        private readonly IEnumerable<IProvideRequestAuthentication> _AuthenticationProviders;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationMessageHandler"/> class.
        /// </summary>
        /// <param name="authenticationProviders">The authentication providers.</param>
        /// <remarks></remarks>
        public AuthenticationMessageHandler(IEnumerable<IProvideRequestAuthentication> authenticationProviders)
        {
            _AuthenticationProviders = authenticationProviders ?? Enumerable.Empty<IProvideRequestAuthentication>();
        }

        /// <summary>
        /// Sends an HTTP request to the inner handler to send to the server as an asynchronous operation.
        /// </summary>
        /// <returns>
        /// Returns <see cref="T:System.Threading.Tasks.Task`1"/>. The task object representing the asynchronous operation.
        /// </returns>
        /// <param name="request">The HTTP request message to send to the server.</param><param name="cancellationToken">A cancellation token to cancel operation.</param><exception cref="T:System.ArgumentNullException">The <paramref name="request"/> was null.</exception>
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            /* If HttpContext.Current.User is authenticated, then return immediately.
             * This is typically the case when the application / IIS is configured to use
             * Windows integration with or without impersonation.
             * */
            if (HttpContext.Current != null && 
                HttpContext.Current.User != null && 
                HttpContext.Current.User.Identity.IsAuthenticated)
            {
                return base.SendAsync(request, cancellationToken);
            }

            return _AuthenticationProviders
                .FirstOrDefault(provider => provider.CanAuthenticate(request))
                .ToMaybe()
                .ToServiceResponse(() => AuthenticationError.ProviderNotFound())
                .Bind(provider => provider.Authenticate(request)
                    .Fmap(principal =>
                        {
                            // If we're hosted within IIS, set the HttpContext.Current.User
                            if (HttpContext.Current != null)
                            {
                                HttpContext.Current.User = principal;
                            }

                            Thread.CurrentPrincipal = principal;

                            return base.SendAsync(request, cancellationToken);
                        }))
                .CatchAndContinue(error =>
                    {
                        var completionSource = new TaskCompletionSource<HttpResponseMessage>();
                        completionSource.SetResult(
                            request.CreateResponse(
                                (HttpStatusCode)error.HttpStatusCode,
                                    new ErrorResponse<Unit>(error)));

                        return new DataResponse<Task<HttpResponseMessage>>(completionSource.Task);
                    })
                .FromRight();
        }
    }
}