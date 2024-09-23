<div align="center">
  <img src="https://github.com/user-attachments/assets/6212ba26-641e-497b-ad6a-7d4d3bd9dc0f" width="500" alt="Miyu_Early"/>
</div>

# PostProcessing-URP
![Unity Version](https://img.shields.io/badge/Unity-2022.3%20LTS%2B-blueviolet?logo=unity)
![Unity Pipeline Support (Built-In)](https://img.shields.io/badge/BiRP_❌-darkgreen?logo=unity)
![Unity Pipeline Support (URP)](https://img.shields.io/badge/URP_✔️-blue?logo=unity)
![Unity Pipeline Support (HDRP)](https://img.shields.io/badge/HDRP_❌-darkred?logo=unity)

A series of post-processing effects for Unity URP (2022.3.20f1) as part of my self study on shader programming.

## Notes
This is an early preview of what I have so far, and there are plans to add some effects (as there are other effects that I created or adapted but haven't uploaded to the repo yet) and 
to redo some of these effects myself to simplify, clean them up, challenge myself to make them from memory, and maybe improve them. 
Some of the effects may have issues when using them or don't work for Forward+ or Deffered rendering.

## Features
- Kuwahara
  - Basic
  - Generalized
  - Anisotropic (w/ Polynomial Weighting)
- Pixel Art
  - Downsampling
- Dithering
- Color Palette Swapping
- Color Blindness
  - Protanopia/Protanomaly
  - Deuteranopia/Deuteranomaly
  - Tritanopia/Tritanomaly
- Zoom
  - 2 Different upscalers (Anti Aliased & Pixelized) + Normal  
- Gamma Corrector
- Tonemapper
  - RGB Clamp
  - Tumblin Rushmeier
  - Schlick
  - Ward
  - Reinhard
  - Reinhard Extended
  - Hable
  - Uchimura
  - Narkowicz ACES
  - Hill ACES
- Hue shifting
- Bloom
- Depth Based Edge Detection
- Vignette
- Chromatic Aberration
- ASCII
- Color Correction
  - Exposure
  - White Balancing
  - Contrast
  - Brightness
  - Color Filtering
  - Saturation
- Sharpness
  - Basic
  - Contrast Adaptive 
- Photoshop Blend Modes
  - Add/Subtract
  - Multiply
  - Color Burn
  - Color Dodge
  - Overlay
  - Soft Light
  - Vivid Light
- Difference of Gaussians
  - Basic
  - Extended
- Blur
  - Box
  - Gaussian 
- Fog
  - Distance 

## Example[s]
Note: I don't have screenshots of every effects in use like basic difference of gaussian.
<br>

Normal
<br>
<img src="https://github.com/user-attachments/assets/db68638b-4640-4474-aa4d-5cb3f8b49a5a" width="600"/>
<br>

Basic Kuwahara
<br>
<img src="https://github.com/user-attachments/assets/7f250cea-3b32-46d4-941a-3abc9c5545f0" width="600"/>
<br>

Generalized Kuwahara
<br>
<img src="https://github.com/user-attachments/assets/3e3e6bf1-03d8-437f-a99a-3f2a8418ff73" width="600"/>
<br>

Anisotropic Kuwahara
<br>
<img src="https://github.com/user-attachments/assets/7836682a-a886-4a84-86db-48f8174362e9" width="600"/>
<br>

Bloom
<br>
<img src="https://github.com/user-attachments/assets/046e716b-61bd-4d03-ad85-c43404243ff4" width="600"/>
<br>

Vignette
<br>
<img src="https://github.com/user-attachments/assets/441e0945-3efc-4662-a9e6-5f3cbf51c3e5" width="600"/>
<br>

Tonemapping (Schlick)
<br>
<img src="https://github.com/user-attachments/assets/4b7b24c1-bde8-4dfd-9506-b30662f170df" width="600"/>
<br>

Pixel Art (Downsampling)
<br>
<img src="https://github.com/user-attachments/assets/7fd87d1a-bc62-4850-bd99-8d8d52d98d46" width="600"/>
<br>

Color Blindness (Deuteranomaly)
<br>
<img src="https://github.com/user-attachments/assets/8be3d3fc-4d9e-4932-b585-be6f19afe743" width="600"/>
<br>

Hue Shifting
<br>
<img src="https://github.com/user-attachments/assets/69e88b6a-ad61-4f03-a8cf-9cd1057d060e" width="600"/>
<br>

Zoom
<br>
<img src="https://github.com/user-attachments/assets/5b68bf60-a730-4034-8fbc-ed4dc1f9534d" width="600"/>
<br>

Color Correct
<br>
<img src="https://github.com/user-attachments/assets/d9eafb32-b889-4ac8-b905-021eff2aa992" width="600"/>
<br>

Extended Difference Of Gaussian
<br>
<img src="https://github.com/user-attachments/assets/f724420d-4f73-4fc6-a515-31988838b505" width="600"/>
<br>

Blur (Box)
<br>
<img src="https://github.com/user-attachments/assets/15d08618-021d-4377-858c-163994d1736a" width="600"/>
<br>

Color Palette Swapping
<br>
<img src="https://github.com/user-attachments/assets/cf101a2d-5e80-4c06-b886-2c16a4bcc893" width="600"/>
<br>

Dithering
<br>
<img src="https://github.com/user-attachments/assets/320bd0ba-8333-4417-98f3-51bb64ed5b2f" width="600"/>
<br>

Chromatic Aberration
<br>
<img src="https://github.com/user-attachments/assets/eb53e5e6-386d-460f-8695-bc6aa1a404f9" width="600"/>
<br>

Sharpness
<br>
<img src="https://github.com/user-attachments/assets/cd5a9f04-e487-4763-a23f-02e7802a8ad9" width="600"/>
<br>

Edge Detect
<br>
<img src="https://github.com/user-attachments/assets/1873fa6c-d8ad-4b1c-ae35-ce22a169ba45" width="600"/>

## Installation
1. Clone repo or download the asset folder and load it into an unity project.
2. Add the render feature of the effect that you want to use to the Universal Renderer Data you are using
3. Create a volume game object and load the effect's volume component in the volume profile to adjust values
4. If needed, you can change the effect's render pass event in its render feature under settings 
    
## Credits/Assets used
The code is based on Acerola's Post Processing effect for Unity's Built In pipeline. 
 - [Post-Processing](https://github.com/GarrettGunnell/Post-Processing) by Acerola/GarrettGunnell
 - "[Blue Archive]Kasumizawa Miyu" (https://skfb.ly/oyBXP) by MOMO_RUI is licensed under Creative Commons Attribution (http://creativecommons.org/licenses/by/4.0/).
 - "Japanese Vending Machine" (https://skfb.ly/6ZCEz) by filadog is licensed under Creative Commons Attribution (http://creativecommons.org/licenses/by/4.0/).
 - Musemi by Occosoftware
 - Magica Cloth 2 by Magica Soft for hair & cloth physics

## License
[MIT](https://choosealicense.com/licenses/mit/)

