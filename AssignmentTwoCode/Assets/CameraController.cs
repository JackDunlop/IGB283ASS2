using System.Linq;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private GameObject upperArmGameObject;
    private GameObject upperArmGameObject2;


    private float padding = 5f; 
    private float minOrthoSize = 15f; 
    private float maxOrthoSize = 100f; 

    void Start()
    {
        upperArmGameObject = GameObject.Find("Upper Arm");
        upperArmGameObject2 = GameObject.Find("Upper Arm2");
    }

    void Update()
    {
        if (upperArmGameObject != null && upperArmGameObject2 != null)
        {
            IGB283Vector3 pos1 = GetObjectCenter(upperArmGameObject);
            IGB283Vector3 pos2 = GetObjectCenter(upperArmGameObject2);
            IGB283Vector3 midpoint = (pos1 + pos2) / 2f;

            float distance = IGB283Vector3.Distance(pos1, pos2);


            float requiredOrthoSize = (distance / 2f) + padding;

   
            Camera.main.orthographicSize = Mathf.Clamp(requiredOrthoSize, minOrthoSize, maxOrthoSize);
        }
    }

    public static IGB283Vector3 GetObjectCenter(GameObject gameObject)
    {
        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
        Mesh gameObjectMesh = meshFilter.mesh;
        IGB283Vector3[] vertices = gameObjectMesh.vertices.Select(v => (IGB283Vector3)v).ToArray();
        IGB283Vector3 center = new IGB283Vector3(0, 0, 0);
        foreach (IGB283Vector3 vertex in vertices)
        {
            center += vertex;
        }
        center /= vertices.Length;
        return center;
    }

}
