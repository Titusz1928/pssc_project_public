using Lab2.Domain.Exceptions;
using Lab2.Domain.Models;
using static Lab2.Domain.Models.Order;

namespace Lab2.Domain.Operations{

    internal abstract class OrderOperation<TState> : DomainOperation<IOrder, TState, IOrder> where TState : class
    {
        public override IOrder Transform(IOrder order, TState? state) => order switch
        {
            UnvalidatedOrder unvalidatedOrder => OnUnvalidated(unvalidatedOrder, state),
            ValidatedOrder validOrder => OnValid(validOrder, state),
            InvalidOrder invalidOrder => OnInvalid(invalidOrder, state),
            CalculatedOrder calculatedOrder => OnCalculated(calculatedOrder, state),
            PayedOrder payedOrder => OnPayed(payedOrder, state),
            _ => throw new InvalidOrderStateException(order.GetType().Name)
        };
        
        protected virtual IOrder OnUnvalidated(UnvalidatedOrder unvalidatedOrder, TState? state) => unvalidatedOrder;
        
        protected virtual IOrder OnValid(ValidatedOrder validOrder, TState? state) => validOrder;
        
        protected virtual IOrder OnInvalid(InvalidOrder invalidOrder, TState? state) => invalidOrder;
        
        protected virtual IOrder OnCalculated(CalculatedOrder calculatedOrder, TState? state) => calculatedOrder;
        
       protected virtual IOrder OnPayed(PayedOrder order, TState? state) => order;
        
    }

    internal abstract class OrderOperation : OrderOperation<object>
    {   
        internal IOrder Transform(IOrder order)=>Transform(order, null);
        protected sealed override IOrder OnUnvalidated(UnvalidatedOrder unvalidatedOrder, object? state) => OnUnvalidated(unvalidatedOrder);
       protected virtual IOrder OnUnvalidated(UnvalidatedOrder unvalidatedOrder) => unvalidatedOrder;
       protected sealed override IOrder OnValid(ValidatedOrder validatedOrder, object? state)=>OnValid(validatedOrder);
     
       protected virtual IOrder OnValid(ValidatedOrder validatedOrder) => validatedOrder;
       
       protected sealed override IOrder OnPayed(PayedOrder payedOrder, object? state) => OnPayed(payedOrder);

       protected virtual IOrder OnPayed(PayedOrder payedOrder) => payedOrder;

       protected sealed override IOrder OnCalculated(CalculatedOrder calculatedOrder, object? state) => OnCalculated(calculatedOrder);

       protected virtual IOrder OnCalculated(CalculatedOrder calculatedOrder) => calculatedOrder;

       protected sealed override IOrder OnInvalid(InvalidOrder invalidOrder, object? state) => OnInvalid(invalidOrder);

       protected virtual IOrder OnInvalid(InvalidOrder invalidOrder) => invalidOrder;
    }

}