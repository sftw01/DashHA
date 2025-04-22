namespace DashHA.Shared
{

    public class InfoMessage
    {
        public string Message { get; set; } = string.Empty;
        public bool Success { get; set; } = false;

        public InfoMessage(bool success, string message)
        {
            this.Message = message;
            this.Success = success;
        }
    }

}
