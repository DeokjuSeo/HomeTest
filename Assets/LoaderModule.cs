using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Threading.Tasks;

public class LoaderModule : MonoBehaviour
{
    public Action<GameObject> OnLoadCompleted;
    private string tempResourcesPath = "Assets/Resources/TempSave/";

    // 1�� ����
    public void LoadAsset_1(string assetName)
    {
        string fileName = Path.GetFileName(assetName);
        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(assetName);

        // ������ ������ ������Ʈ �� Resources ��ġ�� �ӽ������� ����
        File.Copy(assetName, tempResourcesPath + fileName, true);
        AssetDatabase.Refresh();
        GameObject copiedObject = Resources.Load<GameObject>("TempSave/" + fileNameWithoutExtension);

        // ������ ������ ���� �ҷ����� & OnLoadCompleted �׼� ����
        if (copiedObject != null)
        {
            GameObject instantiatedObject = Instantiate(copiedObject);
            OnLoadCompleted?.Invoke(instantiatedObject);
        }
        else
        {
            Debug.LogError(assetName + " ������ �ӽ������� �����ϴ� �κп� ������ �־���");
        }
    }


    // 2�� ����
    public async Task<GameObject> LoadAssetAsync_2(string assetName)
    {
        string fileName = Path.GetFileName(assetName);
        await Task.Run(() => File.Copy(assetName, tempResourcesPath + fileName, true));
        AssetDatabase.Refresh();
        GameObject copiedObject = Resources.Load<GameObject>("TempSave/" + Path.GetFileNameWithoutExtension(assetName));

        if (copiedObject != null)
        {
            return Instantiate(copiedObject);
        }
        else
        {
            Debug.LogError(assetName + " ������ �ӽ������� �����ϴ� �κп� ������ �־���");
            return null;
        }
        // 1�� ���� ��� OnLoadCompleted ���� �ʿ���� �� Task�� ���� �Ѱܹ��� ���ӿ�����Ʈ�� ���� SetParent ������ ����
        // �ð��� �ҿ�Ǵ� File.Copy�۾��� �� �� ���ν����带 �������� ����
    }


    // 3�� ����
    public async Task<GameObject> LoadAssetAsync_3(string assetName)
    {
        string fileName = Path.GetFileName(assetName);
        Debug.Log($"<Step 1> {fileName}�� ���� LoadAsset�۾� ����");

        await Task.Run(() => File.Copy(assetName, tempResourcesPath + fileName, true));

        AssetDatabase.Refresh();
        GameObject copiedObject = Resources.Load<GameObject>("TempSave/" + Path.GetFileNameWithoutExtension(assetName));
        
        if (copiedObject != null)
        {
            GameObject loadedObject = Instantiate(copiedObject);
            Debug.Log($"<Step 2> {loadedObject}�� Instantiate �Ϸ�����");
            return loadedObject;
        }
        else
        {
            Debug.LogError(assetName + " ������ �ӽ������� �����ϴ� �κп� ������ �־���");
            return null;
        }
        // �ǰ� : ���� ���Ͽ��� async Ű���尡 ���� ���·� ���õǾ��µ��� �̺κ��� �񵿱� �޼ҵ忩�� "���� �뷮 ������ �ε�" ����� ���� �����ϴٰ� �����˴ϴ�.
    }



    private void OnApplicationQuit()
    {
        if (Directory.Exists(tempResourcesPath))
        {
            string[] files = Directory.GetFiles(tempResourcesPath);

            foreach (string file in files)
            {
                File.Delete(file);
            }
            AssetDatabase.Refresh();
        }
        // ���� �� Resources/TempSave�� �ӽ÷� �����ߴ� ����(��) �����ϱ� ���� �߰��߽��ϴ�.
    }
}
