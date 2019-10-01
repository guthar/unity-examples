// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/uWater_Fast"
{
    Properties {
		[NoScaleOffset]	_MainTex ("Base (RGB)", 2D) = "white" {} 
		[Normal] 	[NoScaleOffset]	_BumpMap ("Normalmap", 2D) = "bump" {}
		[NoScaleOffset] _Cube ("Cubemap", Cube) = "" { /* used to be TexGen CubeReflect */ }
		_TextureAnimationX ("MainTex Animation X", Range (-10.0, 10.0)) = 0.0
		_TextureAnimationY ("MainTex Animation Y", Range (-10.0, 10.0)) = 0.0
		_TextureScale ("Texture Scale", Float) = 1.0
		_NormalMap0AnimationX ("Normalmap1 Animation X", Range (-10.0,10.0)) = 0.0
		_NormalMap0AnimationY ("Normalmap1 Animation Y", Range (-10.0,10.0)) = 0.0
		_NormalMap0Scale ("Normalmap1 Scale", Float) = 1.0
		_NormalMap1AnimationX ("Normalmap2 Animation X", Range (-10.0,10.0)) = 0.0
		_NormalMap1AnimationY ("Normalmap2 Animation Y", Range (-10.0,10.0)) = 0.0
		_NormalMap1Scale ("Normalmap2 Scale", Float) = 1.0
		_NormalMapOffsets ("Normalmap1 Offset (XY) Normalmap2 Offset (ZW)", Vector) = (0.0, 0.0, 0.0, 0.0)	
		[Toggle(PixelFresnel)] _PixelFresnel("Per Pixel Fresnel (more accurate, but slower)", Int) = 0		
		_ReflectColor ("Reflection Color", Color) = (1,1,1,1)
		_HorizonColor ("Horizon Color", Color) = (1,1,1,1)
		_ReflectionFresnel ("Reflection Fresnel", Float) = 2.0
		_MinReflectionFresnel ("Min Reflection Fresnel", Float) = 0.5
		_HorizonColorFresnel ("Horizon Color Fresnel", Float) = 2.0
		_NormalStrength ("Normal Intensity", Float) = 1.0
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        Cull Back
        ZWrite On
 
        Pass {
            Lighting On
        
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
			#pragma shader_feature PixelFresnel
			#include "UnityCG.cginc"
 
            struct v2f
            {
                float2 uv_MainTex : TEXCOORD0;
                float4 uv_BumpMap : TEXCOORD1;
				float4 viewDir : TEXCOORD2;
				float4 wpos : TEXCOORD3;
                fixed3 directionalambient : COLOR0;
                fixed3 points : COLOR1;
                float4 pos : SV_POSITION;
            };

			sampler2D _MainTex;
			sampler2D _BumpMap;
			samplerCUBE _Cube;
			fixed3 _ReflectColor;
			fixed3 _HorizonColor;
			half _ReflectionFresnel;
			fixed _MinReflectionFresnel;
			half _HorizonColorFresnel;
			fixed _NormalStrength;
			float _TextureAnimationX;
			float _TextureAnimationY;
			fixed _TextureScale;
			float _NormalMap0AnimationX;
			float _NormalMap0AnimationY;
			fixed _NormalMap0Scale;
			float _NormalMap1AnimationX;
			float _NormalMap1AnimationY;
			fixed _NormalMap1Scale;
			fixed4 _NormalMapOffsets;		

			float4 _Scales;

			float3 ShadeVertexLightsDirAmb(float4 vertex, float3 normal, bool spotLight) {
				float3 viewpos = mul (UNITY_MATRIX_MV, vertex).xyz;
				float3 viewN = normalize (mul ((float3x3)UNITY_MATRIX_IT_MV, normal));

				float3 lightColor = UNITY_LIGHTMODEL_AMBIENT.xyz;

				// Iterate through light sources. Max value *must* be hardcoded or WebGL 1.0 support will break.
				for (int i = 0; i < 4; i++) {								
					// Check if is a valid ambient light (w-value). 
					// May not use "continue" keyword because WebGL 1.0 prohibits this.
					if (unity_LightPosition[i].w <= 0.0f) {
						float3 toLight = unity_LightPosition[i].xyz - viewpos.xyz * unity_LightPosition[i].w;
						float lengthSq = dot(toLight, toLight);
						
						toLight *= rsqrt(lengthSq);
						
						float atten = 1.0 / (1.0 + lengthSq * unity_LightAtten[i].z);
						
						if (spotLight) {
							float rho = max(0, dot(toLight, unity_SpotDirection[i].xyz));
							float spotAtt = (rho - unity_LightAtten[i].x) * unity_LightAtten[i].y;
							atten *= saturate(spotAtt);
						}

						float diff = max(0, dot(viewN, toLight));
						lightColor += unity_LightColor[i].rgb * (diff * atten);
					}
				}

				return lightColor;
			}
						
			float3 ShadeVertexLightsPoints(float4 vertex, float3 normal, bool spotLight) {
				float3 viewpos = mul (UNITY_MATRIX_MV, vertex).xyz;
				float3 viewN = normalize (mul ((float3x3)UNITY_MATRIX_IT_MV, normal));

				float3 lightColor = 0;

				// Iterate through light sources. Max value *must* be hardcoded or WebGL 1.0 support will break.
				for (int i = 0; i < 8; i++) {					
					// Check if is a valid light point (w-value). 
					// May not use "continue" keyword because WebGL 1.0 prohibits this.
					if (unity_LightPosition[i].w > 0.0f) {
						float3 toLight = unity_LightPosition[i].xyz - viewpos.xyz * unity_LightPosition[i].w;
						float lengthSq = dot(toLight, toLight);
						
						toLight *= rsqrt(lengthSq);

						float atten = 1.0 / (1.0 + lengthSq * unity_LightAtten[i].z);

						if (spotLight) {
							float rho = max(0, dot(toLight, unity_SpotDirection[i].xyz));
							float spotAtt = (rho - unity_LightAtten[i].x) * unity_LightAtten[i].y;
							atten *= saturate(spotAtt);
						}

						float diff = max(0, dot(viewN, toLight));
						lightColor += unity_LightColor[i].rgb * (diff * atten);
					}
				}

				return lightColor;
			}

            v2f vert (appdata_full v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
				float4 wpos = mul(unity_ObjectToWorld, v.vertex);
                o.uv_MainTex = (v.texcoord * _TextureScale) + 	(float2(_TextureAnimationX, _TextureAnimationY)) * _Time.x;
                o.uv_BumpMap.xy = (v.texcoord * _NormalMap0Scale + _NormalMapOffsets.xy) + (float2 (_NormalMap0AnimationX, _NormalMap0AnimationY)) * _Time.x;
                o.uv_BumpMap.zw = (v.texcoord * _NormalMap1Scale + _NormalMapOffsets.zw) + (float2(_NormalMap1AnimationX, _NormalMap1AnimationY)) * _Time.x;
                o.directionalambient = ShadeVertexLightsDirAmb(v.vertex, v.normal, false);
                o.points = ShadeVertexLightsPoints(v.vertex, v.normal, false);
				float4 nrml = mul(unity_ObjectToWorld, float4(v.normal.xyz,0));
				float3 viewDir = normalize(WorldSpaceViewDir(v.vertex));
				o.viewDir.xyz = viewDir;
				
				#if PixelFresnel
					o.viewDir.w = 1;
				#else
					o.viewDir.w = pow(1 - viewDir.y, _ReflectionFresnel) + _MinReflectionFresnel; 
				#endif

				o.wpos = wpos;

				// Put clampedHorizonFresnel into wpos.w to transport it into fragment-part. 
				#if PixelFresnel
					o.wpos.w = 1;
				#else
					o.wpos.w = clamp( pow(1 - viewDir.y, _HorizonColorFresnel), 0, 1);
				#endif

                return o; 
            }
 
            fixed4 frag (v2f i) : COLOR {
            
                // base texture
                fixed4 ct = tex2D(_MainTex, i.uv_MainTex);

                // base texture
                fixed4 packedNormal = tex2D(_BumpMap, i.uv_BumpMap.xy) + tex2D(_BumpMap, i.uv_BumpMap.zw);
				packedNormal *= 0.5f;
				fixed3 nrml = UnpackNormal(packedNormal);
				
				nrml.xy *= _NormalStrength;
				nrml = normalize(nrml);
				
				#if PixelFresnel
					float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.wpos.xyz);
				#else
					float3 viewDir = i.viewDir;
				#endif
				
				float3 worldRefl = reflect(viewDir.xyz, nrml);
				worldRefl.x = -worldRefl.x;
				worldRefl.y = abs(worldRefl.y);
						
                // modulate with lighting
                float3 c_tex = ct.rgb * i.directionalambient;
                float3 c_notex = ct.a * i.points;
				
				float3 c = c_tex + c_notex;
				
				#if PixelFresnel
					float Fresnel = pow(1 - viewDir.y, _ReflectionFresnel) + _MinReflectionFresnel;
					float HorizonFresnel = pow(1 - viewDir.y, _HorizonColorFresnel);
					float clampedHorizonFresnel = clamp(HorizonFresnel, 0, 1);
				#else
					float Fresnel = i.viewDir.w;
					float clampedHorizonFresnel = i.wpos.w;
				#endif
				
				// add cubemap reflection
				fixed4 refl = texCUBE (_Cube, worldRefl);
				c += refl.rgb * _ReflectColor * Fresnel;
				
				//return float4(refl.rgb, 1);
				
				// blend with horizon color 
				c = lerp(c, _HorizonColor, clampedHorizonFresnel);

                return float4(c, 1.0f);
            }
            ENDCG
        }
    }
  
    FallBack "Diffuse"
}