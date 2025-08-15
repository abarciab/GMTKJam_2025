using UnityEngine;

public class RandomizeNPCMaterials : MonoBehaviour
{
    [SerializeField] GameObject[] _hatObjects;
    [SerializeField] Renderer[] _clothingRenderers;
    [SerializeField] Renderer _skinRenderer;

    [SerializeField] Material[] _clothingMats;
    [SerializeField] Material[] _skinMats;

    private void Start()
    {
        if (_hatObjects.Length > 0)
        {
            var randomHat = _hatObjects[Random.Range(0, _hatObjects.Length)];
            foreach (var hat in _hatObjects)
            {
                hat.SetActive(hat == randomHat);
            }
        }
        if (_clothingRenderers.Length == 0 || _clothingMats.Length == 0 || _skinMats.Length == 0) return;
        foreach (var renderer in _clothingRenderers)
        {
            if (renderer != null)
            {
                var randomClothingMat = _clothingMats[Random.Range(0, _clothingMats.Length)];
                renderer.material = randomClothingMat;
            }
        }
        if (_skinRenderer != null)
        {
            var randomSkinMat = _skinMats[Random.Range(0, _skinMats.Length)];
            _skinRenderer.material = randomSkinMat;
        }
    }
}
