﻿namespace NContext.Common
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Defines extension methods for <see cref="IServiceResponse{T}"/>.
    /// </summary>
    public static class IServiceResponseExtensions
    {
        /// <summary>
        /// If <seealso cref="IServiceResponse{T}.Error" /> is not null, returns a new <see cref="IServiceResponse{T2}" /> instance with the current
        /// <seealso cref="IServiceResponse{T}.Error" />. Else, binds the <seealso cref="IServiceResponse{T}.Data" /> into the specified <paramref name="bindingFunction" />.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2">The type of the next <see cref="IServiceResponse{T2}" /> to return.</typeparam>
        /// <param name="serviceResponse">The service response.</param>
        /// <param name="bindingFunction">The binding function.</param>
        /// <returns>Instance of <see cref="IServiceResponse{T2}" />.</returns>
        public static IServiceResponse<T2> Bind<T, T2>(this IServiceResponse<T> serviceResponse, Func<T, IServiceResponse<T2>> bindingFunction)
        {
            if (serviceResponse.Error != null)
            {
                return CreateGenericErrorResponse<T, T2>(serviceResponse, serviceResponse.Error);
            }

            return bindingFunction.Invoke(serviceResponse.Data);
        }

        /// <summary>
        /// Invokes the specified action if <seealso cref="IServiceResponse{T}.Error" /> is not null. Returns the current instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceResponse">The service response.</param>
        /// <param name="action">The action to invoke.</param>
        /// <returns>The current <see cref="IServiceResponse{T}" /> instance.</returns>
        public static IServiceResponse<T> Catch<T>(this IServiceResponse<T> serviceResponse, Action<Error> action)
        {
            if (serviceResponse.Error != null)
            {
                action.Invoke(serviceResponse.Error);
            }

            return serviceResponse;
        }

        /// <summary>
        /// Invokes the specified function if <seealso cref="IServiceResponse{T}.Error" /> is not null. Allows you to re-direct control flow with a new <typeparamref name="T" /> value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceResponse">The service response.</param>
        /// <param name="continueWithFunction">The continue with function.</param>
        /// <returns>If errors exist, returns the instance of IServiceResponse{T} returned by <paramref name="continueWithFunction" />, else returns current instance.</returns>
        public static IServiceResponse<T> CatchAndContinue<T>(this IServiceResponse<T> serviceResponse, Func<Error, IServiceResponse<T>> continueWithFunction)
        {
            if (serviceResponse.Error != null)
            {
                return continueWithFunction.Invoke(serviceResponse.Error);
            }

            return serviceResponse;
        }
        
        /// <summary>
        /// Invokes the specified function if <seealso cref="IServiceResponse{T}.Error" /> is not null. Allows you to re-direct control flow with a new <typeparamref name="T" /> value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceResponse">The service response.</param>
        /// <param name="continueWithFunction">The continue with function.</param>
        /// <returns>If errors exist, returns the instance of IServiceResponse{T} returned by <paramref name="continueWithFunction" />, else returns current instance.</returns>
        public static IServiceResponse<T> CatchAndContinue<T>(this IServiceResponse<T> serviceResponse, Func<Error, T> continueWithFunction)
        {
            if (serviceResponse.Error != null)
            {
                T result = continueWithFunction.Invoke(serviceResponse.Error);

                return CreateGenericDataResponse<T>(serviceResponse, result);
            }

            return serviceResponse;
        }

        /// <summary>
        /// Invokes the specified action if <see cref="IServiceResponse{T}.Error" /> is null.
        /// Returns the current <see cref="IServiceResponse{T}" /> instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceResponse">The service response.</param>
        /// <param name="action">The action to invoke.</param>
        /// <returns>The current <see cref="IServiceResponse{T}" /> instance.</returns>
        public static IServiceResponse<T> Let<T>(this IServiceResponse<T> serviceResponse, Action<T> action)
        {
            if (serviceResponse.Error == null)
            {
                action.Invoke(serviceResponse.Data);
            }

            return serviceResponse;
        }

        /// <summary>
        /// If <seealso cref="IServiceResponse{T}.Error" /> is not null, returns a new <see cref="IServiceResponse{T2}" /> instance with the current
        /// <seealso cref="IServiceResponse{T}.Error" />. Else, binds the <seealso cref="IServiceResponse{T}.Data" /> into the specified <paramref name="mappingFunction" />.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2">The type of the next <see cref="IServiceResponse{T2}" /> to return.</typeparam>
        /// <param name="serviceResponse">The service response.</param>
        /// <param name="mappingFunction">The mapping function.</param>
        /// <returns>Instance of <see cref="IServiceResponse{T2}" />.</returns>
        public static IServiceResponse<T2> Fmap<T, T2>(this IServiceResponse<T> serviceResponse, Func<T, T2> mappingFunction)
        {
            if (serviceResponse.Error != null)
            {
                return CreateGenericErrorResponse<T, T2>(serviceResponse, serviceResponse.Error);
            }

            T2 result = mappingFunction.Invoke(serviceResponse.Data);

            return CreateGenericDataResponse(serviceResponse, result);
        }

        /// <summary>
        /// Invokes the specified action whether there is <see cref="IServiceResponse{T}.Data" /> or an <see cref="IServiceResponse{T}.Error" />.
        /// Returns the current <see cref="IServiceResponse{T}" /> instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceResponse">The service response.</param>
        /// <param name="action">The action to invoke.</param>
        /// <returns>The current <see cref="IServiceResponse{T}" /> instance.</returns>
        public static IServiceResponse<T> Run<T>(this IServiceResponse<T> serviceResponse, Action<T> action)
        {
            action.Invoke(serviceResponse.Data);

            return serviceResponse;
        }
        
        /// <summary>
        /// Returns the error.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceResponse">The service response.</param>
        /// <returns>Error.</returns>
        /// <exception cref="System.InvalidOperationException">There is nothing to return from left of either - Error or Data.</exception>
        public static Error FromLeft<T>(this IServiceResponse<T> serviceResponse)
        {
            if (!serviceResponse.IsLeft)
            {
                throw new InvalidOperationException("There is nothing to return from left of either - Error or Data.");
            }

            return serviceResponse.Error;
        }

        /// <summary>
        /// Returns the value of <typeparamref name="T"/> if there is no error.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceResponse">The service response.</param>
        /// <returns><typeparamref name="T"/>.</returns>
        /// <exception cref="System.InvalidOperationException">Cannot return right of either when left (errors) exist.</exception>
        public static T FromRight<T>(this IServiceResponse<T> serviceResponse)
        {
            if (serviceResponse.IsRight)
            {
                throw new InvalidOperationException("Cannot return right of either when left (errors) exist.");
            }

            return serviceResponse.Data;
        }

        internal static IServiceResponse<T> CreateGenericDataResponse<T>(IServiceResponse<T> originalResponse, T data)
        {
            if (IsBuiltInDataResponse(originalResponse))
            {
                return new DataResponse<T>(data);
            }

            try
            {
                return Activator.CreateInstance(
                    originalResponse.GetType()
                                    .GetGenericTypeDefinition()
                                    .MakeGenericType(typeof(T)),
                    data) as IServiceResponse<T>;
            }
            catch (TargetInvocationException)
            {
                // No supportable constructor found! Return default.
                return new DataResponse<T>(data);
            }
        }

        internal static IServiceResponse<T2> CreateGenericDataResponse<T, T2>(IServiceResponse<T> originalResponse, T2 data)
        {
            if (IsBuiltInDataResponse(originalResponse))
            {
                return new DataResponse<T2>(data);
            }

            try
            {
                return Activator.CreateInstance(
                    originalResponse.GetType()
                                    .GetGenericTypeDefinition()
                                    .MakeGenericType(typeof(T2)),
                    data) as IServiceResponse<T2>;
            }
            catch (TargetInvocationException)
            {
                // No supportable constructor found! Return default.
                return new DataResponse<T2>(data);
            }
        }

        internal static IServiceResponse<T2> CreateGenericErrorResponse<T, T2>(IServiceResponse<T> originalResponse, Error error)
        {
            if (IsBuiltInErrorResponse(originalResponse))
            {
                return new ErrorResponse<T2>(error);
            }

            try
            {
                return Activator.CreateInstance(
                    originalResponse.GetType()
                                    .GetGenericTypeDefinition()
                                    .MakeGenericType(typeof(T2)),
                    error) as IServiceResponse<T2>;
            }
            catch (TargetInvocationException)
            {
                // No supportable constructor found! Return default.
                return new ErrorResponse<T2>(error);
            }
        }

        private static Boolean IsBuiltInDataResponse<T>(IServiceResponse<T> originalResponse)
        {
#if NET45_OR_GREATER
            var typeInfo = originalResponse.GetType().GetTypeInfo();
            return (originalResponse is DataResponse<T>) ||
                   (typeInfo.IsGenericType && typeof(DataResponse<>).GetTypeInfo().IsAssignableFrom(typeInfo.GetGenericTypeDefinition().GetTypeInfo()));
#else
            var typeInfo = originalResponse.GetType();
            return (originalResponse is DataResponse<T>) ||
                   (typeInfo.IsGenericType && typeof(DataResponse<>).IsAssignableFrom(typeInfo.GetGenericTypeDefinition()));
#endif
        }
        
        private static Boolean IsBuiltInErrorResponse<T>(IServiceResponse<T> originalResponse)
        {
#if NET45_OR_GREATER
            var typeInfo = originalResponse.GetType().GetTypeInfo();
            return (originalResponse is ErrorResponse<T>) ||
                   (typeInfo.IsGenericType && typeof(ErrorResponse<>).GetTypeInfo().IsAssignableFrom(typeInfo.GetGenericTypeDefinition().GetTypeInfo()));
#else
            var typeInfo = originalResponse.GetType();
            return (originalResponse is ErrorResponse<T>) ||
                   (typeInfo.IsGenericType && typeof(ErrorResponse<>).IsAssignableFrom(typeInfo.GetGenericTypeDefinition()));
#endif
        }
    }
}