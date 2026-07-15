// Made with Amplify Shader Editor v1.9.8.1
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "M_Wood"
{
	Properties
	{
		Material_Texture2D_1("Mask", 2D) = "white" {}
		Material_Texture2D_0("Normal", 2D) = "bump" {}
		_Invert("Invert", Float) = 1
		_Color1("Color 1", Color) = (0.682353,0.6784314,0.6392157,1)
		_Color2("Color 2", Color) = (0.6156863,0.572549,0.5333334,1)
		_Color3("Color 3", Color) = (0.764706,0.764706,0.764706,1)
		_UV("UV", Range( 1 , 10)) = 1
		_Roughness_1("Roughness_1", Range( 0 , 1)) = 0.4811782
		_Brightness("Brightness", Range( 0 , 10)) = 1
		_Roughness_2("Roughness_2", Range( 0 , 1)) = 0.2484763
		_Roughness_3("Roughness_3", Range( 0 , 1)) = 0.7696519
		_Desaturation("Desaturation", Range( 0 , 1)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#define ASE_VERSION 19801
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D Material_Texture2D_0;
		uniform float _UV;
		uniform float _Invert;
		uniform float4 _Color1;
		uniform float4 _Color2;
		uniform float4 _Color3;
		uniform sampler2D Material_Texture2D_1;
		uniform float _Brightness;
		uniform float _Desaturation;
		uniform float _Roughness_1;
		uniform float _Roughness_2;
		uniform float _Roughness_3;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 temp_output_22_0 = ( i.uv_texcoord * _UV );
			float3 break3 = UnpackNormal( tex2D( Material_Texture2D_0, temp_output_22_0 ) );
			float4 appendResult6 = (float4(break3.x , ( break3.y * _Invert ) , break3.z , 0.0));
			o.Normal = appendResult6.xyz;
			float4 tex2DNode2 = tex2D( Material_Texture2D_1, temp_output_22_0 );
			float saferPower16 = abs( ( ( tex2DNode2.g / 0.1 ) * 0.8 ) );
			float temp_output_17_0 = ( pow( saferPower16 , 0.1 ) * 0.15 );
			float4 lerpResult18 = lerp( _Color2 , _Color3 , temp_output_17_0);
			float temp_output_13_0 = ( min( tex2DNode2.r , 0.5 ) / 0.5 );
			float4 lerpResult10 = lerp( _Color1 , lerpResult18 , temp_output_13_0);
			float3 desaturateInitialColor36 = ( lerpResult10 * _Brightness ).rgb;
			float desaturateDot36 = dot( desaturateInitialColor36, float3( 0.299, 0.587, 0.114 ));
			float3 desaturateVar36 = lerp( desaturateInitialColor36, desaturateDot36.xxx, _Desaturation );
			o.Albedo = desaturateVar36;
			float lerpResult26 = lerp( _Roughness_2 , _Roughness_3 , temp_output_17_0);
			float lerpResult28 = lerp( _Roughness_1 , lerpResult26 , temp_output_13_0);
			o.Smoothness = lerpResult28;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "AmplifyShaderEditor.MaterialInspector"
}
/*ASEBEGIN
Version=19801
Node;AmplifyShaderEditor.TextureCoordinatesNode;21;-2652.202,119.9308;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;23;-2727.432,366.7662;Inherit;False;Property;_UV;UV;6;0;Create;True;0;0;0;False;0;False;1;1;1;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;22;-2416.432,295.7662;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;2;-2209.129,206.1258;Inherit;True;Property;Material_Texture2D_1;Mask;0;0;Create;False;0;0;0;False;0;False;-1;None;c3d1262ce1d92d64c920edd361c7a69e;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SimpleDivideOpNode;14;-1794.119,328.5723;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0.1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;15;-1588.195,348.0296;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.8;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;16;-1413.079,252.3648;Inherit;False;True;2;0;FLOAT;0;False;1;FLOAT;0.1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;20;-1565.9,-267.6602;Inherit;False;Property;_Color3;Color 3;5;0;Create;True;0;0;0;False;0;False;0.764706,0.764706,0.764706,1;0.235849,0.2167343,0.1969117,1;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.ColorNode;19;-1551.793,-566.7272;Inherit;False;Property;_Color2;Color 2;4;0;Create;True;0;0;0;False;0;False;0.6156863,0.572549,0.5333334,1;0.6415094,0.4445933,0.2269491,1;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;17;-1236.342,211.8283;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.15;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMinOpNode;12;-1406.593,-0.580795;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;-1243.07,1635.128;Inherit;True;Property;Material_Texture2D_0;Normal;1;0;Create;False;0;0;0;False;0;False;-1;None;abc00000000010516440641104343258;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.ColorNode;11;-1103.822,-654.5045;Inherit;False;Property;_Color1;Color 1;3;0;Create;True;0;0;0;False;0;False;0.682353,0.6784314,0.6392157,1;0.6886792,0.4986968,0.2956123,1;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.LerpOp;18;-1168.087,-333.9628;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;13;-1199.049,-60.57426;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;30;-879.7539,346.5241;Inherit;False;Property;_Roughness_2;Roughness_2;9;0;Create;True;0;0;0;False;0;False;0.2484763;0.2484763;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;3;-676.8884,1667.924;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.RangedFloatNode;31;-1006.054,701.3241;Inherit;False;Property;_Roughness_3;Roughness_3;10;0;Create;True;0;0;0;False;0;False;0.7696519;0.7696519;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;34;-487.9077,-134.1995;Inherit;False;Property;_Brightness;Brightness;8;0;Create;True;0;0;0;False;0;False;1;1;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;4;-724.5261,1466.269;Inherit;False;Property;_Invert;Invert;2;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;10;-692.8287,-294.3735;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;35;-177.3743,-94.08496;Inherit;False;Property;_Desaturation;Desaturation;11;0;Create;True;0;0;0;False;0;False;0;0.084;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;32;-259.9077,-336.1995;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;5;-493.7961,1470.632;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;-1;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;26;-546.7574,445.8693;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;29;-443.5335,40.54465;Inherit;False;Property;_Roughness_1;Roughness_1;7;0;Create;True;0;0;0;False;0;False;0.4811782;0.4811782;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;6;-329.789,1648.424;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.DesaturateOpNode;36;124.6257,-165.085;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;28;-99.36583,144.6276;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;628.5365,-92.62275;Float;False;True;-1;2;AmplifyShaderEditor.MaterialInspector;0;0;Standard;M_Wood;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;17;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;22;0;21;0
WireConnection;22;1;23;0
WireConnection;2;1;22;0
WireConnection;14;0;2;2
WireConnection;15;0;14;0
WireConnection;16;0;15;0
WireConnection;17;0;16;0
WireConnection;12;0;2;1
WireConnection;1;1;22;0
WireConnection;18;0;19;0
WireConnection;18;1;20;0
WireConnection;18;2;17;0
WireConnection;13;0;12;0
WireConnection;3;0;1;0
WireConnection;10;0;11;0
WireConnection;10;1;18;0
WireConnection;10;2;13;0
WireConnection;32;0;10;0
WireConnection;32;1;34;0
WireConnection;5;0;3;1
WireConnection;5;1;4;0
WireConnection;26;0;30;0
WireConnection;26;1;31;0
WireConnection;26;2;17;0
WireConnection;6;0;3;0
WireConnection;6;1;5;0
WireConnection;6;2;3;2
WireConnection;36;0;32;0
WireConnection;36;1;35;0
WireConnection;28;0;29;0
WireConnection;28;1;26;0
WireConnection;28;2;13;0
WireConnection;0;0;36;0
WireConnection;0;1;6;0
WireConnection;0;4;28;0
ASEEND*/
//CHKSM=5D773E915D6B79E8B97E78F496F3C5EA97EC580A