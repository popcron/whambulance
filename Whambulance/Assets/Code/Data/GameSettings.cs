﻿using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameSettings
{
    public TipsAsset tips;
    public string levelToLoad = "TestLevel";
    public Player playerPrefab;
    public float sideWalkSize = 0.5f;
    public List<Level> levels = new List<Level>();
    public List<Vehicle> vehicles = new List<Vehicle>();
    public List<Objective> patients = new List<Objective>();
    public List<GameObject> enemyUnits = new List<GameObject>();
    public List<Pedestrian> pedestrians = new List<Pedestrian>();
    public List<AdvancementAsset> advancements = new List<AdvancementAsset>();
    public int maxVehicles = 48;
    public float vehicleLifeTime = 16f;
    public int maxPedestrians = 48;
    public string[] introDialogs = { };
    public string[] patientDialogs = { };

    [Header("Gameplay")]
    public AnimationCurve maxEnemiesOverTime = new AnimationCurve();
    public int maxEnemiesAlive = 60;
    public float maxRescueTime = 60f;
    public float maxDeliveryTime = 60f;
}
