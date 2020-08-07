using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class PostProcessing : MonoBehaviour
{
    [Range(1, 20)]
    public int iterations = 2;

    [Range(0.1f, 10.0f)]
    public float threshold = 1;
    [Range(-0.1f, 1.0f)]
    public float softThreshold = 1;

    [Range(1.0f, 20.0f)]
    public float intensity = 1;

    public Shader bloomShader;
    RenderTexture[] textures = new RenderTexture[20];
    Material bloom;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        //Create a bloom material
        if (bloom == null)
        {
            bloom = new Material(bloomShader);
            bloom.hideFlags = HideFlags.HideAndDontSave;
        }

        bloom.SetFloat("_Threshold", threshold);
        bloom.SetFloat("_SoftThreshold", softThreshold);
        bloom.SetFloat("_Intensity", intensity);
        bloom.SetTexture("_SourceTex", source);

        int width = source.width;
        int height = source.height;
        RenderTextureFormat format = source.format;

        
        RenderTexture currentDest = textures[0] = RenderTexture.GetTemporary(width, height, 0);
        RenderTexture currentSrc = currentDest;
        //First pre filter that decides what pixels contribute to bloom
        Graphics.Blit(source, currentDest, bloom, 0);
        RenderTexture.ReleaseTemporary(currentSrc);

        int i = 1;
        for(; i < iterations; i++)
        {
            width /= 2; height /= 2;
            if (height < 2)
                break;

            currentDest = textures[i] = RenderTexture.GetTemporary(width, height, 1);

            Graphics.Blit(currentSrc, currentDest, bloom, 1);
          
            //RenderTexture.ReleaseTemporary(currentSrc);

            currentSrc = currentDest;
        }

        //Up sampling
        for (i-=2; i > 0; i--){
            currentDest = textures[i];
            textures[i] = null;
            Graphics.Blit(currentSrc, currentDest, bloom, 2);
            RenderTexture.ReleaseTemporary(currentSrc);
            currentSrc = currentDest;
        }


        Graphics.Blit(currentSrc, destination, bloom, 3);

        RenderTexture.ReleaseTemporary(currentDest);
    }

}
