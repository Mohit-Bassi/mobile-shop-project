namespace MobileShop.Domain.Entities;

public class AccessoryCompatibleMobile
{
    public int AccessoryId { get; set; }
    public string CompatibleBrand { get; set; } = null!;
    public string CompatibleModel { get; set; } = null!;

    public Accessory Accessory { get; set; } = null!;
}
