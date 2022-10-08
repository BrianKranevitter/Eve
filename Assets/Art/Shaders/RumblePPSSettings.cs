// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>
#if UNITY_POST_PROCESSING_STACK_V2
using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess( typeof( RumblePPSRenderer ), PostProcessEvent.AfterStack, "Rumble", true )]
public sealed class RumblePPSSettings : PostProcessEffectSettings
{
	[Tooltip( "Rumble_Frecuency" )]
	public FloatParameter _Rumble_Frecuency = new FloatParameter { value = 0f };
	[Tooltip( "Rumble_Aprerture_Natural" )]
	public FloatParameter _Rumble_Aprerture_Natural = new FloatParameter { value = 0f };
	[Tooltip( "Rumble_Aperture_Explosion" )]
	public FloatParameter _Rumble_Aperture_Explosion = new FloatParameter { value = 0f };
}

public sealed class RumblePPSRenderer : PostProcessEffectRenderer<RumblePPSSettings>
{
	public override void Render( PostProcessRenderContext context )
	{
		var sheet = context.propertySheets.Get( Shader.Find( "Rumble" ) );
		sheet.properties.SetFloat( "_Rumble_Frecuency", settings._Rumble_Frecuency );
		sheet.properties.SetFloat( "_Rumble_Aprerture_Natural", settings._Rumble_Aprerture_Natural );
		sheet.properties.SetFloat( "_Rumble_Aperture_Explosion", settings._Rumble_Aperture_Explosion );
		context.command.BlitFullscreenTriangle( context.source, context.destination, sheet, 0 );
	}
}
#endif
