// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "FaceScan"
{
	Properties
	{
		_Albedo("Albedo", 2D) = "white" {}
		_Roughness("Roughness", 2D) = "white" {}
		_Metallic("Metallic", 2D) = "white" {}
		_Emission("Emission", 2D) = "white" {}
		_AO("AO", 2D) = "white" {}
		_Normal("Normal", 2D) = "white" {}
		_Roughness_Intensity("Roughness_Intensity", Float) = 0
		_Normal_Intensity("Normal_Intensity", Float) = 0
		_Emission_Bar("Emission_Bar", 2D) = "white" {}
		_ScanBar_Strenght("ScanBar_Strenght", Float) = 10
		_ScanBar_Value("ScanBar_Value", Range( 0 , 1)) = 1
		_Bar_Color("Bar_Color", Color) = (0,0,0,0)
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
		uniform float _Normal_Intensity;
		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform sampler2D _Emission;
		uniform float4 _Emission_ST;
		uniform float4 _Bar_Color;
		uniform sampler2D _Emission_Bar;
		uniform float4 _Emission_Bar_ST;
		uniform float _ScanBar_Strenght;
		uniform float _ScanBar_Value;
		uniform sampler2D _Metallic;
		uniform float4 _Metallic_ST;
		uniform sampler2D _Roughness;
		uniform float4 _Roughness_ST;
		uniform float _Roughness_Intensity;
		uniform sampler2D _AO;
		uniform float4 _AO_ST;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			float3 NORMAL12 = UnpackScaleNormal( tex2D( _Normal, uv_Normal ), _Normal_Intensity );
			o.Normal = NORMAL12;
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			float4 ALBEDO7 = tex2D( _Albedo, uv_Albedo );
			o.Albedo = ALBEDO7.rgb;
			float2 uv_Emission = i.uv_texcoord * _Emission_ST.xy + _Emission_ST.zw;
			float4 Emission_Texture10 = tex2D( _Emission, uv_Emission );
			float2 uv_Emission_Bar = i.uv_texcoord * _Emission_Bar_ST.xy + _Emission_Bar_ST.zw;
			float4 lerpResult26 = lerp( float4( 0,0,0,0 ) , ( _Bar_Color * tex2D( _Emission_Bar, uv_Emission_Bar ) ) , saturate( ( 1.0 - ( ( ( i.uv_texcoord.x * _ScanBar_Strenght ) + 1.0 ) - ( _ScanBar_Value * 11.0 ) ) ) ));
			float4 Emission40 = ( Emission_Texture10 + lerpResult26 );
			o.Emission = Emission40.rgb;
			float2 uv_Metallic = i.uv_texcoord * _Metallic_ST.xy + _Metallic_ST.zw;
			float4 Metallic9 = tex2D( _Metallic, uv_Metallic );
			o.Metallic = Metallic9.r;
			float2 uv_Roughness = i.uv_texcoord * _Roughness_ST.xy + _Roughness_ST.zw;
			float4 Roughness8 = ( ( 1.0 - tex2D( _Roughness, uv_Roughness ) ) * _Roughness_Intensity );
			o.Smoothness = Roughness8.r;
			float2 uv_AO = i.uv_texcoord * _AO_ST.xy + _AO_ST.zw;
			float4 AO11 = tex2D( _AO, uv_AO );
			o.Occlusion = AO11.r;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18900
1152;580;812;505;3243.993;-1287.889;1.561829;True;False
Node;AmplifyShaderEditor.RangedFloatNode;29;-3231.145,2072.295;Inherit;False;Property;_ScanBar_Strenght;ScanBar_Strenght;9;0;Create;True;0;0;0;False;0;False;10;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;32;-3273.205,1897.126;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;28;-3009.54,1971.107;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;31;-3071.667,2264.517;Inherit;False;Property;_ScanBar_Value;ScanBar_Value;10;0;Create;True;0;0;0;False;0;False;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;39;-2808.287,2173.061;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;11;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;37;-2808.668,1995.402;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;34;-2648.54,1998.267;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;43;-2652.059,1415.959;Inherit;False;Property;_Bar_Color;Bar_Color;11;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;33;-2461.467,1895.397;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;25;-2638.555,1600.312;Inherit;True;Property;_Emission_Bar;Emission_Bar;8;0;Create;True;0;0;0;False;0;False;-1;26268d6fbb291974d861697eb308aae2;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;4;-1676.746,241.5894;Inherit;True;Property;_Emission;Emission;3;0;Create;True;0;0;0;False;0;False;-1;b5186d4f2accce54d90896dba0a6ce10;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;10;-1364.158,264.0581;Inherit;False;Emission_Texture;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;41;-2194.101,1850.482;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;2;-1756.804,-420.5826;Inherit;True;Property;_Roughness;Roughness;1;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;42;-2260.04,1586.198;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;22;-1916.375,798.7472;Inherit;False;Property;_Normal_Intensity;Normal_Intensity;7;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;23;-1652.904,1382.05;Inherit;False;10;Emission_Texture;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;20;-1450.899,-353.7716;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;19;-1705.016,-192.539;Inherit;False;Property;_Roughness_Intensity;Roughness_Intensity;6;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;26;-2060.422,1619.984;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;5;-1697.277,505.0745;Inherit;True;Property;_AO;AO;4;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;3;-1705.832,32.85497;Inherit;True;Property;_Metallic;Metallic;2;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;24;-1361.847,1468.315;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;6;-1705.832,756.5831;Inherit;True;Property;_Normal;Normal;5;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;1;-1760.622,-623.2291;Inherit;True;Property;_Albedo;Albedo;0;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;21;-1294.596,-233.6368;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;8;-1104.415,-236.2435;Inherit;False;Roughness;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;11;-1379.68,536.2992;Inherit;False;AO;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;9;-1365.352,58.68346;Inherit;False;Metallic;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;12;-1378.543,758.7009;Inherit;False;NORMAL;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;40;-1118.535,1510.527;Inherit;False;Emission;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;7;-1436.323,-598.6052;Inherit;False;ALBEDO;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;16;554.7939,237.1469;Inherit;False;9;Metallic;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;15;550.7939,163.1469;Inherit;False;40;Emission;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;14;556.7939,80.14688;Inherit;False;12;NORMAL;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;13;548.7939,9.146881;Inherit;False;7;ALBEDO;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;18;567.7939,419.1469;Inherit;False;11;AO;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;17;553.7939,326.1469;Inherit;False;8;Roughness;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;799.0979,17.00208;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;FaceScan;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;28;0;32;1
WireConnection;28;1;29;0
WireConnection;39;0;31;0
WireConnection;37;0;28;0
WireConnection;34;0;37;0
WireConnection;34;1;39;0
WireConnection;33;0;34;0
WireConnection;10;0;4;0
WireConnection;41;0;33;0
WireConnection;42;0;43;0
WireConnection;42;1;25;0
WireConnection;20;0;2;0
WireConnection;26;1;42;0
WireConnection;26;2;41;0
WireConnection;24;0;23;0
WireConnection;24;1;26;0
WireConnection;6;5;22;0
WireConnection;21;0;20;0
WireConnection;21;1;19;0
WireConnection;8;0;21;0
WireConnection;11;0;5;0
WireConnection;9;0;3;0
WireConnection;12;0;6;0
WireConnection;40;0;24;0
WireConnection;7;0;1;0
WireConnection;0;0;13;0
WireConnection;0;1;14;0
WireConnection;0;2;15;0
WireConnection;0;3;16;0
WireConnection;0;4;17;0
WireConnection;0;5;18;0
ASEEND*/
//CHKSM=AED07FA79002D43AE04A2239CC54DC8379903931