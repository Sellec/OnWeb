using OnUtils.Application.Users;
using OnUtils.Architecture.AppCore;
using System;

namespace OnWeb.Core.Users
{
    class UserManager : CoreComponentBase<ApplicationCore>, IUserContext
    {
        #region Static
        private static UserManager CreateSystemUserInstance()
        {
            var db = new DB.User()
            {
                id = int.MaxValue - 1,
                email = string.Empty,
                phone = string.Empty,
                name = "System",
                password = string.Empty,
                Superuser = 1,
            };
            return new UserManager(db, true);
        }

        // todo заняться этим методом в рамках привязки к aspnet.
        ///// <summary>
        ///// Возвращает или задает адрес для переадресации после успешной авторизации.
        ///// </summary>
        //public static System.Web.Routing.RouteValueDictionary AuthorizationRedirect
        //{
        //    get
        //    {
        //        if (HttpContext.Current.Session != null && HttpContext.Current.Session["AuthorizationRedirect"] != null)
        //        {
        //            var d = HttpContext.Current.Session["AuthorizationRedirect"] as List<string>;
        //            if (d != null)
        //            {
        //                var dd = new System.Web.Routing.RouteValueDictionary();
        //                foreach (var pp in d)
        //                {
        //                    if (pp != null)
        //                    {
        //                        var pair = pp.Split(':');
        //                        if (pair.Length == 2) dd.Add(pair[0], pair[1]);
        //                    }
        //                }

        //                return dd;
        //            }
        //        }

        //        return null;
        //    }
        //    set
        //    {
        //        if (HttpContext.Current.Session != null)
        //        {
        //            var route = new List<string>(from p in value select p.Key + ":" + (p.Value != null ? (string)p.Value : ""));
        //            HttpContext.Current.Session["AuthorizationRedirect"] = route;
        //        }
        //    }
        //}

        #endregion

        private Guid _userID = Guid.Empty;
        private bool _isAuthorized;
        private DB.User _data;
        private PermissionsList _permissions;

        public UserManager(DB.User data, bool isAuthorized, PermissionsList permissions = null)
        {
            _userID = GuidIdentifierGenerator.GenerateGuid(GuidType.User, data.id);
            _data = data;
            _isAuthorized = isAuthorized;
            _permissions = permissions ?? new PermissionsList();
        }

        #region CoreComponentBase
        protected sealed override void OnStart()
        {
        }

        protected sealed override void OnStop()
        {
        }
        #endregion

        #region Permissions
        ///// <summary>
        ///// Обновляет данные и список разрешений пользователя.
        ///// </summary>
        //public void UpdateDataAndPermissions()
        //{
        //    _permissions = null;
        //    getPermissions();

        //    using (var db = new UnitOfWork<DB.User>())
        //    {
        //        var data = db.Repo1.Where(x => x.id == ID).FirstOrDefault();
        //        if (data != null) this._data = data;
        //    }
        //}

        ///**
        //* Возвращает список разрешений, доступных указанному пользователю.
        //* 
        //* @param mixed $IdUser              Если равен 0 или ID текущего пользователя, то возвращает разрешения текущего пользователя. В другом случае возвращает разрешения указанного пользователя.
        //*/
        //public static Users.UserPermissionsList getPermissions(int IdUser = 0)
        //{
        //    var instance = UserManager.Instance;

        //    if (IdUser == 0 && instance == null) return null;
        //    if (IdUser == 0 && instance != null) return instance.getPermissions();

        //    if (_cachedUserPermissions == null) _cachedUserPermissions = new Dictionary<int, Users.UserPermissionsList>();

        //    if (!_cachedUserPermissions.ContainsKey(IdUser))
        //    {
        //        var perms = Users.loadPermissions(IdUser);
        //        _cachedUserPermissions.SetWithExpiration(IdUser, perms, TimeSpan.FromMinutes(1));
        //    }
        //    return _cachedUserPermissions[IdUser];
        //}

        //[ThreadStatic]
        //private static Dictionary<int, Users.UserPermissionsList> _cachedUserPermissions;

        ////public function checkUserByRole($IdRole = null)
        ////{
        ////    try
        ////    {
        ////        $roles = User.rolesByUser($this.getID());
        ////        if (!is_array($IdRole)) $IdRole = array($IdRole);

        ////        foreach ($IdRole as $Id) 
        ////            if (isset($roles[$Id]))
        ////                return true;
        ////    }
        ////    catch (Exception $ex)
        ////    {

        ////    }
        ////    return false;
        ////}

        #endregion

        public DB.User GetData()
        {
            return _data;
        }

        public int IdUser
        {
            get
            {
                if (!_isAuthorized) return 0;
                return this._data.id;
            }
        }

        Guid IUserContext.UserID
        {
            get => _userID;
        }

        bool IUserContext.IsGuest
        {
            get => !_isAuthorized;
        }

        bool IUserContext.IsSuperuser
        {
            get => _isAuthorized && _data.Superuser != 0;
        }

        PermissionsList IUserContext.Permissions
        {
            get => _permissions;
        }
    }
}