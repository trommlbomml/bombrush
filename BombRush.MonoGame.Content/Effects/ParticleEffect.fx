float4x4 WorldViewProjection;
Texture DiffuseTexture;

struct ShaderData
{
    float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
};

sampler BaseTextureSampler = sampler_state
{
	Texture = <DiffuseTexture>;
	MinFilter = LINEAR;
	MagFilter = LINEAR;
};

ShaderData VertexShaderFunction(ShaderData input)
{
    ShaderData output;
	output.Position = mul(input.Position, WorldViewProjection);
	output.TexCoord = input.TexCoord;
	return output;
}

float4 PixelShaderFunction(ShaderData input) : COLOR0
{
    return tex2D(BaseTextureSampler, input.TexCoord);
}

technique ParticelTechnique
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
