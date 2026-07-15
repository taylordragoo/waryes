// Made with Amplify Shader Editor v1.9.8.1
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "M_Standart_Blend"
{
	Properties
	{
		[Header(Vertex Color_G)][Header(_____________________)]_VColor_G_Scale("VColor_G_Scale", Range( 0 , 10)) = 1.37
		_Global_G("Global_G", Range( 0 , 5)) = 1.1
		[Toggle(_USE_LAYERB_UV_ON)] _Use_LayerB_UV("Use_LayerB_UV", Float) = 0
		[Toggle(_USE_WORNLEVEL_ON)] _Use_WornLevel("Use_WornLevel", Float) = 0
		[Toggle(_LAYER_SWAP_ON)] _Layer_Swap("Layer_Swap", Float) = 0
		[Header(Global)][Header(_____________________)]_Brightness("Brightness", Range( 0 , 5)) = 1
		_Desaturation("Desaturation", Range( 0 , 1)) = 0
		[Toggle(_NON_METALLIC_ON)] _Non_Metallic("Non_Metallic", Float) = 0
		[Toggle(_ROUGH_IN_ALPHA_ON)] _Rough_in_Alpha("Rough_in_Alpha", Float) = 0
		[Toggle(_ADD_MASK_ON)] _Add_Mask("Add_Mask", Float) = 0
		_UV_B("UV_B", Range( 0 , 10)) = 1
		_UV("UV", Range( 0 , 10)) = 1
		[Toggle(_UV_SPLIT_ON)] _UV_Split("UV_Split", Float) = 0
		_UV_Separate("UV_Separate", Vector) = (1,1,0,0)
		_Mask("Mask", 2D) = "white" {}
		_MaskTexture("MaskTexture", 2D) = "white" {}
		[Header(Layer_A)][Header(_____________________)]_Color_A("Color_A", Color) = (1,1,1,0)
		_Albedo_A("Albedo_A", 2D) = "white" {}
		_Brightness_A("Brightness_A", Range( 0 , 5)) = 1
		_Desaturation_A("Desaturation_A", Range( 0 , 1)) = 0
		_Roughness_A("Roughness_A", 2D) = "white" {}
		_Rough_A("Rough_A", Range( 0 , 5)) = 1
		_Normal_A("Normal_A", 2D) = "bump" {}
		_Metallic_A("Metallic_A", 2D) = "white" {}
		[Header(Layer_B)][Header(_____________________)]_Albedo_B("Albedo_B", 2D) = "white" {}
		_Color_B("Color_B", Color) = (1,1,1,0)
		_Brightness_B("Brightness_B", Range( 0 , 5)) = 1
		_Worn_Level("Worn_Level", Range( 0 , 1)) = 1
		_Desaturation_B("Desaturation_B", Range( 0 , 1)) = 0
		_Normal_B("Normal_B", 2D) = "bump" {}
		_Roughness_B("Roughness_B", 2D) = "white" {}
		_Rough_B("Rough_B", Range( -5 , 5)) = 1
		_Metallic_B("Metallic_B", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma shader_feature_local _UV_SPLIT_ON
		#pragma shader_feature_local _USE_WORNLEVEL_ON
		#pragma shader_feature_local _LAYER_SWAP_ON
		#pragma shader_feature_local _ADD_MASK_ON
		#pragma shader_feature_local _USE_LAYERB_UV_ON
		#pragma shader_feature_local _NON_METALLIC_ON
		#pragma shader_feature_local _ROUGH_IN_ALPHA_ON
		#define ASE_VERSION 19801
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows dithercrossfade 
		struct Input
		{
			float2 uv_texcoord;
			float4 vertexColor : COLOR;
		};

		uniform sampler2D _Normal_A;
		uniform float2 _UV_Separate;
		uniform float _UV;
		uniform sampler2D _Normal_B;
		uniform sampler2D _Mask;
		uniform float _VColor_G_Scale;
		uniform float _Global_G;
		uniform float _Worn_Level;
		uniform float _Brightness;
		uniform float _Brightness_A;
		uniform float4 _Color_A;
		uniform sampler2D _Albedo_A;
		uniform float _Desaturation_A;
		uniform float _Brightness_B;
		uniform float4 _Color_B;
		uniform sampler2D _Albedo_B;
		uniform float _UV_B;
		uniform float _Desaturation_B;
		uniform sampler2D _MaskTexture;
		uniform float _Desaturation;
		uniform sampler2D _Metallic_A;
		uniform sampler2D _Metallic_B;
		uniform float _Rough_A;
		uniform sampler2D _Roughness_A;
		uniform float _Rough_B;
		uniform sampler2D _Roughness_B;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float4 appendResult109 = (float4(1.0 , 1.0 , 0.0 , 0.0));
			#ifdef _UV_SPLIT_ON
				float4 staticSwitch110 = float4( _UV_Separate, 0.0 , 0.0 );
			#else
				float4 staticSwitch110 = appendResult109;
			#endif
			float2 uv_TexCoord55 = i.uv_texcoord * staticSwitch110.xy;
			float2 temp_output_56_0 = ( uv_TexCoord55 * _UV );
			float2 uv_TexCoord68 = i.uv_texcoord * staticSwitch110.xy;
			float lerpResult93 = lerp( 1.0 , ( i.vertexColor.g + ( i.vertexColor.g * tex2D( _Mask, ( uv_TexCoord68 * _VColor_G_Scale ) ).r ) ) , _Global_G);
			#ifdef _LAYER_SWAP_ON
				float staticSwitch95 = ( 1.0 - lerpResult93 );
			#else
				float staticSwitch95 = lerpResult93;
			#endif
			float clampResult97 = clamp( staticSwitch95 , 0.0 , 1.0 );
			float lerpResult112 = lerp( 0.0 , clampResult97 , _Worn_Level);
			#ifdef _USE_WORNLEVEL_ON
				float staticSwitch114 = lerpResult112;
			#else
				float staticSwitch114 = clampResult97;
			#endif
			float3 lerpResult73 = lerp( UnpackNormal( tex2D( _Normal_A, temp_output_56_0 ) ) , UnpackNormal( tex2D( _Normal_B, temp_output_56_0 ) ) , staticSwitch114);
			o.Normal = lerpResult73;
			float4 tex2DNode40 = tex2D( _Albedo_A, temp_output_56_0 );
			float3 desaturateInitialColor87 = ( _Brightness_A * ( _Color_A * tex2DNode40 ) ).rgb;
			float desaturateDot87 = dot( desaturateInitialColor87, float3( 0.299, 0.587, 0.114 ));
			float3 desaturateVar87 = lerp( desaturateInitialColor87, desaturateDot87.xxx, _Desaturation_A );
			#ifdef _USE_LAYERB_UV_ON
				float2 staticSwitch115 = ( i.uv_texcoord * _UV_B );
			#else
				float2 staticSwitch115 = temp_output_56_0;
			#endif
			float4 tex2DNode80 = tex2D( _Albedo_B, staticSwitch115 );
			float3 desaturateInitialColor42 = ( _Brightness_B * ( _Color_B * tex2DNode80 ) ).rgb;
			float desaturateDot42 = dot( desaturateInitialColor42, float3( 0.299, 0.587, 0.114 ));
			float3 desaturateVar42 = lerp( desaturateInitialColor42, desaturateDot42.xxx, _Desaturation_B );
			float3 lerpResult60 = lerp( desaturateVar87 , desaturateVar42 , staticSwitch114);
			float3 lerpResult65 = lerp( lerpResult60 , desaturateVar87 , tex2D( _MaskTexture, temp_output_56_0 ).r);
			#ifdef _ADD_MASK_ON
				float3 staticSwitch75 = lerpResult65;
			#else
				float3 staticSwitch75 = lerpResult60;
			#endif
			float3 desaturateInitialColor84 = ( _Brightness * staticSwitch75 );
			float desaturateDot84 = dot( desaturateInitialColor84, float3( 0.299, 0.587, 0.114 ));
			float3 desaturateVar84 = lerp( desaturateInitialColor84, desaturateDot84.xxx, _Desaturation );
			o.Albedo = desaturateVar84;
			float lerpResult57 = lerp( tex2D( _Metallic_A, temp_output_56_0 ).r , tex2D( _Metallic_B, staticSwitch115 ).r , staticSwitch114);
			#ifdef _NON_METALLIC_ON
				float staticSwitch119 = 0.0;
			#else
				float staticSwitch119 = lerpResult57;
			#endif
			o.Metallic = staticSwitch119;
			#ifdef _ROUGH_IN_ALPHA_ON
				float staticSwitch102 = ( 1.0 - tex2DNode40.a );
			#else
				float staticSwitch102 = tex2D( _Roughness_A, temp_output_56_0 ).r;
			#endif
			#ifdef _ROUGH_IN_ALPHA_ON
				float staticSwitch103 = ( 1.0 - tex2DNode80.a );
			#else
				float staticSwitch103 = tex2D( _Roughness_B, staticSwitch115 ).r;
			#endif
			float lerpResult50 = lerp( ( _Rough_A * staticSwitch102 ) , ( _Rough_B * staticSwitch103 ) , staticSwitch114);
			o.Smoothness = lerpResult50;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "AmplifyShaderEditor.MaterialInspector"
}
/*ASEBEGIN
Version=19801
Node;AmplifyShaderEditor.RangedFloatNode;107;-3776,-592;Inherit;False;Constant;_Float1;Float 1;34;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;108;-3872,-512;Inherit;False;Constant;_Float2;Float 2;34;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;109;-3632,-560;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.Vector2Node;106;-4032,-912;Inherit;False;Property;_UV_Separate;UV_Separate;14;0;Create;True;0;0;0;False;0;False;1,1;1,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.StaticSwitch;110;-3696,-864;Inherit;False;Property;_UV_Split;UV_Split;13;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT4;0,0,0,0;False;0;FLOAT4;0,0,0,0;False;2;FLOAT4;0,0,0,0;False;3;FLOAT4;0,0,0,0;False;4;FLOAT4;0,0,0,0;False;5;FLOAT4;0,0,0,0;False;6;FLOAT4;0,0,0,0;False;7;FLOAT4;0,0,0,0;False;8;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;67;-5126.986,554.2946;Inherit;False;Property;_VColor_G_Scale;VColor_G_Scale;0;1;[Header];Create;True;2;Vertex Color_G;_____________________;0;0;False;0;False;1.37;1;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;68;-5082.083,405.9005;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;69;-4759.583,450.2004;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.VertexColorNode;70;-4566.204,13.09448;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;77;-4537.785,413.7905;Inherit;True;Property;_Mask;Mask;15;0;Create;True;0;0;0;False;0;False;-1;abc00000000002072991696755431752;abc00000000002072991696755431752;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;71;-4106.339,265.7305;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;82;-3440,-448;Inherit;False;Property;_UV;UV;12;1;[Header];Create;True;0;0;0;False;0;False;1;1;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;55;-3392,-704;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;117;-3424,-320;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;116;-3472,-64;Inherit;False;Property;_UV_B;UV_B;11;1;[Header];Create;True;0;0;0;False;0;False;1;1;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;72;-3966.728,191.0525;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;94;-3770.525,415.4344;Inherit;False;Property;_Global_G;Global_G;1;0;Create;True;0;0;0;False;0;False;1.1;4.62;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;56;-3104,-688;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;118;-3152,-176;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.LerpOp;93;-3464.31,558.1595;Inherit;False;3;0;FLOAT;1;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;115;-2992,-272;Inherit;False;Property;_Use_LayerB_UV;Use_LayerB_UV;3;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT2;0,0;False;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT2;0,0;False;6;FLOAT2;0,0;False;7;FLOAT2;0,0;False;8;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.OneMinusNode;92;-3094.413,338.5095;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;40;-1991.771,-1468.966;Inherit;True;Property;_Albedo_A;Albedo_A;18;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SamplerNode;80;-2148.599,-919.2554;Inherit;True;Property;_Albedo_B;Albedo_B;25;1;[Header];Create;True;2;Layer_B;_____________________;0;0;False;0;False;-1;None;abc00000000001626570680784768565;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.StaticSwitch;95;-2966.718,152.7834;Inherit;False;Property;_Layer_Swap;Layer_Swap;5;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;78;-2153.783,-1129.978;Inherit;False;Property;_Color_B;Color_B;26;0;Create;True;0;0;0;False;0;False;1,1,1,0;1,1,1,0;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.ColorNode;79;-2088.895,-1722.953;Inherit;False;Property;_Color_A;Color_A;17;1;[Header];Create;True;2;Layer_A;_____________________;0;0;False;0;False;1,1,1,0;1,0.9639329,0.9294118,0;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.RangedFloatNode;111;-2784,384;Inherit;False;Property;_Worn_Level;Worn_Level;28;0;Create;True;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;97;-2672,64;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;39;-1702.471,-1597.605;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;43;-1860.652,-1053.303;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;44;-1638.743,-1162.234;Inherit;False;Property;_Brightness_B;Brightness_B;27;0;Create;True;0;0;0;False;0;False;1;0.99;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;61;-1499.491,-1674.087;Inherit;False;Property;_Brightness_A;Brightness_A;19;0;Create;True;0;0;0;False;0;False;1;1.13;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;112;-2512,176;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;38;-1158.974,-1653.904;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;41;-1329.324,-936.5401;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;62;-1168.072,-1535.643;Inherit;False;Property;_Desaturation_A;Desaturation_A;20;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;85;-1310.028,-1045.424;Inherit;False;Property;_Desaturation_B;Desaturation_B;29;0;Create;True;0;0;0;False;0;False;0;0.089;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;114;-2400,64;Inherit;False;Property;_Use_WornLevel;Use_WornLevel;4;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DesaturateOpNode;42;-958.2323,-978.626;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DesaturateOpNode;87;-839.1128,-1620.247;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;64;-366.7427,-936.0366;Inherit;True;Property;_MaskTexture;MaskTexture;16;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.LerpOp;60;-593.4539,-1284.87;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;51;-1779.327,-25.89453;Inherit;True;Property;_Roughness_B;Roughness_B;31;0;Create;True;0;0;0;False;0;False;-1;None;abc00000000002413589591361546280;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SamplerNode;59;-1837.766,-412.2805;Inherit;True;Property;_Roughness_A;Roughness_A;21;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.OneMinusNode;104;-1795.115,-683.5807;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;105;-1712,-1328;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;65;-16.05353,-1395.436;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StaticSwitch;103;-1392,48;Inherit;False;Property;_Rough_in_Alpha1;Rough_in_Alpha;9;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Reference;102;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;81;357.4417,-1421.676;Inherit;False;Property;_Brightness;Brightness;6;1;[Header];Create;True;2;Global;_____________________;0;0;False;0;False;1;1.01;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;75;300.3616,-1316.881;Inherit;False;Property;_Add_Mask;Add_Mask;10;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT3;0,0,0;False;0;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT3;0,0,0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StaticSwitch;102;-1504,-352;Inherit;False;Property;_Rough_in_Alpha;Rough_in_Alpha;9;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;52;-1780.716,509.1035;Inherit;True;Property;_Metallic_B;Metallic_B;33;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SamplerNode;53;-1776,272;Inherit;True;Property;_Metallic_A;Metallic_A;24;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.RangedFloatNode;49;-1765.597,-135.2725;Inherit;False;Property;_Rough_B;Rough_B;32;0;Create;True;0;0;0;False;0;False;1;0.6;-5;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;47;-1807.81,-533.8275;Inherit;False;Property;_Rough_A;Rough_A;22;0;Create;True;0;0;0;False;0;False;1;1;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;46;-1264,-528;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;54;-1759.065,856.7936;Inherit;True;Property;_Normal_A;Normal_A;23;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SamplerNode;98;-1753.462,1101.17;Inherit;True;Property;_Normal_B;Normal_B;30;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;63;663.8107,-1314.019;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;83;551.8387,-1156.976;Inherit;False;Property;_Desaturation;Desaturation;7;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;57;-992,272;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;120;-950.674,164.2086;Inherit;False;Constant;_Float0;Float 0;34;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;48;-1296,-112;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;50;-909.7442,-359.9275;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;73;-957.0308,940.1334;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StaticSwitch;113;-5795.308,-452.7053;Inherit;False;Property;_Layer_Swap1;Layer_Swap;2;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DesaturateOpNode;84;888.7836,-1304.045;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StaticSwitch;119;-800,224;Inherit;False;Property;_Non_Metallic;Non_Metallic;8;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1328,-1200;Float;False;True;-1;2;AmplifyShaderEditor.MaterialInspector;0;0;Standard;M_Standart_Blend;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;17;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;109;0;107;0
WireConnection;109;1;108;0
WireConnection;110;1;109;0
WireConnection;110;0;106;0
WireConnection;68;0;110;0
WireConnection;69;0;68;0
WireConnection;69;1;67;0
WireConnection;77;1;69;0
WireConnection;71;0;70;2
WireConnection;71;1;77;1
WireConnection;55;0;110;0
WireConnection;72;0;70;2
WireConnection;72;1;71;0
WireConnection;56;0;55;0
WireConnection;56;1;82;0
WireConnection;118;0;117;0
WireConnection;118;1;116;0
WireConnection;93;1;72;0
WireConnection;93;2;94;0
WireConnection;115;1;56;0
WireConnection;115;0;118;0
WireConnection;92;0;93;0
WireConnection;40;1;56;0
WireConnection;80;1;115;0
WireConnection;95;1;93;0
WireConnection;95;0;92;0
WireConnection;97;0;95;0
WireConnection;39;0;79;0
WireConnection;39;1;40;0
WireConnection;43;0;78;0
WireConnection;43;1;80;0
WireConnection;112;1;97;0
WireConnection;112;2;111;0
WireConnection;38;0;61;0
WireConnection;38;1;39;0
WireConnection;41;0;44;0
WireConnection;41;1;43;0
WireConnection;114;1;97;0
WireConnection;114;0;112;0
WireConnection;42;0;41;0
WireConnection;42;1;85;0
WireConnection;87;0;38;0
WireConnection;87;1;62;0
WireConnection;64;1;56;0
WireConnection;60;0;87;0
WireConnection;60;1;42;0
WireConnection;60;2;114;0
WireConnection;51;1;115;0
WireConnection;59;1;56;0
WireConnection;104;0;80;4
WireConnection;105;0;40;4
WireConnection;65;0;60;0
WireConnection;65;1;87;0
WireConnection;65;2;64;1
WireConnection;103;1;51;1
WireConnection;103;0;104;0
WireConnection;75;1;60;0
WireConnection;75;0;65;0
WireConnection;102;1;59;1
WireConnection;102;0;105;0
WireConnection;52;1;115;0
WireConnection;53;1;56;0
WireConnection;46;0;47;0
WireConnection;46;1;102;0
WireConnection;54;1;56;0
WireConnection;98;1;56;0
WireConnection;63;0;81;0
WireConnection;63;1;75;0
WireConnection;57;0;53;1
WireConnection;57;1;52;1
WireConnection;57;2;114;0
WireConnection;48;0;49;0
WireConnection;48;1;103;0
WireConnection;50;0;46;0
WireConnection;50;1;48;0
WireConnection;50;2;114;0
WireConnection;73;0;54;0
WireConnection;73;1;98;0
WireConnection;73;2;114;0
WireConnection;84;0;63;0
WireConnection;84;1;83;0
WireConnection;119;1;57;0
WireConnection;119;0;120;0
WireConnection;0;0;84;0
WireConnection;0;1;73;0
WireConnection;0;3;119;0
WireConnection;0;4;50;0
ASEEND*/
//CHKSM=4DEE5E8D55A13FF126726762FBB3FA54B14E9B6E