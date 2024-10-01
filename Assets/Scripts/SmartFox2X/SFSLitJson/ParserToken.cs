namespace SFSLitJson
{
	internal enum ParserToken
	{
		None = 65536,
		Number = 65537,
		True = 65538,
		False = 65539,
		Null = 65540,
		CharSeq = 65541,
		Char = 65542,
		Text = 65543,
		Object = 65544,
		ObjectPrime = 65545,
		Pair = 65546,
		PairRest = 65547,
		Array = 65548,
		ArrayPrime = 65549,
		Value = 65550,
		ValueRest = 65551,
		String = 65552,
		End = 65553,
		Epsilon = 65554
	}
}
