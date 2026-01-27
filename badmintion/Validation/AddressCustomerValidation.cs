using badmintion.DTO;
using badmintion.Models;
using CloudinaryDotNet.Core;
using FluentValidation;

namespace badmintion.Validation
{
    public class AddressCustomerValidation : AbstractValidator<AddressCustomerDTO>
    {
        public AddressCustomerValidation() {
            RuleFor(model => model.Address)
          .NotEmpty().WithMessage("Address không được để trống.")
          .NotNull().WithMessage("Address không được để null.")
          .OverridePropertyName(x => x.Address);

            RuleFor(model => model.ProvinceId)
          .NotEmpty().WithMessage("ProvinceId không được để trống.")
          .NotNull().WithMessage("ProvinceId không được để null.")
          .OverridePropertyName(x => x.ProvinceId);

            RuleFor(model => model.DistrictId)
          .NotEmpty().WithMessage("DistrictId không được để trống.")
          .NotNull().WithMessage("DistrictId không được để null.")
          .OverridePropertyName(x => x.DistrictId);


            RuleFor(model => model.TownId)
         .NotEmpty().WithMessage("TownId không được để trống.")
         .NotNull().WithMessage("TownId không được để null.")
         .OverridePropertyName(x => x.TownId);
        }
    }
}
