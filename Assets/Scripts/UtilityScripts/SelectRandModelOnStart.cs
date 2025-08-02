using UnityEngine;

public class SelectRandModelOnStart: MonoBehaviour
{
    void Start()
    {
        Renderer[] modelVariants = GetComponentsInChildren<Renderer>();
        if (modelVariants == null || modelVariants.Length < 2) return;

        foreach (var variant in modelVariants) {
            variant.gameObject.SetActive(false);
        }

        int randomIndex = Random.Range(0, modelVariants.Length);
        modelVariants[randomIndex].gameObject.SetActive(true);
    }

}
