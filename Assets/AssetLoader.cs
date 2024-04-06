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
                Debug.LogError("씬이름 확인");
            }
        }
    }

    // 1번 문제 관련
    public void Load_1(string assetName)
    {
        LoaderModule.OnLoadCompleted += OnLoadCompleted;
        LoaderModule.LoadAsset_1(assetName);
    }

    private void OnLoadCompleted(GameObject loadedAsset)
    {
        loadedAsset.transform.SetParent(transform);
        Debug.Log("OnLoadCompleted를 통해 배치 완료");
    }

    // 2번 문제 관련
    public async void Load_2(string assetName)
    {
        GameObject loadedAsset = await LoaderModule.LoadAssetAsync_2(assetName);
        loadedAsset.transform.SetParent(transform);
    }

    // 3번 문제 관련
    private List<string> GetObjFiles(string directory)
    {
        List<string> objFiles = new List<string>();

        string path = Path.Combine(Application.dataPath, directory);
        //string path = Path.Combine("C:/Users/USER/Desktop/TestObj");

        if (Directory.Exists(path))
        {
            // 지정된 디렉토리에서 .obj 확장자를 가진 모든 파일을 검색
            string[] files = Directory.GetFiles(path, "*.obj", SearchOption.AllDirectories);

            foreach (string file in files)
            {
                objFiles.Add(file);
            }
        }
        else
        {
            Debug.Log("디렉토리가 존재하지 않음.");
        }
        return objFiles;
    }

    public async void Load_3(List<string> assetNames)
    {
        List<Task<GameObject>> loadTasks = new List<Task<GameObject>>();

        // 모든 3d모델들에 대해 로드 작업을 동시에 시작
        foreach (string assetName in assetNames)
        {
            Task<GameObject> loadTask = LoaderModule.LoadAssetAsync_3(assetName);
            loadTasks.Add(loadTask);
        }

        // 파일 용량 순으로 로드되어 씬에 배치된 모습을 보기 위한 부분
        // 1번 문제처럼 OnLoadCompleted을 사용한다면 아래 작업이 필요 없겠으나 출제 의도가 아니라 생각되어 아래와 같이 작성하였습니다. 
        while (loadTasks.Any())
        {
            Task<GameObject> finishedTask = await Task.WhenAny(loadTasks);
            loadTasks.Remove(finishedTask);

            GameObject loadedAsset = await finishedTask;
            if (loadedAsset != null)
            {
                loadedAsset.transform.SetParent(transform);
                Debug.Log($"<Step 3> {loadedAsset}을(를) 배치 완료했음");
            }
        }
    }
}