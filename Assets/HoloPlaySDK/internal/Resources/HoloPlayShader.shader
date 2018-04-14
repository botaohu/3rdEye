//Copyright 2017 Looking Glass Factory Inc.
//All rights reserved.
//Unauthorized copying or distribution of this file, and the source code contained herein, is strictly prohibited.

Shader "Hidden/HoloPlay/HoloPlay Final" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
	}
		SubShader{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		LOD 300

		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite Off

		Pass{
		CGPROGRAM
#pragma target 3.5
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"

		struct v2f {
		float4 pos : SV_POSITION;
		float2 uv : TEXCOORD0;
	};

	float4 _MainTex_ST;

	v2f vert(appdata_base v) {
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
		return o;
	}

	sampler2D _MainTex;
	float pitch;
	float tilt;
	// float slope;
	float center;
	float numViews;
	float tilesX;
	float tilesY;
	float flipX;
	float flipY;
	float flipSubp;
	const int subs;

	float4 texArr(float3 uvz) {
		//decide which section to take from based on the z.
		float z = floor(uvz.z);
		float x = fmod(z, tilesX) / tilesX;
		float y = floor(z / tilesX) / tilesY;
		x += uvz.x / tilesX;
		y += uvz.y / tilesY;
		float4 c = tex2D(_MainTex, float2(x, y));
		return c;
	}

	float4 frag(v2f IN) : COLOR{
		float4 rgb[3];
		float subp = 1.0 / (_ScreenParams.x * 3.0);
		subp = subp * (1 - flipX) - subp * flipX;
		subp = subp * (1 - flipSubp) - subp * flipSubp;
		float py = 1.0 / _ScreenParams.y;
		float3 nuv = float3(IN.uv.xy, 0);
		nuv.x = (1.0 - flipX) * nuv.x + flipX * (1.0 - nuv.x);
		nuv.y = (1.0 - flipY) * nuv.y + flipY * (1.0 - nuv.y);
		float inAspect = float(_ScreenParams.y) / _ScreenParams.x;
		float aspect = float(_ScreenParams.x) / _ScreenParams.y;
		tilt = tilt * (1 - flipX) - tilt * flipX;
		center = center * (1 - flipX) - center * flipX;

		for (int i; i < subs; i++) {
			nuv.z = (nuv.x + i * subp + IN.uv.y * tilt) * pitch - center;
			nuv.z = fmod(nuv.z + ceil(pitch), 1);
			nuv.z *= numViews;
			rgb[i] = texArr(nuv);
		}

		if (subs == 1)
		{
			return rgb[0];
		}

		return float4(rgb[0].r, rgb[1].g, rgb[2].b, 1);
	}
		ENDCG
	}
	}
		FallBack Off
}