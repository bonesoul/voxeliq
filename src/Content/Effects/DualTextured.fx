float4x4 World; // world matrix.
float4x4 View; // the camera-view matrix.
float4x4 Projection; // the camera-projection.
float3 CameraPosition; // the camera-position.

//float4 AmbientColor;
//float AmbientIntensity;

float FogNear;
float FogFar;
float4 FogColor = {0.5,0.5,0.5,1.0};

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

Texture CrackTextureAtlas;
sampler CrackTextureAtlasSampler = sampler_state
{
	texture = <CrackTextureAtlas>;
	magfilter = POINT; 
	minfilter = POINT;
	mipfilter = POINT;
	AddressU = WRAP;
	AddressV = WRAP;
};

struct VertexShaderInput
{
    float4 Position : POSITION0;	
	float2 blockTextureCoord : TEXCOORD0;  // block texture uv-mapping coordinates.
	float2 crackTextureCoord : TEXCOORD1;  // crack texture uv-mapping coordinates.
	float ambientOcclusionWeight : COLOR0; // ambient occlusion weight.
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float2 blockTextureCoord : TEXCOORD0;
	float2 crackTextureCoord : TEXCOORD1;
    float3 CameraView : TEXCOORD2;
    float Distance : TEXCOORD3;
	float4 Color : COLOR0;
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
	output.crackTextureCoord = input.crackTextureCoord;

	float3 aoColor = float3(1,1,1);
	output.Color.rgb = aoColor * input.ambientOcclusionWeight;
	output.Color.a = 1;

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float4 blockTextureColor = tex2D(BlockTextureAtlasSampler, input.blockTextureCoord);
	float4 crackTextureCoord = tex2D(CrackTextureAtlasSampler, input.crackTextureCoord);
	//float4 ambient = AmbientIntensity * AmbientColor;	    
    float fog = saturate((input.Distance - FogNear) / (FogNear-FogFar));    

    float4 color;
	color.rgb  =  (blockTextureColor.rgb * crackTextureCoord.rgb) * input.Color.rgb;
	color.a = blockTextureColor.a * crackTextureCoord.a;

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
