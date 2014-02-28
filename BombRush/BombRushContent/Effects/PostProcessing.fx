
float EdgeWidth = 1;
float EdgeIntensity = 1;
float NormalThreshold = 0.5;
float DepthThreshold = 0.1;
float NormalSensitivity = 1;
float DepthSensitivity = 10;

float2 ScreenResolution;
texture SceneTexture;
texture NormalDepthTexture;
texture DepthTexture;

sampler SceneSampler : register(s0) = sampler_state
{
    Texture = (SceneTexture);
    MinFilter = Linear;
    MagFilter = Linear;
    AddressU = Clamp;
    AddressV = Clamp;
};

sampler DepthSampler : register(s1) = sampler_state
{
    Texture = (DepthTexture);
    MinFilter = Point;
    MagFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};

sampler NormalDepthSampler : register(s1) = sampler_state
{
    Texture = (NormalDepthTexture);
    MinFilter = Linear;
    MagFilter = Linear;
    AddressU = Clamp;
    AddressV = Clamp;
};

float4 PixelShaderFunction(float2 texCoord : TEXCOORD0) : COLOR0
{
    // Look up the original color from the main scene.
	float3 scene = tex2D(SceneSampler, texCoord);
    
    // Look up four values from the normal/depth texture, offset along the
    // four diagonals from the pixel we are currently shading.
    float2 edgeOffset = EdgeWidth / ScreenResolution;
        
    float4 n1 = tex2D(NormalDepthSampler, texCoord + float2(-1, -1) * edgeOffset);
    float4 n2 = tex2D(NormalDepthSampler, texCoord + float2( 1,  1) * edgeOffset);
    float4 n3 = tex2D(NormalDepthSampler, texCoord + float2(-1,  1) * edgeOffset);
    float4 n4 = tex2D(NormalDepthSampler, texCoord + float2( 1, -1) * edgeOffset);

    // Work out how much the normal and depth values are changing.
    float4 diagonalDelta = abs(n1 - n2) + abs(n3 - n4);

    float normalDelta = dot(diagonalDelta.xyz, 1);
    float depthDelta = diagonalDelta.w;
        
    // Filter out very small changes, in order to produce nice clean results.
    normalDelta = saturate((normalDelta - NormalThreshold) * NormalSensitivity);
    depthDelta = saturate((depthDelta - DepthThreshold) * DepthSensitivity);

    // Does this pixel lie on an edge?
    float edgeAmount = saturate(normalDelta + depthDelta) * EdgeIntensity;
        
    // Apply the edge detection result to the main scene color.
    scene *= (1 - edgeAmount);

    return float4(scene, 1);
}

technique EdgeDetect
{
    pass P0
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
