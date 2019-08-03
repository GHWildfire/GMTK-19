using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility
{
    public enum Tag
    {
        NONE,
        WALL
    };

    /// <summary>
    /// Return the corresponding tag string
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static string FromTag(Tag tag)
    {
        switch (tag)
        {
            case Tag.NONE:
                return "";

            case Tag.WALL:
                return "Wall";
        }

        return "";
    }
}
