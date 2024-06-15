using System;
using System.IO;
using Live2D.Cubism.Core;
using Live2D.Cubism.Framework.Json;
using UnityEngine;
/// <summary>
/// Initialize model.
/// </summary>
public class LoadModel : MonoBehaviour
{
    [SerializeField]
    private FacialExpressionController _facialController;
    
    private string _path;
    void Awake ()
    {
        LoadLive2DModel();
    }

    private  void LoadLive2DModel()
    {
        _path = System.IO.Directory.GetCurrentDirectory();

        if (Application.isEditor)
        {
            _path += "\\Assets\\Live2DModels\\SlimeModel_vts\\SlimeModel.model3.json";
        }
        else
        {
            _path += "\\AnimationWithCV_Data\\StreamingAssets\\Live2DModel\\Live2DModel.model3.json";
        }
        
        //Load model
        var model3Json = CubismModel3Json.LoadAtPath(_path, BuiltinLoadAssetAtPath);
        var model = model3Json.ToModel();

        _facialController.SetModel(model);
    }
    
    /// <summary>
    /// Load asset.
    /// </summary>
    /// <param name="assetType">Asset type.</param>
    /// <param name="absolutePath">Path to asset.</param>
    /// <returns>The asset on succes; <see langword="null"> otherwise.</returns>
    public static object BuiltinLoadAssetAtPath(Type assetType, string absolutePath)
    {
        if (assetType == typeof(byte[]))
        {
            return File.ReadAllBytes(absolutePath);
        }
        else if(assetType == typeof(string))
        {
            return File.ReadAllText(absolutePath);
        }
        else if (assetType == typeof(Texture2D))
        {
            var texture = new Texture2D(1,1);
            texture.LoadImage(File.ReadAllBytes(absolutePath));
            return texture;
        }
        throw new NotSupportedException();
    }
}