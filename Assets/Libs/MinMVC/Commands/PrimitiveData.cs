namespace MinMVC
{
	public class PrimitiveData<T>
	{
		public T value { get; private set; }

		public PrimitiveData(T v)
		{
			value = v;
		}
	}
}
