// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Fatigue"
{
	Properties
	{
		_Blur_Intensity("Blur_Intensity", Range( 0 , 1)) = 0.25
		_Beat_Speed("Beat_Speed", Float) = 0.8
		_Base("Base", Float) = 0.4
		_Vignette("Vignette", 2D) = "white" {}
		_Vignete_Intensity("Vignete_Intensity", Float) = 0.7
		_GlobalIntensity("GlobalIntensity", Float) = 0.5
		_ExplosionBlurIntensity("ExplosionBlurIntensity", Float) = 0
		_BlackScreen_Value("BlackScreen_Value", Float) = 1
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
			
			uniform float _ExplosionBlurIntensity;
			uniform float _GlobalIntensity;
			uniform float _Blur_Intensity;
			uniform float _Beat_Speed;
			uniform float _Base;
			uniform sampler2D _Vignette;
			uniform float4 _Vignette_ST;
			uniform float _Vignete_Intensity;
			uniform float _BlackScreen_Value;


			
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

				float4 color78 = IsGammaSpace() ? float4(0,0,0,0) : float4(0,0,0,0);
				float Global_Intensity68 = _GlobalIntensity;
				float mulTime46 = _Time.y * ( _Beat_Speed * 20.0 * Global_Intensity68 );
				float SinValue62 = ( ( ( sin( mulTime46 ) + 1.0 ) / ( 2.0 + ( 2.4 * _Base ) ) ) + _Base );
				float temp_output_41_0 = ( _Blur_Intensity * 0.03 * SinValue62 );
				float temp_output_66_0 = ( ( _ExplosionBlurIntensity + Global_Intensity68 ) * temp_output_41_0 );
				float2 appendResult23 = (float2(temp_output_66_0 , 0.0));
				float2 texCoord11 = i.texcoord.xy * float2( 1,1 ) + appendResult23;
				float2 uv_MainTex = i.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				float temp_output_37_0 = ( temp_output_66_0 * -1.0 );
				float2 appendResult32 = (float2(temp_output_37_0 , 0.0));
				float2 texCoord33 = i.texcoord.xy * float2( 1,1 ) + appendResult32;
				float2 temp_cast_0 = (temp_output_37_0).xx;
				float2 texCoord31 = i.texcoord.xy * float2( 1,1 ) + temp_cast_0;
				float2 appendResult34 = (float2(0.0 , temp_output_37_0));
				float2 texCoord30 = i.texcoord.xy * float2( 1,1 ) + appendResult34;
				float2 uv_Vignette = i.texcoord.xy * _Vignette_ST.xy + _Vignette_ST.zw;
				float4 lerpResult59 = lerp( ( ( tex2D( _MainTex, texCoord11 ) + tex2D( _MainTex, uv_MainTex ) + tex2D( _MainTex, uv_MainTex ) + tex2D( _MainTex, texCoord33 ) + tex2D( _MainTex, texCoord31 ) + tex2D( _MainTex, texCoord30 ) ) / 6 ) , float4( 0,0,0,0 ) , ( ( tex2D( _Vignette, uv_Vignette ).a * SinValue62 ) * _Vignete_Intensity * Global_Intensity68 ));
				float4 lerpResult76 = lerp( color78 , lerpResult59 , _BlackScreen_Value);
				

				float4 color = lerpResult76;
				
				return color;
			}
			ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	
}
/*ASEBEGIN
Version=18900
1263.333;782;698;269.6667;-974.0922;-224.2138;1.470967;True;False
Node;AmplifyShaderEditor.RangedFloatNode;67;-2127.074,-127.5452;Inherit;False;Property;_GlobalIntensity;GlobalIntensity;5;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;68;-1914.622,-117.1671;Inherit;False;Global_Intensity;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;49;-3535.008,328.7023;Inherit;False;Property;_Beat_Speed;Beat_Speed;1;0;Create;True;0;0;0;False;0;False;0.8;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;71;-3527.404,413.8997;Inherit;False;68;Global_Intensity;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;50;-3243.008,328.7023;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;20;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;46;-3080.043,336.558;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;52;-2774.674,655.0823;Inherit;False;Property;_Base;Base;2;0;Create;True;0;0;0;False;0;False;0.4;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;54;-2640.246,551.109;Inherit;False;2;2;0;FLOAT;2.4;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;44;-2895.043,340.558;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;47;-2729.008,318.7023;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;56;-2481.246,471.109;Inherit;False;2;2;0;FLOAT;2;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;48;-2311.008,326.7023;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;51;-2104.907,379.1686;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;75;-1928.975,-230.5254;Inherit;False;Property;_ExplosionBlurIntensity;ExplosionBlurIntensity;6;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;62;-1940.595,314.7843;Inherit;False;SinValue;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;17;-2140.04,45.9977;Inherit;False;Property;_Blur_Intensity;Blur_Intensity;0;0;Create;True;0;0;0;False;0;False;0.25;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;74;-1606.975,-64.52542;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;41;-1711.709,132.8337;Inherit;True;3;3;0;FLOAT;0;False;1;FLOAT;0.03;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;66;-1367.784,-22.16726;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;37;-1071.217,879.0374;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;-1;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;34;-807.9708,1071.749;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;23;-961.9969,31.46003;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;32;-846.0015,713.7061;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TemplateShaderPropertyNode;15;-696.8062,-95.93836;Inherit;False;0;0;_MainTex;Shader;False;0;5;SAMPLER2D;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;11;-796.8683,-3.017251;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;31;-669.0015,849.7061;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;30;-669.0015,1032.706;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;33;-680.8729,679.2288;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;63;256.4337,1093.656;Inherit;False;62;SinValue;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;18;-406.0402,132.742;Inherit;True;Property;_TextureSample1;Texture Sample 1;0;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;28;-371.6023,1027.745;Inherit;True;Property;_TextureSample4;Texture Sample 4;2;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;27;-367.8094,817.2368;Inherit;True;Property;_TextureSample3;Texture Sample 3;1;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;57;163.7413,835.6176;Inherit;True;Property;_Vignette;Vignette;3;0;Create;True;0;0;0;False;0;False;-1;abe321fc6bc17714baf78abe93c16c10;abe321fc6bc17714baf78abe93c16c10;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;19;-409.8332,343.2498;Inherit;True;Property;_TextureSample2;Texture Sample 2;0;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;29;-382.1792,588.5754;Inherit;True;Property;_TextureSample5;Texture Sample 5;3;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;5;-420.41,-95.91937;Inherit;True;Property;_TextureSample0;Texture Sample 0;0;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;61;493.783,906.0705;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;65;516.5455,1143.559;Inherit;False;Property;_Vignete_Intensity;Vignete_Intensity;4;0;Create;True;0;0;0;False;0;False;0.7;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;25;83.51406,420.9934;Inherit;False;6;6;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;70;492.957,1219.748;Inherit;False;68;Global_Intensity;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.IntNode;40;62.36926,662.2585;Inherit;False;Constant;_Int0;Int 0;4;0;Create;True;0;0;0;False;0;False;6;0;False;0;1;INT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;38;354.3693,470.2586;Inherit;False;2;0;COLOR;0,0,0,0;False;1;INT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;64;829.3561,1001.723;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;59;1272.751,463.2166;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;77;1312.487,726.8553;Inherit;False;Property;_BlackScreen_Value;BlackScreen_Value;7;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;78;1263.587,234.7571;Inherit;False;Constant;_Color0;Color 0;8;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;21;-784.9969,350.46;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;24;-923.9662,389.5032;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;20;-784.9969,167.46;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;76;1555.801,430.6796;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;14;1742.688,436.6122;Float;False;True;-1;2;ASEMaterialInspector;0;10;Fatigue;d143e746ed2c3dd43907da4cf2afafc4;True;SubShader 0 Pass 0;0;0;SubShader 0 Pass 0;1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;-1;False;False;False;False;False;False;False;False;False;False;False;True;2;False;-1;True;7;False;-1;False;False;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;0;;0;0;Standard;0;0;1;True;False;;False;0
WireConnection;68;0;67;0
WireConnection;50;0;49;0
WireConnection;50;2;71;0
WireConnection;46;0;50;0
WireConnection;54;1;52;0
WireConnection;44;0;46;0
WireConnection;47;0;44;0
WireConnection;56;1;54;0
WireConnection;48;0;47;0
WireConnection;48;1;56;0
WireConnection;51;0;48;0
WireConnection;51;1;52;0
WireConnection;62;0;51;0
WireConnection;74;0;75;0
WireConnection;74;1;68;0
WireConnection;41;0;17;0
WireConnection;41;2;62;0
WireConnection;66;0;74;0
WireConnection;66;1;41;0
WireConnection;37;0;66;0
WireConnection;34;1;37;0
WireConnection;23;0;66;0
WireConnection;32;0;37;0
WireConnection;11;1;23;0
WireConnection;31;1;37;0
WireConnection;30;1;34;0
WireConnection;33;1;32;0
WireConnection;18;0;15;0
WireConnection;28;0;15;0
WireConnection;28;1;30;0
WireConnection;27;0;15;0
WireConnection;27;1;31;0
WireConnection;19;0;15;0
WireConnection;29;0;15;0
WireConnection;29;1;33;0
WireConnection;5;0;15;0
WireConnection;5;1;11;0
WireConnection;61;0;57;4
WireConnection;61;1;63;0
WireConnection;25;0;5;0
WireConnection;25;1;18;0
WireConnection;25;2;19;0
WireConnection;25;3;29;0
WireConnection;25;4;27;0
WireConnection;25;5;28;0
WireConnection;38;0;25;0
WireConnection;38;1;40;0
WireConnection;64;0;61;0
WireConnection;64;1;65;0
WireConnection;64;2;70;0
WireConnection;59;0;38;0
WireConnection;59;2;64;0
WireConnection;21;1;24;0
WireConnection;24;1;66;0
WireConnection;20;1;41;0
WireConnection;76;0;78;0
WireConnection;76;1;59;0
WireConnection;76;2;77;0
WireConnection;14;0;76;0
ASEEND*/
//CHKSM=89574BE1F1313C1BFC6F97269DDC8B9716EA4CEA