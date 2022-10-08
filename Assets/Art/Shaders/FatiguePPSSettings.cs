// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>
#if UNITY_POST_PROCESSING_STACK_V2
using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess( typeof( FatiguePPSRenderer ), PostProcessEvent.AfterStack, "Fatigue", true )]
public sealed class FatiguePPSSettings : PostProcessEffectSettings
{
	[Tooltip( "Blur_Intensity" )]
	public FloatParameter _Blur_Intensity = new FloatParameter { value = 0.25f };
	[Tooltip( "Beat_Speed" )]
	public FloatParameter _Beat_Speed = new FloatParameter { value = 0.8f };
	[Tooltip( "Base" )]
	public FloatParameter _Base = new FloatParameter { value = 0.4f };
	[Tooltip( "Vignette" )]
	public TextureParameter _Vignette = new TextureParameter {  };
	[Tooltip( "Vignete_Intensity" )]
	public FloatParameter _Vignete_Intensity = new FloatParameter { value = 0.7f };
	[Tooltip( "GlobalIntensity" )]
	public FloatParameter _GlobalIntensity = new FloatParameter { value = 0.5f };
	[Tooltip( "ExplosionBlurIntensity" )]
	public FloatParameter _ExplosionBlurIntensity = new FloatParameter { value = 0f };
}

public sealed class FatiguePPSRenderer : PostProcessEffectRenderer<FatiguePPSSettings>
{
	public override void Render( PostProcessRenderContext context )
	{
		var sheet = context.propertySheets.Get( Shader.Find( "Fatigue" ) );
		sheet.properties.SetFloat( "_Blur_Intensity", settings._Blur_Intensity );
		sheet.properties.SetFloat( "_Beat_Speed", settings._Beat_Speed );
		sheet.properties.SetFloat( "_Base", settings._Base );
		if(settings._Vignette.value != null) sheet.properties.SetTexture( "_Vignette", settings._Vignette );
		sheet.properties.SetFloat( "_Vignete_Intensity", settings._Vignete_Intensity );
		sheet.properties.SetFloat( "_GlobalIntensity", settings._GlobalIntensity );
		sheet.properties.SetFloat( "_ExplosionBlurIntensity", settings._ExplosionBlurIntensity );
		context.command.BlitFullscreenTriangle( context.source, context.destination, sheet, 0 );
	}
}
#endif
