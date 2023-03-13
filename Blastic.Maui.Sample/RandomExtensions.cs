namespace Blastic.Maui.Sample;

public static class RandomExtensions
{
	public static void Shuffle<T>(this T[] array, Random random)
	{
		int n = array.Length;

		while (n > 1)
		{
			int k = random.Next(n--);
			(array[n], array[k]) = (array[k], array[n]);
		}
	}
}