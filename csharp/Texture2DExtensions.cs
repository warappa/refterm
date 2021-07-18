using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.WIC;
using System.IO;

namespace Refterm
{
    public static class Texture2DExtensions
    {
        public static void Save(this Texture2D texture, Stream stream, SharpDX.Direct3D11.Device deviceDirect3D, SharpDX.Direct3D11.DeviceContext contextDirect3D,
            SharpDX.WIC.ImagingFactory2 wicFactory)
        {

            var textureCopy = new Texture2D(deviceDirect3D, new Texture2DDescription
            {
                Width = (int)texture.Description.Width,
                Height = (int)texture.Description.Height,
                MipLevels = 1,
                ArraySize = 1,
                Format = texture.Description.Format,
                Usage = ResourceUsage.Staging,
                SampleDescription = new SampleDescription(1, 0),
                BindFlags = BindFlags.None,
                CpuAccessFlags = CpuAccessFlags.Read,
                OptionFlags = ResourceOptionFlags.None
            });
            contextDirect3D.CopyResource(texture, textureCopy);

            DataStream dataStream;
            var dataBox = contextDirect3D.MapSubresource(
                textureCopy,
                0,
                0,
                MapMode.Read,
                SharpDX.Direct3D11.MapFlags.None,
                out dataStream);

            var dataRectangle = new SharpDX.DataRectangle
            {
                DataPointer = dataStream.DataPointer,
                Pitch = dataBox.RowPitch
            };

            var bitmap = new SharpDX.WIC.Bitmap(
                wicFactory,
                textureCopy.Description.Width,
                textureCopy.Description.Height,
                SharpDX.WIC.PixelFormat.Format32bppBGRA,
                dataRectangle, 0);

            using (var s = stream)
            {
                s.Position = 0;
                using (var bitmapEncoder = new PngBitmapEncoder(wicFactory, s))
                {
                    using (var bitmapFrameEncode = new BitmapFrameEncode(bitmapEncoder))
                    {
                        bitmapFrameEncode.Initialize();
                        bitmapFrameEncode.SetSize(bitmap.Size.Width, bitmap.Size.Height);
                        var pixelFormat = SharpDX.WIC.PixelFormat.FormatDontCare;
                        bitmapFrameEncode.SetPixelFormat(ref pixelFormat);
                        bitmapFrameEncode.WriteSource(bitmap);
                        bitmapFrameEncode.Commit();
                        bitmapEncoder.Commit();
                    }
                }
            }

            contextDirect3D.UnmapSubresource(textureCopy, 0);
            textureCopy.Dispose();
            bitmap.Dispose();
        }
    }
}
