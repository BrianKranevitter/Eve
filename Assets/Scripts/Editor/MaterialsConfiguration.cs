using UnityEditor;

using UnityEngine;

namespace Game.Editor
{
    public sealed class MaterialsConfiguration : AssetPostprocessor
    {
        [MenuItem("Game/Enable GPU Instancing")]
        public static void EnableGpuInstancing()
        {
            foreach (string guid in AssetDatabase.FindAssets("t:Material", new[] { "Assets/Art" }))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
                if (material != null)
                {
                    material.enableInstancing = true;
                    EditorUtility.SetDirty(material);
                }
            }
        }

        private void OnPostprocessMaterial(Material material) => material.enableInstancing = true;
    }
}