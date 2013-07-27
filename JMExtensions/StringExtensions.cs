namespace JMExtensions {
	public static class StringExtensions {
		public static string Right(this string sValue, int iMaxLength) {
			if (string.IsNullOrEmpty(sValue)) {
				sValue = string.Empty;
			} else if (sValue.Length > iMaxLength) {
				sValue = sValue.Substring(sValue.Length - iMaxLength, iMaxLength);
			}

			return sValue;
		}
	}
}