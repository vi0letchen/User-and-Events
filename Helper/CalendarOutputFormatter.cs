using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using A2.Models;

namespace A2.Helper
{
    public class CalendarOutputFormatter : TextOutputFormatter
    {
        public CalendarOutputFormatter()
        {

            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("text/calendar"));
            SupportedEncodings.Add(Encoding.UTF8);

        }

        public override Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            Event event1 = (Event)context.Object;
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("BEGIN:VCALENDAR");
            builder.AppendLine("VERSION:2.0");
            builder.AppendLine("PRODID:-my hche589");
            builder.AppendLine("BEGIN:VEVENT");
            builder.AppendLine($"UID:{event1.Id}");
            builder.AppendLine($"DTSTAMP:{DateTime.UtcNow.ToString("yyyyMMddTHHmmssZ")}");
            builder.AppendLine($"DTSTART:{event1.Start}");
            builder.AppendLine($"DTEND:{event1.End}");
            builder.AppendLine($"SUMMARY:{event1.Summary}");
            builder.AppendLine($"DESCRIPTION:{event1.Description}");
            builder.AppendLine($"LOCATION:{event1.Location}");
            builder.AppendLine("END:VEVENT");
            builder.AppendLine("END:VCALENDAR");

            string outString = builder.ToString();
            byte[] outBytes = selectedEncoding.GetBytes(outString);
            var response = context.HttpContext.Response.Body;
            return response.WriteAsync(outBytes, 0, outBytes.Length);

        }



    }
} 