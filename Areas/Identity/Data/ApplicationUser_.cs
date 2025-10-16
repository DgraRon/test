using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace ServiceCatalogBGC_V2.Areas.Identity.Data;

// Add profile data for application users by adding properties to the ApplicationUser_ class
public class ApplicationUser_ : IdentityUser
{
    [PersonalData]
    [Column(TypeName ="nvarchar(100)")]
    public string FirstName { get; set; }

    [PersonalData]
    [Column(TypeName = "nvarchar(100)")]
    public string LastName { get; set; }

    // Soft delete / Disable
    public bool IsDisabled { get; set; } = false;
    public DateTime? DisabledAt { get; set; }
    public string? DisabledBy { get; set; }
}

