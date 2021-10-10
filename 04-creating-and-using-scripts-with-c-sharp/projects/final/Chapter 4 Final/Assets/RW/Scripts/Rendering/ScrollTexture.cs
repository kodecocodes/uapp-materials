using UnityEngine;

public class ScrollTexture : MonoBehaviour
{
    // Scroll main texture based on time

    public Vector2 scrollSpeed;

    private Vector2 offset;
    private Renderer rend;

    private void Start()
    {
        rend = GetComponent<Renderer>();
    }

    private void Update()
    {
        offset += Time.deltaTime * scrollSpeed;
        rend.material.SetTextureOffset("_MainTex", offset);
    }
}