using System;
using System.Collections.Generic;
using Lab2.Domain.Models.AwbContactInfo;

namespace Lab2.Domain.Models
{
    public static class Awb
    {
        public interface IAwb
        {
        }

        public record UnvalidatedAwb : IAwb
        {
            public UnvalidatedAwb(UnvalidatedAwbContactInfo unvalidatedAwbContactInfo, AwbOrderInfo? awbOrderInfo = null)
            {
                UnvalidatedAwbContactInfo = unvalidatedAwbContactInfo;
                AwbOrderInfo = awbOrderInfo;
            }

            public UnvalidatedAwbContactInfo UnvalidatedAwbContactInfo { get; }
            public AwbOrderInfo? AwbOrderInfo { get; }
        }
        
        public record InvalidAwb : IAwb
        {
            internal InvalidAwb(UnvalidatedAwbContactInfo unvalidatedAwbContactInfo, IEnumerable<string> reasons, AwbOrderInfo? awbOrderInfo = null)
            {
                UnvalidatedAwbContactInfo = unvalidatedAwbContactInfo;
                AwbOrderInfo = awbOrderInfo;
                Reasons = reasons;
            }

            public UnvalidatedAwbContactInfo UnvalidatedAwbContactInfo { get; }
            public AwbOrderInfo? AwbOrderInfo { get; }
            public IEnumerable<string> Reasons { get; }
        }

        public record ValidatedAwb : IAwb
        {
            internal ValidatedAwb(ValidatedAwbContactInfo validatedAwbContactInfo, AwbOrderInfo awbOrderInfo)
            {
                ValidatedAwbContactInfo = validatedAwbContactInfo;
                AwbOrderInfo = awbOrderInfo;
            }

            public ValidatedAwbContactInfo ValidatedAwbContactInfo { get; }
            public AwbOrderInfo AwbOrderInfo { get; }
        }

        public record FinalizedAwb : IAwb
        {
            internal FinalizedAwb(FinalizedAwbContactInfo finalizedAwbContactInfo, AwbOrderInfo awbOrderInfo)
            {
                FinalizedAwbContactInfo = finalizedAwbContactInfo;
                AwbOrderInfo = awbOrderInfo;
            }

            public FinalizedAwbContactInfo FinalizedAwbContactInfo { get; }
            public AwbOrderInfo AwbOrderInfo { get; }
        }
    }
}
