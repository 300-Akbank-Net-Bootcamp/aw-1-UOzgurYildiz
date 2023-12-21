using System.ComponentModel.DataAnnotations;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace VbApi.Controllers;

public class Staff
{
    [Required]
    [StringLength(maximumLength: 250, MinimumLength = 10)]
    public string? Name { get; set; }

    [EmailAddress(ErrorMessage = "Email address is not valid.")]
    public string? Email { get; set; }

    [Phone(ErrorMessage = "Phone is not valid.")]
    public string? Phone { get; set; }

    [Range(minimum: 30, maximum: 400, ErrorMessage = "Hourly salary does not fall within allowed range.")]
    public decimal? HourlySalary { get; set; }
}

//-------------ÖDEV BAŞLIYOR---------------
public class StaffValidator : AbstractValidator<Staff>
{
    public StaffValidator()
    {
        RuleFor(x => x.Name).Length(10, 250).WithMessage("Name length must be between 10 to 250");
        RuleFor(x => x.Email).EmailAddress().WithMessage("Email not valid");
        RuleFor(x => x.Phone).Matches(new Regex(@"((\(\d{3}\) ?)|(\d{3}-))?\d{3}-\d{4}")).WithMessage("PhoneNumber not valid");
        //Regex from https://stackoverflow.com/questions/12908536/how-to-validate-the-phone-no
        RuleFor(x => x.HourlySalary).InclusiveBetween(30, 400).WithMessage("Hourly salary must be between 30 to 400");
    }
}

//---------------ÖDEV BİTTİ------------------


[Route("api/[controller]")]
[ApiController]
public class StaffController : ControllerBase
{
    public StaffController()
    {
    }

    [HttpPost]
    public Staff Post([FromBody] Staff value)
    {
        return value;
    }
}