using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

public class Galaxy : MonoBehaviour
{
    [SerializeField] private int _numberOfEntities;
    [SerializeField] private float _startDistance;
    [SerializeField] private float _startVelocity;
    [SerializeField] private float _startMass;
    [SerializeField] private float _gravitationModifier;
    [SerializeField] private GameObject _celestialBodyPrefab;

    private NativeArray<Vector3> _positions;
    private NativeArray<Vector3> _velocities;
    private NativeArray<Vector3> _accelerations;
    private NativeArray<float> _masses;
    private TransformAccessArray _transformAccessArray;
    void Start()
    {
        _positions = new NativeArray<Vector3>(_numberOfEntities, Allocator.Persistent);
        _velocities = new NativeArray<Vector3>(_numberOfEntities, Allocator.Persistent);
        _accelerations = new NativeArray<Vector3>(_numberOfEntities, Allocator.Persistent);
        _masses = new NativeArray<float>(_numberOfEntities, Allocator.Persistent);
        Transform[] transforms = new Transform[_numberOfEntities];
        for (int i = 0; i < _numberOfEntities; i++)
        {
            _positions[i] = Random.insideUnitSphere * Random.Range(0, _startDistance);
            _velocities[i] = Random.insideUnitSphere * Random.Range(0, _startVelocity);
            _accelerations[i] = Vector3.zero;
            _masses[i] = Random.Range(1, _startMass);

            transforms[i] = Instantiate(_celestialBodyPrefab, 
                _positions[i], Quaternion.identity).transform;
        }
        _transformAccessArray = new TransformAccessArray(transforms);


    }
    private void Update()
    {
        GravitationJob gravitationJob = new GravitationJob()

        {
            Positions = _positions,
            Velocities = _velocities,
            Accelerations = _accelerations,
            Masses = _masses,
            GravitationModifier = _gravitationModifier,
            DeltaTime = Time.deltaTime

        };
        JobHandle gravitationHandle = gravitationJob.Schedule(_numberOfEntities, 0);
        MoveJob moveJob = new MoveJob()
        {
            Positions = _positions,
            Velocities = _velocities,
            Accelerations = _accelerations,

            DeltaTime = Time.deltaTime
        };
        JobHandle moveHandle = moveJob.Schedule(_transformAccessArray, gravitationHandle);
        moveHandle.Complete();
    }
    private void OnDestroy()
    {
        _positions.Dispose();
        _velocities.Dispose();
        _accelerations.Dispose();
        _masses.Dispose();
        _transformAccessArray.Dispose();

    }
}

public struct GravitationJob : IJobParallelFor
{
    [ReadOnly] public NativeArray<Vector3> Positions;
    [ReadOnly] public NativeArray<Vector3> Velocities;
    public NativeArray<Vector3> Accelerations;
    [ReadOnly] public NativeArray<float> Masses;
    [ReadOnly] public float GravitationModifier;
    [ReadOnly] public float DeltaTime;
    public void Execute(int index)
    {
        for (int i = 0; i < Positions.Length; i++)
        {
            if (i == index) continue;
            float distance = Vector3.Distance(Positions[i], Positions[index]);
            Vector3 direction = Positions[i] - Positions[index];
            Vector3 gravitation = (direction * Masses[i] * GravitationModifier) /
                (Masses[index] * Mathf.Pow(distance, 2));
            Accelerations[index] += gravitation * DeltaTime;
        }
    }
}
public struct MoveJob : IJobParallelForTransform
{
    public NativeArray<Vector3> Positions;
    public NativeArray<Vector3> Velocities;
    public NativeArray<Vector3> Accelerations;
    [ReadOnly] public float DeltaTime;
    public void Execute(int index, TransformAccess transform)
    {
        Vector3 velocity = Velocities[index] + Accelerations[index];
        transform.position += velocity * DeltaTime;
        Positions[index] = transform.position;
        Velocities[index] = velocity;
        Accelerations[index] = Vector3.zero;
    }
}

