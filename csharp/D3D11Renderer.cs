using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;

namespace Refterm
{
    public class D3D11Renderer
    {
        public SharpDX.Direct3D11.Device Device { get; set; }
        public DeviceContext DeviceContext { get; set; }
        public IntPtr FrameLatencyWaitableObject { get; set; }
        public SharpDX.Direct3D11.Buffer ConstantBuffer { get; set; }
        public ComputeShader ComputeShader { get; set; }
        public PixelShader PixelShader { get; set; }
        public VertexShader VertexShader { get; set; }
        public Texture2D GlyphTexture { get; set; }
        public ShaderResourceView GlyphTextureView { get; set; }
        public Texture2D GlyphTransfer { get; set; }
        public ShaderResourceView GlyphTransferView { get; set; }
        public Surface GlyphTransferSurface { get; set; }
        public SharpDX.Direct2D1.RenderTarget DWriteRenderTarget { get; set; }
        public SharpDX.Direct2D1.SolidColorBrush DWriteFillBrush { get; set; }
        public uint CurrentWidth { get; set; }
        public uint CurrentHeight { get; set; }
        public uint MaxCellCount { get; set; }
        public SwapChain2 SwapChain { get; set; }
        public bool UseComputeShader { get; set; }
        public UnorderedAccessView RenderView { get; set; }
        public RenderTargetView RenderTarget { get; set; }
        public SharpDX.Direct3D11.Buffer CellBuffer { get; set; }
        public ShaderResourceView CellView { get; set; }
        public DeviceContext1 DeviceContext1 { get; set; }

        public void SetD3D11GlyphCacheDim(int width, int height)
        {
            ReleaseD3DGlyphCache();

            var textureDesc = new Texture2DDescription
            {
                Width = width,
                Height = height,
                MipLevels = 1,
                ArraySize = 1,
                Format = Format.B8G8R8A8_UNorm,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.ShaderResource | BindFlags.RenderTarget
            };

            GlyphTexture = new Texture2D(Device, textureDesc);
            GlyphTexture.DebugName = "GlyphTexture";
            GlyphTextureView = new ShaderResourceView(Device, GlyphTexture);
            GlyphTextureView.DebugName = "GlyphTextureView";
            var GlyphTextureSurface = GlyphTexture.QueryInterface<Surface>();
            GlyphTextureSurface.DebugName = "GlyphTextureSurface";
        }

        public void SetD3D11GlyphTransferDim(uint width, uint height)
        {
            ReleaseD3DGlyphTransfer();

            var textureDesc = new Texture2DDescription
            {
                Width = (int)width,
                Height = (int)height,
                MipLevels = 1,
                ArraySize = 1,
                Format = Format.B8G8R8A8_UNorm,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.ShaderResource | BindFlags.RenderTarget
            };

            GlyphTransfer = new Texture2D(Device, textureDesc);
            GlyphTransfer.DebugName = "GlyphTransfer";
            GlyphTransferView = new ShaderResourceView(Device, GlyphTransfer);
            GlyphTransferView.DebugName = "GlyphTransferView";
            GlyphTransferSurface = GlyphTransfer.QueryInterface<Surface>();
            GlyphTransferSurface.DebugName = "GlyphTransferSurface";

            D2DAcquire(GlyphTransferSurface);
        }

        private void D2DAcquire(Surface glyphTransferSurface)
        {
            using (var factory = new SharpDX.Direct2D1.Factory(SharpDX.Direct2D1.FactoryType.SingleThreaded, SharpDX.Direct2D1.DebugLevel.Warning))
            {
                var renderTargetProperties = new SharpDX.Direct2D1.RenderTargetProperties
                {
                    PixelFormat = new SharpDX.Direct2D1.PixelFormat(Format.B8G8R8A8_UNorm, SharpDX.Direct2D1.AlphaMode.Premultiplied),
                    Type = SharpDX.Direct2D1.RenderTargetType.Default,
                    Usage = SharpDX.Direct2D1.RenderTargetUsage.None,
                    DpiX = 0,
                    DpiY = 0,
                };
                DWriteRenderTarget = new SharpDX.Direct2D1.RenderTarget(factory, glyphTransferSurface, renderTargetProperties);
                DWriteFillBrush = new SharpDX.Direct2D1.SolidColorBrush(DWriteRenderTarget, new SharpDX.Mathematics.Interop.RawColor4(1, 1, 1, 1));
            }
        }

        public void ReleaseD3DGlyphTransfer()
        {
            D2DRelease();

            GlyphTransfer?.Dispose();
            GlyphTransfer = null;

            GlyphTransferView?.Dispose();
            GlyphTransferView = null;

            GlyphTransferSurface?.Dispose();
            GlyphTransferSurface = null;
        }

        public void D2DRelease()
        {
            DWriteFillBrush?.Dispose();
            DWriteFillBrush = null;

            DWriteRenderTarget?.Dispose();
            DWriteRenderTarget = null;
        }

        private void ReleaseD3DGlyphCache()
        {
            GlyphTexture?.Dispose();
            GlyphTextureView?.Dispose();
        }
    }
}
