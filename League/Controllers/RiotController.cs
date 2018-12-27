using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace League.Controllers
{
        public class RiotController : Controller
    {
        [HttpGet("summoner/{summonerName}")]
        public async Task<HttpServiceResponse<Summoner>> GetSummonerInfo(string summonerName)
        {
            if (!SummonerNameValidator(summonerName))
            {
                var sRepo = new SummonerRepo();

                var s = SummonerRepo.FetchSummoner(summonerName);
                return new HttpServiceResponse<Summoner>(HttpStatusCode.OK);
            }

            return null;
        }

        public bool SummonerNameValidator(string summonerName)
        {
            var pattern = Regex.Escape("^[0-9\\p{L} _\\.]+$");
            return Regex.IsMatch(summonerName, pattern);
        }
    }

        public class SummonerRepo
        {
            private static HttpClient _client = new HttpClient();
            public static async Task<HttpResponseMessage> FetchSummoner(string summonerName)
            {
                const string ApiKey = "RGAPI-7cda8b17-783a-4004-ad5a-b0f75ac25f35";
                var url = $"https://{"na1"}.api.riotgames.com/lol/summoner/{"v3"}/summoners/{"by-name"}/{"RiotSchmick"}?api_key={ApiKey}";
                HttpResponseMessage response = await _client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    return response;
                }
                return new HttpResponseMessage();
            }
        } 
        

    public class Summoner
    {
        public string Name { get; set; }
    }

    public class ApiResponse
    {
        
    }
    
    public class HttpServiceResponse<T> : GlobalApiResponse<T>, IActionResult
    {
        private readonly HttpStatusCode _httpStatusCode;

        public HttpServiceResponse(HttpStatusCode httpStatusCode, ErrorCode serviceErrorCode, string serviceErrorMessage)
            : this(default(T), httpStatusCode, serviceErrorCode, serviceErrorMessage) { }

        public HttpServiceResponse(HttpStatusCode statusCode)
            : this(default(T)) { }
        
        public HttpServiceResponse(T body = default(T), HttpStatusCode httpStatusCode = HttpStatusCode.OK, ErrorCode serviceErrorCode = 0, string serviceErrorMessage = "")
        {
            Body = body;
            ErrorCode = serviceErrorCode;
            ErrorMessage = serviceErrorMessage;
            this._httpStatusCode = httpStatusCode;
        }

        #region IActionResult implementation
        Task IActionResult.ExecuteResultAsync(ActionContext context)
        {
            var result = new ObjectResult(this) { StatusCode = (int)_httpStatusCode };
            return result.ExecuteResultAsync(context);
        }
        #endregion
    }

    public class GlobalApiResponse<T>
    {
        public T Body { get; set; }

        public ErrorCode ErrorCode { get; set; }

        public string ErrorMessage { get; set; }
    }

    public enum ErrorCode
    {
        /*******************************************************************
 *                                                                 *
 *   Error Code Format:                                            *
 *    0000000                                                      *
 *    │└┬┘└┬┘                                                      *
 *    │ │  └─ The actual code (digits 5-7).                        *
 *    │ └──── Service/component that owns the code (digits 2-4).   *
 *    └────── The type of code (digit 1).                          *
 *             ⮡  0 = Success                                      *
 *             ⮡  1 = Error                                        *
 *             ⮡  2 = Warning                                      *
 *                                                                 *
 ******************************************************************/

        // Global (000)
        Success = 0000000,
        UnexpectedError = 1000000,
    }
}