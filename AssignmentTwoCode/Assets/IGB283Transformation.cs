using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class IGB283Transformation : MonoBehaviour
{
    public void RotateAroundPoint(GameObject gameObject, GameObject child, Vector3 point, float angle, float lastAngle)
    {
        float angleRad = angle * Mathf.Deg2Rad;
        float lastAngleRad = lastAngle * Mathf.Deg2Rad;

        Matrix3x3 T1 = Matrix3x3.Translate(-point);
        Matrix3x3 R1 = Matrix3x3.Rotate(-lastAngleRad);
        Matrix3x3 T2 = Matrix3x3.Translate(point);
        Matrix3x3 R2 = Matrix3x3.Rotate(angleRad);
        Matrix3x3 M = T2 * R2 * R1 * T1;

        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
        Mesh mesh = meshFilter.mesh;

        Vector3[] vertices = mesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = M.MultiplyPoint(vertices[i]);
        }
        mesh.vertices = vertices;

        point = M.MultiplyPoint(point);

        if (child != null)
        {
            child.GetComponent<Movement>().RotateAroundPoint(point, angle, lastAngle);
        }
    }

    public static void MoveByOffset(Vector3 offset, Vector3 jointLocation, GameObject child, GameObject gameObject)
    {
        Matrix3x3 T = Matrix3x3.Translate(offset);

        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
        Mesh mesh = meshFilter.mesh;

        Vector3[] vertices = mesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = T.MultiplyPoint(vertices[i]);
        }
        mesh.vertices = vertices;

        jointLocation = T.MultiplyPoint(jointLocation);

        if (child != null)
        {

            MoveByOffset(offset, jointLocation, child, gameObject);
        }
    }
}
