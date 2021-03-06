using System;
using System.Net;
using System.Web;
using ServiceStack.Common.Web;
using ServiceStack.ServiceHost;
using ServiceStack.Text;
using ServiceStack.WebHost.Endpoints.Extensions;

namespace ServiceStack.WebHost.Endpoints.Support
{
	public class RedirectHttpHandler
		: IServiceStackHttpHandler, IHttpHandler
	{
		public string RelativeUrl { get; set; }

		public string AbsoluteUrl { get; set; }

		/// <summary>
		/// Non ASP.NET requests
		/// </summary>
		/// <param name="request"></param>
		/// <param name="response"></param>
		/// <param name="operationName"></param>
		public void ProcessRequest(IHttpRequest request, IHttpResponse response, string operationName)
		{
			if (string.IsNullOrEmpty(RelativeUrl) && string.IsNullOrEmpty(AbsoluteUrl))
				throw new ArgumentNullException("RelativeUrl or AbsoluteUrl");

			if (!string.IsNullOrEmpty(AbsoluteUrl))
			{
				response.StatusCode = (int)HttpStatusCode.Redirect;
				response.AddHeader(HttpHeaders.Location, this.AbsoluteUrl);
			}
			else
			{
				var absoluteUrl = request.AbsoluteUri.WithTrailingSlash() + this.RelativeUrl;
				response.StatusCode = (int)HttpStatusCode.Redirect;
				response.AddHeader(HttpHeaders.Location, absoluteUrl);
			}

			ServiceStack.WebHost.Endpoints.EndpointHost.AddGlobalResponseHeaders(response);

			response.Close();
		}

        /// <summary>
        /// ASP.NET requests
        /// </summary>
        /// <param name="context"></param>
		public void ProcessRequest(HttpContext context)
		{
        	var request = context.Request;
			var response = context.Response;

			if (string.IsNullOrEmpty(RelativeUrl) && string.IsNullOrEmpty(AbsoluteUrl))
				throw new ArgumentNullException("RelativeUrl or AbsoluteUrl");

			if (!string.IsNullOrEmpty(AbsoluteUrl))
			{
				response.StatusCode = (int)HttpStatusCode.Redirect;
				response.AddHeader(HttpHeaders.Location, this.AbsoluteUrl);
			}
			else
			{
                string absoluteUrl;
			    absoluteUrl = this.RelativeUrl.Contains("~/")
			                      ? request.GetApplicationUrl().WithTrailingSlash() + this.RelativeUrl.Replace("~/", "")
			                      : request.Url.AbsoluteUri.WithTrailingSlash() + this.RelativeUrl;
			    response.StatusCode = (int)HttpStatusCode.Redirect;
                response.AddHeader(HttpHeaders.Location, absoluteUrl);
			}

        	response.CloseOutputStream();
		}

		public bool IsReusable
		{
			get { return true; }
		}
	}
}