using System.ComponentModel.DataAnnotations;

namespace CleveCoding.Permissions.Models;

/// <summary>
/// High level definitions of actions.
/// </summary>
public enum UserActionType
{
	[Display(Name = "Overview", GroupName = "Data")]
	ViewIndex = 100,

	[Display(Name = "Details", GroupName = "Data")]
	ViewDetails = 110,

	[Display(Name = "Widget", GroupName = "Data")]
	ViewWidget = 115,

	[Display(Name = "Create", GroupName = "Data")]
	Create = 120,

	[Display(Name = "Update", GroupName = "Data")]
	Update = 130,

	[Display(Name = "Toggle", GroupName = "Data")]
	Toggle = 135,

	[Display(Name = "Delete", GroupName = "Data")]
	Delete = 140,

	[Display(Name = "Download", GroupName = "I/O")]
	Download = 200,

	[Display(Name = "Upload", GroupName = "I/O")]
	Upload = 210,

	[Display(Name = "Export", GroupName = "I/O")]
	Export = 220,

	[Display(Name = "Import", GroupName = "I/O")]
	Import = 230,

	[Display(Name = "Assign", GroupName = "Reviews")]
	Assign = 300,

	[Display(Name = "Review", GroupName = "Reviews")]
	Review = 310,

	[Display(Name = "Approve", GroupName = "Reviews")]
	Approve = 320,

	[Display(Name = "Reject", GroupName = "Reviews")]
	Reject = 330
}
