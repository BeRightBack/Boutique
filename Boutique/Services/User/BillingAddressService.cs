
using Boutique.Data;
using Boutique.EFRepository;
using Boutique.Entity;

namespace Boutique.Services;

public class BillingAddressService : IBillingAddressService
{
    private readonly IRepository<BillingAddress> billingAddressRepository;

    public BillingAddressService(
        IRepository<BillingAddress> billingAddressRepository)
    {
        this.billingAddressRepository = billingAddressRepository;
    }

    public BillingAddress GetBillingAddressById(Guid id)
    {
        return billingAddressRepository.FindByExpression(x => x.Id == id);
    }

    public void InsertBillingAddress(BillingAddress billingAddress)
    {
        if (billingAddress == null)
            throw new ArgumentNullException(nameof(billingAddress));

        billingAddressRepository.Insert(billingAddress);
        billingAddressRepository.SaveChanges();
    }

    public void UpdateBillingAddress(BillingAddress billingAddress)
    {
        if (billingAddress == null)
            throw new ArgumentNullException(nameof(billingAddress));

        billingAddressRepository.Update(billingAddress);
        billingAddressRepository.SaveChanges();
    }
}
