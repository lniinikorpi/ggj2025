Shader "K1mpp4/Unlit/TextureToon"
{
    Properties
    {
        [HDR]_Color("Albedo Tint", Color) = (1,1,1,1)
        _MainTex("Texture", 2D) = "white" {}
        
        // Toon-specific properties
        _Threshold("Toon Threshold", Range(0,1)) = 0.5
        _LightColorMultiplier("Light Color Multiplier", Range(0,2)) = 1.0

        [Header(Stencil)]
        _Stencil ("Stencil ID [0;255]", Float) = 0
        _ReadMask ("ReadMask [0;255]", Int) = 255
        _WriteMask ("WriteMask [0;255]", Int) = 255
        [Enum(UnityEngine.Rendering.CompareFunction)] _StencilComp ("Stencil Comparison", Int) = 0
        [Enum(UnityEngine.Rendering.StencilOp)] _StencilOp ("Stencil Operation", Int) = 0
        [Enum(UnityEngine.Rendering.StencilOp)] _StencilFail ("Stencil Fail", Int) = 0
        [Enum(UnityEngine.Rendering.StencilOp)] _StencilZFail ("Stencil ZFail", Int) = 0
        
        [Header(Rendering)]
        _Offset("Offset", float) = 0
        [Enum(UnityEngine.Rendering.CullMode)] _Culling ("Cull Mode", Int) = 2
        [Enum(Off,0,On,1)] _ZWrite("ZWrite", Int) = 1
        [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest ("ZTest", Int) = 4
        [Enum(None,0,Alpha,1,Red,8,Green,4,Blue,2,RGB,14,RGBA,15)] _ColorMask("Color Mask", Int) = 15
    }

    CGINCLUDE
    #include "UnityCG.cginc"
    #include "AutoLight.cginc"        // For shadow macros (SHADOW_ATTENUATION, etc.)

    // User properties
    half4 _Color;
    sampler2D _MainTex;
    float4 _MainTex_ST;

    // Toon properties
    float _Threshold;
    float _LightColorMultiplier;

    // Unity main directional light direction: _WorldSpaceLightPos0
    // Unity main directional light color: _LightColor0 (in some older pipelines or forward base pass)

    struct appdata
    {
        float4 vertex : POSITION;
        float3 normal : NORMAL;   // We need the normal for lighting
        float2 uv     : TEXCOORD0;
    };

    struct v2f
    {
        float2 uv       : TEXCOORD0;
        float4 vertex   : SV_POSITION;
        float3 worldN   : TEXCOORD1;   // pass the transformed normal
        float4 shadowCoord : TEXCOORD2; // for shadow sampling
        UNITY_VERTEX_OUTPUT_STEREO
    };

    // Vertex shader: transform vertex, compute world normal, prepare shadow coordinates
    v2f vert (appdata v)
    {
        v2f o;
        UNITY_SETUP_INSTANCE_ID(v);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

        // Position
        o.vertex = UnityObjectToClipPos(v.vertex);
        // UV
        o.uv = TRANSFORM_TEX(v.uv, _MainTex);

        // Compute world normal for lighting
        float3 worldNormal = UnityObjectToWorldNormal(v.normal);
        o.worldN = normalize(worldNormal);

        // Prepare shadow coordinates (for shadow attenuation)
        // This uses an AutoLight.cginc macro
        o.shadowCoord = TransformWorldToShadowCoord(
            mul(unity_ObjectToWorld, v.vertex)
        );

        return o;
    }

    // Fragment shader: do basic toon shading + shadowing
    half4 frag (v2f i) : SV_Target
    {
        // Sample base texture
        half4 baseColor = tex2D(_MainTex, i.uv) * _Color;

        // Direction of main directional light (world space).
        // Typically _WorldSpaceLightPos0.w < 0 for directional lights,
        // so the direction is -_WorldSpaceLightPos0.xyz (normalized).
        float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);

        // Simple lambert (N dot L), saturate to [0,1]
        float NdotL = dot(i.worldN, -lightDir);
        NdotL = saturate(NdotL);

        // Toon step (simple one-step)
        // If NdotL > _Threshold, we get a bright band; otherwise we get a dark band
        float toonFactor = NdotL > _Threshold ? 1.0 : 0.0;

        // Get shadow attenuation (0 to 1)
        // If there's no real-time shadow-casting light, this should default to 1
        half shadow = SHADOW_ATTENUATION(i);

        // Combine results:
        //  - "toonFactor" for simple banding
        //  - "shadow" for real-time shadow
        //  - multiply by optional _LightColor0 if you want to incorporate the main light color
        //    Note that in some rendering paths, _LightColor0 might only be set in forward base pass
        half3 mainLightColor = _LightColor0.rgb * _LightColorMultiplier;

        // final shading factor
        half3 lighting = toonFactor * shadow * mainLightColor;

        // Multiply base color by the lighting factor (applied to rgb only, keep alpha from baseColor)
        baseColor.rgb *= lighting;

        return baseColor;
    }

    // Shadow caster pass structures
    struct v2fShadow {
        V2F_SHADOW_CASTER;
        UNITY_VERTEX_OUTPUT_STEREO
    };

    v2fShadow vertShadow(appdata_base v)
    {
        v2fShadow o;
        UNITY_SETUP_INSTANCE_ID(v);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
        TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
        return o;
    }

    float4 fragShadow(v2fShadow i) : SV_Target
    {
        SHADOW_CASTER_FRAGMENT(i)
    }

    ENDCG

    SubShader
    {
        Stencil
        {
            Ref [_Stencil]
            ReadMask [_ReadMask]
            WriteMask [_WriteMask]
            Comp [_StencilComp]
            Pass [_StencilOp]
            Fail [_StencilFail]
            ZFail [_StencilZFail]
        }

        // Main Pass: includes "forward base" so we get the main light + shadows
        Pass
        {
            Tags
            {
                "LightMode"="ForwardBase"      // Important so Unity sends main light + shadows
                "RenderType"="Opaque"
                "Queue"="Geometry"
            }

            LOD 100
            Cull [_Culling]
            Offset [_Offset], [_Offset]
            ZWrite [_ZWrite]
            ZTest [_ZTest]
            ColorMask [_ColorMask]

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            // Make sure to compile with forward base lighting and shadows:
            #pragma multi_compile_fwdbase
            #pragma multi_compile_shadowcaster
            ENDCG
        }

        // Pass to render object as a shadow caster
        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }
            LOD 80
            Cull [_Culling]
            Offset [_Offset], [_Offset]
            ZWrite [_ZWrite]
            ZTest [_ZTest]

            CGPROGRAM
            #pragma vertex vertShadow
            #pragma fragment fragShadow
            #pragma target 2.0
            #pragma multi_compile_shadowcaster
            ENDCG
        }
    }
}
