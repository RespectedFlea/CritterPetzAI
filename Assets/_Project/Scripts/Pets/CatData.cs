using System;

[Serializable]
public class CatData
{
    public string catName;
    public float walkSpeed = 1.5f;
    public float sleepDuration = 5f;
    public float hungerRate = 0.1f;
    public float curiosityLevel = 0.5f;
    public float playfulness = 0.6f;
    public float intelligence = 0.5f; // for future machine learning
}
