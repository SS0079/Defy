Shader "KittyShader/ColorFlipUpDown" 
{
    Properties
    {
	     _Diffuse("Diffuse",Color)=(1,1,1,1)
	     _Specular("Specular",Color)=(1,1,1,1)
	     _Gloss("Gloss",Range(8.0,256))=20
	     _SignColor ("标识色", Color) = (1.0, 0.0, 0.0, 1.0)
	     _Flip ("位置",Range(0,1))=0
	     _Max("最大",float)=1
	     _Min("最小",float)=1
    }
	
    SubShader
    {
        Pass
        { 
            Tags {"LightMode"="ForwardBase" }
            CGPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Lighting.cginc"

            struct a2v
            {
                float4 vertex: POSITION;
            	float3 normal: NORMAL;
            };

            struct v2f
            {
            	float4 pos: SV_POSITION;
            	float3 worldNormal:TEXCOORD0;
				float3 worldPos: TEXCOORD2;
            	fixed4 localPos: TEXCOORD3;
            };
			

            fixed4 _Diffuse;
            fixed4 _Specular;
            float _Gloss;
            fixed4 _SignColor;
			float _Flip;
            float _Max;
            float _Min;
            
            v2f vert (a2v v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
            	o.worldNormal=UnityObjectToWorldNormal(v.normal);
            	o.worldPos=UnityObjectToWorldDir(v.vertex);
            	o.localPos=v.vertex;
                return o;
            }
            //片元渲染器
            fixed4 frag (v2f i) : SV_Target
            {
            	fixed3 ambient=UNITY_LIGHTMODEL_AMBIENT.xyz;
            	fixed3 worldLightDir=normalize(_WorldSpaceLightPos0.xyz);
            	//compute diffuse
            	fixed3 diffuse=_LightColor0.rgb*_Diffuse.rgb*saturate(dot(i.worldNormal,worldLightDir));
            	//get reflect in world space
            	fixed3 reflectDir=normalize(reflect(-worldLightDir,i.worldNormal));
            	//get view dir in world space
            	fixed3 viewDir=normalize(_WorldSpaceCameraPos.xyz-i.worldPos.xyz);
            	//compute specular
            	fixed3 specular=_LightColor0.rgb*_Specular.rgb*pow(saturate(dot(reflectDir,viewDir)),_Gloss);
            	//default sign color is white
                fixed4 col= fixed4(0,0,0,0);
				if (i.localPos.z < lerp(_Min,_Max,_Flip))
				{
					//像素输出的颜色 = _Color
					col = _SignColor;
				}
                return fixed4(ambient+diffuse+specular,1.0) + col;
            }
            ENDCG
        }
    }
}
