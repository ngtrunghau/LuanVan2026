using badmintion.Controllers.Core;
using badmintion.Interface;
using badmintion.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace badmintion.Controllers.CustomerController;

[Authorize]
[Route("api/Customer/AddressCustomer")]
public class CustomerAddressController : AddressCustomerControllerBase
{
    public CustomerAddressController(
        BadmintionNlContext context,
        IAddressCustomerService service,
        IHttpContextAccessor httpContextAccessor)
        : base(context, service, httpContextAccessor)
    {
    }
}
