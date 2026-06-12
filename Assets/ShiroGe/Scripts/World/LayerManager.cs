using UnityEngine;
using UnityEngine.Serialization;

namespace ShiroGe.Scripts.World
{
    public class LayerManager : MonoBehaviour
    {
        public static LayerManager Instance { get; private set; }
        
        [field: SerializeField] public LayerMask InteractiveLayerMask { get; private set; }
        [field: SerializeField] public LayerMask CollisiveLayers { get; private set; }
        [field: SerializeField] public LayerMask NonBuildLayers { get; private set; }
        [field: SerializeField] public LayerMask BuildLayers { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }
    }
}