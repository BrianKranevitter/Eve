// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Helmet"
{
	Properties
	{
		_Albedo("Albedo", 2D) = "white" {}
		_Normal("Normal", 2D) = "bump" {}
		_Roughness("Roughness", 2D) = "white" {}
		_Metallic("Metallic", 2D) = "white" {}
		_Normal_Scale("Normal_Scale", Float) = 0
		_Roughness_Scale("Roughness_Scale", Float) = 0
		_Emission_All("Emission_All", 2D) = "white" {}
		_Emission_Indicator("Emission_Indicator", 2D) = "white" {}
		_AO("AO", 2D) = "white" {}
		_Battery_Level("Battery_Level", Range( 0 , 1)) = 1
		_BatteryLevel_HegithLimit("BatteryLevel_HegithLimit", Float) = -0.9
		_Indicator_Color("Indicator_Color", Color) = (1,0.9963837,0.8553458,0)
		_BatteryLevel_LowLimit("BatteryLevel_LowLimit", Float) = 0.1
		_Emission_BatteryLevel("Emission_BatteryLevel", 2D) = "white" {}
		_LowBatteryLerp("LowBatteryLerp", Range( 0 , 1)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _Normal;
		uniform float4 _Normal_ST;
		uniform float _Normal_Scale;
		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform float4 _Indicator_Color;
		uniform sampler2D _Emission_Indicator;
		uniform float4 _Emission_Indicator_ST;
		uniform sampler2D _Emission_All;
		uniform float4 _Emission_All_ST;
		uniform sampler2D _Emission_BatteryLevel;
		uniform float4 _Emission_BatteryLevel_ST;
		uniform float _Battery_Level;
		uniform float _BatteryLevel_LowLimit;
		uniform float _BatteryLevel_HegithLimit;
		uniform float _LowBatteryLerp;
		uniform sampler2D _Metallic;
		uniform float4 _Metallic_ST;
		uniform sampler2D _Roughness;
		uniform float4 _Roughness_ST;
		uniform float _Roughness_Scale;
		uniform sampler2D _AO;
		uniform float4 _AO_ST;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			float3 NORMAL14 = UnpackScaleNormal( tex2D( _Normal, uv_Normal ), _Normal_Scale );
			o.Normal = NORMAL14;
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			float4 ALBEDO26 = tex2D( _Albedo, uv_Albedo );
			o.Albedo = ALBEDO26.rgb;
			float2 uv_Emission_Indicator = i.uv_texcoord * _Emission_Indicator_ST.xy + _Emission_Indicator_ST.zw;
			float2 uv_Emission_All = i.uv_texcoord * _Emission_All_ST.xy + _Emission_All_ST.zw;
			float4 EMISSION_prev16 = tex2D( _Emission_All, uv_Emission_All );
			float2 uv_Emission_BatteryLevel = i.uv_texcoord * _Emission_BatteryLevel_ST.xy + _Emission_BatteryLevel_ST.zw;
			float4 Emission_Battery80 = tex2D( _Emission_BatteryLevel, uv_Emission_BatteryLevel );
			float4 color103 = IsGammaSpace() ? float4(1,1,1,0) : float4(1,1,1,0);
			float4 color100 = IsGammaSpace() ? float4(1,0.4405904,0,1) : float4(1,0.1631132,0,1);
			float clampResult107 = clamp( ( ( 1.0 - _Battery_Level ) * 2.0 ) , 0.0 , 1.0 );
			float4 lerpResult104 = lerp( color103 , color100 , clampResult107);
			float temp_output_59_0 = ( saturate( ( 1.0 - ( ( ( 1.0 - i.uv_texcoord.y ) - ( ( _Battery_Level * _BatteryLevel_LowLimit ) - _BatteryLevel_HegithLimit ) ) * 143.39 ) ) ) - saturate( ( 1.0 - ( ( ( 1.0 - i.uv_texcoord.y ) - ( ( ( 1.0 - 1.0 ) * 0.1 ) - -0.864 ) ) * 50.0 ) ) ) );
			float4 lerpResult58 = lerp( ( Emission_Battery80 * lerpResult104 ) , float4( 0,0,0,0 ) , temp_output_59_0);
			float4 color91 = IsGammaSpace() ? float4(0.8396226,0,0,0) : float4(0.673178,0,0,0);
			float4 lerpResult92 = lerp( ( Emission_Battery80 * color91 ) , float4( 0,0,0,0 ) , temp_output_59_0);
			float4 lerpResult88 = lerp( lerpResult58 , lerpResult92 , _LowBatteryLerp);
			float4 EMISSION45 = ( ( _Indicator_Color * tex2D( _Emission_Indicator, uv_Emission_Indicator ) ) + saturate( ( EMISSION_prev16 + lerpResult88 ) ) );
			o.Emission = EMISSION45.rgb;
			float2 uv_Metallic = i.uv_texcoord * _Metallic_ST.xy + _Metallic_ST.zw;
			float4 METALLIC18 = tex2D( _Metallic, uv_Metallic );
			o.Metallic = METALLIC18.r;
			float2 uv_Roughness = i.uv_texcoord * _Roughness_ST.xy + _Roughness_ST.zw;
			float4 ROUGHNESS15 = ( 1.0 - ( tex2D( _Roughness, uv_Roughness ) * _Roughness_Scale ) );
			o.Smoothness = ROUGHNESS15.r;
			float2 uv_AO = i.uv_texcoord * _AO_ST.xy + _AO_ST.zw;
			float4 AO17 = ( tex2D( _AO, uv_AO ) * 0.0 );
			o.Occlusion = AO17.r;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18900
540;73;760;523;4061.176;-1423.611;2.133511;True;False
Node;AmplifyShaderEditor.RangedFloatNode;69;-4102.825,3038.629;Inherit;False;Constant;_Float4;Float 4;8;0;Create;True;0;0;0;False;0;False;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;62;-3760.255,3011.074;Inherit;False;2;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;29;-4197.099,2486.577;Inherit;False;Property;_Battery_Level;Battery_Level;9;0;Create;True;0;0;0;False;0;False;1;0.728045;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;78;-3590.915,2650.426;Inherit;False;Property;_BatteryLevel_LowLimit;BatteryLevel_LowLimit;12;0;Create;True;0;0;0;False;0;False;0.1;0.066;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;50;-3386.847,2645.589;Inherit;False;Property;_BatteryLevel_HegithLimit;BatteryLevel_HegithLimit;10;0;Create;True;0;0;0;False;0;False;-0.9;-0.899;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;66;-3570.896,3205.357;Inherit;False;Constant;_Float2;Float 2;9;0;Create;True;0;0;0;False;0;False;-0.864;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;52;-3414.27,2496.044;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;60;-3609.958,3050.441;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;64;-3644.214,2775.806;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;28;-3512.797,2368.347;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;61;-3349.779,3139.894;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;48;-3254.081,2264.194;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;63;-3406.942,2855.534;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;51;-3244.055,2501.336;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;68;-3162.362,2923.695;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;67;-3117.699,3200.341;Inherit;False;Constant;_Float3;Float 3;8;0;Create;True;0;0;0;False;0;False;50;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;79;-3693.318,1612.135;Inherit;True;Property;_Emission_BatteryLevel;Emission_BatteryLevel;13;0;Create;True;0;0;0;False;0;False;-1;52660c3b2042dbe4ca39571abaea2afa;52660c3b2042dbe4ca39571abaea2afa;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;33;-3030.317,2264.702;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;55;-3730.394,2165.083;Inherit;False;2;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;35;-2975.246,2520.531;Inherit;False;Constant;_Float1;Float 1;8;0;Create;True;0;0;0;False;0;False;143.39;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;80;-3399.194,1613.165;Inherit;False;Emission_Battery;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;105;-3467.745,2111.206;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;38;-2771.059,2266.713;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;65;-2877.243,2923.406;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;56;-2544.001,2264.448;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;103;-3088.808,1790.439;Inherit;False;Constant;_Color2;Color 2;15;0;Create;True;0;0;0;False;0;False;1,1,1,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;34;-3106.174,1420.597;Inherit;True;80;Emission_Battery;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;70;-2631.944,2649.233;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;107;-3271.398,2074.229;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;100;-3096.638,1616.203;Inherit;False;Constant;_Color1;Color 1;14;0;Create;True;0;0;0;False;0;False;1,0.4405904,0,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;73;-2309.998,2363.02;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RelayNode;36;-2895.202,1427.762;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;72;-2320.803,2275.405;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;89;-2338.828,2478.077;Inherit;True;80;Emission_Battery;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;91;-2324.65,2687.323;Inherit;False;Constant;_Color0;Color 0;14;0;Create;True;0;0;0;False;0;False;0.8396226,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;104;-2810.699,1847.278;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;11;-3680.829,1398.944;Inherit;True;Property;_Emission_All;Emission_All;6;0;Create;True;0;0;0;False;0;False;-1;6d840fd2be766d34f95db45d63cd06f2;6d840fd2be766d34f95db45d63cd06f2;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;59;-2132.182,2244.057;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;102;-2473.72,1853.367;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;90;-2081.863,2500.44;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;16;-3379.688,1392.478;Inherit;False;EMISSION_prev;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;92;-1840.174,2414.827;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;58;-1916.519,2065.796;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;93;-1708.911,2657.168;Inherit;False;Property;_LowBatteryLerp;LowBatteryLerp;14;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;88;-1402.081,2090.28;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;81;-1376.812,1896.278;Inherit;True;16;EMISSION_prev;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;4;-2695.348,211.4346;Inherit;False;Property;_Roughness_Scale;Roughness_Scale;5;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;74;-1460.5,1668.309;Inherit;True;Property;_Emission_Indicator;Emission_Indicator;7;0;Create;True;0;0;0;False;0;False;-1;e7e19b7b11b3ee94ca2dee5e379b9948;e7e19b7b11b3ee94ca2dee5e379b9948;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;77;-1416.807,1490.452;Inherit;False;Property;_Indicator_Color;Indicator_Color;11;0;Create;True;0;0;0;False;0;False;1,0.9963837,0.8553458,0;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;3;-2776.914,6.933545;Inherit;True;Property;_Roughness;Roughness;2;0;Create;True;0;0;0;False;0;False;-1;None;888eefb3eea450a4ab7b5f3bb9704ad6;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;82;-1086.386,1972.321;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;6;-2632.292,782.4937;Inherit;False;Constant;_AO_Scale;AO_Scale;8;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;95;-920.0743,1973.827;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;5;-2771.292,571.4937;Inherit;True;Property;_AO;AO;8;0;Create;True;0;0;0;False;0;False;-1;None;b33eb6b8a2cb07042b84545e413d8e5a;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;8;-2999.974,-152.4813;Inherit;False;Property;_Normal_Scale;Normal_Scale;4;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;-2433.318,106.3503;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;76;-1084.007,1672.85;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;1;-2818.119,-409.9683;Inherit;True;Property;_Albedo;Albedo;0;0;Create;True;0;0;0;False;0;False;-1;cc59aceb46254394bb1075a8c9581d15;300e117b12f50d543b33775ffbf96789;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;13;-2796.304,-223.3318;Inherit;True;Property;_Normal;Normal;1;0;Create;True;0;0;0;False;0;False;-1;None;7726264fd57ee4e41aacc9c7e4c206ec;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;12;-2768.625,325.6299;Inherit;True;Property;_Metallic;Metallic;3;0;Create;True;0;0;0;False;0;False;-1;None;1ff8226bc79d9d44c96913793339efa7;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;10;-2298.896,84.47796;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;9;-2441.292,676.4937;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;75;-768.6982,1944.537;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;26;-2448.59,-411.1569;Inherit;False;ALBEDO;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;18;-2407.119,356.4209;Inherit;False;METALLIC;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;17;-2306.292,618.4937;Inherit;False;AO;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;15;-2093.563,34.70498;Inherit;False;ROUGHNESS;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;14;-2440.679,-181.4403;Inherit;False;NORMAL;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;45;-545.0626,1947.01;Inherit;False;EMISSION;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;22;-259.9581,136.9554;Inherit;False;45;EMISSION;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;19;-247.9581,203.9554;Inherit;False;18;METALLIC;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;20;-250.9581,-0.04466534;Inherit;False;26;ALBEDO;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;23;-243.1542,357.0975;Inherit;False;17;AO;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;21;-251.9581,65.95536;Inherit;False;14;NORMAL;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;24;-256.9396,279.7026;Inherit;False;15;ROUGHNESS;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;Helmet;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;62;1;69;0
WireConnection;52;0;29;0
WireConnection;52;1;78;0
WireConnection;60;0;62;0
WireConnection;61;0;60;0
WireConnection;61;1;66;0
WireConnection;48;0;28;2
WireConnection;63;0;64;2
WireConnection;51;0;52;0
WireConnection;51;1;50;0
WireConnection;68;0;63;0
WireConnection;68;1;61;0
WireConnection;33;0;48;0
WireConnection;33;1;51;0
WireConnection;55;1;29;0
WireConnection;80;0;79;0
WireConnection;105;0;55;0
WireConnection;38;0;33;0
WireConnection;38;1;35;0
WireConnection;65;0;68;0
WireConnection;65;1;67;0
WireConnection;56;0;38;0
WireConnection;70;0;65;0
WireConnection;107;0;105;0
WireConnection;73;0;70;0
WireConnection;36;0;34;0
WireConnection;72;0;56;0
WireConnection;104;0;103;0
WireConnection;104;1;100;0
WireConnection;104;2;107;0
WireConnection;59;0;72;0
WireConnection;59;1;73;0
WireConnection;102;0;36;0
WireConnection;102;1;104;0
WireConnection;90;0;89;0
WireConnection;90;1;91;0
WireConnection;16;0;11;0
WireConnection;92;0;90;0
WireConnection;92;2;59;0
WireConnection;58;0;102;0
WireConnection;58;2;59;0
WireConnection;88;0;58;0
WireConnection;88;1;92;0
WireConnection;88;2;93;0
WireConnection;82;0;81;0
WireConnection;82;1;88;0
WireConnection;95;0;82;0
WireConnection;7;0;3;0
WireConnection;7;1;4;0
WireConnection;76;0;77;0
WireConnection;76;1;74;0
WireConnection;13;5;8;0
WireConnection;10;0;7;0
WireConnection;9;0;5;0
WireConnection;9;1;6;0
WireConnection;75;0;76;0
WireConnection;75;1;95;0
WireConnection;26;0;1;0
WireConnection;18;0;12;0
WireConnection;17;0;9;0
WireConnection;15;0;10;0
WireConnection;14;0;13;0
WireConnection;45;0;75;0
WireConnection;0;0;20;0
WireConnection;0;1;21;0
WireConnection;0;2;22;0
WireConnection;0;3;19;0
WireConnection;0;4;24;0
WireConnection;0;5;23;0
ASEEND*/
//CHKSM=93FB0F5C6B13299120D042AD239AF69312574200