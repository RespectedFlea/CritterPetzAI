using UnityEngine;

public class AnimalSpawner : MonoBehaviour
{
    [Header("Animal Prefab")]
    public GameObject animalPrefab; // Assign your AnimalPrefab in Inspector

    [Header("Animal Profiles")]
    public AnimalData catProfile;  // Assign your CatProfile.asset
    public AnimalData dogProfile;  // (Optional) Assign DogProfile.asset
    public AnimalData dragonProfile; // (Optional) etc.

    public void SpawnAnimal(AnimalData animalDataProfile, Vector2 spawnPosition)
    {
        GameObject newAnimal = Instantiate(animalPrefab, spawnPosition, Quaternion.identity);

        AnimalComponent animalComponent = newAnimal.GetComponent<AnimalComponent>();

        if (animalComponent != null)
        {
            animalComponent.animalData = animalDataProfile;
        }
    }
}
