using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;




public class Objects : MonoBehaviour
{
  

    public Material points;
    public GameObject baseGameObject; // Base
    public GameObject upperArmGameObject; // Upper Arm
    public GameObject lowerArmGameObject; // Lower Arm
    public GameObject wristGameObject; // Wrist
    public GameObject groundGameObject;

    public GameObject leftWallGameObject;
    public GameObject rightWallGameObject;




    void CreateGameObjects(GameObject gameObject, IGB283Vector3[] vertices, int[] edges, float r, float g, float b)
    {
        CreateObjects(gameObject, points, vertices, edges, r,g,b);
    }


void Awake()
    {
        
        if (baseGameObject != null)
        {
            IGB283Vector3[] baseVertices = new IGB283Vector3[]
            {
            new IGB283Vector3(-1,1,0),
            new IGB283Vector3(-1,0,0),
            new IGB283Vector3(1,0,0),
            new IGB283Vector3(1,1,0),
            };
            if (gameObject.name == "QUTjr")
            {
                CreateGameObjects(baseGameObject, baseVertices, new int[] { 0, 2, 1, 0, 3, 2 }, 0.8f, 0.3f, 0.3f);
            }
            else
            {
                CreateGameObjects(baseGameObject, baseVertices, new int[] { 0, 2, 1, 0, 3, 2 }, 1f, 1f, 1f);
            }
            
        }
        if (upperArmGameObject != null)
        {
            IGB283Vector3[] upperArmVertices = new IGB283Vector3[]
            {
            new IGB283Vector3(-0.25f,0,0),
            new IGB283Vector3(-0.25f,2,0),
            new IGB283Vector3(0.25f,2,0),
            new IGB283Vector3(0.25f,0,0),
            };
           
            if (gameObject.name == "Upper Arm")
            {
                CreateGameObjects(upperArmGameObject, upperArmVertices, new int[] { 1, 2, 0, 2, 3, 0 }, 0.8f, 0.3f, 0.3f);
            }
            else
            {
                CreateGameObjects(upperArmGameObject, upperArmVertices, new int[] { 1, 2, 0, 2, 3, 0 },1f, 1f, 1f);
            }
        }
        if (lowerArmGameObject != null)
        {
            IGB283Vector3[] lowerArmVertices = new IGB283Vector3[]
            {
            new IGB283Vector3(-0.25f,0,0),
            new IGB283Vector3(-0.25f,2,0),
            new IGB283Vector3(0.25f,2,0),
            new IGB283Vector3(0.25f,0,0),
            };
           
            if (gameObject.name == "Lower Arm")
            {
                CreateGameObjects(lowerArmGameObject, lowerArmVertices, new int[] { 1, 2, 0, 2, 3, 0 }, 0.8f, 0.3f, 0.3f);
            }
            else
            {
                CreateGameObjects(lowerArmGameObject, lowerArmVertices, new int[] { 1, 2, 0, 2, 3, 0 }, 1f,1f, 1f);
            }
        }
        if (wristGameObject != null)
        {
            IGB283Vector3[] wristVertices = new IGB283Vector3[]
            {
            new IGB283Vector3(2,1,0),
            new IGB283Vector3(0,0,0),
            new IGB283Vector3(-2,1,0),
           
            };
          
            if (gameObject.name == "Wrist")
            {
                CreateGameObjects(wristGameObject, wristVertices, new int[] { 0, 1, 2 }, 0.8f, 0.3f, 0.3f);
            }
            else
            {
                CreateGameObjects(wristGameObject, wristVertices, new int[] { 0, 1, 2 }, 1f, 1f, 1f);
            }
        }
        if (groundGameObject)
        {
            IGB283Vector3[] groundVertices = new IGB283Vector3[]
           {
            new IGB283Vector3(31f,0,0),
            new IGB283Vector3(-31f,0,0),
            new IGB283Vector3(31f,-2.5f,0),
            new IGB283Vector3(-31f,-2.5f,0),
           };
            CreateGameObjects(groundGameObject, groundVertices, new int[] { 0,2,1, 1, 2, 3 }, 1f,1f,1f);
        }  
        if (rightWallGameObject)
        {
            IGB283Vector3[] rightWallVertices = new IGB283Vector3[]
           {
            new IGB283Vector3(31f,-2.5f,0),
            new IGB283Vector3(32.5f,-2.5f,0),
            new IGB283Vector3(31,10,0),
            new IGB283Vector3(32.5f,10,0),
           };
            CreateGameObjects(rightWallGameObject, rightWallVertices, new int[] { 0,2,1, 1, 2, 3 }, 1f,1f,1f);
        } 
        if (leftWallGameObject)
        {
            IGB283Vector3[] leftWallVertices = new IGB283Vector3[]
           {
            new IGB283Vector3(-31f,-2.5f,0),
            new IGB283Vector3(-32.5f,-2.5f,0),
            new IGB283Vector3(-31,10,0),
            new IGB283Vector3(-32.5f,10,0),
           };
            CreateGameObjects(leftWallGameObject, leftWallVertices, new int[] { 0,2,1, 1, 2, 3 }, 1f,1f,1f);
        }


      

    }


    void CreateObjects(GameObject gameObject, Material material, IGB283Vector3[] vertices, int[] triangles, float r, float b, float g)
    {

        gameObject.AddComponent<MeshCollider>();
        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();

        Mesh objectMesh = new Mesh();
        gameObject.GetComponent<MeshFilter>().mesh = objectMesh;
        gameObject.GetComponent<MeshRenderer>().material = material;

        objectMesh.vertices = vertices.Select(v => (Vector3)v).ToArray();
        objectMesh.triangles = triangles;


        Color[] colours = new Color[vertices.Length];
        Color colourValues = new Color(r, g, b);
        for (int i = 0; i < colours.Length; i++)
        {
            colours[i] = colourValues;
        }
        objectMesh.colors = colours;



        objectMesh.RecalculateNormals();
        objectMesh.RecalculateBounds();
    }

    void Start()
    {


    }

    void Update()
    {

      
    }

}

