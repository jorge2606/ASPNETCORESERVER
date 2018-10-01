using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace server.Dto
{
    public class ClaimPermissionDto 
    {
        public ClaimPermissionDto()
        {
            Children = new List<ClaimPermissionDto>();
        }

        public string Text { get; set; }

        public string Value { get; set; }
            
        public List<ClaimPermissionDto> Children { get; set; }

        public string Name { get; set; }

        public static List<ClaimPermissionDto> GetPermissions()
        {
            var permissions = new List<ClaimPermissionDto>();

            permissions.Add(new ClaimPermissionDto()
            {
                Text = "Users",
                Value = "",
                Children = new List<ClaimPermissionDto>
                {
                    new ClaimPermissionDto()
                    {
                        Text  = "Create",
                        Value = "user.create"
                        
                    },
                    new ClaimPermissionDto()
                    {
                        Text  = "Edit",
                        Value = "user.edit"
                    },
                    new ClaimPermissionDto()
                    {
                        Text  = "Delete",
                        Value = "user.delete"
                    },
                    new ClaimPermissionDto()
                    {
                        Text  = "View",
                        Value = "user.view"
                    }
                }
            });

            return permissions;
            
        }
    }

    public class RoleClaimPermissionDto
    {
        public RoleClaimPermissionDto()
        {
            Children = new List<RoleClaimPermissionDto>();
        }
        
        public string Text { get; set; }

        public string Value { get; set; }

        public bool Checked { get; set; }

        public List<RoleClaimPermissionDto> Children { get; set; }
    }

    public class UpdateRoleClaimPermissionDto
    {
        public Guid Id { get; set; }

        public List<RoleClaimPermissionDto> Claims { get; set; }
    }
}
