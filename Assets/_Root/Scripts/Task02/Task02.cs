using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class Task02 : MonoBehaviour
{
    private int _positions = 20;
    private int _velocities = 15;
    private int _arrayLength = 25;

    public void Start()
    {
        Handle();
    }
    private void FillArray(NativeArray<Vector3> array, int maxValue)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = Random.insideUnitSphere * maxValue;
        }
    }
    private void PrintArray(NativeArray<Vector3> positions, NativeArray<Vector3> velocities, NativeArray<Vector3> finalPositions)
    {
        for (int i = 0; i < positions.Length; i++)
        {
            Debug.Log($"index {i}; position {positions[i]}; velocity {velocities[i]}; final {finalPositions[i]}");
        }
    }
    private void Handle()
    {
        NativeArray<Vector3> positions = new NativeArray<Vector3>(_arrayLength, Allocator.Persistent);
        NativeArray<Vector3> velocities = new NativeArray<Vector3>(_arrayLength, Allocator.Persistent);
        NativeArray<Vector3> finalPositions = new NativeArray<Vector3>(_arrayLength, Allocator.Persistent);
        FillArray(positions, _positions);
        FillArray(velocities, _velocities);
        Debug.Log("Start...");
        MyJob job = new MyJob()
        {
            Positions = positions,
            Velocities = velocities,
            FinalPositions = finalPositions,
        };
        JobHandle handle = job.Schedule(_arrayLength, 0);
        handle.Complete();
        Debug.Log("Done");
        PrintArray(positions, velocities, finalPositions);

        positions.Dispose();
        velocities.Dispose();
        finalPositions.Dispose();
    }

    public struct MyJob : IJobParallelFor
    {
        [ReadOnly]
        public NativeArray<Vector3> Positions;
        [ReadOnly]
        public NativeArray<Vector3> Velocities;
        [WriteOnly]
        public NativeArray<Vector3> FinalPositions;
        public void Execute(int index)
        {
            FinalPositions[index] = Positions[index] + Velocities[index];
        }
    }
}
