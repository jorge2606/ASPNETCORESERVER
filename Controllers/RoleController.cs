using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using server.Dto;
using server.Models;

namespace server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly RoleManager<Role> _roleManager;

        public RoleController(RoleManager<Role> roleManager)
        {
            _roleManager = roleManager;
        }

        [HttpGet("getall")]
        [AllowAnonymous]
        public async Task<ActionResult<List<RoleClaimPermissionDto>>> GetAllRoleClaims(Guid id)
        {
            //Traer permisos
            var allpermissions = ClaimPermissionDto.GetPermissions();

            var rolePermissions = new List<RoleClaimPermissionDto>();

            var role = _roleManager.Roles.FirstOrDefault(x => x.Id == id);

            //Traer todos los Role Claims
            var allRolesClaimd = await _roleManager.GetClaimsAsync(role);

            foreach (var claimPermission in allpermissions)
            {
                //Agregar parent permission
                var parent = new RoleClaimPermissionDto()
                {
                    Text = claimPermission.Text,
                    Value = claimPermission.Value,
                    Checked = false
                };

                rolePermissions.Add(parent);

                foreach (var childPermission in claimPermission.Children)
                {
                    var cheked = false;
                    //pregunto si el Value de ClaimPermissionDto existe en la tabla ASPRoleClaims
                    //si es True lo asigno al childre del claim Base.
                    if (allRolesClaimd.Any(x => x.Value.Equals(childPermission.Value, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        cheked = true;
                    }

                    parent.Children.Add(new RoleClaimPermissionDto()
                    {
                        Text = childPermission.Text,
                        Value = childPermission.Value,
                        Checked = cheked
                    });
                }
                parent.Checked = parent.Children.Any(x => x.Checked);
            }

            return rolePermissions.ToList();

        }

        [HttpPut("UpdateClaims")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateRoleClaims(UpdateRoleClaimPermissionDto model)
        {
            //primero obtengo el rol
            var Rol = _roleManager.Roles.FirstOrDefault(x => x.Id == model.Id);
            //obtengo todos los claims
            var claims = await _roleManager.GetClaimsAsync(Rol);

            //los permisos que viene checkeados del front-end
            foreach (var item in model.Claims)
            {
                foreach (var child in item.Children)
                {
                    if (child.Checked == false)
                    {
                        var claimEliminar = claims.FirstOrDefault(x => x.Value.Equals(child.Value));
                        if (claims.Any(x => x.Value.Equals(child.Value, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            //si esta descheckeado comparo el value con el value de la Tabla y lo elimino por su nombre.
                            var result = await _roleManager.RemoveClaimAsync(Rol, claimEliminar);
                        }
                    }
                    else
                    {
                        //si viene checkeado 
                        //pregunto si el claim exite en la DB sino lo creo
                        if (!claims.Any(x => x.Value.Equals(child.Value, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            await _roleManager.AddClaimAsync(Rol, new Claim("baseproject/permission", child.Value));
                        }

                    }
                }
            }

            return Ok(model);
        }
    }

}