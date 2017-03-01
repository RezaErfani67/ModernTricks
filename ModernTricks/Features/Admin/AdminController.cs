using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using ModernTricks.Models;


namespace ModernTricks.Controllers
{
    [RBAC]
    public class AdminController : Controller
    {
        private MainDBEntities database = new MainDBEntities();

        #region USERS
        // GET: Admin
        public ActionResult Index()
        {
            return View(database.USERS.Where(r => r.Inactive == false || r.Inactive == null).OrderBy(r => r.Lastname).ThenBy(r => r.Firstname).ToList());
        }

        public ViewResult UserDetails(int id)
        {
            USERS user = database.USERS.Find(id);
            SetViewBagData(id);
            return View(user);
        }

        public ActionResult UserCreate()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UserCreate(USERS user)
        {
            if (user.Username == "" || user.Username == null)
            {
                ModelState.AddModelError(string.Empty, "Username cannot be blank");
            }

            try
            {
                if (ModelState.IsValid)
                {
                    List<string> results = database.Database.SqlQuery<String>(string.Format("SELECT Username FROM USERS WHERE Username = '{0}'", user.Username)).ToList();
                    bool _userExistsInTable = (results.Count > 0);

                    USERS _user = null;
                    if (_userExistsInTable)
                    {
                        _user = database.USERS.Where(p => p.Username == user.Username).FirstOrDefault();
                        if (_user != null)
                        {
                            if (_user.Inactive == false)
                            {
                                ModelState.AddModelError(string.Empty, "USERS already exists!");
                            }
                            else
                            {
                                database.Entry(_user).Entity.Inactive = false;
                                database.Entry(_user).Entity.LastModified = System.DateTime.Now;
                                database.Entry(_user).State = EntityState.Modified;
                                database.SaveChanges();
                                return RedirectToAction("Index");
                            }
                        }
                    }
                    else
                    {
                        _user = new USERS();
                        _user.Username = user.Username;
                        _user.Lastname = user.Lastname;
                        _user.Firstname = user.Firstname;
                        _user.Title = user.Title;
                        _user.Initial = user.Initial;
                        _user.EMail = user.EMail;

                        if (ModelState.IsValid)
                        {
                            _user.Inactive = false;
                            _user.LastModified = System.DateTime.Now;

                            database.USERS.Add(_user);
                            database.SaveChanges();
                            return RedirectToAction("Index");
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                //return base.ShowError(ex);
            }

            return View(user);
        }

        [HttpGet]
        public ActionResult UserEdit(int id)
        {
            USERS user = database.USERS.Find(id);
            SetViewBagData(id);
            return View(user);
        }

        [HttpPost]
        public ActionResult UserEdit(USERS user)
        {
            USERS _user = database.USERS.Where(p => p.User_Id == user.User_Id).FirstOrDefault();
            if (_user != null)
            {
                try
                {
                    database.Entry(_user).CurrentValues.SetValues(user);
                    database.Entry(_user).Entity.LastModified = System.DateTime.Now;
                    database.SaveChanges();
                }
                catch (Exception)
                {

                }
            }
            return RedirectToAction("UserDetails", new RouteValueDictionary(new { id = user.User_Id }));
        }

        [HttpPost]
        public ActionResult UserDetails(USERS user)
        {
            if (user.Username == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid USERS Name");
            }

            if (ModelState.IsValid)
            {
                database.Entry(user).Entity.Inactive = user.Inactive;
                database.Entry(user).Entity.LastModified = System.DateTime.Now;
                database.Entry(user).State = EntityState.Modified;
                database.SaveChanges();
            }
            return View(user);
        }

        [HttpGet]
        public ActionResult DeleteUserRole(int id, int userId)
        {
            ROLES role = database.ROLES.Find(id);
            USERS user = database.USERS.Find(userId);

            if (role.USERS.Contains(user))
            {
                role.USERS.Remove(user);
                database.SaveChanges();
            }
            return RedirectToAction("Details", "USERS", new { id = userId });
        }

        [HttpGet]
        public PartialViewResult filter4Users(string _surname)
        {
            return PartialView("_ListUserTable", GetFilteredUserList(_surname));
        }

        [HttpGet]
        public PartialViewResult filterReset()
        {
            return PartialView("_ListUserTable", database.USERS.Where(r => r.Inactive == false || r.Inactive == null).ToList());
        }

        [HttpGet]
        public PartialViewResult DeleteUserReturnPartialView(int userId)
        {
            try
            {
                USERS user = database.USERS.Find(userId);
                if (user != null)
                {
                    database.Entry(user).Entity.Inactive = true;
                    database.Entry(user).Entity.User_Id = user.User_Id;
                    database.Entry(user).Entity.LastModified = System.DateTime.Now;
                    database.Entry(user).State = EntityState.Modified;
                    database.SaveChanges();
                }
            }
            catch
            {
            }
            return this.filterReset();
        }

        public ActionResult DeleteUser(int userId)
        {
            try
            {
                USERS user = database.USERS.Find(userId);
                if (user != null)
                {
                    database.Entry(user).Entity.Inactive = true;
                    database.Entry(user).Entity.User_Id = user.User_Id;
                    database.Entry(user).Entity.LastModified = System.DateTime.Now;
                    database.Entry(user).State = EntityState.Modified;
                    database.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, ex.Message);
            }
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        private IEnumerable<USERS> GetFilteredUserList(string _surname)
        {
            IEnumerable<USERS> _ret = null;
            try
            {
                if (string.IsNullOrEmpty(_surname))
                {
                    _ret = database.USERS.Where(r => r.Inactive == false || r.Inactive == null).ToList();
                }
                else
                {
                    _ret = database.USERS.Where(
               p => p.Username.Contains(_surname) ||
                    p.EMail.Contains(_surname)).ToList();
                }
            }
            catch
            {
            }
            return _ret;
        }

        protected override void Dispose(bool disposing)
        {
            database.Dispose();
            base.Dispose(disposing);
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public PartialViewResult DeleteUserRoleReturnPartialView(int id, int userId)
        {
            ROLES role = database.ROLES.Find(id);
            USERS user = database.USERS.Find(userId);

            if (role.USERS.Contains(user))
            {
                role.USERS.Remove(user);
                database.SaveChanges();
            }
            SetViewBagData(userId);
            return PartialView("_ListUserRoleTable", database.USERS.Find(userId));
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public PartialViewResult AddUserRoleReturnPartialView(int id, int userId)
        {
            ROLES role = database.ROLES.Find(id);
            USERS user = database.USERS.Find(userId);

            if (!role.USERS.Contains(user))
            {
                role.USERS.Add(user);
                database.SaveChanges();
            }
            SetViewBagData(userId);
            return PartialView("_ListUserRoleTable", database.USERS.Find(userId));
        }

        private void SetViewBagData(int _userId)
        {
            ViewBag.UserId = _userId;
            ViewBag.List_boolNullYesNo = this.List_boolNullYesNo();
            ViewBag.RoleId = new SelectList(database.ROLES.OrderBy(p => p.RoleName), "Role_Id", "RoleName");
        }

        public List<SelectListItem> List_boolNullYesNo()
        {
            var _retVal = new List<SelectListItem>();
            try
            {
                _retVal.Add(new SelectListItem { Text = "Not Set", Value = null });
                _retVal.Add(new SelectListItem { Text = "Yes", Value = bool.TrueString });
                _retVal.Add(new SelectListItem { Text = "No", Value = bool.FalseString });
            }
            catch { }
            return _retVal;
        }
        #endregion

        #region ROLES
        public ActionResult RoleIndex()
        {
            return View(database.ROLES.OrderBy(r => r.RoleDescription).ToList());
        }

        public ViewResult RoleDetails(int id)
        {
            USERS user = database.USERS.Where(r => r.Username == User.Identity.Name).FirstOrDefault();
            ROLES role = database.ROLES.Where(r => r.Role_Id == id)
                   .Include(a => a.PERMISSIONS)
                   .Include(a => a.USERS)
                   .FirstOrDefault();

            // USERS combo
            ViewBag.UserId = new SelectList(database.USERS.Where(r => r.Inactive == false || r.Inactive == null), "Id", "Username");
            ViewBag.RoleId = id;

            // Rights combo
            ViewBag.PermissionId = new SelectList(database.PERMISSIONS.OrderBy(a => a.PermissionDescription), "Permission_Id", "PermissionDescription");
            ViewBag.List_boolNullYesNo = this.List_boolNullYesNo();

            return View(role);
        }

        public ActionResult RoleCreate()
        {
            USERS user = database.USERS.Where(r => r.Username == User.Identity.Name).FirstOrDefault();
            ViewBag.List_boolNullYesNo = this.List_boolNullYesNo();
            return View();
        }

        [HttpPost]
        public ActionResult RoleCreate(ROLES _role)
        {
            if (_role.RoleDescription == null)
            {
                ModelState.AddModelError("Role Description", "Role Description must be entered");
            }

            USERS user = database.USERS.Where(r => r.Username == User.Identity.Name).FirstOrDefault();
            if (ModelState.IsValid)
            {
                _role.LastModified = DateTime.Now;
                database.ROLES.Add(_role);
                database.SaveChanges();
                return RedirectToAction("RoleIndex");
            }
            ViewBag.List_boolNullYesNo = this.List_boolNullYesNo();
            return View(_role);
        }


        public ActionResult RoleEdit(int id)
        {
            USERS user = database.USERS.Where(r => r.Username == User.Identity.Name).FirstOrDefault();

            ROLES _role = database.ROLES.Where(r => r.Role_Id == id)
                    .Include(a => a.PERMISSIONS)
                    .Include(a => a.USERS)
                    .FirstOrDefault();

            // USERS combo
            ViewBag.UserId = new SelectList(database.USERS.Where(r => r.Inactive == false || r.Inactive == null), "User_Id", "Username");
            ViewBag.RoleId = id;

            // Rights combo
            ViewBag.PermissionId = new SelectList(database.PERMISSIONS.OrderBy(a => a.Permission_Id), "Permission_Id", "PermissionDescription");
            ViewBag.List_boolNullYesNo = this.List_boolNullYesNo();

            // Rights combo
            ViewBag.ReportId = new SelectList(database.REPORTS.OrderBy(a => a.Report_Id), "Report_Id", "ReportName");


            return View(_role);
        }

        [HttpPost]
        public ActionResult RoleEdit(ROLES _role)
        {
            if (string.IsNullOrEmpty(_role.RoleDescription))
            {
                ModelState.AddModelError("Role Description", "Role Description must be entered");
            }

            //EntityState state = database.Entry(_role).State;
            USERS user = database.USERS.Where(r => r.Username == User.Identity.Name).FirstOrDefault();
            if (ModelState.IsValid)
            {

                database.Entry(_role).State = EntityState.Modified;
                database.SaveChanges();
                return RedirectToAction("RoleDetails", new RouteValueDictionary(new { id = _role.Role_Id }));
            }
            // USERS combo
            ViewBag.UserId = new SelectList(database.USERS.Where(r => r.Inactive == false || r.Inactive == null), "User_Id", "Username");

            // Rights combo
            ViewBag.PermissionId = new SelectList(database.PERMISSIONS.OrderBy(a => a.Permission_Id), "Permission_Id", "PermissionDescription");
            ViewBag.List_boolNullYesNo = this.List_boolNullYesNo();
            return View(_role);
        }

        [HttpGet]
        public ActionResult RoleDelete(int id)
        {
            try
            {
                ROLES _role = database.ROLES.Find(id);
                if (_role != null)
                {
                    _role.USERS.Clear();
                    _role.PERMISSIONS.Clear();

                    database.Entry(_role).State = EntityState.Deleted;
                    database.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, ex.Message);
            }
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public PartialViewResult DeleteUserFromRoleReturnPartialView(int id, int userId)
        {
            ROLES role = database.ROLES.Find(id);
            USERS user = database.USERS.Find(userId);

            if (role.USERS.Contains(user))
            {
                role.USERS.Remove(user);
                database.SaveChanges();
            }
            return PartialView("_ListUsersTable4Role", role);
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public PartialViewResult AddUser2RoleReturnPartialView(int id, int userId)
        {
            ROLES role = database.ROLES.Find(id);
            USERS user = database.USERS.Find(userId);

            if (!role.USERS.Contains(user))
            {
                role.USERS.Add(user);
                database.SaveChanges();
            }
            return PartialView("_ListUsersTable4Role", role);
        }

        #endregion

        #region PERMISSIONS

        public ViewResult PermissionIndex()
        {
            List<PERMISSIONS> _permissions = database.PERMISSIONS
                               .OrderBy(wn => wn.PermissionDescription)
                               .Include(a => a.ROLES)
                               .ToList();
            return View(_permissions);
        }

        public ViewResult PermissionDetails(int id)
        {
            PERMISSIONS _permission = database.PERMISSIONS.Find(id);
            return View(_permission);
        }

        public ActionResult PermissionCreate()
        {
            return View();
        }

        [HttpPost]
        public ActionResult PermissionCreate(PERMISSIONS _permission)
        {
            if (_permission.PermissionDescription == null)
            {
                ModelState.AddModelError("Permission Description", "Permission Description must be entered");
            }

            if (ModelState.IsValid)
            {
                database.PERMISSIONS.Add(_permission);
                database.SaveChanges();
                return RedirectToAction("PermissionIndex");
            }
            return View(_permission);
        }

        public ActionResult PermissionEdit(int id)
        {
            PERMISSIONS _permission = database.PERMISSIONS.Find(id);
            ViewBag.RoleId = new SelectList(database.ROLES.OrderBy(p => p.RoleDescription), "Role_Id", "RoleDescription");
            return View(_permission);
        }

        [HttpPost]
        public ActionResult PermissionEdit(PERMISSIONS _permission)
        {
            if (ModelState.IsValid)
            {
                database.Entry(_permission).State = EntityState.Modified;
                database.SaveChanges();
                return RedirectToAction("PermissionDetails", new RouteValueDictionary(new { id = _permission.Permission_Id }));
            }
            return View(_permission);
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public PartialViewResult DeletePermissionReturnPartialView(int id)
        {
            PERMISSIONS permission = database.PERMISSIONS.Find(id);
            if (permission != null)
            {
                database.Entry(permission).State = EntityState.Deleted;
                database.SaveChanges();
            }
            //return RedirectToAction("PermissionIndex");
            return PartialView("_ListPermissionsTable", database.PERMISSIONS.OrderBy(r => r.PermissionDescription).ToList());
        }

        [HttpGet]
        public ActionResult DeletePermission(int id)
        {
            try
            {
                PERMISSIONS permission = database.PERMISSIONS.Find(id);
                if (permission != null)
                {
                    database.Entry(permission).State = EntityState.Deleted;
                    database.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, ex.Message);
            }
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public PartialViewResult AddPermission2RoleReturnPartialView(int id, int permissionId)
        {
            ROLES role = database.ROLES.Find(id);
            PERMISSIONS _permission = database.PERMISSIONS.Find(permissionId);

            if (!role.PERMISSIONS.Contains(_permission))
            {
                role.PERMISSIONS.Add(_permission);
                database.SaveChanges();
            }
            return PartialView("_ListPermissions", role);
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public PartialViewResult AddAllPermissions2RoleReturnPartialView(int id)
        {
            ROLES _role = database.ROLES.Where(p => p.Role_Id == id).FirstOrDefault();
            List<PERMISSIONS> _permissions = database.PERMISSIONS.ToList();
            foreach (PERMISSIONS _permission in _permissions)
            {
                if (!_role.PERMISSIONS.Contains(_permission))
                {
                    _role.PERMISSIONS.Add(_permission);

                }
            }
            database.SaveChanges();
            return PartialView("_ListPermissions", _role);
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public PartialViewResult DeletePermissionFromRoleReturnPartialView(int id, int permissionId)
        {
            ROLES _role = database.ROLES.Find(id);
            PERMISSIONS _permission = database.PERMISSIONS.Find(permissionId);

            if (_role.PERMISSIONS.Contains(_permission))
            {
                _role.PERMISSIONS.Remove(_permission);
                database.SaveChanges();
            }
            return PartialView("_ListPermissions", _role);
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public PartialViewResult DeleteRoleFromPermissionReturnPartialView(int id, int permissionId)
        {
            ROLES role = database.ROLES.Find(id);
            PERMISSIONS permission = database.PERMISSIONS.Find(permissionId);

            if (role.PERMISSIONS.Contains(permission))
            {
                role.PERMISSIONS.Remove(permission);
                database.SaveChanges();
            }
            return PartialView("_ListRolesTable4Permission", permission);
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public PartialViewResult AddRole2PermissionReturnPartialView(int permissionId, int roleId)
        {
            ROLES role = database.ROLES.Find(roleId);
            PERMISSIONS _permission = database.PERMISSIONS.Find(permissionId);

            if (!role.PERMISSIONS.Contains(_permission))
            {
                role.PERMISSIONS.Add(_permission);
                database.SaveChanges();
            }
            return PartialView("_ListRolesTable4Permission", _permission);
        }

        public ActionResult PermissionsImport()
        {
            var _controllerTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => t != null
                    && t.IsPublic
                    && t.Name.EndsWith("Controller", StringComparison.OrdinalIgnoreCase)
                    && !t.IsAbstract
                    && typeof(IController).IsAssignableFrom(t));

            var _controllerMethods = _controllerTypes.ToDictionary(controllerType => controllerType,
                    controllerType => controllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .Where(m => typeof(ActionResult).IsAssignableFrom(m.ReturnType)));

            foreach (var _controller in _controllerMethods)
            {
                string _controllerName = _controller.Key.Name;
                foreach (var _controllerAction in _controller.Value)
                {
                    string _controllerActionName = _controllerAction.Name;
                    if (_controllerName.EndsWith("Controller"))
                    {
                        _controllerName = _controllerName.Substring(0, _controllerName.LastIndexOf("Controller"));
                    }

                    string _permissionDescription = string.Format("{0}-{1}", _controllerName, _controllerActionName);
                    PERMISSIONS _permission = database.PERMISSIONS.Where(p => p.PermissionDescription == _permissionDescription).FirstOrDefault();
                    if (_permission == null)
                    {
                        if (ModelState.IsValid)
                        {
                            PERMISSIONS _perm = new PERMISSIONS();
                            _perm.PermissionDescription = _permissionDescription;

                            database.PERMISSIONS.Add(_perm);
                            database.SaveChanges();
                        }
                    }
                }
            }
            return RedirectToAction("PermissionIndex");
        }
        #endregion

        #region REPORTS
        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public PartialViewResult DeleteReportFromRoleReturnPartialView(int roleId, int reportId)
        {
            ViewBag.roleId = roleId;
            ROLES _role = database.ROLES.Where(p => p.Role_Id == roleId).FirstOrDefault();
            if (_role != null)
            {
                if (_role.REPORTS != null)
                {
                    foreach (REPORTS _report in _role.REPORTS)
                    {
                        if (_report.Report_Id == reportId)
                        {
                            _role.REPORTS.Remove(_report);
                            break;
                        }
                    }
                    database.Entry(_role).State = EntityState.Modified;
                    database.SaveChanges();
                }
            }
            return PartialView("_ListReportsTable4Role", _role.REPORTS.ToList());
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public PartialViewResult AddReport2RoleReturnPartialView(int roleId, int reportId)
        {
            ViewBag.roleId = roleId;
            REPORTS _report = database.REPORTS.Where(p => p.Report_Id == reportId).FirstOrDefault();
            ROLES _role = database.ROLES.Where(p => p.Role_Id == roleId).FirstOrDefault();
            if (_role != null)
            {
                if (!_role.REPORTS.Contains(_report))
                {
                    _role.REPORTS.Add(_report);
                    database.Entry(_role).State = EntityState.Modified;
                    database.SaveChanges();
                }
            }
            return PartialView("_ListReportsTable4Role", _role.REPORTS.ToList());
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public PartialViewResult AddAllReports2RoleReturnPartialView(int id)
        {
            ViewBag.roleId = id;
            ROLES _role = database.ROLES.Where(p => p.Role_Id == id).FirstOrDefault();
            List<REPORTS> _reports = database.REPORTS.ToList();
            foreach (REPORTS _item in _reports)
            {
                if (!_role.REPORTS.Contains(_item))
                {
                    _role.REPORTS.Add(_item);
                    _role.LastModified = DateTime.Now;
                }
            }
            database.Entry(_role).State = EntityState.Modified;
            database.SaveChanges();
            return PartialView("_ListReportsTable4Role", _role.REPORTS.ToList());
        }

        private List<REPORTS> GetReports4Role(ROLES role)
        {
            List<REPORTS> _retVal = new List<REPORTS>();
            foreach (REPORTS _report in database.REPORTS.Where(r => r.Inactive == false || r.Inactive == null).ToList())
            {
                if (_report.ROLES.Contains(role))
                    _retVal.Add(_report);
            }
            return _retVal;
        }

        public ViewResult ReportIndex()
        {
            List<REPORTS> _reports = database.REPORTS
                               .OrderBy(r => r.ReportName)
                               .ToList();
            return View(_reports);
        }

        public ViewResult ReportDetails(int id)
        {
            REPORTS _reports = database.REPORTS.Find(id);
            return View(_reports);
        }

        public ActionResult ReportCreate()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ReportCreate(REPORTS _report)
        {
            if (_report.ReportDescription == null)
            {
                ModelState.AddModelError("Report Description", "Report Description must be entered");
            }

            if (ModelState.IsValid)
            {
                _report.LastModified = System.DateTime.Now;
                database.REPORTS.Add(_report);
                database.SaveChanges();
                return RedirectToAction("ReportIndex");
            }
            return View(_report);
        }

        public ActionResult ReportEdit(int id)
        {
            REPORTS _report = database.REPORTS.Find(id);
            ViewBag.RoleId = new SelectList(database.ROLES.OrderBy(p => p.RoleDescription), "Role_Id", "RoleName");
            return View(_report);
        }

        [HttpPost]
        public ActionResult ReportEdit(REPORTS _report)
        {
            if (ModelState.IsValid)
            {
                _report.LastModified = System.DateTime.Now;
                database.Entry(_report).State = EntityState.Modified;
                database.SaveChanges();
                return RedirectToAction("ReportDetails", new RouteValueDictionary(new { id = _report.Report_Id }));
            }
            return View(_report);
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult ReportDelete(int id)
        {
            REPORTS _report = database.REPORTS.Find(id);
            if (_report.ROLES.Count > 0)
                _report.ROLES.Clear();

            database.Entry(_report).State = EntityState.Deleted;
            database.SaveChanges();
            return RedirectToAction("ReportIndex");
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public PartialViewResult DeleteRoleFromReportReturnPartialView(int id, int reportId)
        {
            ROLES role = database.ROLES.Find(id);
            REPORTS report = database.REPORTS.Find(reportId);

            if (role.REPORTS.Contains(report))
            {
                role.REPORTS.Remove(report);
                database.SaveChanges();
            }
            return PartialView("_ListRolesTable4Report", report);
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public PartialViewResult AddRole2ReportReturnPartialView(int id, int reportId)
        {
            ROLES role = database.ROLES.Find(id);
            REPORTS _report = database.REPORTS.Find(reportId);

            if (!role.REPORTS.Contains(_report))
            {
                role.REPORTS.Add(_report);
                database.SaveChanges();
            }
            return PartialView("_ListRolesTable4Report", _report);
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public PartialViewResult AddAllRoles2ReportReturnPartialView(int id)
        {
            REPORTS _report = database.REPORTS.Where(p => p.Report_Id == id).FirstOrDefault();
            List<ROLES> _roles = database.ROLES.ToList();
            foreach (ROLES _role in _roles)
            {
                if (!_role.REPORTS.Contains(_report))
                {
                    _role.REPORTS.Add(_report);
                    _role.LastModified = DateTime.Now;
                }
            }
            database.SaveChanges();
            return PartialView("_ListRolesTable4Report", _report);
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public PartialViewResult DeleteReportReturnPartialView(int id)
        {
            REPORTS _report = database.REPORTS.Find(id);
            if (_report.ROLES.Count > 0)
                _report.ROLES.Clear();

            if (_report.PARAMETERS.Count > 0)
                _report.PARAMETERS.Clear();

            database.Entry(_report).State = EntityState.Modified;
            database.SaveChanges();

            database.REPORTS.Remove(_report);
            database.Entry(_report).State = EntityState.Deleted;
            database.SaveChanges();

            return PartialView("_ListReportsTable", database.REPORTS.ToList());
        }


        #region REPORTS PARAMS
        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public PartialViewResult AddReportParameterReturnPartialView(int reportId, string paramName, string paramType, string paramLabel, bool paramRequired)
        {
            try
            {
                REPORTS _report = database.REPORTS.Find(reportId);

                //Create new parameter object and assign values...
                PARAMETERS _reportParameter = new PARAMETERS();
                _reportParameter.ParameterName = paramName;
                _reportParameter.ParameterType = paramType;
                _reportParameter.DisplayLabel = paramLabel;
                _reportParameter.Required = paramRequired;

                _report.PARAMETERS.Add(_reportParameter);

                database.Entry(_report).State = EntityState.Modified;
                database.SaveChanges();
            }
            catch (Exception)
            {
            }
            return PartialView("_ListParamsTable4Report", database.REPORTS.Where(p => p.Report_Id == reportId).FirstOrDefault());
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public PartialViewResult DeleteReportParameterReturnPartialView(int reportId, int paramId)
        {
            try
            {
                REPORTS _report = database.REPORTS.Find(reportId);

                PARAMETERS _filter = database.PARAMETERS.Find(paramId);
                if (_report.PARAMETERS.Contains(_filter))
                {
                    _report.PARAMETERS.Remove(_filter);
                    database.Entry(_report).State = EntityState.Modified;
                    database.PARAMETERS.Remove(_filter);
                    database.SaveChanges();
                }
            }
            catch (Exception)
            {
            }
            return PartialView("_ListParamsTable4Report", database.REPORTS.Where(p => p.Report_Id == reportId).FirstOrDefault());
        }

        #endregion
        #endregion
    }
}