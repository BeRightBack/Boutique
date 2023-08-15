using System.ComponentModel.DataAnnotations;
using Boutique.Entity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Boutique.Areas.Admin.Models.ToDo;
public class Todo
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Please enter a Description")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter a Due Date")]
    [DataType(DataType.Date)]
    public DateTime? DueDate { get; set; }

    [Required(ErrorMessage = "Please select a Category")]
    public string CategoryId { get; set; } = string.Empty;

    [ValidateNever]
    public ToDoCategory Category { get; set; } = null;

    [Required(ErrorMessage = "Please select a Status")]
    public string StatusId { get; set; } = string.Empty;

    [ValidateNever]
    public ToDoStatus Status { get; set; } = null;

    public bool Overdue => StatusId == "open" && DueDate < DateTime.Today;

    public virtual ApplicationUser User { get; set; }

}

public class ToDoCategory
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public class ToDoStatus
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}