// Made with Amplify Shader Editor v1.9.8.1
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "M_CarBlend"
{
	Properties
	{
		[Toggle(_USE_R_MASK_ON)] _Use_R_Mask("Use_R_Mask", Float) = 0
		[Toggle(_USE_DUSTONLY_ON)] _Use_DustOnly("Use_DustOnly", Float) = 0
		_ColorBody("ColorBody", Color) = (1,1,1,0)
		_Car_Albedo("Car_Albedo", 2D) = "white" {}
		_Car_Normal("Car_Normal", 2D) = "bump" {}
		_Car_ARM("Car_ARM", 2D) = "white" {}
		_Metallic_Paint("Metallic_Paint", Range( 0 , 1)) = 1
		_Roughness_Paint("Roughness_Paint", Range( 0 , 5)) = 1
		_Brightness_Dust("Brightness_Dust", Range( 0 , 3)) = 1
		_Brightness_Rust("Brightness_Rust", Range( 0 , 3)) = 1
		_Mask_Rust_Global("Mask_Rust_Global", Range( 0 , 1)) = 0
		_Mask_Dust_Global("Mask_Dust_Global", Range( 0 , 1)) = 0
		_Mask("Mask", 2D) = "white" {}
		_Rust_Albedo("Rust_Albedo", 2D) = "white" {}
		_Rust_Normal("Rust_Normal", 2D) = "bump" {}
		_Dust_Albedo("Dust_Albedo", 2D) = "white" {}
		_T_Flat_Normal("T_Flat_Normal", 2D) = "bump" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#pragma target 3.5
		#pragma shader_feature_local _USE_DUSTONLY_ON
		#pragma shader_feature_local _USE_R_MASK_ON
		#define ASE_VERSION 19801
		#define ASE_USING_SAMPLING_MACROS 1
		#if defined(SHADER_API_D3D11) || defined(SHADER_API_XBOXONE) || defined(UNITY_COMPILER_HLSLCC) || defined(SHADER_API_PSSL) || (defined(SHADER_TARGET_SURFACE_ANALYSIS) && !defined(SHADER_TARGET_SURFACE_ANALYSIS_MOJOSHADER))//ASE Sampler Macros
		#define SAMPLE_TEXTURE2D(tex,samplerTex,coord) tex.Sample(samplerTex,coord)
		#else//ASE Sampling Macros
		#define SAMPLE_TEXTURE2D(tex,samplerTex,coord) tex2D(tex,coord)
		#endif//ASE Sampling Macros

		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		UNITY_DECLARE_TEX2D_NOSAMPLER(_Car_Normal);
		uniform float4 _Car_Normal_ST;
		SamplerState sampler_Linear_Repeat;
		UNITY_DECLARE_TEX2D_NOSAMPLER(_Rust_Normal);
		UNITY_DECLARE_TEX2D_NOSAMPLER(_Mask);
		uniform float4 _Mask_ST;
		SamplerState sampler_Mask;
		uniform float _Mask_Rust_Global;
		UNITY_DECLARE_TEX2D_NOSAMPLER(_T_Flat_Normal);
		uniform float4 _T_Flat_Normal_ST;
		SamplerState sampler_T_Flat_Normal;
		uniform float _Mask_Dust_Global;
		UNITY_DECLARE_TEX2D_NOSAMPLER(_Car_Albedo);
		uniform float4 _Car_Albedo_ST;
		SamplerState sampler_Car_Albedo;
		uniform float4 _ColorBody;
		UNITY_DECLARE_TEX2D_NOSAMPLER(_Rust_Albedo);
		uniform float _Brightness_Rust;
		UNITY_DECLARE_TEX2D_NOSAMPLER(_Dust_Albedo);
		uniform float _Brightness_Dust;
		UNITY_DECLARE_TEX2D_NOSAMPLER(_Car_ARM);
		uniform float4 _Car_ARM_ST;
		SamplerState sampler_Car_ARM;
		uniform float _Metallic_Paint;
		uniform float _Roughness_Paint;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Car_Normal = i.uv_texcoord * _Car_Normal_ST.xy + _Car_Normal_ST.zw;
			float3 tex2DNode22 = UnpackNormal( SAMPLE_TEXTURE2D( _Car_Normal, sampler_Linear_Repeat, uv_Car_Normal ) );
			float2 uv_TexCoord126 = i.uv_texcoord * float2( 6,6 );
			float2 uv_Mask = i.uv_texcoord * _Mask_ST.xy + _Mask_ST.zw;
			float4 tex2DNode51 = SAMPLE_TEXTURE2D( _Mask, sampler_Mask, uv_Mask );
			float3 lerpResult82 = lerp( tex2DNode22 , UnpackNormal( SAMPLE_TEXTURE2D( _Rust_Normal, sampler_Linear_Repeat, uv_TexCoord126 ) ) , ( tex2DNode51.b * _Mask_Rust_Global ));
			#ifdef _USE_DUSTONLY2_ON
				float3 staticSwitch138 = tex2DNode22;
			#else
				float3 staticSwitch138 = lerpResult82;
			#endif
			float2 uv_T_Flat_Normal = i.uv_texcoord * _T_Flat_Normal_ST.xy + _T_Flat_Normal_ST.zw;
			#ifdef _USE_R_MASK_ON
				float staticSwitch145 = tex2DNode51.r;
			#else
				float staticSwitch145 = tex2DNode51.g;
			#endif
			float3 lerpResult83 = lerp( staticSwitch138 , UnpackNormal( SAMPLE_TEXTURE2D( _T_Flat_Normal, sampler_T_Flat_Normal, uv_T_Flat_Normal ) ) , ( staticSwitch145 * _Mask_Dust_Global ));
			o.Normal = lerpResult83;
			float2 uv_Car_Albedo = i.uv_texcoord * _Car_Albedo_ST.xy + _Car_Albedo_ST.zw;
			float4 tex2DNode21 = SAMPLE_TEXTURE2D( _Car_Albedo, sampler_Car_Albedo, uv_Car_Albedo );
			float4 lerpResult73 = lerp( tex2DNode21 , ( tex2DNode21 * _ColorBody ) , tex2DNode51.r);
			float4 lerpResult74 = lerp( lerpResult73 , ( SAMPLE_TEXTURE2D( _Rust_Albedo, sampler_Linear_Repeat, uv_TexCoord126 ) * _Brightness_Rust ) , ( tex2DNode51.b * _Mask_Rust_Global ));
			#ifdef _USE_DUSTONLY_ON
				float4 staticSwitch137 = lerpResult73;
			#else
				float4 staticSwitch137 = lerpResult74;
			#endif
			float4 lerpResult75 = lerp( staticSwitch137 , ( SAMPLE_TEXTURE2D( _Dust_Albedo, sampler_Linear_Repeat, uv_TexCoord126 ) * _Brightness_Dust ) , ( staticSwitch145 * _Mask_Dust_Global ));
			o.Albedo = lerpResult75.rgb;
			float2 uv_Car_ARM = i.uv_texcoord * _Car_ARM_ST.xy + _Car_ARM_ST.zw;
			float4 tex2DNode23 = SAMPLE_TEXTURE2D( _Car_ARM, sampler_Car_ARM, uv_Car_ARM );
			float lerpResult135 = lerp( ( tex2DNode23.b * _Metallic_Paint ) , 0.0 , ( tex2DNode51.b * _Mask_Rust_Global ));
			o.Metallic = lerpResult135;
			float2 appendResult91 = (float2(( 1.0 - ( tex2DNode23.g * _Roughness_Paint ) ) , tex2DNode23.b));
			float2 lerpResult94 = lerp( appendResult91 , float2( 0,0 ) , ( tex2DNode51.b * _Mask_Rust_Global ));
			#ifdef _USE_DUSTONLY2_ON
				float2 staticSwitch141 = appendResult91;
			#else
				float2 staticSwitch141 = lerpResult94;
			#endif
			float2 lerpResult95 = lerp( staticSwitch141 , float2( 0,0 ) , ( staticSwitch145 * _Mask_Dust_Global ));
			o.Smoothness = lerpResult95.x;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "AmplifyShaderEditor.MaterialInspector"
}
/*ASEBEGIN
Version=19801
Node;AmplifyShaderEditor.CommentaryNode;34;-2752,-1008;Inherit;False;555.0779;669.0986;base;4;23;22;21;132;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;23;-2704,-560;Inherit;True;Property;_Car_ARM;Car_ARM;5;0;Create;True;0;0;0;False;0;False;-1;None;abc00000000012376289847282164287;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.RangedFloatNode;133;-2656,-320;Inherit;False;Property;_Roughness_Paint;Roughness_Paint;7;0;Create;True;0;0;0;False;0;False;1;1;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;51;-1760,608;Inherit;True;Property;_Mask;Mask;12;0;Create;True;0;0;0;False;0;False;-1;None;1eaea4ede740fe9439b532c538cf5782;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.RangedFloatNode;19;-1727.579,857.4171;Inherit;False;Property;_Mask_Rust_Global;Mask_Rust_Global;10;0;Create;True;0;0;0;False;0;False;0;0.938;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;126;-2800,352;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;6,6;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;55;-1904,-1168;Inherit;False;Property;_ColorBody;ColorBody;2;0;Create;True;0;0;0;False;0;False;1,1,1,0;1,1,1,0;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SamplerNode;21;-2704,-960;Inherit;True;Property;_Car_Albedo;Car_Albedo;3;0;Create;True;0;0;0;False;0;False;-1;None;abc00000000000771911723007341563;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;132;-2320,-496;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerStateNode;175;-2832,-48;Inherit;False;0;0;0;1;-1;None;1;0;SAMPLER2D;;False;1;SAMPLERSTATE;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;102;-1299.834,518.3639;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerStateNode;176;-2992,-192;Inherit;False;0;0;0;1;-1;None;1;0;SAMPLER2D;;False;1;SAMPLERSTATE;0
Node;AmplifyShaderEditor.SamplerNode;26;-2293.555,-225.8342;Inherit;True;Property;_Rust_Albedo;Rust_Albedo;13;0;Create;True;0;0;0;False;0;False;-1;None;abc00000000016885468317101039418;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;56;-1632,-1216;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;29;-2128,-528;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;41;-1760,-720;Inherit;False;Property;_Brightness_Rust;Brightness_Rust;9;0;Create;True;0;0;0;False;0;False;1;1.070074;0;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;27;-2282.012,-28.31627;Inherit;True;Property;_Rust_Normal;Rust_Normal;14;0;Create;True;0;0;0;False;0;False;-1;None;abc00000000009952354959572609988;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SamplerNode;22;-2688,-768;Inherit;True;Property;_Car_Normal;Car_Normal;4;0;Create;True;0;0;0;False;0;False;-1;None;abc00000000006359049767341128384;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.RelayNode;100;-1121.831,453.9733;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;20;-1728,960;Inherit;False;Property;_Mask_Dust_Global;Mask_Dust_Global;11;0;Create;True;0;0;0;False;0;False;0;0.83;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;145;-1216,864;Inherit;False;Property;_Use_R_Mask;Use_R_Mask;0;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;36;-2352.185,377.0976;Inherit;False;555.08;670.4985;dust;1;30;;1,1,1,1;0;0
Node;AmplifyShaderEditor.LerpOp;73;-1104,-1184;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.DynamicAppendNode;91;-1728,-960;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;42;-1440,-832;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;159;-2112,-400;Inherit;False;Property;_Metallic_Paint;Metallic_Paint;6;0;Create;True;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;82;-704,-240;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;103;-864,1008;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;30;-2302.185,427.0976;Inherit;True;Property;_Dust_Albedo;Dust_Albedo;15;0;Create;True;0;0;0;False;0;False;-1;None;0c0e0cb0d9f64858856bd2163b1273c0;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.RangedFloatNode;46;-864,-848;Inherit;False;Property;_Brightness_Dust;Brightness_Dust;8;0;Create;True;0;0;0;False;0;False;1;1;0;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;74;-640,-1184;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;94;-176,736;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;158;-1744,-240;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RelayNode;101;-704,1008;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;138;-496,-144;Inherit;False;Property;_Use_DustOnly1;Use_DustOnly;1;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Reference;139;True;True;All;9;1;FLOAT3;0,0,0;False;0;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT3;0,0,0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;47;-592,-912;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StaticSwitch;137;-416,-1088;Inherit;False;Property;_Use_DustOnly;Use_DustOnly;1;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StaticSwitch;141;-16,640;Inherit;False;Property;_Use_DustOnly3;Use_DustOnly;1;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Reference;139;True;True;All;9;1;FLOAT2;0,0;False;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT2;0,0;False;6;FLOAT2;0,0;False;7;FLOAT2;0,0;False;8;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;144;-496,224;Inherit;True;Property;_T_Flat_Normal;T_Flat_Normal;16;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.LerpOp;135;288,1232;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;83;-192,-112;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;75;-96,-992;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;95;256,784;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.LerpOp;154;1440,-48;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;139;880,-272;Inherit;False;Property;_Use_DustOnly2;Use_DustOnly;1;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Reference;137;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;146;1952,-176;Float;False;True;-1;3;AmplifyShaderEditor.MaterialInspector;0;0;Standard;M_CarBlend;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;True;17;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;132;0;23;2
WireConnection;132;1;133;0
WireConnection;102;0;51;3
WireConnection;102;1;19;0
WireConnection;26;1;126;0
WireConnection;26;7;175;0
WireConnection;56;0;21;0
WireConnection;56;1;55;0
WireConnection;29;0;132;0
WireConnection;27;1;126;0
WireConnection;27;7;176;0
WireConnection;22;7;176;0
WireConnection;100;0;102;0
WireConnection;145;1;51;2
WireConnection;145;0;51;1
WireConnection;73;0;21;0
WireConnection;73;1;56;0
WireConnection;73;2;51;1
WireConnection;91;0;29;0
WireConnection;91;1;23;3
WireConnection;42;0;26;0
WireConnection;42;1;41;0
WireConnection;82;0;22;0
WireConnection;82;1;27;0
WireConnection;82;2;100;0
WireConnection;103;0;145;0
WireConnection;103;1;20;0
WireConnection;30;1;126;0
WireConnection;30;7;175;0
WireConnection;74;0;73;0
WireConnection;74;1;42;0
WireConnection;74;2;100;0
WireConnection;94;0;91;0
WireConnection;94;2;100;0
WireConnection;158;0;23;3
WireConnection;158;1;159;0
WireConnection;101;0;103;0
WireConnection;138;1;82;0
WireConnection;138;0;22;0
WireConnection;47;0;30;0
WireConnection;47;1;46;0
WireConnection;137;1;74;0
WireConnection;137;0;73;0
WireConnection;141;1;94;0
WireConnection;141;0;91;0
WireConnection;135;0;158;0
WireConnection;135;2;100;0
WireConnection;83;0;138;0
WireConnection;83;1;144;0
WireConnection;83;2;101;0
WireConnection;75;0;137;0
WireConnection;75;1;47;0
WireConnection;75;2;101;0
WireConnection;95;0;141;0
WireConnection;95;2;101;0
WireConnection;154;0;139;0
WireConnection;146;0;75;0
WireConnection;146;1;83;0
WireConnection;146;3;135;0
WireConnection;146;4;95;0
ASEEND*/
//CHKSM=7E74443AB271675CB008888350F42D008BEB4C26