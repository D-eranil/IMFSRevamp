using IMFS.BusinessLogic.Log;
using IMFS.BusinessLogic.Quote;
using IMFS.Web.Models.DBModel;
using IMFS.Web.Models.Misc;
using Microsoft.AspNetCore.Http;
using Request.Body.Peeker;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace IMFS.RateCalculator.API.Helpers
{
    public class LogRequestAndResponseMiddleware
    {
        private readonly RequestDelegate _requestDelegate;
        private readonly IIMFSLogManager _imfsLogManager;
        private readonly IQuoteManager _quoteManager;

        public LogRequestAndResponseMiddleware(RequestDelegate requestDelegate, IIMFSLogManager imfsLogManager, IQuoteManager quoteManager)
        {
            _requestDelegate = requestDelegate;
            _imfsLogManager = imfsLogManager;
            _quoteManager = quoteManager;         
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                string requestBody = string.Empty;
                string responseBody = string.Empty;
                
                //Set country code
                var countryCode = context.Request.Headers["CountryCode"].ToString();
                if (string.IsNullOrEmpty(countryCode))
                {
                    ContextHelper.CountryCode = string.Empty;
                }
                else
                {
                    ContextHelper.CountryCode = countryCode.ToUpper();
                }


                requestBody = await context.Request.PeekBodyAsync();

                string path = context.Request.Path.Value.ToString();

                //Start watch
                var stopWatch = System.Diagnostics.Stopwatch.StartNew();

                var originalBodyStream = context.Response.Body;
                context.Request.EnableBuffering();
                try
                {
                    //new MemoryStream. 
                    using (var memStream = new MemoryStream())
                    {
                        // temporary response body 
                        context.Response.Body = memStream;

                        //execute the Middleware pipeline 
                        await _requestDelegate(context);

                        memStream.Position = 0;

                        responseBody = new StreamReader(memStream).ReadToEnd();//get response body here after next.Invoke()

                        //read the response stream from the beginning
                        context.Response.Body.Seek(0, SeekOrigin.Begin);
                        //Copy the contents of the new memory stream
                        await memStream.CopyToAsync(originalBodyStream);
                    }
                }
                finally
                {
                    context.Response.Body = originalBodyStream;                    
                }
                
                if (path.Contains("swagger") || path.Contains("log/"))
                {
                    return;
                }
                if (context.Response.StatusCode == 200)
                {
                    //Stop watch
                    stopWatch.Stop();                    
                }

                //Create Log object
                var apiLog = new IMFSAPILog();
                apiLog.Url = context.Request.Scheme + "://" + context.Request.Host + path + context.Request.QueryString;
                apiLog.RequestBody = requestBody;
                apiLog.ResponseStatusCode = context.Response.StatusCode.ToString();
                apiLog.ResponseBody = responseBody;
                apiLog.Duration = stopWatch.Elapsed;
                apiLog.IPAddress = context.Request.HttpContext.Connection.RemoteIpAddress.ToString();
                //apiLog.IPAddress = ContextHelper.GetCurrentIPAddress();
                _imfsLogManager.SaveAPIRequest(apiLog);
                
            }
            catch(Exception ex)
            {
                var str = ex.Message;
            }

            
        }
    }
}
