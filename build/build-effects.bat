..\dep\monogame\tools\2MGFX.exe ..\src\Content\Effects\BlockEffect.fx ..\src\Content\Effects\BlockEffect.mgfxo /DEBUG 
..\dep\monogame\tools\2MGFX.exe ..\src\Content\Effects\DualTextured.fx ..\src\Content\Effects\DualTextured.mgfxo /DEBUG 
..\dep\monogame\tools\2MGFX.exe ..\src\Content\Effects\PerlinNoise.fx ..\src\Content\Effects\PerlinNoise.mgfxo /DEBUG 
..\dep\monogame\tools\2MGFX.exe ..\src\Content\Effects\SkyDome.fx ..\src\Content\Effects\SkyDome.mgfxo /DEBUG 

copy ..\src\Content\Effects\BlockEffect.mgfxo ..\bin\debug\windows\monogame\Content\Effects\BlockEffect.mgfxo
copy ..\src\Content\Effects\DualTextured.mgfxo ..\bin\debug\windows\monogame\Content\Effects\DualTextured.mgfxo
copy ..\src\Content\Effects\PerlinNoise.mgfxo ..\bin\debug\windows\monogame\Content\Effects\PerlinNoise.mgfxo
copy ..\src\Content\Effects\SkyDome.mgfxo ..\bin\debug\windows\monogame\Content\Effects\SkyDome.mgfxo

..\dep\monogame\tools\2MGFX.exe ..\src\Content\Effects\PostProcessing\Bloom\BloomCombine.fx ..\src\Content\Effects\PostProcessing\Bloom\BloomCombine.mgfxo /DEBUG 
..\dep\monogame\tools\2MGFX.exe ..\src\Content\Effects\PostProcessing\Bloom\BloomExtract.fx ..\src\Content\Effects\PostProcessing\Bloom\BloomExtract.mgfxo /DEBUG 
..\dep\monogame\tools\2MGFX.exe ..\src\Content\Effects\PostProcessing\Bloom\GaussianBlur.fx ..\src\Content\Effects\PostProcessing\Bloom\GaussianBlur.mgfxo /DEBUG 

copy ..\src\Content\Effects\PostProcessing\Bloom\BloomCombine.mgfxo ..\bin\debug\windows\monogame\Content\Effects\PostProcessing\Bloom\BloomCombine.mgfxo
copy ..\src\Content\Effects\PostProcessing\Bloom\BloomExtract.mgfxo ..\bin\debug\windows\monogame\Content\Effects\PostProcessing\Bloom\BloomExtract.mgfxo
copy ..\src\Content\Effects\PostProcessing\Bloom\GaussianBlur.mgfxo ..\bin\debug\windows\monogame\Content\Effects\PostProcessing\Bloom\GaussianBlur.mgfxo

exit 0
