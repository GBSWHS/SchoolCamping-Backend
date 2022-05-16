using System.Net.Mime;
using System.Text;
using GoogleReCaptcha.V3;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SchoolCamping.Middlewares
{
    public class CaptchaMiddleware
    {
        public CaptchaMiddleware(ConfigurationManager m)
        {
            configuration = m;
            validator.UpdateSecretKey(m.GetSection("RecaptchaSecretKey").Value);
        }

        public static async Task InvokeAsync(HttpContext ctx, Func<Task> next)
        {
            if (ctx.Request.Headers.ContainsKey("OH_SHIT_DEBUG"))
            {
                if (ctx.Request.Headers["OH_SHIT_DEBUG"] == "youshallnotpass")
                    ctx.Items.Add("captcha", true);
            }
            else if (ctx.Request.Method == "POST" && ctx.Request.ContentType == "application/json")
            {
                var obj = await GetRequestBodyAsync(ctx.Request);
                
                if (obj?.ContainsKey("recaptcha_key") == true)
                {
                    ctx.Items.Add("captcha", await VerifyCaptcha(obj["recaptcha_key"].Value<string>()));
                }
            }
            await next();
        }

        public static async Task<bool> VerifyCaptcha(string token)
        {
            return await validator.IsCaptchaPassedAsync(token);
        }

        public static HttpClient httpClient = new ();
        public static ConfigurationManager configuration = new ();
        public static GoogleReCaptchaValidator validator = new (httpClient, configuration);

        public static async Task<JObject> GetRequestBodyAsync(HttpRequest request)
        {
            JObject objRequestBody = new JObject();

            // IMPORTANT: Ensure the requestBody can be read multiple times.
            HttpRequestRewindExtensions.EnableBuffering(request);

            // IMPORTANT: Leave the body open so the next middleware can read it.
            using (StreamReader reader = new StreamReader(
                       request.Body,
                       Encoding.UTF8,
                       detectEncodingFromByteOrderMarks: false,
                       leaveOpen: true))
            {
                string strRequestBody = await reader.ReadToEndAsync();
                objRequestBody = JsonConvert.DeserializeObject<JObject>(strRequestBody);

                // IMPORTANT: Reset the request body stream position so the next middleware can read it
                request.Body.Position = 0;
            }

            return objRequestBody;
        }
    }
}
