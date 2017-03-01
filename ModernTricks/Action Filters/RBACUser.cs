
using ModernTricks.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

public class RBACUser
{
    public Guid User_Id { get; set; }
    public bool IsSysAdmin { get; set; }
    public string Username { get; set; }
    private List<UserRole> Roles = new List<UserRole>();

    public RBACUser(string _username)
    {
        this.Username = _username;
        this.IsSysAdmin = false;
        GetDatabaseUserRolesPermissions();
    }

    private void GetDatabaseUserRolesPermissions()
    {
        using (MainDBEntities _data = new MainDBEntities())
        {
            USERS _user = _data.USERS.Where(u => u.User_Id.ToString() == this.Username).FirstOrDefault();
            if (_user != null)
            {
                this.User_Id = _user.User_Id;
                foreach (ROLES _role in _user.ROLES)
                {
                    UserRole _userRole = new UserRole { Role_Id = _role.Role_Id, RoleName = _role.RoleName, RoleDescription = _role.RoleDescription };                    
                    foreach (PERMISSIONS _permission in _role.PERMISSIONS)
                    {
                        _userRole.Permissions.Add(new PERMISSIONS { Permission_Id = _permission.Permission_Id, PermissionDescription = _permission.PermissionDescription, ROLES = _permission.ROLES });
                    }

                    foreach (REPORTS _report in _role.REPORTS)
                    {
                        _userRole.Reports.Add(new REPORTS { Report_Id = _report.Report_Id, Inactive = _report.Inactive, ReportName = _report.ReportName, ReportDescription = _report.ReportDescription, Template = _report.Template, StoredProcedureName = _report.StoredProcedureName, ROLES = _report.ROLES, PARAMETERS = _report.PARAMETERS });
                    }
                    this.Roles.Add(_userRole);

                    if (!this.IsSysAdmin)
                        this.IsSysAdmin = _role.IsSysAdmin;
                }
            }
        }
    }

    public bool HasPermission(string requiredPermission)
    {
        bool bFound = false;
        foreach (UserRole role in this.Roles)
        {
            bFound = (role.Permissions.Where(p => p.PermissionDescription.ToLower() == requiredPermission.ToLower()).ToList().Count > 0);
            if (bFound)
                break;
        }
        return bFound;
    }    
            
    public bool HasRole(string role)
    {
        return (this.Roles.Where(p => p.RoleName == role).ToList().Count > 0);
    }

    public bool HasRoles(string roles)
    {
        bool bFound = false;
        string[] _roles = roles.ToLower().Split(';');
        foreach (UserRole role in this.Roles)
        {
            try
            {
                bFound = _roles.Contains(role.RoleName.ToLower());
                if (bFound)
                    return bFound;
            }
            catch (Exception)
            {              
            }            
        }
        return bFound;
    }

    //Worker function   
    public List<REPORTS> GetReports()
    {
        List<REPORTS> _retVal = new List<REPORTS>();
        foreach (UserRole _role in this.Roles)
        {
            foreach (REPORTS _report in _role.Reports)
            {
                if (_report.Inactive == false)
                {
                    if (!_retVal.Contains(_report))
                    {
                        _retVal.Add(_report);
                    }
                }
            }
        }
        return _retVal;
    }
}

public class UserRole
{
    public int Role_Id { get; set; }
    public string RoleName { get; set; }
    public string RoleDescription { get; set; }
    public bool IsSysAdmin { get; set; }
    public DateTime? LastModified { get; set; }
    public List<PERMISSIONS> Permissions = new List<PERMISSIONS>();
    public List<REPORTS> Reports = new List<REPORTS>();
}