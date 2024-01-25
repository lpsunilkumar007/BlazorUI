using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Portal.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.Services.Managers
{
    public class ApplicationStateManager : IPortal
    {
        public bool? IsMobile { get; set; }

        private readonly IList<bool> _isLoading = new List<bool>();

        public bool IsLoading
        {
            get => _isLoading.Count > 0;
            set
            {
                var currentIsLoading = _isLoading.Count > 0;

                if (value)
                    _isLoading.Add(true);
                else if (_isLoading.Count > 0)
                    _isLoading.RemoveAt(0);

                var newIsLoading = _isLoading.Count > 0;

                if (newIsLoading != currentIsLoading) // We have a change in value
                {
                    IsLoadingChange.InvokeAsync(_isLoading.Count > 0);
                }
            }
        }

        public EventCallback<bool> IsLoadingChange { get; set; }

    }
}
