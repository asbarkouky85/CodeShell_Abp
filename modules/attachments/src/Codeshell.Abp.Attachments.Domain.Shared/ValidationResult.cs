namespace Codeshell.Abp.Attachments
{
    public class ValidationResult
    {
        public string Message { get; private set; }
        public string[] Parameters { get; private set; }

        public ValidationResult(string v, string[] strings)
        {
            this.Message = v;
            this.Parameters = strings;
        }
    }
}