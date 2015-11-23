using UnityEngine;
using System.Collections;

public static class Utility {
    public static T[] ShuffleArray<T>(T[] array, int seed) { 
        System.Random random = new System.Random(seed);

        for (int i = 0; i<array.Length - 1; i++ )
        {
            int randomIndex = random.Next(i, array.Length);
            T temp = array[randomIndex];
            array[randomIndex] = array[i];
            array[i] = temp;
        }

        return array;
    }

}
