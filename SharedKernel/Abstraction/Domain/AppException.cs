namespace SharedKernel.Abstraction;

public class AppException(string message = "Ada yang salah. Hubungi tim IT GNY", Exception? innerException = null)
    : Exception(message, innerException);