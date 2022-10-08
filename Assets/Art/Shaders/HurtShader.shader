// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "HurtShader"
{
	Properties
	{
		_BigSplats("BigSplats", 2D) = "white" {}
		_MidSplats("MidSplats", 2D) = "white" {}
		_Splats3("Splats3", Range( 0 , 1)) = 1
		_Splats2("Splats2", Range( 0 , 1)) = 0.8572289
		_Splats1("Splats1", Range( 0 , 1)) = 1
		_LargeSplats("LargeSplats", 2D) = "white" {}
		_Blood_Color("Blood_Color", Color) = (1,0,0,0)
		_Screen_Center("Screen_Center", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}

	}

	SubShader
	{
		LOD 0

		Cull Off
		ZWrite Off
		ZTest Always
		
		Pass
		{
			CGPROGRAM

			

			#pragma vertex Vert
			#pragma fragment Frag
			#pragma target 3.0

			#include "UnityCG.cginc"
			
		
			struct ASEAttributesDefault
			{
				float3 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				
			};

			struct ASEVaryingsDefault
			{
				float4 vertex : SV_POSITION;
				float2 texcoord : TEXCOORD0;
				float2 texcoordStereo : TEXCOORD1;
			#if STEREO_INSTANCING_ENABLED
				uint stereoTargetEyeIndex : SV_RenderTargetArrayIndex;
			#endif
				
			};

			uniform sampler2D _MainTex;
			uniform half4 _MainTex_TexelSize;
			uniform half4 _MainTex_ST;
			
			uniform float4 _Blood_Color;
			uniform sampler2D _LargeSplats;
			uniform float4 _LargeSplats_ST;
			uniform float _Splats1;
			uniform sampler2D _BigSplats;
			uniform float4 _BigSplats_ST;
			uniform float _Splats2;
			uniform sampler2D _MidSplats;
			uniform float4 _MidSplats_ST;
			uniform float _Splats3;
			uniform sampler2D _Screen_Center;
			uniform float4 _Screen_Center_ST;


			
			float2 TransformTriangleVertexToUV (float2 vertex)
			{
				float2 uv = (vertex + 1.0) * 0.5;
				return uv;
			}

			ASEVaryingsDefault Vert( ASEAttributesDefault v  )
			{
				ASEVaryingsDefault o;
				o.vertex = float4(v.vertex.xy, 0.0, 1.0);
				o.texcoord = TransformTriangleVertexToUV (v.vertex.xy);
#if UNITY_UV_STARTS_AT_TOP
				o.texcoord = o.texcoord * float2(1.0, -1.0) + float2(0.0, 1.0);
#endif
				o.texcoordStereo = TransformStereoScreenSpaceTex (o.texcoord, 1.0);

				v.texcoord = o.texcoordStereo;
				float4 ase_ppsScreenPosVertexNorm = float4(o.texcoordStereo,0,1);

				

				return o;
			}

			float4 Frag (ASEVaryingsDefault i  ) : SV_Target
			{
				float4 ase_ppsScreenPosFragNorm = float4(i.texcoordStereo,0,1);

				float2 uv_MainTex = i.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				float2 uv_LargeSplats = i.texcoord.xy * _LargeSplats_ST.xy + _LargeSplats_ST.zw;
				float4 temp_cast_0 = (( 1.0 - _Splats1 )).xxxx;
				float2 uv_BigSplats = i.texcoord.xy * _BigSplats_ST.xy + _BigSplats_ST.zw;
				float4 temp_cast_1 = (( 1.0 - _Splats2 )).xxxx;
				float2 uv_MidSplats = i.texcoord.xy * _MidSplats_ST.xy + _MidSplats_ST.zw;
				float4 temp_cast_2 = (( 1.0 - _Splats3 )).xxxx;
				float2 uv_Screen_Center = i.texcoord.xy * _Screen_Center_ST.xy + _Screen_Center_ST.zw;
				

				float4 color = saturate( ( tex2D( _MainTex, uv_MainTex ) + saturate( ( saturate( ( _Blood_Color * ( saturate( (float4( 0,0,0,0 ) + (tex2D( _LargeSplats, uv_LargeSplats ) - temp_cast_0) * (float4( 1,1,1,1 ) - float4( 0,0,0,0 )) / (float4( 1,1,1,1 ) - temp_cast_0)) ) + saturate( (float4( 0,0,0,0 ) + (tex2D( _BigSplats, uv_BigSplats ) - temp_cast_1) * (float4( 1,1,1,1 ) - float4( 0,0,0,0 )) / (float4( 1,1,1,1 ) - temp_cast_1)) ) + saturate( (float4( 0,0,0,0 ) + (tex2D( _MidSplats, uv_MidSplats ) - temp_cast_2) * (float4( 1,1,1,1 ) - float4( 0,0,0,0 )) / (float4( 1,1,1,1 ) - temp_cast_2)) ) ) ) ) - tex2D( _Screen_Center, uv_Screen_Center ) ) ) ) );
				
				return color;
			}
			ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	
}
/*ASEBEGIN
Version=18900
624;72.66667;1197.333;789;911.5377;1091.443;1.997624;True;False
Node;AmplifyShaderEditor.RangedFloatNode;9;-1628.355,352.3275;Inherit;False;Property;_Splats3;Splats3;2;0;Create;True;0;0;0;False;0;False;1;0.8;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;10;-1641.42,-83.37782;Inherit;False;Property;_Splats2;Splats2;3;0;Create;True;0;0;0;False;0;False;0.8572289;0.8;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;15;-1682.708,-545.4914;Inherit;False;Property;_Splats1;Splats1;4;0;Create;True;0;0;0;False;0;False;1;0.8;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;14;-1565.416,-777.3786;Inherit;True;Property;_LargeSplats;LargeSplats;5;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;2;-1540.042,-350.9802;Inherit;True;Property;_BigSplats;BigSplats;0;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;4;-1535.387,82.09218;Inherit;True;Property;_MidSplats;MidSplats;1;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;22;-1378.844,-573.4023;Inherit;False;2;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;23;-1363.244,343.0977;Inherit;False;2;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;21;-1349.764,-104.9319;Inherit;False;2;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;8;-1186.992,156.2968;Inherit;True;5;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,1,1,1;False;3;COLOR;0,0,0,0;False;4;COLOR;1,1,1,1;False;1;COLOR;0
Node;AmplifyShaderEditor.TFHCRemapNode;16;-1199.823,-721.9656;Inherit;True;5;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,1,1,1;False;3;COLOR;0,0,0,0;False;4;COLOR;1,1,1,1;False;1;COLOR;0
Node;AmplifyShaderEditor.TFHCRemapNode;11;-1180.935,-296.2522;Inherit;True;5;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,1,1,1;False;3;COLOR;0,0,0,0;False;4;COLOR;1,1,1,1;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;13;-825.2085,-207.2096;Inherit;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;17;-867.5075,-710.62;Inherit;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;12;-869.9023,209.1898;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;18;-553.9295,-591.6844;Inherit;False;Property;_Blood_Color;Blood_Color;6;0;Create;True;0;0;0;False;0;False;1,0,0,0;1,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;6;-546.4734,-221.1934;Inherit;True;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;19;-239.9133,-383.4821;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;27;20.47638,-388.4187;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;29;-102.3977,-139.588;Inherit;True;Property;_Screen_Center;Screen_Center;7;0;Create;True;0;0;0;False;0;False;-1;7e34bb5a484960f498957f8b91cf3ed0;7e34bb5a484960f498957f8b91cf3ed0;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TemplateShaderPropertyNode;25;-477.1483,-723.6722;Inherit;False;0;0;_MainTex;Shader;False;0;5;SAMPLER2D;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;30;221.0906,-333.8389;Inherit;True;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;24;-289.7576,-664.6209;Inherit;True;Property;_TextureSample0;Texture Sample 0;6;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;31;462.3916,-321.5518;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;26;695.0389,-458.3481;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;28;873.4476,-417.8423;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;1;1301.033,-396.0188;Float;False;True;-1;2;ASEMaterialInspector;0;10;HurtShader;d143e746ed2c3dd43907da4cf2afafc4;True;SubShader 0 Pass 0;0;0;SubShader 0 Pass 0;1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;-1;False;False;False;False;False;False;False;False;False;False;False;True;2;False;-1;True;7;False;-1;False;False;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;0;;0;0;Standard;0;0;1;True;False;;False;0
WireConnection;22;1;15;0
WireConnection;23;1;9;0
WireConnection;21;1;10;0
WireConnection;8;0;4;0
WireConnection;8;1;23;0
WireConnection;16;0;14;0
WireConnection;16;1;22;0
WireConnection;11;0;2;0
WireConnection;11;1;21;0
WireConnection;13;0;11;0
WireConnection;17;0;16;0
WireConnection;12;0;8;0
WireConnection;6;0;17;0
WireConnection;6;1;13;0
WireConnection;6;2;12;0
WireConnection;19;0;18;0
WireConnection;19;1;6;0
WireConnection;27;0;19;0
WireConnection;30;0;27;0
WireConnection;30;1;29;0
WireConnection;24;0;25;0
WireConnection;31;0;30;0
WireConnection;26;0;24;0
WireConnection;26;1;31;0
WireConnection;28;0;26;0
WireConnection;1;0;28;0
ASEEND*/
//CHKSM=7A2D244C8D25017AA1E3DAE84D1857D9B25E38EA