
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using System.Security.Claims;



namespace Maui.Hyb.Pages.Authentication
{
    public partial class Login
    {
        #region Prop


        private bool _passwordVisibility;
        private InputType _passwordInput = InputType.Password;
        private string _passwordInputIcon = Icons.Material.Filled.VisibilityOff;

        #endregion
        protected override async Task OnInitializedAsync()
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private async Task SubmitAsync()
        {

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
            //_tokenModel.Email = "mukesh@blazorhero.com";
            // _tokenModel.Password = "123Pa$$word!";
        }

        private void FillBasicUserCredentials()
        {
            // _tokenModel.Email = "john@blazorhero.com";
            //_tokenModel.Password = "123Pa$$word!";
        }
    }


}
