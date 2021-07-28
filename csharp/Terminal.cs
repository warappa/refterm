using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DirectWrite;
using SharpDX.DXGI;
using System;
using System.Buffers;
using System.ComponentModel;
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
        private static byte[] OpeningMessage = new byte[] { 0xE0, 0xA4, 0x9C, 0xE0, 0xA5, 0x8B, 0x20, 0xE0, 0xA4, 0xB8, 0xE0, 0xA5, 0x8B, 0x20, 0xE0, 0xA4, 0xB0, 0xE0, 0xA4, 0xB9, 0xE0, 0xA4, 0xBE, 0x20, 0xE0, 0xA4, 0xB9, 0xE0, 0xA5, 0x8B, 0x20, 0xE0, 0xA4, 0x89, 0xE0, 0xA4, 0xB8, 0xE0, 0xA5, 0x87, 0x20, 0xE0, 0xA4, 0xA4, 0xE0, 0xA5, 0x8B, 0x20, 0xE0, 0xA4, 0x9C, 0xE0, 0xA4, 0x97, 0xE0, 0xA4, 0xBE, 0x20, 0xE0, 0xA4, 0xB8, 0xE0, 0xA4, 0x95, 0xE0, 0xA4, 0xA4, 0xE0, 0xA5, 0x87, 0x20, 0xE0, 0xA4, 0xB9, 0xE0, 0xA5, 0x88, 0xE0, 0xA4, 0x82, 0x2C, 0x20, 0xE0, 0xA4, 0xB2, 0xE0, 0xA5, 0x87, 0xE0, 0xA4, 0x95, 0xE0, 0xA4, 0xBF, 0xE0, 0xA4, 0xA8, 0x20, 0xE0, 0xA4, 0x9C, 0xE0, 0xA5, 0x8B, 0x20, 0xE0, 0xA4, 0x86, 0xE0, 0xA4, 0x81, 0xE0, 0xA4, 0x96, 0xE0, 0xA5, 0x87, 0x20, 0xE0, 0xA4, 0xAE, 0xE0, 0xA5, 0x82, 0xE0, 0xA4, 0x81, 0xE0, 0xA4, 0xA6, 0x20, 0xE0, 0xA4, 0x95, 0xE0, 0xA4, 0xB0, 0x20, 0xE0, 0xA4, 0xB8, 0xE0, 0xA5, 0x8B, 0xE0, 0xA4, 0xA8, 0xE0, 0xA5, 0x87, 0x20, 0xE0, 0xA4, 0x95, 0xE0, 0xA4, 0xBE, 0x20, 0xE0, 0xA4, 0x85, 0xE0, 0xA4, 0xAD, 0xE0, 0xA4, 0xBF, 0xE0, 0xA4, 0xA8, 0xE0, 0xA4, 0xAF, 0x20, 0xE0, 0xA4, 0x95, 0xE0, 0xA4, 0xB0, 0x20, 0xE0, 0xA4, 0xB0, 0xE0, 0xA4, 0xB9, 0xE0, 0xA4, 0xBE, 0x20, 0xE0, 0xA4, 0xB9, 0xE0, 0xA5, 0x8B, 0x20, 0xE0, 0xA4, 0x89, 0xE0, 0xA4, 0xB8, 0xE0, 0xA5, 0x87, 0x20, 0xE0, 0xA4, 0x95, 0xE0, 0xA5, 0x88, 0xE0, 0xA4, 0xB8, 0xE0, 0xA5, 0x87, 0x20, 0xE0, 0xA4, 0x9C, 0xE0, 0xA4, 0x97, 0xE0, 0xA4, 0xBE, 0xE0, 0xA4, 0x8F, 0xE0, 0xA4, 0x82, 0xE0, 0xA4, 0x97, 0xE0, 0xA5, 0x87, 0x20, 0x7C, 0x20, (byte)'\n' };

        public uint DefaultForegroundColor { get; private set; } = 0x00afafaf;
        public uint DefaultBackgroundColor { get; private set; } = 0x000c0c0c;

        private bool lineWrap = true;
        private CursorState runningCursor;

        private Process? childProcess;
        private CancellationTokenSource childProcessCancellationTokenSource;

        private SourceBuffer ScrollBackBuffer { get; }
        private const int scrollBackBufferSize = 16 * 1024 * 1024;

        private D3D11Renderer renderer;
        private int textureWidth;
        private int textureHeight;
        private uint transferWidth;
        private uint transferHeight;
        private uint maxWidth;
        private uint maxHeight;
        private byte[] computeShaderBytes = Shaders.ReftermCShaderBytes;
        private byte[] pixelShaderBytes = Shaders.ReftermPSShaderBytes;
        private byte[] vertexShaderBytes = Shaders.ReftermVSShaderBytes;

        private GlyphGenerator glyphGen;
        private const int minDirectCodepoint = 32;
        private const int maxDirectCodepoint = 126;
        private Partitioner partitioner;
        private int maxLineCount;
        private Line[] lines;
        private int currentLineIndex = 0;
        private string requestedFontName;
        private int requestedFontHeight;
        private GpuGlyphIndex[] reservedTileTable = new GpuGlyphIndex[maxDirectCodepoint - minDirectCodepoint];
        private GlyphTable glyphTable;
        private static byte[] OverhangMask = new byte[32]
        {
            255, 255, 255, 255,  255, 255, 255, 255,  255, 255, 255, 255,  255, 255, 255, 255,
            0, 0, 0, 0,  0, 0, 0, 0,  0, 0, 0, 0,  0, 0, 0, 0
        };

        private TerminalBuffer screenBuffer;
        private bool quit;
        private long viewingLineOffset;
        private bool debugHighlighting;
        private char lastChar;
        private DateTime lastOutput = DateTime.MinValue;
        private char[] commandLine = new char[256];
        private int commandLineCount = 0;
        private char[] promptBuffer = new char[] { '>', ' ' };
        private char[] cursorBuffer = null;
        private int lineCount = 0;
        private bool shouldLayoutLines = true;

        public Terminal(IntPtr window)
        {
            lineWrap = true;

            runningCursor = new CursorState(this);
            screenBuffer = new TerminalBuffer(this, 0, 0);

            partitioner = new Partitioner();

            textureWidth = 4 * 2048;
            textureHeight = 4 * 2048;
            //transferWidth = 1024;
            //transferHeight = 512;
            transferWidth = (uint)textureWidth;
            transferHeight = (uint)textureHeight;
            maxWidth = 1024;
            maxHeight = 1024;

            renderer = AcquireD3D11Renderer(window, true);

            renderer.SetD3D11GlyphCacheDim(textureWidth, textureHeight);
            renderer.SetD3D11GlyphTransferDim(transferWidth, transferHeight);

            glyphGen = AllocateGlyphGenerator(transferWidth, transferHeight);
            ScrollBackBuffer = AllocateSourceBuffer(scrollBackBufferSize);

            Uniscribe.NativeMethods.ScriptRecordDigitSubstitution(Uniscribe.Defaults.LOCALE_USER_DEFAULT, out partitioner.UniscribeDigitSubstitution);
            Uniscribe.NativeMethods.ScriptApplyDigitSubstitution(ref partitioner.UniscribeDigitSubstitution, out partitioner.UniControl, out partitioner.UniState);

            maxLineCount = 8192;
            lines = new Line[maxLineCount];

            for (var i = 0; i < maxLineCount; i++)
            {
                lines[i] = new Line();
            }

            RevertToDefaultFont();
            RefreshFont();

            NativeWindows.ShowWindow(window, NativeWindows.ShowWindowOption.SW_SHOWDEFAULT);

            lines[0].StartingProps = runningCursor.Props;
            AppendOutput($"Refterm v{Assembly.GetExecutingAssembly().GetName().Version}\n");
            AppendOutput("THIS IS \x1b[38;2;255;0;0m\x1b[5mNOT\x1b[0m A REAL \x1b[9mTERMINAL\x1b[0m.\r\n" +
                "It is a reference renderer for demonstrating how to easily build relatively efficient terminal displays.\r\n" +
                "\x1b[38;2;255;0;0m\x1b[5m\x1b[4mDO NOT\x1b[0m attempt to use this as your terminal, or you will be \x1b[2mvery\x1b[0m sad.\r\n"
                );

            AppendOutput("\n");
            AppendOutput(Encoding.UTF8.GetString(OpeningMessage));
            AppendOutput("\n");

            var blinkMS = 500; // TODO(casey): Use this in blink determination
            var minTermSize = 512;
            var width = minTermSize;
            var height = minTermSize;

            while (!quit)
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

                NativeWindows.RECT clientRectangle;
                NativeWindows.GetClientRect(window, out clientRectangle);

                if (((clientRectangle.Left + minTermSize) <= clientRectangle.Right) &&
                   ((clientRectangle.Top + minTermSize) <= clientRectangle.Bottom))
                {
                    width = clientRectangle.Right - clientRectangle.Left;
                    height = clientRectangle.Bottom - clientRectangle.Top;

                    var margin = 8;
                    var newDimX = (uint)SafeRatio1((uint)width - margin, glyphGen.FontWidth);
                    var newDimY = (uint)SafeRatio1((uint)height - margin, glyphGen.FontHeight);
                    if (newDimX > maxWidth)
                    {
                        newDimX = maxWidth;
                    }
                    if (newDimY > maxHeight)
                    {
                        newDimY = maxHeight;
                    }

                    // TODO(casey): Maybe only allocate on size differences,
                    // etc. Make a real resize function here for people who care.
                    if ((screenBuffer.DimX != newDimX) ||
                       (screenBuffer.DimY != newDimY))
                    {
                        DeallocateTerminalBuffer(screenBuffer);
                        screenBuffer = AllocateTerminalBuffer(newDimX, newDimY);
                    }
                }

                while (!shouldLayoutLines)
                {
                    var ms = (DateTime.UtcNow - lastOutput).TotalMilliseconds;
                    if (ms > blinkMS)
                    {
                        shouldLayoutLines = true;
                    }

                    Thread.Sleep(16);
                }

                LayoutLines();

                // TODO(casey): Split RendererDraw into two!
                // Update, and render, since we only need to update if we actually get new input.

                var blink = DateTime.UtcNow.Millisecond > blinkMS;
                if (renderer.Device is null)
                {
                    renderer = AcquireD3D11Renderer(window, false);
                    RefreshFont();
                }
                if (renderer.Device is not null)
                {
                    RendererDraw((uint)width, (uint)height, screenBuffer, blink ? 0xffffffff : 0xff222222);
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

            DWriteRelease(glyphGen);
            ReleaseD3D11Renderer();

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

        private void ReleaseD3D11RenderTargets()
        {
            renderer.RenderView?.Dispose();
            renderer.RenderView = null;

            renderer.RenderTarget?.Dispose();
            renderer.RenderTarget = null;
        }

        private void RendererDraw(uint width, uint height, TerminalBuffer term, uint blinkModulate)
        {
            // TODO(casey): This should be split into two routines now, since we don't actually
            // need to resubmit anything if the terminal hasn't updated.

            // resize RenderView to match window size
            if (width != renderer.CurrentWidth || height != renderer.CurrentHeight)
            {
                renderer.DeviceContext.ClearState();

                ReleaseD3D11RenderTargets();

                renderer.DeviceContext.Flush();

                if (width != 0 && height != 0)
                {
                    renderer.SwapChain.ResizeBuffers(0, (int)width, (int)height, Format.Unknown, SwapChainFlags.FrameLatencyWaitAbleObject);

                    using (var buffer = renderer.SwapChain.GetBackBuffer<Texture2D>(0))
                    {
                        buffer.DebugName = "BackBuffer Texture";

                        if (renderer.UseComputeShader)
                        {
                            renderer.RenderView = new UnorderedAccessView(renderer.Device, buffer);
                        }
                        else
                        {
                            renderer.RenderTarget = new RenderTargetView(renderer.Device, buffer);
                            renderer.RenderTarget.DebugName = "RenderTarget";

                            var viewPort = new SharpDX.Mathematics.Interop.RawViewportF
                            {
                                X = 0,
                                Y = 0,
                                Width = width,
                                Height = height
                            };

                            renderer.DeviceContext.Rasterizer.SetViewports(new[] { viewPort }, 1);
                            renderer.DeviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleStrip;
                        }
                    }
                }

                renderer.CurrentWidth = width;
                renderer.CurrentHeight = height;
            }

            //var CellCount = Renderer.CurrentWidth * Renderer.CurrentHeight;
            var cellCount = term.DimX * term.DimY;
            if (renderer.MaxCellCount < cellCount)
            {
                SetD3D11MaxCellCount(cellCount);
            }

            if (renderer.RenderView is not null ||
                renderer.RenderTarget is not null)
            {
                renderer.DeviceContext.MapSubresource(renderer.ConstantBuffer, 0,
                    MapMode.WriteDiscard, SharpDX.Direct3D11.MapFlags.None, out var mapped);

                var constData = new RendererConstBuffer
                {
                    CellSizeX = glyphGen.FontWidth,
                    CellSizeY = glyphGen.FontHeight,
                    TermSizeX = term.DimX,
                    TermSizeY = term.DimY,
                    TopMargin = 8,
                    LeftMargin = 8,
                    BlinkModulate = blinkModulate,
                    MarginColor = 0x000c0c0c,

                    StrikeMin = glyphGen.FontHeight / 2 - glyphGen.FontHeight / 10,
                    StrikeMax = glyphGen.FontHeight / 2 + glyphGen.FontHeight / 10,
                    UnderlineMin = glyphGen.FontHeight - glyphGen.FontHeight / 5,
                    UnderlineMax = glyphGen.FontHeight,
                };
                mapped.Write(constData);

                renderer.DeviceContext.UnmapSubresource(renderer.ConstantBuffer, 0);

                renderer.DeviceContext.MapSubresource(renderer.CellBuffer, 0, MapMode.WriteDiscard, SharpDX.Direct3D11.MapFlags.None, out mapped);

                var topCellCount = term.DimX * (term.DimY - term.FirstLineY);
                var bottomCellCount = term.DimX * (term.FirstLineY);

                mapped.WriteRange(
                    term.Cells,
                    (int)(term.FirstLineY * term.DimX),
                    (int)(topCellCount));

                mapped.WriteRange(term.Cells, 0, (int)bottomCellCount);

                renderer.DeviceContext.UnmapSubresource(renderer.CellBuffer, 0);

                // this should match t0/t1 order in hlsl shader
                var Resources = new[] { renderer.CellView, renderer.GlyphTextureView };

                if (renderer.UseComputeShader)
                {
                    // this issues compute shader for window size, which in real terminal should match its size
                    renderer.DeviceContext.ComputeShader.SetConstantBuffers(0, 1, renderer.ConstantBuffer);
                    renderer.DeviceContext.ComputeShader.SetShaderResources(0, Resources);
                    renderer.DeviceContext.ComputeShader.SetUnorderedAccessViews(0, renderer.RenderView);
                    renderer.DeviceContext.ComputeShader.SetShader(renderer.ComputeShader, null, 0);

                    renderer.DeviceContext.Dispatch((int)(renderer.CurrentWidth + 7) / 8, (int)(renderer.CurrentHeight + 7) / 8, 1);
                }
                else
                {
                    // NOTE(casey): This MUST be set every frame, because PAGE FLIPPING, I guess :/
                    renderer.DeviceContext.OutputMerger.SetRenderTargets(renderer.RenderTarget);
                    renderer.DeviceContext.PixelShader.SetConstantBuffers(0, 1, renderer.ConstantBuffer);
                    renderer.DeviceContext.PixelShader.SetShaderResources(0, Resources);
                    renderer.DeviceContext.VertexShader.SetShader(renderer.VertexShader, null, 0);
                    renderer.DeviceContext.PixelShader.SetShader(renderer.PixelShader, null, 0);

                    renderer.DeviceContext.Draw(4, 0);
                }
            }

            var vsync = false;
            var presentResult = renderer.SwapChain.Present(vsync ? 1 : 0, PresentFlags.None);

            if ((presentResult == SharpDX.DXGI.ResultCode.DeviceReset) ||
                (presentResult == SharpDX.DXGI.ResultCode.DeviceRemoved))
            {
                //Assert(!"Device lost!");
                ReleaseD3D11Renderer();
            }

            if (renderer.RenderView is not null)
            {
                renderer.DeviceContext1.DiscardView(renderer.RenderView);
            }
        }

        private void ReleaseD3D11Renderer()
        {
            // TODO(casey): When you want to release a D3D11 device, do you have to release all the sub-components?
            // Can you just release the main device and have all the sub-components release themselves?

            ReleaseD3DCellBuffer();
            ReleaseD3DGlyphCache();
            ReleaseD3DGlyphTransfer();
            ReleaseD3D11RenderTargets();

            renderer.FrameLatencyWaitableObject = IntPtr.Zero;

            renderer.ComputeShader?.Dispose();
            renderer.PixelShader?.Dispose();
            renderer.VertexShader?.Dispose();

            renderer.ConstantBuffer?.Dispose();

            renderer.RenderView?.Dispose();
            renderer.SwapChain?.Dispose();

            renderer.DeviceContext?.Dispose();
            renderer.DeviceContext1?.Dispose();
            renderer.Device?.Dispose();

            var zeroRenderer = new D3D11Renderer();
            renderer = zeroRenderer;
        }

        private void ReleaseD3DGlyphTransfer()
        {
            renderer.ReleaseD3DGlyphTransfer();
        }

        private void ReleaseD3DGlyphCache()
        {
            if (renderer.GlyphTexture is not null)
            {
                renderer.GlyphTexture.Dispose();
                renderer.GlyphTexture = null;
            }

            if (renderer.GlyphTextureView is not null)
            {
                renderer.GlyphTextureView.Dispose();
                renderer.GlyphTextureView = null;
            }
        }

        private void SetD3D11MaxCellCount(uint Count)
        {
            ReleaseD3DCellBuffer();

            if (renderer.Device is not null)
            {
                var cellBufferDesc = new BufferDescription
                {
                    SizeInBytes = (int)Count * Marshal.SizeOf<RendererCell>(),
                    //ByteWidth = Count * sizeof(renderer_cell),
                    Usage = ResourceUsage.Dynamic,
                    BindFlags = BindFlags.ShaderResource,
                    CpuAccessFlags = CpuAccessFlags.Write,
                    OptionFlags = ResourceOptionFlags.BufferStructured,
                    StructureByteStride = Marshal.SizeOf<RendererCell>(),
                };

                renderer.CellBuffer = new SharpDX.Direct3D11.Buffer(renderer.Device, cellBufferDesc);
                renderer.CellBuffer.DebugName = "CellBuffer";

                var cellViewDesc = new ShaderResourceViewDescription
                {
                    Dimension = ShaderResourceViewDimension.Buffer,
                    Buffer = new ShaderResourceViewDescription.BufferResource
                    {
                        FirstElement = 0,
                        ElementCount = (int)Count
                    }
                };

                renderer.CellView = new ShaderResourceView(renderer.Device, renderer.CellBuffer, cellViewDesc);
                renderer.CellView.DebugName = "CellView";

                renderer.MaxCellCount = Count;
            }
        }

        private void ReleaseD3DCellBuffer()
        {
            if (renderer.CellBuffer is not null)
            {
                renderer.CellBuffer.Dispose();
                renderer.CellBuffer = null;
            }

            if (renderer.CellView is not null)
            {
                renderer.CellView.Dispose();
                renderer.CellView = null;
            }
        }

        void LayoutLines()
        {
            shouldLayoutLines = false;
            // TODO(casey): Probably want to do something better here - this over-clears, since we clear
            // the whole thing and then also each line, for no real reason other than to make line wrapping
            // simpler.
            screenBuffer.Clear();

            //
            // TODO(casey): This code is super bad, and there's no need for it to keep repeating itself.
            //

            // TODO(casey): How do we know how far back to go, for control chars?
            var lineCount = 2 * screenBuffer.DimY;
            var lineOffset = currentLineIndex + viewingLineOffset - lineCount;

            var cursorJumped = false;

            var cursor = new CursorState(this);
            cursor.ClearCursor();

            for (var lineIndexIndex = 0; lineIndexIndex <= lineCount; ++lineIndexIndex)
            {
                var lineIndex = (lineOffset + lineIndexIndex) % maxLineCount;
                if (lineIndex < 0)
                {
                    lineIndex += maxLineCount;
                }

                var line = lines[lineIndex];

                var remaining = (int)(line.OnePastLastP - line.FirstP);
                var consumed = 0;
                while (remaining > 0)
                {
                    var range = ReadSourceAt(ScrollBackBuffer, line.FirstP + (ulong)consumed, remaining);
                    if (range.Count == 0)
                    {
                        break;
                    }
                    cursor.Props = line.StartingProps;
                    if (ParseLineIntoGlyphs(ref range, cursor, line.ContainsComplexChars))
                    {
                        cursorJumped = true;
                    }

                    var cc = range.Count;

                    remaining = cc;
                    consumed = cc;
                }
            }

            if (cursorJumped)
            {
                cursor.Position.X = 0;
                cursor.Position.Y = screenBuffer.DimY - 4;
            }

            AdvanceRow(cursor.Position);
            cursor.ClearProps();

#if false
    uint32_t CLCount = Terminal->CommandLineCount;

    source_buffer_range CommandLineRange = {0};
    CommandLineRange.AbsoluteP = 0;
    CommandLineRange.Count = CLCount;
    CommandLineRange.Data = (char *)Terminal->CommandLine;
#else
#endif
            var promptRange = GetPromptBufferRange();
            ParseLineIntoGlyphs(ref promptRange, cursor, false);

            var commandLineRange = new SourceBufferRange();
            commandLineRange.Count = commandLineCount;
            commandLineRange.Data = new Memory<char>(commandLine, 0, commandLineCount);
            ParseLineIntoGlyphs(ref commandLineRange, cursor, true);

            var cursorRange = GetCursorBufferRange();
            ParseLineIntoGlyphs(ref cursorRange, cursor, true);
            AdvanceRowNoClear(cursor.Position);

            runningCursor.ClearCursor();

            screenBuffer.FirstLineY = cursorJumped ? 0 : cursor.Position.Y;
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

        private bool ParseLineIntoGlyphs(ref SourceBufferRange range,
                                CursorState cursor, bool containsComplexChars)
        {
            var cursorJumped = false;

            while (range.Count > 0)
            {
                var span = range.Data.Span;

                // NOTE(casey): Eat all non-Unicode
                var Peek = PeekToken(span, 0);
                if ((Peek == '\x1b') && AtEscape(span))
                {
                    if (ParseEscape(ref range, cursor))
                    {
                        cursorJumped = true;
                    }
                }
                else if (Peek == '\r')
                {
                    GetToken(ref range);
                    cursor.Position.X = 0;
                }
                else if (Peek == '\n')
                {
                    GetToken(ref range);
                    AdvanceRow(cursor.Position);
                }
                else if (containsComplexChars)
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

                    var data = range.Data.Span;
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

                    var subRange = new SourceBufferRange(range, index);
                    ParseWithUniscribe(subRange, cursor);
                    range.Skip(subRange.Count);
                }
                else
                {
                    // NOTE(casey): It's not an escape, and we know there are only simple characters on the line.

                    var codePoint = GetToken(ref range);
                    ref var cell = ref GetCell(screenBuffer, cursor.Position);

                    GpuGlyphIndex gpuIndex;
                    if (IsDirectCodepoint(codePoint))
                    {
                        gpuIndex = reservedTileTable[codePoint - minDirectCodepoint];
                    }
                    else
                    {
                        //Assert(CodePoint <= 127);
                        var RunHash = ComputeGlyphHash(2, codePoint);
                        var State = FindGlyphEntryByHash(glyphTable, RunHash);


                        if (State.FilledState != GlyphEntryState.Rasterized)
                        {
                            PrepareTilesForTransfer(1, codePoint.ToString(), GetSingleTileUnitDim());

                            TransferTile(0, State.GPUIndex);

                            UpdateGlyphCacheEntry(State.Entry, GlyphEntryState.Rasterized, State.DimX, State.DimY);
                        }

                        gpuIndex = State.GPUIndex;
                    }

                    SetCellDirect(gpuIndex, cursor.Props, ref cell);

                    AdvanceColumn(cursor.Position);
                }
            }

            return cursorJumped;
        }

        static void UpdateGlyphCacheEntry(GlyphEntry entry, GlyphEntryState newState, uint newDimX, uint newDimY)
        {
            //GlyphEntry Entry = GetEntry(Table, ID);

            entry.FilledState = newState;
            entry.DimX = newDimX;
            entry.DimY = newDimY;
        }

        private static ref GlyphEntry GetEntry(GlyphTable table, uint index)
        {
            //Assert(Index < Table->EntryCount);
            return ref table.Entries[index];
        }

        private static GlyphDim GetSingleTileUnitDim()
        {
            var Result = new GlyphDim
            {
                TileCount = 1,
                XScale = 1.0f,
                YScale = 1.0f
            };
            return Result;
        }

        private void TransferTile(uint tileIndex, GpuGlyphIndex destIndex)
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

            if (renderer.DeviceContext is not null)
            {
                var point = UnpackGlyphCachePoint(destIndex);
                var x = (int)(point.X * glyphGen.FontWidth);
                var y = (int)(point.Y * glyphGen.FontHeight);

                var sourceBox = new ResourceRegion(
                    (int)(tileIndex * glyphGen.FontWidth), 0, 0,
                    (int)((tileIndex + 1) * glyphGen.FontWidth),
                    (int)glyphGen.FontHeight, 1);

                try
                {
                    renderer.DeviceContext.CopySubresourceRegion(renderer.GlyphTransfer, 0,
                        sourceBox, renderer.GlyphTexture, 0, x, y, 0);
                }
                catch (Exception exc)
                {
                    Console.WriteLine($"GPU index failed: {destIndex.Value}");
                }
            }
        }

        private static GlyphCachePoint UnpackGlyphCachePoint(GpuGlyphIndex index)
        {
            var result = new GlyphCachePoint();

            result.X = index.Value & 0xffff;
            result.Y = index.Value >> 16;

            return result;
        }

        private void PrepareTilesForTransfer(int count, ReadOnlySpan<char> str, GlyphDim dim)
        {
            //Assert(StringLen == Count);

            //SharpDX.Direct2D1.RenderTarget
            DWriteDrawText(str.Slice(0, count).ToString(), 0, 0, glyphGen.TransferWidth, glyphGen.TransferHeight,
                           renderer.DWriteRenderTarget, renderer.DWriteFillBrush, dim.XScale, dim.YScale);
        }

        private void DWriteDrawText(string str,
                               uint left, uint top, uint right, uint bottom,
                               RenderTarget renderTarget,
                               SolidColorBrush fillBrush,
                               float xScale, float yScale)
        {
            var rect = new SharpDX.Mathematics.Interop.RawRectangleF();

            rect.Left = left;
            rect.Top = top;
            rect.Right = right;
            rect.Bottom = bottom;

            renderTarget.Transform = Matrix3x2.Scaling(xScale, yScale, new Vector2(0, 0));

            renderTarget.BeginDraw();
            renderTarget.Clear(new SharpDX.Mathematics.Interop.RawColor4(1, 1, 1, 0));

            renderTarget.DrawText(str, str.Length, glyphGen.TextFormat, rect, fillBrush,
                DrawTextOptions.Clip | DrawTextOptions.EnableColorFont,
                MeasuringMode.Natural);

            renderTarget.EndDraw();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static GlyphHash ComputeGlyphHash(int count, char at)
        {
            var code = HashCode.Combine(count, at);

            return new GlyphHash
            {
                Value = code
            };
        }

        private static GlyphHash ComputeGlyphHash(int count, ReadOnlySpan<char> at)
        {
            var len = at.Length;
            if (len == 0)
            {
                return ComputeGlyphHash(count, at);
            }

            var code = 0;
            for (var i = 0; i < len; i++)
            {
                code = HashCode.Combine(count, code, at[i]);
            }

            return new GlyphHash
            {
                Value = code
            };
        }

        private void ParseWithUniscribe(SourceBufferRange utf8Range, CursorState cursor)
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

            var spanBytes = MemoryMarshal.Cast<char, byte>(utf8Range.Data.Span.Slice(0, utf8Range.Count));

            var unicodeString = Encoding.Unicode.GetString(spanBytes);
            utf8Range.Data.Span.Slice(0, utf8Range.Count)
                .CopyTo(partitioner.Expansion.AsSpan());

            var count = unicodeString.Length;

            Uniscribe.NativeMethods.ScriptItemize(unicodeString, unicodeString.Length,
                partitioner.Items.Length,
                ref partitioner.UniControl,
                ref partitioner.UniState, partitioner.Items, out var ItemCount);

            var segment = false;

            for (var itemIndex = 0;
                itemIndex < ItemCount;
                ++itemIndex)
            {
                //var Items = Partitioner.Items.AsSpan(ItemIndex);
                var item = partitioner.Items[itemIndex];

                //Assert((DWORD)Item->iCharPos < Count);
                var strCount = count - item.iCharPos;
                if ((itemIndex + 1) < ItemCount)
                {
                    //Assert(Item[1].iCharPos >= Item[0].iCharPos);
                    strCount = partitioner.Items[itemIndex + 1].iCharPos - item.iCharPos;
                }

                var str = unicodeString.AsSpan(item.iCharPos, strCount).ToString();

                var isComplex = Uniscribe.NativeMethods.ScriptIsComplex(str, strCount, Uniscribe.Constants.SIC_COMPLEX)
                    == Uniscribe.Constants.S_OK;

                Uniscribe.NativeMethods.ScriptBreak(str, strCount, ref item.a, partitioner.Log);

                var segCount = 0;

                partitioner.SegP[segCount++] = 0;
                for (var checkIndex = 0; checkIndex < strCount; ++checkIndex)
                {
                    var attr = partitioner.Log[checkIndex];
                    var shouldBreak = str[checkIndex] == ' ';
                    if (isComplex)
                    {
                        shouldBreak |= attr.fSoftBreak == 1;
                    }
                    else
                    {
                        shouldBreak |= attr.fCharStop == 1;
                    }

                    if (shouldBreak)
                    {
                        partitioner.SegP[segCount++] = (uint)checkIndex;
                    }
                }

                partitioner.SegP[segCount++] = (uint)strCount;

                var dSeg = 1;
                var segStart = 0;
                var segStop = segCount - 1;
                if (item.a.fRTL > 0 ||
                    item.a.fLayoutRTL > 0)
                {
                    dSeg = -1;
                    segStart = segCount - 2;
                    segStop = -1;
                }

                for (var segIndex = segStart; segIndex != segStop; segIndex += dSeg)
                {
                    var start = partitioner.SegP[segIndex];
                    var end = partitioner.SegP[segIndex + 1];
                    var thisCount = (int)(end - start);

                    if (thisCount > 0)
                    {
                        var run = str.AsSpan((int)start, thisCount);
                        var codePoint = run[0];

                        if ((thisCount == 1) && IsDirectCodepoint(codePoint))
                        {
                            ref var cell = ref GetCell(screenBuffer, cursor.Position);
                            var props = cursor.Props;
                            if (debugHighlighting)
                            {
                                props.Background = 0x00800000;
                            }

                            SetCellDirect(reservedTileTable[codePoint - minDirectCodepoint], props, ref cell);

                            AdvanceColumn(cursor.Position);
                        }
                        else
                        {
                            // TODO(casey): This wastes a lookup on the tile count.
                            // It should save the entry somehow, and roll it into the first cell.

                            var prepped = false;
                            var runHash = ComputeGlyphHash(2 * thisCount, run);
                            var glyphDim = GetGlyphDim(thisCount, run, runHash);
                            for (var tileIndex = 0u;
                                tileIndex < glyphDim.TileCount;
                                ++tileIndex)
                            {
                                ref var cell = ref GetCell(screenBuffer, cursor.Position);
                                var tileHash = ComputeHashForTileIndex(runHash, tileIndex);
                                var state = FindGlyphEntryByHash(glyphTable, tileHash);
                                if (state.FilledState != GlyphEntryState.Rasterized)
                                {
                                    if (!prepped)
                                    {
                                        PrepareTilesForTransfer(thisCount, run, glyphDim);
                                        prepped = true;
                                    }

                                    TransferTile(tileIndex, state.GPUIndex);
                                    UpdateGlyphCacheEntry(state.Entry, GlyphEntryState.Rasterized, state.DimX, state.DimY);
                                }

                                var props = cursor.Props;
                                if (debugHighlighting)
                                {
                                    props.Background = segment ? 0x0008080u : 0x00000080u;
                                    segment = !segment;
                                }

                                SetCellDirect(state.GPUIndex, props, ref cell);

                                AdvanceColumn(cursor.Position);
                            }
                        }
                    }
                }
            }
        }

        private static GlyphHash ComputeHashForTileIndex(GlyphHash tile0Hash, uint tileIndex)
        {
            return new GlyphHash
            {
                Value = HashCode.Combine(tile0Hash.Value, tileIndex)
            };
        }

        private GlyphDim GetGlyphDim(int count, ReadOnlySpan<char> str, GlyphHash runHash)
        {
            /* TODO(casey): Windows can only 2^31 glyph runs - which
               seems fine, but... technically Unicode can have more than two
               billion combining characters, so I guess theoretically this
               code is broken - another "reason" to do a custom glyph rasterizer? */

            var result = new GlyphDim();

            var stringLen = count;
            //Assert(StringLen == Count);

            var size = new SIZE();
            var state = FindGlyphEntryByHash(glyphTable, runHash);
            if (state.FilledState == GlyphEntryState.None)
            {
                if (stringLen > 0)
                {
                    size = DWriteGetTextExtent(str);
                }

                UpdateGlyphCacheEntry(state.Entry, GlyphEntryState.Sized, (uint)size.cx, (uint)size.cy);
            }
            else
            {
                size.cx = state.DimX;
                size.cy = state.DimY;
            }

            result.TileCount = SafeRatio1(size.cx + glyphGen.FontWidth / 2, glyphGen.FontWidth);

            result.XScale = 1.0f;
            if (size.cx > glyphGen.FontWidth)
            {
                result.XScale = SafeRatio1(result.TileCount * glyphGen.FontWidth,
                                           (float)(size.cx));
            }

            result.YScale = 1.0f;
            if (size.cy > glyphGen.FontHeight)
            {
                result.YScale = SafeRatio1(glyphGen.FontHeight, (float)size.cy);
            }

            return result;
        }

        private SIZE DWriteGetTextExtent(ReadOnlySpan<char> str)
        {
            var result = new SIZE();

            using var layout = new TextLayout(glyphGen.DWriteFactory, str.ToString(), glyphGen.TextFormat, glyphGen.TransferWidth, glyphGen.TransferHeight);

            if (layout is not null)
            {
                var Metrics = layout.Metrics;
                //Assert(Metrics.left == 0);
                //Assert(Metrics.top == 0);
                result.cx = (uint)(Metrics.Width + 0.5f);
                result.cy = (uint)(Metrics.Height + 0.5f);
            }

            return result;
        }

        private static GlyphState FindGlyphEntryByHash(GlyphTable glphyTable, GlyphHash runHash)
        {
            if (!glphyTable.Dictionary.TryGetValue(runHash.Value, out var entry))
            {
                entry = new GlyphEntry();
                entry.GPUIndex = glphyTable.PickNextFreeGpuIndex();
                glphyTable.Dictionary[runHash.Value] = entry;
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

        private static void SetCellDirect(GpuGlyphIndex gpuIndex, GlyphProps props, ref RendererCell dest)
        {
            dest.GlyphIndex = gpuIndex.Value;
            var Foreground = props.Foreground;
            var Background = props.Background;
            if (props.Flags.HasFlag(TerminalCellStyle.ReverseVideo))
            {
                Foreground = props.Background;
                Background = props.Foreground;
            }

            if (props.Flags.HasFlag(TerminalCellStyle.Invisible))
            {
                dest.GlyphIndex = 0;
            }

            dest.Foreground = Foreground | ((uint)props.Flags << 24);
            dest.Background = Background;
        }

        private static bool IsDirectCodepoint(char codePoint)
        {
            return codePoint >= minDirectCodepoint &&
                   codePoint < maxDirectCodepoint;
        }

        private ref RendererCell GetCell(TerminalBuffer Buffer, Position Point)
        {
            return ref Buffer.GetCell(Point);
        }

        private void AdvanceColumn(Position Point)
        {
            ++Point.X;
            if (lineWrap &&
                (Point.X >= screenBuffer.DimX))
            {
                AdvanceRow(Point);
            }
        }

        private void AdvanceRow(Position point)
        {
            AdvanceRowNoClear(point);
            ClearLine(screenBuffer, point.Y);
        }

        private void AdvanceRowNoClear(Position point)
        {
            point.X = 0;
            ++point.Y;
            if (point.Y >= screenBuffer.DimY)
            {
                point.Y = 0;
            }
        }

        private static void ClearLine(TerminalBuffer buffer, uint y)
        {
            buffer.ClearLine(y);
        }

        private static SourceBufferRange ReadSourceAt(SourceBuffer buffer, ulong absoluteP, int count)
        {
            var result = new SourceBufferRange();
            if (IsInBuffer(buffer, absoluteP))
            {
                var relativePosition = (int)(absoluteP % (ulong)buffer.DataSize);

                result.AbsoluteP = absoluteP;
                result.Count = (int)Math.Min((ulong)count, buffer.AbsoluteFilledSize - absoluteP);

                var availableInRestBuffer = buffer.DataSize - (uint)relativePosition;

                if (result.Count > availableInRestBuffer)
                {
                    result.Count = (int)availableInRestBuffer;
                    if (result.Count == 0)
                    {
                        buffer.Reset();
                        result.Count = Math.Min(count, buffer.DataSize);
                    }
                }

                result.Data = buffer.Data.Slice(relativePosition, result.Count);
            }

            return result;
        }

        private static bool IsInBuffer(SourceBuffer buffer, ulong absoluteP)
        {
            var backwardOffset = buffer.AbsoluteFilledSize - absoluteP;
            return (absoluteP < buffer.AbsoluteFilledSize) &&
                          (backwardOffset < (ulong)buffer.DataSize);
        }

        private TerminalBuffer AllocateTerminalBuffer(uint dimX, uint dimY)
        {
            var buffer = new TerminalBuffer(this, dimX, dimY);

            shouldLayoutLines = true;

            return buffer;
        }

        private static void DeallocateTerminalBuffer(TerminalBuffer buffer)
        {
            buffer.Dispose();
        }

        private void ProcessMessages()
        {
            while (NativeWindows.PeekMessage(out var Message, new HandleRef(), 0, 0, NativeWindows.PeekMessageParams.PM_REMOVE))
            {
                switch (Message.msg)
                {
                    case NativeWindows.WndMsg.WM_QUIT:
                        {
                            quit = true;
                        }
                        break;

                    case NativeWindows.WndMsg.WM_KEYDOWN:
                        {
                            switch ((NativeWindows.VirtualKeys)Message.wParam)
                            {
                                case NativeWindows.VirtualKeys.Prior:
                                    {
                                        viewingLineOffset -= screenBuffer.DimY / 2;
                                    }
                                    break;

                                case NativeWindows.VirtualKeys.Next:
                                    {
                                        viewingLineOffset += screenBuffer.DimY / 2;
                                    }
                                    break;
                            }

                            if (viewingLineOffset > 0)
                            {
                                viewingLineOffset = 0;
                            }

                            if (viewingLineOffset < -lineCount)
                            {
                                viewingLineOffset = -lineCount;
                            }
                        }
                        break;

                    case NativeWindows.WndMsg.WM_CHAR:
                        {
                            switch ((NativeWindows.VirtualKeys)Message.wParam)
                            {
                                case NativeWindows.VirtualKeys.Back:
                                    {
                                        while ((commandLineCount > 0) &&
                                              IsUTF8Extension(commandLine[commandLineCount - 1]))
                                        {
                                            --commandLineCount;
                                        }

                                        if (commandLineCount > 0)
                                        {
                                            --commandLineCount;
                                        }
                                    }
                                    break;

                                case NativeWindows.VirtualKeys.Return:
                                    {
                                        ExecuteCommandLine();
                                        commandLineCount = 0;
                                        viewingLineOffset = 0;
                                    }
                                    break;

                                default:
                                    {
                                        var Char = (char)Message.wParam;
                                        var Chars = new char[2];
                                        var CharCount = 0;

                                        if (char.IsHighSurrogate(Char))
                                        {
                                            lastChar = Char;
                                        }
                                        else if (char.IsLowSurrogate(Char))
                                        {
                                            if (char.IsSurrogatePair(lastChar, Char))
                                            {
                                                Chars[0] = lastChar;
                                                Chars[1] = Char;
                                                CharCount = 2;
                                            }
                                            lastChar = '\0';
                                        }
                                        else
                                        {
                                            Chars[0] = Char;
                                            CharCount = 1;
                                        }

                                        if (CharCount > 0)
                                        {
                                            var command = Chars.AsSpan().ToString();
                                            var nextSpan = commandLine.AsSpan(commandLineCount);
                                            command.CopyTo(nextSpan);
                                            commandLineCount += command.Length - 1;
                                        }
                                    }
                                    break;
                            }
                        }
                        break;
                }
            }
        }

        private void ExecuteCommandLine()
        {
            // TODO(casey): All of this is complete garbage and should never ever be used.

            commandLine[commandLineCount] = '\0';
            var paramStart = 0;
            while (paramStart <= commandLineCount)
            {
                if (commandLine[paramStart] == ' ') break;
                ++paramStart;
            }

            var parameter = commandLine.AsMemory(paramStart..);
            parameter.Span[0] = '\0';
            if (paramStart < commandLineCount)
            {
                ++paramStart;
                parameter = parameter.Slice(1);
            }

            var command = commandLine.AsSpan(0, paramStart - 1).ToString();

            var ParamRange = new SourceBufferRange();
            ParamRange.Data = parameter;
            ParamRange.Count = commandLineCount - paramStart;

            // TODO(casey): Collapse all these options into a little array, so there's no
            // copy-pasta.
            AppendOutput("\n");

            if (command == "status")
            {
                runningCursor.ClearProps();
                AppendOutput($"RefTerm {Assembly.GetExecutingAssembly().GetName().Version}\n");
                AppendOutput($"Size: {screenBuffer.DimX} x {screenBuffer.DimY}\n");
                //AppendOutput("Fast pipe: %s\n", EnableFastPipe ? "ON" : "off");
                AppendOutput($"Font: {requestedFontName} {requestedFontHeight}\n");
                AppendOutput($"Line Wrap: {(lineWrap ? "ON" : "off")}\n");
                AppendOutput($"Debug: {(debugHighlighting ? "ON" : "off")}\n");
                //AppendOutput("Throttling: %s\n", !NoThrottle ? "ON" : "off");
            }
            //else if (command == "fastpipe")
            //{
            //    Terminal->EnableFastPipe = !Terminal->EnableFastPipe;
            //    AppendOutput(Terminal, "Fast pipe: %s\n", Terminal->EnableFastPipe ? "ON" : "off");
            //}
            else if (command == "linewrap")
            {
                lineWrap = !lineWrap;
                AppendOutput($"LineWrap: {(lineWrap ? "ON" : "off")}\n");
            }
            else if (command == "debug")
            {
                debugHighlighting = !debugHighlighting;
                AppendOutput($"Debug: {(debugHighlighting ? "ON" : "off")}\n");
            }
            //else if (command == "throttle")
            //{
            //    NoThrottle = !NoThrottle;
            //    AppendOutput("Throttling: %s\n", !NoThrottle ? "ON" : "off");
            //}
            else if (command == "font")
            {
                requestedFontName = commandLine.AsSpan(0..paramStart).ToString();

                RefreshFont();
                AppendOutput($"Font: {requestedFontName}\n");
            }
            else if (command == "fontsize")
            {
                requestedFontHeight = (int)ParseNumber(ref ParamRange);
                RefreshFont();
                AppendOutput("Font height: %u\n", requestedFontHeight);
            }
            else if ((command == "kill") ||
                    (command == "break"))
            {
                KillProcess();
            }
            else if ((command == "clear") ||
                    (command == "cls"))
            {
                for (var i = 0; i < lines.Length; i++)
                {
                    var line = lines[i];
                    line.Clear(this);
                }

                ScrollBackBuffer.Clear();
            }
            else if ((command == "exit") ||
                    (command == "quit"))
            {
                KillProcess();
                AppendOutput("Exiting...\n");
                quit = true;
            }
            else if ((command == "echo") ||
                    (command == "print"))
            {
                AppendOutput($"{parameter.Span.ToString()}\n");
            }
            else if (command == "")
            {
            }
            else if (command == "type")
            {
                var param = new string(commandLine.AsSpan(paramStart).ToArray().TakeWhile(x => x != '\0').ToArray());
                if (File.Exists(param))
                {

                    _ = Task.Run(() =>
                    {
                        var stopwatch = new Stopwatch();
                        stopwatch.Start();

                        var fileLength = 0L;
                        using (var stream = File.OpenRead(param))
                        {
                            var buffer = new byte[4 * 1024];
                            var bufferSpan = buffer.AsSpan();
                            int read;
                            Encoding encoding = null;
                            do
                            {
                                read = ReadStreamToOutput(stream, bufferSpan, ref encoding);
                                fileLength += read;
                            } while (read != 0);
                        }

                        stopwatch.Stop();

                        var seconds = stopwatch.ElapsedMilliseconds / 1000d;
                        AppendOutput($"\n\nTotal time: {seconds}s ({(fileLength / (1024d * 1024d * 1024d)) / seconds:0.000}GB/s)");
                    });
                }
                else
                {
                    AppendOutput("File does not exist");
                }
            }
            else
            {
                var extension = "";
                if (!command.EndsWith(".exe") &&
                    !command.EndsWith(".bat") &&
                    !command.EndsWith(".com") &&
                    !command.EndsWith(".cmd"))
                {
                    extension = ".exe";
                }

                var processName = $"{command}{extension}";

                var param = new string(commandLine.AsSpan(paramStart).ToArray().TakeWhile(x => x != '\0').ToArray());
                var processCommandLine = $"{processName} {param}";

                var started = ExecuteSubProcess(processName, param);
                if (!started)
                {
                    processName = "c:\\Windows\\System32\\cmd.exe";
                    processCommandLine = $"cmd.exe /c {processName} {parameter.Span.ToString()}";
                    if (!(started = ExecuteSubProcess(processName, processCommandLine)))
                    {
                        AppendOutput($"ERROR: Unable to execute {commandLine}\n");
                    }
                }

                if (started)
                {
                    AppendOutput($"> {command} {param}{Environment.NewLine}");
                }
            }
        }

        private bool ExecuteSubProcess(string processName, string processCommandLine)
        {
            if (childProcess is not null)
            {
                KillProcess();
            }

            var codePage = CultureInfo.CurrentCulture.TextInfo.OEMCodePage;
            var encoding = Encoding.GetEncoding(codePage);

            var process = new Process();
            var processDir = ".\\";

            process.StartInfo = new ProcessStartInfo
            {
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                StandardOutputEncoding = encoding,
                StandardErrorEncoding = encoding,
                StandardInputEncoding = encoding,
                FileName = processName,
                Arguments = processCommandLine,
                WorkingDirectory = processDir,
                LoadUserProfile = true
            };

            process.EnableRaisingEvents = true;
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

            childProcessCancellationTokenSource = new CancellationTokenSource();
            var syncObj = process.SynchronizingObject;

            _ = Task.Run(async () =>
            {
                if (childProcessCancellationTokenSource is null)
                {
                    return;
                }

                var token = childProcessCancellationTokenSource.Token;
                var buffer = new byte[4 * 1024];

                var stream = childProcess.StandardOutput.BaseStream;
                Encoding encoding = null;
                var fallbackEncoding = childProcess.StandardOutput.CurrentEncoding;

                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        var read = 0;
                        do
                        {
                            read = ReadStreamToOutput(stream, buffer, ref encoding, fallbackEncoding, syncObj);
                        }
                        while (read > 0 &&
                            !token.IsCancellationRequested);
                    }
                    catch (Exception exc)
                    {
                        AppendOutput(exc.Message);
                    }

                    if (token.IsCancellationRequested)
                    {
                        return;
                    }

                    await Task.Delay(1);
                }
            });

            childProcess = process;

            return true;
        }

        private int ReadStreamToOutput(Stream stream, Span<byte> buffer, ref Encoding? encoding, Encoding? fallbackEncoding = null, ISynchronizeInvoke syncObj = null)
        {
            var read = stream
                .Read(buffer);

            if (read > 0)
            {
                if (encoding is null)
                {
                    encoding = TryDetectEncoding(fallbackEncoding, buffer, read);
                }

                if (encoding is not null)
                {
                    var readOnly = (ReadOnlySpan<byte>)buffer.Slice(0, read);

                    var converterBuffer = ArrayPool<char>.Shared.Rent(5 * buffer.Length);
                    var convertedBufferSpan = converterBuffer.AsSpan();

                    var charCount = encoding.GetChars(readOnly, convertedBufferSpan);
                    var convertedSpan = convertedBufferSpan.Slice(0, charCount);

                    if (syncObj?.InvokeRequired == true)
                    {
                        syncObj.Invoke((Action<string>)(o => AppendOutput(o)), new object[] { new string(convertedSpan) });
                    }
                    else
                    {
                        AppendOutput(convertedSpan);
                    }

                    ArrayPool<char>.Shared.Return(converterBuffer);
                }
            }

            return read;
        }

        private static Encoding TryDetectEncoding(Encoding fallbackEncoding, ReadOnlySpan<byte> buffer, int read)
        {
            Encoding detectedEncoding;

            if (read >= 2 && buffer[0] == 255 && buffer[1] == 254)
            {
                detectedEncoding = Encoding.Unicode;
            }
            else if (read >= 3 &&
                buffer[0] == 239 && buffer[1] == 187 && buffer[2] == 191)
            {
                detectedEncoding = Encoding.UTF8;
            }
            else
            {
                detectedEncoding = fallbackEncoding ?? Encoding.GetEncoding(CultureInfo.CurrentCulture.TextInfo.OEMCodePage);
            }

            return detectedEncoding;
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            var lastOutput = (sender as Process).StandardOutput.ReadToEnd();
            if (lastOutput.Length > 0)
            {
                AppendOutput(lastOutput);
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

        private void KillProcess()
        {
            if (childProcess is not null)
            {
                childProcess.Kill();
                CloseProcess();
            }
        }

        private void CloseProcess()
        {
            childProcessCancellationTokenSource?.Cancel();
            childProcessCancellationTokenSource = null;

            if (childProcess is not null)
            {
                childProcess.ErrorDataReceived -= Process_OutputDataReceived;
                childProcess.OutputDataReceived -= Process_OutputDataReceived;
                childProcess.Exited -= Process_Exited;
            }

            childProcess?.Dispose();
            childProcess = null;
        }

        private static bool IsUTF8Extension(char A)
        {
            var Result = ((A & 0xc0) == 0x80);
            return Result;
        }

        private void AppendOutput(string format, params object[] args)
        {
            // TODO(casey): This is all garbage code.  You need a checked printf here, and of
            // course there isn't one of those.  Ideally this would change over to using
            // a real concatenator here, like with a #define system, but this is just
            // a hack for now to do basic printing from the internal code.

            string text;

            if (args.Length == 0)
            {
                text = format;
            }
            else
            {
                try
                {
                    text = string.Format(format, args);
                }
                catch
                {
                    text = format;
                }
            }

            AppendOutput(text.AsSpan());
        }

        private void AppendOutput(ReadOnlySpan<char> data)
        {
            var remaining = data.Length;
            while (remaining > 0)
            {
                var dest = GetNextWritableRange(ScrollBackBuffer, remaining);

                data.Slice(0, dest.Count)
                    .CopyTo(dest.Data.Span);

                CommitWrite(ScrollBackBuffer, dest.Count);
                ParseLines(dest, runningCursor);

                remaining -= dest.Count;
            }

            shouldLayoutLines = true;
        }

        private void ParseLines(SourceBufferRange range, CursorState cursor)
        {
            /* TODO(casey): Currently, if the commit of line data _straddles_ a control code boundary
               this code does not properly _stop_ the processing cursor.  This can cause an edge case
               where a VT code that splits a line _doesn't_ split the line as it should.  To fix this
               the ending code just needs to check to see if the reason it couldn't parse an escape
               code in "AtEscape" was that it ran out of characters, and if so, don't advance the parser
               past that point.
            */

            range = new SourceBufferRange(range);

            var carriage = Vector128.Create((byte)'\n');
            var escape = Vector128.Create((byte)'\x1b');
            var complex = Vector128.Create((byte)128);

            var splitLineAtCount = 4096;
            while (range.Count > 0)
            {
                var count = range.Count;
                if (count > splitLineAtCount)
                {
                    count = splitLineAtCount;
                }

                var containsComplex = Vector128.Create((byte)0);
                var data = range.Data.Span;
                var consumed = 0;
                while (count >= 16)
                {
                    var Batch = MemoryMarshal.Cast<char, Vector128<byte>>(data)[0];

                    var testC = Sse2.CompareEqual(Batch, carriage);
                    var testE = Sse2.CompareEqual(Batch, escape);
                    var testX = Sse2.And(Batch, complex);
                    var test = Sse2.Or(testC, testE);
                    var check = Sse2.MoveMask(test.AsByte());
                    if (check != 0)
                    {
                        var advance = CountTrailingZeroBytes(check);
                        var maskData = OverhangMask.AsSpan(16 - advance);
                        var maskX = MemoryMarshal.Cast<byte, Vector128<byte>>(maskData)[0];

                        testX = Sse2.And(maskX, testX);
                        consumed += advance;
                        break;
                    }

                    containsComplex = Sse2.Or(containsComplex, testX.AsByte());
                    count -= 16;
                    data = data.Slice(16);
                    consumed += 16;
                }

                ConsumeCount(range, consumed);

                lines[currentLineIndex].ContainsComplexChars |= complex.ToScalar() > 0;

                if (AtEscape(range.Data.Span))
                {
                    var feedAt = range.AbsoluteP;
                    if (ParseEscape(ref range, cursor))
                    {
                        LineFeed(feedAt, feedAt, cursor.Props);
                    }
                }
                else
                {
                    var token = GetToken(ref range);
                    if (token == '\n')
                    {
                        LineFeed(range.AbsoluteP, range.AbsoluteP, cursor.Props);
                    }
                    else if (token < 0) // TODO(casey): Not sure what is a "combining char" here, really, but this is a rough test
                    {
                        lines[currentLineIndex].ContainsComplexChars = true;
                    }
                }

                UpdateLineEnd(range.AbsoluteP);
                if (GetLineLength(lines[currentLineIndex]) > (ulong)splitLineAtCount)
                {
                    LineFeed(range.AbsoluteP, range.AbsoluteP, cursor.Props);
                }
            }
        }

        private int CountTrailingZeroBytes(int value)
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

        private static ulong GetLineLength(Line line)
        {
            //Assert(Line->OnePastLastP >= Line->FirstP);
            return line.OnePastLastP - line.FirstP;
        }

        private void LineFeed(ulong atP, ulong nextLineStart, GlyphProps atProps)
        {
            UpdateLineEnd(atP);

            ++currentLineIndex;
            if (currentLineIndex >= maxLineCount)
            {
                currentLineIndex = 0;
            }

            var line = lines[currentLineIndex];
            line.FirstP = nextLineStart;
            line.OnePastLastP = nextLineStart;
            line.ContainsComplexChars = false;
            line.StartingProps = atProps;

            if (lineCount <= currentLineIndex)
            {
                lineCount = currentLineIndex + 1;
            }
        }

        private void UpdateLineEnd(ulong toP)
        {
            lines[currentLineIndex].OnePastLastP = toP;
        }

        private static bool ParseEscape(ref SourceBufferRange range, CursorState cursor)
        {
            var movedCursor = false;

            GetToken(ref range);
            GetToken(ref range);

            var command = (char)0;
            uint paramCount = 0;
            var parameters = new uint[8];

            while ((paramCount < parameters.Length) && range.Count > 0)
            {
                var token = PeekToken(range.Data.Span, 0);
                if (IsDigit(token))
                {
                    parameters[paramCount++] = ParseNumber(ref range);
                    var Semi = GetToken(ref range);
                    if (Semi != ';')
                    {
                        command = Semi;
                        break;
                    }
                }
                else
                {
                    command = GetToken(ref range);
                }
            }

            switch (command)
            {
                case 'H':
                    {
                        // NOTE(casey): Move cursor to X,Y position
                        cursor.Position.X = parameters[1] - 1;
                        cursor.Position.Y = parameters[0] - 1;
                        movedCursor = true;
                    }
                    break;

                case 'm':
                    {
                        // NOTE(casey): Set graphics mode
                        if (parameters[0] == 0)
                        {
                            cursor.ClearProps();
                        }

                        if (parameters[0] == 1) cursor.Props.Flags |= TerminalCellStyle.Bold;
                        if (parameters[0] == 2) cursor.Props.Flags |= TerminalCellStyle.Dim;
                        if (parameters[0] == 3) cursor.Props.Flags |= TerminalCellStyle.Italic;
                        if (parameters[0] == 4) cursor.Props.Flags |= TerminalCellStyle.Underline;
                        if (parameters[0] == 5) cursor.Props.Flags |= TerminalCellStyle.Blinking;
                        if (parameters[0] == 7) cursor.Props.Flags |= TerminalCellStyle.ReverseVideo;
                        if (parameters[0] == 8) cursor.Props.Flags |= TerminalCellStyle.Invisible;
                        if (parameters[0] == 9) cursor.Props.Flags |= TerminalCellStyle.Strikethrough;

                        if ((parameters[0] == 38) && (parameters[1] == 2)) cursor.Props.Foreground = PackRGB(parameters[2], parameters[3], parameters[4]);
                        if ((parameters[0] == 48) && (parameters[1] == 2)) cursor.Props.Background = PackRGB(parameters[2], parameters[3], parameters[4]);
                    }
                    break;
            }

            return movedCursor;
        }

        private static uint PackRGB(uint r, uint g, uint b)
        {
            if (r > 255) r = 255;
            if (g > 255) g = 255;
            if (b > 255) b = 255;

            return ((b << 16) | (g << 8) | (r << 0));
        }

        private static uint ParseNumber(ref SourceBufferRange range)
        {
            uint number = 0;
            while (IsDigit(PeekToken(range.Data.Span, 0)))
            {
                var Token = GetToken(ref range);
                number = (uint)(10 * number + (Token - '0'));
            }
            return number;
        }

        static bool IsDigit(char digit)
        {
            return (digit >= '0') && (digit <= '9');
        }

        private static char NullToken = '\0';
        private static ref char GetToken(ref SourceBufferRange range)
        {
            ref var result = ref NullToken;

            if (range.Count > 0)
            {
                result = ref range.Data.Span[0];
                ConsumeCount(range, 1);
            }

            return ref result;
        }

        private static bool AtEscape(Span<char> span)
        {
            return PeekToken(span, 0) == '\x1b' &&
                   PeekToken(span, 1) == '[';
        }

        private static char PeekToken(Span<char> span, int ordinal)
        {
            var Result = (char)0;

            if (ordinal < span.Length)
            {
                Result = span[ordinal];
            }

            return Result;
        }

        private static void ConsumeCount(SourceBufferRange source, int count)
        {
            if (count > source.Count)
            {
                count = source.Count;
            }

            source.Data = source.Data.Slice(count);
            source.AbsoluteP += (ulong)count;
            source.Count -= count;
        }

        private void CommitWrite(SourceBuffer buffer, int size)
        {
            //Assert(Buffer->RelativePoint < Buffer->DataSize);
            //Assert(Size <= Buffer->DataSize);

            buffer.RelativePoint += size;
            buffer.AbsoluteFilledSize += (ulong)size;

            var WrappedRelative = buffer.RelativePoint - buffer.DataSize;
            buffer.RelativePoint = (buffer.RelativePoint >= buffer.DataSize) ? WrappedRelative : buffer.RelativePoint;

            //Assert(Buffer.RelativePoint < Buffer.DataSize);
        }

        private static SourceBufferRange GetNextWritableRange(SourceBuffer buffer, int maxCount)
        {
            //Assert(Buffer->RelativePoint < Buffer->DataSize);

            var range = new SourceBufferRange();
            range.AbsoluteP = buffer.AbsoluteFilledSize;
            range.Count = Math.Min(maxCount, buffer.DataSize - buffer.RelativePoint);

            if (range.Count <= 0)
            {
                buffer.Reset();
                range.Count = Math.Min(maxCount, buffer.Data.Length);
            }

            range.Data = buffer.Data.Slice(buffer.RelativePoint, range.Count);

            return range;
        }

        private bool RefreshFont()
        {
            var refreshed = false;

            var parameters = new GlyphTableParams();

            parameters.ReservedTileCount = ArrayCount(reservedTileTable) + 1;

            for (var attempt = 0; attempt <= 1; ++attempt)
            {
                refreshed = SetFont(requestedFontName, (uint)requestedFontHeight);
                if (refreshed)
                {
                    parameters.CacheTileCountInX = SafeRatio1((uint)textureWidth, glyphGen.FontWidth);
                    parameters.EntryCount = GetExpectedTileCountForDimension((uint)textureWidth, (uint)textureHeight);
                    parameters.HashCount = 4096;

                    if (parameters.EntryCount > parameters.ReservedTileCount)
                    {
                        parameters.EntryCount -= parameters.ReservedTileCount;
                        break;
                    }
                }

                RevertToDefaultFont();
            }

            glyphTable = PlaceGlyphTableInMemory(parameters);

            InitializeDirectGlyphTable(parameters, reservedTileTable, true);

            var unitDim = GetSingleTileUnitDim();

            for (var tileIndex = 0u;
                tileIndex < ArrayCount(reservedTileTable);
                ++tileIndex)
            {
                var letter = (char)(minDirectCodepoint + tileIndex);
                PrepareTilesForTransfer(1, letter.ToString(), unitDim);
                TransferTile(0, reservedTileTable[tileIndex]);
            }

            // NOTE(casey): Clear the reserved 0 tile
            var nothing = "\0";
            var zeroTile = new GpuGlyphIndex();
            PrepareTilesForTransfer(0, nothing, unitDim);
            TransferTile(0, zeroTile);

            return refreshed;
        }

        private static GpuGlyphIndex PackGlyphCachePoint(uint x, uint y)
        {
            return new GpuGlyphIndex { Value = (y << 16) | x };
        }

        private GlyphTable PlaceGlyphTableInMemory(GlyphTableParams parameters)
        {
            //Assert(Params.HashCount >= 1);
            //Assert(Params.EntryCount >= 2);
            //Assert(IsPowerOfTwo(Params.HashCount));
            //Assert(Params.CacheTileCountInX >= 1);

            var table = new GlyphTable();
            table.Params = parameters;
            table.Entries = new GlyphEntry[parameters.EntryCount];
            table.EntryCount = (uint)table.Entries.Length;
            for (var i = 0; i < table.EntryCount; i++)
            {
                table.Entries[i] = new GlyphEntry();
            }

            var startingTile = parameters.ReservedTileCount;
            var x = startingTile % parameters.CacheTileCountInX;
            var y = startingTile / parameters.CacheTileCountInX;
            for (var entryIndex = 0u;
                    entryIndex < parameters.EntryCount;
                    ++entryIndex)
            {
                if (x >= parameters.CacheTileCountInX)
                {
                    x = 0;
                    ++y;
                }

                var entry = GetEntry(table, entryIndex);

                entry.GPUIndex = PackGlyphCachePoint(x, y);

                entry.FilledState = GlyphEntryState.None;
                entry.DimX = 0;
                entry.DimY = 0;
                entry.Used = false;
                ++x;
            }

            return table;
        }

        private void InitializeDirectGlyphTable(GlyphTableParams parameters, GpuGlyphIndex[] reservedTable, bool skipZeroSlot)
        {
            //Assert(Params.CacheTileCountInX >= 1);

            var skipAmount = skipZeroSlot ? 1u : 0;

            var x = skipAmount;
            var y = 0u;

            for (var entryIndex = 0;
                entryIndex < (parameters.ReservedTileCount - skipAmount);
                ++entryIndex)
            {
                if (x >= parameters.CacheTileCountInX)
                {
                    x = 0;
                    ++y;
                }

                reservedTable[entryIndex] = PackGlyphCachePoint(x, y);
                glyphTable.Entries[entryIndex].Used = true;

                ++x;
            }
        }

        private uint GetExpectedTileCountForDimension(uint width, uint height)
        {
            var perRow = SafeRatio1(width, glyphGen.FontWidth);
            var perColumn = SafeRatio1(height, glyphGen.FontHeight);

            return perRow * perColumn;
        }

        private uint SafeRatio1(uint a, uint b)
        {
            return b != 0 ? a / b : b;
        }

        private float SafeRatio1(float a, float b)
        {
            return b != 0 ? a / b : b;
        }

        private bool SetFont(string fontName, uint fontHeight)
        {
            var Result = DWriteSetFont(fontName, fontHeight);
            return Result;
        }

        private bool DWriteSetFont(string FontName, uint FontHeight)
        {
            var Result = false;

            if (glyphGen.DWriteFactory is not null)
            {
                var textFormat = new TextFormat(
                    glyphGen.DWriteFactory,
                    FontName,
                    FontWeight.Regular,
                    FontStyle.Normal,
                    FontStretch.Normal,
                    FontHeight
                    );

                glyphGen.TextFormat = textFormat;

                if (glyphGen.TextFormat is not null)
                {
                    glyphGen.TextFormat.ParagraphAlignment = ParagraphAlignment.Near;
                    glyphGen.TextFormat.TextAlignment = TextAlignment.Leading;

                    glyphGen.FontWidth = 0;
                    glyphGen.FontHeight = 0;
                    IncludeLetterBounds("M");
                    IncludeLetterBounds("g");

                    Result = true;
                }
            }

            return Result;
        }

        private void IncludeLetterBounds(string letter)
        {
            using var textLayout = new TextLayout(
                glyphGen.DWriteFactory, letter, glyphGen.TextFormat, glyphGen.TransferWidth, glyphGen.TransferHeight);

            if (textLayout is not null)
            {
                // TODO(casey): Real cell size determination would go here - probably with input from the user?

                var charMetrics = textLayout.Metrics; // TODO: or just "Metrics"?
                var lineMetrics = textLayout.GetLineMetrics();

                if (glyphGen.FontHeight < (uint)(lineMetrics[0].Height + 0.5f))
                {
                    glyphGen.FontHeight = (uint)(lineMetrics[0].Height + 0.5f);
                }

                if (glyphGen.FontHeight < (uint)(charMetrics.Height + 0.5f))
                {
                    glyphGen.FontHeight = (uint)(charMetrics.Height + 0.5f);
                }

                if (glyphGen.FontWidth < (uint)(charMetrics.Width + 0.5f))
                {
                    glyphGen.FontWidth = (uint)(charMetrics.Width + 0.5f);
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
            requestedFontName = "Cascadia Mono";
            requestedFontHeight = 17;
        }

        private SourceBuffer AllocateSourceBuffer(int dataSize)
        {
            var info = new NativeWindows.SYSTEM_INFO();
            NativeWindows.GetSystemInfo(ref info);

            dataSize = (int)((dataSize + info.dwAllocationGranularity - 1) & ~(info.dwAllocationGranularity - 1));

            var result = new SourceBuffer(dataSize);

            return result;
        }

        private GlyphGenerator AllocateGlyphGenerator(uint transferWidth, uint transferHeight)
        {
            var glyphGen = new GlyphGenerator();

            glyphGen.TransferWidth = transferWidth;
            glyphGen.TransferHeight = transferHeight;

            DWriteInit(glyphGen);

            return glyphGen;
        }

        private void DWriteInit(GlyphGenerator glyphGen)
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
            renderer.ComputeShader = new ComputeShader(renderer.Device, computeShaderBytes);
            renderer.ComputeShader.DebugName = "ComputeShader";
            renderer.PixelShader = new PixelShader(renderer.Device, pixelShaderBytes);
            renderer.PixelShader.DebugName = "PixelShader";
            renderer.VertexShader = new VertexShader(renderer.Device, vertexShaderBytes);
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
