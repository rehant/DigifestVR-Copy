//
// SelectOARShader for oarconv by Fumi.Iseki 2015 (C) v1.3.2
//
// see also http://www.nsl.tuis.ac.jp/xoops/modules/xpwiki/?OAR%20Converter
//
//

using UnityEngine;
using UnityEditor;
using System.IO;
using System;


public sealed class SelectOARShader : AssetPostprocessor
{
	private const string NormalShader       = "Legacy Shaders/Diffuse";
	private const string TransShader        = "Legacy Shaders/Transparent/Diffuse";                 // Alpha Blending
	private const string TransCutShader     = "Legacy Shaders/Transparent/Cutout/Diffuse";          // Alpha Cutoff
	private const string TransCutSoftShader = "Legacy Shaders/Transparent/Cutout/Soft Edge Unlit";  // Alpha Blending
	private const string TransSpecShader    = "Legacy Shaders/Transparent/Specular";                // Alpha Blending + Specular
	private const string TransCutSpecShader = "Legacy Shaders/Transparent/Cutout/Specular";         // Alpha Cutoff + Specular
	private const string SpecularShader     = "Standard";
	private const string BrightShader       = "Legacy Shaders/Self-Illumin/Specular";
	private const string GlowShader         = "Standard";
	private const string TreeShader         = "Legacy Shaders/Transparent/Cutout/Soft Edge Unlit";
	
	private const string MaterialFolder = "Materials";
	private const string PhantomFolder  = "Phantoms";
	
//	private float colorRed   = 0.0f;
//	private float colorGreen = 0.0f;
//	private float colorBlue  = 0.0f;
	private float transparent = 1.0f;
	private float cutoff = 0.0f;
	private float shininess = 0.0f;
	private float glow   = 0.0f;
	private float bright = 0.0f;
//	private float light  = 0.0f;
	private char  kind   = 'O';		// Object
	
	
	void OnPreprocessModel()
	{
		string currentFolder = Path.GetDirectoryName (assetPath);
		if (!AssetDatabase.IsValidFolder (currentFolder + "/" + MaterialFolder)) {
			AssetDatabase.CreateFolder (currentFolder, MaterialFolder);
		}
		
		ModelImporter modelImporter = assetImporter as ModelImporter;
//		modelImporter.animationType = ModelImporterAnimationType.None;
		if (!currentFolder.Contains ("/" + PhantomFolder)) {
//			ModelImporter modelImporter = assetImporter as ModelImporter;
			modelImporter.addCollider = true;
		}
	}
	
	
	Material OnAssignMaterialModel(Material material, Renderer renderer)
	{
		string textureName;
		if (material.mainTexture != null) {
			textureName = material.mainTexture.name;
		} 
		else {
			textureName = material.name;
		}
		//
		string currentFolder = Path.GetDirectoryName (assetPath);
		string materialPath = string.Format("{0}/{1}/{2}.mat", currentFolder, MaterialFolder, textureName);
		
		Material mt = AssetDatabase.LoadAssetAtPath<Material> (materialPath);
		if (mt != null) return null;
		
		//
		getParamsFromTextureName(textureName);
		
		if (kind=='T' || kind=='G') {	// Tree or Grass
			material.shader = Shader.Find(TreeShader);
		}
		//
		else if (transparent < 0.99f) {
			if (shininess>0.01f) {
				if (cutoff>0.01f) {     // Alpha Cutoff
					material.shader = Shader.Find(TransCutSpecShader);
					if (material.HasProperty("_Cutoff")) material.SetFloat("_Cutoff", cutoff);
				} 
				else {                  // Alpha Blending
				    if (transparent>=0.90f) {
				    	material.shader = Shader.Find(TransCutSpecShader);
						if (material.HasProperty("_Cutoff")) material.SetFloat("_Cutoff", 0.2f);
					}
					else {
						material.shader = Shader.Find(TransSpecShader);
					}
				}
				if (material.HasProperty("_Shininess"))  material.SetFloat("_Shininess", 1.0f - shininess);
			}
			//
			else {
				if (cutoff>0.01f) {     // Alpha Cutoff
					material.shader = Shader.Find(TransCutShader);
					if (material.HasProperty("_Cutoff")) material.SetFloat("_Cutoff", cutoff);
				} 
				else {                  // Alpha Blending
				    if (transparent>=0.90f) {
				    	material.shader = Shader.Find(TransCutSoftShader);
						if (material.HasProperty("_Cutoff")) material.SetFloat("_Cutoff", 0.9f);
					}
					else {
						material.shader = Shader.Find(TransShader);
					}
				}
			}
		}
		//
		else if (glow > 0.02f) {
			material.EnableKeyword("_EMISSION");
			material.shader = Shader.Find(GlowShader);
			if (material.HasProperty("_EmissionColor")) {
				Color col = material.GetColor("_Color");
				float fac = col.maxColorComponent;
				if (fac>0.01f) {
					if (glow>0.99f) glow = 0.99f;
					col = col*(glow/fac);
				}
				material.SetColor("_EmissionColor", col);
			}
			if (material.HasProperty("_Glossiness")) material.SetFloat("_Glossiness", shininess);
		}
		//
		else if (bright > 0.01f) {
			material.shader = Shader.Find(BrightShader);
			if (material.HasProperty("_Shininess"))  material.SetFloat("_Shininess", 1.0f - shininess);
		}
		//
		else if (shininess > 0.01f) {
			material.shader = Shader.Find(SpecularShader);
			if (material.HasProperty("_Metallic"))   material.SetFloat("_Metallic",  shininess/2.0f);
			if (material.HasProperty("_Glossiness")) material.SetFloat("_Glossiness", 0.5f + shininess/2.0f);
		}
		//
		else {
			material.shader = Shader.Find(NormalShader);
		}
		
		AssetDatabase.CreateAsset(material, materialPath);
		//
		return null;
	}
	
	
	private void getParamsFromTextureName(string name)
	{
		int atr_len = 32;	// MTRL_ATTR_LEN /3*4 (36/3*4 = 32)
		//
		if (name.Length >= atr_len) {
			string sub = name.Substring (name.Length - atr_len, atr_len);
			string enc = sub.Replace('$', '/');
			//Debug.Log("Base64 String = "+enc);
			byte[] dec = Convert.FromBase64String(enc);	// MTRL_ATTR_LEN (32/4*3 = 24)
			
//			colorRed    = 1.0f - (float)dec[0]/255.0f;
//			colorGreen  = 1.0f - (float)dec[1]/255.0f;
//			colorBlue   = 1.0f - (float)dec[2]/255.0f;
			transparent = 1.0f - (float)dec[3]/255.0f;
			cutoff 		= (float)dec[4]/255.0f;
			shininess 	= (float)dec[5]/255.0f;
			glow   		= (float)dec[6]/255.0f;
			bright 		= (float)dec[7]/255.0f;
//			light  		= (float)dec[8]/255.0f;
			kind   		= (char)dec[23];
		}
	}
}
