﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bwoah_shared.Utils
{
    public static class ArrayExtensions
    {
        public static T[] SubArray<T>(this T[] data, int length)
        {
            return SubArray(data, 0, length);
        }

        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }

        public static void Add<T>(this T[] data, T[] array)
        {
            T[] result = new T[data.Length + array.Length];
            Array.Copy(data, result, array.Length);
            Array.Copy(data, 0, result, data.Length, array.Length);
            data = result;
        }
    }
}
