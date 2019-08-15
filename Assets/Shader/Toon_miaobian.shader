// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Tut/Shader/Toon/miaobian" {
Properties {
		//_Color("Main Color",color)=(1,1,1,1)
		_Outline("Thick of Outline",range(0,0.1))=0.02
		_Factor("Factor",range(0,1))=0.5
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
	}
	SubShader {
		pass{
		Tags{"LightMode"="Always"}
		Cull Front
		ZWrite On
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#include "UnityCG.cginc"
		float _Outline;
		float _Factor;
		float4 _Color;
		struct v2f {
			float4 pos:SV_POSITION;
		};

		v2f vert (appdata_full v) {
			v2f o;
			float3 dir=normalize(v.vertex.xyz);
			float3 dir2=v.normal;
			float D=dot(dir,dir2);
			dir=dir*sign(D);
			dir=dir*_Factor+dir2*(1-_Factor);
			v.vertex.xyz+=dir*_Outline;
			o.pos=UnityObjectToClipPos(v.vertex);
			return o;
		}
		float4 frag(v2f i):COLOR
		{
			float4 c = 0;
			return c;
		}
		ENDCG
		}
		pass{
		Tags{"LightMode"="ForwardBase"}
		Cull Back
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#include "UnityCG.cginc"

		float4 _LightColor0;
		float4 _Color;
		float _Steps;
		float _ToonEffect;
		sampler2D _MainTex;
		float4 _MainTex_ST;

		struct v2f {
			float4 pos:SV_POSITION;
			float3 lightDir:TEXCOORD0;
			float3 viewDir:TEXCOORD1;
			float3 normal:TEXCOORD2;
			float2 uv:TEXCOORD3;
		};

		v2f vert (appdata_full v) {
			v2f o;
			o.pos=UnityObjectToClipPos(v.vertex);
			o.normal=v.normal;
			o.lightDir=ObjSpaceLightDir(v.vertex);
			o.viewDir=ObjSpaceViewDir(v.vertex);
			o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);

			return o;
		}
		float4 frag(v2f i):COLOR
		{
			float4 c=tex2D(_MainTex, i.uv);
			float3 N=normalize(i.normal);
			float3 viewDir=normalize(i.viewDir);
			float3 lightDir=normalize(i.lightDir);
			float diff=max(0,dot(N,i.lightDir));
			diff=(diff+1)/2;
			diff=smoothstep(0,1,diff);
			c=c*_LightColor0*(diff);
			return c;
		}
		ENDCG
		}
		Pass {	//开启影子
		Tags { "LightMode"="ShadowCaster" }  
		CGPROGRAM  
		#pragma vertex vert  
		#pragma fragment frag  
		#pragma multi_compile_shadowcaster  
		#include "UnityCG.cginc"  
  
		sampler2D _Shadow;  
  
		struct v2f
		{  
			V2F_SHADOW_CASTER;  
			//float2 uv:TEXCOORD2;  
		};  
  
		v2f vert(appdata_base v)
		{  
			v2f o;  
			//o.uv = v.texcoord.xy;  
			TRANSFER_SHADOW_CASTER_NORMALOFFSET(o);  
			return o;  
		}  
  
		float4 frag( v2f i ) : SV_Target  
		{  
			//fixed alpha = tex2D(_Shadow, i.uv).a;  
			//clip(alpha - 0.5);  
			SHADOW_CASTER_FRAGMENT(i)  
		}  
	    ENDCG  
		}  
	} 
}