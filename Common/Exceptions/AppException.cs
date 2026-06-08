namespace RetroGaming.Common.Exceptions
{
    public class AppException : Exception
    {
        public int StatusCode { get; }

        public AppException(string message, int statusCode = 400) : base(message)
        {
            StatusCode = statusCode;
        }

        public static AppException NotFound(string entity, int id)
            => new($"{entity} with ID {id} was not found.", 404);

        public static AppException BadRequest(string message)
            => new(message, 400);

        public static AppException Conflict(string message)
            => new(message, 409);
    }
}