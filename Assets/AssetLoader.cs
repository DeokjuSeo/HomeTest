using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AssetLoader : MonoBehaviour
{
    [field: SerializeField]
    public LoaderModule LoaderModule { get; set; }

    private void Start()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        //string selectedAssetName = EditorUtility.OpenFilePanel("Select obj model", "", "obj");
        //List<string> selectedAssetNames = GetObjFiles("/Resources/Models");

        if(sceneName == "Test_3")
        {
            List<string> selectedAssetNames = GetObjFiles("Resources/Models");
            Load_3(selectedAssetNames);
        }
        else
        {
            string selectedAssetName = EditorUtility.OpenFilePanel("Select obj model", "", "obj");
            if(sceneName == "Test_1")
            {
                Load_1(selectedAssetName);
            }
            else if(sceneName == "Test_2")
            {
                Load_2(selectedAssetName);
            }
            else
            {
                Debug.LogError("���̸� Ȯ��");
            }
        }
    }

    // 1�� ���� ����
    public void Load_1(string assetName)
    {
        LoaderModule.OnLoadCompleted += OnLoadCompleted;
        LoaderModule.LoadAsset_1(assetName);
    }

    private void OnLoadCompleted(GameObject loadedAsset)
    {
        loadedAsset.transform.SetParent(transform);
        Debug.Log("OnLoadCompleted�� ���� ��ġ �Ϸ�");
    }

    // 2�� ���� ����
    public async void Load_2(string assetName)
    {
        GameObject loadedAsset = await LoaderModule.LoadAssetAsync_2(assetName);
        loadedAsset.transform.SetParent(transform);
    }

    // 3�� ���� ����
    private List<string> GetObjFiles(string directory)
    {
        List<string> objFiles = new List<string>();

        string path = Path.Combine(Application.dataPath, directory);
        //string path = Path.Combine("C:/Users/USER/Desktop/TestObj");

        if (Directory.Exists(path))
        {
            // ������ ���丮���� .obj Ȯ���ڸ� ���� ��� ������ �˻�
            string[] files = Directory.GetFiles(path, "*.obj", SearchOption.AllDirectories);

            foreach (string file in files)
            {
                objFiles.Add(file);
            }
        }
        else
        {
            Debug.Log("���丮�� �������� ����.");
        }
        return objFiles;
    }

    public async void Load_3(List<string> assetNames)
    {
        List<Task<GameObject>> loadTasks = new List<Task<GameObject>>();

        // ��� 3d�𵨵鿡 ���� �ε� �۾��� ���ÿ� ����
        foreach (string assetName in assetNames)
        {
            Task<GameObject> loadTask = LoaderModule.LoadAssetAsync_3(assetName);
            loadTasks.Add(loadTask);
        }

        // ���� �뷮 ������ �ε�Ǿ� ���� ��ġ�� ����� ���� ���� �κ�
        // 1�� ����ó�� OnLoadCompleted�� ����Ѵٸ� �Ʒ� �۾��� �ʿ� �������� ���� �ǵ��� �ƴ϶� �����Ǿ� �Ʒ��� ���� �ۼ��Ͽ����ϴ�. 
        while (loadTasks.Any())
        {
            Task<GameObject> finishedTask = await Task.WhenAny(loadTasks);
            loadTasks.Remove(finishedTask);

            GameObject loadedAsset = await finishedTask;
            if (loadedAsset != null)
            {
                loadedAsset.transform.SetParent(transform);
                Debug.Log($"<Step 3> {loadedAsset}��(��) ��ġ �Ϸ�����");
            }
        }
    }
}