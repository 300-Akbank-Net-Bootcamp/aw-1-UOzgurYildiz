using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using System.Text.RegularExpressions;

namespace VbApi.Controllers;

public class Employee : IValidatableObject
{
    [Required]
    [StringLength(maximumLength: 250, MinimumLength = 10, ErrorMessage = "Invalid Name")]
    public string? Name { get; set; }

    [Required] 
    public DateTime DateOfBirth { get; set; }

    [EmailAddress(ErrorMessage = "Email address is not valid.")]
    public string? Email { get; set; }

    [Phone(ErrorMessage = "Phone is not valid.")]
    public string? Phone { get; set; }

    [Range(minimum: 50, maximum: 400, ErrorMessage = "Hourly salary does not fall within allowed range.")]
    [MinLegalSalaryRequired(minJuniorSalary: 50, minSeniorSalary: 200)]
    public double HourlySalary { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var minAllowedBirthDate = DateTime.Today.AddYears(-65);
        if (minAllowedBirthDate > DateOfBirth)
        {
            yield return new ValidationResult("Birthdate is not valid.");
        }
    }
}

//-------------ÖDEV BAŞLIYOR---------------

public class EmployeeValidator : AbstractValidator <Employee>
{
    public EmployeeValidator()
    {
        RuleFor(x => x.Name).Length(10, 250).WithMessage("Name length must be between 10 to 250");
        RuleFor(x => x.DateOfBirth).NotNull().WithMessage("Please enter a date of birth").Must(DateOfBirth => DateOfBirth < DateTime.Today.AddYears(-65)).WithMessage("Birthdate not valid");
        RuleFor(x => x.Email).EmailAddress().WithMessage("Email not valid");
        RuleFor(x => x.Phone).Matches(new Regex(@"((\(\d{3}\) ?)|(\d{3}-))?\d{3}-\d{4}")).WithMessage("PhoneNumber not valid");
        //Regex from https://stackoverflow.com/questions/12908536/how-to-validate-the-phone-no
        RuleFor(x => x).Must(x => MinLegalSalaryRequired(x));
        RuleFor(x => x.HourlySalary).InclusiveBetween(30, 400).WithMessage("Hourly salary must be between 30 to 400");
    }

    private bool MinLegalSalaryRequired(Employee e)
    {
        var minJuniorSalary = 50;
        var minSeniorSalary = 200;

        var dateBeforeThirtyYears = DateTime.Today.AddYears(-30);
        var isOlderThanThirtyYears = e.DateOfBirth <= dateBeforeThirtyYears;
        var hourlySalary = e.HourlySalary;

        var isValidSalary = isOlderThanThirtyYears ? hourlySalary >= minSeniorSalary : hourlySalary >= minJuniorSalary;

        return isValidSalary;
    }

}

//---------------ÖDEV BİTTİ------------------

public class MinLegalSalaryRequiredAttribute : ValidationAttribute
{
    public MinLegalSalaryRequiredAttribute(double minJuniorSalary, double minSeniorSalary)
    {
        MinJuniorSalary = minJuniorSalary;
        MinSeniorSalary = minSeniorSalary;
    }

    public double MinJuniorSalary { get; }
    public double MinSeniorSalary { get; }
    public string GetErrorMessage() => $"Minimum hourly salary is not valid.";

    protected override ValidationResult? IsValid(object value, ValidationContext validationContext)
    {
        var employee = (Employee)validationContext.ObjectInstance;
        var dateBeforeThirtyYears = DateTime.Today.AddYears(-30);
        var isOlderThanThirdyYears = employee.DateOfBirth <= dateBeforeThirtyYears;
        var hourlySalary = (double)value;

        var isValidSalary = isOlderThanThirdyYears ? hourlySalary >= MinSeniorSalary : hourlySalary >= MinJuniorSalary;

        return isValidSalary ? ValidationResult.Success : new ValidationResult(GetErrorMessage());
    }
}

[Route("api/[controller]")]
[ApiController]
public class EmployeeController : ControllerBase
{
    public EmployeeController()
    {
    }

    [HttpPost]
    public Employee Post([FromBody] Employee value)
    {
        if (value.DateOfBirth > DateTime.Now.AddYears(-30) && value.HourlySalary < 200)
        {
            
        }
        return value;
    }
}