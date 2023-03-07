using System.IO;
using UnityEditor;
using UnityEngine;

namespace ProceduralShaderAnimation.ImageLogic
{
    /*struct AnimationInfo
    {
        public 
    }*/
    
    public class ShaderAnimationGenerator : MonoBehaviour
    {
        public Texture2D GenerateAnimationTexture()
        {
            Texture2D tex = new(2, 1, TextureFormat.RGBAFloat, true);

            float[] test = {0.4f, 0.5f, 0.6f, 0.7f, 0.8f, 0.9f, 0.2f, 0.1f};

            tex.SetPixelData(test, 0, 0); // mip 0
            //tex.SetPixelData(data, 1, 12); // mip 1
            //tex.filterMode = FilterMode.Point;
            tex.Apply(updateMipmaps: false);

            var image = tex.EncodeToEXR();

            Debug.Log(Application.dataPath);
            var dirPath = Application.dataPath + "/../SaveImages/";
            if(!Directory.Exists(dirPath)) {
                Directory.CreateDirectory(dirPath);
            }
            File.WriteAllBytes(dirPath + "Image" + ".exr", image);
            
            //GetComponent<Renderer>().sharedMaterial.mainTexture = tex;

            return tex;
        } 
    }
}
