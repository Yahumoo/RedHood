using UnityEngine;

public class Chair : Item
{
    public Mesh Chair_1;
    public Mesh Chair_2;
    [SerializeField] MeshFilter meshFilter;
    [SerializeField] MeshCollider meshCollider;

    private void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();
    }
    public void SellectedChair()
    {
        meshFilter.mesh = Chair_2;
        meshCollider.sharedMesh = Chair_2;

        transform.localRotation = Quaternion.Euler(-90, 0, 0);
        transform.localPosition = new Vector3(0, -0.1f, 0.05f);
    }
    public void DeselectedChair()
    {
        meshFilter.mesh = Chair_1;
        meshCollider.sharedMesh = Chair_1;
    }
}