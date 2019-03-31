using System;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Web.Mvc;

using TraceCore.Data;

namespace OnWeb.Plugins.Developing
{
    public class Controller : ModuleController<Module>
    {
        public FileResult BackupDB()
        {
            try
            {
                var connectionString = ConnectionStringFactory.Instance.Providers.First().GetConnectionString();
                var connection = new System.Data.SqlClient.SqlConnectionStringBuilder(connectionString);

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
                    SevenZip.SevenZipExtractor.SetLibraryPath(@"C:\Program Files\7-Zip\7z.dll");
                    var coder = new SevenZip.SevenZipCompressor();
                    coder.ArchiveFormat = SevenZip.OutArchiveFormat.SevenZip;
                    coder.CompressionLevel = SevenZip.CompressionLevel.Fast;
                    coder.CompressionMethod = SevenZip.CompressionMethod.Lzma2;
                    coder.CompressionMode = SevenZip.CompressionMode.Create;
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
            var str = $"Пароль '{password}' в хешированном виде выглядит как '{UserManager.hashPassword(password)}'.\r\n\r\n<br><br>";
            if (IdUser.HasValue)
            {
                using (var db = new UnitOfWork<DB.User>())
                {
                    var user = db.Repo1.Where(x => x.id == IdUser.Value).FirstOrDefault();
                    if (user == null) str += $"Пользователь {IdUser} не был найден. Пароль не был обновлен.\r\n<br>";
                    else
                    {
                        db.DataContext.ExecuteQuery("UPDATE users SET password='" + UserManager.hashPassword(password) + "' WHERE id='" + IdUser.Value + "'");
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
            var addr = AddressManager.Instance.searchAddressByOneString(address);
            return Content(addr?.KodAddress);
        }

        public ActionResult TestGeo(string address)
        {
            System.Net.IPAddress ip = null;
            if (System.Net.IPAddress.TryParse(address, out ip))
            {
                var addr = AddressManager.Instance.GetAddressByIP(ip);
                return Content(addr?.KodAddress);
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