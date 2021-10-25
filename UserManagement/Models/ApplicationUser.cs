using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagement.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required, MaxLength(256)]
        public string FullName { get; set; }
        public byte[] ProfilePicture { get; set; }
    }
}
