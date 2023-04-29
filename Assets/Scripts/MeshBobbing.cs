using Unity.Mathematics;
using UnityEngine;

public sealed class MeshBobbing : MonoBehaviour
{
    [SerializeField] private float amount = 0.01f;
    [SerializeField] private float frequency = 1.5f;

    private Vector3 initialLocalPosition;
    private Vector3 targetLocalPosition => initialLocalPosition + amount * Vector3.up;

    // Start is called before the first frame update
    void Start()
    {
        initialLocalPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        var t = Time.time * frequency % 1;
        var u = t switch
        {
            < 0.1f => 10 * t,
            < 0.5f => 1,
            < 0.6f => 6 - 10 * t,
            _ => 0,
        };
        transform.localPosition = Vector3.Lerp(initialLocalPosition, targetLocalPosition, u);
    }
}
