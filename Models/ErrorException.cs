
public class CustomError
{
    public string Code { get; set; }
    public string Message { get; set; }
    public string? Payload { get; set; }
    public bool ReportToTeam { get; set; } = false;
}


public class CustomErrorException : Exception
{
    public string Code { get; private set; }
    public string? Payload { get; private set; }
    public bool ReportToTeam { get; private set; }

    public CustomErrorException(CustomError error) : base(error.Message)
    {
        Code = error.Code;
        Payload = error.Payload;
        ReportToTeam = error.ReportToTeam;
    }
}