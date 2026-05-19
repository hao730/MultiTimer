using System;
using System.IO;
using System.Media;

namespace MultiTimer.Models;

/// <summary>
/// 負責播放提醒音效。
/// </summary>
public static class SoundService
{
    private static readonly string AlarmPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.Windows),
        @"Media\Ring02.wav");

    public static void PlayAlarm()
    {
        if (File.Exists(AlarmPath))
        {
            using var player = new SoundPlayer(AlarmPath);
            player.Play();
        }
        else
        {
            SystemSounds.Exclamation.Play();
        }
    }
}
