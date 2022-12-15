using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class Lesson2_Task1 : MonoBehaviour
{
    public int minArrayValue;
    public int maxArrayValue;
    public int arrayLength;
    public void Start()
    {
        Reset();
    }
    private void FillArray(NativeArray<int> array, int minValue, int maxValue)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = Random.Range(minValue, maxValue);
        }
    }
    private void PrintArray(NativeArray<int> array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            Debug.Log($"index {i}; value {array[i]}");
        }
    }
    public void Reset()
    {
        NativeArray<int> array = new NativeArray<int>(arrayLength, Allocator.Persistent);
        FillArray(array, minArrayValue, maxArrayValue);
        PrintArray(array);
        Debug.Log("Start...");
        MyJob job = new MyJob();

        job.maxValue = 10;
        job.jobArray = array;
        
        JobHandle jobHandle = job.Schedule();
        jobHandle.Complete();
        PrintArray(array);
        array.Dispose();

    }
}
public struct MyJob : IJob
{
    public NativeArray<int> jobArray;
    public int maxValue;
    public void Execute()
    {
        for (int i = 0; i < jobArray.Length; i++)
        {
            if (jobArray[i] > maxValue)
            {

                jobArray[i] = 0;
            }
        }
    }


}
