using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class FMScreenDepthNormal : MonoBehaviour
{
    public Camera Cam;
    public Material Mat;

    // Start is called before the first frame update
    void Start()
    {
        Cam = this.GetComponent<Camera>();
        Cam.depthTextureMode = Cam.depthTextureMode | DepthTextureMode.Depth;

        if (Mat == null)
        {
            Mat = new Material(Shader.Find("Hidden/FMShader_ScreenDepthNormal"));
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        //Graphics.Blit(source, destination);

        Graphics.Blit(source, destination, Mat);
    }
}
