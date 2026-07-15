// Made with Amplify Shader Editor v1.9.8.1
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "M_Standart_Master"
{
	Properties
	{
		Material_Texture2D_5("ARM", 2D) = "white" {}
		Material_Texture2D_1("Albedo", 2D) = "white" {}
		Material_Texture2D_0("Normal", 2D) = "bump" {}
		Material_Texture2D_3("Roughness", 2D) = "white" {}
		Material_Texture2D_4("Mask", 2D) = "white" {}
		Material_Texture2D_2("Metallic", 2D) = "white" {}
		_Brightness("Brightness", Range( 0 , 5)) = 1
		_Contrast("Contrast", Range( 0 , 5)) = 1
		_Roughness("Roughness", Range( 0 , 5)) = 1
		[Toggle(_MASK_IN_ARM_ON)] _Mask_In_ARM("Mask_In_ARM", Float) = 0
		[Toggle(_METALLIC_CONSTANT_ON)] _Metallic_Constant("Metallic_Constant", Float) = 1
		_Metallic("Metallic", Range( 0 , 1)) = 0
		[Toggle(_ARM_ON)] _ARM("ARM", Float) = 1
		[Toggle(_COLORMASK_ON)] _ColorMask("ColorMask", Float) = 0
		_Color("Color", Color) = (0,0,0,0)
		_Desaturation("Desaturation", Range( 0 , 1)) = 0
		_UV("UV", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma shader_feature_local _COLORMASK_ON
		#pragma shader_feature_local _MASK_IN_ARM_ON
		#pragma shader_feature_local _ARM_ON
		#pragma shader_feature_local _METALLIC_CONSTANT_ON
		#define ASE_VERSION 19801
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D Material_Texture2D_0;
		uniform float _UV;
		uniform sampler2D Material_Texture2D_1;
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

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 temp_output_35_0 = ( i.uv_texcoord * _UV );
			float3 tex2DNode7 = UnpackNormal( tex2D( Material_Texture2D_0, temp_output_35_0 ) );
			o.Normal = tex2DNode7;
			float4 tex2DNode3 = tex2D( Material_Texture2D_1, temp_output_35_0 );
			float4 tex2DNode1 = tex2D( Material_Texture2D_4, temp_output_35_0 );
			float4 tex2DNode25 = tex2D( Material_Texture2D_5, temp_output_35_0 );
			#ifdef _MASK_IN_ARM_ON
				float staticSwitch24 = ( 1.0 - tex2DNode25.r );
			#else
				float staticSwitch24 = tex2DNode1.r;
			#endif
			float4 lerpResult8 = lerp( tex2DNode3 , ( tex2DNode3 * _Color ) , staticSwitch24);
			#ifdef _COLORMASK_ON
				float4 staticSwitch13 = lerpResult8;
			#else
				float4 staticSwitch13 = tex2DNode3;
			#endif
			float4 saferPower33 = abs( ( staticSwitch13 * _Brightness ) );
			float4 temp_cast_0 = (_Contrast).xxxx;
			float3 desaturateInitialColor22 = pow( saferPower33 , temp_cast_0 ).rgb;
			float desaturateDot22 = dot( desaturateInitialColor22, float3( 0.299, 0.587, 0.114 ));
			float3 desaturateVar22 = lerp( desaturateInitialColor22, desaturateDot22.xxx, _Desaturation );
			o.Albedo = desaturateVar22;
			#ifdef _METALLIC_CONSTANT_ON
				float staticSwitch38 = _Metallic;
			#else
				float staticSwitch38 = tex2D( Material_Texture2D_2, temp_output_35_0 ).b;
			#endif
			#ifdef _ARM_ON
				float staticSwitch27 = tex2DNode25.b;
			#else
				float staticSwitch27 = staticSwitch38;
			#endif
			o.Metallic = staticSwitch27;
			#ifdef _ARM_ON
				float staticSwitch28 = ( 1.0 - tex2DNode25.g );
			#else
				float staticSwitch28 = ( 1.0 - tex2D( Material_Texture2D_3, temp_output_35_0 ).r );
			#endif
			o.Smoothness = ( staticSwitch28 * _Roughness );
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "AmplifyShaderEditor.MaterialInspector"
}
/*ASEBEGIN
Version=19801
Node;AmplifyShaderEditor.TextureCoordinatesNode;34;-2549.99,254.4783;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;36;-2400.725,565.7603;Inherit;False;Property;_UV;UV;17;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;35;-2265.096,439.7053;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;25;-1872,240;Inherit;True;Property;Material_Texture2D_5;ARM;0;0;Create;False;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SamplerNode;1;-2096,-176;Inherit;True;Property;Material_Texture2D_4;Mask;4;0;Create;False;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.ColorNode;2;-1632,-752;Inherit;False;Property;_Color;Color;15;0;Create;True;0;0;0;False;0;False;0,0,0,0;0.8,0.8,0.8,1;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SamplerNode;3;-1648,-464;Inherit;True;Property;Material_Texture2D_1;Albedo;1;0;Create;False;0;0;0;False;0;False;-1;None;abc00000000011835080966170663043;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.OneMinusNode;26;-1504.434,132.2318;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;5;-1264,-640;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StaticSwitch;24;-1200,-224;Inherit;False;Property;_Mask_In_ARM;Mask_In_ARM;10;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;8;-1056,-608;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StaticSwitch;13;-784,-528;Inherit;False;Property;_ColorMask;ColorMask;14;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;10;-1792,1072;Inherit;True;Property;Material_Texture2D_3;Roughness;3;0;Create;False;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.RangedFloatNode;12;-624,-256;Inherit;False;Property;_Brightness;Brightness;6;0;Create;True;0;0;0;False;0;False;1;1;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;14;-304,-368;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;16;-1488,1104;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;32;-496,-192;Inherit;False;Property;_Contrast;Contrast;7;0;Create;True;0;0;0;False;0;False;1;1;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;37;-1454.614,453.2679;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;20;-1792,512;Inherit;True;Property;Material_Texture2D_2;Metallic;5;0;Create;False;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.RangedFloatNode;39;-1488,736;Inherit;False;Property;_Metallic;Metallic;12;0;Create;True;0;0;0;False;0;False;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;17;-592,1040;Inherit;False;Property;_Roughness;Roughness;8;0;Create;True;0;0;0;False;0;False;1;0.5;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;18;-544,-96;Inherit;False;Property;_Desaturation;Desaturation;16;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;28;-992,864;Inherit;False;Property;_Use_ARM1;Use_ARM;13;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Reference;27;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;38;-1408,592;Inherit;False;Property;_Metallic_Constant;Metallic_Constant;11;0;Create;True;0;0;0;False;0;False;0;1;1;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;33;-145.4221,-262.1862;Inherit;False;True;2;0;COLOR;0,0,0,0;False;1;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;4;-1680,16;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;7;-736,1472;Inherit;True;Property;Material_Texture2D_0;Normal;2;0;Create;False;0;0;0;False;0;False;-1;None;abc00000000016124000965088101700;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.BreakToComponentsNode;9;-432,1488;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;15;-304,1504;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;-1;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;19;-144,1488;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;11;-448,1616;Inherit;False;Property;_Normal_Invert;Normal_Invert;9;0;Create;True;0;0;0;False;0;False;-1;-1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DesaturateOpNode;22;32,-240;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;21;-304,800;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;27;-32,-96;Inherit;False;Property;_ARM;ARM;13;0;Create;True;0;0;0;False;0;False;0;1;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;432,-112;Float;False;True;-1;2;AmplifyShaderEditor.MaterialInspector;0;0;Standard;M_Standart_Master;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;17;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;35;0;34;0
WireConnection;35;1;36;0
WireConnection;25;1;35;0
WireConnection;1;1;35;0
WireConnection;3;1;35;0
WireConnection;26;0;25;1
WireConnection;5;0;3;0
WireConnection;5;1;2;0
WireConnection;24;1;1;1
WireConnection;24;0;26;0
WireConnection;8;0;3;0
WireConnection;8;1;5;0
WireConnection;8;2;24;0
WireConnection;13;1;3;0
WireConnection;13;0;8;0
WireConnection;10;1;35;0
WireConnection;14;0;13;0
WireConnection;14;1;12;0
WireConnection;16;0;10;1
WireConnection;37;0;25;2
WireConnection;20;1;35;0
WireConnection;28;1;16;0
WireConnection;28;0;37;0
WireConnection;38;1;20;3
WireConnection;38;0;39;0
WireConnection;33;0;14;0
WireConnection;33;1;32;0
WireConnection;4;0;1;1
WireConnection;7;1;35;0
WireConnection;9;0;7;0
WireConnection;15;0;9;1
WireConnection;15;1;11;0
WireConnection;19;0;9;0
WireConnection;19;1;15;0
WireConnection;19;2;9;2
WireConnection;22;0;33;0
WireConnection;22;1;18;0
WireConnection;21;0;28;0
WireConnection;21;1;17;0
WireConnection;27;1;38;0
WireConnection;27;0;25;3
WireConnection;0;0;22;0
WireConnection;0;1;7;0
WireConnection;0;3;27;0
WireConnection;0;4;21;0
ASEEND*/
//CHKSM=1B6BC7EB075788055F1D1693B19418F6E225403D