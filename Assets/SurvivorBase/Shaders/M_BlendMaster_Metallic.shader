// Made with Amplify Shader Editor v1.9.8.1
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "M_BlendMaster_Metallic"
{
	Properties
	{
		[Header(Material_1)][Header(_________________)]_Mat1_UV("Mat1_UV", Range( 0 , 10)) = 0.77
		_Mat1_Desaturation("Mat1_Desaturation", Range( 0 , 1)) = 0
		_Mat1_TextureDetails("Mat1_TextureDetails", Range( 0 , 1)) = 0.37
		_Mat1_BaseColor("Mat1_BaseColor", Range( 0 , 5)) = 5
		_Mat1_Roughness("Mat1_Roughness", Range( 0 , 2)) = 0.5
		_Mat1_Metallic("Mat1_Metallic", Range( 0 , 1)) = 0
		[Toggle(_USECOLOR_ON)] _UseColor("UseColor", Float) = 0
		_Mat1_Color("Mat1_Color", Color) = (1,1,1,0)
		[Toggle(_MAT1_BAKEDNORMAL_ON)] _Mat1_BakedNormal("Mat1_BakedNormal", Float) = 0
		[Toggle(_MAT1_BAKEDMASKUV_ON)] _Mat1_BakedMaskUV("Mat1_BakedMaskUV", Float) = 0
		[Toggle(_MAT1_COLORTEXTURE_ON)] _Mat1_ColorTexture("Mat1_ColorTexture", Float) = 0
		[Toggle(_MAT1_ROUGHNESSTEXTURE_ON)] _Mat1_RoughnessTexture("Mat1_RoughnessTexture", Float) = 0
		_Mat1_BaseTexture("Mat1_BaseTexture", 2D) = "white" {}
		_Mat1_BakedUV1("Mat1_BakedUV1", Range( 0 , 20)) = 1
		_Mat1_RoughnessT("Mat1_RoughnessT", 2D) = "white" {}
		_Mat1_Base_Normal("Mat1_Base_Normal", 2D) = "bump" {}
		[Header(Material_2)][Header(_________________)]_Mat2_UV("Mat2_UV", Range( 0 , 10)) = 2
		_Mat2_Desaturation("Mat2_Desaturation", Range( 0 , 1)) = 0
		_BakedNormal("BakedNormal", 2D) = "bump" {}
		[Toggle(_USECOLOR_2_ON)] _UseColor_2("UseColor_2", Float) = 0
		_Mat2_BaseColor("Mat2_BaseColor", Range( 0 , 5)) = 3.75
		_Mat2_Roughness("Mat2_Roughness", Range( 0 , 2)) = 0
		_Mat2_Color("Mat2_Color", Color) = (1,1,1,0)
		_Mat2_Metallic("Mat2_Metallic", Range( 0 , 1)) = 0
		[Toggle(_MAT2_COLORTEXTURE_ON)] _Mat2_ColorTexture("Mat2_ColorTexture", Float) = 1
		[Toggle(_MAT2_ROUGHNESSTEXTURE_ON)] _Mat2_RoughnessTexture("Mat2_RoughnessTexture", Float) = 0
		_Mat2_RoughnessT("Mat2_RoughnessT", 2D) = "white" {}
		_Mat2_BaseTexture("Mat2_BaseTexture", 2D) = "white" {}
		_Mat2_Base_Normal("Mat2_Base_Normal", 2D) = "bump" {}
		_Mat2_Mask_Variation("Mat2_Mask_Variation", 2D) = "white" {}
		[Header(Material_3)][Header(_________________)]_Mat3_UV("Mat3_UV", Range( 0 , 10)) = 2
		_Mat3_BaseColor("Mat3_BaseColor", Range( 0 , 5)) = 3.75
		_Mat3_Desaturation("Mat3_Desaturation", Range( 0 , 1)) = 0.327
		_Mat3_Roughness("Mat3_Roughness", Range( 0 , 2)) = 0
		_Mat3_Metallic("Mat3_Metallic", Range( 0 , 1)) = 0
		[Toggle(_MAT3_ROUGHNESSTEXTURE_ON)] _Mat3_RoughnessTexture("Mat3_RoughnessTexture", Float) = 0
		_Mat3_RoughnessT("Mat3_RoughnessT", 2D) = "white" {}
		[Toggle(_MAT3_COLORTEXTURE_ON)] _Mat3_ColorTexture("Mat3_ColorTexture", Float) = 1
		_Mat3_BaseTexture("Mat3_BaseTexture", 2D) = "white" {}
		_Mat3_Base_Normal("Mat3_Base_Normal", 2D) = "bump" {}
		[Header(Vertex Color_R)][Header(____________________)]_VColor_R_Scale("VColor_R_Scale", Range( 0 , 10)) = 1.37
		_Global_R("Global_R", Range( 0 , 10)) = 1.1
		_Detail_Fallof_R("Detail_Fallof_R", Range( 0 , 10)) = 3.4
		_Detail_Sharp_R("Detail_Sharp_R", Range( 0 , 10)) = 3.4
		_BlendDetail_R("BlendDetail_R", 2D) = "white" {}
		_BlendMask_R("BlendMask_R", 2D) = "white" {}
		[Header(Vertex Color_G)][Header(_____________________)]_VColor_G_Scale("VColor_G_Scale", Range( 0 , 10)) = 1.37
		_Global_G("Global_G", Range( 0 , 5)) = 1.1
		_Blend_Mask_G("Blend_Mask_G", 2D) = "white" {}
		_Worn_Level("Worn_Level", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#pragma target 3.0
		#pragma shader_feature_local _MAT1_BAKEDNORMAL_ON
		#pragma shader_feature_local _MAT3_COLORTEXTURE_ON
		#pragma shader_feature_local _MAT2_COLORTEXTURE_ON
		#pragma shader_feature_local _USECOLOR_2_ON
		#pragma shader_feature_local _MAT1_COLORTEXTURE_ON
		#pragma shader_feature_local _USECOLOR_ON
		#pragma shader_feature_local _MAT1_BAKEDMASKUV_ON
		#pragma shader_feature_local _MAT3_ROUGHNESSTEXTURE_ON
		#pragma shader_feature_local _MAT2_ROUGHNESSTEXTURE_ON
		#pragma shader_feature_local _MAT1_ROUGHNESSTEXTURE_ON
		#define ASE_VERSION 19801
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
			float4 vertexColor : COLOR;
		};

		uniform sampler2D _Mat3_Base_Normal;
		uniform float _Mat3_UV;
		uniform sampler2D _Mat2_Base_Normal;
		uniform float _Mat2_UV;
		uniform sampler2D _Mat1_Base_Normal;
		uniform float _Mat1_UV;
		uniform sampler2D _Blend_Mask_G;
		uniform float _VColor_G_Scale;
		uniform float _Global_G;
		uniform float _Worn_Level;
		uniform sampler2D _BlendMask_R;
		uniform float _VColor_R_Scale;
		uniform sampler2D _BlendDetail_R;
		uniform float4 _BlendDetail_R_ST;
		uniform float _Detail_Fallof_R;
		uniform float _Detail_Sharp_R;
		uniform float _Global_R;
		uniform sampler2D _BakedNormal;
		uniform float _Mat1_BakedUV1;
		uniform float _Mat3_BaseColor;
		uniform sampler2D _Mat3_BaseTexture;
		uniform float _Mat3_Desaturation;
		uniform float _Mat2_BaseColor;
		uniform float4 _Mat2_Color;
		uniform sampler2D _Mat2_BaseTexture;
		uniform float _Mat2_Desaturation;
		uniform float _Mat1_BaseColor;
		uniform float4 _Mat1_Color;
		uniform sampler2D _Mat1_BaseTexture;
		uniform sampler2D _Mat2_Mask_Variation;
		uniform float _Mat1_TextureDetails;
		uniform float _Mat1_Desaturation;
		uniform float _Mat3_Metallic;
		uniform float _Mat2_Metallic;
		uniform float _Mat1_Metallic;
		uniform float _Mat3_Roughness;
		uniform sampler2D _Mat3_RoughnessT;
		uniform float _Mat2_Roughness;
		uniform sampler2D _Mat2_RoughnessT;
		uniform float _Mat1_Roughness;
		uniform sampler2D _Mat1_RoughnessT;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 temp_output_78_0 = ( i.uv_texcoord * _Mat3_UV );
			float2 temp_output_50_0 = ( i.uv_texcoord * _Mat2_UV );
			float2 temp_output_46_0 = ( i.uv_texcoord * _Mat1_UV );
			float temp_output_34_0 = ( i.vertexColor.g + ( i.vertexColor.g * tex2D( _Blend_Mask_G, ( i.uv_texcoord * _VColor_G_Scale ) ).r ) );
			float lerpResult115 = lerp( 1.0 , temp_output_34_0 , _Global_G);
			float lerpResult130 = lerp( 1.0 , lerpResult115 , _Worn_Level);
			float clampResult58 = clamp( lerpResult130 , 0.0 , 1.0 );
			float3 lerpResult18 = lerp( UnpackNormal( tex2D( _Mat2_Base_Normal, temp_output_50_0 ) ) , UnpackNormal( tex2D( _Mat1_Base_Normal, temp_output_46_0 ) ) , clampResult58);
			float2 uv_BlendDetail_R = i.uv_texcoord * _BlendDetail_R_ST.xy + _BlendDetail_R_ST.zw;
			float saferPower108 = abs( ( tex2D( _BlendMask_R, ( i.uv_texcoord * _VColor_R_Scale ) ).g + tex2D( _BlendDetail_R, uv_BlendDetail_R ).r ) );
			float saferPower99 = abs( ( i.vertexColor.r + ( i.vertexColor.r * pow( saferPower108 , _Detail_Fallof_R ) ) ) );
			float clampResult100 = clamp( pow( saferPower99 , _Detail_Sharp_R ) , 0.0 , 1.0 );
			float lerpResult127 = lerp( 1.0 , ( clampResult100 * _Global_R ) , _Worn_Level);
			float clampResult103 = clamp( lerpResult127 , 0.0 , 1.0 );
			float3 lerpResult88 = lerp( UnpackNormal( tex2D( _Mat3_Base_Normal, temp_output_78_0 ) ) , lerpResult18 , clampResult103);
			float2 temp_output_122_0 = ( i.uv_texcoord * _Mat1_BakedUV1 );
			#ifdef _MAT1_BAKEDNORMAL_ON
				float3 staticSwitch120 = BlendNormals( lerpResult88 , UnpackNormal( tex2D( _BakedNormal, temp_output_122_0 ) ) );
			#else
				float3 staticSwitch120 = lerpResult88;
			#endif
			o.Normal = staticSwitch120;
			float4 temp_cast_0 = (_Mat3_BaseColor).xxxx;
			#ifdef _MAT3_COLORTEXTURE_ON
				float4 staticSwitch81 = tex2D( _Mat3_BaseTexture, temp_output_78_0 );
			#else
				float4 staticSwitch81 = temp_cast_0;
			#endif
			float3 desaturateInitialColor116 = ( _Mat3_BaseColor * staticSwitch81 ).rgb;
			float desaturateDot116 = dot( desaturateInitialColor116, float3( 0.299, 0.587, 0.114 ));
			float3 desaturateVar116 = lerp( desaturateInitialColor116, desaturateDot116.xxx, _Mat3_Desaturation );
			float4 temp_cast_2 = (_Mat2_BaseColor).xxxx;
			float4 tex2DNode12 = tex2D( _Mat2_BaseTexture, temp_output_50_0 );
			#ifdef _USECOLOR_2_ON
				float4 staticSwitch133 = ( _Mat2_Color * tex2DNode12 );
			#else
				float4 staticSwitch133 = temp_cast_2;
			#endif
			#ifdef _MAT2_COLORTEXTURE_ON
				float4 staticSwitch11 = tex2DNode12;
			#else
				float4 staticSwitch11 = staticSwitch133;
			#endif
			float4 temp_output_36_0 = ( _Mat2_BaseColor * staticSwitch11 );
			float3 desaturateInitialColor125 = temp_output_36_0.rgb;
			float desaturateDot125 = dot( desaturateInitialColor125, float3( 0.299, 0.587, 0.114 ));
			float3 desaturateVar125 = lerp( desaturateInitialColor125, desaturateDot125.xxx, _Mat2_Desaturation );
			float4 temp_cast_4 = (_Mat1_BaseColor).xxxx;
			#ifdef _USECOLOR_ON
				float4 staticSwitch53 = _Mat1_Color;
			#else
				float4 staticSwitch53 = temp_cast_4;
			#endif
			#ifdef _MAT1_COLORTEXTURE_ON
				float4 staticSwitch9 = ( _Mat1_Color * tex2D( _Mat1_BaseTexture, temp_output_46_0 ) );
			#else
				float4 staticSwitch9 = staticSwitch53;
			#endif
			float4 temp_output_42_0 = ( _Mat1_BaseColor * staticSwitch9 );
			#ifdef _MAT1_BAKEDMASKUV_ON
				float2 staticSwitch124 = temp_output_122_0;
			#else
				float2 staticSwitch124 = temp_output_46_0;
			#endif
			float4 tex2DNode40 = tex2D( _Mat2_Mask_Variation, staticSwitch124 );
			float4 lerpResult41 = lerp( temp_output_42_0 , temp_output_36_0 , tex2DNode40.r);
			float4 lerpResult44 = lerp( temp_output_42_0 , lerpResult41 , _Mat1_TextureDetails);
			float3 desaturateInitialColor111 = lerpResult44.rgb;
			float desaturateDot111 = dot( desaturateInitialColor111, float3( 0.299, 0.587, 0.114 ));
			float3 desaturateVar111 = lerp( desaturateInitialColor111, desaturateDot111.xxx, _Mat1_Desaturation );
			float3 lerpResult17 = lerp( desaturateVar125 , desaturateVar111 , clampResult58);
			float3 lerpResult87 = lerp( desaturateVar116 , lerpResult17 , clampResult103);
			o.Albedo = lerpResult87;
			float lerpResult20 = lerp( _Mat2_Metallic , _Mat1_Metallic , clampResult58);
			float lerpResult89 = lerp( _Mat3_Metallic , lerpResult20 , clampResult103);
			o.Metallic = lerpResult89;
			float4 temp_cast_6 = (_Mat3_Roughness).xxxx;
			#ifdef _MAT3_ROUGHNESSTEXTURE_ON
				float4 staticSwitch85 = tex2D( _Mat3_RoughnessT, temp_output_78_0 );
			#else
				float4 staticSwitch85 = temp_cast_6;
			#endif
			float4 temp_cast_7 = (_Mat2_Roughness).xxxx;
			#ifdef _MAT2_ROUGHNESSTEXTURE_ON
				float4 staticSwitch15 = tex2D( _Mat2_RoughnessT, temp_output_50_0 );
			#else
				float4 staticSwitch15 = temp_cast_7;
			#endif
			float4 temp_cast_8 = (_Mat1_Roughness).xxxx;
			#ifdef _MAT1_ROUGHNESSTEXTURE_ON
				float4 staticSwitch13 = tex2D( _Mat1_RoughnessT, temp_output_46_0 );
			#else
				float4 staticSwitch13 = temp_cast_8;
			#endif
			float4 lerpResult113 = lerp( ( _Mat1_Roughness * staticSwitch13 ) , staticSwitch15 , tex2DNode40.r);
			float4 lerpResult19 = lerp( staticSwitch15 , lerpResult113 , clampResult58);
			float4 lerpResult90 = lerp( staticSwitch85 , lerpResult19 , clampResult103);
			o.Smoothness = lerpResult90.r;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "AmplifyShaderEditor.MaterialInspector"
}
/*ASEBEGIN
Version=19801
Node;AmplifyShaderEditor.TextureCoordinatesNode;91;-5919.711,-251.3643;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;92;-5964.612,-102.9712;Inherit;False;Property;_VColor_R_Scale;VColor_R_Scale;40;1;[Header];Create;True;2;Vertex Color_R;____________________;0;0;False;0;False;1.37;1.37;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;93;-5597.211,-207.0644;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;95;-5401.411,-31.57428;Inherit;True;Property;_BlendMask_R;BlendMask_R;45;0;Create;True;0;0;0;False;0;False;-1;abc00000000013913144832383490117;abc00000000013913144832383490117;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SamplerNode;105;-5402.399,215.386;Inherit;True;Property;_BlendDetail_R;BlendDetail_R;44;0;Create;True;0;0;0;False;0;False;-1;abc00000000013913144832383490117;abc00000000013913144832383490117;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SimpleAddOpNode;106;-5065.687,115.2861;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;107;-4981.188,293.386;Inherit;False;Property;_Detail_Fallof_R;Detail_Fallof_R;42;0;Create;True;0;0;0;False;0;False;3.4;0;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;22;-6042.909,-1523.444;Inherit;False;Property;_VColor_G_Scale;VColor_G_Scale;46;1;[Header];Create;True;2;Vertex Color_G;_____________________;0;0;False;0;False;1.37;1.75;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;23;-5998.007,-1671.837;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;49;-3003.359,-1130.062;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;48;-2975.765,-942.4423;Inherit;False;Property;_Mat2_UV;Mat2_UV;16;1;[Header];Create;True;2;Material_2;_________________;0;0;False;0;False;2;5;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;47;-3471.75,-2784.038;Inherit;False;Property;_Mat1_UV;Mat1_UV;0;1;[Header];Create;True;2;Material_1;_________________;0;0;False;0;False;0.77;5;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;45;-3452.458,-3006.651;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;25;-5675.507,-1627.537;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PowerNode;108;-4887.596,46.38593;Inherit;False;True;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;94;-5160.734,-436.1703;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;50;-2722.863,-1112.254;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;46;-3171.963,-2988.842;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;96;-4700.87,-183.5344;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;21;-5482.126,-2064.643;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;26;-5453.708,-1663.947;Inherit;True;Property;_Blend_Mask_G;Blend_Mask_G;49;0;Create;True;0;0;0;False;0;False;-1;abc00000000013913144832383490117;abc00000000013913144832383490117;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SamplerNode;12;-2448.598,-1200.224;Inherit;True;Property;_Mat2_BaseTexture;Mat2_BaseTexture;27;0;Create;True;0;0;0;False;0;False;-1;None;abc00000000014263446164930343250;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.ColorNode;131;-2816,-1408;Inherit;False;Property;_Mat2_Color;Mat2_Color;22;0;Create;True;0;0;0;False;0;False;1,1,1,0;0.6508099,0.6662837,0.7075471,0;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.TextureCoordinatesNode;123;-3562.637,-3701.915;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;52;-2880.862,-2814.735;Inherit;False;Property;_Mat1_Color;Mat1_Color;7;0;Create;True;0;0;0;False;0;False;1,1,1,0;0.2963687,0.3107867,0.3396226,0;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.RangedFloatNode;2;-2681.977,-2917.504;Inherit;False;Property;_Mat1_BaseColor;Mat1_BaseColor;3;0;Create;True;1;Material_1;0;0;False;0;False;5;2.38;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;10;-2865.102,-2584.05;Inherit;True;Property;_Mat1_BaseTexture;Mat1_BaseTexture;12;0;Create;True;0;0;0;False;0;False;-1;abc00000000012436328587221969967;abc00000000004508512144070620652;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.RangedFloatNode;121;-3677.579,-3520.308;Inherit;False;Property;_Mat1_BakedUV1;Mat1_BakedUV1;13;0;Create;True;0;0;0;False;0;False;1;1;0;20;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;98;-4561.256,-258.2123;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;97;-4685.777,54.49361;Inherit;False;Property;_Detail_Sharp_R;Detail_Sharp_R;43;0;Create;True;0;0;0;False;0;False;3.4;0;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;29;-5022.262,-1812.007;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;132;-2496,-1424;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;6;-2496,-1584;Inherit;False;Property;_Mat2_BaseColor;Mat2_BaseColor;20;0;Create;True;0;0;0;False;0;False;3.75;0.5;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;122;-3251.363,-3782.201;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.StaticSwitch;53;-2421.461,-2785.436;Inherit;False;Property;_UseColor;UseColor;6;0;Create;True;0;0;0;False;0;False;0;0;1;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;114;-2494.315,-2602.656;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.PowerNode;99;-4427.963,-195.7574;Inherit;False;True;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;34;-4882.652,-1886.685;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;38;-4575.629,-1625.969;Inherit;False;Property;_Global_G;Global_G;48;0;Create;True;0;0;0;False;0;False;1.1;0.7;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;133;-2256,-1424;Inherit;False;Property;_UseColor_2;UseColor_2;19;0;Create;True;0;0;0;False;0;False;0;0;1;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StaticSwitch;9;-2180.714,-2715.021;Inherit;False;Property;_Mat1_ColorTexture;Mat1_ColorTexture;10;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;77;-2651.171,-3882.924;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StaticSwitch;124;-2758.916,-3356.941;Inherit;False;Property;_Mat1_BakedMaskUV;Mat1_BakedMaskUV;9;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT2;0,0;False;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT2;0,0;False;6;FLOAT2;0,0;False;7;FLOAT2;0,0;False;8;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.StaticSwitch;11;-2086.365,-1264.931;Inherit;False;Property;_Mat2_ColorTexture;Mat2_ColorTexture;24;0;Create;True;0;0;0;False;0;False;0;1;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;76;-2623.578,-3695.304;Inherit;False;Property;_Mat3_UV;Mat3_UV;30;1;[Header];Create;True;2;Material_3;_________________;0;0;False;0;False;2;2;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;100;-4183.042,-240.2764;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;101;-4254.233,2.503613;Inherit;False;Property;_Global_R;Global_R;41;0;Create;True;0;0;0;False;0;False;1.1;1.1;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;115;-4229.22,-1691.583;Inherit;False;3;0;FLOAT;1;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;129;-3904,304;Inherit;False;Property;_Worn_Level;Worn_Level;50;0;Create;True;0;0;0;False;0;False;1;1.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;78;-2370.676,-3865.116;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;42;-1820.184,-2782.022;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;40;-1835.346,-3059.084;Inherit;True;Property;_Mat2_Mask_Variation;Mat2_Mask_Variation;29;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;36;-1847.89,-1417.621;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;102;-3932.672,-183.3544;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;130;-3856,-1648;Inherit;False;3;0;FLOAT;1;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;14;-2282.177,-2168.714;Inherit;True;Property;_Mat1_RoughnessT;Mat1_RoughnessT;14;0;Create;True;0;0;0;False;0;False;-1;None;abc00000000014263446164930343250;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.RangedFloatNode;3;-2256,-2288;Inherit;False;Property;_Mat1_Roughness;Mat1_Roughness;4;0;Create;True;0;0;0;False;0;False;0.5;0.28;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;-2040.761,-2521.577;Inherit;True;Property;_Mat1_Base_Normal;Mat1_Base_Normal;15;0;Create;True;0;0;0;False;0;False;-1;None;abc00000000016930792349297706691;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.ClampOpNode;58;-1830.79,-1761.042;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;5;-2103.992,-1154.513;Inherit;True;Property;_Mat2_Base_Normal;Mat2_Base_Normal;28;0;Create;True;0;0;0;False;0;False;-1;None;abc00000000016930792349297706691;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SamplerNode;16;-2396.822,-827.223;Inherit;True;Property;_Mat2_RoughnessT;Mat2_RoughnessT;26;0;Create;True;0;0;0;False;0;False;-1;None;abc00000000014263446164930343250;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.RangedFloatNode;7;-2394.308,-952.4541;Inherit;False;Property;_Mat2_Roughness;Mat2_Roughness;21;0;Create;True;0;0;0;False;0;False;0;0.43;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;79;-2096.411,-3953.086;Inherit;True;Property;_Mat3_BaseTexture;Mat3_BaseTexture;38;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.RangedFloatNode;80;-2060.324,-4102.271;Inherit;False;Property;_Mat3_BaseColor;Mat3_BaseColor;31;0;Create;True;0;0;0;False;0;False;3.75;3.75;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;41;-1430.915,-2662.903;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;127;-3696,-160;Inherit;False;3;0;FLOAT;1;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;43;-1658.813,-2524.642;Inherit;False;Property;_Mat1_TextureDetails;Mat1_TextureDetails;2;0;Create;True;0;0;0;False;0;False;0.37;0.291;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;13;-1944.107,-2223.572;Inherit;False;Property;_Mat1_RoughnessTexture;Mat1_RoughnessTexture;11;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ClampOpNode;103;-976,-1168;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;18;-1078.792,-1846.719;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;84;-1751.806,-3907.375;Inherit;True;Property;_Mat3_Base_Normal;Mat3_Base_Normal;39;0;Create;True;0;0;0;False;0;False;-1;abc00000000016124000965088101700;abc00000000016124000965088101700;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.LerpOp;44;-1327.607,-2544.285;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StaticSwitch;81;-1734.179,-4017.793;Inherit;False;Property;_Mat3_ColorTexture;Mat3_ColorTexture;37;0;Create;True;0;0;0;False;0;False;0;1;1;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StaticSwitch;15;-2072.979,-877.9038;Inherit;False;Property;_Mat2_RoughnessTexture;Mat2_RoughnessTexture;25;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;126;-1791.076,-1306.619;Inherit;False;Property;_Mat2_Desaturation;Mat2_Desaturation;17;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;112;-1484.663,-2222.139;Inherit;False;Property;_Mat1_Desaturation;Mat1_Desaturation;1;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;134;-1632,-2064;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;88;-409.1505,-1881.376;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;117;-1404.142,-3955.07;Inherit;False;Property;_Mat3_Desaturation;Mat3_Desaturation;32;0;Create;True;0;0;0;False;0;False;0.327;0.275;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.DesaturateOpNode;125;-1536.763,-1511.943;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;113;-1318.689,-1381.794;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;82;-2044.635,-3580.085;Inherit;True;Property;_Mat3_RoughnessT;Mat3_RoughnessT;36;0;Create;True;0;0;0;False;0;False;-1;abc00000000009428763281507424020;abc00000000009428763281507424020;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;104;-1435.62,-4105.841;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;83;-2042.121,-3705.316;Inherit;False;Property;_Mat3_Roughness;Mat3_Roughness;33;0;Create;True;0;0;0;False;0;False;0;0.47;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;4;-1883.439,-2313.027;Inherit;False;Property;_Mat1_Metallic;Mat1_Metallic;5;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.DesaturateOpNode;111;-1220.75,-2302.663;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;8;-2069.511,-967.07;Inherit;False;Property;_Mat2_Metallic;Mat2_Metallic;23;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;118;62.12686,-1398.377;Inherit;True;Property;_BakedNormal;BakedNormal;18;0;Create;True;0;0;0;False;0;False;-1;abc00000000014974086189499053042;abc00000000014974086189499053042;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.DesaturateOpNode;116;-1140.229,-4034.294;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;17;-1076.785,-2065.968;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;20;-1098.375,-1634.341;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;86;-1717.325,-3719.932;Inherit;False;Property;_Mat3_Metallic;Mat3_Metallic;34;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;85;-1720.792,-3630.766;Inherit;False;Property;_Mat3_RoughnessTexture;Mat3_RoughnessTexture;35;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;19;-1088,-1392;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.BlendNormalsNode;119;257.7593,-1651.033;Inherit;False;0;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;90;-410.1926,-1435.984;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;89;-403.017,-1646.578;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;87;-176,-2144;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;28;-5057.869,-1568.779;Inherit;False;Property;_Detail_Sharp_G;Detail_Sharp_G;47;0;Create;True;0;0;0;False;0;False;3.4;1.03;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;39;-4254.067,-1811.827;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;32;-4504.437,-1868.749;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;27;-4742.859,-1765.729;Inherit;False;True;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;120;496,-1824;Inherit;False;Property;_Mat1_BakedNormal;Mat1_BakedNormal;8;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT3;0,0,0;False;0;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT3;0,0,0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1374.926,-1847.719;Float;False;True;-1;2;AmplifyShaderEditor.MaterialInspector;0;0;Standard;M_BlendMaster_Metallic;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;17;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;93;0;91;0
WireConnection;93;1;92;0
WireConnection;95;1;93;0
WireConnection;106;0;95;2
WireConnection;106;1;105;1
WireConnection;25;0;23;0
WireConnection;25;1;22;0
WireConnection;108;0;106;0
WireConnection;108;1;107;0
WireConnection;50;0;49;0
WireConnection;50;1;48;0
WireConnection;46;0;45;0
WireConnection;46;1;47;0
WireConnection;96;0;94;1
WireConnection;96;1;108;0
WireConnection;26;1;25;0
WireConnection;12;1;50;0
WireConnection;10;1;46;0
WireConnection;98;0;94;1
WireConnection;98;1;96;0
WireConnection;29;0;21;2
WireConnection;29;1;26;1
WireConnection;132;0;131;0
WireConnection;132;1;12;0
WireConnection;122;0;123;0
WireConnection;122;1;121;0
WireConnection;53;1;2;0
WireConnection;53;0;52;0
WireConnection;114;0;52;0
WireConnection;114;1;10;0
WireConnection;99;0;98;0
WireConnection;99;1;97;0
WireConnection;34;0;21;2
WireConnection;34;1;29;0
WireConnection;133;1;6;0
WireConnection;133;0;132;0
WireConnection;9;1;53;0
WireConnection;9;0;114;0
WireConnection;124;1;46;0
WireConnection;124;0;122;0
WireConnection;11;1;133;0
WireConnection;11;0;12;0
WireConnection;100;0;99;0
WireConnection;115;1;34;0
WireConnection;115;2;38;0
WireConnection;78;0;77;0
WireConnection;78;1;76;0
WireConnection;42;0;2;0
WireConnection;42;1;9;0
WireConnection;40;1;124;0
WireConnection;36;0;6;0
WireConnection;36;1;11;0
WireConnection;102;0;100;0
WireConnection;102;1;101;0
WireConnection;130;1;115;0
WireConnection;130;2;129;0
WireConnection;14;1;46;0
WireConnection;1;1;46;0
WireConnection;58;0;130;0
WireConnection;5;1;50;0
WireConnection;16;1;50;0
WireConnection;79;1;78;0
WireConnection;41;0;42;0
WireConnection;41;1;36;0
WireConnection;41;2;40;1
WireConnection;127;1;102;0
WireConnection;127;2;129;0
WireConnection;13;1;3;0
WireConnection;13;0;14;0
WireConnection;103;0;127;0
WireConnection;18;0;5;0
WireConnection;18;1;1;0
WireConnection;18;2;58;0
WireConnection;84;1;78;0
WireConnection;44;0;42;0
WireConnection;44;1;41;0
WireConnection;44;2;43;0
WireConnection;81;1;80;0
WireConnection;81;0;79;0
WireConnection;15;1;7;0
WireConnection;15;0;16;0
WireConnection;134;0;3;0
WireConnection;134;1;13;0
WireConnection;88;0;84;0
WireConnection;88;1;18;0
WireConnection;88;2;103;0
WireConnection;125;0;36;0
WireConnection;125;1;126;0
WireConnection;113;0;134;0
WireConnection;113;1;15;0
WireConnection;113;2;40;1
WireConnection;82;1;78;0
WireConnection;104;0;80;0
WireConnection;104;1;81;0
WireConnection;111;0;44;0
WireConnection;111;1;112;0
WireConnection;118;1;122;0
WireConnection;116;0;104;0
WireConnection;116;1;117;0
WireConnection;17;0;125;0
WireConnection;17;1;111;0
WireConnection;17;2;58;0
WireConnection;20;0;8;0
WireConnection;20;1;4;0
WireConnection;20;2;58;0
WireConnection;85;1;83;0
WireConnection;85;0;82;0
WireConnection;19;0;15;0
WireConnection;19;1;113;0
WireConnection;19;2;58;0
WireConnection;119;0;88;0
WireConnection;119;1;118;0
WireConnection;90;0;85;0
WireConnection;90;1;19;0
WireConnection;90;2;103;0
WireConnection;89;0;86;0
WireConnection;89;1;20;0
WireConnection;89;2;103;0
WireConnection;87;0;116;0
WireConnection;87;1;17;0
WireConnection;87;2;103;0
WireConnection;39;0;32;0
WireConnection;39;1;38;0
WireConnection;32;0;27;0
WireConnection;27;0;34;0
WireConnection;27;1;28;0
WireConnection;120;1;88;0
WireConnection;120;0;119;0
WireConnection;0;0;87;0
WireConnection;0;1;120;0
WireConnection;0;3;89;0
WireConnection;0;4;90;0
ASEEND*/
//CHKSM=E09756A245AB93BC722CC24F2A6BB328D014C686