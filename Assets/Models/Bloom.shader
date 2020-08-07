Shader "Unlit/Bloom"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "black" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass // 0
        {
			
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            //#pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                //UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex, _SourceTex;
            float4 _MainTex_ST;
			float4 _MainTex_TexelSize;
			float _Threshold;
			float _Intensity;
			half _SoftThreshold;

			half3 Sample(float2 uv) {
				return tex2D(_MainTex, uv).rgb;
			}
			half3 BoxSample(float2 uv, float delta) {
				float4 o = _MainTex_TexelSize.xyxy * float2(-delta, delta).xxyy;
				half3 s = Sample(uv + o.xy) + Sample(uv + o.zy) + Sample(uv + o.xw) + Sample(uv + o.zw);

				//Divide by 4
				return s * 0.25f;
				//return s;
			}

			half3 Prefilter(half3 c) {
				half brightness = max(c.r, max(c.g, c.b));
				//half knee = _Threshold * _SoftThreshold;
				//half soft = brightness - _Threshold + knee;
				//soft = clamp(soft, 0, 2 * knee);
				//soft = soft * soft / (4 * knee + 0.00001);
				//half contribution = max(soft, brightness - _Threshold);
				half contribution = max(0, brightness - _Threshold);
				contribution /= max(brightness, 0.0001);
				return c * contribution;
			}

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
                //o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                //UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                //fixed4 col = tex2D(_MainTex, i.uv);
				fixed4 col = half4(Prefilter(BoxSample(i.uv, 1)), 1);
                // apply fog
                //UNITY_APPLY_FOG(i.fogCoord, col);
				return col;// *half4(0, 1, 0, 0);
            }
            ENDCG
        }



		Pass // 1
        {
			
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            //#pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                //UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex, _SourceTex;
            float4 _MainTex_ST;
			float4 _MainTex_TexelSize;
			float _Intensity;

			half3 Sample(float2 uv) {
				return tex2D(_MainTex, uv).rgb;
			}
			half3 BoxSample(float2 uv, float delta) {
				float4 o = _MainTex_TexelSize.xyxy * float2(-delta, delta).xxyy;
				half3 s = Sample(uv + o.xy) + Sample(uv + o.zy) + Sample(uv + o.xw) + Sample(uv + o.zw);

				//Divide by 4
				return s * 0.25f;
			}

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
                //o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                //UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                //fixed4 col = tex2D(_MainTex, i.uv);
				fixed4 col = half4(BoxSample(i.uv, 1), 1);
                // apply fog
                //UNITY_APPLY_FOG(i.fogCoord, col);
				return col;// *half4(0, 1, 0, 0);
            }
            ENDCG
        }
		
		Pass //2
        {
			Blend One One

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            //#pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                //UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex, _SourceTex;
            float4 _MainTex_ST;
			float4 _MainTex_TexelSize;
			float _Intensity;

			half3 Sample(float2 uv) {
				return tex2D(_MainTex, uv).rgb;
			}
			half3 BoxSample(float2 uv, float delta) {
				float4 o = _MainTex_TexelSize.xyxy * float2(-delta, delta).xxyy;
				half3 s = Sample(uv + o.xy) + Sample(uv + o.zy) + Sample(uv + o.xw) + Sample(uv + o.zw);

				//Divide by 4
				return s * 0.25f;
			}

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
                //o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                //UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                //fixed4 col = tex2D(_MainTex, i.uv);
				fixed4 col = half4(BoxSample(i.uv, 0.5), 1);
                // apply fog
                //UNITY_APPLY_FOG(i.fogCoord, col);
				return col;// *half4(0, 1, 0, 0);
            }
            ENDCG
        }

		Pass //Pass 3
        {
			
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            //#pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                //UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex, _SourceTex;
            float4 _MainTex_ST;
			float4 _MainTex_TexelSize;
			float _Intensity;

			half3 Sample(float2 uv) {
				return tex2D(_MainTex, uv).rgb;
			}
			half3 BoxSample(float2 uv, float delta) {
				float4 o = _MainTex_TexelSize.xyxy * float2(-delta, delta).xxyy;
				half3 s = Sample(uv + o.xy) + Sample(uv + o.zy) + Sample(uv + o.xw) + Sample(uv + o.zw);

				//Divide by 4
				return s * 0.25;
			}

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
                //o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                //UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                //fixed4 col = tex2D(_MainTex, i.uv);
				fixed4 col = tex2D(_SourceTex, i.uv);
				col.rgb += _Intensity * BoxSample(i.uv, 0.5);
				//col.rgb += BoxSample(i.uv, 0.5);
				
				//col.rgb +=  BoxSample(i.uv, 0.5);

                // apply fog
                //UNITY_APPLY_FOG(i.fogCoord, col);
				return col;// *half4(0, 1, 0, 0);
            }
            ENDCG
        }



    }
}
