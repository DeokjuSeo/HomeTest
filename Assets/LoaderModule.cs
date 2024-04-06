using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Threading.Tasks;

public class LoaderModule : MonoBehaviour
{
    public Action<GameObject> OnLoadCompleted;
    private string tempResourcesPath = "Assets/Resources/TempSave/";

    // 1번 문제
    public void LoadAsset_1(string assetName)
    {
        string fileName = Path.GetFileName(assetName);
        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(assetName);

        // 선택한 파일을 프로젝트 내 Resources 위치로 임시적으로 복사
        File.Copy(assetName, tempResourcesPath + fileName, true);
        AssetDatabase.Refresh();
        GameObject copiedObject = Resources.Load<GameObject>("TempSave/" + fileNameWithoutExtension);

        // 복사한 파일을 씬에 불러오기 & OnLoadCompleted 액션 실행
        if (copiedObject != null)
        {
            GameObject instantiatedObject = Instantiate(copiedObject);
            OnLoadCompleted?.Invoke(instantiatedObject);
        }
        else
        {
            Debug.LogError(assetName + " 파일을 임시폴더로 복사하는 부분에 문제가 있었음");
        }
    }


    // 2번 문제
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
            Debug.LogError(assetName + " 파일을 임시폴더로 복사하는 부분에 문제가 있었음");
            return null;
        }
        // 1번 문제 대비 OnLoadCompleted 구현 필요없이 이 Task를 통해 넘겨받은 게임오브젝트를 직접 SetParent 가능한 이점
        // 시간이 소요되는 File.Copy작업을 할 때 메인스레드를 차단하지 않음
    }


    // 3번 문제
    public async Task<GameObject> LoadAssetAsync_3(string assetName)
    {
        string fileName = Path.GetFileName(assetName);
        Debug.Log($"<Step 1> {fileName}에 대한 LoadAsset작업 시작");

        await Task.Run(() => File.Copy(assetName, tempResourcesPath + fileName, true));

        AssetDatabase.Refresh();
        GameObject copiedObject = Resources.Load<GameObject>("TempSave/" + Path.GetFileNameWithoutExtension(assetName));
        
        if (copiedObject != null)
        {
            GameObject loadedObject = Instantiate(copiedObject);
            Debug.Log($"<Step 2> {loadedObject}를 Instantiate 완료했음");
            return loadedObject;
        }
        else
        {
            Debug.LogError(assetName + " 파일을 임시폴더로 복사하는 부분에 문제가 있었음");
            return null;
        }
        // 의견 : 출제 파일에는 async 키워드가 빠진 형태로 제시되었는데요 이부분이 비동기 메소드여야 "파일 용량 순으로 로드" 기능이 구현 가능하다고 생각됩니다.
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
        // 종료 시 Resources/TempSave에 임시로 저장했던 파일(들) 삭제하기 위해 추가했습니다.
    }
}
