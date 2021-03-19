using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DotStimulus
{
    public class FlowManager : MonoBehaviour
    {
        [SerializeField] private Mesh dotMesh;
        [SerializeField] private Material dotMeshMaterial;
        [SerializeField] private int numDots;
        [SerializeField] private float density;
        [SerializeField] private float size;
        [SerializeField] private float speed;
        [SerializeField] private float lifetime;
        [SerializeField] private bool enableHotUpdate;

        [Header("Velocity variance")]
        [SerializeField] private float maxPitch;
        [SerializeField] private float maxYaw;
        
        private int oldNumDots;
        private float oldDensity;
        private float oldSize;
        private float oldSpeed;
        private float oldLifetime;
        
        private Dot[] dots;
        private DotShaderData _shaderData;
        
        [SerializeField] private Bounds bounds;
        private static readonly int ShaderProperties = Shader.PropertyToID("_Properties");

        public void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            // Copying the material into a new instance allows for multiple active stimuli
            // at the same time as they require separate material buffers
            dotMeshMaterial = new Material(dotMeshMaterial);
            numDots = Mathf.RoundToInt(density * bounds.extents.x * bounds.extents.y * bounds.extents.z);
            GenerateDots();
            _shaderData?.ClearBuffers();
            
            if(numDots > 0)
                _shaderData = new DotShaderData(dotMesh, numDots);
            
            oldNumDots = numDots;
            oldDensity = density;
            oldSize = size;
            oldSpeed = speed;
            oldLifetime = lifetime;
        }

        public void Update()
        {
            if (enableHotUpdate && HasChanged())
                Initialize();
            if (numDots <= 0)
                return;

            for(var i = 0; i < dots.Length; i++)
            {
                dots[i].UpdateDot();
                _shaderData.UpdateMeshPropertiesBuffer(dots, transform, i);
            }
        
            var meshPropertiesBuffer = _shaderData.MeshPropertiesBuffer;
            meshPropertiesBuffer.SetData(_shaderData.MeshProps);
            dotMeshMaterial.SetBuffer(ShaderProperties, meshPropertiesBuffer);
            Graphics.DrawMeshInstancedIndirect(dotMesh, 0, dotMeshMaterial, bounds, _shaderData.ArgsBuffer);
        }

        private bool HasChanged()
        {
            if(oldNumDots != numDots)
                return true;
            if (Math.Abs(oldDensity - density) > 0.001)
                return true;
            if (Math.Abs(oldSize - size) > 0.001)
                return true;
            if(Math.Abs(oldSpeed - speed) > 0.001)
                return true;
            if(Math.Abs(oldLifetime - lifetime) > 0.001)
                return true;
            return false;
        }

        private void GenerateDots()
        {
            if (numDots <= 0)
                return;
            
            dots = new Dot[numDots];

            int i;
            for (i = 0; i < numDots; i++)
            {
                var boundsMax = bounds.max;
                var boundsMin = bounds.min;
                
                var randomPosition = new Vector3(Random.Range(boundsMin.x / 2, boundsMax.x / 2), 
                    Random.Range(boundsMin.y / 2, boundsMax.y / 2), 
                    Random.Range(boundsMin.z / 2, boundsMax.z / 2));

                var velocityRotation =
                    Quaternion.Euler(Random.Range(-maxPitch, maxPitch), Random.Range(-maxYaw, maxYaw), 0);
                var randomVelocity = velocityRotation * Vector3.forward * speed;

                dots[i] = new Dot(randomVelocity, 
                    new Vector3(randomPosition.x, randomPosition.y, randomPosition.z),
                    size, bounds, lifetime);
            }
        }
        
        public void OnDestroy()
        {
            _shaderData.ClearBuffers();
        }

        public void OnApplicationQuit()
        {
            _shaderData.ClearBuffers();
        }

        public void OnDrawGizmosSelected()
        {
            var rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
            Gizmos.matrix = rotationMatrix;
            Gizmos.color = new Color(1.0f, 0.0f, 0.0f, 0.2f);
            Gizmos.DrawWireCube(transform.TransformPoint(bounds.center), bounds.extents);
        }
    }
}
