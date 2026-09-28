// Made with Amplify Shader Editor v1.9.6.3
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Nicrom/NHP/ASE/Skybox"
{
	Properties
	{
		[Header(Sky)]_SkyColor("Sky Color", Color) = (0.4449537,0.8723237,0.9528302,1)
		_HorizonColor("Horizon Color", Color) = (0.4511392,0.6372486,0.6981132,1)
		_ColorBlendStart("Color Blend Start", Range( 0 , 1)) = 0
		_ColorBlendEnd("Color Blend End", Range( 0 , 1)) = 0.28
		[Space]_GroundColor("Ground Color", Color) = (0.2659754,0.3415705,0.4056604,1)
		_GroundColorBlend("Ground Color Blend", Range( -1 , -0.001)) = -0.8
		[Header(Fog)][Toggle(_ENABLEFOG_ON)] _EnableFog("Enable Fog", Float) = 0
		_FogHeight("Fog Height", Range( 0 , 1)) = 0.04
		_FogOpacity("Fog Opacity", Range( 0 , 1)) = 0.7
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IgnoreProjector" = "True" "ForceNoShadowCasting" = "True" "IsEmissive" = "true"  }
		Cull Off
		ZWrite On
		ZTest LEqual
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma shader_feature _ENABLEFOG_ON
		#pragma surface surf Unlit keepalpha noshadow noambient novertexlights nolightmap  nodynlightmap nodirlightmap nofog nometa noforwardadd 
		struct Input
		{
			float3 worldPos;
		};

		uniform float4 _GroundColor;
		uniform float4 _HorizonColor;
		uniform float4 _SkyColor;
		uniform float _ColorBlendStart;
		uniform float _ColorBlendEnd;
		uniform float _GroundColorBlend;
		uniform half _FogHeight;
		uniform half _FogOpacity;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float3 ase_worldPos = i.worldPos;
			float3 normalizeResult31 = normalize( ase_worldPos );
			float3 break32 = normalizeResult31;
			float2 appendResult44 = (float2(( atan2( break32.x , break32.z ) / UNITY_PI ) , ( asin( break32.y ) / ( UNITY_PI / 2.0 ) )));
			float2 SkyboxUV46 = appendResult44;
			float smoothstepResult428 = smoothstep( _ColorBlendStart , _ColorBlendEnd , (SkyboxUV46).y);
			float4 lerpResult48 = lerp( _HorizonColor , _SkyColor , smoothstepResult428);
			float smoothstepResult440 = smoothstep( _GroundColorBlend , 0.0 , (SkyboxUV46).y);
			float4 lerpResult431 = lerp( _GroundColor , lerpResult48 , smoothstepResult440);
			float4 SkyColorGradient53 = lerpResult431;
			float3 normalizeResult256 = normalize( ase_worldPos );
			half FOG_MASK271 = saturate( pow( (0.0 + (abs( (normalizeResult256).y ) - 0.0) * (1.0 - 0.0) / (_FogHeight - 0.0)) , _FogOpacity ) );
			float4 lerpResult274 = lerp( unity_FogColor , SkyColorGradient53 , FOG_MASK271);
			#ifdef _ENABLEFOG_ON
				float4 staticSwitch276 = lerpResult274;
			#else
				float4 staticSwitch276 = SkyColorGradient53;
			#endif
			float4 Emission151 = staticSwitch276;
			o.Emission = Emission151.rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
}
/*ASEBEGIN
Version=19603
Node;AmplifyShaderEditor.CommentaryNode;45;-4360.1,513.6053;Inherit;False;1536.984;509.6083;SkyBox UVs;12;30;46;44;39;35;43;40;42;41;34;32;31;Skybox UVs;1,1,1,1;0;0
Node;AmplifyShaderEditor.WorldPosInputsNode;30;-4342.1,680.5439;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.NormalizeNode;31;-4135.556,680.6052;Inherit;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.BreakToComponentsNode;32;-3963.559,678.6052;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.RangedFloatNode;42;-3772.558,906.606;Inherit;False;Constant;_Float1;Float 1;5;0;Create;True;0;0;0;False;0;False;2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PiNode;41;-3802.559,831.606;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ATan2OpNode;34;-3570.562,620.6053;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ASinOpNode;35;-3574.562,724.6058;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;40;-3574.562,857.6061;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;21;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;254;-2554.445,514.4254;Inherit;False;1656.44;395.8624;;11;271;267;265;264;259;260;261;258;430;256;255;Fog;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;43;-3395.562,793.606;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;39;-3396.563,672.6053;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;255;-2527.445,581.4265;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DynamicAppendNode;44;-3225.567,722.6055;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;140;-3838.741,-774.6636;Inherit;False;1659.673;1029.733;;16;440;442;437;438;436;53;431;435;48;428;50;52;429;427;426;47;Sky Gradient;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;46;-3073.518,719.2786;Inherit;False;SkyboxUV;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.NormalizeNode;256;-2320.244,583.4787;Inherit;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SwizzleNode;430;-2144.422,577.5295;Inherit;False;FLOAT;1;1;2;3;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;47;-3816.29,-334.4704;Inherit;False;46;SkyboxUV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SwizzleNode;429;-3601.494,-332.6057;Inherit;False;FLOAT;1;1;2;3;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;436;-3552.053,-59.13613;Inherit;False;46;SkyboxUV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;260;-1996.446,755.4267;Half;False;Constant;_Float40;Float 40;55;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;261;-1983.446,582.4264;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;258;-2119.545,673.7974;Half;False;Property;_FogHeight;Fog Height;7;0;Create;True;0;0;0;False;0;False;0.04;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;426;-3724.569,-246.923;Inherit;False;Property;_ColorBlendStart;Color Blend Start;2;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;427;-3727.177,-155.9103;Inherit;False;Property;_ColorBlendEnd;Color Blend End;3;0;Create;True;0;0;0;False;0;False;0.28;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;50;-3419.263,-644.2352;Inherit;False;Property;_HorizonColor;Horizon Color;1;0;Create;True;0;0;0;False;0;False;0.4511392,0.6372486,0.6981132,1;0.4511392,0.6372486,0.6981132,1;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.TFHCRemapNode;264;-1782.599,615.0084;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;52;-3427.66,-453.7466;Inherit;False;Property;_SkyColor;Sky Color;0;0;Create;True;0;0;0;False;1;Header(Sky);False;0.4449537,0.8723237,0.9528302,1;0.4449537,0.8723237,0.9528302,1;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SmoothstepOpNode;428;-3360.178,-261.9106;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;442;-3336.421,113.63;Inherit;False;Constant;_Float0;Float 0;10;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;438;-3460.331,28.41117;Inherit;False;Property;_GroundColorBlend;Ground Color Blend;5;0;Create;True;0;0;0;False;0;False;-0.8;0;-1;-0.001;0;1;FLOAT;0
Node;AmplifyShaderEditor.SwizzleNode;437;-3337.255,-57.27155;Inherit;False;FLOAT;1;1;2;3;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;259;-1861.888,827.08;Half;False;Property;_FogOpacity;Fog Opacity;8;0;Create;True;0;0;0;False;0;False;0.7;0.01;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;435;-3138.436,-737.9973;Inherit;False;Property;_GroundColor;Ground Color;4;0;Create;True;0;0;0;False;1;Space;False;0.2659754,0.3415705,0.4056604,1;0,0,0,0;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.LerpOp;48;-3115.212,-473.0097;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.PowerNode;265;-1548.925,700.0084;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;440;-3095.939,13.4236;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;431;-2727.97,-477.0865;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;267;-1365.705,698.2076;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;53;-2524.173,-482.1663;Inherit;False;SkyColorGradient;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;252;-1915.659,-125.0482;Inherit;False;1025.459;386.0319;;7;151;276;424;274;278;423;277;Emission;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;271;-1186.931,692.4864;Half;False;FOG_MASK;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;277;-1817.425,161.2174;Inherit;False;271;FOG_MASK;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;423;-1869.315,75.17637;Inherit;False;53;SkyColorGradient;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.FogAndAmbientColorsNode;278;-1888.212,-2.30318;Inherit;False;unity_FogColor;0;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;424;-1655.518,-76.79312;Inherit;False;53;SkyColorGradient;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;274;-1581.149,56.76389;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StaticSwitch;276;-1390.916,-20.56253;Float;False;Property;_EnableFog;Enable Fog;6;0;Create;True;0;0;0;False;1;Header(Fog);False;0;0;0;True;;Toggle;2;Key0;Key1;Create;False;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;425;-506.9846,198.5529;Inherit;False;505.5728;505;;2;0;54;Master Node;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;151;-1128.406,-20.38455;Inherit;False;Emission;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;54;-467.9847,309.6675;Inherit;False;151;Emission;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-250.4122,268.5528;Float;False;True;-1;2;;0;0;Unlit;Nicrom/NHP/ASE/Skybox;False;False;False;False;True;True;True;True;True;True;True;True;False;False;True;True;False;False;False;False;False;Off;1;False;;3;False;;False;0;False;;0;False;;False;0;Opaque;0;True;False;0;False;Opaque;;Geometry;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;False;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;31;0;30;0
WireConnection;32;0;31;0
WireConnection;34;0;32;0
WireConnection;34;1;32;2
WireConnection;35;0;32;1
WireConnection;40;0;41;0
WireConnection;40;1;42;0
WireConnection;43;0;35;0
WireConnection;43;1;40;0
WireConnection;39;0;34;0
WireConnection;39;1;41;0
WireConnection;44;0;39;0
WireConnection;44;1;43;0
WireConnection;46;0;44;0
WireConnection;256;0;255;0
WireConnection;430;0;256;0
WireConnection;429;0;47;0
WireConnection;261;0;430;0
WireConnection;264;0;261;0
WireConnection;264;2;258;0
WireConnection;264;4;260;0
WireConnection;428;0;429;0
WireConnection;428;1;426;0
WireConnection;428;2;427;0
WireConnection;437;0;436;0
WireConnection;48;0;50;0
WireConnection;48;1;52;0
WireConnection;48;2;428;0
WireConnection;265;0;264;0
WireConnection;265;1;259;0
WireConnection;440;0;437;0
WireConnection;440;1;438;0
WireConnection;440;2;442;0
WireConnection;431;0;435;0
WireConnection;431;1;48;0
WireConnection;431;2;440;0
WireConnection;267;0;265;0
WireConnection;53;0;431;0
WireConnection;271;0;267;0
WireConnection;274;0;278;0
WireConnection;274;1;423;0
WireConnection;274;2;277;0
WireConnection;276;1;424;0
WireConnection;276;0;274;0
WireConnection;151;0;276;0
WireConnection;0;2;54;0
ASEEND*/
//CHKSM=D4FA1CBFC69AD546B52B8ED7402F5ED8551F67A3