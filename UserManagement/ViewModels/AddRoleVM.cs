using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagement.ViewModels
{
    public class AddRoleVM
    {
        [Required, StringLength(256),Display(Name ="Role Name")]
        public string RoleName { get; set; }
    }
}
