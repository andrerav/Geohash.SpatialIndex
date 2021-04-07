namespace Geohash.SpatialIndex.Core
{
	public static class GeohashIndexUtil
	{
		/// <summary>
		/// Find the longest common prefix among a collection of strings
		/// </summary>
		/// <param name="strings"></param>
		/// <returns>The longest common prefix from the given collection of strings</returns>
		public static string LongestCommonPrefix(string[] strings)
		{
			if (strings.Length == 0)
			{
				return "";
			}
			var pre = strings[0];
			var match = pre.Length - 1;
			for (var i = 1; i < strings.Length; i++)
			{
				var cur = strings[i];
				int j, k;
				for (j = 0, k = 0; j <= match && k < cur.Length; j++, k++)
				{
					if (pre[j] != cur[k])
						break;
				}
				match = j - 1;
				if (match == -1)
					return "";
			}
			return pre.Substring(0, match + 1);
		}
	}
}