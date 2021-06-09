using System;
using System.Collections.Generic;

namespace Tandem.Model
{
  public class Order : BaseModel
  {
    public Guid ClientId { get; set; }

    public List<Product> Products { get; set; }
  }
}
