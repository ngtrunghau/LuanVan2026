using badmintion.Authorization;
using badmintion.Controllers.Core;
using badmintion.Interface;
using badmintion.Models;
using Microsoft.AspNetCore.Mvc;

namespace badmintion.Controllers;

[Route("api/[controller]")]
[Authorize]
public class AddressCustomerController : AddressCustomerControllerBase
{
    public AddressCustomerController(
        BadmintionNlContext context,
        IAddressCustomerService service,
        IHttpContextAccessor httpContextAccessor)
        : base(context, service, httpContextAccessor)
    {
    }
}
