using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NewLoad : MonoBehaviour
{
    [SerializeField] private CameraController cameraController;
    [SerializeField] private Speedometer speedometer; // Reference to the Speedometer script

    private GameObject _mMyGameObject;

    // LEVEL LOADÝNG 
    private bool clearPreviousScene = false;
    private SceneInstance previousLoadedScene;


    public void InstantiateGameobjectUsingAssetReference(string key)
    {
        Addressables.InstantiateAsync(key).Completed += obj =>
        {
            _mMyGameObject = obj.Result;
            // Set the target Rigidbody for the Speedometer
            var carRigidbody = _mMyGameObject.GetComponent<Rigidbody>();
            if (carRigidbody != null && speedometer != null)
            {
                speedometer.target = carRigidbody;
            }
            // Set the camera target to the new car
            cameraController.SetTarget(_mMyGameObject.transform);
        };
    }

    private void OnLoadDone(AsyncOperationHandle<GameObject> obj)
    {
        _mMyGameObject = obj.Result;
    }

    public void ReleaseGameobjectUsingAssetReference()
    {
        Addressables.Release(_mMyGameObject);
    }

    public void LoadAddressableLevel(string addressableKey)
    {
        if (clearPreviousScene)
        {
            Addressables.UnloadSceneAsync(previousLoadedScene).Completed += (asyncHandle) =>
            {
                clearPreviousScene = false;
                previousLoadedScene = new SceneInstance();

            };
        }
        Addressables.LoadSceneAsync(addressableKey, LoadSceneMode.Additive).Completed += (asyncHandle) =>
        {
            clearPreviousScene = true;
            previousLoadedScene = asyncHandle.Result;

        };
        GameObject background = GameObject.FindWithTag("Background");
        if (background != null)
        {
            background.SetActive(false);
        }

    }
}