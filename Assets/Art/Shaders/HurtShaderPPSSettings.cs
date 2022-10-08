// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>
#if UNITY_POST_PROCESSING_STACK_V2
using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess( typeof( HurtShaderPPSRenderer ), PostProcessEvent.AfterStack, "HurtShader", true )]
public sealed class HurtShaderPPSSettings : PostProcessEffectSettings
{
	[Tooltip( "BigSplats" )]
	public TextureParameter _BigSplats = new TextureParameter {  };
	[Tooltip( "MidSplats" )]
	public TextureParameter _MidSplats = new TextureParameter {  };
	[Tooltip( "Splats3" )]
	public FloatParameter _Splats3 = new FloatParameter { value = 1f };
	[Tooltip( "Splats2" )]
	public FloatParameter _Splats2 = new FloatParameter { value = 0.8572289f };
	[Tooltip( "Splats1" )]
	public FloatParameter _Splats1 = new FloatParameter { value = 1f };
	[Tooltip( "LargeSplats" )]
	public TextureParameter _LargeSplats = new TextureParameter {  };
	[Tooltip( "Blood_Color" )]
	public ColorParameter _Blood_Color = new ColorParameter { value = new Color(1f,0f,0f,0f) };
	[Tooltip( "Screen_Center" )]
	public TextureParameter _Screen_Center = new TextureParameter {  };
}

public sealed class HurtShaderPPSRenderer : PostProcessEffectRenderer<HurtShaderPPSSettings>
{
	public override void Render( PostProcessRenderContext context )
	{
		var sheet = context.propertySheets.Get( Shader.Find( "HurtShader" ) );
		if(settings._BigSplats.value != null) sheet.properties.SetTexture( "_BigSplats", settings._BigSplats );
		if(settings._MidSplats.value != null) sheet.properties.SetTexture( "_MidSplats", settings._MidSplats );
		sheet.properties.SetFloat( "_Splats3", settings._Splats3 );
		sheet.properties.SetFloat( "_Splats2", settings._Splats2 );
		sheet.properties.SetFloat( "_Splats1", settings._Splats1 );
		if(settings._LargeSplats.value != null) sheet.properties.SetTexture( "_LargeSplats", settings._LargeSplats );
		sheet.properties.SetColor( "_Blood_Color", settings._Blood_Color );
		if(settings._Screen_Center.value != null) sheet.properties.SetTexture( "_Screen_Center", settings._Screen_Center );
		context.command.BlitFullscreenTriangle( context.source, context.destination, sheet, 0 );
	}
}
#endif
