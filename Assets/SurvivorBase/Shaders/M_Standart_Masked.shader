// Made with Amplify Shader Editor v1.9.8.1
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "M_Standart_Cutout"
{
	Properties
	{
		Material_Texture2D_5("ARM", 2D) = "white" {}
		_MainTex("Albedo", 2D) = "white" {}
		Material_Texture2D_0("Normal", 2D) = "bump" {}
		Material_Texture2D_3("Roughness", 2D) = "white" {}
		Material_Texture2D_4("Mask", 2D) = "white" {}
		Material_Texture2D_2("Metallic", 2D) = "white" {}
		_Brightness("Brightness", Range( 0 , 5)) = 1
		_Contrast("Contrast", Range( 0 , 5)) = 1
		[Toggle(_METALLIC_CONSTANT_ON)] _Metallic_Constant("Metallic_Constant", Float) = 0
		_Metallic("Metallic", Range( 0 , 1)) = 1
		[Toggle(_ROUGHNESS_CONSTANT_ON)] _Roughness_Constant("Roughness_Constant", Float) = 0
		_Roughness("Roughness", Range( 0 , 5)) = 1
		[Toggle(_MASK_IN_ARM_ON)] _Mask_In_ARM("Mask_In_ARM", Float) = 0
		[Toggle(_USE_ARM_ON)] _Use_ARM("Use_ARM", Float) = 0
		[Toggle(_USEALPHA_ON)] _UseAlpha("Use Alpha", Float) = 0
		[Toggle(_ALBEDO_ALPHA_ON)] _Albedo_Alpha("Albedo_Alpha", Float) = 0
		[Toggle(_COLORMASK_ON)] _ColorMask("ColorMask", Float) = 0
		_Color("Color", Color) = (0,0,0,0)
		_Desaturation("Desaturation", Range( 0 , 1)) = 0
		_UV("UV", Float) = 1
		_Opacity("Opacity", Range( 0 , 3)) = 1
		_OpacityCipValue("OpacityCipValue", Range( 0 , 0.3)) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "AlphaTest+0" "IgnoreProjector" = "True" }
		Cull Off
		AlphaToMask On
		CGPROGRAM
		#pragma target 3.0
		#pragma shader_feature_local _COLORMASK_ON
		#pragma shader_feature_local _MASK_IN_ARM_ON
		#pragma shader_feature_local _METALLIC_CONSTANT_ON
		#pragma shader_feature_local _USE_ARM_ON
		#pragma shader_feature_local _ROUGHNESS_CONSTANT_ON
		#pragma shader_feature_local _ALBEDO_ALPHA_ON
		#pragma shader_feature_local _USEALPHA_ON
		#define ASE_VERSION 19801
		#pragma surface surf Standard keepalpha 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D Material_Texture2D_0;
		uniform float _UV;
		uniform sampler2D _MainTex;
		uniform float4 _Color;
		uniform sampler2D Material_Texture2D_4;
		uniform sampler2D Material_Texture2D_5;
		uniform float _Brightness;
		uniform float _Contrast;
		uniform float _Desaturation;
		uniform sampler2D Material_Texture2D_2;
		uniform float _Metallic;
		uniform sampler2D Material_Texture2D_3;
		uniform float _Roughness;
		uniform float _Opacity;
		uniform float _OpacityCipValue;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 temp_output_35_0 = ( i.uv_texcoord * _UV );
			float3 tex2DNode7 = UnpackNormal( tex2D( Material_Texture2D_0, temp_output_35_0 ) );
			o.Normal = tex2DNode7;
			float4 tex2DNode3 = tex2D( _MainTex, temp_output_35_0 );
			float4 tex2DNode25 = tex2D( Material_Texture2D_5, temp_output_35_0 );
			#ifdef _MASK_IN_ARM_ON
				float staticSwitch24 = ( 1.0 - tex2DNode25.r );
			#else
				float staticSwitch24 = ( 1.0 - tex2D( Material_Texture2D_4, temp_output_35_0 ).r );
			#endif
			float4 lerpResult8 = lerp( tex2DNode3 , ( tex2DNode3 * _Color ) , staticSwitch24);
			#ifdef _COLORMASK_ON
				float4 staticSwitch13 = lerpResult8;
			#else
				float4 staticSwitch13 = tex2DNode3;
			#endif
			float4 temp_cast_0 = (_Contrast).xxxx;
			float3 desaturateInitialColor22 = pow( ( staticSwitch13 * _Brightness ) , temp_cast_0 ).rgb;
			float desaturateDot22 = dot( desaturateInitialColor22, float3( 0.299, 0.587, 0.114 ));
			float3 desaturateVar22 = lerp( desaturateInitialColor22, desaturateDot22.xxx, _Desaturation );
			o.Albedo = desaturateVar22;
			#ifdef _USE_ARM_ON
				float staticSwitch27 = tex2DNode25.b;
			#else
				float staticSwitch27 = tex2D( Material_Texture2D_2, temp_output_35_0 ).b;
			#endif
			#ifdef _METALLIC_CONSTANT_ON
				float staticSwitch42 = _Metallic;
			#else
				float staticSwitch42 = staticSwitch27;
			#endif
			o.Metallic = staticSwitch42;
			#ifdef _USE_ARM_ON
				float staticSwitch28 = ( 1.0 - tex2DNode25.g );
			#else
				float staticSwitch28 = ( 1.0 - tex2D( Material_Texture2D_3, temp_output_35_0 ).r );
			#endif
			#ifdef _ROUGHNESS_CONSTANT_ON
				float staticSwitch41 = _Roughness;
			#else
				float staticSwitch41 = ( staticSwitch28 * _Roughness );
			#endif
			o.Smoothness = staticSwitch41;
			o.Alpha = 1;
			#ifdef _USEALPHA_ON
				float staticSwitch29 = tex2DNode3.a;
			#else
				float staticSwitch29 = pow( tex2DNode25.r , _Opacity );
			#endif
			#ifdef _ALBEDO_ALPHA_ON
				float staticSwitch44 = staticSwitch29;
			#else
				float staticSwitch44 = tex2DNode3.g;
			#endif
			#ifdef _MASK_IN_ARM_ON
				float staticSwitch46 = tex2DNode25.r;
			#else
				float staticSwitch46 = staticSwitch44;
			#endif
			clip( staticSwitch46 - _OpacityCipValue );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "AmplifyShaderEditor.MaterialInspector"
}
/*ASEBEGIN
Version=19801
Node;AmplifyShaderEditor.TextureCoordinatesNode;34;-2549.99,254.4783;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;36;-2400.725,565.7603;Inherit;False;Property;_UV;UV;20;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;35;-2265.096,439.7053;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;1;-2096,-176;Inherit;True;Property;Material_Texture2D_4;Mask;4;0;Create;False;0;0;0;False;0;False;-1;None;abc00000000015841410210242463388;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SamplerNode;25;-1872,240;Inherit;True;Property;Material_Texture2D_5;ARM;0;0;Create;False;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.ColorNode;2;-1632,-752;Inherit;False;Property;_Color;Color;18;0;Create;True;0;0;0;False;0;False;0,0,0,0;0.8,0.8,0.8,1;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.OneMinusNode;4;-1680,16;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;26;-1504.434,132.2318;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;3;-1648,-464;Inherit;True;Property;_MainTex;Albedo;1;0;Create;False;0;0;0;False;0;False;-1;None;eb03533a1288bf64eaa0d800aee7e18d;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;5;-1264,-640;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StaticSwitch;24;-1200,-224;Inherit;False;Property;_Mask_In_ARM;Mask_In_ARM;13;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;8;-1056,-608;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;10;-1792,1072;Inherit;True;Property;Material_Texture2D_3;Roughness;3;0;Create;False;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.RangedFloatNode;38;-1280,656;Inherit;False;Property;_Opacity;Opacity;21;0;Create;True;0;0;0;False;0;False;1;1;0;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;13;-784,-528;Inherit;False;Property;_ColorMask;ColorMask;17;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;12;-624,-256;Inherit;False;Property;_Brightness;Brightness;6;0;Create;True;0;0;0;False;0;False;1;1.65;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;16;-1488,1104;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;37;-1454.614,453.2679;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;40;-1173.423,425.5107;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;14;-304,-368;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;32;-496,-192;Inherit;False;Property;_Contrast;Contrast;7;0;Create;True;0;0;0;False;0;False;1;1;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;20;-1792,512;Inherit;True;Property;Material_Texture2D_2;Metallic;5;0;Create;False;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.RangedFloatNode;17;-672,1008;Inherit;False;Property;_Roughness;Roughness;11;0;Create;True;0;0;0;False;0;False;1;1;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;29;-864,400;Inherit;False;Property;_UseAlpha;Use Alpha;15;0;Create;True;0;0;0;False;0;False;0;0;1;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;28;-976,880;Inherit;False;Property;_Use_ARM1;Use_ARM;14;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Reference;27;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;18;-400,-96;Inherit;False;Property;_Desaturation;Desaturation;19;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;33;-145.4221,-262.1862;Inherit;False;False;2;0;COLOR;0,0,0,0;False;1;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.StaticSwitch;27;-256,48;Inherit;False;Property;_Use_ARM;Use_ARM;14;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;21;-464,576;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;43;-368,224;Inherit;False;Property;_Metallic;Metallic;9;0;Create;True;0;0;0;False;0;False;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;44;-608,352;Inherit;False;Property;_Albedo_Alpha;Albedo_Alpha;16;0;Create;True;0;0;0;False;0;False;0;0;1;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;7;-736,1472;Inherit;True;Property;Material_Texture2D_0;Normal;2;0;Create;False;0;0;0;False;0;False;-1;None;abc00000000012122850438009226639;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.BreakToComponentsNode;9;-432,1488;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;15;-304,1504;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;-1;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;19;-144,1488;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;11;-448,1616;Inherit;False;Property;_Normal_Invert;Normal_Invert;12;0;Create;True;0;0;0;False;0;False;-1;-1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DesaturateOpNode;22;32,-240;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StaticSwitch;41;-64,480;Inherit;False;Property;_Roughness_Constant;Roughness_Constant;10;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;42;80,16;Inherit;False;Property;_Metallic_Constant;Metallic_Constant;8;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;39;-1024,512;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;46;-320,384;Inherit;False;Property;_Mask_In_ARM1;Mask_In_ARM;13;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Reference;24;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;45;-128,304;Inherit;False;Property;_OpacityCipValue;OpacityCipValue;22;0;Create;True;0;0;0;False;0;False;1;0.3;0;0.3;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;432,-112;Float;False;True;-1;2;AmplifyShaderEditor.MaterialInspector;0;0;Standard;M_Standart_Cutout;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Off;0;False;;0;False;;False;0;False;;0;False;;False;0;Custom;0.5;True;False;0;True;Opaque;;AlphaTest;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;5;False;;10;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;True;0;0;False;;-1;0;True;_OpacityCipValue;0;0;0;False;0.1;False;;0;False;;False;17;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;35;0;34;0
WireConnection;35;1;36;0
WireConnection;1;1;35;0
WireConnection;25;1;35;0
WireConnection;4;0;1;1
WireConnection;26;0;25;1
WireConnection;3;1;35;0
WireConnection;5;0;3;0
WireConnection;5;1;2;0
WireConnection;24;1;4;0
WireConnection;24;0;26;0
WireConnection;8;0;3;0
WireConnection;8;1;5;0
WireConnection;8;2;24;0
WireConnection;10;1;35;0
WireConnection;13;1;3;0
WireConnection;13;0;8;0
WireConnection;16;0;10;1
WireConnection;37;0;25;2
WireConnection;40;0;25;1
WireConnection;40;1;38;0
WireConnection;14;0;13;0
WireConnection;14;1;12;0
WireConnection;20;1;35;0
WireConnection;29;1;40;0
WireConnection;29;0;3;4
WireConnection;28;1;16;0
WireConnection;28;0;37;0
WireConnection;33;0;14;0
WireConnection;33;1;32;0
WireConnection;27;1;20;3
WireConnection;27;0;25;3
WireConnection;21;0;28;0
WireConnection;21;1;17;0
WireConnection;44;1;3;2
WireConnection;44;0;29;0
WireConnection;7;1;35;0
WireConnection;9;0;7;0
WireConnection;15;0;9;1
WireConnection;15;1;11;0
WireConnection;19;0;9;0
WireConnection;19;1;15;0
WireConnection;19;2;9;2
WireConnection;22;0;33;0
WireConnection;22;1;18;0
WireConnection;41;1;21;0
WireConnection;41;0;17;0
WireConnection;42;1;27;0
WireConnection;42;0;43;0
WireConnection;39;0;25;1
WireConnection;39;1;38;0
WireConnection;46;1;44;0
WireConnection;46;0;25;1
WireConnection;0;0;22;0
WireConnection;0;1;7;0
WireConnection;0;3;42;0
WireConnection;0;4;41;0
WireConnection;0;10;46;0
ASEEND*/
//CHKSM=431824DA4BC6BA25A9153FE7972D2933B2E76CCD