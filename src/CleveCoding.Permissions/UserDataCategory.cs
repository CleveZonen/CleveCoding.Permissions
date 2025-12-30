using System.ComponentModel.DataAnnotations;

namespace CleveCoding.Permissions;

/// <summary>
/// Employee Data Categories for privacy classifications.
/// </summary>
public enum UserDataCategory : byte
{
	[Display(Name = "Geen")]
	None = 0,

	[Display(Name = "Persoonlijke Informatie")]
	PersonalIdentity = 10, // name, SSN, DOB

	[Display(Name = "Contact Informatie")]
	ContactInformation = 20, // address, phone

	[Display(Name = "Contact Noodgevallen")]
	EmergencyContacts = 30,

	[Display(Name = "Kinderen")]
	Children = 40,

	[Display(Name = "Educatie")]
	Education = 50,

	[Display(Name = "Overeenkomsten")]
	Contracts = 60, // role, salary band

	[Display(Name = "Documenten")]
	Documents = 70,

	[Display(Name = "Reviews")]
	Reviews = 80,

	[Display(Name = "Ziektedagen")]
	SickDays = 90,

	[Display(Name = "Nevenbetrekkingen")]
	Sidejobs = 100
}