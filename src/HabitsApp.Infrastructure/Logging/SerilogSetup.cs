using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Serilog.Sinks.MSSqlServer;

namespace HabitsApp.Infrastructure.Logging;
public static class SerilogSetup
{
    public static void ConfigureSerilog(IConfiguration configuration)
    {
        var columnOptions = new ColumnOptions
        {
            Store = new Collection<StandardColumn>
           {
               StandardColumn.Message,
               StandardColumn.Level,
               StandardColumn.Exception,
               StandardColumn.TimeStamp,
               StandardColumn.Properties
           },
        };

     

        var sinkOptions = new MSSqlServerSinkOptions
        {
            TableName = "ErrorLogs",
            AutoCreateSqlTable = true
        };

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Error()
            .MinimumLevel.Override("Microsoft.AspNetCore.Diagnostics.ExceptionHandlerMiddleware",LogEventLevel.Fatal)
            .WriteTo.MSSqlServer(
                connectionString: configuration.GetConnectionString("SqlServer")!,
                sinkOptions: sinkOptions,
                columnOptions: columnOptions,
                restrictedToMinimumLevel:LogEventLevel.Error
                )
            .CreateLogger();
    }
}
