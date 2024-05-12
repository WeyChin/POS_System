using Microsoft.EntityFrameworkCore;
using POS_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POS_System.DA.UserDA
{
    public class UserDAL : DbContext
    {
        public UserDAL(DbContextOptions<UserDAL> options) : base(options)
        {
        }

        #region "SELECT"
        #region "GET ALL"
        public DbSet<UserModels> Users { get; set; }
        public DbSet<UserLevelModel> UserLevel { get; set; }
        public DbSet<UserGroupModel> UserGroup { get; set; }
        #endregion

        public Models.UserModels GetUserByUsernameAndPassword(string username, string password)
        {
            try
            {
                return Users.FirstOrDefault(u => u.UserName == username && u.Password == password && u.UserLevelID == 1);
            }
            catch
            {
                throw;
            }
        }

        public Models.UserModels GetUserByUserID(int userID)
        {
            try
            {
                return Users.FirstOrDefault(u => u.UserID == userID);
            }
            catch
            {
                throw;
            }
        }

        public Models.UserModels GetUserByUsername(string username)
        {
            return Users.FirstOrDefault(u => u.UserName == username);
        }

        public List<UserDetailsModel> GetUsersWithDetails()
        {
            var query = from ur in Users
                        join ul in UserLevel on ur.UserLevelID equals ul.UserLevelID
                        join ug in UserGroup on ur.UserGroupID equals ug.UserGroupID
                        join urCreated in Users on ur.CreatedBy equals urCreated.UserID into urCreatedGroup
                        from urCreated in urCreatedGroup.DefaultIfEmpty()
                        join urModified in Users on ur.ModifiedBy equals urModified.UserID into urModifiedGroup
                        from urModified in urModifiedGroup.DefaultIfEmpty()
                        select new UserDetailsModel
                        {
                            UserID = ur.UserID,
                            UserLevelName = ul.LevelName,
                            UserGroupName = ug.GroupName,
                            UserName = ur.UserName,
                            FullName = ur.FullName,
                            Description = ur.Description,
                            Remarks = ur.Remarks,
                            Password = ur.Password,
                            CreatedBy = ur.CreatedBy,
                            CreatedUser = urCreated.FullName,
                            CreatedDate = ur.CreatedDate,
                            ModifiedBy = ur.ModifiedBy,
                            ModifiedUser = urModified.FullName,
                            ModifiedDate = ur.ModifiedDate,
                            Active = ur.Active
                        };

            return query.ToList();
        }
        #endregion
    }
}
