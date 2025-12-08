using System.ComponentModel.DataAnnotations;

namespace CleveCoding.Permissions.Models;

/// <summary>
/// High level definitions of actions.
/// </summary>
[Flags]
public enum UserActionType
{
    [Display(Name = "Overview")]
    ReadIndex = 1,

    [Display(Name = "Details")]
    ReadDetails = 2,

    [Display(Name = "Create")]
    Create = 4,

    [Display(Name = "Edit")]
    Edit = 8,

    [Display(Name = "Delete")]
    Delete = 16,

    [Display(Name = "Download")]
    Download = 32,

    [Display(Name = "Upload")]
    Upload = 64
}
