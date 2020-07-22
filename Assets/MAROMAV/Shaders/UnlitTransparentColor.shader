
Shader "MAROMAV/Unlit Transparent Color" {
  Properties {
    _Color ("Color", COLOR) = (1, 1, 1, 1)
  }
  SubShader {
    Tags {
      "Queue" = "Transparent"
      "IgnoreProjector" = "True"
      "RenderType"="Transparent"
    }
    LOD 100

    ZWrite Off
    Blend SrcAlpha OneMinusSrcAlpha

    Pass {
      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag

      #include "UnityCG.cginc"

      struct appdata {
        float4 vertex : POSITION;
      };

      struct v2f {
        float4 vertex : SV_POSITION;
      };

      float4 _Color;
      float4 _MainTex_ST;

      v2f vert (appdata v) {
        v2f o;
        float4 vertex4;
        vertex4.xyz = v.vertex;
        vertex4.w = 1.0;
        o.vertex = UnityObjectToClipPos(vertex4);
        return o;
      }

      fixed4 frag (v2f i) : SV_Target {
        return _Color;
      }
      ENDCG
    }
  }
  FallBack "Unlit/Transparent"
}
