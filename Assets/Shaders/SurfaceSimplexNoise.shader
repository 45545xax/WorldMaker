Shader "Noise/PlanetarySurface"
{
    Properties
    {
        _TemperatureSeed("Temperature Seed", Int) = 345
        _HumiditySeed("Humidity Seed", Int) = 987
        _WaterLevel("Water Level", Range(0, 1)) = 0.66
        [Enum(SphereShaderDrawType)] _DrawType("Draw Type", Float) = 0

        _Seed("Land Seed", Int) = 2343
        _XOffset("X Offset", Range(0, 1)) = 0
        _YOffset("Y Offset", Range(0, 1)) = 0
        _ZOffset("Z Offset", Range(0, 1)) = 0
        _MinHeight("Min Height", Range(0, 1)) = 0.15
        _MaxHeight("Max Height", Range(0, 1)) = 0.5
        _Multiplier("Noise Multiplier", Range(0.5, 10)) = 1
        _Octaves("Number of Octaves", Range(1, 30)) = 10
        _Lacunarity("Lacunarity", Range(1, 2)) = 1.5
        _Persistence("Persistance", Range(0, 1)) = 0.7
        _LayerStrength("Layer Strength", Range(0, 1)) = 1
        _HeightExponent("Height Exponent", Range(0, 10)) = 1
        _RidgedNoise("Ridged Noise", Int) = 0
        _DomainWarping("Domain Warping", Int) = 0

        _Seed2("Land Seed 2", Int) = 2343
        _XOffset2("X Offset 2", Range(0, 1)) = 0
        _YOffset2("Y Offset 2", Range(0, 1)) = 0
        _ZOffset2("Z Offset 2", Range(0, 1)) = 0
        _Multiplier2("Noise Multiplier 2", Range(0.5, 10)) = 1
        _Octaves2("Number of Octaves 2", Range(1, 30)) = 10
        _Lacunarity2("Lacunarity 2", Range(1, 2)) = 1.5
        _Persistence2("Persistance 2", Range(0, 1)) = 0.7
        _LayerStrength2("Layer Strength 2", Range(0, 1)) = 1
        _HeightExponent2("Height Exponent 2", Range(0, 10)) = 1
        _RidgedNoise2("Ridged Noise 2", Int) = 0
        _DomainWarping2("Domain Warping2", Int) = 0

        //_Color ("Color", Color) = (1,1,1,1)
        _HeightMap("Heightmap", 2D) = "white" {}
        _IsHeightmapSet("Is Heightmap Set", Int) = 0
        _IsEroded("Is Eroded Heightmap", Int) = 0
        _MainMap("Main Map", 2D) = "white" {}
        _IsMainmapSet("Is Main Map Set", Int) = 0
        _LandMask("Land Mask", 2D) = "white" {}
        _IsLandmaskSet("Is Land Mask Set", Int) = 0
        _Glossiness("Smoothness", Range(0,1)) = 0.7
        _Metallic("Metallicity", Range(0,1)) = 0.4
        _LandGlossiness("Land Smoothness", Range(0,1)) = 0.25
        _LandMetallic("Land Metallicity", Range(0,1)) = 0.1

        _FlowTex("FlowMap", 2D) = "black" {}
        _IsFlowTexSet("Is Flow Texture Set", Int) = 0

        _ColorSteps("Color Steps", Int) = 6
        _ColorStep1("Color Step 1", Range(0, 1)) = 0.01
        _Color1("Color 1", Color) = (0.69, 0.63, 0.43)
        _ColorStep2("Color Step 2", Range(0, 1)) = 0.1
        _Color2("Color 2", Color) = (0.5, 0.5, 0.27)
        _ColorStep3("Color Step 3", Range(0, 1)) = 0.6
        _Color3("Color 3", Color) = (0.2, 0.27, 0.03)
        _ColorStep4("Color Step 4", Range(0, 1)) = 0.8
        _Color4("Color 4", Color) = (0.53, 0.54, 0.36)
        _ColorStep5("Color Step 5", Range(0, 1)) = 0.95
        _Color5("Color 5", Color) = (0.45, 0.41, 0.32)
        _ColorStep6("Color Step 6", Range(0, 1)) = 1.0
        _Color6("Color 6", Color) = (0.95, 0.95, 0.95)
        _ColorStep7("Color Step 7", Range(0, 1)) = 1.0
        _Color7("Color 7", Color) = (0.95, 0.95, 0.95)
        _ColorStep8("Color Step 8", Range(0, 1)) = 1.0
        _Color8("Color 8", Color) = (0.95, 0.95, 0.95)

        _OceanColorSteps("OceanColor Steps", Int) = 3
        _OceanColorStep1("Ocean Color Step 1", Range(0, 1)) = 0
        _OceanColor1("Ocean Color 1", Color) = (0.25, 0.33, 0.39)
        _OceanColorStep2("Ocean Color Step 2", Range(0, 1)) = 0.5
        _OceanColor2("Ocean Color 2", Color) = (0.19, 0.21, 0.29)
        _OceanColorStep3("Ocean Color Step 3", Range(0, 1)) = 1
        _OceanColor3("Ocean Color 3", Color) = (0.19, 0.2, 0.27)
        _OceanColorStep4("Ocean Color Step 4", Range(0, 1)) = 1
        _OceanColor4("Ocean Color 4", Color) = (0.19, 0.2, 0.27)

        _IceTemperatureThreshold1("Ice Threshold 1", Float) = 0
        _IceTemperatureThreshold2("Ice Threshold 2", Float) = -10
        _DesertThreshold1("Desert Threshold 1", Float) = 10
        _DesertThreshold2("Desert Threshold 2", Float) = 20
        _HighHumidityLightnessPercentage("Humidity Lightness", Range(0,1)) = 0.2
        _IceColor("Ice Color", Color) = (0.95, 0.95, 0.95)
        _DesertColor("Desert Color", Color) = (0.75, 0.88, 0.55)
        _NormalScale("Normal Scale Multiplier", Float) = 50
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 200

            CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows
        #include "Assets/Shaders/Simplex.cginc"
        #include "Assets/Shaders/Sphere.cginc"

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        #define LONGITUDE_STEP 0.000244140f
        #define LATITUDE_STEP 0.000488281f
        #define PI 3.14159265359

        sampler2D_float _HeightMap;
        int _IsHeightmapSet;
        int _IsEroded;
        sampler2D_float _MainMap;
        int _IsMainmapSet;
        sampler2D_float _LandMask;
        int _IsLandmaskSet;
        sampler2D_float _FlowTex;
        int _IsFlowTexSet;

        struct Input
        {
            float2 uv_HeightMap;
            float3 worldNormal; INTERNAL_DATA
            float3 worldPos;
        };

        half _Glossiness;
        half _Metallic;
        half _LandGlossiness;
        half _LandMetallic;
        float _Multiplier;
        int _Seed;
        int _TemperatureSeed;
        int _HumiditySeed;
        int _Octaves;
        float _Lacunarity;
        float _Persistence;
        float _LayerStrength;
        float _HeightExponent;
        int _Seed2;
        float _XOffset2;
        float _YOffset2;
        float _ZOffset2;
        float _MinHeight;
        float _MaxHeight;
        float _Multiplier2;
        int _Octaves2;
        float _Lacunarity2;
        float _Persistence2;
        float _LayerStrength2;
        float _HeightExponent2;
        int _RidgedNoise2;
        //fixed4 _Color;

        float _WaterLevel;
        float _ColorStep1;
        fixed4 _Color1;
        float _ColorStep2;
        fixed4 _Color2;
        float _ColorStep3;
        fixed4 _Color3;
        float _ColorStep4;
        fixed4 _Color4;
        float _ColorStep5;
        fixed4 _Color5;
        float _ColorStep6;
        fixed4 _Color6;
        float _ColorStep7;
        fixed4 _Color7;
        float _ColorStep8;
        fixed4 _Color8;

        float _OceanColorStep1;
        fixed4 _OceanColor1;
        float _OceanColorStep2;
        fixed4 _OceanColor2;
        float _OceanColorStep3;
        fixed4 _OceanColor3;
        float _OceanColorStep4;
        fixed4 _OceanColor4;

        float _IceTemperatureThreshold1;
        float _IceTemperatureThreshold2;
        float _DesertThreshold1;
        float _DesertThreshold2;
        float _HighHumidityLightnessPercentage;
        fixed4 _IceColor;
        fixed4 _DesertColor;
        float _NormalScale;

        float _DrawType;
        float _XOffset;
        float _YOffset;
        float _ZOffset;
        int _RidgedNoise;
        int _DomainWarping;
        int _DomainWarping2;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            float2 uv = IN.uv_HeightMap;
            if (uv.x > 2) uv.x -= 2;
            else if (uv.x > 1) uv.x -= 1;

            float height = 0;
            float3 offset = float3(_XOffset, _YOffset, _ZOffset);
            float3 offset2 = float3(_XOffset2, _YOffset2, _ZOffset2);

            if (_IsHeightmapSet > 0 || _IsEroded > 0)
            {
                //float4 c = pow(tex2D(_HeightMap, uv), 1/2.2);
                float4 c = tex2D(_HeightMap, uv);
                height = (c.r + c.g + c.b) / 3;
            }
            else
            {
                height = sphereHeight(uv, offset, _Seed, _Multiplier, _Octaves, _Lacunarity, _Persistence, _RidgedNoise, _HeightExponent, _LayerStrength, _DomainWarping,
                                                    offset2, _Seed2, _Multiplier2, _Octaves2, _Lacunarity2, _Persistence2, _RidgedNoise2, _HeightExponent2, _LayerStrength2, _DomainWarping2);
                height = (height - _MinHeight) / (_MaxHeight - _MinHeight);
            }

            bool isAboveWater = height > _WaterLevel;

            if (_DrawType == 1) // Drawing a Heightmap.
            {
                o.Albedo = float4(height, height, height, 1);
                o.Metallic = 0;
                o.Smoothness = 0;
            }
            else if (_DrawType == 2) // Drawing a Landmask.
            {
                if (isAboveWater)
                {
                    o.Albedo = float3(1, 1, 1);
                }
                else
                {
                    o.Albedo = float3(0, 0, 0);
                }

                o.Metallic = 0;
                o.Smoothness = 0;
            }
            else if (_DrawType == 7) // Drawing a Landmask.
            {
                if (isAboveWater)
                {
                    o.Albedo = float3(0, 0, 0);
                }
                else
                {
                    o.Albedo = float3(1, 1, 1);
                }

                o.Metallic = 0;
                o.Smoothness = 0;
            }
            else
            {
                // Temperature
                float temperature = sphereNoise(uv, offset, _TemperatureSeed, _Multiplier, _Octaves, _Lacunarity, _Persistence, 0, 0);
                temperature -= 0.5;
                temperature *= 10;
                temperature += 0.5;
                // Adjusts temperature with a sigmoid curve.
                temperature = 1 / (1 + pow(20, -temperature));

                temperature *= 20; // From 0 to 20

                if (isAboveWater)
                {
                    // From -35 to 10
                    float elevationRatio = ((height - _WaterLevel) / (1 - _WaterLevel));
                    float temperatureDrop = (elevationRatio * 5) * 7; //World has 5km height, temperature falls 7 degrees for each km.
                    temperature -= temperatureDrop;
                }
                else
                {
                    temperature -= 5;
                }

                float actualLatitude = (abs(uv.y - 0.5) * 2);
                float latitudeTemperature = 0.9 - actualLatitude;
                //latitudeTemperature = pow(latitudeTemperature, 0.5f);
                latitudeTemperature *= 40; // From 0 to 10
                latitudeTemperature -= 15; // From -15 to 25
                temperature += latitudeTemperature; // -15 to 45

                // Humidity
                float humidity = sphereNoise(uv, offset, _HumiditySeed, _Multiplier, _Octaves, _Lacunarity, _Persistence, 0, 0);
                // Adjusts humidity with a sigmoid curve.
                humidity = 1 / (1 + pow(20, -humidity));

                if (_DrawType == 3) // Drawing a Temperature mask.
                {
                    o.Metallic = 0;
                    o.Smoothness = 0;
                    o.Albedo = temperatureColor(temperature);
                }
                else if (_DrawType == 5) // Drawing a Humidity mask.
                {
                    o.Metallic = 0;
                    o.Smoothness = 0;

                    if (!isAboveWater)
                        o.Albedo = float4(0, 0, 0.5, 1);
                    else
                    {
                        o.Albedo = humidityColor(humidity);
                    }
                }
                else
                {
                    float2 prevLongitude = float2(uv.x - LONGITUDE_STEP, uv.y);
                    float2 prevLatitude = float2(uv.x, uv.y - LATITUDE_STEP);
                    //float2 nextLongitude = float2(uv.x + LONGITUDE_STEP, uv.y);
                    //float2 nextLatitude = float2(uv.x, uv.y + LATITUDE_STEP);
                    if (prevLongitude.x < 0) prevLongitude.x += 1;
                    if (prevLatitude.y < 0)  prevLatitude.y = 0;
                    //if (nextLongitude.x > 0) nextLongitude.x -= 1;
                    //if (nextLatitude.y > 1)  nextLatitude.y = 1;

                    float prevLongitudeHeight = 0;
                    float prevLatitudeHeight = 0;
                    //float nextLongitudeHeight = 0;
                    //float nextLatitudeHeight = 0;

                    if (_IsHeightmapSet > 0 || _IsEroded > 0)
                    {
                        //float4 prevLongitudeColor = pow(tex2D(_HeightMap, prevLongitude), 1/2.2);
                        float4 prevLongitudeColor = tex2D(_HeightMap, prevLongitude);
                        prevLongitudeHeight = (prevLongitudeColor.r + prevLongitudeColor.g + prevLongitudeColor.b) / 3;
                        //float4 prevLatitudeColor = pow(tex2D(_HeightMap, prevLatitude), 1/2.2);
                        float4 prevLatitudeColor = tex2D(_HeightMap, prevLatitude);
                        prevLatitudeHeight = (prevLatitudeColor.r + prevLatitudeColor.g + prevLatitudeColor.b) / 3;

                        //float4 nextLongitudeColor = tex2D(_HeightMap, nextLongitude);
                        //nextLongitudeHeight = (nextLongitudeColor.r + nextLongitudeColor.g + nextLongitudeColor.b) / 3;

                        //float4 nextLatitudeColor = tex2D(_HeightMap, nextLatitude);
                        //nextLatitudeHeight = (nextLatitudeColor.r + nextLatitudeColor.g + nextLatitudeColor.b) / 3;
                    }
                    else
                    {
                        prevLongitudeHeight = sphereHeight(prevLongitude, offset, _Seed, _Multiplier, _Octaves, _Lacunarity, _Persistence, _RidgedNoise > 0, _HeightExponent, _LayerStrength, _DomainWarping > 0, offset2, _Seed2, _Multiplier2, _Octaves2, _Lacunarity2, _Persistence2, _RidgedNoise2 > 0, _HeightExponent2, _LayerStrength2, _DomainWarping2 > 0);
                        prevLatitudeHeight = sphereHeight(prevLatitude, offset, _Seed, _Multiplier, _Octaves, _Lacunarity, _Persistence, _RidgedNoise > 0, _HeightExponent, _LayerStrength, _DomainWarping > 0, offset2, _Seed2, _Multiplier2, _Octaves2, _Lacunarity2, _Persistence2, _RidgedNoise2 > 0, _HeightExponent2, _LayerStrength2, _DomainWarping2 > 0);
                        //nextLongitudeHeight = sphereHeight(nextLongitude, offset, _Seed, _Multiplier, _Octaves, _Lacunarity, _Persistence, _RidgedNoise > 0, _HeightExponent, _LayerStrength, _DomainWarping > 0, offset2, _Seed2, _Multiplier2, _Octaves2, _Lacunarity2, _Persistence2, _RidgedNoise2 > 0, _HeightExponent2, _LayerStrength2, _DomainWarping2 > 0);
                        //nextLatitudeHeight = sphereHeight(nextLatitude, offset, _Seed, _Multiplier, _Octaves, _Lacunarity, _Persistence, _RidgedNoise > 0, _HeightExponent, _LayerStrength, _DomainWarping > 0, offset2, _Seed2, _Multiplier2, _Octaves2, _Lacunarity2, _Persistence2, _RidgedNoise2 > 0, _HeightExponent2, _LayerStrength2, _DomainWarping2 > 0);
                        prevLongitudeHeight = (prevLongitudeHeight - _MinHeight) / (_MaxHeight - _MinHeight);
                        prevLatitudeHeight = (prevLatitudeHeight - _MinHeight) / (_MaxHeight - _MinHeight);
                    }

                    float verticalDeltaHeight = prevLatitudeHeight - height;
                    float horizontalDeltaHeight = prevLongitudeHeight - height;

                    if (_DrawType == 4) // Drawing a Normal mask.
                    {
                        o.Metallic = 0;
                        o.Smoothness = 0;

                        if (!isAboveWater)
                        {
                            float4 normalColor = float4(0.5, 0.5, 1, 0.5);

                            o.Albedo = normalColor;
                        }
                        else
                        {
                            float4 normalColor = float4(
                                horizontalDeltaHeight * _NormalScale + 0.5,
                                verticalDeltaHeight * _NormalScale + 0.5,
                                1,
                                horizontalDeltaHeight * _NormalScale + 0.5);

                            o.Albedo = normalColor;
                        }
                    }
                    else if (_DrawType == 0 || _DrawType == 6) // Drawing the main map.
                    {
                        // Get Land Color
                        float4 color = float4(0, 0, 0, 1);
                        int isflowColor = 0;
                        float4 flowColor = float4(0, 0, 0, 0);

                        if (_IsFlowTexSet)
                        {
                            flowColor = tex2D(_FlowTex, uv);
                            if ((flowColor.r != 0 || flowColor.g != 0 || flowColor.b != 0) && isAboveWater)
                            {
                                if (flowColor.a >= 1)
                                {
                                    color = flowColor;
                                    isflowColor = 1;
                                }
                            }
                        }

                        if (isflowColor == 0)
                        {
                            if (_IsMainmapSet)
                            {
                                color = tex2D(_MainMap, uv);
                            }
                            else
                            {
                                color = colorAtElevation(height, _WaterLevel, temperature, humidity,
                                    _IceTemperatureThreshold1, _IceTemperatureThreshold2,
                                    _DesertThreshold1, _DesertThreshold2,
                                    _HighHumidityLightnessPercentage, _IceColor, _DesertColor,
                                    _IsLandmaskSet, isAboveWater,
                                    _ColorStep1, _Color1, _ColorStep2, _Color2, _ColorStep3, _Color3, _ColorStep4, _Color4, _ColorStep5, _Color5, _ColorStep6, _Color6, _ColorStep7, _Color7, _ColorStep8, _Color8,
                                    _OceanColorStep1, _OceanColor1, _OceanColorStep2, _OceanColor2, _OceanColorStep3, _OceanColor3, _OceanColorStep4, _OceanColor4);

                                if (flowColor.a > 0 && isAboveWater)
                                {
                                    color = color * (1 - flowColor.a) + flowColor * flowColor.a;
                                    color.a = 1;
                                }
                            }
                        }

                        float3 normal = float3(0, 0, 1);
                        if (!isAboveWater)
                        {
                            o.Metallic = _Metallic;
                            o.Smoothness = _Glossiness;
                        }
                        else
                        {
                            o.Metallic = _LandMetallic;
                            o.Smoothness = _LandGlossiness;

                            normal = float3(
                                horizontalDeltaHeight * _NormalScale + 0.5,
                                verticalDeltaHeight * _NormalScale + 0.5,
                                1);
                        }
                        normal = normalize(normal);

                        if (_DrawType == 0) // Land
                        {
                            o.Albedo = color;
                            o.Normal = normal;
                        }
                        else if (_DrawType == 6) // Land with baked normals.
                        {
                            float3 lightRay = float3(1.25, 1.25, 0.7);
                            float projection = dot(normal, lightRay);
                            color *= projection * 0.75;
                            o.Albedo = color;
                        }
                    }
                }
            }
            o.Alpha = 1;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
