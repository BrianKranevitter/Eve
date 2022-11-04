// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "EmissionByDistance"
{
	Properties
	{
		_Albedo("Albedo", 2D) = "white" {}
		_Normal("Normal", 2D) = "bump" {}
		_Emission("Emission", 2D) = "white" {}
		_Metallic("Metallic", 2D) = "white" {}
		_Roughness("Roughness", 2D) = "white" {}
		_AmbientOclussion("Ambient Oclussion", 2D) = "white" {}
		_PlayerPosition("PlayerPosition", Vector) = (1,1,1,0)
		_Range("Range", Float) = 10
		_RoughnessValue("Roughness Value", Float) = 0
		_Emission_Color("Emission_Color", Color) = (1,0,0,0)
		_Albedo_Tint("Albedo_Tint", Color) = (0.15,0.15,0.15,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
		};

		uniform sampler2D _Normal;
		uniform float4 _Normal_ST;
		uniform float4 _Albedo_Tint;
		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform float4 _Emission_Color;
		uniform sampler2D _Emission;
		uniform float4 _Emission_ST;
		uniform float3 _PlayerPosition;
		uniform float _Range;
		uniform sampler2D _Metallic;
		uniform float4 _Metallic_ST;
		uniform sampler2D _Roughness;
		uniform float4 _Roughness_ST;
		uniform float _RoughnessValue;
		uniform sampler2D _AmbientOclussion;
		uniform float4 _AmbientOclussion_ST;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			o.Normal = UnpackNormal( tex2D( _Normal, uv_Normal ) );
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			float4 tex2DNode1 = tex2D( _Albedo, uv_Albedo );
			o.Albedo = ( _Albedo_Tint * tex2DNode1 ).rgb;
			float2 uv_Emission = i.uv_texcoord * _Emission_ST.xy + _Emission_ST.zw;
			float grayscale32 = Luminance(tex2D( _Emission, uv_Emission ).rgb);
			float3 ase_worldPos = i.worldPos;
			float4 temp_cast_2 = (( distance( _PlayerPosition , ase_worldPos ) / _Range )).xxxx;
			float4 Emission10 = saturate( ( saturate( ( _Emission_Color * grayscale32 ) ) - temp_cast_2 ) );
			o.Emission = Emission10.rgb;
			float2 uv_Metallic = i.uv_texcoord * _Metallic_ST.xy + _Metallic_ST.zw;
			o.Metallic = tex2D( _Metallic, uv_Metallic ).r;
			float2 uv_Roughness = i.uv_texcoord * _Roughness_ST.xy + _Roughness_ST.zw;
			float4 tex2DNode5 = tex2D( _Roughness, uv_Roughness );
			o.Smoothness = saturate( ( 1.0 - ( tex2DNode5 * _RoughnessValue ) ) ).r;
			float2 uv_AmbientOclussion = i.uv_texcoord * _AmbientOclussion_ST.xy + _AmbientOclussion_ST.zw;
			o.Occlusion = tex2D( _AmbientOclussion, uv_AmbientOclussion ).r;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18900
1358;722.6667;601.3334;327.6667;1378.396;762.0426;2.288721;True;False
Node;AmplifyShaderEditor.SamplerNode;3;-2647.2,-129.8015;Inherit;True;Property;_Emission;Emission;2;0;Create;True;0;0;0;False;0;False;-1;705c87ef0f201994bbfbd28686c2f01a;ac91154fb9011f542a92af4dfe4e5c18;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCGrayscale;32;-2341.978,-132.4081;Inherit;True;0;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;14;-2514.883,91.52869;Inherit;False;Property;_PlayerPosition;PlayerPosition;6;0;Create;True;0;0;0;False;0;False;1,1,1;1,1,1;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ColorNode;34;-2365.978,-327.4081;Inherit;False;Property;_Emission_Color;Emission_Color;9;0;Create;True;0;0;0;False;0;False;1,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WorldPosInputsNode;19;-2514.12,279.2941;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DistanceOpNode;17;-2256.392,169.5227;Inherit;True;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;33;-2119.978,-98.40811;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;12;-2251.011,452.337;Inherit;False;Property;_Range;Range;7;0;Create;True;0;0;0;False;0;False;10;50;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;24;-2020.556,174.0621;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;35;-1985.978,-104.4081;Inherit;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;31;-348.3467,763.3559;Inherit;False;Property;_RoughnessValue;Roughness Value;8;0;Create;True;0;0;0;False;0;False;0;1.19;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;5;-589.1001,627.9905;Inherit;True;Property;_Roughness;Roughness;4;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;25;-1806.609,-91.2;Inherit;True;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;26;-1560.584,-90.1864;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;28;-200.1804,616.7744;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;10;-1307.505,-96.25982;Inherit;False;Emission;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;7;27.20187,539.4003;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;1;-599.8699,-273.7359;Inherit;True;Property;_Albedo;Albedo;0;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;37;-556.1462,-501.9534;Inherit;False;Property;_Albedo_Tint;Albedo_Tint;10;0;Create;True;0;0;0;False;0;False;0.15,0.15,0.15,0;0.15,0.15,0.15,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.NegateNode;27;-28.875,422.5531;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;2;-581.3674,-62.43188;Inherit;True;Property;_Normal;Normal;1;0;Create;True;0;0;0;False;0;False;-1;None;2f0ab9eadb047a2439df2d018cc59251;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;30;258.1631,441.9145;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;6;-579.1854,881.2233;Inherit;True;Property;_AmbientOclussion;Ambient Oclussion;5;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;4;-598.5263,410.5925;Inherit;True;Property;_Metallic;Metallic;3;0;Create;True;0;0;0;False;0;False;-1;None;ba00f65fa8896ee4eaf7dc7e2e599eb2;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;9;95.69241,258.5043;Inherit;False;10;Emission;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;36;-178.2875,-218.7516;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;471.3403,208.6958;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;EmissionByDistance;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;5;False;-1;10;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;32;0;3;0
WireConnection;17;0;14;0
WireConnection;17;1;19;0
WireConnection;33;0;34;0
WireConnection;33;1;32;0
WireConnection;24;0;17;0
WireConnection;24;1;12;0
WireConnection;35;0;33;0
WireConnection;25;0;35;0
WireConnection;25;1;24;0
WireConnection;26;0;25;0
WireConnection;28;0;5;0
WireConnection;28;1;31;0
WireConnection;10;0;26;0
WireConnection;7;0;28;0
WireConnection;27;0;5;0
WireConnection;30;0;7;0
WireConnection;36;0;37;0
WireConnection;36;1;1;0
WireConnection;0;0;36;0
WireConnection;0;1;2;0
WireConnection;0;2;9;0
WireConnection;0;3;4;0
WireConnection;0;4;30;0
WireConnection;0;5;6;0
ASEEND*/
//CHKSM=4C5336EC5FA1F5938675163EB00407B10E90FFDF