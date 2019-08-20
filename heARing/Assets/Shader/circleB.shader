Shader "Custom/circleB" {
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard alpha:fade
        #pragma target 3.0

        struct Input {
            float3 worldPos;
        };

        float _A;
        void surf (Input IN, inout SurfaceOutputStandard o) {
            float dist = distance( fixed3(0,0,0), IN.worldPos);
            float val = abs(sin(dist*3.0-_Time*_A));
            if( val > 0.98 ){
            //yellow
                o.Albedo = fixed4(200/255.0, 200/255.0, 0/255.0,  0);
                o.Alpha = 0.8;
            } else {
                o.Albedo = fixed4(1, 1, 1, 0);
            }
        }
        ENDCG
    }
    FallBack "Diffuse"
}
