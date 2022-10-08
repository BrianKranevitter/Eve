// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Tripod"
{
	Properties
	{
		_Albedo("Albedo", 2D) = "white" {}
		_Normal("Normal", 2D) = "bump" {}
		_Emissive("Emissive", 2D) = "white" {}
		_Metalness("Metalness", 2D) = "white" {}
		_Occlusion("Occlusion", 2D) = "white" {}
		_Roughness("Roughness", 2D) = "white" {}
		[HDR]_EmissionColor("Emission Color", Color) = (2.118547,2.118547,2.118547,0)
		_Offset("Offset", Range( 0 , 1)) = 0.2
		_RoughnessIntensity("RoughnessIntensity", Float) = 0
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
		uniform sampler2D _Emissive;
		uniform float4 _Emissive_ST;
		uniform float _Offset;
		uniform float4 _EmissionColor;
		uniform sampler2D _Metalness;
		uniform float4 _Metalness_ST;
		uniform sampler2D _Roughness;
		uniform float4 _Roughness_ST;
		uniform float _RoughnessIntensity;
		uniform sampler2D _Occlusion;
		uniform float4 _Occlusion_ST;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			float3 Normal9 = UnpackNormal( tex2D( _Normal, uv_Normal ) );
			o.Normal = Normal9;
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			float4 Albedo8 = tex2D( _Albedo, uv_Albedo );
			o.Albedo = Albedo8.rgb;
			float2 uv_Emissive = i.uv_texcoord * _Emissive_ST.xy + _Emissive_ST.zw;
			float4 tex2DNode3 = tex2D( _Emissive, uv_Emissive );
			float mulTime44 = _Time.y * 3.0;
			float mulTime56 = _Time.y * 2.0;
			float4 Emissive10 = saturate( ( ( tex2DNode3 * ( saturate( ( sin( mulTime44 ) - 0.2 ) ) + _Offset ) * _EmissionColor ) + ( tex2DNode3 * saturate( ( ( sin( ( mulTime56 + -1.3 ) ) * 2.0 ) - 1.5 ) ) * _EmissionColor ) ) );
			o.Emission = Emissive10.rgb;
			float2 uv_Metalness = i.uv_texcoord * _Metalness_ST.xy + _Metalness_ST.zw;
			float4 Metalness12 = tex2D( _Metalness, uv_Metalness );
			o.Metallic = Metalness12.r;
			float2 uv_Roughness = i.uv_texcoord * _Roughness_ST.xy + _Roughness_ST.zw;
			float4 Roughness11 = saturate( ( 1.0 - ( tex2D( _Roughness, uv_Roughness ) * _RoughnessIntensity ) ) );
			o.Smoothness = Roughness11.r;
			float2 uv_Occlusion = i.uv_texcoord * _Occlusion_ST.xy + _Occlusion_ST.zw;
			float4 AmbientOcclusion13 = tex2D( _Occlusion, uv_Occlusion );
			o.Occlusion = AmbientOcclusion13.r;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18900
1298.667;72.66667;740.6667;1015;536.1901;421.934;2.027835;True;False
Node;AmplifyShaderEditor.CommentaryNode;73;-1290.993,1261.374;Inherit;False;3424.965;867.7452;;25;29;10;35;31;72;3;74;71;70;67;63;65;53;68;62;64;57;44;59;54;56;60;55;76;77;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;55;-1245.407,1882.165;Inherit;False;Constant;_Float3;Float 3;6;0;Create;True;0;0;0;False;0;False;2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;60;-846.8015,1949.338;Inherit;False;Constant;_Float0;Float 0;6;0;Create;True;0;0;0;False;0;False;-1.3;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;56;-1069.472,1877.228;Inherit;True;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;54;-491.0986,1551.184;Inherit;False;Constant;_Float2;Float 2;6;0;Create;True;0;0;0;False;0;False;3;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;59;-643.9868,1879.385;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;44;-317.9955,1547.378;Inherit;True;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;53;-132.5769,1546.469;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;68;49.18146,1632.308;Inherit;False;Constant;_Float5;Float 5;6;0;Create;True;0;0;0;False;0;False;0.2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;57;-416.6429,1884.182;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;64;-223.1742,1964.841;Inherit;False;Constant;_Float1;Float 1;6;0;Create;True;0;0;0;False;0;False;2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;65;158.8576,1931.29;Inherit;False;Constant;_Float4;Float 4;6;0;Create;True;0;0;0;False;0;False;1.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;62;-58.64273,1874.553;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;67;202.2265,1545.363;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;71;561.7823,1420.01;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;63;352.3608,1873.671;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;77;506.8558,1516.993;Inherit;False;Property;_Offset;Offset;7;0;Create;True;0;0;0;False;0;False;0.2;0.2;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;76;901.3568,1365.318;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.2;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;6;-246.5863,435.2664;Inherit;True;Property;_Roughness;Roughness;5;0;Create;True;0;0;0;False;0;False;-1;None;2ad6219567d73e04a840932c45803b2e;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;3;815.3575,1492.032;Inherit;True;Property;_Emissive;Emissive;2;0;Create;True;0;0;0;False;0;False;-1;b7a0928a4106d4ef5a518a669245c5eb;83e0762cfe135694f8dad56359371256;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;80;-130.4041,647.2455;Inherit;False;Property;_RoughnessIntensity;RoughnessIntensity;8;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;70;648.9304,1896.519;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;74;667.5728,1687.357;Inherit;False;Property;_EmissionColor;Emission Color;6;1;[HDR];Create;True;0;0;0;False;0;False;2.118547,2.118547,2.118547,0;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;79;87.58832,479.7051;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;72;1180.84,1850.846;Inherit;True;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;29;1140.636,1372.384;Inherit;True;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;78;252.0251,482.1276;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;31;1494.788,1570.287;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;4;-251.3241,5.50889;Inherit;True;Property;_Metalness;Metalness;3;0;Create;True;0;0;0;False;0;False;-1;None;5e325fe983d78b44397ce73423753f9e;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;2;-245.5679,-212.3358;Inherit;True;Property;_Normal;Normal;1;0;Create;True;0;0;0;False;0;False;-1;None;399e04f1fbef8ee48aa62be963d17a9d;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;1;-234.0118,-415.8903;Inherit;True;Property;_Albedo;Albedo;0;0;Create;True;0;0;0;False;0;False;-1;None;16010ddb970fce949af4caaf2f9dcc03;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;5;-249.7439,213.1131;Inherit;True;Property;_Occlusion;Occlusion;4;0;Create;True;0;0;0;False;0;False;-1;None;8d8349e5931d2ee45a78ae3b1d3cd9ca;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;83;426.8903,466.4224;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;35;1745.734,1579.095;Inherit;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;12;83.32918,3.857094;Inherit;False;Metalness;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;8;112.273,-394.4942;Inherit;False;Albedo;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;9;91.94842,-198.4695;Inherit;False;Normal;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;10;1971.8,1585.51;Inherit;False;Emissive;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;11;579.2186,444.2355;Inherit;False;Roughness;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;13;80.39419,211.2844;Inherit;False;AmbientOcclusion;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;16;627.4796,-40.29929;Inherit;False;12;Metalness;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;7;635.3261,-325.2801;Inherit;False;8;Albedo;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;18;608.6188,148.3035;Inherit;False;13;AmbientOcclusion;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;15;632.8682,-137.2951;Inherit;False;10;Emissive;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;17;624.7852,59.39069;Inherit;False;11;Roughness;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;14;635.5624,-234.291;Inherit;False;9;Normal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;864.7805,-235.5391;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;Tripod;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;56;0;55;0
WireConnection;59;0;56;0
WireConnection;59;1;60;0
WireConnection;44;0;54;0
WireConnection;53;0;44;0
WireConnection;57;0;59;0
WireConnection;62;0;57;0
WireConnection;62;1;64;0
WireConnection;67;0;53;0
WireConnection;67;1;68;0
WireConnection;71;0;67;0
WireConnection;63;0;62;0
WireConnection;63;1;65;0
WireConnection;76;0;71;0
WireConnection;76;1;77;0
WireConnection;70;0;63;0
WireConnection;79;0;6;0
WireConnection;79;1;80;0
WireConnection;72;0;3;0
WireConnection;72;1;70;0
WireConnection;72;2;74;0
WireConnection;29;0;3;0
WireConnection;29;1;76;0
WireConnection;29;2;74;0
WireConnection;78;0;79;0
WireConnection;31;0;29;0
WireConnection;31;1;72;0
WireConnection;83;0;78;0
WireConnection;35;0;31;0
WireConnection;12;0;4;0
WireConnection;8;0;1;0
WireConnection;9;0;2;0
WireConnection;10;0;35;0
WireConnection;11;0;83;0
WireConnection;13;0;5;0
WireConnection;0;0;7;0
WireConnection;0;1;14;0
WireConnection;0;2;15;0
WireConnection;0;3;16;0
WireConnection;0;4;17;0
WireConnection;0;5;18;0
ASEEND*/
//CHKSM=96D6B75B6B83CA84C433E57765949E9A54F0F25B