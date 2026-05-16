using SlimDX;

static class MathHelpers
{
	// Counter-clockwise (2D cross product). Positive when b lies to the left of a.
	public static float Ccw(Vector2 a, Vector2 b)
	{
		return a.X * b.Y - a.Y * b.X;
	}
}
