using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public static class Extensions {

    public static Vector2 ToVector2(this Point source) {
        return new Vector2(source.X, source.Y);
    }
}
