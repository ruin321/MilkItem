using System.IO;
using System.Text;
using HarmonyLib;
using MTM101BaldAPI;
using MTM101BaldAPI.AssetTools;
using UnityEngine;

namespace MilkItem
{
    
    
    
    [HarmonyPatch]
    internal static class PatchAudioLoadDirect
    {
        static System.Reflection.MethodBase TargetMethod()
        {
            var t = typeof(AssetLoader);
            if (t == null) return null;
            return t.GetMethod("AudioClipFromFile",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static,
                null,
                new System.Type[] { typeof(string), typeof(AudioType) },
                null);
        }

        static bool Prepare() => TargetMethod() != null;

        [HarmonyPrefix]
        static bool Prefix(string path, AudioType type, ref AudioClip __result)
        {
            try
            {
                if (type != AudioType.WAV) return true;                    
                if (string.IsNullOrEmpty(path) || !File.Exists(path)) return true;
                AudioClip clip = WavDirectDecoder.Decode(path);
                if (clip == null) return true;                            
                __result = clip;
                return false;
            }
            catch (System.Exception)
            {
                return true;                                              
            }
        }
    }

    
    
    internal static class WavDirectDecoder
    {
        public static AudioClip Decode(string path)
        {
            byte[] data = File.ReadAllBytes(path);
            if (data.Length < 44) return null;
            if (GetAscii(data, 0, 4) != "RIFF" || GetAscii(data, 8, 4) != "WAVE") return null;

            ushort fmtTag = 0, channels = 0, bits = 0;
            uint sampleRate = 0;
            bool fmtDone = false;
            int dataStart = -1, dataLen = 0;

            int pos = 12;
            while (pos + 8 <= data.Length)
            {
                string id = GetAscii(data, pos, 4);
                uint sz = System.BitConverter.ToUInt32(data, pos + 4);
                int body = pos + 8;
                if (id == "fmt ")
                {
                    if (body + 2 <= data.Length) fmtTag = System.BitConverter.ToUInt16(data, body);
                    if (body + 4 <= data.Length) channels = System.BitConverter.ToUInt16(data, body + 2);
                    if (body + 8 <= data.Length) sampleRate = System.BitConverter.ToUInt32(data, body + 4);
                    if (body + 14 <= data.Length) bits = System.BitConverter.ToUInt16(data, body + 14);
                    fmtDone = true;
                }
                else if (id == "data")
                {
                    dataStart = body;
                    dataLen = (int)System.Math.Min(sz, (uint)System.Math.Max(0, data.Length - body));
                }
                int step = 8 + (int)sz;
                if (step <= 0) break;
                pos += step;
                if ((pos & 1) == 1) pos++; 
            }

            if (!fmtDone || dataStart < 0 || channels == 0 || sampleRate == 0) return null;
            int bytesPerSample = bits / 8;
            if (bytesPerSample <= 0) return null;
            int blockAlign = channels * bytesPerSample;
            int frameCount = dataLen / blockAlign;
            if (frameCount <= 0) return null;

            float[] samples = new float[frameCount * channels];
            int p = dataStart;
            if (fmtTag == 1 && bits == 16)
            {
                for (int i = 0; i < samples.Length; i++) { short s = System.BitConverter.ToInt16(data, p); samples[i] = s / 32768f; p += 2; }
            }
            else if (fmtTag == 1 && bits == 8)
            {
                for (int i = 0; i < samples.Length; i++) { byte b = data[p++]; samples[i] = (b - 128) / 128f; }
            }
            else if (fmtTag == 3)
            {
                for (int i = 0; i < samples.Length; i++) { if (p + 4 > data.Length) break; samples[i] = System.BitConverter.ToSingle(data, p); p += 4; }
            }
            else
            {
                return null; 
            }

            AudioClip clip = AudioClip.Create(Path.GetFileNameWithoutExtension(path), frameCount, channels, (int)sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private static string GetAscii(byte[] d, int off, int len)
        {
            using (var s = new MemoryStream())
            {
                for (int i = off; i < off + len && i < d.Length; i++) s.WriteByte(d[i]);
                return Encoding.ASCII.GetString(s.ToArray());
            }
        }
    }
}