namespace Finacle.Infrastructure.Auth.BasicJwt
{
    public class AuthHttpClientHandler : DelegatingHandler
    {
        private readonly IJwtService _tokenService;

        public AuthHttpClientHandler(IJwtService tokenService)
        {
            _tokenService = tokenService;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await _tokenService.GetTokenAsync();
            if (token != null)
            {
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
