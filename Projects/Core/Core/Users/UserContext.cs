using OnUtils.Application.Users;
using OnUtils.Architecture.AppCore;
using System;

namespace OnWeb.Core.Users
{
    class UserContext : CoreComponentBase<ApplicationCore>, IUserContext
    {
        private Guid _userID = Guid.Empty;
        private bool _isAuthorized;
        private DB.User _data;
        private PermissionsList _permissions;

        public UserContext(DB.User data, bool isAuthorized)
        {
            _userID = GuidIdentifierGenerator.GenerateGuid(GuidType.User, data.id);
            _data = data;
            _isAuthorized = isAuthorized;
            _permissions = new PermissionsList();
        }

        public void ApplyPermissions(PermissionsList permissions = null)
        {
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

        public DB.User GetData()
        {
            return _data;
        }

        #region Свойства
        public int IdUser
        {
            get => !_isAuthorized ? 0 : this._data.id;
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
        #endregion
    }
}
