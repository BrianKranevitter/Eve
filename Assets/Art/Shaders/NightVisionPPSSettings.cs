// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>
#if UNITY_POST_PROCESSING_STACK_V2
using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess( typeof( NightVisionPPSRenderer ), PostProcessEvent.AfterStack, "NightVision", true )]
public sealed class NightVisionPPSSettings : PostProcessEffectSettings
{
	[Tooltip( "Color" )]
	public ColorParameter _Color = new ColorParameter { value = new Color(1f,0.1289307f,0.8354284f,0f) };
	[Tooltip( "Noise_Speed" )]
	public FloatParameter _Noise_Speed = new FloatParameter { value = 0f };
	[Tooltip( "Noise_Scale" )]
	public Vector4Parameter _Noise_Scale = new Vector4Parameter { value = new Vector4(80f,750f,0f,0f) };
	[Tooltip( "Remap_Min" )]
	public FloatParameter _Remap_Min = new FloatParameter { value = 0f };
	[Tooltip( "Remap_Max" )]
	public FloatParameter _Remap_Max = new FloatParameter { value = 0f };
	[Tooltip( "Noise_Strenght" )]
	public FloatParameter _Noise_Strenght = new FloatParameter { value = 0.56f };
	[Tooltip( "Grayscale Value" )]
	public FloatParameter _GrayscaleValue = new FloatParameter { value = 0f };
}

public sealed class NightVisionPPSRenderer : PostProcessEffectRenderer<NightVisionPPSSettings>
{
	public override void Render( PostProcessRenderContext context )
	{
		var sheet = context.propertySheets.Get( Shader.Find( "NightVision" ) );
		sheet.properties.SetColor( "_Color", settings._Color );
		sheet.properties.SetFloat( "_Noise_Speed", settings._Noise_Speed );
		sheet.properties.SetVector( "_Noise_Scale", settings._Noise_Scale );
		sheet.properties.SetFloat( "_Remap_Min", settings._Remap_Min );
		sheet.properties.SetFloat( "_Remap_Max", settings._Remap_Max );
		sheet.properties.SetFloat( "_Noise_Strenght", settings._Noise_Strenght );
		sheet.properties.SetFloat( "_GrayscaleValue", settings._GrayscaleValue );
		context.command.BlitFullscreenTriangle( context.source, context.destination, sheet, 0 );
	}
}
#endif
