using System.Windows.Media.Imaging;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using RemoteX.PC.Core;
using RemoteX.Core;
using ZXing;
using ZXing.QrCode;

namespace RemoteX.PC.DebugBackendLauncher
{
    static class Extensions
    {

        public static BitmapImage ToBitmapImage(this Bitmap bitmap)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Png); // 坑点：格式选Bmp时，不带透明度

                stream.Position = 0;
                BitmapImage result = new BitmapImage();
                result.BeginInit();
                // According to MSDN, "The default OnDemand cache option retains access to the stream until the image is needed."
                // Force the bitmap to load right now so we can dispose the stream.
                result.CacheOption = BitmapCacheOption.OnLoad;
                result.StreamSource = stream;
                result.EndInit();
                result.Freeze();
                return result;
            }
        }

        public static Bitmap GetQRCode(this IServerConnection self)
        {
            BarcodeWriter writer = new BarcodeWriter();
            writer.Format = BarcodeFormat.QR_CODE;
            QrCodeEncodingOptions options = new QrCodeEncodingOptions();
            options.DisableECI = true;
            options.CharacterSet = "UTF-8";
            options.Width = 500;
            options.Height = 500;
            options.Margin = 1;
            writer.Options = options;
            string encodedConnection = self.ConnectCode;
            Bitmap map = writer.Write(encodedConnection);
            return map;
        }
    }
}
