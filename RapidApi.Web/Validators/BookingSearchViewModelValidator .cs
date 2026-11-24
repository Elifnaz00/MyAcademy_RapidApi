using FluentValidation;
using RapidApi.Web.Models;

namespace RapidApi.Web.Validators
{
    public class BookingSearchViewModelValidator : AbstractValidator<BookingSearchViewModel>
    {
        public BookingSearchViewModelValidator()
        {
            RuleFor(x => x.City)
                .NotEmpty()
                .WithMessage("Şehir alanı boş bırakılamaz.")
                .MaximumLength(100)
                .WithMessage("Şehir adı en fazla 100 karakter olabilir.");

            RuleFor(x => x.CheckInDate)
                .NotNull()
                .WithMessage("Giriş tarihi seçilmelidir.")
                .Must(date => !date.HasValue || date.Value.Date >= DateTime.Today)
                .WithMessage("Giriş tarihi bugünden önce olamaz.");

            RuleFor(x => x.CheckOutDate)
                .NotNull()
                .WithMessage("Çıkış tarihi seçilmelidir.")
                .Must((model, checkOut) =>
                    !model.CheckInDate.HasValue ||
                    !checkOut.HasValue ||
                    checkOut.Value.Date > model.CheckInDate.Value.Date)
                .WithMessage("Çıkış tarihi, giriş tarihinden sonra olmalıdır.");

            RuleFor(x => x.Adult)
                .GreaterThanOrEqualTo(1)
                .WithMessage("En az 1 yetişkin seçilmelidir.");

            RuleFor(x => x.Child)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Çocuk sayısı negatif olamaz.");

            RuleFor(x => x.Rooms)
                .GreaterThanOrEqualTo(1)
                .WithMessage("En az 1 oda seçilmelidir.");

            RuleFor(x => x.Currency)
                .NotEmpty()
                .WithMessage("Para birimi seçilmelidir.");

            RuleFor(x => x.ChildAges)
                .NotNull()
                .WithMessage("Çocuk yaşları bilgisi gereklidir.")
                .Must((model, ages) =>
                    ages != null && ages.Count == model.Child)
                .WithMessage("Seçilen çocuk sayısı ile çocuk yaşları eşleşmelidir.");

            RuleForEach(x => x.ChildAges)
                .InclusiveBetween(0, 17)
                .WithMessage("Çocuk yaşı 0 ile 17 arasında olmalıdır.");
        }
    }
}
