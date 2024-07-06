using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ModelPostImporter : AssetPostprocessor
{
    void OnPostprocessModel(GameObject go)
    {
        var assetsToReload = new HashSet<string>();
        var importer = AssetImporter.GetAtPath(assetPath) as ModelImporter;

        var materials = AssetDatabase.LoadAllAssetsAtPath(importer.assetPath).Where(x => x.GetType() == typeof(Material)).ToArray();

        string destinationPath = Directory.CreateDirectory(Path.GetDirectoryName(assetPath) + "\\Materials\\").FullName;

        foreach (var material in materials)
        {
            var newAssetPath = destinationPath + material.name + ".mat";
            newAssetPath = AssetDatabase.GenerateUniqueAssetPath(newAssetPath);

            var error = AssetDatabase.ExtractAsset(material, newAssetPath);
            if (string.IsNullOrEmpty(error))
            {
                assetsToReload.Add(importer.assetPath);
            }
        }

        foreach (var path in assetsToReload)
        {
            AssetDatabase.WriteImportSettingsIfDirty(path);
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }
    }
}
