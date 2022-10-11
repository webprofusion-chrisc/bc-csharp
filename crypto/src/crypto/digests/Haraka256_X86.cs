﻿#if NETCOREAPP3_0_OR_GREATER
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;

namespace Org.BouncyCastle.Crypto.Digests
{
    using Aes = System.Runtime.Intrinsics.X86.Aes;
    using Sse2 = System.Runtime.Intrinsics.X86.Sse2;

    public static class Haraka256_X86
    {
        public static bool IsSupported => Aes.IsSupported;

        // Haraka round constants
        private static readonly Vector128<byte>[] DefaultRoundConstants = new Vector128<byte>[]
        {
            Vector128.Create(0x9D, 0x7B, 0x81, 0x75, 0xF0, 0xFE, 0xC5, 0xB2, 0x0A, 0xC0, 0x20, 0xE6, 0x4C, 0x70, 0x84, 0x06),
            Vector128.Create(0x17, 0xF7, 0x08, 0x2F, 0xA4, 0x6B, 0x0F, 0x64, 0x6B, 0xA0, 0xF3, 0x88, 0xE1, 0xB4, 0x66, 0x8B),
            Vector128.Create(0x14, 0x91, 0x02, 0x9F, 0x60, 0x9D, 0x02, 0xCF, 0x98, 0x84, 0xF2, 0x53, 0x2D, 0xDE, 0x02, 0x34),
            Vector128.Create(0x79, 0x4F, 0x5B, 0xFD, 0xAF, 0xBC, 0xF3, 0xBB, 0x08, 0x4F, 0x7B, 0x2E, 0xE6, 0xEA, 0xD6, 0x0E),
            Vector128.Create(0x44, 0x70, 0x39, 0xBE, 0x1C, 0xCD, 0xEE, 0x79, 0x8B, 0x44, 0x72, 0x48, 0xCB, 0xB0, 0xCF, 0xCB),
            Vector128.Create(0x7B, 0x05, 0x8A, 0x2B, 0xED, 0x35, 0x53, 0x8D, 0xB7, 0x32, 0x90, 0x6E, 0xEE, 0xCD, 0xEA, 0x7E),
            Vector128.Create(0x1B, 0xEF, 0x4F, 0xDA, 0x61, 0x27, 0x41, 0xE2, 0xD0, 0x7C, 0x2E, 0x5E, 0x43, 0x8F, 0xC2, 0x67),
            Vector128.Create(0x3B, 0x0B, 0xC7, 0x1F, 0xE2, 0xFD, 0x5F, 0x67, 0x07, 0xCC, 0xCA, 0xAF, 0xB0, 0xD9, 0x24, 0x29),
            Vector128.Create(0xEE, 0x65, 0xD4, 0xB9, 0xCA, 0x8F, 0xDB, 0xEC, 0xE9, 0x7F, 0x86, 0xE6, 0xF1, 0x63, 0x4D, 0xAB),
            Vector128.Create(0x33, 0x7E, 0x03, 0xAD, 0x4F, 0x40, 0x2A, 0x5B, 0x64, 0xCD, 0xB7, 0xD4, 0x84, 0xBF, 0x30, 0x1C),
            Vector128.Create(0x00, 0x98, 0xF6, 0x8D, 0x2E, 0x8B, 0x02, 0x69, 0xBF, 0x23, 0x17, 0x94, 0xB9, 0x0B, 0xCC, 0xB2),
            Vector128.Create(0x8A, 0x2D, 0x9D, 0x5C, 0xC8, 0x9E, 0xAA, 0x4A, 0x72, 0x55, 0x6F, 0xDE, 0xA6, 0x78, 0x04, 0xFA),
            Vector128.Create(0xD4, 0x9F, 0x12, 0x29, 0x2E, 0x4F, 0xFA, 0x0E, 0x12, 0x2A, 0x77, 0x6B, 0x2B, 0x9F, 0xB4, 0xDF),
            Vector128.Create(0xEE, 0x12, 0x6A, 0xBB, 0xAE, 0x11, 0xD6, 0x32, 0x36, 0xA2, 0x49, 0xF4, 0x44, 0x03, 0xA1, 0x1E),
            Vector128.Create(0xA6, 0xEC, 0xA8, 0x9C, 0xC9, 0x00, 0x96, 0x5F, 0x84, 0x00, 0x05, 0x4B, 0x88, 0x49, 0x04, 0xAF),
            Vector128.Create(0xEC, 0x93, 0xE5, 0x27, 0xE3, 0xC7, 0xA2, 0x78, 0x4F, 0x9C, 0x19, 0x9D, 0xD8, 0x5E, 0x02, 0x21),
            Vector128.Create(0x73, 0x01, 0xD4, 0x82, 0xCD, 0x2E, 0x28, 0xB9, 0xB7, 0xC9, 0x59, 0xA7, 0xF8, 0xAA, 0x3A, 0xBF),
            Vector128.Create(0x6B, 0x7D, 0x30, 0x10, 0xD9, 0xEF, 0xF2, 0x37, 0x17, 0xB0, 0x86, 0x61, 0x0D, 0x70, 0x60, 0x62),
            Vector128.Create(0xC6, 0x9A, 0xFC, 0xF6, 0x53, 0x91, 0xC2, 0x81, 0x43, 0x04, 0x30, 0x21, 0xC2, 0x45, 0xCA, 0x5A),
            Vector128.Create(0x3A, 0x94, 0xD1, 0x36, 0xE8, 0x92, 0xAF, 0x2C, 0xBB, 0x68, 0x6B, 0x22, 0x3C, 0x97, 0x23, 0x92),
        };

        public static void Hash(ReadOnlySpan<byte> input, Span<byte> output)
        {
            if (!IsSupported)
                throw new PlatformNotSupportedException(nameof(Haraka256_X86));

            var s1 = Load128(input[  ..16]);
            var s2 = Load128(input[16..32]);

            ImplRounds(ref s1, ref s2, DefaultRoundConstants.AsSpan(0, 20));

            s1 = Sse2.Xor(s1, Load128(input[  ..16]));
            s2 = Sse2.Xor(s2, Load128(input[16..32]));

            Store128(ref s1, output[  ..16]);
            Store128(ref s2, output[16..32]);
        }

        public static void Permute(ReadOnlySpan<byte> input, Span<byte> output)
        {
            if (!IsSupported)
                throw new PlatformNotSupportedException(nameof(Haraka256_X86));

            var s1 = Load128(input[  ..16]);
            var s2 = Load128(input[16..32]);

            ImplRounds(ref s1, ref s2, DefaultRoundConstants.AsSpan(0, 20));

            Store128(ref s1, output[  ..16]);
            Store128(ref s2, output[16..32]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ImplRounds(ref Vector128<byte> s1, ref Vector128<byte> s2, Span<Vector128<byte>> rc)
        {
            ImplRound(ref s1, ref s2, rc[  .. 4]);
            ImplRound(ref s1, ref s2, rc[ 4.. 8]);
            ImplRound(ref s1, ref s2, rc[ 8..12]);
            ImplRound(ref s1, ref s2, rc[12..16]);
            ImplRound(ref s1, ref s2, rc[16..20]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ImplRound(ref Vector128<byte> s1, ref Vector128<byte> s2, Span<Vector128<byte>> rc)
        {
            ImplAes(ref s1, ref s2, rc[ ..2]);
            ImplAes(ref s1, ref s2, rc[2..4]);
            ImplMix(ref s1, ref s2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ImplAes(ref Vector128<byte> s1, ref Vector128<byte> s2, Span<Vector128<byte>> rc)
        {
            s1 = Aes.Encrypt(s1, rc[0]);
            s2 = Aes.Encrypt(s2, rc[1]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ImplMix(ref Vector128<byte> s1, ref Vector128<byte> s2)
        {
            Vector128<uint> t1 = s1.AsUInt32();
            Vector128<uint> t2 = s2.AsUInt32();
            s1 = Sse2.UnpackLow(t1, t2).AsByte();
            s2 = Sse2.UnpackHigh(t1, t2).AsByte();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Vector128<byte> Load128(ReadOnlySpan<byte> t)
        {
#if NET7_0_OR_GREATER
            return Vector128.Create<byte>(t);
#else
            if (BitConverter.IsLittleEndian && Unsafe.SizeOf<Vector128<byte>>() == 16)
                return Unsafe.ReadUnaligned<Vector128<byte>>(ref Unsafe.AsRef(t[0]));

            return Vector128.Create(t[0], t[1], t[2], t[3], t[4], t[5], t[6], t[7], t[8], t[9], t[10], t[11], t[12],
                t[13], t[14], t[15]);
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Store128(ref Vector128<byte> s, Span<byte> t)
        {
#if NET7_0_OR_GREATER
            Vector128.CopyTo(s, t);
#else
            if (BitConverter.IsLittleEndian && Unsafe.SizeOf<Vector128<byte>>() == 16)
            {
                Unsafe.WriteUnaligned(ref t[0], s);
                return;
            }

            var u = s.AsUInt64();
            Utilities.Pack.UInt64_To_LE(u.GetElement(0), t);
            Utilities.Pack.UInt64_To_LE(u.GetElement(1), t[8..]);
#endif
        }
    }
}
#endif
