using Unity.VisualScripting;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [SerializeField] private float _fov, _viewDistance;
    [SerializeField] private int _rayCount;
    [SerializeField] LayerMask _layermask;
    private float _angle, _angleDelta;
    private Mesh _mesh;

    private void Start()
    {
        _mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = _mesh;
        _angle = 0f;
        _angleDelta = _fov / _rayCount;
    }

    private void Update()
    {
        DrawFieldOfView();
    }

    public void DrawFieldOfView() {
        // Empty mesh
        Vector3[] vertices = new Vector3[_rayCount + 1 + 1];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[_rayCount * 3];

        //Direction
        Vector3 rayOrigin = Vector3.zero;
        vertices[0] = rayOrigin;
        int triangleIndex = 0;

        for (int i = 0; i < _rayCount + 1; i++)
        {
            Vector3 vertex = rayOrigin + GetVectorFromAngle(_angle) * _viewDistance;

            Ray ray = new Ray(rayOrigin, vertex);
            RaycastHit hit;
            bool raycast = Physics.Raycast(ray, out hit, _viewDistance, _layermask);
            if (!raycast)   //No hit
            {
                vertices[i] = vertex;
            }
            else // hit
            {
                Debug.Log("Hit");
                vertices[i] = hit.point;
            }

            if (i > 0)
            {
                triangles[triangleIndex + 0] = 0;
                triangles[triangleIndex + 1] = i - 1;
                triangles[triangleIndex + 2] = i;
                triangleIndex += 3;
            }

            _angle -= _angleDelta;
        }

        _mesh.vertices = vertices;
        _mesh.uv = uv;
        _mesh.triangles = triangles;
    }

    private Vector3 GetVectorFromAngle(float angle)
    {
        float rad = angle * Mathf.PI / 180f;
        return new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad));
    }
    private float GetAngleFromVector(Vector3 vector)
    {
        float n = Mathf.Atan2(vector.z, vector.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;
        return n;
    }

}
