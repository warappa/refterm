using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DirectWrite;
using SharpDX.DXGI;
using SharpDX.WIC;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Refterm
{
    public class Terminal
    {
        static byte[] OpeningMessage = new byte[] { 0xE0, 0xA4, 0x9C, 0xE0, 0xA5, 0x8B, 0x20, 0xE0, 0xA4, 0xB8, 0xE0, 0xA5, 0x8B, 0x20, 0xE0, 0xA4, 0xB0, 0xE0, 0xA4, 0xB9, 0xE0, 0xA4, 0xBE, 0x20, 0xE0, 0xA4, 0xB9, 0xE0, 0xA5, 0x8B, 0x20, 0xE0, 0xA4, 0x89, 0xE0, 0xA4, 0xB8, 0xE0, 0xA5, 0x87, 0x20, 0xE0, 0xA4, 0xA4, 0xE0, 0xA5, 0x8B, 0x20, 0xE0, 0xA4, 0x9C, 0xE0, 0xA4, 0x97, 0xE0, 0xA4, 0xBE, 0x20, 0xE0, 0xA4, 0xB8, 0xE0, 0xA4, 0x95, 0xE0, 0xA4, 0xA4, 0xE0, 0xA5, 0x87, 0x20, 0xE0, 0xA4, 0xB9, 0xE0, 0xA5, 0x88, 0xE0, 0xA4, 0x82, 0x2C, 0x20, 0xE0, 0xA4, 0xB2, 0xE0, 0xA5, 0x87, 0xE0, 0xA4, 0x95, 0xE0, 0xA4, 0xBF, 0xE0, 0xA4, 0xA8, 0x20, 0xE0, 0xA4, 0x9C, 0xE0, 0xA5, 0x8B, 0x20, 0xE0, 0xA4, 0x86, 0xE0, 0xA4, 0x81, 0xE0, 0xA4, 0x96, 0xE0, 0xA5, 0x87, 0x20, 0xE0, 0xA4, 0xAE, 0xE0, 0xA5, 0x82, 0xE0, 0xA4, 0x81, 0xE0, 0xA4, 0xA6, 0x20, 0xE0, 0xA4, 0x95, 0xE0, 0xA4, 0xB0, 0x20, 0xE0, 0xA4, 0xB8, 0xE0, 0xA5, 0x8B, 0xE0, 0xA4, 0xA8, 0xE0, 0xA5, 0x87, 0x20, 0xE0, 0xA4, 0x95, 0xE0, 0xA4, 0xBE, 0x20, 0xE0, 0xA4, 0x85, 0xE0, 0xA4, 0xAD, 0xE0, 0xA4, 0xBF, 0xE0, 0xA4, 0xA8, 0xE0, 0xA4, 0xAF, 0x20, 0xE0, 0xA4, 0x95, 0xE0, 0xA4, 0xB0, 0x20, 0xE0, 0xA4, 0xB0, 0xE0, 0xA4, 0xB9, 0xE0, 0xA4, 0xBE, 0x20, 0xE0, 0xA4, 0xB9, 0xE0, 0xA5, 0x8B, 0x20, 0xE0, 0xA4, 0x89, 0xE0, 0xA4, 0xB8, 0xE0, 0xA5, 0x87, 0x20, 0xE0, 0xA4, 0x95, 0xE0, 0xA5, 0x88, 0xE0, 0xA4, 0xB8, 0xE0, 0xA5, 0x87, 0x20, 0xE0, 0xA4, 0x9C, 0xE0, 0xA4, 0x97, 0xE0, 0xA4, 0xBE, 0xE0, 0xA4, 0x8F, 0xE0, 0xA4, 0x82, 0xE0, 0xA4, 0x97, 0xE0, 0xA5, 0x87, 0x20, 0x7C, 0x20, (byte)'\n' };

        const int LARGEST_AVAILABLE = int.MaxValue - 1;

        private ConcurrentQueue<char[]> outputTransfer = new ConcurrentQueue<char[]>();
        public bool LineWrap { get; set; } = true;
        public Process? ChildProcess { get; set; }
        public String StandardIn { get; set; }
        public String StandardOut { get; set; }
        public String StandardError { get; set; }
        public uint DefaultForegroundColor { get; set; } = 0x00afafaf;
        public uint DefaultBackgroundColor { get; set; } = 0x000c0c0c;
        public int PipeSize { get; set; } = 16 * 1024 * 1024;

        private IntPtr threadHandle;
        private IntPtr window;

        public CursorState RunningCursor { get; set; }
        public int TextureWidth { get; set; }
        public int TextureHeight { get; set; }
        public uint TransferWidth { get; }
        public uint TransferHeight { get; }
        public uint MaxWidth { get; }
        public uint MaxHeight { get; }
        public D3D11Renderer Renderer { get; }
        public GlyphGenerator GlyphGen { get; }
        internal SourceBuffer ScrollBackBuffer { get; }

        // TODO: Initialize!
        public byte[] CssShaderBytes { get; private set; } = Shaders.ReftermCShaderBytes;
        public byte[] PSShaderBytes { get; private set; } = Shaders.ReftermPSShaderBytes;
        public byte[] VSShaderBytes { get; private set; } = Shaders.ReftermVSShaderBytes;

        public Partitioner Partitioner { get; private set; }
        public int MaxLineCount { get; }
        public Line[] Lines { get; }
        public string RequestedFontName { get; private set; }
        public int RequestedFontHeight { get; private set; }
        public const int MinDirectCodepoint = 32;
        public const int MaxDirectCodepoint = 126;
        public GpuGlyphIndex[] ReservedTileTable { get; set; } = new GpuGlyphIndex[MaxDirectCodepoint - MinDirectCodepoint];
        public bool Quit { get; private set; }
        public long ViewingLineOffset { get; private set; }
        public bool DebugHighlighting { get; private set; }
        public char LastChar { get; private set; }
        public CancellationTokenSource ChildProcessCancellationTokenSource { get; private set; }
        private DateTime lastOutput = DateTime.MinValue;

        int CommandLineCount = 0;

        public int LineCount = 0;

        public int CurrentLineIndex = 0;
        public TerminalBuffer ScreenBuffer;

        GlyphTable GlyphTable;

        private char[] CommandLine = new char[256];
        private char[] promptBuffer = new char[] { '>', ' ' };
        private char[] cursorBuffer = null;

        static char[] DefaultSeed = new int[16]
            {
                178, 201, 95, 240, 40, 41, 143, 216,
                2, 209, 178, 114, 232, 4, 176, 188
            }
            .Select(x => (char)x)
            .ToArray();

        bool shouldLayoutLines = true;

        public Terminal(IntPtr window)
        {
            this.threadHandle = (IntPtr)NativeWindows.GetCurrentThreadId();
            this.window = window;
            LineWrap = true;

            RunningCursor = new CursorState(this);
            ScreenBuffer = new TerminalBuffer(this);

            Partitioner = new Partitioner();

            TextureWidth = 4 * 2048;
            TextureHeight = 4 * 2048;
            //TransferWidth = 1024;
            //TransferHeight = 512;
            TransferWidth = (uint)TextureWidth;
            TransferHeight = (uint)TextureHeight;
            MaxWidth = 1024;
            MaxHeight = 1024;

            Renderer = AcquireD3D11Renderer(window, true);

            Renderer.SetD3D11GlyphCacheDim(TextureWidth, TextureHeight);
            Renderer.SetD3D11GlyphTransferDim(TransferWidth, TransferHeight);

            GlyphGen = AllocateGlyphGenerator(TransferWidth, TransferHeight, Renderer.GlyphTransferSurface);
            ScrollBackBuffer = AllocateSourceBuffer(PipeSize);

            Uniscribe.NativeMethods.ScriptRecordDigitSubstitution(Uniscribe.Defaults.LOCALE_USER_DEFAULT, out Partitioner.UniscribeDigitSubstitution);
            Uniscribe.NativeMethods.ScriptApplyDigitSubstitution(ref Partitioner.UniscribeDigitSubstitution, out Partitioner.UniControl, out Partitioner.UniState);

            MaxLineCount = 8192;
            Lines = new Line[MaxLineCount];

            for (var i = 0; i < MaxLineCount; i++)
            {
                Lines[i] = new Line();
            }

            RevertToDefaultFont();
            RefreshFont();

            NativeWindows.ShowWindow(window, NativeWindows.ShowWindowOption.SW_SHOWDEFAULT);

            Lines[0].StartingProps = RunningCursor.Props;
            AppendOutput($"Refterm v{Assembly.GetExecutingAssembly().GetName().Version}\n");
            AppendOutput("THIS IS \x1b[38;2;255;0;0m\x1b[5mNOT\x1b[0m A REAL \x1b[9mTERMINAL\x1b[0m.\r\n" +
                "It is a reference renderer for demonstrating how to easily build relatively efficient terminal displays.\r\n" +
                "\x1b[38;2;255;0;0m\x1b[5m\x1b[4mDO NOT\x1b[0m attempt to use this as your terminal, or you will be \x1b[2mvery\x1b[0m sad.\r\n"
                );

            AppendOutput("\n");
            AppendOutput(Encoding.UTF8.GetString(OpeningMessage));
            AppendOutput("\n");

            var BlinkMS = 500; // TODO(casey): Use this in blink determination
            int MinTermSize = 512;
            int Width = MinTermSize;
            int Height = MinTermSize;

            char LastChar = '\0';

            while (!Quit)
            {
                //if (!Terminal->NoThrottle)
                //{
                //    HANDLE Handles[8];
                //    DWORD HandleCount = 0;

                //    Handles[HandleCount++] = Terminal->FastPipeReady;
                //    if (Terminal->Legacy_ReadStdOut != INVALID_HANDLE_VALUE) Handles[HandleCount++] = Terminal->Legacy_ReadStdOut;
                //    if (Terminal->Legacy_ReadStdError != INVALID_HANDLE_VALUE) Handles[HandleCount++] = Terminal->Legacy_ReadStdError;
                //    MsgWaitForMultipleObjects(HandleCount, Handles, FALSE, BlinkMS, QS_ALLINPUT);
                //}

                ProcessMessages();

                NativeWindows.RECT Rect;
                NativeWindows.GetClientRect(window, out Rect);

                if (((Rect.Left + MinTermSize) <= Rect.Right) &&
                   ((Rect.Top + MinTermSize) <= Rect.Bottom))
                {
                    Width = Rect.Right - Rect.Left;
                    Height = Rect.Bottom - Rect.Top;

                    uint Margin = 8;
                    uint NewDimX = SafeRatio1((uint)Width - Margin, GlyphGen.FontWidth);
                    uint NewDimY = SafeRatio1((uint)Height - Margin, GlyphGen.FontHeight);
                    if (NewDimX > MaxWidth)
                    {
                        NewDimX = MaxWidth;
                    }
                    if (NewDimY > MaxHeight)
                    {
                        NewDimY = MaxHeight;
                    }

                    // TODO(casey): Maybe only allocate on size differences,
                    // etc. Make a real resize function here for people who care.
                    if ((ScreenBuffer.DimX != NewDimX) ||
                       (ScreenBuffer.DimY != NewDimY))
                    {
                        DeallocateTerminalBuffer(ScreenBuffer);
                        ScreenBuffer = AllocateTerminalBuffer(NewDimX, NewDimY);

                    }
                }

                while (!shouldLayoutLines)
                {
                    var ms = (DateTime.UtcNow - lastOutput).TotalMilliseconds;
                    if (ms > BlinkMS)
                    {
                        shouldLayoutLines = true;
                    }

                    Thread.Sleep(16);
                }

                LayoutLines();

                // TODO(casey): Split RendererDraw into two!
                // Update, and render, since we only need to update if we actually get new input.

                var Blink = DateTime.UtcNow.Millisecond > BlinkMS;
                if (Renderer.Device is null)
                {
                    Renderer = AcquireD3D11Renderer(window, false);
                    RefreshFont();
                }
                if (Renderer.Device is not null)
                {
                    RendererDraw((uint)Width, (uint)Height, ScreenBuffer, Blink ? 0xffffffff : 0xff222222);
                }
                //++FrameIndex;
                //++FrameCount;

                //LARGE_INTEGER Now;
                //QueryPerformanceCounter(&Now);

                //if (Now.QuadPart >= UpdateTitle)
                //{
                //    UpdateTitle = Now.QuadPart + Frequency.QuadPart;

                //    double FramesPerSec = (double)FrameCount * Frequency.QuadPart / (Now.QuadPart - Time.QuadPart);
                //    Time = Now;
                //    FrameCount = 0;

                //    WCHAR Title[1024];

                //    if (NoThrottle)
                //    {
                //        glyph_table_stats Stats = GetAndClearStats(GlyphTable);
                //        wsprintfW(Title, L"refterm Size=%dx%d RenderFPS=%d.%02d CacheHits/Misses=%d/%d Recycle:%d",
                //                      ScreenBuffer.DimX, ScreenBuffer.DimY, (int)FramesPerSec, (int)(FramesPerSec * 100) % 100,
                //                      (int)Stats.HitCount, (int)Stats.MissCount, (int)Stats.RecycleCount);
                //    }
                //    else
                //    {
                //        wsprintfW(Title, L"refterm");
                //    }

                //    SetWindowTextW(Window, Title);
                //}
            }

            DWriteRelease(GlyphGen);
            ReleaseD3D11Renderer(Renderer);

            // TODO(casey): How do we actually do an ensured-kill here?  Like even if we crash?  Is there some kind
            // of process parameter we can pass to CreateProcess that will ensure it is killed?  Because this won't.
            KillProcess();

            Application.Exit(new System.ComponentModel.CancelEventArgs(false));
        }

        void DWriteRelease(GlyphGenerator GlyphGen)
        {
            /* NOTE(casey): There is literally no point to this function
               whatsoever except to stop the D3D debug runtime from
               complaining about unreleased resources when the program
               exits.  EVEN THOUGH THEY WOULD BE AUTOMATICALLY RELEASED
               AT THAT TIME.  So now here I am manually releasing them,
               which wastes the user's time, for no reason at all. */

            GlyphGen.DWriteFactory?.Dispose();
            GlyphGen.DWriteFactory = null;
        }

        static void ReleaseD3D11RenderTargets(D3D11Renderer Renderer)
        {
            Renderer.RenderView?.Dispose();
            Renderer.RenderView = null;

            Renderer.RenderTarget?.Dispose();
            Renderer.RenderTarget = null;
        }

        void RendererDraw(uint Width, uint Height, TerminalBuffer Term, uint BlinkModulate)
        {
            // TODO(casey): This should be split into two routines now, since we don't actually
            // need to resubmit anything if the terminal hasn't updated.

            GlyphTable Table = GlyphTable;
            //D3D11Renderer Renderer = &Terminal->Renderer;
            //GlyphGenerator GlyphGen = &Terminal->GlyphGen;
            SourceBuffer Source = ScrollBackBuffer;

            // resize RenderView to match window size
            if (Width != Renderer.CurrentWidth || Height != Renderer.CurrentHeight)
            {
                Renderer.DeviceContext.ClearState();

                ReleaseD3D11RenderTargets(Renderer);

                Renderer.DeviceContext.Flush();

                if (Width != 0 && Height != 0)
                {
                    Renderer.SwapChain.ResizeBuffers(0, (int)Width, (int)Height, Format.Unknown, SwapChainFlags.FrameLatencyWaitAbleObject);

                    //ID3D11Texture2D* Buffer;

                    using (var Buffer = Renderer.SwapChain.GetBackBuffer<Texture2D>(0))
                    {
                        Buffer.DebugName = "BackBuffer Texture";
                        //hr = IDXGISwapChain_GetBuffer(Renderer.SwapChain, 0, &IID_ID3D11Texture2D, (void**)&Buffer);
                        //AssertHR(hr);

                        if (Renderer.UseComputeShader)
                        {
                            Renderer.RenderView = new UnorderedAccessView(Renderer.Device, Buffer);
                            //hr = ID3D11Device_CreateUnorderedAccessView(Renderer.Device, (ID3D11Resource*)Buffer, 0, &Renderer.RenderView);
                            //AssertHR(hr);
                        }
                        else
                        {
                            Renderer.RenderTarget = new RenderTargetView(Renderer.Device, Buffer);
                            Renderer.RenderTarget.DebugName = "RenderTarget";
                            //hr = ID3D11Device_CreateRenderTargetView(Renderer.Device, (ID3D11Resource*)Buffer, 0, &Renderer.RenderTarget);
                            //AssertHR(hr);

                            var ViewPort = new SharpDX.Mathematics.Interop.RawViewportF
                            {
                                X = 0,
                                Y = 0,
                                Width = (float)Width,
                                Height = (float)Height
                            };

                            Renderer.DeviceContext.Rasterizer.SetViewports(new[] { ViewPort }, 1);
                            Renderer.DeviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleStrip;
                        }
                    }
                }

                Renderer.CurrentWidth = Width;
                Renderer.CurrentHeight = Height;
            }

            //var CellCount = Renderer.CurrentWidth * Renderer.CurrentHeight;
            var CellCount = Term.DimX * Term.DimY;
            if (Renderer.MaxCellCount < CellCount)
            {
                SetD3D11MaxCellCount(Renderer, CellCount);
            }

            if (Renderer.RenderView is not null ||
                Renderer.RenderTarget is not null)
            {
                var dataBox = Renderer.DeviceContext.MapSubresource(Renderer.ConstantBuffer, 0,
                    MapMode.WriteDiscard, SharpDX.Direct3D11.MapFlags.None, out var Mapped);
                //hr = ID3D11DeviceContext_Map(Renderer.DeviceContext, (ID3D11Resource*)Renderer.ConstantBuffer, 0, D3D11_MAP_WRITE_DISCARD, 0, &Mapped);
                //AssertHR(hr);
                {
                    var ConstData = new RendererConstBuffer
                    {
                        CellSizeX = GlyphGen.FontWidth,
                        CellSizeY = GlyphGen.FontHeight,
                        TermSizeX = Term.DimX,
                        TermSizeY = Term.DimY,
                        TopMargin = 8,
                        LeftMargin = 8,
                        BlinkModulate = BlinkModulate,
                        MarginColor = 0x000c0c0c,

                        StrikeMin = GlyphGen.FontHeight / 2 - GlyphGen.FontHeight / 10,
                        StrikeMax = GlyphGen.FontHeight / 2 + GlyphGen.FontHeight / 10,
                        UnderlineMin = GlyphGen.FontHeight - GlyphGen.FontHeight / 5,
                        UnderlineMax = GlyphGen.FontHeight,
                    };
                    Mapped.Write(ConstData);
                }
                Renderer.DeviceContext.UnmapSubresource(Renderer.ConstantBuffer, 0);

                var dataBox2 = Renderer.DeviceContext.MapSubresource(Renderer.CellBuffer, 0, MapMode.WriteDiscard, SharpDX.Direct3D11.MapFlags.None, out Mapped);
                //hr = ID3D11DeviceContext_Map(Renderer.DeviceContext, (ID3D11Resource*)Renderer.CellBuffer, 0, D3D11_MAP_WRITE_DISCARD, 0, &Mapped);
                //AssertHR(hr);
                {
                    var Cells = Mapped;

                    var TopCellCount = Term.DimX * (Term.DimY - Term.FirstLineY);
                    var BottomCellCount = Term.DimX * (Term.FirstLineY);
                    //Assert((TopCellCount + BottomCellCount) == (Term.DimX * Term.DimY));

                    Cells.WriteRange(
                        Term.Cells,
                        (int)(Term.FirstLineY * Term.DimX),
                        (int)(TopCellCount));

                    Cells.WriteRange(Term.Cells, 0, (int)BottomCellCount);
                }
                Renderer.DeviceContext.UnmapSubresource(Renderer.CellBuffer, 0);

                // this should match t0/t1 order in hlsl shader
                var Resources = new[] { Renderer.CellView, Renderer.GlyphTextureView };

                if (Renderer.UseComputeShader)
                {
                    // this issues compute shader for window size, which in real terminal should match its size
                    Renderer.DeviceContext.ComputeShader.SetConstantBuffers(0, 1, Renderer.ConstantBuffer);
                    Renderer.DeviceContext.ComputeShader.SetShaderResources(0, Resources);
                    Renderer.DeviceContext.ComputeShader.SetUnorderedAccessViews(0, Renderer.RenderView);
                    //ID3D11DeviceContext_CSSetUnorderedAccessViews(Renderer.DeviceContext, 0, 1, &Renderer.RenderView, NULL);
                    Renderer.DeviceContext.ComputeShader.SetShader(Renderer.ComputeShader, null, 0);
                    //ID3D11DeviceContext_CSSetShader(Renderer.DeviceContext, Renderer.ComputeShader, 0, 0);
                    Renderer.DeviceContext.Dispatch((int)(Renderer.CurrentWidth + 7) / 8, (int)(Renderer.CurrentHeight + 7) / 8, 1);
                }
                else
                {
                    // NOTE(casey): This MUST be set every frame, because PAGE FLIPPING, I guess :/
                    Renderer.DeviceContext.OutputMerger.SetRenderTargets(Renderer.RenderTarget);
                    //ID3D11DeviceContext_OMSetRenderTargets(Renderer.DeviceContext, 1, &Renderer.RenderTarget, 0);

                    Renderer.DeviceContext.PixelShader.SetConstantBuffers(0, 1, Renderer.ConstantBuffer);
                    Renderer.DeviceContext.PixelShader.SetShaderResources(0, Resources);
                    Renderer.DeviceContext.VertexShader.SetShader(Renderer.VertexShader, null, 0);
                    Renderer.DeviceContext.PixelShader.SetShader(Renderer.PixelShader, null, 0);
                    Renderer.DeviceContext.Draw(4, 0);
                }
            }

            var Vsync = false;
            var presentResult = Renderer.SwapChain.Present(Vsync ? 1 : 0, PresentFlags.None);

            //hr = IDXGISwapChain1_Present(Renderer.SwapChain, Vsync ? 1 : 0, 0);
            if ((presentResult == SharpDX.DXGI.ResultCode.DeviceReset) ||
                (presentResult == SharpDX.DXGI.ResultCode.DeviceRemoved))
            {
                //Assert(!"Device lost!");
                ReleaseD3D11Renderer(Renderer);
            }
            else
            {
                //AssertHR(hr);
            }

            if (Renderer.RenderView is not null)
            {
                Renderer.DeviceContext1.DiscardView(Renderer.RenderView);
                //ID3D11DeviceContext1_DiscardView(Renderer.DeviceContext1, (ID3D11View*)Renderer.RenderView);
            }
        }

        static void ReleaseD3D11Renderer(D3D11Renderer Renderer)
        {
            // TODO(casey): When you want to release a D3D11 device, do you have to release all the sub-components?
            // Can you just release the main device and have all the sub-components release themselves?

            ReleaseD3DCellBuffer(Renderer);
            ReleaseD3DGlyphCache(Renderer);
            ReleaseD3DGlyphTransfer(Renderer);
            ReleaseD3D11RenderTargets(Renderer);

            Renderer.FrameLatencyWaitableObject = IntPtr.Zero;

            Renderer.ComputeShader?.Dispose();
            Renderer.PixelShader?.Dispose();
            Renderer.VertexShader?.Dispose();

            Renderer.ConstantBuffer?.Dispose();

            Renderer.RenderView?.Dispose();
            Renderer.SwapChain?.Dispose();

            Renderer.DeviceContext?.Dispose();
            Renderer.DeviceContext1?.Dispose();
            Renderer.Device?.Dispose();

            D3D11Renderer ZeroRenderer = new D3D11Renderer();
            Renderer = ZeroRenderer;
        }

        static void ReleaseD3DGlyphTransfer(D3D11Renderer Renderer)
        {
            Renderer.ReleaseD3DGlyphTransfer();
        }

        static void ReleaseD3DGlyphCache(D3D11Renderer Renderer)
        {
            if (Renderer.GlyphTexture is not null)
            {
                Renderer.GlyphTexture.Dispose();
                Renderer.GlyphTexture = null;
            }

            if (Renderer.GlyphTextureView is not null)
            {
                Renderer.GlyphTextureView.Dispose();
                Renderer.GlyphTextureView = null;
            }
        }

        static void SetD3D11MaxCellCount(D3D11Renderer Renderer, uint Count)
        {
            ReleaseD3DCellBuffer(Renderer);

            if (Renderer.Device is not null)
            {

                var CellBufferDesc = new BufferDescription
                {
                    SizeInBytes = (int)Count * Marshal.SizeOf<RendererCell>(),
                    //ByteWidth = Count * sizeof(renderer_cell),
                    Usage = ResourceUsage.Dynamic,
                    BindFlags = BindFlags.ShaderResource,
                    CpuAccessFlags = CpuAccessFlags.Write,
                    OptionFlags = ResourceOptionFlags.BufferStructured,
                    StructureByteStride = Marshal.SizeOf<RendererCell>(),
                };

                Renderer.CellBuffer = new SharpDX.Direct3D11.Buffer(Renderer.Device, CellBufferDesc);
                Renderer.CellBuffer.DebugName = "CellBuffer";
                //if (SUCCEEDED(ID3D11Device_CreateBuffer(Renderer.Device, &CellBufferDesc, 0, &Renderer.CellBuffer)))
                {
                    var CellViewDesc = new ShaderResourceViewDescription
                    {
                        Dimension = ShaderResourceViewDimension.Buffer,
                        Buffer = new ShaderResourceViewDescription.BufferResource
                        {
                            FirstElement = 0,
                            ElementCount = (int)Count
                        }
                    };

                    Renderer.CellView = new ShaderResourceView(Renderer.Device, Renderer.CellBuffer, CellViewDesc);
                    Renderer.CellView.DebugName = "CellView";
                }

                Renderer.MaxCellCount = Count;
            }
        }

        static void ReleaseD3DCellBuffer(D3D11Renderer Renderer)
        {
            if (Renderer.CellBuffer is not null)
            {
                Renderer.CellBuffer.Dispose();
                Renderer.CellBuffer = null;
            }

            if (Renderer.CellView is not null)
            {
                Renderer.CellView.Dispose();
                Renderer.CellView = null;
            }
        }

        void LayoutLines()
        {
            shouldLayoutLines = false;
            // TODO(casey): Probably want to do something better here - this over-clears, since we clear
            // the whole thing and then also each line, for no real reason other than to make line wrapping
            // simpler.
            ScreenBuffer.Clear();

            //
            // TODO(casey): This code is super bad, and there's no need for it to keep repeating itself.
            //

            // TODO(casey): How do we know how far back to go, for control chars?
            var LineCount = 2 * ScreenBuffer.DimY;
            var LineOffset = CurrentLineIndex + ViewingLineOffset - LineCount;

            var CursorJumped = false;

            CursorState Cursor = new CursorState(this);
            Cursor.ClearCursor();

            for (var LineIndexIndex = 0; LineIndexIndex < LineCount; ++LineIndexIndex)
            {
                var LineIndex = (LineOffset + LineIndexIndex) % MaxLineCount;
                if (LineIndex < 0)
                {
                    LineIndex += MaxLineCount;
                }

                var Line = Lines[LineIndex];

                var remaining = (int)(Line.OnePastLastP - Line.FirstP);
                var consumed = 0;
                while (remaining > 0)
                {
                    var Range = ReadSourceAt(ScrollBackBuffer, Line.FirstP + (ulong)consumed, remaining);
                    if (Range.Count == 0)
                    {
                        break;
                    }
                    Cursor.Props = Line.StartingProps;
                    if (ParseLineIntoGlyphs(ref Range, Cursor, Line.ContainsComplexChars))
                    {
                        CursorJumped = true;
                    }

                    var cc = Range.Count;

                    remaining = cc;
                    consumed = cc;
                }
            }

            if (CursorJumped)
            {
                Cursor.Position.X = 0;
                Cursor.Position.Y = ScreenBuffer.DimY - 4;
            }

            AdvanceRow(Cursor.Position);
            Cursor.ClearProps();

#if false
    uint32_t CLCount = Terminal->CommandLineCount;

    source_buffer_range CommandLineRange = {0};
    CommandLineRange.AbsoluteP = 0;
    CommandLineRange.Count = CLCount;
    CommandLineRange.Data = (char *)Terminal->CommandLine;
#else
#endif
            SourceBufferRange PromptRange = GetPromptBufferRange();
            ParseLineIntoGlyphs(ref PromptRange, Cursor, false);

            SourceBufferRange CommandLineRange = new SourceBufferRange();
            CommandLineRange.Count = (int)CommandLineCount;
            CommandLineRange.Data = new Memory<char>(CommandLine, 0, CommandLineCount);
            ParseLineIntoGlyphs(ref CommandLineRange, Cursor, true);

            SourceBufferRange CursorRange = GetCursorBufferRange();
            ParseLineIntoGlyphs(ref CursorRange, Cursor, true);
            AdvanceRowNoClear(Cursor.Position);

            RunningCursor.ClearCursor();

            ScreenBuffer.FirstLineY = CursorJumped ? 0 : Cursor.Position.Y;
        }

        private SourceBufferRange GetCursorBufferRange()
        {
            if (cursorBuffer is null)
            {
                var cursorCode = new char[] { '\x1b', '[', '5', 'm', (char)0xe2, (char)0x96, (char)0x88 }
                                .Select(x => (byte)x)
                                .ToArray();
                cursorBuffer = Encoding.UTF8.GetChars(cursorCode);
            }

            var cursorBufferRange = new SourceBufferRange();
            cursorBufferRange.Count = cursorBuffer.Length;
            cursorBufferRange.Data = cursorBuffer;

            return cursorBufferRange;
        }

        private SourceBufferRange GetPromptBufferRange()
        {
            var promptBufferRange = new SourceBufferRange();
            promptBufferRange.Count = promptBuffer.Length;
            promptBufferRange.Data = promptBuffer;

            return promptBufferRange;
        }

        bool ParseLineIntoGlyphs(ref SourceBufferRange Range,
                                CursorState Cursor, bool ContainsComplexChars)
        {
            var CursorJumped = false;

            while (Range.Count > 0)
            {
                var span = Range.Data.Span;

                // NOTE(casey): Eat all non-Unicode
                char Peek = PeekToken(span, 0);
                if ((Peek == '\x1b') && AtEscape(span))
                {
                    if (ParseEscape(ref Range, Cursor))
                    {
                        CursorJumped = true;
                    }
                }
                else if (Peek == '\r')
                {
                    GetToken(ref Range);
                    Cursor.Position.X = 0;
                }
                else if (Peek == '\n')
                {
                    GetToken(ref Range);
                    AdvanceRow(Cursor.Position);
                }
                else if (ContainsComplexChars)
                {
                    /* TODO(casey): Currently, if you have a long line that force-splits, it will not
                       recombine properly with Unicode.  I _DO NOT_ think this should be fixed in
                       the line parser.  Instead, the fix should be what should happen here to begin
                       with, which is that the glyph chunking should happen in a state machine,
                       NOT using buffer runs like Uniscribe does.

                       So I believe the _correct_ design here is that you have a state machine instead
                       of Uniscribe for complex grapheme clusters, and _that_ will "just work" here
                       as well as being much much faster than the current path, which is very slow
                       because of Uniscribe _and_ is limited to intermediate buffer sizes.
                    */

                    // NOTE(casey): If it's not an escape, and this line contains fancy Unicode stuff,
                    // it's something we need to pass to a shaper to find out how it
                    // has to be segmented.  Which sadly is Uniscribe at this point :(
                    // Putting something actually good in here would probably be a massive improvement.

                    // NOTE(casey): Scan for the next escape code (which Uniscribe helpfully totally fails to handle)

                    //// NOTE(casey): Pass the range between the escape codes to Uniscribe

                    var data = Range.Data.Span;
                    var len = data.Length;
                    var index = 1; // must read at least 1
                    while (index < len)
                    {
                        var c = data[index];
                        if (c == '\n' ||
                            c == '\r' ||
                            c == '\x1b')
                        {
                            break;
                        }

                        index++;
                    }

                    var subRange = new SourceBufferRange(Range, index);
                    ParseWithUniscribe(subRange, Cursor);
                    Range.Skip(subRange.Count);
                }
                else
                {
                    // NOTE(casey): It's not an escape, and we know there are only simple characters on the line.

                    char CodePoint = GetToken(ref Range);
                    ref var Cell = ref GetCell(ScreenBuffer, Cursor.Position);

                    //if (Cell is null)
                    {
                        GpuGlyphIndex GPUIndex = new GpuGlyphIndex();
                        if (IsDirectCodepoint(CodePoint))
                        {
                            GPUIndex = ReservedTileTable[CodePoint - MinDirectCodepoint];
                        }
                        else
                        {
                            //Assert(CodePoint <= 127);
                            GlyphHash RunHash = ComputeGlyphHash(2, CodePoint, DefaultSeed);
                            GlyphState State = FindGlyphEntryByHash(GlyphTable, RunHash);


                            if (State.FilledState != GlyphEntryState.Rasterized)
                            {
                                PrepareTilesForTransfer(GlyphGen, Renderer, 1, CodePoint.ToString(), GetSingleTileUnitDim());

                                TransferTile(GlyphGen, Renderer, 0, State.GPUIndex);

                                UpdateGlyphCacheEntry(GlyphTable, State.Entry, GlyphEntryState.Rasterized, State.DimX, State.DimY);
                            }

                            GPUIndex = State.GPUIndex;
                        }

                        if (GPUIndex.Value != 31 &&
                            GPUIndex.Value != 32 &&
                            GPUIndex.Value != 1 &&
                            GPUIndex.Value != 0)
                        {

                        }
                        SetCellDirect(GPUIndex, Cursor.Props, ref Cell);
                    }

                    AdvanceColumn(Cursor.Position);
                }
            }

            return CursorJumped;
        }

        static void UpdateGlyphCacheEntry(GlyphTable Table, GlyphEntry Entry, GlyphEntryState NewState, uint NewDimX, uint NewDimY)
        {
            //GlyphEntry Entry = GetEntry(Table, ID);

            Entry.FilledState = NewState;
            Entry.DimX = NewDimX;
            Entry.DimY = NewDimY;
        }

        static ref GlyphEntry GetEntry(GlyphTable Table, uint Index)
        {
            //Assert(Index < Table->EntryCount);
            return ref Table.Entries[Index];
        }

        static GlyphDim GetSingleTileUnitDim()
        {
            GlyphDim Result = new GlyphDim
            {
                TileCount = 1,
                XScale = 1.0f,
                YScale = 1.0f
            };
            return Result;
        }

        void TransferTile(GlyphGenerator GlyphGen, D3D11Renderer Renderer, uint TileIndex, GpuGlyphIndex DestIndex)
        {
            /* TODO(casey):

               Regardless of whether DirectWrite or GDI is used, rasterizing glyphs via Windows' libraries is extremely slow.

               It may appear that this code path itself is the reason for the slowness, because this does a very inefficient
               "draw-then-transfer" for every glyph group, which is the slowest possible way you could do it.  However, I
               actually _tried_ doing batching, where you make a single call to DirectWrite or GDI to rasterize
               large sets of glyphs which are then transfered all at once.

               Although this does help the performance (IIRC there was about 2x total speedup in heavy use),
               the peformance is still about two orders of magnitude away from where it should be.  So I removed the batching,
               because it complicates the code quite a bit, and does not actually produce acceptable performance.

               I believe the only solution to actual fast glyph generation is to just write something that isn't as
               bad as GDI/DirectWrite.  It's a waste of code complexity to try to get a reasonable speed out of them, unless
               someone else manages to find some magic switches I didn't find that make them work at a high speed in
               bulk.
            */

            /* TODO(casey):

               At the moment, we do not do anything to fix the problem of trying to set the font size
               so large that it cannot be rasterized into the transfer buffer.  At some point, maybe
               we should warn about that and revert the font size to something smaller?
            */

            if (Renderer.DeviceContext is not null)
            {
                GlyphCachePoint Point = UnpackGlyphCachePoint(DestIndex);
                var X = (int)(Point.X * GlyphGen.FontWidth);
                var Y = (int)(Point.Y * GlyphGen.FontHeight);

                var sourceBox = new ResourceRegion(
                    (int)(TileIndex * GlyphGen.FontWidth), 0, 0,
                    (int)((TileIndex + 1) * GlyphGen.FontWidth),
                    (int)GlyphGen.FontHeight, 1);

                try
                {
                    Renderer.DeviceContext.CopySubresourceRegion(Renderer.GlyphTransfer, 0,
                        sourceBox, Renderer.GlyphTexture, 0, X, Y, 0);
                }
                catch (Exception exc)
                {
                    Console.WriteLine($"GPU index failed: {DestIndex.Value}");
                }
                finally
                {
                    var a = NativeWindows.GetLastError();
                }
            }
        }

        static GlyphCachePoint UnpackGlyphCachePoint(GpuGlyphIndex P)
        {
            GlyphCachePoint Result = new GlyphCachePoint();

            Result.X = P.Value & 0xffff;
            Result.Y = P.Value >> 16;

            return Result;
        }

        void PrepareTilesForTransfer(GlyphGenerator GlyphGen, D3D11Renderer Renderer, int Count, ReadOnlySpan<char> String, GlyphDim Dim)
        {
            var StringLen = Count;
            //Assert(StringLen == Count);
            
            //SharpDX.Direct2D1.RenderTarget
            DWriteDrawText(GlyphGen, StringLen, String.ToString(), 0, 0, GlyphGen.TransferWidth, GlyphGen.TransferHeight,
                           Renderer.DWriteRenderTarget, Renderer.DWriteFillBrush, Dim.XScale, Dim.YScale);
        }

        void DWriteDrawText(GlyphGenerator GlyphGen, int StringLen, string String,
                               uint Left, uint Top, uint Right, uint Bottom,
                               RenderTarget RenderTarget,
                               SolidColorBrush FillBrush,
                               float XScale, float YScale)
        {
            SharpDX.Mathematics.Interop.RawRectangleF Rect = new SharpDX.Mathematics.Interop.RawRectangleF();

            Rect.Left = (float)Left;
            Rect.Top = (float)Top;
            Rect.Right = (float)Right;
            Rect.Bottom = (float)Bottom;

            RenderTarget.Transform = Matrix3x2.Scaling(XScale, YScale, new Vector2(0, 0));

            RenderTarget.BeginDraw();
            RenderTarget.Clear(new SharpDX.Mathematics.Interop.RawColor4(1, 1, 1, 0));
            RenderTarget.DrawText(String, StringLen, GlyphGen.TextFormat, Rect, FillBrush,
                DrawTextOptions.Clip | DrawTextOptions.EnableColorFont,
                MeasuringMode.Natural);
            //RenderTarget.DrawEllipse(
            //                new SharpDX.Direct2D1.Ellipse
            //                {
            //                    RadiusX = 100,
            //                    RadiusY = 100,
            //                    Point = new SharpDX.Mathematics.Interop.RawVector2(28, 28)
            //                },
            //                FillBrush
            //                );

            RenderTarget.Flush();
            RenderTarget.EndDraw();



            //if (!SUCCEEDED(Error))
            //{
            //    Assert(!"EndDraw failed");
            //}
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static GlyphHash ComputeGlyphHash(int Count, char At, char[] Seedx16)
        {
            var code = HashCode.Combine(Count, At);

            return new GlyphHash
            {
                Value = code
            };
        }

        static GlyphHash ComputeGlyphHash(int Count, ReadOnlySpan<char> At, char[] Seedx16)
        {
            var len = At.Length;
            if (len == 0)
            {
                return ComputeGlyphHash(Count, At, Seedx16);
            }

            int code = 0;
            for (var i = 0; i < len; i++)
            {
                code = HashCode.Combine(Count, code, At[i]);
            }

            return new GlyphHash
            {
                Value = code
            };
        }

        void ParseWithUniscribe(SourceBufferRange UTF8Range, CursorState Cursor)
        {
            /* TODO(casey): This code is absolutely horrible - Uniscribe is basically unusable as an API, because it doesn't support
               a clean state-machine way of feeding things to it.  So I don't even know how you would really use it in a way
               that guaranteed you didn't have buffer-too-small problems.  It's just horrible.

               Plus, it is UTF16, which means we have to do an entire conversion here before we even call it :(

               I would rather get rid of this function altogether and move to something that supports UTF8, because we never
               actually want to have to do this garbage - it's just a giant hack for no reason.  Basically everything in this
               function should be replaced by something that can turn UTF8 into chunks that need to be rasterized together.

               That's all we need here, and it could be done very efficiently with sensible code.
            */

            //example_partitioner* Partitioner = Partitioner;
            //var utf8RawBytes = (byte[])UTF8Range.Data;// .ToArray().Take(UTF8Range.Count).TakeWhile(x => x != '\0').ToArray();
            //var sourceString = new string();
            //var sourceBytes = UTF8Range.Data.ToArray().TakeWhile(x => x != '\0').ToArray();
            //var utf8Bytes = Encoding.UTF8.GetBytes(sourceString);
            //byte[] unicodeBytes = Encoding.Convert(Encoding.UTF8, Encoding.Unicode, utf8Bytes);
            //var unicodeString = Encoding.Unicode.GetString((byte*)UTF8Range.Data.Pin().Pointer, UTF8Range.Count);

            //var unicodeString = new string(UTF8Range.Data.Span.ToArray().Take(UTF8Range.Count).TakeWhile(x => x != '\0').ToArray());
            //string unicodeString = null;
            //unsafe
            //{
            //    using (var pin = UTF8Range.Data.Pin())
            //    {

            //        unicodeString = Encoding.Unicode.GetString((byte*)pin.Pointer, 
            //            //Encoding.Unicode.GetByteCount(UTF8Range.Data.Span)
            //            UTF8Range.Count);
            //    }
            //}
            var spanBytes = MemoryMarshal.Cast<char, byte>(UTF8Range.Data.Span.Slice(0, (int)UTF8Range.Count));

            //var utf8String = new string(UTF8Range.Data.Span.Slice(0, UTF8Range.Count));
            //var unicodeBytes = Encoding.Convert(Encoding.UTF8, Encoding.Unicode, spanBytes.ToArray());

            //var Count = MultiByteToWideChar(CP_UTF8, 0, UTF8Range.Data, (DWORD)UTF8Range.Count,
            //                                  Partitioner->Expansion, ArrayCount(Partitioner->Expansion));

            var unicodeString = Encoding.Unicode.GetString(spanBytes);
            //unicodeBytes.CopyTo(Partitioner.Expansion, unicodeBytes.Length);
            UTF8Range.Data.Span.Slice(0, (int)UTF8Range.Count)
                .CopyTo(Partitioner.Expansion.AsSpan());

            var Count = unicodeString.Length;

            Uniscribe.NativeMethods.ScriptItemize(unicodeString, unicodeString.Length,
                Partitioner.Items.Length,
                ref Partitioner.UniControl,
                ref Partitioner.UniState, Partitioner.Items, out var ItemCount);

            var Segment = false;

            for (int ItemIndex = 0;
                ItemIndex < ItemCount;
                ++ItemIndex)
            {
                //var Items = Partitioner.Items.AsSpan(ItemIndex);
                var Item = Partitioner.Items[ItemIndex];

                //Assert((DWORD)Item->iCharPos < Count);
                var StrCount = Count - Item.iCharPos;
                if ((ItemIndex + 1) < ItemCount)
                {
                    //Assert(Item[1].iCharPos >= Item[0].iCharPos);
                    StrCount = Partitioner.Items[ItemIndex + 1].iCharPos - Item.iCharPos;
                }

                var Str = unicodeString.AsSpan(Item.iCharPos, StrCount).ToString();

                var IsComplex = Uniscribe.NativeMethods.ScriptIsComplex(Str, StrCount, Uniscribe.Constants.SIC_COMPLEX) == Uniscribe.Constants.S_OK;
                Uniscribe.NativeMethods.ScriptBreak(Str, StrCount, ref Item.a, Partitioner.Log);

                int SegCount = 0;

                Partitioner.SegP[SegCount++] = 0;
                for (var CheckIndex = 0; CheckIndex < StrCount; ++CheckIndex)
                {
                    var Attr = Partitioner.Log[CheckIndex];
                    var ShouldBreak = Str[CheckIndex] == ' ';
                    if (IsComplex)
                    {
                        ShouldBreak |= Attr.fSoftBreak == 1;
                    }
                    else
                    {
                        ShouldBreak |= Attr.fCharStop == 1;
                    }

                    if (ShouldBreak)
                    {
                        Partitioner.SegP[SegCount++] = (uint)CheckIndex;
                    }
                }

                Partitioner.SegP[SegCount++] = (uint)StrCount;

                int dSeg = 1;
                int SegStart = 0;
                int SegStop = SegCount - 1;
                if (Item.a.fRTL > 0 ||
                    Item.a.fLayoutRTL > 0)
                {
                    dSeg = -1;
                    SegStart = SegCount - 2;
                    SegStop = -1;
                }

                for (int SegIndex = SegStart; SegIndex != SegStop; SegIndex += dSeg)
                {
                    var Start = Partitioner.SegP[SegIndex];
                    var End = Partitioner.SegP[SegIndex + 1];
                    var ThisCount = (int)(End - Start);

                    if (ThisCount > 0)
                    {
                        var Run = Str.AsSpan((int)Start, ThisCount);
                        char CodePoint = Run[0];
                        if ((ThisCount == 1) && IsDirectCodepoint(CodePoint))
                        {
                            ref var Cell = ref GetCell(ScreenBuffer, Cursor.Position);
                            //if (Cell is not null)
                            {
                                GlyphProps Props = Cursor.Props;
                                if (DebugHighlighting)
                                {
                                    Props.Background = 0x00800000;
                                }

                                SetCellDirect(ReservedTileTable[CodePoint - MinDirectCodepoint], Props, ref Cell);
                            }

                            AdvanceColumn(Cursor.Position);
                        }
                        else
                        {
                            // TODO(casey): This wastes a lookup on the tile count.
                            // It should save the entry somehow, and roll it into the first cell.

                            var Prepped = false;
                            GlyphHash RunHash = ComputeGlyphHash(2 * ThisCount, Run, DefaultSeed);
                            GlyphDim GlyphDim = GetGlyphDim(GlyphGen, GlyphTable, ThisCount, Run, RunHash);
                            for (var TileIndex = 0u;
                                TileIndex < GlyphDim.TileCount;
                                ++TileIndex)
                            {
                                ref var Cell = ref GetCell(ScreenBuffer, Cursor.Position);
                                //if (Cell is not null)
                                {
                                    GlyphHash TileHash = ComputeHashForTileIndex(RunHash, TileIndex);
                                    GlyphState State = FindGlyphEntryByHash(GlyphTable, TileHash);
                                    if (State.FilledState != GlyphEntryState.Rasterized)
                                    {
                                        if (!Prepped)
                                        {
                                            PrepareTilesForTransfer(GlyphGen, Renderer, ThisCount, Run, GlyphDim);
                                            Prepped = true;
                                        }

                                        TransferTile(GlyphGen, Renderer, TileIndex, State.GPUIndex);
                                        UpdateGlyphCacheEntry(GlyphTable, State.Entry, GlyphEntryState.Rasterized, State.DimX, State.DimY);
                                    }

                                    GlyphProps Props = Cursor.Props;
                                    if (DebugHighlighting)
                                    {
                                        Props.Background = Segment ? 0x0008080u : 0x00000080u;
                                        Segment = !Segment;
                                    }
                                    SetCellDirect(State.GPUIndex, Props, ref Cell);
                                }

                                AdvanceColumn(Cursor.Position);
                            }

                        }
                    }
                }
            }
        }

        static GlyphHash ComputeHashForTileIndex(GlyphHash Tile0Hash, uint TileIndex)
        {
            return new GlyphHash
            {
                Value = HashCode.Combine(Tile0Hash.Value, TileIndex)
            };
            //__m128i HashValue = Tile0Hash.Value;
            //if (TileIndex)
            //{
            //    HashValue = _mm_xor_si128(HashValue, _mm_set1_epi32(TileIndex));
            //    HashValue = _mm_aesdec_si128(HashValue, _mm_setzero_si128());
            //    HashValue = _mm_aesdec_si128(HashValue, _mm_setzero_si128());
            //    HashValue = _mm_aesdec_si128(HashValue, _mm_setzero_si128());
            //    HashValue = _mm_aesdec_si128(HashValue, _mm_setzero_si128());
            //}

            //glyph_hash Result = { HashValue };
            //return Result;
        }

        GlyphDim GetGlyphDim(GlyphGenerator GlyphGen, GlyphTable Table, int Count, ReadOnlySpan<char> String, GlyphHash RunHash)
        {
            /* TODO(casey): Windows can only 2^31 glyph runs - which
               seems fine, but... technically Unicode can have more than two
               billion combining characters, so I guess theoretically this
               code is broken - another "reason" to do a custom glyph rasterizer? */

            GlyphDim Result = new GlyphDim();

            var StringLen = Count;
            //Assert(StringLen == Count);

            SIZE Size = new SIZE();
            GlyphState State = FindGlyphEntryByHash(Table, RunHash);
            if (State.FilledState == GlyphEntryState.None)
            {
                if (StringLen > 0)
                {
                    Size = DWriteGetTextExtent(GlyphGen, StringLen, String);
                }

                UpdateGlyphCacheEntry(Table, State.Entry, GlyphEntryState.Sized, (uint)Size.cx, (uint)Size.cy);
            }
            else
            {
                Size.cx = State.DimX;
                Size.cy = State.DimY;
            }

            Result.TileCount = SafeRatio1((uint)(Size.cx + GlyphGen.FontWidth / 2), GlyphGen.FontWidth);

            Result.XScale = 1.0f;
            if ((uint)Size.cx > GlyphGen.FontWidth)
            {
                Result.XScale = SafeRatio1((float)(Result.TileCount * GlyphGen.FontWidth),
                                           (float)(Size.cx));
            }

            Result.YScale = 1.0f;
            if ((uint)Size.cy > GlyphGen.FontHeight)
            {
                Result.YScale = SafeRatio1((float)GlyphGen.FontHeight, (float)Size.cy);
            }

            return Result;
        }

        SIZE DWriteGetTextExtent(GlyphGenerator GlyphGen, int StringLen, ReadOnlySpan<char> String)
        {
            var Result = new SIZE();

            using var Layout = new TextLayout(GlyphGen.DWriteFactory, String.ToString(), GlyphGen.TextFormat, GlyphGen.TransferWidth, GlyphGen.TransferHeight);

            if (Layout is not null)
            {
                var Metrics = Layout.Metrics;
                //Assert(Metrics.left == 0);
                //Assert(Metrics.top == 0);
                Result.cx = (uint)(Metrics.Width + 0.5f);
                Result.cy = (uint)(Metrics.Height + 0.5f);
            }

            return Result;
        }

        static GlyphState FindGlyphEntryByHash(GlyphTable Table, GlyphHash RunHash)
        {
            if (!Table.Dictionary.TryGetValue(RunHash.Value, out var entry))
            {
                entry = new GlyphEntry();
                entry.GPUIndex = Table.PickNextFreeGpuIndex();
                Table.Dictionary[RunHash.Value] = entry;
            }

            return new GlyphState
            {
                DimX = entry.DimX,
                DimY = entry.DimY,
                FilledState = entry.FilledState,
                GPUIndex = entry.GPUIndex,
                Entry = entry
                //ID = 0
            };

            //glyph_entry* Result = 0;

            //            uint32_t* Slot = GetSlotPointer(Table, RunHash);
            //            uint32_t EntryIndex = *Slot;
            //            while (EntryIndex)
            //            {
            //                glyph_entry* Entry = GetEntry(Table, EntryIndex);
            //                if (GlyphHashesAreEqual(Entry->HashValue, RunHash))
            //                {
            //                    Result = Entry;
            //                    break;
            //                }

            //                EntryIndex = Entry->NextWithSameHash;
            //            }

            //            if (Result)
            //            {
            //                Assert(EntryIndex);

            //                // NOTE(casey): An existing entry was found, remove it from the LRU
            //                glyph_entry* Prev = GetEntry(Table, Result->PrevLRU);
            //                glyph_entry* Next = GetEntry(Table, Result->NextLRU);

            //                Prev->NextLRU = Result->NextLRU;
            //                Next->PrevLRU = Result->PrevLRU;

            //                ValidateLRU(Table, -1);

            //                ++Table->Stats.HitCount;
            //            }
            //            else
            //            {
            //                // NOTE(casey): No existing entry was found, allocate a new one and link it into the hash chain

            //                EntryIndex = PopFreeEntry(Table);
            //                Assert(EntryIndex);

            //                Result = GetEntry(Table, EntryIndex);
            //                Assert(Result->FilledState == 0);
            //                Assert(Result->NextWithSameHash == 0);
            //                Assert(Result->DimX == 0);
            //                Assert(Result->DimY == 0);

            //                Result->NextWithSameHash = *Slot;
            //                Result->HashValue = RunHash;
            //                *Slot = EntryIndex;

            //                ++Table->Stats.MissCount;
            //            }

            //            // NOTE(casey): Update the LRU doubly-linked list to ensure this entry is now "first"
            //            glyph_entry* Sentinel = GetSentinel(Table);
            //            Assert(Result != Sentinel);
            //            Result->NextLRU = Sentinel->NextLRU;
            //            Result->PrevLRU = 0;

            //            glyph_entry* NextLRU = GetEntry(Table, Sentinel->NextLRU);
            //            NextLRU->PrevLRU = EntryIndex;
            //            Sentinel->NextLRU = EntryIndex;

            //#if DEBUG_VALIDATE_LRU
            //    Result->Ordering = Sentinel->Ordering++;
            //#endif
            //            ValidateLRU(Table, 1);

            //            glyph_state State;
            //            State.ID = EntryIndex;
            //            State.DimX = Result->DimX;
            //            State.DimY = Result->DimY;
            //            State.GPUIndex = Result->GPUIndex;
            //            State.FilledState = Result->FilledState;

            //            return State;
        }

        static void SetCellDirect(GpuGlyphIndex GPUIndex, GlyphProps Props, ref RendererCell Dest)
        {
            Dest.GlyphIndex = GPUIndex.Value;
            var Foreground = Props.Foreground;
            var Background = Props.Background;
            if (Props.Flags.HasFlag(TerminalCellStyle.ReverseVideo))
            {
                Foreground = Props.Background;
                Background = Props.Foreground;
            }

            if (Props.Flags.HasFlag(TerminalCellStyle.Invisible))
            {
                Dest.GlyphIndex = 0;
            }

            Dest.Foreground = Foreground | ((uint)Props.Flags << 24);
            Dest.Background = Background;
        }

        static bool IsDirectCodepoint(char CodePoint)
        {
            var Result = CodePoint >= MinDirectCodepoint &&
                         CodePoint < MaxDirectCodepoint;
            return Result;
        }

        ref RendererCell GetCell(TerminalBuffer Buffer, Position Point)
        {
            return ref Buffer.GetCell(Point);
        }

        void AdvanceColumn(Position Point)
        {
            ++Point.X;
            if (LineWrap &&
                (Point.X >= ScreenBuffer.DimX))
            {
                AdvanceRow(Point);
            }
        }
        public void AdvanceRow(Position Point)
        {
            AdvanceRowNoClear(Point);
            ClearLine(ScreenBuffer, Point.Y);
        }

        void AdvanceRowNoClear(Position Point)
        {
            Point.X = 0;
            ++Point.Y;
            if (Point.Y >= ScreenBuffer.DimY)
            {
                Point.Y = 0;
            }
        }

        static void ClearLine(TerminalBuffer Buffer, uint Y)
        {
            Buffer.ClearLine(Y);
        }

        static SourceBufferRange ReadSourceAt(SourceBuffer Buffer, ulong AbsoluteP, int Count)
        {
            SourceBufferRange Result = new SourceBufferRange();
            if (IsInBuffer(Buffer, AbsoluteP))
            {
                int relativePosition = (int)(AbsoluteP % (ulong)Buffer.DataSize);

                Result.AbsoluteP = AbsoluteP;
                Result.Count = (int)Math.Min((ulong)Count, Buffer.AbsoluteFilledSize - AbsoluteP);

                var availableInRestBuffer = Buffer.DataSize - (uint)relativePosition;

                if (Result.Count > availableInRestBuffer)
                {
                    Result.Count = (int)availableInRestBuffer;
                    if (Result.Count == 0)
                    {
                        Buffer.RelativePoint = 0;
                        Result.Count = Math.Min(Count, Buffer.DataSize);
                    }
                }

                Result.Data = Buffer.Data.Slice(relativePosition, (int)Result.Count);
            }

            return Result;
        }

        static bool IsInBuffer(SourceBuffer Buffer, ulong AbsoluteP)
        {
            var BackwardOffset = Buffer.AbsoluteFilledSize - AbsoluteP;
            var Result = ((AbsoluteP < Buffer.AbsoluteFilledSize) &&
                          (BackwardOffset < (ulong)Buffer.DataSize));
            return Result;
        }

        public TerminalBuffer AllocateTerminalBuffer(uint DimX, uint DimY)
        {
            TerminalBuffer Result = new TerminalBuffer(this);

            //int TotalSize = sizeof(renderer_cell) * DimX * DimY;
            //Result.Cells = VirtualAlloc(0, TotalSize, MEM_RESERVE | MEM_COMMIT, PAGE_READWRITE);
            Result.Cells = new RendererCell[DimX * DimY];

            if (Result.Cells is not null) //???
            {
                Result.DimX = DimX;
                Result.DimY = DimY;
            }

            shouldLayoutLines = true;

            return Result;
        }

        static void DeallocateTerminalBuffer(TerminalBuffer Buffer)
        {
            if (Buffer is not null &&
                Buffer.Cells is not null)
            {
                //VirtualFree(Buffer.Cells, 0, MEM_RELEASE);
                Buffer.DimX = Buffer.DimY = 0;
                Buffer.Cells = null; // TODO: or new ...?
            }
        }

        void ProcessMessages()
        {
            while (NativeWindows.PeekMessage(out var Message, new HandleRef(), 0, 0, NativeWindows.PeekMessageParams.PM_REMOVE))
            {
                switch (Message.msg)
                {
                    case NativeWindows.WndMsg.WM_QUIT:
                        {
                            Quit = true;
                        }
                        break;

                    case NativeWindows.WndMsg.WM_KEYDOWN:
                        {
                            switch ((NativeWindows.VirtualKeys)Message.wParam)
                            {
                                case NativeWindows.VirtualKeys.Prior:
                                    {
                                        ViewingLineOffset -= ScreenBuffer.DimY / 2;
                                    }
                                    break;

                                case NativeWindows.VirtualKeys.Next:
                                    {
                                        ViewingLineOffset += ScreenBuffer.DimY / 2;
                                    }
                                    break;
                            }

                            if (ViewingLineOffset > 0)
                            {
                                ViewingLineOffset = 0;
                            }

                            if (ViewingLineOffset < -(int)LineCount)
                            {
                                ViewingLineOffset = -(int)LineCount;
                            }
                        }
                        break;

                    case NativeWindows.WndMsg.WM_CHAR:
                        {
                            switch ((NativeWindows.VirtualKeys)Message.wParam)
                            {
                                case NativeWindows.VirtualKeys.Back:
                                    {
                                        while ((CommandLineCount > 0) &&
                                              IsUTF8Extension(CommandLine[CommandLineCount - 1]))
                                        {
                                            --CommandLineCount;
                                        }

                                        if (CommandLineCount > 0)
                                        {
                                            --CommandLineCount;
                                        }
                                    }
                                    break;

                                case NativeWindows.VirtualKeys.Return:
                                    {
                                        ExecuteCommandLine();
                                        CommandLineCount = 0;
                                        ViewingLineOffset = 0;
                                    }
                                    break;

                                default:
                                    {
                                        char Char = (char)Message.wParam;
                                        char[] Chars = new char[2];
                                        int CharCount = 0;

                                        if (char.IsHighSurrogate(Char))
                                        {
                                            LastChar = Char;
                                        }
                                        else if (char.IsLowSurrogate(Char))
                                        {
                                            if (char.IsSurrogatePair(LastChar, Char))
                                            {
                                                Chars[0] = LastChar;
                                                Chars[1] = Char;
                                                CharCount = 2;
                                            }
                                            LastChar = '\0';
                                        }
                                        else
                                        {
                                            Chars[0] = Char;
                                            CharCount = 1;
                                        }

                                        if (CharCount > 0)
                                        {
                                            var command = Chars.AsSpan().ToString();
                                            var nextSpan = CommandLine.AsSpan(CommandLineCount);
                                            command.CopyTo(nextSpan);
                                            CommandLineCount += command.Length - 1;
                                        }
                                    }
                                    break;
                            }
                        }
                        break;
                }
            }
        }

        void ExecuteCommandLine()
        {
            // TODO(casey): All of this is complete garbage and should never ever be used.

            CommandLine[CommandLineCount] = '\0';
            int ParamStart = 0;
            while (ParamStart <= CommandLineCount)
            {
                if (CommandLine[ParamStart] == ' ') break;
                ++ParamStart;
            }

            char[] A = CommandLine;
            var B = CommandLine.AsMemory(ParamStart..);
            B.Span[0] = '\0';
            if (ParamStart < CommandLineCount)
            {
                ++ParamStart;
                B = B.Slice(1);
            }

            var command = CommandLine.AsSpan(0, ParamStart - 1).ToString();

            SourceBufferRange ParamRange = new SourceBufferRange();
            ParamRange.Data = B;
            ParamRange.Count = (int)(CommandLineCount - ParamStart);

            // TODO(casey): Collapse all these options into a little array, so there's no
            // copy-pasta.
            AppendOutput("\n");

            if (command == "status")
            {
                RunningCursor.ClearProps();
                AppendOutput($"RefTerm {Assembly.GetExecutingAssembly().GetName().Version}\n");
                AppendOutput($"Size: {ScreenBuffer.DimX} x {ScreenBuffer.DimY}\n");
                //AppendOutput("Fast pipe: %s\n", EnableFastPipe ? "ON" : "off");
                AppendOutput($"Font: {RequestedFontName} {RequestedFontHeight}\n");
                AppendOutput($"Line Wrap: {(LineWrap ? "ON" : "off")}\n");
                AppendOutput($"Debug: {(DebugHighlighting ? "ON" : "off")}\n");
                //AppendOutput("Throttling: %s\n", !NoThrottle ? "ON" : "off");
            }
            //else if (command == "fastpipe")
            //{
            //    Terminal->EnableFastPipe = !Terminal->EnableFastPipe;
            //    AppendOutput(Terminal, "Fast pipe: %s\n", Terminal->EnableFastPipe ? "ON" : "off");
            //}
            else if (command == "linewrap")
            {
                LineWrap = !LineWrap;
                AppendOutput($"LineWrap: {(LineWrap ? "ON" : "off")}\n");
            }
            else if (command == "debug")
            {
                DebugHighlighting = !DebugHighlighting;
                AppendOutput($"Debug: {(DebugHighlighting ? "ON" : "off")}\n");
            }
            //else if (command == "throttle")
            //{
            //    NoThrottle = !NoThrottle;
            //    AppendOutput("Throttling: %s\n", !NoThrottle ? "ON" : "off");
            //}
            else if (command == "font")
            {
                RequestedFontName = CommandLine.AsSpan(0..ParamStart).ToString();

                RefreshFont();
                AppendOutput($"Font: {RequestedFontName}\n");
            }
            else if (command == "fontsize")
            {
                RequestedFontHeight = (int)ParseNumber(ref ParamRange);
                RefreshFont();
                AppendOutput("Font height: %u\n", RequestedFontHeight);
            }
            else if ((command == "kill") ||
                    (command == "break"))
            {
                KillProcess();
            }
            else if ((command == "clear") ||
                    (command == "cls"))
            {
                for (var i = 0; i < Lines.Length; i++)
                {
                    var line = Lines[i];
                    line.Clear(this);
                }

                ScrollBackBuffer.Clear();
            }
            else if ((command == "exit") ||
                    (command == "quit"))
            {
                KillProcess();
                AppendOutput("Exiting...\n");
                Quit = true;
            }
            else if ((command == "echo") ||
                    (command == "print"))
            {
                AppendOutput($"{B.Span.ToString()}\n");
            }
            else if (command == "")
            {
            }
            else
            {
                var processName = $"{command}.exe";

                var param = new string(CommandLine.AsSpan(ParamStart).ToArray().TakeWhile(x => x != '\0').ToArray());
                var processCommandLine = $"{processName} {param}";

                var started = ExecuteSubProcess(processName, param);
                if (!started)
                {
                    processName = "c:\\Windows\\System32\\cmd.exe";
                    processCommandLine = $"cmd.exe /c {A.AsSpan().ToString()}.exe {B.Span.ToString()}";
                    if (!(started = ExecuteSubProcess(processName, processCommandLine)))
                    {
                        AppendOutput($"ERROR: Unable to execute {CommandLine}\n");
                    }
                }

                if (started)
                {
                    AppendOutput($"> {command} {param}{Environment.NewLine}");
                }
            }
        }

        bool ExecuteSubProcess(string ProcessName, string ProcessCommandLine)
        {
            if (ChildProcess is not null)
            {
                KillProcess();
            }

            var codePage = CultureInfo.CurrentCulture.TextInfo.OEMCodePage;
            var encoding = Encoding.GetEncoding(codePage);

            var process = new Process();
            var ProcessDir = ".\\";

            process.StartInfo = new ProcessStartInfo
            {
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                StandardOutputEncoding = encoding,
                StandardErrorEncoding = encoding,
                StandardInputEncoding = encoding,
                FileName = ProcessName,
                Arguments = ProcessCommandLine,
                WorkingDirectory = ProcessDir,
                LoadUserProfile = true
            };

            process.EnableRaisingEvents = true;
            //process.ErrorDataReceived += Process_OutputDataReceived;
            //process.OutputDataReceived += Process_OutputDataReceived;
            process.Exited += Process_Exited;

            try
            {
                if (!process.Start())
                {
                    process.ErrorDataReceived -= Process_OutputDataReceived;
                    process.OutputDataReceived -= Process_OutputDataReceived;
                    process.Exited -= Process_Exited;
                    return false;
                }

            }
            catch
            {
                return false;
            }

            //process.BeginOutputReadLine();
            //process.BeginErrorReadLine();
            ChildProcessCancellationTokenSource = new CancellationTokenSource();
            var syncObj = process.SynchronizingObject;

            Task.Run(async () =>
            {
                if (ChildProcessCancellationTokenSource is null)
                {
                    return;
                }
                var token = ChildProcessCancellationTokenSource.Token;
                var buffer = new byte[4 * 1024];
                Encoding detectedEncoding = Encoding.GetEncoding(CultureInfo.CurrentCulture.TextInfo.OEMCodePage);
                var offset = 0;

                var stream = ChildProcess.StandardOutput.BaseStream;

                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        int read = 0;
                        do
                        {
                            read = stream
                                .Read(buffer, offset, buffer.Length - offset);

                            if (read > 0)
                            {
                                if (detectedEncoding is null)
                                {
                                    detectedEncoding = TryDetectEncoding(encoding, buffer, read);
                                }

                                if (detectedEncoding is not null)
                                {
                                    var outputString = detectedEncoding.GetChars(buffer, 0, offset + read);
                                    //outputTransfer.Enqueue(outputString);

                                    if (syncObj?.InvokeRequired == true)
                                    {
                                        syncObj.Invoke((Action<char[]>)(o => AppendOutput((char[])o)), new[] { outputString });
                                    }
                                    else
                                    {
                                        AppendOutput(outputString);
                                    }

                                    offset = 0;
                                }
                            }
                        }
                        while (read > 0 &&
                            !token.IsCancellationRequested);
                    }
                    catch (Exception exc)
                    {

                    }

                    if (token.IsCancellationRequested)
                    {
                        return;
                    }

                    await Task.Delay(1);
                }
            });

            ChildProcess = process;

            return true;
        }

        private static Encoding TryDetectEncoding(Encoding encoding, byte[] buffer, int read)
        {
            Encoding detectedEncoding;

            var nullCount = 0f;
            var maxCheck = Math.Min(read, 256);
            for (var i = 1; i < maxCheck; i += 2)
            {
                var value = buffer[i];
                if (value == 0)
                {
                    nullCount++;
                }
            }

            if ((nullCount * 2) / maxCheck > 0.5 ||
                (maxCheck >= 2 && buffer[0] == 255 && buffer[1] == 254))
            {
                detectedEncoding = Encoding.Unicode;
            }
            else if (maxCheck >= 3 &&
                buffer[0] == 239 && buffer[1] == 187 && buffer[2] == 191)
            {
                detectedEncoding = Encoding.UTF8;
            }
            else
            {
                detectedEncoding = encoding;
            }

            return detectedEncoding;
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            var lastOutput = (sender as Process).StandardOutput.ReadToEnd();
            if (lastOutput.Length > 0)
            {
                outputTransfer.Append(lastOutput.ToCharArray());
            }
            CloseProcess();
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data is null)
            {
                return;
            }

            AppendOutput(e.Data + '\n');
        }

        void KillProcess()
        {
            if (ChildProcess is not null)
            {
                ChildProcess.Kill();
                CloseProcess();
            }
        }

        void CloseProcess()
        {
            NativeWindows.FreeConsole();
            ChildProcessCancellationTokenSource?.Cancel();
            ChildProcessCancellationTokenSource = null;

            if (ChildProcess is not null)
            {
                ChildProcess.ErrorDataReceived -= Process_OutputDataReceived;
                ChildProcess.OutputDataReceived -= Process_OutputDataReceived;
                ChildProcess.Exited -= Process_Exited;
            }

            ChildProcess?.Dispose();
            ChildProcess = null;
        }

        static bool IsUTF8Extension(char A)
        {
            var Result = ((A & 0xc0) == 0x80);
            return Result;
        }

        void AppendOutput(string Format, params object[] args)
        {
            // TODO(casey): This is all garbage code.  You need a checked printf here, and of
            // course there isn't one of those.  Ideally this would change over to using
            // a real concatenator here, like with a #define system, but this is just
            // a hack for now to do basic printing from the internal code.


            string str;

            if (args.Length == 0)
            {
                str = Format;
            }
            else
            {
                try
                {
                    str = string.Format(Format, args);
                }
                catch
                {
                    str = Format;
                }
            }

            AppendOutput(str.AsSpan());
        }

        void AppendOutput(ReadOnlySpan<char> data)
        {
            var remaining = (int)data.Length;
            while (remaining > 0)
            {
                var Dest = GetNextWritableRange(ScrollBackBuffer, remaining);
                //Dest.Count = Math.Min(ScrollBackBuffer.DataSize - ScrollBackBuffer.RelativePoint, remaining);

                data.Slice(0, Dest.Count)
                    .CopyTo(Dest.Data.Span);

                CommitWrite(ScrollBackBuffer, Dest.Count);
                ParseLines(Dest, RunningCursor);

                remaining -= Dest.Count;
            }

            shouldLayoutLines = true;
        }

        static byte[] OverhangMask = new byte[32]
        {
            255, 255, 255, 255,  255, 255, 255, 255,  255, 255, 255, 255,  255, 255, 255, 255,
            0, 0, 0, 0,  0, 0, 0, 0,  0, 0, 0, 0,  0, 0, 0, 0
        };
        void ParseLines(SourceBufferRange Range, CursorState Cursor)
        {
            /* TODO(casey): Currently, if the commit of line data _straddles_ a control code boundary
               this code does not properly _stop_ the processing cursor.  This can cause an edge case
               where a VT code that splits a line _doesn't_ split the line as it should.  To fix this
               the ending code just needs to check to see if the reason it couldn't parse an escape
               code in "AtEscape" was that it ran out of characters, and if so, don't advance the parser
               past that point.
            */

            Range = new SourceBufferRange(Range);

            var Carriage = Vector128.Create((byte)'\n');
            var Escape = Vector128.Create((byte)'\x1b');
            //var Complex = Vector128.Create(1, 128, 1, 128, 1, 128, 1, 128, 1, 128, 1, 128, 1, 128, 1, 128);
            //var Complex = Vector128.Create(0b1111_1111_1000_0000).AsUInt16();
            var Complex = Vector128.Create((byte)128);

            var SplitLineAtCount = 4096;
            var LastP = Range.AbsoluteP;
            while (Range.Count > 0)
            {
                //var ContainsComplex = false;
                //__m128i ContainsComplex = _mm_setzero_si128();


                int Count = Range.Count;
                if (Count > SplitLineAtCount)
                {
                    Count = SplitLineAtCount;
                }

                //var Data = Range.Data.Span;
                //int index = 0;
                //var testC = false;
                //var testE = false;
                //var testX = false;
                //while (index < Count)
                //{
                //    var c = Data[index];
                //    testC |= c == '\n';
                //    testE |= c == '\x1b';
                //    testX |= c == (char)0x80;
                //    var test = testC || testE || testX;

                //    //__m128i Batch = _mm_loadu_si128((__m128i*)Data);
                //    //__m128i TestC = _mm_cmpeq_epi8(Batch, Carriage);
                //    //__m128i TestE = _mm_cmpeq_epi8(Batch, Escape);
                //    //__m128i TestX = _mm_and_si128(Batch, Complex);
                //    //__m128i Test = _mm_or_si128(TestC, TestE);
                //    //int Check = _mm_movemask_epi8(Test);
                //    //if (Check)
                //    //{
                //    //    int Advance = _tzcnt_u32(Check);
                //    //    __m128i MaskX = _mm_loadu_si128((__m128i*)(OverhangMask + 16 - Advance));
                //    //    TestX = _mm_and_si128(MaskX, TestX);
                //    //    ContainsComplex = _mm_or_si128(ContainsComplex, TestX);
                //    //    Count -= Advance;
                //    //    Data += Advance;
                //    //    break;
                //    //}

                //    //ContainsComplex = _mm_or_si128(ContainsComplex, TestX);
                //    ContainsComplex |= test;
                //    if (ContainsComplex)
                //    {
                //        break;
                //    }
                //    index++;
                //}

                //Lines[CurrentLineIndex].ContainsComplexChars |= ContainsComplex;
                //Range = ConsumeCount(Range, index);

                var ContainsComplex = Vector128.Create((byte)0);
                //var Data = MemoryMarshal.Cast<char, byte>(Range.Data.Span);
                var Data = Range.Data.Span;
                var consumed = 0;
                while (Count >= 16)
                {

                    //var Batch = Vector128.Create(
                    //    Data[0], Data[1], Data[2], Data[3], Data[4], Data[5], Data[6], Data[7])
                    //    .AsByte();
                    var Batch = MemoryMarshal.Cast<char, Vector128<byte>>(Data)[0];

                    var testC = Sse2.CompareEqual(Batch, Carriage);
                    var testE = Sse2.CompareEqual(Batch, Escape);
                    var testX = Sse2.And(Batch, Complex);
                    var test = Sse2.Or(testC, testE);
                    var check = Sse2.MoveMask(test.AsByte());
                    if (check != 0)
                    {
                        var advance = CountTrailingZeroBytes(check);
                        //var maskX = Vector128.Create(OverhangMask[16 - advance]);
                        var maskData = OverhangMask.AsSpan(16 - advance);
                        //var maskX = Vector128.Create(
                        //    maskData[0], maskData[1], maskData[2], maskData[3], maskData[4], maskData[5], maskData[6], maskData[7],
                        //    maskData[8], maskData[9], maskData[10], maskData[11], maskData[12], maskData[13], maskData[14], maskData[15])
                        //    //.AsUInt16()
                        //    ;
                        var maskX = MemoryMarshal.Cast<byte, Vector128<byte>>(maskData)[0];

                        testX = Sse2.And(maskX, testX);
                        ContainsComplex = Sse2.Or(ContainsComplex, testX.AsByte());

                        Count -= advance;
                        Data = Data.Slice(advance);
                        consumed += advance;
                        break;
                    }

                    ContainsComplex = Sse2.Or(ContainsComplex, testX.AsByte());
                    Count -= 16;
                    Data = Data.Slice(16);
                    consumed += 16;
                }

                ConsumeCount(Range, consumed);
                //var ttt = Sse2.MoveMask(Vector128.Create((byte)128));
                //var uuu = Sse2.MoveMask(ContainsComplex);
                //var xxx = Sse2.SumAbsoluteDifferences(ContainsComplex, Vector128.Create((byte)0));

                var c = Complex.ToScalar() > 0;


                Lines[CurrentLineIndex].ContainsComplexChars |= c;
                //_mm_movemask_epi8(ContainsComplex);

                if (AtEscape(Range.Data.Span))
                {
                    var FeedAt = Range.AbsoluteP;
                    if (ParseEscape(ref Range, Cursor))
                    {
                        LineFeed(FeedAt, FeedAt, Cursor.Props);
                    }
                }
                else
                {
                    char Token = GetToken(ref Range);
                    if (Token == '\n')
                    {
                        LineFeed(Range.AbsoluteP, Range.AbsoluteP, Cursor.Props);
                    }
                    else if (Token < 0) // TODO(casey): Not sure what is a "combining char" here, really, but this is a rough test
                    {
                        Lines[CurrentLineIndex].ContainsComplexChars = true;
                    }
                }

                UpdateLineEnd(Range.AbsoluteP);
                if (GetLineLength(Lines[CurrentLineIndex]) > (ulong)SplitLineAtCount)
                {
                    LineFeed(Range.AbsoluteP, Range.AbsoluteP, Cursor.Props);
                }
            }
        }

        public int CountTrailingZeroBytes(byte[] packet)
        {
            var i = 0;
            var length = packet.Length;
            while (i < length)
            {
                if (packet[length - i - 1] != 0)
                {
                    return i;
                }

                i--;
            }
            return i;
        }



        public int CountTrailingZeroBytes(int value)
        {
            var i = 0;
            while (i < 4)
            {
                if ((value >> (3 - i) & 0xff) != 0)
                {
                    return i;
                }

                i++;
            }
            return i;
        }

        static ulong GetLineLength(Line Line)
        {
            //Assert(Line->OnePastLastP >= Line->FirstP);
            var Result = Line.OnePastLastP - Line.FirstP;
            return Result;
        }

        void LineFeed(ulong AtP, ulong NextLineStart, GlyphProps AtProps)
        {
            UpdateLineEnd(AtP);
            ++CurrentLineIndex;
            if (CurrentLineIndex >= MaxLineCount)
            {
                CurrentLineIndex = 0;
            }

            var Line = Lines[CurrentLineIndex];
            Line.FirstP = NextLineStart;
            Line.OnePastLastP = NextLineStart;
            Line.ContainsComplexChars = false;
            Line.StartingProps = AtProps;

            if (LineCount <= CurrentLineIndex)
            {
                LineCount = CurrentLineIndex + 1;
            }
        }

        void UpdateLineEnd(ulong ToP)
        {
            Lines[CurrentLineIndex].OnePastLastP = ToP;
        }

        static bool ParseEscape(ref SourceBufferRange Range, CursorState Cursor)
        {
            var MovedCursor = false;

            GetToken(ref Range);
            GetToken(ref Range);

            char Command = (char)0;
            uint ParamCount = 0;
            uint[] Params = new uint[8];

            while ((ParamCount < Params.Length) && Range.Count > 0)
            {
                char Token = PeekToken(Range.Data.Span, 0);
                if (IsDigit(Token))
                {
                    Params[ParamCount++] = ParseNumber(ref Range);
                    char Semi = GetToken(ref Range);
                    if (Semi != ';')
                    {
                        Command = Semi;
                        break;
                    }
                }
                else
                {
                    Command = GetToken(ref Range);
                }
            }

            switch (Command)
            {
                case 'H':
                    {
                        // NOTE(casey): Move cursor to X,Y position
                        Cursor.Position.X = Params[1] - 1;
                        Cursor.Position.Y = Params[0] - 1;
                        MovedCursor = true;
                    }
                    break;

                case 'm':
                    {
                        // NOTE(casey): Set graphics mode
                        if (Params[0] == 0)
                        {
                            Cursor.ClearProps();
                        }

                        if (Params[0] == 1) Cursor.Props.Flags |= TerminalCellStyle.Bold;
                        if (Params[0] == 2) Cursor.Props.Flags |= TerminalCellStyle.Dim;
                        if (Params[0] == 3) Cursor.Props.Flags |= TerminalCellStyle.Italic;
                        if (Params[0] == 4) Cursor.Props.Flags |= TerminalCellStyle.Underline;
                        if (Params[0] == 5) Cursor.Props.Flags |= TerminalCellStyle.Blinking;
                        if (Params[0] == 7) Cursor.Props.Flags |= TerminalCellStyle.ReverseVideo;
                        if (Params[0] == 8) Cursor.Props.Flags |= TerminalCellStyle.Invisible;
                        if (Params[0] == 9) Cursor.Props.Flags |= TerminalCellStyle.Strikethrough;

                        if ((Params[0] == 38) && (Params[1] == 2)) Cursor.Props.Foreground = PackRGB(Params[2], Params[3], Params[4]);
                        if ((Params[0] == 48) && (Params[1] == 2)) Cursor.Props.Background = PackRGB(Params[2], Params[3], Params[4]);
                    }
                    break;
            }

            return MovedCursor;
        }

        static uint PackRGB(uint R, uint G, uint B)
        {
            if (R > 255) R = 255;
            if (G > 255) G = 255;
            if (B > 255) B = 255;
            uint Result = ((B << 16) | (G << 8) | (R << 0));
            return Result;
        }

        static uint ParseNumber(ref SourceBufferRange Range)
        {
            uint Result = 0;
            while (IsDigit(PeekToken(Range.Data.Span, 0)))
            {
                char Token = GetToken(ref Range);
                Result = (uint)(10 * Result + (Token - '0'));
            }
            return Result;
        }

        static bool IsDigit(char Digit)
        {
            var Result = ((Digit >= '0') && (Digit <= '9'));
            return Result;
        }

        private static char NullToken = '\0';
        static ref char GetToken(ref SourceBufferRange Range)
        {
            ref char Result = ref NullToken;

            if (Range.Count > 0)
            {
                Result = ref Range.Data.Span[0];
                ConsumeCount(Range, 1);
            }

            return ref Result;
        }

        static bool AtEscape(Span<char> span)
        {
            var Result = PeekToken(span, 0) == '\x1b' &&
                         PeekToken(span, 1) == '[';
            return Result;
        }
        static char PeekToken(Span<char> span, int Ordinal)
        {
            char Result = (char)0;

            if (Ordinal < span.Length)
            {
                Result = span[Ordinal];
            }

            return Result;
        }

        static void ConsumeCount(SourceBufferRange Source, int Count)
        {
            //SourceBufferRange Result = new SourceBufferRange(Source);

            if (Count > Source.Count)
            {
                Count = Source.Count;
            }

            Source.Data = Source.Data.Slice((int)Count);
            Source.AbsoluteP += (ulong)Count;
            Source.Count -= Count;
        }

        void CommitWrite(SourceBuffer Buffer, int Size)
        {
            //Assert(Buffer->RelativePoint < Buffer->DataSize);
            //Assert(Size <= Buffer->DataSize);

            Buffer.RelativePoint += Size;
            Buffer.AbsoluteFilledSize += (ulong)Size;

            var WrappedRelative = Buffer.RelativePoint - Buffer.DataSize;
            Buffer.RelativePoint = (Buffer.RelativePoint >= Buffer.DataSize) ? (int)WrappedRelative : Buffer.RelativePoint;

            //Assert(Buffer.RelativePoint < Buffer.DataSize);
        }

        static SourceBufferRange GetNextWritableRange(SourceBuffer Buffer, int MaxCount)
        {
            //Assert(Buffer->RelativePoint < Buffer->DataSize);

            var Result = new SourceBufferRange();
            Result.AbsoluteP = Buffer.AbsoluteFilledSize;
            Result.Count = Math.Min(MaxCount, (int)(Buffer.DataSize - Buffer.RelativePoint));

            if (Result.Count <= 0)
            {
                Buffer.RelativePoint = 0;

                Buffer.Data = new Memory<char>(Buffer.InternalData);

                Result.Count = Math.Min(Result.Count, (int)Buffer.Data.Length);
            }

            Result.Data = Buffer.Data.Slice((int)Buffer.RelativePoint, (int)Result.Count);

            return Result;
        }

        private bool RefreshFont()
        {
            var Result = false;

            GlyphTableParams parameters = new GlyphTableParams();

            parameters.ReservedTileCount = ArrayCount(ReservedTileTable) + 1;

            for (int Try = 0; Try <= 1; ++Try)
            {
                Result = SetFont(GlyphGen, RequestedFontName, (uint)RequestedFontHeight);
                if (Result)
                {
                    parameters.CacheTileCountInX = SafeRatio1((uint)TextureWidth, GlyphGen.FontWidth);
                    parameters.EntryCount = GetExpectedTileCountForDimension(GlyphGen, (uint)TextureWidth, (uint)TextureHeight);
                    parameters.HashCount = 4096;

                    if (parameters.EntryCount > parameters.ReservedTileCount)
                    {
                        parameters.EntryCount -= parameters.ReservedTileCount;
                        break;
                    }
                }

                RevertToDefaultFont();
            }

            GlyphTable = PlaceGlyphTableInMemory(parameters);

            InitializeDirectGlyphTable(parameters, ReservedTileTable, true);

            GlyphDim UnitDim = GetSingleTileUnitDim();

            for (var TileIndex = 0u;
                TileIndex < ArrayCount(ReservedTileTable);
                ++TileIndex)
            {
                char Letter = (char)(MinDirectCodepoint + TileIndex);
                PrepareTilesForTransfer(GlyphGen, Renderer, 1, Letter.ToString(), UnitDim);
                TransferTile(GlyphGen, Renderer, 0, ReservedTileTable[TileIndex]);
            }

            // NOTE(casey): Clear the reserved 0 tile
            var Nothing = "\0";
            GpuGlyphIndex ZeroTile = new GpuGlyphIndex();
            PrepareTilesForTransfer(GlyphGen, Renderer, 0, Nothing, UnitDim);
            TransferTile(GlyphGen, Renderer, 0, ZeroTile);

            return Result;
        }

        static GpuGlyphIndex PackGlyphCachePoint(uint X, uint Y)
        {
            GpuGlyphIndex Result = new GpuGlyphIndex { Value = (Y << 16) | X };
            return Result;
        }

        static GlyphTable PlaceGlyphTableInMemory(GlyphTableParams Params)
        {
            //Assert(Params.HashCount >= 1);
            //Assert(Params.EntryCount >= 2);
            //Assert(IsPowerOfTwo(Params.HashCount));
            //Assert(Params.CacheTileCountInX >= 1);

            GlyphTable Result = new GlyphTable();
            Result.Params = Params;
            Result.Entries = new GlyphEntry[Params.EntryCount];
            Result.EntryCount = (uint)Result.Entries.Length;
            for (var i = 0; i < Result.EntryCount; i++)
            {
                Result.Entries[i] = new GlyphEntry();
            }

            var StartingTile = Params.ReservedTileCount;
            var X = StartingTile % Params.CacheTileCountInX;
            var Y = StartingTile / Params.CacheTileCountInX;
            for (var EntryIndex = 0u;
                    EntryIndex < Params.EntryCount;
                    ++EntryIndex)
            {
                if (X >= Params.CacheTileCountInX)
                {
                    X = 0;
                    ++Y;
                }

                GlyphEntry Entry = GetEntry(Result, EntryIndex);

                Entry.GPUIndex = PackGlyphCachePoint(X, Y);

                Entry.FilledState = GlyphEntryState.None;
                Entry.DimX = 0;
                Entry.DimY = 0;
                Entry.Used = false;
                ++X;
            }

            return Result;
        }

        void InitializeDirectGlyphTable(GlyphTableParams Params, GpuGlyphIndex[] Table, bool SkipZeroSlot)
        {
            //Assert(Params.CacheTileCountInX >= 1);

            var skipAmount = SkipZeroSlot ? 1u : 0;

            var X = skipAmount;
            var Y = 0u;
            for (var EntryIndex = 0;
                EntryIndex < (Params.ReservedTileCount - skipAmount);
                ++EntryIndex)
            {
                if (X >= Params.CacheTileCountInX)
                {
                    X = 0;
                    ++Y;
                }

                Table[EntryIndex] = PackGlyphCachePoint(X, Y);
                this.GlyphTable.Entries[EntryIndex].Used = true;

                ++X;
            }
        }
        uint GetExpectedTileCountForDimension(GlyphGenerator GlyphGen, uint Width, uint Height)
        {
            uint PerRow = SafeRatio1(Width, GlyphGen.FontWidth);
            uint PerColumn = SafeRatio1(Height, GlyphGen.FontHeight);
            uint Result = PerRow * PerColumn;

            return Result;
        }

        private uint SafeRatio1(uint a, uint b)
        {
            return b != 0 ? a / b : b;
        }
        private float SafeRatio1(float a, float b)
        {
            return b != 0 ? a / b : b;
        }

        bool SetFont(GlyphGenerator GlyphGen, string FontName, uint FontHeight)
        {
            var Result = DWriteSetFont(GlyphGen, FontName, FontHeight);
            return Result;
        }

        bool DWriteSetFont(GlyphGenerator GlyphGen, string FontName, uint FontHeight)
        {
            var Result = false;

            if (GlyphGen.DWriteFactory is not null)
            {
                var textFormat = new TextFormat(
                    GlyphGen.DWriteFactory,
                    FontName,
                    FontWeight.Regular,
                    FontStyle.Normal,
                    FontStretch.Normal,
                    FontHeight
                    );

                GlyphGen.TextFormat = textFormat;

                if (GlyphGen.TextFormat is not null)
                {
                    GlyphGen.TextFormat.ParagraphAlignment = ParagraphAlignment.Near;
                    GlyphGen.TextFormat.TextAlignment = TextAlignment.Leading;

                    GlyphGen.FontWidth = 0;
                    GlyphGen.FontHeight = 0;
                    IncludeLetterBounds(GlyphGen, "M");
                    IncludeLetterBounds(GlyphGen, "g");

                    Result = true;
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

        public uint ArrayCount(GpuGlyphIndex[] array)
        {
            return (uint)array.Length;
        }

        private void RevertToDefaultFont()
        {
            //RequestedFontName = "Courier New";
            //RequestedFontHeight = 25;
            RequestedFontName = "Cascadia Mono";
            RequestedFontHeight = 17;
        }

        private SourceBuffer AllocateSourceBuffer(int dataSize)
        {
            SourceBuffer result = new SourceBuffer();

            SYSTEM_INFO info = new SYSTEM_INFO();
            NativeWindows.GetSystemInfo(ref info);

            dataSize = (int)((dataSize + info.dwAllocationGranularity - 1) & ~(info.dwAllocationGranularity - 1));

            result.InternalData = new char[dataSize];
            result.Data = new Memory<char>(result.InternalData);
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
            //var flags = DeviceCreationFlags.BgraSupport | DeviceCreationFlags.SingleThreaded | DeviceCreationFlags.Debug;
            if (enableDebugging)
            {
                flags |= DeviceCreationFlags.Debug;
            }

            var levels = new[] { SharpDX.Direct3D.FeatureLevel.Level_11_0 };

            try
            {
                var device = new SharpDX.Direct3D11.Device(DriverType.Hardware, flags, levels);
                renderer.Device = device;
                renderer.DeviceContext = device.ImmediateContext;
            }
            catch
            {
                var device = new SharpDX.Direct3D11.Device(DriverType.Warp, flags, levels);
                renderer.Device = device;
                renderer.DeviceContext = device.ImmediateContext;
            }

            renderer.DeviceContext1 = renderer.DeviceContext.QueryInterface<SharpDX.Direct3D11.DeviceContext1>();

            if (enableDebugging)
            {
                ActivateD3D11DebugInfo(renderer.Device);
            }

            renderer.SwapChain = AquireDXGISwapChain(renderer.Device, window, false);
            renderer.FrameLatencyWaitableObject = renderer.SwapChain.FrameLatencyWaitableObject;

            var constantBufferDesc = new BufferDescription
            {
                SizeInBytes = Marshal.SizeOf<RendererConstBuffer>(),
                StructureByteStride = Marshal.SizeOf<RendererConstBuffer>(),
                Usage = ResourceUsage.Dynamic,
                BindFlags = BindFlags.ConstantBuffer,
                CpuAccessFlags = CpuAccessFlags.Write
            };

            renderer.ConstantBuffer = SharpDX.Direct3D11.Buffer.Create<RendererConstBuffer>(renderer.Device, new[] { new RendererConstBuffer() }, constantBufferDesc);
            renderer.ConstantBuffer.DebugName = "RendererConstBuffer";
            renderer.ComputeShader = new ComputeShader(renderer.Device, CssShaderBytes);
            renderer.ComputeShader.DebugName = "ComputeShader";
            renderer.PixelShader = new PixelShader(renderer.Device, PSShaderBytes);
            renderer.PixelShader.DebugName = "PixelShader";
            renderer.VertexShader = new VertexShader(renderer.Device, VSShaderBytes);
            renderer.VertexShader.DebugName = "VertexShader";

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
                Format = Format.B8G8R8A8_UNorm,
                SampleDescription = new SampleDescription(1, 0),
                Usage = bufferUsage,
                BufferCount = 2,
                SwapEffect = SwapEffect.FlipDiscard,
                Scaling = Scaling.None,
                AlphaMode = SharpDX.DXGI.AlphaMode.Ignore,
                Flags = SwapChainFlags.FrameLatencyWaitAbleObject,
            };

            using (var factory = new SharpDX.DXGI.Factory2())
            {
                using (var swapChain1 = new SwapChain1(factory, device, window, ref swapChainDesc))
                {
                    swapChain1.DebugName = "SwapChain1";
                    var swapChain2 = swapChain1.QueryInterface<SwapChain2>();
                    swapChain2.DebugName = "SwapChain2";
                    factory.MakeWindowAssociation(window, WindowAssociationFlags.IgnoreAltEnter | WindowAssociationFlags.IgnoreAll);
                    return swapChain2;
                }
            }
        }
    }
}
