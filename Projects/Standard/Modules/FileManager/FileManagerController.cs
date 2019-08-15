using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnWeb.Modules.FileManager
{
    using Core.Modules;

    public class FileManagerController : ModuleControllerUser<FileManager>
    {
        public override ActionResult Index()
        {
            return Content("");
        }

        [HttpPost]
        public ActionResult UploadFile(string moduleName = null, string uniqueKey = null)
        {
            var result = JsonAnswer<int>();

            try
            {
                if (!string.IsNullOrEmpty(uniqueKey) && uniqueKey.Length > 255) throw new ArgumentOutOfRangeException(nameof(uniqueKey), "Длина уникального ключа не может быть больше 255 символов.");
                if (string.IsNullOrEmpty(moduleName)) throw new ArgumentNullException(nameof(moduleName), "Не указан модуль, для которого загружается файл.");

                var module = AppCore.GetModulesManager().GetModule(moduleName) ?? (int.TryParse(moduleName, out int idModule) ? AppCore.GetModulesManager().GetModule(idModule) : null);
                if (module == null) throw new Exception("Указанный модуль не найден.");

                var hpf = HttpContext.Request.Files["file"] as HttpPostedFileBase;
                if (hpf == null) throw new ArgumentNullException("Не найден загружаемый файл.");

                var rootDirectory = System.Web.Hosting.HostingEnvironment.MapPath("/");
                var filePathRelative = Path.Combine("data/filesClosed/", Guid.NewGuid().ToString() + Path.GetExtension(hpf.FileName));
                var filePath = Path.Combine(rootDirectory, filePathRelative);
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                hpf.SaveAs(filePath);
                switch (Module.Register(out var file, hpf.FileName, filePathRelative, uniqueKey, DateTime.Now.AddDays(1)))
                {
                    case RegisterResult.Error:
                    case RegisterResult.NotFound:
                        result.FromFail("Не удалось зарегистрировать переданный файл.");
                        break;

                    case RegisterResult.Success:
                        result.Data = file.IdFile;
                        result.FromSuccess("");
                        break;
                }
            }
            catch (Exception ex)
            {
                this.RegisterEventWithCode(System.Net.HttpStatusCode.InternalServerError, "Ошибка во время регистрации файла", $"moduleName='{moduleName}'.\r\nuniqueKey='{uniqueKey}'.", ex);
                result.FromFail("Неожиданная ошибка во время регистрации файла.");
            }

            return ReturnJson(result);
        }

        [ModuleAction("file")]
        public FileResult File(int? IdFile = null)
        {
            try
            {
                if (!IdFile.HasValue) throw new ArgumentNullException(nameof(IdFile), "Не указан номер файла.");

                using (var db = Module.CreateUnitOfWork())
                {
                    var file = db.File.Where(x => x.IdFile == IdFile.Value && !x.IsRemoved && !x.IsRemoving).Select(x => new { x.PathFile, x.NameFile }).FirstOrDefault();
                    if (file == null) throw new Exception("Файл не найден.");

                    var rootDirectory = System.Web.Hosting.HostingEnvironment.MapPath("/");
                    var filePath = Path.Combine(rootDirectory, file.PathFile);

                    return base.File(filePath, System.Net.Mime.MediaTypeNames.Application.Octet, file.NameFile);

                    byte[] fileBytes = System.IO.File.ReadAllBytes(Path.Combine(rootDirectory, file.PathFile));
                    var result = File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, file.NameFile);
                    Response.Headers["Content-Length"] = fileBytes.Length.ToString();
                    return result;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [ModuleAction("image")]
        public ActionResult FileImage(int? IdFile = null, int? MaxWidth = null, int? MaxHeight = null)
        {
            try
            {
                var rootDirectory = System.Web.Hosting.HostingEnvironment.MapPath("/");
                var filePath = string.Empty;
                var fileName = string.Empty;

                if (!IdFile.HasValue) filePath = "data/img/files/argumentzero.jpg"; //Не указан номер файла.
                else
                {
                    using (var db = Module.CreateUnitOfWork())
                    {
                        var file = db.File.Where(x => x.IdFile == IdFile.Value && !x.IsRemoved && !x.IsRemoving).Select(x => new { x.PathFile, x.NameFile }).FirstOrDefault();
                        if (file == null) filePath = "data/img/files/notfound.jpg"; //Файл не найден.
                        else
                        {
                            if (!System.IO.File.Exists(Path.Combine(rootDirectory, file.PathFile)) && !System.IO.File.Exists(Path.Combine(rootDirectory, "bin", file.PathFile)))
                            {
                                filePath = "data/img/files/notfound.jpg"; //Файл не найден.
                                if (Debug.IsDeveloper)
                                {
                                    var paramss = string.Join("&", new List<string>() { !MaxWidth.HasValue ? null : "MaxWidth=" + MaxWidth.Value, !MaxHeight.HasValue ? null : "MaxHeight=" + MaxHeight.Value }.
                                    Where(x => !string.IsNullOrEmpty(x)));

                                    var url = "http://dombonus.ru/fm/image/" + IdFile.Value + (!string.IsNullOrEmpty(paramss) ? "?" + paramss : "");
                                    return Redirect(url);
                                }
                            }
                            else
                            {
                                filePath = file.PathFile;
                                fileName = file.NameFile;
                            }
                        }
                    }
                }

                string path = null;

                if (System.IO.File.Exists(Path.Combine(rootDirectory, filePath))) path = Path.Combine(rootDirectory, filePath);
                if (System.IO.File.Exists(Path.Combine(rootDirectory, "bin", filePath))) path = Path.Combine(rootDirectory, "bin", filePath);

                if (!string.IsNullOrEmpty(path))
                {
                    var isNeedResize = MaxWidth.HasValue || MaxHeight.HasValue;
                    if (!isNeedResize)
                    {
                        byte[] fileBytes = System.IO.File.ReadAllBytes(path);
                        return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, string.IsNullOrEmpty(fileName) ? Path.GetFileName(filePath) : fileName);
                    }
                    else
                    {
                        using (var image = Image.FromFile(path))
                        using (var imagePreview = image.Resize(MaxWidth.HasValue ? MaxWidth.Value : 0, MaxHeight.HasValue ? MaxHeight.Value : 0))
                        {
                            //using (var stream = new System.IO.MemoryStream())
                            {
                                var stream = new System.IO.MemoryStream();
                                imagePreview.Save(stream, image.RawFormat);
                                stream.Position = 0;
                                return File(stream, System.Net.Mime.MediaTypeNames.Application.Octet, string.IsNullOrEmpty(fileName) ? Path.GetFileName(filePath) : fileName);
                            }
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [ModuleAction("imageCrop")]
        public ActionResult FileImageCrop(int? IdFile = null, int? MaxWidth = null, int? MaxHeight = null)
        {
            try
            {
                var rootDirectory = System.Web.Hosting.HostingEnvironment.MapPath("/");
                var filePath = string.Empty;
                var fileName = string.Empty;
                DateTime? dbChangeTime = null;

                if (!IdFile.HasValue) filePath = "data/img/files/argumentzero.jpg"; //Не указан номер файла.
                else
                {
                    using (var db = Module.CreateUnitOfWork())
                    {
                        var file = db.File.Where(x => x.IdFile == IdFile.Value && !x.IsRemoved && !x.IsRemoving).Select(x => new { x.PathFile, x.NameFile, x.DateChange }).FirstOrDefault();
                        if (file == null) filePath = "data/img/files/notfound.jpg"; //Файл не найден.
                        else
                        {
                            if (!System.IO.File.Exists(Path.Combine(rootDirectory, file.PathFile)) && !System.IO.File.Exists(Path.Combine(rootDirectory, "bin", file.PathFile)))
                            {
                                filePath = "data/img/files/notfound.jpg"; //Файл не найден.
                                if (Debug.IsDeveloper)
                                {
                                    var paramss = string.Join("&", new List<string>() { !MaxWidth.HasValue ? null : "MaxWidth=" + MaxWidth.Value, !MaxHeight.HasValue ? null : "MaxHeight=" + MaxHeight.Value }.
                                    Where(x => !string.IsNullOrEmpty(x)));

                                    var url = "http://dombonus.ru/fm/imageCrop/" + IdFile.Value + (!string.IsNullOrEmpty(paramss) ? "?" + paramss : "");
                                    return Redirect(url);
                                }
                            }
                            else
                            {
                                filePath = file.PathFile;
                                fileName = file.NameFile;
                                dbChangeTime = file.DateChange.FromTimestamp();
                            }
                        }
                    }
                }

                string path = null;

                if (System.IO.File.Exists(Path.Combine(rootDirectory, filePath))) path = Path.Combine(rootDirectory, filePath);
                if (System.IO.File.Exists(Path.Combine(rootDirectory, "bin", filePath))) path = Path.Combine(rootDirectory, "bin", filePath);

                if (!string.IsNullOrEmpty(path))
                {
                    var isNeedResize = MaxWidth.HasValue || MaxHeight.HasValue;
                    if (!isNeedResize)
                    {
                        byte[] fileBytes = System.IO.File.ReadAllBytes(path);
                        return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, string.IsNullOrEmpty(fileName) ? Path.GetFileName(filePath) : fileName);
                    }
                    else
                    {
                        var name = $"cropped_{(MaxWidth.HasValue ? MaxWidth.Value : 0)}_{(MaxHeight.HasValue ? MaxHeight.Value : 0)}_" + (IdFile.HasValue ? IdFile.Value + Path.GetExtension(path) : Path.GetFileName(path));
                        var filePathRelative = Path.Combine("data/filesModified/", name);
                        var filePath2 = Path.Combine(rootDirectory, filePathRelative);
                        Directory.CreateDirectory(Path.GetDirectoryName(filePath2));

                        if (System.IO.File.Exists(filePath2))
                        {
                            var fileChangeTime = System.IO.File.GetLastWriteTimeUtc(filePath2);
                            if (dbChangeTime.HasValue && dbChangeTime.Value > fileChangeTime) System.IO.File.Delete(filePath2);
                        }

                        if (!System.IO.File.Exists(filePath2))
                        {
                            var image = cropImage(path, MaxWidth.HasValue ? MaxWidth.Value : 0, MaxHeight.HasValue ? MaxHeight.Value : 0);
                            {
                                image.Item1.Save(filePath2, image.Item2);
                                if (dbChangeTime.HasValue) System.IO.File.SetLastWriteTimeUtc(filePath2, dbChangeTime.Value);
                            }
                        }

                        if (System.IO.File.Exists(filePath2))
                        {
                            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath2);
                            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, string.IsNullOrEmpty(fileName) ? Path.GetFileName(filePath) : fileName);
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private Tuple<Image, ImageFormat> cropImage(string aInitialImageFilePath, int aNewImageWidth, int aNewImageHeight)
        {
            if (aNewImageWidth < 0 || aNewImageHeight < 0) return null;

            // Массив с поддерживаемыми типами изображений
            var lAllowedExtensions = new List<ImageFormat>() { ImageFormat.Gif, ImageFormat.Jpeg, ImageFormat.Png };

            using (var image = Image.FromFile(aInitialImageFilePath))
            {
                // Получаем размеры и тип изображения в виде числа
                decimal lInitialImageWidth = image.Width;
                decimal lInitialImageHeight = image.Height;

                var lImageExtensionId = image.RawFormat;

                if (!lAllowedExtensions.Contains(lImageExtensionId)) return null;
                var lImageExtension = lImageExtensionId;

                // Определяем отображаемую область
                decimal lCroppedImageWidth = 0;
                decimal lCroppedImageHeight = 0;
                decimal lInitialImageCroppingX = 0;
                decimal lInitialImageCroppingY = 0;
                if (aNewImageWidth / aNewImageHeight > lInitialImageWidth / lInitialImageHeight)
                {
                    lCroppedImageWidth = Math.Floor(lInitialImageWidth);
                    lCroppedImageHeight = Math.Floor(lInitialImageWidth * aNewImageHeight / aNewImageWidth);
                    lInitialImageCroppingY = Math.Floor((lInitialImageHeight - lCroppedImageHeight) / 2);
                }
                else
                {
                    lCroppedImageWidth = Math.Floor(lInitialImageHeight * aNewImageWidth / aNewImageHeight);
                    lCroppedImageHeight = Math.Floor(lInitialImageHeight);
                    lInitialImageCroppingX = Math.Floor((lInitialImageWidth - lCroppedImageWidth) / 2);
                }

                var newImage = new Bitmap(aNewImageWidth, aNewImageHeight, image.PixelFormat);
                using (var gr = Graphics.FromImage(newImage))
                {
                    //gr.SmoothingMode = SmoothingMode.HighQuality;
                    // gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    //gr.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    gr.DrawImage(image, new Rectangle(0, 0, aNewImageWidth, aNewImageHeight), new Rectangle((int)lInitialImageCroppingX, (int)lInitialImageCroppingY, (int)lCroppedImageWidth, (int)lCroppedImageHeight), GraphicsUnit.Pixel);
                }

                return new Tuple<Image, ImageFormat>(newImage, lImageExtensionId);
            }
        }
    }
}
