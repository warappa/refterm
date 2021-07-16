using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Threading;

namespace Refterm
{
    public class WinApi
    {
        [DllImport("user32.dll")]
        public static extern bool SetProcessDPIAware();

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern void GetSystemInfo(ref SYSTEM_INFO Info);
    }

    public struct GpuGlyphIndex
    {
        public uint Value;
    }

    class Program
    {
        static void Main(string[] args)
        {
            WinApi.SetProcessDPIAware();

            var terminal = new Terminal();

            while (true)
            {
                var key = Console.ReadKey();

                if (key.Key == ConsoleKey.Escape)
                {
                    break;
                }


            }
        }
    }

    public class Terminal
    {
        public bool LineWrap { get; set; }
        public Process? ChildProcess { get; set; }
        public String StandardIn { get; set; }
        public String StandardOut { get; set; }
        public String StandardError { get; set; }
        public int DefaultForegroundColor { get; set; } = 0x00afafaf;
        public int DefaultBackgroundColor { get; set; } = 0x000c0c0c;
        public long PipeSize { get; set; } = 16 * 1024 * 1024;
        public CursorState RunningCursor { get; set; }
        public int TextureWidth { get; set; }
        public int TextureHeight { get; set; }
        public uint TransferWidth { get; }
        public uint TransferHeight { get; }
        public int MaxWidth { get; }
        public int MaxHeight { get; }
        public D3D11Renderer Renderer { get; }
        public GlyphGenerator GlyphGen { get; }
        internal SourceBuffer ScrollBackBuffer { get; }

        // TODO: Initialize!
        public byte[] CssShaderBytes { get; private set; }
        public byte[] PSShaderBytes { get; private set; }
        public byte[] VSShaderBytes { get; private set; }

        public Partitioner Partitioner { get; private set; }
        public int MaxLineCount { get; }
        public string[] Lines { get; }
        public string RequestedFontName { get; private set; }
        public int RequestedFontHeight { get; private set; }
        public const int MinDirectCodepoint = 32;
        public const int MaxDirectCodepoint = 126;
        public GpuGlyphIndex[] ReservedTileTable { get; set; }= new GpuGlyphIndex[MaxDirectCodepoint - MinDirectCodepoint];

        public Terminal(IntPtr window)
        {
            RunningCursor = new CursorState(this);

            TextureWidth = 2048;
            TextureHeight = 2048;
            TransferWidth = 1024;
            TransferHeight = 512;
            MaxWidth = 1024;
            MaxHeight = 1024;

            Renderer = AcquireD3D11Renderer(window, false);

            GlyphGen = AllocateGlyphGenerator(TransferWidth, TransferHeight, Renderer.GlyphTransferSurface);
            ScrollBackBuffer = AllocateSourceBuffer(PipeSize);

            Uniscribe.NativeMethods.ScriptRecordDigitSubstitution(0, out Partitioner.UniscribeDigitSubstitution);
            Uniscribe.NativeMethods.ScriptApplyDigitSubstitution(Partitioner.UniscribeDigitSubstitution, out Partitioner.UniControl, out Partitioner.UniState);

            MaxLineCount = 8192;
            Lines = new string[MaxLineCount];

            RevertToDefaultFont();
            RefreshFont();
        }

        private void RefreshFont()
        {
            int Result = 0;

            GlyphTableParams parameters = new GlyphTableParams();

            parameters.ReservedTileCount = ArrayCount(ReservedTileTable) + 1;

            for (int Try = 0; Try <= 1; ++Try)
            {
                Result = SetFont(GlyphGen, RequestedFontName, (uint)RequestedFontHeight);
                if (Result > 0)
                {
                    Params.CacheTileCountInX = SafeRatio1(Terminal->REFTERM_TEXTURE_WIDTH, Terminal->GlyphGen.FontWidth);
                    Params.EntryCount = GetExpectedTileCountForDimension(&Terminal->GlyphGen, Terminal->REFTERM_TEXTURE_WIDTH, Terminal->REFTERM_TEXTURE_HEIGHT);
                    Params.HashCount = 4096;

                    if (Params.EntryCount > Params.ReservedTileCount)
                    {
                        Params.EntryCount -= Params.ReservedTileCount;
                        break;
                    }
                }

                RevertToDefaultFont(Terminal);
            }
        }

        int SetFont(GlyphGenerator GlyphGen, string FontName, uint FontHeight)
        {
            int Result = DWriteSetFont(GlyphGen, FontName, FontHeight);
            return Result;
        }

        int DWriteSetFont(GlyphGenerator GlyphGen, string FontName, uint FontHeight)
        {
            int Result = 0;

            DWriteReleaseFont(GlyphGen);

            if (GlyphGen.DWriteFactory is not null)
            {
                var textFormat = new SharpDX.DirectWrite.TextFormat(
                    GlyphGen.DWriteFactory,
                    FontName, 
                    SharpDX.DirectWrite.FontWeight.Regular,
                    SharpDX.DirectWrite.FontStyle.Normal,
                    SharpDX.DirectWrite.FontStretch.Normal,
                    FontHeight
                    );

                GlyphGen.TextFormat = textFormat;

                if (GlyphGen.TextFormat is not null)
                {
                    GlyphGen.TextFormat.ParagraphAlignment = SharpDX.DirectWrite.ParagraphAlignment.Near;
                    GlyphGen.TextFormat.TextAlignment = SharpDX.DirectWrite.TextAlignment.Leading;

                    GlyphGen.FontWidth = 0;
                    GlyphGen.FontHeight = 0;
                    IncludeLetterBounds(GlyphGen, "M");
                    IncludeLetterBounds(GlyphGen, "g");

                    Result = 1;
                }
            }

            return Result;
        }

        void IncludeLetterBounds(GlyphGenerator GlyphGen, string Letter)
        {
            using var textLayout = new SharpDX.DirectWrite.TextLayout(
                GlyphGen.DWriteFactory, Letter, GlyphGen.TextFormat, GlyphGen.TransferWidth, GlyphGen.TransferHeight);
            
            if (textLayout is not null)
            {
                // TODO(casey): Real cell size determination would go here - probably with input from the user?
                
                var charMetrics = textLayout.Metrics; // TODO: or just "Metrics"?
                var lineMetrics = textLayout.GetLineMetrics();

                if (GlyphGen.FontHeight < (uint)(lineMetrics[0].Height + 0.5f))
                {
                    GlyphGen.FontHeight = (uint)(lineMetrics[0].Height + 0.5f);
                }

                if (GlyphGen.FontHeight < (uint)(charMetrics.Height + 0.5f))
                {
                    GlyphGen.FontHeight = (uint)(charMetrics.Height + 0.5f);
                }

                if (GlyphGen.FontWidth < (uint)(charMetrics.Width + 0.5f))
                {
                    GlyphGen.FontWidth = (uint)(charMetrics.Width + 0.5f);
                }
            }
        }

        void DWriteReleaseFont(GlyphGenerator GlyphGen)
        {
            if (GlyphGen.FontFace is not null)
            {
                GlyphGen.FontFace.Release();
                GlyphGen.FontFace = null;
            }
        }

        public uint ArrayCount(GpuGlyphIndex[] array)
        {
            return (uint)array.Length;
        }

        private void RevertToDefaultFont()
        {
            RequestedFontName = "Courier New";
            RequestedFontHeight = 25;
        }

        private SourceBuffer AllocateSourceBuffer(long dataSize)
        {
            SourceBuffer result = new SourceBuffer();

            SYSTEM_INFO info = new SYSTEM_INFO();
            WinApi.GetSystemInfo(ref info);

            dataSize = (dataSize + info.dwAllocationGranularity - 1) & ~(info.dwAllocationGranularity - 1);

            var section = MemoryMappedFile.CreateNew("mapName", dataSize, MemoryMappedFileAccess.ReadWrite, MemoryMappedFileOptions.None, HandleInheritability.None);

            var view = section.CreateViewStream();

            result.Data = view;
            result.DataSize = dataSize;

            return result;
        }

        private GlyphGenerator AllocateGlyphGenerator(uint transferWidth, uint transferHeight, Surface glyphTransferSurface)
        {
            var glyphGen = new GlyphGenerator();

            glyphGen.TransferWidth = transferWidth;
            glyphGen.TransferHeight = transferHeight;

            DWriteInit(glyphGen, glyphTransferSurface);

            return glyphGen;
        }

        private void DWriteInit(GlyphGenerator glyphGen, Surface glyphTransferSurface)
        {
            glyphGen.DWriteFactory = new SharpDX.DirectWrite.Factory(SharpDX.DirectWrite.FactoryType.Shared);
        }

        private D3D11Renderer AcquireD3D11Renderer(IntPtr window, bool enableDebugging)
        {
            var renderer = new D3D11Renderer();

            var flags = DeviceCreationFlags.BgraSupport | DeviceCreationFlags.SingleThreaded;
            if (enableDebugging)
            {
                flags |= DeviceCreationFlags.Debug;
            }

            var levels = new[] { FeatureLevel.Level_11_0 };
            var device = new SharpDX.Direct3D11.Device(DriverType.Hardware, flags, levels);
            renderer.Device = device;
            renderer.DeviceContext = device.ImmediateContext;

            if (enableDebugging)
            {
                ActivateD3D11DebugInfo(device);
            }

            var swapChain = AquireDXGISwapChain(device, window, false);
            renderer.FrameLatencyWaitableObject = swapChain.FrameLatencyWaitableObject;

            var constantBufferDesc = new BufferDescription
            {
                SizeInBytes = typeof(RendererConstBuffer).StructLayoutAttribute.Size,
                Usage = ResourceUsage.Dynamic,
                BindFlags = BindFlags.ConstantBuffer,
                CpuAccessFlags = CpuAccessFlags.Write
            };

            var constantBuffer = new RendererConstBuffer();
            renderer.ConstantBuffer = SharpDX.Direct3D11.Buffer.Create<RendererConstBuffer>(device, ref constantBuffer, constantBufferDesc);
            renderer.ComputeShader = new ComputeShader(device, CssShaderBytes);
            renderer.PixelShader = new PixelShader(device, PSShaderBytes);
            renderer.VertexShader = new VertexShader(device, VSShaderBytes);

            renderer.SetD3D11GlyphCacheDim(TextureWidth, TextureHeight);

            return renderer;
        }

        private void ActivateD3D11DebugInfo(SharpDX.Direct3D11.Device device)
        {
            using var info = device.QueryInterface<SharpDX.Direct3D11.InfoQueue>();
            info.SetBreakOnSeverity(MessageSeverity.Corruption, true);
            info.SetBreakOnSeverity(MessageSeverity.Error, true);
        }

        private SwapChain2 AquireDXGISwapChain(SharpDX.Direct3D11.Device device, IntPtr window, bool useComputeShader)
        {
            var bufferUsage = Usage.RenderTargetOutput;

            if (useComputeShader)
            {
                bufferUsage |= Usage.UnorderedAccess;
            }

            var swapChainDesc = new SwapChainDescription1()
            {
                Format = Format.R8G8B8A8_UNorm,
                SampleDescription = new SampleDescription(1, 0),
                Usage = bufferUsage,
                BufferCount = 2,
                SwapEffect = SwapEffect.FlipDiscard,
                Scaling = Scaling.None,
                AlphaMode = AlphaMode.Ignore,
                Flags = SwapChainFlags.FrameLatencyWaitAbleObject,
                //ModeDescription = new ModeDescription(Format.B8G8R8A8_UNorm),
                //OutputHandle = window,

            };
            using (var factory = new Factory2())
            {
                using (var swapChain1 = new SwapChain1(factory, device, window, ref swapChainDesc))
                {
                    var swapChain2 = swapChain1.QueryInterface<SwapChain2>();
                    factory.MakeWindowAssociation(window, WindowAssociationFlags.IgnoreAltEnter | WindowAssociationFlags.IgnoreAll);
                    return swapChain2;
                }
            }
        }
    }

    public class Partitioner
    {
        // TODO(casey): Get rid of Uniscribe so this garbage doesn't have to happen

        public Uniscribe.SCRIPT_DIGITSUBSTITUTE UniscribeDigitSubstitution;
        public Uniscribe.SCRIPT_CONTROL UniControl;
        public Uniscribe.SCRIPT_STATE UniState;
        //public Uniscribe.SCRIPT_CACHE UniCache;

        public char[] Expansion = new char[1024];
        public Uniscribe.SCRIPT_ITEM[] Items = new Uniscribe.SCRIPT_ITEM[1024];
        public Uniscribe.SCRIPT_LOGATTR[] Log = new Uniscribe.SCRIPT_LOGATTR[1024];
        public uint[] SegP = new uint[1026];
    }

    class SourceBuffer
    {
        public long DataSize;


        /// <summary>
        /// char*
        /// </summary>
        public Stream Data;

        // NOTE(casey): For circular buffer
        public uint RelativePoint;

        // NOTE(casey): For cache checking
        public uint AbsoluteFilledSize;
    }
    [StructLayout(LayoutKind.Sequential)]
    internal struct SYSTEM_INFO
    {
        internal ushort wProcessorArchitecture;
        internal ushort wReserved;
        internal uint dwPageSize;
        internal IntPtr lpMinimumApplicationAddress;
        internal IntPtr lpMaximumApplicationAddress;
        internal IntPtr dwActiveProcessorMask;
        internal uint dwNumberOfProcessors;
        internal uint dwProcessorType;
        internal uint dwAllocationGranularity;
        internal ushort wProcessorLevel;
        internal ushort wProcessorRevision;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct GlyphDim
    {
        uint TileCount;
        float XScale;
        float YScale;
    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct GlyphGenerator
    {
        public uint FontWidth;
        public uint FontHeight;
        public uint Pitch;
        public IntPtr Pixels;

        public uint TransferWidth;
        public uint TransferHeight;

        // NOTE(casey): For DWrite-based generation:
        public SharpDX.DirectWrite.Factory DWriteFactory;
        public IntPtr FontFace;
        public SharpDX.DirectWrite.TextFormat TextFormat;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct RendererConstBuffer
    {
        [MarshalAs(UnmanagedType.U4, SizeConst = 2)]
        public uint[] CellSize;
        [MarshalAs(UnmanagedType.U4, SizeConst = 2)]
        public uint[] TermSize;
        [MarshalAs(UnmanagedType.U4, SizeConst = 2)]
        public uint[] TopLeftMargin;
        [MarshalAs(UnmanagedType.U4, SizeConst = 1)]
        public uint BlinkModulate;
        [MarshalAs(UnmanagedType.U4, SizeConst = 1)]
        public uint MarginColor;
        [MarshalAs(UnmanagedType.U4, SizeConst = 1)]
        public uint StrikeMin;
        [MarshalAs(UnmanagedType.U4, SizeConst = 1)]
        public uint StrikeMax;
        [MarshalAs(UnmanagedType.U4, SizeConst = 1)]
        public uint UnderlineMin;
        [MarshalAs(UnmanagedType.U4, SizeConst = 1)]
        public uint UnderlineMax;
    }

    public class D3D11Renderer
    {
        public SharpDX.Direct3D11.Device Device { get; set; }
        public DeviceContext DeviceContext { get; set; }
        public IntPtr FrameLatencyWaitableObject { get; internal set; }
        public SharpDX.Direct3D11.Buffer ConstantBuffer { get; internal set; }
        public ComputeShader ComputeShader { get; internal set; }
        public PixelShader PixelShader { get; internal set; }
        public VertexShader VertexShader { get; internal set; }
        public Texture2D GlyphTexture { get; private set; }
        public ShaderResourceView GlyphTextureView { get; private set; }
        public Texture2D GlyphTransfer { get; private set; }
        public ShaderResourceView GlyphTransferView { get; private set; }
        public Surface GlyphTransferSurface { get; private set; }
        public SharpDX.Direct2D1.RenderTarget DWriteRenderTarget { get; private set; }
        public SharpDX.Direct2D1.SolidColorBrush DWriteFillBrush { get; private set; }

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

        internal void SetD3D11GlyphTransferDim(int width, int height)
        {
            ReleaseD3DGlyphTransfer();

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

            GlyphTransfer = new Texture2D(Device, textureDesc);
            GlyphTransferView = new ShaderResourceView(Device, GlyphTexture);
            GlyphTransferSurface = GlyphTransfer.QueryInterface<Surface>();

            D2DAcquire(GlyphTransferSurface);
        }

        private void D2DAcquire(Surface glyphTransferSurface)//, out SharpDX.Direct2D1.RenderTarget dWriteRenderTarget, ref SharpDX.Direct2D1.SolidColorBrush dWriteFillBrush)
        {
            using (var factory = new SharpDX.Direct2D1.Factory(SharpDX.Direct2D1.FactoryType.SingleThreaded, SharpDX.Direct2D1.DebugLevel.Error))
            {
                var renderTargetProperties = new SharpDX.Direct2D1.RenderTargetProperties();
                DWriteRenderTarget = new SharpDX.Direct2D1.RenderTarget(factory, glyphTransferSurface, renderTargetProperties);
                DWriteFillBrush = new SharpDX.Direct2D1.SolidColorBrush(DWriteRenderTarget, new SharpDX.Mathematics.Interop.RawColor4(1, 1, 1, 1));
            }
        }

        private void ReleaseD3DGlyphTransfer()
        {
            throw new NotImplementedException();
        }
    }

    public class CursorState
    {
        private readonly Terminal terminal;

        public Point Position { get; internal set; } = new Point();
        public GlyphProps Props { get; internal set; } = new GlyphProps();

        public CursorState(Terminal terminal)
        {
            this.terminal = terminal;

            ClearCursor();
        }

        public void ClearCursor()
        {
            Position = new Point();
            ClearProps();
        }

        internal void ClearProps()
        {
            Props.Foreground = terminal.DefaultForegroundColor;
            Props.Background = terminal.DefaultBackgroundColor;
            Props.Flags = 0;
        }
    }

    public class GlyphProps
    {
        public int Foreground { get; set; }
        public int Background { get; set; }
        public int Flags { get; set; }
    }


}
