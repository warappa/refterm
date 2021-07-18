﻿using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DirectWrite;
using SharpDX.DXGI;
using SharpDX.WIC;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Text;
using System.Windows.Forms;

namespace Refterm
{
    public class Terminal
    {
        static string OpeningMessage = string.Join("", new int[] { 0xE0, 0xA4, 0x9C, 0xE0, 0xA5, 0x8B, 0x20, 0xE0, 0xA4, 0xB8, 0xE0, 0xA5, 0x8B, 0x20, 0xE0, 0xA4, 0xB0, 0xE0, 0xA4, 0xB9, 0xE0, 0xA4, 0xBE, 0x20, 0xE0, 0xA4, 0xB9, 0xE0, 0xA5, 0x8B, 0x20, 0xE0, 0xA4, 0x89, 0xE0, 0xA4, 0xB8, 0xE0, 0xA5, 0x87, 0x20, 0xE0, 0xA4, 0xA4, 0xE0, 0xA5, 0x8B, 0x20, 0xE0, 0xA4, 0x9C, 0xE0, 0xA4, 0x97, 0xE0, 0xA4, 0xBE, 0x20, 0xE0, 0xA4, 0xB8, 0xE0, 0xA4, 0x95, 0xE0, 0xA4, 0xA4, 0xE0, 0xA5, 0x87, 0x20, 0xE0, 0xA4, 0xB9, 0xE0, 0xA5, 0x88, 0xE0, 0xA4, 0x82, 0x2C, 0x20, 0xE0, 0xA4, 0xB2, 0xE0, 0xA5, 0x87, 0xE0, 0xA4, 0x95, 0xE0, 0xA4, 0xBF, 0xE0, 0xA4, 0xA8, 0x20, 0xE0, 0xA4, 0x9C, 0xE0, 0xA5, 0x8B, 0x20, 0xE0, 0xA4, 0x86, 0xE0, 0xA4, 0x81, 0xE0, 0xA4, 0x96, 0xE0, 0xA5, 0x87, 0x20, 0xE0, 0xA4, 0xAE, 0xE0, 0xA5, 0x82, 0xE0, 0xA4, 0x81, 0xE0, 0xA4, 0xA6, 0x20, 0xE0, 0xA4, 0x95, 0xE0, 0xA4, 0xB0, 0x20, 0xE0, 0xA4, 0xB8, 0xE0, 0xA5, 0x8B, 0xE0, 0xA4, 0xA8, 0xE0, 0xA5, 0x87, 0x20, 0xE0, 0xA4, 0x95, 0xE0, 0xA4, 0xBE, 0x20, 0xE0, 0xA4, 0x85, 0xE0, 0xA4, 0xAD, 0xE0, 0xA4, 0xBF, 0xE0, 0xA4, 0xA8, 0xE0, 0xA4, 0xAF, 0x20, 0xE0, 0xA4, 0x95, 0xE0, 0xA4, 0xB0, 0x20, 0xE0, 0xA4, 0xB0, 0xE0, 0xA4, 0xB9, 0xE0, 0xA4, 0xBE, 0x20, 0xE0, 0xA4, 0xB9, 0xE0, 0xA5, 0x8B, 0x20, 0xE0, 0xA4, 0x89, 0xE0, 0xA4, 0xB8, 0xE0, 0xA5, 0x87, 0x20, 0xE0, 0xA4, 0x95, 0xE0, 0xA5, 0x88, 0xE0, 0xA4, 0xB8, 0xE0, 0xA5, 0x87, 0x20, 0xE0, 0xA4, 0x9C, 0xE0, 0xA4, 0x97, 0xE0, 0xA4, 0xBE, 0xE0, 0xA4, 0x8F, 0xE0, 0xA4, 0x82, 0xE0, 0xA4, 0x97, 0xE0, 0xA5, 0x87, 0x20, 0x7C, 0x20, (int)'\n' }
            .Select(x => (char)x)
            .ToArray());

        const int LARGEST_AVAILABLE = int.MaxValue - 1;

        public bool LineWrap { get; set; } = true;
        public Process? ChildProcess { get; set; }
        public String StandardIn { get; set; }
        public String StandardOut { get; set; }
        public String StandardError { get; set; }
        public uint DefaultForegroundColor { get; set; } = 0x00afafaf;
        public uint DefaultBackgroundColor { get; set; } = 0x000c0c0c;
        public long PipeSize { get; set; } = 16 * 1024 * 1024;

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
        public byte[] CssShaderBytes { get; private set; } = Shaders.ReftermCSShaderBytes;
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

        int CommandLineCount = 0;

        public int LineCount = 0;

        public int CurrentLineIndex = 0;
        public TerminalBuffer ScreenBuffer;

        GlyphTable GlyphTable;

        char[] CommandLine = new char[256];

        static char[] DefaultSeed = new int[16]
            {
                178, 201, 95, 240, 40, 41, 143, 216,
                2, 209, 178, 114, 232, 4, 176, 188
            }
            .Select(x => (char)x)
            .ToArray();

        public Terminal(IntPtr window)
        {
            this.threadHandle = (IntPtr)NativeWindows.GetCurrentThreadId();
            this.window = window;
            LineWrap = true;

            RunningCursor = new CursorState(this);
            ScreenBuffer = new TerminalBuffer(this);

            Partitioner = new Partitioner();

            TextureWidth = 2048;
            TextureHeight = 2048;
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

            //AppendOutput("\n"); // TODO(casey): Better line startup - this is here just to initialize the running cursor.
            Lines[0].StartingProps = RunningCursor.Props;
            AppendOutput("Refterm v%u\n", Assembly.GetExecutingAssembly().GetName().Version);
            AppendOutput("THIS IS \x1b[38;2;255;0;0m\x1b[5mNOT\x1b[0m A REAL \x1b[9mTERMINAL\x1b[0m.\r\n" +
                "It is a reference renderer for demonstrating how to easily build relatively efficient terminal displays.\r\n" +
                "\x1b[38;2;255;0;0m\x1b[5m\x1b[4mDO NOT\x1b[0m attempt to use this as your terminal, or you will be \x1b[2mvery\x1b[0m sad.\r\n"
                );

            AppendOutput("\n");
            AppendOutput(OpeningMessage);
            AppendOutput("\n");

            var BlinkMS = 500; // TODO(casey): Use this in blink determination
            int MinTermSize = 512;
            int Width = MinTermSize;
            int Height = MinTermSize;

            //LARGE_INTEGER Frequency, Time;
            //QueryPerformanceFrequency(&Frequency);
            //QueryPerformanceCounter(&Time);

            //LARGE_INTEGER StartTime;
            //QueryPerformanceCounter(&StartTime);

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

                do
                {
                    //int FastIn = UpdateTerminalBuffer(FastPipe);
                    var SlowIn = UpdateTerminalBuffer(ChildProcess?.StandardOutput);
                    var ErrIn = UpdateTerminalBuffer(ChildProcess?.StandardError);

                    //if (!SlowIn && (Legacy_ReadStdOut != INVALID_HANDLE_VALUE))
                    //{
                    //    //CloseHandle(Legacy_ReadStdOut); // TODO(casey): Not sure if this is supposed to be called?
                    //    //Legacy_ReadStdOut = INVALID_HANDLE_VALUE;
                    //}

                    //if (!ErrIn && (Legacy_ReadStdError != INVALID_HANDLE_VALUE))
                    //{
                    //    //CloseHandle(Legacy_ReadStdError); // TODO(casey): Not sure if this is supposed to be called?
                    //    //Legacy_ReadStdError = INVALID_HANDLE_VALUE;
                    //}
                }
                while ((Renderer.FrameLatencyWaitableObject != IntPtr.Zero) &&
                          (NativeWindows.WaitForSingleObject(Renderer.FrameLatencyWaitableObject, 0) == NativeWindows.WAIT_TIMEOUT));

                //ResetEvent(FastPipeReady);
                //ReadFile(FastPipe, 0, 0, 0, &FastPipeTrigger);

                LayoutLines();

                // TODO(casey): Split RendererDraw into two!
                // Update, and render, since we only need to update if we actually get new input.

                //long BlinkTimer;
                //QueryPerformanceCounter(&BlinkTimer);
                //int Blink = ((1000 * (BlinkTimer.QuadPart - StartTime.QuadPart) / (BlinkMS * Frequency.QuadPart)) & 1);
                if (Renderer.Device is null)
                {
                    Renderer = AcquireD3D11Renderer(window, false);
                    RefreshFont();
                }
                if (Renderer.Device is not null)
                {
                    RendererDraw((uint)Width, (uint)Height, ScreenBuffer, /*Blink ? 0xffffffff :*/ 0xff222222);
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

            DWriteReleaseFont(GlyphGen);
            GlyphGen.DWriteFactory?.Dispose();
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

            var CellCount = Renderer.CurrentWidth * Renderer.CurrentHeight;
            if (Renderer.MaxCellCount < CellCount)
            {
                SetD3D11MaxCellCount(Renderer, CellCount);
            }

            if (Renderer.RenderView is not null ||
                Renderer.RenderTarget is not null)
            {
                var dataBox = Renderer.DeviceContext.MapSubresource(Renderer.ConstantBuffer, 0, MapMode.WriteDiscard, SharpDX.Direct3D11.MapFlags.None, out var Mapped);
                //hr = ID3D11DeviceContext_Map(Renderer.DeviceContext, (ID3D11Resource*)Renderer.ConstantBuffer, 0, D3D11_MAP_WRITE_DISCARD, 0, &Mapped);
                //AssertHR(hr);
                {
                    var ConstData = new RendererConstBuffer
                    {
                        CellSize = new uint[] { GlyphGen.FontWidth, GlyphGen.FontHeight },
                        TermSize = new uint[] { Term.DimX, Term.DimY },
                        TopLeftMargin = new uint[] { 0, 0 },
                        BlinkModulate = BlinkModulate,
                        MarginColor = 0x99ff00ff,// 0x000c0c0c,

                        StrikeMin = GlyphGen.FontHeight / 2 - GlyphGen.FontHeight / 10,
                        StrikeMax = GlyphGen.FontHeight / 2 + GlyphGen.FontHeight / 10,
                        UnderlineMin = GlyphGen.FontHeight - GlyphGen.FontHeight / 5,
                        UnderlineMax = GlyphGen.FontHeight,
                    };
                    Mapped.Write(ConstData);

                    //memcpy(Mapped.pData, &ConstData, sizeof(ConstData));
                }
                Renderer.DeviceContext.UnmapSubresource(Renderer.ConstantBuffer, 0);

                var dataBox2 = Renderer.DeviceContext.MapSubresource(Renderer.CellBuffer, 0, SharpDX.Direct3D11.MapMode.WriteDiscard, SharpDX.Direct3D11.MapFlags.None, out Mapped);
                //hr = ID3D11DeviceContext_Map(Renderer.DeviceContext, (ID3D11Resource*)Renderer.CellBuffer, 0, D3D11_MAP_WRITE_DISCARD, 0, &Mapped);
                //AssertHR(hr);
                {
                    var Cells = Mapped;

                    var TopCellCount = Term.DimX * (Term.DimY - Term.FirstLineY);
                    var BottomCellCount = Term.DimX * (Term.FirstLineY);
                    //Assert((TopCellCount + BottomCellCount) == (Term.DimX * Term.DimY));

                    var termCellsSpan = Term.Cells.AsSpan();
                    var termCellsFirstCopy = Term.Cells.AsSpan(
                        (int)(Term.FirstLineY * Term.DimX),
                        (int)(TopCellCount));

                    unsafe
                    {
                        var cellSpan =
                            new Span<RendererCell>(
                            Cells.DataPointer.ToPointer(),
                            (int)Cells.Length / Marshal.SizeOf<RendererCell>());

                        var targetCellSpan = cellSpan.Slice( (int)TopCellCount);

                        termCellsFirstCopy.CopyTo(cellSpan);

                        termCellsSpan.Slice(0, (int)BottomCellCount).CopyTo(targetCellSpan);
                    }
                    //using (var pin = Term.Cells.AsMemory().Pin()) {
                    //    System.Buffer.MemoryCopy(pin.Pointer, 

                    //    Cells.WriteRange(.Pointer, (int)(TopCellCount * typeof(RendererCell).StructLayoutAttribute.Size)));
                    //    termSpan.CopyTo()

                    //memcpy(Cells, Term.Cells + Term.FirstLineY * Term.DimX, TopCellCount * sizeof(renderer_cell));
                    //    memcpy(Cells + TopCellCount, Term.Cells, BotCellCount * sizeof(renderer_cell));
                    //}
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


            if (false)
            {

                //var dev = new DeviceManager();
                //dev.Initialize(96);
                //using var a = Renderer.SwapChain.GetBackBuffer<Texture2D>(0);
                //a.Save(File.OpenWrite(@"c:\Temp\buffer.jpg"), Renderer.Device, Renderer.DeviceContext1, new ImagingFactory2());

                Renderer.GlyphTexture.Save(File.OpenWrite(@"c:\Temp\buffer.jpg"), Renderer.Device, Renderer.DeviceContext, new ImagingFactory2());
            }


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

                SourceBufferRange Range = ReadSourceAt(ScrollBackBuffer, Line.FirstP, Line.OnePastLastP - Line.FirstP);
                Cursor.Props = Line.StartingProps;
                if (ParseLineIntoGlyphs(Range, Cursor, Line.ContainsComplexChars))
                {
                    CursorJumped = true;
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
            char[] Prompt = new char[] { '>', ' ' };
            SourceBufferRange PromptRange = new SourceBufferRange();
            PromptRange.Count = Prompt.Length;
            PromptRange.Data = Prompt;
            ParseLineIntoGlyphs(PromptRange, Cursor, false);

            SourceBufferRange CommandLineRange = new SourceBufferRange();
            CommandLineRange.Count = CommandLineCount;
            CommandLineRange.Data = CommandLine;
            ParseLineIntoGlyphs(CommandLineRange, Cursor, true);

            char[] CursorCode = new char[] { '\x1b', '[', '5', 'm', (char)0xe2, (char)0x96, (char)0x88 };
            SourceBufferRange CursorRange = new SourceBufferRange();
            CursorRange.Count = CursorCode.Length;
            CursorRange.Data = CursorCode;
            ParseLineIntoGlyphs(CursorRange, Cursor, true);
            AdvanceRowNoClear(Cursor.Position);

            ScreenBuffer.FirstLineY = CursorJumped ? 0 : Cursor.Position.Y;
        }

        bool ParseLineIntoGlyphs(SourceBufferRange Range,
                                CursorState Cursor, bool ContainsComplexChars)
        {
            var CursorJumped = false;

            while (Range.Count > 0)
            {
                // NOTE(casey): Eat all non-Unicode
                char Peek = PeekToken(Range, 0);
                if ((Peek == '\x1b') && AtEscape(Range))
                {
                    if (ParseEscape(Range, Cursor))
                    {
                        CursorJumped = true;
                    }
                }
                else if (Peek == '\r')
                {
                    GetToken(Range);
                    Cursor.Position.X = 0;
                }
                else if (Peek == '\n')
                {
                    GetToken(Range);
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
                    SourceBufferRange SubRange = Range;
                    do
                    {
                        Range = ConsumeCount(Range, 1);
                    } while (Range.Count > 0 &&
                                (Range.Data.Span[0] != '\n') &&
                                (Range.Data.Span[0] != '\r') &&
                                (Range.Data.Span[0] != '\x1b'));


                    // NOTE(casey): Pass the range between the escape codes to Uniscribe
                    SubRange.Count = Range.AbsoluteP - SubRange.AbsoluteP;
                    ParseWithUniscribe(SubRange, Cursor);
                }
                else
                {
                    // NOTE(casey): It's not an escape, and we know there are only simple characters on the line.

                    char CodePoint = GetToken(Range);
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
                            GlyphHash RunHash = ComputeGlyphHash(2, CodePoint.ToString(), DefaultSeed);
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

        static GlyphEntry GetEntry(GlyphTable Table, uint Index)
        {
            //Assert(Index < Table->EntryCount);
            GlyphEntry Result = Table.Entries[Index];
            return Result;
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
                    Renderer.DeviceContext.CopySubresourceRegion(Renderer.GlyphTexture, 0, sourceBox, Renderer.GlyphTransfer, 0, X, Y, 0);
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

        void PrepareTilesForTransfer(GlyphGenerator GlyphGen, D3D11Renderer Renderer, int Count, string String, GlyphDim Dim)
        {
            var StringLen = Count;
            //Assert(StringLen == Count);

            //SharpDX.Direct2D1.RenderTarget
            DWriteDrawText(GlyphGen, StringLen, String, 0, 0, GlyphGen.TransferWidth, GlyphGen.TransferHeight,
                           Renderer.DWriteRenderTarget, Renderer.DWriteFillBrush, Dim.XScale, Dim.YScale);
        }

        void DWriteDrawText(GlyphGenerator GlyphGen, int StringLen, string String,
                               uint Left, uint Top, uint Right, uint Bottom,
                               SharpDX.Direct2D1.RenderTarget RenderTarget,
                               SharpDX.Direct2D1.SolidColorBrush FillBrush,
                               float XScale, float YScale)
        {
            SharpDX.Mathematics.Interop.RawRectangleF Rect = new SharpDX.Mathematics.Interop.RawRectangleF();

            Rect.Left = (float)Left;
            Rect.Top = (float)Top;
            Rect.Right = (float)Right;
            Rect.Bottom = (float)Bottom;

            RenderTarget.Transform = SharpDX.Matrix3x2.Scaling(XScale, YScale, new SharpDX.Vector2(0, 0));

            RenderTarget.BeginDraw();
            RenderTarget.Clear(new SharpDX.Mathematics.Interop.RawColor4(255, 0, 255, 0));
            RenderTarget.DrawText(String, StringLen, GlyphGen.TextFormat, Rect, FillBrush,
                SharpDX.Direct2D1.DrawTextOptions.Clip | SharpDX.Direct2D1.DrawTextOptions.EnableColorFont,
                SharpDX.Direct2D1.MeasuringMode.Natural);
            RenderTarget.Flush();
            RenderTarget.EndDraw();
            //if (!SUCCEEDED(Error))
            //{
            //    Assert(!"EndDraw failed");
            //}
        }

        static GlyphHash ComputeGlyphHash(int Count, string At, char[] Seedx16)
        {
            var code = HashCode.Combine(Count, At.GetHashCode());
            return new GlyphHash
            {
                Value = code
            };
            //            /* TODO(casey):

            //              Consider and test some alternate hash designs.  The hash here
            //              was the simplest thing to type in, but it is not necessarily
            //              the best hash for the job.  It may be that less AES rounds 
            //              would produce equivalently collision-free results for the
            //              problem space.  It may be that non-AES hashing would be
            //              better.  Some careful analysis would be nice.
            //            */

            //            // TODO(casey): Does the result of a grapheme composition
            //            // depend on whether or not it was RTL or LTR?  Or are there
            //            // no fonts that ever get used in both directions, so it doesn't
            //            // matter?

            //            // TODO(casey): Double-check exactly the pattern
            //            // we want to use for the hash here

            //            GlyphHash Result = new GlyphHash();

            //            // TODO(casey): Should there be an IV?
            //            __m128i HashValue = _mm_cvtsi64_si128(Count);
            //            HashValue = _mm_xor_si128(HashValue, _mm_loadu_si128((__m128i*)Seedx16));

            //            var ChunkCount = Count / 16;
            //            while (ChunkCount-- > 0)
            //            {
            //                __m128i In = _mm_loadu_si128((__m128i*)At);

            //                HashValue = _mm_xor_si128(HashValue, In);
            //                HashValue = _mm_aesdec_si128(HashValue, _mm_setzero_si128());
            //                HashValue = _mm_aesdec_si128(HashValue, _mm_setzero_si128());
            //                HashValue = _mm_aesdec_si128(HashValue, _mm_setzero_si128());
            //                HashValue = _mm_aesdec_si128(HashValue, _mm_setzero_si128());
            //            }

            //            var Overhang = Count % 16;


            //#if 0
            //    __m128i In = _mm_loadu_si128((__m128i *)At);
            //#else
            //            // TODO(casey): This needs to be improved - it's too slow, and the #if 0 branch would be nice but can't
            //            // work because of overrun, etc.
            //            char Temp[16];
            //            __movsb((unsigned char *)Temp, At, Overhang);
            //            __m128i In = _mm_loadu_si128((__m128i*)Temp);
            //#endif
            //            In = _mm_and_si128(In, _mm_loadu_si128((__m128i*)(OverhangMask + 16 - Overhang)));
            //            HashValue = _mm_xor_si128(HashValue, In);
            //            HashValue = _mm_aesdec_si128(HashValue, _mm_setzero_si128());
            //            HashValue = _mm_aesdec_si128(HashValue, _mm_setzero_si128());
            //            HashValue = _mm_aesdec_si128(HashValue, _mm_setzero_si128());
            //            HashValue = _mm_aesdec_si128(HashValue, _mm_setzero_si128());

            //            Result.Value = HashValue;

            //            return Result;
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
            var sourceString = new string(UTF8Range.Data.ToArray());
            var c = Encoding.Unicode.GetBytes(sourceString);
            c.CopyTo(Partitioner.Expansion, 0);

            //var Count = MultiByteToWideChar(CP_UTF8, 0, UTF8Range.Data, (DWORD)UTF8Range.Count,
            //                                  Partitioner->Expansion, ArrayCount(Partitioner->Expansion));
            char[] Data = Partitioner.Expansion;
            var Count = sourceString.Length;

            Uniscribe.NativeMethods.ScriptItemize(sourceString, c.Length, Partitioner.Items.Length,
                ref Partitioner.UniControl,
                ref Partitioner.UniState, Partitioner.Items, out var ItemCount);

            var Segment = false;

            for (int ItemIndex = 0;
                ItemIndex < ItemCount;
                ++ItemIndex)
            {
                var Items = Partitioner.Items.AsSpan(ItemIndex);

                //Assert((DWORD)Item->iCharPos < Count);
                var StrCount = Count - Items[0].iCharPos;
                if ((ItemIndex + 1) < ItemCount)
                {
                    //Assert(Item[1].iCharPos >= Item[0].iCharPos);
                    StrCount = Items[1].iCharPos - Items[0].iCharPos;
                }

                var Str = new string(Data.AsSpan(Items[0].iCharPos));

                var IsComplex = Uniscribe.NativeMethods.ScriptIsComplex(Str, StrCount, Uniscribe.Constants.SIC_COMPLEX) == Uniscribe.Constants.S_OK;
                Uniscribe.NativeMethods.ScriptBreak(Str, StrCount, ref Items[0].a, Partitioner.Log);

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
                if (Items[0].a.fRTL > 0 ||
                    Items[0].a.fLayoutRTL > 0)
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
                        var Run = Str.AsSpan((int)Start);
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
                            GlyphHash RunHash = ComputeGlyphHash(2 * ThisCount, new string(Run), DefaultSeed);
                            GlyphDim GlyphDim = GetGlyphDim(GlyphGen, GlyphTable, ThisCount, new string(Run), RunHash);
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
                                            PrepareTilesForTransfer(GlyphGen, Renderer, ThisCount, new string(Run), GlyphDim);
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

        GlyphDim GetGlyphDim(GlyphGenerator GlyphGen, GlyphTable Table, int Count, string String, GlyphHash RunHash)
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

        SIZE DWriteGetTextExtent(GlyphGenerator GlyphGen, int StringLen, string String)
        {
            SIZE Result = new SIZE();

            using var Layout = new TextLayout(GlyphGen.DWriteFactory, String, GlyphGen.TextFormat, GlyphGen.TransferWidth, GlyphGen.TransferHeight);

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
                         CodePoint <= MaxDirectCodepoint;
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

        static SourceBufferRange ReadSourceAt(SourceBuffer Buffer, int AbsoluteP, int Count)
        {
            SourceBufferRange Result = new SourceBufferRange();
            if (IsInBuffer(Buffer, AbsoluteP))
            {
                Result.AbsoluteP = AbsoluteP;
                Result.Count = (Buffer.AbsoluteFilledSize - AbsoluteP);
                Result.Data = Buffer.Data.Slice(Buffer.DataSize + Buffer.RelativePoint - Result.Count);

                if (Result.Count > Result.Data.Length)
                {

                }

                if (Result.Count > Count)
                {
                    Result.Count = Count;
                }
            }

            return Result;
        }

        static bool IsInBuffer(SourceBuffer Buffer, int AbsoluteP)
        {
            var BackwardOffset = Buffer.AbsoluteFilledSize - AbsoluteP;
            var Result = ((AbsoluteP < Buffer.AbsoluteFilledSize) &&
                          (BackwardOffset < Buffer.DataSize));
            return Result;
        }

        bool UpdateTerminalBuffer(StreamReader FromPipe)
        {
            var Result = false;

            if (FromPipe is not null)
            {
                Result = true;

                var Term = ScreenBuffer;

                var PendingCount = GetPipePendingDataCount(FromPipe);
                if (PendingCount > 0)
                {
                    SourceBufferRange Dest = GetNextWritableRange(ScrollBackBuffer, PendingCount);

                    var ReadCount = 0;
                    var read = FromPipe.ReadBlock(Dest.Data.Span.Slice(0, Dest.Count));
                    if (read > 0)
                    {
                        //Assert(ReadCount <= Dest.Count);
                        Dest.Count = ReadCount;
                        CommitWrite(ScrollBackBuffer, Dest.Count);
                        ParseLines(Dest, RunningCursor);
                    }
                }
                else
                {
                    //var Error = GetLastError();
                    //if ((Error == ERROR_BROKEN_PIPE) ||
                    //   (Error == ERROR_INVALID_HANDLE))
                    //{
                    //    Result = 0;
                    //}
                }
            }

            return Result;
        }

        static int GetPipePendingDataCount(StreamReader Pipe)
        {
            return Pipe.Peek();
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
            var handle = new HandleRef(null, window);

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

                                            //DWORD SpaceLeft = ArrayCount(Terminal->CommandLine) - Terminal->CommandLineCount;
                                            //Terminal->CommandLineCount +=
                                            //    WideCharToMultiByte(CP_UTF8, 0,
                                            //                        Chars, CharCount,
                                            //                        Terminal->CommandLine + Terminal->CommandLineCount,
                                            //                        SpaceLeft, 0, 0);
                                            var command = Encoding.Unicode.GetString(
                                                Encoding.Convert(
                                                    Encoding.UTF8,
                                                    Encoding.Unicode,
                                                    Chars.Select(x => (byte)x).ToArray()));
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
            while (ParamStart < CommandLineCount)
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

            var command = new string(CommandLine.AsSpan(0, ParamStart - 1));

            SourceBufferRange ParamRange = new SourceBufferRange();
            ParamRange.Data = B;
            ParamRange.Count = (int)(CommandLineCount - ParamStart);

            // TODO(casey): Collapse all these options into a little array, so there's no
            // copy-pasta.
            AppendOutput("\n");

            if (command == "status")
            {
                RunningCursor.ClearProps();
                AppendOutput("RefTerm v%u\n", Assembly.GetExecutingAssembly().GetName().Version);
                AppendOutput("Size: %u x %u\n", ScreenBuffer.DimX, ScreenBuffer.DimY);
                //AppendOutput("Fast pipe: %s\n", EnableFastPipe ? "ON" : "off");
                AppendOutput("Font: %S %u\n", RequestedFontName, RequestedFontHeight);
                AppendOutput("Line Wrap: %s\n", LineWrap ? "ON" : "off");
                //AppendOutput("Debug: %s\n", DebugHighlighting ? "ON" : "off");
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
                AppendOutput("LineWrap: %s\n", LineWrap ? "ON" : "off");
            }
            else if (command == "debug")
            {
                DebugHighlighting = !DebugHighlighting;
                AppendOutput("Debug: %s\n", DebugHighlighting ? "ON" : "off");
            }
            //else if (command == "throttle")
            //{
            //    NoThrottle = !NoThrottle;
            //    AppendOutput("Throttling: %s\n", !NoThrottle ? "ON" : "off");
            //}
            else if (command == "font")
            {
                //DWORD NullAt = MultiByteToWideChar(CP_UTF8, 0, B, (DWORD)(Terminal->CommandLineCount - ParamStart),
                //                                   Terminal->RequestedFontName, ArrayCount(Terminal->RequestedFontName) - 1);

                //RequestedFontName[NullAt] = 0;

                RequestedFontName = new string(CommandLine.AsSpan(0..ParamStart));

                RefreshFont();
                AppendOutput("Font: %S\n", RequestedFontName);
            }
            else if (command == "fontsize")
            {
                RequestedFontHeight = (int)ParseNumber(ParamRange);
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
                RunningCursor.ClearCursor();
                for (var i = 0; i < Lines.Length; i++)
                {
                    var line = Lines[i];
                    line.Clear();
                }
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
                AppendOutput("%s\n", B);
            }
            else if (command == "")
            {
            }
            else
            {
                //char ProcessName[ArrayCount(Terminal->CommandLine) + 1];
                //char ProcessCommandLine[ArrayCount(Terminal->CommandLine) + 1];
                var processName = $"{new string(A.AsSpan())}.exe";
                var processCommandLine = $"{processName} {new string(B.Span)}";

                if (!ExecuteSubProcess(processName, processCommandLine))
                {
                    processName = "c:\\Windows\\System32\\cmd.exe";
                    processCommandLine = $"cmd.exe /c {new string(A.AsSpan())}.exe {new string(B.Span)}";
                    if (!ExecuteSubProcess(processName, processCommandLine))
                    {
                        AppendOutput("ERROR: Unable to execute %s\n", CommandLine);
                    }
                }
            }
        }

        bool ExecuteSubProcess(string ProcessName, string ProcessCommandLine)
        {
            if (ChildProcess is not null)
            {
                KillProcess();
            }

            //PROCESS_INFORMATION ProcessInfo = { 0 };
            //STARTUPINFOA StartupInfo = { sizeof(StartupInfo) };
            //StartupInfo.dwFlags = STARTF_USESTDHANDLES;

            //SECURITY_ATTRIBUTES Inherit = { sizeof(Inherit) };
            //Inherit.bInheritHandle = TRUE;

            //CreatePipe(&StartupInfo.hStdInput, &Terminal->Legacy_WriteStdIn, &Inherit, 0);
            //CreatePipe(&Terminal->Legacy_ReadStdOut, &StartupInfo.hStdOutput, &Inherit, 0);
            //CreatePipe(&Terminal->Legacy_ReadStdError, &StartupInfo.hStdError, &Inherit, 0);

            //SetHandleInformation(Terminal->Legacy_WriteStdIn, HANDLE_FLAG_INHERIT, 0);
            //SetHandleInformation(Terminal->Legacy_ReadStdOut, HANDLE_FLAG_INHERIT, 0);
            //SetHandleInformation(Terminal->Legacy_ReadStdError, HANDLE_FLAG_INHERIT, 0);

            var process = new Process();
            var ProcessDir = ".\\";

            process.StartInfo = new ProcessStartInfo
            {
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                FileName = ProcessName,
                Arguments = ProcessCommandLine,
                WorkingDirectory = ProcessDir
            };

            if (!process.Start())
            {
                return false;
            }
            ChildProcess = process;

            return true;

            //int Result = 0;

            //char* ProcessDir = ".\\";
            //if (CreateProcessA(
            //    ProcessName,
            //    ProcessCommandLine,
            //    0,
            //    0,
            //    TRUE,
            //    CREATE_NO_WINDOW | CREATE_SUSPENDED,
            //    0,
            //    ProcessDir,
            //    &StartupInfo,
            //    &ProcessInfo))
            //{
            //    //if (Terminal->EnableFastPipe)
            //    //{
            //    //    wchar_t PipeName[64];
            //    //    wsprintfW(PipeName, L"\\\\.\\pipe\\fastpipe%x", ProcessInfo.dwProcessId);
            //    //    Terminal->FastPipe = CreateNamedPipeW(PipeName, PIPE_ACCESS_DUPLEX | FILE_FLAG_OVERLAPPED, 0, 1,
            //    //                                              Terminal->PipeSize, Terminal->PipeSize, 0, 0);

            //    //    // TODO(casey): Should give this its own event / overlapped
            //    //    ConnectNamedPipe(Terminal->FastPipe, &Terminal->FastPipeTrigger);

            //    //    DWORD Error = GetLastError();
            //    //    Assert(Error == ERROR_IO_PENDING);
            //    //}

            //    ResumeThread(ProcessInfo.hThread);
            //    CloseHandle(ProcessInfo.hThread);
            //    Terminal->ChildProcess = ProcessInfo.hProcess;

            //    Result = 1;
            //}

            //CloseHandle(StartupInfo.hStdInput);
            //CloseHandle(StartupInfo.hStdOutput);
            //CloseHandle(StartupInfo.hStdError);

            //return Result;
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
            ChildProcess?.Dispose(); ;
            //CloseHandle(Legacy_WriteStdIn);
            //CloseHandle(Legacy_ReadStdOut);
            //CloseHandle(Legacy_ReadStdError);
            //CloseHandle(Terminal->FastPipe);

            ChildProcess = null;
            //Legacy_WriteStdIn = null;
            //Legacy_ReadStdOut = null;
            //Legacy_ReadStdError = null;
            //FastPipe = null;
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

            SourceBufferRange Dest = GetNextWritableRange(ScrollBackBuffer, LARGEST_AVAILABLE);
            //va_list ArgList;
            //va_start(ArgList, Format);
            //int Used = wvsprintfA(Dest.Data, Format, ArgList);
            //va_end(ArgList);

            var str = string.Format(Format, args);
            var used = str.Length;

            str.AsSpan().CopyTo(Dest.Data.Span);

            Dest.Count = used;
            CommitWrite(ScrollBackBuffer, Dest.Count);
            ParseLines(Dest, RunningCursor);
        }

        void ParseLines(SourceBufferRange Range, CursorState Cursor)
        {
            /* TODO(casey): Currently, if the commit of line data _straddles_ a control code boundary
               this code does not properly _stop_ the processing cursor.  This can cause an edge case
               where a VT code that splits a line _doesn't_ split the line as it should.  To fix this
               the ending code just needs to check to see if the reason it couldn't parse an escape
               code in "AtEscape" was that it ran out of characters, and if so, don't advance the parser
               past that point.
            */

            var Carriage = Vector128.Create('\n');
            var Escape = Vector128.Create('\x1b');
            var Complex = Vector128.Create(0x80);

            var SplitLineAtCount = 4096;
            var LastP = Range.AbsoluteP;
            while (Range.Count > 0)
            {
                var ContainsComplex = false;
                //__m128i ContainsComplex = _mm_setzero_si128();
                //var ContainsComplex = Vector128.Create(0);

                int Count = Range.Count;
                if (Count > SplitLineAtCount)
                {
                    Count = SplitLineAtCount;
                }

                var Data = Range.Data.Span;
                var index = 0;
                while (index < Count)
                {
                    var c = Data[index];
                    var testC = c == '\n';
                    var testE = c == '\x1b';
                    var testX = c == (char)0x80;
                    var test = testC && testE;


                    //__m128i Batch = _mm_loadu_si128((__m128i*)Data);
                    //__m128i TestC = _mm_cmpeq_epi8(Batch, Carriage);
                    //__m128i TestE = _mm_cmpeq_epi8(Batch, Escape);
                    //__m128i TestX = _mm_and_si128(Batch, Complex);
                    //__m128i Test = _mm_or_si128(TestC, TestE);
                    //int Check = _mm_movemask_epi8(Test);
                    //if (Check)
                    //{
                    //    int Advance = _tzcnt_u32(Check);
                    //    __m128i MaskX = _mm_loadu_si128((__m128i*)(OverhangMask + 16 - Advance));
                    //    TestX = _mm_and_si128(MaskX, TestX);
                    //    ContainsComplex = _mm_or_si128(ContainsComplex, TestX);
                    //    Count -= Advance;
                    //    Data += Advance;
                    //    break;
                    //}

                    //ContainsComplex = _mm_or_si128(ContainsComplex, TestX);
                    ContainsComplex |= testX;

                    index++;
                }

                //Range = ConsumeCount(Range, Data - Range.Data);
                Range = ConsumeCount(Range, index);

                Lines[CurrentLineIndex].ContainsComplexChars |= ContainsComplex;
                //_mm_movemask_epi8(ContainsComplex);

                if (AtEscape(Range))
                {
                    int FeedAt = Range.AbsoluteP;
                    if (ParseEscape(Range, Cursor))
                    {
                        LineFeed(FeedAt, FeedAt, Cursor.Props);
                    }
                }
                else
                {
                    char Token = GetToken(Range);
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
                if (GetLineLength(Lines[CurrentLineIndex]) > SplitLineAtCount)
                {
                    LineFeed(Range.AbsoluteP, Range.AbsoluteP, Cursor.Props);
                }
            }
        }

        static int GetLineLength(Line Line)
        {
            //Assert(Line->OnePastLastP >= Line->FirstP);
            var Result = Line.OnePastLastP - Line.FirstP;
            return Result;
        }

        void LineFeed(int AtP, int NextLineStart, GlyphProps AtProps)
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

        void UpdateLineEnd(int ToP)
        {
            Lines[CurrentLineIndex].OnePastLastP = ToP;
        }

        static bool ParseEscape(SourceBufferRange Range, CursorState Cursor)
        {
            var MovedCursor = false;

            GetToken(Range);
            GetToken(Range);

            char Command = (char)0;
            uint ParamCount = 0;
            uint[] Params = new uint[8];

            while ((ParamCount < Params.Length) && Range.Count > 0)
            {
                char Token = PeekToken(Range, 0);
                if (IsDigit(Token))
                {
                    Params[ParamCount++] = ParseNumber(Range);
                    char Semi = GetToken(Range);
                    if (Semi != ';')
                    {
                        Command = Semi;
                        break;
                    }
                }
                else
                {
                    Command = GetToken(Range);
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

        static uint ParseNumber(SourceBufferRange Range)
        {
            uint Result = 0;
            while (IsDigit(PeekToken(Range, 0)))
            {
                char Token = GetToken(Range);
                Result = (uint)(10 * Result + (Token - '0'));
            }
            return Result;
        }

        static bool IsDigit(char Digit)
        {
            var Result = ((Digit >= '0') && (Digit <= '9'));
            return Result;
        }

        static char GetToken(SourceBufferRange Range)
        {
            char Result = (char)0;

            if (Range.Count > 0)
            {
                Result = Range.Data.Span[0];
                Range = ConsumeCount(Range, 1);
            }

            return Result;
        }

        static bool AtEscape(SourceBufferRange Range)
        {
            var Result = ((PeekToken(Range, 0) == '\x1b') &&
                          (PeekToken(Range, 1) == '['));
            return Result;
        }
        static char PeekToken(SourceBufferRange Range, int Ordinal)
        {
            char Result = (char)0;

            if (Ordinal < Range.Count)
            {
                Result = Range.Data.Span[Ordinal];
            }

            return Result;
        }

        static SourceBufferRange ConsumeCount(SourceBufferRange Source, int Count)
        {
            SourceBufferRange Result = Source;

            if (Count > Result.Count)
            {
                Count = Result.Count;
            }

            Result.Data = Result.Data.Slice(Count);
            Result.AbsoluteP += Count;
            Result.Count -= Count;

            return Result;
        }

        void CommitWrite(SourceBuffer Buffer, int Size)
        {
            //Assert(Buffer->RelativePoint < Buffer->DataSize);
            //Assert(Size <= Buffer->DataSize);

            Buffer.RelativePoint += Size;
            Buffer.AbsoluteFilledSize += Size;

            var WrappedRelative = Buffer.RelativePoint - Buffer.DataSize;
            Buffer.RelativePoint = (Buffer.RelativePoint >= Buffer.DataSize) ? WrappedRelative : Buffer.RelativePoint;

            //Assert(Buffer.RelativePoint < Buffer.DataSize);
        }

        static SourceBufferRange GetNextWritableRange(SourceBuffer Buffer, int MaxCount)
        {
            //Assert(Buffer->RelativePoint < Buffer->DataSize);

            var Result = new SourceBufferRange();
            Result.AbsoluteP = Buffer.AbsoluteFilledSize;
            Result.Count = Buffer.DataSize;
            Result.Data = Buffer.Data.Slice(Buffer.RelativePoint);

            if (Result.Count > MaxCount)
            {
                Result.Count = MaxCount;
            }

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

                ++X;
            }

            //GetAndClearStats(Result);

            //if (Memory)
            //{
            //    // NOTE(casey): Always put the glyph_entry array at the base of the memory, because the
            //    // compiler may generate aligned-SSE ops, which would crash if it was unaligned.
            //    glyph_entry* Entries = (glyph_entry*)Memory;
            //    Result = (glyph_table*)(Entries + Params.EntryCount);
            //    Result->HashTable = (uint32_t*)(Result + 1);
            //    Result->Entries = Entries;

            //    Result->HashMask = Params.HashCount - 1;
            //    Result->HashCount = Params.HashCount;
            //    Result->EntryCount = Params.EntryCount;

            //    memset(Result->HashTable, 0, Result->HashCount * sizeof(Result->HashTable[0]));

            //    uint32_t StartingTile = Params.ReservedTileCount;

            //    glyph_entry* Sentinel = GetSentinel(Result);
            //    uint32_t X = StartingTile % Params.CacheTileCountInX;
            //    uint32_t Y = StartingTile / Params.CacheTileCountInX;
            //    for (uint32_t EntryIndex = 0;
            //        EntryIndex < Params.EntryCount;
            //        ++EntryIndex)
            //    {
            //        if (X >= Params.CacheTileCountInX)
            //        {
            //            X = 0;
            //            ++Y;
            //        }

            //        glyph_entry* Entry = GetEntry(Result, EntryIndex);
            //        if ((EntryIndex + 1) < Params.EntryCount)
            //        {
            //            Entry->NextWithSameHash = EntryIndex + 1;
            //        }
            //        else
            //        {
            //            Entry->NextWithSameHash = 0;
            //        }
            //        Entry->GPUIndex = PackGlyphCachePoint(X, Y);

            //        Entry->FilledState = 0;
            //        Entry->DimX = 0;
            //        Entry->DimY = 0;

            //        ++X;
            //    }

            //    GetAndClearStats(Result);
            //}

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
        private int SafeRatio1(int a, int b)
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

            DWriteReleaseFont(GlyphGen);

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

        void DWriteReleaseFont(GlyphGenerator GlyphGen)
        {
            //if (GlyphGen.FontFace is not null)
            //{
            //    GlyphGen.FontFace.Release();
            //    GlyphGen.FontFace = null;
            //}
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
            NativeWindows.GetSystemInfo(ref info);

            dataSize = (dataSize + info.dwAllocationGranularity - 1) & ~(info.dwAllocationGranularity - 1);

            //var section = MemoryMappedFile.CreateNew("mapName", dataSize, MemoryMappedFileAccess.ReadWrite, MemoryMappedFileOptions.None, HandleInheritability.None);

            //var accessor = section.CreateViewAccessor();


            //var m = new Memory<char>(accessor.SafeMemoryMappedViewHandle.);

            result.InternalData = new char[dataSize * 2];
            result.Data = new Memory<char>(result.InternalData);
            result.DataSize = (int)dataSize;

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

            var constantBuffer = new RendererConstBuffer();
            renderer.ConstantBuffer = SharpDX.Direct3D11.Buffer.Create<RendererConstBuffer>(renderer.Device, ref constantBuffer, constantBufferDesc);
            renderer.ComputeShader = new ComputeShader(renderer.Device, CssShaderBytes);
            renderer.PixelShader = new PixelShader(renderer.Device, PSShaderBytes);
            renderer.VertexShader = new VertexShader(renderer.Device, VSShaderBytes);

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
                //ModeDescription = new ModeDescription(Format.B8G8R8A8_UNorm),
                //OutputHandle = window,

            };

            using (var factory = new SharpDX.DXGI.Factory2())
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
}