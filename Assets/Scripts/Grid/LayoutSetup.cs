using UnityEngine;

namespace Kartografowie.Grid
{
    public class LayoutSetup : MonoBehaviour
    {
        public GameObject[] mapVariants; //warianty mapy

        void Start()
        {
            if (mapVariants.Length == 0)
            {
                Debug.LogError("Brak wariant�w mapy do losowania!");
                return;
            }

            int randomIndex = Random.Range(0, mapVariants.Length);

            GameObject selectedMap = Instantiate(mapVariants[randomIndex], gameObject.transform);

            Debug.Log($"Wylosowano map�: {selectedMap.name}");
        }
    }
}