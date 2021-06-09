namespace Tandem.Service.Config
{
  public class OrderServiceConfig
  {
    public bool GenerateShippingPackingSlip { get; set; }
    public bool GenerateRoyaltyPackingSlip { get; set; }
    public bool ActivateMembership { get; set; }
    public bool ApplyMembershipChange { get; set; }
    public bool NotifyClient { get; set; }
  }
}
