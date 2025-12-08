using System.ComponentModel.DataAnnotations;

namespace CleveCoding.Permissions.Models;

/// <summary>
/// High level definitions of actions.
/// </summary>
public enum UserActionType
{
	[Display(Name = "Overview", GroupName = "CRUD")]
	ViewIndex = 100,

	[Display(Name = "Details", GroupName = "CRUD")]
	ViewDetails = 110,

	[Display(Name = "Create", GroupName = "CRUD")]
	Create = 120,

	[Display(Name = "Update", GroupName = "CRUD")]
	Update = 130,

	[Display(Name = "Toggle", GroupName = "CRUD")]
	Toggle = 135,

	[Display(Name = "Delete", GroupName = "CRUD")]
	Delete = 140,

	[Display(Name = "Download", GroupName = "I/O")]
	Download = 200,

	[Display(Name = "Upload", GroupName = "I/O")]
	Upload = 210,

	[Display(Name = "Export", GroupName = "I/O")]
	Export = 220,

	[Display(Name = "Import", GroupName = "I/O")]
	Import = 230,

	[Display(Name = "Assign", GroupName = "REVIEWS")]
	Assign = 300,

	[Display(Name = "Review", GroupName = "REVIEWS")]
	Review = 310,

	[Display(Name = "Approve", GroupName = "REVIEWS")]
	Approve = 320,

	[Display(Name = "Reject", GroupName = "REVIEWS")]
	Reject = 330
}
