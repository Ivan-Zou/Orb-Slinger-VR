Shader "Custom/GravityPadMask"
{
    SubShader
    {
        Tags { "Queue" = "Background" }
        ZWrite Off
        ColorMask 0
        Pass
        {
            Stencil
            {
                Ref 1
                Comp Always
                Pass Replace
            }
            ZTest Always
        }
    }
}
