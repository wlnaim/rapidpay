using System;
using System.Collections.Generic;

namespace RapidPayAPI.Models;

public partial class Card
{
    public string CardNumber { get; set; } = null!;

    public decimal Balance { get; set; }
}
