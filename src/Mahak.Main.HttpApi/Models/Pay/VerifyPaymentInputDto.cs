using System;

namespace Mahak.Main.Models.Pay;

public class VerifyPaymentInputDto
{
    public string TransactionCode { get; set; }
    public bool Result { get; set; }
}