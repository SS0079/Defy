Shader "KittyShader/ColorFlipRotateOverlayUnlit"
{
    Properties
    {
        _Diffuse ("Diffuse", Color) = (1,1,1,1)
        _Round("Round",Range(-4,4))=0
        _Tint("Color tint",Color)=(1,1,1,1)
    }
    SubShader
    {
        Tags { "Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            Tags{"LightMode" = "ForwardBase"}
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"


            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 objPos:TEXCOORD1;
            };

            float4 _Diffuse;
            float _Round;
            float4 _Tint;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.objPos=v.vertex;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float thisAngle=-degrees(atan2(i.objPos.x,i.objPos.y)) ;
                float4 roundTint=_Diffuse;
                int round_int=floor(abs(_Round));
                if (_Round>0)//clockwise
                {
                    if (thisAngle<0)
                    {
                        thisAngle=360+thisAngle;
                    }
                    if (thisAngle<frac(_Round)*360)//inside 
                    {
                        roundTint=float4(_Tint.xyz,clamp(round_int+1,0,4)*_Tint.w);
                    }
                    else if(round_int>0)
                    {
                        roundTint=float4(_Tint.xyz,clamp(round_int,0,4)*_Tint.w);
                    }
                }
                else if(_Round<0)//anti clockwise
                {
                    if (thisAngle>0)
                    {
                        thisAngle=thisAngle-360;
                    }
                    if (thisAngle>frac(-_Round)*-360)//inside 
                    {
                        roundTint=float4(_Tint.xyz,clamp(round_int+1,0,4)*_Tint.w);
                    }
                    else if(round_int>0)
                    {
                        roundTint=float4(_Tint.xyz,clamp(round_int,0,4)*_Tint.w);
                    }
                }
                
                // if (roundTint.w<=0)
                // {
                //     discard;
                // }
                return float4(roundTint);
            }
            ENDCG
        }
    }
}
