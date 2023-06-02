// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "RightArm"
{
	Properties
	{
		_Albedo("Albedo", 2D) = "white" {}
		_Roughness("Roughness", 2D) = "white" {}
		_Metallic("Metallic", 2D) = "white" {}
		_Emission("Emission", 2D) = "white" {}
		_Normal("Normal", 2D) = "white" {}
		_Roughness_Value("Roughness_Value", Float) = 0
		_AO("AO", 2D) = "white" {}
		_HurtAmount("HurtAmount", Range( 0 , 1)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _Normal;
		uniform float4 _Normal_ST;
		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform float _HurtAmount;
		uniform sampler2D _Emission;
		uniform float4 _Emission_ST;
		uniform sampler2D _Metallic;
		uniform float4 _Metallic_ST;
		uniform sampler2D _Roughness;
		uniform float4 _Roughness_ST;
		uniform float _Roughness_Value;
		uniform sampler2D _AO;
		uniform float4 _AO_ST;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			float3 NORMAL7 = UnpackNormal( tex2D( _Normal, uv_Normal ) );
			o.Normal = NORMAL7;
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			float4 ALBEDO6 = tex2D( _Albedo, uv_Albedo );
			o.Albedo = ALBEDO6.rgb;
			float2 uv_Emission = i.uv_texcoord * _Emission_ST.xy + _Emission_ST.zw;
			float4 tex2DNode4 = tex2D( _Emission, uv_Emission );
			float4 color24 = IsGammaSpace() ? float4(1,0,0,0) : float4(1,0,0,0);
			float4 lerpResult26 = lerp( tex2DNode4 , ( tex2DNode4 * color24 ) , min( _HurtAmount , 1.0 ));
			float mulTime31 = _Time.y * ( _HurtAmount * 3.0 );
			float4 ifLocalVar28 = 0;
			if( _HurtAmount == 1.0 )
				ifLocalVar28 = ( lerpResult26 * abs( sin( mulTime31 ) ) );
			else if( _HurtAmount < 1.0 )
				ifLocalVar28 = lerpResult26;
			float4 EMISSION10 = ifLocalVar28;
			o.Emission = EMISSION10.rgb;
			float2 uv_Metallic = i.uv_texcoord * _Metallic_ST.xy + _Metallic_ST.zw;
			float4 METALLIC9 = tex2D( _Metallic, uv_Metallic );
			o.Metallic = METALLIC9.r;
			float2 uv_Roughness = i.uv_texcoord * _Roughness_ST.xy + _Roughness_ST.zw;
			float3 ROUGHNESS8 = ( 1.0 - saturate( ( UnpackNormal( tex2D( _Roughness, uv_Roughness ) ) * _Roughness_Value ) ) );
			o.Smoothness = ROUGHNESS8.x;
			float2 uv_AO = i.uv_texcoord * _AO_ST.xy + _AO_ST.zw;
			float4 AO22 = tex2D( _AO, uv_AO );
			o.Occlusion = AO22.r;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18900
1389.333;798;480;269.6667;2392.418;40.11264;1;False;False
Node;AmplifyShaderEditor.RangedFloatNode;27;-1903.133,726.1638;Inherit;False;Property;_HurtAmount;HurtAmount;8;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;36;-1697.616,888.3698;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;3;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;31;-1572.346,887.916;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;24;-1800.367,538.5088;Inherit;False;Constant;_EmissionColor;Emission Color;8;0;Create;True;0;0;0;False;0;False;1,0,0,0;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;4;-2067.043,338.9653;Inherit;True;Property;_Emission;Emission;3;0;Create;True;0;0;0;False;0;False;-1;1ec3941da536cf847af1937454373696;1ec3941da536cf847af1937454373696;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;2;-1791.678,-191.1622;Inherit;True;Property;_Roughness;Roughness;1;0;Create;True;0;0;0;False;0;False;-1;None;f9937fa5b10f9e241b68db4650777914;True;0;False;white;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SinOpNode;30;-1390.39,665.543;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;25;-1536.104,438.5272;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMinOpNode;38;-1523.616,547.3698;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;15;-1710.913,9.040758;Inherit;False;Property;_Roughness_Value;Roughness_Value;6;0;Create;True;0;0;0;False;0;False;0;1.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;35;-1214.616,586.3698;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;14;-1480.939,-146.7893;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;26;-1335.358,365.9168;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;33;-1026.424,362.7404;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;16;-1269.452,-158.6308;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.OneMinusNode;20;-1108.993,-161.2966;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;5;-1790.013,-390.8845;Inherit;True;Property;_Normal;Normal;4;0;Create;True;0;0;0;False;0;False;-1;None;7b7c3e1c2ebcfc744bb3f2c9a382cdcc;True;0;False;white;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;1;-1778.95,-613.7179;Inherit;True;Property;_Albedo;Albedo;0;0;Create;True;0;0;0;False;0;False;-1;None;7e14a61d5c412f3459500cd178c31671;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;21;-1748.149,1069.759;Inherit;True;Property;_AO;AO;7;0;Create;True;0;0;0;False;0;False;-1;None;f6149a9ee53538b45b8d1d15feeb32fa;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;3;-1790.025,163.549;Inherit;True;Property;_Metallic;Metallic;2;0;Create;True;0;0;0;False;0;False;-1;None;b550843b2cb494844890659f08a7dcb1;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ConditionalIfNode;28;-784.684,347.1172;Inherit;True;False;5;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;9;-1415.718,201.1655;Inherit;False;METALLIC;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;8;-942.3548,-171.9371;Inherit;False;ROUGHNESS;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;7;-1432.315,-334.3137;Inherit;False;NORMAL;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;22;-1443.751,1074.128;Inherit;False;AO;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;6;-1446.947,-580.0175;Inherit;False;ALBEDO;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;10;-483.865,272.2217;Inherit;True;EMISSION;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;23;-63.0943,394.8929;Inherit;False;22;AO;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;18;-73.04565,210.3387;Inherit;False;9;METALLIC;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;12;-80.38947,35.90474;Inherit;False;7;NORMAL;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;17;-76.04565,124.3387;Inherit;False;10;EMISSION;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;11;-87.90414,-43.34298;Inherit;False;6;ALBEDO;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;19;-77.04565,295.3387;Inherit;False;8;ROUGHNESS;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;13;-1999.378,-336.2192;Inherit;False;Property;_Normal_Scale;Normal_Scale;5;0;Create;True;0;0;0;False;0;False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;432.3946,-44.40845;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;RightArm;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;36;0;27;0
WireConnection;31;0;36;0
WireConnection;30;0;31;0
WireConnection;25;0;4;0
WireConnection;25;1;24;0
WireConnection;38;0;27;0
WireConnection;35;0;30;0
WireConnection;14;0;2;0
WireConnection;14;1;15;0
WireConnection;26;0;4;0
WireConnection;26;1;25;0
WireConnection;26;2;38;0
WireConnection;33;0;26;0
WireConnection;33;1;35;0
WireConnection;16;0;14;0
WireConnection;20;0;16;0
WireConnection;28;0;27;0
WireConnection;28;3;33;0
WireConnection;28;4;26;0
WireConnection;9;0;3;0
WireConnection;8;0;20;0
WireConnection;7;0;5;0
WireConnection;22;0;21;0
WireConnection;6;0;1;0
WireConnection;10;0;28;0
WireConnection;0;0;11;0
WireConnection;0;1;12;0
WireConnection;0;2;17;0
WireConnection;0;3;18;0
WireConnection;0;4;19;0
WireConnection;0;5;23;0
ASEEND*/
//CHKSM=6FA7DCC75B789590557DD41D76835AFECB159D71