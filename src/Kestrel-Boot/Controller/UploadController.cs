using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace HttpBroker
{
    [Produces("application/json")]
    [Route("vsse/file/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        IConfiguration _config;
        IOption<Config> _opt;
        ISysService<UserFile> _service;
        ILogger<UploadController> _logger;
        private static readonly FormOptions _defaultFormOptions = new FormOptions();
        private readonly string _serverPath;
        public UploadController(IOption<Config> option, IConfiguration config, ISysService<UserFile> svr, ILogger<UploadController> log)
        {
            _opt = option;
            _config = config;
            _service = svr;
            _logger = log;
            _serverPath = Path.Combine(Directory.GetCurrentDirectory(), "Public");
        }

        [HttpGet("list")]
        [Authorize]
        public IActionResult FileList(int pageIndex, int pageSize)
        {
            var data = _service.Get(pageIndex - 1, pageSize);
            return new JsonResult(data);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteFile(string id)
        {
            _service.Remove(id);
            return new JsonResult(new { code = 0 });
        }

        public class UploadModel
        {
            public string Name { get; set; }
            public string OriginalName { get; set; }
            public int Chunk { get; set; }
            public int Chunks { get; set; }
            public int Offset { get; set; }
            public IFormFile File { get; set; }
        }

        [HttpPost("file")]
        public async Task<IActionResult> UploadFile([FromForm] UploadModel model)
        {
            UserFile file = new UserFile();
            file.Name = model.Name;
            file.OriginalName = model.OriginalName;
            file.Ext = Path.GetExtension(model.OriginalName);
            file.FileType = GetFileType(file.Ext);
            file.CreatedAt = DateTime.Now;

            string root = Path.Combine(_serverPath, "upload", DateTime.Now.ToString("yyyyMMdd"));

            CreateDirectory(root);

            string filePath = Path.Combine(root, model.Name);

            long length = -1;
            if (model.File != null)
            {
                length = model.File.Length;
                using (var fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    fs.Seek(model.Offset, SeekOrigin.Begin);
                    await model.File.CopyToAsync(fs);
                }
            }
            file.Url = filePath.Replace(_serverPath, "").Replace("\\", "/");
            file.Length = length;
            if (model.Chunk + 1 == model.Chunks)
            {
                var m = _service.Create(file);
                return Ok(new { id = m.Id, finish = true, url = file.Url });
            }
            else
            {
                return Ok(new { finish = false });
            }
        }

        [HttpPost("bigfile")]
        [DisableFormValueModelBinding]
        public async Task<IActionResult> UploadFile()
        {
            if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
            {
                return BadRequest($"Expected a multipart request, but got {Request.ContentType}");
            }

            UserFile file = null;

            var boundary = MultipartRequestHelper.GetBoundary(
                MediaTypeHeaderValue.Parse(Request.ContentType),
                _defaultFormOptions.MultipartBoundaryLengthLimit);
            var reader = new MultipartReader(boundary, HttpContext.Request.Body);
            try
            {
                var section = await reader.ReadNextSectionAsync();
                while (section != null)
                {
                    ContentDispositionHeaderValue contentDisposition;
                    var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out contentDisposition);

                    if (hasContentDispositionHeader)
                    {
                        if (MultipartRequestHelper.HasFileContentDisposition(contentDisposition))
                        {
                            var originalName = contentDisposition.FileName.Value;

                            file = new UserFile();
                            file.OriginalName = originalName;
                            file.Ext = Path.GetExtension(originalName);
                            file.FileType = GetFileType(file.Ext);
                            file.CreatedAt = DateTime.Now;
                            file.Name = Guid.NewGuid() + file.Ext;

                            string root = Path.Combine(_serverPath, "upload", DateTime.Now.ToString("yyyyMMdd"));
                            CreateDirectory(root);
                            string targetFilePath = Path.Combine(root, file.Name);

                            using (var targetStream = System.IO.File.Create(targetFilePath))
                            {
                                await section.Body.CopyToAsync(targetStream);
                                _logger.LogInformation($"Copied the uploaded file '{targetFilePath}'");
                            }

                            file.Url = targetFilePath.Replace(_serverPath, "").Replace("\\", "/");
                            file.Length = section.Body.Length;
                        }
                    }
                    section = await reader.ReadNextSectionAsync();
                }
            }
            catch (Exception ex)
            {

            }
         
            if (file != null)
            {
                var m = _service.Create(file);
                return Ok(new { code = 0, id = m.Id, finish = true, url = file.Url });
            }
            else
            {
                return BadRequest(new { code = -1, message = "not found file stream" });
            }
        }

        /// <summary>
        /// DownloadBigFile用于下载大文件，循环读取大文件的内容到服务器内存，然后发送给客户端浏览器
        /// </summary>
        public IActionResult DownloadBigFile()
        {
            var filePath = @"D:\Download\测试文档.xlsx";//要下载的文件地址，这个文件会被分成片段，通过循环逐步读取到ASP.NET Core中，然后发送给客户端浏览器
            var fileName = Path.GetFileName(filePath);//测试文档.xlsx

            int bufferSize = 1024;//这就是ASP.NET Core循环读取下载文件的缓存大小，这里我们设置为了1024字节，也就是说ASP.NET Core每次会从下载文件中读取1024字节的内容到服务器内存中，然后发送到客户端浏览器，这样避免了一次将整个下载文件都加载到服务器内存中，导致服务器崩溃

            Response.ContentType = "application/vnd.ms-excel";//由于我们下载的是一个Excel文件，所以设置ContentType为application/vnd.ms-excel

            var contentDisposition = "attachment;" + "filename=" + System.Web.HttpUtility.UrlEncode(fileName);//在Response的Header中设置下载文件的文件名，这样客户端浏览器才能正确显示下载的文件名，注意这里要用HttpUtility.UrlEncode编码文件名，否则有些浏览器可能会显示乱码文件名
            Response.Headers.Add("Content-Disposition", new string[] { contentDisposition });

            //使用FileStream开始循环读取要下载文件的内容
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                using (Response.Body)//调用Response.Body.Dispose()并不会关闭客户端浏览器到ASP.NET Core服务器的连接，之后还可以继续往Response.Body中写入数据
                {
                    long contentLength = fs.Length;//获取下载文件的大小
                    Response.ContentLength = contentLength;//在Response的Header中设置下载文件的大小，这样客户端浏览器才能正确显示下载的进度

                    byte[] buffer;
                    long hasRead = 0;//变量hasRead用于记录已经发送了多少字节的数据到客户端浏览器

                    //如果hasRead小于contentLength，说明下载文件还没读取完毕，继续循环读取下载文件的内容，并发送到客户端浏览器
                    while (hasRead < contentLength)
                    {
                        //HttpContext.RequestAborted.IsCancellationRequested可用于检测客户端浏览器和ASP.NET Core服务器之间的连接状态，如果HttpContext.RequestAborted.IsCancellationRequested返回true，说明客户端浏览器中断了连接
                        if (HttpContext.RequestAborted.IsCancellationRequested)
                        {
                            //如果客户端浏览器中断了到ASP.NET Core服务器的连接，这里应该立刻break，取消下载文件的读取和发送，避免服务器耗费资源
                            break;
                        }

                        buffer = new byte[bufferSize];

                        int currentRead = fs.Read(buffer, 0, bufferSize);//从下载文件中读取bufferSize(1024字节)大小的内容到服务器内存中

                        Response.Body.Write(buffer, 0, currentRead);//发送读取的内容数据到客户端浏览器
                        Response.Body.Flush();//注意每次Write后，要及时调用Flush方法，及时释放服务器内存空间

                        hasRead += currentRead;//更新已经发送到客户端浏览器的字节数
                    }
                }
            }

            return new EmptyResult();
        }

        public async Task<IActionResult> Sample_File()
        {
            if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
            {
                return BadRequest($"Expected a multipart request, but got {Request.ContentType}");
            }

            var formAccumulator = new KeyValueAccumulator();
            string targetFilePath = null;

            var boundary = MultipartRequestHelper.GetBoundary(
                MediaTypeHeaderValue.Parse(Request.ContentType),
                _defaultFormOptions.MultipartBoundaryLengthLimit);
            var reader = new MultipartReader(boundary, HttpContext.Request.Body);

            var section = await reader.ReadNextSectionAsync();
            while (section != null)
            {
                ContentDispositionHeaderValue contentDisposition;
                var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out contentDisposition);

                if (hasContentDispositionHeader)
                {
                    if (MultipartRequestHelper.HasFileContentDisposition(contentDisposition))
                    {
                        string root = Path.Combine(_serverPath, "upload", DateTime.Now.ToString("yyyyMMdd"));
                        CreateDirectory(root);

                        targetFilePath = Path.Combine(root, contentDisposition.FileName.Value);
                        using (var targetStream = System.IO.File.Create(targetFilePath))
                        {
                            await section.Body.CopyToAsync(targetStream);

                            _logger.LogInformation($"Copied the uploaded file '{targetFilePath}'");
                        }
                    }
                    else if (MultipartRequestHelper.HasFormDataContentDisposition(contentDisposition))
                    {
                        var key = HeaderUtilities.RemoveQuotes(contentDisposition.Name).Value;
                        var encoding = GetEncoding(section);
                        using (var streamReader = new StreamReader(
                            section.Body,
                            encoding,
                            detectEncodingFromByteOrderMarks: true,
                            bufferSize: 1024,
                            leaveOpen: true))
                        {
                            // The value length limit is enforced by MultipartBodyLengthLimit
                            var value = await streamReader.ReadToEndAsync();
                            if (String.Equals(value, "undefined", StringComparison.OrdinalIgnoreCase))
                            {
                                value = String.Empty;
                            }
                            formAccumulator.Append(key, value);

                            if (formAccumulator.ValueCount > _defaultFormOptions.ValueCountLimit)
                            {
                                throw new InvalidDataException($"Form key count limit {_defaultFormOptions.ValueCountLimit} exceeded.");
                            }
                        }
                    }
                }
                section = await reader.ReadNextSectionAsync();
            }

            // Bind form data to a model
            //var user = new User();

            var formValueProvider = new FormValueProvider(
                BindingSource.Form,
                new FormCollection(formAccumulator.GetResults()),
                CultureInfo.CurrentCulture);

            //var bindingSuccessful = await TryUpdateModelAsync(user, prefix: "",
            //    valueProvider: formValueProvider);
            //if (!bindingSuccessful)
            //{
            //    if (!ModelState.IsValid)
            //    {
            //        return BadRequest(ModelState);
            //    }
            //}

            return Ok(new { code = 0 });
        }

        private Encoding GetEncoding(MultipartSection section)
        {
            MediaTypeHeaderValue mediaType;
            var hasMediaTypeHeader = MediaTypeHeaderValue.TryParse(section.ContentType, out mediaType);
            if (!hasMediaTypeHeader || Encoding.UTF7.Equals(mediaType.Encoding))
            {
                return Encoding.UTF8;
            }
            return mediaType.Encoding;
        }

        /// <summary>
        /// 递归创建文件夹
        /// </summary>
        /// <param name="directoryName"></param>
        private void CreateDirectory(string directoryName)
        {
            string sParentDirectory = Path.GetDirectoryName(directoryName);
            if (!Directory.Exists(sParentDirectory))
                CreateDirectory(sParentDirectory);
            if (!Directory.Exists(directoryName))
                Directory.CreateDirectory(directoryName);
        }

        private int GetFileType(string ext)
        {
            if (ext.ToLower() == ".jpg" || ext.ToLower() == ".jpeg" ||
                ext.ToLower() == ".png" || ext.ToLower() == ".bmp")
            {
                return 1;
            }

            if (ext.ToLower() == ".mp4" || ext.ToLower() == ".avi" ||
                ext.ToLower() == ".flv" || ext.ToLower() == ".rmvb")
            {
                return 2;
            }

            return 3;
        }
    }
}
