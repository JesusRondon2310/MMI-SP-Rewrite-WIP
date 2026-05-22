using System;
using System.Collections.Generic;
using System.IO;
using System.Media;
using MMI_SP.Dialogue;

namespace MMI_SP.iFruit
{
    internal static class MechanicSound
    {
        // ==========================================
        // BLOQUE 1: Datos
        // ==========================================
        internal enum SoundFamily { Confirm, Deny }

        private static readonly Random _rnd = new Random();

        private static readonly List<string> _confirmPaths = new List<string>
        {
            "Resources/Mechanic/mechanic_1.wav",
            "Resources/Mechanic/mechanic_2.wav",
            "Resources/Mechanic/mechanic_3.wav",
            "Resources/Mechanic/mechanic_4.wav",
            "Resources/Mechanic/mechanic_5.wav"
        };

        private static readonly List<string> _denyPaths = new List<string>
        {
            "Resources/Mechanic/mechanic_dont_1.wav",
            "Resources/Mechanic/mechanic_dont_2.wav",
            "Resources/Mechanic/mechanic_dont_3.wav",
            "Resources/Mechanic/mechanic_dont_4.wav",
            "Resources/Mechanic/mechanic_dont_5.wav"
        };

        // ==========================================
        // BLOQUE 2: Funciones
        // ==========================================
        internal static void Play(SoundFamily family)
        {
            List<string> paths = GetPaths(family);
            if (paths.Count == 0) return;

            int index = _rnd.Next(0, paths.Count);
            string path = paths[index];
            if (!File.Exists(path)) return;

            using (var fileStream = File.OpenRead(path))
            using (var audioPlayer = new AudioPlayer(fileStream))
            {
                audioPlayer.Volume = Config.iFruitVolume;
                using (var player = new SoundPlayer(audioPlayer))
                {
                    player.Play();
                }
            }
        }

        private static List<string> GetPaths(SoundFamily family)
        {
            switch (family)
            {
                case SoundFamily.Confirm: return _confirmPaths;
                case SoundFamily.Deny: return _denyPaths;
                default: return new List<string>();
            }
        }
    }
}