namespace flitter;

[System.Serializable]
public class FlitterException : System.Exception
{
	public FlitterException() { }
	public FlitterException(string message) : base(message) { }
	public FlitterException(string message, System.Exception inner) : base(message, inner) { }
	protected FlitterException(
		System.Runtime.Serialization.SerializationInfo info,
		System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}