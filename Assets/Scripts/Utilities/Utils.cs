using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;

public class Utils : MonoBehaviour
{
    public static float ConvertReflexAngle(float value)
    {
        float angle = value % 360f;
        angle = Mathf.Abs(angle) > 180f ? angle - 360f * Mathf.Sign(angle) : angle;
        return angle;
    }

    public static float Sin01(float value)
    {
        return Mathf.Sin(value * 0.5f * Mathf.PI);
    }

    public static Vector2 GetMousePosition()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    public static Vector2 RadianToVector2(float radian)
    {
        return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
    }

    public static Vector2 DegreeToVector2(float degree)
    {
        return RadianToVector2(degree * Mathf.Deg2Rad);
    }

    public static List<int> RandomFromIntPool(List<int> pool, int amount, bool remove = true)
    {
        List<int> choices = new List<int>();
        List<int> source;
        if (remove)
        {
            source = pool;
        } else
        {
            source = new List<int>(pool);
        }
        int index;
        for (int i = 0; i < amount; i++)
        {
            index = Random.Range(0, source.Count);
            choices.Add(source[index]);
            source.RemoveAt(index);
        }
        return choices;
    }

    public static List<int> GenerateIndexPool(int amount)
    {
        List<int> pool = new List<int>();
        for (int i = 0; i < amount; i++)
        {
            pool.Add(i);
        }
        return pool;
    }
}
