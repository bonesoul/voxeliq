//------- Constants --------
float4x4 xView;
float4x4 xProjection;
float4x4 xWorld;

float4 HorizonColor;
float4 SunColor;		
float4 NightColor;

float4 MorningTint;		
float4 EveningTint;	

float timeOfDay;
 
//------- Texture Samplers --------
Texture xTexture;

sampler TextureSampler = sampler_state { texture = <xTexture> ; magfilter = linear; minfilter = linear; mipfilter=linear; AddressU = mirror; AddressV = mirror;};Texture xTexture0;

//------- Technique: SkyDome --------
 struct SDVertexToPixel
 {    
     float4 Position         : POSITION;
     float2 TextureCoords    : TEXCOORD0;
     float4 ObjectPosition    : TEXCOORD1;
 };
 
 struct SDPixelToFrame
 {
     float4 Color : COLOR0;
 };
 
 SDVertexToPixel SkyDomeVS( float4 inPos : POSITION, float2 inTexCoords: TEXCOORD0)
 {    
     SDVertexToPixel Output = (SDVertexToPixel)0;
     float4x4 preViewProjection = mul (xView, xProjection);
     float4x4 preWorldViewProjection = mul (xWorld, preViewProjection);
     
     Output.Position = mul(inPos, preWorldViewProjection);
     Output.TextureCoords = inTexCoords;
     Output.ObjectPosition = inPos;
     
     return Output;    
 }
 
 SDPixelToFrame SkyDomePS(SDVertexToPixel PSIn)
 {
     SDPixelToFrame Output = (SDPixelToFrame)0;        
 
	 float4 topColor = SunColor;
     float4 bottomColor = HorizonColor;    
	 float4 nColor = NightColor;

	 nColor *= (4 - PSIn.TextureCoords.y) * .125f;

     float4 cloudValue = tex2D(TextureSampler, PSIn.TextureCoords).r;

	 if(timeOfDay <= 12)
	 {
		bottomColor *= timeOfDay / 12;	
		topColor	*= timeOfDay / 12;	
		nColor		*= timeOfDay / 12;
		cloudValue	*= timeOfDay / 12;
	 }
	 else
	 {
		bottomColor *= (timeOfDay - 24) / -12;	
		topColor	*= (timeOfDay - 24) / -12;						
		nColor		*= (timeOfDay - 24) / -12;
		cloudValue	*= (timeOfDay - 24) / -12;
	 }

	 bottomColor += (MorningTint * .05) * ((24 - timeOfDay)/24);
	 bottomColor += (EveningTint * .05) * (timeOfDay / 24);	
	 topColor += nColor;
	 bottomColor += nColor;

     float4 baseColor = lerp(bottomColor, topColor, saturate((PSIn.ObjectPosition.y)/0.9f));
	 float4 outCloudValue = lerp(bottomColor, cloudValue, saturate((PSIn.ObjectPosition.y)/0.5f));

     Output.Color = lerp(baseColor, 1, outCloudValue);        
 
     return Output;
 }

 SDVertexToPixel SkyStarDomeVS( float4 inPos : POSITION, float2 inTexCoords: TEXCOORD0)
 {    
     SDVertexToPixel Output = (SDVertexToPixel)0;
     float4x4 preViewProjection = mul (xView, xProjection);
     float4x4 preWorldViewProjection = mul (xWorld, preViewProjection);
     
     Output.Position = mul(inPos, preWorldViewProjection);
     Output.TextureCoords = inTexCoords;
     Output.ObjectPosition = inPos;
     
     return Output;    
 }

 SDPixelToFrame SkyStarDomePS(SDVertexToPixel PSIn)
 {
     SDPixelToFrame Output = (SDPixelToFrame)0;        
 
	 float4 topColor = SunColor;
     float4 bottomColor = HorizonColor;    
	 float4 nColor = NightColor;

	 nColor *= (4 - PSIn.TextureCoords.y) * .125f;

     float4 cloudValue = tex2D(TextureSampler, PSIn.TextureCoords).r;

	 if(timeOfDay <= 12)
	 {
		bottomColor *= timeOfDay / 12;	
		topColor	*= timeOfDay / 12;	
		nColor		*= timeOfDay / 12;
		cloudValue	*= (timeOfDay - 24) / -12;
	 }
	 else
	 {
		bottomColor *= (timeOfDay - 24) / -12;	
		topColor	*= (timeOfDay - 24) / -12;						
		nColor		*= (timeOfDay - 24) / -12;
		cloudValue	*= timeOfDay / 12;
	 }

	 bottomColor += (MorningTint * .05) * ((24 - timeOfDay)/24);
	 bottomColor += (EveningTint * .05) * (timeOfDay / 24);	
	 topColor += nColor;
	 bottomColor += nColor;

     float4 baseColor = lerp(bottomColor, topColor, saturate((PSIn.ObjectPosition.y)/0.9f));
	 float4 outCloudValue = lerp(bottomColor, cloudValue, saturate((PSIn.ObjectPosition.y)/0.5f));

     Output.Color = lerp(baseColor, 1, outCloudValue);        
 
     return Output;
 }

 technique SkyDome
 {
     pass Pass0
     {
         VertexShader = compile vs_4_0 SkyDomeVS();
         PixelShader = compile ps_4_0 SkyDomePS();
     }
 }

 technique SkyStarDome
 {
     pass Pass0
     {
         VertexShader = compile vs_4_0 SkyStarDomeVS();
         PixelShader = compile ps_4_0 SkyStarDomePS();
     }
 }