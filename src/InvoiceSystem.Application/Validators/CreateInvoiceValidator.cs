using FluentValidation;
using InvoiceSystem.Application.DTOs;

namespace InvoiceSystem.Application.Validators;

public class CreateInvoiceValidator : AbstractValidator<CreateInvoiceDto>
{
    public CreateInvoiceValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("Customer ID is required");

        RuleFor(x => x.IssueDate)
            .NotEmpty().WithMessage("Issue date is required")
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Issue date cannot be in the future");

        RuleFor(x => x.DueDate)
            .NotEmpty().WithMessage("Due date is required")
            .GreaterThan(x => x.IssueDate).WithMessage("Due date must be after issue date");

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("At least one item is required")
            .Must(items => items != null && items.Count > 0).WithMessage("Invoice must have at least one item");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Item description is required")
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");

            item.RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than 0");

            item.RuleFor(x => x.UnitPrice)
                .GreaterThan(0).WithMessage("Unit price must be greater than 0");
        });
    }
}
