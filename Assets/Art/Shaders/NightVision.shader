// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "NightVision"
{
	Properties
	{
		_Color("Color", Color) = (1,0.1289307,0.8354284,0)
		_Noise_Speed("Noise_Speed", Float) = 0
		_Noise_Scale("Noise_Scale", Vector) = (80,750,0,0)
		_Remap_Min("Remap_Min", Float) = 0
		_Remap_Max("Remap_Max", Float) = 0
		_Noise_Strenght("Noise_Strenght", Float) = 0.56
		_GrayscaleValue("Grayscale Value", Float) = 0
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
			#include "UnityShaderVariables.cginc"

		
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
			
			uniform float _GrayscaleValue;
			uniform float4 _Color;
			uniform float2 _Noise_Scale;
			uniform float _Noise_Speed;
			uniform float _Noise_Strenght;
			uniform float _Remap_Min;
			uniform float _Remap_Max;


			float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }
			float snoise( float2 v )
			{
				const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
				float2 i = floor( v + dot( v, C.yy ) );
				float2 x0 = v - i + dot( i, C.xx );
				float2 i1;
				i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
				float4 x12 = x0.xyxy + C.xxzz;
				x12.xy -= i1;
				i = mod2D289( i );
				float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
				float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
				m = m * m;
				m = m * m;
				float3 x = 2.0 * frac( p * C.www ) - 1.0;
				float3 h = abs( x ) - 0.5;
				float3 ox = floor( x + 0.5 );
				float3 a0 = x - ox;
				m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
				float3 g;
				g.x = a0.x * x0.x + h.x * x0.y;
				g.yz = a0.yz * x12.xz + h.yz * x12.yw;
				return 130.0 * dot( m, g );
			}
			

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
				float grayscale29 = Luminance(tex2D( _MainTex, uv_MainTex ).rgb);
				float4 temp_cast_1 = (grayscale29).xxxx;
				float4 lerpResult27 = lerp( tex2D( _MainTex, uv_MainTex ) , temp_cast_1 , _GrayscaleValue);
				float mulTime14 = _Time.y * _Noise_Speed;
				float2 appendResult13 = (float2(mulTime14 , 0.0));
				float2 texCoord11 = i.texcoord.xy * _Noise_Scale + appendResult13;
				float simplePerlin2D10 = snoise( texCoord11 );
				simplePerlin2D10 = simplePerlin2D10*0.5 + 0.5;
				float lerpResult30 = lerp( simplePerlin2D10 , 1.0 , _Noise_Strenght);
				float4 temp_cast_2 = (_Remap_Min).xxxx;
				float4 temp_cast_3 = (_Remap_Max).xxxx;
				

				float4 color = (float4( 0,0,0,0 ) + (( ( lerpResult27 / _Color ) / saturate( lerpResult30 ) ) - temp_cast_2) * (float4( 1,1,1,1 ) - float4( 0,0,0,0 )) / (temp_cast_3 - temp_cast_2));
				
				return color;
			}
			ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	
}
/*ASEBEGIN
Version=18900
1276;725.3334;695;325.3333;599.6614;140.5147;1.448736;False;False
Node;AmplifyShaderEditor.RangedFloatNode;15;-1185.751,533.9543;Inherit;False;Property;_Noise_Speed;Noise_Speed;1;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;14;-939.4382,504.3566;Inherit;False;1;0;FLOAT;3;False;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateShaderPropertyNode;4;-1218.148,-42.27794;Inherit;False;0;0;_MainTex;Shader;False;0;5;SAMPLER2D;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;12;-686.3582,356.9555;Inherit;False;Property;_Noise_Scale;Noise_Scale;2;0;Create;True;0;0;0;False;0;False;80,750;80,750;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.DynamicAppendNode;13;-707.5193,505.5399;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TemplateShaderPropertyNode;25;-983.6154,-264.35;Inherit;False;0;0;_MainTex;Shader;False;0;5;SAMPLER2D;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;5;-1059.364,-59.71009;Inherit;True;Property;_TextureSample0;Texture Sample 0;0;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;11;-475.4291,367.636;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;22;-121.6804,594.4019;Inherit;False;Property;_Noise_Strenght;Noise_Strenght;5;0;Create;True;0;0;0;False;0;False;0.56;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;10;-177.8815,336.9295;Inherit;True;Simplex2D;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCGrayscale;29;-695.288,-59.10276;Inherit;False;0;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;26;-810.6158,-273.2524;Inherit;True;Property;_TextureSample1;Texture Sample 1;0;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;28;-672.2451,154.4227;Inherit;False;Property;_GrayscaleValue;Grayscale Value;6;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;30;78.08344,277.6746;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0.6;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;27;-431.491,-114.0599;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;7;-328.739,113.1292;Inherit;False;Property;_Color;Color;0;0;Create;True;0;0;0;False;0;False;1,0.1289307,0.8354284,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleDivideOpNode;6;104.0727,-8.250064;Inherit;True;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;23;287.0582,262.3696;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;18;497.6989,234.8705;Inherit;False;Property;_Remap_Min;Remap_Min;3;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;19;527.6989,357.8705;Inherit;False;Property;_Remap_Max;Remap_Max;4;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;31;488.3395,-4.816337;Inherit;False;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.TFHCRemapNode;17;779.4277,-27.65566;Inherit;True;5;0;COLOR;0,0,0,0;False;1;COLOR;0.318,0.318,0.318,0;False;2;COLOR;0.672,0.672,0.672,1;False;3;COLOR;0,0,0,0;False;4;COLOR;1,1,1,1;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;3;1121.937,-7.177382;Float;False;True;-1;2;ASEMaterialInspector;0;10;NightVision;d143e746ed2c3dd43907da4cf2afafc4;True;SubShader 0 Pass 0;0;0;SubShader 0 Pass 0;1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;-1;False;False;False;False;False;False;False;False;False;False;False;True;2;False;-1;True;7;False;-1;False;False;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;0;;0;0;Standard;0;0;1;True;False;;False;0
WireConnection;14;0;15;0
WireConnection;13;0;14;0
WireConnection;5;0;4;0
WireConnection;11;0;12;0
WireConnection;11;1;13;0
WireConnection;10;0;11;0
WireConnection;29;0;5;0
WireConnection;26;0;25;0
WireConnection;30;0;10;0
WireConnection;30;2;22;0
WireConnection;27;0;26;0
WireConnection;27;1;29;0
WireConnection;27;2;28;0
WireConnection;6;0;27;0
WireConnection;6;1;7;0
WireConnection;23;0;30;0
WireConnection;31;0;6;0
WireConnection;31;1;23;0
WireConnection;17;0;31;0
WireConnection;17;1;18;0
WireConnection;17;2;19;0
WireConnection;3;0;17;0
ASEEND*/
//CHKSM=3D9F8D9AD0778646ED701E6490EB85435696859C