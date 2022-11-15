using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SettingsManager
{
    public static float Saturation = -0.5f;
    public static float Brightness = 0;
    public static float Contrast = 0;
    public static float Sharpness = 0;

    public static float MasterVolume = 1;
    public static float SoundVolume = 1;
    public static float VoiceVolume = 1;
    public static float MusicVolume = 1;
    
    public static float mouseSensitivity = 1;

    public static ref float FindFloatByName(string name)
    {
        switch (name)
        {
            case nameof(Saturation):
                return ref Saturation;
            case nameof(Brightness):
                return ref Brightness;
            case nameof(Contrast):
                return ref Contrast;
            case nameof(Sharpness):
                return ref Sharpness;
            
            case nameof(MasterVolume):
                return ref MasterVolume;
            case nameof(SoundVolume):
                return ref SoundVolume;
            case nameof(VoiceVolume):
                return ref VoiceVolume;
            case nameof(MusicVolume):
                return ref MusicVolume;
            
            case nameof(mouseSensitivity):
                return ref mouseSensitivity;
        }

        throw new Exception("Error finding float property \"" + name + "\".");
    }
}
