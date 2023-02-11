using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;

public class CalculateManager : MonoBehaviour
{
    [BurstCompile(CompileSynchronously = true)]
    struct CalculateJob: IJobParallelFor
    {
        [NativeDisableParallelForRestriction]
        public NativeArray<int> rows;
        [NativeDisableParallelForRestriction]
        public NativeArray<int> columns;
        public NativeArray<int> results;

        public float maxDistance;

        public void Execute(int index)
        {

            int pixelCount = rows.Length;

            int p1Mag = Magnitude(rows[index], columns[index]);

            int count = 0;
            
            for(int i=0; i<pixelCount; i++)
            {
                int p2Mag = Magnitude(rows[i], columns[i]);

                int dist = Distance(p1Mag, p2Mag);

                if(dist < maxDistance) count++;
            }

            results[index] = count;


        }

        int Magnitude(int a, int b)
        {
            return a*a + b*b;
        }

        int Distance(int mag1, int mag2)
        {
            return Mathf.Abs(mag1 - mag2);
        }

    }

    public NativeArray<int> rows;
    public NativeArray<int> columns;
    public NativeArray<int> results;

    public float maxDistance;

    JobHandle handle;
    public int[] GetMiddlePoint(List<Pixel> pixels, int maxDifference)
    {
        rows = new NativeArray<int>(pixels.Count, Allocator.Persistent);
        columns = new NativeArray<int>(pixels.Count, Allocator.Persistent);
        results = new NativeArray<int>(pixels.Count, Allocator.Persistent);

        int pixelCount = pixels.Count;

        for(int i=0; i<pixelCount; i++)
        {
            rows[i] = pixels[i].row;
            columns[i] = pixels[i].column;
            results[i] = 0;
        }

        CalculateJob job = new CalculateJob();

        job.rows = rows;
        job.columns = columns;
        job.results = results;
        job.maxDistance = maxDistance;

        handle = job.Schedule(pixelCount, 1);

        handle.Complete();

        int maxCount = 0;

        int[] maxIndexes = new int[2];

        for(int i=0; i<pixelCount; i++)
        {
            if(results[i] > maxCount)
            {
                maxCount = results[i];
                maxIndexes[0] = rows[i];
                maxIndexes[1] = columns[i];
            }
        }

        rows.Dispose();
        columns.Dispose();
        results.Dispose();

        return maxIndexes;
    }
}
