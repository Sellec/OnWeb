using OnUtils.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OnWeb.Messaging
{
    using Core;
    using Core.DB;
    using Modules.MessagingEmail;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Compiler", "CS0618")]
    class SubscriptionsManager : CoreComponentBase, ISubscriptionsManager, IUnitOfWorkAccessor<CoreContext>
    {
        #region CoreComponentBase
        protected sealed override void OnStart()
        {
        }

        protected sealed override void OnStop()
        {
        }
        #endregion

        List<Subscription> ISubscriptionsManager.getList(bool? isEnabled, bool? isSubscriptionAllowed)
        {
            using (var db = this.CreateUnitOfWork())
            {
                var list = (from p in db.Subscription
                            where (!isEnabled.HasValue || p.status == (isEnabled.Value ? 1 : 0)) &&
                                    (!isSubscriptionAllowed.HasValue || p.AllowSubscribe == (isSubscriptionAllowed.Value ? 1 : 0))
                            orderby p.name
                            select p).ToList();

                return list;
            }
        }

        Subscription ISubscriptionsManager.create(string name, bool allowSubscribe)
        {
            try
            {
                // setError(null);// todo

                using (var db = this.CreateUnitOfWork())
                {
                    var subscription = new Subscription() { name = name, AllowSubscribe = (byte)(allowSubscribe ? 1 : 0), description = "", status = 1 };
                    db.Subscription.Add(subscription);

                    if (db.SaveChanges() == 0) throw new Exception("Не удалось добавить лист рассылки.");

                    return subscription;
                }
            }
            catch (Exception ex)
            {
                // setError(ex.Message); //todo
                return null;
            }
        }

        bool ISubscriptionsManager.send(int IdSubscription, string subject, string body, ContentType contentType, List<int> files, ICollection<string> excludedAddresses)
        {
            try
            {
                // setError(null); //todo

                if (string.IsNullOrEmpty(body) || string.IsNullOrEmpty(body.Trim())) throw new Exception("Тело письма не может быть пустым.");

                using (var db = this.CreateUnitOfWork())
                {
                    var data = db.Subscription.Where(x => x.id == IdSubscription).FirstOrDefault();
                    if (data == null) throw new Exception("Такой лист рассылки не найден в базе данных!");
                    if (data.status == 0) throw new Exception("Лист рассылки отключен!");

                    if (string.IsNullOrEmpty(subject)) subject = data.name;

                    var emails = (from p in db.SubscriptionEmail where p.subscr_id == IdSubscription select new { Email = p.email, Name = p.email }).ToDictionary(x => x.Email, x => x.Name);

                    (from s in db.SubscriptionRole
                     join r in db.RoleUser on s.IdRole equals r.IdRole
                     join u in db.Users on r.IdUser equals u.IdUser
                     where s.IdSubscription == IdSubscription
                     select u).ToList().ForEach(x => emails[x.email] = x.Caption);

                    foreach (var pair in emails) Debug.WriteLine("IdSubscription: {0} : {1}", pair.Key, pair.Value);

                    bool full = true;
                    foreach (var pair in emails)
                    {
                        if (excludedAddresses != null && excludedAddresses.Contains(pair.Key)) continue;

                        AppCore.Get<IEmailService>().SendMailFromSite(pair.Value, pair.Key, subject, body, contentType, files);
                    }

                    if (!full) Debug.WriteLine("IdSubscription={0}. Исполнен частично.", IdSubscription);
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                //setError(ex.Message); //todo
                return false;
            }
        }

        bool ISubscriptionsManager.subscribeEmail(int IdSubscription, string email)
        {
            try
            {
                // setError(null); //todo

                if (string.IsNullOrEmpty(email)) throw new Exception("Email не может быть пустым.");
                email = email.ToLower();

                using (var db = this.CreateUnitOfWork())
                {
                    var data = db.Subscription.Where(x => x.id == IdSubscription).FirstOrDefault();
                    if (data == null) throw new Exception("Такой лист рассылки не найден в базе данных!");

                    var context = AppCore.GetUserContextManager().GetCurrentUserContext();

                    using (var scope = db.CreateScope())
                    {
                        db.SubscriptionEmail.AddOrUpdate(new SubscriptionEmail()
                        {
                            subscr_id = IdSubscription,
                            email = email,
                            IdUserChange = context.IdUser,
                            DateChange = DateTime.Now.Timestamp()
                        });

                        if (db.SaveChanges() == 0) throw new Exception("Не удалось добавить подписчика в лист рассылки.");

                        db.SubscriptionHistory.AddOrUpdate(new SubscriptionHistory() { email = email, subscr_id = IdSubscription });
                        db.SaveChanges();

                        scope.Commit();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                //setError(ex.Message);
                return false;
            }
        }

        bool ISubscriptionsManager.subscribeRole(int IdSubscription, int IdRole)
        {
            try
            {
                //setError(null);

                using (var db = this.CreateUnitOfWork())
                {
                    var data = db.Subscription.Where(x => x.id == IdSubscription).FirstOrDefault();
                    if (data == null) throw new Exception("Такой лист рассылки не найден в базе данных!");

                    var role = db.Role.Where(x => x.IdRole == IdRole).FirstOrDefault();
                    if (role == null) throw new Exception("Такая роль не найдена в базе данных.");

                    var context = AppCore.GetUserContextManager().GetCurrentUserContext();

                    using (var scope = db.CreateScope())
                    {
                        db.SubscriptionRole.AddOrUpdate(new SubscriptionRole()
                        {
                            IdSubscription = IdSubscription,
                            IdRole = IdRole,
                            IdUserChange = context.IdUser,
                            DateChange = DateTime.Now.Timestamp()
                        });

                        if (db.SaveChanges() == 0) throw new Exception("Не удалось добавить роль в лист рассылки.");
                        scope.Commit();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                //setError(ex.Message);
                return false;
            }
        }

        bool ISubscriptionsManager.unsubscribeEmail(int IdSubscription, string email)
        {
            try
            {
                //setError(null);

                using (var db = this.CreateUnitOfWork())
                {
                    var data = db.Subscription.Where(x => x.id == IdSubscription).FirstOrDefault();
                    if (data == null) throw new Exception("Такой лист рассылки не найден в базе данных!");

                    var rows = db.SubscriptionEmail.Where(x => x.subscr_id == IdSubscription && x.email == email.ToLower()).Delete();
                }
                return true;
            }
            catch (Exception ex)
            {
                //setError(ex.Message);
                return false;
            }
        }


    }
}