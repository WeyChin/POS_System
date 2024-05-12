using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace POS_System.Models
{
    public class UserModels
    {
        [Key] public int UserID { get; set; }
        public int UserLevelID { get; set; }
        public int UserGroupID { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public Boolean Active { get; set; }
        public string Description { get; set; }
        public string Remarks { get; set; }
        public string Password { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

    public class UserGroupModel
    {
        [Key] public int UserGroupID { get; set; }
        public string GroupName { get; set; }
        public string Description { get; set; }
        public string Remarks { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

    public class UserLevelModel
    {
        [Key] public int UserLevelID { get; set; }
        public string LevelName { get; set; }
        public string Description { get; set; }
        public string Remarks { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

    public class UserDetailsModel
    {
        [Key] public int UserID { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string UserLevelName { get; set; }
        public int UserLevelID { get; set; }
        public string UserGroupName { get; set; }
        public int UserGroupID { get; set; }
        public Boolean Active { get; set; }
        public string Description { get; set; }
        public string Remarks { get; set; }
        public string Password { get; set; }
        public string CreatedUser { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? ModifiedUser { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

    public class ViewAllUserModel
    {
        public List<UserDetailsModel> UserDetails { get; set; }
    }

    public class UserRegistrationViewModel
    {
        public List<UserLevelModel> UserLevels { get; set; }
        public List<UserGroupModel> UserGroups { get; set; }
    }

    public class EditUserPageViewModel
    {
        public UserRegistrationViewModel UserRegistrationViewModel { get; set; }
        public List<UserModels> Users { get; set; }
    }

}
