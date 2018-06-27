///////////////////////////////////////////////////////////////////////////////
// INFO                                                                      //
///////////////////////////////////////////////////////////////////////////////

// NanoJPEG -- Unmanaged C# Port
// version 1.0.0 (10-Aug-2015)
// by Johannes Bildstein <info@fotostein.at>
// original license below

///////////////////////////////////////////////////////////////////////////////
// START ORIGINAL LICENSE SECTION                                            //
///////////////////////////////////////////////////////////////////////////////

// NanoJPEG -- KeyJ's Tiny Baseline JPEG Decoder
// version 1.3.2 (2014-02-02)
// by Martin J. Fiedler <martin.fiedler@gmx.net>
//
// This software is published under the terms of KeyJ's Research License,
// version 0.2. Usage of this software is subject to the following conditions:
// 0. There's no warranty whatsoever. The author(s) of this software can not
//    be held liable for any damages that occur when using this software.
// 1. This software may be used freely for both non-commercial and commercial
//    purposes.
// 2. This software may be redistributed freely as long as no fees are charged
//    for the distribution and this license information is included.
// 3. This software may be modified freely except for this license information,
//    which must not be changed in any way.
// 4. If anything other than configuration, indentation or comments have been
//    altered in the code, the original author(s) must receive a copy of the
//    modified code.

///////////////////////////////////////////////////////////////////////////////
// END ORIGINAL LICENSE SECTION                                              //
///////////////////////////////////////////////////////////////////////////////

using System;
using System.IO;
using System.Runtime.InteropServices;

namespace NanoJpeg
{
    /// <summary>
    /// Error codes for decoding errors
    /// </summary>
    public enum NJErrorCode
    {
        /// <summary>
        /// Not a JPEG file
        /// </summary>
        NoJpeg,
        /// <summary>
        /// Unsupported format
        /// </summary>
        Unsupported,
        /// <summary>
        /// Internal error
        /// </summary>
        InternalError,
        /// <summary>
        /// Syntax error
        /// </summary>
        SyntaxError,
    }

    /// <summary>
    /// Exception for decoding errors
    /// </summary>
    public class NJException : Exception
    {
        /// <summary>
        /// The error code of this exception
        /// </summary>
        public NJErrorCode ErrorCode
        {
            get { return _ErrorCode; }
        }
        private NJErrorCode _ErrorCode = NJErrorCode.InternalError;

        /// <summary>
        /// Creates a new instance of the <see cref="NJException"/> class
        /// </summary>
        /// <param name="ErrorCode">The error code of this exception</param>
        public NJException(NJErrorCode ErrorCode)
            : base(ErrorCode.ToString())
        {
            _ErrorCode = ErrorCode;
        }
    }

    /// <summary>
    /// Provides methods to decode a Jpeg image.
    /// <para>NOT thread safe</para>
    /// </summary>
    public unsafe sealed class NJImage : IDisposable
    {
        #region Variables/Constants

        private bool IsDisposed;
        private bool IsDone;

        private byte* pos;
        private int size;
        private int length;
        private int width, height;
        private int mbwidth, mbheight;
        private int mbsizex, mbsizey;
        private int ncomp;
        private Component* comp;
        private int qtused, qtavail;
        private byte** qtab;
        private VLCCode** vlctab;
        private int buf, bufbits;
        private int* block;
        private int rstinterval;
        private byte* rgb;
        
        const int W1 = 2841;
        const int W2 = 2676;
        const int W3 = 2408;
        const int W5 = 1609;
        const int W6 = 1108;
        const int W7 = 565;

        const int CF4A = -9;
        const int CF4B = 111;
        const int CF4C = 29;
        const int CF4D = -3;
        const int CF3A = 28;
        const int CF3B = 109;
        const int CF3C = -9;
        const int CF3X = 104;
        const int CF3Y = 27;
        const int CF3Z = -3;
        const int CF2A = 139;
        const int CF2B = -11;

        static byte[] njZZ = { 0, 1, 8, 16, 9, 2, 3, 10, 17, 24, 32, 25, 18,
11, 4, 5, 12, 19, 26, 33, 40, 48, 41, 34, 27, 20, 13, 6, 7, 14, 21, 28, 35,
42, 49, 56, 57, 50, 43, 36, 29, 22, 15, 23, 30, 37, 44, 51, 58, 59, 52, 45,
38, 31, 39, 46, 53, 60, 61, 54, 47, 55, 62, 63 };

        #endregion

        #region Properties

        /// <summary>
        /// The width of the last decoded image
        /// </summary>
        public int Width
        {
            get { return width; }
        }
        /// <summary>
        /// The height of the last decoded image
        /// </summary>
        public int Height
        {
            get { return height; }
        }
        /// <summary>
        /// States if the last decoded image is a color (RGB) or grayscale image
        /// </summary>
        public bool IsColor
        {
            get { return ncomp != 1; }
        }
        /// <summary>
        /// Pointer to the pixels
        /// </summary>
        public byte* Image
        {
            get
            {
                if (ncomp == 1) return comp[0].pixels;
                else return rgb;
            }
        }
        /// <summary>
        /// Size of the image in bytes
        /// </summary>
        public int ImageSize
        {
            get { return width * height * ncomp; }
        }

        #endregion

        #region Helper Structs

        private struct VLCCode
        {
            public byte bits;
            public byte code;
        }

        private struct Component
        {
            public int cid;
            public int ssx;
            public int ssy;
            public int width;
            public int height;
            public int stride;
            public int qtsel;
            public int actabsel;
            public int dctabsel;
            public int dcpred;
            public byte* pixels;
        }

        #endregion

        #region Init/Dispose

        /// <summary>
        /// Creates a new instance of the <see cref="NJImage"/> class
        /// </summary>
        public NJImage()
        {
            comp = (Component*)Marshal.AllocHGlobal(3 * Marshal.SizeOf(typeof(Component)));
            block = (int*)Marshal.AllocHGlobal(64 * Marshal.SizeOf(typeof(int)));

            FillMem(comp, new Component(), 3);

            qtab = (byte**)Marshal.AllocHGlobal(4 * IntPtr.Size);
            vlctab = (VLCCode**)Marshal.AllocHGlobal(4 * IntPtr.Size);
            for (int i = 0; i < 4; i++)
            {
                qtab[i] = (byte*)Marshal.AllocHGlobal(64 * Marshal.SizeOf(typeof(byte)));
                vlctab[i] = (VLCCode*)Marshal.AllocHGlobal(65536 * Marshal.SizeOf(typeof(VLCCode)));

                FillMem((long*)qtab[i], 0, 64 / 8);//use long instead of byte
                FillMem((long*)vlctab[i], 0, 65536 / 4);//use long instead of VLCCode (length=2)
            }
        }

        /// <summary>
        /// Finalizer of the <see cref="NJImage"/> class
        /// </summary>
        ~NJImage()
        {
            Dispose(false);
        }

        /// <summary>
        /// Releases all allocated resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool managed)
        {
            if (!IsDisposed)
            {
                if (rgb != null)
                {
                    Marshal.FreeHGlobal((IntPtr)rgb);
                    rgb = null;
                }

                for (int i = 0; i < 4; i++)
                {
                    if (qtab[i] != null) Marshal.FreeHGlobal((IntPtr)qtab[i]);
                    if (vlctab[i] != null) Marshal.FreeHGlobal((IntPtr)vlctab[i]);
                }

                if (qtab != null) Marshal.FreeHGlobal((IntPtr)qtab);
                if (vlctab != null) Marshal.FreeHGlobal((IntPtr)vlctab);
                if (comp != null) Marshal.FreeHGlobal((IntPtr)comp);
                if (block != null) Marshal.FreeHGlobal((IntPtr)block);

                IsDisposed = true;
            }
        }

        #endregion
        
        #region Public Methods

        /// <summary>
        /// Decodes a jpeg image
        /// </summary>
        /// <param name="jpeg">Path to the image file</param>
        public void Decode(string jpeg)
        {
            byte[] data = File.ReadAllBytes(jpeg);
            fixed (byte* ptr = data)
            {
                Decode(ptr, data.Length, false);
            }
        }

        /// <summary>
        /// Decodes a jpeg image
        /// </summary>
        /// <param name="jpeg">The stream that contains the jpeg data</param>
        public void Decode(Stream jpeg)
        {
            byte[] data = new byte[jpeg.Length];
            jpeg.Read(data, 0, (int)jpeg.Length);

            fixed (byte* ptr = data)
            {
                Decode(ptr, data.Length, false);
            }
        }

        /// <summary>
        /// Decodes a jpeg image
        /// </summary>
        /// <param name="jpeg">The stream that contains the jpeg data</param>
        public void Decode(MemoryStream jpeg)
        {
            fixed (byte* ptr = jpeg.GetBuffer())
            {
                Decode(ptr, (int)jpeg.Length, false);
            }
        }

        /// <summary>
        /// Decodes a jpeg image
        /// </summary>
        /// <param name="jpeg">The stream that contains the jpeg data</param>
        public void Decode(UnmanagedMemoryStream jpeg)
        {
            Decode(jpeg.PositionPointer, (int)jpeg.Length, false);
        }

        /// <summary>
        /// Decodes a jpeg image
        /// </summary>
        /// <param name="jpeg"></param>
        public void Decode(byte[] jpeg)
        {
            fixed(byte* ptr = jpeg)
            {
                Decode(ptr, jpeg.Length, false);
            }
        }

        /// <summary>
        /// Decodes a jpeg image
        /// </summary>
        /// <param name="jpeg">Pointer to the jpeg data</param>
        /// <param name="size">Size of the jpeg data in bytes</param>
        public void Decode(byte* jpeg, int size)
        {
            Decode(jpeg, size, false);
        }

        /// <summary>
        /// Decodes a jpeg image
        /// </summary>
        /// <param name="jpeg">Pointer to the jpeg data</param>
        /// <param name="size">Size of the jpeg data in bytes</param>
        /// <param name="flip">True to flip the Red and Blue channel. This is useful for System.Drawing (GDI+) classes.</param>
        public void Decode(byte* jpeg, int size, bool flip)
        {
            Init();

            this.pos = jpeg;
            this.size = size & 0x7FFFFFFF;
            if (this.size < 2) throw new NJException(NJErrorCode.NoJpeg);
            if (((this.pos[0] ^ 0xFF) | (this.pos[1] ^ 0xD8)) != 0) throw new NJException(NJErrorCode.NoJpeg);
            Skip(2);
            while (!IsDone)
            {
                if ((this.size < 2) || (this.pos[0] != 0xFF)) throw new NJException(NJErrorCode.SyntaxError);
                Skip(2);
                switch (this.pos[-1])
                {
                    case 0xC0: DecodeSOF(); break;
                    case 0xC4: DecodeDHT(); break;
                    case 0xDB: DecodeDQT(); break;
                    case 0xDD: DecodeDRI(); break;
                    case 0xDA: DecodeScan(); break;
                    case 0xFE: SkipMarker(); break;
                    default:
                        if ((this.pos[-1] & 0xF0) == 0xE0) SkipMarker();
                        else throw new NJException(NJErrorCode.Unsupported);
                        break;
                }
            }
            ConvertYCC(flip);
        }
        
        #endregion
        
        #region Private Methods

        private void Init()
        {
            if (rgb != null)
            {
                Marshal.FreeHGlobal((IntPtr)rgb);
                rgb = null;
            }

            FillMem(comp, new Component(), 3);
            IsDone = false;
            bufbits = 0;
        }

        private void RowIDCT(int* blk)
        {
            int x0, x1, x2, x3, x4, x5, x6, x7, x8;
            if (((x1 = blk[4] << 11)
                | (x2 = blk[6])
                | (x3 = blk[2])
                | (x4 = blk[1])
                | (x5 = blk[7])
                | (x6 = blk[5])
                | (x7 = blk[3])) == 0)
            {
                blk[0] = blk[1] = blk[2] = blk[3] = blk[4] = blk[5] = blk[6] = blk[7] = blk[0] << 3;
                return;
            }
            x0 = (blk[0] << 11) + 128;
            x8 = W7 * (x4 + x5);
            x4 = x8 + (W1 - W7) * x4;
            x5 = x8 - (W1 + W7) * x5;
            x8 = W3 * (x6 + x7);
            x6 = x8 - (W3 - W5) * x6;
            x7 = x8 - (W3 + W5) * x7;
            x8 = x0 + x1;
            x0 -= x1;
            x1 = W6 * (x3 + x2);
            x2 = x1 - (W2 + W6) * x2;
            x3 = x1 + (W2 - W6) * x3;
            x1 = x4 + x6;
            x4 -= x6;
            x6 = x5 + x7;
            x5 -= x7;
            x7 = x8 + x3;
            x8 -= x3;
            x3 = x0 + x2;
            x0 -= x2;
            x2 = (181 * (x4 + x5) + 128) >> 8;
            x4 = (181 * (x4 - x5) + 128) >> 8;
            blk[0] = (x7 + x1) >> 8;
            blk[1] = (x3 + x2) >> 8;
            blk[2] = (x0 + x4) >> 8;
            blk[3] = (x8 + x6) >> 8;
            blk[4] = (x8 - x6) >> 8;
            blk[5] = (x0 - x4) >> 8;
            blk[6] = (x3 - x2) >> 8;
            blk[7] = (x7 - x1) >> 8;
        }

        private void ColIDCT(int* blk, byte* outv, int stride)
        {
            int x0, x1, x2, x3, x4, x5, x6, x7, x8;
            if (((x1 = blk[8 * 4] << 8)
                | (x2 = blk[8 * 6])
                | (x3 = blk[8 * 2])
                | (x4 = blk[8 * 1])
                | (x5 = blk[8 * 7])
                | (x6 = blk[8 * 5])
                | (x7 = blk[8 * 3])) == 0)
            {
                x1 = Clip(((blk[0] + 32) >> 6) + 128);
                for (x0 = 8; x0 != 0; --x0)
                {
                    *outv = (byte)x1;
                    outv += stride;
                }
                return;
            }
            x0 = (blk[0] << 8) + 8192;
            x8 = W7 * (x4 + x5) + 4;
            x4 = (x8 + (W1 - W7) * x4) >> 3;
            x5 = (x8 - (W1 + W7) * x5) >> 3;
            x8 = W3 * (x6 + x7) + 4;
            x6 = (x8 - (W3 - W5) * x6) >> 3;
            x7 = (x8 - (W3 + W5) * x7) >> 3;
            x8 = x0 + x1;
            x0 -= x1;
            x1 = W6 * (x3 + x2) + 4;
            x2 = (x1 - (W2 + W6) * x2) >> 3;
            x3 = (x1 + (W2 - W6) * x3) >> 3;
            x1 = x4 + x6;
            x4 -= x6;
            x6 = x5 + x7;
            x5 -= x7;
            x7 = x8 + x3;
            x8 -= x3;
            x3 = x0 + x2;
            x0 -= x2;
            x2 = (181 * (x4 + x5) + 128) >> 8;
            x4 = (181 * (x4 - x5) + 128) >> 8;
            *outv = Clip(((x7 + x1) >> 14) + 128); outv += stride;
            *outv = Clip(((x3 + x2) >> 14) + 128); outv += stride;
            *outv = Clip(((x0 + x4) >> 14) + 128); outv += stride;
            *outv = Clip(((x8 + x6) >> 14) + 128); outv += stride;
            *outv = Clip(((x8 - x6) >> 14) + 128); outv += stride;
            *outv = Clip(((x0 - x4) >> 14) + 128); outv += stride;
            *outv = Clip(((x3 - x2) >> 14) + 128); outv += stride;
            *outv = Clip(((x7 - x1) >> 14) + 128);
        }

        private int ShowBits(int bits)
        {
            byte newbyte;
            if (bits == 0) return 0;
            while (this.bufbits < bits)
            {
                if (this.size <= 0)
                {
                    this.buf = (this.buf << 8) | 0xFF;
                    this.bufbits += 8;
                    continue;
                }
                newbyte = *this.pos++;
                this.size--;
                this.bufbits += 8;
                this.buf = (this.buf << 8) | newbyte;
                if (newbyte == 0xFF)
                {
                    if (this.size != 0)
                    {
                        byte marker = *this.pos++;
                        this.size--;
                        switch (marker)
                        {
                            case 0x00:
                            case 0xFF:
                                break;
                            case 0xD9: this.size = 0; break;
                            default:
                                if ((marker & 0xF8) != 0xD0) throw new NJException(NJErrorCode.SyntaxError);
                                else
                                {
                                    this.buf = (this.buf << 8) | marker;
                                    this.bufbits += 8;
                                }
                                break;
                        }
                    }
                    else throw new NJException(NJErrorCode.SyntaxError);
                }
            }
            return (this.buf >> (this.bufbits - bits)) & ((1 << bits) - 1);
        }

        private void DecodeSOF()
        {
            int i, ssxmax = 0, ssymax = 0;
            Component* c;
            DecodeLength();
            if (this.length < 9) throw new NJException(NJErrorCode.SyntaxError);
            if (this.pos[0] != 8) throw new NJException(NJErrorCode.Unsupported);
            this.height = Decode16(this.pos + 1);
            this.width = Decode16(this.pos + 3);
            this.ncomp = this.pos[5];
            Skip(6);
            switch (this.ncomp)
            {
                case 1:
                case 3:
                    break;
                default:
                    throw new NJException(NJErrorCode.Unsupported);
            }
            if (this.length < (this.ncomp * 3)) throw new NJException(NJErrorCode.SyntaxError);
            for (i = 0, c = this.comp; i < this.ncomp; ++i, ++c)
            {
                c->cid = this.pos[0];
                if ((c->ssx = this.pos[1] >> 4) == 0) throw new NJException(NJErrorCode.SyntaxError);
                if ((c->ssx & (c->ssx - 1)) != 0) throw new NJException(NJErrorCode.Unsupported);  // non-power of two
                if ((c->ssy = this.pos[1] & 15) == 0) throw new NJException(NJErrorCode.SyntaxError);
                if ((c->ssy & (c->ssy - 1)) != 0) throw new NJException(NJErrorCode.Unsupported);  // non-power of two
                if (((c->qtsel = this.pos[2]) & 0xFC) != 0) throw new NJException(NJErrorCode.SyntaxError);
                Skip(3);
                this.qtused |= 1 << c->qtsel;
                if (c->ssx > ssxmax) ssxmax = c->ssx;
                if (c->ssy > ssymax) ssymax = c->ssy;
            }
            if (this.ncomp == 1)
            {
                c = this.comp;
                c->ssx = c->ssy = ssxmax = ssymax = 1;
            }
            this.mbsizex = ssxmax << 3;
            this.mbsizey = ssymax << 3;
            this.mbwidth = (this.width + this.mbsizex - 1) / this.mbsizex;
            this.mbheight = (this.height + this.mbsizey - 1) / this.mbsizey;
            for (i = 0, c = this.comp; i < this.ncomp; ++i, ++c)
            {
                c->width = (this.width * c->ssx + ssxmax - 1) / ssxmax;
                c->height = (this.height * c->ssy + ssymax - 1) / ssymax;
                c->stride = this.mbwidth * c->ssx << 3;
                if (((c->width < 3) && (c->ssx != ssxmax)) || ((c->height < 3) && (c->ssy != ssymax))) throw new NJException(NJErrorCode.Unsupported);
                c->pixels = (byte*)Marshal.AllocHGlobal(c->stride * this.mbheight * c->ssy << 3);
            }
            if (this.ncomp == 3) this.rgb = (byte*)Marshal.AllocHGlobal(this.width * this.height * this.ncomp);
            Skip(this.length);
        }

        private void DecodeDHT()
        {
            int codelen, currcnt, remain, spread, i, j;
            VLCCode* vlc;
            byte* counts = stackalloc byte[16];
            DecodeLength();
            while (this.length >= 17)
            {
                i = this.pos[0];
                if ((i & 0xEC) != 0) throw new NJException(NJErrorCode.SyntaxError);
                if ((i & 0x02) != 0) throw new NJException(NJErrorCode.Unsupported);
                i = (i | (i >> 3)) & 3;  // combined DC/AC + tableid value
                for (codelen = 1; codelen <= 16; ++codelen)
                    counts[codelen - 1] = this.pos[codelen];
                Skip(17);
                vlc = &this.vlctab[i][0];
                remain = spread = 65536;
                for (codelen = 1; codelen <= 16; ++codelen)
                {
                    spread >>= 1;
                    currcnt = counts[codelen - 1];
                    if (currcnt == 0) continue;
                    if (this.length < currcnt) throw new NJException(NJErrorCode.SyntaxError);
                    remain -= currcnt << (16 - codelen);
                    if (remain < 0) throw new NJException(NJErrorCode.SyntaxError);
                    for (i = 0; i < currcnt; ++i)
                    {
                        byte code = this.pos[i];
                        for (j = spread; j != 0; --j)
                        {
                            vlc->bits = (byte)codelen;
                            vlc->code = code;
                            ++vlc;
                        }
                    }
                    Skip(currcnt);
                }
                while (remain-- != 0)
                {
                    vlc->bits = 0;
                    ++vlc;
                }
            }
            if (this.length != 0) throw new NJException(NJErrorCode.SyntaxError);
        }

        private void DecodeDQT()
        {
            int i;
            byte* t;
            DecodeLength();
            while (this.length >= 65)
            {
                i = this.pos[0];
                if ((i & 0xFC) != 0) throw new NJException(NJErrorCode.SyntaxError);
                this.qtavail |= 1 << i;
                t = &this.qtab[i][0];
                for (i = 0; i < 64; ++i) t[i] = this.pos[i + 1];
                Skip(65);
            }
            if (this.length != 0) throw new NJException(NJErrorCode.SyntaxError);
        }

        private int GetVLC(VLCCode* vlc, byte* code)
        {
            int value = ShowBits(16);
            int bits = vlc[value].bits;
            if (bits == 0) throw new NJException(NJErrorCode.SyntaxError);
            SkipBits(bits);
            value = vlc[value].code;
            if (code != null) *code = (byte)value;
            bits = value & 15;
            if (bits == 0) return 0;
            value = GetBits(bits);
            if (value < (1 << (bits - 1))) value += ((-1) << bits) + 1;
            return value;
        }

        private void DecodeBlock(Component* c, byte* outv)
        {
            byte code = 0;
            int value, coef = 0;
            FillMem((long*)block, 0, 64 / 2);//use long instead of int
            c->dcpred += GetVLC(&this.vlctab[c->dctabsel][0], null);
            this.block[0] = (c->dcpred) * this.qtab[c->qtsel][0];
            do
            {
                value = GetVLC(&this.vlctab[c->actabsel][0], &code);
                if (code == 0) break;  // EOB
                if ((code & 0x0F) == 0 && code != 0xF0) throw new NJException(NJErrorCode.SyntaxError);
                coef += (code >> 4) + 1;
                if (coef > 63) throw new NJException(NJErrorCode.SyntaxError);
                this.block[(int)njZZ[coef]] = value * this.qtab[c->qtsel][coef];
            } while (coef < 63);
            for (coef = 0; coef < 64; coef += 8) { RowIDCT(&this.block[coef]); }
            for (coef = 0; coef < 8; ++coef) { ColIDCT(&this.block[coef], &outv[coef], c->stride); }
        }

        private void DecodeScan()
        {
            int i, mbx, mby, sbx, sby;
            int rstcount = this.rstinterval, nextrst = 0;
            Component* c;
            DecodeLength();
            if (this.length < (4 + 2 * this.ncomp)) throw new NJException(NJErrorCode.SyntaxError);
            if (this.pos[0] != this.ncomp) throw new NJException(NJErrorCode.Unsupported);
            Skip(1);
            for (i = 0, c = this.comp; i < this.ncomp; ++i, ++c)
            {
                if (this.pos[0] != c->cid) throw new NJException(NJErrorCode.SyntaxError);
                if ((this.pos[1] & 0xEE) != 0) throw new NJException(NJErrorCode.SyntaxError);
                c->dctabsel = this.pos[1] >> 4;
                c->actabsel = (this.pos[1] & 1) | 2;
                Skip(2);
            }
            if (this.pos[0] != 0 || this.pos[1] != 63 || this.pos[2] != 0) throw new NJException(NJErrorCode.Unsupported);
            Skip(this.length);
            mbx = mby = 0;
            while(true)
            {
                for (i = 0, c = this.comp; i < this.ncomp; ++i, ++c)
                {
                    for (sby = 0; sby < c->ssy; ++sby)
                    {
                        for (sbx = 0; sbx < c->ssx; ++sbx)
                        {
                            DecodeBlock(c, &c->pixels[((mby * c->ssy + sby) * c->stride + mbx * c->ssx + sbx) << 3]);
                        }
                    }
                }
                if (++mbx >= this.mbwidth)
                {
                    mbx = 0;
                    if (++mby >= this.mbheight) break;
                }
                if (this.rstinterval != 0 && --rstcount == 0)
                {
                    this.bufbits &= 0xF8;
                    i = GetBits(16);
                    if ((i & 0xFFF8) != 0xFFD0 || (i & 7) != nextrst) throw new NJException(NJErrorCode.SyntaxError);
                    nextrst = (nextrst + 1) & 7;
                    rstcount = this.rstinterval;
                    for (i = 0; i < 3; ++i) { this.comp[i].dcpred = 0; }
                }
            }
            IsDone = true;
        }

        private void UpsampleH(Component* c)
        {
            int xmax = c->width - 3;
            byte* outv, lin, lout;
            int x, y;
            try
            {
                outv = (byte*)Marshal.AllocHGlobal((c->width * c->height) << 1);
                lin = c->pixels;
                lout = outv;
                for (y = c->height; y != 0; --y)
                {
                    lout[0] = CF(CF2A * lin[0] + CF2B * lin[1]);
                    lout[1] = CF(CF3X * lin[0] + CF3Y * lin[1] + CF3Z * lin[2]);
                    lout[2] = CF(CF3A * lin[0] + CF3B * lin[1] + CF3C * lin[2]);

                    for (x = 0; x < xmax; ++x)
                    {
                        lout[(x << 1) + 3] = CF(CF4A * lin[x] + CF4B * lin[x + 1] + CF4C * lin[x + 2] + CF4D * lin[x + 3]);
                        lout[(x << 1) + 4] = CF(CF4D * lin[x] + CF4C * lin[x + 1] + CF4B * lin[x + 2] + CF4A * lin[x + 3]);
                    }
                    
                    lin += c->stride;
                    lout += c->width << 1;

                    lout[-3] = CF(CF3A * lin[-1] + CF3B * lin[-2] + CF3C * lin[-3]);
                    lout[-2] = CF(CF3X * lin[-1] + CF3Y * lin[-2] + CF3Z * lin[-3]);
                    lout[-1] = CF(CF2A * lin[-1] + CF2B * lin[-2]);
                }
                c->width <<= 1;
                c->stride = c->width;
            }
            finally { if (c->pixels != null) Marshal.FreeHGlobal((IntPtr)c->pixels); }
            c->pixels = outv;
        }

        private void UpsampleV(Component* c)
        {
            int w = c->width, s1 = c->stride, s2 = s1 + s1;
            byte* outv, cin, cout;
            int x, y;
            try
            {
                outv = (byte*)Marshal.AllocHGlobal((c->width * c->height) << 1);
                for (x = 0; x < w; ++x)
                {
                    cin = &c->pixels[x];
                    cout = &outv[x];
                    *cout = CF(CF2A * cin[0] + CF2B * cin[s1]); cout += w;
                    *cout = CF(CF3X * cin[0] + CF3Y * cin[s1] + CF3Z * cin[s2]); cout += w;
                    *cout = CF(CF3A * cin[0] + CF3B * cin[s1] + CF3C * cin[s2]); cout += w;
                    cin += s1;
                    for (y = c->height - 3; y != 0; --y)
                    {
                        *cout = CF(CF4A * cin[-s1] + CF4B * cin[0] + CF4C * cin[s1] + CF4D * cin[s2]); cout += w;
                        *cout = CF(CF4D * cin[-s1] + CF4C * cin[0] + CF4B * cin[s1] + CF4A * cin[s2]); cout += w;
                        cin += s1;
                    }
                    cin += s1;
                    *cout = CF(CF3A * cin[0] + CF3B * cin[-s1] + CF3C * cin[-s2]); cout += w;
                    *cout = CF(CF3X * cin[0] + CF3Y * cin[-s1] + CF3Z * cin[-s2]); cout += w;
                    *cout = CF(CF2A * cin[0] + CF2B * cin[-s1]);
                }
                c->height <<= 1;
                c->stride = c->width;
            }
            finally { if (c->pixels != null) Marshal.FreeHGlobal((IntPtr)c->pixels); }
            c->pixels = outv;
        }
        
        private void ConvertYCC(bool flip)
        {
            int i;
            int w = this.width;
            int h = this.height;
            Component* c;

            for (i = 0, c = this.comp; i < this.ncomp; ++i, ++c)
            {
                while ((c->width < w) || (c->height < h))
                {
                    if (c->width < w) UpsampleH(c);
                    if (c->height < h) UpsampleV(c);
                }
                if ((c->width < w) || (c->height < h)) throw new NJException(NJErrorCode.InternalError);
            }

            if (this.ncomp == 3)
            {
                // convert to RGB
                int x, yy, y, cb, cr, r, g, b;
                byte* prgb = this.rgb;
                byte* py = this.comp[0].pixels;
                byte* pcb = this.comp[1].pixels;
                byte* pcr = this.comp[2].pixels;
                int rs = this.comp[0].stride - w;
                int gs = this.comp[1].stride - w;
                int bs = this.comp[2].stride - w;

                for (yy = this.height; yy != 0; --yy)
                {
                    for (x = 0; x < w; ++x)
                    {
                        y = *py++ << 8;
                        cb = *pcb++ - 128;
                        cr = *pcr++ - 128;

                        g = (y - 88 * cb - 183 * cr + 128) >> 8;

                        if (flip)
                        {
                            b = (y + 359 * cr + 128) >> 8;
                            r = (y + 454 * cb + 128) >> 8;
                        }
                        else
                        {
                            r = (y + 359 * cr + 128) >> 8;
                            b = (y + 454 * cb + 128) >> 8;
                        }

                        if (r < 0) *prgb++ = 0;
                        else if (r > 0xFF) *prgb++ = 0xFF;
                        else *prgb++ = (byte)r;

                        if (g < 0) *prgb++ = 0;
                        else if (g > 0xFF) *prgb++ = 0xFF;
                        else *prgb++ = (byte)g;

                        if (b < 0) *prgb++ = 0;
                        else if (b > 0xFF) *prgb++ = 0xFF;
                        else *prgb++ = (byte)b;
                    }
                    py += rs;
                    pcb += gs;
                    pcr += bs;
                }

                Marshal.FreeHGlobal((IntPtr)this.comp[0].pixels);
                Marshal.FreeHGlobal((IntPtr)this.comp[1].pixels);
                Marshal.FreeHGlobal((IntPtr)this.comp[2].pixels);
                this.comp[0].pixels = this.comp[1].pixels = this.comp[2].pixels = null;
            }
            else if (this.comp[0].width != this.comp[0].stride)
            {
                // grayscale -> only remove stride
                int y, x;
                int cw = this.comp[0].width;
                int cs = this.comp[0].stride;
                int d = cs - cw;
                byte* pin = &this.comp[0].pixels[cs];
                byte* pout = &this.comp[0].pixels[cw];

                for (y = this.comp[0].height - 1; y != 0; --y)
                {
                    for (x = 0; x < cw; x++) *pout++ = *pin++;
                    pin += d;
                }
                this.comp[0].stride = cw;

                Marshal.FreeHGlobal((IntPtr)this.comp[0].pixels);
                this.comp[0].pixels = null;
            }
        }

        #endregion

        #region Helper Methods
        
        private static byte Clip(int x)
        {
            if (x < 0) return 0;
            else if (x > 0xFF) return 0xFF;
            else return (byte)x;
        }

        private static byte CF(int x)
        {
            x = (x + 64) >> 7;
            if (x < 0) return 0;
            else if (x > 0xFF) return 0xFF;
            else return (byte)x;
        }

        private void SkipBits(int bits)
        {
            if (this.bufbits < bits) ShowBits(bits);
            this.bufbits -= bits;
        }

        private int GetBits(int bits)
        {
            int res = ShowBits(bits);
            SkipBits(bits);
            return res;
        }
        
        private void Skip(int count)
        {
            this.pos += count;
            this.size -= count;
            this.length -= count;
            if (this.size < 0) throw new NJException(NJErrorCode.SyntaxError);
        }

        private void DecodeLength()
        {
            if (this.size < 2) throw new NJException(NJErrorCode.SyntaxError);
            this.length = Decode16(this.pos);
            if (this.length > this.size) throw new NJException(NJErrorCode.SyntaxError);
            Skip(2);
        }

        private void SkipMarker()
        {
            DecodeLength();
            Skip(this.length);
        }
        
        private void DecodeDRI()
        {
            DecodeLength();
            if (this.length < 2) throw new NJException(NJErrorCode.SyntaxError);
            this.rstinterval = Decode16(this.pos);
            Skip(this.length);
        }

        private static ushort Decode16(byte* pos)
        {
            return (ushort)((pos[0] << 8) | pos[1]);
        }

        private static void FillMem(byte* block, byte value, int count)
        {
            for (int i = 0; i < count; i++) block[i] = value;
        }

        private static void FillMem(int* block, int value, int count)
        {
            for (int i = 0; i < count; i++) block[i] = value;
        }

        private static void FillMem(long* block, int value, int count)
        {
            for (int i = 0; i < count; i++) block[i] = value;
        }

        private static void FillMem(Component* block, Component value, int count)
        {
            for (int i = 0; i < count; i++) block[i] = value;
        }

        private static void FillMem(VLCCode* block, VLCCode value, int count)
        {
            for (int i = 0; i < count; i++) block[i] = value;
        }
        
        #endregion
    }
}