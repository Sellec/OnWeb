using OnUtils.Application.Users;
using System;

namespace OnWeb.Core.Users
{
    class UserContext : CoreComponentBase, IUserContext
    {
        private int _idUser = 0;
        private bool _isAuthorized;
        private DB.User _data;
        private UserPermissions _permissions;

        public UserContext(DB.User data, bool isAuthorized)
        {
            _idUser = data.IdUser;
            _data = data;
            _isAuthorized = isAuthorized;
            _permissions = new UserPermissions();
        }

        public void ApplyPermissions(UserPermissions permissions = null)
        {
            _permissions = permissions ?? new UserPermissions();
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
            get => !_isAuthorized ? 0 : this._data.IdUser;
        }

        int IUserContext.IdUser
        {
            get => _idUser;
        }

        bool IUserContext.IsGuest
        {
            get => !_isAuthorized;
        }

        bool IUserContext.IsSuperuser
        {
            get => _isAuthorized && _data.Superuser != 0;
        }

        UserPermissions IUserContext.Permissions
        {
            get => _permissions;
        }
        #endregion
    }
}
