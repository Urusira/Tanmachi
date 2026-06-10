using System.Collections.Generic;
using UnityEngine;


namespace ShiroGe.Scripts.Utils
{
    public static class ListExtensions
    {
        /// <summary>
        /// Метод расширения для перемешивания любого списка
        /// </summary>
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            for (int i = n - 1; i > 0; i--)
            {
                int randomIndex = Random.Range(0, i + 1);
                (list[randomIndex], list[i]) = (list[i], list[randomIndex]);
            }
        }
    }
}