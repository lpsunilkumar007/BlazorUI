using Blazored.FluentValidation;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using System.Security.Claims;
using Portal.Shared.Models.Api.Request.Identity;


namespace Portal.UI.Pages.Authentication
{
    public partial class Login
    {
        #region Prop
        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
        private TokenRequest _tokenModel = new();

        private bool _passwordVisibility;
        private InputType _passwordInput = InputType.Password;
        private string _passwordInputIcon = Icons.Material.Filled.VisibilityOff;

        #endregion
        protected override async Task OnInitializedAsync()
        {
            var state = await _stateProvider.GetAuthenticationStateAsync();
            if (state != new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())))
            {
                _navigationManager.NavigateTo("/");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private async Task SubmitAsync()
        {
            var result = await _authenticationService.Login(_tokenModel);
            if (!result.Succeeded)
            {
                foreach (var message in result.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void TogglePasswordVisibility()
        {
            if (_passwordVisibility)
            {
                _passwordVisibility = false;
                _passwordInputIcon = Icons.Material.Filled.VisibilityOff;
                _passwordInput = InputType.Password;
            }
            else
            {
                _passwordVisibility = true;
                _passwordInputIcon = Icons.Material.Filled.Visibility;
                _passwordInput = InputType.Text;
            }
        }

        private void FillAdministratorCredentials()
        {
            _tokenModel.Email = "mukesh@blazorhero.com";
            _tokenModel.Password = "123Pa$$word!";
        }

        private void FillBasicUserCredentials()
        {
            _tokenModel.Email = "john@blazorhero.com";
            _tokenModel.Password = "123Pa$$word!";
        }
    }


}
