float4x4 World;
float4x4 View;
float4x4 Projection;
float4x4 LightMatrix;
texture CelTexture;
texture DiffuseTexture;
texture ShadowMap;
float3 LightDir;

sampler samplerCelTexture = sampler_state
{
	Texture = <CelTexture>;
	MINFILTER = POINT;
	MAGFILTER = POINT;
	MIPFILTER = POINT;
};

sampler samplerDiffuseTexture = sampler_state
{
	Texture = <DiffuseTexture>;
	MINFILTER = ANISOTROPIC;
	MAGFILTER = ANISOTROPIC;
	MIPFILTER = POINT;
	AddressU = Wrap;
	AddressV = Wrap;
};

sampler ShadowMapSampler = sampler_state {
   Texture=<ShadowMap>;
   MinFilter = POINT;
   MipFilter = POINT;
   MagFilter = POINT;
};

struct CelShaderVSI
{
    float4 Position : POSITION0;
	float3 Normal   : NORMAL0;
	float2 TexCoord : TEXCOORD0;
};

struct CelShaderVSO
{
    float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
	float3 Normal   : TEXCOORD1;
	float4 Pos2DAsSeenByLight : TEXCOORD2;
};

CelShaderVSO CelShaderVSFunction(CelShaderVSI input)
{
    CelShaderVSO output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
	output.Normal = mul(input.Normal, World);
	output.TexCoord = input.TexCoord;
	output.Pos2DAsSeenByLight = mul(worldPosition, LightMatrix);

    return output;
}

float4 CelShaderPSFunction(CelShaderVSO input) : COLOR0
{
	float diffuse = saturate(dot(normalize(input.Normal), normalize(-LightDir)));
	float4 color = tex2D(samplerCelTexture, float2(diffuse, 0));

	float2 projectedTexCoords;
	projectedTexCoords.x = input.Pos2DAsSeenByLight.x/input.Pos2DAsSeenByLight.w/2.0f + 0.5f;
	projectedTexCoords.y = -input.Pos2DAsSeenByLight.y/input.Pos2DAsSeenByLight.w/2.0f + 0.5f;

	float shadowFactor = 0.5;
	if ((saturate(projectedTexCoords).x == projectedTexCoords.x) && (saturate(projectedTexCoords).y == projectedTexCoords.y))
	{
		float depthStoredInShadowMap = tex2D(ShadowMapSampler, projectedTexCoords).x;
		float realDistance = input.Pos2DAsSeenByLight.z/input.Pos2DAsSeenByLight.w;

		if ((realDistance - 1.0f/100.0f) <= depthStoredInShadowMap)
		{
			shadowFactor = 1.0f;
		}
	}

	return tex2D(samplerDiffuseTexture, input.TexCoord) * color * diffuse * shadowFactor;
}

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float3 Normal : NORMAL0;
};

struct NormalDepthVertexShaderOutput
{
    float4 Position : POSITION;
    float4 Color : COLOR0;
};

NormalDepthVertexShaderOutput NormalDepthVertexShader(VertexShaderInput input)
{
    NormalDepthVertexShaderOutput output;

    output.Position = mul(mul(mul(input.Position, World), View), Projection);

    float3 worldNormal = mul(input.Normal, World);
    output.Color.rgb = (worldNormal + 1) / 2;
    output.Color.a = output.Position.z / output.Position.w;
    
    return output;    
}

float4 NormalDepthPixelShader(float4 color : COLOR0) : COLOR0
{
    return color;
}

//--------------------------------------------
//Shadow Depth Calculation Shader
//--------------------------------------------

struct ShadowVSOut
{
	float4 Position : POSITION0;
	float4 PositionSeenByLight : TEXCOORD0;
};

ShadowVSOut ShadowDepthVS(float4 position : POSITION0)
{
	ShadowVSOut output;
	float4 worldPosition = mul(position, World);
	output.Position = mul(worldPosition, LightMatrix);
	output.PositionSeenByLight = output.Position;

	return output;
}

float4 ShadowDepthPS(ShadowVSOut input) : COLOR0
{
	return input.PositionSeenByLight.z/input.PositionSeenByLight.w;
}

sampler defaultSampler;

float4 DepthDebugPS(float4 pos : POSITION0, float2 tex : TEXCOORD0) : COLOR0
{
	float f = tex2D(defaultSampler, tex).x;
	return float4(f,f,f,1.0f);
}

technique DepthDebug
{
	pass p0
	{
		PixelShader = compile ps_2_0 DepthDebugPS();
	}
}

technique ShadowDepth
{
	pass Pass1
	{
		CullMode = CCW;
		VertexShader = compile vs_2_0 ShadowDepthVS();
		PixelShader = compile ps_2_0 ShadowDepthPS();
	}
}

technique CelShading
{
    pass Pass1
    {
		CullMode = CCW;
        VertexShader = compile vs_2_0 CelShaderVSFunction();
        PixelShader = compile ps_2_0 CelShaderPSFunction();
    }
}

technique NormalDepth
{
    pass P0
    {
        VertexShader = compile vs_2_0 NormalDepthVertexShader();
        PixelShader = compile ps_2_0 NormalDepthPixelShader();
    }
}