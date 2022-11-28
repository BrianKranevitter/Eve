// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "HeadHologram"
{
	Properties
	{
		_Clouds_1_X_offset_Speed("Clouds_1_X_offset_Speed", Float) = 0
		_Clouds_3_X_offset_Speed("Clouds_3_X_offset_Speed", Float) = 0
		_Clouds_2_X_offset_Speed("Clouds_2_X_offset_Speed", Float) = 0
		_Clouds_1("Clouds_1", 2D) = "gray" {}
		_Texture0("Texture 0", 2D) = "gray" {}
		_Texture3("Texture 3", 2D) = "gray" {}
		_Clouds_1_Y_Offset_Speed("Clouds_1_Y_Offset_Speed", Float) = 0
		_Clouds_2_Y_Offset_Speed("Clouds_2_Y_Offset_Speed", Float) = 0
		_Clouds_3_Y_Offset_Speed("Clouds_3_Y_Offset_Speed", Float) = 0
		_Clouds_1_Remap_Frecuency("Clouds_1_Remap_Frecuency", Float) = 0
		_Clouds_3_Remap_Frecuency("Clouds_3_Remap_Frecuency", Float) = 0
		_Clouds_2_Remap_Frecuency("Clouds_2_Remap_Frecuency", Float) = 0
		_Clouds_1_Remap_Strenght("Clouds_1_Remap_Strenght", Float) = 0
		_Clouds_2_Remap_Strenght("Clouds_2_Remap_Strenght", Float) = 0
		_Clouds_3_Remap_Strenght("Clouds_3_Remap_Strenght", Float) = 0
		_Clouds_1_Remap_Added("Clouds_1_Remap_Added", Float) = 0
		_Clouds_2_Remap_Added("Clouds_2_Remap_Added", Float) = 0
		_Clouds_3_Remap_Added("Clouds_3_Remap_Added", Float) = 0
		_HologramGrid_Little("HologramGrid_Little", 2D) = "white" {}
		_HologramGrid_Big("HologramGrid_Big", 2D) = "white" {}
		_Clouds1_Strenght("Clouds1_Strenght", Float) = 0
		_Clouds2_Strenght("Clouds2_Strenght", Float) = 0
		_Clouds3_Strenght("Clouds3_Strenght", Float) = 0
		_GridBig_Strenght("GridBig_Strenght", Float) = 0
		_GridLittle_Strenght("GridLittle_Strenght", Float) = 0
		_ColorGrid_Little("ColorGrid_Little", Color) = (0,0,0,0)
		_ColorGrid_Big("ColorGrid_Big", Color) = (0,0,0,0)
		_MinOld("MinOld", Range( 0 , 1)) = 0
		_MaxOld("MaxOld", Range( 0 , 1)) = 0
		_Mask_Strenght("Mask_Strenght", Float) = 1
		_Mask_Offset("Mask_Offset", Float) = 0

	}
	
	SubShader
	{
		
		
		Tags { "RenderType"="Opaque" }
	LOD 100

		CGINCLUDE
		#pragma target 3.0
		ENDCG
		Blend Off
		AlphaToMask On
		Cull Back
		ColorMask RGBA
		ZWrite On
		ZTest LEqual
		Offset 0 , 0
		
		
		
		Pass
		{
			Name "Unlit"
			Tags { "LightMode"="ForwardBase" }
			CGPROGRAM

			

			#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
			//only defining to not throw compilation error over Unity 5.5
			#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
			#endif
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#include "UnityCG.cginc"
			#include "UnityShaderVariables.cginc"
			#define ASE_NEEDS_FRAG_WORLD_POSITION


			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			
			struct v2f
			{
				float4 vertex : SV_POSITION;
				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				float3 worldPos : TEXCOORD0;
				#endif
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_texcoord4 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			uniform float _GridLittle_Strenght;
			uniform float4 _ColorGrid_Little;
			uniform sampler2D _HologramGrid_Little;
			uniform float _GridBig_Strenght;
			uniform float4 _ColorGrid_Big;
			uniform sampler2D _HologramGrid_Big;
			uniform sampler2D _Clouds_1;
			uniform float _Clouds_1_X_offset_Speed;
			uniform float _Clouds_1_Y_Offset_Speed;
			uniform float _Clouds_1_Remap_Frecuency;
			uniform float _Clouds_1_Remap_Strenght;
			uniform float _Clouds_1_Remap_Added;
			uniform float _Clouds1_Strenght;
			uniform sampler2D _Texture0;
			uniform float _Clouds_2_X_offset_Speed;
			uniform float _Clouds_2_Y_Offset_Speed;
			uniform float _Clouds_2_Remap_Frecuency;
			uniform float _Clouds_2_Remap_Strenght;
			uniform float _Clouds_2_Remap_Added;
			uniform float _Clouds2_Strenght;
			uniform sampler2D _Texture3;
			uniform float _Clouds_3_X_offset_Speed;
			uniform float _Clouds_3_Y_Offset_Speed;
			uniform float _Clouds_3_Remap_Frecuency;
			uniform float _Clouds_3_Remap_Strenght;
			uniform float _Clouds_3_Remap_Added;
			uniform float _Clouds3_Strenght;
			uniform float _MinOld;
			uniform float _MaxOld;
			uniform float _Mask_Strenght;
			uniform float _Mask_Offset;
			inline float4 TriplanarSampling68( sampler2D topTexMap, float3 worldPos, float3 worldNormal, float falloff, float2 tiling, float3 normalScale, float3 index )
			{
				float3 projNormal = ( pow( abs( worldNormal ), falloff ) );
				projNormal /= ( projNormal.x + projNormal.y + projNormal.z ) + 0.00001;
				float3 nsign = sign( worldNormal );
				half4 xNorm; half4 yNorm; half4 zNorm;
				xNorm = tex2D( topTexMap, tiling * worldPos.zy * float2(  nsign.x, 1.0 ) );
				yNorm = tex2D( topTexMap, tiling * worldPos.xz * float2(  nsign.y, 1.0 ) );
				zNorm = tex2D( topTexMap, tiling * worldPos.xy * float2( -nsign.z, 1.0 ) );
				return xNorm * projNormal.x + yNorm * projNormal.y + zNorm * projNormal.z;
			}
			
			inline float4 TriplanarSampling66( sampler2D topTexMap, float3 worldPos, float3 worldNormal, float falloff, float2 tiling, float3 normalScale, float3 index )
			{
				float3 projNormal = ( pow( abs( worldNormal ), falloff ) );
				projNormal /= ( projNormal.x + projNormal.y + projNormal.z ) + 0.00001;
				float3 nsign = sign( worldNormal );
				half4 xNorm; half4 yNorm; half4 zNorm;
				xNorm = tex2D( topTexMap, tiling * worldPos.zy * float2(  nsign.x, 1.0 ) );
				yNorm = tex2D( topTexMap, tiling * worldPos.xz * float2(  nsign.y, 1.0 ) );
				zNorm = tex2D( topTexMap, tiling * worldPos.xy * float2( -nsign.z, 1.0 ) );
				return xNorm * projNormal.x + yNorm * projNormal.y + zNorm * projNormal.z;
			}
			
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
			

			
			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				float3 ase_worldNormal = UnityObjectToWorldNormal(v.ase_normal);
				o.ase_texcoord1.xyz = ase_worldNormal;
				float4 ase_clipPos = UnityObjectToClipPos(v.vertex);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				o.ase_texcoord2 = screenPos;
				
				o.ase_texcoord3.xy = v.ase_texcoord.xy;
				o.ase_texcoord4 = v.vertex;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord1.w = 0;
				o.ase_texcoord3.zw = 0;
				float3 vertexValue = float3(0, 0, 0);
				#if ASE_ABSOLUTE_VERTEX_POS
				vertexValue = v.vertex.xyz;
				#endif
				vertexValue = vertexValue;
				#if ASE_ABSOLUTE_VERTEX_POS
				v.vertex.xyz = vertexValue;
				#else
				v.vertex.xyz += vertexValue;
				#endif
				o.vertex = UnityObjectToClipPos(v.vertex);

				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				#endif
				return o;
			}
			
			fixed4 frag (v2f i ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
				fixed4 finalColor;
				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				float3 WorldPosition = i.worldPos;
				#endif
				float2 temp_cast_0 = (1.67).xx;
				float3 ase_worldNormal = i.ase_texcoord1.xyz;
				float4 triplanar68 = TriplanarSampling68( _HologramGrid_Little, WorldPosition, ase_worldNormal, 1.0, temp_cast_0, 1.0, 0 );
				float grayscale72 = Luminance(triplanar68.xyz);
				float4 GridLittle78 = ( _ColorGrid_Little * grayscale72 );
				float2 temp_cast_2 = (0.74).xx;
				float4 triplanar66 = TriplanarSampling66( _HologramGrid_Big, WorldPosition, ase_worldNormal, 1.0, temp_cast_2, 1.0, 0 );
				float grayscale71 = Luminance(triplanar66.xyz);
				float4 GridBig77 = ( _ColorGrid_Big * grayscale71 );
				float mulTime29 = _Time.y * _Clouds_1_X_offset_Speed;
				float4 screenPos = i.ase_texcoord2;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float mulTime19 = _Time.y * _Clouds_1_Y_Offset_Speed;
				float2 appendResult47 = (float2(( mulTime29 + ase_screenPosNorm.x ) , ( ase_screenPosNorm.y + mulTime19 )));
				float mulTime18 = _Time.y * _Clouds_1_Remap_Frecuency;
				float4 temp_cast_4 = (( ( sin( mulTime18 ) * _Clouds_1_Remap_Strenght ) + _Clouds_1_Remap_Added )).xxxx;
				float mulTime9 = _Time.y * _Clouds_2_X_offset_Speed;
				float mulTime8 = _Time.y * _Clouds_2_Y_Offset_Speed;
				float2 appendResult38 = (float2(( mulTime9 + ase_screenPosNorm.x ) , ( ase_screenPosNorm.y + mulTime8 )));
				float mulTime7 = _Time.y * _Clouds_2_Remap_Frecuency;
				float4 temp_cast_5 = (( ( sin( mulTime7 ) * _Clouds_2_Remap_Strenght ) + _Clouds_2_Remap_Added )).xxxx;
				float mulTime10 = _Time.y * _Clouds_3_X_offset_Speed;
				float mulTime17 = _Time.y * _Clouds_3_Y_Offset_Speed;
				float2 appendResult32 = (float2(( mulTime10 + ase_screenPosNorm.x ) , ( ase_screenPosNorm.y + mulTime17 )));
				float mulTime12 = _Time.y * _Clouds_3_Remap_Frecuency;
				float4 temp_cast_6 = (( ( sin( mulTime12 ) * _Clouds_3_Remap_Strenght ) + _Clouds_3_Remap_Added )).xxxx;
				float4 temp_output_69_0 = saturate( ( ( (float4( 0,0,0,0 ) + (tex2D( _Clouds_1, appendResult47 ) - temp_cast_4) * (float4( 1,1,1,1 ) - float4( 0,0,0,0 )) / (float4( 1,1,1,1 ) - temp_cast_4)) * _Clouds1_Strenght ) + ( ( (float4( 0,0,0,0 ) + (tex2D( _Texture0, appendResult38 ) - temp_cast_5) * (float4( 1,1,1,1 ) - float4( 0,0,0,0 )) / (float4( 1,1,1,1 ) - temp_cast_5)) * _Clouds2_Strenght ) + ( (float4( 0,0,0,0 ) + (tex2D( _Texture3, appendResult32 ) - temp_cast_6) * (float4( 1,1,1,1 ) - float4( 0,0,0,0 )) / (float4( 1,1,1,1 ) - temp_cast_6)) * _Clouds3_Strenght ) ) ) );
				float4 appendResult79 = (float4(temp_output_69_0.r , temp_output_69_0.r , temp_output_69_0.r , temp_output_69_0.r));
				float4 Clouds82 = appendResult79;
				float2 texCoord80 = i.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float simplePerlin2D86 = snoise( texCoord80*5.5 );
				simplePerlin2D86 = simplePerlin2D86*0.5 + 0.5;
				float4 lerpResult97 = lerp( float4( 0,0,0,0 ) , ( ( _GridLittle_Strenght * GridLittle78 ) + ( _GridBig_Strenght * GridBig77 ) + Clouds82 ) , saturate( (0.0 + (simplePerlin2D86 - _MinOld) * (1.0 - 0.0) / (_MaxOld - _MinOld)) ));
				float4 lerpResult104 = lerp( float4( 0,0,0,0 ) , lerpResult97 , saturate( ( ( i.ase_texcoord4.xyz.z * _Mask_Strenght ) + _Mask_Offset ) ));
				
				
				finalColor = lerpResult104;
				return finalColor;
			}
			ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	
}
/*ASEBEGIN
Version=18900
1127.333;674;835.3334;392.3333;-2498.687;1723.526;1.179801;True;False
Node;AmplifyShaderEditor.RangedFloatNode;1;-2363.333,2201.647;Inherit;False;Property;_Clouds_3_Remap_Frecuency;Clouds_3_Remap_Frecuency;11;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;2;-2561.833,2047.561;Inherit;False;Property;_Clouds_3_Y_Offset_Speed;Clouds_3_Y_Offset_Speed;9;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;3;-1849.294,1330.484;Inherit;False;Property;_Clouds_2_Remap_Frecuency;Clouds_2_Remap_Frecuency;12;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;4;-2058.322,866.0931;Inherit;False;Property;_Clouds_2_X_offset_Speed;Clouds_2_X_offset_Speed;2;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;5;-2047.794,1176.398;Inherit;False;Property;_Clouds_2_Y_Offset_Speed;Clouds_2_Y_Offset_Speed;8;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;6;-2572.359,1737.256;Inherit;False;Property;_Clouds_3_X_offset_Speed;Clouds_3_X_offset_Speed;1;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;10;-2289.125,1744.529;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenPosInputsNode;11;-1793.437,969.5182;Float;False;0;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleTimeNode;12;-2075.817,2204.965;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;17;-2227.857,2027.087;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;16;-2008.146,88.01306;Inherit;False;Property;_Clouds_1_Y_Offset_Speed;Clouds_1_Y_Offset_Speed;7;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;9;-1775.087,873.3661;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;8;-1713.819,1155.924;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;7;-1561.78,1333.802;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;13;-1809.646,242.0992;Inherit;False;Property;_Clouds_1_Remap_Frecuency;Clouds_1_Remap_Frecuency;10;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;14;-2018.673,-222.2919;Inherit;False;Property;_Clouds_1_X_offset_Speed;Clouds_1_X_offset_Speed;0;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenPosInputsNode;15;-2307.474,1840.681;Float;False;0;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ScreenPosInputsNode;20;-1753.787,-118.8668;Float;False;0;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;21;-2024.825,1820.34;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;23;-1461.149,1452.125;Inherit;False;Property;_Clouds_2_Remap_Strenght;Clouds_2_Remap_Strenght;14;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;18;-1522.131,245.4171;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;22;-1495.533,1116.239;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;19;-1674.171,67.53809;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;25;-1880.085,2222.657;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;27;-1366.047,1351.495;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;28;-1975.188,2323.287;Inherit;False;Property;_Clouds_3_Remap_Strenght;Clouds_3_Remap_Strenght;15;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;29;-1735.438,-215.0189;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;24;-2009.572,1987.402;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;26;-1510.787,949.1771;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;35;-1191.326,1366.977;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;32;-1829.657,1902.494;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;30;-1471.138,-139.2079;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;31;-1604.223,676.3021;Inherit;True;Property;_Texture0;Texture 0;4;0;Create;True;0;0;0;True;0;False;87dbd29693e793846ad279c2ae3900f3;87dbd29693e793846ad279c2ae3900f3;False;gray;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.RangedFloatNode;40;-1199.067,1538.381;Inherit;False;Property;_Clouds_2_Remap_Added;Clouds_2_Remap_Added;17;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;41;-1713.105,2409.544;Inherit;False;Property;_Clouds_3_Remap_Added;Clouds_3_Remap_Added;18;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;36;-2118.261,1547.465;Inherit;True;Property;_Texture3;Texture 3;5;0;Create;True;0;0;0;True;0;False;87dbd29693e793846ad279c2ae3900f3;87dbd29693e793846ad279c2ae3900f3;False;gray;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;33;-1705.365,2238.14;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;38;-1315.619,1031.331;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;39;-1455.885,27.85413;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;37;-1326.399,263.1102;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;34;-1421.5,363.7401;Inherit;False;Property;_Clouds_1_Remap_Strenght;Clouds_1_Remap_Strenght;13;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;46;-1564.575,-413.6259;Inherit;True;Property;_Clouds_1;Clouds_1;3;0;Create;True;0;0;0;True;0;False;3b78ad6594e52c84697416ba2a12cc9d;3b78ad6594e52c84697416ba2a12cc9d;False;gray;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SamplerNode;49;-1650.853,1841.666;Inherit;True;Property;_TextureSample4;Texture Sample 4;0;0;Create;True;0;0;0;False;0;False;-1;646718291b2fbbf40831bcf139711de1;646718291b2fbbf40831bcf139711de1;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;47;-1275.97,-57.05481;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;48;-1151.678,278.5922;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;42;-1027.663,1363.659;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;45;-1541.701,2234.822;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;44;-1136.814,970.504;Inherit;True;Property;_TextureSample0;Texture Sample 0;0;0;Create;True;0;0;0;False;0;False;-1;646718291b2fbbf40831bcf139711de1;646718291b2fbbf40831bcf139711de1;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;43;-1159.419,449.9952;Inherit;False;Property;_Clouds_1_Remap_Added;Clouds_1_Remap_Added;16;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;55;-1307.291,1854.168;Inherit;True;5;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,1,1,1;False;3;COLOR;0,0,0,0;False;4;COLOR;1,1,1,1;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;54;-988.0154,275.274;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;53;-793.2524,983.0052;Inherit;True;5;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,1,1,1;False;3;COLOR;0,0,0,0;False;4;COLOR;1,1,1,1;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;52;-1223.779,2097.531;Inherit;False;Property;_Clouds3_Strenght;Clouds3_Strenght;23;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;51;-708.1084,1242.854;Inherit;False;Property;_Clouds2_Strenght;Clouds2_Strenght;22;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;50;-1097.165,-117.8818;Inherit;True;Property;_TextureSample1;Texture Sample 1;0;0;Create;True;0;0;0;False;0;False;-1;646718291b2fbbf40831bcf139711de1;646718291b2fbbf40831bcf139711de1;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;59;-673.8684,136.17;Inherit;False;Property;_Clouds1_Strenght;Clouds1_Strenght;21;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;58;-753.6045,-105.3809;Inherit;True;5;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,1,1,1;False;3;COLOR;0,0,0,0;False;4;COLOR;1,1,1,1;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;56;-1023.007,1883.944;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;57;-494.1084,1038.854;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;64;8.495483,-2677.642;Inherit;False;Constant;_Tiling_Big;Tiling_Big;21;0;Create;True;0;0;0;False;0;False;0.74;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;65;-89.94775,-2917.403;Inherit;True;Property;_HologramGrid_Big;HologramGrid_Big;20;0;Create;True;0;0;0;False;0;False;d1ffeb5418c13284c9de98c82c794555;d1ffeb5418c13284c9de98c82c794555;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.RangedFloatNode;63;122.2913,-2226.323;Inherit;False;Constant;_Tiling_Little;Tiling_Little;22;0;Create;True;0;0;0;False;0;False;1.67;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;61;-425.4485,-33.56689;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.TexturePropertyNode;60;23.84808,-2466.083;Inherit;True;Property;_HologramGrid_Little;HologramGrid_Little;19;0;Create;True;0;0;0;False;0;False;d1ffeb5418c13284c9de98c82c794555;d1ffeb5418c13284c9de98c82c794555;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SimpleAddOpNode;62;-405.4144,1170.344;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TriplanarNode;68;301.6371,-2383.197;Inherit;True;Spherical;World;False;Top Texture 0;_TopTexture0;white;-1;None;Mid Texture 0;_MidTexture0;white;-1;None;Bot Texture 0;_BotTexture0;white;-1;None;Triplanar Sampler;Tangent;10;0;SAMPLER2D;;False;5;FLOAT;1;False;1;SAMPLER2D;;False;6;FLOAT;0;False;2;SAMPLER2D;;False;7;FLOAT;0;False;9;FLOAT3;0,0,0;False;8;FLOAT;1;False;3;FLOAT2;1,1;False;4;FLOAT;1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TriplanarNode;66;158.4612,-2775.755;Inherit;True;Spherical;World;False;Top Texture 1;_TopTexture1;white;-1;None;Mid Texture 1;_MidTexture1;white;-1;None;Bot Texture 1;_BotTexture1;white;-1;None;Triplanar Sampler;Tangent;10;0;SAMPLER2D;;False;5;FLOAT;1;False;1;SAMPLER2D;;False;6;FLOAT;0;False;2;SAMPLER2D;;False;7;FLOAT;0;False;9;FLOAT3;0,0,0;False;8;FLOAT;1;False;3;FLOAT2;1,1;False;4;FLOAT;1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;67;-239.5354,-78.8009;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;73;571.0199,-2923.458;Inherit;False;Property;_ColorGrid_Big;ColorGrid_Big;27;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCGrayscale;71;611.2607,-2741.061;Inherit;False;0;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCGrayscale;72;736.0064,-2337.832;Inherit;False;0;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;69;-105.4296,-135.7639;Inherit;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;70;711.6855,-2565.172;Inherit;False;Property;_ColorGrid_Little;ColorGrid_Little;26;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;76;954.6689,-2317.007;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;74;834.3027,-2773.215;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.BreakToComponentsNode;75;81.42053,104.3651;Inherit;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.TextureCoordinatesNode;80;1347.207,-1536.785;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;77;993.8041,-2762.925;Inherit;False;GridBig;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.DynamicAppendNode;79;359.643,-73.87183;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;78;1142.156,-2301.11;Inherit;False;GridLittle;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;88;1433.103,-1909.493;Inherit;False;Property;_GridBig_Strenght;GridBig_Strenght;24;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;83;1461.26,-1817.268;Inherit;False;77;GridBig;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;81;1442.077,-2133.682;Inherit;False;Property;_GridLittle_Strenght;GridLittle_Strenght;25;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;99;2537.317,-1485.901;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;87;1723.969,-1203.853;Inherit;False;Property;_MaxOld;MaxOld;29;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;84;1450.471,-2049.092;Inherit;False;78;GridLittle;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;85;1696.921,-1303.461;Inherit;False;Property;_MinOld;MinOld;28;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;86;1684.906,-1535.314;Inherit;True;Simplex2D;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;5.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;82;528.3199,-146.5919;Inherit;False;Clouds;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;101;2580.837,-1284.399;Inherit;False;Property;_Mask_Strenght;Mask_Strenght;30;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;103;2913.793,-1223.565;Inherit;False;Property;_Mask_Offset;Mask_Offset;31;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;91;1657.644,-1818.814;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TFHCRemapNode;92;2073.973,-1457.842;Inherit;True;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;11;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;90;1653.191,-2055.158;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;100;2847.429,-1454.185;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;89;1496.368,-1684.52;Inherit;False;82;Clouds;1;0;OBJECT;;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SaturateNode;96;2374.223,-1588.262;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;93;2061.857,-1777.636;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT4;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;102;3092.714,-1444.197;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;105;3276.56,-1550.438;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;97;2616.39,-1773.838;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;104;3440.628,-1719.092;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;94;104.941,-196.3918;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;95;-189.4246,-339.2979;Inherit;False;Property;_Clouds_Color;Clouds_Color;6;0;Create;True;0;0;0;False;0;False;0,1,0.1972687,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;98;3716.819,-1804.573;Float;False;True;-1;2;ASEMaterialInspector;100;1;HeadHologram;0770190933193b94aaa3065e307002fa;True;Unlit;0;0;Unlit;2;False;True;0;1;False;-1;0;False;-1;0;1;False;-1;0;False;-1;True;0;False;-1;0;False;-1;False;False;False;False;False;False;False;False;False;True;1;False;-1;False;True;0;False;-1;False;True;True;True;True;True;0;False;-1;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;1;RenderType=Opaque=RenderType;True;2;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=ForwardBase;False;0;;0;0;Standard;1;Vertex Position,InvertActionOnDeselection;1;0;1;True;False;;False;0
WireConnection;10;0;6;0
WireConnection;12;0;1;0
WireConnection;17;0;2;0
WireConnection;9;0;4;0
WireConnection;8;0;5;0
WireConnection;7;0;3;0
WireConnection;21;0;10;0
WireConnection;21;1;15;1
WireConnection;18;0;13;0
WireConnection;22;0;11;2
WireConnection;22;1;8;0
WireConnection;19;0;16;0
WireConnection;25;0;12;0
WireConnection;27;0;7;0
WireConnection;29;0;14;0
WireConnection;24;0;15;2
WireConnection;24;1;17;0
WireConnection;26;0;9;0
WireConnection;26;1;11;1
WireConnection;35;0;27;0
WireConnection;35;1;23;0
WireConnection;32;0;21;0
WireConnection;32;1;24;0
WireConnection;30;0;29;0
WireConnection;30;1;20;1
WireConnection;33;0;25;0
WireConnection;33;1;28;0
WireConnection;38;0;26;0
WireConnection;38;1;22;0
WireConnection;39;0;20;2
WireConnection;39;1;19;0
WireConnection;37;0;18;0
WireConnection;49;0;36;0
WireConnection;49;1;32;0
WireConnection;47;0;30;0
WireConnection;47;1;39;0
WireConnection;48;0;37;0
WireConnection;48;1;34;0
WireConnection;42;0;35;0
WireConnection;42;1;40;0
WireConnection;45;0;33;0
WireConnection;45;1;41;0
WireConnection;44;0;31;0
WireConnection;44;1;38;0
WireConnection;55;0;49;0
WireConnection;55;1;45;0
WireConnection;54;0;48;0
WireConnection;54;1;43;0
WireConnection;53;0;44;0
WireConnection;53;1;42;0
WireConnection;50;0;46;0
WireConnection;50;1;47;0
WireConnection;58;0;50;0
WireConnection;58;1;54;0
WireConnection;56;0;55;0
WireConnection;56;1;52;0
WireConnection;57;0;53;0
WireConnection;57;1;51;0
WireConnection;61;0;58;0
WireConnection;61;1;59;0
WireConnection;62;0;57;0
WireConnection;62;1;56;0
WireConnection;68;0;60;0
WireConnection;68;3;63;0
WireConnection;66;0;65;0
WireConnection;66;3;64;0
WireConnection;67;0;61;0
WireConnection;67;1;62;0
WireConnection;71;0;66;0
WireConnection;72;0;68;0
WireConnection;69;0;67;0
WireConnection;76;0;70;0
WireConnection;76;1;72;0
WireConnection;74;0;73;0
WireConnection;74;1;71;0
WireConnection;75;0;69;0
WireConnection;77;0;74;0
WireConnection;79;0;75;0
WireConnection;79;1;75;0
WireConnection;79;2;75;0
WireConnection;79;3;75;0
WireConnection;78;0;76;0
WireConnection;86;0;80;0
WireConnection;82;0;79;0
WireConnection;91;0;88;0
WireConnection;91;1;83;0
WireConnection;92;0;86;0
WireConnection;92;1;85;0
WireConnection;92;2;87;0
WireConnection;90;0;81;0
WireConnection;90;1;84;0
WireConnection;100;0;99;3
WireConnection;100;1;101;0
WireConnection;96;0;92;0
WireConnection;93;0;90;0
WireConnection;93;1;91;0
WireConnection;93;2;89;0
WireConnection;102;0;100;0
WireConnection;102;1;103;0
WireConnection;105;0;102;0
WireConnection;97;1;93;0
WireConnection;97;2;96;0
WireConnection;104;1;97;0
WireConnection;104;2;105;0
WireConnection;94;0;95;0
WireConnection;94;1;69;0
WireConnection;98;0;104;0
ASEEND*/
//CHKSM=07158F473421D05ACEC1D8EF5BEA710CA87F07CC