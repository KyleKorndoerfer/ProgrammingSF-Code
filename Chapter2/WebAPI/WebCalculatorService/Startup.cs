using System.Web.Http;

using Owin;

namespace WebCalculatorService
{
	public static class Startup
	{
		public static void ConfigureApp( IAppBuilder appBuilder )
		{
			HttpConfiguration config = new HttpConfiguration();

			FormatterConfig.ConfigureFormatters( config.Formatters );
			RouteConfig.RegisterRoutes( config.Routes );

			appBuilder.UseWebApi( config );
		}
	}
}
