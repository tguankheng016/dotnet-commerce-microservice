namespace CommerceMicro.Modules.Resiliency;

public class RetryOptions
{
    public int RetryCount { get; set; }

    public int SleepDuration { get; set; }
}
