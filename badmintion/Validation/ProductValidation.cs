using badmintion.DTO;
using badmintion.Models;
using FluentValidation;

namespace badmintion.Validation
{
    public class ProductValidation : AbstractValidator<ProductDTO>
    {
        public ProductValidation() {
            RuleFor(model => model.Name)
       .NotEmpty().WithMessage("Name không được để trống.")
       .NotNull().WithMessage("Name không được để null.")
       .OverridePropertyName(x => x.Name);

            RuleFor(model => model.Descriptions)
          .NotEmpty().WithMessage("Descriptions không được để trống.")
          .NotNull().WithMessage("Descriptions không được để null.")
          .OverridePropertyName(x => x.Descriptions);

            RuleFor(model => model.Price)
          .NotEmpty().WithMessage("Price không được để trống.")
          .NotNull().WithMessage("Price không được để null.")
          .OverridePropertyName(x => x.Price);


            RuleFor(model => model.ImageUrl)
         .NotEmpty().WithMessage("ImageUrl không được để trống.")
         .NotNull().WithMessage("ImageUrl không được để null.")
         .OverridePropertyName(x => x.ImageUrl);

            RuleFor(model => model.CategoriesId)
         .NotEmpty().WithMessage("CategoriesId không được để trống.")
         .NotNull().WithMessage("CategoriesId không được để null.")
         .OverridePropertyName(x => x.CategoriesId);
        }
    }
}
