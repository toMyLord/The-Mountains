Shader "Unlit/NoiseBurn"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NoiseTex ("Noise" ,2D) = "white" {}
        _JianbianTex ("Jianbian", 2D) = "white" {}
        _FlowSpeed ("Flow Speed", Range(-1,1)) = -0.2
        _BurnColor1 ("Burn Color 1", Color) = (1,1,1,1)
        _BurnColor2 ("Burn Color 2", Color) = (1,1,1,1)
        _BurnThresthold ("Burn Progress", Range(0,1)) = 0.5
        _BurnStep ("Burn Step", Range(0,1)) = 0.5
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _NoiseTex;
            fixed4 _BurnColor1;
            fixed4 _BurnColor2;
            fixed _BurnThresthold;
            fixed _BurnStep;
            fixed _FlowSpeed;
            sampler2D _JianbianTex;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed burnFactor1 = tex2D(_NoiseTex, i.uv + _Time.y * fixed2(0, _FlowSpeed));
                fixed burnFactor2 = tex2D(_NoiseTex, float2(i.uv.x + 0.33f,i.uv.y) + _Time.y * fixed2(0, _FlowSpeed * 1.3f));
                fixed burnFactor3 = tex2D(_NoiseTex, float2(i.uv.x + 0.67f,i.uv.y) + _Time.y * fixed2(0, _FlowSpeed * 1.7f));
                fixed jianbianFactor = tex2D(_JianbianTex, i.uv);
                fixed burnFactor = (burnFactor1 + burnFactor2 + burnFactor3) / 3.0f;
                if(burnFactor > jianbianFactor)
                    discard;
                fixed4 col = tex2D(_MainTex, i.uv);
                fixed colorFactor = (burnFactor - _BurnThresthold);
                fixed stepColorFactor = smoothstep(0,_BurnStep,colorFactor);
                if(colorFactor<_BurnStep)
                {
                    fixed colorBurn = lerp(_BurnColor2, _BurnColor1, stepColorFactor);
                    col = lerp(_BurnColor2, _BurnColor1,stepColorFactor);
                }
                return col;
            }
            ENDCG
        }
    }
}
