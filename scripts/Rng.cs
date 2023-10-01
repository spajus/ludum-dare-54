using System;

public static class Rng {
    private static Random rng = new Random();

    public static float NextFloat(float minValue, float maxValue) {
        return (float) rng.NextDouble() * (maxValue - minValue) + minValue;
    }

    public static float NextFloat() {
        return rng.NextSingle();
    }
}
