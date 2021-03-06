﻿namespace ShaderForm.Demo
{
	public static class UniformHelper
	{
		public static bool IsNameValid(string name)
		{
			if (string.IsNullOrWhiteSpace(name)) return false;
			if (!char.IsLetter(name[0])) return false;
			foreach (char c in name)
			{
				if (!char.IsLetterOrDigit(c)) return false;
			}
			return true;
		}
	}
}
