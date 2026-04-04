namespace ApiGateway.Configuration;

public static class SecurityConfiguration
{
    public static WebApplication UseGatewaySecurity(this WebApplication app)
    {
        var enableHttpsRedirection = app.Configuration.GetValue("GatewaySecurity:EnableHttpsRedirection", false);
        var enableHsts = app.Configuration.GetValue("GatewaySecurity:EnableHsts", false);

        if (enableHsts && !app.Environment.IsDevelopment())
        {
            app.UseHsts();
        }

        if (enableHttpsRedirection)
        {
            app.UseHttpsRedirection();
        }

        return app;
    }
}
