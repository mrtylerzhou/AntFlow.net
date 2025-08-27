namespace AntFlow.Core.Exception;

public class AFBizException : System.Exception
{
    // Constructor that accepts a message and an inner exception
    public AFBizException(string msg, System.Exception cause)
        : base(msg, cause)
    {
        Msg = msg;
        Code = "1"; // Default code
        ErrT = null;
    }

    // Constructor with code, message, and additional error details
    public AFBizException(string code, string msg, object errT)
        : base(msg)
    {
        Code = code;
        Msg = msg;
        ErrT = errT;
    }

    // Constructor with code and message
    public AFBizException(string code, string msg)
        : this(code, msg, null)
    {
    }

    // Constructor with just message (using default code)
    public AFBizException(string msg)
        : this("1", msg)
    {
    }

    // Constructor with type, code, message, and sub message
    public AFBizException(int type, string code, string msg, string subMessage)
        : this(code, msg)
    {
        Type = type;
        SubMessage = subMessage;
    }

    // Constructor with type, message, and sub message (using default code)
    public AFBizException(int type, string msg, string subMessage)
        : this("1", msg)
    {
        Type = type;
        SubMessage = subMessage;
    }

    // Constructor with type, error level, message, and sub message
    public AFBizException(int type, int errLevel, string msg, string subMessage)
        : this("1", msg)
    {
        Type = type;
        ErrLevel = errLevel;
        SubMessage = subMessage;
    }

    // Constructor with code, type, error level, message, and sub message
    public AFBizException(string code, int type, int errLevel, string msg, string subMessage)
        : this(code, msg)
    {
        Type = type;
        ErrLevel = errLevel;
        SubMessage = subMessage;
    }

    // Code for the exception
    public string Code { get; private set; }

    // Message for the exception
    public string Msg { get; private set; }

    // Additional details about the error
    public object ErrT { get; private set; }

    // Flag to determine if the error should be logged
    public bool IsLog { get; set; } = true;

    // Type of the error
    public int? Type { get; private set; }

    // Error level (severity)
    public int? ErrLevel { get; private set; }

    // Sub message with more details
    public string SubMessage { get; private set; }
}