using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using TraceCore.Data;

namespace OnWeb.Plugins.FileManager
{
    public class Controller : ModuleController<Module, UnitOfWork<DB.File>>
    {
        [HttpPost]
        public ActionResult UploadFile(string moduleName = null, string uniqueKey = null)
        {
            var result = JsonAnswer<int>();

            try
            {
                if (!string.IsNullOrEmpty(uniqueKey) && uniqueKey.Length > 255) throw new ArgumentOutOfRangeException(nameof(uniqueKey), "Длина уникального ключа не может быть больше 255 символов.");
                if (string.IsNullOrEmpty(moduleName)) throw new ArgumentNullException(nameof(moduleName), "Не указан модуль, для которого загружается файл.");

                var module = ModulesManager.getModuleByName(moduleName) ?? ModulesManager.getModuleByNameBase(moduleName);
                if (module == null) throw new Exception("Указанный модуль не найден.");

                var hpf = HttpContext.Request.Files["file"] as HttpPostedFileBase;
                if (hpf == null) throw new ArgumentNullException("Не найден загружаемый файл.");

                var rootDirectory = System.Web.Hosting.HostingEnvironment.MapPath("/");
                var filePathRelative = Path.Combine("data/filesClosed/", Guid.NewGuid().ToString() + Path.GetExtension(hpf.FileName));
                var filePath = Path.Combine(rootDirectory, filePathRelative);
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                hpf.SaveAs(filePath);
                var file = TraceWeb.FileManager.Register(module, hpf.FileName, filePathRelative, UserManager.Instance.getID(), uniqueKey, DateTime.Now.AddDays(1));
                if (file == null) throw new Exception(TraceWeb.FileManager.getError());

                result.Data = file.IdFile;
                result.FromSuccess("");
            }
            catch (Exception ex) { result.FromException(ex); }

            return ReturnJson(result);
        }

        [Route("{url}")]
        public ActionResult DownloadFile(string url)
        {
            return File(url, "application/pdf");
        }

        [HttpPost]
        public ActionResult DeleteFile(string url)
        {
            try
            {
                System.IO.File.Delete(url);
                var msg = new { msg = "File Deleted!" };
                return Json(msg);
            }
            catch (Exception e)
            {
                //If you want this working with a custom error you need to change the name of 
                //variable customErrorKeyStr in line 85, from jquery-upload-file-error to jquery_upload_file_error 
                var msg = new { jquery_upload_file_error = e.Message };
                return Json(msg);
            }
        }

        [ModuleAction("file")]
        public FileResult File(int? IdFile = null)
        {
            try
            {
                if (!IdFile.HasValue) throw new ArgumentNullException(nameof(IdFile), "Не указан номер файла.");

                var file = DB.Repo1.Where(x => x.IdFile == IdFile.Value).Select(x => new { x.PathFile, x.NameFile }).FirstOrDefault();
                if (file == null) throw new Exception("Файл не найден.");

                var rootDirectory = System.Web.Hosting.HostingEnvironment.MapPath("/");

                byte[] fileBytes = System.IO.File.ReadAllBytes(Path.Combine(rootDirectory, file.PathFile));
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, file.NameFile);
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
                    var file = DB.Repo1.Where(x => x.IdFile == IdFile.Value).Select(x => new { x.PathFile, x.NameFile }).FirstOrDefault();
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
                    var file = DB.Repo1.Where(x => x.IdFile == IdFile.Value).Select(x => new { x.PathFile, x.NameFile, x.DateChange }).FirstOrDefault();
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

        public ActionResult SuggestionsAddress()
        {
            if (Request.Path.Contains("suggest/address"))
            {
                try
                {
                    var data1 = Request.Headers.AllKeys.ToDictionary(x => x, x => Request.Headers[x]);
                    var data2 = Request.Form.AllKeys.ToDictionary(x => x, x => Request.Headers[x]);
                    var data3 = Request.QueryString.AllKeys.ToDictionary(x => x, x => Request.Headers[x]);

                    string documentContents;
                    using (var receiveStream = Request.InputStream)
                    {
                        using (var readStream = new StreamReader(receiveStream, Request.ContentEncoding))
                        {
                            documentContents = readStream.ReadToEnd();
                        }
                    }

                    var dd = Newtonsoft.Json.JsonConvert.DeserializeObject<DaData.Entities.Suggestions.AddressSuggestQuery>(documentContents);
                    if (dd != null)
                    {
                        var client = AddressManager.Instance.GetDadataClient();
                        var result = client.QueryAddress(dd);

                        if (result.IsSuccess && result.Data != null && result.Data.suggestions != null)
                        {
                            var preparedResult = AddressManager.Instance.PrepareAddressDataIntoDB(result.Data.suggestions.Select(x =>
                            {
                                //if (x.data.fias_level == "8") x.data.ka
                                return x.data;
                            }).ToArray());

                            if (preparedResult != null)
                                foreach (var pair in preparedResult)
                                    if (pair.Key.kladr_id == pair.Value.KodBuildingCommon)
                                        pair.Key.kladr_id = pair.Value.KodBuilding;
                        }

                        var resultText = Newtonsoft.Json.JsonConvert.SerializeObject(result.Data);
                        return Content(resultText);
                    }

                    var d = Request.Params;
                }
                catch (Exception ex)
                {
                }
            }

            return null;
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