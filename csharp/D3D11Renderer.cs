using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;

namespace Refterm
{
    public class D3D11Renderer
    {
        public SharpDX.Direct3D11.Device Device { get; set; }
        public SharpDX.Direct3D11.DeviceContext DeviceContext { get; set; }
        //public IntPtr FrameLatencyWaitableObject { get; internal set; }
        public IntPtr FrameLatencyWaitableObject { get; internal set; }
        public SharpDX.Direct3D11.Buffer ConstantBuffer { get; internal set; }
        public ComputeShader ComputeShader { get; internal set; }
        public PixelShader PixelShader { get; internal set; }
        public VertexShader VertexShader { get; internal set; }
        public Texture2D GlyphTexture { get; internal set; }
        public ShaderResourceView GlyphTextureView { get; internal set; }
        public Texture2D GlyphTransfer { get; set; }
        public ShaderResourceView GlyphTransferView { get; set; }
        public Surface GlyphTransferSurface { get; set; }
        public SharpDX.Direct2D1.RenderTarget DWriteRenderTarget { get; set; }
        public SharpDX.Direct2D1.SolidColorBrush DWriteFillBrush { get; set; }
        public uint CurrentWidth { get; internal set; }
        public uint CurrentHeight { get; internal set; }
        public uint MaxCellCount { get; internal set; }
        public SwapChain2 SwapChain { get; internal set; }
        public bool UseComputeShader { get; internal set; }
        public UnorderedAccessView RenderView { get; internal set; }
        public RenderTargetView RenderTarget { get; internal set; }
        public SharpDX.Direct3D11.Buffer CellBuffer { get; internal set; }
        public ShaderResourceView CellView { get; internal set; }
        public SharpDX.Direct3D11.DeviceContext1 DeviceContext1 { get; internal set; }

        internal void SetD3D11GlyphCacheDim(int width, int height)
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
                BindFlags = BindFlags.ShaderResource
            };

            GlyphTexture = new Texture2D(Device, textureDesc);
            GlyphTextureView = new ShaderResourceView(Device, GlyphTexture);
        }

        private void ReleaseD3DGlyphCache()
        {
            GlyphTexture?.Dispose();
            GlyphTextureView?.Dispose();
        }

        internal void SetD3D11GlyphTransferDim(uint width, uint height)
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
            GlyphTransferView = new ShaderResourceView(Device, GlyphTransfer);
            GlyphTransferSurface = GlyphTransfer.QueryInterface<Surface>();

            D2DAcquire(GlyphTransferSurface);
        }

        private void D2DAcquire(Surface glyphTransferSurface)//, out SharpDX.Direct2D1.RenderTarget dWriteRenderTarget, ref SharpDX.Direct2D1.SolidColorBrush dWriteFillBrush)
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
    }
}
