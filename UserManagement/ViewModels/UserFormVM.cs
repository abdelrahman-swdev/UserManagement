using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagement.ViewModels
{
    public class UserFormVM
    {
        public string Id { get; set; }

        [Required, Display(Name ="Full Name"),StringLength(256)]
        public string FullName { get; set; }

        [Required, Display(Name = "User Name"), StringLength(256)]
        public string UserName { get; set; }

        [Required, Display(Name = "Email"), StringLength(256)]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
