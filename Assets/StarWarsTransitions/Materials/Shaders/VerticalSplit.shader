Shader "SinaC/SWT/VerticalSplit"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Progress("Progress", Range(0, 1)) = 0
        _Blur("Blur", Range(0, 1)) = 0.1
        [Toggle]_Reverse("Reverse", float) = 0
        
        /* Necessary for making the shader UI compatible */
        _StencilComp("Stencil Comparison", Float) = 8
        _Stencil("Stencil ID", Float) = 0
        _StencilOp("Stencil Operation", Float) = 0
        _StencilWriteMask("Stencil Write Mask", Float) = 255
        _StencilReadMask("Stencil Read Mask", Float) = 255
        _ColorMask("Color Mask", Float) = 15
        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip("Use Alpha Clip", Float) = 0
    }
    SubShader
    {
        /* Necessary for making the shader UI compatible */
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        /* Necessary for making the shader UI compatible */
        Stencil
        {
            Ref[_Stencil]
            Comp[_StencilComp]
            Pass[_StencilOp]
            ReadMask[_StencilReadMask]
            WriteMask[_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest Always
        //ZTest[unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask[_ColorMask]

        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "UnityUI.cginc" //Necessary for making the shader UI compatible

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT //Necessary for making the shader UI compatible
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP //Necessary for making the shader UI compatible

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR; //Necessary for making the shader UI compatible
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 worldPosition : TEXCOORD1; //Necessary for making the shader UI compatible
                fixed4 color : COLOR0; //Necessary for making the shader UI compatible
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _ClipRect;
            float _Blur;
            float _Progress;
            bool _Reverse;

            v2f vert (appdata v)
            {
                v2f o;
                o.worldPosition = v.vertex;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float y = abs(i.uv.y - 0.5) * 2;
                y = (y - 0.5) * (1 - _Blur) + 0.5;
                if(_Reverse)
                    y = 1 - y;
                
                const float halfBlur = _Blur / 2;
                const float min = _Progress - halfBlur;
                const float max = _Progress + halfBlur;

                float alpha = 1;
                
                if(y < min)
                    alpha = 0;
                else if(y < max)
                    alpha = (y - min) / _Blur;

                fixed4 col = fixed4(1,1,1, alpha);
                col *= i.color * tex2D (_MainTex, i.uv);

#ifdef UNITY_UI_CLIP_RECT
                col.a *= UnityGet2DClipping(i.worldPosition.xy, _ClipRect);
#endif

#ifdef UNITY_UI_ALPHACLIP
                clip(col.a - 0.001);
#endif
                return col;
            }
            ENDCG
        }
    }
}
