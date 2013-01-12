// note: in order to get monogame-dx working, SV_POSITION has to be used instead of POSITION0
// which discussed here: http://monogame.codeplex.com/discussions/402753

float4x4 World; // world matrix.
float4x4 View; // the camera-view matrix.
float4x4 Projection; // the camera-projection.
float3 CameraPosition; // the camera-position.

float TimeOfDay; // Time of day.

float4 HorizonColor; // Horizon color used for fogging.
float4 SunColor;		
float4 NightColor;

float4 MorningTint;		
float4 EveningTint;	

float FogNear; // Near fog plane.
float FogFar; // Far fog plane.

Texture BlockTextureAtlas;
sampler BlockTextureAtlasSampler = sampler_state
{
	texture = <BlockTextureAtlas>;
	magfilter = point; // filter for objects smaller than actual.
	minfilter = point; // filter for objects larger than actual.
	mipfilter = point; // filter for resizing the image up close and far away.
	AddressU = WRAP;
	AddressV = WRAP;
};

struct VertexShaderInput
{
    float4 Position				: SV_POSITION;	
	float2 blockTextureCoord	: TEXCOORD0;	// block texture uv-mapping coordinates.
	float SunLight				: COLOR0;		// crack texture uv-mapping coordinates.
    //float3 LocalLight			: COLOR1;		// ambient occlusion weight.
};

struct VertexShaderOutput
{
    float4 Position				: SV_POSITION;
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

	float sunColor = SunColor;

	if(TimeOfDay <= 12)
		sunColor *= TimeOfDay / 12;	
	else
		sunColor *= (TimeOfDay - 24) / -12;	

	output.Color.rgb = (sunColor * input.SunLight); // + (input.LocalLight.rgb);
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

	float4 sunColor = SunColor;	 
	float4 fogColor = HorizonColor;
    float4 nightColor = NightColor;

	nightColor *= (4 - input.blockTextureCoord.y) * .125f;

	if(TimeOfDay <= 12)
		fogColor *= TimeOfDay / 12;
	else
		fogColor *= (TimeOfDay - 24) / -12;	

	fogColor += (MorningTint * .05) * ((24 - TimeOfDay)/24);
	fogColor += (EveningTint * .05) * (TimeOfDay / 24);	
	sunColor += nightColor;
	fogColor += nightColor;

    return lerp(fogColor, color ,fog);
}

technique BlockTechnique
{
    pass Pass1
    {
        VertexShader = compile vs_4_0 VertexShaderFunction();
        PixelShader = compile ps_4_0 PixelShaderFunction();
    }
}
