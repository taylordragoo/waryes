// Made with Amplify Shader Editor v1.9.8
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "M_Glass"
{
	Properties
	{
		[Toggle(_ROUGH_IN_B_ON)] _Rough_In_B("Rough_In_B", Float) = 0
		[Toggle(_USEALBEDO_ON)] _UseAlbedo("UseAlbedo", Float) = 0
		_Albedo("Albedo", 2D) = "white" {}
		_Brightness("Brightness", Range( 0 , 5)) = 1
		_A("A", Range( -1 , 3)) = 1.3
		_B("B", Range( -1 , 3)) = 0
		_Desaturation("Desaturation", Range( 0 , 1)) = 0
		_Metallic("Metallic", Range( 0 , 1)) = 0
		_Opacity_A("Opacity_A", Range( 0 , 3)) = 0.5
		_Opacity_B("Opacity_B", Range( 0 , 3)) = 0.5
		_Mask("Mask", 2D) = "white" {}
		_BaseColor_A("Base Color_A", Color) = (0.5294118,0.5294118,0.5294118,1)
		_BaseColor_B("Base Color_B", Color) = (0.5294118,0.5294118,0.5294118,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" }
		Cull Off
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#pragma shader_feature_local _USEALBEDO_ON
		#pragma shader_feature_local _ROUGH_IN_B_ON
		#define ASE_VERSION 19800
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float4 _BaseColor_A;
		uniform float4 _BaseColor_B;
		uniform sampler2D _Mask;
		uniform float4 _Mask_ST;
		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform float _Brightness;
		uniform float _Desaturation;
		uniform float _Metallic;
		uniform float _A;
		uniform float _B;
		uniform float _Opacity_A;
		uniform float _Opacity_B;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Mask = i.uv_texcoord * _Mask_ST.xy + _Mask_ST.zw;
			float4 tex2DNode12 = tex2D( _Mask, uv_Mask );
			float4 lerpResult4 = lerp( _BaseColor_A , _BaseColor_B , tex2DNode12.g);
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			#ifdef _USEALBEDO_ON
				float4 staticSwitch5 = ( tex2D( _Albedo, uv_Albedo ) * _Brightness );
			#else
				float4 staticSwitch5 = lerpResult4;
			#endif
			float3 desaturateInitialColor6 = staticSwitch5.rgb;
			float desaturateDot6 = dot( desaturateInitialColor6, float3( 0.299, 0.587, 0.114 ));
			float3 desaturateVar6 = lerp( desaturateInitialColor6, desaturateDot6.xxx, _Desaturation );
			o.Albedo = desaturateVar6;
			o.Metallic = _Metallic;
			#ifdef _ROUGH_IN_B_ON
				float staticSwitch13 = tex2DNode12.b;
			#else
				float staticSwitch13 = tex2DNode12.g;
			#endif
			float lerpResult14 = lerp( _A , _B , staticSwitch13);
			o.Smoothness = lerpResult14;
			float lerpResult17 = lerp( _Opacity_A , _Opacity_B , tex2DNode12.g);
			o.Alpha = lerpResult17;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard alpha:fade keepalpha fullforwardshadows 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19800
Node;AmplifyShaderEditor.ColorNode;2;-864,-144;Inherit;False;Property;_BaseColor_A;Base Color_A;11;0;Create;True;0;0;0;False;0;False;0.5294118,0.5294118,0.5294118,1;0.5294118,0.5294118,0.5294118,1;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.ColorNode;3;-864,80;Inherit;False;Property;_BaseColor_B;Base Color_B;12;0;Create;True;0;0;0;False;0;False;0.5294118,0.5294118,0.5294118,0;0.5294118,0.5294118,0.5294118,0;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SamplerNode;9;-1072,320;Inherit;True;Property;_Albedo;Albedo;2;0;Create;True;0;0;0;False;0;False;-1;abc00000000004754645270228602497;abc00000000004754645270228602497;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.RangedFloatNode;11;-736,496;Inherit;False;Property;_Brightness;Brightness;3;0;Create;True;0;0;0;False;0;False;1;1;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;12;-1136,864;Inherit;True;Property;_Mask;Mask;10;0;Create;True;0;0;0;False;0;False;-1;abc00000000004754645270228602497;abc00000000011112476408527520068;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.LerpOp;4;-448,-96;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;10;-592,352;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StaticSwitch;13;-640,800;Inherit;False;Property;_Rough_In_B;Rough_In_B;0;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;19;-368,800;Inherit;False;Property;_Opacity_A;Opacity_A;8;0;Create;True;0;0;0;False;0;False;0.5;0.282;0;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;18;-448,880;Inherit;False;Property;_Opacity_B;Opacity_B;9;0;Create;True;0;0;0;False;0;False;0.5;0.5;0;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;5;-224,-32;Inherit;False;Property;_UseAlbedo;UseAlbedo;1;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;15;-496,576;Inherit;False;Property;_A;A;4;0;Create;True;0;0;0;False;0;False;1.3;1.18;-1;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;16;-560,656;Inherit;False;Property;_B;B;5;0;Create;True;0;0;0;False;0;False;0;-0.03;-1;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;7;-208,224;Inherit;False;Property;_Desaturation;Desaturation;6;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.DesaturateOpNode;6;55.35669,121.712;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;8;-64,320;Inherit;False;Property;_Metallic;Metallic;7;0;Create;True;0;0;0;False;0;False;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;14;-208,464;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;17;-112,880;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;288,80;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;M_Glass;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Off;0;False;;0;False;;False;0;False;;0;False;;False;0;Transparent;0.5;True;True;0;False;Transparent;;Transparent;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;2;5;False;;10;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;17;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;4;0;2;0
WireConnection;4;1;3;0
WireConnection;4;2;12;2
WireConnection;10;0;9;0
WireConnection;10;1;11;0
WireConnection;13;1;12;2
WireConnection;13;0;12;3
WireConnection;5;1;4;0
WireConnection;5;0;10;0
WireConnection;6;0;5;0
WireConnection;6;1;7;0
WireConnection;14;0;15;0
WireConnection;14;1;16;0
WireConnection;14;2;13;0
WireConnection;17;0;19;0
WireConnection;17;1;18;0
WireConnection;17;2;12;2
WireConnection;0;0;6;0
WireConnection;0;3;8;0
WireConnection;0;4;14;0
WireConnection;0;9;17;0
ASEEND*/
//CHKSM=B655413D924F6BACF0B23BEA1E1DB4B2E3A8AC15