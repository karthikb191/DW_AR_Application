// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/SurfaceHighlight"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _HighlightColor ("HighlightColor", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _NormalMap ("Normal Map", 2D) = "white" {}
        _SpecularMap ("Specular Map", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
		_Multiplier("Multiplier", Range(0,100)) = 1.0
		_Transparency("Transparency", Range(0,1)) = 1.0
		_Outline("Outline", Range(0,0.2)) = 0.1
        _OutlineColor ("OutlineColor", Color) = (1,1,1,1)
			
    }
    SubShader
    {
		GrabPass{ }

		Tags {"Queue" = "Transparent" "RenderType" = "Geometry"}
		ZWrite Off
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf NoLighting vertex:vert //alpha:fade

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _NormalMap;
		sampler2D _SpecularMap;

		struct Input
		{
			float2 uv_MainTex;
			float2 uv_NormalMap;
			float2 uv_SpecularMap;
			float3 viewDir;
			float3 worldPos;
		};


		half _Glossiness;
		half _Metallic;
		float _Multiplier;
		float _Transparency;
		float _Outline;
		half3 _HighlightColor;
		half3 _OutlineColor;
		fixed4 _Color;


		void vert(inout appdata_full v)
		{
			float4 vPos = mul(UNITY_MATRIX_MV, v.vertex);
			
			vPos += _Multiplier;
			v.vertex.xyz += v.normal * _Outline;
		}


		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf(Input IN, inout SurfaceOutput o)
		{
			
			o.Albedo = _OutlineColor * _Transparency;

			o.Alpha = _Transparency;

		}

		fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten)
		{
			//return fixed4(0, 0, 0, 0);
			fixed4 c;
			c.rgb = s.Albedo;
			c.a = s.Alpha;
			return c;
		}

		ENDCG


		Tags{ "Queue" = "Transparent" "RenderType" = "Geometry" }
		//GrabPass{ }

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf NoLighting vertex:vert

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _NormalMap;
		sampler2D _SpecularMap;
		uniform sampler2D _GrabTexture;

		struct Input
		{
			float2 uv_MainTex;
			float2 uv_NormalMap;
			float2 uv_SpecularMap;
			float3 viewDir;
			float3 worldPos;
			float4 grabUV;
		};


		half _Glossiness;
		half _Metallic;
		float _Multiplier;
		float _Transparency;
		float _Outline;
		half3 _HighlightColor;
		fixed4 _Color;


		void vert(inout appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);
			//float4 vPos = mul(UNITY_MATRIX_MV, v.vertex);
			//
			//vPos += _Multiplier;
			//v.vertex.xyz += v.normal * _Outline;
			//
			float4 hpos = UnityObjectToClipPos(v.vertex);
			o.grabUV = ComputeGrabScreenPos(hpos);
			//o.vert = v.vertex;
		}


		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
		// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf(Input IN, inout SurfaceOutput o)
		{
			o.Emission = tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(IN.grabUV));;
			//o.Albedo = 1;
			o.Alpha = 1.0;
		}

		fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten)
		{
			//return fixed4(0, 0, 0, 0);
			fixed4 c;
			c.rgb = s.Albedo;
			c.a = s.Alpha;
			return c;
		}


		ENDCG


        Tags {"Queue" = "Transparent+1" "RenderType" = "Transparent"}
        LOD 200
		ZWrite Off

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows alpha:fade

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _NormalMap;
        sampler2D _SpecularMap;
		

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_NormalMap;
            float2 uv_SpecularMap;
			float3 viewDir;
			float3 worldPos;
        };


        half _Glossiness;
        half _Metallic;
		float _Multiplier;
		float _Transparency;
		half3 _HighlightColor;
        fixed4 _Color;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;


			//Add in a normal map
			o.Normal = UnpackNormal(tex2D(_NormalMap, IN.uv_NormalMap));

			//o.Normal *= ccc;
			
			//Add in a specular map
			fixed4 specularTex = tex2D(_SpecularMap, IN.uv_SpecularMap);

			float3 viewDir = IN.viewDir;
			//float3 dir = normalize( viewDir - IN.worldPos);
			float res = (dot(viewDir, o.Normal));
			res = lerp(0, 0.8, res);
			float m = (_Multiplier - 1) * res * _Transparency;


			//half3 highlightColor = half3(0, 1, 0);

			float3 col = (m * _HighlightColor);


			half t = 1 - _Transparency;
			o.Albedo = (c * t) + col;
			
			
            // Metallic and smoothness come from slider variables
            o.Metallic = specularTex.r * _Metallic;
            o.Smoothness = specularTex.z * _Glossiness;


			o.Alpha = c.a * _Transparency * res;

        }
        ENDCG

    }
    FallBack "Diffuse"
}
