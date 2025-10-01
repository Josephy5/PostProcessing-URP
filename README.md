<div align="center">
  <img src="https://github.com/user-attachments/assets/6212ba26-641e-497b-ad6a-7d4d3bd9dc0f" width="500" alt="Miyu_Early"/>
</div>

# PostProcessing-URP
![Unity Version](https://img.shields.io/badge/Unity-6000.0%58LTS%2B-blueviolet?logo=unity)
![Unity Pipeline Support (Built-In)](https://img.shields.io/badge/BiRP_❌-darkgreen?logo=unity)
![Unity Pipeline Support (URP)](https://img.shields.io/badge/URP_✔️-blue?logo=unity)
![Unity Pipeline Support (HDRP)](https://img.shields.io/badge/HDRP_❌-darkred?logo=unity)

A series of post-processing effects for Unity URP (6000.0.58f1). These effects were from Acerola's Post Processing repo, which was designed for Unity's Built-In Pipeline. 
I have adapted these effects over to Unity 6 URP as part of my self study on shader programming and to improve their performance in Unity. Initially, I modified them for Unity 2022 URP, 
but I am gradually updating this repo with improved versions of the Unity 2022 effects I did over to Unity 6, along with other improvements. 

You can access all previous code and materials for the PostProcessing-URP that I developed in Unity 2022 URP here in the [Unity 2022.3.20f1 branch](https://github.com/Josephy5/PostProcessing-URP/tree/Unity-2022.3.20f1-Version)

## Notes
As of now, I have only updated the Anisotropic Kuwahara effect to work with Unity 6, as that was my last contribution during my time at Serious Point Games. 
I plan to gradually bring over the remaining effects to Unity 6, one by one. Do note that some of the effects may have issues when using them or 
don't work for Forward+ or Deffered rendering.

## Includes
- Kuwahara
  - Anisotropic (w/ Polynomial Weighting)

## Example[s]
Note: I don't have screenshots of every effects in use like basic difference of gaussian.
<br>

Normal
<br>
<img src="https://github.com/user-attachments/assets/db68638b-4640-4474-aa4d-5cb3f8b49a5a" width="600"/>
<br>

Anisotropic Kuwahara
<br>
<img src="https://github.com/user-attachments/assets/7836682a-a886-4a84-86db-48f8174362e9" width="600"/>
<br>

## Installation
1. Clone repo or download the asset folder and load it into an unity project.
2. Add the render feature of the effect that you want to use to the Universal Renderer Data you are using
3. Create a volume game object and load the effect's volume component in the volume profile to adjust values
4. If needed, you can change the effect's render pass event in its render feature under settings 
    
## Credits/Assets used
The code is based on Acerola's Post Processing effect for Unity's Built In pipeline. 
 - [Post-Processing](https://github.com/GarrettGunnell/Post-Processing) by Acerola/GarrettGunnell. Licensed under MIT license - See [THIRD PARTY LICENSES](THIRD_PARTY_LICENSES) for details.
 - "[Blue Archive]Kasumizawa Miyu" (https://skfb.ly/oyBXP) by MOMO_RUI is licensed under Creative Commons Attribution (http://creativecommons.org/licenses/by/4.0/).
 - "Japanese Vending Machine" (https://skfb.ly/6ZCEz) by filadog is licensed under Creative Commons Attribution (http://creativecommons.org/licenses/by/4.0/).
 - Musemi by Occosoftware. Used for presentation purposes only.
 - Magica Cloth 2 by Magica Soft for hair & cloth physics. Used for presentation purposes only.

