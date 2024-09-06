using System.Numerics;

namespace Leapster;

public class Levels
{

    public static readonly List<Level> AllLevels =
    [
        // test level
        new Level(new Vector2(50.0f, 50.0f),
            new Vector4(22f, 382f, 300f, 32.093f),
            new Vector4(431f, 479f, 300f, 32.093f),
            new Vector4(755f, 374f, 300f, 32.093f),
            new Vector4(720f, 452f, 93.023f, 32.093f),
            new Vector4(749f, 427f, 93.023f, 32.093f),
            new Vector4(756f, 404f, 93.023f, 32.093f),
            new Vector4(23f, 179f, 100f, 38.983f)),

        new Level(new Vector2(50.0f, 50.0f),
            new Vector4(222f, 386f, 100f, 10f),
            new Vector4(548f, 363f, 100f, 10f),
            new Vector4(546f, 444f, 100f, 10f),
            new Vector4(409f, 533f, 100f, 10f),
            new Vector4(311f, 548f, 100f, 10f),
            new Vector4(191f, 515f, 100f, 10f),
            new Vector4(149f, 482f, 100f, 10f),
            new Vector4(146f, 287f, 100f, 10f),
            new Vector4(57f, 223f, 100f, 10f),
            new Vector4(779f, 183f, 100f, 10f),
            new Vector4(608f, 119f, 100f, 10f),
            new Vector4(418f, 221f, 100f, 10f))
    ];

}
