// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "DoorPanel"
{
	Properties
	{
		_Albedo("Albedo", 2D) = "white" {}
		_Hand_Emissive("Hand_Emissive", 2D) = "white" {}
		_Acces_Granted("Acces_Granted", 2D) = "white" {}
		_Big_Light("Big_Light", 2D) = "white" {}
		_Card_Required("Card_Required", 2D) = "white" {}
		_Hand_Color("Hand_Color", Color) = (0,0,0,0)
		_Acces_Granted_Color("Acces_Granted_Color", Color) = (0,0,0,0)
		_Card_Required_Color("Card_Required_Color", Color) = (0,0,0,0)
		_Big_Light_Color("Big_Light_Color", Color) = (0,0,0,0)
		_Hand_Intensity("Hand_Intensity", Float) = 0
		_Acces_Granted_intensity("Acces_Granted_intensity", Float) = 0
		_Card_Required_Intensity("Card_Required_Intensity", Float) = 0
		_Big_Light_Intensity("Big_Light_Intensity", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf StandardSpecular keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform sampler2D _Acces_Granted;
		uniform float4 _Acces_Granted_ST;
		uniform float _Acces_Granted_intensity;
		uniform float4 _Acces_Granted_Color;
		uniform sampler2D _Big_Light;
		uniform float4 _Big_Light_ST;
		uniform float _Big_Light_Intensity;
		uniform float4 _Big_Light_Color;
		uniform sampler2D _Card_Required;
		uniform float4 _Card_Required_ST;
		uniform float _Card_Required_Intensity;
		uniform float4 _Card_Required_Color;
		uniform sampler2D _Hand_Emissive;
		uniform float4 _Hand_Emissive_ST;
		uniform float _Hand_Intensity;
		uniform float4 _Hand_Color;

		void surf( Input i , inout SurfaceOutputStandardSpecular o )
		{
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			float4 Albedo9 = tex2D( _Albedo, uv_Albedo );
			o.Albedo = Albedo9.rgb;
			float2 uv_Acces_Granted = i.uv_texcoord * _Acces_Granted_ST.xy + _Acces_Granted_ST.zw;
			float4 Access33 = saturate( ( ( tex2D( _Acces_Granted, uv_Acces_Granted ) * _Acces_Granted_intensity ) * _Acces_Granted_Color ) );
			float2 uv_Big_Light = i.uv_texcoord * _Big_Light_ST.xy + _Big_Light_ST.zw;
			float4 Big_Light42 = saturate( ( ( tex2D( _Big_Light, uv_Big_Light ) * _Big_Light_Intensity ) * _Big_Light_Color ) );
			float2 uv_Card_Required = i.uv_texcoord * _Card_Required_ST.xy + _Card_Required_ST.zw;
			float4 Card_Required40 = saturate( ( ( tex2D( _Card_Required, uv_Card_Required ) * _Card_Required_Intensity ) * _Card_Required_Color ) );
			float2 uv_Hand_Emissive = i.uv_texcoord * _Hand_Emissive_ST.xy + _Hand_Emissive_ST.zw;
			float4 Hand19 = saturate( ( ( tex2D( _Hand_Emissive, uv_Hand_Emissive ) * _Hand_Intensity ) * _Hand_Color ) );
			o.Emission = ( Access33 + Big_Light42 + Card_Required40 + Hand19 ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18900
1086;72.66667;971.3333;833.6667;1458.844;318.0142;1.858418;True;False
Node;AmplifyShaderEditor.SamplerNode;27;-2216.393,1479.659;Inherit;True;Property;_Acces_Granted;Acces_Granted;3;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;26;-2089.635,1261.178;Inherit;False;Property;_Hand_Intensity;Hand_Intensity;13;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;32;-2098.755,1679.676;Inherit;False;Property;_Acces_Granted_intensity;Acces_Granted_intensity;14;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;37;-2225.46,1881.232;Inherit;True;Property;_Card_Required;Card_Required;5;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;3;-2207.273,1058.894;Inherit;True;Property;_Hand_Emissive;Hand_Emissive;2;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;47;-2196.878,2512.937;Inherit;False;Property;_Big_Light_Intensity;Big_Light_Intensity;16;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;46;-2251.043,2292.519;Inherit;True;Property;_Big_Light;Big_Light;4;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;38;-2171.295,2103.917;Inherit;False;Property;_Card_Required_Intensity;Card_Required_Intensity;15;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;25;-1821.408,1106.703;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;41;-1915.187,2496.493;Inherit;False;Property;_Big_Light_Color;Big_Light_Color;12;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;28;-1830.525,1527.468;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;45;-1865.178,2344.862;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;39;-1889.602,2085.206;Inherit;False;Property;_Card_Required_Color;Card_Required_Color;11;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;16;-1871.416,1258.335;Inherit;False;Property;_Hand_Color;Hand_Color;9;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;34;-1839.594,1933.575;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;29;-1880.533,1679.1;Inherit;False;Property;_Acces_Granted_Color;Acces_Granted_Color;10;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;15;-1609.849,1156.67;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;30;-1618.966,1577.435;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;44;-1653.62,2394.829;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;35;-1628.035,1983.542;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;36;-1478.472,1998.649;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;43;-1504.057,2409.936;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;31;-1469.403,1592.542;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;18;-1460.286,1171.777;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;19;-1274.844,1170.782;Inherit;False;Hand;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;42;-1314.08,2404.407;Inherit;False;Big_Light;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;33;-1283.961,1591.547;Inherit;False;Access;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;1;-2082.921,-509.8905;Inherit;True;Property;_Albedo;Albedo;0;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;40;-1288.496,1993.12;Inherit;False;Card_Required;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;56;-1017.084,170.8706;Inherit;False;19;Hand;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;55;-1017.084,86.58859;Inherit;False;40;Card_Required;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;9;-1674.303,-464.7383;Inherit;False;Albedo;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;53;-1006.381,-91.34023;Inherit;False;33;Access;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;54;-1006.381,-5.72037;Inherit;False;42;Big_Light;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;2;-2079.87,-302.4565;Inherit;True;Property;_Normal;Normal;1;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;61;-1948.757,371.028;Inherit;False;Property;_Roughness_Multiplier;Roughness_Multiplier;17;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;52;-332.5639,315.9796;Inherit;False;13;AO;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;5;-2071.322,147.9287;Inherit;True;Property;_Roughness;Roughness;7;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;6;-2042.04,521.3642;Inherit;True;Property;_AO;AO;8;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;11;-1701.895,53.99249;Inherit;False;Metallnes;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;60;-1752.084,201.7544;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;12;-1261.324,174.6565;Inherit;False;Roughness;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;49;-344.4013,-43.84096;Inherit;False;10;Normal;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;4;-2071.321,-72.87124;Inherit;True;Property;_Metallnes;Metallnes;6;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;50;-333.8498,169.3915;Inherit;False;11;Metallnes;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;14;-1585.456,204.912;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;13;-1647.542,618.0474;Inherit;False;AO;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;51;-332.5639,247.8291;Inherit;False;12;Roughness;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;48;-348.2589,-108.1341;Inherit;False;9;Albedo;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;57;-734.8057,26.38705;Inherit;False;4;4;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;10;-1688.099,-227.4466;Inherit;False;Normal;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;58;-1426.91,200.991;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;-1;2;ASEMaterialInspector;0;0;StandardSpecular;DoorPanel;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;4;False;-1;1;False;-1;0;0;False;-1;0;False;-1;1;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;25;0;3;0
WireConnection;25;1;26;0
WireConnection;28;0;27;0
WireConnection;28;1;32;0
WireConnection;45;0;46;0
WireConnection;45;1;47;0
WireConnection;34;0;37;0
WireConnection;34;1;38;0
WireConnection;15;0;25;0
WireConnection;15;1;16;0
WireConnection;30;0;28;0
WireConnection;30;1;29;0
WireConnection;44;0;45;0
WireConnection;44;1;41;0
WireConnection;35;0;34;0
WireConnection;35;1;39;0
WireConnection;36;0;35;0
WireConnection;43;0;44;0
WireConnection;31;0;30;0
WireConnection;18;0;15;0
WireConnection;19;0;18;0
WireConnection;42;0;43;0
WireConnection;33;0;31;0
WireConnection;40;0;36;0
WireConnection;9;0;1;0
WireConnection;11;0;4;0
WireConnection;60;0;5;0
WireConnection;60;1;61;0
WireConnection;12;0;58;0
WireConnection;14;0;60;0
WireConnection;13;0;6;0
WireConnection;57;0;53;0
WireConnection;57;1;54;0
WireConnection;57;2;55;0
WireConnection;57;3;56;0
WireConnection;10;0;2;0
WireConnection;58;0;14;0
WireConnection;0;0;48;0
WireConnection;0;2;57;0
ASEEND*/
//CHKSM=557BFC5B5F774C1F838003BBA5FCFBA4602A909D