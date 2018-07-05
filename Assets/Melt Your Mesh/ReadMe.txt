Thank you for downloading Melt Your Mesh! :)

Melt Your Mesh is a shader package. It offers a GPU approach to melt mesh effect in unity3d.
There are two kinds of version shader in it.
=> "Melt Your Mesh/Default" contains melt effect with standard shading.
=> "Melt Your Mesh/Tessellation" contains a tessellation version of it. This can be a option for high-end devices.

How to setup ?
  => Attach material with shader "Melt Your Mesh" to your gameobject.
  => Attach script component "Melt.cs" to your gameobject.
  => This script does two things: first it control material parameters of the melt shader, second it move gameobject back and forth.
  => Inspector parameter "Melt Y" control the melt begin height, any gameobject below it will be melted.
  => Inspector parameter "Use Tessellation" toggle tessellation and non-tessellation version melt shader. Notice only use tessellation on high-end devices.

How to modify and expand melt shader ?
  => The core shader logic is inside "Core.cginc". We distort mesh vertices in vertex shader.
  => Currently we use unity build-in default standard shading. If you want to use your own shading model, you can expand it by specify custom surface or lighting function.

Demo scene demonstrates all features. Please refer the Demo scene as example usage.
Try to tweak the parameters in inspector, have fun with it!

If you like it, please give it a good review on asset store. Thanks so much ! It means a lot to me as a developer.
Any suggestion or improvement you want, please contact qq_d_y@163.com.
Hope we can help more and more game developers.