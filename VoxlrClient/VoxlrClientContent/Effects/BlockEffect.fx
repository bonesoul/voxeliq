float4x4 World; // world matrix.
float4x4 View; // the camera-view matrix.
float4x4 Projection; // the camera-projection.
float3 CameraPosition; // the camera-position.

float FogNear;
float FogFar;
float4 FogColor = {0.5,0.5,0.5,1.0};
float3 SunColor = float3(1,1,1);

Texture BlockTextureAtlas;
sampler BlockTextureAtlasSampler = sampler_state
{
	texture = <BlockTextureAtlas>;
	magfilter = POINT; // filter for objects smaller than actual.
	minfilter = POINT; // filter for objects larger than actual.
	mipfilter = POINT; // filter for resizing the image up close and far away.
	AddressU = WRAP;
	AddressV = WRAP;
};

struct VertexShaderInput
{
    float4 Position				: POSITION0;	
	float2 blockTextureCoord	: TEXCOORD0;	// block texture uv-mapping coordinates.
	float SunLight				: COLOR0;		// crack texture uv-mapping coordinates.
    //float3 LocalLight			: COLOR1;		// ambient occlusion weight.
};

struct VertexShaderOutput
{
    float4 Position				: POSITION0;
    float2 blockTextureCoord	: TEXCOORD0;
    float3 CameraView			: TEXCOORD1;
    float Distance				: TEXCOORD2;
	float4 Color				: COLOR0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
    output.CameraView = normalize(CameraPosition - worldPosition);
    output.Distance = length(CameraPosition - worldPosition);

    output.blockTextureCoord = input.blockTextureCoord;
	output.Color.rgb = (SunColor * input.SunLight); // + (input.LocalLight.rgb);
	output.Color.a = 1;

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float4 blockTextureColor = tex2D(BlockTextureAtlasSampler, input.blockTextureCoord);
    float fog = saturate((input.Distance - FogNear) / (FogNear-FogFar));    

    float4 color;
	color.rgb  = blockTextureColor.rgb * input.Color.rgb;
	color.a = blockTextureColor.a;
    if(color.a == 0) { clip(-1); }

    return lerp(FogColor, color ,fog);
}

technique BlockTechnique
{
    pass Pass1
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
