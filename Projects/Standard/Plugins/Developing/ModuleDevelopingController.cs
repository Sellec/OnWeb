using System;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Web.Mvc;
using SevenZip;

using OnUtils.Data;

namespace OnWeb.Plugins.ModuleDeveloping
{
    using Core.Modules;
    using CoreBind.Modules;
    using CoreBind.Routing;
    using Core.DB;

    public class ModuleDevelopingController : ModuleControllerUser<ModuleDeveloping>
    {
        public FileResult BackupDB()
        {
            try
            {
                var connectionString = DataAccessManager.GetConnectionStringResolver()?.ResolveConnectionStringForDataContext(null);
                var connection = new SqlConnectionStringBuilder(connectionString);

                var rootDirectory = System.Web.Hosting.HostingEnvironment.MapPath("/");
                var filePathRelative = $"bin/Backup/{connection.InitialCatalog}.bak";
                var filePath = Path.GetFullPath(Path.Combine(rootDirectory, filePathRelative));

                try
                {
                    var directory = Path.GetDirectoryName(filePath);
                    Directory.CreateDirectory(directory);

                    var dInfo = new DirectoryInfo(directory);
                    var dSecurity = dInfo.GetAccessControl();
                    dSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
                    dInfo.SetAccessControl(dSecurity);
                }
                catch { }

                Debug.WriteLine("{0}, {1}", connection.InitialCatalog, filePath);

                using (var conn = new SqlConnection(connectionString))
                {
                    try
                    {
                        using (var command = new SqlCommand(String.Format("BACKUP DATABASE {0} TO DISK='{1}' WITH FORMAT, INIT, COMPRESSION", connection.InitialCatalog, filePath), conn))
                        {
                            command.CommandTimeout = 0;
                            conn.Open();
                            command.ExecuteNonQuery();
                        }
                    }
                    catch (SqlException ex)
                    {
                        if (ex.Message.Contains("BACKUP DATABASE WITH COMPRESSION"))
                        {
                            using (var command = new SqlCommand(String.Format("BACKUP DATABASE {0} TO DISK='{1}' WITH FORMAT, INIT", connection.InitialCatalog, filePath), conn))
                            {
                                command.CommandTimeout = 0;
                                command.ExecuteNonQuery();
                            }
                        }
                        else throw;
                    }
                }

                if (System.IO.File.Exists(filePath))
                {
                    SevenZipExtractor.SetLibraryPath(@"C:\Program Files\7-Zip\7z.dll");
                    var coder = new SevenZipCompressor();
                    coder.ArchiveFormat = OutArchiveFormat.SevenZip;
                    coder.CompressionLevel = CompressionLevel.Fast;
                    coder.CompressionMethod = CompressionMethod.Lzma2;
                    coder.CompressionMode = CompressionMode.Create;
                    coder.CompressFiles(filePath + ".7z", filePath);

                    filePath = filePath + ".7z";
                }

                byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, Path.GetFileName(filePath));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return null;
            }
        }

        public ActionResult PasswordHash(string password, int? IdUser)
        {
            var str = $"Пароль '{password}' в хешированном виде выглядит как '{UsersExtensions.hashPassword(password)}'.\r\n\r\n<br><br>";
            if (IdUser.HasValue)
            {
                using (var db = new UnitOfWork<User>())
                {
                    var user = db.Repo1.Where(x => x.id == IdUser.Value).FirstOrDefault();
                    if (user == null) str += $"Пользователь {IdUser} не был найден. Пароль не был обновлен.\r\n<br>";
                    else
                    {
                        db.DataContext.ExecuteQuery("UPDATE users SET password='" + UsersExtensions.hashPassword(password) + "' WHERE id='" + IdUser.Value + "'");
                        //user.password = UserManager.hashPassword(password);
                        //db.SaveChanges();

                        str += $"Пароль пользователя {IdUser} ({user.Caption}, {user.email}, {user.phone}) обновлен на '{password}'.\r\n<br>";
                    }
                }
            }
            return Content(str);
        }

        public ActionResult TestAddress(string address)
        {
            var result = AppCore.Get<Core.Addresses.IManager>().SearchAddress(address);
            return Content(result.Result?.KodAddress);
        }

        public ActionResult TestGeo(string address)
        {
            System.Net.IPAddress ip = null;
            if (System.Net.IPAddress.TryParse(address, out ip))
            {
                var result = AppCore.Get<Core.Addresses.IManager>().GetAddressByIP(ip);
                return Content(result.Result?.KodAddress);
            }
            else return Content("not");
        }

        public ActionResult TestError500()
        {
            Session["asdasd"] = DateTime.Now;
            var iss = HttpContext.Session.IsReadOnly;
            Debug.WriteLineNoLog("{0}", iss);

            var d = DateTime.Now;
            while ((DateTime.Now - d).TotalSeconds <= 5) { }

            //throw new Exception("тестовая ошибка");

            return Content("123123123");
        }
    }
}