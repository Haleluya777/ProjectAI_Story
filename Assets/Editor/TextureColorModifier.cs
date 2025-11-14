using UnityEngine;
using UnityEditor;
using System.IO;

public class TextureColorModifier
{
    // 새로운 메뉴 아이템을 "Assets" 메뉴에 추가합니다.
    [MenuItem("Assets/Create Tinted Texture")]
    private static void CreateTintedTexture()
    {
        // 프로젝트 뷰에서 선택된 텍스처를 가져옵니다.
        Texture2D selectedTexture = Selection.activeObject as Texture2D;

        if (selectedTexture == null)
        {
            EditorUtility.DisplayDialog("오류", "프로젝트 뷰에서 텍스처 파일을 선택해주세요.", "확인");
            return;
        }

        // 변경할 색상 (여기서는 파란색으로 설정)
        Color tintColor = Color.black;

        // 원본 텍스처의 경로를 가져옵니다.
        string originalPath = AssetDatabase.GetAssetPath(selectedTexture);
        TextureImporter textureImporter = AssetImporter.GetAtPath(originalPath) as
        TextureImporter;

        if (textureImporter == null)
        {
            EditorUtility.DisplayDialog("오류", "텍스처 임포터 설정을 가져올 수 없습니다.", "확인");
            return;
        }

        // 픽셀 데이터에 접근하려면 isReadable 플래그가 true여야 합니다.
        bool originalIsReadable = textureImporter.isReadable;
        if (!originalIsReadable)
        {
            textureImporter.isReadable = true;
            AssetDatabase.ImportAsset(originalPath, ImportAssetOptions.ForceUpdate);
        }

        // 원본 텍스처의 픽셀 데이터를 읽어옵니다.
        Color[] originalPixels = selectedTexture.GetPixels();
        Color[] newPixels = new Color[originalPixels.Length];

        // 각 픽셀에 색상을 곱하여 채색 효과를 줍니다.
        for (int i = 0; i < originalPixels.Length; i++)
        {
            // 원본 픽셀의 회색조(grayscale) 값을 유지하면서 색상을 입힙니다.
            float grayscale = originalPixels[i].grayscale;
            newPixels[i] = tintColor * grayscale;
            newPixels[i].a = originalPixels[i].a; // 원본 알파값 유지
        }

        // 새로운 텍스처를 생성하고 픽셀 데이터를 적용합니다.
        Texture2D newTexture = new Texture2D(selectedTexture.width, selectedTexture.height);
        newTexture.SetPixels(newPixels);
        newTexture.Apply();

        // 새로운 텍스처를 PNG 파일로 인코딩합니다.
        byte[] pngData = newTexture.EncodeToPNG();
        Object.DestroyImmediate(newTexture); // 메모리에서 임시 텍스처 제거

        // 새로운 파일 경로를 생성합니다. (예: "MyTexture_blue.png")
        string directory = Path.GetDirectoryName(originalPath);
        string fileName = Path.GetFileNameWithoutExtension(originalPath);
        string newFileName = $"{fileName}_{tintColor.ToString()}.png";
        string newPath = Path.Combine(directory, newFileName);

        // 파일로 저장하고 에셋 데이터베이스에 추가합니다.
        File.WriteAllBytes(newPath, pngData);
        AssetDatabase.Refresh();

        // 원본 텍스처의 isReadable 설정을 원래대로 되돌립니다.
        if (!originalIsReadable)
        {
            textureImporter.isReadable = false;
            AssetDatabase.ImportAsset(originalPath, ImportAssetOptions.ForceUpdate);
        }

        EditorUtility.DisplayDialog("성공", $"새로운 텍스처가 다음 경로에 저장되었습니다:\n{newPath}", "확인");
    }
}