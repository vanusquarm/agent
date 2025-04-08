using FluentValidation;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace Transactions.API.Validators;

public class GenericSqlInjectSpecialCharsValidator<T> : AbstractValidator<T>
{
    private static readonly string[] SqlKeywords =
    {
        "select", "insert", "update", "delete", "drop", "union", "exec", "execute", "declare", "--", ";", "/*", "*/", "xp_", "benchmark"
    };

    private static readonly Regex SpecialCharsRegex = new Regex(@"[;'\=\""<>\\\[\]\(\)]", RegexOptions.Compiled);

    public GenericSqlInjectSpecialCharsValidator(IEnumerable<Expression<Func<T, string>>> properties)
    {
        foreach (var property in properties)
        {
            RuleFor(property)
                .Must(value =>!ContainsSqlQuery(value))
                .WithMessage("{PropertyName} contains SQL query.")
                .Must(value => !ContainsSpecialChars(value))
                .WithMessage("{PropertyName} contains special characters.");
        }
    }

    private bool ContainsSqlQuery(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return false; // Skip validation if empty/null
        }

        foreach (var keyword in SqlKeywords)
        {
            if (input.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return true;
            }
        }

        return false;
    }

    private bool ContainsSpecialChars(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return false; // Skip validation if empty/null
        }



        return SpecialCharsRegex.IsMatch(input);
    }

    private string GetPropertyName<TProp>(ValidationContext<T> context)
    {
        return context.PropertyName;
    }
}
