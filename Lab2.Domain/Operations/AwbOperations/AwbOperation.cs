using Lab2.Domain.Exceptions;
using Lab2.Domain.Models;
using static Lab2.Domain.Models.Awb;

namespace Lab2.Domain.Operations
{
    internal abstract class AwbOperation<TState> : DomainOperation<IAwb, TState, IAwb> where TState : class
    {
        public override IAwb Transform(IAwb awb, TState? state) => awb switch
        {
            UnvalidatedAwb unvalidatedAwb => OnUnvalidated(unvalidatedAwb, state),
            ValidatedAwb validatedAwb => OnValidated(validatedAwb, state),
            InvalidAwb invalidAwb => OnInvalid(invalidAwb, state),
            FinalizedAwb finalizedAwb => OnFinalized(finalizedAwb, state),
            _ => throw new InvalidAwbStateException(awb.GetType().Name)
        };

        protected virtual IAwb OnUnvalidated(UnvalidatedAwb unvalidatedAwb, TState? state) => unvalidatedAwb;

        protected virtual IAwb OnValidated(ValidatedAwb validatedAwb, TState? state) => validatedAwb;

        protected virtual IAwb OnInvalid(InvalidAwb invalidAwb, TState? state) => invalidAwb;

        protected virtual IAwb OnFinalized(FinalizedAwb finalizedAwb, TState? state) => finalizedAwb;
    }

    internal abstract class AwbOperation : AwbOperation<object>
    {
        internal IAwb Transform(IAwb awb) => Transform(awb, null);

        protected sealed override IAwb OnUnvalidated(UnvalidatedAwb unvalidatedAwb, object? state) => OnUnvalidated(unvalidatedAwb);

        protected virtual IAwb OnUnvalidated(UnvalidatedAwb unvalidatedAwb) => unvalidatedAwb;

        protected sealed override IAwb OnValidated(ValidatedAwb validatedAwb, object? state) => OnValidated(validatedAwb);

        protected virtual IAwb OnValidated(ValidatedAwb validatedAwb) => validatedAwb;

        protected sealed override IAwb OnInvalid(InvalidAwb invalidAwb, object? state) => OnInvalid(invalidAwb);

        protected virtual IAwb OnInvalid(InvalidAwb invalidAwb) => invalidAwb;

        protected sealed override IAwb OnFinalized(FinalizedAwb finalizedAwb, object? state) => OnFinalized(finalizedAwb);

        protected virtual IAwb OnFinalized(FinalizedAwb finalizedAwb) => finalizedAwb;
    }
}
