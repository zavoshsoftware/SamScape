using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http.WebHost;
using System.Web.Routing;
using System.Web.SessionState;
//using eShop21;

namespace Helpers
{
    public class Utils
    {
        public static string CreateThumbnail(string OriginalFileFullPath)
        {
            string imageUploadPath = "/Uploads/ProductImage/";
            string filename = string.Empty;

            if (File.Exists(OriginalFileFullPath))
            {
                Image img = Bitmap.FromFile(OriginalFileFullPath);
                Bitmap bmp = new Bitmap(img);

                bmp = BitmapManipulator.ThumbnailBitmap(bmp, 68, 85);

                string thumbfilename = Path.GetFileNameWithoutExtension(OriginalFileFullPath) + "_Thumb" + Path.GetExtension(OriginalFileFullPath);

                string thumb_file_relative_path = imageUploadPath + thumbfilename;

                bmp.Save(HttpContext.Current.Server.MapPath(thumb_file_relative_path), System.Drawing.Imaging.ImageFormat.Jpeg);
                img.Dispose();
                bmp.Dispose();
                filename = thumb_file_relative_path;
            }
            return filename;
        }
    }

    public class MyHttpControllerHandler : HttpControllerHandler, IRequiresSessionState
    {
        public MyHttpControllerHandler(RouteData routeData): base(routeData)
        {
        }
    }

    public class MyHttpControllerRouteHandler : HttpControllerRouteHandler
    {
        protected override IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new MyHttpControllerHandler(requestContext.RouteData);
        }
    }
}