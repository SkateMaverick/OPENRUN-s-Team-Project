// Made with Amplify Shader Editor v1.9.9
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Nicrom/NHP/ASE/Stylised Flower With Stem"
{
	Properties
	{
		[HideInInspector] _EmissionColor("Emission Color", Color) = (1,1,1,1)
		[HideInInspector] _AlphaCutoff("Alpha Cutoff ", Range(0, 1)) = 0.5
		_FlowerColor1( "Flower Color 1", Color ) = ( 0.7843137, 0.454902, 0.1411765, 1 )
		_FlowerColor2( "Flower Color 2", Color ) = ( 0.8980392, 0.9529412, 1, 1 )
		_ColorBlendStart( "Color Blend Start", Range( 0, 1 ) ) = 0.1
		_ColorBlendEnd( "Color Blend End", Range( 0, 1 ) ) = 0.15
		_StemColor( "Stem Color", Color ) = ( 0.3960784, 0.5647059, 0.1019608, 1 )
		[NoScaleOffset] _MainTex( "Flower Texture", 2D ) = "white" {}
		[NoScaleOffset] _StemTexture( "Stem Texture", 2D ) = "white" {}
		_MBDefaultBending( "MB Default Bending", Float ) = 0
		_MBAmplitude( "MB Amplitude", Float ) = 1.5
		_MBAmplitudeOffset( "MB Amplitude Offset", Float ) = 2
		_MBFrequency( "MB Frequency", Float ) = 1.11
		_MBFrequencyOffset( "MB Frequency Offset", Float ) = 0
		_MBPhase( "MB Phase", Float ) = 1
		_MBWindDir( "MB Wind Dir", Range( 0, 360 ) ) = 0
		_MBWindDirOffset( "MB Wind Dir Offset", Range( 0, 180 ) ) = 20
		_MBWindDirBlend( "MB Wind Dir Blend", Range( 0, 1 ) ) = 0
		_MBMaxHeight( "MB Max Height", Float ) = 1
		[Toggle( _ENABLEHORIZONTALBENDING_ON )] _EnableHorizontalBending( "Enable Horizontal Bending ", Float ) = 1
		_DBHorizontalAmplitude( "DB Horizontal Amplitude", Float ) = 2
		_DBHorizontalFrequency( "DB Horizontal Frequency", Float ) = 1.16
		_DBHorizontalPhase( "DB Horizontal Phase", Float ) = 1
		_DBHorizontalMaxRadius( "DB Horizontal Max Radius", Float ) = 0.05
		[Toggle( _ENABLESLOPECORRECTION_ON )] _EnableSlopeCorrection( "Enable Slope Correction", Float ) = 1
		_SlopeCorrectionMagnitude( "Slope Correction Magnitude", Range( 0, 1 ) ) = 1
		_SlopeCorrectionOffset( "Slope Correction Offset", Range( 0, 1 ) ) = 0
		[NoScaleOffset] _NoiseTexture( "Noise Texture", 2D ) = "white" {}
		_NoiseTextureTilling( "Noise Tilling - Static (XY), Animated (ZW)", Vector ) = ( 1, 1, 1, 1 )
		_NoisePannerSpeed( "Noise Panner Speed", Vector ) = ( 0.05, 0.03, 0, 0 )
		[HideInInspector] _texcoord( "", 2D ) = "white" {}


		//_TransmissionShadow( "Transmission Shadow", Range( 0, 1 ) ) = 0.5
		//_TransStrength( "Trans Strength", Range( 0, 50 ) ) = 1
		//_TransNormal( "Trans Normal Distortion", Range( 0, 1 ) ) = 0.5
		//_TransScattering( "Trans Scattering", Range( 1, 50 ) ) = 2
		//_TransDirect( "Trans Direct", Range( 0, 1 ) ) = 0.9
		//_TransAmbient( "Trans Ambient", Range( 0, 1 ) ) = 0.1
		//_TransShadow( "Trans Shadow", Range( 0, 1 ) ) = 0.5
		//_TessPhongStrength( "Tess Phong Strength", Range( 0, 1 ) ) = 0.5
		//_TessValue( "Tess Max Tessellation", Range( 1, 32 ) ) = 16
		//_TessMin( "Tess Min Distance", Float ) = 10
		//_TessMax( "Tess Max Distance", Float ) = 25
		//_TessEdgeLength ( "Tess Edge length", Range( 2, 50 ) ) = 16
		//_TessMaxDisp( "Tess Max Displacement", Float ) = 25

		[HideInInspector][ToggleOff] _SpecularHighlights("Specular Highlights", Float) = 1
		[HideInInspector][ToggleOff] _EnvironmentReflections("Environment Reflections", Float) = 1
		[HideInInspector][ToggleOff] _ReceiveShadows("Receive Shadows", Float) = 1

		[HideInInspector] _QueueOffset("_QueueOffset", Float) = 0
        [HideInInspector] _QueueControl("_QueueControl", Float) = -1

        [HideInInspector][NoScaleOffset] unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset] unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset] unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}

		//[HideInInspector][ToggleUI] _AddPrecomputedVelocity("Add Precomputed Velocity", Float) = 1
	}

	SubShader
	{
		LOD 0

		

		

		Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" "Queue"="Geometry" "UniversalMaterialType"="Lit" "AlwaysRenderMotionVectors"="false" }

		Cull Off
		ZWrite On
		ZTest LEqual
		Offset 0 , 0
		AlphaToMask Off

		

		HLSLINCLUDE
		#pragma target 4.5
		#pragma prefer_hlslcc gles
		// ensure rendering platforms toggle list is visible

		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Filtering.hlsl"

		#ifndef ASE_TESS_FUNCS
		#define ASE_TESS_FUNCS
		float4 FixedTess( float tessValue )
		{
			return tessValue;
		}

		float CalcDistanceTessFactor (float4 vertex, float minDist, float maxDist, float tess, float4x4 o2w, float3 cameraPos )
		{
			float3 wpos = mul(o2w,vertex).xyz;
			float dist = distance (wpos, cameraPos);
			float f = clamp(1.0 - (dist - minDist) / (maxDist - minDist), 0.01, 1.0) * tess;
			return f;
		}

		float4 CalcTriEdgeTessFactors (float3 triVertexFactors)
		{
			float4 tess;
			tess.x = 0.5 * (triVertexFactors.y + triVertexFactors.z);
			tess.y = 0.5 * (triVertexFactors.x + triVertexFactors.z);
			tess.z = 0.5 * (triVertexFactors.x + triVertexFactors.y);
			tess.w = (triVertexFactors.x + triVertexFactors.y + triVertexFactors.z) / 3.0f;
			return tess;
		}

		float CalcEdgeTessFactor (float3 wpos0, float3 wpos1, float edgeLen, float3 cameraPos, float4 scParams )
		{
			float dist = distance (0.5 * (wpos0+wpos1), cameraPos);
			float len = distance(wpos0, wpos1);
			float f = max(len * scParams.y / (edgeLen * dist), 1.0);
			return f;
		}

		float DistanceFromPlane (float3 pos, float4 plane)
		{
			float d = dot (float4(pos,1.0f), plane);
			return d;
		}

		bool WorldViewFrustumCull (float3 wpos0, float3 wpos1, float3 wpos2, float cullEps, float4 planes[6] )
		{
			float4 planeTest;
			planeTest.x = (( DistanceFromPlane(wpos0, planes[0]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos1, planes[0]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos2, planes[0]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.y = (( DistanceFromPlane(wpos0, planes[1]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos1, planes[1]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos2, planes[1]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.z = (( DistanceFromPlane(wpos0, planes[2]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos1, planes[2]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos2, planes[2]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.w = (( DistanceFromPlane(wpos0, planes[3]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos1, planes[3]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos2, planes[3]) > -cullEps) ? 1.0f : 0.0f );
			return !all (planeTest);
		}

		float4 DistanceBasedTess( float4 v0, float4 v1, float4 v2, float tess, float minDist, float maxDist, float4x4 o2w, float3 cameraPos )
		{
			float3 f;
			f.x = CalcDistanceTessFactor (v0,minDist,maxDist,tess,o2w,cameraPos);
			f.y = CalcDistanceTessFactor (v1,minDist,maxDist,tess,o2w,cameraPos);
			f.z = CalcDistanceTessFactor (v2,minDist,maxDist,tess,o2w,cameraPos);

			return CalcTriEdgeTessFactors (f);
		}

		float4 EdgeLengthBasedTess( float4 v0, float4 v1, float4 v2, float edgeLength, float4x4 o2w, float3 cameraPos, float4 scParams )
		{
			float3 pos0 = mul(o2w,v0).xyz;
			float3 pos1 = mul(o2w,v1).xyz;
			float3 pos2 = mul(o2w,v2).xyz;
			float4 tess;
			tess.x = CalcEdgeTessFactor (pos1, pos2, edgeLength, cameraPos, scParams);
			tess.y = CalcEdgeTessFactor (pos2, pos0, edgeLength, cameraPos, scParams);
			tess.z = CalcEdgeTessFactor (pos0, pos1, edgeLength, cameraPos, scParams);
			tess.w = (tess.x + tess.y + tess.z) / 3.0f;
			return tess;
		}

		float4 EdgeLengthBasedTessCull( float4 v0, float4 v1, float4 v2, float edgeLength, float maxDisplacement, float4x4 o2w, float3 cameraPos, float4 scParams, float4 planes[6] )
		{
			float3 pos0 = mul(o2w,v0).xyz;
			float3 pos1 = mul(o2w,v1).xyz;
			float3 pos2 = mul(o2w,v2).xyz;
			float4 tess;

			if (WorldViewFrustumCull(pos0, pos1, pos2, maxDisplacement, planes))
			{
				tess = 0.0f;
			}
			else
			{
				tess.x = CalcEdgeTessFactor (pos1, pos2, edgeLength, cameraPos, scParams);
				tess.y = CalcEdgeTessFactor (pos2, pos0, edgeLength, cameraPos, scParams);
				tess.z = CalcEdgeTessFactor (pos0, pos1, edgeLength, cameraPos, scParams);
				tess.w = (tess.x + tess.y + tess.z) / 3.0f;
			}
			return tess;
		}
		#endif //ASE_TESS_FUNCS
		ENDHLSL

		
		Pass
		{
			
			Name "Forward"
			Tags { "LightMode"="UniversalForward" }

			Blend One Zero, One Zero
			ZWrite On
			ZTest LEqual
			Offset 0 , 0
			ColorMask RGBA

			

			HLSLPROGRAM

			#pragma multi_compile_local _ALPHATEST_ON
			#define _NORMAL_DROPOFF_TS 1
			#pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
			#pragma multi_compile_instancing
			#pragma instancing_options renderinglayer
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define ASE_VERSION 19900
			#define ASE_SRP_VERSION 170003


			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
			#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ EVALUATE_SH_MIXED EVALUATE_SH_VERTEX
			#pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
			#pragma multi_compile_fragment _ _REFLECTION_PROBE_BLENDING
			#pragma multi_compile_fragment _ _REFLECTION_PROBE_BOX_PROJECTION
			#pragma multi_compile_fragment _ _SHADOWS_SOFT _SHADOWS_SOFT_LOW _SHADOWS_SOFT_MEDIUM _SHADOWS_SOFT_HIGH
			#pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
			#pragma multi_compile _ _LIGHT_LAYERS
			#pragma multi_compile_fragment _ _LIGHT_COOKIES
			#pragma multi_compile _ _FORWARD_PLUS

			#pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
			#pragma multi_compile _ SHADOWS_SHADOWMASK
			#pragma multi_compile _ DIRLIGHTMAP_COMBINED
			#pragma multi_compile _ LIGHTMAP_ON
			#pragma multi_compile _ DYNAMICLIGHTMAP_ON
			#pragma multi_compile _ USE_LEGACY_LIGHTMAPS

			#pragma vertex vert
			#pragma fragment frag

			#if defined(_SPECULAR_SETUP) && defined(ASE_LIGHTING_SIMPLE)
				#define _SPECULAR_COLOR 1
			#endif

			#define SHADERPASS SHADERPASS_FORWARD

			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ProbeVolumeVariants.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DBuffer.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#if defined(LOD_FADE_CROSSFADE)
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/LODCrossFade.hlsl"
            #endif

			#if defined(UNITY_INSTANCING_ENABLED) && defined(_TERRAIN_INSTANCED_PERPIXEL_NORMAL)
				#define ENABLE_TERRAIN_PERPIXEL_NORMAL
			#endif

			#define ASE_NEEDS_TEXTURE_COORDINATES1
			#define ASE_NEEDS_VERT_TEXTURE_COORDINATES1
			#define ASE_NEEDS_VERT_POSITION
			#define ASE_NEEDS_TEXTURE_COORDINATES0
			#define ASE_NEEDS_FRAG_TEXTURE_COORDINATES0
			#pragma shader_feature_local _ENABLESLOPECORRECTION_ON
			#pragma shader_feature_local _ENABLEHORIZONTALBENDING_ON


			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE) && (SHADER_TARGET >= 45)
				#define ASE_SV_DEPTH SV_DepthLessEqual
				#define ASE_SV_POSITION_QUALIFIERS linear noperspective centroid
			#else
				#define ASE_SV_DEPTH SV_Depth
				#define ASE_SV_POSITION_QUALIFIERS
			#endif

			struct Attributes
			{
				float4 positionOS : POSITION;
				half3 normalOS : NORMAL;
				half4 tangentOS : TANGENT;
				float4 texcoord : TEXCOORD0;
				#if defined(LIGHTMAP_ON) || defined(ASE_NEEDS_TEXTURE_COORDINATES1)
					float4 texcoord1 : TEXCOORD1;
				#endif
				#if defined(DYNAMICLIGHTMAP_ON) || defined(ASE_NEEDS_TEXTURE_COORDINATES2)
					float4 texcoord2 : TEXCOORD2;
				#endif
				float4 ase_color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryings
			{
				ASE_SV_POSITION_QUALIFIERS float4 positionCS : SV_POSITION;
				float3 positionWS : TEXCOORD0;
				half3 normalWS : TEXCOORD1;
				half4 tangentWS : TEXCOORD2;
				float4 lightmapUVOrVertexSH : TEXCOORD3;
				#if defined(ASE_FOG) || defined(_ADDITIONAL_LIGHTS_VERTEX)
					half4 fogFactorAndVertexLight : TEXCOORD4;
				#endif
				#if defined(DYNAMICLIGHTMAP_ON)
					float2 dynamicLightmapUV : TEXCOORD5;
				#endif
				#if defined(USE_APV_PROBE_OCCLUSION)
					float4 probeOcclusion : TEXCOORD6;
				#endif
				float4 ase_texcoord7 : TEXCOORD7;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _StemColor;
			float4 _FlowerColor2;
			float4 _NoiseTextureTilling;
			float4 _FlowerColor1;
			float2 _NoisePannerSpeed;
			float _ColorBlendStart;
			float _SlopeCorrectionMagnitude;
			float _SlopeCorrectionOffset;
			float _DBHorizontalMaxRadius;
			float _DBHorizontalPhase;
			float _DBHorizontalFrequency;
			float _DBHorizontalAmplitude;
			float _MBMaxHeight;
			float _MBDefaultBending;
			float _MBPhase;
			float _MBFrequencyOffset;
			float _MBFrequency;
			float _MBAmplitudeOffset;
			float _MBAmplitude;
			float _MBWindDirOffset;
			float _MBWindDirBlend;
			float _ColorBlendEnd;
			float _MBWindDir;
			#ifdef ASE_TRANSMISSION
				float _TransmissionShadow;
			#endif
			#ifdef ASE_TRANSLUCENCY
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			#ifdef SCENEPICKINGPASS
				float4 _SelectionID;
			#endif

			#ifdef SCENESELECTIONPASS
				int _ObjectId;
				int _PassValue;
			#endif

			float MBGlobalWindDir;
			sampler2D _NoiseTexture;
			sampler2D _MainTex;
			sampler2D _StemTexture;


			float3 RotateAroundAxis( float3 center, float3 original, float3 u, float angle )
			{
				original -= center;
				float C = cos( angle );
				float S = sin( angle );
				float t = 1 - C;
				float m00 = t * u.x * u.x + C;
				float m01 = t * u.x * u.y - S * u.z;
				float m02 = t * u.x * u.z + S * u.y;
				float m10 = t * u.x * u.y + S * u.z;
				float m11 = t * u.y * u.y + C;
				float m12 = t * u.y * u.z - S * u.x;
				float m20 = t * u.x * u.z - S * u.y;
				float m21 = t * u.y * u.z + S * u.x;
				float m22 = t * u.z * u.z + C;
				float3x3 finalMatrix = float3x3( m00, m01, m02, m10, m11, m12, m20, m21, m22 );
				return mul( finalMatrix, original ) + center;
			}
			

			PackedVaryings VertexFunction( Attributes input  )
			{
				PackedVaryings output = (PackedVaryings)0;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				float lerpResult1811 = lerp( _MBWindDir , MBGlobalWindDir , _MBWindDirBlend);
				float MB_WindDirection870 = lerpResult1811;
				float MB_WindDirectionOffset1373 = _MBWindDirOffset;
				float2 texCoord1502 = input.texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float3 appendResult1503 = (float3(texCoord1502.x , 0.0 , texCoord1502.y));
				float3 LocalPivot1504 = -appendResult1503;
				float3 objToWorld11_g99 = mul( GetObjectToWorldMatrix(), float4( LocalPivot1504, 1 ) ).xyz;
				float2 appendResult10_g99 = (float2(objToWorld11_g99.x , objToWorld11_g99.z));
				float2 UVs27_g114 = appendResult10_g99;
				float4 temp_output_24_0_g114 = _NoiseTextureTilling;
				float2 AnimatedNoiseTilling29_g114 = (temp_output_24_0_g114).zw;
				float2 panner7_g114 = ( 0.1 * _Time.y * _NoisePannerSpeed + float2( 0,0 ));
				float4 AnimatedNoise1344 = tex2Dlod( _NoiseTexture, float4( ( ( UVs27_g114 * AnimatedNoiseTilling29_g114 ) + panner7_g114 ), 0, 0.0) );
				float temp_output_11_0_g130 = radians( ( ( MB_WindDirection870 + ( MB_WindDirectionOffset1373 *  (-1.0 + ( (AnimatedNoise1344).x - 0.0 ) * ( 1.0 - -1.0 ) / ( 1.0 - 0.0 ) ) ) ) * -1.0 ) );
				float3 appendResult14_g130 = (float3(cos( temp_output_11_0_g130 ) , 0.0 , sin( temp_output_11_0_g130 )));
				float3 worldToObj35_g130 = mul( GetWorldToObjectMatrix(), float4( appendResult14_g130, 1 ) ).xyz;
				float3 worldToObj36_g130 = mul( GetWorldToObjectMatrix(), float4( float3( 0,0,0 ), 1 ) ).xyz;
				float3 normalizeResult34_g130 = normalize( (( worldToObj35_g130 - worldToObj36_g130 )).xyz );
				float3 MB_RotationAxis1420 = normalizeResult34_g130;
				float3 RotationAxis56_g131 = MB_RotationAxis1420;
				float MB_Amplitude880 = _MBAmplitude;
				float MB_AmplitudeOffset1356 = _MBAmplitudeOffset;
				float2 StaticNoileTilling28_g114 = (temp_output_24_0_g114).xy;
				float4 StaticNoise1340 = tex2Dlod( _NoiseTexture, float4( ( UVs27_g114 * StaticNoileTilling28_g114 ), 0, 0.0) );
				float4 StaticWorldNoise31_g129 = StaticNoise1340;
				float3 objToWorld52_g129 = mul( GetObjectToWorldMatrix(), float4( float3( 0,0,0 ), 1 ) ).xyz;
				float MB_Frequency873 = _MBFrequency;
				float MB_FrequencyOffset1525 = _MBFrequencyOffset;
				float DB_PhaseShift928 = input.ase_color.a;
				float MB_Phase1360 = _MBPhase;
				float MB_DefaultBending877 = _MBDefaultBending;
				float MB_MaxHeight1335 = _MBMaxHeight;
				float MB_RotationAngle97 = radians( ( ( ( ( MB_Amplitude880 + ( MB_AmplitudeOffset1356 * (StaticWorldNoise31_g129).x ) ) * sin( ( ( ( objToWorld52_g129.x + objToWorld52_g129.z ) + ( ( ( _TimeParameters.x ) * ( MB_Frequency873 + ( MB_FrequencyOffset1525 * (StaticWorldNoise31_g129).x ) ) ) + ( ( 2.0 * PI ) * DB_PhaseShift928 ) ) ) * MB_Phase1360 ) ) ) + MB_DefaultBending877 ) * ( input.positionOS.xyz.y / MB_MaxHeight1335 ) ) );
				float RotationAngle54_g131 = MB_RotationAngle97;
				float3 PivotPoint60_g131 = LocalPivot1504;
				float3 break62_g131 = PivotPoint60_g131;
				float3 appendResult45_g131 = (float3(break62_g131.x , input.positionOS.xyz.y , break62_g131.z));
				float3 rotatedValue30_g131 = RotateAroundAxis( appendResult45_g131, input.positionOS.xyz, RotationAxis56_g131, RotationAngle54_g131 );
				float DB_HorizontalAmplitude1221 = _DBHorizontalAmplitude;
				float DB_HorizontalFrequency1219 = _DBHorizontalFrequency;
				float Frequency41_g115 = DB_HorizontalFrequency1219;
				float3 objToWorld53_g115 = mul( GetObjectToWorldMatrix(), float4( float3( 0,0,0 ), 1 ) ).xyz;
				float DB_HorizontalPhase1401 = _DBHorizontalPhase;
				float3 PivotPoint49_g115 = LocalPivot1504;
				float3 break52_g115 = PivotPoint49_g115;
				float3 appendResult20_g115 = (float3(break52_g115.x , input.positionOS.xyz.y , break52_g115.z));
				float DB_HorizontalMaxRadius1403 = _DBHorizontalMaxRadius;
				float3 rotatedValue33_g115 = RotateAroundAxis( PivotPoint49_g115, input.positionOS.xyz, float3( 0, 1, 0 ), radians( ( ( DB_HorizontalAmplitude1221 * sin( ( ( ( ( _TimeParameters.x ) * Frequency41_g115 ) - ( ( 2.0 * PI ) * ( 1.0 - DB_PhaseShift928 ) ) ) + ( ( ( objToWorld53_g115.x + objToWorld53_g115.z ) + ( ( _TimeParameters.x ) * Frequency41_g115 ) ) * DB_HorizontalPhase1401 ) ) ) ) * ( distance( input.positionOS.xyz , appendResult20_g115 ) / DB_HorizontalMaxRadius1403 ) ) ) );
				#ifdef _ENABLEHORIZONTALBENDING_ON
				float3 staticSwitch1214 = ( ( rotatedValue33_g115 - input.positionOS.xyz ) * 1.0 );
				#else
				float3 staticSwitch1214 = float3( 0, 0, 0 );
				#endif
				float3 DB_VertexOffset769 = staticSwitch1214;
				float3 rotatedValue34_g131 = RotateAroundAxis( PivotPoint60_g131, ( rotatedValue30_g131 + DB_VertexOffset769 ), RotationAxis56_g131, RotationAngle54_g131 );
				float3 temp_output_1533_0 = ( ( rotatedValue34_g131 - input.positionOS.xyz ) * step( 0.01 , input.positionOS.xyz.y ) );
				float3 MainBending89_g158 = temp_output_1533_0;
				float3 appendResult15_g158 = (float3(0.0 , 1.0 , 0.0));
				float3 objToWorld98_g158 = mul( GetObjectToWorldMatrix(), float4( appendResult15_g158, 1 ) ).xyz;
				float3 objToWorld102_g158 = mul( GetObjectToWorldMatrix(), float4( float3( 0,0,0 ), 1 ) ).xyz;
				float3 break20_g158 = ( objToWorld98_g158 - objToWorld102_g158 );
				float3 appendResult24_g158 = (float3(-break20_g158.z , 0.0 , break20_g158.x));
				float3 appendResult3_g158 = (float3(0.0 , 1.0 , 0.0));
				float3 objToWorld100_g158 = mul( GetObjectToWorldMatrix(), float4( appendResult3_g158, 1 ) ).xyz;
				float3 objToWorld106_g158 = mul( GetObjectToWorldMatrix(), float4( float3( 0,0,0 ), 1 ) ).xyz;
				float3 temp_output_107_0_g158 = ( objToWorld100_g158 - objToWorld106_g158 );
				float3 break108_g158 = temp_output_107_0_g158;
				float3 lerpResult84_g158 = lerp( float3( 0, 1, 0 ) , temp_output_107_0_g158 , step( 0.001 , ( abs( break108_g158.x ) + abs( break108_g158.z ) ) ));
				float3 normalizeResult7_g158 = normalize( lerpResult84_g158 );
				float dotResult9_g158 = dot( normalizeResult7_g158 , float3( 0, 1, 0 ) );
				float temp_output_12_0_g158 = acos( dotResult9_g158 );
				float NaNPrevention21_g158 = step( 0.01 , abs( ( temp_output_12_0_g158 * ( 180.0 / PI ) ) ) );
				float3 lerpResult26_g158 = lerp( float3( 1, 0, 0 ) , appendResult24_g158 , NaNPrevention21_g158);
				float3 worldToObj99_g158 = mul( GetWorldToObjectMatrix(), float4( lerpResult26_g158, 1 ) ).xyz;
				float3 worldToObj105_g158 = mul( GetWorldToObjectMatrix(), float4( float3( 0,0,0 ), 1 ) ).xyz;
				float3 normalizeResult49_g158 = normalize( ( worldToObj99_g158 - worldToObj105_g158 ) );
				float3 RotationAxis30_g158 = normalizeResult49_g158;
				float SlopeCorrectionOffset1805 = _SlopeCorrectionOffset;
				float SlopeCorrectionMagnitude1775 = _SlopeCorrectionMagnitude;
				float RotationAngle29_g158 = ( saturate( (  (0.0 + ( (StaticNoise1340).x - 0.0 ) * ( SlopeCorrectionOffset1805 - 0.0 ) / ( 1.0 - 0.0 ) ) + SlopeCorrectionMagnitude1775 ) ) * temp_output_12_0_g158 );
				float3 rotatedValue35_g158 = RotateAroundAxis( LocalPivot1504, ( input.positionOS.xyz + MainBending89_g158 ), RotationAxis30_g158, RotationAngle29_g158 );
				float3 lerpResult52_g158 = lerp( MainBending89_g158 , ( rotatedValue35_g158 - input.positionOS.xyz ) , NaNPrevention21_g158);
				#ifdef _ENABLESLOPECORRECTION_ON
				float3 staticSwitch1781 = lerpResult52_g158;
				#else
				float3 staticSwitch1781 = temp_output_1533_0;
				#endif
				float3 LocalVertexOffset1045 = staticSwitch1781;
				
				output.ase_texcoord7.xy = input.texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord7.zw = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = LocalVertexOffset1045;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					input.positionOS.xyz = vertexValue;
				#else
					input.positionOS.xyz += vertexValue;
				#endif
				input.normalOS = input.normalOS;
				input.tangentOS = input.tangentOS;

				VertexPositionInputs vertexInput = GetVertexPositionInputs( input.positionOS.xyz );
				VertexNormalInputs normalInput = GetVertexNormalInputs( input.normalOS, input.tangentOS );

				OUTPUT_LIGHTMAP_UV(input.texcoord1, unity_LightmapST, output.lightmapUVOrVertexSH.xy);
				#if defined(DYNAMICLIGHTMAP_ON)
					output.dynamicLightmapUV.xy = input.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
				#endif
				OUTPUT_SH4(vertexInput.positionWS, normalInput.normalWS.xyz, GetWorldSpaceNormalizeViewDir(vertexInput.positionWS), output.lightmapUVOrVertexSH.xyz, output.probeOcclusion);

				#if defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)
					output.lightmapUVOrVertexSH.zw = input.texcoord.xy;
					output.lightmapUVOrVertexSH.xy = input.texcoord.xy * unity_LightmapST.xy + unity_LightmapST.zw;
				#endif

				#if defined(ASE_FOG) || defined(_ADDITIONAL_LIGHTS_VERTEX)
					output.fogFactorAndVertexLight = 0;
					#if defined(ASE_FOG) && !defined(_FOG_FRAGMENT)
						output.fogFactorAndVertexLight.x = ComputeFogFactor(vertexInput.positionCS.z);
					#endif
					#ifdef _ADDITIONAL_LIGHTS_VERTEX
						half3 vertexLight = VertexLighting( vertexInput.positionWS, normalInput.normalWS );
						output.fogFactorAndVertexLight.yzw = vertexLight;
					#endif
				#endif

				output.positionCS = vertexInput.positionCS;
				output.positionWS = vertexInput.positionWS;
				output.normalWS = normalInput.normalWS;
				output.tangentWS = float4( normalInput.tangentWS, ( input.tangentOS.w > 0.0 ? 1.0 : -1.0 ) * GetOddNegativeScale() );
				return output;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 positionOS : INTERNALTESSPOS;
				half3 normalOS : NORMAL;
				half4 tangentOS : TANGENT;
				float4 texcoord : TEXCOORD0;
				#if defined(LIGHTMAP_ON) || defined(ASE_NEEDS_TEXTURE_COORDINATES1)
					float4 texcoord1 : TEXCOORD1;
				#endif
				#if defined(DYNAMICLIGHTMAP_ON) || defined(ASE_NEEDS_TEXTURE_COORDINATES2)
					float4 texcoord2 : TEXCOORD2;
				#endif
				float4 ase_color : COLOR;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( Attributes input )
			{
				VertexControl output;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				output.positionOS = input.positionOS;
				output.normalOS = input.normalOS;
				output.tangentOS = input.tangentOS;
				output.texcoord = input.texcoord;
				#if defined(LIGHTMAP_ON) || defined(ASE_NEEDS_TEXTURE_COORDINATES1)
					output.texcoord1 = input.texcoord1;
				#endif
				#if defined(DYNAMICLIGHTMAP_ON) || defined(ASE_NEEDS_TEXTURE_COORDINATES2)
					output.texcoord2 = input.texcoord2;
				#endif
				output.ase_color = input.ase_color;
				return output;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> input)
			{
				TessellationFactors output;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				output.edge[0] = tf.x; output.edge[1] = tf.y; output.edge[2] = tf.z; output.inside = tf.w;
				return output;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			PackedVaryings DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				Attributes output = (Attributes) 0;
				output.positionOS = patch[0].positionOS * bary.x + patch[1].positionOS * bary.y + patch[2].positionOS * bary.z;
				output.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				output.tangentOS = patch[0].tangentOS * bary.x + patch[1].tangentOS * bary.y + patch[2].tangentOS * bary.z;
				output.texcoord = patch[0].texcoord * bary.x + patch[1].texcoord * bary.y + patch[2].texcoord * bary.z;
				#if defined(LIGHTMAP_ON) || defined(ASE_NEEDS_TEXTURE_COORDINATES1)
					output.texcoord1 = patch[0].texcoord1 * bary.x + patch[1].texcoord1 * bary.y + patch[2].texcoord1 * bary.z;
				#endif
				#if defined(DYNAMICLIGHTMAP_ON) || defined(ASE_NEEDS_TEXTURE_COORDINATES2)
					output.texcoord2 = patch[0].texcoord2 * bary.x + patch[1].texcoord2 * bary.y + patch[2].texcoord2 * bary.z;
				#endif
				output.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = output.positionOS.xyz - patch[i].normalOS * (dot(output.positionOS.xyz, patch[i].normalOS) - dot(patch[i].positionOS.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				output.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * output.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], output);
				return VertexFunction(output);
			}
			#else
			PackedVaryings vert ( Attributes input )
			{
				return VertexFunction( input );
			}
			#endif

			half4 frag ( PackedVaryings input
						#if defined( ASE_DEPTH_WRITE_ON )
						,out float outputDepth : ASE_SV_DEPTH
						#endif
						#ifdef _WRITE_RENDERING_LAYERS
						, out float4 outRenderingLayers : SV_Target1
						#endif
						 ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

				#if defined(LOD_FADE_CROSSFADE)
					LODFadeCrossFade( input.positionCS );
				#endif

				#if defined(MAIN_LIGHT_CALCULATE_SHADOWS)
					float4 shadowCoord = TransformWorldToShadowCoord( input.positionWS );
				#else
					float4 shadowCoord = float4(0, 0, 0, 0);
				#endif

				// @diogo: mikktspace compliant
				float renormFactor = 1.0 / max( FLT_MIN, length( input.normalWS ) );

				float3 PositionWS = input.positionWS;
				float3 PositionRWS = GetCameraRelativePositionWS( PositionWS );
				float3 ViewDirWS = GetWorldSpaceNormalizeViewDir( PositionWS );
				float4 ShadowCoord = shadowCoord;
				float4 ScreenPosNorm = float4( GetNormalizedScreenSpaceUV( input.positionCS ), input.positionCS.zw );
				float4 ClipPos = ComputeClipSpacePosition( ScreenPosNorm.xy, input.positionCS.z ) * input.positionCS.w;
				float4 ScreenPos = ComputeScreenPos( ClipPos );
				float3 TangentWS = input.tangentWS.xyz * renormFactor;
				float3 BitangentWS = cross( input.normalWS, input.tangentWS.xyz ) * input.tangentWS.w * renormFactor;
				float3 NormalWS = input.normalWS * renormFactor;

				#if defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)
					float2 sampleCoords = (input.lightmapUVOrVertexSH.zw / _TerrainHeightmapRecipSize.zw + 0.5f) * _TerrainHeightmapRecipSize.xy;
					NormalWS = TransformObjectToWorldNormal(normalize(SAMPLE_TEXTURE2D(_TerrainNormalmapTexture, sampler_TerrainNormalmapTexture, sampleCoords).rgb * 2 - 1));
					TangentWS = -cross(GetObjectToWorldMatrix()._13_23_33, NormalWS);
					BitangentWS = cross(NormalWS, -TangentWS);
				#endif

				float2 uv_MainTex1481 = input.ase_texcoord7.xy;
				float4 tex2DNode1481 = tex2D( _MainTex, uv_MainTex1481 );
				float2 uv_StemTexture1482 = input.ase_texcoord7.xy;
				float4 tex2DNode1482 = tex2D( _StemTexture, uv_StemTexture1482 );
				float TextureMask1458 = step( 1.5 , input.ase_texcoord7.xy.x );
				float4 lerpResult1486 = lerp( tex2DNode1481 , tex2DNode1482 , TextureMask1458);
				float4 TextureColor1487 = lerpResult1486;
				float DistanceToCenter1466 = distance( float2( 0.5,0.5 ) , input.ase_texcoord7.xy );
				float ColorBlendStart1506 = _ColorBlendStart;
				float ColorBlendEnd1812 = _ColorBlendEnd;
				float4 lerpResult1480 = lerp( _FlowerColor1 , _FlowerColor2 , ( saturate( ( ( DistanceToCenter1466 - ColorBlendStart1506 ) / ColorBlendEnd1812 ) ) * step( ColorBlendStart1506 , DistanceToCenter1466 ) ));
				float4 lerpResult1485 = lerp( lerpResult1480 , _StemColor , TextureMask1458);
				float4 Color1488 = lerpResult1485;
				float4 Albedo1493 = ( TextureColor1487 * Color1488 );
				
				float lerpResult1492 = lerp( tex2DNode1481.a , tex2DNode1482.a , TextureMask1458);
				float Opacity1494 = lerpResult1492;
				

				float3 BaseColor = Albedo1493.rgb;
				float3 Normal = float3(0, 0, 1);
				float3 Specular = 0.5;
				float Metallic = 0.0;
				float Smoothness = 0.0;
				float Occlusion = 1;
				float3 Emission = 0;
				float Alpha = Opacity1494;
				float AlphaClipThreshold = 0.5;
				float AlphaClipThresholdShadow = 0.5;
				float3 BakedGI = 0;
				float3 RefractionColor = 1;
				float RefractionIndex = 1;
				float3 Transmission = 1;
				float3 Translucency = 1;

				#if defined( ASE_DEPTH_WRITE_ON )
					float DeviceDepth = ClipPos.z;
				#endif

				#ifdef _CLEARCOAT
					float CoatMask = 0;
					float CoatSmoothness = 0;
				#endif

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				#if defined(MAIN_LIGHT_CALCULATE_SHADOWS) && defined(ASE_CHANGES_WORLD_POS)
					ShadowCoord = TransformWorldToShadowCoord( PositionWS );
				#endif

				InputData inputData = (InputData)0;
				inputData.positionWS = PositionWS;
				inputData.positionCS = float4( input.positionCS.xy, ClipPos.zw / ClipPos.w );
				inputData.normalizedScreenSpaceUV = ScreenPosNorm.xy;
				inputData.viewDirectionWS = ViewDirWS;
				inputData.shadowCoord = ShadowCoord;

				#ifdef _NORMALMAP
						#if _NORMAL_DROPOFF_TS
							inputData.normalWS = TransformTangentToWorld(Normal, half3x3(TangentWS, BitangentWS, NormalWS));
						#elif _NORMAL_DROPOFF_OS
							inputData.normalWS = TransformObjectToWorldNormal(Normal);
						#elif _NORMAL_DROPOFF_WS
							inputData.normalWS = Normal;
						#endif
					inputData.normalWS = NormalizeNormalPerPixel(inputData.normalWS);
				#else
					inputData.normalWS = NormalWS;
				#endif

				#ifdef ASE_FOG
					inputData.fogCoord = InitializeInputDataFog(float4(inputData.positionWS, 1.0), input.fogFactorAndVertexLight.x);
				#endif
				#ifdef _ADDITIONAL_LIGHTS_VERTEX
					inputData.vertexLighting = input.fogFactorAndVertexLight.yzw;
				#endif

				#if defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)
					float3 SH = SampleSH(inputData.normalWS.xyz);
				#else
					float3 SH = input.lightmapUVOrVertexSH.xyz;
				#endif

				#if defined(DYNAMICLIGHTMAP_ON)
					inputData.bakedGI = SAMPLE_GI(input.lightmapUVOrVertexSH.xy, input.dynamicLightmapUV.xy, SH, inputData.normalWS);
					inputData.shadowMask = SAMPLE_SHADOWMASK(input.lightmapUVOrVertexSH.xy);
				#elif !defined(LIGHTMAP_ON) && (defined(PROBE_VOLUMES_L1) || defined(PROBE_VOLUMES_L2))
					inputData.bakedGI = SAMPLE_GI( SH, GetAbsolutePositionWS(inputData.positionWS),
						inputData.normalWS,
						inputData.viewDirectionWS,
						input.positionCS.xy,
						input.probeOcclusion,
						inputData.shadowMask );
				#else
					inputData.bakedGI = SAMPLE_GI(input.lightmapUVOrVertexSH.xy, SH, inputData.normalWS);
					inputData.shadowMask = SAMPLE_SHADOWMASK(input.lightmapUVOrVertexSH.xy);
				#endif

				#ifdef ASE_BAKEDGI
					inputData.bakedGI = BakedGI;
				#endif

				#if defined(DEBUG_DISPLAY)
					#if defined(DYNAMICLIGHTMAP_ON)
						inputData.dynamicLightmapUV = input.dynamicLightmapUV.xy;
					#endif
					#if defined(LIGHTMAP_ON)
						inputData.staticLightmapUV = input.lightmapUVOrVertexSH.xy;
					#else
						inputData.vertexSH = SH;
					#endif
					#if defined(USE_APV_PROBE_OCCLUSION)
						inputData.probeOcclusion = input.probeOcclusion;
					#endif
				#endif

				SurfaceData surfaceData;
				surfaceData.albedo              = BaseColor;
				surfaceData.metallic            = saturate(Metallic);
				surfaceData.specular            = Specular;
				surfaceData.smoothness          = saturate(Smoothness),
				surfaceData.occlusion           = Occlusion,
				surfaceData.emission            = Emission,
				surfaceData.alpha               = saturate(Alpha);
				surfaceData.normalTS            = Normal;
				surfaceData.clearCoatMask       = 0;
				surfaceData.clearCoatSmoothness = 1;

				#ifdef _CLEARCOAT
					surfaceData.clearCoatMask       = saturate(CoatMask);
					surfaceData.clearCoatSmoothness = saturate(CoatSmoothness);
				#endif

				#ifdef _DBUFFER
					ApplyDecalToSurfaceData(input.positionCS, surfaceData, inputData);
				#endif

				#ifdef ASE_LIGHTING_SIMPLE
					half4 color = UniversalFragmentBlinnPhong( inputData, surfaceData);
				#else
					half4 color = UniversalFragmentPBR( inputData, surfaceData);
				#endif

				#ifdef ASE_TRANSMISSION
				{
					float shadow = _TransmissionShadow;

					#define SUM_LIGHT_TRANSMISSION(Light)\
						float3 atten = Light.color * Light.distanceAttenuation;\
						atten = lerp( atten, atten * Light.shadowAttenuation, shadow );\
						half3 transmission = max( 0, -dot( inputData.normalWS, Light.direction ) ) * atten * Transmission;\
						color.rgb += BaseColor * transmission;

					SUM_LIGHT_TRANSMISSION( GetMainLight( inputData.shadowCoord ) );

					#if defined(_ADDITIONAL_LIGHTS)
						uint meshRenderingLayers = GetMeshRenderingLayer();
						uint pixelLightCount = GetAdditionalLightsCount();
						#if USE_FORWARD_PLUS
							[loop] for (uint lightIndex = 0; lightIndex < min(URP_FP_DIRECTIONAL_LIGHTS_COUNT, MAX_VISIBLE_LIGHTS); lightIndex++)
							{
								FORWARD_PLUS_SUBTRACTIVE_LIGHT_CHECK

								Light light = GetAdditionalLight(lightIndex, inputData.positionWS, inputData.shadowMask);
								#ifdef _LIGHT_LAYERS
								if (IsMatchingLightLayer(light.layerMask, meshRenderingLayers))
								#endif
								{
									SUM_LIGHT_TRANSMISSION( light );
								}
							}
						#endif
						LIGHT_LOOP_BEGIN( pixelLightCount )
							Light light = GetAdditionalLight(lightIndex, inputData.positionWS, inputData.shadowMask);
							#ifdef _LIGHT_LAYERS
							if (IsMatchingLightLayer(light.layerMask, meshRenderingLayers))
							#endif
							{
								SUM_LIGHT_TRANSMISSION( light );
							}
						LIGHT_LOOP_END
					#endif
				}
				#endif

				#ifdef ASE_TRANSLUCENCY
				{
					float shadow = _TransShadow;
					float normal = _TransNormal;
					float scattering = _TransScattering;
					float direct = _TransDirect;
					float ambient = _TransAmbient;
					float strength = _TransStrength;

					#define SUM_LIGHT_TRANSLUCENCY(Light)\
						float3 atten = Light.color * Light.distanceAttenuation;\
						atten = lerp( atten, atten * Light.shadowAttenuation, shadow );\
						half3 lightDir = Light.direction + inputData.normalWS * normal;\
						half VdotL = pow( saturate( dot( inputData.viewDirectionWS, -lightDir ) ), scattering );\
						half3 translucency = atten * ( VdotL * direct + inputData.bakedGI * ambient ) * Translucency;\
						color.rgb += BaseColor * translucency * strength;

					SUM_LIGHT_TRANSLUCENCY( GetMainLight( inputData.shadowCoord ) );

					#if defined(_ADDITIONAL_LIGHTS)
						uint meshRenderingLayers = GetMeshRenderingLayer();
						uint pixelLightCount = GetAdditionalLightsCount();
						#if USE_FORWARD_PLUS
							[loop] for (uint lightIndex = 0; lightIndex < min(URP_FP_DIRECTIONAL_LIGHTS_COUNT, MAX_VISIBLE_LIGHTS); lightIndex++)
							{
								FORWARD_PLUS_SUBTRACTIVE_LIGHT_CHECK

								Light light = GetAdditionalLight(lightIndex, inputData.positionWS, inputData.shadowMask);
								#ifdef _LIGHT_LAYERS
								if (IsMatchingLightLayer(light.layerMask, meshRenderingLayers))
								#endif
								{
									SUM_LIGHT_TRANSLUCENCY( light );
								}
							}
						#endif
						LIGHT_LOOP_BEGIN( pixelLightCount )
							Light light = GetAdditionalLight(lightIndex, inputData.positionWS, inputData.shadowMask);
							#ifdef _LIGHT_LAYERS
							if (IsMatchingLightLayer(light.layerMask, meshRenderingLayers))
							#endif
							{
								SUM_LIGHT_TRANSLUCENCY( light );
							}
						LIGHT_LOOP_END
					#endif
				}
				#endif

				#ifdef ASE_REFRACTION
					float4 projScreenPos = ScreenPos / ScreenPos.w;
					float3 refractionOffset = ( RefractionIndex - 1.0 ) * mul( UNITY_MATRIX_V, float4( NormalWS,0 ) ).xyz * ( 1.0 - dot( NormalWS, ViewDirWS ) );
					projScreenPos.xy += refractionOffset.xy;
					float3 refraction = SHADERGRAPH_SAMPLE_SCENE_COLOR( projScreenPos.xy ) * RefractionColor;
					color.rgb = lerp( refraction, color.rgb, color.a );
					color.a = 1;
				#endif

				#ifdef ASE_FINAL_COLOR_ALPHA_MULTIPLY
					color.rgb *= color.a;
				#endif

				#ifdef ASE_FOG
					#ifdef TERRAIN_SPLAT_ADDPASS
						color.rgb = MixFogColor(color.rgb, half3(0,0,0), inputData.fogCoord);
					#else
						color.rgb = MixFog(color.rgb, inputData.fogCoord);
					#endif
				#endif

				#if defined( ASE_DEPTH_WRITE_ON )
					outputDepth = DeviceDepth;
				#endif

				#ifdef _WRITE_RENDERING_LAYERS
					uint renderingLayers = GetMeshRenderingLayer();
					outRenderingLayers = float4( EncodeMeshRenderingLayer( renderingLayers ), 0, 0, 0 );
				#endif

				return color;
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "ShadowCaster"
			Tags { "LightMode"="ShadowCaster" }

			ZWrite On
			ZTest LEqual
			AlphaToMask Off
			ColorMask 0

			HLSLPROGRAM

			#pragma multi_compile_local _ALPHATEST_ON
			#define _NORMAL_DROPOFF_TS 1
			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#define ASE_FOG 1
			#define ASE_VERSION 19900
			#define ASE_SRP_VERSION 170003


			#pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW

			#pragma vertex vert
			#pragma fragment frag

			#if defined(_SPECULAR_SETUP) && defined(ASE_LIGHTING_SIMPLE)
				#define _SPECULAR_COLOR 1
			#endif

			#define SHADERPASS SHADERPASS_SHADOWCASTER

			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#if defined(LOD_FADE_CROSSFADE)
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/LODCrossFade.hlsl"
            #endif

			#define ASE_NEEDS_TEXTURE_COORDINATES1
			#define ASE_NEEDS_VERT_POSITION
			#define ASE_NEEDS_TEXTURE_COORDINATES0
			#define ASE_NEEDS_FRAG_TEXTURE_COORDINATES0
			#pragma shader_feature_local _ENABLESLOPECORRECTION_ON
			#pragma shader_feature_local _ENABLEHORIZONTALBENDING_ON


			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE) && (SHADER_TARGET >= 45)
				#define ASE_SV_DEPTH SV_DepthLessEqual
				#define ASE_SV_POSITION_QUALIFIERS linear noperspective centroid
			#else
				#define ASE_SV_DEPTH SV_Depth
				#define ASE_SV_POSITION_QUALIFIERS
			#endif

			struct Attributes
			{
				float4 positionOS : POSITION;
				half3 normalOS : NORMAL;
				half4 tangentOS : TANGENT;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryings
			{
				ASE_SV_POSITION_QUALIFIERS float4 positionCS : SV_POSITION;
				float3 positionWS : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _StemColor;
			float4 _FlowerColor2;
			float4 _NoiseTextureTilling;
			float4 _FlowerColor1;
			float2 _NoisePannerSpeed;
			float _ColorBlendStart;
			float _SlopeCorrectionMagnitude;
			float _SlopeCorrectionOffset;
			float _DBHorizontalMaxRadius;
			float _DBHorizontalPhase;
			float _DBHorizontalFrequency;
			float _DBHorizontalAmplitude;
			float _MBMaxHeight;
			float _MBDefaultBending;
			float _MBPhase;
			float _MBFrequencyOffset;
			float _MBFrequency;
			float _MBAmplitudeOffset;
			float _MBAmplitude;
			float _MBWindDirOffset;
			float _MBWindDirBlend;
			float _ColorBlendEnd;
			float _MBWindDir;
			#ifdef ASE_TRANSMISSION
				float _TransmissionShadow;
			#endif
			#ifdef ASE_TRANSLUCENCY
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			#ifdef SCENEPICKINGPASS
				float4 _SelectionID;
			#endif

			#ifdef SCENESELECTIONPASS
				int _ObjectId;
				int _PassValue;
			#endif

			float MBGlobalWindDir;
			sampler2D _NoiseTexture;
			sampler2D _MainTex;
			sampler2D _StemTexture;


			float3 _LightDirection;
			float3 _LightPosition;

			float3 RotateAroundAxis( float3 center, float3 original, float3 u, float angle )
			{
				original -= center;
				float C = cos( angle );
				float S = sin( angle );
				float t = 1 - C;
				float m00 = t * u.x * u.x + C;
				float m01 = t * u.x * u.y - S * u.z;
				float m02 = t * u.x * u.z + S * u.y;
				float m10 = t * u.x * u.y + S * u.z;
				float m11 = t * u.y * u.y + C;
				float m12 = t * u.y * u.z - S * u.x;
				float m20 = t * u.x * u.z - S * u.y;
				float m21 = t * u.y * u.z + S * u.x;
				float m22 = t * u.z * u.z + C;
				float3x3 finalMatrix = float3x3( m00, m01, m02, m10, m11, m12, m20, m21, m22 );
				return mul( finalMatrix, original ) + center;
			}
			

			PackedVaryings VertexFunction( Attributes input )
			{
				PackedVaryings output;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( output );

				float lerpResult1811 = lerp( _MBWindDir , MBGlobalWindDir , _MBWindDirBlend);
				float MB_WindDirection870 = lerpResult1811;
				float MB_WindDirectionOffset1373 = _MBWindDirOffset;
				float2 texCoord1502 = input.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float3 appendResult1503 = (float3(texCoord1502.x , 0.0 , texCoord1502.y));
				float3 LocalPivot1504 = -appendResult1503;
				float3 objToWorld11_g99 = mul( GetObjectToWorldMatrix(), float4( LocalPivot1504, 1 ) ).xyz;
				float2 appendResult10_g99 = (float2(objToWorld11_g99.x , objToWorld11_g99.z));
				float2 UVs27_g114 = appendResult10_g99;
				float4 temp_output_24_0_g114 = _NoiseTextureTilling;
				float2 AnimatedNoiseTilling29_g114 = (temp_output_24_0_g114).zw;
				float2 panner7_g114 = ( 0.1 * _Time.y * _NoisePannerSpeed + float2( 0,0 ));
				float4 AnimatedNoise1344 = tex2Dlod( _NoiseTexture, float4( ( ( UVs27_g114 * AnimatedNoiseTilling29_g114 ) + panner7_g114 ), 0, 0.0) );
				float temp_output_11_0_g130 = radians( ( ( MB_WindDirection870 + ( MB_WindDirectionOffset1373 *  (-1.0 + ( (AnimatedNoise1344).x - 0.0 ) * ( 1.0 - -1.0 ) / ( 1.0 - 0.0 ) ) ) ) * -1.0 ) );
				float3 appendResult14_g130 = (float3(cos( temp_output_11_0_g130 ) , 0.0 , sin( temp_output_11_0_g130 )));
				float3 worldToObj35_g130 = mul( GetWorldToObjectMatrix(), float4( appendResult14_g130, 1 ) ).xyz;
				float3 worldToObj36_g130 = mul( GetWorldToObjectMatrix(), float4( float3( 0,0,0 ), 1 ) ).xyz;
				float3 normalizeResult34_g130 = normalize( (( worldToObj35_g130 - worldToObj36_g130 )).xyz );
				float3 MB_RotationAxis1420 = normalizeResult34_g130;
				float3 RotationAxis56_g131 = MB_RotationAxis1420;
				float MB_Amplitude880 = _MBAmplitude;
				float MB_AmplitudeOffset1356 = _MBAmplitudeOffset;
				float2 StaticNoileTilling28_g114 = (temp_output_24_0_g114).xy;
				float4 StaticNoise1340 = tex2Dlod( _NoiseTexture, float4( ( UVs27_g114 * StaticNoileTilling28_g114 ), 0, 0.0) );
				float4 StaticWorldNoise31_g129 = StaticNoise1340;
				float3 objToWorld52_g129 = mul( GetObjectToWorldMatrix(), float4( float3( 0,0,0 ), 1 ) ).xyz;
				float MB_Frequency873 = _MBFrequency;
				float MB_FrequencyOffset1525 = _MBFrequencyOffset;
				float DB_PhaseShift928 = input.ase_color.a;
				float MB_Phase1360 = _MBPhase;
				float MB_DefaultBending877 = _MBDefaultBending;
				float MB_MaxHeight1335 = _MBMaxHeight;
				float MB_RotationAngle97 = radians( ( ( ( ( MB_Amplitude880 + ( MB_AmplitudeOffset1356 * (StaticWorldNoise31_g129).x ) ) * sin( ( ( ( objToWorld52_g129.x + objToWorld52_g129.z ) + ( ( ( _TimeParameters.x ) * ( MB_Frequency873 + ( MB_FrequencyOffset1525 * (StaticWorldNoise31_g129).x ) ) ) + ( ( 2.0 * PI ) * DB_PhaseShift928 ) ) ) * MB_Phase1360 ) ) ) + MB_DefaultBending877 ) * ( input.positionOS.xyz.y / MB_MaxHeight1335 ) ) );
				float RotationAngle54_g131 = MB_RotationAngle97;
				float3 PivotPoint60_g131 = LocalPivot1504;
				float3 break62_g131 = PivotPoint60_g131;
				float3 appendResult45_g131 = (float3(break62_g131.x , input.positionOS.xyz.y , break62_g131.z));
				float3 rotatedValue30_g131 = RotateAroundAxis( appendResult45_g131, input.positionOS.xyz, RotationAxis56_g131, RotationAngle54_g131 );
				float DB_HorizontalAmplitude1221 = _DBHorizontalAmplitude;
				float DB_HorizontalFrequency1219 = _DBHorizontalFrequency;
				float Frequency41_g115 = DB_HorizontalFrequency1219;
				float3 objToWorld53_g115 = mul( GetObjectToWorldMatrix(), float4( float3( 0,0,0 ), 1 ) ).xyz;
				float DB_HorizontalPhase1401 = _DBHorizontalPhase;
				float3 PivotPoint49_g115 = LocalPivot1504;
				float3 break52_g115 = PivotPoint49_g115;
				float3 appendResult20_g115 = (float3(break52_g115.x , input.positionOS.xyz.y , break52_g115.z));
				float DB_HorizontalMaxRadius1403 = _DBHorizontalMaxRadius;
				float3 rotatedValue33_g115 = RotateAroundAxis( PivotPoint49_g115, input.positionOS.xyz, float3( 0, 1, 0 ), radians( ( ( DB_HorizontalAmplitude1221 * sin( ( ( ( ( _TimeParameters.x ) * Frequency41_g115 ) - ( ( 2.0 * PI ) * ( 1.0 - DB_PhaseShift928 ) ) ) + ( ( ( objToWorld53_g115.x + objToWorld53_g115.z ) + ( ( _TimeParameters.x ) * Frequency41_g115 ) ) * DB_HorizontalPhase1401 ) ) ) ) * ( distance( input.positionOS.xyz , appendResult20_g115 ) / DB_HorizontalMaxRadius1403 ) ) ) );
				#ifdef _ENABLEHORIZONTALBENDING_ON
				float3 staticSwitch1214 = ( ( rotatedValue33_g115 - input.positionOS.xyz ) * 1.0 );
				#else
				float3 staticSwitch1214 = float3( 0, 0, 0 );
				#endif
				float3 DB_VertexOffset769 = staticSwitch1214;
				float3 rotatedValue34_g131 = RotateAroundAxis( PivotPoint60_g131, ( rotatedValue30_g131 + DB_VertexOffset769 ), RotationAxis56_g131, RotationAngle54_g131 );
				float3 temp_output_1533_0 = ( ( rotatedValue34_g131 - input.positionOS.xyz ) * step( 0.01 , input.positionOS.xyz.y ) );
				float3 MainBending89_g158 = temp_output_1533_0;
				float3 appendResult15_g158 = (float3(0.0 , 1.0 , 0.0));
				float3 objToWorld98_g158 = mul( GetObjectToWorldMatrix(), float4( appendResult15_g158, 1 ) ).xyz;
				float3 objToWorld102_g158 = mul( GetObjectToWorldMatrix(), float4( float3( 0,0,0 ), 1 ) ).xyz;
				float3 break20_g158 = ( objToWorld98_g158 - objToWorld102_g158 );
				float3 appendResult24_g158 = (float3(-break20_g158.z , 0.0 , break20_g158.x));
				float3 appendResult3_g158 = (float3(0.0 , 1.0 , 0.0));
				float3 objToWorld100_g158 = mul( GetObjectToWorldMatrix(), float4( appendResult3_g158, 1 ) ).xyz;
				float3 objToWorld106_g158 = mul( GetObjectToWorldMatrix(), float4( float3( 0,0,0 ), 1 ) ).xyz;
				float3 temp_output_107_0_g158 = ( objToWorld100_g158 - objToWorld106_g158 );
				float3 break108_g158 = temp_output_107_0_g158;
				float3 lerpResult84_g158 = lerp( float3( 0, 1, 0 ) , temp_output_107_0_g158 , step( 0.001 , ( abs( break108_g158.x ) + abs( break108_g158.z ) ) ));
				float3 normalizeResult7_g158 = normalize( lerpResult84_g158 );
				float dotResult9_g158 = dot( normalizeResult7_g158 , float3( 0, 1, 0 ) );
				float temp_output_12_0_g158 = acos( dotResult9_g158 );
				float NaNPrevention21_g158 = step( 0.01 , abs( ( temp_output_12_0_g158 * ( 180.0 / PI ) ) ) );
				float3 lerpResult26_g158 = lerp( float3( 1, 0, 0 ) , appendResult24_g158 , NaNPrevention21_g158);
				float3 worldToObj99_g158 = mul( GetWorldToObjectMatrix(), float4( lerpResult26_g158, 1 ) ).xyz;
				float3 worldToObj105_g158 = mul( GetWorldToObjectMatrix(), float4( float3( 0,0,0 ), 1 ) ).xyz;
				float3 normalizeResult49_g158 = normalize( ( worldToObj99_g158 - worldToObj105_g158 ) );
				float3 RotationAxis30_g158 = normalizeResult49_g158;
				float SlopeCorrectionOffset1805 = _SlopeCorrectionOffset;
				float SlopeCorrectionMagnitude1775 = _SlopeCorrectionMagnitude;
				float RotationAngle29_g158 = ( saturate( (  (0.0 + ( (StaticNoise1340).x - 0.0 ) * ( SlopeCorrectionOffset1805 - 0.0 ) / ( 1.0 - 0.0 ) ) + SlopeCorrectionMagnitude1775 ) ) * temp_output_12_0_g158 );
				float3 rotatedValue35_g158 = RotateAroundAxis( LocalPivot1504, ( input.positionOS.xyz + MainBending89_g158 ), RotationAxis30_g158, RotationAngle29_g158 );
				float3 lerpResult52_g158 = lerp( MainBending89_g158 , ( rotatedValue35_g158 - input.positionOS.xyz ) , NaNPrevention21_g158);
				#ifdef _ENABLESLOPECORRECTION_ON
				float3 staticSwitch1781 = lerpResult52_g158;
				#else
				float3 staticSwitch1781 = temp_output_1533_0;
				#endif
				float3 LocalVertexOffset1045 = staticSwitch1781;
				
				output.ase_texcoord1.xy = input.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord1.zw = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = LocalVertexOffset1045;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					input.positionOS.xyz = vertexValue;
				#else
					input.positionOS.xyz += vertexValue;
				#endif

				input.normalOS = input.normalOS;
				input.tangentOS = input.tangentOS;

				float3 positionWS = TransformObjectToWorld( input.positionOS.xyz );
				float3 normalWS = TransformObjectToWorldDir(input.normalOS);

				#if _CASTING_PUNCTUAL_LIGHT_SHADOW
					float3 lightDirectionWS = normalize(_LightPosition - positionWS);
				#else
					float3 lightDirectionWS = _LightDirection;
				#endif

				float4 positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, lightDirectionWS));

				//code for UNITY_REVERSED_Z is moved into Shadows.hlsl from 6000.0.22 and or higher
				positionCS = ApplyShadowClamping(positionCS);

				output.positionCS = positionCS;
				output.positionWS = positionWS;
				return output;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 positionOS : INTERNALTESSPOS;
				half3 normalOS : NORMAL;
				half4 tangentOS : TANGENT;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( Attributes input )
			{
				VertexControl output;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				output.positionOS = input.positionOS;
				output.normalOS = input.normalOS;
				output.tangentOS = input.tangentOS;
				output.ase_texcoord1 = input.ase_texcoord1;
				output.ase_color = input.ase_color;
				output.ase_texcoord = input.ase_texcoord;
				return output;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> input)
			{
				TessellationFactors output;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				output.edge[0] = tf.x; output.edge[1] = tf.y; output.edge[2] = tf.z; output.inside = tf.w;
				return output;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			PackedVaryings DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				Attributes output = (Attributes) 0;
				output.positionOS = patch[0].positionOS * bary.x + patch[1].positionOS * bary.y + patch[2].positionOS * bary.z;
				output.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				output.tangentOS = patch[0].tangentOS * bary.x + patch[1].tangentOS * bary.y + patch[2].tangentOS * bary.z;
				output.ase_texcoord1 = patch[0].ase_texcoord1 * bary.x + patch[1].ase_texcoord1 * bary.y + patch[2].ase_texcoord1 * bary.z;
				output.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				output.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = output.positionOS.xyz - patch[i].normalOS * (dot(output.positionOS.xyz, patch[i].normalOS) - dot(patch[i].positionOS.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				output.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * output.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], output);
				return VertexFunction(output);
			}
			#else
			PackedVaryings vert ( Attributes input )
			{
				return VertexFunction( input );
			}
			#endif

			half4 frag(	PackedVaryings input
						#if defined( ASE_DEPTH_WRITE_ON )
						,out float outputDepth : ASE_SV_DEPTH
						#endif
						 ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( input );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( input );

				#if defined(MAIN_LIGHT_CALCULATE_SHADOWS) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					float4 shadowCoord = TransformWorldToShadowCoord(input.positionWS);
				#else
					float4 shadowCoord = float4(0, 0, 0, 0);
				#endif

				float3 PositionWS = input.positionWS;
				float3 PositionRWS = GetCameraRelativePositionWS( input.positionWS );
				float4 ShadowCoord = shadowCoord;
				float4 ScreenPosNorm = float4( GetNormalizedScreenSpaceUV( input.positionCS ), input.positionCS.zw );
				float4 ClipPos = ComputeClipSpacePosition( ScreenPosNorm.xy, input.positionCS.z ) * input.positionCS.w;
				float4 ScreenPos = ComputeScreenPos( ClipPos );

				float2 uv_MainTex1481 = input.ase_texcoord1.xy;
				float4 tex2DNode1481 = tex2D( _MainTex, uv_MainTex1481 );
				float2 uv_StemTexture1482 = input.ase_texcoord1.xy;
				float4 tex2DNode1482 = tex2D( _StemTexture, uv_StemTexture1482 );
				float TextureMask1458 = step( 1.5 , input.ase_texcoord1.xy.x );
				float lerpResult1492 = lerp( tex2DNode1481.a , tex2DNode1482.a , TextureMask1458);
				float Opacity1494 = lerpResult1492;
				

				float Alpha = Opacity1494;
				float AlphaClipThreshold = 0.5;
				float AlphaClipThresholdShadow = 0.5;

				#if defined( ASE_DEPTH_WRITE_ON )
					float DeviceDepth = input.positionCS.z;
				#endif

				#ifdef _ALPHATEST_ON
					#ifdef _ALPHATEST_SHADOW_ON
						clip(Alpha - AlphaClipThresholdShadow);
					#else
						clip(Alpha - AlphaClipThreshold);
					#endif
				#endif

				#if defined(LOD_FADE_CROSSFADE)
					LODFadeCrossFade( input.positionCS );
				#endif

				#if defined( ASE_DEPTH_WRITE_ON )
					outputDepth = DeviceDepth;
				#endif

				return 0;
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "DepthOnly"
			Tags { "LightMode"="DepthOnly" }

			ZWrite On
			ColorMask R
			AlphaToMask Off

			HLSLPROGRAM

			#pragma multi_compile_local _ALPHATEST_ON
			#define _NORMAL_DROPOFF_TS 1
			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#define ASE_FOG 1
			#define ASE_VERSION 19900
			#define ASE_SRP_VERSION 170003


			#pragma vertex vert
			#pragma fragment frag

			#if defined(_SPECULAR_SETUP) && defined(ASE_LIGHTING_SIMPLE)
				#define _SPECULAR_COLOR 1
			#endif

			#define SHADERPASS SHADERPASS_DEPTHONLY

			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#if defined(LOD_FADE_CROSSFADE)
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/LODCrossFade.hlsl"
            #endif

			#define ASE_NEEDS_TEXTURE_COORDINATES1
			#define ASE_NEEDS_VERT_POSITION
			#define ASE_NEEDS_TEXTURE_COORDINATES0
			#define ASE_NEEDS_FRAG_TEXTURE_COORDINATES0
			#pragma shader_feature_local _ENABLESLOPECORRECTION_ON
			#pragma shader_feature_local _ENABLEHORIZONTALBENDING_ON


			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE) && (SHADER_TARGET >= 45)
				#define ASE_SV_DEPTH SV_DepthLessEqual
				#define ASE_SV_POSITION_QUALIFIERS linear noperspective centroid
			#else
				#define ASE_SV_DEPTH SV_Depth
				#define ASE_SV_POSITION_QUALIFIERS
			#endif

			struct Attributes
			{
				float4 positionOS : POSITION;
				half3 normalOS : NORMAL;
				half4 tangentOS : TANGENT;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryings
			{
				ASE_SV_POSITION_QUALIFIERS float4 positionCS : SV_POSITION;
				float3 positionWS : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _StemColor;
			float4 _FlowerColor2;
			float4 _NoiseTextureTilling;
			float4 _FlowerColor1;
			float2 _NoisePannerSpeed;
			float _ColorBlendStart;
			float _SlopeCorrectionMagnitude;
			float _SlopeCorrectionOffset;
			float _DBHorizontalMaxRadius;
			float _DBHorizontalPhase;
			float _DBHorizontalFrequency;
			float _DBHorizontalAmplitude;
			float _MBMaxHeight;
			float _MBDefaultBending;
			float _MBPhase;
			float _MBFrequencyOffset;
			float _MBFrequency;
			float _MBAmplitudeOffset;
			float _MBAmplitude;
			float _MBWindDirOffset;
			float _MBWindDirBlend;
			float _ColorBlendEnd;
			float _MBWindDir;
			#ifdef ASE_TRANSMISSION
				float _TransmissionShadow;
			#endif
			#ifdef ASE_TRANSLUCENCY
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			#ifdef SCENEPICKINGPASS
				float4 _SelectionID;
			#endif

			#ifdef SCENESELECTIONPASS
				int _ObjectId;
				int _PassValue;
			#endif

			float MBGlobalWindDir;
			sampler2D _NoiseTexture;
			sampler2D _MainTex;
			sampler2D _StemTexture;


			float3 RotateAroundAxis( float3 center, float3 original, float3 u, float angle )
			{
				original -= center;
				float C = cos( angle );
				float S = sin( angle );
				float t = 1 - C;
				float m00 = t * u.x * u.x + C;
				float m01 = t * u.x * u.y - S * u.z;
				float m02 = t * u.x * u.z + S * u.y;
				float m10 = t * u.x * u.y + S * u.z;
				float m11 = t * u.y * u.y + C;
				float m12 = t * u.y * u.z - S * u.x;
				float m20 = t * u.x * u.z - S * u.y;
				float m21 = t * u.y * u.z + S * u.x;
				float m22 = t * u.z * u.z + C;
				float3x3 finalMatrix = float3x3( m00, m01, m02, m10, m11, m12, m20, m21, m22 );
				return mul( finalMatrix, original ) + center;
			}
			

			PackedVaryings VertexFunction( Attributes input  )
			{
				PackedVaryings output = (PackedVaryings)0;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				float lerpResult1811 = lerp( _MBWindDir , MBGlobalWindDir , _MBWindDirBlend);
				float MB_WindDirection870 = lerpResult1811;
				float MB_WindDirectionOffset1373 = _MBWindDirOffset;
				float2 texCoord1502 = input.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float3 appendResult1503 = (float3(texCoord1502.x , 0.0 , texCoord1502.y));
				float3 LocalPivot1504 = -appendResult1503;
				float3 objToWorld11_g99 = mul( GetObjectToWorldMatrix(), float4( LocalPivot1504, 1 ) ).xyz;
				float2 appendResult10_g99 = (float2(objToWorld11_g99.x , objToWorld11_g99.z));
				float2 UVs27_g114 = appendResult10_g99;
				float4 temp_output_24_0_g114 = _NoiseTextureTilling;
				float2 AnimatedNoiseTilling29_g114 = (temp_output_24_0_g114).zw;
				float2 panner7_g114 = ( 0.1 * _Time.y * _NoisePannerSpeed + float2( 0,0 ));
				float4 AnimatedNoise1344 = tex2Dlod( _NoiseTexture, float4( ( ( UVs27_g114 * AnimatedNoiseTilling29_g114 ) + panner7_g114 ), 0, 0.0) );
				float temp_output_11_0_g130 = radians( ( ( MB_WindDirection870 + ( MB_WindDirectionOffset1373 *  (-1.0 + ( (AnimatedNoise1344).x - 0.0 ) * ( 1.0 - -1.0 ) / ( 1.0 - 0.0 ) ) ) ) * -1.0 ) );
				float3 appendResult14_g130 = (float3(cos( temp_output_11_0_g130 ) , 0.0 , sin( temp_output_11_0_g130 )));
				float3 worldToObj35_g130 = mul( GetWorldToObjectMatrix(), float4( appendResult14_g130, 1 ) ).xyz;
				float3 worldToObj36_g130 = mul( GetWorldToObjectMatrix(), float4( float3( 0,0,0 ), 1 ) ).xyz;
				float3 normalizeResult34_g130 = normalize( (( worldToObj35_g130 - worldToObj36_g130 )).xyz );
				float3 MB_RotationAxis1420 = normalizeResult34_g130;
				float3 RotationAxis56_g131 = MB_RotationAxis1420;
				float MB_Amplitude880 = _MBAmplitude;
				float MB_AmplitudeOffset1356 = _MBAmplitudeOffset;
				float2 StaticNoileTilling28_g114 = (temp_output_24_0_g114).xy;
				float4 StaticNoise1340 = tex2Dlod( _NoiseTexture, float4( ( UVs27_g114 * StaticNoileTilling28_g114 ), 0, 0.0) );
				float4 StaticWorldNoise31_g129 = StaticNoise1340;
				float3 objToWorld52_g129 = mul( GetObjectToWorldMatrix(), float4( float3( 0,0,0 ), 1 ) ).xyz;
				float MB_Frequency873 = _MBFrequency;
				float MB_FrequencyOffset1525 = _MBFrequencyOffset;
				float DB_PhaseShift928 = input.ase_color.a;
				float MB_Phase1360 = _MBPhase;
				float MB_DefaultBending877 = _MBDefaultBending;
				float MB_MaxHeight1335 = _MBMaxHeight;
				float MB_RotationAngle97 = radians( ( ( ( ( MB_Amplitude880 + ( MB_AmplitudeOffset1356 * (StaticWorldNoise31_g129).x ) ) * sin( ( ( ( objToWorld52_g129.x + objToWorld52_g129.z ) + ( ( ( _TimeParameters.x ) * ( MB_Frequency873 + ( MB_FrequencyOffset1525 * (StaticWorldNoise31_g129).x ) ) ) + ( ( 2.0 * PI ) * DB_PhaseShift928 ) ) ) * MB_Phase1360 ) ) ) + MB_DefaultBending877 ) * ( input.positionOS.xyz.y / MB_MaxHeight1335 ) ) );
				float RotationAngle54_g131 = MB_RotationAngle97;
				float3 PivotPoint60_g131 = LocalPivot1504;
				float3 break62_g131 = PivotPoint60_g131;
				float3 appendResult45_g131 = (float3(break62_g131.x , input.positionOS.xyz.y , break62_g131.z));
				float3 rotatedValue30_g131 = RotateAroundAxis( appendResult45_g131, input.positionOS.xyz, RotationAxis56_g131, RotationAngle54_g131 );
				float DB_HorizontalAmplitude1221 = _DBHorizontalAmplitude;
				float DB_HorizontalFrequency1219 = _DBHorizontalFrequency;
				float Frequency41_g115 = DB_HorizontalFrequency1219;
				float3 objToWorld53_g115 = mul( GetObjectToWorldMatrix(), float4( float3( 0,0,0 ), 1 ) ).xyz;
				float DB_HorizontalPhase1401 = _DBHorizontalPhase;
				float3 PivotPoint49_g115 = LocalPivot1504;
				float3 break52_g115 = PivotPoint49_g115;
				float3 appendResult20_g115 = (float3(break52_g115.x , input.positionOS.xyz.y , break52_g115.z));
				float DB_HorizontalMaxRadius1403 = _DBHorizontalMaxRadius;
				float3 rotatedValue33_g115 = RotateAroundAxis( PivotPoint49_g115, input.positionOS.xyz, float3( 0, 1, 0 ), radians( ( ( DB_HorizontalAmplitude1221 * sin( ( ( ( ( _TimeParameters.x ) * Frequency41_g115 ) - ( ( 2.0 * PI ) * ( 1.0 - DB_PhaseShift928 ) ) ) + ( ( ( objToWorld53_g115.x + objToWorld53_g115.z ) + ( ( _TimeParameters.x ) * Frequency41_g115 ) ) * DB_HorizontalPhase1401 ) ) ) ) * ( distance( input.positionOS.xyz , appendResult20_g115 ) / DB_HorizontalMaxRadius1403 ) ) ) );
				#ifdef _ENABLEHORIZONTALBENDING_ON
				float3 staticSwitch1214 = ( ( rotatedValue33_g115 - input.positionOS.xyz ) * 1.0 );
				#else
				float3 staticSwitch1214 = float3( 0, 0, 0 );
				#endif
				float3 DB_VertexOffset769 = staticSwitch1214;
				float3 rotatedValue34_g131 = RotateAroundAxis( PivotPoint60_g131, ( rotatedValue30_g131 + DB_VertexOffset769 ), RotationAxis56_g131, RotationAngle54_g131 );
				float3 temp_output_1533_0 = ( ( rotatedValue34_g131 - input.positionOS.xyz ) * step( 0.01 , input.positionOS.xyz.y ) );
				float3 MainBending89_g158 = temp_output_1533_0;
				float3 appendResult15_g158 = (float3(0.0 , 1.0 , 0.0));
				float3 objToWorld98_g158 = mul( GetObjectToWorldMatrix(), float4( appendResult15_g158, 1 ) ).xyz;
				float3 objToWorld102_g158 = mul( GetObjectToWorldMatrix(), float4( float3( 0,0,0 ), 1 ) ).xyz;
				float3 break20_g158 = ( objToWorld98_g158 - objToWorld102_g158 );
				float3 appendResult24_g158 = (float3(-break20_g158.z , 0.0 , break20_g158.x));
				float3 appendResult3_g158 = (float3(0.0 , 1.0 , 0.0));
				float3 objToWorld100_g158 = mul( GetObjectToWorldMatrix(), float4( appendResult3_g158, 1 ) ).xyz;
				float3 objToWorld106_g158 = mul( GetObjectToWorldMatrix(), float4( float3( 0,0,0 ), 1 ) ).xyz;
				float3 temp_output_107_0_g158 = ( objToWorld100_g158 - objToWorld106_g158 );
				float3 break108_g158 = temp_output_107_0_g158;
				float3 lerpResult84_g158 = lerp( float3( 0, 1, 0 ) , temp_output_107_0_g158 , step( 0.001 , ( abs( break108_g158.x ) + abs( break108_g158.z ) ) ));
				float3 normalizeResult7_g158 = normalize( lerpResult84_g158 );
				float dotResult9_g158 = dot( normalizeResult7_g158 , float3( 0, 1, 0 ) );
				float temp_output_12_0_g158 = acos( dotResult9_g158 );
				float NaNPrevention21_g158 = step( 0.01 , abs( ( temp_output_12_0_g158 * ( 180.0 / PI ) ) ) );
				float3 lerpResult26_g158 = lerp( float3( 1, 0, 0 ) , appendResult24_g158 , NaNPrevention21_g158);
				float3 worldToObj99_g158 = mul( GetWorldToObjectMatrix(), float4( lerpResult26_g158, 1 ) ).xyz;
				float3 worldToObj105_g158 = mul( GetWorldToObjectMatrix(), float4( float3( 0,0,0 ), 1 ) ).xyz;
				float3 normalizeResult49_g158 = normalize( ( worldToObj99_g158 - worldToObj105_g158 ) );
				float3 RotationAxis30_g158 = normalizeResult49_g158;
				float SlopeCorrectionOffset1805 = _SlopeCorrectionOffset;
				float SlopeCorrectionMagnitude1775 = _SlopeCorrectionMagnitude;
				float RotationAngle29_g158 = ( saturate( (  (0.0 + ( (StaticNoise1340).x - 0.0 ) * ( SlopeCorrectionOffset1805 - 0.0 ) / ( 1.0 - 0.0 ) ) + SlopeCorrectionMagnitude1775 ) ) * temp_output_12_0_g158 );
				float3 rotatedValue35_g158 = RotateAroundAxis( LocalPivot1504, ( input.positionOS.xyz + MainBending89_g158 ), RotationAxis30_g158, RotationAngle29_g158 );
				float3 lerpResult52_g158 = lerp( MainBending89_g158 , ( rotatedValue35_g158 - input.positionOS.xyz ) , NaNPrevention21_g158);
				#ifdef _ENABLESLOPECORRECTION_ON
				float3 staticSwitch1781 = lerpResult52_g158;
				#else
				float3 staticSwitch1781 = temp_output_1533_0;
				#endif
				float3 LocalVertexOffset1045 = staticSwitch1781;
				
				output.ase_texcoord1.xy = input.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord1.zw = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = LocalVertexOffset1045;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					input.positionOS.xyz = vertexValue;
				#else
					input.positionOS.xyz += vertexValue;
				#endif

				input.normalOS = input.normalOS;
				input.tangentOS = input.tangentOS;

				VertexPositionInputs vertexInput = GetVertexPositionInputs( input.positionOS.xyz );

				output.positionCS = vertexInput.positionCS;
				output.positionWS = vertexInput.positionWS;
				return output;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 positionOS : INTERNALTESSPOS;
				half3 normalOS : NORMAL;
				half4 tangentOS : TANGENT;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( Attributes input )
			{
				VertexControl output;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				output.positionOS = input.positionOS;
				output.normalOS = input.normalOS;
				output.tangentOS = input.tangentOS;
				output.ase_texcoord1 = input.ase_texcoord1;
				output.ase_color = input.ase_color;
				output.ase_texcoord = input.ase_texcoord;
				return output;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> input)
			{
				TessellationFactors output;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				output.edge[0] = tf.x; output.edge[1] = tf.y; output.edge[2] = tf.z; output.inside = tf.w;
				return output;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			PackedVaryings DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				Attributes output = (Attributes) 0;
				output.positionOS = patch[0].positionOS * bary.x + patch[1].positionOS * bary.y + patch[2].positionOS * bary.z;
				output.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				output.tangentOS = patch[0].tangentOS * bary.x + patch[1].tangentOS * bary.y + patch[2].tangentOS * bary.z;
				output.ase_texcoord1 = patch[0].ase_texcoord1 * bary.x + patch[1].ase_texcoord1 * bary.y + patch[2].ase_texcoord1 * bary.z;
				output.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				output.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = output.positionOS.xyz - patch[i].normalOS * (dot(output.positionOS.xyz, patch[i].normalOS) - dot(patch[i].positionOS.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				output.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * output.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], output);
				return VertexFunction(output);
			}
			#else
			PackedVaryings vert ( Attributes input )
			{
				return VertexFunction( input );
			}
			#endif

			half4 frag(	PackedVaryings input
						#if defined( ASE_DEPTH_WRITE_ON )
						,out float outputDepth : ASE_SV_DEPTH
						#endif
						 ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( input );

				#if defined(MAIN_LIGHT_CALCULATE_SHADOWS) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					float4 shadowCoord = TransformWorldToShadowCoord(input.positionWS);
				#else
					float4 shadowCoord = float4(0, 0, 0, 0);
				#endif

				float3 PositionWS = input.positionWS;
				float3 PositionRWS = GetCameraRelativePositionWS( input.positionWS );
				float4 ShadowCoord = shadowCoord;
				float4 ScreenPosNorm = float4( GetNormalizedScreenSpaceUV( input.positionCS ), input.positionCS.zw );
				float4 ClipPos = ComputeClipSpacePosition( ScreenPosNorm.xy, input.positionCS.z ) * input.positionCS.w;
				float4 ScreenPos = ComputeScreenPos( ClipPos );

				float2 uv_MainTex1481 = input.ase_texcoord1.xy;
				float4 tex2DNode1481 = tex2D( _MainTex, uv_MainTex1481 );
				float2 uv_StemTexture1482 = input.ase_texcoord1.xy;
				float4 tex2DNode1482 = tex2D( _StemTexture, uv_StemTexture1482 );
				float TextureMask1458 = step( 1.5 , input.ase_texcoord1.xy.x );
				float lerpResult1492 = lerp( tex2DNode1481.a , tex2DNode1482.a , TextureMask1458);
				float Opacity1494 = lerpResult1492;
				

				float Alpha = Opacity1494;
				float AlphaClipThreshold = 0.5;

				#if defined( ASE_DEPTH_WRITE_ON )
					float DeviceDepth = input.positionCS.z;
				#endif

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				#if defined(LOD_FADE_CROSSFADE)
					LODFadeCrossFade( input.positionCS );
				#endif

				#if defined( ASE_DEPTH_WRITE_ON )
					outputDepth = DeviceDepth;
				#endif

				return 0;
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "Meta"
			Tags { "LightMode"="Meta" }

			Cull Off

			HLSLPROGRAM
			#pragma multi_compile_local _ALPHATEST_ON
			#define _NORMAL_DROPOFF_TS 1
			#define ASE_FOG 1
			#define ASE_VERSION 19900
			#define ASE_SRP_VERSION 170003

			#pragma shader_feature EDITOR_VISUALIZATION

			#pragma vertex vert
			#pragma fragment frag

			#if defined(_SPECULAR_SETUP) && defined(ASE_LIGHTING_SIMPLE)
				#define _SPECULAR_COLOR 1
			#endif

			#define SHADERPASS SHADERPASS_META

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/MetaInput.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#define ASE_NEEDS_TEXTURE_COORDINATES1
			#define ASE_NEEDS_VERT_TEXTURE_COORDINATES1
			#define ASE_NEEDS_VERT_POSITION
			#define ASE_NEEDS_TEXTURE_COORDINATES0
			#define ASE_NEEDS_FRAG_TEXTURE_COORDINATES0
			#pragma shader_feature_local _ENABLESLOPECORRECTION_ON
			#pragma shader_feature_local _ENABLEHORIZONTALBENDING_ON


			struct Attributes
			{
				float4 positionOS : POSITION;
				half3 normalOS : NORMAL;
				half4 tangentOS : TANGENT;
				float4 texcoord0 : TEXCOORD0;
				float4 texcoord1 : TEXCOORD1;
				float4 texcoord2 : TEXCOORD2;
				float4 ase_color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryings
			{
				float4 positionCS : SV_POSITION;
				float3 positionWS : TEXCOORD0;
				#ifdef EDITOR_VISUALIZATION
					float4 VizUV : TEXCOORD1;
					float4 LightCoord : TEXCOORD2;
				#endif
				float4 ase_texcoord3 : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _StemColor;
			float4 _FlowerColor2;
			float4 _NoiseTextureTilling;
			float4 _FlowerColor1;
			float2 _NoisePannerSpeed;
			float _ColorBlendStart;
			float _SlopeCorrectionMagnitude;
			float _SlopeCorrectionOffset;
			float _DBHorizontalMaxRadius;
			float _DBHorizontalPhase;
			float _DBHorizontalFrequency;
			float _DBHorizontalAmplitude;
			float _MBMaxHeight;
			float _MBDefaultBending;
			float _MBPhase;
			float _MBFrequencyOffset;
			float _MBFrequency;
			float _MBAmplitudeOffset;
			float _MBAmplitude;
			float _MBWindDirOffset;
			float _MBWindDirBlend;
			float _ColorBlendEnd;
			float _MBWindDir;
			#ifdef ASE_TRANSMISSION
				float _TransmissionShadow;
			#endif
			#ifdef ASE_TRANSLUCENCY
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			#ifdef SCENEPICKINGPASS
				float4 _SelectionID;
			#endif

			#ifdef SCENESELECTIONPASS
				int _ObjectId;
				int _PassValue;
			#endif

			float MBGlobalWindDir;
			sampler2D _NoiseTexture;
			sampler2D _MainTex;
			sampler2D _StemTexture;


			float3 RotateAroundAxis( float3 center, float3 original, float3 u, float angle )
			{
				original -= center;
				float C = cos( angle );
				float S = sin( angle );
				float t = 1 - C;
				float m00 = t * u.x * u.x + C;
				float m01 = t * u.x * u.y - S * u.z;
				float m02 = t * u.x * u.z + S * u.y;
				float m10 = t * u.x * u.y + S * u.z;
				float m11 = t * u.y * u.y + C;
				float m12 = t * u.y * u.z - S * u.x;
				float m20 = t * u.x * u.z - S * u.y;
				float m21 = t * u.y * u.z + S * u.x;
				float m22 = t * u.z * u.z + C;
				float3x3 finalMatrix = float3x3( m00, m01, m02, m10, m11, m12, m20, m21, m22 );
				return mul( finalMatrix, original ) + center;
			}
			

			PackedVaryings VertexFunction( Attributes input  )
			{
				PackedVaryings output = (PackedVaryings)0;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				float lerpResult1811 = lerp( _MBWindDir , MBGlobalWindDir , _MBWindDirBlend);
				float MB_WindDirection870 = lerpResult1811;
				float MB_WindDirectionOffset1373 = _MBWindDirOffset;
				float2 texCoord1502 = input.texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float3 appendResult1503 = (float3(texCoord1502.x , 0.0 , texCoord1502.y));
				float3 LocalPivot1504 = -appendResult1503;
				float3 objToWorld11_g99 = mul( GetObjectToWorldMatrix(), float4( LocalPivot1504, 1 ) ).xyz;
				float2 appendResult10_g99 = (float2(objToWorld11_g99.x , objToWorld11_g99.z));
				float2 UVs27_g114 = appendResult10_g99;
				float4 temp_output_24_0_g114 = _NoiseTextureTilling;
				float2 AnimatedNoiseTilling29_g114 = (temp_output_24_0_g114).zw;
				float2 panner7_g114 = ( 0.1 * _Time.y * _NoisePannerSpeed + float2( 0,0 ));
				float4 AnimatedNoise1344 = tex2Dlod( _NoiseTexture, float4( ( ( UVs27_g114 * AnimatedNoiseTilling29_g114 ) + panner7_g114 ), 0, 0.0) );
				float temp_output_11_0_g130 = radians( ( ( MB_WindDirection870 + ( MB_WindDirectionOffset1373 *  (-1.0 + ( (AnimatedNoise1344).x - 0.0 ) * ( 1.0 - -1.0 ) / ( 1.0 - 0.0 ) ) ) ) * -1.0 ) );
				float3 appendResult14_g130 = (float3(cos( temp_output_11_0_g130 ) , 0.0 , sin( temp_output_11_0_g130 )));
				float3 worldToObj35_g130 = mul( GetWorldToObjectMatrix(), float4( appendResult14_g130, 1 ) ).xyz;
				float3 worldToObj36_g130 = mul( GetWorldToObjectMatrix(), float4( float3( 0,0,0 ), 1 ) ).xyz;
				float3 normalizeResult34_g130 = normalize( (( worldToObj35_g130 - worldToObj36_g130 )).xyz );
				float3 MB_RotationAxis1420 = normalizeResult34_g130;
				float3 RotationAxis56_g131 = MB_RotationAxis1420;
				float MB_Amplitude880 = _MBAmplitude;
				float MB_AmplitudeOffset1356 = _MBAmplitudeOffset;
				float2 StaticNoileTilling28_g114 = (temp_output_24_0_g114).xy;
				float4 StaticNoise1340 = tex2Dlod( _NoiseTexture, float4( ( UVs27_g114 * StaticNoileTilling28_g114 ), 0, 0.0) );
				float4 StaticWorldNoise31_g129 = StaticNoise1340;
				float3 objToWorld52_g129 = mul( GetObjectToWorldMatrix(), float4( float3( 0,0,0 ), 1 ) ).xyz;
				float MB_Frequency873 = _MBFrequency;
				float MB_FrequencyOffset1525 = _MBFrequencyOffset;
				float DB_PhaseShift928 = input.ase_color.a;
				float MB_Phase1360 = _MBPhase;
				float MB_DefaultBending877 = _MBDefaultBending;
				float MB_MaxHeight1335 = _MBMaxHeight;
				float MB_RotationAngle97 = radians( ( ( ( ( MB_Amplitude880 + ( MB_AmplitudeOffset1356 * (StaticWorldNoise31_g129).x ) ) * sin( ( ( ( objToWorld52_g129.x + objToWorld52_g129.z ) + ( ( ( _TimeParameters.x ) * ( MB_Frequency873 + ( MB_FrequencyOffset1525 * (StaticWorldNoise31_g129).x ) ) ) + ( ( 2.0 * PI ) * DB_PhaseShift928 ) ) ) * MB_Phase1360 ) ) ) + MB_DefaultBending877 ) * ( input.positionOS.xyz.y / MB_MaxHeight1335 ) ) );
				float RotationAngle54_g131 = MB_RotationAngle97;
				float3 PivotPoint60_g131 = LocalPivot1504;
				float3 break62_g131 = PivotPoint60_g131;
				float3 appendResult45_g131 = (float3(break62_g131.x , input.positionOS.xyz.y , break62_g131.z));
				float3 rotatedValue30_g131 = RotateAroundAxis( appendResult45_g131, input.positionOS.xyz, RotationAxis56_g131, RotationAngle54_g131 );
				float DB_HorizontalAmplitude1221 = _DBHorizontalAmplitude;
				float DB_HorizontalFrequency1219 = _DBHorizontalFrequency;
				float Frequency41_g115 = DB_HorizontalFrequency1219;
				float3 objToWorld53_g115 = mul( GetObjectToWorldMatrix(), float4( float3( 0,0,0 ), 1 ) ).xyz;
				float DB_HorizontalPhase1401 = _DBHorizontalPhase;
				float3 PivotPoint49_g115 = LocalPivot1504;
				float3 break52_g115 = PivotPoint49_g115;
				float3 appendResult20_g115 = (float3(break52_g115.x , input.positionOS.xyz.y , break52_g115.z));
				float DB_HorizontalMaxRadius1403 = _DBHorizontalMaxRadius;
				float3 rotatedValue33_g115 = RotateAroundAxis( PivotPoint49_g115, input.positionOS.xyz, float3( 0, 1, 0 ), radians( ( ( DB_HorizontalAmplitude1221 * sin( ( ( ( ( _TimeParameters.x ) * Frequency41_g115 ) - ( ( 2.0 * PI ) * ( 1.0 - DB_PhaseShift928 ) ) ) + ( ( ( objToWorld53_g115.x + objToWorld53_g115.z ) + ( ( _TimeParameters.x ) * Frequency41_g115 ) ) * DB_HorizontalPhase1401 ) ) ) ) * ( distance( input.positionOS.xyz , appendResult20_g115 ) / DB_HorizontalMaxRadius1403 ) ) ) );
				#ifdef _ENABLEHORIZONTALBENDING_ON
				float3 staticSwitch1214 = ( ( rotatedValue33_g115 - input.positionOS.xyz ) * 1.0 );
				#else
				float3 staticSwitch1214 = float3( 0, 0, 0 );
				#endif
				float3 DB_VertexOffset769 = staticSwitch1214;
				float3 rotatedValue34_g131 = RotateAroundAxis( PivotPoint60_g131, ( rotatedValue30_g131 + DB_VertexOffset769 ), RotationAxis56_g131, RotationAngle54_g131 );
				float3 temp_output_1533_0 = ( ( rotatedValue34_g131 - input.positionOS.xyz ) * step( 0.01 , input.positionOS.xyz.y ) );
				float3 MainBending89_g158 = temp_output_1533_0;
				float3 appendResult15_g158 = (float3(0.0 , 1.0 , 0.0));
				float3 objToWorld98_g158 = mul( GetObjectToWorldMatrix(), float4( appendResult15_g158, 1 ) ).xyz;
				float3 objToWorld102_g158 = mul( GetObjectToWorldMatrix(), float4( float3( 0,0,0 ), 1 ) ).xyz;
				float3 break20_g158 = ( objToWorld98_g158 - objToWorld102_g158 );
				float3 appendResult24_g158 = (float3(-break20_g158.z , 0.0 , break20_g158.x));
				float3 appendResult3_g158 = (float3(0.0 , 1.0 , 0.0));
				float3 objToWorld100_g158 = mul( GetObjectToWorldMatrix(), float4( appendResult3_g158, 1 ) ).xyz;
				float3 objToWorld106_g158 = mul( GetObjectToWorldMatrix(), float4( float3( 0,0,0 ), 1 ) ).xyz;
				float3 temp_output_107_0_g158 = ( objToWorld100_g158 - objToWorld106_g158 );
				float3 break108_g158 = temp_output_107_0_g158;
				float3 lerpResult84_g158 = lerp( float3( 0, 1, 0 ) , temp_output_107_0_g158 , step( 0.001 , ( abs( break108_g158.x ) + abs( break108_g158.z ) ) ));
				float3 normalizeResult7_g158 = normalize( lerpResult84_g158 );
				float dotResult9_g158 = dot( normalizeResult7_g158 , float3( 0, 1, 0 ) );
				float temp_output_12_0_g158 = acos( dotResult9_g158 );
				float NaNPrevention21_g158 = step( 0.01 , abs( ( temp_output_12_0_g158 * ( 180.0 / PI ) ) ) );
				float3 lerpResult26_g158 = lerp( float3( 1, 0, 0 ) , appendResult24_g158 , NaNPrevention21_g158);
				float3 worldToObj99_g158 = mul( GetWorldToObjectMatrix(), float4( lerpResult26_g158, 1 ) ).xyz;
				float3 worldToObj105_g158 = mul( GetWorldToObjectMatrix(), float4( float3( 0,0,0 ), 1 ) ).xyz;
				float3 normalizeResult49_g158 = normalize( ( worldToObj99_g158 - worldToObj105_g158 ) );
				float3 RotationAxis30_g158 = normalizeResult49_g158;
				float SlopeCorrectionOffset1805 = _SlopeCorrectionOffset;
				float SlopeCorrectionMagnitude1775 = _SlopeCorrectionMagnitude;
				float RotationAngle29_g158 = ( saturate( (  (0.0 + ( (StaticNoise1340).x - 0.0 ) * ( SlopeCorrectionOffset1805 - 0.0 ) / ( 1.0 - 0.0 ) ) + SlopeCorrectionMagnitude1775 ) ) * temp_output_12_0_g158 );
				float3 rotatedValue35_g158 = RotateAroundAxis( LocalPivot1504, ( input.positionOS.xyz + MainBending89_g158 ), RotationAxis30_g158, RotationAngle29_g158 );
				float3 lerpResult52_g158 = lerp( MainBending89_g158 , ( rotatedValue35_g158 - input.positionOS.xyz ) , NaNPrevention21_g158);
				#ifdef _ENABLESLOPECORRECTION_ON
				float3 staticSwitch1781 = lerpResult52_g158;
				#else
				float3 staticSwitch1781 = temp_output_1533_0;
				#endif
				float3 LocalVertexOffset1045 = staticSwitch1781;
				
				output.ase_texcoord3.xy = input.texcoord0.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord3.zw = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = LocalVertexOffset1045;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					input.positionOS.xyz = vertexValue;
				#else
					input.positionOS.xyz += vertexValue;
				#endif

				input.normalOS = input.normalOS;
				input.tangentOS = input.tangentOS;

				#ifdef EDITOR_VISUALIZATION
					float2 VizUV = 0;
					float4 LightCoord = 0;
					UnityEditorVizData(input.positionOS.xyz, input.texcoord0.xy, input.texcoord1.xy, input.texcoord2.xy, VizUV, LightCoord);
					output.VizUV = float4(VizUV, 0, 0);
					output.LightCoord = LightCoord;
				#endif

				output.positionCS = MetaVertexPosition( input.positionOS, input.texcoord1.xy, input.texcoord1.xy, unity_LightmapST, unity_DynamicLightmapST );
				output.positionWS = TransformObjectToWorld( input.positionOS.xyz );
				return output;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 positionOS : INTERNALTESSPOS;
				half3 normalOS : NORMAL;
				half4 tangentOS : TANGENT;
				float4 ase_color : COLOR;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( Attributes input )
			{
				VertexControl output;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				output.positionOS = input.positionOS;
				output.normalOS = input.normalOS;
				output.tangentOS = input.tangentOS;
				output.ase_color = input.ase_color;
				return output;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> input)
			{
				TessellationFactors output;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				output.edge[0] = tf.x; output.edge[1] = tf.y; output.edge[2] = tf.z; output.inside = tf.w;
				return output;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			PackedVaryings DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				Attributes output = (Attributes) 0;
				output.positionOS = patch[0].positionOS * bary.x + patch[1].positionOS * bary.y + patch[2].positionOS * bary.z;
				output.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				output.tangentOS = patch[0].tangentOS * bary.x + patch[1].tangentOS * bary.y + patch[2].tangentOS * bary.z;
				output.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = output.positionOS.xyz - patch[i].normalOS * (dot(output.positionOS.xyz, patch[i].normalOS) - dot(patch[i].positionOS.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				output.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * output.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], output);
				return VertexFunction(output);
			}
			#else
			PackedVaryings vert ( Attributes input )
			{
				return VertexFunction( input );
			}
			#endif

			half4 frag(PackedVaryings input  ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( input );

				#if defined(MAIN_LIGHT_CALCULATE_SHADOWS) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					float4 shadowCoord = TransformWorldToShadowCoord(input.positionWS);
				#else
					float4 shadowCoord = float4(0, 0, 0, 0);
				#endif

				float3 PositionWS = input.positionWS;
				float3 PositionRWS = GetCameraRelativePositionWS( input.positionWS );
				float4 ShadowCoord = shadowCoord;

				float2 uv_MainTex1481 = input.ase_texcoord3.xy;
				float4 tex2DNode1481 = tex2D( _MainTex, uv_MainTex1481 );
				float2 uv_StemTexture1482 = input.ase_texcoord3.xy;
				float4 tex2DNode1482 = tex2D( _StemTexture, uv_StemTexture1482 );
				float TextureMask1458 = step( 1.5 , input.ase_texcoord3.xy.x );
				float4 lerpResult1486 = lerp( tex2DNode1481 , tex2DNode1482 , TextureMask1458);
				float4 TextureColor1487 = lerpResult1486;
				float DistanceToCenter1466 = distance( float2( 0.5,0.5 ) , input.ase_texcoord3.xy );
				float ColorBlendStart1506 = _ColorBlendStart;
				float ColorBlendEnd1812 = _ColorBlendEnd;
				float4 lerpResult1480 = lerp( _FlowerColor1 , _FlowerColor2 , ( saturate( ( ( DistanceToCenter1466 - ColorBlendStart1506 ) / ColorBlendEnd1812 ) ) * step( ColorBlendStart1506 , DistanceToCenter1466 ) ));
				float4 lerpResult1485 = lerp( lerpResult1480 , _StemColor , TextureMask1458);
				float4 Color1488 = lerpResult1485;
				float4 Albedo1493 = ( TextureColor1487 * Color1488 );
				
				float lerpResult1492 = lerp( tex2DNode1481.a , tex2DNode1482.a , TextureMask1458);
				float Opacity1494 = lerpResult1492;
				

				float3 BaseColor = Albedo1493.rgb;
				float3 Emission = 0;
				float Alpha = Opacity1494;
				float AlphaClipThreshold = 0.5;

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				MetaInput metaInput = (MetaInput)0;
				metaInput.Albedo = BaseColor;
				metaInput.Emission = Emission;
				#ifdef EDITOR_VISUALIZATION
					metaInput.VizUV = input.VizUV.xy;
					metaInput.LightCoord = input.LightCoord;
				#endif

				return UnityMetaFragment(metaInput);
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "Universal2D"
			Tags { "LightMode"="Universal2D" }

			Blend One Zero, One Zero
			ZWrite On
			ZTest LEqual
			Offset 0 , 0
			ColorMask RGBA

			HLSLPROGRAM

			#pragma multi_compile_local _ALPHATEST_ON
			#define _NORMAL_DROPOFF_TS 1
			#define ASE_FOG 1
			#define ASE_VERSION 19900
			#define ASE_SRP_VERSION 170003


			#pragma vertex vert
			#pragma fragment frag

			#if defined(_SPECULAR_SETUP) && defined(ASE_LIGHTING_SIMPLE)
				#define _SPECULAR_COLOR 1
			#endif

			#define SHADERPASS SHADERPASS_2D

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#define ASE_NEEDS_TEXTURE_COORDINATES1
			#define ASE_NEEDS_VERT_POSITION
			#define ASE_NEEDS_TEXTURE_COORDINATES0
			#define ASE_NEEDS_FRAG_TEXTURE_COORDINATES0
			#pragma shader_feature_local _ENABLESLOPECORRECTION_ON
			#pragma shader_feature_local _ENABLEHORIZONTALBENDING_ON


			struct Attributes
			{
				float4 positionOS : POSITION;
				half3 normalOS : NORMAL;
				half4 tangentOS : TANGENT;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryings
			{
				float4 positionCS : SV_POSITION;
				float3 positionWS : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _StemColor;
			float4 _FlowerColor2;
			float4 _NoiseTextureTilling;
			float4 _FlowerColor1;
			float2 _NoisePannerSpeed;
			float _ColorBlendStart;
			float _SlopeCorrectionMagnitude;
			float _SlopeCorrectionOffset;
			float _DBHorizontalMaxRadius;
			float _DBHorizontalPhase;
			float _DBHorizontalFrequency;
			float _DBHorizontalAmplitude;
			float _MBMaxHeight;
			float _MBDefaultBending;
			float _MBPhase;
			float _MBFrequencyOffset;
			float _MBFrequency;
			float _MBAmplitudeOffset;
			float _MBAmplitude;
			float _MBWindDirOffset;
			float _MBWindDirBlend;
			float _ColorBlendEnd;
			float _MBWindDir;
			#ifdef ASE_TRANSMISSION
				float _TransmissionShadow;
			#endif
			#ifdef ASE_TRANSLUCENCY
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			#ifdef SCENEPICKINGPASS
				float4 _SelectionID;
			#endif

			#ifdef SCENESELECTIONPASS
				int _ObjectId;
				int _PassValue;
			#endif

			float MBGlobalWindDir;
			sampler2D _NoiseTexture;
			sampler2D _MainTex;
			sampler2D _StemTexture;


			float3 RotateAroundAxis( float3 center, float3 original, float3 u, float angle )
			{
				original -= center;
				float C = cos( angle );
				float S = sin( angle );
				float t = 1 - C;
				float m00 = t * u.x * u.x + C;
				float m01 = t * u.x * u.y - S * u.z;
				float m02 = t * u.x * u.z + S * u.y;
				float m10 = t * u.x * u.y + S * u.z;
				float m11 = t * u.y * u.y + C;
				float m12 = t * u.y * u.z - S * u.x;
				float m20 = t * u.x * u.z - S * u.y;
				float m21 = t * u.y * u.z + S * u.x;
				float m22 = t * u.z * u.z + C;
				float3x3 finalMatrix = float3x3( m00, m01, m02, m10, m11, m12, m20, m21, m22 );
				return mul( finalMatrix, original ) + center;
			}
			

			PackedVaryings VertexFunction( Attributes input  )
			{
				PackedVaryings output = (PackedVaryings)0;
				UNITY_SETUP_INSTANCE_ID( input );
				UNITY_TRANSFER_INSTANCE_ID( input, output );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( output );

				float lerpResult1811 = lerp( _MBWindDir , MBGlobalWindDir , _MBWindDirBlend);
				float MB_WindDirection870 = lerpResult1811;
				float MB_WindDirectionOffset1373 = _MBWindDirOffset;
				float2 texCoord1502 = input.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float3 appendResult1503 = (float3(texCoord1502.x , 0.0 , texCoord1502.y));
				float3 LocalPivot1504 = -appendResult1503;
				float3 objToWorld11_g99 = mul( GetObjectToWorldMatrix(), float4( LocalPivot1504, 1 ) ).xyz;
				float2 appendResult10_g99 = (float2(objToWorld11_g99.x , objToWorld11_g99.z));
				float2 UVs27_g114 = appendResult10_g99;
				float4 temp_output_24_0_g114 = _NoiseTextureTilling;
				float2 AnimatedNoiseTilling29_g114 = (temp_output_24_0_g114).zw;
				float2 panner7_g114 = ( 0.1 * _Time.y * _NoisePannerSpeed + float2( 0,0 ));
				float4 AnimatedNoise1344 = tex2Dlod( _NoiseTexture, float4( ( ( UVs27_g114 * AnimatedNoiseTilling29_g114 ) + panner7_g114 ), 0, 0.0) );
				float temp_output_11_0_g130 = radians( ( ( MB_WindDirection870 + ( MB_WindDirectionOffset1373 *  (-1.0 + ( (AnimatedNoise1344).x - 0.0 ) * ( 1.0 - -1.0 ) / ( 1.0 - 0.0 ) ) ) ) * -1.0 ) );
				float3 appendResult14_g130 = (float3(cos( temp_output_11_0_g130 ) , 0.0 , sin( temp_output_11_0_g130 )));
				float3 worldToObj35_g130 = mul( GetWorldToObjectMatrix(), float4( appendResult14_g130, 1 ) ).xyz;
				float3 worldToObj36_g130 = mul( GetWorldToObjectMatrix(), float4( float3( 0,0,0 ), 1 ) ).xyz;
				float3 normalizeResult34_g130 = normalize( (( worldToObj35_g130 - worldToObj36_g130 )).xyz );
				float3 MB_RotationAxis1420 = normalizeResult34_g130;
				float3 RotationAxis56_g131 = MB_RotationAxis1420;
				float MB_Amplitude880 = _MBAmplitude;
				float MB_AmplitudeOffset1356 = _MBAmplitudeOffset;
				float2 StaticNoileTilling28_g114 = (temp_output_24_0_g114).xy;
				float4 StaticNoise1340 = tex2Dlod( _NoiseTexture, float4( ( UVs27_g114 * StaticNoileTilling28_g114 ), 0, 0.0) );
				float4 StaticWorldNoise31_g129 = StaticNoise1340;
				float3 objToWorld52_g129 = mul( GetObjectToWorldMatrix(), float4( float3( 0,0,0 ), 1 ) ).xyz;
				float MB_Frequency873 = _MBFrequency;
				float MB_FrequencyOffset1525 = _MBFrequencyOffset;
				float DB_PhaseShift928 = input.ase_color.a;
				float MB_Phase1360 = _MBPhase;
				float MB_DefaultBending877 = _MBDefaultBending;
				float MB_MaxHeight1335 = _MBMaxHeight;
				float MB_RotationAngle97 = radians( ( ( ( ( MB_Amplitude880 + ( MB_AmplitudeOffset1356 * (StaticWorldNoise31_g129).x ) ) * sin( ( ( ( objToWorld52_g129.x + objToWorld52_g129.z ) + ( ( ( _TimeParameters.x ) * ( MB_Frequency873 + ( MB_FrequencyOffset1525 * (StaticWorldNoise31_g129).x ) ) ) + ( ( 2.0 * PI ) * DB_PhaseShift928 ) ) ) * MB_Phase1360 ) ) ) + MB_DefaultBending877 ) * ( input.positionOS.xyz.y / MB_MaxHeight1335 ) ) );
				float RotationAngle54_g131 = MB_RotationAngle97;
				float3 PivotPoint60_g131 = LocalPivot1504;
				float3 break62_g131 = PivotPoint60_g131;
				float3 appendResult45_g131 = (float3(break62_g131.x , input.positionOS.xyz.y , break62_g131.z));
				float3 rotatedValue30_g131 = RotateAroundAxis( appendResult45_g131, input.positionOS.xyz, RotationAxis56_g131, RotationAngle54_g131 );
				float DB_HorizontalAmplitude1221 = _DBHorizontalAmplitude;
				float DB_HorizontalFrequency1219 = _DBHorizontalFrequency;
				float Frequency41_g115 = DB_HorizontalFrequency1219;
				float3 objToWorld53_g115 = mul( GetObjectToWorldMatrix(), float4( float3( 0,0,0 ), 1 ) ).xyz;
				float DB_HorizontalPhase1401 = _DBHorizontalPhase;
				float3 PivotPoint49_g115 = LocalPivot1504;
				float3 break52_g115 = PivotPoint49_g115;
				float3 appendResult20_g115 = (float3(break52_g115.x , input.positionOS.xyz.y , break52_g115.z));
				float DB_HorizontalMaxRadius1403 = _DBHorizontalMaxRadius;
				float3 rotatedValue33_g115 = RotateAroundAxis( PivotPoint49_g115, input.positionOS.xyz, float3( 0, 1, 0 ), radians( ( ( DB_HorizontalAmplitude1221 * sin( ( ( ( ( _TimeParameters.x ) * Frequency41_g115 ) - ( ( 2.0 * PI ) * ( 1.0 - DB_PhaseShift928 ) ) ) + ( ( ( objToWorld53_g115.x + objToWorld53_g115.z ) + ( ( _TimeParameters.x ) * Frequency41_g115 ) ) * DB_HorizontalPhase1401 ) ) ) ) * ( distance( input.positionOS.xyz , appendResult20_g115 ) / DB_HorizontalMaxRadius1403 ) ) ) );
				#ifdef _ENABLEHORIZONTALBENDING_ON
				float3 staticSwitch1214 = ( ( rotatedValue33_g115 - input.positionOS.xyz ) * 1.0 );
				#else
				float3 staticSwitch1214 = float3( 0, 0, 0 );
				#endif
				float3 DB_VertexOffset769 = staticSwitch1214;
				float3 rotatedValue34_g131 = RotateAroundAxis( PivotPoint60_g131, ( rotatedValue30_g131 + DB_VertexOffset769 ), RotationAxis56_g131, RotationAngle54_g131 );
				float3 temp_output_1533_0 = ( ( rotatedValue34_g131 - input.positionOS.xyz ) * step( 0.01 , input.positionOS.xyz.y ) );
				float3 MainBending89_g158 = temp_output_1533_0;
				float3 appendResult15_g158 = (float3(0.0 , 1.0 , 0.0));
				float3 objToWorld98_g158 = mul( GetObjectToWorldMatrix(), float4( appendResult15_g158, 1 ) ).xyz;
				float3 objToWorld102_g158 = mul( GetObjectToWorldMatrix(), float4( float3( 0,0,0 ), 1 ) ).xyz;
				float3 break20_g158 = ( objToWorld98_g158 - objToWorld102_g158 );
				float3 appendResult24_g158 = (float3(-break20_g158.z , 0.0 , break20_g158.x));
				float3 appendResult3_g158 = (float3(0.0 , 1.0 , 0.0));
				float3 objToWorld100_g158 = mul( GetObjectToWorldMatrix(), float4( appendResult3_g158, 1 ) ).xyz;
				float3 objToWorld106_g158 = mul( GetObjectToWorldMatrix(), float4( float3( 0,0,0 ), 1 ) ).xyz;
				float3 temp_output_107_0_g158 = ( objToWorld100_g158 - objToWorld106_g158 );
				float3 break108_g158 = temp_output_107_0_g158;
				float3 lerpResult84_g158 = lerp( float3( 0, 1, 0 ) , temp_output_107_0_g158 , step( 0.001 , ( abs( break108_g158.x ) + abs( break108_g158.z ) ) ));
				float3 normalizeResult7_g158 = normalize( lerpResult84_g158 );
				float dotResult9_g158 = dot( normalizeResult7_g158 , float3( 0, 1, 0 ) );
				float temp_output_12_0_g158 = acos( dotResult9_g158 );
				float NaNPrevention21_g158 = step( 0.01 , abs( ( temp_output_12_0_g158 * ( 180.0 / PI ) ) ) );
				float3 lerpResult26_g158 = lerp( float3( 1, 0, 0 ) , appendResult24_g158 , NaNPrevention21_g158);
				float3 worldToObj99_g158 = mul( GetWorldToObjectMatrix(), float4( lerpResult26_g158, 1 ) ).xyz;
				float3 worldToObj105_g158 = mul( GetWorldToObjectMatrix(), float4( float3( 0,0,0 ), 1 ) ).xyz;
				float3 normalizeResult49_g158 = normalize( ( worldToObj99_g158 - worldToObj105_g158 ) );
				float3 RotationAxis30_g158 = normalizeResult49_g158;
				float SlopeCorrectionOffset1805 = _SlopeCorrectionOffset;
				float SlopeCorrectionMagnitude1775 = _SlopeCorrectionMagnitude;
				float RotationAngle29_g158 = ( saturate( (  (0.0 + ( (StaticNoise1340).x - 0.0 ) * ( SlopeCorrectionOffset1805 - 0.0 ) / ( 1.0 - 0.0 ) ) + SlopeCorrectionMagnitude1775 ) ) * temp_output_12_0_g158 );
				float3 rotatedValue35_g158 = RotateAroundAxis( LocalPivot1504, ( input.positionOS.xyz + MainBending89_g158 ), RotationAxis30_g158, RotationAngle29_g158 );
				float3 lerpResult52_g158 = lerp( MainBending89_g158 , ( rotatedValue35_g158 - input.positionOS.xyz ) , NaNPrevention21_g158);
				#ifdef _ENABLESLOPECORRECTION_ON
				float3 staticSwitch1781 = lerpResult52_g158;
				#else
				float3 staticSwitch1781 = temp_output_1533_0;
				#endif
				float3 LocalVertexOffset1045 = staticSwitch1781;
				
				output.ase_texcoord1.xy = input.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord1.zw = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = LocalVertexOffset1045;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					input.positionOS.xyz = vertexValue;
				#else
					input.positionOS.xyz += vertexValue;
				#endif

				input.normalOS = input.normalOS;
				input.tangentOS = input.tangentOS;

				VertexPositionInputs vertexInput = GetVertexPositionInputs( input.positionOS.xyz );

				output.positionCS = vertexInput.positionCS;
				output.positionWS = vertexInput.positionWS;
				return output;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 positionOS : INTERNALTESSPOS;
				half3 normalOS : NORMAL;
				half4 tangentOS : TANGENT;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( Attributes input )
			{
				VertexControl output;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				output.positionOS = input.positionOS;
				output.normalOS = input.normalOS;
				output.tangentOS = input.tangentOS;
				output.ase_texcoord1 = input.ase_texcoord1;
				output.ase_color = input.ase_color;
				output.ase_texcoord = input.ase_texcoord;
				return output;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> input)
			{
				TessellationFactors output;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				output.edge[0] = tf.x; output.edge[1] = tf.y; output.edge[2] = tf.z; output.inside = tf.w;
				return output;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			PackedVaryings DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				Attributes output = (Attributes) 0;
				output.positionOS = patch[0].positionOS * bary.x + patch[1].positionOS * bary.y + patch[2].positionOS * bary.z;
				output.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				output.tangentOS = patch[0].tangentOS * bary.x + patch[1].tangentOS * bary.y + patch[2].tangentOS * bary.z;
				output.ase_texcoord1 = patch[0].ase_texcoord1 * bary.x + patch[1].ase_texcoord1 * bary.y + patch[2].ase_texcoord1 * bary.z;
				output.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				output.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = output.positionOS.xyz - patch[i].normalOS * (dot(output.positionOS.xyz, patch[i].normalOS) - dot(patch[i].positionOS.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				output.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * output.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], output);
				return VertexFunction(output);
			}
			#else
			PackedVaryings vert ( Attributes input )
			{
				return VertexFunction( input );
			}
			#endif

			half4 frag(PackedVaryings input  ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( input );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( input );

				#if defined(MAIN_LIGHT_CALCULATE_SHADOWS) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					float4 shadowCoord = TransformWorldToShadowCoord(input.positionWS);
				#else
					float4 shadowCoord = float4(0, 0, 0, 0);
				#endif

				float3 PositionWS = input.positionWS;
				float3 PositionRWS = GetCameraRelativePositionWS( input.positionWS );
				float4 ShadowCoord = shadowCoord;

				float2 uv_MainTex1481 = input.ase_texcoord1.xy;
				float4 tex2DNode1481 = tex2D( _MainTex, uv_MainTex1481 );
				float2 uv_StemTexture1482 = input.ase_texcoord1.xy;
				float4 tex2DNode1482 = tex2D( _StemTexture, uv_StemTexture1482 );
				float TextureMask1458 = step( 1.5 , input.ase_texcoord1.xy.x );
				float4 lerpResult1486 = lerp( tex2DNode1481 , tex2DNode1482 , TextureMask1458);
				float4 TextureColor1487 = lerpResult1486;
				float DistanceToCenter1466 = distance( float2( 0.5,0.5 ) , input.ase_texcoord1.xy );
				float ColorBlendStart1506 = _ColorBlendStart;
				float ColorBlendEnd1812 = _ColorBlendEnd;
				float4 lerpResult1480 = lerp( _FlowerColor1 , _FlowerColor2 , ( saturate( ( ( DistanceToCenter1466 - ColorBlendStart1506 ) / ColorBlendEnd1812 ) ) * step( ColorBlendStart1506 , DistanceToCenter1466 ) ));
				float4 lerpResult1485 = lerp( lerpResult1480 , _StemColor , TextureMask1458);
				float4 Color1488 = lerpResult1485;
				float4 Albedo1493 = ( TextureColor1487 * Color1488 );
				
				float lerpResult1492 = lerp( tex2DNode1481.a , tex2DNode1482.a , TextureMask1458);
				float Opacity1494 = lerpResult1492;
				

				float3 BaseColor = Albedo1493.rgb;
				float Alpha = Opacity1494;
				float AlphaClipThreshold = 0.5;

				half4 color = half4(BaseColor, Alpha );

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				return color;
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "DepthNormals"
			Tags { "LightMode"="DepthNormals" }

			ZWrite On
			Blend One Zero
			ZTest LEqual
			ZWrite On

			HLSLPROGRAM

			#pragma multi_compile_local _ALPHATEST_ON
			#define _NORMAL_DROPOFF_TS 1
			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#define ASE_FOG 1
			#define ASE_VERSION 19900
			#define ASE_SRP_VERSION 170003


			#pragma vertex vert
			#pragma fragment frag

			#if defined(_SPECULAR_SETUP) && defined(ASE_LIGHTING_SIMPLE)
				#define _SPECULAR_COLOR 1
			#endif

			#define SHADERPASS SHADERPASS_DEPTHNORMALSONLY
			//#define SHADERPASS SHADERPASS_DEPTHNORMALS

			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#if defined(LOD_FADE_CROSSFADE)
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/LODCrossFade.hlsl"
            #endif

			#define ASE_NEEDS_TEXTURE_COORDINATES1
			#define ASE_NEEDS_VERT_POSITION
			#define ASE_NEEDS_TEXTURE_COORDINATES0
			#define ASE_NEEDS_FRAG_TEXTURE_COORDINATES0
			#pragma shader_feature_local _ENABLESLOPECORRECTION_ON
			#pragma shader_feature_local _ENABLEHORIZONTALBENDING_ON


			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE) && (SHADER_TARGET >= 45)
				#define ASE_SV_DEPTH SV_DepthLessEqual
				#define ASE_SV_POSITION_QUALIFIERS linear noperspective centroid
			#else
				#define ASE_SV_DEPTH SV_Depth
				#define ASE_SV_POSITION_QUALIFIERS
			#endif

			struct Attributes
			{
				float4 positionOS : POSITION;
				half3 normalOS : NORMAL;
				half4 tangentOS : TANGENT;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryings
			{
				ASE_SV_POSITION_QUALIFIERS float4 positionCS : SV_POSITION;
				float3 positionWS : TEXCOORD0;
				float3 normalWS : TEXCOORD1;
				half4 tangentWS : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _StemColor;
			float4 _FlowerColor2;
			float4 _NoiseTextureTilling;
			float4 _FlowerColor1;
			float2 _NoisePannerSpeed;
			float _ColorBlendStart;
			float _SlopeCorrectionMagnitude;
			float _SlopeCorrectionOffset;
			float _DBHorizontalMaxRadius;
			float _DBHorizontalPhase;
			float _DBHorizontalFrequency;
			float _DBHorizontalAmplitude;
			float _MBMaxHeight;
			float _MBDefaultBending;
			float _MBPhase;
			float _MBFrequencyOffset;
			float _MBFrequency;
			float _MBAmplitudeOffset;
			float _MBAmplitude;
			float _MBWindDirOffset;
			float _MBWindDirBlend;
			float _ColorBlendEnd;
			float _MBWindDir;
			#ifdef ASE_TRANSMISSION
				float _TransmissionShadow;
			#endif
			#ifdef ASE_TRANSLUCENCY
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			#ifdef SCENEPICKINGPASS
				float4 _SelectionID;
			#endif

			#ifdef SCENESELECTIONPASS
				int _ObjectId;
				int _PassValue;
			#endif

			float MBGlobalWindDir;
			sampler2D _NoiseTexture;
			sampler2D _MainTex;
			sampler2D _StemTexture;


			float3 RotateAroundAxis( float3 center, float3 original, float3 u, float angle )
			{
				original -= center;
				float C = cos( angle );
				float S = sin( angle );
				float t = 1 - C;
				float m00 = t * u.x * u.x + C;
				float m01 = t * u.x * u.y - S * u.z;
				float m02 = t * u.x * u.z + S * u.y;
				float m10 = t * u.x * u.y + S * u.z;
				float m11 = t * u.y * u.y + C;
				float m12 = t * u.y * u.z - S * u.x;
				float m20 = t * u.x * u.z - S * u.y;
				float m21 = t * u.y * u.z + S * u.x;
				float m22 = t * u.z * u.z + C;
				float3x3 finalMatrix = float3x3( m00, m01, m02, m10, m11, m12, m20, m21, m22 );
				return mul( finalMatrix, original ) + center;
			}
			

			PackedVaryings VertexFunction( Attributes input  )
			{
				PackedVaryings output = (PackedVaryings)0;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				float lerpResult1811 = lerp( _MBWindDir , MBGlobalWindDir , _MBWindDirBlend);
				float MB_WindDirection870 = lerpResult1811;
				float MB_WindDirectionOffset1373 = _MBWindDirOffset;
				float2 texCoord1502 = input.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float3 appendResult1503 = (float3(texCoord1502.x , 0.0 , texCoord1502.y));
				float3 LocalPivot1504 = -appendResult1503;
				float3 objToWorld11_g99 = mul( GetObjectToWorldMatrix(), float4( LocalPivot1504, 1 ) ).xyz;
				float2 appendResult10_g99 = (float2(objToWorld11_g99.x , objToWorld11_g99.z));
				float2 UVs27_g114 = appendResult10_g99;
				float4 temp_output_24_0_g114 = _NoiseTextureTilling;
				float2 AnimatedNoiseTilling29_g114 = (temp_output_24_0_g114).zw;
				float2 panner7_g114 = ( 0.1 * _Time.y * _NoisePannerSpeed + float2( 0,0 ));
				float4 AnimatedNoise1344 = tex2Dlod( _NoiseTexture, float4( ( ( UVs27_g114 * AnimatedNoiseTilling29_g114 ) + panner7_g114 ), 0, 0.0) );
				float temp_output_11_0_g130 = radians( ( ( MB_WindDirection870 + ( MB_WindDirectionOffset1373 *  (-1.0 + ( (AnimatedNoise1344).x - 0.0 ) * ( 1.0 - -1.0 ) / ( 1.0 - 0.0 ) ) ) ) * -1.0 ) );
				float3 appendResult14_g130 = (float3(cos( temp_output_11_0_g130 ) , 0.0 , sin( temp_output_11_0_g130 )));
				float3 worldToObj35_g130 = mul( GetWorldToObjectMatrix(), float4( appendResult14_g130, 1 ) ).xyz;
				float3 worldToObj36_g130 = mul( GetWorldToObjectMatrix(), float4( float3( 0,0,0 ), 1 ) ).xyz;
				float3 normalizeResult34_g130 = normalize( (( worldToObj35_g130 - worldToObj36_g130 )).xyz );
				float3 MB_RotationAxis1420 = normalizeResult34_g130;
				float3 RotationAxis56_g131 = MB_RotationAxis1420;
				float MB_Amplitude880 = _MBAmplitude;
				float MB_AmplitudeOffset1356 = _MBAmplitudeOffset;
				float2 StaticNoileTilling28_g114 = (temp_output_24_0_g114).xy;
				float4 StaticNoise1340 = tex2Dlod( _NoiseTexture, float4( ( UVs27_g114 * StaticNoileTilling28_g114 ), 0, 0.0) );
				float4 StaticWorldNoise31_g129 = StaticNoise1340;
				float3 objToWorld52_g129 = mul( GetObjectToWorldMatrix(), float4( float3( 0,0,0 ), 1 ) ).xyz;
				float MB_Frequency873 = _MBFrequency;
				float MB_FrequencyOffset1525 = _MBFrequencyOffset;
				float DB_PhaseShift928 = input.ase_color.a;
				float MB_Phase1360 = _MBPhase;
				float MB_DefaultBending877 = _MBDefaultBending;
				float MB_MaxHeight1335 = _MBMaxHeight;
				float MB_RotationAngle97 = radians( ( ( ( ( MB_Amplitude880 + ( MB_AmplitudeOffset1356 * (StaticWorldNoise31_g129).x ) ) * sin( ( ( ( objToWorld52_g129.x + objToWorld52_g129.z ) + ( ( ( _TimeParameters.x ) * ( MB_Frequency873 + ( MB_FrequencyOffset1525 * (StaticWorldNoise31_g129).x ) ) ) + ( ( 2.0 * PI ) * DB_PhaseShift928 ) ) ) * MB_Phase1360 ) ) ) + MB_DefaultBending877 ) * ( input.positionOS.xyz.y / MB_MaxHeight1335 ) ) );
				float RotationAngle54_g131 = MB_RotationAngle97;
				float3 PivotPoint60_g131 = LocalPivot1504;
				float3 break62_g131 = PivotPoint60_g131;
				float3 appendResult45_g131 = (float3(break62_g131.x , input.positionOS.xyz.y , break62_g131.z));
				float3 rotatedValue30_g131 = RotateAroundAxis( appendResult45_g131, input.positionOS.xyz, RotationAxis56_g131, RotationAngle54_g131 );
				float DB_HorizontalAmplitude1221 = _DBHorizontalAmplitude;
				float DB_HorizontalFrequency1219 = _DBHorizontalFrequency;
				float Frequency41_g115 = DB_HorizontalFrequency1219;
				float3 objToWorld53_g115 = mul( GetObjectToWorldMatrix(), float4( float3( 0,0,0 ), 1 ) ).xyz;
				float DB_HorizontalPhase1401 = _DBHorizontalPhase;
				float3 PivotPoint49_g115 = LocalPivot1504;
				float3 break52_g115 = PivotPoint49_g115;
				float3 appendResult20_g115 = (float3(break52_g115.x , input.positionOS.xyz.y , break52_g115.z));
				float DB_HorizontalMaxRadius1403 = _DBHorizontalMaxRadius;
				float3 rotatedValue33_g115 = RotateAroundAxis( PivotPoint49_g115, input.positionOS.xyz, float3( 0, 1, 0 ), radians( ( ( DB_HorizontalAmplitude1221 * sin( ( ( ( ( _TimeParameters.x ) * Frequency41_g115 ) - ( ( 2.0 * PI ) * ( 1.0 - DB_PhaseShift928 ) ) ) + ( ( ( objToWorld53_g115.x + objToWorld53_g115.z ) + ( ( _TimeParameters.x ) * Frequency41_g115 ) ) * DB_HorizontalPhase1401 ) ) ) ) * ( distance( input.positionOS.xyz , appendResult20_g115 ) / DB_HorizontalMaxRadius1403 ) ) ) );
				#ifdef _ENABLEHORIZONTALBENDING_ON
				float3 staticSwitch1214 = ( ( rotatedValue33_g115 - input.positionOS.xyz ) * 1.0 );
				#else
				float3 staticSwitch1214 = float3( 0, 0, 0 );
				#endif
				float3 DB_VertexOffset769 = staticSwitch1214;
				float3 rotatedValue34_g131 = RotateAroundAxis( PivotPoint60_g131, ( rotatedValue30_g131 + DB_VertexOffset769 ), RotationAxis56_g131, RotationAngle54_g131 );
				float3 temp_output_1533_0 = ( ( rotatedValue34_g131 - input.positionOS.xyz ) * step( 0.01 , input.positionOS.xyz.y ) );
				float3 MainBending89_g158 = temp_output_1533_0;
				float3 appendResult15_g158 = (float3(0.0 , 1.0 , 0.0));
				float3 objToWorld98_g158 = mul( GetObjectToWorldMatrix(), float4( appendResult15_g158, 1 ) ).xyz;
				float3 objToWorld102_g158 = mul( GetObjectToWorldMatrix(), float4( float3( 0,0,0 ), 1 ) ).xyz;
				float3 break20_g158 = ( objToWorld98_g158 - objToWorld102_g158 );
				float3 appendResult24_g158 = (float3(-break20_g158.z , 0.0 , break20_g158.x));
				float3 appendResult3_g158 = (float3(0.0 , 1.0 , 0.0));
				float3 objToWorld100_g158 = mul( GetObjectToWorldMatrix(), float4( appendResult3_g158, 1 ) ).xyz;
				float3 objToWorld106_g158 = mul( GetObjectToWorldMatrix(), float4( float3( 0,0,0 ), 1 ) ).xyz;
				float3 temp_output_107_0_g158 = ( objToWorld100_g158 - objToWorld106_g158 );
				float3 break108_g158 = temp_output_107_0_g158;
				float3 lerpResult84_g158 = lerp( float3( 0, 1, 0 ) , temp_output_107_0_g158 , step( 0.001 , ( abs( break108_g158.x ) + abs( break108_g158.z ) ) ));
				float3 normalizeResult7_g158 = normalize( lerpResult84_g158 );
				float dotResult9_g158 = dot( normalizeResult7_g158 , float3( 0, 1, 0 ) );
				float temp_output_12_0_g158 = acos( dotResult9_g158 );
				float NaNPrevention21_g158 = step( 0.01 , abs( ( temp_output_12_0_g158 * ( 180.0 / PI ) ) ) );
				float3 lerpResult26_g158 = lerp( float3( 1, 0, 0 ) , appendResult24_g158 , NaNPrevention21_g158);
				float3 worldToObj99_g158 = mul( GetWorldToObjectMatrix(), float4( lerpResult26_g158, 1 ) ).xyz;
				float3 worldToObj105_g158 = mul( GetWorldToObjectMatrix(), float4( float3( 0,0,0 ), 1 ) ).xyz;
				float3 normalizeResult49_g158 = normalize( ( worldToObj99_g158 - worldToObj105_g158 ) );
				float3 RotationAxis30_g158 = normalizeResult49_g158;
				float SlopeCorrectionOffset1805 = _SlopeCorrectionOffset;
				float SlopeCorrectionMagnitude1775 = _SlopeCorrectionMagnitude;
				float RotationAngle29_g158 = ( saturate( (  (0.0 + ( (StaticNoise1340).x - 0.0 ) * ( SlopeCorrectionOffset1805 - 0.0 ) / ( 1.0 - 0.0 ) ) + SlopeCorrectionMagnitude1775 ) ) * temp_output_12_0_g158 );
				float3 rotatedValue35_g158 = RotateAroundAxis( LocalPivot1504, ( input.positionOS.xyz + MainBending89_g158 ), RotationAxis30_g158, RotationAngle29_g158 );
				float3 lerpResult52_g158 = lerp( MainBending89_g158 , ( rotatedValue35_g158 - input.positionOS.xyz ) , NaNPrevention21_g158);
				#ifdef _ENABLESLOPECORRECTION_ON
				float3 staticSwitch1781 = lerpResult52_g158;
				#else
				float3 staticSwitch1781 = temp_output_1533_0;
				#endif
				float3 LocalVertexOffset1045 = staticSwitch1781;
				
				output.ase_texcoord3.xy = input.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord3.zw = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = LocalVertexOffset1045;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					input.positionOS.xyz = vertexValue;
				#else
					input.positionOS.xyz += vertexValue;
				#endif

				input.normalOS = input.normalOS;
				input.tangentOS = input.tangentOS;

				VertexPositionInputs vertexInput = GetVertexPositionInputs( input.positionOS.xyz );
				VertexNormalInputs normalInput = GetVertexNormalInputs( input.normalOS, input.tangentOS );

				output.positionCS = vertexInput.positionCS;
				output.positionWS = vertexInput.positionWS;
				output.normalWS = normalInput.normalWS;
				output.tangentWS = float4( normalInput.tangentWS, ( input.tangentOS.w > 0.0 ? 1.0 : -1.0 ) * GetOddNegativeScale() );
				return output;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 positionOS : INTERNALTESSPOS;
				half3 normalOS : NORMAL;
				half4 tangentOS : TANGENT;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( Attributes input )
			{
				VertexControl output;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				output.positionOS = input.positionOS;
				output.normalOS = input.normalOS;
				output.tangentOS = input.tangentOS;
				output.ase_texcoord1 = input.ase_texcoord1;
				output.ase_color = input.ase_color;
				output.ase_texcoord = input.ase_texcoord;
				return output;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> input)
			{
				TessellationFactors output;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				output.edge[0] = tf.x; output.edge[1] = tf.y; output.edge[2] = tf.z; output.inside = tf.w;
				return output;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			PackedVaryings DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				Attributes output = (Attributes) 0;
				output.positionOS = patch[0].positionOS * bary.x + patch[1].positionOS * bary.y + patch[2].positionOS * bary.z;
				output.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				output.tangentOS = patch[0].tangentOS * bary.x + patch[1].tangentOS * bary.y + patch[2].tangentOS * bary.z;
				output.ase_texcoord1 = patch[0].ase_texcoord1 * bary.x + patch[1].ase_texcoord1 * bary.y + patch[2].ase_texcoord1 * bary.z;
				output.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				output.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = output.positionOS.xyz - patch[i].normalOS * (dot(output.positionOS.xyz, patch[i].normalOS) - dot(patch[i].positionOS.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				output.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * output.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], output);
				return VertexFunction(output);
			}
			#else
			PackedVaryings vert ( Attributes input )
			{
				return VertexFunction( input );
			}
			#endif

			void frag(	PackedVaryings input
						, out half4 outNormalWS : SV_Target0
						#if defined( ASE_DEPTH_WRITE_ON )
						,out float outputDepth : ASE_SV_DEPTH
						#endif
						#ifdef _WRITE_RENDERING_LAYERS
						, out float4 outRenderingLayers : SV_Target1
						#endif
						 )
			{
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( input );

				#if defined(MAIN_LIGHT_CALCULATE_SHADOWS) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					float4 shadowCoord = TransformWorldToShadowCoord(input.positionWS);
				#else
					float4 shadowCoord = float4(0, 0, 0, 0);
				#endif

				// @diogo: mikktspace compliant
				float renormFactor = 1.0 / max( FLT_MIN, length( input.normalWS ) );

				float3 PositionWS = input.positionWS;
				float3 PositionRWS = GetCameraRelativePositionWS( input.positionWS );
				float4 ShadowCoord = shadowCoord;
				float4 ScreenPosNorm = float4( GetNormalizedScreenSpaceUV( input.positionCS ), input.positionCS.zw );
				float4 ClipPos = ComputeClipSpacePosition( ScreenPosNorm.xy, input.positionCS.z ) * input.positionCS.w;
				float4 ScreenPos = ComputeScreenPos( ClipPos );
				float3 TangentWS = input.tangentWS.xyz * renormFactor;
				float3 BitangentWS = cross( input.normalWS, input.tangentWS.xyz ) * input.tangentWS.w * renormFactor;
				float3 NormalWS = input.normalWS * renormFactor;

				float2 uv_MainTex1481 = input.ase_texcoord3.xy;
				float4 tex2DNode1481 = tex2D( _MainTex, uv_MainTex1481 );
				float2 uv_StemTexture1482 = input.ase_texcoord3.xy;
				float4 tex2DNode1482 = tex2D( _StemTexture, uv_StemTexture1482 );
				float TextureMask1458 = step( 1.5 , input.ase_texcoord3.xy.x );
				float lerpResult1492 = lerp( tex2DNode1481.a , tex2DNode1482.a , TextureMask1458);
				float Opacity1494 = lerpResult1492;
				

				float3 Normal = float3(0, 0, 1);
				float Alpha = Opacity1494;
				float AlphaClipThreshold = 0.5;

				#if defined( ASE_DEPTH_WRITE_ON )
					float DeviceDepth = input.positionCS.z;
				#endif

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				#if defined(LOD_FADE_CROSSFADE)
					LODFadeCrossFade( input.positionCS );
				#endif

				#if defined( ASE_DEPTH_WRITE_ON )
					outputDepth = DeviceDepth;
				#endif

				#if defined(_GBUFFER_NORMALS_OCT)
					float2 octNormalWS = PackNormalOctQuadEncode(NormalWS);
					float2 remappedOctNormalWS = saturate(octNormalWS * 0.5 + 0.5);
					half3 packedNormalWS = PackFloat2To888(remappedOctNormalWS);
					outNormalWS = half4(packedNormalWS, 0.0);
				#else
					#if defined(_NORMALMAP)
						#if _NORMAL_DROPOFF_TS
							float3 normalWS = TransformTangentToWorld(Normal, half3x3(TangentWS, BitangentWS, NormalWS));
						#elif _NORMAL_DROPOFF_OS
							float3 normalWS = TransformObjectToWorldNormal(Normal);
						#elif _NORMAL_DROPOFF_WS
							float3 normalWS = Normal;
						#endif
					#else
						float3 normalWS = NormalWS;
					#endif
					outNormalWS = half4(NormalizeNormalPerPixel(normalWS), 0.0);
				#endif

				#ifdef _WRITE_RENDERING_LAYERS
					uint renderingLayers = GetMeshRenderingLayer();
					outRenderingLayers = float4(EncodeMeshRenderingLayer(renderingLayers), 0, 0, 0);
				#endif
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "GBuffer"
			Tags { "LightMode"="UniversalGBuffer" }

			Blend One Zero, One Zero
			ZWrite On
			ZTest LEqual
			Offset 0 , 0
			ColorMask RGBA
			

			HLSLPROGRAM

			#pragma multi_compile_local _ALPHATEST_ON
			#define _NORMAL_DROPOFF_TS 1
			#pragma multi_compile_instancing
			#pragma instancing_options renderinglayer
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define ASE_VERSION 19900
			#define ASE_SRP_VERSION 170003


			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
			#pragma multi_compile_fragment _ _REFLECTION_PROBE_BLENDING
			#pragma multi_compile_fragment _ _REFLECTION_PROBE_BOX_PROJECTION
			#pragma multi_compile_fragment _ _SHADOWS_SOFT _SHADOWS_SOFT_LOW _SHADOWS_SOFT_MEDIUM _SHADOWS_SOFT_HIGH
			#pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
			#pragma multi_compile_fragment _ _GBUFFER_NORMALS_OCT
			#pragma multi_compile_fragment _ _RENDER_PASS_ENABLED

			#pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
			#pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE
			#pragma multi_compile _ SHADOWS_SHADOWMASK
			#pragma multi_compile _ DIRLIGHTMAP_COMBINED
			#pragma multi_compile _ USE_LEGACY_LIGHTMAPS
			#pragma multi_compile _ LIGHTMAP_ON
			#pragma multi_compile _ DYNAMICLIGHTMAP_ON

			#pragma vertex vert
			#pragma fragment frag

			#if defined(_SPECULAR_SETUP) && defined(ASE_LIGHTING_SIMPLE)
				#define _SPECULAR_COLOR 1
			#endif

			#define SHADERPASS SHADERPASS_GBUFFER

			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ProbeVolumeVariants.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DBuffer.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#if defined(LOD_FADE_CROSSFADE)
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/LODCrossFade.hlsl"
            #endif

			#if defined(UNITY_INSTANCING_ENABLED) && defined(_TERRAIN_INSTANCED_PERPIXEL_NORMAL)
				#define ENABLE_TERRAIN_PERPIXEL_NORMAL
			#endif

			#define ASE_NEEDS_TEXTURE_COORDINATES1
			#define ASE_NEEDS_VERT_TEXTURE_COORDINATES1
			#define ASE_NEEDS_VERT_POSITION
			#define ASE_NEEDS_TEXTURE_COORDINATES0
			#define ASE_NEEDS_FRAG_TEXTURE_COORDINATES0
			#pragma shader_feature_local _ENABLESLOPECORRECTION_ON
			#pragma shader_feature_local _ENABLEHORIZONTALBENDING_ON


			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE) && (SHADER_TARGET >= 45)
				#define ASE_SV_DEPTH SV_DepthLessEqual
				#define ASE_SV_POSITION_QUALIFIERS linear noperspective centroid
			#else
				#define ASE_SV_DEPTH SV_Depth
				#define ASE_SV_POSITION_QUALIFIERS
			#endif

			struct Attributes
			{
				float4 positionOS : POSITION;
				half3 normalOS : NORMAL;
				half4 tangentOS : TANGENT;
				float4 texcoord : TEXCOORD0;
				#if defined(LIGHTMAP_ON) || defined(ASE_NEEDS_TEXTURE_COORDINATES1)
					float4 texcoord1 : TEXCOORD1;
				#endif
				#if defined(DYNAMICLIGHTMAP_ON) || defined(ASE_NEEDS_TEXTURE_COORDINATES2)
					float4 texcoord2 : TEXCOORD2;
				#endif
				float4 ase_color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryings
			{
				ASE_SV_POSITION_QUALIFIERS float4 positionCS : SV_POSITION;
				float3 positionWS : TEXCOORD0;
				half3 normalWS : TEXCOORD1;
				half4 tangentWS : TEXCOORD2;
				float4 lightmapUVOrVertexSH : TEXCOORD3;
				#if defined(ASE_FOG) || defined(_ADDITIONAL_LIGHTS_VERTEX)
					half4 fogFactorAndVertexLight : TEXCOORD4;
				#endif
				#if defined(DYNAMICLIGHTMAP_ON)
					float2 dynamicLightmapUV : TEXCOORD5;
				#endif
				#if defined(USE_APV_PROBE_OCCLUSION)
					float4 probeOcclusion : TEXCOORD6;
				#endif
				float4 ase_texcoord7 : TEXCOORD7;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _StemColor;
			float4 _FlowerColor2;
			float4 _NoiseTextureTilling;
			float4 _FlowerColor1;
			float2 _NoisePannerSpeed;
			float _ColorBlendStart;
			float _SlopeCorrectionMagnitude;
			float _SlopeCorrectionOffset;
			float _DBHorizontalMaxRadius;
			float _DBHorizontalPhase;
			float _DBHorizontalFrequency;
			float _DBHorizontalAmplitude;
			float _MBMaxHeight;
			float _MBDefaultBending;
			float _MBPhase;
			float _MBFrequencyOffset;
			float _MBFrequency;
			float _MBAmplitudeOffset;
			float _MBAmplitude;
			float _MBWindDirOffset;
			float _MBWindDirBlend;
			float _ColorBlendEnd;
			float _MBWindDir;
			#ifdef ASE_TRANSMISSION
				float _TransmissionShadow;
			#endif
			#ifdef ASE_TRANSLUCENCY
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			#ifdef SCENEPICKINGPASS
				float4 _SelectionID;
			#endif

			#ifdef SCENESELECTIONPASS
				int _ObjectId;
				int _PassValue;
			#endif

			float MBGlobalWindDir;
			sampler2D _NoiseTexture;
			sampler2D _MainTex;
			sampler2D _StemTexture;


			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/UnityGBuffer.hlsl"

			float3 RotateAroundAxis( float3 center, float3 original, float3 u, float angle )
			{
				original -= center;
				float C = cos( angle );
				float S = sin( angle );
				float t = 1 - C;
				float m00 = t * u.x * u.x + C;
				float m01 = t * u.x * u.y - S * u.z;
				float m02 = t * u.x * u.z + S * u.y;
				float m10 = t * u.x * u.y + S * u.z;
				float m11 = t * u.y * u.y + C;
				float m12 = t * u.y * u.z - S * u.x;
				float m20 = t * u.x * u.z - S * u.y;
				float m21 = t * u.y * u.z + S * u.x;
				float m22 = t * u.z * u.z + C;
				float3x3 finalMatrix = float3x3( m00, m01, m02, m10, m11, m12, m20, m21, m22 );
				return mul( finalMatrix, original ) + center;
			}
			

			PackedVaryings VertexFunction( Attributes input  )
			{
				PackedVaryings output = (PackedVaryings)0;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				float lerpResult1811 = lerp( _MBWindDir , MBGlobalWindDir , _MBWindDirBlend);
				float MB_WindDirection870 = lerpResult1811;
				float MB_WindDirectionOffset1373 = _MBWindDirOffset;
				float2 texCoord1502 = input.texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float3 appendResult1503 = (float3(texCoord1502.x , 0.0 , texCoord1502.y));
				float3 LocalPivot1504 = -appendResult1503;
				float3 objToWorld11_g99 = mul( GetObjectToWorldMatrix(), float4( LocalPivot1504, 1 ) ).xyz;
				float2 appendResult10_g99 = (float2(objToWorld11_g99.x , objToWorld11_g99.z));
				float2 UVs27_g114 = appendResult10_g99;
				float4 temp_output_24_0_g114 = _NoiseTextureTilling;
				float2 AnimatedNoiseTilling29_g114 = (temp_output_24_0_g114).zw;
				float2 panner7_g114 = ( 0.1 * _Time.y * _NoisePannerSpeed + float2( 0,0 ));
				float4 AnimatedNoise1344 = tex2Dlod( _NoiseTexture, float4( ( ( UVs27_g114 * AnimatedNoiseTilling29_g114 ) + panner7_g114 ), 0, 0.0) );
				float temp_output_11_0_g130 = radians( ( ( MB_WindDirection870 + ( MB_WindDirectionOffset1373 *  (-1.0 + ( (AnimatedNoise1344).x - 0.0 ) * ( 1.0 - -1.0 ) / ( 1.0 - 0.0 ) ) ) ) * -1.0 ) );
				float3 appendResult14_g130 = (float3(cos( temp_output_11_0_g130 ) , 0.0 , sin( temp_output_11_0_g130 )));
				float3 worldToObj35_g130 = mul( GetWorldToObjectMatrix(), float4( appendResult14_g130, 1 ) ).xyz;
				float3 worldToObj36_g130 = mul( GetWorldToObjectMatrix(), float4( float3( 0,0,0 ), 1 ) ).xyz;
				float3 normalizeResult34_g130 = normalize( (( worldToObj35_g130 - worldToObj36_g130 )).xyz );
				float3 MB_RotationAxis1420 = normalizeResult34_g130;
				float3 RotationAxis56_g131 = MB_RotationAxis1420;
				float MB_Amplitude880 = _MBAmplitude;
				float MB_AmplitudeOffset1356 = _MBAmplitudeOffset;
				float2 StaticNoileTilling28_g114 = (temp_output_24_0_g114).xy;
				float4 StaticNoise1340 = tex2Dlod( _NoiseTexture, float4( ( UVs27_g114 * StaticNoileTilling28_g114 ), 0, 0.0) );
				float4 StaticWorldNoise31_g129 = StaticNoise1340;
				float3 objToWorld52_g129 = mul( GetObjectToWorldMatrix(), float4( float3( 0,0,0 ), 1 ) ).xyz;
				float MB_Frequency873 = _MBFrequency;
				float MB_FrequencyOffset1525 = _MBFrequencyOffset;
				float DB_PhaseShift928 = input.ase_color.a;
				float MB_Phase1360 = _MBPhase;
				float MB_DefaultBending877 = _MBDefaultBending;
				float MB_MaxHeight1335 = _MBMaxHeight;
				float MB_RotationAngle97 = radians( ( ( ( ( MB_Amplitude880 + ( MB_AmplitudeOffset1356 * (StaticWorldNoise31_g129).x ) ) * sin( ( ( ( objToWorld52_g129.x + objToWorld52_g129.z ) + ( ( ( _TimeParameters.x ) * ( MB_Frequency873 + ( MB_FrequencyOffset1525 * (StaticWorldNoise31_g129).x ) ) ) + ( ( 2.0 * PI ) * DB_PhaseShift928 ) ) ) * MB_Phase1360 ) ) ) + MB_DefaultBending877 ) * ( input.positionOS.xyz.y / MB_MaxHeight1335 ) ) );
				float RotationAngle54_g131 = MB_RotationAngle97;
				float3 PivotPoint60_g131 = LocalPivot1504;
				float3 break62_g131 = PivotPoint60_g131;
				float3 appendResult45_g131 = (float3(break62_g131.x , input.positionOS.xyz.y , break62_g131.z));
				float3 rotatedValue30_g131 = RotateAroundAxis( appendResult45_g131, input.positionOS.xyz, RotationAxis56_g131, RotationAngle54_g131 );
				float DB_HorizontalAmplitude1221 = _DBHorizontalAmplitude;
				float DB_HorizontalFrequency1219 = _DBHorizontalFrequency;
				float Frequency41_g115 = DB_HorizontalFrequency1219;
				float3 objToWorld53_g115 = mul( GetObjectToWorldMatrix(), float4( float3( 0,0,0 ), 1 ) ).xyz;
				float DB_HorizontalPhase1401 = _DBHorizontalPhase;
				float3 PivotPoint49_g115 = LocalPivot1504;
				float3 break52_g115 = PivotPoint49_g115;
				float3 appendResult20_g115 = (float3(break52_g115.x , input.positionOS.xyz.y , break52_g115.z));
				float DB_HorizontalMaxRadius1403 = _DBHorizontalMaxRadius;
				float3 rotatedValue33_g115 = RotateAroundAxis( PivotPoint49_g115, input.positionOS.xyz, float3( 0, 1, 0 ), radians( ( ( DB_HorizontalAmplitude1221 * sin( ( ( ( ( _TimeParameters.x ) * Frequency41_g115 ) - ( ( 2.0 * PI ) * ( 1.0 - DB_PhaseShift928 ) ) ) + ( ( ( objToWorld53_g115.x + objToWorld53_g115.z ) + ( ( _TimeParameters.x ) * Frequency41_g115 ) ) * DB_HorizontalPhase1401 ) ) ) ) * ( distance( input.positionOS.xyz , appendResult20_g115 ) / DB_HorizontalMaxRadius1403 ) ) ) );
				#ifdef _ENABLEHORIZONTALBENDING_ON
				float3 staticSwitch1214 = ( ( rotatedValue33_g115 - input.positionOS.xyz ) * 1.0 );
				#else
				float3 staticSwitch1214 = float3( 0, 0, 0 );
				#endif
				float3 DB_VertexOffset769 = staticSwitch1214;
				float3 rotatedValue34_g131 = RotateAroundAxis( PivotPoint60_g131, ( rotatedValue30_g131 + DB_VertexOffset769 ), RotationAxis56_g131, RotationAngle54_g131 );
				float3 temp_output_1533_0 = ( ( rotatedValue34_g131 - input.positionOS.xyz ) * step( 0.01 , input.positionOS.xyz.y ) );
				float3 MainBending89_g158 = temp_output_1533_0;
				float3 appendResult15_g158 = (float3(0.0 , 1.0 , 0.0));
				float3 objToWorld98_g158 = mul( GetObjectToWorldMatrix(), float4( appendResult15_g158, 1 ) ).xyz;
				float3 objToWorld102_g158 = mul( GetObjectToWorldMatrix(), float4( float3( 0,0,0 ), 1 ) ).xyz;
				float3 break20_g158 = ( objToWorld98_g158 - objToWorld102_g158 );
				float3 appendResult24_g158 = (float3(-break20_g158.z , 0.0 , break20_g158.x));
				float3 appendResult3_g158 = (float3(0.0 , 1.0 , 0.0));
				float3 objToWorld100_g158 = mul( GetObjectToWorldMatrix(), float4( appendResult3_g158, 1 ) ).xyz;
				float3 objToWorld106_g158 = mul( GetObjectToWorldMatrix(), float4( float3( 0,0,0 ), 1 ) ).xyz;
				float3 temp_output_107_0_g158 = ( objToWorld100_g158 - objToWorld106_g158 );
				float3 break108_g158 = temp_output_107_0_g158;
				float3 lerpResult84_g158 = lerp( float3( 0, 1, 0 ) , temp_output_107_0_g158 , step( 0.001 , ( abs( break108_g158.x ) + abs( break108_g158.z ) ) ));
				float3 normalizeResult7_g158 = normalize( lerpResult84_g158 );
				float dotResult9_g158 = dot( normalizeResult7_g158 , float3( 0, 1, 0 ) );
				float temp_output_12_0_g158 = acos( dotResult9_g158 );
				float NaNPrevention21_g158 = step( 0.01 , abs( ( temp_output_12_0_g158 * ( 180.0 / PI ) ) ) );
				float3 lerpResult26_g158 = lerp( float3( 1, 0, 0 ) , appendResult24_g158 , NaNPrevention21_g158);
				float3 worldToObj99_g158 = mul( GetWorldToObjectMatrix(), float4( lerpResult26_g158, 1 ) ).xyz;
				float3 worldToObj105_g158 = mul( GetWorldToObjectMatrix(), float4( float3( 0,0,0 ), 1 ) ).xyz;
				float3 normalizeResult49_g158 = normalize( ( worldToObj99_g158 - worldToObj105_g158 ) );
				float3 RotationAxis30_g158 = normalizeResult49_g158;
				float SlopeCorrectionOffset1805 = _SlopeCorrectionOffset;
				float SlopeCorrectionMagnitude1775 = _SlopeCorrectionMagnitude;
				float RotationAngle29_g158 = ( saturate( (  (0.0 + ( (StaticNoise1340).x - 0.0 ) * ( SlopeCorrectionOffset1805 - 0.0 ) / ( 1.0 - 0.0 ) ) + SlopeCorrectionMagnitude1775 ) ) * temp_output_12_0_g158 );
				float3 rotatedValue35_g158 = RotateAroundAxis( LocalPivot1504, ( input.positionOS.xyz + MainBending89_g158 ), RotationAxis30_g158, RotationAngle29_g158 );
				float3 lerpResult52_g158 = lerp( MainBending89_g158 , ( rotatedValue35_g158 - input.positionOS.xyz ) , NaNPrevention21_g158);
				#ifdef _ENABLESLOPECORRECTION_ON
				float3 staticSwitch1781 = lerpResult52_g158;
				#else
				float3 staticSwitch1781 = temp_output_1533_0;
				#endif
				float3 LocalVertexOffset1045 = staticSwitch1781;
				
				output.ase_texcoord7.xy = input.texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord7.zw = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = LocalVertexOffset1045;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					input.positionOS.xyz = vertexValue;
				#else
					input.positionOS.xyz += vertexValue;
				#endif

				input.normalOS = input.normalOS;
				input.tangentOS = input.tangentOS;

				VertexPositionInputs vertexInput = GetVertexPositionInputs( input.positionOS.xyz );
				VertexNormalInputs normalInput = GetVertexNormalInputs( input.normalOS, input.tangentOS );

				OUTPUT_LIGHTMAP_UV(input.texcoord1, unity_LightmapST, output.lightmapUVOrVertexSH.xy);
				#if defined(DYNAMICLIGHTMAP_ON)
					output.dynamicLightmapUV.xy = input.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
				#endif
				OUTPUT_SH4(vertexInput.positionWS, normalInput.normalWS.xyz, GetWorldSpaceNormalizeViewDir(vertexInput.positionWS), output.lightmapUVOrVertexSH.xyz, output.probeOcclusion);

				#if defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)
					output.lightmapUVOrVertexSH.zw = input.texcoord.xy;
					output.lightmapUVOrVertexSH.xy = input.texcoord.xy * unity_LightmapST.xy + unity_LightmapST.zw;
				#endif

				#if defined(ASE_FOG) || defined(_ADDITIONAL_LIGHTS_VERTEX)
					output.fogFactorAndVertexLight = 0;
					#if defined(ASE_FOG) && !defined(_FOG_FRAGMENT)
						// @diogo: no fog applied in GBuffer
					#endif
					#ifdef _ADDITIONAL_LIGHTS_VERTEX
						half3 vertexLight = VertexLighting( vertexInput.positionWS, normalInput.normalWS );
						output.fogFactorAndVertexLight.yzw = vertexLight;
					#endif
				#endif

				output.positionCS = vertexInput.positionCS;
				output.positionWS = vertexInput.positionWS;
				output.normalWS = normalInput.normalWS;
				output.tangentWS = float4( normalInput.tangentWS, ( input.tangentOS.w > 0.0 ? 1.0 : -1.0 ) * GetOddNegativeScale() );
				return output;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 positionOS : INTERNALTESSPOS;
				half3 normalOS : NORMAL;
				half4 tangentOS : TANGENT;
				float4 texcoord : TEXCOORD0;
				#if defined(LIGHTMAP_ON) || defined(ASE_NEEDS_TEXTURE_COORDINATES1)
					float4 texcoord1 : TEXCOORD1;
				#endif
				#if defined(DYNAMICLIGHTMAP_ON) || defined(ASE_NEEDS_TEXTURE_COORDINATES2)
					float4 texcoord2 : TEXCOORD2;
				#endif
				float4 ase_color : COLOR;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( Attributes input )
			{
				VertexControl output;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				output.positionOS = input.positionOS;
				output.normalOS = input.normalOS;
				output.tangentOS = input.tangentOS;
				output.texcoord = input.texcoord;
				#if defined(LIGHTMAP_ON) || defined(ASE_NEEDS_TEXTURE_COORDINATES1)
					output.texcoord1 = input.texcoord1;
				#endif
				#if defined(DYNAMICLIGHTMAP_ON) || defined(ASE_NEEDS_TEXTURE_COORDINATES2)
					output.texcoord2 = input.texcoord2;
				#endif
				output.ase_color = input.ase_color;
				return output;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> input)
			{
				TessellationFactors output;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				output.edge[0] = tf.x; output.edge[1] = tf.y; output.edge[2] = tf.z; output.inside = tf.w;
				return output;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			PackedVaryings DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				Attributes output = (Attributes) 0;
				output.positionOS = patch[0].positionOS * bary.x + patch[1].positionOS * bary.y + patch[2].positionOS * bary.z;
				output.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				output.tangentOS = patch[0].tangentOS * bary.x + patch[1].tangentOS * bary.y + patch[2].tangentOS * bary.z;
				output.texcoord = patch[0].texcoord * bary.x + patch[1].texcoord * bary.y + patch[2].texcoord * bary.z;
				#if defined(LIGHTMAP_ON) || defined(ASE_NEEDS_TEXTURE_COORDINATES1)
					output.texcoord1 = patch[0].texcoord1 * bary.x + patch[1].texcoord1 * bary.y + patch[2].texcoord1 * bary.z;
				#endif
				#if defined(DYNAMICLIGHTMAP_ON) || defined(ASE_NEEDS_TEXTURE_COORDINATES2)
					output.texcoord2 = patch[0].texcoord2 * bary.x + patch[1].texcoord2 * bary.y + patch[2].texcoord2 * bary.z;
				#endif
				output.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = output.positionOS.xyz - patch[i].normalOS * (dot(output.positionOS.xyz, patch[i].normalOS) - dot(patch[i].positionOS.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				output.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * output.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], output);
				return VertexFunction(output);
			}
			#else
			PackedVaryings vert ( Attributes input )
			{
				return VertexFunction( input );
			}
			#endif

			FragmentOutput frag ( PackedVaryings input
								#if defined( ASE_DEPTH_WRITE_ON )
								,out float outputDepth : ASE_SV_DEPTH
								#endif
								 )
			{
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

				#if defined(LOD_FADE_CROSSFADE)
					LODFadeCrossFade( input.positionCS );
				#endif

				#if defined(MAIN_LIGHT_CALCULATE_SHADOWS)
					float4 shadowCoord = TransformWorldToShadowCoord( input.positionWS );
				#else
					float4 shadowCoord = float4(0, 0, 0, 0);
				#endif

				// @diogo: mikktspace compliant
				float renormFactor = 1.0 / max( FLT_MIN, length( input.normalWS ) );

				float3 PositionWS = input.positionWS;
				float3 PositionRWS = GetCameraRelativePositionWS( PositionWS );
				float3 ViewDirWS = GetWorldSpaceNormalizeViewDir( PositionWS );
				float4 ShadowCoord = shadowCoord;
				float4 ScreenPosNorm = float4( GetNormalizedScreenSpaceUV( input.positionCS ), input.positionCS.zw );
				float4 ClipPos = ComputeClipSpacePosition( ScreenPosNorm.xy, input.positionCS.z ) * input.positionCS.w;
				float4 ScreenPos = ComputeScreenPos( ClipPos );
				float3 TangentWS = input.tangentWS.xyz * renormFactor;
				float3 BitangentWS = cross( input.normalWS, input.tangentWS.xyz ) * input.tangentWS.w * renormFactor;
				float3 NormalWS = input.normalWS * renormFactor;

				#if defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)
					float2 sampleCoords = (input.lightmapUVOrVertexSH.zw / _TerrainHeightmapRecipSize.zw + 0.5f) * _TerrainHeightmapRecipSize.xy;
					NormalWS = TransformObjectToWorldNormal(normalize(SAMPLE_TEXTURE2D(_TerrainNormalmapTexture, sampler_TerrainNormalmapTexture, sampleCoords).rgb * 2 - 1));
					TangentWS = -cross(GetObjectToWorldMatrix()._13_23_33, NormalWS);
					BitangentWS = cross(NormalWS, -TangentWS);
				#endif

				float2 uv_MainTex1481 = input.ase_texcoord7.xy;
				float4 tex2DNode1481 = tex2D( _MainTex, uv_MainTex1481 );
				float2 uv_StemTexture1482 = input.ase_texcoord7.xy;
				float4 tex2DNode1482 = tex2D( _StemTexture, uv_StemTexture1482 );
				float TextureMask1458 = step( 1.5 , input.ase_texcoord7.xy.x );
				float4 lerpResult1486 = lerp( tex2DNode1481 , tex2DNode1482 , TextureMask1458);
				float4 TextureColor1487 = lerpResult1486;
				float DistanceToCenter1466 = distance( float2( 0.5,0.5 ) , input.ase_texcoord7.xy );
				float ColorBlendStart1506 = _ColorBlendStart;
				float ColorBlendEnd1812 = _ColorBlendEnd;
				float4 lerpResult1480 = lerp( _FlowerColor1 , _FlowerColor2 , ( saturate( ( ( DistanceToCenter1466 - ColorBlendStart1506 ) / ColorBlendEnd1812 ) ) * step( ColorBlendStart1506 , DistanceToCenter1466 ) ));
				float4 lerpResult1485 = lerp( lerpResult1480 , _StemColor , TextureMask1458);
				float4 Color1488 = lerpResult1485;
				float4 Albedo1493 = ( TextureColor1487 * Color1488 );
				
				float lerpResult1492 = lerp( tex2DNode1481.a , tex2DNode1482.a , TextureMask1458);
				float Opacity1494 = lerpResult1492;
				

				float3 BaseColor = Albedo1493.rgb;
				float3 Normal = float3(0, 0, 1);
				float3 Specular = 0.5;
				float Metallic = 0.0;
				float Smoothness = 0.0;
				float Occlusion = 1;
				float3 Emission = 0;
				float Alpha = Opacity1494;
				float AlphaClipThreshold = 0.5;
				float AlphaClipThresholdShadow = 0.5;
				float3 BakedGI = 0;
				float3 RefractionColor = 1;
				float RefractionIndex = 1;
				float3 Transmission = 1;
				float3 Translucency = 1;

				#if defined( ASE_DEPTH_WRITE_ON )
					float DeviceDepth = ClipPos.z;
				#endif

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				#if defined(MAIN_LIGHT_CALCULATE_SHADOWS) && defined(ASE_CHANGES_WORLD_POS)
					ShadowCoord = TransformWorldToShadowCoord( PositionWS );
				#endif

				InputData inputData = (InputData)0;
				inputData.positionWS = PositionWS;
				inputData.positionCS = float4( input.positionCS.xy, ClipPos.zw / ClipPos.w );
				inputData.normalizedScreenSpaceUV = ScreenPosNorm.xy;
				inputData.shadowCoord = ShadowCoord;

				#ifdef _NORMALMAP
					#if _NORMAL_DROPOFF_TS
						inputData.normalWS = TransformTangentToWorld(Normal, half3x3( TangentWS, BitangentWS, NormalWS ));
					#elif _NORMAL_DROPOFF_OS
						inputData.normalWS = TransformObjectToWorldNormal(Normal);
					#elif _NORMAL_DROPOFF_WS
						inputData.normalWS = Normal;
					#endif
				#else
					inputData.normalWS = NormalWS;
				#endif

				inputData.normalWS = NormalizeNormalPerPixel(inputData.normalWS);
				inputData.viewDirectionWS = SafeNormalize( ViewDirWS );

				#ifdef ASE_FOG
					// @diogo: no fog applied in GBuffer
				#endif
				#ifdef _ADDITIONAL_LIGHTS_VERTEX
					inputData.vertexLighting = input.fogFactorAndVertexLight.yzw;
				#endif

				#if defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)
					float3 SH = SampleSH(inputData.normalWS.xyz);
				#else
					float3 SH = input.lightmapUVOrVertexSH.xyz;
				#endif

				#if defined(DYNAMICLIGHTMAP_ON)
					inputData.bakedGI = SAMPLE_GI(input.lightmapUVOrVertexSH.xy, input.dynamicLightmapUV.xy, SH, inputData.normalWS);
					inputData.shadowMask = SAMPLE_SHADOWMASK(input.lightmapUVOrVertexSH.xy);
				#elif !defined(LIGHTMAP_ON) && (defined(PROBE_VOLUMES_L1) || defined(PROBE_VOLUMES_L2))
					inputData.bakedGI = SAMPLE_GI(SH,
						GetAbsolutePositionWS(inputData.positionWS),
						inputData.normalWS,
						inputData.viewDirectionWS,
						input.positionCS.xy,
						input.probeOcclusion,
						inputData.shadowMask);
				#else
					inputData.bakedGI = SAMPLE_GI(input.lightmapUVOrVertexSH.xy, SH, inputData.normalWS);
					inputData.shadowMask = SAMPLE_SHADOWMASK(input.lightmapUVOrVertexSH.xy);
				#endif

				#ifdef ASE_BAKEDGI
					inputData.bakedGI = BakedGI;
				#endif

				#if defined(DEBUG_DISPLAY)
					#if defined(DYNAMICLIGHTMAP_ON)
						inputData.dynamicLightmapUV = input.dynamicLightmapUV.xy;
						#endif
					#if defined(LIGHTMAP_ON)
						inputData.staticLightmapUV = input.lightmapUVOrVertexSH.xy;
					#else
						inputData.vertexSH = SH;
					#endif
					#if defined(USE_APV_PROBE_OCCLUSION)
						inputData.probeOcclusion = input.probeOcclusion;
					#endif
				#endif

				#ifdef _DBUFFER
					ApplyDecal(input.positionCS,
						BaseColor,
						Specular,
						inputData.normalWS,
						Metallic,
						Occlusion,
						Smoothness);
				#endif

				BRDFData brdfData;
				InitializeBRDFData(BaseColor, Metallic, Specular, Smoothness, Alpha, brdfData);

				Light mainLight = GetMainLight(inputData.shadowCoord, inputData.positionWS, inputData.shadowMask);
				half4 color;
				MixRealtimeAndBakedGI(mainLight, inputData.normalWS, inputData.bakedGI, inputData.shadowMask);
				color.rgb = GlobalIllumination(brdfData, inputData.bakedGI, Occlusion, inputData.positionWS, inputData.normalWS, inputData.viewDirectionWS);
				color.a = Alpha;

				#ifdef ASE_FINAL_COLOR_ALPHA_MULTIPLY
					color.rgb *= color.a;
				#endif

				#if defined( ASE_DEPTH_WRITE_ON )
					outputDepth = DeviceDepth;
				#endif

				return BRDFDataToGbuffer(brdfData, inputData, Smoothness, Emission + color.rgb, Occlusion);
			}

			ENDHLSL
		}

		
		Pass
		{
			
			Name "SceneSelectionPass"
			Tags { "LightMode"="SceneSelectionPass" }

			Cull Off
			AlphaToMask Off

			HLSLPROGRAM

			#pragma multi_compile_local _ALPHATEST_ON
			#define _NORMAL_DROPOFF_TS 1
			#define ASE_FOG 1
			#define ASE_VERSION 19900
			#define ASE_SRP_VERSION 170003


			#pragma vertex vert
			#pragma fragment frag

			#if defined(_SPECULAR_SETUP) && defined(ASE_LIGHTING_SIMPLE)
				#define _SPECULAR_COLOR 1
			#endif

			#define SCENESELECTIONPASS 1

			#define ATTRIBUTES_NEED_NORMAL
			#define ATTRIBUTES_NEED_TANGENT
			#define SHADERPASS SHADERPASS_DEPTHONLY

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#define ASE_NEEDS_TEXTURE_COORDINATES1
			#define ASE_NEEDS_VERT_POSITION
			#define ASE_NEEDS_TEXTURE_COORDINATES0
			#define ASE_NEEDS_FRAG_TEXTURE_COORDINATES0
			#pragma shader_feature_local _ENABLESLOPECORRECTION_ON
			#pragma shader_feature_local _ENABLEHORIZONTALBENDING_ON


			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE) && (SHADER_TARGET >= 45)
				#define ASE_SV_DEPTH SV_DepthLessEqual
				#define ASE_SV_POSITION_QUALIFIERS linear noperspective centroid
			#else
				#define ASE_SV_DEPTH SV_Depth
				#define ASE_SV_POSITION_QUALIFIERS
			#endif

			struct Attributes
			{
				float4 positionOS : POSITION;
				half3 normalOS : NORMAL;
				half4 tangentOS : TANGENT;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryings
			{
				ASE_SV_POSITION_QUALIFIERS float4 positionCS : SV_POSITION;
				float3 positionWS : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _StemColor;
			float4 _FlowerColor2;
			float4 _NoiseTextureTilling;
			float4 _FlowerColor1;
			float2 _NoisePannerSpeed;
			float _ColorBlendStart;
			float _SlopeCorrectionMagnitude;
			float _SlopeCorrectionOffset;
			float _DBHorizontalMaxRadius;
			float _DBHorizontalPhase;
			float _DBHorizontalFrequency;
			float _DBHorizontalAmplitude;
			float _MBMaxHeight;
			float _MBDefaultBending;
			float _MBPhase;
			float _MBFrequencyOffset;
			float _MBFrequency;
			float _MBAmplitudeOffset;
			float _MBAmplitude;
			float _MBWindDirOffset;
			float _MBWindDirBlend;
			float _ColorBlendEnd;
			float _MBWindDir;
			#ifdef ASE_TRANSMISSION
				float _TransmissionShadow;
			#endif
			#ifdef ASE_TRANSLUCENCY
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			#ifdef SCENEPICKINGPASS
				float4 _SelectionID;
			#endif

			#ifdef SCENESELECTIONPASS
				int _ObjectId;
				int _PassValue;
			#endif

			float MBGlobalWindDir;
			sampler2D _NoiseTexture;
			sampler2D _MainTex;
			sampler2D _StemTexture;


			float3 RotateAroundAxis( float3 center, float3 original, float3 u, float angle )
			{
				original -= center;
				float C = cos( angle );
				float S = sin( angle );
				float t = 1 - C;
				float m00 = t * u.x * u.x + C;
				float m01 = t * u.x * u.y - S * u.z;
				float m02 = t * u.x * u.z + S * u.y;
				float m10 = t * u.x * u.y + S * u.z;
				float m11 = t * u.y * u.y + C;
				float m12 = t * u.y * u.z - S * u.x;
				float m20 = t * u.x * u.z - S * u.y;
				float m21 = t * u.y * u.z + S * u.x;
				float m22 = t * u.z * u.z + C;
				float3x3 finalMatrix = float3x3( m00, m01, m02, m10, m11, m12, m20, m21, m22 );
				return mul( finalMatrix, original ) + center;
			}
			

			struct SurfaceDescription
			{
				float Alpha;
				float AlphaClipThreshold;
			};

			PackedVaryings VertexFunction(Attributes input  )
			{
				PackedVaryings output;
				ZERO_INITIALIZE(PackedVaryings, output);

				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				float lerpResult1811 = lerp( _MBWindDir , MBGlobalWindDir , _MBWindDirBlend);
				float MB_WindDirection870 = lerpResult1811;
				float MB_WindDirectionOffset1373 = _MBWindDirOffset;
				float2 texCoord1502 = input.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float3 appendResult1503 = (float3(texCoord1502.x , 0.0 , texCoord1502.y));
				float3 LocalPivot1504 = -appendResult1503;
				float3 objToWorld11_g99 = mul( GetObjectToWorldMatrix(), float4( LocalPivot1504, 1 ) ).xyz;
				float2 appendResult10_g99 = (float2(objToWorld11_g99.x , objToWorld11_g99.z));
				float2 UVs27_g114 = appendResult10_g99;
				float4 temp_output_24_0_g114 = _NoiseTextureTilling;
				float2 AnimatedNoiseTilling29_g114 = (temp_output_24_0_g114).zw;
				float2 panner7_g114 = ( 0.1 * _Time.y * _NoisePannerSpeed + float2( 0,0 ));
				float4 AnimatedNoise1344 = tex2Dlod( _NoiseTexture, float4( ( ( UVs27_g114 * AnimatedNoiseTilling29_g114 ) + panner7_g114 ), 0, 0.0) );
				float temp_output_11_0_g130 = radians( ( ( MB_WindDirection870 + ( MB_WindDirectionOffset1373 *  (-1.0 + ( (AnimatedNoise1344).x - 0.0 ) * ( 1.0 - -1.0 ) / ( 1.0 - 0.0 ) ) ) ) * -1.0 ) );
				float3 appendResult14_g130 = (float3(cos( temp_output_11_0_g130 ) , 0.0 , sin( temp_output_11_0_g130 )));
				float3 worldToObj35_g130 = mul( GetWorldToObjectMatrix(), float4( appendResult14_g130, 1 ) ).xyz;
				float3 worldToObj36_g130 = mul( GetWorldToObjectMatrix(), float4( float3( 0,0,0 ), 1 ) ).xyz;
				float3 normalizeResult34_g130 = normalize( (( worldToObj35_g130 - worldToObj36_g130 )).xyz );
				float3 MB_RotationAxis1420 = normalizeResult34_g130;
				float3 RotationAxis56_g131 = MB_RotationAxis1420;
				float MB_Amplitude880 = _MBAmplitude;
				float MB_AmplitudeOffset1356 = _MBAmplitudeOffset;
				float2 StaticNoileTilling28_g114 = (temp_output_24_0_g114).xy;
				float4 StaticNoise1340 = tex2Dlod( _NoiseTexture, float4( ( UVs27_g114 * StaticNoileTilling28_g114 ), 0, 0.0) );
				float4 StaticWorldNoise31_g129 = StaticNoise1340;
				float3 objToWorld52_g129 = mul( GetObjectToWorldMatrix(), float4( float3( 0,0,0 ), 1 ) ).xyz;
				float MB_Frequency873 = _MBFrequency;
				float MB_FrequencyOffset1525 = _MBFrequencyOffset;
				float DB_PhaseShift928 = input.ase_color.a;
				float MB_Phase1360 = _MBPhase;
				float MB_DefaultBending877 = _MBDefaultBending;
				float MB_MaxHeight1335 = _MBMaxHeight;
				float MB_RotationAngle97 = radians( ( ( ( ( MB_Amplitude880 + ( MB_AmplitudeOffset1356 * (StaticWorldNoise31_g129).x ) ) * sin( ( ( ( objToWorld52_g129.x + objToWorld52_g129.z ) + ( ( ( _TimeParameters.x ) * ( MB_Frequency873 + ( MB_FrequencyOffset1525 * (StaticWorldNoise31_g129).x ) ) ) + ( ( 2.0 * PI ) * DB_PhaseShift928 ) ) ) * MB_Phase1360 ) ) ) + MB_DefaultBending877 ) * ( input.positionOS.xyz.y / MB_MaxHeight1335 ) ) );
				float RotationAngle54_g131 = MB_RotationAngle97;
				float3 PivotPoint60_g131 = LocalPivot1504;
				float3 break62_g131 = PivotPoint60_g131;
				float3 appendResult45_g131 = (float3(break62_g131.x , input.positionOS.xyz.y , break62_g131.z));
				float3 rotatedValue30_g131 = RotateAroundAxis( appendResult45_g131, input.positionOS.xyz, RotationAxis56_g131, RotationAngle54_g131 );
				float DB_HorizontalAmplitude1221 = _DBHorizontalAmplitude;
				float DB_HorizontalFrequency1219 = _DBHorizontalFrequency;
				float Frequency41_g115 = DB_HorizontalFrequency1219;
				float3 objToWorld53_g115 = mul( GetObjectToWorldMatrix(), float4( float3( 0,0,0 ), 1 ) ).xyz;
				float DB_HorizontalPhase1401 = _DBHorizontalPhase;
				float3 PivotPoint49_g115 = LocalPivot1504;
				float3 break52_g115 = PivotPoint49_g115;
				float3 appendResult20_g115 = (float3(break52_g115.x , input.positionOS.xyz.y , break52_g115.z));
				float DB_HorizontalMaxRadius1403 = _DBHorizontalMaxRadius;
				float3 rotatedValue33_g115 = RotateAroundAxis( PivotPoint49_g115, input.positionOS.xyz, float3( 0, 1, 0 ), radians( ( ( DB_HorizontalAmplitude1221 * sin( ( ( ( ( _TimeParameters.x ) * Frequency41_g115 ) - ( ( 2.0 * PI ) * ( 1.0 - DB_PhaseShift928 ) ) ) + ( ( ( objToWorld53_g115.x + objToWorld53_g115.z ) + ( ( _TimeParameters.x ) * Frequency41_g115 ) ) * DB_HorizontalPhase1401 ) ) ) ) * ( distance( input.positionOS.xyz , appendResult20_g115 ) / DB_HorizontalMaxRadius1403 ) ) ) );
				#ifdef _ENABLEHORIZONTALBENDING_ON
				float3 staticSwitch1214 = ( ( rotatedValue33_g115 - input.positionOS.xyz ) * 1.0 );
				#else
				float3 staticSwitch1214 = float3( 0, 0, 0 );
				#endif
				float3 DB_VertexOffset769 = staticSwitch1214;
				float3 rotatedValue34_g131 = RotateAroundAxis( PivotPoint60_g131, ( rotatedValue30_g131 + DB_VertexOffset769 ), RotationAxis56_g131, RotationAngle54_g131 );
				float3 temp_output_1533_0 = ( ( rotatedValue34_g131 - input.positionOS.xyz ) * step( 0.01 , input.positionOS.xyz.y ) );
				float3 MainBending89_g158 = temp_output_1533_0;
				float3 appendResult15_g158 = (float3(0.0 , 1.0 , 0.0));
				float3 objToWorld98_g158 = mul( GetObjectToWorldMatrix(), float4( appendResult15_g158, 1 ) ).xyz;
				float3 objToWorld102_g158 = mul( GetObjectToWorldMatrix(), float4( float3( 0,0,0 ), 1 ) ).xyz;
				float3 break20_g158 = ( objToWorld98_g158 - objToWorld102_g158 );
				float3 appendResult24_g158 = (float3(-break20_g158.z , 0.0 , break20_g158.x));
				float3 appendResult3_g158 = (float3(0.0 , 1.0 , 0.0));
				float3 objToWorld100_g158 = mul( GetObjectToWorldMatrix(), float4( appendResult3_g158, 1 ) ).xyz;
				float3 objToWorld106_g158 = mul( GetObjectToWorldMatrix(), float4( float3( 0,0,0 ), 1 ) ).xyz;
				float3 temp_output_107_0_g158 = ( objToWorld100_g158 - objToWorld106_g158 );
				float3 break108_g158 = temp_output_107_0_g158;
				float3 lerpResult84_g158 = lerp( float3( 0, 1, 0 ) , temp_output_107_0_g158 , step( 0.001 , ( abs( break108_g158.x ) + abs( break108_g158.z ) ) ));
				float3 normalizeResult7_g158 = normalize( lerpResult84_g158 );
				float dotResult9_g158 = dot( normalizeResult7_g158 , float3( 0, 1, 0 ) );
				float temp_output_12_0_g158 = acos( dotResult9_g158 );
				float NaNPrevention21_g158 = step( 0.01 , abs( ( temp_output_12_0_g158 * ( 180.0 / PI ) ) ) );
				float3 lerpResult26_g158 = lerp( float3( 1, 0, 0 ) , appendResult24_g158 , NaNPrevention21_g158);
				float3 worldToObj99_g158 = mul( GetWorldToObjectMatrix(), float4( lerpResult26_g158, 1 ) ).xyz;
				float3 worldToObj105_g158 = mul( GetWorldToObjectMatrix(), float4( float3( 0,0,0 ), 1 ) ).xyz;
				float3 normalizeResult49_g158 = normalize( ( worldToObj99_g158 - worldToObj105_g158 ) );
				float3 RotationAxis30_g158 = normalizeResult49_g158;
				float SlopeCorrectionOffset1805 = _SlopeCorrectionOffset;
				float SlopeCorrectionMagnitude1775 = _SlopeCorrectionMagnitude;
				float RotationAngle29_g158 = ( saturate( (  (0.0 + ( (StaticNoise1340).x - 0.0 ) * ( SlopeCorrectionOffset1805 - 0.0 ) / ( 1.0 - 0.0 ) ) + SlopeCorrectionMagnitude1775 ) ) * temp_output_12_0_g158 );
				float3 rotatedValue35_g158 = RotateAroundAxis( LocalPivot1504, ( input.positionOS.xyz + MainBending89_g158 ), RotationAxis30_g158, RotationAngle29_g158 );
				float3 lerpResult52_g158 = lerp( MainBending89_g158 , ( rotatedValue35_g158 - input.positionOS.xyz ) , NaNPrevention21_g158);
				#ifdef _ENABLESLOPECORRECTION_ON
				float3 staticSwitch1781 = lerpResult52_g158;
				#else
				float3 staticSwitch1781 = temp_output_1533_0;
				#endif
				float3 LocalVertexOffset1045 = staticSwitch1781;
				
				output.ase_texcoord1.xy = input.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord1.zw = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = LocalVertexOffset1045;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					input.positionOS.xyz = vertexValue;
				#else
					input.positionOS.xyz += vertexValue;
				#endif

				input.normalOS = input.normalOS;

				VertexPositionInputs vertexInput = GetVertexPositionInputs( input.positionOS.xyz );

				output.positionCS = vertexInput.positionCS;
				output.positionWS = vertexInput.positionWS;
				return output;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 positionOS : INTERNALTESSPOS;
				half3 normalOS : NORMAL;
				half4 tangentOS : TANGENT;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( Attributes input )
			{
				VertexControl output;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				output.positionOS = input.positionOS;
				output.normalOS = input.normalOS;
				output.tangentOS = input.tangentOS;
				output.ase_texcoord1 = input.ase_texcoord1;
				output.ase_color = input.ase_color;
				output.ase_texcoord = input.ase_texcoord;
				return output;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> input)
			{
				TessellationFactors output;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				output.edge[0] = tf.x; output.edge[1] = tf.y; output.edge[2] = tf.z; output.inside = tf.w;
				return output;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			PackedVaryings DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				Attributes output = (Attributes) 0;
				output.positionOS = patch[0].positionOS * bary.x + patch[1].positionOS * bary.y + patch[2].positionOS * bary.z;
				output.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				output.tangentOS = patch[0].tangentOS * bary.x + patch[1].tangentOS * bary.y + patch[2].tangentOS * bary.z;
				output.ase_texcoord1 = patch[0].ase_texcoord1 * bary.x + patch[1].ase_texcoord1 * bary.y + patch[2].ase_texcoord1 * bary.z;
				output.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				output.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = output.positionOS.xyz - patch[i].normalOS * (dot(output.positionOS.xyz, patch[i].normalOS) - dot(patch[i].positionOS.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				output.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * output.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], output);
				return VertexFunction(output);
			}
			#else
			PackedVaryings vert ( Attributes input )
			{
				return VertexFunction( input );
			}
			#endif

			half4 frag( PackedVaryings input
				#if defined( ASE_DEPTH_WRITE_ON )
				,out float outputDepth : ASE_SV_DEPTH
				#endif
				 ) : SV_Target
			{
				SurfaceDescription surfaceDescription = (SurfaceDescription)0;

				float3 PositionWS = input.positionWS;
				float3 PositionRWS = GetCameraRelativePositionWS( PositionWS );
				float4 ScreenPosNorm = float4( GetNormalizedScreenSpaceUV( input.positionCS ), input.positionCS.zw );
				float4 ClipPos = ComputeClipSpacePosition( ScreenPosNorm.xy, input.positionCS.z ) * input.positionCS.w;

				float2 uv_MainTex1481 = input.ase_texcoord1.xy;
				float4 tex2DNode1481 = tex2D( _MainTex, uv_MainTex1481 );
				float2 uv_StemTexture1482 = input.ase_texcoord1.xy;
				float4 tex2DNode1482 = tex2D( _StemTexture, uv_StemTexture1482 );
				float TextureMask1458 = step( 1.5 , input.ase_texcoord1.xy.x );
				float lerpResult1492 = lerp( tex2DNode1481.a , tex2DNode1482.a , TextureMask1458);
				float Opacity1494 = lerpResult1492;
				

				surfaceDescription.Alpha = Opacity1494;
				surfaceDescription.AlphaClipThreshold = 0.5;

				#if defined( ASE_DEPTH_WRITE_ON )
					float DeviceDepth = input.positionCS.z;
				#endif

				#if _ALPHATEST_ON
					float alphaClipThreshold = 0.01f;
					#if ALPHA_CLIP_THRESHOLD
						alphaClipThreshold = surfaceDescription.AlphaClipThreshold;
					#endif
					clip(surfaceDescription.Alpha - alphaClipThreshold);
				#endif

				#if defined( ASE_DEPTH_WRITE_ON )
					outputDepth = DeviceDepth;
				#endif

				return half4( _ObjectId, _PassValue, 1.0, 1.0 );
			}

			ENDHLSL
		}

		
		Pass
		{
			
			Name "ScenePickingPass"
			Tags { "LightMode"="Picking" }

			AlphaToMask Off

			HLSLPROGRAM

			#pragma multi_compile_local _ALPHATEST_ON
			#define _NORMAL_DROPOFF_TS 1
			#define ASE_FOG 1
			#define ASE_VERSION 19900
			#define ASE_SRP_VERSION 170003


			#pragma vertex vert
			#pragma fragment frag

			#if defined(_SPECULAR_SETUP) && defined(ASE_LIGHTING_SIMPLE)
				#define _SPECULAR_COLOR 1
			#endif

		    #define SCENEPICKINGPASS 1

			#define ATTRIBUTES_NEED_NORMAL
			#define ATTRIBUTES_NEED_TANGENT
			#define SHADERPASS SHADERPASS_DEPTHONLY

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#define ASE_NEEDS_TEXTURE_COORDINATES1
			#define ASE_NEEDS_VERT_POSITION
			#define ASE_NEEDS_TEXTURE_COORDINATES0
			#define ASE_NEEDS_FRAG_TEXTURE_COORDINATES0
			#pragma shader_feature_local _ENABLESLOPECORRECTION_ON
			#pragma shader_feature_local _ENABLEHORIZONTALBENDING_ON


			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE) && (SHADER_TARGET >= 45)
				#define ASE_SV_DEPTH SV_DepthLessEqual
				#define ASE_SV_POSITION_QUALIFIERS linear noperspective centroid
			#else
				#define ASE_SV_DEPTH SV_Depth
				#define ASE_SV_POSITION_QUALIFIERS
			#endif

			struct Attributes
			{
				float4 positionOS : POSITION;
				half3 normalOS : NORMAL;
				half4 tangentOS : TANGENT;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryings
			{
				ASE_SV_POSITION_QUALIFIERS float4 positionCS : SV_POSITION;
				float3 positionWS : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _StemColor;
			float4 _FlowerColor2;
			float4 _NoiseTextureTilling;
			float4 _FlowerColor1;
			float2 _NoisePannerSpeed;
			float _ColorBlendStart;
			float _SlopeCorrectionMagnitude;
			float _SlopeCorrectionOffset;
			float _DBHorizontalMaxRadius;
			float _DBHorizontalPhase;
			float _DBHorizontalFrequency;
			float _DBHorizontalAmplitude;
			float _MBMaxHeight;
			float _MBDefaultBending;
			float _MBPhase;
			float _MBFrequencyOffset;
			float _MBFrequency;
			float _MBAmplitudeOffset;
			float _MBAmplitude;
			float _MBWindDirOffset;
			float _MBWindDirBlend;
			float _ColorBlendEnd;
			float _MBWindDir;
			#ifdef ASE_TRANSMISSION
				float _TransmissionShadow;
			#endif
			#ifdef ASE_TRANSLUCENCY
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			#ifdef SCENEPICKINGPASS
				float4 _SelectionID;
			#endif

			#ifdef SCENESELECTIONPASS
				int _ObjectId;
				int _PassValue;
			#endif

			float MBGlobalWindDir;
			sampler2D _NoiseTexture;
			sampler2D _MainTex;
			sampler2D _StemTexture;


			float3 RotateAroundAxis( float3 center, float3 original, float3 u, float angle )
			{
				original -= center;
				float C = cos( angle );
				float S = sin( angle );
				float t = 1 - C;
				float m00 = t * u.x * u.x + C;
				float m01 = t * u.x * u.y - S * u.z;
				float m02 = t * u.x * u.z + S * u.y;
				float m10 = t * u.x * u.y + S * u.z;
				float m11 = t * u.y * u.y + C;
				float m12 = t * u.y * u.z - S * u.x;
				float m20 = t * u.x * u.z - S * u.y;
				float m21 = t * u.y * u.z + S * u.x;
				float m22 = t * u.z * u.z + C;
				float3x3 finalMatrix = float3x3( m00, m01, m02, m10, m11, m12, m20, m21, m22 );
				return mul( finalMatrix, original ) + center;
			}
			

			struct SurfaceDescription
			{
				float Alpha;
				float AlphaClipThreshold;
			};

			PackedVaryings VertexFunction( Attributes input  )
			{
				PackedVaryings output;
				ZERO_INITIALIZE(PackedVaryings, output);

				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				float lerpResult1811 = lerp( _MBWindDir , MBGlobalWindDir , _MBWindDirBlend);
				float MB_WindDirection870 = lerpResult1811;
				float MB_WindDirectionOffset1373 = _MBWindDirOffset;
				float2 texCoord1502 = input.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float3 appendResult1503 = (float3(texCoord1502.x , 0.0 , texCoord1502.y));
				float3 LocalPivot1504 = -appendResult1503;
				float3 objToWorld11_g99 = mul( GetObjectToWorldMatrix(), float4( LocalPivot1504, 1 ) ).xyz;
				float2 appendResult10_g99 = (float2(objToWorld11_g99.x , objToWorld11_g99.z));
				float2 UVs27_g114 = appendResult10_g99;
				float4 temp_output_24_0_g114 = _NoiseTextureTilling;
				float2 AnimatedNoiseTilling29_g114 = (temp_output_24_0_g114).zw;
				float2 panner7_g114 = ( 0.1 * _Time.y * _NoisePannerSpeed + float2( 0,0 ));
				float4 AnimatedNoise1344 = tex2Dlod( _NoiseTexture, float4( ( ( UVs27_g114 * AnimatedNoiseTilling29_g114 ) + panner7_g114 ), 0, 0.0) );
				float temp_output_11_0_g130 = radians( ( ( MB_WindDirection870 + ( MB_WindDirectionOffset1373 *  (-1.0 + ( (AnimatedNoise1344).x - 0.0 ) * ( 1.0 - -1.0 ) / ( 1.0 - 0.0 ) ) ) ) * -1.0 ) );
				float3 appendResult14_g130 = (float3(cos( temp_output_11_0_g130 ) , 0.0 , sin( temp_output_11_0_g130 )));
				float3 worldToObj35_g130 = mul( GetWorldToObjectMatrix(), float4( appendResult14_g130, 1 ) ).xyz;
				float3 worldToObj36_g130 = mul( GetWorldToObjectMatrix(), float4( float3( 0,0,0 ), 1 ) ).xyz;
				float3 normalizeResult34_g130 = normalize( (( worldToObj35_g130 - worldToObj36_g130 )).xyz );
				float3 MB_RotationAxis1420 = normalizeResult34_g130;
				float3 RotationAxis56_g131 = MB_RotationAxis1420;
				float MB_Amplitude880 = _MBAmplitude;
				float MB_AmplitudeOffset1356 = _MBAmplitudeOffset;
				float2 StaticNoileTilling28_g114 = (temp_output_24_0_g114).xy;
				float4 StaticNoise1340 = tex2Dlod( _NoiseTexture, float4( ( UVs27_g114 * StaticNoileTilling28_g114 ), 0, 0.0) );
				float4 StaticWorldNoise31_g129 = StaticNoise1340;
				float3 objToWorld52_g129 = mul( GetObjectToWorldMatrix(), float4( float3( 0,0,0 ), 1 ) ).xyz;
				float MB_Frequency873 = _MBFrequency;
				float MB_FrequencyOffset1525 = _MBFrequencyOffset;
				float DB_PhaseShift928 = input.ase_color.a;
				float MB_Phase1360 = _MBPhase;
				float MB_DefaultBending877 = _MBDefaultBending;
				float MB_MaxHeight1335 = _MBMaxHeight;
				float MB_RotationAngle97 = radians( ( ( ( ( MB_Amplitude880 + ( MB_AmplitudeOffset1356 * (StaticWorldNoise31_g129).x ) ) * sin( ( ( ( objToWorld52_g129.x + objToWorld52_g129.z ) + ( ( ( _TimeParameters.x ) * ( MB_Frequency873 + ( MB_FrequencyOffset1525 * (StaticWorldNoise31_g129).x ) ) ) + ( ( 2.0 * PI ) * DB_PhaseShift928 ) ) ) * MB_Phase1360 ) ) ) + MB_DefaultBending877 ) * ( input.positionOS.xyz.y / MB_MaxHeight1335 ) ) );
				float RotationAngle54_g131 = MB_RotationAngle97;
				float3 PivotPoint60_g131 = LocalPivot1504;
				float3 break62_g131 = PivotPoint60_g131;
				float3 appendResult45_g131 = (float3(break62_g131.x , input.positionOS.xyz.y , break62_g131.z));
				float3 rotatedValue30_g131 = RotateAroundAxis( appendResult45_g131, input.positionOS.xyz, RotationAxis56_g131, RotationAngle54_g131 );
				float DB_HorizontalAmplitude1221 = _DBHorizontalAmplitude;
				float DB_HorizontalFrequency1219 = _DBHorizontalFrequency;
				float Frequency41_g115 = DB_HorizontalFrequency1219;
				float3 objToWorld53_g115 = mul( GetObjectToWorldMatrix(), float4( float3( 0,0,0 ), 1 ) ).xyz;
				float DB_HorizontalPhase1401 = _DBHorizontalPhase;
				float3 PivotPoint49_g115 = LocalPivot1504;
				float3 break52_g115 = PivotPoint49_g115;
				float3 appendResult20_g115 = (float3(break52_g115.x , input.positionOS.xyz.y , break52_g115.z));
				float DB_HorizontalMaxRadius1403 = _DBHorizontalMaxRadius;
				float3 rotatedValue33_g115 = RotateAroundAxis( PivotPoint49_g115, input.positionOS.xyz, float3( 0, 1, 0 ), radians( ( ( DB_HorizontalAmplitude1221 * sin( ( ( ( ( _TimeParameters.x ) * Frequency41_g115 ) - ( ( 2.0 * PI ) * ( 1.0 - DB_PhaseShift928 ) ) ) + ( ( ( objToWorld53_g115.x + objToWorld53_g115.z ) + ( ( _TimeParameters.x ) * Frequency41_g115 ) ) * DB_HorizontalPhase1401 ) ) ) ) * ( distance( input.positionOS.xyz , appendResult20_g115 ) / DB_HorizontalMaxRadius1403 ) ) ) );
				#ifdef _ENABLEHORIZONTALBENDING_ON
				float3 staticSwitch1214 = ( ( rotatedValue33_g115 - input.positionOS.xyz ) * 1.0 );
				#else
				float3 staticSwitch1214 = float3( 0, 0, 0 );
				#endif
				float3 DB_VertexOffset769 = staticSwitch1214;
				float3 rotatedValue34_g131 = RotateAroundAxis( PivotPoint60_g131, ( rotatedValue30_g131 + DB_VertexOffset769 ), RotationAxis56_g131, RotationAngle54_g131 );
				float3 temp_output_1533_0 = ( ( rotatedValue34_g131 - input.positionOS.xyz ) * step( 0.01 , input.positionOS.xyz.y ) );
				float3 MainBending89_g158 = temp_output_1533_0;
				float3 appendResult15_g158 = (float3(0.0 , 1.0 , 0.0));
				float3 objToWorld98_g158 = mul( GetObjectToWorldMatrix(), float4( appendResult15_g158, 1 ) ).xyz;
				float3 objToWorld102_g158 = mul( GetObjectToWorldMatrix(), float4( float3( 0,0,0 ), 1 ) ).xyz;
				float3 break20_g158 = ( objToWorld98_g158 - objToWorld102_g158 );
				float3 appendResult24_g158 = (float3(-break20_g158.z , 0.0 , break20_g158.x));
				float3 appendResult3_g158 = (float3(0.0 , 1.0 , 0.0));
				float3 objToWorld100_g158 = mul( GetObjectToWorldMatrix(), float4( appendResult3_g158, 1 ) ).xyz;
				float3 objToWorld106_g158 = mul( GetObjectToWorldMatrix(), float4( float3( 0,0,0 ), 1 ) ).xyz;
				float3 temp_output_107_0_g158 = ( objToWorld100_g158 - objToWorld106_g158 );
				float3 break108_g158 = temp_output_107_0_g158;
				float3 lerpResult84_g158 = lerp( float3( 0, 1, 0 ) , temp_output_107_0_g158 , step( 0.001 , ( abs( break108_g158.x ) + abs( break108_g158.z ) ) ));
				float3 normalizeResult7_g158 = normalize( lerpResult84_g158 );
				float dotResult9_g158 = dot( normalizeResult7_g158 , float3( 0, 1, 0 ) );
				float temp_output_12_0_g158 = acos( dotResult9_g158 );
				float NaNPrevention21_g158 = step( 0.01 , abs( ( temp_output_12_0_g158 * ( 180.0 / PI ) ) ) );
				float3 lerpResult26_g158 = lerp( float3( 1, 0, 0 ) , appendResult24_g158 , NaNPrevention21_g158);
				float3 worldToObj99_g158 = mul( GetWorldToObjectMatrix(), float4( lerpResult26_g158, 1 ) ).xyz;
				float3 worldToObj105_g158 = mul( GetWorldToObjectMatrix(), float4( float3( 0,0,0 ), 1 ) ).xyz;
				float3 normalizeResult49_g158 = normalize( ( worldToObj99_g158 - worldToObj105_g158 ) );
				float3 RotationAxis30_g158 = normalizeResult49_g158;
				float SlopeCorrectionOffset1805 = _SlopeCorrectionOffset;
				float SlopeCorrectionMagnitude1775 = _SlopeCorrectionMagnitude;
				float RotationAngle29_g158 = ( saturate( (  (0.0 + ( (StaticNoise1340).x - 0.0 ) * ( SlopeCorrectionOffset1805 - 0.0 ) / ( 1.0 - 0.0 ) ) + SlopeCorrectionMagnitude1775 ) ) * temp_output_12_0_g158 );
				float3 rotatedValue35_g158 = RotateAroundAxis( LocalPivot1504, ( input.positionOS.xyz + MainBending89_g158 ), RotationAxis30_g158, RotationAngle29_g158 );
				float3 lerpResult52_g158 = lerp( MainBending89_g158 , ( rotatedValue35_g158 - input.positionOS.xyz ) , NaNPrevention21_g158);
				#ifdef _ENABLESLOPECORRECTION_ON
				float3 staticSwitch1781 = lerpResult52_g158;
				#else
				float3 staticSwitch1781 = temp_output_1533_0;
				#endif
				float3 LocalVertexOffset1045 = staticSwitch1781;
				
				output.ase_texcoord1.xy = input.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord1.zw = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = LocalVertexOffset1045;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					input.positionOS.xyz = vertexValue;
				#else
					input.positionOS.xyz += vertexValue;
				#endif

				input.normalOS = input.normalOS;

				VertexPositionInputs vertexInput = GetVertexPositionInputs( input.positionOS.xyz );

				output.positionCS = vertexInput.positionCS;
				output.positionWS = vertexInput.positionWS;
				return output;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 positionOS : INTERNALTESSPOS;
				half3 normalOS : NORMAL;
				half4 tangentOS : TANGENT;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( Attributes input )
			{
				VertexControl output;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				output.positionOS = input.positionOS;
				output.normalOS = input.normalOS;
				output.tangentOS = input.tangentOS;
				output.ase_texcoord1 = input.ase_texcoord1;
				output.ase_color = input.ase_color;
				output.ase_texcoord = input.ase_texcoord;
				return output;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> input)
			{
				TessellationFactors output;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				output.edge[0] = tf.x; output.edge[1] = tf.y; output.edge[2] = tf.z; output.inside = tf.w;
				return output;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			PackedVaryings DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				Attributes output = (Attributes) 0;
				output.positionOS = patch[0].positionOS * bary.x + patch[1].positionOS * bary.y + patch[2].positionOS * bary.z;
				output.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				output.tangentOS = patch[0].tangentOS * bary.x + patch[1].tangentOS * bary.y + patch[2].tangentOS * bary.z;
				output.ase_texcoord1 = patch[0].ase_texcoord1 * bary.x + patch[1].ase_texcoord1 * bary.y + patch[2].ase_texcoord1 * bary.z;
				output.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				output.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = output.positionOS.xyz - patch[i].normalOS * (dot(output.positionOS.xyz, patch[i].normalOS) - dot(patch[i].positionOS.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				output.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * output.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], output);
				return VertexFunction(output);
			}
			#else
			PackedVaryings vert ( Attributes input )
			{
				return VertexFunction( input );
			}
			#endif

			half4 frag( PackedVaryings input
				#if defined( ASE_DEPTH_WRITE_ON )
				,out float outputDepth : ASE_SV_DEPTH
				#endif
				 ) : SV_Target
			{
				SurfaceDescription surfaceDescription = (SurfaceDescription)0;

				float3 PositionWS = input.positionWS;
				float3 PositionRWS = GetCameraRelativePositionWS( PositionWS );
				float4 ScreenPosNorm = float4( GetNormalizedScreenSpaceUV( input.positionCS ), input.positionCS.zw );
				float4 ClipPos = ComputeClipSpacePosition( ScreenPosNorm.xy, input.positionCS.z ) * input.positionCS.w;

				float2 uv_MainTex1481 = input.ase_texcoord1.xy;
				float4 tex2DNode1481 = tex2D( _MainTex, uv_MainTex1481 );
				float2 uv_StemTexture1482 = input.ase_texcoord1.xy;
				float4 tex2DNode1482 = tex2D( _StemTexture, uv_StemTexture1482 );
				float TextureMask1458 = step( 1.5 , input.ase_texcoord1.xy.x );
				float lerpResult1492 = lerp( tex2DNode1481.a , tex2DNode1482.a , TextureMask1458);
				float Opacity1494 = lerpResult1492;
				

				surfaceDescription.Alpha = Opacity1494;
				surfaceDescription.AlphaClipThreshold = 0.5;

				#if defined( ASE_DEPTH_WRITE_ON )
					float DeviceDepth = input.positionCS.z;
				#endif

				#if _ALPHATEST_ON
					float alphaClipThreshold = 0.01f;
					#if ALPHA_CLIP_THRESHOLD
						alphaClipThreshold = surfaceDescription.AlphaClipThreshold;
					#endif
						clip(surfaceDescription.Alpha - alphaClipThreshold);
				#endif

				#if defined( ASE_DEPTH_WRITE_ON )
					outputDepth = DeviceDepth;
				#endif

				return _SelectionID;
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "MotionVectors"
			Tags { "LightMode"="MotionVectors" }

			ColorMask RG

			HLSLPROGRAM

			#pragma multi_compile_local _ALPHATEST_ON
			#define _NORMAL_DROPOFF_TS 1
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#define ASE_FOG 1
			#define ASE_VERSION 19900
			#define ASE_SRP_VERSION 170003


			#pragma vertex vert
			#pragma fragment frag

			#if defined(_SPECULAR_SETUP) && defined(ASE_LIGHTING_SIMPLE)
				#define _SPECULAR_COLOR 1
			#endif

            #define SHADERPASS SHADERPASS_MOTION_VECTORS

            #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"
		    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
		    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
		    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
		    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
		    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
		    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
		    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
		    #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#if defined(LOD_FADE_CROSSFADE)
				#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/LODCrossFade.hlsl"
			#endif

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/MotionVectorsCommon.hlsl"

			#define ASE_NEEDS_TEXTURE_COORDINATES1
			#define ASE_NEEDS_VERT_POSITION
			#define ASE_NEEDS_TEXTURE_COORDINATES0
			#define ASE_NEEDS_FRAG_TEXTURE_COORDINATES0
			#pragma shader_feature_local _ENABLESLOPECORRECTION_ON
			#pragma shader_feature_local _ENABLEHORIZONTALBENDING_ON


			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE) && (SHADER_TARGET >= 45)
				#define ASE_SV_DEPTH SV_DepthLessEqual
				#define ASE_SV_POSITION_QUALIFIERS linear noperspective centroid
			#else
				#define ASE_SV_DEPTH SV_Depth
				#define ASE_SV_POSITION_QUALIFIERS
			#endif

			struct Attributes
			{
				float4 positionOS : POSITION;
				float3 positionOld : TEXCOORD4;
				#if _ADD_PRECOMPUTED_VELOCITY
					float3 alembicMotionVector : TEXCOORD5;
				#endif
				half3 normalOS : NORMAL;
				half4 tangentOS : TANGENT;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryings
			{
				float4 positionCS : SV_POSITION;
				float4 positionCSNoJitter : TEXCOORD0;
				float4 previousPositionCSNoJitter : TEXCOORD1;
				float3 positionWS : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _StemColor;
			float4 _FlowerColor2;
			float4 _NoiseTextureTilling;
			float4 _FlowerColor1;
			float2 _NoisePannerSpeed;
			float _ColorBlendStart;
			float _SlopeCorrectionMagnitude;
			float _SlopeCorrectionOffset;
			float _DBHorizontalMaxRadius;
			float _DBHorizontalPhase;
			float _DBHorizontalFrequency;
			float _DBHorizontalAmplitude;
			float _MBMaxHeight;
			float _MBDefaultBending;
			float _MBPhase;
			float _MBFrequencyOffset;
			float _MBFrequency;
			float _MBAmplitudeOffset;
			float _MBAmplitude;
			float _MBWindDirOffset;
			float _MBWindDirBlend;
			float _ColorBlendEnd;
			float _MBWindDir;
			#ifdef ASE_TRANSMISSION
				float _TransmissionShadow;
			#endif
			#ifdef ASE_TRANSLUCENCY
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			#ifdef SCENEPICKINGPASS
				float4 _SelectionID;
			#endif

			#ifdef SCENESELECTIONPASS
				int _ObjectId;
				int _PassValue;
			#endif

			float MBGlobalWindDir;
			sampler2D _NoiseTexture;
			sampler2D _MainTex;
			sampler2D _StemTexture;


			float3 RotateAroundAxis( float3 center, float3 original, float3 u, float angle )
			{
				original -= center;
				float C = cos( angle );
				float S = sin( angle );
				float t = 1 - C;
				float m00 = t * u.x * u.x + C;
				float m01 = t * u.x * u.y - S * u.z;
				float m02 = t * u.x * u.z + S * u.y;
				float m10 = t * u.x * u.y + S * u.z;
				float m11 = t * u.y * u.y + C;
				float m12 = t * u.y * u.z - S * u.x;
				float m20 = t * u.x * u.z - S * u.y;
				float m21 = t * u.y * u.z + S * u.x;
				float m22 = t * u.z * u.z + C;
				float3x3 finalMatrix = float3x3( m00, m01, m02, m10, m11, m12, m20, m21, m22 );
				return mul( finalMatrix, original ) + center;
			}
			

			PackedVaryings VertexFunction( Attributes input  )
			{
				PackedVaryings output = (PackedVaryings)0;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				float lerpResult1811 = lerp( _MBWindDir , MBGlobalWindDir , _MBWindDirBlend);
				float MB_WindDirection870 = lerpResult1811;
				float MB_WindDirectionOffset1373 = _MBWindDirOffset;
				float2 texCoord1502 = input.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float3 appendResult1503 = (float3(texCoord1502.x , 0.0 , texCoord1502.y));
				float3 LocalPivot1504 = -appendResult1503;
				float3 objToWorld11_g99 = mul( GetObjectToWorldMatrix(), float4( LocalPivot1504, 1 ) ).xyz;
				float2 appendResult10_g99 = (float2(objToWorld11_g99.x , objToWorld11_g99.z));
				float2 UVs27_g114 = appendResult10_g99;
				float4 temp_output_24_0_g114 = _NoiseTextureTilling;
				float2 AnimatedNoiseTilling29_g114 = (temp_output_24_0_g114).zw;
				float2 panner7_g114 = ( 0.1 * _Time.y * _NoisePannerSpeed + float2( 0,0 ));
				float4 AnimatedNoise1344 = tex2Dlod( _NoiseTexture, float4( ( ( UVs27_g114 * AnimatedNoiseTilling29_g114 ) + panner7_g114 ), 0, 0.0) );
				float temp_output_11_0_g130 = radians( ( ( MB_WindDirection870 + ( MB_WindDirectionOffset1373 *  (-1.0 + ( (AnimatedNoise1344).x - 0.0 ) * ( 1.0 - -1.0 ) / ( 1.0 - 0.0 ) ) ) ) * -1.0 ) );
				float3 appendResult14_g130 = (float3(cos( temp_output_11_0_g130 ) , 0.0 , sin( temp_output_11_0_g130 )));
				float3 worldToObj35_g130 = mul( GetWorldToObjectMatrix(), float4( appendResult14_g130, 1 ) ).xyz;
				float3 worldToObj36_g130 = mul( GetWorldToObjectMatrix(), float4( float3( 0,0,0 ), 1 ) ).xyz;
				float3 normalizeResult34_g130 = normalize( (( worldToObj35_g130 - worldToObj36_g130 )).xyz );
				float3 MB_RotationAxis1420 = normalizeResult34_g130;
				float3 RotationAxis56_g131 = MB_RotationAxis1420;
				float MB_Amplitude880 = _MBAmplitude;
				float MB_AmplitudeOffset1356 = _MBAmplitudeOffset;
				float2 StaticNoileTilling28_g114 = (temp_output_24_0_g114).xy;
				float4 StaticNoise1340 = tex2Dlod( _NoiseTexture, float4( ( UVs27_g114 * StaticNoileTilling28_g114 ), 0, 0.0) );
				float4 StaticWorldNoise31_g129 = StaticNoise1340;
				float3 objToWorld52_g129 = mul( GetObjectToWorldMatrix(), float4( float3( 0,0,0 ), 1 ) ).xyz;
				float MB_Frequency873 = _MBFrequency;
				float MB_FrequencyOffset1525 = _MBFrequencyOffset;
				float DB_PhaseShift928 = input.ase_color.a;
				float MB_Phase1360 = _MBPhase;
				float MB_DefaultBending877 = _MBDefaultBending;
				float MB_MaxHeight1335 = _MBMaxHeight;
				float MB_RotationAngle97 = radians( ( ( ( ( MB_Amplitude880 + ( MB_AmplitudeOffset1356 * (StaticWorldNoise31_g129).x ) ) * sin( ( ( ( objToWorld52_g129.x + objToWorld52_g129.z ) + ( ( ( _TimeParameters.x ) * ( MB_Frequency873 + ( MB_FrequencyOffset1525 * (StaticWorldNoise31_g129).x ) ) ) + ( ( 2.0 * PI ) * DB_PhaseShift928 ) ) ) * MB_Phase1360 ) ) ) + MB_DefaultBending877 ) * ( input.positionOS.xyz.y / MB_MaxHeight1335 ) ) );
				float RotationAngle54_g131 = MB_RotationAngle97;
				float3 PivotPoint60_g131 = LocalPivot1504;
				float3 break62_g131 = PivotPoint60_g131;
				float3 appendResult45_g131 = (float3(break62_g131.x , input.positionOS.xyz.y , break62_g131.z));
				float3 rotatedValue30_g131 = RotateAroundAxis( appendResult45_g131, input.positionOS.xyz, RotationAxis56_g131, RotationAngle54_g131 );
				float DB_HorizontalAmplitude1221 = _DBHorizontalAmplitude;
				float DB_HorizontalFrequency1219 = _DBHorizontalFrequency;
				float Frequency41_g115 = DB_HorizontalFrequency1219;
				float3 objToWorld53_g115 = mul( GetObjectToWorldMatrix(), float4( float3( 0,0,0 ), 1 ) ).xyz;
				float DB_HorizontalPhase1401 = _DBHorizontalPhase;
				float3 PivotPoint49_g115 = LocalPivot1504;
				float3 break52_g115 = PivotPoint49_g115;
				float3 appendResult20_g115 = (float3(break52_g115.x , input.positionOS.xyz.y , break52_g115.z));
				float DB_HorizontalMaxRadius1403 = _DBHorizontalMaxRadius;
				float3 rotatedValue33_g115 = RotateAroundAxis( PivotPoint49_g115, input.positionOS.xyz, float3( 0, 1, 0 ), radians( ( ( DB_HorizontalAmplitude1221 * sin( ( ( ( ( _TimeParameters.x ) * Frequency41_g115 ) - ( ( 2.0 * PI ) * ( 1.0 - DB_PhaseShift928 ) ) ) + ( ( ( objToWorld53_g115.x + objToWorld53_g115.z ) + ( ( _TimeParameters.x ) * Frequency41_g115 ) ) * DB_HorizontalPhase1401 ) ) ) ) * ( distance( input.positionOS.xyz , appendResult20_g115 ) / DB_HorizontalMaxRadius1403 ) ) ) );
				#ifdef _ENABLEHORIZONTALBENDING_ON
				float3 staticSwitch1214 = ( ( rotatedValue33_g115 - input.positionOS.xyz ) * 1.0 );
				#else
				float3 staticSwitch1214 = float3( 0, 0, 0 );
				#endif
				float3 DB_VertexOffset769 = staticSwitch1214;
				float3 rotatedValue34_g131 = RotateAroundAxis( PivotPoint60_g131, ( rotatedValue30_g131 + DB_VertexOffset769 ), RotationAxis56_g131, RotationAngle54_g131 );
				float3 temp_output_1533_0 = ( ( rotatedValue34_g131 - input.positionOS.xyz ) * step( 0.01 , input.positionOS.xyz.y ) );
				float3 MainBending89_g158 = temp_output_1533_0;
				float3 appendResult15_g158 = (float3(0.0 , 1.0 , 0.0));
				float3 objToWorld98_g158 = mul( GetObjectToWorldMatrix(), float4( appendResult15_g158, 1 ) ).xyz;
				float3 objToWorld102_g158 = mul( GetObjectToWorldMatrix(), float4( float3( 0,0,0 ), 1 ) ).xyz;
				float3 break20_g158 = ( objToWorld98_g158 - objToWorld102_g158 );
				float3 appendResult24_g158 = (float3(-break20_g158.z , 0.0 , break20_g158.x));
				float3 appendResult3_g158 = (float3(0.0 , 1.0 , 0.0));
				float3 objToWorld100_g158 = mul( GetObjectToWorldMatrix(), float4( appendResult3_g158, 1 ) ).xyz;
				float3 objToWorld106_g158 = mul( GetObjectToWorldMatrix(), float4( float3( 0,0,0 ), 1 ) ).xyz;
				float3 temp_output_107_0_g158 = ( objToWorld100_g158 - objToWorld106_g158 );
				float3 break108_g158 = temp_output_107_0_g158;
				float3 lerpResult84_g158 = lerp( float3( 0, 1, 0 ) , temp_output_107_0_g158 , step( 0.001 , ( abs( break108_g158.x ) + abs( break108_g158.z ) ) ));
				float3 normalizeResult7_g158 = normalize( lerpResult84_g158 );
				float dotResult9_g158 = dot( normalizeResult7_g158 , float3( 0, 1, 0 ) );
				float temp_output_12_0_g158 = acos( dotResult9_g158 );
				float NaNPrevention21_g158 = step( 0.01 , abs( ( temp_output_12_0_g158 * ( 180.0 / PI ) ) ) );
				float3 lerpResult26_g158 = lerp( float3( 1, 0, 0 ) , appendResult24_g158 , NaNPrevention21_g158);
				float3 worldToObj99_g158 = mul( GetWorldToObjectMatrix(), float4( lerpResult26_g158, 1 ) ).xyz;
				float3 worldToObj105_g158 = mul( GetWorldToObjectMatrix(), float4( float3( 0,0,0 ), 1 ) ).xyz;
				float3 normalizeResult49_g158 = normalize( ( worldToObj99_g158 - worldToObj105_g158 ) );
				float3 RotationAxis30_g158 = normalizeResult49_g158;
				float SlopeCorrectionOffset1805 = _SlopeCorrectionOffset;
				float SlopeCorrectionMagnitude1775 = _SlopeCorrectionMagnitude;
				float RotationAngle29_g158 = ( saturate( (  (0.0 + ( (StaticNoise1340).x - 0.0 ) * ( SlopeCorrectionOffset1805 - 0.0 ) / ( 1.0 - 0.0 ) ) + SlopeCorrectionMagnitude1775 ) ) * temp_output_12_0_g158 );
				float3 rotatedValue35_g158 = RotateAroundAxis( LocalPivot1504, ( input.positionOS.xyz + MainBending89_g158 ), RotationAxis30_g158, RotationAngle29_g158 );
				float3 lerpResult52_g158 = lerp( MainBending89_g158 , ( rotatedValue35_g158 - input.positionOS.xyz ) , NaNPrevention21_g158);
				#ifdef _ENABLESLOPECORRECTION_ON
				float3 staticSwitch1781 = lerpResult52_g158;
				#else
				float3 staticSwitch1781 = temp_output_1533_0;
				#endif
				float3 LocalVertexOffset1045 = staticSwitch1781;
				
				output.ase_texcoord3.xy = input.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord3.zw = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = LocalVertexOffset1045;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					input.positionOS.xyz = vertexValue;
				#else
					input.positionOS.xyz += vertexValue;
				#endif

				VertexPositionInputs vertexInput = GetVertexPositionInputs( input.positionOS.xyz );

				#if defined(APLICATION_SPACE_WARP_MOTION)
					output.positionCSNoJitter = mul(_NonJitteredViewProjMatrix, mul(UNITY_MATRIX_M, input.positionOS));
					output.positionCS = output.positionCSNoJitter;
				#else
					output.positionCS = vertexInput.positionCS;
					output.positionCSNoJitter = mul(_NonJitteredViewProjMatrix, mul(UNITY_MATRIX_M, input.positionOS));
				#endif

				float4 prevPos = ( unity_MotionVectorsParams.x == 1 ) ? float4( input.positionOld, 1 ) : input.positionOS;

				#if _ADD_PRECOMPUTED_VELOCITY
					prevPos = prevPos - float4(input.alembicMotionVector, 0);
				#endif

				output.previousPositionCSNoJitter = mul( _PrevViewProjMatrix, mul( UNITY_PREV_MATRIX_M, prevPos ) );

				output.positionWS = vertexInput.positionWS;

				// removed in ObjectMotionVectors.hlsl found in unity 6000.0.23 and higher
				//ApplyMotionVectorZBias( output.positionCS );
				return output;
			}

			PackedVaryings vert ( Attributes input )
			{
				return VertexFunction( input );
			}

			half4 frag(	PackedVaryings input
				#if defined( ASE_DEPTH_WRITE_ON )
				,out float outputDepth : ASE_SV_DEPTH
				#endif
				 ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( input );

				float3 PositionWS = input.positionWS;
				float3 PositionRWS = GetCameraRelativePositionWS( PositionWS );
				float4 ScreenPosNorm = float4( GetNormalizedScreenSpaceUV( input.positionCS ), input.positionCS.zw );
				float4 ClipPos = ComputeClipSpacePosition( ScreenPosNorm.xy, input.positionCS.z ) * input.positionCS.w;

				float2 uv_MainTex1481 = input.ase_texcoord3.xy;
				float4 tex2DNode1481 = tex2D( _MainTex, uv_MainTex1481 );
				float2 uv_StemTexture1482 = input.ase_texcoord3.xy;
				float4 tex2DNode1482 = tex2D( _StemTexture, uv_StemTexture1482 );
				float TextureMask1458 = step( 1.5 , input.ase_texcoord3.xy.x );
				float lerpResult1492 = lerp( tex2DNode1481.a , tex2DNode1482.a , TextureMask1458);
				float Opacity1494 = lerpResult1492;
				

				float Alpha = Opacity1494;
				float AlphaClipThreshold = 0.5;

				#if defined( ASE_DEPTH_WRITE_ON )
					float DeviceDepth = input.positionCS.z;
				#endif

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				#if defined(ASE_CHANGES_WORLD_POS)
					float3 positionOS = mul( GetWorldToObjectMatrix(),  float4( PositionWS, 1.0 ) ).xyz;
					float3 previousPositionWS = mul( GetPrevObjectToWorldMatrix(),  float4( positionOS, 1.0 ) ).xyz;
					input.positionCSNoJitter = mul( _NonJitteredViewProjMatrix, float4( PositionWS, 1.0 ) );
					input.previousPositionCSNoJitter = mul( _PrevViewProjMatrix, float4( previousPositionWS, 1.0 ) );
				#endif

				#if defined(LOD_FADE_CROSSFADE)
					LODFadeCrossFade( input.positionCS );
				#endif

				#if defined( ASE_DEPTH_WRITE_ON )
					outputDepth = DeviceDepth;
				#endif

				#if defined(APLICATION_SPACE_WARP_MOTION)
					return float4( CalcAswNdcMotionVectorFromCsPositions( input.positionCSNoJitter, input.previousPositionCSNoJitter ), 1 );
				#else
					return float4( CalcNdcMotionVectorFromCsPositions( input.positionCSNoJitter, input.previousPositionCSNoJitter ), 0, 0 );
				#endif
			}
			ENDHLSL
		}

	
	}
	
	CustomEditor "UnityEditor.ShaderGraphLitGUI"
	FallBack "Hidden/Shader Graph/FallbackError"
	
	Fallback Off
}

/*ASEBEGIN
Version=19900
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;490;-2426.842,3970.215;Inherit;False;889.0664;633.0813;;10;1458;1457;1455;1456;928;738;1504;1503;1502;1817;Vertex Colors and UVs Baked Data;1,1,1,1;0;0
Node;AmplifyShaderEditor.TextureCoordinatesNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1502;-2398.06,4442.469;Inherit;False;1;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1503;-2141.731,4449.933;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.NegateNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1817;-1946.465,4448.778;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1504;-1770.731,4442.933;Inherit;False;LocalPivot;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1343;-3074.098,2178.515;Inherit;False;1405.583;634.9444;;8;1500;1536;1340;1344;1784;1815;1538;1814;World Space Noise;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;866;-4856.785,2299.094;Inherit;False;1526.371;1405.004;;38;870;1811;1810;1809;1373;952;1805;1804;1335;1334;1506;1505;1534;1775;1774;880;877;873;1360;1356;1525;1262;1524;850;300;1286;480;687;1403;1221;1219;1401;1218;1308;1220;1249;1470;1812;Material Properties;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1500;-2988.906,2216.792;Inherit;False;1504;LocalPivot;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.Vector4Node, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1815;-2824.401,2509.249;Inherit;False;Property;_NoiseTextureTilling;Noise Tilling - Static (XY), Animated (ZW);27;0;Create;False;0;0;0;False;0;False;1,1,1,1;1,1,1,1;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1536;-2774.618,2221.383;Inherit;False;WorldSpaceUVs - NHP;-1;;99;88a2e8a391a04e241878bdb87d9283a3;0;1;6;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1814;-2754.401,2682.25;Float;False;Property;_NoisePannerSpeed;Noise Panner Speed;28;0;Create;True;0;0;0;False;0;False;0.05,0.03;0.08,0.1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1810;-4809.595,3300.19;Inherit;False;Property;_MBWindDirBlend;MB Wind Dir Blend;16;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1308;-4030.775,2579.569;Float;False;Property;_DBHorizontalPhase;DB Horizontal Phase;21;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1538;-2755.761,2310.993;Inherit;True;Property;_NoiseTexture;Noise Texture;26;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;None;512fa11ad89d84543ad8d6c8d9cb6743;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.VertexColorNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;738;-2387.048,4032.784;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1218;-4032.775,2483.569;Float;False;Property;_DBHorizontalFrequency;DB Horizontal Frequency;20;0;Create;True;0;0;0;False;0;False;1.16;3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1809;-4807.595,3212.19;Inherit;False;Global;MBGlobalWindDir;MB Global Wind Dir;28;1;[HideInInspector];Create;True;0;0;0;False;0;False;0;0;0;360;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;850;-4810.689,3122.742;Float;False;Property;_MBWindDir;MB Wind Dir;14;0;Create;True;0;0;0;False;0;False;0;0;0;360;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1249;-4032.775,2675.569;Float;False;Property;_DBHorizontalMaxRadius;DB Horizontal Max Radius;22;0;Create;True;0;0;0;False;0;False;0.05;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1220;-4032.775,2386.569;Float;False;Property;_DBHorizontalAmplitude;DB Horizontal Amplitude;19;0;Create;True;0;0;0;False;0;False;2;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1524;-4811.99,2929.007;Inherit;False;Property;_MBFrequencyOffset;MB Frequency Offset;12;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1784;-2455.037,2439.188;Inherit;False;WorldSpaceNoise - NHP;-1;;114;af5fa9ff24e18344ebcc05b64d296c57;0;4;22;FLOAT2;0,0;False;20;SAMPLER2D;;False;24;FLOAT4;1,1,1,1;False;19;FLOAT2;0.1,0.1;False;2;COLOR;0;COLOR;16
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1410;-3067.739,3072.701;Inherit;False;1399.019;639.176;;10;769;1214;1215;1535;1405;1406;1399;1520;1408;1400;Detail Bending;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;480;-4814.087,2834.873;Float;False;Property;_MBFrequency;MB Frequency;11;0;Create;True;0;0;0;False;0;False;1.11;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;952;-4805.979,3394.934;Float;False;Property;_MBWindDirOffset;MB Wind Dir Offset;15;0;Create;True;0;0;0;False;0;False;20;0;0;180;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;687;-4814.087,2546.874;Float;False;Property;_MBDefaultBending;MB Default Bending;8;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1334;-4808.824,3517.175;Inherit;False;Property;_MBMaxHeight;MB Max Height;17;0;Create;True;0;0;0;False;0;False;1;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1286;-4810.689,3025.742;Float;False;Property;_MBPhase;MB Phase;13;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1401;-3645.776,2580.569;Inherit;False;DB_HorizontalPhase;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1262;-4814.087,2738.873;Float;False;Property;_MBAmplitudeOffset;MB Amplitude Offset;10;0;Create;True;0;0;0;False;0;False;2;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1219;-3673.776,2486.569;Float;False;DB_HorizontalFrequency;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;928;-1961.451,4118.086;Float;False;DB_PhaseShift;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1403;-3691.776,2671.569;Inherit;False;DB_HorizontalMaxRadius;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1221;-3675.776,2386.569;Float;False;DB_HorizontalAmplitude;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;300;-4814.087,2642.873;Float;False;Property;_MBAmplitude;MB Amplitude;9;0;Create;True;0;0;0;False;0;False;1.5;1.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1811;-4496.122,3191.33;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1360;-4292.113,3025.742;Inherit;False;MB_Phase;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1408;-2999.21,3517.783;Inherit;False;1403;DB_HorizontalMaxRadius;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;873;-4295.511,2834.873;Float;False;MB_Frequency;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1344;-1920.92,2511.391;Inherit;False;AnimatedNoise;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1068;-1405.837,2435.614;Inherit;False;2939.069;1275.755;;27;1045;1781;1807;1779;1806;1780;1533;1415;1421;1413;1507;1420;97;1785;1786;1366;1370;1512;1375;1367;1369;1376;1362;1378;1526;1365;1371;Main Bending;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1520;-2902.462,3604.302;Inherit;False;1504;LocalPivot;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;880;-4287.511,2642.873;Float;False;MB_Amplitude;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1399;-3002.925,3200.148;Inherit;False;1221;DB_HorizontalAmplitude;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1405;-2974.383,3354.192;Inherit;False;1401;DB_HorizontalPhase;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1406;-2936.383,3433.192;Inherit;False;928;DB_PhaseShift;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;870;-4321.689,3187.193;Float;False;MB_WindDirection;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1400;-3003.925,3275.148;Inherit;False;1219;DB_HorizontalFrequency;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;877;-4318.511,2546.874;Float;False;MB_DefaultBending;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1335;-4289.527,3514.187;Inherit;False;MB_MaxHeight;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1356;-4327.511,2738.873;Inherit;False;MB_AmplitudeOffset;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1525;-4329.516,2929.165;Inherit;False;MB_FrequencyOffset;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1373;-4343.689,3393.742;Inherit;False;MB_WindDirectionOffset;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1340;-1921.077,2393.786;Inherit;False;StaticNoise;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1512;-1316.048,3441.396;Inherit;False;928;DB_PhaseShift;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1378;-1310.849,2689.062;Inherit;False;1344;AnimatedNoise;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.Vector3Node, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1215;-2494.941,3184.301;Float;False;Constant;_Vector2;Vector 2;27;0;Create;True;0;0;0;False;0;False;0,0,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1362;-1336.906,2862.775;Inherit;False;877;MB_DefaultBending;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1526;-1340.51,3248.244;Inherit;False;1525;MB_FrequencyOffset;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1369;-1279.582,3349.775;Inherit;False;1360;MB_Phase;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1371;-1294.582,3627.775;Inherit;False;1340;StaticNoise;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1535;-2595.002,3342.698;Inherit;False;HorizontalBending - NHP;-1;;115;0b16e2546645f904a949bfd32be36037;0;7;44;FLOAT;1;False;39;FLOAT;1;False;43;FLOAT;1;False;40;FLOAT;0;False;46;FLOAT;2;False;47;FLOAT3;0,0,0;False;45;FLOAT;1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1370;-1312.51,3539.948;Inherit;False;1335;MB_MaxHeight;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1367;-1304.582,3154.775;Inherit;False;873;MB_Frequency;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1375;-1329.849,2494.062;Inherit;False;870;MB_WindDirection;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1366;-1341.582,3057.775;Inherit;False;1356;MB_AmplitudeOffset;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1376;-1368.849,2594.062;Inherit;False;1373;MB_WindDirectionOffset;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1365;-1302.582,2958.775;Inherit;False;880;MB_Amplitude;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1786;-982.2729,3162.038;Inherit;False;RotationAngle - NHP;-1;;129;87b0b7c0fc8f1424db43b84d20c2e79b;0;9;36;FLOAT;0;False;35;FLOAT;0;False;34;FLOAT;1;False;28;FLOAT;1;False;47;FLOAT;0;False;29;FLOAT;1;False;46;FLOAT;0;False;42;FLOAT;0.1;False;27;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1214;-2265.668,3263.249;Float;False;Property;_EnableHorizontalBending;Enable Horizontal Bending ;18;0;Create;True;0;0;0;False;0;False;0;1;1;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT3;0,0,0;False;0;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT3;0,0,0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.FunctionNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1785;-987.121,2579.576;Inherit;False;RotationAxis - NHP;-1;;130;b90648f17dcc4bc449d46e8cf04564ff;0;3;20;FLOAT;0;False;19;FLOAT;0;False;18;FLOAT4;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1456;-2363.843,4294.766;Inherit;False;0;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1455;-2317.697,4212.805;Inherit;False;Constant;_Float2;Float 2;26;0;Create;True;0;0;0;False;0;False;1.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1420;-543.849,2575.062;Inherit;False;MB_RotationAxis;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;769;-1900.229,3264.676;Float;False;DB_VertexOffset;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1804;-4034.173,2886.452;Inherit;False;Property;_SlopeCorrectionOffset;Slope Correction Offset;25;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1774;-4030.819,2787.221;Inherit;False;Property;_SlopeCorrectionMagnitude;Slope Correction Magnitude;24;0;Create;True;0;0;0;False;0;False;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;97;-537.988,3159.164;Float;False;MB_RotationAngle;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1457;-2130.982,4252.969;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1415;-239.4421,3006.921;Inherit;False;769;DB_VertexOffset;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1805;-3662.326,2885.461;Inherit;False;SlopeCorrectionOffset;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1775;-3690.997,2787.021;Inherit;False;SlopeCorrectionMagnitude;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1413;-246.4321,2833.488;Inherit;False;97;MB_RotationAngle;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1421;-235.4321,2729.488;Inherit;False;1420;MB_RotationAxis;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1507;-199.0092,2912.417;Inherit;False;1504;LocalPivot;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;489;-376.9557,770.6545;Inherit;False;1907.837;1399.909;;32;1477;1488;1485;1484;1479;1480;1478;1476;1475;1474;1472;1471;1473;1469;1813;1467;1468;1466;1494;1487;1493;1491;1492;1490;1489;1486;1481;1482;1483;1465;1464;1463;Albedo;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1458;-1958.008,4248.586;Float;False;TextureMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1806;168.9883,3096.126;Inherit;False;1805;SlopeCorrectionOffset;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1779;241.8991,3287.028;Inherit;False;1504;LocalPivot;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1780;139.4043,3006.25;Inherit;False;1775;SlopeCorrectionMagnitude;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1533;64.104,2854.921;Inherit;False;MainBending - NHP;-1;;131;01dba1f3bc33e4b4fa301d2180819576;0;4;55;FLOAT3;0,0,0;False;53;FLOAT;0;False;59;FLOAT3;0,0,0;False;58;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1807;230.9883,3193.126;Inherit;False;1340;StaticNoise;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1482;-331.9526,1969.693;Inherit;True;Property;_StemTexture;Stem Texture;6;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1483;-234.4138,1872.09;Inherit;False;1458;TextureMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1481;-337.8076,1668.115;Inherit;True;Property;_MainTex;Flower Texture;5;1;[NoScaleOffset];Create;False;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.FunctionNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1855;507.5047,3057.542;Inherit;False;SlopeCorrection - NHP;-1;;158;af38de3ca0adf3c4ba9b6a3dd482959e;0;5;87;FLOAT3;0,0,0;False;42;FLOAT;1;False;92;FLOAT;0;False;93;FLOAT4;0,0,0,0;False;41;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1492;107.5862,1977.09;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1781;936.2124,2848.605;Float;False;Property;_EnableSlopeCorrection;Enable Slope Correction;23;0;Create;True;0;0;0;False;0;False;0;1;1;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT3;0,0,0;False;0;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT3;0,0,0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1045;1295.557,2849.422;Float;False;LocalVertexOffset;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1265;1923.979,2049.843;Inherit;False;631.4954;512.0168;;4;1508;296;1574;1816;Master Node;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1494;322.1372,1971.11;Float;False;Opacity;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1463;-309.8395,846.4044;Inherit;False;Constant;_Vector33;Vector 33;20;0;Create;True;0;0;0;False;0;False;0.5,0.5;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TexCoordVertexDataNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1464;-327.9157,974.8167;Inherit;False;0;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DistanceOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1465;-66.73778,910.1024;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1505;-4807.063,2352.738;Inherit;False;Property;_ColorBlendStart;Color Blend Start;2;0;Create;True;0;0;0;False;0;False;0.1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1470;-4803.965,2453.833;Inherit;False;Property;_ColorBlendEnd;Color Blend End;3;0;Create;True;0;0;0;False;0;False;0.15;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1506;-4286.782,2351.879;Inherit;False;ColorBlendStart;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1466;117.5732,903.9054;Inherit;False;DistanceToCenter;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1468;-180.3753,1225.116;Inherit;False;1506;ColorBlendStart;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1467;-189.3753,1146.116;Inherit;False;1466;DistanceToCenter;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1812;-4284.864,2453.152;Inherit;False;ColorBlendEnd;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1469;66.62472,1194.116;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1813;12.72603,1302.437;Inherit;False;1812;ColorBlendEnd;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1472;178.6248,1402.117;Inherit;False;1506;ColorBlendStart;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1473;258.6248,1242.116;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1471;146.6248,1482.117;Inherit;False;1466;DistanceToCenter;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1474;418.6248,1434.117;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1475;402.6248,1242.116;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1477;521.9259,892.152;Inherit;False;Property;_FlowerColor1;Flower Color 1;0;0;Create;True;0;0;0;False;0;False;0.7843137,0.454902,0.1411765,1;0.5754717,0.3435538,0.1465824,1;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1478;594.6249,1338.117;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1476;551.7269,1072.368;Inherit;False;Property;_FlowerColor2;Flower Color 2;1;0;Create;True;0;0;0;False;0;False;0.8980392,0.9529412,1,1;0.009433985,0.4014694,1,1;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.LerpOp, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1480;858.4489,1070.839;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1479;790.1998,1326.214;Float;False;Property;_StemColor;Stem Color;4;0;Create;True;0;0;0;False;0;False;0.3960784,0.5647059,0.1019608,1;0.8301887,0.5461104,0.3015307,1;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1484;809.6268,1516.508;Inherit;False;1458;TextureMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1486;118.5862,1735.09;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1485;1124.392,1278.601;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1488;1316.935,1272.132;Inherit;False;Color;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1487;327.5022,1731.593;Inherit;False;TextureColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1490;823.9894,1838.812;Inherit;False;1487;TextureColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1489;851.2313,1937.337;Inherit;False;1488;Color;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1491;1069.609,1881.003;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1493;1234.211,1877.149;Float;False;Albedo;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1330;-19002.03,10716.32;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1328;-19355.95,10790.4;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1534;-3836.964,3010.247;Inherit;False;Property;_AlphaCutoff;Alpha Cutoff;7;0;Create;True;0;0;0;False;0;False;0.5;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1508;2068.027,2295.639;Inherit;False;1494;Opacity;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1574;2026.197,2426.878;Inherit;False;1045;LocalVertexOffset;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1816;2091.049,2201.133;Inherit;False;Constant;_Float3;Float 3;30;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;296;2065.489,2106.808;Inherit;False;1493;Albedo;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1856;2291.679,2118.861;Float;False;False;-1;3;UnityEditor.ShaderGraphLitGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;ExtraPrePass;0;0;ExtraPrePass;6;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;5;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;AlwaysRenderMotionVectors=false;True;5;True;12;all;0;False;True;1;1;False;;0;False;;0;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;0;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1857;2291.679,2118.861;Float;False;True;-1;3;UnityEditor.ShaderGraphLitGUI;0;12;Nicrom/NHP/ASE/Stylised Flower With Stem;94348b07e5e8bab40bd6c8a1e3df54cd;True;Forward;0;1;Forward;21;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;2;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;5;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;AlwaysRenderMotionVectors=false;True;5;True;12;all;0;False;True;1;1;False;;0;False;;1;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;1;LightMode=UniversalForward;False;False;0;;0;0;Standard;48;Lighting Model;0;0;Workflow;1;0;Surface;0;0;  Refraction Model;0;0;  Blend;0;0;Two Sided;0;638850416714004705;Alpha Clipping;1;0;  Use Shadow Threshold;0;0;Fragment Normal Space,InvertActionOnDeselection;0;0;Forward Only;0;0;Transmission;0;0;  Transmission Shadow;0.5,False,;0;Translucency;0;0;  Translucency Strength;1,False,;0;  Normal Distortion;0.5,False,;0;  Scattering;2,False,;0;  Direct;0.9,False,;0;  Ambient;0.1,False,;0;  Shadow;0.5,False,;0;Cast Shadows;1;0;Receive Shadows;1;0;Receive SSAO;1;0;Specular Highlights;1;0;Environment Reflections;1;0;Motion Vectors;1;0;  Add Precomputed Velocity;0;0;  XR Motion Vectors;0;0;GPU Instancing;1;0;LOD CrossFade;1;0;Built-in Fog;1;0;_FinalColorxAlpha;0;0;Meta Pass;1;0;Override Baked GI;0;0;Extra Pre Pass;0;0;Tessellation;0;0;  Phong;0;0;  Strength;0.5,False,;0;  Type;0;0;  Tess;16,False,;0;  Min;10,False,;0;  Max;25,False,;0;  Edge Length;16,False,;0;  Max Displacement;25,False,;0;Write Depth;0;0;  Early Z;0;0;Vertex Position,InvertActionOnDeselection;1;0;Debug Display;0;0;Clear Coat;0;0;0;12;False;True;True;True;True;True;True;True;True;True;True;False;False;;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1858;2291.679,2118.861;Float;False;False;-1;3;UnityEditor.ShaderGraphLitGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;ShadowCaster;0;2;ShadowCaster;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;5;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;AlwaysRenderMotionVectors=false;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;False;False;True;False;False;False;False;0;False;;False;False;False;False;False;False;False;False;False;True;1;False;;True;3;False;;False;True;1;LightMode=ShadowCaster;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1859;2291.679,2118.861;Float;False;False;-1;3;UnityEditor.ShaderGraphLitGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;DepthOnly;0;3;DepthOnly;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;5;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;AlwaysRenderMotionVectors=false;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;False;False;True;True;False;False;False;0;False;;False;False;False;False;False;False;False;False;False;True;1;False;;False;False;True;1;LightMode=DepthOnly;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1860;2291.679,2118.861;Float;False;False;-1;3;UnityEditor.ShaderGraphLitGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;Meta;0;4;Meta;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;5;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;AlwaysRenderMotionVectors=false;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=Meta;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1861;2291.679,2118.861;Float;False;False;-1;3;UnityEditor.ShaderGraphLitGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;Universal2D;0;5;Universal2D;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;5;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;AlwaysRenderMotionVectors=false;True;5;True;12;all;0;False;True;1;1;False;;0;False;;1;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;False;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;1;LightMode=Universal2D;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1862;2291.679,2118.861;Float;False;False;-1;3;UnityEditor.ShaderGraphLitGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;DepthNormals;0;6;DepthNormals;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;5;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;AlwaysRenderMotionVectors=false;True;5;True;12;all;0;False;True;1;1;False;;0;False;;0;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;False;;True;3;False;;False;True;1;LightMode=DepthNormals;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1863;2291.679,2118.861;Float;False;False;-1;3;UnityEditor.ShaderGraphLitGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;GBuffer;0;7;GBuffer;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;5;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;AlwaysRenderMotionVectors=false;True;5;True;12;all;0;False;True;1;1;False;;0;False;;1;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;1;LightMode=UniversalGBuffer;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1864;2291.679,2118.861;Float;False;False;-1;3;UnityEditor.ShaderGraphLitGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;SceneSelectionPass;0;8;SceneSelectionPass;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;5;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;AlwaysRenderMotionVectors=false;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=SceneSelectionPass;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1865;2291.679,2118.861;Float;False;False;-1;3;UnityEditor.ShaderGraphLitGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;ScenePickingPass;0;9;ScenePickingPass;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;5;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;AlwaysRenderMotionVectors=false;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=Picking;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1866;2291.679,2118.861;Float;False;False;-1;3;UnityEditor.ShaderGraphLitGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;MotionVectors;0;10;MotionVectors;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;5;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;AlwaysRenderMotionVectors=false;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;False;False;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=MotionVectors;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;1867;2291.679,2118.861;Float;False;False;-1;3;UnityEditor.ShaderGraphLitGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;XRMotionVectors;0;11;XRMotionVectors;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;5;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;AlwaysRenderMotionVectors=false;True;5;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;True;1;False;;255;False;;1;False;;7;False;;3;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;1;LightMode=XRMotionVectors;False;False;0;;0;0;Standard;0;False;0
WireConnection;1503;0;1502;1
WireConnection;1503;2;1502;2
WireConnection;1817;0;1503;0
WireConnection;1504;0;1817;0
WireConnection;1536;6;1500;0
WireConnection;1784;22;1536;0
WireConnection;1784;20;1538;0
WireConnection;1784;24;1815;0
WireConnection;1784;19;1814;0
WireConnection;1401;0;1308;0
WireConnection;1219;0;1218;0
WireConnection;928;0;738;4
WireConnection;1403;0;1249;0
WireConnection;1221;0;1220;0
WireConnection;1811;0;850;0
WireConnection;1811;1;1809;0
WireConnection;1811;2;1810;0
WireConnection;1360;0;1286;0
WireConnection;873;0;480;0
WireConnection;1344;0;1784;16
WireConnection;880;0;300;0
WireConnection;870;0;1811;0
WireConnection;877;0;687;0
WireConnection;1335;0;1334;0
WireConnection;1356;0;1262;0
WireConnection;1525;0;1524;0
WireConnection;1373;0;952;0
WireConnection;1340;0;1784;0
WireConnection;1535;44;1399;0
WireConnection;1535;39;1400;0
WireConnection;1535;43;1405;0
WireConnection;1535;40;1406;0
WireConnection;1535;46;1408;0
WireConnection;1535;47;1520;0
WireConnection;1786;36;1362;0
WireConnection;1786;35;1365;0
WireConnection;1786;34;1366;0
WireConnection;1786;28;1367;0
WireConnection;1786;47;1526;0
WireConnection;1786;29;1369;0
WireConnection;1786;46;1512;0
WireConnection;1786;42;1370;0
WireConnection;1786;27;1371;0
WireConnection;1214;1;1215;0
WireConnection;1214;0;1535;0
WireConnection;1785;20;1375;0
WireConnection;1785;19;1376;0
WireConnection;1785;18;1378;0
WireConnection;1420;0;1785;0
WireConnection;769;0;1214;0
WireConnection;97;0;1786;0
WireConnection;1457;0;1455;0
WireConnection;1457;1;1456;1
WireConnection;1805;0;1804;0
WireConnection;1775;0;1774;0
WireConnection;1458;0;1457;0
WireConnection;1533;55;1421;0
WireConnection;1533;53;1413;0
WireConnection;1533;59;1507;0
WireConnection;1533;58;1415;0
WireConnection;1855;87;1533;0
WireConnection;1855;42;1780;0
WireConnection;1855;92;1806;0
WireConnection;1855;93;1807;0
WireConnection;1855;41;1779;0
WireConnection;1492;0;1481;4
WireConnection;1492;1;1482;4
WireConnection;1492;2;1483;0
WireConnection;1781;1;1533;0
WireConnection;1781;0;1855;0
WireConnection;1045;0;1781;0
WireConnection;1494;0;1492;0
WireConnection;1465;0;1463;0
WireConnection;1465;1;1464;0
WireConnection;1506;0;1505;0
WireConnection;1466;0;1465;0
WireConnection;1812;0;1470;0
WireConnection;1469;0;1467;0
WireConnection;1469;1;1468;0
WireConnection;1473;0;1469;0
WireConnection;1473;1;1813;0
WireConnection;1474;0;1472;0
WireConnection;1474;1;1471;0
WireConnection;1475;0;1473;0
WireConnection;1478;0;1475;0
WireConnection;1478;1;1474;0
WireConnection;1480;0;1477;0
WireConnection;1480;1;1476;0
WireConnection;1480;2;1478;0
WireConnection;1486;0;1481;0
WireConnection;1486;1;1482;0
WireConnection;1486;2;1483;0
WireConnection;1485;0;1480;0
WireConnection;1485;1;1479;0
WireConnection;1485;2;1484;0
WireConnection;1488;0;1485;0
WireConnection;1487;0;1486;0
WireConnection;1491;0;1490;0
WireConnection;1491;1;1489;0
WireConnection;1493;0;1491;0
WireConnection;1857;0;296;0
WireConnection;1857;3;1816;0
WireConnection;1857;4;1816;0
WireConnection;1857;6;1508;0
WireConnection;1857;8;1574;0
ASEEND*/
//CHKSM=882702ED4D8CB74F8132C5D882960694DC23ECBF