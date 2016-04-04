﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.Net.Http.Headers;
using WebFor.Core.Services.Shared;
using Microsoft.AspNet.Hosting;
using System.IO;
using Microsoft.AspNet.Mvc.Routing;

namespace WebFor.Infrastructure.Services.Shared
{
    public class CkEditorFileUploder : ICkEditorFileUploder
    {
        private IHostingEnvironment _environment;
        private IUrlHelper _urlHelper;

        public CkEditorFileUploder(IHostingEnvironment environment, IUrlHelper urlHelper)
        {
            _urlHelper = urlHelper;
            _environment = environment;
        }

        public async Task<string> UploadAsync(IFormFile upload, List<string> webRootPath, string relativePath, string CKEditorFuncNum, string CKEditor, string langCode)
        {
            string vImagePath = String.Empty;
            string vMessage = String.Empty;
            string vFilePath = String.Empty;
            string vOutput = String.Empty;
            try
            {
                if (upload != null && upload.Length > 0)
                {
                    var fileName = Path.GetFileNameWithoutExtension(ContentDispositionHeaderValue.Parse(upload.ContentDisposition).FileName.Trim('"'));
                    var extension = Path.GetExtension(ContentDispositionHeaderValue.Parse(upload.ContentDisposition).FileName.Trim('"'));

                    var vFileName = fileName + "-" + DateTime.Now.ToString("yyyyMMdd-HHMMssff") + extension;

                    webRootPath.Insert(0, _environment.WebRootPath);

                    var vFolderPath = Path.Combine(webRootPath.ToArray());

                    if (!Directory.Exists(vFolderPath))
                    {
                        Directory.CreateDirectory(vFolderPath);
                    }

                    vFilePath = Path.Combine(vFolderPath, vFileName);
                    await upload.SaveAsAsync(vFilePath);
                    vImagePath = _urlHelper.Content(relativePath + vFileName);
                    vMessage = "The file uploaded successfully.";
                }
            }

            catch (Exception e)
            {
                vMessage = "There was an issue uploading:" + e.Message;
            }

            return vOutput = @"<html><body><script>window.parent.CKEDITOR.tools.callFunction(" + CKEditorFuncNum + ", \"" + vImagePath + "\", \"" + vMessage + "\");</script></body></html>";
        }
    }
}
