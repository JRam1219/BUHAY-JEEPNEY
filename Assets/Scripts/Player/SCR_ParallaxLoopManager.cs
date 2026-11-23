using UnityEngine;

public class SCR_ParallaxLoopManager : MonoBehaviour
{
    [System.Serializable]
    public class ParallaxLayer
    {
        public string layerName;
        public Transform tileA;
        public Transform tileB;
        [Range(0f, 1.5f)] public float speedMultiplier = 1f;
        public bool stopLoopingAtGoal = false; // Only check this for the Road/Buildings if needed

        [HideInInspector] public float tileWidth;
        [HideInInspector] public float resetX;
    }

    [Header("Master Speed")]
    public float baseSpeed = 6f;
    public bool isLooping = true;

    [Header("Parallax Layers")]
    public ParallaxLayer[] layers;

    [Header("Goal / Destination")]
    public Transform destinationObject; // Terminal or SceneExitTrigger (moves at road speed 1.0)
    public int roadLayerIndex = 0;      // Index of your Road layer in the list above

    private bool destinationSpawned = false;

    private void Start()
    {
        foreach (var layer in layers)
        {
            if (layer.tileA != null && layer.tileB != null)
            {
                // Auto-measure exact width from how you lined them up in the Scene view
                layer.tileWidth = Mathf.Abs(layer.tileB.position.x - layer.tileA.position.x);
                layer.resetX = layer.tileA.position.x - layer.tileWidth;
            }
        }
    }

    private void Update()
    {
        if (baseSpeed <= 0f)
            return;

        foreach (var layer in layers)
        {
            if (layer.tileA == null || layer.tileB == null)
                continue;

            float moveStep = baseSpeed * layer.speedMultiplier * Time.deltaTime;
            layer.tileA.Translate(Vector3.left * moveStep);
            layer.tileB.Translate(Vector3.left * moveStep);

            // Keep background layers (sky/city) looping so you never see an empty void
            bool shouldRecycle = isLooping || !layer.stopLoopingAtGoal;
            if (shouldRecycle)
            {
                if (layer.tileA.position.x <= layer.resetX)
                {
                    layer.tileA.position = new Vector3(layer.tileB.position.x + layer.tileWidth, layer.tileA.position.y, layer.tileA.position.z);
                }

                if (layer.tileB.position.x <= layer.resetX)
                {
                    layer.tileB.position = new Vector3(layer.tileA.position.x + layer.tileWidth, layer.tileB.position.y, layer.tileB.position.z);
                }
            }
        }

        // Scroll the destination into view at full road speed (1.0)
        if (destinationSpawned && destinationObject != null)
        {
            destinationObject.Translate(Vector3.left * baseSpeed * Time.deltaTime);
        }
    }

    public void StopLoopingAndSpawnGoal()
    {
        if (!isLooping)
            return;

        isLooping = false;

        if (destinationObject != null && roadLayerIndex < layers.Length)
        {
            var road = layers[roadLayerIndex];
            float rightmostX = Mathf.Max(road.tileA.position.x, road.tileB.position.x);

            destinationObject.position = new Vector3(rightmostX + road.tileWidth, destinationObject.position.y, destinationObject.position.z);
            destinationObject.gameObject.SetActive(true);
            destinationSpawned = true;
        }
    }
}