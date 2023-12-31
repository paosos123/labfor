﻿using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEditor;

public class WeaponTests
{
    GameObject projectilePrefab;
    GameObject laserPrefab;
    GameObject asteroidPrefab;
    GameObject spaceshipPrefab;

    [SetUp]
    public void Setup()
    {
        GameManager.InitializeTestingEnvironment(false, false, false, false, false);

        spaceshipPrefab = ((GameObject)Resources.Load("TestsReferences", typeof(GameObject))).GetComponent<TestsReferences>().spaceshipPrefab;
        asteroidPrefab = ((GameObject)Resources.Load("TestsReferences", typeof(GameObject))).GetComponent<TestsReferences>().asteroidPrefab;
        projectilePrefab = ((GameObject)Resources.Load("TestsReferences", typeof(GameObject))).GetComponent<TestsReferences>().projectilePrefab;
        laserPrefab = ((GameObject)Resources.Load("TestsReferences", typeof(GameObject))).GetComponent<TestsReferences>().laserPrefab;
    }

    void ClearScene()
    {
        Transform[] objects = Object.FindObjectsOfType<Transform>();
        foreach (Transform obj in objects)
        {
            if(obj != null)
                Object.DestroyImmediate(obj.gameObject);
        }
    }

/*
    // Uncomment the code from line 35 up to line 237, run the tests again and compare the code coverage results

    [Test]
    public void _01_ProjectilePrefabExists()
    {
        Assert.NotNull(projectilePrefab);
    }

    [Test]
    public void _02_ProjectilePrefabCanBeInstantiated()
    {
        ClearScene();
        GameObject projectile = (GameObject)Object.Instantiate(projectilePrefab);
        projectile.name = "Projectile";
        Assert.NotNull(GameObject.Find("Projectile"));
    }

    [Test]
    public void _03_ProjectilePrefabHasRequiredComponentTransform()
    {
        Assert.IsNotNull(projectilePrefab.GetComponent<Transform>());
    }

    [Test]
    public void _04_ProjectilePrefabHasRequiredComponentCollider()
    {
        Assert.IsNotNull(projectilePrefab.GetComponent<BoxCollider2D>());
        Assert.IsTrue(projectilePrefab.GetComponent<BoxCollider2D>().size == new Vector2(0.2f, 0.2f));
    }

    [Test]
    public void _05_ProjectilePrefabHasRequiredComponentControllerScript()
    {
        Assert.IsNotNull(projectilePrefab.GetComponent<ProjectileController>());
    }

    [Test]
    public void _06_ProjectilePrefabHasRequiredVisual()
    {
        Transform visualChild = projectilePrefab.transform.GetChild(0);
        Assert.IsTrue(visualChild.name == "Visual");
        Assert.IsNotNull(visualChild);
        Assert.IsNotNull(visualChild.GetComponent<MeshRenderer>());
        Assert.IsNotNull(visualChild.GetComponent<MeshRenderer>().sharedMaterials[0]);
        Assert.IsNotNull(visualChild.GetComponent<MeshFilter>());
        Assert.IsNotNull(visualChild.GetComponent<MeshFilter>().sharedMesh);
    }

    [Test]
    public void _07_ProjectileCanMove()
    {
        ClearScene();
        ProjectileController projectile = Object.Instantiate(projectilePrefab, Vector3.zero, Quaternion.identity).GetComponent<ProjectileController>();
        projectile.Move();
        Assert.IsTrue(projectile.transform.position != Vector3.zero);
    }

    [Test]
    public void _08_ProjectileDirectionCanBeChanged()
    {
        ClearScene();
        ProjectileController projectile = Object.Instantiate(projectilePrefab, Vector3.zero, Quaternion.identity).GetComponent<ProjectileController>();
        projectile.SetDirection(Vector2.up);
        Assert.IsTrue(projectile.GetDirection() == Vector2.up);
    }

    [UnityTest]
    public IEnumerator _09_ProjectileMovesAccordingToItsDirectionVector()
    {
        ClearScene();
        ProjectileController projectile = Object.Instantiate(projectilePrefab, Vector3.zero, Quaternion.identity).GetComponent<ProjectileController>();
        projectile.SetDirection(Vector2.up);
        Assert.IsTrue(projectile.GetDirection() == Vector2.up);
        
        float t = 0.5f;
        while (t > 0.0f)
        {
            t -= Time.deltaTime;
            yield return null;
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                   