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
    
    public static  float FindFloatByName_Default(string name)
    {
        switch (name)
        {
            case nameof(Saturation):
                return -0.5f;
            case nameof(Brightness):
                return 0;
            case nameof(Contrast):
                return 0;
            case nameof(Sharpness):
                return 0;
            
            case nameof(MasterVolume):
                return 1;
            case nameof(SoundVolume):
                return 1;
            case nameof(VoiceVolume):
                return 1;
            case nameof(MusicVolume):
                return 1;
            
            case nameof(mouseSensitivity):
                return 1;
        }

        throw new Exception("Error finding float property \"" + name + "\".");
    }
}
