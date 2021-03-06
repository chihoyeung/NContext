namespace NContext.Data.Specifications
{
    using System;
    using System.Linq.Expressions;

    using NContext.Data.Persistence;

    /// <summary>
    /// Defines a base abstraction for creating specifications.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <remarks></remarks>
    public abstract class SpecificationBase<TEntity> where TEntity : class, IEntity
    {
        /// <summary>
        /// Performs an implicit conversion from <see cref="SpecificationBase{TEntity}" /> to <see cref="Expression{Func{TEntity, Boolean}}" />.
        /// </summary>
        /// <param name="specification">The specification.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Expression<Func<TEntity, Boolean>>(SpecificationBase<TEntity> specification)
        {
            return specification.IsSatisfiedBy();
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="SpecificationBase{TEntity}" /> to <see cref="Func{TEntity, Boolean}" />.
        /// </summary>
        /// <param name="specification">The specification.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Func<TEntity, Boolean>(SpecificationBase<TEntity> specification)
        {
            return specification.IsSatisfiedBy().Compile();
        }

        /// <summary>
        /// And operator
        /// </summary>
        /// <param name="leftSideSpecification">left operand in this AND operation</param>
        /// <param name="rightSideSpecification">right operand in this AND operation</param>
        /// <returns>New specification</returns>
        public static SpecificationBase<TEntity> operator &(SpecificationBase<TEntity> leftSideSpecification, SpecificationBase<TEntity> rightSideSpecification)
        {
            return new AndSpecification<TEntity>(leftSideSpecification, rightSideSpecification);
        }

        /// <summary>
        /// Or operator
        /// </summary>
        /// <param name="leftSideSpecification">left operand in this OR operation</param>
        /// <param name="rightSideSpecification">right operand in this OR operation</param>
        /// <returns>New specification </returns>
        public static SpecificationBase<TEntity> operator |(SpecificationBase<TEntity> leftSideSpecification, SpecificationBase<TEntity> rightSideSpecification)
        {
            return new OrSpecification<TEntity>(leftSideSpecification, rightSideSpecification);
        }

        /// <summary>
        /// Not specification
        /// </summary>
        /// <param name="specification">Specification to negate</param>
        /// <returns>New specification</returns>
        public static SpecificationBase<TEntity> operator !(SpecificationBase<TEntity> specification)
        {
            return new NotSpecification<TEntity>(specification);
        }

        /// <summary>
        /// Override operator false, only for support AND OR operators
        /// </summary>
        /// <param name="specification">Specification instance</param>
        /// <returns>See False operator in C#</returns>
        public static Boolean operator false(SpecificationBase<TEntity> specification)
        {
            return false;
        }

        /// <summary>
        /// Override operator True, only for support AND OR operators
        /// </summary>
        /// <param name="specification">Specification instance</param>
        /// <returns>See True operator in C#</returns>
        public static Boolean operator true(SpecificationBase<TEntity> specification)
        {
            return true;
        }

        /// <summary>
        /// Returns a boolean expression which determines whether the specification is satisfied.
        /// </summary>
        /// <returns>Expression that evaluates whether the specification satifies the expression.</returns>
        public abstract Expression<Func<TEntity, Boolean>> IsSatisfiedBy();
    }
}