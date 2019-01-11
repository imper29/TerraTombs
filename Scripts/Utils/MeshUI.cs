using UnityEngine;

namespace Utils
{
    public class MeshUI : MonoBehaviour
    {
        [SerializeField]
        private MeshFilter meshFilter;
        [SerializeField]
        private MeshRenderer meshRenderer;
        [SerializeField]
        private float rotationSpeed;

        public void SetDisplay(Mesh mesh, Material mat, float rotationSpeed)
        {
            meshFilter.sharedMesh = mesh;
            meshRenderer.sharedMaterial = mat;

            this.rotationSpeed = rotationSpeed;
        }

        private void Update()
        {
            meshFilter.transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
        }
    }
}
