using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Sinks.PostgreSQL;


namespace Common.Api.BaseConfiguration;

public static class LoggingExtension
{
    public static IHostBuilder UseLogging(this IHostBuilder hostBuilder, Action<HostBuilderContext, LoggerConfiguration>? configureLogger = null)
    {
        hostBuilder.UseSerilog(
            (ctx, lc) =>
            {
                if (ctx.HostingEnvironment.IsDevelopment())
                {
                    lc.WriteTo.Console();

                }
                else
                {
                    // var connectionString = ctx.Configuration.GetConnectionString("DefaultConnection");
                    // var columnOptions = new Dictionary<string, ColumnWriterBase>
                    // {
                    //     { "Timestamp", new TimestampColumnWriter() },
                    //     { "Level", new LevelColumnWriter() },
                    //     { "Message", new RenderedMessageColumnWriter() },
                    //     { "Exception", new ExceptionColumnWriter() },
                    //     { "Properties", new LogEventSerializedColumnWriter() }
                    // };
                    //
                    // lc.WriteTo.PostgreSQL(
                    //     connectionString: connectionString,
                    //     tableName: "Logs",
                    //     needAutoCreateTable: true,
                    //     columnOptions: columnOptions
                    // );
                }

                // lc.ReadFrom.Configuration(ctx.Configuration);
                // if (configureLogger is not null)
                // {
                //     configureLogger(ctx, lc);
                // }
            }
        );

        return hostBuilder;
    }
}
