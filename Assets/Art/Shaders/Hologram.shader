// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Hologram"
{
	Properties
	{
		_Clouds_1_X_offset_Speed("Clouds_1_X_offset_Speed", Float) = 0
		_Clouds_3_X_offset_Speed("Clouds_3_X_offset_Speed", Float) = 0
		_Clouds_2_X_offset_Speed("Clouds_2_X_offset_Speed", Float) = 0
		_Clouds_1("Clouds_1", 2D) = "gray" {}
		_Clouds_2("Clouds_2", 2D) = "gray" {}
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
		_Tiling_Big("Tiling_Big", Float) = 0
		_Tiling_Little("Tiling_Little", Float) = 0
		_Clouds1_Strenght("Clouds1_Strenght", Float) = 0
		_Clouds2_Strenght("Clouds2_Strenght", Float) = 0
		_Clouds3_Strenght("Clouds3_Strenght", Float) = 0
		_GridBig_Strenght("GridBig_Strenght", Float) = 0
		_GridLittle_Strenght("GridLittle_Strenght", Float) = 0
		_ColorGrid_Little("ColorGrid_Little", Color) = (0,0,0,0)
		_ColorGrid_Big("ColorGrid_Big", Color) = (0,0,0,0)
		_MinOld("MinOld", Range( 0 , 1)) = 0
		_MaxOld("MaxOld", Range( 0 , 1)) = 0

	}
	
	SubShader
	{
		
		
		Tags { "RenderType"="Opaque" }
	LOD 100

		CGINCLUDE
		#pragma target 3.0
		ENDCG
		Blend One One, One One
		BlendOp Add, Add
		AlphaToMask On
		Cull Back
		ColorMask RGB
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
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			uniform float _GridLittle_Strenght;
			uniform float4 _ColorGrid_Little;
			uniform sampler2D _HologramGrid_Little;
			uniform float _Tiling_Little;
			uniform float _GridBig_Strenght;
			uniform float4 _ColorGrid_Big;
			uniform sampler2D _HologramGrid_Big;
			uniform float _Tiling_Big;
			uniform sampler2D _Clouds_1;
			uniform float _Clouds_1_X_offset_Speed;
			uniform float _Clouds_1_Y_Offset_Speed;
			uniform float _Clouds_1_Remap_Frecuency;
			uniform float _Clouds_1_Remap_Strenght;
			uniform float _Clouds_1_Remap_Added;
			uniform float _Clouds1_Strenght;
			uniform sampler2D _Clouds_2;
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
			inline float4 TriplanarSampling83( sampler2D topTexMap, float3 worldPos, float3 worldNormal, float falloff, float2 tiling, float3 normalScale, float3 index )
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
			
			inline float4 TriplanarSampling125( sampler2D topTexMap, float3 worldPos, float3 worldNormal, float falloff, float2 tiling, float3 normalScale, float3 index )
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
				float2 temp_cast_0 = (_Tiling_Little).xx;
				float3 ase_worldNormal = i.ase_texcoord1.xyz;
				float4 triplanar83 = TriplanarSampling83( _HologramGrid_Little, WorldPosition, ase_worldNormal, 1.0, temp_cast_0, 1.0, 0 );
				float grayscale144 = Luminance(triplanar83.xyz);
				float4 GridLittle138 = ( _ColorGrid_Little * grayscale144 );
				float2 temp_cast_2 = (_Tiling_Big).xx;
				float4 triplanar125 = TriplanarSampling125( _HologramGrid_Big, WorldPosition, ase_worldNormal, 1.0, temp_cast_2, 1.0, 0 );
				float grayscale147 = Luminance(triplanar125.xyz);
				float4 GridBig133 = ( _ColorGrid_Big * grayscale147 );
				float mulTime45 = _Time.y * _Clouds_1_X_offset_Speed;
				float4 screenPos = i.ase_texcoord2;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float mulTime46 = _Time.y * _Clouds_1_Y_Offset_Speed;
				float2 appendResult20 = (float2(( mulTime45 + ase_screenPosNorm.x ) , ( ase_screenPosNorm.y + mulTime46 )));
				float mulTime47 = _Time.y * _Clouds_1_Remap_Frecuency;
				float4 temp_cast_4 = (( ( sin( mulTime47 ) * _Clouds_1_Remap_Strenght ) + _Clouds_1_Remap_Added )).xxxx;
				float mulTime62 = _Time.y * _Clouds_2_X_offset_Speed;
				float mulTime63 = _Time.y * _Clouds_2_Y_Offset_Speed;
				float2 appendResult71 = (float2(( mulTime62 + ase_screenPosNorm.x ) , ( ase_screenPosNorm.y + mulTime63 )));
				float mulTime64 = _Time.y * _Clouds_2_Remap_Frecuency;
				float4 temp_cast_5 = (( ( sin( mulTime64 ) * _Clouds_2_Remap_Strenght ) + _Clouds_2_Remap_Added )).xxxx;
				float mulTime92 = _Time.y * _Clouds_3_X_offset_Speed;
				float mulTime94 = _Time.y * _Clouds_3_Y_Offset_Speed;
				float2 appendResult107 = (float2(( mulTime92 + ase_screenPosNorm.x ) , ( ase_screenPosNorm.y + mulTime94 )));
				float mulTime99 = _Time.y * _Clouds_3_Remap_Frecuency;
				float4 temp_cast_6 = (( ( sin( mulTime99 ) * _Clouds_3_Remap_Strenght ) + _Clouds_3_Remap_Added )).xxxx;
				float4 temp_output_82_0 = saturate( ( ( (float4( 0,0,0,0 ) + (tex2D( _Clouds_1, appendResult20 ) - temp_cast_4) * (float4( 1,1,1,1 ) - float4( 0,0,0,0 )) / (float4( 1,1,1,1 ) - temp_cast_4)) * _Clouds1_Strenght ) + ( ( (float4( 0,0,0,0 ) + (tex2D( _Clouds_2, appendResult71 ) - temp_cast_5) * (float4( 1,1,1,1 ) - float4( 0,0,0,0 )) / (float4( 1,1,1,1 ) - temp_cast_5)) * _Clouds2_Strenght ) + ( (float4( 0,0,0,0 ) + (tex2D( _Texture3, appendResult107 ) - temp_cast_6) * (float4( 1,1,1,1 ) - float4( 0,0,0,0 )) / (float4( 1,1,1,1 ) - temp_cast_6)) * _Clouds3_Strenght ) ) ) );
				float4 appendResult153 = (float4(temp_output_82_0.r , temp_output_82_0.r , temp_output_82_0.r , temp_output_82_0.r));
				float4 Clouds90 = appendResult153;
				float2 texCoord173 = i.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float simplePerlin2D171 = snoise( texCoord173*5.5 );
				simplePerlin2D171 = simplePerlin2D171*0.5 + 0.5;
				float4 lerpResult170 = lerp( float4( 0,0,0,0 ) , ( ( _GridLittle_Strenght * GridLittle138 ) + ( _GridBig_Strenght * GridBig133 ) + Clouds90 ) , saturate( (0.0 + (simplePerlin2D171 - _MinOld) * (1.0 - 0.0) / (_MaxOld - _MinOld)) ));
				
				
				finalColor = lerpResult170;
				return finalColor;
			}
			ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	
}
/*ASEBEGIN
Version=18900
1489.333;794.6667;480;269.6667;-605.6732;650.651;2.46328;False;False
Node;AmplifyShaderEditor.RangedFloatNode;103;-3170.382,4166.012;Inherit;False;Property;_Clouds_3_Remap_Frecuency;Clouds_3_Remap_Frecuency;11;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;106;-3368.882,4011.926;Inherit;False;Property;_Clouds_3_Y_Offset_Speed;Clouds_3_Y_Offset_Speed;9;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;60;-2656.344,3294.849;Inherit;False;Property;_Clouds_2_Remap_Frecuency;Clouds_2_Remap_Frecuency;12;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;58;-2865.371,2830.458;Inherit;False;Property;_Clouds_2_X_offset_Speed;Clouds_2_X_offset_Speed;2;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;59;-2854.844,3140.763;Inherit;False;Property;_Clouds_2_Y_Offset_Speed;Clouds_2_Y_Offset_Speed;8;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;95;-3379.409,3701.621;Inherit;False;Property;_Clouds_3_X_offset_Speed;Clouds_3_X_offset_Speed;1;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;94;-3034.907,3991.452;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;41;-2815.196,2052.378;Inherit;False;Property;_Clouds_1_Y_Offset_Speed;Clouds_1_Y_Offset_Speed;7;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenPosInputsNode;96;-3114.524,3805.046;Float;False;0;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;28;-2825.723,1742.073;Inherit;False;Property;_Clouds_1_X_offset_Speed;Clouds_1_X_offset_Speed;0;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;48;-2616.696,2206.464;Inherit;False;Property;_Clouds_1_Remap_Frecuency;Clouds_1_Remap_Frecuency;10;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;99;-2882.867,4169.33;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenPosInputsNode;61;-2600.486,2933.883;Float;False;0;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleTimeNode;92;-3096.175,3708.894;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;62;-2582.137,2837.731;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;63;-2520.869,3120.289;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;64;-2368.829,3298.167;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;98;-2782.237,4287.652;Inherit;False;Property;_Clouds_3_Remap_Strenght;Clouds_3_Remap_Strenght;15;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;67;-2173.097,3315.86;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;65;-2317.837,2913.542;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;100;-2687.135,4187.022;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;101;-2816.621,3951.767;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;45;-2542.488,1749.346;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;68;-2268.199,3416.49;Inherit;False;Property;_Clouds_2_Remap_Strenght;Clouds_2_Remap_Strenght;14;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;66;-2302.583,3080.604;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;97;-2831.875,3784.705;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenPosInputsNode;13;-2560.837,1845.498;Float;False;0;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleTimeNode;46;-2481.221,2031.903;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;47;-2329.181,2209.782;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;40;-2262.934,1992.219;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;71;-2122.669,2995.696;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SinOpNode;49;-2133.449,2227.475;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;69;-1998.376,3331.342;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;27;-2278.188,1825.157;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;70;-2006.117,3502.746;Inherit;False;Property;_Clouds_2_Remap_Added;Clouds_2_Remap_Added;17;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;51;-2228.55,2328.105;Inherit;False;Property;_Clouds_1_Remap_Strenght;Clouds_1_Remap_Strenght;13;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;102;-2512.414,4202.505;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;107;-2636.707,3866.859;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;56;-2411.273,2640.667;Inherit;True;Property;_Clouds_2;Clouds_2;4;0;Create;True;0;0;0;True;0;False;87dbd29693e793846ad279c2ae3900f3;87dbd29693e793846ad279c2ae3900f3;False;gray;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.TexturePropertyNode;93;-2925.311,3511.83;Inherit;True;Property;_Texture3;Texture 3;5;0;Create;True;0;0;0;True;0;False;87dbd29693e793846ad279c2ae3900f3;87dbd29693e793846ad279c2ae3900f3;False;gray;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.RangedFloatNode;108;-2520.155,4373.909;Inherit;False;Property;_Clouds_3_Remap_Added;Clouds_3_Remap_Added;18;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;50;-1958.728,2242.957;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;20;-2083.02,1907.31;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;29;-2371.625,1550.739;Inherit;True;Property;_Clouds_1;Clouds_1;3;0;Create;True;0;0;0;True;0;False;3b78ad6594e52c84697416ba2a12cc9d;3b78ad6594e52c84697416ba2a12cc9d;False;gray;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SamplerNode;104;-2457.902,3806.031;Inherit;True;Property;_TextureSample4;Texture Sample 4;0;0;Create;True;0;0;0;False;0;False;-1;646718291b2fbbf40831bcf139711de1;646718291b2fbbf40831bcf139711de1;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;72;-1943.864,2934.869;Inherit;True;Property;_TextureSample1;Texture Sample 1;0;0;Create;True;0;0;0;False;0;False;-1;646718291b2fbbf40831bcf139711de1;646718291b2fbbf40831bcf139711de1;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;73;-1834.713,3328.024;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;53;-1966.469,2414.36;Inherit;False;Property;_Clouds_1_Remap_Added;Clouds_1_Remap_Added;16;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;91;-2348.751,4199.187;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;18;-1904.215,1846.483;Inherit;True;Property;_TextureSample0;Texture Sample 0;0;0;Create;True;0;0;0;False;0;False;-1;646718291b2fbbf40831bcf139711de1;646718291b2fbbf40831bcf139711de1;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;120;-1515.158,3207.219;Inherit;False;Property;_Clouds2_Strenght;Clouds2_Strenght;24;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;122;-2030.829,4061.896;Inherit;False;Property;_Clouds3_Strenght;Clouds3_Strenght;25;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;74;-1600.302,2947.37;Inherit;True;5;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,1,1,1;False;3;COLOR;0,0,0,0;False;4;COLOR;1,1,1,1;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;52;-1795.065,2239.639;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;105;-2114.34,3818.533;Inherit;True;5;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,1,1,1;False;3;COLOR;0,0,0,0;False;4;COLOR;1,1,1,1;False;1;COLOR;0
Node;AmplifyShaderEditor.TFHCRemapNode;38;-1560.654,1858.984;Inherit;True;5;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,1,1,1;False;3;COLOR;0,0,0,0;False;4;COLOR;1,1,1,1;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;118;-1480.918,2100.535;Inherit;False;Property;_Clouds1_Strenght;Clouds1_Strenght;23;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;119;-1301.158,3003.219;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;121;-1830.057,3848.309;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.TexturePropertyNode;85;-783.2015,-501.7186;Inherit;True;Property;_HologramGrid_Little;HologramGrid_Little;19;0;Create;True;0;0;0;False;0;False;d1ffeb5418c13284c9de98c82c794555;d1ffeb5418c13284c9de98c82c794555;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;117;-1232.498,1930.798;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;109;-1212.464,3134.709;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;89;-684.7583,-261.9579;Inherit;False;Property;_Tiling_Little;Tiling_Little;22;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;123;-798.5541,-713.277;Inherit;False;Property;_Tiling_Big;Tiling_Big;21;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;124;-896.9973,-953.0378;Inherit;True;Property;_HologramGrid_Big;HologramGrid_Big;20;0;Create;True;0;0;0;False;0;False;d1ffeb5418c13284c9de98c82c794555;d1ffeb5418c13284c9de98c82c794555;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.TriplanarNode;125;-648.5884,-811.3906;Inherit;True;Spherical;World;False;Top Texture 1;_TopTexture1;white;-1;None;Mid Texture 1;_MidTexture1;white;-1;None;Bot Texture 1;_BotTexture1;white;-1;None;Triplanar Sampler;Tangent;10;0;SAMPLER2D;;False;5;FLOAT;1;False;1;SAMPLER2D;;False;6;FLOAT;0;False;2;SAMPLER2D;;False;7;FLOAT;0;False;9;FLOAT3;0,0,0;False;8;FLOAT;1;False;3;FLOAT2;1,1;False;4;FLOAT;1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;81;-1046.585,1885.564;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TriplanarNode;83;-505.4125,-418.8317;Inherit;True;Spherical;World;False;Top Texture 0;_TopTexture0;white;-1;None;Mid Texture 0;_MidTexture0;white;-1;None;Bot Texture 0;_BotTexture0;white;-1;None;Triplanar Sampler;Tangent;10;0;SAMPLER2D;;False;5;FLOAT;1;False;1;SAMPLER2D;;False;6;FLOAT;0;False;2;SAMPLER2D;;False;7;FLOAT;0;False;9;FLOAT3;0,0,0;False;8;FLOAT;1;False;3;FLOAT2;1,1;False;4;FLOAT;1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCGrayscale;144;-71.04315,-373.4673;Inherit;False;0;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;149;-236.0297,-959.0929;Inherit;False;Property;_ColorGrid_Big;ColorGrid_Big;29;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;146;-95.36399,-600.8068;Inherit;False;Property;_ColorGrid_Little;ColorGrid_Little;28;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCGrayscale;147;-195.7889,-776.696;Inherit;False;0;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;82;-912.4791,1828.601;Inherit;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;148;27.25309,-808.8501;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.BreakToComponentsNode;152;-725.629,2068.73;Inherit;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;145;147.6194,-352.6423;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;133;186.7545,-798.5603;Inherit;False;GridBig;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;138;335.1062,-336.7456;Inherit;False;GridLittle;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.DynamicAppendNode;153;-447.4066,1890.493;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;173;540.1573,427.5803;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;176;916.9191,760.512;Inherit;False;Property;_MaxOld;MaxOld;31;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;171;877.857,429.0511;Inherit;True;Simplex2D;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;5.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;175;889.8714,660.9036;Inherit;False;Property;_MinOld;MinOld;30;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;137;626.053,54.87155;Inherit;False;Property;_GridBig_Strenght;GridBig_Strenght;26;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;139;643.4213,-84.72742;Inherit;False;138;GridLittle;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;135;654.2108,147.0973;Inherit;False;133;GridBig;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;90;-278.7296,1817.773;Inherit;False;Clouds;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;141;635.0276,-169.3169;Inherit;False;Property;_GridLittle_Strenght;GridLittle_Strenght;27;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;110;689.3188,279.8449;Inherit;False;90;Clouds;1;0;OBJECT;;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;140;846.1412,-90.79301;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;136;850.594,145.5508;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TFHCRemapNode;174;1277.915,431.7797;Inherit;True;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;11;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;126;1254.808,186.729;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT4;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;177;1567.173,376.1024;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;35;-996.4741,1625.067;Inherit;False;Property;_Clouds_Color;Clouds_Color;6;0;Create;True;0;0;0;False;0;False;0,1,0.1972687,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;170;1574.445,176.7094;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;34;-702.1086,1767.973;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;143;1887.197,187.5774;Float;False;True;-1;2;ASEMaterialInspector;100;1;Hologram;0770190933193b94aaa3065e307002fa;True;Unlit;0;0;Unlit;2;True;True;4;1;False;-1;1;False;-1;4;1;False;-1;1;False;-1;True;1;False;-1;1;False;-1;False;False;False;False;False;False;False;False;False;True;1;False;-1;False;True;0;False;-1;True;True;True;True;True;False;0;False;-1;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;1;RenderType=Opaque=RenderType;True;2;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=ForwardBase;False;0;;0;0;Standard;1;Vertex Position,InvertActionOnDeselection;1;0;1;True;False;;False;0
WireConnection;94;0;106;0
WireConnection;99;0;103;0
WireConnection;92;0;95;0
WireConnection;62;0;58;0
WireConnection;63;0;59;0
WireConnection;64;0;60;0
WireConnection;67;0;64;0
WireConnection;65;0;62;0
WireConnection;65;1;61;1
WireConnection;100;0;99;0
WireConnection;101;0;96;2
WireConnection;101;1;94;0
WireConnection;45;0;28;0
WireConnection;66;0;61;2
WireConnection;66;1;63;0
WireConnection;97;0;92;0
WireConnection;97;1;96;1
WireConnection;46;0;41;0
WireConnection;47;0;48;0
WireConnection;40;0;13;2
WireConnection;40;1;46;0
WireConnection;71;0;65;0
WireConnection;71;1;66;0
WireConnection;49;0;47;0
WireConnection;69;0;67;0
WireConnection;69;1;68;0
WireConnection;27;0;45;0
WireConnection;27;1;13;1
WireConnection;102;0;100;0
WireConnection;102;1;98;0
WireConnection;107;0;97;0
WireConnection;107;1;101;0
WireConnection;50;0;49;0
WireConnection;50;1;51;0
WireConnection;20;0;27;0
WireConnection;20;1;40;0
WireConnection;104;0;93;0
WireConnection;104;1;107;0
WireConnection;72;0;56;0
WireConnection;72;1;71;0
WireConnection;73;0;69;0
WireConnection;73;1;70;0
WireConnection;91;0;102;0
WireConnection;91;1;108;0
WireConnection;18;0;29;0
WireConnection;18;1;20;0
WireConnection;74;0;72;0
WireConnection;74;1;73;0
WireConnection;52;0;50;0
WireConnection;52;1;53;0
WireConnection;105;0;104;0
WireConnection;105;1;91;0
WireConnection;38;0;18;0
WireConnection;38;1;52;0
WireConnection;119;0;74;0
WireConnection;119;1;120;0
WireConnection;121;0;105;0
WireConnection;121;1;122;0
WireConnection;117;0;38;0
WireConnection;117;1;118;0
WireConnection;109;0;119;0
WireConnection;109;1;121;0
WireConnection;125;0;124;0
WireConnection;125;3;123;0
WireConnection;81;0;117;0
WireConnection;81;1;109;0
WireConnection;83;0;85;0
WireConnection;83;3;89;0
WireConnection;144;0;83;0
WireConnection;147;0;125;0
WireConnection;82;0;81;0
WireConnection;148;0;149;0
WireConnection;148;1;147;0
WireConnection;152;0;82;0
WireConnection;145;0;146;0
WireConnection;145;1;144;0
WireConnection;133;0;148;0
WireConnection;138;0;145;0
WireConnection;153;0;152;0
WireConnection;153;1;152;0
WireConnection;153;2;152;0
WireConnection;153;3;152;0
WireConnection;171;0;173;0
WireConnection;90;0;153;0
WireConnection;140;0;141;0
WireConnection;140;1;139;0
WireConnection;136;0;137;0
WireConnection;136;1;135;0
WireConnection;174;0;171;0
WireConnection;174;1;175;0
WireConnection;174;2;176;0
WireConnection;126;0;140;0
WireConnection;126;1;136;0
WireConnection;126;2;110;0
WireConnection;177;0;174;0
WireConnection;170;1;126;0
WireConnection;170;2;177;0
WireConnection;34;0;35;0
WireConnection;34;1;82;0
WireConnection;143;0;170;0
ASEEND*/
//CHKSM=C7C7CD43D83978903A221634B8CCDC3007EC23C8