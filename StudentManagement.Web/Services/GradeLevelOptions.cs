using Microsoft.AspNetCore.Mvc.Rendering;

namespace StudentManagement.Web.Services;

public static class GradeLevelOptions
{
    public static List<SelectListItem> All { get; } = new()
    {
        new("Kindergarten", "Kindergarten"),
        new("1st Grade", "1st Grade"),
        new("2nd Grade", "2nd Grade"),
        new("3rd Grade", "3rd Grade"),
        new("4th Grade", "4th Grade"),
        new("5th Grade", "5th Grade"),
        new("6th Grade", "6th Grade"),
        new("7th Grade", "7th Grade"),
        new("8th Grade", "8th Grade"),
        new("9th Grade", "9th Grade"),
        new("10th Grade", "10th Grade"),
        new("11th Grade", "11th Grade"),
        new("12th Grade", "12th Grade"),
    };
}