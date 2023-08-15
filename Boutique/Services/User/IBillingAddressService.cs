
using Boutique.Entity;

namespace Boutique.Services;

public interface IBillingAddressService
{
    BillingAddress GetBillingAddressById(Guid id);

    void InsertBillingAddress(BillingAddress billingAddress);

    void UpdateBillingAddress(BillingAddress billingAddress);

}
