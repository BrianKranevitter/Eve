// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Helmet"
{
	Properties
	{
		_Albedo("Albedo", 2D) = "white" {}
		_Normal("Normal", 2D) = "white" {}
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
		uniform float _Battery_Level;
		uniform float _BatteryLevel_LowLimit;
		uniform float _BatteryLevel_HegithLimit;
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
			float4 lerpResult58 = lerp( EMISSION_prev16 , float4( 0,0,0,0 ) , ( saturate( ( 1.0 - ( ( ( 1.0 - i.uv_texcoord.y ) - ( ( ( 1.0 - _Battery_Level ) * _BatteryLevel_LowLimit ) - _BatteryLevel_HegithLimit ) ) * 143.39 ) ) ) - saturate( ( 1.0 - ( ( ( 1.0 - i.uv_texcoord.y ) - ( ( ( 1.0 - 1.0 ) * 0.1 ) - -0.864 ) ) * 50.0 ) ) ) ));
			float4 EMISSION45 = ( ( _Indicator_Color * tex2D( _Emission_Indicator, uv_Emission_Indicator ) ) + lerpResult58 );
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
1358;722.6667;601.3334;327.6667;9855.332;-423.3737;10.24286;False;False
Node;AmplifyShaderEditor.RangedFloatNode;69;-3427.182,2971.78;Inherit;False;Constant;_Float4;Float 4;8;0;Create;True;0;0;0;False;0;False;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;29;-3176.353,2452.208;Inherit;False;Property;_Battery_Level;Battery_Level;9;0;Create;True;0;0;0;False;0;False;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;62;-3084.612,2944.225;Inherit;False;2;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;78;-2915.272,2583.577;Inherit;False;Property;_BatteryLevel_LowLimit;BatteryLevel_LowLimit;12;0;Create;True;0;0;0;False;0;False;0.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;55;-2891.825,2439.163;Inherit;False;2;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;64;-2968.571,2708.957;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;52;-2738.627,2429.195;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;60;-2934.316,2983.592;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;50;-2711.204,2578.74;Inherit;False;Property;_BatteryLevel_HegithLimit;BatteryLevel_HegithLimit;10;0;Create;True;0;0;0;False;0;False;-0.9;-0.9;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;28;-2820.914,2206.087;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;66;-2895.253,3138.508;Inherit;False;Constant;_Float2;Float 2;9;0;Create;True;0;0;0;False;0;False;-0.864;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;51;-2568.412,2434.487;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;48;-2578.438,2197.345;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;61;-2674.136,3073.045;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;63;-2731.299,2788.685;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;35;-2299.603,2453.682;Inherit;False;Constant;_Float1;Float 1;8;0;Create;True;0;0;0;False;0;False;143.39;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;68;-2486.719,2856.846;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;33;-2354.674,2197.853;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;67;-2442.057,3133.492;Inherit;False;Constant;_Float3;Float 3;8;0;Create;True;0;0;0;False;0;False;50;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;65;-2201.6,2856.557;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;38;-2095.416,2199.864;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;11;-2705.275,1446.824;Inherit;True;Property;_Emission_All;Emission_All;6;0;Create;True;0;0;0;False;0;False;-1;6d840fd2be766d34f95db45d63cd06f2;6d840fd2be766d34f95db45d63cd06f2;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;70;-1928.143,2824.987;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;16;-2058.995,1438.363;Inherit;False;EMISSION_prev;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;56;-1868.36,2197.599;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;34;-1679.019,1852.606;Inherit;False;16;EMISSION_prev;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;73;-1634.358,2296.171;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;72;-1645.163,2208.556;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;4;-2695.348,211.4346;Inherit;False;Property;_Roughness_Scale;Roughness_Scale;5;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;3;-2776.914,6.933545;Inherit;True;Property;_Roughness;Roughness;2;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;59;-1456.541,2177.208;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;77;-1416.807,1490.452;Inherit;False;Property;_Indicator_Color;Indicator_Color;11;0;Create;True;0;0;0;False;0;False;1,0.9963837,0.8553458,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;74;-1460.5,1668.309;Inherit;True;Property;_Emission_Indicator;Emission_Indicator;7;0;Create;True;0;0;0;False;0;False;-1;e7e19b7b11b3ee94ca2dee5e379b9948;e7e19b7b11b3ee94ca2dee5e379b9948;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RelayNode;36;-1446.339,1872.974;Inherit;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;-2433.318,106.3503;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;6;-2632.292,782.4937;Inherit;False;Constant;_AO_Scale;AO_Scale;8;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;8;-2999.974,-152.4813;Inherit;False;Property;_Normal_Scale;Normal_Scale;4;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;58;-1186.698,1991.424;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;5;-2771.292,571.4937;Inherit;True;Property;_AO;AO;8;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;76;-1084.007,1672.85;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;10;-2298.896,84.47796;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;9;-2441.292,676.4937;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;12;-2768.625,325.6299;Inherit;True;Property;_Metallic;Metallic;3;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;1;-2818.119,-409.9683;Inherit;True;Property;_Albedo;Albedo;0;0;Create;True;0;0;0;False;0;False;-1;cc59aceb46254394bb1075a8c9581d15;cc59aceb46254394bb1075a8c9581d15;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;75;-914.3329,1938.552;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;13;-2796.304,-223.3318;Inherit;True;Property;_Normal;Normal;1;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;18;-2407.119,356.4209;Inherit;False;METALLIC;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;26;-2448.59,-411.1569;Inherit;False;ALBEDO;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;14;-2440.679,-181.4403;Inherit;False;NORMAL;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;15;-2093.563,34.70498;Inherit;False;ROUGHNESS;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;45;-682.7173,1937.035;Inherit;False;EMISSION;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;17;-2306.292,618.4937;Inherit;False;AO;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;22;-259.9581,136.9554;Inherit;False;45;EMISSION;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;24;-256.9396,279.7026;Inherit;False;15;ROUGHNESS;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;19;-247.9581,203.9554;Inherit;False;18;METALLIC;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;21;-251.9581,65.95536;Inherit;False;14;NORMAL;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;23;-243.1542,357.0975;Inherit;False;17;AO;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;20;-250.9581,-0.04466534;Inherit;False;26;ALBEDO;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;Helmet;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;62;1;69;0
WireConnection;55;1;29;0
WireConnection;52;0;55;0
WireConnection;52;1;78;0
WireConnection;60;0;62;0
WireConnection;51;0;52;0
WireConnection;51;1;50;0
WireConnection;48;0;28;2
WireConnection;61;0;60;0
WireConnection;61;1;66;0
WireConnection;63;0;64;2
WireConnection;68;0;63;0
WireConnection;68;1;61;0
WireConnection;33;0;48;0
WireConnection;33;1;51;0
WireConnection;65;0;68;0
WireConnection;65;1;67;0
WireConnection;38;0;33;0
WireConnection;38;1;35;0
WireConnection;70;0;65;0
WireConnection;16;0;11;0
WireConnection;56;0;38;0
WireConnection;73;0;70;0
WireConnection;72;0;56;0
WireConnection;59;0;72;0
WireConnection;59;1;73;0
WireConnection;36;0;34;0
WireConnection;7;0;3;0
WireConnection;7;1;4;0
WireConnection;58;0;36;0
WireConnection;58;2;59;0
WireConnection;76;0;77;0
WireConnection;76;1;74;0
WireConnection;10;0;7;0
WireConnection;9;0;5;0
WireConnection;9;1;6;0
WireConnection;75;0;76;0
WireConnection;75;1;58;0
WireConnection;13;5;8;0
WireConnection;18;0;12;0
WireConnection;26;0;1;0
WireConnection;14;0;13;0
WireConnection;15;0;10;0
WireConnection;45;0;75;0
WireConnection;17;0;9;0
WireConnection;0;0;20;0
WireConnection;0;1;21;0
WireConnection;0;2;22;0
WireConnection;0;3;19;0
WireConnection;0;4;24;0
WireConnection;0;5;23;0
ASEEND*/
//CHKSM=8DBE1C0D65FECDDEB80E592F8CEBE241A3C47CCA