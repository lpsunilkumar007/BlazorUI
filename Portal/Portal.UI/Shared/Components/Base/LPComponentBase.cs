using Microsoft.AspNetCore.Components;

namespace Portal.UI.Shared.Components.Base
{
    public abstract class LPComponentBase : ComponentBase, IDisposable
    {
        #region Properties
        [Inject] protected ILogger<LPComponentBase> Logger { get; set; } = default!;
        #endregion

        #region Ctor
        protected LPComponentBase()
        {

        }
        #endregion

        #region Methods
        /// <summary>
        /// Dispose
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void Dispose()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
